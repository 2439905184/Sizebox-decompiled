using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public abstract class ConvexInternalShape : ConvexShape
	{
		protected IndexedVector3 m_localScaling;

		protected IndexedVector3 m_implicitShapeDimensions;

		protected float m_collisionMargin;

		protected float m_padding;

		public ConvexInternalShape()
		{
			m_localScaling = new IndexedVector3(1f);
			m_collisionMargin = 0.04f;
		}

		public override void Cleanup()
		{
			base.Cleanup();
		}

		public override IndexedVector3 LocalGetSupportingVertex(ref IndexedVector3 vec)
		{
			IndexedVector3 result = LocalGetSupportingVertexWithoutMargin(ref vec);
			if (GetMargin() != 0f)
			{
				IndexedVector3 indexedVector = vec;
				if (indexedVector.LengthSquared() < 1.4210855E-14f)
				{
					indexedVector = new IndexedVector3(-1f);
				}
				indexedVector.Normalize();
				result += GetMargin() * indexedVector;
			}
			return result;
		}

		public IndexedVector3 GetImplicitShapeDimensions()
		{
			return m_implicitShapeDimensions;
		}

		public void SetSafeMargin(float minDimension)
		{
			SetSafeMargin(minDimension, 0.1f);
		}

		public void SetSafeMargin(float minDimension, float defaultMarginMultiplier)
		{
			float num = defaultMarginMultiplier * minDimension;
			if (num < GetMargin())
			{
				SetMargin(num);
			}
		}

		public void SetSafeMargin(ref IndexedVector3 halfExtents)
		{
			SetSafeMargin(ref halfExtents, 0.1f);
		}

		public void SetSafeMargin(ref IndexedVector3 halfExtents, float defaultMarginMultiplier)
		{
			float minDimension = halfExtents[halfExtents.MinAxis()];
			SetSafeMargin(minDimension, defaultMarginMultiplier);
		}

		public override void GetAabb(ref IndexedMatrix t, out IndexedVector3 aabbMin, out IndexedVector3 aabbMax)
		{
			GetAabbSlow(ref t, out aabbMin, out aabbMax);
		}

		public override void GetAabbSlow(ref IndexedMatrix trans, out IndexedVector3 aabbMin, out IndexedVector3 aabbMax)
		{
			float margin = GetMargin();
			aabbMin = IndexedVector3.Zero;
			aabbMax = IndexedVector3.Zero;
			for (int i = 0; i < 3; i++)
			{
				IndexedVector3 indexedVector = default(IndexedVector3);
				indexedVector[i] = 1f;
				IndexedVector3 vec = indexedVector * trans._basis;
				IndexedVector3 indexedVector2 = LocalGetSupportingVertex(ref vec);
				aabbMax[i] = (trans * indexedVector2)[i] + margin;
				indexedVector[i] = -1f;
				vec = indexedVector * trans._basis;
				indexedVector2 = LocalGetSupportingVertex(ref vec);
				aabbMin[i] = (trans * indexedVector2)[i] - margin;
			}
		}

		public override void SetLocalScaling(ref IndexedVector3 scaling)
		{
			m_localScaling = scaling.Absolute();
		}

		public override IndexedVector3 GetLocalScaling()
		{
			return m_localScaling;
		}

		public IndexedVector3 GetLocalScalingNV()
		{
			return m_localScaling;
		}

		public override void SetMargin(float margin)
		{
			m_collisionMargin = margin;
		}

		public override float GetMargin()
		{
			return m_collisionMargin;
		}

		public float GetMarginNV()
		{
			return m_collisionMargin;
		}

		public override int GetNumPreferredPenetrationDirections()
		{
			return 0;
		}

		public override void GetPreferredPenetrationDirection(int index, out IndexedVector3 penetrationVector)
		{
			penetrationVector = IndexedVector3.Zero;
		}
	}
}
