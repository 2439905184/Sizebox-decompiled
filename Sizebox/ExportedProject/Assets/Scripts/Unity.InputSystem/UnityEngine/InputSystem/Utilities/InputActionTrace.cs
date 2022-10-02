using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.LowLevel;

namespace UnityEngine.InputSystem.Utilities
{
	public sealed class InputActionTrace : IEnumerable<InputActionTrace.ActionEventPtr>, IEnumerable, IDisposable
	{
		public struct ActionEventPtr
		{
			internal InputActionState m_State;

			internal unsafe ActionEvent* m_Ptr;

			public unsafe InputAction action
			{
				get
				{
					return m_State.GetActionOrNull(m_Ptr->bindingIndex);
				}
			}

			public unsafe InputActionPhase phase
			{
				get
				{
					return m_Ptr->phase;
				}
			}

			public unsafe InputControl control
			{
				get
				{
					return m_State.controls[m_Ptr->controlIndex];
				}
			}

			public unsafe IInputInteraction interaction
			{
				get
				{
					int interactionIndex = m_Ptr->interactionIndex;
					if (interactionIndex == -1)
					{
						return null;
					}
					return m_State.interactions[interactionIndex];
				}
			}

			public unsafe double time
			{
				get
				{
					return m_Ptr->baseEvent.time;
				}
			}

			public unsafe double startTime
			{
				get
				{
					return m_Ptr->startTime;
				}
			}

			public double duration
			{
				get
				{
					return time - startTime;
				}
			}

			public unsafe int valueSizeInBytes
			{
				get
				{
					return m_Ptr->valueSizeInBytes;
				}
			}

			public unsafe object ReadValueAsObject()
			{
				int bufferSize = m_Ptr->valueSizeInBytes;
				byte* valueData = m_Ptr->valueData;
				return control.ReadValueFromBufferAsObject(valueData, bufferSize);
			}

			public unsafe void ReadValue(void* buffer, int bufferSize)
			{
				int num = m_Ptr->valueSizeInBytes;
				if (bufferSize < num)
				{
					throw new ArgumentException(string.Format("Expected buffer of at least {0} bytes but got buffer of just {1} bytes instead", num, bufferSize), "bufferSize");
				}
				UnsafeUtility.MemCpy(buffer, m_Ptr->valueData, num);
			}

			public unsafe TValue ReadValue<TValue>() where TValue : struct
			{
				int num = m_Ptr->valueSizeInBytes;
				if (UnsafeUtility.SizeOf<TValue>() != num)
				{
					throw new InvalidOperationException(string.Format("Cannot read a value of type '{0}' with size {1} from event on action '{2}' with value size {3}", typeof(TValue).Name, UnsafeUtility.SizeOf<TValue>(), action, num));
				}
				TValue output = new TValue();
				UnsafeUtility.MemCpy(UnsafeUtility.AddressOf(ref output), m_Ptr->valueData, num);
				return output;
			}

			public unsafe override string ToString()
			{
				if (m_Ptr == null)
				{
					return "<null>";
				}
				string text = ((action.actionMap != null) ? (action.actionMap.name + "/" + action.name) : action.name);
				return string.Format("{{ action={0} phase={1} time={2} control={3} value={4} interaction={5} duration={6} }}", text, phase, time, control, ReadValueAsObject(), interaction, duration);
			}
		}

		private struct Enumerator : IEnumerator<ActionEventPtr>, IEnumerator, IDisposable
		{
			private readonly InputActionTrace m_Trace;

			private unsafe readonly ActionEvent* m_Buffer;

			private readonly int m_EventCount;

			private unsafe ActionEvent* m_CurrentEvent;

			private int m_CurrentIndex;

			public unsafe ActionEventPtr Current
			{
				get
				{
					InputActionState state = m_Trace.m_ActionMapStates[m_CurrentEvent->stateIndex];
					ActionEventPtr result = default(ActionEventPtr);
					result.m_State = state;
					result.m_Ptr = m_CurrentEvent;
					return result;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return Current;
				}
			}

			public unsafe Enumerator(InputActionTrace trace)
			{
				m_Trace = trace;
				m_Buffer = (ActionEvent*)trace.m_EventBuffer.bufferPtr.data;
				m_EventCount = trace.m_EventBuffer.eventCount;
				m_CurrentEvent = null;
				m_CurrentIndex = 0;
			}

