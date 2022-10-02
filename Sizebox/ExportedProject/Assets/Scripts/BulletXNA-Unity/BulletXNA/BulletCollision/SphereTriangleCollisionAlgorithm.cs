using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class SphereTriangleCollisionAlgorithm : ActivatingCollisionAlgorithm
	{
		private bool m_ownManifold;

		private PersistentManifold m_manifoldPtr;

		private bool m_swapped;

		public SphereTriangleCollisionAlgorithm()
		{
		}

		public SphereTriangleCollisionAlgorithm(PersistentManifold mf, CollisionAlgorithmConstructionInfo ci, CollisionObject body0, CollisionObject body1, bool swapped)
			: base(ci, body0, body1)
		{
			m_ownManifold = false;
			m_manifoldPtr = mf;
			m_swapped = swapped;
		}

		public virtual void Initialize(PersistentManifold mf, CollisionAlgorithmConstructionInfo ci, CollisionObject body0, CollisionObject body1, bool swapped)
		{
			base.Initialize(ci, body0, body1);
			m_ownManifold = false;
			m_manifoldPtr = mf;
			m_swapped = swapped;
		}

		public SphereTriangleCollisionAlgorithm(CollisionAlgorithmConstructionInfo ci)
			: base(ci)
		{
		}

		public override void Initialize(CollisionAlgorithmConstructionInfo ci)
		{
			base.Initialize(ci);
		}

		public override void Cleanup()
		{
			base.Cleanup();
			if (m_ownManifold && m_manifoldPtr != null)
			{
				m_dispatcher.ReleaseManifold(m_manifoldPtr);
				m_manifoldPtr = null;
			}
			m_ownManifold = false;
			BulletGlobals.SphereTriangleCollisionAlgorithmPool.Free(this);
		}

		public override void ProcessCollision(CollisionObject body0, CollisionObject body1, DispatcherInfo dispatchInfo, ManifoldResult resultOut)
		{
			if (m_manifoldPtr == null)
			{
				return;
			}
			CollisionObject collisionObject = (m_swapped ? body1 : body0);
			CollisionObject collisionObject2 = (m_swapped ? body0 : body1);
			SphereShape sphere = collisionObject.GetCollisionShape() as SphereShape;
			TriangleShape triangle = collisionObject2.GetCollisionShape() as TriangleShape;
			resultOut.SetPersistentManifold(m_manifoldPtr);
			using (SphereTriangleDetector sphereTriangleDetector = BulletGlobals.SphereTriangleDetectorPool.Get())
			{
				sphereTriangleDetector.Initialize(sphere, triangle, m_manifoldPtr.GetContactBreakingThreshold());
				ClosestPointInput input = ClosestPointInput.Default();
				input.m_maximumDistanceSquared = float.MaxValue;
				collisionObject.GetWorldTransform(out input.m_transformA);
				collisionObject2.GetWorldTransform(out input.m_transformB);
				bool swapped = m_swapped;
				sphereTriangleDetector.GetClosestPoints(ref input, resultOut, dispatchInfo.getDebugDraw(), swapped);
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
