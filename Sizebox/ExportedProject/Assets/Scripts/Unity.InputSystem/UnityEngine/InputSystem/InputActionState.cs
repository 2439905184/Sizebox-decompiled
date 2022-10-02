using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
	internal class InputActionState : IInputStateChangeMonitor, ICloneable, IDisposable
	{
		[StructLayout(LayoutKind.Explicit, Size = 12)]
		internal struct InteractionState
		{
			[Flags]
			private enum Flags
			{
				TimerRunning = 1
			}

			[FieldOffset(0)]
			private ushort m_TriggerControlIndex;

			[FieldOffset(2)]
			private byte m_Phase;

			[FieldOffset(3)]
			private byte m_Flags;

			[FieldOffset(4)]
			private double m_StartTime;

			public int triggerControlIndex
			{
				get
				{
					return m_TriggerControlIndex;
				}
				set
				{
					if (value < 0 || value > 65535)
					{
						throw new NotSupportedException("Cannot have more than ushort.MaxValue controls in a single InputActionState");
					}
					m_TriggerControlIndex = (ushort)value;
				}
			}

			public double startTime
			{
				get
				{
					return m_StartTime;
				}
				set
				{
					m_StartTime = value;
				}
			}

			public bool isTimerRunning
			{
				get
				{
					return (m_Flags & 1) == 1;
				}
				set
				{
					if (value)
					{
						m_Flags |= 1;
						return;
					}
					Flags flags = ~Flags.TimerRunning;
					m_Flags &= (byte)flags;
				}
			}

			public InputActionPhase phase
			{
				get
				{
					return (InputActionPhase)m_Phase;
				}
				set
				{
					m_Phase = (byte)value;
				}
			}
		}

		[StructLayout(LayoutKind.Explicit, Size = 20)]
		internal struct BindingState
		{
			[Flags]
			public enum Flags
			{
				ChainsWithNext = 1,
				EndOfChain = 2,
				Composite = 4,
				PartOfComposite = 8,
				InitialStateCheckPending = 0x10,
				WantsInitialStateCheck = 0x20
			}

			[FieldOffset(0)]
			private byte m_ControlCount;

			[FieldOffset(1)]
			private byte m_InteractionCount;

			[FieldOffset(2)]
			private byte m_ProcessorCount;

			[FieldOffset(3)]
			private byte m_MapIndex;

			[FieldOffset(4)]
			private byte m_Flags;

			[FieldOffset(5)]
			private byte m_PartIndex;

			[FieldOffset(6)]
			private ushort m_ActionIndex;

			[FieldOffset(8)]
			private ushort m_CompositeOrCompositeBindingIndex;

			[FieldOffset(10)]
			private ushort m_ProcessorStartIndex;

			[FieldOffset(12)]
			private ushort m_InteractionStartIndex;

			[FieldOffset(14)]
			private ushort m_ControlStartIndex;

			[FieldOffset(16)]
			private int m_TriggerEventIdForComposite;

			public int controlStartIndex
			{
				get
				{
					return m_ControlStartIndex;
				}
				set
				{
					if (value >= 65535)
					{
						throw new NotSupportedException("Total control count in state cannot exceed byte.MaxValue=" + ushort.MaxValue);
					}
					m_ControlStartIndex = (ushort)value;
				}
			}

			public int controlCount
			{
				get
				{
					return m_ControlCount;
				}
				set
				{
					if (value >= 255)
					{
						throw new NotSupportedException("Control count per binding cannot exceed byte.MaxValue=" + byte.MaxValue);
					}
					m_ControlCount = (byte)value;
				}
			}

			public int interactionStartIndex
			{
				get
				{
					if (m_InteractionStartIndex == ushort.MaxValue)
					{
						return -1;
					}
					return m_InteractionStartIndex;
				}
				set
				{
					if (value == -1)
					{
						m_InteractionStartIndex = ushort.MaxValue;
						return;
					}
					if (value >= 65535)
					{
						throw new NotSupportedException("Interaction count cannot exceed ushort.MaxValue=" + ushort.MaxValue);
					}
					m_InteractionStartIndex = (ushort)value;
				}
			}

			public int interactionCount
			{
				get
				{
					return m_InteractionCount;
				}
				set
				{
					if (value >= 255)
					{
						throw new NotSupportedException("Interaction count per binding cannot exceed byte.MaxValue=" + byte.MaxValue);
					}
					m_InteractionCount = (byte)value;
				}
			}

			public int processorStartIndex
			{
				get
				{
					if (m_ProcessorStartIndex == ushort.MaxValue)
					{
						return -1;
					}
					return m_ProcessorStartIndex;
				}
				set
				{
					if (value == -1)
					{
						m_ProcessorStartIndex = ushort.MaxValue;
						return;
					}
					if (value >= 65535)
					{
						throw new NotSupportedException("Processor count cannot exceed ushort.MaxValue=" + ushort.MaxValue);
					}
					m_ProcessorStartIndex = (ushort)value;
				}
			}

			public int processorCount
			{
				get
				{
					return m_ProcessorCount;
				}
				set
				{
					if (value >= 255)
					{
						throw new NotSupportedException("Processor count per binding cannot exceed byte.MaxValue=" + byte.MaxValue);
					}
					m_ProcessorCount = (byte)value;
				}
			}

			public int actionIndex
			{
				get
				{
					if (m_ActionIndex == ushort.MaxValue)
					{
						return -1;
					}
					return m_ActionIndex;
				}
				set
				{
					if (value == -1)
					{
						m_ActionIndex = ushort.MaxValue;
						return;
					}
					if (value >= 65535)
					{
						throw new NotSupportedException("Action count cannot exceed ushort.MaxValue=" + ushort.MaxValue);
					}
					m_ActionIndex = (ushort)value;
				}
			}

			public int mapIndex
			{
				get
				{
					return m_MapIndex;
				}
				set
				{
					if (value >= 255)
					{
						throw new NotSupportedException("Map count cannot exceed byte.MaxValue=" + byte.MaxValue);
					}
					m_MapIndex = (byte)value;
				}
			}

			public int compositeOrCompositeBindingIndex
			{
				get
				{
					if (m_CompositeOrCompositeBindingIndex == ushort.MaxValue)
					{
						return -1;
					}
					return m_CompositeOrCompositeBindingIndex;
				}
				set
				{
					if (value == -1)
					{
						m_CompositeOrCompositeBindingIndex = ushort.MaxValue;
						return;
					}
					if (value >= 65535)
					{
						throw new NotSupportedException("Composite count cannot exceed ushort.MaxValue=" + ushort.MaxValue);
					}
					m_CompositeOrCompositeBindingIndex = (ushort)value;
				}
			}

			public int triggerEventIdForComposite
			{
				get
				{
					return m_TriggerEventIdForComposite;
				}
				set
				{
					m_TriggerEventIdForComposite = value;
				}
			}

			public Flags flags
			{
				get
				{
					return (Flags)m_Flags;
				}
				set
				{
					m_Flags = (byte)value;
				}
			}

			public bool chainsWithNext
			{
				get
				{
					return (flags & Flags.ChainsWithNext) == Flags.ChainsWithNext;
				}
				set
				{
					if (value)
					{
						flags |= Flags.ChainsWithNext;
					}
					else
					{
						flags &= ~Flags.ChainsWithNext;
					}
				}
			}

			public bool isEndOfChain
			{
				get
				{
					return (flags & Flags.EndOfChain) == Flags.EndOfChain;
				}
				set
				{
					if (value)
					{
						flags |= Flags.EndOfChain;
					}
					else
					{
						flags &= ~Flags.EndOfChain;
					}
				}
			}

			public bool isPartOfChain
			{
				get
				{
					if (!chainsWithNext)
					{
						return isEndOfChain;
					}
					return true;
				}
			}

			public bool isComposite
			{
				get
				{
					return (flags & Flags.Composite) == Flags.Composite;
				}
				set
				{
					if (value)
					{
						flags |= Flags.Composite;
					}
					else
					{
						flags &= ~Flags.Composite;
					}
				}
			}

			public bool isPartOfComposite
			{
				get
				{
					return (flags & Flags.PartOfComposite) == Flags.PartOfComposite;
				}
				set
				{
					if (value)
					{
						flags |= Flags.PartOfComposite;
					}
					else
					{
						flags &= ~Flags.PartOfComposite;
					}
				}
			}

			public bool initialStateCheckPending
			{
				get
				{
					return (flags & Flags.InitialStateCheckPending) != 0;
				}
				set
				{
					if (value)
					{
						flags |= Flags.InitialStateCheckPending;
					}
					else
					{
						flags &= ~Flags.InitialStateCheckPending;
					}
				}
			}

			public bool wantsInitialStateCheck
			{
				get
				{
					return (flags & Flags.WantsInitialStateCheck) != 0;
				}
				set
				{
					if (value)
					{
						flags |= Flags.WantsInitialStateCheck;
					}
					else
					{
						flags &= ~Flags.WantsInitialStateCheck;
					}
				}
			}

			public int partIndex
			{
				get
				{
					return m_PartIndex;
				}
				set
				{
					if (partIndex < 0)
					{
						throw new ArgumentOutOfRangeException("value", "Part index must not be negative");
					}
					if (partIndex > 255)
					{
						throw new InvalidOperationException("Part count must not exceed byte.MaxValue=" + byte.MaxValue);
					}
					m_PartIndex = (byte)value;
				}
			}
		}

		[StructLayout(LayoutKind.Explicit, Size = 36)]
		public struct TriggerState
		{
			[Flags]
			public enum Flags
			{
				HaveMagnitude = 1,
				PassThrough = 2,
				MayNeedConflictResolution = 4,
				HasMultipleConcurrentActuations = 8
			}

			[FieldOffset(0)]
			private byte m_Phase;

			[FieldOffset(1)]
			private byte m_Flags;

			[FieldOffset(2)]
			private byte m_MapIndex;

			[FieldOffset(4)]
			private double m_Time;

			[FieldOffset(12)]
			private double m_StartTime;

			[FieldOffset(20)]
			private ushort m_ControlIndex;

			[FieldOffset(24)]
			private ushort m_BindingIndex;

			[FieldOffset(26)]
			private ushort m_InteractionIndex;

			[FieldOffset(28)]
			private float m_Magnitude;

			[FieldOffset(32)]
			private uint m_LastTriggeredInUpdate;

			public InputActionPhase phase
			{
				get
				{
					return (InputActionPhase)m_Phase;
				}
				set
				{
					m_Phase = (byte)value;
				}
			}

			public bool isDisabled
			{
				get
				{
					return phase == InputActionPhase.Disabled;
				}
			}

			public bool isWaiting
			{
				get
				{
					return phase == InputActionPhase.Waiting;
				}
			}

			public bool isStarted
			{
				get
				{
					return phase == InputActionPhase.Started;
				}
			}

			public bool isPerformed
			{
				get
				{
					return phase == InputActionPhase.Performed;
				}
			}

			public bool isCanceled
			{
				get
				{
					return phase == InputActionPhase.Canceled;
				}
			}

			public double time
			{
				get
				{
					return m_Time;
				}
				set
				{
					m_Time = value;
				}
			}

			public double startTime
			{
				get
				{
					return m_StartTime;
				}
				set
				{
					m_StartTime = value;
				}
			}

			public float magnitude
			{
				get
				{
					return m_Magnitude;
				}
				set
				{
					flags |= Flags.HaveMagnitude;
					m_Magnitude = value;
				}
			}

			public bool haveMagnitude
			{
				get
				{
					return (flags & Flags.HaveMagnitude) != 0;
				}
			}

			public int mapIndex
			{
				get
				{
					return m_MapIndex;
				}
				set
				{
					if (value < 0 || value > 255)
					{
						throw new NotSupportedException("More than byte.MaxValue InputActionMaps in a single InputActionState");
					}
					m_MapIndex = (byte)value;
				}
			}

			public int controlIndex
			{
				get
				{
					if (m_ControlIndex == ushort.MaxValue)
					{
						return -1;
					}
					return m_ControlIndex;
				}
				set
				{
					if (value == -1)
					{
						m_ControlIndex = ushort.MaxValue;
						return;
					}
					if (value < 0 || value >= 65535)
					{
						throw new NotSupportedException("More than ushort.MaxValue-1 controls in a single InputActionState");
					}
					m_ControlIndex = (ushort)value;
				}
			}

			public int bindingIndex
			{
				get
				{
					return m_BindingIndex;
				}
				set
				{
					if (value < 0 || value > 65535)
					{
						throw new NotSupportedException("More than ushort.MaxValue bindings in a single InputActionState");
					}
					m_BindingIndex = (ushort)value;
				}
			}

			public int interactionIndex
			{
				get
				{
					if (m_InteractionIndex == ushort.MaxValue)
					{
						return -1;
					}
					return m_InteractionIndex;
				}
				set
				{
					if (value == -1)
					{
						m_InteractionIndex = ushort.MaxValue;
						return;
					}
					if (value < 0 || value >= 65535)
					{
						throw new NotSupportedException("More than ushort.MaxValue-1 interactions in a single InputActionState");
					}
					m_InteractionIndex = (ushort)value;
				}
			}

			public uint lastTriggeredInUpdate
			{
				get
				{
					return m_LastTriggeredInUpdate;
				}
				set
				{
					m_LastTriggeredInUpdate = value;
				}
			}

			public bool isPassThrough
			{
				get
				{
					return (flags & Flags.PassThrough) != 0;
				}
				set
				{
					if (value)
					{
						flags |= Flags.PassThrough;
					}
					else
					{
						flags &= ~Flags.PassThrough;
					}
				}
			}

			public bool mayNeedConflictResolution
			{
				get
				{
					return (flags & Flags.MayNeedConflictResolution) != 0;
				}
				set
				{
					if (value)
					{
						flags |= Flags.MayNeedConflictResolution;
					}
					else
					{
						flags &= ~Flags.MayNeedConflictResolution;
					}
				}
			}

			public bool hasMultipleConcurrentActuations
			{
				get
				{
					return (flags & Flags.HasMultipleConcurrentActuations) != 0;
				}
				set
				{
					if (value)
					{
						flags |= Flags.HasMultipleConcurrentActuations;
					}
					else
					{
						flags &= ~Flags.HasMultipleConcurrentActuations;
					}
				}
			}

			public Flags flags
			{
				get
				{
					return (Flags)m_Flags;
				}
				set
				{
					m_Flags = (byte)value;
				}
			}
		}

		public struct ActionMapIndices
		{
			public int actionStartIndex;

			public int actionCount;

			public int controlStartIndex;

			public int controlCount;

			public int bindingStartIndex;

			public int bindingCount;

			public int interactionStartIndex;

			public int interactionCount;

			public int processorStartIndex;

			public int processorCount;

			public int compositeStartIndex;

			public int compositeCount;
		}

		public struct UnmanagedMemory : IDisposable
		{
			public unsafe void* basePtr;

			public int mapCount;

			public int actionCount;

			public int interactionCount;

			public int bindingCount;

			public int controlCount;

			public int compositeCount;

			public unsafe TriggerState* actionStates;

			public unsafe BindingState* bindingStates;

			public unsafe InteractionState* interactionStates;

			public unsafe float* controlMagnitudes;

			public unsafe float* compositeMagnitudes;

			public unsafe int* enabledControls;

			public unsafe ushort* actionBindingIndicesAndCounts;

			public unsafe ushort* actionBindingIndices;

			public unsafe int* controlIndexToBindingIndex;

			public unsafe ActionMapIndices* mapIndices;

			public unsafe bool isAllocated
			{
				get
				{
					return basePtr != null;
				}
			}

			public unsafe int sizeInBytes
			{
				get
				{
					return mapCount * sizeof(ActionMapIndices) + actionCount * sizeof(TriggerState) + bindingCount * sizeof(BindingState) + interactionCount * sizeof(InteractionState) + controlCount * 4 + compositeCount * 4 + controlCount * 4 + actionCount * 2 * 2 + bindingCount * 2 + (controlCount + 31) / 32 * 4;
				}
			}

			public unsafe void Allocate(int mapCount, int actionCount, int bindingCount, int controlCount, int interactionCount, int compositeCount)
			{
				this.mapCount = mapCount;
				this.actionCount = actionCount;
				this.interactionCount = interactionCount;
				this.bindingCount = bindingCount;
				this.controlCount = controlCount;
				this.compositeCount = compositeCount;
				byte* ptr = (byte*)UnsafeUtility.Malloc(sizeInBytes, 4, Allocator.Persistent);
				UnsafeUtility.MemClear(ptr, sizeInBytes);
				basePtr = ptr;
				mapIndices = (ActionMapIndices*)ptr;
				ptr = (byte*)(enabledControls = (int*)((byte*)(actionBindingIndices = (ushort*)((byte*)(actionBindingIndicesAndCounts = (ushort*)((byte*)(controlIndexToBindingIndex = (int*)((byte*)(compositeMagnitudes = (float*)((byte*)(controlMagnitudes = (float*)((byte*)(bindingStates = (BindingState*)((byte*)(interactionStates = (InteractionState*)((byte*)(actionStates = (TriggerState*)(ptr + mapCount * sizeof(ActionMapIndices))) + actionCount * sizeof(TriggerState))) + interactionCount * sizeof(InteractionState))) + bindingCount * sizeof(BindingState))) + controlCount * 4)) + compositeCount * 4)) + controlCount * 4)) + actionCount * 2 * 2)) + bindingCount * 2)) + (controlCount + 31) / 32 * 4;
			}

			public unsafe void Dispose()
			{
				if (basePtr != null)
				{
					UnsafeUtility.Free(basePtr, Allocator.Persistent);
					basePtr = null;
					actionStates = null;
					interactionStates = null;
					bindingStates = null;
					mapIndices = null;
					controlMagnitudes = null;
					compositeMagnitudes = null;
					controlIndexToBindingIndex = null;
					actionBindingIndices = null;
					actionBindingIndicesAndCounts = null;
					mapCount = 0;
					actionCount = 0;
					bindingCount = 0;
					controlCount = 0;
					interactionCount = 0;
					compositeCount = 0;
				}
			}

			public unsafe void CopyDataFrom(UnmanagedMemory memory)
			{
				UnsafeUtility.MemCpy(mapIndices, memory.mapIndices, memory.mapCount * sizeof(ActionMapIndices));
				UnsafeUtility.MemCpy(actionStates, memory.actionStates, memory.actionCount * sizeof(TriggerState));
				UnsafeUtility.MemCpy(bindingStates, memory.bindingStates, memory.bindingCount * sizeof(BindingState));
				UnsafeUtility.MemCpy(interactionStates, memory.interactionStates, memory.interactionCount * sizeof(InteractionState));
				UnsafeUtility.MemCpy(controlMagnitudes, memory.controlMagnitudes, memory.controlCount * 4);
				UnsafeUtility.MemCpy(compositeMagnitudes, memory.compositeMagnitudes, memory.compositeCount * 4);
				UnsafeUtility.MemCpy(controlIndexToBindingIndex, memory.controlIndexToBindingIndex, memory.controlCount * 4);
				UnsafeUtility.MemCpy(actionBindingIndicesAndCounts, memory.actionBindingIndicesAndCounts, memory.actionCount * 2 * 2);
				UnsafeUtility.MemCpy(actionBindingIndices, memory.actionBindingIndices, memory.bindingCount * 2);
				UnsafeUtility.MemCpy(enabledControls, memory.enabledControls, (memory.controlCount + 31) / 32 * 4);
			}

			public UnmanagedMemory Clone()
			{
				if (!isAllocated)
				{
					return default(UnmanagedMemory);
				}
				UnmanagedMemory result = default(UnmanagedMemory);
				result.Allocate(mapCount, actionCount, controlCount: controlCount, bindingCount: bindingCount, interactionCount: interactionCount, compositeCount: compositeCount);
				result.CopyDataFrom(this);
				return result;
			}
		}

		public const int kInvalidIndex = -1;

		public InputActionMap[] maps;

		public InputControl[] controls;

		public IInputInteraction[] interactions;

		public InputProcessor[] processors;

		public InputBindingComposite[] composites;

		public int totalProcessorCount;

		public UnmanagedMemory memory;

		private bool m_OnBeforeUpdateHooked;

		private bool m_OnAfterUpdateHooked;

		private Action m_OnBeforeUpdateDelegate;

		private Action m_OnAfterUpdateDelegate;

		internal static InlinedArray<GCHandle> s_GlobalList;

		internal static InlinedArray<Action<object, InputActionChange>> s_OnActionChange;

		internal static InlinedArray<Action<object>> s_OnActionControlsChanged;

		public int totalCompositeCount
		{
			get
			{
				return memory.compositeCount;
			}
		}

		public int totalMapCount
		{
			get
			{
				return memory.mapCount;
			}
		}

		public int totalActionCount
		{
			get
			{
				return memory.actionCount;
			}
		}

		public int totalBindingCount
		{
			get
			{
				return memory.bindingCount;
			}
		}

		public int totalInteractionCount
		{
			get
			{
				return memory.interactionCount;
			}
		}

		public int totalControlCount
		{
			get
			{
				return memory.controlCount;
			}
		}

		public unsafe ActionMapIndices* mapIndices
		{
			get
			{
				return memory.mapIndices;
			}
		}

		public unsafe TriggerState* actionStates
		{
			get
			{
				return memory.actionStates;
			}
		}

		public unsafe BindingState* bindingStates
		{
			get
			{
				return memory.bindingStates;
			}
		}

		public unsafe InteractionState* interactionStates
		{
			get
			{
				return memory.interactionStates;
			}
		}

		public unsafe int* controlIndexToBindingIndex
		{
			get
			{
				return memory.controlIndexToBindingIndex;
			}
		}

		public unsafe int* enabledControls
		{
			get
			{
				return memory.enabledControls;
			}
		}

		public void Initialize(InputBindingResolver resolver)
		{
			ClaimDataFrom(resolver);
			AddToGlobaList();
		}

		internal void ClaimDataFrom(InputBindingResolver resolver)
		{
			totalProcessorCount = resolver.totalProcessorCount;
			maps = resolver.maps;
			interactions = resolver.interactions;
			processors = resolver.processors;
			composites = resolver.composites;
			controls = resolver.controls;
			memory = resolver.memory;
			resolver.memory = default(UnmanagedMemory);
		}

		~InputActionState()
		{
			Destroy(true);
		}

		public void Dispose()
		{
			Destroy();
		}

		private unsafe void Destroy(bool isFinalizing = false)
		{
			if (!isFinalizing)
			{
				for (int i = 0; i < totalMapCount; i++)
				{
					InputActionMap inputActionMap = maps[i];
					if (inputActionMap.enabled)
					{
						DisableControls(i, mapIndices[i].controlStartIndex, mapIndices[i].controlCount);
					}
					if (inputActionMap.m_Asset != null)
					{
						inputActionMap.m_Asset.m_SharedStateForAllMaps = null;
					}
					inputActionMap.m_State = null;
					inputActionMap.m_MapIndexInState = -1;
					inputActionMap.m_EnabledActionsCount = 0;
					InputAction[] actions = inputActionMap.m_Actions;
					if (actions != null)
					{
						for (int j = 0; j < actions.Length; j++)
						{
							actions[j].m_ActionIndexInState = -1;
						}
					}
				}
				RemoveMapFromGlobalList();
			}
			memory.Dispose();
		}

		public InputActionState Clone()
		{
			return new InputActionState
			{
				maps = ArrayHelpers.Copy(maps),
				controls = ArrayHelpers.Copy(controls),
				interactions = ArrayHelpers.Copy(interactions),
				processors = ArrayHelpers.Copy(processors),
				composites = ArrayHelpers.Copy(composites),
				totalProcessorCount = totalProcessorCount,
				memory = memory.Clone()
			};
		}

		object ICloneable.Clone()
		{
			return Clone();
		}

		private bool IsUsingDevice(InputDevice device)
		{
			bool flag = false;
			for (int i = 0; i < totalMapCount; i++)
			{
				ReadOnlyArray<InputDevice>? devices = maps[i].devices;
				if (!devices.HasValue)
				{
					flag = true;
				}
				else if (Enumerable.Contains(devices.Value, device))
				{
					return true;
				}
			}
			if (!flag)
			{
				return false;
			}
			for (int j = 0; j < totalControlCount; j++)
			{
				if (controls[j].device == device)
				{
					return true;
				}
			}
			return false;
		}

		private bool CanUseDevice(InputDevice device)
		{
			bool flag = false;
			for (int i = 0; i < totalMapCount; i++)
			{
				ReadOnlyArray<InputDevice>? devices = maps[i].devices;
				if (!devices.HasValue)
				{
					flag = true;
				}
				else if (Enumerable.Contains(devices.Value, device))
				{
					return true;
				}
			}
			if (!flag)
			{
				return false;
			}
			for (int j = 0; j < totalMapCount; j++)
			{
				InputBinding[] bindings = maps[j].m_Bindings;
				if (bindings == null)
				{
					continue;
				}
				int num = bindings.Length;
				for (int k = 0; k < num; k++)
				{
					if (InputControlPath.TryFindControl(device, bindings[k].effectivePath) != null)
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool HasEnabledActions()
		{
			for (int i = 0; i < totalMapCount; i++)
			{
				if (maps[i].enabled)
				{
					return true;
				}
			}
			return false;
		}

		public unsafe void RestoreActionStates(UnmanagedMemory oldState)
		{
			for (int i = 0; i < memory.bindingCount; i++)
			{
				BindingState* ptr = memory.bindingStates + i;
				if (ptr->isPartOfComposite)
				{
					continue;
				}
				int actionIndex = ptr->actionIndex;
				if (actionIndex != -1 && oldState.actionStates[actionIndex].phase != 0)
				{
					TriggerState* ptr2 = memory.actionStates + actionIndex;
					if (ptr2->phase == InputActionPhase.Disabled)
					{
						ptr2->phase = InputActionPhase.Waiting;
						int mapIndex = ptr2->mapIndex;
						maps[mapIndex].m_EnabledActionsCount++;
					}
					EnableControls(ptr2->mapIndex, ptr->controlStartIndex, ptr->controlCount);
				}
			}
			HookOnBeforeUpdate();
			if (s_OnActionChange.length <= 0)
			{
				return;
			}
			for (int j = 0; j < totalMapCount; j++)
			{
				InputActionMap inputActionMap = maps[j];
				if (inputActionMap.m_SingletonAction == null && inputActionMap.m_EnabledActionsCount == inputActionMap.m_Actions.LengthSafe())
				{
					NotifyListenersOfActionChange(InputActionChange.ActionMapEnabled, inputActionMap);
					continue;
				}
				ReadOnlyArray<InputAction> actions = inputActionMap.actions;
				for (int k = 0; k < actions.Count; k++)
				{
					NotifyListenersOfActionChange(InputActionChange.ActionEnabled, actions[k]);
				}
			}
		}

		public unsafe void ResetActionState(int actionIndex, InputActionPhase toPhase = InputActionPhase.Waiting)
		{
			TriggerState* ptr = actionStates + actionIndex;
			if (ptr->phase != InputActionPhase.Waiting)
			{
				ptr->time = InputRuntime.s_Instance.currentTime;
				if (ptr->interactionIndex != -1)
				{
					int bindingIndex = ptr->bindingIndex;
					if (bindingIndex != -1)
					{
						int mapIndex = ptr->mapIndex;
						int interactionCount = bindingStates[bindingIndex].interactionCount;
						int interactionStartIndex = bindingStates[bindingIndex].interactionStartIndex;
						for (int i = 0; i < interactionCount; i++)
						{
							int interactionIndex = interactionStartIndex + i;
							ResetInteractionStateAndCancelIfNecessary(mapIndex, bindingIndex, interactionIndex);
						}
					}
				}
				else
				{
					ChangePhaseOfAction(InputActionPhase.Canceled, ref actionStates[actionIndex]);
				}
			}
			TriggerState triggerState = *ptr;
			triggerState.phase = toPhase;
			triggerState.controlIndex = -1;
			triggerState.bindingIndex = 0;
			triggerState.interactionIndex = -1;
			triggerState.startTime = 0.0;
			triggerState.time = 0.0;
			triggerState.hasMultipleConcurrentActuations = false;
			triggerState.lastTriggeredInUpdate = 0u;
			*ptr = triggerState;
		}

		public unsafe ref TriggerState FetchActionState(InputAction action)
		{
			return ref actionStates[action.m_ActionIndexInState];
		}

		public unsafe ActionMapIndices FetchMapIndices(InputActionMap map)
		{
			return mapIndices[map.m_MapIndexInState];
		}

		public unsafe void EnableAllActions(InputActionMap map)
		{
			EnableControls(map);
			int mapIndexInState = map.m_MapIndexInState;
			int actionCount = mapIndices[mapIndexInState].actionCount;
			int actionStartIndex = mapIndices[mapIndexInState].actionStartIndex;
			for (int i = 0; i < actionCount; i++)
			{
				int num = actionStartIndex + i;
				if (actionStates[num].isDisabled)
				{
					actionStates[num].phase = InputActionPhase.Waiting;
				}
			}
			map.m_EnabledActionsCount = actionCount;
			HookOnBeforeUpdate();
			if (map.m_SingletonAction != null)
			{
				NotifyListenersOfActionChange(InputActionChange.ActionEnabled, map.m_SingletonAction);
			}
			else
			{
				NotifyListenersOfActionChange(InputActionChange.ActionMapEnabled, map);
			}
		}

		private unsafe void EnableControls(InputActionMap map)
		{
			int mapIndexInState = map.m_MapIndexInState;
			int controlCount = mapIndices[mapIndexInState].controlCount;
			int controlStartIndex = mapIndices[mapIndexInState].controlStartIndex;
			if (controlCount > 0)
			{
				EnableControls(mapIndexInState, controlStartIndex, controlCount);
			}
		}

		public unsafe void EnableSingleAction(InputAction action)
		{
			EnableControls(action);
			int actionIndexInState = action.m_ActionIndexInState;
			actionStates[actionIndexInState].phase = InputActionPhase.Waiting;
			action.m_ActionMap.m_EnabledActionsCount++;
			HookOnBeforeUpdate();
			NotifyListenersOfActionChange(InputActionChange.ActionEnabled, action);
		}

		private unsafe void EnableControls(InputAction action)
		{
			int actionIndexInState = action.m_ActionIndexInState;
			int mapIndexInState = action.m_ActionMap.m_MapIndexInState;
			int bindingStartIndex = mapIndices[mapIndexInState].bindingStartIndex;
			int bindingCount = mapIndices[mapIndexInState].bindingCount;
			BindingState* ptr = memory.bindingStates;
			for (int i = 0; i < bindingCount; i++)
			{
				int num = bindingStartIndex + i;
				BindingState* ptr2 = ptr + num;
				if (ptr2->actionIndex == actionIndexInState && !ptr2->isPartOfComposite)
				{
					int controlCount = ptr2->controlCount;
					if (controlCount != 0)
					{
						EnableControls(mapIndexInState, ptr2->controlStartIndex, controlCount);
					}
				}
			}
		}

		public unsafe void DisableAllActions(InputActionMap map)
		{
			DisableControls(map);
			int mapIndexInState = map.m_MapIndexInState;
			int actionStartIndex = mapIndices[mapIndexInState].actionStartIndex;
			int actionCount = mapIndices[mapIndexInState].actionCount;
			for (int i = 0; i < actionCount; i++)
			{
				int num = actionStartIndex + i;
				if (actionStates[num].phase != 0)
				{
					ResetActionState(num, InputActionPhase.Disabled);
				}
			}
			map.m_EnabledActionsCount = 0;
			if (map.m_SingletonAction != null)
			{
				NotifyListenersOfActionChange(InputActionChange.ActionDisabled, map.m_SingletonAction);
			}
			else
			{
				NotifyListenersOfActionChange(InputActionChange.ActionMapDisabled, map);
			}
		}

		private unsafe void DisableControls(InputActionMap map)
		{
			int mapIndexInState = map.m_MapIndexInState;
			int controlCount = mapIndices[mapIndexInState].controlCount;
			int controlStartIndex = mapIndices[mapIndexInState].controlStartIndex;
			if (controlCount > 0)
			{
				DisableControls(mapIndexInState, controlStartIndex, controlCount);
			}
		}

		public void DisableSingleAction(InputAction action)
		{
			DisableControls(action);
			ResetActionState(action.m_ActionIndexInState, InputActionPhase.Disabled);
			action.m_ActionMap.m_EnabledActionsCount--;
			NotifyListenersOfActionChange(InputActionChange.ActionDisabled, action);
		}

		private unsafe void DisableControls(InputAction action)
		{
			int actionIndexInState = action.m_ActionIndexInState;
			int mapIndexInState = action.m_ActionMap.m_MapIndexInState;
			int bindingStartIndex = mapIndices[mapIndexInState].bindingStartIndex;
			int bindingCount = mapIndices[mapIndexInState].bindingCount;
			BindingState* ptr = memory.bindingStates;
			for (int i = 0; i < bindingCount; i++)
			{
				int num = bindingStartIndex + i;
				BindingState* ptr2 = ptr + num;
				if (ptr2->actionIndex == actionIndexInState && !ptr2->isPartOfComposite)
				{
					int controlCount = ptr2->controlCount;
					if (controlCount != 0)
					{
						DisableControls(mapIndexInState, ptr2->controlStartIndex, controlCount);
					}
				}
			}
		}

		private unsafe void EnableControls(int mapIndex, int controlStartIndex, int numControls)
		{
			InputManager s_Manager = InputSystem.s_Manager;
			for (int i = 0; i < numControls; i++)
			{
				int num = controlStartIndex + i;
				if (!IsControlEnabled(num))
				{
					int num2 = controlIndexToBindingIndex[num];
					long monitorIndex = ToCombinedMapAndControlAndBindingIndex(mapIndex, num, num2);
					BindingState* ptr = bindingStates + num2;
					if (ptr->wantsInitialStateCheck)
					{
						ptr->initialStateCheckPending = true;
					}
					s_Manager.AddStateChangeMonitor(controls[num], this, monitorIndex);
					SetControlEnabled(num, true);
				}
			}
		}

		private unsafe void DisableControls(int mapIndex, int controlStartIndex, int numControls)
		{
			InputManager s_Manager = InputSystem.s_Manager;
			for (int i = 0; i < numControls; i++)
			{
				int num = controlStartIndex + i;
				if (IsControlEnabled(num))
				{
					int num2 = controlIndexToBindingIndex[num];
					long monitorIndex = ToCombinedMapAndControlAndBindingIndex(mapIndex, num, num2);
					BindingState* ptr = bindingStates + num2;
					if (ptr->wantsInitialStateCheck)
					{
						ptr->initialStateCheckPending = false;
					}
					s_Manager.RemoveStateChangeMonitor(controls[num], this, monitorIndex);
					SetControlEnabled(num, false);
				}
			}
		}

		private unsafe bool IsControlEnabled(int controlIndex)
		{
			int num = controlIndex / 32;
			int num2 = 1 << controlIndex % 32;
			return (enabledControls[num] & num2) != 0;
		}

		private unsafe void SetControlEnabled(int controlIndex, bool state)
		{
			int num = controlIndex / 32;
			int num2 = 1 << controlIndex % 32;
			if (state)
			{
				enabledControls[num] |= num2;
			}
			else
			{
				enabledControls[num] &= ~num2;
			}
		}

		private void HookOnBeforeUpdate()
		{
			if (!m_OnBeforeUpdateHooked)
			{
				if (m_OnBeforeUpdateDelegate == null)
				{
					m_OnBeforeUpdateDelegate = OnBeforeInitialUpdate;
				}
				InputSystem.s_Manager.onBeforeUpdate += m_OnBeforeUpdateDelegate;
				m_OnBeforeUpdateHooked = true;
			}
		}

		private void UnhookOnBeforeUpdate()
		{
			if (m_OnBeforeUpdateHooked)
			{
				InputSystem.s_Manager.onBeforeUpdate -= m_OnBeforeUpdateDelegate;
				m_OnBeforeUpdateHooked = false;
			}
		}

		private unsafe void OnBeforeInitialUpdate()
		{
			if (InputState.currentUpdateType == InputUpdateType.BeforeRender)
			{
				return;
			}
			UnhookOnBeforeUpdate();
			InputEvent inputEvent = default(InputEvent);
			inputEvent.eventId = 1234;
			InputEvent inputEvent2 = inputEvent;
			InputEventPtr eventPtr = new InputEventPtr(&inputEvent2);
			double currentTime = InputRuntime.s_Instance.currentTime;
			for (int i = 0; i < totalBindingCount; i++)
			{
				if (!bindingStates[i].initialStateCheckPending)
				{
					continue;
				}
				bindingStates[i].initialStateCheckPending = false;
				int mapIndex = bindingStates[i].mapIndex;
				int controlStartIndex = bindingStates[i].controlStartIndex;
				int controlCount = bindingStates[i].controlCount;
				for (int j = 0; j < controlCount; j++)
				{
					int num = controlStartIndex + j;
					if (!controls[num].CheckStateIsAtDefault())
					{
						ProcessControlStateChange(mapIndex, num, i, currentTime, eventPtr);
					}
				}
			}
		}

		void IInputStateChangeMonitor.NotifyControlStateChanged(InputControl control, double time, InputEventPtr eventPtr, long mapControlAndBindingIndex)
		{
			int mapIndex;
			int controlIndex;
			int bindingIndex;
			SplitUpMapAndControlAndBindingIndex(mapControlAndBindingIndex, out mapIndex, out controlIndex, out bindingIndex);
			ProcessControlStateChange(mapIndex, controlIndex, bindingIndex, time, eventPtr);
		}

		void IInputStateChangeMonitor.NotifyTimerExpired(InputControl control, double time, long mapControlAndBindingIndex, int interactionIndex)
		{
			int mapIndex;
			int controlIndex;
			int bindingIndex;
			SplitUpMapAndControlAndBindingIndex(mapControlAndBindingIndex, out mapIndex, out controlIndex, out bindingIndex);
			ProcessTimeout(time, mapIndex, controlIndex, bindingIndex, interactionIndex);
		}

		private static long ToCombinedMapAndControlAndBindingIndex(int mapIndex, int controlIndex, int bindingIndex)
		{
			return controlIndex | ((long)bindingIndex << 32) | ((long)mapIndex << 48);
		}

		private static void SplitUpMapAndControlAndBindingIndex(long mapControlAndBindingIndex, out int mapIndex, out int controlIndex, out int bindingIndex)
		{
			controlIndex = (int)(mapControlAndBindingIndex & 0xFFFFFFFFu);
			bindingIndex = (int)((mapControlAndBindingIndex >> 32) & 0xFFFF);
			mapIndex = (int)(mapControlAndBindingIndex >> 48);
		}

		private unsafe void ProcessControlStateChange(int mapIndex, int controlIndex, int bindingIndex, double time, InputEventPtr eventPtr)
		{
			BindingState* ptr = bindingStates + bindingIndex;
			int actionIndex = ptr->actionIndex;
			TriggerState triggerState = default(TriggerState);
			triggerState.mapIndex = mapIndex;
			triggerState.controlIndex = controlIndex;
			triggerState.bindingIndex = bindingIndex;
			triggerState.interactionIndex = -1;
			triggerState.time = time;
			triggerState.startTime = time;
			triggerState.isPassThrough = actionIndex != -1 && actionStates[actionIndex].isPassThrough;
			TriggerState trigger = triggerState;
			if (m_OnBeforeUpdateHooked)
			{
				ptr->initialStateCheckPending = false;
			}
			bool flag = false;
			if (ptr->isPartOfComposite)
			{
				int compositeOrCompositeBindingIndex = ptr->compositeOrCompositeBindingIndex;
				BindingState* ptr2 = bindingStates + compositeOrCompositeBindingIndex;
				if (ShouldIgnoreControlStateChangeOnCompositeBinding(ptr2, eventPtr) || ShouldIgnoreControlStateChange(ref trigger, actionIndex))
				{
					return;
				}
				int interactionCount = ptr2->interactionCount;
				if (interactionCount > 0)
				{
					flag = true;
					ProcessInteractions(ref trigger, ptr2->interactionStartIndex, interactionCount);
				}
			}
			else if (ShouldIgnoreControlStateChange(ref trigger, actionIndex))
			{
				return;
			}
			int interactionCount2 = ptr->interactionCount;
			if (interactionCount2 > 0 && !ptr->isPartOfComposite)
			{
				ProcessInteractions(ref trigger, ptr->interactionStartIndex, interactionCount2);
			}
			else if (!flag)
			{
				ProcessDefaultInteraction(ref trigger, actionIndex);
			}
		}

		private unsafe static bool ShouldIgnoreControlStateChangeOnCompositeBinding(BindingState* binding, InputEvent* eventPtr)
		{
			if (eventPtr == null)
			{
				return false;
			}
			int eventId = eventPtr->eventId;
			if (binding->triggerEventIdForComposite == eventId)
			{
				return true;
			}
			binding->triggerEventIdForComposite = eventId;
			return false;
		}

		private unsafe bool ShouldIgnoreControlStateChange(ref TriggerState trigger, int actionIndex)
		{
			TriggerState* ptr = actionStates + actionIndex;
			if (!ptr->mayNeedConflictResolution)
			{
				return false;
			}
			if (!trigger.haveMagnitude)
			{
				trigger.magnitude = ComputeMagnitude(trigger.bindingIndex, trigger.controlIndex);
			}
			int num = trigger.controlIndex;
			if (bindingStates[trigger.bindingIndex].isPartOfComposite)
			{
				int compositeOrCompositeBindingIndex = bindingStates[trigger.bindingIndex].compositeOrCompositeBindingIndex;
				int compositeOrCompositeBindingIndex2 = bindingStates[compositeOrCompositeBindingIndex].compositeOrCompositeBindingIndex;
				memory.compositeMagnitudes[compositeOrCompositeBindingIndex2] = trigger.magnitude;
				num = bindingStates[compositeOrCompositeBindingIndex].controlStartIndex;
			}
			else
			{
				memory.controlMagnitudes[num] = trigger.magnitude;
			}
			if (ptr->controlIndex == -1)
			{
				return false;
			}
			if (trigger.magnitude > ptr->magnitude)
			{
				if (trigger.magnitude > 0f && num != ptr->controlIndex && ptr->magnitude > 0f)
				{
					ptr->hasMultipleConcurrentActuations = true;
				}
				return false;
			}
			int num2 = ptr->controlIndex;
			if (bindingStates[ptr->bindingIndex].isPartOfComposite)
			{
				int compositeOrCompositeBindingIndex3 = bindingStates[ptr->bindingIndex].compositeOrCompositeBindingIndex;
				num2 = bindingStates[compositeOrCompositeBindingIndex3].controlStartIndex;
			}
			if (trigger.magnitude < ptr->magnitude)
			{
				if (num != num2)
				{
					return true;
				}
				if (!ptr->hasMultipleConcurrentActuations)
				{
					return false;
				}
				ushort num3 = memory.actionBindingIndicesAndCounts[actionIndex * 2];
				ushort num4 = memory.actionBindingIndicesAndCounts[actionIndex * 2 + 1];
				float num5 = trigger.magnitude;
				int num6 = -1;
				int bindingIndex = -1;
				int num7 = 0;
				for (int i = 0; i < num4; i++)
				{
					ushort num8 = memory.actionBindingIndices[num3 + i];
					BindingState* ptr2 = memory.bindingStates + (int)num8;
					if (ptr2->isComposite)
					{
						int controlStartIndex = ptr2->controlStartIndex;
						int compositeOrCompositeBindingIndex4 = ptr2->compositeOrCompositeBindingIndex;
						float num9 = memory.compositeMagnitudes[compositeOrCompositeBindingIndex4];
						if (num9 > 0f)
						{
							num7++;
						}
						if (num9 > num5)
						{
							num6 = controlStartIndex;
							bindingIndex = controlIndexToBindingIndex[controlStartIndex];
							num5 = num9;
						}
					}
					else
					{
						if (ptr2->isPartOfComposite)
						{
							continue;
						}
						for (int j = 0; j < ptr2->controlCount; j++)
						{
							int num10 = ptr2->controlStartIndex + j;
							float num11 = memory.controlMagnitudes[num10];
							if (num11 > 0f)
							{
								num7++;
							}
							if (num11 > num5)
							{
								num6 = num10;
								bindingIndex = num8;
								num5 = num11;
							}
						}
					}
				}
				if (num7 <= 1)
				{
					ptr->hasMultipleConcurrentActuations = false;
				}
				if (num6 != -1)
				{
					trigger.controlIndex = num6;
					trigger.bindingIndex = bindingIndex;
					trigger.magnitude = num5;
					return false;
				}
			}
			if (Mathf.Approximately(trigger.magnitude, ptr->magnitude))
			{
				if (bindingStates[trigger.bindingIndex].isPartOfComposite && num == num2)
				{
					return false;
				}
				if (trigger.magnitude > 0f && num != ptr->controlIndex)
				{
					ptr->hasMultipleConcurrentActuations = true;
				}
				return true;
			}
			return false;
		}

		private unsafe void ProcessDefaultInteraction(ref TriggerState trigger, int actionIndex)
		{
			switch (actionStates[actionIndex].phase)
			{
			case InputActionPhase.Waiting:
				if (trigger.isPassThrough)
				{
					ChangePhaseOfAction(InputActionPhase.Performed, ref trigger);
				}
				else if (IsActuated(ref trigger))
				{
					ChangePhaseOfAction(InputActionPhase.Started, ref trigger);
					ChangePhaseOfAction(InputActionPhase.Performed, ref trigger, InputActionPhase.Started);
				}
				break;
			case InputActionPhase.Started:
				if (!IsActuated(ref trigger))
				{
					ChangePhaseOfAction(InputActionPhase.Canceled, ref trigger);
				}
				else
				{
					ChangePhaseOfAction(InputActionPhase.Performed, ref trigger, InputActionPhase.Started);
				}
				break;
			case InputActionPhase.Performed:
				ChangePhaseOfAction(InputActionPhase.Performed, ref trigger, InputActionPhase.Performed);
				break;
			}
		}

		private unsafe void ProcessInteractions(ref TriggerState trigger, int interactionStartIndex, int interactionCount)
		{
			InputInteractionContext inputInteractionContext = default(InputInteractionContext);
			inputInteractionContext.m_State = this;
			inputInteractionContext.m_TriggerState = trigger;
			InputInteractionContext context = inputInteractionContext;
			for (int i = 0; i < interactionCount; i++)
			{
				int num = interactionStartIndex + i;
				InteractionState interactionState = interactionStates[num];
				IInputInteraction obj = interactions[num];
				context.m_TriggerState.phase = interactionState.phase;
				context.m_TriggerState.startTime = interactionState.startTime;
				context.m_TriggerState.interactionIndex = num;
				obj.Process(ref context);
			}
		}

		private unsafe void ProcessTimeout(double time, int mapIndex, int controlIndex, int bindingIndex, int interactionIndex)
		{
			InteractionState interactionState = interactionStates[interactionIndex];
			InputInteractionContext inputInteractionContext = default(InputInteractionContext);
			inputInteractionContext.m_State = this;
			inputInteractionContext.m_TriggerState = new TriggerState
			{
				phase = interactionState.phase,
				time = time,
				mapIndex = mapIndex,
				controlIndex = controlIndex,
				bindingIndex = bindingIndex,
				interactionIndex = interactionIndex,
				startTime = interactionState.startTime
			};
			inputInteractionContext.timerHasExpired = true;
			InputInteractionContext context = inputInteractionContext;
			interactionState.isTimerRunning = false;
			interactionStates[interactionIndex] = interactionState;
			interactions[interactionIndex].Process(ref context);
		}

		internal unsafe void StartTimeout(float seconds, ref TriggerState trigger)
		{
			InputManager s_Manager = InputSystem.s_Manager;
			double time = trigger.time;
			InputControl control = controls[trigger.controlIndex];
			int interactionIndex = trigger.interactionIndex;
			long monitorIndex = ToCombinedMapAndControlAndBindingIndex(trigger.mapIndex, trigger.controlIndex, trigger.bindingIndex);
			if (interactionStates[interactionIndex].isTimerRunning)
			{
				StopTimeout(trigger.mapIndex, trigger.controlIndex, trigger.bindingIndex, interactionIndex);
			}
			s_Manager.AddStateChangeMonitorTimeout(control, this, time + (double)seconds, monitorIndex, interactionIndex);
			InteractionState interactionState = interactionStates[interactionIndex];
			interactionState.isTimerRunning = true;
			interactionStates[interactionIndex] = interactionState;
		}

		private unsafe void StopTimeout(int mapIndex, int controlIndex, int bindingIndex, int interactionIndex)
		{
			InputManager s_Manager = InputSystem.s_Manager;
			long monitorIndex = ToCombinedMapAndControlAndBindingIndex(mapIndex, controlIndex, bindingIndex);
			s_Manager.RemoveStateChangeMonitorTimeout(this, monitorIndex, interactionIndex);
			InteractionState interactionState = interactionStates[interactionIndex];
			interactionState.isTimerRunning = false;
			interactionStates[interactionIndex] = interactionState;
		}

		internal unsafe void ChangePhaseOfInteraction(InputActionPhase newPhase, ref TriggerState trigger, InputActionPhase phaseAfterPerformed = InputActionPhase.Waiting)
		{
			int interactionIndex = trigger.interactionIndex;
			int bindingIndex = trigger.bindingIndex;
			InputActionPhase phaseAfterPerformedOrCanceled = InputActionPhase.Waiting;
			if (newPhase == InputActionPhase.Performed)
			{
				phaseAfterPerformedOrCanceled = phaseAfterPerformed;
			}
			if (interactionStates[interactionIndex].isTimerRunning)
			{
				StopTimeout(trigger.mapIndex, trigger.controlIndex, trigger.bindingIndex, trigger.interactionIndex);
			}
			interactionStates[interactionIndex].phase = newPhase;
			interactionStates[interactionIndex].triggerControlIndex = trigger.controlIndex;
			interactionStates[interactionIndex].startTime = trigger.startTime;
			int actionIndex = bindingStates[bindingIndex].actionIndex;
			if (actionIndex != -1)
			{
				if (actionStates[actionIndex].phase == InputActionPhase.Waiting)
				{
					ChangePhaseOfAction(newPhase, ref trigger, phaseAfterPerformedOrCanceled);
				}
				else if (newPhase == InputActionPhase.Canceled && actionStates[actionIndex].interactionIndex == trigger.interactionIndex)
				{
					ChangePhaseOfAction(newPhase, ref trigger);
					int interactionStartIndex = bindingStates[bindingIndex].interactionStartIndex;
					int interactionCount = bindingStates[bindingIndex].interactionCount;
					for (int i = 0; i < interactionCount; i++)
					{
						int num = interactionStartIndex + i;
						if (num != trigger.interactionIndex && interactionStates[num].phase == InputActionPhase.Started)
						{
							double startTime = interactionStates[num].startTime;
							TriggerState triggerState = default(TriggerState);
							triggerState.phase = InputActionPhase.Started;
							triggerState.controlIndex = interactionStates[num].triggerControlIndex;
							triggerState.bindingIndex = trigger.bindingIndex;
							triggerState.interactionIndex = num;
							triggerState.time = startTime;
							triggerState.startTime = startTime;
							TriggerState trigger2 = triggerState;
							ChangePhaseOfAction(InputActionPhase.Started, ref trigger2);
							break;
						}
					}
				}
				else if (actionStates[actionIndex].interactionIndex == trigger.interactionIndex)
				{
					ChangePhaseOfAction(newPhase, ref trigger, phaseAfterPerformedOrCanceled);
					if (newPhase == InputActionPhase.Performed)
					{
						int interactionStartIndex2 = bindingStates[bindingIndex].interactionStartIndex;
						int interactionCount2 = bindingStates[bindingIndex].interactionCount;
						for (int j = 0; j < interactionCount2; j++)
						{
							int num2 = interactionStartIndex2 + j;
							if (num2 != trigger.interactionIndex)
							{
								ResetInteractionState(trigger.mapIndex, trigger.bindingIndex, num2);
							}
						}
					}
				}
			}
			if (newPhase == InputActionPhase.Performed && phaseAfterPerformed != InputActionPhase.Waiting)
			{
				interactionStates[interactionIndex].phase = phaseAfterPerformed;
			}
			else if (newPhase == InputActionPhase.Performed || newPhase == InputActionPhase.Canceled)
			{
				ResetInteractionState(trigger.mapIndex, trigger.bindingIndex, trigger.interactionIndex);
			}
		}

		private unsafe void ChangePhaseOfAction(InputActionPhase newPhase, ref TriggerState trigger, InputActionPhase phaseAfterPerformedOrCanceled = InputActionPhase.Waiting)
		{
			int actionIndex = bindingStates[trigger.bindingIndex].actionIndex;
			if (actionIndex == -1)
			{
				return;
			}
			TriggerState* ptr = actionStates + actionIndex;
			if (ptr->isDisabled)
			{
				return;
			}
			if (ptr->isPassThrough && trigger.interactionIndex == -1)
			{
				ChangePhaseOfActionInternal(actionIndex, ptr, newPhase, ref trigger);
				if (ptr->isDisabled)
				{
					return;
				}
			}
			else if (newPhase == InputActionPhase.Performed && ptr->phase == InputActionPhase.Waiting)
			{
				ChangePhaseOfActionInternal(actionIndex, ptr, InputActionPhase.Started, ref trigger);
				if (ptr->isDisabled)
				{
					return;
				}
				ChangePhaseOfActionInternal(actionIndex, ptr, newPhase, ref trigger);
				if (ptr->isDisabled)
				{
					return;
				}
				if (phaseAfterPerformedOrCanceled == InputActionPhase.Waiting)
				{
					ChangePhaseOfActionInternal(actionIndex, ptr, InputActionPhase.Canceled, ref trigger);
				}
				if (ptr->isDisabled)
				{
					return;
				}
				ptr->phase = phaseAfterPerformedOrCanceled;
			}
			else
			{
				ChangePhaseOfActionInternal(actionIndex, ptr, newPhase, ref trigger);
				if (ptr->isDisabled)
				{
					return;
				}
				if (newPhase == InputActionPhase.Performed || newPhase == InputActionPhase.Canceled)
				{
					ptr->phase = phaseAfterPerformedOrCanceled;
				}
			}
			if (ptr->phase == InputActionPhase.Waiting)
			{
				ptr->controlIndex = -1;
				ptr->flags &= ~TriggerState.Flags.HaveMagnitude;
			}
		}

		private unsafe void ChangePhaseOfActionInternal(int actionIndex, TriggerState* actionState, InputActionPhase newPhase, ref TriggerState trigger)
		{
			TriggerState triggerState = trigger;
			triggerState.flags = actionState->flags;
			triggerState.phase = newPhase;
			if (!triggerState.haveMagnitude)
			{
				triggerState.magnitude = ComputeMagnitude(trigger.bindingIndex, trigger.controlIndex);
			}
			if (newPhase == InputActionPhase.Performed)
			{
				triggerState.lastTriggeredInUpdate = InputUpdate.s_UpdateStepCount;
			}
			else
			{
				triggerState.lastTriggeredInUpdate = actionState->lastTriggeredInUpdate;
			}
			if (newPhase == InputActionPhase.Started)
			{
				triggerState.startTime = triggerState.time;
			}
			*actionState = triggerState;
			InputActionMap inputActionMap = maps[trigger.mapIndex];
			InputAction inputAction = inputActionMap.m_Actions[actionIndex - mapIndices[trigger.mapIndex].actionStartIndex];
			trigger.phase = newPhase;
			switch (newPhase)
			{
			case InputActionPhase.Started:
				CallActionListeners(actionIndex, inputActionMap, newPhase, ref inputAction.m_OnStarted, "started");
				break;
			case InputActionPhase.Performed:
				CallActionListeners(actionIndex, inputActionMap, newPhase, ref inputAction.m_OnPerformed, "performed");
				break;
			case InputActionPhase.Canceled:
				CallActionListeners(actionIndex, inputActionMap, newPhase, ref inputAction.m_OnCanceled, "canceled");
				break;
			}
		}

		private void CallActionListeners(int actionIndex, InputActionMap actionMap, InputActionPhase phase, ref InlinedArray<Action<InputAction.CallbackContext>> listeners, string callbackName)
		{
			InlinedArray<Action<InputAction.CallbackContext>> callbacks = actionMap.m_ActionCallbacks;
			if (listeners.length == 0 && callbacks.length == 0 && s_OnActionChange.length == 0)
			{
				return;
			}
			InputAction.CallbackContext callbackContext = default(InputAction.CallbackContext);
			callbackContext.m_State = this;
			callbackContext.m_ActionIndex = actionIndex;
			InputAction.CallbackContext argument = callbackContext;
			InputAction action = argument.action;
			if (s_OnActionChange.length > 0)
			{
				InputActionChange arg;
				switch (phase)
				{
				default:
					return;
				case InputActionPhase.Started:
					arg = InputActionChange.ActionStarted;
					break;
				case InputActionPhase.Performed:
					arg = InputActionChange.ActionPerformed;
					break;
				case InputActionPhase.Canceled:
					arg = InputActionChange.ActionCanceled;
					break;
				}
				for (int i = 0; i < s_OnActionChange.length; i++)
				{
					s_OnActionChange[i](action, arg);
				}
			}
			DelegateHelpers.InvokeCallbacksSafe(ref listeners, argument, callbackName, action);
			DelegateHelpers.InvokeCallbacksSafe(ref callbacks, argument, callbackName, actionMap);
		}

		private object GetActionOrNoneString(ref TriggerState trigger)
		{
			InputAction actionOrNull = GetActionOrNull(ref trigger);
			if (actionOrNull == null)
			{
				return "<none>";
			}
			return actionOrNull;
		}

		internal unsafe InputAction GetActionOrNull(int bindingIndex)
		{
			int actionIndex = bindingStates[bindingIndex].actionIndex;
			if (actionIndex == -1)
			{
				return null;
			}
			int mapIndex = bindingStates[bindingIndex].mapIndex;
			int actionStartIndex = mapIndices[mapIndex].actionStartIndex;
			return maps[mapIndex].m_Actions[actionIndex - actionStartIndex];
		}

		internal unsafe InputAction GetActionOrNull(ref TriggerState trigger)
		{
			int actionIndex = bindingStates[trigger.bindingIndex].actionIndex;
			if (actionIndex == -1)
			{
				return null;
			}
			int actionStartIndex = mapIndices[trigger.mapIndex].actionStartIndex;
			return maps[trigger.mapIndex].m_Actions[actionIndex - actionStartIndex];
		}

		internal InputControl GetControl(ref TriggerState trigger)
		{
			return controls[trigger.controlIndex];
		}

		private IInputInteraction GetInteractionOrNull(ref TriggerState trigger)
		{
			if (trigger.interactionIndex == -1)
			{
				return null;
			}
			return interactions[trigger.interactionIndex];
		}

		internal unsafe int GetBindingIndexInMap(int bindingIndex)
		{
			int mapIndex = bindingStates[bindingIndex].mapIndex;
			int bindingStartIndex = mapIndices[mapIndex].bindingStartIndex;
			return bindingIndex - bindingStartIndex;
		}

		internal unsafe int GetBindingIndexInState(int mapIndex, int bindingIndexInMap)
		{
			return mapIndices[mapIndex].bindingStartIndex + bindingIndexInMap;
		}

		internal unsafe InputBinding GetBinding(int bindingIndex)
		{
			int mapIndex = bindingStates[bindingIndex].mapIndex;
			int bindingStartIndex = mapIndices[mapIndex].bindingStartIndex;
			return maps[mapIndex].m_Bindings[bindingIndex - bindingStartIndex];
		}

		private unsafe void ResetInteractionStateAndCancelIfNecessary(int mapIndex, int bindingIndex, int interactionIndex)
		{
			int actionIndex = bindingStates[bindingIndex].actionIndex;
			if (actionStates[actionIndex].interactionIndex == interactionIndex)
			{
				InputActionPhase phase = interactionStates[interactionIndex].phase;
				if ((uint)(phase - 2) <= 1u)
				{
					ChangePhaseOfInteraction(InputActionPhase.Canceled, ref actionStates[actionIndex]);
				}
			}
			ResetInteractionState(mapIndex, bindingIndex, interactionIndex);
		}

		private unsafe void ResetInteractionState(int mapIndex, int bindingIndex, int interactionIndex)
		{
			interactions[interactionIndex].Reset();
			if (interactionStates[interactionIndex].isTimerRunning)
			{
				int triggerControlIndex = interactionStates[interactionIndex].triggerControlIndex;
				StopTimeout(mapIndex, triggerControlIndex, bindingIndex, interactionIndex);
			}
			interactionStates[interactionIndex] = new InteractionState
			{
				phase = InputActionPhase.Waiting
			};
		}

		internal unsafe int GetValueSizeInBytes(int bindingIndex, int controlIndex)
		{
			if (bindingStates[bindingIndex].isPartOfComposite)
			{
				int compositeOrCompositeBindingIndex = bindingStates[bindingIndex].compositeOrCompositeBindingIndex;
				int compositeOrCompositeBindingIndex2 = bindingStates[compositeOrCompositeBindingIndex].compositeOrCompositeBindingIndex;
				return composites[compositeOrCompositeBindingIndex2].valueSizeInBytes;
			}
			return controls[controlIndex].valueSizeInBytes;
		}

		internal unsafe Type GetValueType(int bindingIndex, int controlIndex)
		{
			if (bindingStates[bindingIndex].isPartOfComposite)
			{
				int compositeOrCompositeBindingIndex = bindingStates[bindingIndex].compositeOrCompositeBindingIndex;
				int compositeOrCompositeBindingIndex2 = bindingStates[compositeOrCompositeBindingIndex].compositeOrCompositeBindingIndex;
				return composites[compositeOrCompositeBindingIndex2].valueType;
			}
			return controls[controlIndex].valueType;
		}

		internal bool IsActuated(ref TriggerState trigger, float threshold = 0f)
		{
			if (!trigger.haveMagnitude)
			{
				trigger.magnitude = ComputeMagnitude(trigger.bindingIndex, trigger.controlIndex);
			}
			if (trigger.magnitude < 0f)
			{
				return true;
			}
			if (Mathf.Approximately(threshold, 0f))
			{
				return trigger.magnitude > 0f;
			}
			return trigger.magnitude >= threshold;
		}

		private unsafe float ComputeMagnitude(int bindingIndex, int controlIndex)
		{
			if (bindingStates[bindingIndex].isPartOfComposite)
			{
				int compositeOrCompositeBindingIndex = bindingStates[bindingIndex].compositeOrCompositeBindingIndex;
				int compositeOrCompositeBindingIndex2 = bindingStates[compositeOrCompositeBindingIndex].compositeOrCompositeBindingIndex;
				InputBindingComposite obj = composites[compositeOrCompositeBindingIndex2];
				InputBindingCompositeContext context = new InputBindingCompositeContext
				{
					m_State = this,
					m_BindingIndex = compositeOrCompositeBindingIndex
				};
				return obj.EvaluateMagnitude(ref context);
			}
			InputControl inputControl = controls[controlIndex];
			if (inputControl.CheckStateIsAtDefault())
			{
				return 0f;
			}
			return inputControl.EvaluateMagnitude();
		}

		internal unsafe void ReadValue(int bindingIndex, int controlIndex, void* buffer, int bufferSize)
		{
			InputControl control = null;
			if (bindingStates[bindingIndex].isPartOfComposite)
			{
				int compositeOrCompositeBindingIndex = bindingStates[bindingIndex].compositeOrCompositeBindingIndex;
				int compositeOrCompositeBindingIndex2 = bindingStates[compositeOrCompositeBindingIndex].compositeOrCompositeBindingIndex;
				InputBindingComposite obj = composites[compositeOrCompositeBindingIndex2];
				InputBindingCompositeContext context = new InputBindingCompositeContext
				{
					m_State = this,
					m_BindingIndex = compositeOrCompositeBindingIndex
				};
				obj.ReadValue(ref context, buffer, bufferSize);
			}
			else
			{
				control = controls[controlIndex];
				control.ReadValueIntoBuffer(buffer, bufferSize);
			}
			int processorCount = bindingStates[bindingIndex].processorCount;
			if (processorCount > 0)
			{
				int processorStartIndex = bindingStates[bindingIndex].processorStartIndex;
				for (int i = 0; i < processorCount; i++)
				{
					processors[processorStartIndex + i].Process(buffer, bufferSize, control);
				}
			}
		}

		internal unsafe TValue ReadValue<TValue>(int bindingIndex, int controlIndex, bool ignoreComposites = false) where TValue : struct
		{
			TValue val = default(TValue);
			InputControl<TValue> inputControl = null;
			if (!ignoreComposites && bindingStates[bindingIndex].isPartOfComposite)
			{
				int compositeOrCompositeBindingIndex = bindingStates[bindingIndex].compositeOrCompositeBindingIndex;
				int compositeOrCompositeBindingIndex2 = bindingStates[compositeOrCompositeBindingIndex].compositeOrCompositeBindingIndex;
				InputBindingComposite inputBindingComposite = composites[compositeOrCompositeBindingIndex2];
				InputBindingComposite<TValue> inputBindingComposite2 = inputBindingComposite as InputBindingComposite<TValue>;
				if (inputBindingComposite2 == null)
				{
					Type type = inputBindingComposite.GetType();
					while (type != null && !type.IsGenericType)
					{
						type = type.BaseType;
					}
					throw new InvalidOperationException(string.Format("Cannot read value of type '{0}' from composite '{1}' bound to action '{2}' (composite is a '{3}' with value type '{4}')", typeof(TValue).Name, inputBindingComposite, GetActionOrNull(bindingIndex), compositeOrCompositeBindingIndex2.GetType().Name, TypeHelpers.GetNiceTypeName(type.GetGenericArguments()[0])));
				}
				InputBindingCompositeContext inputBindingCompositeContext = default(InputBindingCompositeContext);
				inputBindingCompositeContext.m_State = this;
				inputBindingCompositeContext.m_BindingIndex = compositeOrCompositeBindingIndex;
				InputBindingCompositeContext context = inputBindingCompositeContext;
				val = inputBindingComposite2.ReadValue(ref context);
				bindingIndex = compositeOrCompositeBindingIndex;
			}
			else
			{
				InputControl inputControl2 = controls[controlIndex];
				inputControl = inputControl2 as InputControl<TValue>;
				if (inputControl == null)
				{
					throw new InvalidOperationException(string.Format("Cannot read value of type '{0}' from control '{1}' bound to action '{2}' (control is a '{3}' with value type '{4}')", TypeHelpers.GetNiceTypeName(typeof(TValue)), inputControl2.path, GetActionOrNull(bindingIndex), inputControl2.GetType().Name, TypeHelpers.GetNiceTypeName(inputControl2.valueType)));
				}
				val = inputControl.ReadValue();
			}
			int processorCount = bindingStates[bindingIndex].processorCount;
			if (processorCount > 0)
			{
				int processorStartIndex = bindingStates[bindingIndex].processorStartIndex;
				for (int i = 0; i < processorCount; i++)
				{
					InputProcessor<TValue> inputProcessor;
					if ((inputProcessor = processors[processorStartIndex + i] as InputProcessor<TValue>) != null)
					{
						val = inputProcessor.Process(val, inputControl);
					}
				}
			}
			return val;
		}

		internal unsafe TValue ReadCompositePartValue<TValue, TComparer>(int bindingIndex, int partNumber, bool* buttonValuePtr, out int controlIndex, TComparer comparer = default(TComparer)) where TValue : struct where TComparer : IComparer<TValue>
		{
			TValue val = default(TValue);
			int num = bindingIndex + 1;
			bool flag = true;
			controlIndex = -1;
			for (int i = num; i < totalBindingCount && bindingStates[i].isPartOfComposite; i++)
			{
				if (bindingStates[i].partIndex != partNumber)
				{
					continue;
				}
				int controlCount = bindingStates[i].controlCount;
				int controlStartIndex = bindingStates[i].controlStartIndex;
				for (int j = 0; j < controlCount; j++)
				{
					int num2 = controlStartIndex + j;
					TValue output = ReadValue<TValue>(i, num2, true);
					if (flag)
					{
						val = output;
						controlIndex = num2;
						flag = false;
					}
					else if (comparer.Compare(output, val) > 0)
					{
						val = output;
						controlIndex = num2;
					}
					if (buttonValuePtr != null && controlIndex == num2)
					{
						InputControl inputControl = controls[num2];
						ButtonControl buttonControl;
						if ((buttonControl = inputControl as ButtonControl) != null)
						{
							*buttonValuePtr = buttonControl.isPressed;
						}
						else if (inputControl is InputControl<float>)
						{
							void* ptr = UnsafeUtility.AddressOf(ref output);
							*buttonValuePtr = *(float*)ptr >= ButtonControl.s_GlobalDefaultButtonPressPoint;
						}
					}
				}
			}
			return val;
		}

		internal unsafe object ReadValueAsObject(int bindingIndex, int controlIndex)
		{
			InputControl control = null;
			object obj2;
			if (bindingStates[bindingIndex].isPartOfComposite)
			{
				int compositeOrCompositeBindingIndex = bindingStates[bindingIndex].compositeOrCompositeBindingIndex;
				int compositeOrCompositeBindingIndex2 = bindingStates[compositeOrCompositeBindingIndex].compositeOrCompositeBindingIndex;
				InputBindingComposite obj = composites[compositeOrCompositeBindingIndex2];
				InputBindingCompositeContext context = new InputBindingCompositeContext
				{
					m_State = this,
					m_BindingIndex = compositeOrCompositeBindingIndex
				};
				obj2 = obj.ReadValueAsObject(ref context);
			}
			else
			{
				control = controls[controlIndex];
				obj2 = control.ReadValueAsObject();
			}
			int processorCount = bindingStates[bindingIndex].processorCount;
			if (processorCount > 0)
			{
				int processorStartIndex = bindingStates[bindingIndex].processorStartIndex;
				for (int i = 0; i < processorCount; i++)
				{
					obj2 = processors[processorStartIndex + i].ProcessAsObject(obj2, control);
				}
			}
			return obj2;
		}

		internal unsafe bool ReadValueAsButton(int bindingIndex, int controlIndex)
		{
			ButtonControl buttonControl = null;
			if (!bindingStates[bindingIndex].isPartOfComposite)
			{
				buttonControl = controls[controlIndex] as ButtonControl;
			}
			float num = ReadValue<float>(bindingIndex, controlIndex);
			if (buttonControl != null)
			{
				return num >= buttonControl.pressPointOrDefault;
			}
			return num >= ButtonControl.s_GlobalDefaultButtonPressPoint;
		}

		private void AddToGlobaList()
		{
			CompactGlobalList();
			GCHandle value = GCHandle.Alloc(this, GCHandleType.Weak);
			s_GlobalList.AppendWithCapacity(value);
		}

		private void RemoveMapFromGlobalList()
		{
			int length = s_GlobalList.length;
			for (int i = 0; i < length; i++)
			{
				if (s_GlobalList[i].Target == this)
				{
					s_GlobalList[i].Free();
					s_GlobalList.RemoveAtByMovingTailWithCapacity(i);
					break;
				}
			}
		}

		private static void CompactGlobalList()
		{
			int length = s_GlobalList.length;
			int num = 0;
			for (int i = 0; i < length; i++)
			{
				GCHandle value = s_GlobalList[i];
				if (value.IsAllocated && value.Target != null)
				{
					if (num != i)
					{
						s_GlobalList[num] = value;
					}
					num++;
				}
				else
				{
					if (value.IsAllocated)
					{
						s_GlobalList[i].Free();
					}
					s_GlobalList[i] = default(GCHandle);
				}
			}
			s_GlobalList.length = num;
		}

		internal static void NotifyListenersOfActionChange(InputActionChange change, object actionOrMapOrAsset)
		{
			DelegateHelpers.InvokeCallbacksSafe(ref s_OnActionChange, actionOrMapOrAsset, change, "onActionChange");
			if (change == InputActionChange.BoundControlsChanged)
			{
				DelegateHelpers.InvokeCallbacksSafe(ref s_OnActionControlsChanged, actionOrMapOrAsset, "onActionControlsChange");
			}
		}

		internal static void ResetGlobals()
		{
			DestroyAllActionMapStates();
			for (int i = 0; i < s_GlobalList.length; i++)
			{
				if (s_GlobalList[i].IsAllocated)
				{
					s_GlobalList[i].Free();
				}
			}
			s_GlobalList.length = 0;
			s_OnActionChange.Clear();
			s_OnActionControlsChanged.Clear();
		}

		internal unsafe static int FindAllEnabledActions(List<InputAction> result)
		{
			int num = 0;
			int length = s_GlobalList.length;
			for (int i = 0; i < length; i++)
			{
				GCHandle gCHandle = s_GlobalList[i];
				if (!gCHandle.IsAllocated)
				{
					continue;
				}
				InputActionState inputActionState = (InputActionState)gCHandle.Target;
				if (inputActionState == null)
				{
					continue;
				}
				int num2 = inputActionState.totalMapCount;
				InputActionMap[] array = inputActionState.maps;
				for (int j = 0; j < num2; j++)
				{
					InputActionMap inputActionMap = array[j];
					if (!inputActionMap.enabled)
					{
						continue;
					}
					InputAction[] actions = inputActionMap.m_Actions;
					int num3 = actions.Length;
					if (inputActionMap.m_EnabledActionsCount == num3)
					{
						result.AddRange(actions);
						num += num3;
						continue;
					}
					int actionStartIndex = inputActionState.mapIndices[inputActionMap.m_MapIndexInState].actionStartIndex;
					for (int k = 0; k < num3; k++)
					{
						if (inputActionState.actionStates[actionStartIndex + k].phase != 0)
						{
							result.Add(actions[k]);
							num++;
						}
					}
				}
			}
			return num;
		}

		internal static void OnDeviceChange(InputDevice device, InputDeviceChange change)
		{
			for (int i = 0; i < s_GlobalList.length; i++)
			{
				GCHandle gCHandle = s_GlobalList[i];
				if (!gCHandle.IsAllocated || gCHandle.Target == null)
				{
					if (gCHandle.IsAllocated)
					{
						s_GlobalList[i].Free();
					}
					s_GlobalList.RemoveAtWithCapacity(i);
					i--;
					continue;
				}
				InputActionState inputActionState = (InputActionState)gCHandle.Target;
				if ((change != 0 || inputActionState.CanUseDevice(device)) && (change != InputDeviceChange.Removed || inputActionState.IsUsingDevice(device)) && (change != InputDeviceChange.UsageChanged || inputActionState.IsUsingDevice(device) || inputActionState.CanUseDevice(device)) && (change != InputDeviceChange.ConfigurationChanged || inputActionState.IsUsingDevice(device)))
				{
					for (int j = 0; j < inputActionState.totalMapCount && !inputActionState.maps[j].LazyResolveBindings(); j++)
					{
					}
				}
			}
		}

		internal static void DeferredResolutionOfBindings()
		{
			for (int i = 0; i < s_GlobalList.length; i++)
			{
				GCHandle gCHandle = s_GlobalList[i];
				if (!gCHandle.IsAllocated || gCHandle.Target == null)
				{
					if (gCHandle.IsAllocated)
					{
						s_GlobalList[i].Free();
					}
					s_GlobalList.RemoveAtWithCapacity(i);
					i--;
				}
				else
				{
					InputActionState inputActionState = (InputActionState)gCHandle.Target;
					for (int j = 0; j < inputActionState.totalMapCount; j++)
					{
						inputActionState.maps[j].ResolveBindingsIfNecessary();
					}
				}
			}
		}

		internal static void DisableAllActions()
		{
			for (int i = 0; i < s_GlobalList.length; i++)
			{
				GCHandle gCHandle = s_GlobalList[i];
				if (gCHandle.IsAllocated && gCHandle.Target != null)
				{
					InputActionState obj = (InputActionState)gCHandle.Target;
					int num = obj.totalMapCount;
					InputActionMap[] array = obj.maps;
					for (int j = 0; j < num; j++)
					{
						array[j].Disable();
					}
				}
			}
		}

		internal static void DestroyAllActionMapStates()
		{
			while (s_GlobalList.length > 0)
			{
				int index = s_GlobalList.length - 1;
				GCHandle gCHandle = s_GlobalList[index];
				if (!gCHandle.IsAllocated || gCHandle.Target == null)
				{
					if (gCHandle.IsAllocated)
					{
						s_GlobalList[index].Free();
					}
					s_GlobalList.RemoveAtWithCapacity(index);
				}
				else
				{
					((InputActionState)gCHandle.Target).Destroy();
				}
			}
		}
	}
}
