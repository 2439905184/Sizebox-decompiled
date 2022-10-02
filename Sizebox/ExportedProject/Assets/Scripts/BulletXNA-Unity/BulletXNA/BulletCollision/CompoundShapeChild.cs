using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class CompoundShapeChild
	{
		public IndexedMatrix m_transform;

		public CollisionShape m_childShape;

		public BroadphaseNativeTypes m_childShapeType;

		public float m_childMargin;

		public DbvtNode m_treeNode;

		public override bool Equals(object obj)
		{
			CompoundShapeChild compoundShapeChild = (CompoundShapeChild)obj;
			if (m_transform == compoundShapeChild.m_transform && m_childShape == compoundShapeChild.m_childShape && m_childShapeType == compoundShapeChild.m_childShapeType)
			{
				return m_childMargin == compoundShapeChild.m_childMargin;
			}
			return false;
		}
	}
}
