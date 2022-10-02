using System.Text;
using UnityEngine.EventSystems;

namespace UnityEngine.InputSystem.UI
{
	public class ExtendedPointerEventData : PointerEventData
	{
		public InputDevice device { get; set; }

		public int touchId { get; set; }

		public UIPointerType pointerType { get; set; }

		public Vector3 trackedDevicePosition { get; set; }

		public Quaternion trackedDeviceOrientation { get; set; }

		public ExtendedPointerEventData(EventSystem eventSystem)
			: base(eventSystem)
		{
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.ToString());
			stringBuilder.AppendLine("<b>device</b>: " + device);
			stringBuilder.AppendLine("<b>pointerType</b>: " + pointerType);
			stringBuilder.AppendLine("<b>touchId</b>: " + touchId);
			stringBuilder.AppendLine("<b>trackedDevicePosition</b>: " + trackedDevicePosition);
			stringBuilder.AppendLine("<b>trackedDeviceOrientation</b>: " + trackedDeviceOrientation);
			return stringBuilder.ToString();
		}

		internal static int MakePointerIdForTouch(int deviceId, int touchId)
		{
			return (deviceId << 24) + touchId;
		}
	}
}
