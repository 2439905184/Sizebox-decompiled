using System;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Serialization;

namespace UnityEngine.InputSystem
{
	[Serializable]
	public sealed class InputAction : ICloneable, IDisposable
	{
		public struct CallbackContext
		{
			internal InputActionState m_State;

			internal int m_ActionIndex;

			private int actionIndex
			{
				get
				{
					return m_ActionIndex;
				}
			}

			private unsafe int bindingIndex
			{
				get
				{
					return m_State.actionStates[actionIndex].bindingIndex;
				}
			}

			private unsafe int controlIndex
			{
				get
				{
					return m_State.actionStates[actionIndex].controlIndex;
				}
			}

			private unsafe int interactionIndex
			{
				get
				{
					return m_State.actionStates[actionIndex].interactionIndex;
				}
			}

			public unsafe InputActionPhase phase
			{
				get
				{
					if (m_State == null)
					{
						return InputActionPhase.Disabled;
					}
					return m_State.actionStates[actionIndex].phase;
				}
			}

			public bool started
			{
				get
				{
					return phase == InputActionPhase.Started;
				}
			}

			public bool performed
			{
				get
				{
					return phase == InputActionPhase.Performed;
				}
			}

			public bool canceled
			{
				get
				{
					return phase == InputActionPhase.Canceled;
				}
			}

			public InputAction action
			{
				get
				{
					InputActionState state = m_State;
					if (state == null)
					{
						return null;
					}
					return state.GetActionOrNull(bindingIndex);
				}
			}

			public InputControl control
			{
				get
				{
					InputActionState state = m_State;
					if (state == null)
					{
						return null;
					}
					return state.controls[controlIndex];
				}
			}

			public IInputInteraction interaction
			{
				get
				{
					if (m_State == null)
					{
						return null;
					}
					int num = interactionIndex;
					if (num == -1)
					{
						return null;
					}
					return m_State.interactions[num];
				}
			}

			public unsafe double time
			{
				get
				{
					if (m_State == null)
					{
						return 0.0;
					}
					return m_State.actionStates[actionIndex].time;
				}
			}

			public unsafe double startTime
			{
				get
				{
					if (m_State == null)
					{
						return 0.0;
					}
					return m_State.actionStates[actionIndex].startTime;
				}
			}

			public double duration
			{
				get
				{
					return time - startTime;
				}
			}

			public Type valueType
			{
				get
				{
					InputActionState state = m_State;
					if (state == null)
					{
						return null;
					}
					return state.GetValueType(bindingIndex, controlIndex);
				}
			}

			public int valueSizeInBytes
			{
				get
				{
					if (m_State == null)
					{
						return 0;
					}
					return m_State.GetValueSizeInBytes(bindingIndex, controlIndex);
				}
			}

			public unsafe void ReadValue(void* buffer, int bufferSize)
			{
				if (buffer == null)
				{
					throw new ArgumentNullException("buffer");
				}
				InputActionState state = m_State;
				if (state != null)
				{
					state.ReadValue(bindingIndex, controlIndex, buffer, bufferSize);
				}
			}

			public TValue ReadValue<TValue>() where TValue : struct
			{
				TValue result = default(TValue);
				if (m_State != null && phase != InputActionPhase.Canceled)
				{
					return m_State.ReadValue<TValue>(bindingIndex, controlIndex);
				}
				return result;
			}

			public bool ReadValueAsButton()
			{
				bool result = false;
				if (m_State != null && phase != InputActionPhase.Canceled)
				{
					result = m_State.ReadValueAsButton(bindingIndex, controlIndex);
				}
				return result;
			}

			public object ReadValueAsObject()
			{
				InputActionState state = m_State;
				if (state == null)
				{
					return null;
				}
				return state.ReadValueAsObject(bindingIndex, controlIndex);
			}

			public override string ToString()
			{
				return string.Format("{{ action={0} phase={1} time={2} control={3} value={4} interaction={5} }}", action, phase, time, control, ReadValueAsObject(), interaction);
			}
		}

