using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 12)]
	public struct SetSamplingFrequencyCommand : IInputDeviceCommandInfo
	{
		internal const int kSize = 12;

		[FieldOffset(0)]
		public InputDeviceCommand baseCommand;

		[FieldOffset(8)]
		public float frequency;

		public static FourCC Type
		{
			get
			{
				return new FourCC('S', 'S', 'P', 'L');
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return Type;
			}
		}

		public static SetSamplingFrequencyCommand Create(float frequency)
		{
			SetSamplingFrequencyCommand result = default(SetSamplingFrequencyCommand);
			result.baseCommand = new InputDeviceCommand(Type, 12);
			result.frequency = frequency;
			return result;
		}
	}
}
