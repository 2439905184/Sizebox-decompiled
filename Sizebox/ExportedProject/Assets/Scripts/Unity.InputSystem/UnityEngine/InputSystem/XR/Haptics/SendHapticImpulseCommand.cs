using System.Runtime.InteropServices;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.XR.Haptics
{
	[StructLayout(LayoutKind.Explicit, Size = 20)]
	public struct SendHapticImpulseCommand : IInputDeviceCommandInfo
	{
		private const int kSize = 20;

		[FieldOffset(0)]
		private InputDeviceCommand baseCommand;

		[FieldOffset(8)]
		private int channel;

		[FieldOffset(12)]
		private float amplitude;

		[FieldOffset(16)]
		private float duration;

		private static FourCC Type
		{
			get
			{
				return new FourCC('X', 'H', 'I', '0');
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return Type;
			}
		}

		public static SendHapticImpulseCommand Create(int motorChannel, float motorAmplitude, float motorDuration)
		{
			SendHapticImpulseCommand result = default(SendHapticImpulseCommand);
			result.baseCommand = new InputDeviceCommand(Type, 20);
			result.channel = motorChannel;
			result.amplitude = motorAmplitude;
			result.duration = motorDuration;
			return result;
		}
	}
}
