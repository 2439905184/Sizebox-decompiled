using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 12)]
	internal struct QuerySamplingFrequencyCommand : IInputDeviceCommandInfo
	{
		internal const int kSize = 12;

		[FieldOffset(0)]
		public InputDeviceCommand baseCommand;

		[FieldOffset(8)]
		public float frequency;

		public static FourCC Type
		{
			get
			{
				return new FourCC('S', 'M', 'P', 'L');
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return Type;
			}
		}

		public static QuerySamplingFrequencyCommand Create()
		{
			QuerySamplingFrequencyCommand result = default(QuerySamplingFrequencyCommand);
			result.baseCommand = new InputDeviceCommand(Type, 12);
			return result;
		}
	}
}
