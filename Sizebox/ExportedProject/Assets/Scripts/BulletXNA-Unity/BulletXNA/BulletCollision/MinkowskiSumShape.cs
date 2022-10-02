using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class MinkowskiSumShape : ConvexInternalShape
	{
		private IndexedMatrix m_transA;

		private IndexedMatrix m_transB;

		private ConvexShape m_shapeA;

		private ConvexShape m_shapeB;

		public MinkowskiSumShape(ConvexShape shapeA, ConvexShape shapeB)
		{
			m_shapeA = shapeA;
			m_shapeB = shapeB;
			m_transA = IndexedMatrix.Identity;
			m_transB = IndexedMatrix.Identity;
			m_shapeType = BroadphaseNativeTypes.MINKOWSKI_DIFFERENCE_SHAPE_PROXYTYPE;
		}

		public override IndexedVector3 LocalGetSupportingVertexWithoutMargin(ref IndexedVector3 vec)
		{
			IndexedVector3 vec2 = vec * m_transA._basis;
			IndexedVector3 indexedVector = m_transA * m_shapeA.LocalGetSupportingVertexWithoutMargin(ref vec2);
			vec2 = -vec * m_transB._basis;
			IndexedVector3 indexedVector2 = m_transB * m_shapeB.LocalGetSupportingVertexWithoutMargin(ref vec2);
			return indexedVector - indexedVector2;
		}

		public override void BatchedUnitVectorGetSupportingVertexWithoutMargin(IndexedVector3[] vectors, IndexedVector4[] supportVerticesOut, int numVectors)
		{
			for (int i = 0; i < numVectors; i++)
			{
				IndexedVector3 vec = vectors[i];
				supportVerticesOut[i] = new IndexedVector4(LocalGetSupportingVertexWithoutMargin(ref vec), 0f);
			}
		}

		public override void CalculateLocalInertia(float mass, out IndexedVector3 inertia)
		{
			inertia = IndexedVector3.Zero;
		}

		public void SetTransformA(ref IndexedMatrix transA)
		{
			m_transA = transA;
		}

		public void SetTransformB(ref IndexedMatrix transB)
		{
			m_transB = transB;
		}

		public IndexedMatrix GetTransformA()
		{
			return m_transA;
		}

		public IndexedMatrix GetTransformB()
		{
			return m_transB;
		}

		public override float GetMargin()
		{
			return m_shapeA.GetMargin() + m_shapeB.GetMargin();
		}

		public ConvexShape GetShapeA()
		{
			return m_shapeA;
		}

		public ConvexShape GetShapeB()
		{
			return m_shapeB;
		}

		public override string GetName()
		{
			return "MinkowskiSum";
		}
	}
}
