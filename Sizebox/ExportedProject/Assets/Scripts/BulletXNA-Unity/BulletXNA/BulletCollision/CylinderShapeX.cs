using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class CylinderShapeX : CylinderShape
	{
		public CylinderShapeX(IndexedVector3 halfExtents)
			: this(ref halfExtents)
		{
		}

		public CylinderShapeX(ref IndexedVector3 halfExtents)
			: base(ref halfExtents)
		{
			m_upAxis = 0;
		}

		public override IndexedVector3 LocalGetSupportingVertexWithoutMargin(ref IndexedVector3 vec)
		{
			return CylinderLocalSupportX(GetHalfExtentsWithoutMargin(), vec);
		}

		public override void BatchedUnitVectorGetSupportingVertexWithoutMargin(IndexedVector3[] vectors, IndexedVector4[] supportVerticesOut, int numVectors)
		{
			IndexedVector3 halfExtentsWithoutMargin = GetHalfExtentsWithoutMargin();
			for (int i = 0; i < numVectors; i++)
			{
				supportVerticesOut[i] = new IndexedVector4(CylinderLocalSupportX(halfExtentsWithoutMargin, vectors[i]), 0f);
			}
		}

		public override string GetName()
		{
			return "CylinderX";
		}

		public override float GetRadius()
		{
			return GetHalfExtentsWithMargin().Y;
		}
	}
}
