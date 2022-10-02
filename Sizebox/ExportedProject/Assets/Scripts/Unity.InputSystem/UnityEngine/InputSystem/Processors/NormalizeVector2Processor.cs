using UnityEngine.Scripting;

namespace UnityEngine.InputSystem.Processors
{
	[Preserve]
	internal class NormalizeVector2Processor : InputProcessor<Vector2>
	{
		public override Vector2 Process(Vector2 value, InputControl control)
		{
			return value.normalized;
		}
	}
}