		[Tooltip("Human readable name of the action. Must be unique within its action map (case is ignored). Can be changed without breaking references to the action.")]
		[SerializeField]
		internal string m_Name;

		[SerializeField]
		internal InputActionType m_Type;

		[FormerlySerializedAs("m_ExpectedControlLayout")]
		[Tooltip("Type of control expected by the action (e.g. \"Button\" or \"Stick\"). This will limit the controls shown when setting up bindings in the UI and will also limit which controls can be bound interactively to the action.")]
		[SerializeField]
		internal string m_ExpectedControlType;

		[Tooltip("Unique ID of the action (GUID). Used to reference the action from bindings such that actions can be renamed without breaking references.")]
		[SerializeField]
		internal string m_Id;

		[SerializeField]
		internal string m_Processors;

		[SerializeField]
		internal string m_Interactions;

		[SerializeField]
		internal InputBinding[] m_SingletonActionBindings;

		[NonSerialized]
		internal InputBinding? m_BindingMask;

		[NonSerialized]
		internal int m_BindingsStartIndex;

		[NonSerialized]
		internal int m_BindingsCount;

		[NonSerialized]
		internal int m_ControlStartIndex;

		[NonSerialized]
		internal int m_ControlCount;

		[NonSerialized]
		internal int m_ActionIndexInState = -1;

		[NonSerialized]
		internal InputActionMap m_ActionMap;

		[NonSerialized]
		internal InlinedArray<Action<CallbackContext>> m_OnStarted;

		[NonSerialized]
		internal InlinedArray<Action<CallbackContext>> m_OnCanceled;

		[NonSerialized]
		internal InlinedArray<Action<CallbackContext>> m_OnPerformed;

		public string name
		{
			get
			{
				return m_Name;
			}
		}

		public InputActionType type
		{
			get
			{
				return m_Type;
			}
		}

