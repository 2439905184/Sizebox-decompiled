using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	internal struct JoystickState : IInputStateTypeInfo
	{
		public enum Button
		{
			HatSwitchUp = 0,
			HatSwitchDown = 1,
			HatSwitchLeft = 2,
			HatSwitchRight = 3,
			Trigger = 4
		}

		[InputControl(name = "trigger", displayName = "Trigger", layout = "Button", usages = new string[] { "PrimaryTrigger", "PrimaryAction", "Submit" }, bit = 4u)]
		public int buttons;

		[InputControl(displayName = "Stick", layout = "Stick", usage = "Primary2DMotion", processors = "stickDeadzone")]
		public Vector2 stick;

		public static FourCC kFormat
		{
			get
			{
				return new FourCC('J', 'O', 'Y');
			}
		}

		public FourCC format
		{
			get
			{
				return kFormat;
			}
		}
	}
}
