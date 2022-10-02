using System.ComponentModel;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem.Interactions
{
	[Preserve]
	[DisplayName("Press")]
	public class PressInteraction : IInputInteraction
	{
		[Tooltip("The amount of actuation a control requires before being considered pressed. If not set, default to 'Default Press Point' in the global input settings.")]
		public float pressPoint;

		[Tooltip("Determines how button presses trigger the action. By default (PressOnly), the action is performed on press. With ReleaseOnly, the action is performed on release. With PressAndRelease, the action is performed on press and release.")]
		public PressBehavior behavior;

		private bool m_WaitingForRelease;

		private float pressPointOrDefault
		{
			get
			{
				if (!(pressPoint > 0f))
				{
					return ButtonControl.s_GlobalDefaultButtonPressPoint;
				}
				return pressPoint;
			}
		}

		public void Process(ref InputInteractionContext context)
		{
			bool flag = context.ControlIsActuated(pressPointOrDefault);
			switch (behavior)
			{
			case PressBehavior.PressOnly:
				if (m_WaitingForRelease)
				{
					if (!flag)
					{
						m_WaitingForRelease = false;
						context.Canceled();
					}
				}
				else if (flag)
				{
					m_WaitingForRelease = true;
					context.PerformedAndStayPerformed();
				}
				break;
			case PressBehavior.ReleaseOnly:
				if (m_WaitingForRelease && !flag)
				{
					m_WaitingForRelease = false;
					context.Performed();
					context.Canceled();
				}
				else if (flag)
				{
					m_WaitingForRelease = true;
					context.Started();
				}
				break;
			case PressBehavior.PressAndRelease:
				if (m_WaitingForRelease)
				{
					m_WaitingForRelease = flag;
					if (!flag)
					{
						context.Performed();
						context.Canceled();
					}
				}
				else if (flag)
				{
					m_WaitingForRelease = true;
					context.PerformedAndStayPerformed();
				}
				break;
			}
		}

		public void Reset()
		{
			m_WaitingForRelease = false;
		}
	}
}
