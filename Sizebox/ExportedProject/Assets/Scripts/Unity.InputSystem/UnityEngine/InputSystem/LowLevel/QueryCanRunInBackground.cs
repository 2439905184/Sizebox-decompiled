using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 9)]
	public struct QueryCanRunInBackground : IInputDeviceCommandInfo
	{
		internal const int kSize = 8;

		[FieldOffset(0)]
		public InputDeviceCommand baseCommand;

		[FieldOffset(8)]
		public bool canRunInBackground;

		public static FourCC Type
		{
			get
			{
				return new FourCC('Q', 'R', 'I', 'B');
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return Type;
			}
		}

		public static QueryCanRunInBackground Create()
		{
			QueryCanRunInBackground result = default(QueryCanRunInBackground);
			result.baseCommand = new InputDeviceCommand(Type);
			result.canRunInBackground = false;
			return result;
		}
	}
}
