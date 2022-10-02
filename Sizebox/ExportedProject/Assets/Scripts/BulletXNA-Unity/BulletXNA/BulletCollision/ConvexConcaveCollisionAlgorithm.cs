using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class ConvexConcaveCollisionAlgorithm : ActivatingCollisionAlgorithm
	{
		private bool m_isSwapped;

		private ConvexTriangleCallback m_convexTriangleCallback;

		public ConvexConcaveCollisionAlgorithm()
		{
		}

		public ConvexConcaveCollisionAlgorithm(CollisionAlgorithmConstructionInfo ci, CollisionObject body0, CollisionObject body1, bool isSwapped)
			: base(ci, body0, body1)
		{
			m_isSwapped = isSwapped;
			m_convexTriangleCallback = new ConvexTriangleCallback(m_dispatcher, body0, body1, isSwapped);
		}

		public void Inititialize(CollisionAlgorithmConstructionInfo ci, CollisionObject body0, CollisionObject body1, bool isSwapped)
		{
			base.Initialize(ci, body0, body1);
			m_isSwapped = isSwapped;
			if (m_convexTriangleCallback == null)
			{
				m_convexTriangleCallback = new ConvexTriangleCallback(m_dispatcher, body0, body1, isSwapped);
			}
			else
			{
				m_convexTriangleCallback.Initialize(m_dispatcher, body0, body1, isSwapped);
			}
		}

		public override void Cleanup()
		{
			base.Cleanup();
			if (m_convexTriangleCallback != null)
			{
				m_convexTriangleCallback.Cleanup();
			}
			BulletGlobals.ConvexConcaveCollisionAlgorithmPool.Free(this);
		}

		public override void ProcessCollision(CollisionObject bodyA, CollisionObject bodyB, DispatcherInfo dispatchInfo, ManifoldResult resultOut)
		{
			CollisionObject collisionObject = (m_isSwapped ? bodyB : bodyA);
			CollisionObject collisionObject2 = (m_isSwapped ? bodyA : bodyB);
			if (collisionObject2.GetCollisionShape().IsConcave())
			{
				CollisionObject collisionObject3 = collisionObject2;
				ConcaveShape concaveShape = collisionObject3.GetCollisionShape() as ConcaveShape;
				if (collisionObject.GetCollisionShape().IsConvex())
				{
					float margin = concaveShape.GetMargin();
					resultOut.SetPersistentManifold(m_convexTriangleCallback.m_manifoldPtr);
					m_convexTriangleCallback.SetTimeStepAndCounters(margin, dispatchInfo, resultOut);
					m_convexTriangleCallback.m_manifoldPtr.SetBodies(collisionObject, collisionObject2);
					IndexedVector3 aabbMin = m_convexTriangleCallback.GetAabbMin();
					IndexedVector3 aabbMax = m_convexTriangleCallback.GetAabbMax();
					concaveShape.ProcessAllTriangles(m_convexTriangleCallback, ref aabbMin, ref aabbMax);
					resultOut.RefreshContactPoints();
				}
			}
		}

		public override float CalculateTimeOfImpact(CollisionObject bodyA, CollisionObject bodyB, DispatcherInfo dispatchInfo, ManifoldResult resultOut)
		{
			CollisionObject collisionObject = (m_isSwapped ? bodyB : bodyA);
			CollisionObject collisionObject2 = (m_isSwapped ? bodyA : bodyB);
			float num = (collisionObject.GetInterpolationWorldTransform()._origin - collisionObject.GetWorldTransform()._origin).LengthSquared();
			if (num < collisionObject.GetCcdSquareMotionThreshold())
			{
				return 1f;
			}
			IndexedMatrix indexedMatrix = collisionObject2.GetWorldTransform().Inverse();
			IndexedMatrix from = indexedMatrix * collisionObject.GetWorldTransform();
			IndexedMatrix to = indexedMatrix * collisionObject.GetInterpolationWorldTransform();
			if (collisionObject2.GetCollisionShape().IsConcave())
			{
				IndexedVector3 output = from._origin;
				MathUtil.VectorMin(to._origin, ref output);
				IndexedVector3 output2 = from._origin;
				MathUtil.VectorMax(to._origin, ref output2);
				IndexedVector3 indexedVector = new IndexedVector3(collisionObject.GetCcdSweptSphereRadius());
				output -= indexedVector;
				output2 += indexedVector;
				float hitFraction = 1f;
				using (LocalTriangleSphereCastCallback localTriangleSphereCastCallback = BulletGlobals.LocalTriangleSphereCastCallbackPool.Get())
				{
					localTriangleSphereCastCallback.Initialize(ref from, ref to, collisionObject.GetCcdSweptSphereRadius(), hitFraction);
					localTriangleSphereCastCallback.m_hitFraction = collisionObject.GetHitFraction();
					CollisionObject collisionObject3 = collisionObject2;
					ConcaveShape concaveShape = collisionObject3.GetCollisionShape() as ConcaveShape;
					if (concaveShape != null)
					{
						concaveShape.ProcessAllTriangles(localTriangleSphereCastCallback, ref output, ref output2);
					}
					if (localTriangleSphereCastCallback.m_hitFraction < collisionObject.GetHitFraction())
					{
						collisionObject.SetHitFraction(localTriangleSphereCastCallback.m_hitFraction);
						return localTriangleSphereCastCallback.m_hitFraction;
					}
				}
			}
			return 1f;
		}

		public override void GetAllContactManifolds(PersistentManifoldArray manifoldArray)
		{
			if (m_convexTriangleCallback.m_manifoldPtr != null)
			{
				manifoldArray.Add(m_convexTriangleCallback.m_manifoldPtr);
			}
		}

		public void ClearCache()
		{
			m_convexTriangleCallback.ClearCache();
		}
	}
}
