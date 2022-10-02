using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	internal struct PointerState : IInputStateTypeInfo
	{
		private uint pointerId;

		[InputControl(layout = "Vector2", displayName = "Position", usage = "Point")]
		public Vector2 position;

		[InputControl(layout = "Vector2", displayName = "Delta", usage = "Secondary2DMotion")]
		public Vector2 delta;

		[InputControl(layout = "Analog", displayName = "Pressure", usage = "Pressure")]
		public float pressure;

		[InputControl(layout = "Vector2", displayName = "Radius", usage = "Radius")]
		public Vector2 radius;

		[InputControl(name = "press", displayName = "Press", layout = "Button", format = "BIT", bit = 0u)]
		public ushort buttons;

		public static FourCC kFormat
		{
			get
			{
				return new FourCC('P', 'T', 'R');
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
