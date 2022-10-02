namespace BulletXNA.BulletCollision
{
	public class LocalInfoAdder : ConvexResultCallback
	{
		public ConvexResultCallback m_userCallback;

		public int m_i;

		public LocalInfoAdder(int i, ConvexResultCallback user)
		{
			m_userCallback = user;
			m_i = i;
			m_closestHitFraction = user.m_closestHitFraction;
		}

		public override bool NeedsCollision(BroadphaseProxy proxy0)
		{
			return m_userCallback.NeedsCollision(proxy0);
		}

		public override float AddSingleResult(ref LocalConvexResult r, bool b)
		{
			LocalShapeInfo localShapeInfo = default(LocalShapeInfo);
			localShapeInfo.m_shapePart = -1;
			localShapeInfo.m_triangleIndex = m_i;
			r.m_localShapeInfo = localShapeInfo;
			float result = m_userCallback.AddSingleResult(ref r, b);
			m_closestHitFraction = m_userCallback.m_closestHitFraction;
			return result;
		}
	}
}
