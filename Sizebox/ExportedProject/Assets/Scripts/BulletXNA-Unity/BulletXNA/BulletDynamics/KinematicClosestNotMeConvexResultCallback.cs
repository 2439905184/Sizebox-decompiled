using BulletXNA.BulletCollision;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletDynamics
{
	public class KinematicClosestNotMeConvexResultCallback : ClosestConvexResultCallback
	{
		protected CollisionObject m_me;

		protected IndexedVector3 m_up;

		protected float m_minSlopeDot;

		public KinematicClosestNotMeConvexResultCallback(CollisionObject me, IndexedVector3 up, float minSlopeDot)
			: base(IndexedVector3.Zero, IndexedVector3.Zero)
		{
			m_me = me;
			m_up = up;
			m_minSlopeDot = minSlopeDot;
		}

		public override float AddSingleResult(ref LocalConvexResult convexResult, bool normalInWorldSpace)
		{
			if (convexResult.m_hitCollisionObject == m_me)
			{
				return 1f;
			}
			float num = IndexedVector3.Dot(b: (!normalInWorldSpace) ? (convexResult.m_hitCollisionObject.GetWorldTransform()._basis * convexResult.m_hitNormalLocal) : convexResult.m_hitNormalLocal, a: m_up);
			if (num < m_minSlopeDot)
			{
				return 1f;
			}
			return base.AddSingleResult(ref convexResult, normalInWorldSpace);
		}
	}
}
