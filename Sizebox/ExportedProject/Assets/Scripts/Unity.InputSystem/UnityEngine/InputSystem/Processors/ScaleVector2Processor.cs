using UnityEngine.Scripting;

namespace UnityEngine.InputSystem.Processors
{
	[Preserve]
	public class ScaleVector2Processor : InputProcessor<Vector2>
	{
		[Tooltip("Scale factor to multiple the incoming Vector2's X component by.")]
		public float x = 1f;

		[Tooltip("Scale factor to multiple the incoming Vector2's Y component by.")]
		public float y = 1f;

		public override Vector2 Process(Vector2 value, InputControl control)
		{
			return new Vector2(value.x * x, value.y * y);
		}
	}
}
