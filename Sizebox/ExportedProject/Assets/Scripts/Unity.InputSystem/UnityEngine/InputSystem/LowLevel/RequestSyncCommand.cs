using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 8)]
	public struct RequestSyncCommand : IInputDeviceCommandInfo
	{
		internal const int kSize = 8;

		[FieldOffset(0)]
		public InputDeviceCommand baseCommand;

		public static FourCC Type
		{
			get
			{
				return new FourCC('S', 'Y', 'N', 'C');
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return Type;
			}
		}

		public static RequestSyncCommand Create()
		{
			RequestSyncCommand result = default(RequestSyncCommand);
			result.baseCommand = new InputDeviceCommand(Type);
			return result;
		}
	}
}
