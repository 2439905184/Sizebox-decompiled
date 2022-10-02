using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
	public static class InputActionRebindingExtensions
	{
		public sealed class RebindingOperation : IDisposable
		{
			[Flags]
			private enum Flags
			{
				Started = 1,
				Completed = 2,
				Canceled = 4,
				OnEventHooked = 8,
				OnAfterUpdateHooked = 0x10,
				DontIgnoreNoisyControls = 0x40,
				DontGeneralizePathOfSelectedControl = 0x80,
				AddNewBinding = 0x100,
				SuppressMatchingEvents = 0x200
			}

			public const float kDefaultMagnitudeThreshold = 0.2f;

			private InputAction m_ActionToRebind;

			private InputBinding? m_BindingMask;

			private Type m_ControlType;

			private InternedString m_ExpectedLayout;

			private int m_IncludePathCount;

			private string[] m_IncludePaths;

			private int m_ExcludePathCount;

			private string[] m_ExcludePaths;

			private int m_TargetBindingIndex = -1;

			private string m_BindingGroupForNewBinding;

			private string m_CancelBinding;

			private float m_MagnitudeThreshold = 0.2f;

			private float[] m_Scores;

			private float[] m_Magnitudes;

			private double m_LastMatchTime;

			private double m_StartTime;

			private float m_Timeout;

			private float m_WaitSecondsAfterMatch;

			private InputControlList<InputControl> m_Candidates;

			private Action<RebindingOperation> m_OnComplete;

			private Action<RebindingOperation> m_OnCancel;

			private Action<RebindingOperation> m_OnPotentialMatch;

			private Func<InputControl, string> m_OnGeneratePath;

			private Func<InputControl, InputEventPtr, float> m_OnComputeScore;

			private Action<RebindingOperation, string> m_OnApplyBinding;

			private Action<InputEventPtr, InputDevice> m_OnEventDelegate;

			private Action m_OnAfterUpdateDelegate;

			private InputControlLayout.Cache m_LayoutCache;

			private StringBuilder m_PathBuilder;

			private Flags m_Flags;

			private int m_StartingActuationsCount;

			private float[] m_StaringActuationMagnitudes;

			private InputControl[] m_StartingActuationControls;

			public InputAction action
			{
				get
				{
					return m_ActionToRebind;
				}
			}

			public InputBinding? bindingMask
			{
				get
				{
					return m_BindingMask;
				}
			}

			public InputControlList<InputControl> candidates
			{
				get
				{
					return m_Candidates;
				}
			}

			public ReadOnlyArray<float> scores
			{
				get
				{
					return new ReadOnlyArray<float>(m_Scores, 0, m_Candidates.Count);
				}
			}

			public ReadOnlyArray<float> magnitudes
			{
				get
				{
					return new ReadOnlyArray<float>(m_Magnitudes, 0, m_Candidates.Count);
				}
			}

			public InputControl selectedControl
			{
				get
				{
					if (m_Candidates.Count == 0)
					{
						return null;
					}
					return m_Candidates[0];
				}
			}

			public bool started
			{
				get
				{
					return (m_Flags & Flags.Started) != 0;
				}
			}

			public bool completed
			{
				get
				{
					return (m_Flags & Flags.Completed) != 0;
				}
			}

			public bool canceled
			{
				get
				{
					return (m_Flags & Flags.Canceled) != 0;
				}
			}

			public double startTime
			{
				get
				{
					return m_StartTime;
				}
			}

			public float timeout
			{
				get
				{
					return m_Timeout;
				}
			}

			public string expectedControlType
			{
				get
				{
					return m_ExpectedLayout;
				}
			}

			public RebindingOperation WithAction(InputAction action)
			{
				ThrowIfRebindInProgress();
				if (action == null)
				{
					throw new ArgumentNullException("action");
				}
				if (action.enabled)
				{
					throw new InvalidOperationException(string.Format("Cannot rebind action '{0}' while it is enabled", action));
				}
				m_ActionToRebind = action;
				if (!string.IsNullOrEmpty(action.expectedControlType))
				{
					WithExpectedControlType(action.expectedControlType);
				}
				else if (action.type == InputActionType.Button)
				{
					WithExpectedControlType("Button");
				}
				return this;
			}

			public RebindingOperation WithMatchingEventsBeingSuppressed(bool value = true)
			{
				ThrowIfRebindInProgress();
				if (value)
				{
					m_Flags |= Flags.SuppressMatchingEvents;
				}
				else
				{
					m_Flags &= ~Flags.SuppressMatchingEvents;
				}
				return this;
			}

			public RebindingOperation WithCancelingThrough(string binding)
			{
				ThrowIfRebindInProgress();
				m_CancelBinding = binding;
				return this;
			}

			public RebindingOperation WithCancelingThrough(InputControl control)
			{
				ThrowIfRebindInProgress();
				if (control == null)
				{
					throw new ArgumentNullException("control");
				}
				return WithCancelingThrough(control.path);
			}

			public RebindingOperation WithExpectedControlType(string layoutName)
			{
				ThrowIfRebindInProgress();
				m_ExpectedLayout = new InternedString(layoutName);
				return this;
			}

			public RebindingOperation WithExpectedControlType(Type type)
			{
				ThrowIfRebindInProgress();
				if (type != null && !typeof(InputControl).IsAssignableFrom(type))
				{
					throw new ArgumentException("Type '" + type.Name + "' is not an InputControl", "type");
				}
				m_ControlType = type;
				return this;
			}

			public RebindingOperation WithExpectedControlType<TControl>() where TControl : InputControl
			{
				ThrowIfRebindInProgress();
				return WithExpectedControlType(typeof(TControl));
			}

			public RebindingOperation WithTargetBinding(int bindingIndex)
			{
				m_TargetBindingIndex = bindingIndex;
				return this;
			}

			public RebindingOperation WithBindingMask(InputBinding? bindingMask)
			{
				m_BindingMask = bindingMask;
				return this;
			}

			public RebindingOperation WithBindingGroup(string group)
			{
				return WithBindingMask(new InputBinding
				{
					groups = group
				});
			}

			public RebindingOperation WithoutGeneralizingPathOfSelectedControl()
			{
				m_Flags |= Flags.DontGeneralizePathOfSelectedControl;
				return this;
			}

			public RebindingOperation WithRebindAddingNewBinding(string group = null)
			{
				m_Flags |= Flags.AddNewBinding;
				m_BindingGroupForNewBinding = group;
				return this;
			}

			public RebindingOperation WithMagnitudeHavingToBeGreaterThan(float magnitude)
			{
				ThrowIfRebindInProgress();
				if (magnitude < 0f)
				{
					throw new ArgumentException(string.Format("Magnitude has to be positive but was {0}", magnitude), "magnitude");
				}
				m_MagnitudeThreshold = magnitude;
				return this;
			}

			public RebindingOperation WithoutIgnoringNoisyControls()
			{
				ThrowIfRebindInProgress();
				m_Flags |= Flags.DontIgnoreNoisyControls;
				return this;
			}

			public RebindingOperation WithControlsHavingToMatchPath(string path)
			{
				ThrowIfRebindInProgress();
				if (string.IsNullOrEmpty(path))
				{
					throw new ArgumentNullException("path");
				}
				for (int i = 0; i < m_IncludePathCount; i++)
				{
					if (string.Compare(m_IncludePaths[i], path, StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						return this;
					}
				}
				ArrayHelpers.AppendWithCapacity(ref m_IncludePaths, ref m_IncludePathCount, path);
				return this;
			}

			public RebindingOperation WithControlsExcluding(string path)
			{
				ThrowIfRebindInProgress();
				if (string.IsNullOrEmpty(path))
				{
					throw new ArgumentNullException("path");
				}
				for (int i = 0; i < m_ExcludePathCount; i++)
				{
					if (string.Compare(m_ExcludePaths[i], path, StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						return this;
					}
				}
				ArrayHelpers.AppendWithCapacity(ref m_ExcludePaths, ref m_ExcludePathCount, path);
				return this;
			}

			public RebindingOperation WithTimeout(float timeInSeconds)
			{
				m_Timeout = timeInSeconds;
				return this;
			}

			public RebindingOperation OnComplete(Action<RebindingOperation> callback)
			{
				m_OnComplete = callback;
				return this;
			}

			public RebindingOperation OnCancel(Action<RebindingOperation> callback)
			{
				m_OnCancel = callback;
				return this;
			}

			public RebindingOperation OnPotentialMatch(Action<RebindingOperation> callback)
			{
				m_OnPotentialMatch = callback;
				return this;
			}

			public RebindingOperation OnGeneratePath(Func<InputControl, string> callback)
			{
				m_OnGeneratePath = callback;
				return this;
			}

			public RebindingOperation OnComputeScore(Func<InputControl, InputEventPtr, float> callback)
			{
				m_OnComputeScore = callback;
				return this;
			}

			public RebindingOperation OnApplyBinding(Action<RebindingOperation, string> callback)
			{
				m_OnApplyBinding = callback;
				return this;
			}

			public RebindingOperation OnMatchWaitForAnother(float seconds)
			{
				m_WaitSecondsAfterMatch = seconds;
				return this;
			}

			public RebindingOperation Start()
			{
				if (started)
				{
					return this;
				}
				if (m_ActionToRebind != null && m_ActionToRebind.bindings.Count == 0 && (m_Flags & Flags.AddNewBinding) == 0)
				{
					throw new InvalidOperationException(string.Format("Action '{0}' must have at least one existing binding or must be used with WithRebindingAddNewBinding()", action));
				}
				if (m_ActionToRebind == null && m_OnApplyBinding == null)
				{
					throw new InvalidOperationException("Must either have an action (call WithAction()) to apply binding to or have a custom callback to apply the binding (call OnApplyBinding())");
				}
				m_StartTime = InputRuntime.s_Instance.currentTime;
				if (m_WaitSecondsAfterMatch > 0f || m_Timeout > 0f)
				{
					HookOnAfterUpdate();
					m_LastMatchTime = -1.0;
				}
				HookOnEvent();
				m_Flags |= Flags.Started;
				m_Flags &= ~Flags.Canceled;
				m_Flags &= ~Flags.Completed;
				return this;
			}

			public void Cancel()
			{
				if (started)
				{
					OnCancel();
				}
			}

			public void Complete()
			{
				if (started)
				{
					OnComplete();
				}
			}

			public void AddCandidate(InputControl control, float score, float magnitude = -1f)
			{
				if (control == null)
				{
					throw new ArgumentNullException("control");
				}
				int num = m_Candidates.IndexOf(control);
				if (num != -1)
				{
					m_Scores[num] = score;
				}
				else
				{
					int count = m_Candidates.Count;
					int count2 = m_Candidates.Count;
					m_Candidates.Add(control);
					ArrayHelpers.AppendWithCapacity(ref m_Scores, ref count, score);
					ArrayHelpers.AppendWithCapacity(ref m_Magnitudes, ref count2, magnitude);
				}
				SortCandidatesByScore();
			}

			public void RemoveCandidate(InputControl control)
			{
				if (control == null)
				{
					throw new ArgumentNullException("control");
				}
				int num = m_Candidates.IndexOf(control);
				if (num != -1)
				{
					int count = m_Candidates.Count;
					m_Candidates.RemoveAt(num);
					ArrayHelpers.EraseAtWithCapacity(m_Scores, ref count, num);
				}
			}

			public void Dispose()
			{
				UnhookOnEvent();
				UnhookOnAfterUpdate();
				m_Candidates.Dispose();
				m_LayoutCache.Clear();
			}

			~RebindingOperation()
			{
				Dispose();
			}

			public RebindingOperation Reset()
			{
				Cancel();
				m_ActionToRebind = null;
				m_BindingMask = null;
				m_ControlType = null;
				m_ExpectedLayout = default(InternedString);
				m_IncludePathCount = 0;
				m_ExcludePathCount = 0;
				m_TargetBindingIndex = -1;
				m_BindingGroupForNewBinding = null;
				m_CancelBinding = null;
				m_MagnitudeThreshold = 0.2f;
				m_Timeout = 0f;
				m_WaitSecondsAfterMatch = 0f;
				m_Flags = (Flags)0;
				m_StartingActuationControls.Clear();
				m_StartingActuationsCount = 0;
				return this;
			}

			private void HookOnEvent()
			{
				if ((m_Flags & Flags.OnEventHooked) == 0)
				{
					if (m_OnEventDelegate == null)
					{
						m_OnEventDelegate = OnEvent;
					}
					InputSystem.onEvent += m_OnEventDelegate;
					m_Flags |= Flags.OnEventHooked;
				}
			}

			private void UnhookOnEvent()
			{
				if ((m_Flags & Flags.OnEventHooked) != 0)
				{
					InputSystem.onEvent -= m_OnEventDelegate;
					m_Flags &= ~Flags.OnEventHooked;
				}
			}

			private unsafe void OnEvent(InputEventPtr eventPtr, InputDevice device)
			{
				if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>())
				{
					return;
				}
				ReadOnlyArray<InputControl> allControls = device.allControls;
				int count = allControls.Count;
				bool flag = false;
				bool flag2 = false;
				for (int i = 0; i < count; i++)
				{
					InputControl inputControl = allControls[i];
					void* statePtrFromStateEvent = inputControl.GetStatePtrFromStateEvent(eventPtr);
					if (statePtrFromStateEvent == null)
					{
						continue;
					}
					if (!string.IsNullOrEmpty(m_CancelBinding) && InputControlPath.Matches(m_CancelBinding, inputControl) && !inputControl.CheckStateIsAtDefault(statePtrFromStateEvent, null) && inputControl.HasValueChangeInState(statePtrFromStateEvent))
					{
						OnCancel();
						break;
					}
					if ((inputControl.noisy && (m_Flags & Flags.DontIgnoreNoisyControls) == 0) || (m_ExcludePathCount > 0 && HavePathMatch(inputControl, m_ExcludePaths, m_ExcludePathCount)))
					{
						continue;
					}
					flag2 = true;
					if ((m_IncludePathCount > 0 && !HavePathMatch(inputControl, m_IncludePaths, m_IncludePathCount)) || (m_ControlType != null && !m_ControlType.IsInstanceOfType(inputControl)) || (!m_ExpectedLayout.IsEmpty() && m_ExpectedLayout != inputControl.m_Layout && !InputControlLayout.s_Layouts.IsBasedOn(m_ExpectedLayout, inputControl.m_Layout)))
					{
						continue;
					}
					if (inputControl.CheckStateIsAtDefault(statePtrFromStateEvent, null))
					{
						int num = m_StartingActuationControls.IndexOfReference(inputControl);
						if (num != -1)
						{
							m_StaringActuationMagnitudes[num] = 0f;
						}
						continue;
					}
					float num2 = inputControl.EvaluateMagnitude(statePtrFromStateEvent);
					if (num2 >= 0f)
					{
						int num3 = m_StartingActuationControls.IndexOfReference(inputControl);
						float num4;
						if (num3 != -1)
						{
							num4 = m_StaringActuationMagnitudes[num3];
						}
						else
						{
							num4 = inputControl.EvaluateMagnitude();
							int count2 = m_StartingActuationsCount;
							ArrayHelpers.AppendWithCapacity(ref m_StartingActuationControls, ref m_StartingActuationsCount, inputControl);
							ArrayHelpers.AppendWithCapacity(ref m_StaringActuationMagnitudes, ref count2, num4);
						}
						if (Mathf.Abs(num4 - num2) < m_MagnitudeThreshold)
						{
							continue;
						}
					}
					float num5;
					if (m_OnComputeScore != null)
					{
						num5 = m_OnComputeScore(inputControl, eventPtr);
					}
					else
					{
						num5 = num2;
						if (!inputControl.synthetic)
						{
							num5 += 1f;
						}
					}
					int num6 = m_Candidates.IndexOf(inputControl);
					if (num6 != -1)
					{
						if (m_Scores[num6] < num5)
						{
							flag = true;
							m_Scores[num6] = num5;
							if (m_WaitSecondsAfterMatch > 0f)
							{
								m_LastMatchTime = InputRuntime.s_Instance.currentTime;
							}
						}
						continue;
					}
					int count3 = m_Candidates.Count;
					int count4 = m_Candidates.Count;
					m_Candidates.Add(inputControl);
					ArrayHelpers.AppendWithCapacity(ref m_Scores, ref count3, num5);
					ArrayHelpers.AppendWithCapacity(ref m_Magnitudes, ref count4, num2);
					flag = true;
					if (m_WaitSecondsAfterMatch > 0f)
					{
						m_LastMatchTime = InputRuntime.s_Instance.currentTime;
					}
				}
				if (flag2 && (m_Flags & Flags.SuppressMatchingEvents) != 0)
				{
					eventPtr.handled = true;
				}
				if (flag && !canceled)
				{
					if (m_OnPotentialMatch != null)
					{
						SortCandidatesByScore();
						m_OnPotentialMatch(this);
					}
					else if (m_WaitSecondsAfterMatch <= 0f)
					{
						OnComplete();
					}
					else
					{
						SortCandidatesByScore();
					}
				}
			}

			private void SortCandidatesByScore()
			{
				int count = m_Candidates.Count;
				if (count <= 1)
				{
					return;
				}
				for (int i = 1; i < count; i++)
				{
					int num = i;
					while (num > 0 && m_Scores[num - 1] < m_Scores[num])
					{
						m_Scores.SwapElements(num, num - 1);
						m_Candidates.SwapElements(num, num - 1);
						m_Magnitudes.SwapElements(i, num - 1);
						num--;
					}
				}
			}

			private static bool HavePathMatch(InputControl control, string[] paths, int pathCount)
			{
				for (int i = 0; i < pathCount; i++)
				{
					if (InputControlPath.MatchesPrefix(paths[i], control))
					{
						return true;
					}
				}
				return false;
			}

			private void HookOnAfterUpdate()
			{
				if ((m_Flags & Flags.OnAfterUpdateHooked) == 0)
				{
					if (m_OnAfterUpdateDelegate == null)
					{
						m_OnAfterUpdateDelegate = OnAfterUpdate;
					}
					InputSystem.onAfterUpdate += m_OnAfterUpdateDelegate;
					m_Flags |= Flags.OnAfterUpdateHooked;
				}
			}

			private void UnhookOnAfterUpdate()
			{
				if ((m_Flags & Flags.OnAfterUpdateHooked) != 0)
				{
					InputSystem.onAfterUpdate -= m_OnAfterUpdateDelegate;
					m_Flags &= ~Flags.OnAfterUpdateHooked;
				}
			}

			private void OnAfterUpdate()
			{
				if (m_LastMatchTime < 0.0 && m_Timeout > 0f && InputRuntime.s_Instance.currentTime - m_StartTime > (double)m_Timeout)
				{
					Cancel();
				}
				else if (!(m_WaitSecondsAfterMatch <= 0f) && !(m_LastMatchTime < 0.0) && InputRuntime.s_Instance.currentTime >= m_LastMatchTime + (double)m_WaitSecondsAfterMatch)
				{
					Complete();
				}
			}

			private void OnComplete()
			{
				SortCandidatesByScore();
				if (m_Candidates.Count > 0)
				{
					InputControl inputControl = m_Candidates[0];
					string text = inputControl.path;
					if (m_OnGeneratePath != null)
					{
						string text2 = m_OnGeneratePath(inputControl);
						if (!string.IsNullOrEmpty(text2))
						{
							text = text2;
						}
						else if ((m_Flags & Flags.DontGeneralizePathOfSelectedControl) == 0)
						{
							text = GeneratePathForControl(inputControl);
						}
					}
					else if ((m_Flags & Flags.DontGeneralizePathOfSelectedControl) == 0)
					{
						text = GeneratePathForControl(inputControl);
					}
					if (m_OnApplyBinding != null)
					{
						m_OnApplyBinding(this, text);
					}
					else if ((m_Flags & Flags.AddNewBinding) != 0)
					{
						m_ActionToRebind.AddBinding(text, null, null, m_BindingGroupForNewBinding);
					}
					else if (m_TargetBindingIndex >= 0)
					{
						if (m_TargetBindingIndex >= m_ActionToRebind.bindings.Count)
						{
							throw new InvalidOperationException(string.Format("Target binding index {0} out of range for action '{1}' with {2} bindings", m_TargetBindingIndex, m_ActionToRebind, m_ActionToRebind.bindings.Count));
						}
						m_ActionToRebind.ApplyBindingOverride(m_TargetBindingIndex, text);
					}
					else if (m_BindingMask.HasValue)
					{
						InputBinding value = m_BindingMask.Value;
						value.overridePath = text;
						m_ActionToRebind.ApplyBindingOverride(value);
					}
					else
					{
						m_ActionToRebind.ApplyBindingOverride(text);
					}
				}
				m_Flags |= Flags.Completed;
				Action<RebindingOperation> onComplete = m_OnComplete;
				if (onComplete != null)
				{
					onComplete(this);
				}
				ResetAfterMatchCompleted();
			}

			private void OnCancel()
			{
				m_Flags |= Flags.Canceled;
				Action<RebindingOperation> onCancel = m_OnCancel;
				if (onCancel != null)
				{
					onCancel(this);
				}
				ResetAfterMatchCompleted();
			}

			private void ResetAfterMatchCompleted()
			{
				m_Flags &= ~Flags.Started;
				m_Candidates.Clear();
				m_Candidates.Capacity = 0;
				m_StartTime = -1.0;
				m_StartingActuationControls.Clear();
				m_StartingActuationsCount = 0;
				UnhookOnEvent();
				UnhookOnAfterUpdate();
			}

			private void ThrowIfRebindInProgress()
			{
				if (started)
				{
					throw new InvalidOperationException("Cannot reconfigure rebinding while operation is in progress");
				}
			}

			private string GeneratePathForControl(InputControl control)
			{
				InputDevice device = control.device;
				InternedString internedString = InputControlLayout.s_Layouts.FindLayoutThatIntroducesControl(control, m_LayoutCache);
				if (m_PathBuilder == null)
				{
					m_PathBuilder = new StringBuilder();
				}
				else
				{
					m_PathBuilder.Length = 0;
				}
				control.BuildPath(internedString, m_PathBuilder);
				return m_PathBuilder.ToString();
			}
		}

		private class DeferBindingResolutionWrapper : IDisposable
		{
			public void Acquire()
			{
				InputActionMap.s_DeferBindingResolution++;
			}

			public void Dispose()
			{
				if (InputActionMap.s_DeferBindingResolution > 0)
				{
					InputActionMap.s_DeferBindingResolution--;
				}
				InputActionState.DeferredResolutionOfBindings();
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass7_0
		{
			public ReadOnlyArray<InputBinding> bindings;
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass7_1
		{
			public int firstPartIndex;

			public string[] partStrings;

			public int partCount;

			public _003C_003Ec__DisplayClass7_0 CS_0024_003C_003E8__locals1;

			internal string _003CGetBindingDisplayString_003Eb__0(string fragment)
			{
				string text = string.Empty;
				for (int i = 0; i < partCount; i++)
				{
					if (string.Equals(CS_0024_003C_003E8__locals1.bindings[firstPartIndex + i].name, fragment, StringComparison.InvariantCultureIgnoreCase))
					{
						text = (string.IsNullOrEmpty(text) ? partStrings[i] : (text + "|" + partStrings[i]));
					}
				}
				return text;
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass24_0
		{
			public string group;

			internal bool _003CPerformInteractiveRebinding_003Eb__0(InputControlScheme x)
			{
				return group.Equals(x.bindingGroup, StringComparison.InvariantCultureIgnoreCase);
			}
		}

		private static DeferBindingResolutionWrapper s_DeferBindingResolutionWrapper;

		public static int GetBindingIndex(this InputAction action, InputBinding bindingMask)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			ReadOnlyArray<InputBinding> bindings = action.bindings;
			for (int i = 0; i < bindings.Count; i++)
			{
				if (bindingMask.Matches(bindings[i]))
				{
					return i;
				}
			}
			return -1;
		}

		public static int GetBindingIndex(this InputAction action, string group = null, string path = null)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			return action.GetBindingIndex(new InputBinding(path, null, group));
		}

		public static InputBinding? GetBindingForControl(this InputAction action, InputControl control)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			int bindingIndexForControl = action.GetBindingIndexForControl(control);
			if (bindingIndexForControl == -1)
			{
				return null;
			}
			return action.bindings[bindingIndexForControl];
		}

		public unsafe static int GetBindingIndexForControl(this InputAction action, InputControl control)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			InputActionMap orCreateActionMap = action.GetOrCreateActionMap();
			orCreateActionMap.ResolveBindingsIfNecessary();
			InputActionState state = orCreateActionMap.m_State;
			int num = Array.IndexOf(state.controls, control);
			if (num == -1)
			{
				return -1;
			}
			int actionIndexInState = action.m_ActionIndexInState;
			int totalBindingCount = state.totalBindingCount;
			for (int i = 0; i < totalBindingCount; i++)
			{
				InputActionState.BindingState* ptr = state.bindingStates + i;
				if (ptr->actionIndex == actionIndexInState && ptr->controlStartIndex <= num && num < ptr->controlStartIndex + ptr->controlCount)
				{
					int bindingIndexInMap = state.GetBindingIndexInMap(i);
					return action.BindingIndexOnMapToBindingIndexOnAction(bindingIndexInMap);
				}
			}
			return -1;
		}

		public static string GetBindingDisplayString(this InputAction action, InputBinding.DisplayStringOptions options = (InputBinding.DisplayStringOptions)0, string group = null)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			InputBinding bindingMask;
			if (!string.IsNullOrEmpty(group))
			{
				bindingMask = InputBinding.MaskByGroup(group);
			}
			else
			{
				InputBinding? inputBinding = action.FindEffectiveBindingMask();
				bindingMask = ((!inputBinding.HasValue) ? default(InputBinding) : inputBinding.Value);
			}
			return action.GetBindingDisplayString(bindingMask, options);
		}

		public static string GetBindingDisplayString(this InputAction action, InputBinding bindingMask, InputBinding.DisplayStringOptions options = (InputBinding.DisplayStringOptions)0)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			string text = string.Empty;
			ReadOnlyArray<InputBinding> bindings = action.bindings;
			for (int i = 0; i < bindings.Count; i++)
			{
				if (bindingMask.Matches(bindings[i]))
				{
					string bindingDisplayString = action.GetBindingDisplayString(i, options);
					text = ((!(text != "")) ? bindingDisplayString : (text + " | " + bindingDisplayString));
				}
			}
			return text;
		}

		public static string GetBindingDisplayString(this InputAction action, int bindingIndex, InputBinding.DisplayStringOptions options = (InputBinding.DisplayStringOptions)0)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			string deviceLayoutName;
			string controlPath;
			return action.GetBindingDisplayString(bindingIndex, out deviceLayoutName, out controlPath, options);
		}

		public unsafe static string GetBindingDisplayString(this InputAction action, int bindingIndex, out string deviceLayoutName, out string controlPath, InputBinding.DisplayStringOptions options = (InputBinding.DisplayStringOptions)0)
		{
			_003C_003Ec__DisplayClass7_0 _003C_003Ec__DisplayClass7_ = new _003C_003Ec__DisplayClass7_0();
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			deviceLayoutName = null;
			controlPath = null;
			_003C_003Ec__DisplayClass7_.bindings = action.bindings;
			int count = _003C_003Ec__DisplayClass7_.bindings.Count;
			if (bindingIndex < 0 || bindingIndex >= count)
			{
				throw new ArgumentOutOfRangeException(string.Format("Binding index {0} is out of range on action '{1}' with {2} bindings", bindingIndex, action, _003C_003Ec__DisplayClass7_.bindings.Count), "bindingIndex");
			}
			if (_003C_003Ec__DisplayClass7_.bindings[bindingIndex].isComposite)
			{
				_003C_003Ec__DisplayClass7_1 _003C_003Ec__DisplayClass7_2 = new _003C_003Ec__DisplayClass7_1();
				_003C_003Ec__DisplayClass7_2.CS_0024_003C_003E8__locals1 = _003C_003Ec__DisplayClass7_;
				string name = NameAndParameters.Parse(_003C_003Ec__DisplayClass7_2.CS_0024_003C_003E8__locals1.bindings[bindingIndex].effectivePath).name;
				_003C_003Ec__DisplayClass7_2.firstPartIndex = bindingIndex + 1;
				int i;
				for (i = _003C_003Ec__DisplayClass7_2.firstPartIndex; i < count && _003C_003Ec__DisplayClass7_2.CS_0024_003C_003E8__locals1.bindings[i].isPartOfComposite; i++)
				{
				}
				_003C_003Ec__DisplayClass7_2.partCount = i - _003C_003Ec__DisplayClass7_2.firstPartIndex;
				_003C_003Ec__DisplayClass7_2.partStrings = new string[_003C_003Ec__DisplayClass7_2.partCount];
				for (int j = 0; j < _003C_003Ec__DisplayClass7_2.partCount; j++)
				{
					_003C_003Ec__DisplayClass7_2.partStrings[j] = action.GetBindingDisplayString(_003C_003Ec__DisplayClass7_2.firstPartIndex + j, options);
				}
				string displayFormatString = InputBindingComposite.GetDisplayFormatString(name);
				if (string.IsNullOrEmpty(displayFormatString))
				{
					return StringHelpers.Join("/", _003C_003Ec__DisplayClass7_2.partStrings);
				}
				return StringHelpers.ExpandTemplateString(displayFormatString, _003C_003Ec__DisplayClass7_2._003CGetBindingDisplayString_003Eb__0);
			}
			InputControl control = null;
			InputActionMap orCreateActionMap = action.GetOrCreateActionMap();
			orCreateActionMap.ResolveBindingsIfNecessary();
			InputActionState state = orCreateActionMap.m_State;
			int bindingIndexInMap = action.BindingIndexOnActionToBindingIndexOnMap(bindingIndex);
			int bindingIndexInState = state.GetBindingIndexInState(orCreateActionMap.m_MapIndexInState, bindingIndexInMap);
			InputActionState.BindingState* ptr = state.bindingStates + bindingIndexInState;
			if (ptr->controlCount > 0)
			{
				control = state.controls[ptr->controlStartIndex];
			}
			InputBinding inputBinding = _003C_003Ec__DisplayClass7_.bindings[bindingIndex];
			if (string.IsNullOrEmpty(inputBinding.effectiveInteractions))
			{
				inputBinding.overrideInteractions = action.interactions;
			}
			else if (!string.IsNullOrEmpty(action.interactions))
			{
				inputBinding.overrideInteractions = inputBinding.effectiveInteractions + ";action.interactions";
			}
			return inputBinding.ToDisplayString(out deviceLayoutName, out controlPath, options, control);
		}

		public static void ApplyBindingOverride(this InputAction action, string newPath, string group = null, string path = null)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			action.ApplyBindingOverride(new InputBinding
			{
				overridePath = newPath,
				groups = group,
				path = path
			});
		}

		public static void ApplyBindingOverride(this InputAction action, InputBinding bindingOverride)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			bindingOverride.action = action.name;
			action.GetOrCreateActionMap().ApplyBindingOverride(bindingOverride);
		}

		public static void ApplyBindingOverride(this InputAction action, int bindingIndex, InputBinding bindingOverride)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			int bindingIndex2 = action.BindingIndexOnActionToBindingIndexOnMap(bindingIndex);
			bindingOverride.action = action.name;
			action.GetOrCreateActionMap().ApplyBindingOverride(bindingIndex2, bindingOverride);
		}

		public static void ApplyBindingOverride(this InputAction action, int bindingIndex, string path)
		{
			if (path == null)
			{
				throw new ArgumentException("Binding path cannot be null", "path");
			}
			action.ApplyBindingOverride(bindingIndex, new InputBinding
			{
				overridePath = path
			});
		}

		public static int ApplyBindingOverride(this InputActionMap actionMap, InputBinding bindingOverride)
		{
			if (actionMap == null)
			{
				throw new ArgumentNullException("actionMap");
			}
			InputBinding[] bindings = actionMap.m_Bindings;
			if (bindings == null)
			{
				return 0;
			}
			int num = bindings.Length;
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				if (bindingOverride.Matches(ref bindings[i]))
				{
					bindings[i].overridePath = bindingOverride.overridePath;
					bindings[i].overrideInteractions = bindingOverride.overrideInteractions;
					bindings[i].overrideProcessors = bindingOverride.overrideProcessors;
					num2++;
				}
			}
			if (num2 > 0)
			{
				actionMap.ClearPerActionCachedBindingData();
				actionMap.LazyResolveBindings();
			}
			return num2;
		}

		public static void ApplyBindingOverride(this InputActionMap actionMap, int bindingIndex, InputBinding bindingOverride)
		{
			if (actionMap == null)
			{
				throw new ArgumentNullException("actionMap");
			}
			InputBinding[] bindings = actionMap.m_Bindings;
			int num = ((bindings != null) ? bindings.Length : 0);
			if (bindingIndex < 0 || bindingIndex >= num)
			{
				throw new ArgumentOutOfRangeException("bindingIndex", string.Format("Cannot apply override to binding at index {0} in map '{1}' with only {2} bindings", bindingIndex, actionMap, num));
			}
			actionMap.m_Bindings[bindingIndex].overridePath = bindingOverride.overridePath;
			actionMap.m_Bindings[bindingIndex].overrideInteractions = bindingOverride.overrideInteractions;
			actionMap.m_Bindings[bindingIndex].overrideProcessors = bindingOverride.overrideProcessors;
			actionMap.ClearPerActionCachedBindingData();
			actionMap.LazyResolveBindings();
		}

		public static void RemoveBindingOverride(this InputAction action, int bindingIndex)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			action.ApplyBindingOverride(bindingIndex, default(InputBinding));
		}

		public static void RemoveBindingOverride(this InputAction action, InputBinding bindingMask)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			bindingMask.overridePath = null;
			bindingMask.overrideInteractions = null;
			bindingMask.overrideProcessors = null;
			action.ApplyBindingOverride(bindingMask);
		}

		private static void RemoveBindingOverride(this InputActionMap actionMap, InputBinding bindingMask)
		{
			if (actionMap == null)
			{
				throw new ArgumentNullException("actionMap");
			}
			bindingMask.overridePath = null;
			bindingMask.overrideInteractions = null;
			bindingMask.overrideProcessors = null;
			actionMap.ApplyBindingOverride(bindingMask);
		}

		public static void RemoveAllBindingOverrides(this InputAction action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			string name = action.name;
			InputActionMap orCreateActionMap = action.GetOrCreateActionMap();
			InputBinding[] bindings = orCreateActionMap.m_Bindings;
			if (bindings == null)
			{
				return;
			}
			int num = bindings.Length;
			for (int i = 0; i < num; i++)
			{
				if (string.Compare(bindings[i].action, name, StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					bindings[i].overridePath = null;
					bindings[i].overrideInteractions = null;
					bindings[i].overrideProcessors = null;
				}
			}
			orCreateActionMap.ClearPerActionCachedBindingData();
			orCreateActionMap.LazyResolveBindings();
		}

		public static void ApplyBindingOverrides(this InputActionMap actionMap, IEnumerable<InputBinding> overrides)
		{
			if (actionMap == null)
			{
				throw new ArgumentNullException("actionMap");
			}
			if (overrides == null)
			{
				throw new ArgumentNullException("overrides");
			}
			foreach (InputBinding @override in overrides)
			{
				actionMap.ApplyBindingOverride(@override);
			}
		}

		public static void RemoveBindingOverrides(this InputActionMap actionMap, IEnumerable<InputBinding> overrides)
		{
			if (actionMap == null)
			{
				throw new ArgumentNullException("actionMap");
			}
			if (overrides == null)
			{
				throw new ArgumentNullException("overrides");
			}
			foreach (InputBinding @override in overrides)
			{
				actionMap.RemoveBindingOverride(@override);
			}
		}

		public static void RemoveAllBindingOverrides(this InputActionMap actionMap)
		{
			if (actionMap == null)
			{
				throw new ArgumentNullException("actionMap");
			}
			if (actionMap.m_Bindings != null)
			{
				InputBinding bindingOverride = default(InputBinding);
				int num = actionMap.m_Bindings.Length;
				for (int i = 0; i < num; i++)
				{
					actionMap.ApplyBindingOverride(i, bindingOverride);
				}
			}
		}

		public static int ApplyBindingOverridesOnMatchingControls(this InputAction action, InputControl control)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			ReadOnlyArray<InputBinding> bindings = action.bindings;
			int count = bindings.Count;
			int num = 0;
			for (int i = 0; i < count; i++)
			{
				InputControl inputControl = InputControlPath.TryFindControl(control, bindings[i].path);
				if (inputControl != null)
				{
					action.ApplyBindingOverride(i, inputControl.path);
					num++;
				}
			}
			return num;
		}

		public static int ApplyBindingOverridesOnMatchingControls(this InputActionMap actionMap, InputControl control)
		{
			if (actionMap == null)
			{
				throw new ArgumentNullException("actionMap");
			}
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			ReadOnlyArray<InputAction> actions = actionMap.actions;
			int count = actions.Count;
			int result = 0;
			for (int i = 0; i < count; i++)
			{
				result = actions[i].ApplyBindingOverridesOnMatchingControls(control);
			}
			return result;
		}

		public static RebindingOperation PerformInteractiveRebinding(this InputAction action, int bindingIndex = -1)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			RebindingOperation rebindingOperation = new RebindingOperation().WithAction(action).OnMatchWaitForAnother(0.05f).WithControlsExcluding("<Pointer>/delta")
				.WithControlsExcluding("<Pointer>/position")
				.WithControlsExcluding("<Touchscreen>/touch*/position")
				.WithControlsExcluding("<Touchscreen>/touch*/delta")
				.WithControlsExcluding("<Mouse>/clickCount")
				.WithMatchingEventsBeingSuppressed();
			if (rebindingOperation.expectedControlType != "Button")
			{
				rebindingOperation.WithCancelingThrough("<Keyboard>/escape");
			}
			if (bindingIndex >= 0)
			{
				ReadOnlyArray<InputBinding> bindings = action.bindings;
				if (bindingIndex >= bindings.Count)
				{
					throw new ArgumentOutOfRangeException(string.Format("Binding index {0} is out of range for action '{1}' with {2} bindings", bindingIndex, action, bindings.Count), "bindings");
				}
				if (bindings[bindingIndex].isComposite)
				{
					throw new InvalidOperationException(string.Format("Cannot perform rebinding on composite binding '{0}' of '{1}'", bindings[bindingIndex], action));
				}
				rebindingOperation.WithTargetBinding(bindingIndex);
				if (bindings[bindingIndex].isPartOfComposite)
				{
					int num = bindingIndex - 1;
					while (num > 0 && !bindings[num].isComposite)
					{
						num--;
					}
					if (num >= 0 && bindings[num].isComposite)
					{
						string expectedControlLayoutName = InputBindingComposite.GetExpectedControlLayoutName(bindings[num].GetNameOfComposite(), bindings[bindingIndex].name);
						if (!string.IsNullOrEmpty(expectedControlLayoutName))
						{
							rebindingOperation.WithExpectedControlType(expectedControlLayoutName);
						}
					}
				}
				string groups = bindings[bindingIndex].groups;
				InputActionMap actionMap = action.actionMap;
				InputActionAsset inputActionAsset = ((actionMap != null) ? actionMap.asset : null);
				if (inputActionAsset != null && !string.IsNullOrEmpty(action.bindings[bindingIndex].groups))
				{
					string[] array = groups.Split(';');
					for (int i = 0; i < array.Length; i++)
					{
						_003C_003Ec__DisplayClass24_0 _003C_003Ec__DisplayClass24_ = new _003C_003Ec__DisplayClass24_0();
						_003C_003Ec__DisplayClass24_.group = array[i];
						int num2 = inputActionAsset.controlSchemes.IndexOf(_003C_003Ec__DisplayClass24_._003CPerformInteractiveRebinding_003Eb__0);
						if (num2 == -1)
						{
							continue;
						}
						foreach (InputControlScheme.DeviceRequirement deviceRequirement in inputActionAsset.controlSchemes[num2].deviceRequirements)
						{
							rebindingOperation.WithControlsHavingToMatchPath(deviceRequirement.controlPath);
						}
					}
				}
			}
			return rebindingOperation;
		}

		internal static IDisposable DeferBindingResolution()
		{
			if (s_DeferBindingResolutionWrapper == null)
			{
				s_DeferBindingResolutionWrapper = new DeferBindingResolutionWrapper();
			}
			s_DeferBindingResolutionWrapper.Acquire();
			return s_DeferBindingResolutionWrapper;
		}
	}
}
