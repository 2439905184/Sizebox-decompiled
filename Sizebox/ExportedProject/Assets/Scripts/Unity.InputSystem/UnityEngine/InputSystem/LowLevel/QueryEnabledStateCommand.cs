using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 9)]
	public struct QueryEnabledStateCommand : IInputDeviceCommandInfo
	{
		internal const int kSize = 9;

		[FieldOffset(0)]
		public InputDeviceCommand baseCommand;

		[FieldOffset(8)]
		public bool isEnabled;

		public static FourCC Type
		{
			get
			{
				return new FourCC('Q', 'E', 'N', 'B');
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return Type;
			}
		}

		public static QueryEnabledStateCommand Create()
		{
			QueryEnabledStateCommand result = default(QueryEnabledStateCommand);
			result.baseCommand = new InputDeviceCommand(Type, 9);
			return result;
		}
	}
}
