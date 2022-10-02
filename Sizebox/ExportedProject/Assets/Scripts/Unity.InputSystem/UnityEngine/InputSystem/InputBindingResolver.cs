using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Collections;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
	internal struct InputBindingResolver : IDisposable
	{
		public int totalProcessorCount;

		public int totalCompositeCount;

		public int totalInteractionCount;

		public InputActionMap[] maps;

		public InputControl[] controls;

		public InputActionState.UnmanagedMemory memory;

		public IInputInteraction[] interactions;

		public InputProcessor[] processors;

		public InputBindingComposite[] composites;

		public InputBinding? bindingMask;

		private List<NameAndParameters> m_Parameters;

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

		public int totalControlCount
		{
			get
			{
				return memory.controlCount;
			}
		}

		public void Dispose()
		{
			memory.Dispose();
		}

		public void StartWithArraysFrom(InputActionState state)
		{
			maps = state.maps;
			interactions = state.interactions;
			processors = state.processors;
			composites = state.composites;
			controls = state.controls;
			if (maps != null)
			{
				Array.Clear(maps, 0, state.totalMapCount);
			}
			if (interactions != null)
			{
				Array.Clear(interactions, 0, state.totalInteractionCount);
			}
			if (processors != null)
			{
				Array.Clear(processors, 0, state.totalProcessorCount);
			}
			if (composites != null)
			{
				Array.Clear(composites, 0, state.totalCompositeCount);
			}
			if (controls != null)
			{
				Array.Clear(controls, 0, state.totalControlCount);
			}
			state.maps = null;
			state.interactions = null;
			state.processors = null;
			state.composites = null;
			state.controls = null;
		}

		public unsafe void AddActionMap(InputActionMap map)
		{
			InputAction[] actions = map.m_Actions;
			InputBinding[] bindings = map.m_Bindings;
			int num = ((bindings != null) ? bindings.Length : 0);
			int num2 = ((actions != null) ? actions.Length : 0);
			int num3 = totalMapCount;
			int num4 = totalActionCount;
			int num5 = totalBindingCount;
			int controlStartIndex = totalControlCount;
			int num6 = totalInteractionCount;
			int num7 = totalProcessorCount;
			int num8 = totalCompositeCount;
			InputActionState.UnmanagedMemory unmanagedMemory = default(InputActionState.UnmanagedMemory);
			unmanagedMemory.Allocate(totalMapCount + 1, totalActionCount + num2, totalBindingCount + num, interactionCount: totalInteractionCount, compositeCount: totalCompositeCount, controlCount: totalControlCount);
			if (memory.isAllocated)
			{
				unmanagedMemory.CopyDataFrom(memory);
			}
			int num9 = -1;
			int num10 = -1;
			int currentCompositePartCount = 0;
			int num11 = -1;
			InputAction inputAction = null;
			InputBinding? inputBinding = map.m_BindingMask;
			ReadOnlyArray<InputDevice>? devices = map.devices;
			InputControlList<InputControl> matches = new InputControlList<InputControl>(Allocator.Temp);
			try
			{
				for (int i = 0; i < num; i++)
				{
					InputActionState.BindingState* bindingStates = unmanagedMemory.bindingStates;
					ref InputBinding reference = ref bindings[i];
					int num12 = num5 + i;
					bool isComposite = reference.isComposite;
					bool flag = !isComposite && reference.isPartOfComposite;
					InputActionState.BindingState* ptr = bindingStates + num12;
					try
					{
						int controlStartIndex2 = 0;
						int num13 = -1;
						int num14 = -1;
						int actionIndex = -1;
						int partIndex = -1;
						int num15 = 0;
						int num16 = 0;
						int num17 = 0;
						if (flag && num9 == -1)
						{
							throw new InvalidOperationException(string.Format("Binding '{0}' is marked as being part of a composite but the preceding binding is not a composite", reference));
						}
						int num18 = -1;
						string action = reference.action;
						InputAction inputAction2 = null;
						if (!flag)
						{
							if (!string.IsNullOrEmpty(action))
							{
								num18 = map.FindActionIndex(action);
							}
							else if (map.m_SingletonAction != null)
							{
								num18 = 0;
							}
							if (num18 != -1)
							{
								inputAction2 = actions[num18];
							}
						}
						else
						{
							num18 = num11;
							inputAction2 = inputAction;
						}
						if (isComposite)
						{
							num9 = num12;
							inputAction = inputAction2;
							num11 = num18;
						}
						string effectivePath = reference.effectivePath;
						if (!string.IsNullOrEmpty(effectivePath) && inputAction2 != null && (isComposite || !bindingMask.HasValue || bindingMask.Value.Matches(ref reference, InputBinding.MatchOptions.EmptyGroupMatchesAny)) && (isComposite || !inputBinding.HasValue || inputBinding.Value.Matches(ref reference, InputBinding.MatchOptions.EmptyGroupMatchesAny)) && (isComposite || inputAction2 == null || !inputAction2.m_BindingMask.HasValue || inputAction2.m_BindingMask.Value.Matches(ref reference, InputBinding.MatchOptions.EmptyGroupMatchesAny)))
						{
							string effectiveProcessors = reference.effectiveProcessors;
							if (!string.IsNullOrEmpty(effectiveProcessors))
							{
								num14 = ResolveProcessors(effectiveProcessors);
								if (num14 != -1)
								{
									num17 = totalProcessorCount - num14;
								}
							}
							if (!string.IsNullOrEmpty(inputAction2.m_Processors))
							{
								int num19 = ResolveProcessors(inputAction2.m_Processors);
								if (num19 != -1)
								{
									if (num14 == -1)
									{
										num14 = num19;
									}
									num17 += totalProcessorCount - num19;
								}
							}
							string effectiveInteractions = reference.effectiveInteractions;
							if (!string.IsNullOrEmpty(effectiveInteractions))
							{
								num13 = ResolveInteractions(effectiveInteractions);
								if (num13 != -1)
								{
									num16 = totalInteractionCount - num13;
								}
							}
							if (!string.IsNullOrEmpty(inputAction2.m_Interactions))
							{
								int num20 = ResolveInteractions(inputAction2.m_Interactions);
								if (num20 != -1)
								{
									if (num13 == -1)
									{
										num13 = num20;
									}
									num16 += totalInteractionCount - num20;
								}
							}
							if (isComposite)
							{
								InputBindingComposite value = InstantiateBindingComposite(reference.path);
								num10 = ArrayHelpers.AppendWithCapacity(ref composites, ref totalCompositeCount, value);
								controlStartIndex2 = memory.controlCount + matches.Count;
							}
							else
							{
								if (!flag && num9 != -1)
								{
									currentCompositePartCount = 0;
									num9 = -1;
									num10 = -1;
									inputAction = null;
									num11 = -1;
								}
								controlStartIndex2 = memory.controlCount + matches.Count;
								if (devices.HasValue)
								{
									ReadOnlyArray<InputDevice> value2 = devices.Value;
									for (int j = 0; j < value2.Count; j++)
									{
										InputDevice inputDevice = value2[j];
										if (inputDevice.added)
										{
											num15 += InputControlPath.TryFindControls(inputDevice, effectivePath, 0, ref matches);
										}
									}
								}
								else
								{
									num15 = InputSystem.FindControls(effectivePath, ref matches);
								}
							}
						}
						if (flag && num9 != -1 && num15 > 0)
						{
							if (string.IsNullOrEmpty(reference.name))
							{
								throw new InvalidOperationException(string.Format("Binding '{0}' that is part of composite '{1}' is missing a name", reference, composites[num10]));
							}
							partIndex = AssignCompositePartIndex(composites[num10], reference.name, ref currentCompositePartCount);
							bindingStates[num9].controlCount += num15;
							actionIndex = bindingStates[num9].actionIndex;
						}
						else if (num18 != -1)
						{
							actionIndex = num4 + num18;
						}
						*ptr = new InputActionState.BindingState
						{
							controlStartIndex = controlStartIndex2,
							controlCount = num15,
							interactionStartIndex = num13,
							interactionCount = num16,
							processorStartIndex = num14,
							processorCount = num17,
							isComposite = isComposite,
							isPartOfComposite = reference.isPartOfComposite,
							partIndex = partIndex,
							actionIndex = actionIndex,
							compositeOrCompositeBindingIndex = (isComposite ? num10 : num9),
							mapIndex = totalMapCount,
							wantsInitialStateCheck = (inputAction2 != null && inputAction2.wantsInitialStateCheck)
						};
					}
					catch (Exception ex)
					{
						Debug.LogError(string.Format("{0} while resolving binding '{1}' in action map '{2}'", ex.GetType().Name, reference, map));
						Debug.LogException(ex);
						if (ex.IsExceptionIndicatingBugInCode())
						{
							throw;
						}
					}
				}
				int count = matches.Count;
				int num21 = memory.controlCount + count;
				if (unmanagedMemory.interactionCount != totalInteractionCount || unmanagedMemory.compositeCount != totalCompositeCount || unmanagedMemory.controlCount != num21)
				{
					InputActionState.UnmanagedMemory unmanagedMemory2 = default(InputActionState.UnmanagedMemory);
					unmanagedMemory2.Allocate(unmanagedMemory.mapCount, unmanagedMemory.actionCount, unmanagedMemory.bindingCount, num21, totalInteractionCount, totalCompositeCount);
					unmanagedMemory2.CopyDataFrom(unmanagedMemory);
					unmanagedMemory.Dispose();
					unmanagedMemory = unmanagedMemory2;
				}
				int length = memory.controlCount;
				ArrayHelpers.AppendListWithCapacity(ref controls, ref length, matches);
				for (int k = 0; k < num; k++)
				{
					InputActionState.BindingState* intPtr = unmanagedMemory.bindingStates + (num5 + k);
					int controlCount = intPtr->controlCount;
					int controlStartIndex3 = intPtr->controlStartIndex;
					for (int l = 0; l < controlCount; l++)
					{
						unmanagedMemory.controlIndexToBindingIndex[controlStartIndex3 + l] = num5 + k;
					}
				}
				for (int m = memory.interactionCount; m < unmanagedMemory.interactionCount; m++)
				{
					unmanagedMemory.interactionStates[m].phase = InputActionPhase.Waiting;
				}
				int num22 = memory.bindingCount;
				for (int n = 0; n < num2; n++)
				{
					InputAction inputAction3 = actions[n];
					int num23 = (inputAction3.m_ActionIndexInState = num4 + n);
					int num24 = num22;
					int num25 = 0;
					int num26 = 0;
					for (int num27 = 0; num27 < num; num27++)
					{
						int num28 = num5 + num27;
						InputActionState.BindingState* ptr2 = unmanagedMemory.bindingStates + num28;
						if (ptr2->actionIndex != num23 || ptr2->isPartOfComposite)
						{
							continue;
						}
						unmanagedMemory.actionBindingIndices[num22] = (ushort)num28;
						num22++;
						num25++;
						if (ptr2->isComposite)
						{
							if (ptr2->controlCount > 0)
							{
								num26++;
							}
						}
						else
						{
							num26 += ptr2->controlCount;
						}
					}
					unmanagedMemory.actionBindingIndicesAndCounts[num23 * 2] = (ushort)num24;
					unmanagedMemory.actionBindingIndicesAndCounts[num23 * 2 + 1] = (ushort)num25;
					bool flag2 = inputAction3.type == InputActionType.PassThrough;
					bool mayNeedConflictResolution = !flag2 && num26 > 1;
					unmanagedMemory.actionStates[num23] = new InputActionState.TriggerState
					{
						phase = InputActionPhase.Disabled,
						mapIndex = num3,
						controlIndex = -1,
						interactionIndex = -1,
						isPassThrough = flag2,
						mayNeedConflictResolution = mayNeedConflictResolution
					};
				}
				unmanagedMemory.mapIndices[num3] = new InputActionState.ActionMapIndices
				{
					actionStartIndex = num4,
					actionCount = num2,
					controlStartIndex = controlStartIndex,
					controlCount = count,
					bindingStartIndex = num5,
					bindingCount = num,
					interactionStartIndex = num6,
					interactionCount = totalInteractionCount - num6,
					processorStartIndex = num7,
					processorCount = totalProcessorCount - num7,
					compositeStartIndex = num8,
					compositeCount = totalCompositeCount - num8
				};
				map.m_MapIndexInState = num3;
				int count2 = memory.mapCount;
				ArrayHelpers.AppendWithCapacity(ref maps, ref count2, map, 4);
				memory.Dispose();
				memory = unmanagedMemory;
			}
			catch (Exception)
			{
				unmanagedMemory.Dispose();
				throw;
			}
			finally
			{
				matches.Dispose();
			}
		}

		private int ResolveInteractions(string interactionString)
		{
			if (!NameAndParameters.ParseMultiple(interactionString, ref m_Parameters))
			{
				return -1;
			}
			int result = totalInteractionCount;
			for (int i = 0; i < m_Parameters.Count; i++)
			{
				Type type = InputInteraction.s_Interactions.LookupTypeRegistration(m_Parameters[i].name);
				if (type == null)
				{
					throw new InvalidOperationException("No interaction with name '" + m_Parameters[i].name + "' (mentioned in '" + interactionString + "') has been registered");
				}
				IInputInteraction inputInteraction;
				if ((inputInteraction = Activator.CreateInstance(type) as IInputInteraction) == null)
				{
					throw new InvalidOperationException("Interaction '" + m_Parameters[i].name + "' (mentioned in '" + interactionString + "') is not an IInputInteraction");
				}
				NamedValue.ApplyAllToObject(inputInteraction, m_Parameters[i].parameters);
				ArrayHelpers.AppendWithCapacity(ref interactions, ref totalInteractionCount, inputInteraction);
			}
			return result;
		}

		private int ResolveProcessors(string processorString)
		{
			if (!NameAndParameters.ParseMultiple(processorString, ref m_Parameters))
			{
				return -1;
			}
			int result = totalProcessorCount;
			for (int i = 0; i < m_Parameters.Count; i++)
			{
				Type type = InputProcessor.s_Processors.LookupTypeRegistration(m_Parameters[i].name);
				if (type == null)
				{
					throw new InvalidOperationException("No processor with name '" + m_Parameters[i].name + "' (mentioned in '" + processorString + "') has been registered");
				}
				InputProcessor inputProcessor;
				if ((inputProcessor = Activator.CreateInstance(type) as InputProcessor) == null)
				{
					throw new InvalidOperationException("Type '" + type.Name + "' registered as processor called '" + m_Parameters[i].name + "' (mentioned in '" + processorString + "') is not an InputProcessor");
				}
				NamedValue.ApplyAllToObject(inputProcessor, m_Parameters[i].parameters);
				ArrayHelpers.AppendWithCapacity(ref processors, ref totalProcessorCount, inputProcessor);
			}
			return result;
		}

		private static InputBindingComposite InstantiateBindingComposite(string nameAndParameters)
		{
			NameAndParameters nameAndParameters2 = NameAndParameters.Parse(nameAndParameters);
			Type type = InputBindingComposite.s_Composites.LookupTypeRegistration(nameAndParameters2.name);
			if (type == null)
			{
				throw new InvalidOperationException("No binding composite with name '" + nameAndParameters2.name + "' has been registered");
			}
			InputBindingComposite inputBindingComposite;
			if ((inputBindingComposite = Activator.CreateInstance(type) as InputBindingComposite) == null)
			{
				throw new InvalidOperationException("Registered type '" + type.Name + "' used for '" + nameAndParameters2.name + "' is not an InputBindingComposite");
			}
			NamedValue.ApplyAllToObject(inputBindingComposite, nameAndParameters2.parameters);
			return inputBindingComposite;
		}

		private static int AssignCompositePartIndex(object composite, string name, ref int currentCompositePartCount)
		{
			Type type = composite.GetType();
			FieldInfo field = type.GetField(name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (field == null)
			{
				throw new InvalidOperationException(string.Format("Cannot find public field '{0}' used as parameter of binding composite '{1}' of type '{2}'", name, composite, type));
			}
			if (field.FieldType != typeof(int))
			{
				throw new InvalidOperationException(string.Format("Field '{0}' used as a parameter of binding composite '{1}' must be of type 'int' but is of type '{2}' instead", name, composite, type.Name));
			}
			int num = (int)field.GetValue(composite);
			if (num == 0)
			{
				num = ++currentCompositePartCount;
				field.SetValue(composite, num);
			}
			return num;
		}
	}
}
