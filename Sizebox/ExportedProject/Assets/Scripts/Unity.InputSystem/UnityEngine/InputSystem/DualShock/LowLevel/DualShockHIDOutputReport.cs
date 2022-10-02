using System;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.DualShock.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 40)]
	internal struct DualShockHIDOutputReport : IInputDeviceCommandInfo
	{
		[Flags]
		public enum Flags
		{
			Rumble = 1,
			Color = 2
		}

		internal const int kSize = 40;

		internal const int kReportId = 5;

		[FieldOffset(0)]
		public InputDeviceCommand baseCommand;

		[FieldOffset(8)]
		public byte reportId;

		[FieldOffset(9)]
		public byte flags;

		[FieldOffset(10)]
		public unsafe fixed byte unknown1[2];

		[FieldOffset(12)]
		public byte highFrequencyMotorSpeed;

		[FieldOffset(13)]
		public byte lowFrequencyMotorSpeed;

		[FieldOffset(14)]
		public byte redColor;

		[FieldOffset(15)]
		public byte greenColor;

		[FieldOffset(16)]
		public byte blueColor;

		[FieldOffset(17)]
		public unsafe fixed byte unknown2[23];

		public static FourCC Type
		{
			get
			{
				return new FourCC('H', 'I', 'D', 'O');
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return Type;
			}
		}

		public void SetMotorSpeeds(float lowFreq, float highFreq)
		{
			flags |= 1;
			lowFrequencyMotorSpeed = (byte)Mathf.Clamp(lowFreq * 255f, 0f, 255f);
			highFrequencyMotorSpeed = (byte)Mathf.Clamp(highFreq * 255f, 0f, 255f);
		}

		public void SetColor(Color color)
		{
			flags |= 2;
			redColor = (byte)Mathf.Clamp(color.r * 255f, 0f, 255f);
			greenColor = (byte)Mathf.Clamp(color.g * 255f, 0f, 255f);
			blueColor = (byte)Mathf.Clamp(color.b * 255f, 0f, 255f);
		}

		public static DualShockHIDOutputReport Create()
		{
			DualShockHIDOutputReport result = default(DualShockHIDOutputReport);
			result.baseCommand = new InputDeviceCommand(Type, 40);
			result.reportId = 5;
			return result;
		}
	}
}
