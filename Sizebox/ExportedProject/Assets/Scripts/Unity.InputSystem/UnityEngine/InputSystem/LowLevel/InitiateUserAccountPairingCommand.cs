using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 8)]
	public struct InitiateUserAccountPairingCommand : IInputDeviceCommandInfo
	{
		public enum Result
		{
			SuccessfullyInitiated = 1,
			ErrorNotSupported = -1,
			ErrorAlreadyInProgress = -2
		}

		internal const int kSize = 8;

		[FieldOffset(0)]
		public InputDeviceCommand baseCommand;

		public static FourCC Type
		{
			get
			{
				return new FourCC('P', 'A', 'I', 'R');
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return Type;
			}
		}

		public static InitiateUserAccountPairingCommand Create()
		{
			InitiateUserAccountPairingCommand result = default(InitiateUserAccountPairingCommand);
			result.baseCommand = new InputDeviceCommand(Type);
			return result;
		}
	}
}
