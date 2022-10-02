using System;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Serialization;

namespace UnityEngine.InputSystem.OnScreen
{
	[AddComponentMenu("Input/On-Screen Stick")]
	public class OnScreenStick : OnScreenControl, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IDragHandler
	{
		[FormerlySerializedAs("movementRange")]
		[SerializeField]
		private float m_MovementRange = 50f;

		[InputControl(layout = "Vector2")]
		[SerializeField]
		private string m_ControlPath;

		private Vector3 m_StartPos;

		private Vector2 m_PointerDownPos;

		public float movementRange
		{
			get
			{
				return m_MovementRange;
			}
			set
			{
				m_MovementRange = value;
			}
		}

		protected override string controlPathInternal
		{
			get
			{
				return m_ControlPath;
			}
			set
			{
				m_ControlPath = value;
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (eventData == null)
			{
				throw new ArgumentNullException("eventData");
			}
			RectTransformUtility.ScreenPointToLocalPointInRectangle(base.transform.parent.GetComponentInParent<RectTransform>(), eventData.position, eventData.pressEventCamera, out m_PointerDownPos);
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (eventData == null)
			{
				throw new ArgumentNullException("eventData");
			}
			Vector2 localPoint;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(base.transform.parent.GetComponentInParent<RectTransform>(), eventData.position, eventData.pressEventCamera, out localPoint);
			Vector2 vector = localPoint - m_PointerDownPos;
			vector = Vector2.ClampMagnitude(vector, movementRange);
			((RectTransform)base.transform).anchoredPosition = m_StartPos + (Vector3)vector;
			Vector2 value = new Vector2(vector.x / movementRange, vector.y / movementRange);
			SendValueToControl(value);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			((RectTransform)base.transform).anchoredPosition = m_StartPos;
			SendValueToControl(Vector2.zero);
		}

		private void Start()
		{
			m_StartPos = ((RectTransform)base.transform).anchoredPosition;
		}
	}
}
