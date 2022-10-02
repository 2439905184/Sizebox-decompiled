using UnityEngine.Scripting;

namespace UnityEngine.InputSystem.Processors
{
	[Preserve]
	internal class InvertVector2Processor : InputProcessor<Vector2>
	{
		public bool invertX = true;

		public bool invertY = true;

		public override Vector2 Process(Vector2 value, InputControl control)
		{
			if (invertX)
			{
				value.x *= -1f;
			}
			if (invertY)
			{
				value.y *= -1f;
			}
			return value;
		}
	}
}
