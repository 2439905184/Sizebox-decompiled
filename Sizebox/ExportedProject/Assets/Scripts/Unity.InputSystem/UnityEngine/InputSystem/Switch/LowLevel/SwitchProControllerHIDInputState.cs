using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.Switch.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 20)]
	internal struct SwitchProControllerHIDInputState : IInputStateTypeInfo
	{
		public enum Button
		{
			North = 11,
			South = 8,
			West = 10,
			East = 9,
			StickL = 18,
			StickR = 19,
			L = 12,
			R = 13,
			ZL = 14,
			ZR = 15,
			Plus = 17,
			Minus = 16,
			X = 11,
			B = 8,
			Y = 10,
			A = 9
		}

		[FieldOffset(0)]
		[InputControl(name = "dpad", format = "BIT", layout = "Dpad", bit = 24u, sizeInBits = 4u, defaultState = 8)]
		[InputControl(name = "dpad/up", format = "BIT", layout = "DiscreteButton", parameters = "minValue=7,maxValue=1,nullValue=8,wrapAtValue=7", bit = 24u, sizeInBits = 4u)]
		[InputControl(name = "dpad/right", format = "BIT", layout = "DiscreteButton", parameters = "minValue=1,maxValue=3", bit = 24u, sizeInBits = 4u)]
		[InputControl(name = "dpad/down", format = "BIT", layout = "DiscreteButton", parameters = "minValue=3,maxValue=5", bit = 24u, sizeInBits = 4u)]
		[InputControl(name = "dpad/left", format = "BIT", layout = "DiscreteButton", parameters = "minValue=5, maxValue=7", bit = 24u, sizeInBits = 4u)]
		[InputControl(name = "buttonNorth", displayName = "X", shortDisplayName = "X", bit = 11u)]
		[InputControl(name = "buttonSouth", displayName = "B", shortDisplayName = "B", bit = 8u, usage = "Back")]
		[InputControl(name = "buttonWest", displayName = "Y", shortDisplayName = "Y", bit = 10u, usage = "SecondaryAction")]
		[InputControl(name = "buttonEast", displayName = "A", shortDisplayName = "A", bit = 9u, usage = "PrimaryAction")]
		[InputControl(name = "leftStickPress", displayName = "Left Stick", bit = 18u)]
		[InputControl(name = "rightStickPress", displayName = "Right Stick", bit = 19u)]
		[InputControl(name = "leftShoulder", displayName = "L", shortDisplayName = "L", bit = 12u)]
		[InputControl(name = "rightShoulder", displayName = "R", shortDisplayName = "R", bit = 13u)]
		[InputControl(name = "leftTrigger", displayName = "ZL", shortDisplayName = "ZL", format = "BIT", bit = 14u)]
		[InputControl(name = "rightTrigger", displayName = "ZR", shortDisplayName = "ZR", format = "BIT", bit = 15u)]
		[InputControl(name = "start", displayName = "Plus", bit = 17u, usage = "Menu")]
		[InputControl(name = "select", displayName = "Minus", bit = 16u)]
		public uint buttons;

		[FieldOffset(4)]
		[InputControl(name = "leftStick", format = "VC2S", layout = "Stick")]
		[InputControl(name = "leftStick/x", offset = 0u, format = "USHT", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5")]
		[InputControl(name = "leftStick/left", offset = 0u, format = "USHT", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0.15,clampMax=0.5,invert")]
		[InputControl(name = "leftStick/right", offset = 0u, format = "USHT", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=0.85")]
		[InputControl(name = "leftStick/y", offset = 2u, format = "USHT", parameters = "invert,normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5")]
		[InputControl(name = "leftStick/up", offset = 2u, format = "USHT", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0.15,clampMax=0.5,invert")]
		[InputControl(name = "leftStick/down", offset = 2u, format = "USHT", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=0.85,invert=false")]
		public ushort leftStickX;

		[FieldOffset(6)]
		public ushort leftStickY;

		[FieldOffset(8)]
		[InputControl(name = "rightStick", format = "VC2S", layout = "Stick")]
		[InputControl(name = "rightStick/x", offset = 0u, format = "USHT", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5")]
		[InputControl(name = "rightStick/left", offset = 0u, format = "USHT", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
		[InputControl(name = "rightStick/right", offset = 0u, format = "USHT", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1")]
		[InputControl(name = "rightStick/y", offset = 2u, format = "USHT", parameters = "invert,normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5")]
		[InputControl(name = "rightStick/up", offset = 2u, format = "USHT", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0.15,clampMax=0.5,invert")]
		[InputControl(name = "rightStick/down", offset = 2u, format = "USHT", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=0.85,invert=false")]
		public ushort rightStickX;

		[FieldOffset(10)]
		public ushort rightStickY;

		public FourCC format
		{
			get
			{
				return new FourCC('H', 'I', 'D');
			}
		}

		public float leftTrigger
		{
			get
			{
				if ((buttons & 0x4000) == 0)
				{
					return 0f;
				}
				return 1f;
			}
		}

		public float rightTrigger
		{
			get
			{
				if ((buttons & 0x8000) == 0)
				{
					return 0f;
				}
				return 1f;
			}
		}

		public SwitchProControllerHIDInputState WithButton(Button button, bool value = true)
		{
			uint num = (uint)(1 << (int)button);
			if (value)
			{
				buttons |= num;
			}
			else
			{
				buttons &= ~num;
			}
			buttons |= 134217728u;
			leftStickX = 32768;
			leftStickY = 32768;
			rightStickX = 32768;
			rightStickY = 32768;
			return this;
		}
	}
}
