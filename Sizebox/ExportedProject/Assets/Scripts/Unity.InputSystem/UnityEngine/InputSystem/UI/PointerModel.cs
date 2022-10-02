using UnityEngine.EventSystems;

namespace UnityEngine.InputSystem.UI
{
	internal struct PointerModel
	{
		public struct ButtonState
		{
			private bool m_IsPressed;

			private PointerEventData.FramePressState m_FramePressState;

			private RaycastResult pressRaycast;

			private GameObject pressObject;

			private GameObject rawPressObject;

			private GameObject lastPressObject;

			private GameObject dragObject;

			private Vector2 pressPosition;

			private float clickTime;

			private int clickCount;

			private bool dragging;

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

			public bool wasReleasedThisFrame
			{
				get
				{
					if (m_FramePressState != PointerEventData.FramePressState.Released)
					{
						return m_FramePressState == PointerEventData.FramePressState.PressedAndReleased;
					}
					return true;
				}
			}

			public void CopyPressStateTo(PointerEventData eventData)
			{
				eventData.pointerPressRaycast = pressRaycast;
				eventData.pressPosition = pressPosition;
				eventData.clickCount = clickCount;
				eventData.clickTime = clickTime;
				eventData.pointerPress = lastPressObject;
				eventData.pointerPress = pressObject;
				eventData.rawPointerPress = rawPressObject;
				eventData.pointerDrag = dragObject;
				eventData.dragging = dragging;
			}

			public void CopyPressStateFrom(PointerEventData eventData)
			{
				pressRaycast = eventData.pointerPressRaycast;
				pressObject = eventData.pointerPress;
				rawPressObject = eventData.rawPointerPress;
				lastPressObject = eventData.lastPress;
				pressPosition = eventData.pressPosition;
				clickTime = eventData.clickTime;
				clickCount = eventData.clickCount;
				dragObject = eventData.pointerDrag;
				dragging = eventData.dragging;
			}

			public void OnEndFrame()
			{
				m_FramePressState = PointerEventData.FramePressState.NotChanged;
			}
		}

		public bool changedThisFrame;

		public ButtonState leftButton;

		public ButtonState rightButton;

		public ButtonState middleButton;

		public ExtendedPointerEventData eventData;

		private Vector2 m_ScreenPosition;

		private Vector2 m_ScrollDelta;

		private Vector3 m_WorldPosition;

		private Quaternion m_WorldOrientation;

		public UIPointerType pointerType
		{
			get
			{
				return eventData.pointerType;
			}
		}

		public Vector2 screenPosition
		{
			get
			{
				return m_ScreenPosition;
			}
			set
			{
				if (m_ScreenPosition != value)
				{
					m_ScreenPosition = value;
					changedThisFrame = true;
				}
			}
		}

		public Vector3 worldPosition
		{
			get
			{
				return m_WorldPosition;
			}
			set
			{
				if (m_WorldPosition != value)
				{
					m_WorldPosition = value;
					changedThisFrame = true;
				}
			}
		}

		public Quaternion worldOrientation
		{
			get
			{
				return m_WorldOrientation;
			}
			set
			{
				if (m_WorldOrientation != value)
				{
					m_WorldOrientation = value;
					changedThisFrame = true;
				}
			}
		}

		public Vector2 scrollDelta
		{
			get
			{
				return m_ScrollDelta;
			}
			set
			{
				if (m_ScrollDelta != value)
				{
					changedThisFrame = true;
					m_ScrollDelta = value;
				}
			}
		}

		public PointerModel(int pointerId, int touchId, UIPointerType pointerType, InputDevice device, ExtendedPointerEventData eventData)
		{
			this.eventData = eventData;
			eventData.pointerId = pointerId;
			eventData.touchId = touchId;
			eventData.pointerType = pointerType;
			eventData.device = device;
			changedThisFrame = false;
			leftButton = default(ButtonState);
			leftButton.OnEndFrame();
			rightButton = default(ButtonState);
			rightButton.OnEndFrame();
			middleButton = default(ButtonState);
			middleButton.OnEndFrame();
			m_ScreenPosition = default(Vector2);
			m_ScrollDelta = default(Vector2);
			m_WorldOrientation = default(Quaternion);
			m_WorldPosition = default(Vector3);
		}

		public void OnFrameFinished()
		{
			changedThisFrame = false;
			scrollDelta = default(Vector2);
			leftButton.OnEndFrame();
			rightButton.OnEndFrame();
			middleButton.OnEndFrame();
		}
	}
}
