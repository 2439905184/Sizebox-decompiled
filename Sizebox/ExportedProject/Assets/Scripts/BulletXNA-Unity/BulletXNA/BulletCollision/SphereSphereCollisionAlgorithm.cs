using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class SphereSphereCollisionAlgorithm : ActivatingCollisionAlgorithm
	{
		public bool m_ownManifold;

		public PersistentManifold m_manifoldPtr;

		public SphereSphereCollisionAlgorithm()
		{
		}

		public SphereSphereCollisionAlgorithm(PersistentManifold mf, CollisionAlgorithmConstructionInfo ci, CollisionObject body0, CollisionObject body1)
			: base(ci, body0, body1)
		{
			m_ownManifold = false;
			m_manifoldPtr = mf;
			if (m_manifoldPtr == null)
			{
				m_manifoldPtr = m_dispatcher.GetNewManifold(body0, body1);
				m_ownManifold = true;
			}
		}

		public virtual void Initialize(PersistentManifold mf, CollisionAlgorithmConstructionInfo ci, CollisionObject colObj0, CollisionObject colObj1)
		{
			base.Initialize(ci, colObj0, colObj1);
			m_ownManifold = false;
			m_manifoldPtr = mf;
			if (m_manifoldPtr == null)
			{
				m_manifoldPtr = m_dispatcher.GetNewManifold(colObj0, colObj1);
				m_ownManifold = true;
			}
		}

		public SphereSphereCollisionAlgorithm(CollisionAlgorithmConstructionInfo ci)
			: base(ci)
		{
		}

		public override void Initialize(CollisionAlgorithmConstructionInfo ci)
		{
			base.Initialize(ci);
		}

		public override void ProcessCollision(CollisionObject body0, CollisionObject body1, DispatcherInfo dispatchInfo, ManifoldResult resultOut)
		{
			if (m_manifoldPtr == null)
			{
				return;
			}
			resultOut.SetPersistentManifold(m_manifoldPtr);
			SphereShape sphereShape = body0.GetCollisionShape() as SphereShape;
			SphereShape sphereShape2 = body1.GetCollisionShape() as SphereShape;
			IndexedVector3 indexedVector = body0.GetWorldTransform()._origin - body1.GetWorldTransform()._origin;
			float num = indexedVector.Length();
			float radius = sphereShape.GetRadius();
			float radius2 = sphereShape2.GetRadius();
			if (num > radius + radius2)
			{
				resultOut.RefreshContactPoints();
				return;
			}
			float depth = num - (radius + radius2);
			IndexedVector3 normalOnBInWorld = new IndexedVector3(1f, 0f, 0f);
			if (num > 1.1920929E-07f)
			{
				normalOnBInWorld = indexedVector / num;
			}
			IndexedVector3 pointInWorld = body1.GetWorldTransform()._origin + radius2 * normalOnBInWorld;
			resultOut.AddContactPoint(ref normalOnBInWorld, ref pointInWorld, depth);
			resultOut.RefreshContactPoints();
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

		public override void Cleanup()
		{
			if (m_ownManifold && m_manifoldPtr != null)
			{
				m_dispatcher.ReleaseManifold(m_manifoldPtr);
			}
			BulletGlobals.SphereSphereCollisionAlgorithmPool.Free(this);
		}
	}
}
