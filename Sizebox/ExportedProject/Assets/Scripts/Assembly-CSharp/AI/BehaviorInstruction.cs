using UnityEngine;

namespace AI
{
	public struct BehaviorInstruction
	{
		public IBehavior newBehavior;

		public EntityBase target;

		public Vector3 cursorPoint;
	}
}
