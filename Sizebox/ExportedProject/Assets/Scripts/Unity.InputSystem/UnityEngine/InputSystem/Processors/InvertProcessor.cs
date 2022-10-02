using UnityEngine.Scripting;

namespace UnityEngine.InputSystem.Processors
{
	[Preserve]
	internal class InvertProcessor : InputProcessor<float>
	{
		public override float Process(float value, InputControl control)
		{
			return value * -1f;
		}
	}
}
