using BulletXNA.BulletCollision;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletDynamics
{
	public class KinematicClosestNotMeRayResultCallback : ClosestRayResultCallback
	{
		protected CollisionObject m_me;

		public KinematicClosestNotMeRayResultCallback(CollisionObject me)
			: base(IndexedVector3.Zero, IndexedVector3.Zero)
		{
			m_me = me;
		}

		public override float AddSingleResult(ref LocalRayResult rayResult, bool normalInWorldSpace)
		{
			if (rayResult.m_collisionObject == m_me)
			{
				return 1f;
			}
			return base.AddSingleResult(ref rayResult, normalInWorldSpace);
		}
	}
}
