using System.Runtime.InteropServices;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.XR.Haptics
{
	[StructLayout(LayoutKind.Explicit, Size = 16)]
	public struct GetCurrentHapticStateCommand : IInputDeviceCommandInfo
	{
		private const int kSize = 16;

		[FieldOffset(0)]
		private InputDeviceCommand baseCommand;

		[FieldOffset(8)]
		public uint samplesQueued;

		[FieldOffset(12)]
		public uint samplesAvailable;

		private static FourCC Type
		{
			get
			{
				return new FourCC('X', 'H', 'S', '0');
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return Type;
			}
		}

		public HapticState currentState
		{
			get
			{
				return new HapticState(samplesQueued, samplesAvailable);
			}
		}

		public static GetCurrentHapticStateCommand Create()
		{
			GetCurrentHapticStateCommand result = default(GetCurrentHapticStateCommand);
			result.baseCommand = new InputDeviceCommand(Type, 16);
			return result;
		}
	}
}
