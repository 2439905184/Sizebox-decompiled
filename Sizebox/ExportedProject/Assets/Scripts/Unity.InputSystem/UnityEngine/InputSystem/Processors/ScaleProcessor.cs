using UnityEngine.Scripting;

namespace UnityEngine.InputSystem.Processors
{
	[Preserve]
	internal class ScaleProcessor : InputProcessor<float>
	{
		[Tooltip("Scale factor to multiply incoming float values by.")]
		public float factor = 1f;

		public override float Process(float value, InputControl control)
		{
			return value * factor;
		}
	}
}
