using UnityEngine.Scripting;

namespace UnityEngine.InputSystem.Processors
{
	[Preserve]
	internal class NormalizeVector3Processor : InputProcessor<Vector3>
	{
		public override Vector3 Process(Vector3 value, InputControl control)
		{
			return value.normalized;
		}
	}
}
