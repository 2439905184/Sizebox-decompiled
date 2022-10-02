using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 30)]
	public struct MouseState : IInputStateTypeInfo
	{
		[FieldOffset(0)]
		[InputControl(usage = "Point")]
		public Vector2 position;

		[FieldOffset(8)]
		[InputControl(usage = "Secondary2DMotion")]
		public Vector2 delta;

		[FieldOffset(16)]
		[InputControl(displayName = "Scroll")]
		[InputControl(name = "scroll/x", aliases = new string[] { "horizontal" }, usage = "ScrollHorizontal", displayName = "Left/Right")]
		[InputControl(name = "scroll/y", aliases = new string[] { "vertical" }, usage = "ScrollVertical", displayName = "Up/Down", shortDisplayName = "Wheel")]
		public Vector2 scroll;

		[FieldOffset(24)]
		[InputControl(name = "press", useStateFrom = "leftButton", synthetic = true, usages = new string[] { })]
		[InputControl(name = "leftButton", layout = "Button", bit = 0u, usage = "PrimaryAction", displayName = "Left Button", shortDisplayName = "LMB")]
		[InputControl(name = "rightButton", layout = "Button", bit = 1u, usage = "SecondaryAction", displayName = "Right Button", shortDisplayName = "RMB")]
		[InputControl(name = "middleButton", layout = "Button", bit = 2u, displayName = "Middle Button", shortDisplayName = "MMB")]
		[InputControl(name = "forwardButton", layout = "Button", bit = 3u, usage = "Forward", displayName = "Forward")]
		[InputControl(name = "backButton", layout = "Button", bit = 4u, usage = "Back", displayName = "Back")]
		[InputControl(name = "pressure", layout = "Axis", usage = "Pressure", offset = 4294967294u, format = "FLT", sizeInBits = 32u)]
		[InputControl(name = "radius", layout = "Vector2", usage = "Radius", offset = 4294967294u, format = "VEC2", sizeInBits = 64u)]
		[InputControl(name = "pointerId", layout = "Digital", format = "BIT", sizeInBits = 1u, offset = 4294967294u)]
		public ushort buttons;

		[FieldOffset(26)]
		private ushort displayIndex;

		[FieldOffset(28)]
		[InputControl(layout = "Integer", displayName = "Click Count", synthetic = true)]
		public ushort clickCount;

		public static FourCC Format
		{
			get
			{
				return new FourCC('M', 'O', 'U', 'S');
			}
		}

		public FourCC format
		{
			get
			{
				return Format;
			}
		}

		public MouseState WithButton(MouseButton button, bool state = true)
		{
			int num = 1 << (int)button;
			if (state)
			{
				buttons |= (ushort)num;
			}
			else
			{
				buttons &= (ushort)(~num);
			}
			return this;
		}
	}
}
