using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class CompoundPrimitiveManager : IPrimitiveManagerBase
	{
		public GImpactCompoundShape m_compoundShape;

		public virtual void Cleanup()
		{
		}

		public CompoundPrimitiveManager(CompoundPrimitiveManager compound)
		{
			m_compoundShape = compound.m_compoundShape;
		}

		public CompoundPrimitiveManager(GImpactCompoundShape compoundShape)
		{
			m_compoundShape = compoundShape;
		}

		public CompoundPrimitiveManager()
		{
			m_compoundShape = null;
		}

		public virtual bool IsTrimesh()
		{
			return false;
		}

		public virtual int GetPrimitiveCount()
		{
			return m_compoundShape.GetNumChildShapes();
		}

		public virtual void GetPrimitiveBox(int prim_index, out AABB primbox)
		{
			IndexedMatrix t = ((!m_compoundShape.ChildrenHasTransform()) ? IndexedMatrix.Identity : m_compoundShape.GetChildTransform(prim_index));
			CollisionShape childShape = m_compoundShape.GetChildShape(prim_index);
			childShape.GetAabb(t, out primbox.m_min, out primbox.m_max);
		}

		public virtual void GetPrimitiveTriangle(int prim_index, PrimitiveTriangle triangle)
		{
		}
	}
}
