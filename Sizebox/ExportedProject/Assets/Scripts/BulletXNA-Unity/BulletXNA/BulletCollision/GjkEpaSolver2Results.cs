using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public struct GjkEpaSolver2Results
	{
		public GjkEpaSolver2Status status;

		public IndexedVector3 witnesses0;

		public IndexedVector3 witnesses1;

		public IndexedVector3 normal;

		public float distance;
	}
}
