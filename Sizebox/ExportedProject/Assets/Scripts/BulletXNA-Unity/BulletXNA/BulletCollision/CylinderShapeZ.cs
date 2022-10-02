using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class CylinderShapeZ : CylinderShape
	{
		public CylinderShapeZ(IndexedVector3 halfExtents)
			: this(ref halfExtents)
		{
		}

		public CylinderShapeZ(ref IndexedVector3 halfExtents)
			: base(ref halfExtents)
		{
			m_upAxis = 2;
		}

		public override IndexedVector3 LocalGetSupportingVertexWithoutMargin(ref IndexedVector3 vec)
		{
			return CylinderLocalSupportZ(GetHalfExtentsWithoutMargin(), vec);
		}

		public override void BatchedUnitVectorGetSupportingVertexWithoutMargin(IndexedVector3[] vectors, IndexedVector4[] supportVerticesOut, int numVectors)
		{
			IndexedVector3 halfExtentsWithoutMargin = GetHalfExtentsWithoutMargin();
			for (int i = 0; i < numVectors; i++)
			{
				supportVerticesOut[i] = new IndexedVector4(CylinderLocalSupportZ(halfExtentsWithoutMargin, vectors[i]), 0f);
			}
		}

		public override string GetName()
		{
			return "CylinderZ";
		}

		public override float GetRadius()
		{
			return GetHalfExtentsWithMargin().X;
		}
	}
}
