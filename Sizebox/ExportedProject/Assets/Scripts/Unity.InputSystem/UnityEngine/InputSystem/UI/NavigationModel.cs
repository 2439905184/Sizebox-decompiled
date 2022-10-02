using UnityEngine.EventSystems;

namespace UnityEngine.InputSystem.UI
{
	internal struct NavigationModel
	{
		public struct ButtonState
		{
			private bool m_IsPressed;

			private PointerEventData.FramePressState m_FramePressState;

			public bool isPressed
			{
				get
				{
					return m_IsPressed;
				}
				set
				{
					if (m_IsPressed != value)
					{
						m_IsPressed = value;
						if (m_FramePressState == PointerEventData.FramePressState.NotChanged && value)
						{
							m_FramePressState = PointerEventData.FramePressState.Pressed;
						}
						else if (m_FramePressState == PointerEventData.FramePressState.NotChanged && !value)
						{
							m_FramePressState = PointerEventData.FramePressState.Released;
						}
						else if (m_FramePressState == PointerEventData.FramePressState.Pressed && !value)
						{
							m_FramePressState = PointerEventData.FramePressState.PressedAndReleased;
						}
					}
				}
			}

			public bool wasPressedThisFrame
			{
				get
				{
					if (m_FramePressState != 0)
					{
						return m_FramePressState == PointerEventData.FramePressState.PressedAndReleased;
					}
					return true;
				}
			}

			public void OnFrameFinished()
			{
				m_FramePressState = PointerEventData.FramePressState.NotChanged;
			}
		}

		public Vector2 move;

		public int consecutiveMoveCount;

		public MoveDirection lastMoveDirection;

		public float lastMoveTime;

		public ButtonState submitButton;

		public ButtonState cancelButton;

		public AxisEventData eventData;

		public void Reset()
		{
			move = Vector2.zero;
			OnFrameFinished();
		}

		public void OnFrameFinished()
		{
			submitButton.OnFrameFinished();
			cancelButton.OnFrameFinished();
		}
	}
}
