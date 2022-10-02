using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 9)]
	public struct EnableIMECompositionCommand : IInputDeviceCommandInfo
	{
		internal const int kSize = 12;

		[FieldOffset(0)]
		public InputDeviceCommand baseCommand;

		[FieldOffset(8)]
		private byte m_ImeEnabled;

		public static FourCC Type
		{
			get
			{
				return new FourCC('I', 'M', 'E', 'M');
			}
		}

		public bool imeEnabled
		{
			get
			{
				return m_ImeEnabled != 0;
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return Type;
			}
		}

		public static EnableIMECompositionCommand Create(bool enabled)
		{
			EnableIMECompositionCommand result = default(EnableIMECompositionCommand);
			result.baseCommand = new InputDeviceCommand(Type, 9);
			result.m_ImeEnabled = (byte)(enabled ? byte.MaxValue : 0);
			return result;
		}
	}
}
