namespace BulletXNA.BulletCollision
{
	public class LocalInfoAdder2 : RayResultCallback
	{
		public int m_i;

		public RayResultCallback m_userCallback;

		public LocalInfoAdder2(int i, RayResultCallback user)
		{
			m_i = i;
			m_userCallback = user;
			m_closestHitFraction = user.m_closestHitFraction;
		}

		public override float AddSingleResult(ref LocalRayResult r, bool b)
		{
			LocalShapeInfo localShapeInfo = default(LocalShapeInfo);
			localShapeInfo.m_shapePart = -1;
			localShapeInfo.m_triangleIndex = m_i;
			r.m_localShapeInfo = localShapeInfo;
			float result = m_userCallback.AddSingleResult(ref r, b);
			m_closestHitFraction = m_userCallback.m_closestHitFraction;
			return result;
		}

		public override bool NeedsCollision(BroadphaseProxy proxy0)
		{
			return m_userCallback.NeedsCollision(proxy0);
		}

		public virtual void cleanup()
		{
		}
	}
}
