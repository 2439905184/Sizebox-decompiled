using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.DualShock.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 32)]
	internal struct DualShock4HIDInputReport : IInputStateTypeInfo
	{
		[FieldOffset(0)]
		public byte reportId;

		[FieldOffset(1)]
		[InputControl(name = "leftStick", layout = "Stick", format = "VC2B")]
		[InputControl(name = "leftStick/x", offset = 0u, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
		[InputControl(name = "leftStick/left", offset = 0u, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
		[InputControl(name = "leftStick/right", offset = 0u, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1")]
		[InputControl(name = "leftStick/y", offset = 1u, format = "BYTE", parameters = "invert,normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
		[InputControl(name = "leftStick/up", offset = 1u, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
		[InputControl(name = "leftStick/down", offset = 1u, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1,invert=false")]
		public byte leftStickX;

		[FieldOffset(2)]
		public byte leftStickY;

		[FieldOffset(3)]
		[InputControl(name = "rightStick", layout = "Stick", format = "VC2B")]
		[InputControl(name = "rightStick/x", offset = 0u, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
		[InputControl(name = "rightStick/left", offset = 0u, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
		[InputControl(name = "rightStick/right", offset = 0u, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1")]
		[InputControl(name = "rightStick/y", offset = 1u, format = "BYTE", parameters = "invert,normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
		[InputControl(name = "rightStick/up", offset = 1u, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
		[InputControl(name = "rightStick/down", offset = 1u, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1,invert=false")]
		public byte rightStickX;

		[FieldOffset(4)]
		public byte rightStickY;

		[FieldOffset(5)]
		[InputControl(name = "dpad", format = "BIT", layout = "Dpad", sizeInBits = 4u, defaultState = 8)]
		[InputControl(name = "dpad/up", format = "BIT", layout = "DiscreteButton", parameters = "minValue=7,maxValue=1,nullValue=8,wrapAtValue=7", bit = 0u, sizeInBits = 4u)]
		[InputControl(name = "dpad/right", format = "BIT", layout = "DiscreteButton", parameters = "minValue=1,maxValue=3", bit = 0u, sizeInBits = 4u)]
		[InputControl(name = "dpad/down", format = "BIT", layout = "DiscreteButton", parameters = "minValue=3,maxValue=5", bit = 0u, sizeInBits = 4u)]
		[InputControl(name = "dpad/left", format = "BIT", layout = "DiscreteButton", parameters = "minValue=5, maxValue=7", bit = 0u, sizeInBits = 4u)]
		[InputControl(name = "buttonWest", displayName = "Square", bit = 4u)]
		[InputControl(name = "buttonSouth", displayName = "Cross", bit = 5u)]
		[InputControl(name = "buttonEast", displayName = "Circle", bit = 6u)]
		[InputControl(name = "buttonNorth", displayName = "Triangle", bit = 7u)]
		public byte buttons1;

		[FieldOffset(6)]
		[InputControl(name = "leftShoulder", bit = 0u)]
		[InputControl(name = "rightShoulder", bit = 1u)]
		[InputControl(name = "leftTriggerButton", layout = "Button", bit = 2u)]
		[InputControl(name = "rightTriggerButton", layout = "Button", bit = 3u)]
		[InputControl(name = "select", displayName = "Share", bit = 4u)]
		[InputControl(name = "start", displayName = "Options", bit = 5u)]
		[InputControl(name = "leftStickPress", bit = 6u)]
		[InputControl(name = "rightStickPress", bit = 7u)]
		public byte buttons2;

		[FieldOffset(7)]
		[InputControl(name = "systemButton", layout = "Button", displayName = "System", bit = 0u)]
		[InputControl(name = "touchpadButton", layout = "Button", displayName = "Touchpad Press", bit = 1u)]
		public byte buttons3;

		[FieldOffset(8)]
		[InputControl(name = "leftTrigger", format = "BYTE")]
		public byte leftTrigger;

		[FieldOffset(9)]
		[InputControl(name = "rightTrigger", format = "BYTE")]
		public byte rightTrigger;

		[FieldOffset(30)]
		public byte batteryLevel;

		public FourCC format
		{
			get
			{
				return new FourCC('H', 'I', 'D');
			}
		}
	}
}
