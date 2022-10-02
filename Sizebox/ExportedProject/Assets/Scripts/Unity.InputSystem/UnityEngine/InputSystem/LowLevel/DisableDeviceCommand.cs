using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 8)]
	public struct DisableDeviceCommand : IInputDeviceCommandInfo
	{
		internal const int kSize = 8;

		[FieldOffset(0)]
		public InputDeviceCommand baseCommand;

		public static FourCC Type
		{
			get
			{
				return new FourCC('D', 'S', 'B', 'L');
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return Type;
			}
		}

		public static DisableDeviceCommand Create()
		{
			DisableDeviceCommand result = default(DisableDeviceCommand);
			result.baseCommand = new InputDeviceCommand(Type);
			return result;
		}
	}
}
