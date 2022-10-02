using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public abstract class PolyhedralConvexAabbCachingShape : PolyhedralConvexShape
	{
		protected IndexedVector3 m_localAabbMin;

		protected IndexedVector3 m_localAabbMax;

		protected bool m_isLocalAabbValid;

		public PolyhedralConvexAabbCachingShape()
		{
			m_localAabbMin = new IndexedVector3(1f);
			m_localAabbMax = new IndexedVector3(-1f);
			m_isLocalAabbValid = false;
		}

		protected void SetCachedLocalAabb(ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax)
		{
			m_isLocalAabbValid = true;
			m_localAabbMin = aabbMin;
			m_localAabbMax = aabbMax;
		}

		protected void GetCachedLocalAabb(out IndexedVector3 aabbMin, out IndexedVector3 aabbMax)
		{
			aabbMin = m_localAabbMin;
			aabbMax = m_localAabbMax;
		}

		public void GetNonvirtualAabb(ref IndexedMatrix trans, out IndexedVector3 aabbMin, out IndexedVector3 aabbMax, float margin)
		{
			AabbUtil2.TransformAabb(ref m_localAabbMin, ref m_localAabbMax, margin, ref trans, out aabbMin, out aabbMax);
		}

		public override void GetAabb(ref IndexedMatrix trans, out IndexedVector3 aabbMin, out IndexedVector3 aabbMax)
		{
			GetNonvirtualAabb(ref trans, out aabbMin, out aabbMax, GetMargin());
		}

		public override void SetLocalScaling(ref IndexedVector3 scaling)
		{
			base.SetLocalScaling(ref scaling);
			RecalcLocalAabb();
		}

		public void RecalcLocalAabb()
		{
			m_isLocalAabbValid = true;
			IndexedVector3[] vectors = new IndexedVector3[6]
			{
				new IndexedVector3(1f, 0f, 0f),
				new IndexedVector3(0f, 1f, 0f),
				new IndexedVector3(0f, 0f, 1f),
				new IndexedVector3(-1f, 0f, 0f),
				new IndexedVector3(0f, -1f, 0f),
				new IndexedVector3(0f, 0f, -1f)
			};
			IndexedVector4[] array = new IndexedVector4[6];
			BatchedUnitVectorGetSupportingVertexWithoutMargin(vectors, array, 6);
			for (int i = 0; i < 3; i++)
			{
				IndexedVector3 indexedVector = new IndexedVector3(array[i]);
				m_localAabbMax[i] = indexedVector[i] + m_collisionMargin;
				indexedVector = new IndexedVector3(array[i + 3]);
				m_localAabbMin[i] = indexedVector[i] - m_collisionMargin;
			}
		}
	}
}
