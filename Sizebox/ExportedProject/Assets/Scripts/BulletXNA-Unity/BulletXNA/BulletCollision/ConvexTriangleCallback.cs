using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class ConvexTriangleCallback : ITriangleCallback
	{
		private CollisionObject m_convexBody;

		private CollisionObject m_triBody;

		private IndexedVector3 m_aabbMin;

		private IndexedVector3 m_aabbMax;

		private ManifoldResult m_resultOut;

		private IDispatcher m_dispatcher;

		private DispatcherInfo m_dispatchInfoPtr;

		private float m_collisionMarginTriangle;

		public int m_triangleCount;

		public PersistentManifold m_manifoldPtr;

		public virtual bool graphics()
		{
			return false;
		}

		public ConvexTriangleCallback(IDispatcher dispatcher, CollisionObject body0, CollisionObject body1, bool isSwapped)
		{
			m_dispatcher = dispatcher;
			m_convexBody = (isSwapped ? body1 : body0);
			m_triBody = (isSwapped ? body0 : body1);
			m_manifoldPtr = m_dispatcher.GetNewManifold(m_convexBody, m_triBody);
			ClearCache();
		}

		public void Initialize(IDispatcher dispatcher, CollisionObject body0, CollisionObject body1, bool isSwapped)
		{
			m_dispatcher = dispatcher;
			m_convexBody = (isSwapped ? body1 : body0);
			m_triBody = (isSwapped ? body0 : body1);
			m_manifoldPtr = m_dispatcher.GetNewManifold(m_convexBody, m_triBody);
			ClearCache();
		}

		public virtual void Cleanup()
		{
			ClearCache();
			m_dispatcher.ReleaseManifold(m_manifoldPtr);
		}

		public void SetTimeStepAndCounters(float collisionMarginTriangle, DispatcherInfo dispatchInfo, ManifoldResult resultOut)
		{
			m_dispatchInfoPtr = dispatchInfo;
			m_collisionMarginTriangle = collisionMarginTriangle;
			m_resultOut = resultOut;
			IndexedMatrix t = m_triBody.GetWorldTransform().Inverse() * m_convexBody.GetWorldTransform();
			CollisionShape collisionShape = m_convexBody.GetCollisionShape();
			collisionShape.GetAabb(ref t, out m_aabbMin, out m_aabbMax);
			IndexedVector3 indexedVector = new IndexedVector3(collisionMarginTriangle);
			m_aabbMax += indexedVector;
			m_aabbMin -= indexedVector;
		}

		public virtual void ProcessTriangle(IndexedVector3[] triangle, int partId, int triangleIndex)
		{
			CollisionAlgorithmConstructionInfo collisionAlgorithmConstructionInfo = default(CollisionAlgorithmConstructionInfo);
			collisionAlgorithmConstructionInfo.SetDispatcher(m_dispatcher);
			CollisionObject triBody = m_triBody;
			if (!m_convexBody.GetCollisionShape().IsConvex())
			{
				return;
			}
			using (TriangleShape triangleShape = BulletGlobals.TriangleShapePool.Get())
			{
				triangleShape.Initialize(ref triangle[0], ref triangle[1], ref triangle[2]);
				triangleShape.SetMargin(m_collisionMarginTriangle);
				CollisionShape collisionShape = triBody.GetCollisionShape();
				triBody.InternalSetTemporaryCollisionShape(triangleShape);
				CollisionAlgorithm collisionAlgorithm = collisionAlgorithmConstructionInfo.GetDispatcher().FindAlgorithm(m_convexBody, m_triBody, m_manifoldPtr);
				if (m_resultOut.GetBody0Internal() == m_triBody)
				{
					m_resultOut.SetShapeIdentifiersA(partId, triangleIndex);
				}
				else
				{
					m_resultOut.SetShapeIdentifiersB(partId, triangleIndex);
				}
				collisionAlgorithm.ProcessCollision(m_convexBody, m_triBody, m_dispatchInfoPtr, m_resultOut);
				collisionAlgorithmConstructionInfo.GetDispatcher().FreeCollisionAlgorithm(collisionAlgorithm);
				collisionAlgorithm = null;
				triBody.InternalSetTemporaryCollisionShape(collisionShape);
			}
		}

		public void ClearCache()
		{
			m_dispatcher.ClearManifold(m_manifoldPtr);
		}

		public IndexedVector3 GetAabbMin()
		{
			return m_aabbMin;
		}

		public IndexedVector3 GetAabbMax()
		{
			return m_aabbMax;
		}
	}
}
