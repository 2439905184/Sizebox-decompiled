using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class OptimizedBvhNode
	{
		public IndexedVector3 m_aabbMinOrg;

		public IndexedVector3 m_aabbMaxOrg;

		public int m_escapeIndex;

		public int m_subPart;

		public int m_triangleIndex;
	}
}
