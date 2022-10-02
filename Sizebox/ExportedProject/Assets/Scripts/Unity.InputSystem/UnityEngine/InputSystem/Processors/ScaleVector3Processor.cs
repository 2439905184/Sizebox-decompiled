using UnityEngine.Scripting;

namespace UnityEngine.InputSystem.Processors
{
	[Preserve]
	public class ScaleVector3Processor : InputProcessor<Vector3>
	{
		public float x = 1f;

		public float y = 1f;

		public float z = 1f;

		public override Vector3 Process(Vector3 value, InputControl control)
		{
			return new Vector3(value.x * x, value.y * y, value.z * z);
		}
	}
}
