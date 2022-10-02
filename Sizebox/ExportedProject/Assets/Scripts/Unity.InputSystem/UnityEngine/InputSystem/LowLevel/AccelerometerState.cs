using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	internal struct AccelerometerState : IInputStateTypeInfo
	{
		[InputControl(displayName = "Acceleration", processors = "CompensateDirection", noisy = true)]
		public Vector3 acceleration;

		public static FourCC kFormat
		{
			get
			{
				return new FourCC('A', 'C', 'C', 'L');
			}
		}

		public FourCC format
		{
			get
			{
				return kFormat;
			}
		}
	}
}
