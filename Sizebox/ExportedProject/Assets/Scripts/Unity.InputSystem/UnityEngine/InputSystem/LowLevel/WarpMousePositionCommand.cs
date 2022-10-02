using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 16)]
	internal struct WarpMousePositionCommand : IInputDeviceCommandInfo
	{
		internal const int kSize = 16;

		[FieldOffset(0)]
		public InputDeviceCommand baseCommand;

		[FieldOffset(8)]
		public Vector2 warpPositionInPlayerDisplaySpace;

		public static FourCC Type
		{
			get
			{
				return new FourCC('W', 'P', 'M', 'S');
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return Type;
			}
		}

		public static WarpMousePositionCommand Create(Vector2 position)
		{
			WarpMousePositionCommand result = default(WarpMousePositionCommand);
			result.baseCommand = new InputDeviceCommand(Type, 16);
			result.warpPositionInPlayerDisplaySpace = position;
			return result;
		}
	}
}
