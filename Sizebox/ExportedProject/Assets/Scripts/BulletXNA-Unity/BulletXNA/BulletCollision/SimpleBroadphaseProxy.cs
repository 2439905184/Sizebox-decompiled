using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class SimpleBroadphaseProxy : BroadphaseProxy
	{
		private int m_nextFree;

		private int m_position;

		public SimpleBroadphaseProxy(int position)
		{
			m_position = position;
		}

		public SimpleBroadphaseProxy(int position, ref IndexedVector3 minpt, ref IndexedVector3 maxpt, BroadphaseNativeTypes shapeType, object userPtr, CollisionFilterGroups collisionFilterGroup, CollisionFilterGroups collisionFilterMask, object multiSapProxy)
			: base(ref minpt, ref maxpt, userPtr, collisionFilterGroup, collisionFilterMask, multiSapProxy)
		{
			m_position = position;
		}

		public int GetNextFree()
		{
			return m_nextFree;
		}

		public int GetPosition()
		{
			return m_position;
		}

		public void SetNextFree(int nextFree)
		{
			m_nextFree = nextFree;
		}
	}
}
