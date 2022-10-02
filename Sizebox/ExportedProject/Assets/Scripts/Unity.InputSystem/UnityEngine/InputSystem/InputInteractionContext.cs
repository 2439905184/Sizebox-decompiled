using System;

namespace UnityEngine.InputSystem
{
	public struct InputInteractionContext
	{
		[Flags]
		internal enum Flags
		{
			TimerHasExpired = 2
		}

		internal InputActionState m_State;

		internal Flags m_Flags;

		internal InputActionState.TriggerState m_TriggerState;

		public InputAction action
		{
			get
			{
				return m_State.GetActionOrNull(ref m_TriggerState);
			}
		}

		public InputControl control
		{
			get
			{
				return m_State.GetControl(ref m_TriggerState);
			}
		}

		public InputActionPhase phase
		{
			get
			{
				return m_TriggerState.phase;
			}
		}

		public double time
		{
			get
			{
				return m_TriggerState.time;
			}
		}

		public double startTime
		{
			get
			{
				return m_TriggerState.startTime;
			}
		}

		public bool timerHasExpired
		{
			get
			{
				return (m_Flags & Flags.TimerHasExpired) != 0;
			}
			internal set
			{
				if (value)
				{
					m_Flags |= Flags.TimerHasExpired;
				}
				else
				{
					m_Flags &= ~Flags.TimerHasExpired;
				}
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

		internal int mapIndex
		{
			get
			{
				return m_TriggerState.mapIndex;
			}
		}

		internal int controlIndex
		{
			get
			{
				return m_TriggerState.controlIndex;
			}
		}

		internal int bindingIndex
		{
			get
			{
				return m_TriggerState.bindingIndex;
			}
		}

		internal int interactionIndex
		{
			get
			{
				return m_TriggerState.interactionIndex;
			}
		}

		public bool ControlIsActuated(float threshold = 0f)
		{
			return m_State.IsActuated(ref m_TriggerState, threshold);
		}

		public void Started()
		{
			m_TriggerState.startTime = time;
			m_State.ChangePhaseOfInteraction(InputActionPhase.Started, ref m_TriggerState);
		}

		public void Performed()
		{
			if (m_TriggerState.phase == InputActionPhase.Waiting)
			{
				m_TriggerState.startTime = time;
			}
			m_State.ChangePhaseOfInteraction(InputActionPhase.Performed, ref m_TriggerState);
		}

		public void PerformedAndStayStarted()
		{
			if (m_TriggerState.phase == InputActionPhase.Waiting)
			{
				m_TriggerState.startTime = time;
			}
			m_State.ChangePhaseOfInteraction(InputActionPhase.Performed, ref m_TriggerState, InputActionPhase.Started);
		}

		public void PerformedAndStayPerformed()
		{
			if (m_TriggerState.phase == InputActionPhase.Waiting)
			{
				m_TriggerState.startTime = time;
			}
			m_State.ChangePhaseOfInteraction(InputActionPhase.Performed, ref m_TriggerState, InputActionPhase.Performed);
		}

		public void Canceled()
		{
			if (m_TriggerState.phase != InputActionPhase.Canceled)
			{
				m_State.ChangePhaseOfInteraction(InputActionPhase.Canceled, ref m_TriggerState);
			}
		}

		public void Waiting()
		{
			if (m_TriggerState.phase != InputActionPhase.Waiting)
			{
				m_State.ChangePhaseOfInteraction(InputActionPhase.Waiting, ref m_TriggerState);
			}
		}

		public void SetTimeout(float seconds)
		{
			m_State.StartTimeout(seconds, ref m_TriggerState);
		}

		public TValue ReadValue<TValue>() where TValue : struct
		{
			return m_State.ReadValue<TValue>(m_TriggerState.bindingIndex, m_TriggerState.controlIndex);
		}
	}
}
