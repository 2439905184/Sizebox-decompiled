using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class BoxBoxCollisionAlgorithm : ActivatingCollisionAlgorithm
	{
		private bool m_ownManifold;

		private PersistentManifold m_manifoldPtr;

		public BoxBoxCollisionAlgorithm()
		{
		}

		public BoxBoxCollisionAlgorithm(CollisionAlgorithmConstructionInfo ci)
			: base(ci)
		{
		}

		public override void Initialize(CollisionAlgorithmConstructionInfo ci)
		{
			base.Initialize(ci);
		}

		public BoxBoxCollisionAlgorithm(PersistentManifold mf, CollisionAlgorithmConstructionInfo ci, CollisionObject body0, CollisionObject body1)
			: base(ci)
		{
			m_ownManifold = false;
			m_manifoldPtr = mf;
			if (m_manifoldPtr == null && m_dispatcher.NeedsCollision(body0, body1))
			{
				m_manifoldPtr = m_dispatcher.GetNewManifold(body0, body1);
				m_ownManifold = true;
			}
		}

		public void Initialize(PersistentManifold mf, CollisionAlgorithmConstructionInfo ci, CollisionObject body0, CollisionObject body1)
		{
			base.Initialize(ci);
			m_ownManifold = false;
			m_manifoldPtr = mf;
			if (m_manifoldPtr == null && m_dispatcher.NeedsCollision(body0, body1))
			{
				m_manifoldPtr = m_dispatcher.GetNewManifold(body0, body1);
				m_ownManifold = true;
			}
		}

		public override void Cleanup()
		{
			if (m_ownManifold)
			{
				if (m_manifoldPtr != null)
				{
					m_dispatcher.ReleaseManifold(m_manifoldPtr);
					m_manifoldPtr = null;
				}
				m_ownManifold = false;
			}
			BulletGlobals.BoxBoxCollisionAlgorithmPool.Free(this);
			base.Cleanup();
		}

		public override void ProcessCollision(CollisionObject body0, CollisionObject body1, DispatcherInfo dispatchInfo, ManifoldResult resultOut)
		{
			if (m_manifoldPtr != null)
			{
				BoxShape box = body0.GetCollisionShape() as BoxShape;
				BoxShape box2 = body1.GetCollisionShape() as BoxShape;
				resultOut.SetPersistentManifold(m_manifoldPtr);
				ClosestPointInput input = ClosestPointInput.Default();
				input.m_maximumDistanceSquared = float.MaxValue;
				input.m_transformA = body0.GetWorldTransform();
				input.m_transformB = body1.GetWorldTransform();
				BoxBoxDetector.GetClosestPoints(box, box2, ref input, resultOut, dispatchInfo.getDebugDraw(), false);
				if (m_ownManifold)
				{
					resultOut.RefreshContactPoints();
				}
			}
		}

		public override float CalculateTimeOfImpact(CollisionObject body0, CollisionObject body1, DispatcherInfo dispatchInfo, ManifoldResult resultOut)
		{
			return 1f;
		}

		public override void GetAllContactManifolds(PersistentManifoldArray manifoldArray)
		{
			if (m_manifoldPtr != null && m_ownManifold)
			{
				manifoldArray.Add(m_manifoldPtr);
			}
		}
	}
}
