using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class Convex2dShape : ConvexShape
	{
		private ConvexShape m_childConvexShape;

		public Convex2dShape(ConvexShape convexChildShape)
		{
			m_childConvexShape = convexChildShape;
			m_shapeType = BroadphaseNativeTypes.CONVEX_2D_SHAPE_PROXYTYPE;
		}

		public override void Cleanup()
		{
			base.Cleanup();
		}

		public override IndexedVector3 LocalGetSupportingVertexWithoutMargin(ref IndexedVector3 vec)
		{
			return m_childConvexShape.LocalGetSupportingVertexWithoutMargin(ref vec);
		}

		public override IndexedVector3 LocalGetSupportingVertex(ref IndexedVector3 vec)
		{
			return m_childConvexShape.LocalGetSupportingVertex(ref vec);
		}

		public override void BatchedUnitVectorGetSupportingVertexWithoutMargin(IndexedVector3[] vectors, IndexedVector4[] supportVerticesOut, int numVectors)
		{
			m_childConvexShape.BatchedUnitVectorGetSupportingVertexWithoutMargin(vectors, supportVerticesOut, numVectors);
		}

		public override void CalculateLocalInertia(float mass, out IndexedVector3 inertia)
		{
			m_childConvexShape.CalculateLocalInertia(mass, out inertia);
		}

		public ConvexShape GetChildShape()
		{
			return m_childConvexShape;
		}

		public override string GetName()
		{
			return "Convex2dShape";
		}

		public override void GetAabb(ref IndexedMatrix t, out IndexedVector3 aabbMin, out IndexedVector3 aabbMax)
		{
			m_childConvexShape.GetAabb(ref t, out aabbMin, out aabbMax);
		}

		public override void GetAabbSlow(ref IndexedMatrix t, out IndexedVector3 aabbMin, out IndexedVector3 aabbMax)
		{
			m_childConvexShape.GetAabbSlow(ref t, out aabbMin, out aabbMax);
		}

		public void SetLocalScaling(IndexedVector3 scaling)
		{
			m_childConvexShape.SetLocalScaling(ref scaling);
		}

		public override IndexedVector3 GetLocalScaling()
		{
			return m_childConvexShape.GetLocalScaling();
		}

		public override void SetMargin(float margin)
		{
			m_childConvexShape.SetMargin(margin);
		}

		public override float GetMargin()
		{
			return m_childConvexShape.GetMargin();
		}

		public override int GetNumPreferredPenetrationDirections()
		{
			return m_childConvexShape.GetNumPreferredPenetrationDirections();
		}

		public override void GetPreferredPenetrationDirection(int index, out IndexedVector3 penetrationVector)
		{
			m_childConvexShape.GetPreferredPenetrationDirection(index, out penetrationVector);
		}
	}
}
