using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class DbvtProxy : BroadphaseProxy
	{
		public DbvtNode m_leaf;

		public DbvtProxy[] links = new DbvtProxy[2];

		public int stage;

		public DbvtNode leaf
		{
			get
			{
				return m_leaf;
			}
			set
			{
				m_leaf = value;
				DbvtNode parent = m_leaf.parent;
			}
		}

		public DbvtProxy(ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax, object userPtr, CollisionFilterGroups collisionFilterGroup, CollisionFilterGroups collisionFilterMask)
			: base(ref aabbMin, ref aabbMax, userPtr, collisionFilterGroup, collisionFilterMask, null)
		{
			links[0] = (links[1] = null);
		}
	}
}