			public unsafe bool MoveNext()
			{
				if (m_CurrentIndex == m_EventCount)
				{
					return false;
				}
				if (m_CurrentEvent == null)
				{
					m_CurrentEvent = m_Buffer;
					return m_CurrentEvent != null;
				}
				m_CurrentIndex++;
				if (m_CurrentIndex == m_EventCount)
				{
					return false;
				}
				m_CurrentEvent = (ActionEvent*)InputEvent.GetNextInMemory((InputEvent*)m_CurrentEvent);
				return true;
			}

			public unsafe void Reset()
			{
				m_CurrentEvent = null;
				m_CurrentIndex = 0;
			}

			public void Dispose()
			{
			}
		}

		private bool m_SubscribedToAll;

		private bool m_OnActionChangeHooked;

		private InlinedArray<InputAction> m_SubscribedActions;

		private InlinedArray<InputActionMap> m_SubscribedActionMaps;

		private InputEventBuffer m_EventBuffer;

		private InlinedArray<InputActionState> m_ActionMapStates;

		private InlinedArray<InputActionState> m_ActionMapStateClones;

		private Action<InputAction.CallbackContext> m_CallbackDelegate;

		private Action<object, InputActionChange> m_ActionChangeDelegate;

		public InputEventBuffer buffer
		{
			get
			{
				return m_EventBuffer;
			}
		}

		public int count
		{
			get
			{
				return m_EventBuffer.eventCount;
			}
		}

		public InputActionTrace()
		{
		}

		public InputActionTrace(InputAction action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			SubscribeTo(action);
		}

		public InputActionTrace(InputActionMap actionMap)
		{
			if (actionMap == null)
			{
				throw new ArgumentNullException("actionMap");
			}
			SubscribeTo(actionMap);
		}

		public void SubscribeToAll()
		{
			if (!m_SubscribedToAll)
			{
				HookOnActionChange();
				m_SubscribedToAll = true;
				while (m_SubscribedActions.length > 0)
				{
					UnsubscribeFrom(m_SubscribedActions[m_SubscribedActions.length - 1]);
				}
				while (m_SubscribedActionMaps.length > 0)
				{
					UnsubscribeFrom(m_SubscribedActionMaps[m_SubscribedActionMaps.length - 1]);
				}
			}
		}

		public void UnsubscribeFromAll()
		{
			if (count == 0)
			{
				UnhookOnActionChange();
			}
			m_SubscribedToAll = false;
			while (m_SubscribedActions.length > 0)
			{
				UnsubscribeFrom(m_SubscribedActions[m_SubscribedActions.length - 1]);
			}
			while (m_SubscribedActionMaps.length > 0)
			{
				UnsubscribeFrom(m_SubscribedActionMaps[m_SubscribedActionMaps.length - 1]);
			}
		}

		public void SubscribeTo(InputAction action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			if (m_CallbackDelegate == null)
			{
				m_CallbackDelegate = RecordAction;
			}
			action.performed += m_CallbackDelegate;
			action.started += m_CallbackDelegate;
			action.canceled += m_CallbackDelegate;
			m_SubscribedActions.AppendWithCapacity(action);
		}

		public void SubscribeTo(InputActionMap actionMap)
		{
			if (actionMap == null)
			{
				throw new ArgumentNullException("actionMap");
			}
			if (m_CallbackDelegate == null)
			{
				m_CallbackDelegate = RecordAction;
			}
			actionMap.actionTriggered += m_CallbackDelegate;
			m_SubscribedActionMaps.AppendWithCapacity(actionMap);
		}

		public void UnsubscribeFrom(InputAction action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			if (m_CallbackDelegate != null)
			{
				action.performed -= m_CallbackDelegate;
				action.started -= m_CallbackDelegate;
				action.canceled -= m_CallbackDelegate;
				int num = m_SubscribedActions.IndexOfReference(action);
				if (num != -1)
				{
					m_SubscribedActions.RemoveAtWithCapacity(num);
				}
			}
		}

		public void UnsubscribeFrom(InputActionMap actionMap)
		{
			if (actionMap == null)
			{
				throw new ArgumentNullException("actionMap");
			}
			if (m_CallbackDelegate != null)
			{
				actionMap.actionTriggered -= m_CallbackDelegate;
				int num = m_SubscribedActionMaps.IndexOfReference(actionMap);
				if (num != -1)
				{
					m_SubscribedActionMaps.RemoveAtWithCapacity(num);
				}
			}
		}

