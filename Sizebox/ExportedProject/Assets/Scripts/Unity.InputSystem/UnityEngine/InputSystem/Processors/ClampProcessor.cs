using UnityEngine.Scripting;

namespace UnityEngine.InputSystem.Processors
{
	[Preserve]
	internal class ClampProcessor : InputProcessor<float>
	{
		public float min;

		public float max;

		public override float Process(float value, InputControl control)
		{
			return Mathf.Clamp(value, min, max);
		}
	}
}
