using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class UniformScalingShape : ConvexShape
	{
		private ConvexShape m_childConvexShape;

		private float m_uniformScalingFactor;

		public UniformScalingShape(ConvexShape childConvexShape, float uniformScalingFactor)
		{
			m_childConvexShape = childConvexShape;
			m_uniformScalingFactor = uniformScalingFactor;
			m_shapeType = BroadphaseNativeTypes.UNIFORM_SCALING_SHAPE_PROXYTYPE;
		}

		public override void Cleanup()
		{
			base.Cleanup();
		}

		public override IndexedVector3 LocalGetSupportingVertexWithoutMargin(ref IndexedVector3 vec)
		{
			IndexedVector3 indexedVector = m_childConvexShape.LocalGetSupportingVertexWithoutMargin(ref vec);
			return indexedVector * m_uniformScalingFactor;
		}

		public override void BatchedUnitVectorGetSupportingVertexWithoutMargin(IndexedVector3[] vectors, IndexedVector4[] supportVerticesOut, int numVectors)
		{
			m_childConvexShape.BatchedUnitVectorGetSupportingVertexWithoutMargin(vectors, supportVerticesOut, numVectors);
			for (int i = 0; i < numVectors; i++)
			{
				supportVerticesOut[i] *= m_uniformScalingFactor;
			}
		}

		public override IndexedVector3 LocalGetSupportingVertex(ref IndexedVector3 vec)
		{
			IndexedVector3 indexedVector = m_childConvexShape.LocalGetSupportingVertex(ref vec);
			return indexedVector * m_uniformScalingFactor;
		}

		public override void CalculateLocalInertia(float mass, out IndexedVector3 inertia)
		{
			IndexedVector3 inertia2;
			m_childConvexShape.CalculateLocalInertia(mass, out inertia2);
			inertia = inertia2 * m_uniformScalingFactor;
		}

		public override void GetAabb(ref IndexedMatrix t, out IndexedVector3 aabbMin, out IndexedVector3 aabbMax)
		{
			GetAabbSlow(ref t, out aabbMin, out aabbMax);
		}

		public override void GetAabbSlow(ref IndexedMatrix t, out IndexedVector3 aabbMin, out IndexedVector3 aabbMax)
		{
			IndexedVector3[] array = new IndexedVector3[6]
			{
				new IndexedVector3(1f, 0f, 0f),
				new IndexedVector3(0f, 1f, 0f),
				new IndexedVector3(0f, 0f, 1f),
				new IndexedVector3(-1f, 0f, 0f),
				new IndexedVector3(0f, -1f, 0f),
				new IndexedVector3(0f, 0f, -1f)
			};
			IndexedVector4[] array2 = new IndexedVector4[6]
			{
				IndexedVector4.Zero,
				IndexedVector4.Zero,
				IndexedVector4.Zero,
				IndexedVector4.Zero,
				IndexedVector4.Zero,
				IndexedVector4.Zero
			};
			for (int i = 0; i < 6; i++)
			{
				array[i] *= t._basis;
			}
			new ObjectArray<IndexedVector4>(6);
			BatchedUnitVectorGetSupportingVertexWithoutMargin(array, array2, 6);
			IndexedVector3 indexedVector = new IndexedVector3(0f, 0f, 0f);
			IndexedVector3 indexedVector2 = new IndexedVector3(0f, 0f, 0f);
			for (int j = 0; j < 3; j++)
			{
				IndexedVector3 indexedVector3 = new IndexedVector3(array2[j].X, array2[j].Y, array2[j].Z);
				indexedVector2[j] = (t * indexedVector3)[j];
				indexedVector3 = new IndexedVector3(array2[j + 3].X, array2[j + 3].Y, array2[j + 3].Z);
				indexedVector[j] = (t * indexedVector3)[j];
			}
			IndexedVector3 indexedVector4 = new IndexedVector3(GetMargin());
			aabbMin = indexedVector - indexedVector4;
			aabbMax = indexedVector2 + indexedVector4;
		}

		public override void SetLocalScaling(ref IndexedVector3 scaling)
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
			return m_childConvexShape.GetMargin() * m_uniformScalingFactor;
		}

		public override int GetNumPreferredPenetrationDirections()
		{
			return m_childConvexShape.GetNumPreferredPenetrationDirections();
		}

		public override void GetPreferredPenetrationDirection(int index, out IndexedVector3 penetrationVector)
		{
			m_childConvexShape.GetPreferredPenetrationDirection(index, out penetrationVector);
		}

		public float GetUniformScalingFactor()
		{
			return m_uniformScalingFactor;
		}

		public ConvexShape GetChildShape()
		{
			return m_childConvexShape;
		}

		public override string GetName()
		{
			return "UniformScalingShape";
		}
	}
}
