using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
	[Serializable]
	public sealed class InputActionMap : ICloneable, ISerializationCallbackReceiver, IInputActionCollection, IEnumerable<InputAction>, IEnumerable, IDisposable
	{
		[Serializable]
		internal struct BindingJson
		{
			public string name;

			public string id;

			public string path;

			public string interactions;

			public string processors;

			public string groups;

			public string action;

			public bool isComposite;

			public bool isPartOfComposite;

			public InputBinding ToBinding()
			{
				InputBinding result = default(InputBinding);
				result.name = (string.IsNullOrEmpty(name) ? null : name);
				result.m_Id = (string.IsNullOrEmpty(id) ? null : id);
				result.path = (string.IsNullOrEmpty(path) ? null : path);
				result.action = (string.IsNullOrEmpty(action) ? null : action);
				result.interactions = (string.IsNullOrEmpty(interactions) ? null : interactions);
				result.processors = (string.IsNullOrEmpty(processors) ? null : processors);
				result.groups = (string.IsNullOrEmpty(groups) ? null : groups);
				result.isComposite = isComposite;
				result.isPartOfComposite = isPartOfComposite;
				return result;
			}

			public static BindingJson FromBinding(ref InputBinding binding)
			{
				BindingJson result = default(BindingJson);
				result.name = binding.name;
				result.id = binding.m_Id;
				result.path = binding.path;
				result.action = binding.action;
				result.interactions = binding.interactions;
				result.processors = binding.processors;
				result.groups = binding.groups;
				result.isComposite = binding.isComposite;
				result.isPartOfComposite = binding.isPartOfComposite;
				return result;
			}
		}

		[Serializable]
		internal struct ReadActionJson
		{
			public string name;

			public string type;

			public string id;

			public string expectedControlType;

			public string expectedControlLayout;

			public string processors;

			public string interactions;

			public bool passThrough;

			public bool initialStateCheck;

			public BindingJson[] bindings;

			public InputAction ToAction(string actionName = null)
			{
				if (!string.IsNullOrEmpty(expectedControlLayout))
				{
					expectedControlType = expectedControlLayout;
				}
				InputActionType inputActionType = InputActionType.Value;
				if (!string.IsNullOrEmpty(type))
				{
					inputActionType = (InputActionType)Enum.Parse(typeof(InputActionType), type, true);
				}
				else if (passThrough)
				{
					inputActionType = InputActionType.PassThrough;
				}
				else if (initialStateCheck)
				{
					inputActionType = InputActionType.Value;
				}
				else if (!string.IsNullOrEmpty(expectedControlType) && (expectedControlType == "Button" || expectedControlType == "Key"))
				{
					inputActionType = InputActionType.Button;
				}
				return new InputAction(actionName ?? name, inputActionType)
				{
					m_Id = (string.IsNullOrEmpty(id) ? null : id),
					m_ExpectedControlType = ((!string.IsNullOrEmpty(expectedControlType)) ? expectedControlType : null),
					m_Processors = processors,
					m_Interactions = interactions
				};
			}
		}

		[Serializable]
		internal struct WriteActionJson
		{
			public string name;

			public string type;

			public string id;

			public string expectedControlType;

			public string processors;

			public string interactions;

			public static WriteActionJson FromAction(InputAction action)
			{
				WriteActionJson result = default(WriteActionJson);
				result.name = action.m_Name;
				result.type = action.m_Type.ToString();
				result.id = action.m_Id;
				result.expectedControlType = action.m_ExpectedControlType;
				result.processors = action.processors;
				result.interactions = action.interactions;
				return result;
			}
		}

		[Serializable]
		internal struct ReadMapJson
		{
			public string name;

			public string id;

			public ReadActionJson[] actions;

			public BindingJson[] bindings;
		}

		[Serializable]
		internal struct WriteMapJson
		{
			public string name;

			public string id;

			public WriteActionJson[] actions;

			public BindingJson[] bindings;

			public static WriteMapJson FromMap(InputActionMap map)
			{
				WriteActionJson[] array = null;
				BindingJson[] array2 = null;
				InputAction[] array3 = map.m_Actions;
				if (array3 != null)
				{
					int num = array3.Length;
					array = new WriteActionJson[num];
					for (int i = 0; i < num; i++)
					{
						array[i] = WriteActionJson.FromAction(array3[i]);
					}
				}
				InputBinding[] array4 = map.m_Bindings;
				if (array4 != null)
				{
					int num2 = array4.Length;
					array2 = new BindingJson[num2];
					for (int j = 0; j < num2; j++)
					{
						array2[j] = BindingJson.FromBinding(ref array4[j]);
					}
				}
				WriteMapJson result = default(WriteMapJson);
				result.name = map.name;
				result.id = map.id.ToString();
				result.actions = array;
				result.bindings = array2;
				return result;
			}
		}

		[Serializable]
		internal struct WriteFileJson
		{
			public WriteMapJson[] maps;

			public static WriteFileJson FromMap(InputActionMap map)
			{
				WriteFileJson result = default(WriteFileJson);
				result.maps = new WriteMapJson[1] { WriteMapJson.FromMap(map) };
				return result;
			}

			public static WriteFileJson FromMaps(IEnumerable<InputActionMap> maps)
			{
				int num = maps.Count();
				if (num == 0)
				{
					return default(WriteFileJson);
				}
				WriteMapJson[] array = new WriteMapJson[num];
				int num2 = 0;
				foreach (InputActionMap map in maps)
				{
					array[num2++] = WriteMapJson.FromMap(map);
				}
				WriteFileJson result = default(WriteFileJson);
				result.maps = array;
				return result;
			}
		}

		[Serializable]
		internal struct ReadFileJson
		{
			public ReadActionJson[] actions;

			public ReadMapJson[] maps;

			public InputActionMap[] ToMaps()
			{
				List<InputActionMap> list = new List<InputActionMap>();
				List<List<InputAction>> list2 = new List<List<InputAction>>();
				List<List<InputBinding>> list3 = new List<List<InputBinding>>();
				ReadActionJson[] array = actions;
				int num = ((array != null) ? array.Length : 0);
				for (int i = 0; i < num; i++)
				{
					ReadActionJson readActionJson = actions[i];
					if (string.IsNullOrEmpty(readActionJson.name))
					{
						throw new InvalidOperationException(string.Format("Action number {0} has no name", i + 1));
					}
					string text = null;
					string text2 = readActionJson.name;
					int num2 = text2.IndexOf('/');
					if (num2 != -1)
					{
						text = text2.Substring(0, num2);
						text2 = text2.Substring(num2 + 1);
						if (string.IsNullOrEmpty(text2))
						{
							throw new InvalidOperationException("Invalid action name '" + readActionJson.name + "' (missing action name after '/')");
						}
					}
					InputActionMap inputActionMap = null;
					int j;
					for (j = 0; j < list.Count; j++)
					{
						if (string.Compare(list[j].name, text, StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							inputActionMap = list[j];
							break;
						}
					}
					if (inputActionMap == null)
					{
						inputActionMap = new InputActionMap(text);
						j = list.Count;
						list.Add(inputActionMap);
						list2.Add(new List<InputAction>());
						list3.Add(new List<InputBinding>());
					}
					InputAction inputAction = readActionJson.ToAction(text2);
					list2[j].Add(inputAction);
					if (readActionJson.bindings != null)
					{
						List<InputBinding> list4 = list3[j];
						for (int k = 0; k < readActionJson.bindings.Length; k++)
						{
							BindingJson bindingJson = readActionJson.bindings[k];
							InputBinding item = bindingJson.ToBinding();
							item.action = inputAction.m_Name;
							list4.Add(item);
						}
					}
				}
				ReadMapJson[] array2 = maps;
				int num3 = ((array2 != null) ? array2.Length : 0);
				for (int l = 0; l < num3; l++)
				{
					ReadMapJson readMapJson = maps[l];
					string name = readMapJson.name;
					if (string.IsNullOrEmpty(name))
					{
						throw new InvalidOperationException(string.Format("Map number {0} has no name", l + 1));
					}
					InputActionMap inputActionMap2 = null;
					int m;
					for (m = 0; m < list.Count; m++)
					{
						if (string.Compare(list[m].name, name, StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							inputActionMap2 = list[m];
							break;
						}
					}
					if (inputActionMap2 == null)
					{
						inputActionMap2 = new InputActionMap(name)
						{
							m_Id = (string.IsNullOrEmpty(readMapJson.id) ? null : readMapJson.id)
						};
						m = list.Count;
						list.Add(inputActionMap2);
						list2.Add(new List<InputAction>());
						list3.Add(new List<InputBinding>());
					}
					ReadActionJson[] array3 = readMapJson.actions;
					int num4 = ((array3 != null) ? array3.Length : 0);
					for (int n = 0; n < num4; n++)
					{
						ReadActionJson readActionJson2 = readMapJson.actions[n];
						if (string.IsNullOrEmpty(readActionJson2.name))
						{
							throw new InvalidOperationException(string.Format("Action number {0} in map '{1}' has no name", l + 1, name));
						}
						InputAction inputAction2 = readActionJson2.ToAction();
						list2[m].Add(inputAction2);
						if (readActionJson2.bindings != null)
						{
							List<InputBinding> list5 = list3[m];
							for (int num5 = 0; num5 < readActionJson2.bindings.Length; num5++)
							{
								BindingJson bindingJson2 = readActionJson2.bindings[num5];
								InputBinding item2 = bindingJson2.ToBinding();
								item2.action = inputAction2.m_Name;
								list5.Add(item2);
							}
						}
					}
					BindingJson[] bindings = readMapJson.bindings;
					int num6 = ((bindings != null) ? bindings.Length : 0);
					List<InputBinding> list6 = list3[m];
					for (int num7 = 0; num7 < num6; num7++)
					{
						BindingJson bindingJson3 = readMapJson.bindings[num7];
						InputBinding item3 = bindingJson3.ToBinding();
						list6.Add(item3);
					}
				}
				for (int num8 = 0; num8 < list.Count; num8++)
				{
					InputActionMap inputActionMap3 = list[num8];
					InputAction[] array4 = list2[num8].ToArray();
					InputBinding[] bindings2 = list3[num8].ToArray();
					inputActionMap3.m_Actions = array4;
					inputActionMap3.m_Bindings = bindings2;
					for (int num9 = 0; num9 < array4.Length; num9++)
					{
						array4[num9].m_ActionMap = inputActionMap3;
					}
				}
				return list.ToArray();
			}
		}

		[SerializeField]
		internal string m_Name;

		[SerializeField]
		internal string m_Id;

		[SerializeField]
		internal InputActionAsset m_Asset;

		[SerializeField]
		internal InputAction[] m_Actions;

		[SerializeField]
		internal InputBinding[] m_Bindings;

		[NonSerialized]
		private InputBinding[] m_BindingsForEachAction;

		[NonSerialized]
		private InputControl[] m_ControlsForEachAction;

		[NonSerialized]
		internal int m_EnabledActionsCount;

		[NonSerialized]
		internal InputAction m_SingletonAction;

		[NonSerialized]
		internal int m_MapIndexInState = -1;

		[NonSerialized]
		internal InputActionState m_State;

		[NonSerialized]
		private bool m_NeedToResolveBindings;

		[NonSerialized]
		internal InputBinding? m_BindingMask;

		[NonSerialized]
		private int m_DevicesCount = -1;

		[NonSerialized]
		private InputDevice[] m_DevicesArray;

		[NonSerialized]
		internal InlinedArray<Action<InputAction.CallbackContext>> m_ActionCallbacks;

		internal static int s_DeferBindingResolution;

		public string name
		{
			get
			{
				return m_Name;
			}
		}

		public InputActionAsset asset
		{
			get
			{
				return m_Asset;
			}
		}

		public Guid id
		{
			get
			{
				if (string.IsNullOrEmpty(m_Id))
				{
					GenerateId();
				}
				return new Guid(m_Id);
			}
		}

		internal Guid idDontGenerate
		{
			get
			{
				if (string.IsNullOrEmpty(m_Id))
				{
					return default(Guid);
				}
				return new Guid(m_Id);
			}
		}

		public bool enabled
		{
			get
			{
				return m_EnabledActionsCount > 0;
			}
		}

		public ReadOnlyArray<InputAction> actions
		{
			get
			{
				return new ReadOnlyArray<InputAction>(m_Actions);
			}
		}

		public ReadOnlyArray<InputBinding> bindings
		{
			get
			{
				return new ReadOnlyArray<InputBinding>(m_Bindings);
			}
		}

		public ReadOnlyArray<InputControlScheme> controlSchemes
		{
			get
			{
				if (m_Asset == null)
				{
					return default(ReadOnlyArray<InputControlScheme>);
				}
				return m_Asset.controlSchemes;
			}
		}

		public InputBinding? bindingMask
		{
			get
			{
				return m_BindingMask;
			}
			set
			{
				if (!(m_BindingMask == value))
				{
					m_BindingMask = value;
					LazyResolveBindings();
				}
			}
		}

		public ReadOnlyArray<InputDevice>? devices
		{
			get
			{
				if (m_DevicesCount < 0)
				{
					if (asset != null)
					{
						return asset.devices;
					}
					return null;
				}
				return new ReadOnlyArray<InputDevice>(m_DevicesArray, 0, m_DevicesCount);
			}
			set
			{
				if (!value.HasValue)
				{
					if (m_DevicesCount < 0)
					{
						return;
					}
					if ((m_DevicesArray != null) & (m_DevicesCount > 0))
					{
						Array.Clear(m_DevicesArray, 0, m_DevicesCount);
					}
					m_DevicesCount = -1;
				}
				else
				{
					if (m_DevicesCount == value.Value.Count)
					{
						bool flag = true;
						for (int i = 0; i < m_DevicesCount; i++)
						{
							if (m_DevicesArray[i] != value.Value[i])
							{
								flag = false;
								break;
							}
						}
						if (flag)
						{
							return;
						}
					}
					if (m_DevicesCount > 0)
					{
						m_DevicesArray.Clear(ref m_DevicesCount);
					}
					m_DevicesCount = 0;
					ArrayHelpers.AppendListWithCapacity(ref m_DevicesArray, ref m_DevicesCount, value.Value);
				}
				LazyResolveBindings();
			}
		}

		public InputAction this[string actionNameOrId]
		{
			get
			{
				if (actionNameOrId == null)
				{
					throw new ArgumentNullException("actionNameOrId");
				}
				InputAction inputAction = FindAction(actionNameOrId);
				if (inputAction == null)
				{
					throw new KeyNotFoundException("Cannot find action '" + actionNameOrId + "'");
				}
				return inputAction;
			}
		}

		public event Action<InputAction.CallbackContext> actionTriggered
		{
			add
			{
				m_ActionCallbacks.AppendWithCapacity(value);
			}
			remove
			{
				m_ActionCallbacks.RemoveByMovingTailWithCapacity(value);
			}
		}

		public InputActionMap()
		{
			m_DevicesCount = -1;
		}

		public InputActionMap(string name)
			: this()
		{
			m_Name = name;
			m_DevicesCount = -1;
		}

		public void Dispose()
		{
			InputActionState state = m_State;
			if (state != null)
			{
				state.Dispose();
			}
		}

		internal int FindActionIndex(string nameOrId)
		{
			if (string.IsNullOrEmpty(nameOrId))
			{
				return -1;
			}
			if (m_Actions == null)
			{
				return -1;
			}
			int num = m_Actions.Length;
			if (nameOrId.StartsWith("{") && nameOrId.EndsWith("}"))
			{
				int length = nameOrId.Length - 2;
				for (int i = 0; i < num; i++)
				{
					if (string.Compare(m_Actions[i].m_Id, 0, nameOrId, 1, length) == 0)
					{
						return i;
					}
				}
			}
			for (int j = 0; j < num; j++)
			{
				if (m_Actions[j].m_Id == nameOrId || string.Compare(m_Actions[j].m_Name, nameOrId, StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					return j;
				}
			}
			return -1;
		}

		private int FindActionIndex(Guid id)
		{
			if (m_Actions == null)
			{
				return -1;
			}
			int num = m_Actions.Length;
			for (int i = 0; i < num; i++)
			{
				if (m_Actions[i].idDontGenerate == id)
				{
					return i;
				}
			}
			return -1;
		}

		public InputAction FindAction(string nameOrId, bool throwIfNotFound = false)
		{
			if (nameOrId == null)
			{
				throw new ArgumentNullException("nameOrId");
			}
			int num = FindActionIndex(nameOrId);
			if (num == -1)
			{
				if (throwIfNotFound)
				{
					throw new ArgumentException(string.Format("No action '{0}' in '{1}'", nameOrId, this), "nameOrId");
				}
				return null;
			}
			return m_Actions[num];
		}

		public InputAction FindAction(Guid id)
		{
			int num = FindActionIndex(id);
			if (num == -1)
			{
				return null;
			}
			return m_Actions[num];
		}

		public bool IsUsableWithDevice(InputDevice device)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			if (m_Bindings == null)
			{
				return false;
			}
			InputBinding[] array = m_Bindings;
			foreach (InputBinding inputBinding in array)
			{
				string effectivePath = inputBinding.effectivePath;
				if (!string.IsNullOrEmpty(effectivePath) && InputControlPath.Matches(effectivePath, device))
				{
					return true;
				}
			}
			return false;
		}

		public void Enable()
		{
			if (m_Actions != null && m_EnabledActionsCount != m_Actions.Length)
			{
				ResolveBindingsIfNecessary();
				m_State.EnableAllActions(this);
			}
		}

		public void Disable()
		{
			if (enabled)
			{
				m_State.DisableAllActions(this);
			}
		}

		public InputActionMap Clone()
		{
			InputActionMap inputActionMap = new InputActionMap
			{
				m_Name = m_Name
			};
			if (m_Actions != null)
			{
				int num = m_Actions.Length;
				InputAction[] array = new InputAction[num];
				for (int i = 0; i < num; i++)
				{
					InputAction inputAction = m_Actions[i];
					array[i] = new InputAction
					{
						m_Name = inputAction.m_Name,
						m_ActionMap = inputActionMap,
						m_Type = inputAction.m_Type,
						m_Interactions = inputAction.m_Interactions,
						m_Processors = inputAction.m_Processors,
						m_ExpectedControlType = inputAction.m_ExpectedControlType
					};
				}
				inputActionMap.m_Actions = array;
			}
			if (m_Bindings != null)
			{
				int num2 = m_Bindings.Length;
				InputBinding[] array2 = new InputBinding[num2];
				Array.Copy(m_Bindings, 0, array2, 0, num2);
				for (int j = 0; j < num2; j++)
				{
					array2[j].m_Id = null;
				}
				inputActionMap.m_Bindings = array2;
			}
			return inputActionMap;
		}

		object ICloneable.Clone()
		{
			return Clone();
		}

		public bool Contains(InputAction action)
		{
			if (action == null)
			{
				return false;
			}
			return action.actionMap == this;
		}

		public override string ToString()
		{
			if (m_Asset != null)
			{
				return string.Format("{0}:{1}", m_Asset, m_Name);
			}
			if (!string.IsNullOrEmpty(m_Name))
			{
				return m_Name;
			}
			return "<Unnamed Action Map>";
		}

		public IEnumerator<InputAction> GetEnumerator()
		{
			return actions.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		internal ReadOnlyArray<InputBinding> GetBindingsForSingleAction(InputAction action)
		{
			if (m_BindingsForEachAction == null)
			{
				SetUpPerActionCachedBindingData();
			}
			return new ReadOnlyArray<InputBinding>(m_BindingsForEachAction, action.m_BindingsStartIndex, action.m_BindingsCount);
		}

		internal ReadOnlyArray<InputControl> GetControlsForSingleAction(InputAction action)
		{
			if (m_ControlsForEachAction == null)
			{
				SetUpPerActionCachedBindingData();
			}
			return new ReadOnlyArray<InputControl>(m_ControlsForEachAction, action.m_ControlStartIndex, action.m_ControlCount);
		}

		private unsafe void SetUpPerActionCachedBindingData()
		{
			if (m_Bindings == null)
			{
				return;
			}
			if (m_SingletonAction != null)
			{
				m_BindingsForEachAction = m_Bindings;
				InputActionState state = m_State;
				m_ControlsForEachAction = ((state != null) ? state.controls : null);
				m_SingletonAction.m_BindingsStartIndex = 0;
				m_SingletonAction.m_BindingsCount = m_Bindings.Length;
				m_SingletonAction.m_ControlStartIndex = 0;
				InputAction singletonAction = m_SingletonAction;
				InputActionState state2 = m_State;
				singletonAction.m_ControlCount = ((state2 != null) ? state2.totalControlCount : 0);
				return;
			}
			InputActionState state3 = m_State;
			InputActionState.ActionMapIndices actionMapIndices = ((state3 != null) ? state3.FetchMapIndices(this) : default(InputActionState.ActionMapIndices));
			for (int i = 0; i < m_Actions.Length; i++)
			{
				InputAction obj = m_Actions[i];
				obj.m_BindingsCount = 0;
				obj.m_BindingsStartIndex = -1;
				obj.m_ControlCount = 0;
				obj.m_ControlStartIndex = -1;
			}
			int num = m_Bindings.Length;
			for (int j = 0; j < num; j++)
			{
				InputAction inputAction = FindAction(m_Bindings[j].action);
				if (inputAction != null)
				{
					inputAction.m_BindingsCount++;
				}
			}
			int num2 = 0;
			if (m_State != null && (m_ControlsForEachAction == null || m_ControlsForEachAction.Length != actionMapIndices.controlCount))
			{
				if (actionMapIndices.controlCount == 0)
				{
					m_ControlsForEachAction = null;
				}
				else
				{
					m_ControlsForEachAction = new InputControl[actionMapIndices.controlCount];
				}
			}
			InputBinding[] array = null;
			int num3 = 0;
			int num4 = 0;
			while (num4 < m_Bindings.Length)
			{
				InputAction inputAction2 = FindAction(m_Bindings[num4].action);
				if (inputAction2 == null || inputAction2.m_BindingsStartIndex != -1)
				{
					num4++;
					continue;
				}
				inputAction2.m_BindingsStartIndex = ((array != null) ? num2 : num4);
				inputAction2.m_ControlStartIndex = num3;
				int bindingsCount = inputAction2.m_BindingsCount;
				int num5 = num4;
				for (int k = 0; k < bindingsCount; k++)
				{
					if (FindAction(m_Bindings[num5].action) != inputAction2)
					{
						if (array == null)
						{
							array = new InputBinding[m_Bindings.Length];
							num2 = num5;
							Array.Copy(m_Bindings, 0, array, 0, num5);
						}
						do
						{
							num5++;
						}
						while (FindAction(m_Bindings[num5].action) != inputAction2);
					}
					else if (num4 == num5)
					{
						num4++;
					}
					if (array != null)
					{
						array[num2++] = m_Bindings[num5];
					}
					if (m_State != null && !m_Bindings[num5].isComposite)
					{
						int controlCount = m_State.bindingStates[actionMapIndices.bindingStartIndex + num5].controlCount;
						if (controlCount > 0)
						{
							Array.Copy(m_State.controls, m_State.bindingStates[actionMapIndices.bindingStartIndex + num5].controlStartIndex, m_ControlsForEachAction, num3, controlCount);
							num3 += controlCount;
							inputAction2.m_ControlCount += controlCount;
						}
					}
					num5++;
				}
			}
			if (array == null)
			{
				m_BindingsForEachAction = m_Bindings;
			}
			else
			{
				m_BindingsForEachAction = array;
			}
		}

		internal void ClearPerActionCachedBindingData()
		{
			m_BindingsForEachAction = null;
			m_ControlsForEachAction = null;
		}

		internal void GenerateId()
		{
			m_Id = Guid.NewGuid().ToString();
		}

		internal bool LazyResolveBindings()
		{
			m_ControlsForEachAction = null;
			if (m_State == null)
			{
				return false;
			}
			if (s_DeferBindingResolution > 0)
			{
				m_NeedToResolveBindings = true;
				return false;
			}
			ResolveBindings();
			return true;
		}

		internal void ResolveBindingsIfNecessary()
		{
			if (m_State == null || m_NeedToResolveBindings)
			{
				ResolveBindings();
			}
		}

		internal void ResolveBindings()
		{
			InputActionState.UnmanagedMemory oldState = default(InputActionState.UnmanagedMemory);
			try
			{
				InputBindingResolver resolver = default(InputBindingResolver);
				OneOrMore<InputActionMap, ReadOnlyArray<InputActionMap>> oneOrMore;
				if (m_Asset != null)
				{
					oneOrMore = m_Asset.actionMaps;
					resolver.bindingMask = m_Asset.m_BindingMask;
				}
				else
				{
					oneOrMore = this;
				}
				bool flag = false;
				if (m_State != null)
				{
					oldState = m_State.memory.Clone();
					flag = m_State.HasEnabledActions();
					for (int i = 0; i < oneOrMore.Count; i++)
					{
						InputActionMap inputActionMap = oneOrMore[i];
						if (flag)
						{
							m_State.DisableAllActions(inputActionMap);
						}
						if (inputActionMap.m_SingletonAction != null)
						{
							InputActionState.NotifyListenersOfActionChange(InputActionChange.BoundControlsAboutToChange, inputActionMap.m_SingletonAction);
						}
						else if (m_Asset == null)
						{
							InputActionState.NotifyListenersOfActionChange(InputActionChange.BoundControlsAboutToChange, inputActionMap);
						}
					}
					if (m_Asset != null)
					{
						InputActionState.NotifyListenersOfActionChange(InputActionChange.BoundControlsAboutToChange, m_Asset);
					}
					resolver.StartWithArraysFrom(m_State);
					m_State.memory.Dispose();
				}
				for (int j = 0; j < oneOrMore.Count; j++)
				{
					resolver.AddActionMap(oneOrMore[j]);
				}
				if (m_State == null)
				{
					if (m_Asset != null)
					{
						InputActionState inputActionState = new InputActionState();
						for (int k = 0; k < oneOrMore.Count; k++)
						{
							oneOrMore[k].m_State = inputActionState;
						}
						m_Asset.m_SharedStateForAllMaps = inputActionState;
					}
					else
					{
						m_State = new InputActionState();
					}
					m_State.Initialize(resolver);
				}
				else
				{
					m_State.ClaimDataFrom(resolver);
				}
				for (int l = 0; l < oneOrMore.Count; l++)
				{
					InputActionMap inputActionMap2 = oneOrMore[l];
					inputActionMap2.m_NeedToResolveBindings = false;
					inputActionMap2.m_ControlsForEachAction = null;
					if (inputActionMap2.m_SingletonAction != null)
					{
						InputActionState.NotifyListenersOfActionChange(InputActionChange.BoundControlsChanged, inputActionMap2.m_SingletonAction);
					}
					else if (m_Asset == null)
					{
						InputActionState.NotifyListenersOfActionChange(InputActionChange.BoundControlsChanged, inputActionMap2);
					}
				}
				if (m_Asset != null)
				{
					InputActionState.NotifyListenersOfActionChange(InputActionChange.BoundControlsChanged, m_Asset);
				}
				if (flag)
				{
					m_State.RestoreActionStates(oldState);
				}
			}
			finally
			{
				oldState.Dispose();
			}
		}

		internal int FindBinding(InputBinding match)
		{
			int num = m_Bindings.LengthSafe();
			for (int i = 0; i < num; i++)
			{
				if (match.Matches(ref m_Bindings[i]))
				{
					return i;
				}
			}
			return -1;
		}

		public static InputActionMap[] FromJson(string json)
		{
			if (json == null)
			{
				throw new ArgumentNullException("json");
			}
			return JsonUtility.FromJson<ReadFileJson>(json).ToMaps();
		}

		public static string ToJson(IEnumerable<InputActionMap> maps)
		{
			if (maps == null)
			{
				throw new ArgumentNullException("maps");
			}
			return JsonUtility.ToJson(WriteFileJson.FromMaps(maps), true);
		}

		public string ToJson()
		{
			return JsonUtility.ToJson(WriteFileJson.FromMap(this), true);
		}

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			m_State = null;
			m_MapIndexInState = -1;
			if (m_Actions != null)
			{
				int num = m_Actions.Length;
				for (int i = 0; i < num; i++)
				{
					m_Actions[i].m_ActionMap = this;
				}
			}
			ClearPerActionCachedBindingData();
		}
	}
}