		public unsafe void RecordAction(InputAction.CallbackContext context)
		{
			int num = m_ActionMapStates.IndexOfReference(context.m_State);
			if (num == -1)
			{
				num = m_ActionMapStates.AppendWithCapacity(context.m_State);
			}
			HookOnActionChange();
			int valueSizeInBytes = context.valueSizeInBytes;
			ActionEvent* ptr = (ActionEvent*)m_EventBuffer.AllocateEvent(ActionEvent.GetEventSizeWithValueSize(valueSizeInBytes));
			ref InputActionState.TriggerState reference = ref context.m_State.actionStates[context.m_ActionIndex];
			ptr->baseEvent.type = ActionEvent.Type;
			ptr->baseEvent.time = reference.time;
			ptr->stateIndex = num;
			ptr->controlIndex = reference.controlIndex;
			ptr->bindingIndex = reference.bindingIndex;
			ptr->interactionIndex = reference.interactionIndex;
			ptr->startTime = reference.startTime;
			ptr->phase = reference.phase;
			byte* valueData = ptr->valueData;
			context.ReadValue(valueData, valueSizeInBytes);
		}

		public void Clear()
		{
			m_EventBuffer.Reset();
			m_ActionMapStates.ClearWithCapacity();
		}

		~InputActionTrace()
		{
			DisposeInternal();
		}

		public override string ToString()
		{
			if (count == 0)
			{
				return "[]";
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('[');
			bool flag = true;
			using (IEnumerator<ActionEventPtr> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ActionEventPtr current = enumerator.Current;
					if (!flag)
					{
						stringBuilder.Append(",\n");
					}
					stringBuilder.Append(current.ToString());
					flag = false;
				}
			}
			stringBuilder.Append(']');
			return stringBuilder.ToString();
		}

		public void Dispose()
		{
			UnsubscribeFromAll();
			DisposeInternal();
		}

		private void DisposeInternal()
		{
			for (int i = 0; i < m_ActionMapStateClones.length; i++)
			{
				m_ActionMapStateClones[i].Dispose();
			}
			m_EventBuffer.Dispose();
			m_ActionMapStates.Clear();
			m_ActionMapStateClones.Clear();
			if (m_ActionChangeDelegate != null)
			{
				InputSystem.onActionChange -= m_ActionChangeDelegate;
				m_ActionChangeDelegate = null;
			}
		}

		public IEnumerator<ActionEventPtr> GetEnumerator()
		{
			return new Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private void HookOnActionChange()
		{
			if (!m_OnActionChangeHooked)
			{
				if (m_ActionChangeDelegate == null)
				{
					m_ActionChangeDelegate = OnActionChange;
				}
				InputSystem.onActionChange += m_ActionChangeDelegate;
				m_OnActionChangeHooked = true;
			}
		}

		private void UnhookOnActionChange()
		{
			if (m_OnActionChangeHooked)
			{
				InputSystem.onActionChange -= m_ActionChangeDelegate;
				m_OnActionChangeHooked = false;
			}
		}

		private void OnActionChange(object actionOrMap, InputActionChange change)
		{
			if (m_SubscribedToAll && (uint)(change - 4) <= 2u)
			{
				InputAction obj = (InputAction)actionOrMap;
				int actionIndexInState = obj.m_ActionIndexInState;
				InputActionState state = obj.m_ActionMap.m_State;
				InputAction.CallbackContext callbackContext = default(InputAction.CallbackContext);
				callbackContext.m_State = state;
				callbackContext.m_ActionIndex = actionIndexInState;
				InputAction.CallbackContext context = callbackContext;
				RecordAction(context);
			}
			else
			{
				if (change != InputActionChange.BoundControlsAboutToChange)
				{
					return;
				}
				InputAction inputAction = actionOrMap as InputAction;
				InputActionMap inputActionMap = ((inputAction == null) ? (actionOrMap as InputActionMap) : inputAction.m_ActionMap);
				InputActionState state2 = inputActionMap.m_State;
				if (state2 != null)
				{
					int num = m_ActionMapStates.IndexOfReference(state2);
					if (num != -1)
					{
						InputActionState value = state2.Clone();
						m_ActionMapStateClones.Append(value);
						m_ActionMapStates[num] = value;
					}
				}
			}
		}
	}
}
