using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.DualShock.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 32)]
	internal struct DualShock3HIDInputReport : IInputStateTypeInfo
	{
		[FieldOffset(0)]
		private ushort padding1;

		[FieldOffset(2)]
		[InputControl(name = "select", displayName = "Share", bit = 0u)]
		[InputControl(name = "leftStickPress", bit = 1u)]
		[InputControl(name = "rightStickPress", bit = 2u)]
		[InputControl(name = "start", displayName = "Options", bit = 3u)]
		[InputControl(name = "dpad", format = "BIT", layout = "Dpad", bit = 4u, sizeInBits = 4u)]
		[InputControl(name = "dpad/up", bit = 4u)]
		[InputControl(name = "dpad/right", bit = 5u)]
		[InputControl(name = "dpad/down", bit = 6u)]
		[InputControl(name = "dpad/left", bit = 7u)]
		public byte buttons1;

		[FieldOffset(3)]
		[InputControl(name = "leftTriggerButton", layout = "Button", bit = 0u)]
		[InputControl(name = "rightTriggerButton", layout = "Button", bit = 1u)]
		[InputControl(name = "leftShoulder", bit = 2u)]
		[InputControl(name = "rightShoulder", bit = 3u)]
		[InputControl(name = "buttonNorth", displayName = "Triangle", bit = 4u)]
		[InputControl(name = "buttonEast", displayName = "Circle", bit = 5u)]
		[InputControl(name = "buttonSouth", displayName = "Cross", bit = 6u)]
		[InputControl(name = "buttonWest", displayName = "Square", bit = 7u)]
		public byte buttons2;

		[FieldOffset(4)]
		[InputControl(name = "systemButton", layout = "Button", displayName = "System", bit = 0u)]
		[InputControl(name = "touchpadButton", layout = "Button", displayName = "Touchpad Press", bit = 1u)]
		public byte buttons3;

		[FieldOffset(5)]
		private byte padding2;

		[FieldOffset(6)]
		[InputControl(name = "leftStick", layout = "Stick", format = "VC2B")]
		[InputControl(name = "leftStick/x", offset = 0u, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
		[InputControl(name = "leftStick/left", offset = 0u, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
		[InputControl(name = "leftStick/right", offset = 0u, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1")]
		[InputControl(name = "leftStick/y", offset = 1u, format = "BYTE", parameters = "invert,normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
		[InputControl(name = "leftStick/up", offset = 1u, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
		[InputControl(name = "leftStick/down", offset = 1u, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1,invert=false")]
		public byte leftStickX;

		[FieldOffset(7)]
		public byte leftStickY;

		[FieldOffset(8)]
		[InputControl(name = "rightStick", layout = "Stick", format = "VC2B")]
		[InputControl(name = "rightStick/x", offset = 0u, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
		[InputControl(name = "rightStick/left", offset = 0u, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
		[InputControl(name = "rightStick/right", offset = 0u, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1")]
		[InputControl(name = "rightStick/y", offset = 1u, format = "BYTE", parameters = "invert,normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
		[InputControl(name = "rightStick/up", offset = 1u, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
		[InputControl(name = "rightStick/down", offset = 1u, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1,invert=false")]
		public byte rightStickX;

		[FieldOffset(9)]
		public byte rightStickY;

		[FieldOffset(10)]
		private unsafe fixed byte padding3[8];

		[FieldOffset(18)]
		[InputControl(name = "leftTrigger", format = "BYTE")]
		public byte leftTrigger;

		[FieldOffset(19)]
		[InputControl(name = "rightTrigger", format = "BYTE")]
		public byte rightTrigger;

		public FourCC format
		{
			get
			{
				return new FourCC('H', 'I', 'D');
			}
		}
	}
}