		public Guid id
		{
			get
			{
				MakeSureIdIsInPlace();
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

		public string expectedControlType
		{
			get
			{
				return m_ExpectedControlType;
			}
			set
			{
				m_ExpectedControlType = value;
			}
		}

		public string processors
		{
			get
			{
				return m_Processors;
			}
		}

		public string interactions
		{
			get
			{
				return m_Interactions;
			}
		}

		public InputActionMap actionMap
		{
			get
			{
				if (!isSingletonAction)
				{
					return m_ActionMap;
				}
				return null;
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
				if (!(value == m_BindingMask))
				{
					if (value.HasValue)
					{
						InputBinding value2 = value.Value;
						value2.action = name;
						value = value2;
					}
					m_BindingMask = value;
					InputActionMap orCreateActionMap = GetOrCreateActionMap();
					if (orCreateActionMap.m_State != null)
					{
						orCreateActionMap.LazyResolveBindings();
					}
				}
			}
		}

		public ReadOnlyArray<InputBinding> bindings
		{
			get
			{
				return GetOrCreateActionMap().GetBindingsForSingleAction(this);
			}
		}

		public ReadOnlyArray<InputControl> controls
		{
			get
			{
				InputActionMap orCreateActionMap = GetOrCreateActionMap();
				orCreateActionMap.ResolveBindingsIfNecessary();
				return orCreateActionMap.GetControlsForSingleAction(this);
			}
		}

		public InputActionPhase phase
		{
			get
			{
				return currentState.phase;
			}
		}

		public bool enabled
		{
			get
			{
				return phase != InputActionPhase.Disabled;
			}
		}

		public unsafe bool triggered
		{
			get
			{
				InputActionMap orCreateActionMap = GetOrCreateActionMap();
				if (orCreateActionMap.m_State == null)
				{
					return false;
				}
				uint lastTriggeredInUpdate = orCreateActionMap.m_State.actionStates[m_ActionIndexInState].lastTriggeredInUpdate;
				if (lastTriggeredInUpdate != 0)
				{
					return lastTriggeredInUpdate == InputUpdate.s_UpdateStepCount;
				}
				return false;
			}
		}

		public unsafe InputControl activeControl
		{
			get
			{
				InputActionState state = GetOrCreateActionMap().m_State;
				if (state != null)
				{
					int controlIndex = state.actionStates[m_ActionIndexInState].controlIndex;
					if (controlIndex != -1)
					{
						return state.controls[controlIndex];
					}
				}
				return null;
			}
		}

		internal bool wantsInitialStateCheck
		{
			get
			{
				return type == InputActionType.Value;
			}
		}

		internal bool isSingletonAction
		{
			get
			{
				if (m_ActionMap != null)
				{
					return m_ActionMap.m_SingletonAction == this;
				}
				return true;
			}
		}

		private InputActionState.TriggerState currentState
		{
			get
			{
				if (m_ActionIndexInState == -1)
				{
					return default(InputActionState.TriggerState);
				}
				return m_ActionMap.m_State.FetchActionState(this);
			}
		}

		public event Action<CallbackContext> started
		{
			add
			{
				m_OnStarted.Append(value);
			}
			remove
			{
				m_OnStarted.Remove(value);
			}
		}

		public event Action<CallbackContext> canceled
		{
			add
			{
				m_OnCanceled.Append(value);
			}
			remove
			{
				m_OnCanceled.Remove(value);
			}
		}

		public event Action<CallbackContext> performed
		{
			add
			{
				m_OnPerformed.Append(value);
			}
			remove
			{
				m_OnPerformed.Remove(value);
			}
		}

		public InputAction()
		{
		}

		public InputAction(string name = null, InputActionType type = InputActionType.Value, string binding = null, string interactions = null, string processors = null, string expectedControlType = null)
		{
			m_Name = name;
			m_Type = type;
			if (!string.IsNullOrEmpty(binding))
			{
				m_SingletonActionBindings = new InputBinding[1]
				{
					new InputBinding
					{
						path = binding,
						interactions = interactions,
						processors = processors,
						action = m_Name
					}
				};
				m_BindingsStartIndex = 0;
				m_BindingsCount = 1;
			}
			else
			{
				m_Interactions = interactions;
				m_Processors = processors;
			}
			m_ExpectedControlType = expectedControlType;
		}

		public void Dispose()
		{
			InputActionMap inputActionMap = m_ActionMap;
			if (inputActionMap != null)
			{
				InputActionState state = inputActionMap.m_State;
				if (state != null)
				{
					state.Dispose();
				}
			}
		}

		public override string ToString()
		{
			string text = ((m_Name == null) ? "<Unnamed>" : ((m_ActionMap == null || isSingletonAction || string.IsNullOrEmpty(m_ActionMap.name)) ? m_Name : (m_ActionMap.name + "/" + m_Name)));
			ReadOnlyArray<InputControl> readOnlyArray = controls;
			if (readOnlyArray.Count > 0)
			{
				text += "[";
				bool flag = true;
				foreach (InputControl item in readOnlyArray)
				{
					if (!flag)
					{
						text += ",";
					}
					text += item.path;
					flag = false;
				}
				text += "]";
			}
			return text;
		}

		public void Enable()
		{
			if (!enabled)
			{
				InputActionMap orCreateActionMap = GetOrCreateActionMap();
				orCreateActionMap.ResolveBindingsIfNecessary();
				orCreateActionMap.m_State.EnableSingleAction(this);
			}
		}

		public void Disable()
		{
			if (enabled)
			{
				m_ActionMap.m_State.DisableSingleAction(this);
			}
		}

		public InputAction Clone()
		{
			return new InputAction(m_Name, m_Type)
			{
				m_SingletonActionBindings = bindings.ToArray(),
				m_BindingsCount = m_BindingsCount,
				m_ExpectedControlType = m_ExpectedControlType,
				m_Interactions = m_Interactions,
				m_Processors = m_Processors
			};
		}

		object ICloneable.Clone()
		{
			return Clone();
		}

		public unsafe TValue ReadValue<TValue>() where TValue : struct
		{
			TValue result = default(TValue);
			InputActionState state = GetOrCreateActionMap().m_State;
			if (state != null)
			{
				InputActionState.TriggerState* ptr = state.actionStates + m_ActionIndexInState;
				int controlIndex = ptr->controlIndex;
				if (controlIndex != -1)
				{
					return state.ReadValue<TValue>(ptr->bindingIndex, controlIndex);
				}
			}
			return result;
		}

		public unsafe object ReadValueAsObject()
		{
			InputActionState state = GetOrCreateActionMap().m_State;
			if (state == null)
			{
				return null;
			}
			InputActionState.TriggerState* ptr = state.actionStates + m_ActionIndexInState;
			int controlIndex = ptr->controlIndex;
			if (controlIndex != -1)
			{
				return state.ReadValueAsObject(ptr->bindingIndex, controlIndex);
			}
			return null;
		}

		internal string MakeSureIdIsInPlace()
		{
			if (string.IsNullOrEmpty(m_Id))
			{
				GenerateId();
			}
			return m_Id;
		}

		internal void GenerateId()
		{
			m_Id = Guid.NewGuid().ToString();
		}

		internal InputActionMap GetOrCreateActionMap()
		{
			if (m_ActionMap == null)
			{
				CreateInternalActionMapForSingletonAction();
			}
			return m_ActionMap;
		}

		private void CreateInternalActionMapForSingletonAction()
		{
			m_ActionMap = new InputActionMap
			{
				m_Actions = new InputAction[1] { this },
				m_SingletonAction = this,
				m_Bindings = m_SingletonActionBindings
			};
		}

		internal InputBinding? FindEffectiveBindingMask()
		{
			if (m_BindingMask.HasValue)
			{
				return m_BindingMask;
			}
			InputActionMap inputActionMap = m_ActionMap;
			if (inputActionMap != null && inputActionMap.m_BindingMask.HasValue)
			{
				return m_ActionMap.m_BindingMask;
			}
			InputActionMap inputActionMap2 = m_ActionMap;
			if (inputActionMap2 == null)
			{
				return null;
			}
			InputActionAsset asset = inputActionMap2.m_Asset;
			if ((object)asset == null)
			{
				return null;
			}
			return asset.m_BindingMask;
		}

		internal int BindingIndexOnActionToBindingIndexOnMap(int indexOfBindingOnAction)
		{
			InputBinding[] array = GetOrCreateActionMap().m_Bindings;
			int num = array.LengthSafe();
			string strB = name;
			int num2 = -1;
			for (int i = 0; i < num; i++)
			{
				ref InputBinding reference = ref array[i];
				if (string.Compare(reference.action, strB, StringComparison.InvariantCultureIgnoreCase) == 0 || !(reference.action != m_Id))
				{
					num2++;
					if (num2 == indexOfBindingOnAction)
					{
						return i;
					}
				}
			}
			throw new ArgumentOutOfRangeException("indexOfBindingOnAction", string.Format("Binding index {0} is out of range for action '{1}' with {2} bindings", indexOfBindingOnAction, this, num2 + 1));
		}

		internal int BindingIndexOnMapToBindingIndexOnAction(int indexOfBindingOnMap)
		{
			InputBinding[] array = GetOrCreateActionMap().m_Bindings;
			string strB = name;
			int num = 0;
			for (int num2 = indexOfBindingOnMap - 1; num2 >= 0; num2--)
			{
				ref InputBinding reference = ref array[num2];
				if (string.Compare(reference.action, strB, StringComparison.InvariantCultureIgnoreCase) == 0 || reference.action == m_Id)
				{
					num++;
				}
			}
			return num;
		}
	}
}
