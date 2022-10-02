using System.Runtime.InteropServices;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.XR.Haptics
{
	[StructLayout(LayoutKind.Explicit, Size = 20)]
	public struct GetHapticCapabilitiesCommand : IInputDeviceCommandInfo
	{
		private const int kSize = 20;

		[FieldOffset(0)]
		private InputDeviceCommand baseCommand;

		[FieldOffset(8)]
		public uint numChannels;

		[FieldOffset(12)]
		public uint frequencyHz;

		[FieldOffset(16)]
		public uint maxBufferSize;

		private static FourCC Type
		{
			get
			{
				return new FourCC('X', 'H', 'C', '0');
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return Type;
			}
		}

		public HapticCapabilities capabilities
		{
			get
			{
				return new HapticCapabilities(numChannels, frequencyHz, maxBufferSize);
			}
		}

		public static GetHapticCapabilitiesCommand Create()
		{
			GetHapticCapabilitiesCommand result = default(GetHapticCapabilitiesCommand);
			result.baseCommand = new InputDeviceCommand(Type, 20);
			return result;
		}
	}
}
