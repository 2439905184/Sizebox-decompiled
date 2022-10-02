using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class BridgeTriangleConvexcastCallback : TriangleConvexcastCallback, IDisposable
	{
		private ConvexResultCallback m_resultCallback;

		private CollisionObject m_collisionObject;

		private ConcaveShape m_triangleMesh;

		public BridgeTriangleConvexcastCallback()
		{
		}

		public BridgeTriangleConvexcastCallback(ConvexShape castShape, ref IndexedMatrix from, ref IndexedMatrix to, ConvexResultCallback resultCallback, CollisionObject collisionObject, ConcaveShape triangleMesh, ref IndexedMatrix triangleToWorld)
			: base(castShape, ref from, ref to, ref triangleToWorld, triangleMesh.GetMargin())
		{
			m_resultCallback = resultCallback;
			m_collisionObject = collisionObject;
			m_triangleMesh = triangleMesh;
		}

		public void Initialize(ConvexShape castShape, ref IndexedMatrix from, ref IndexedMatrix to, ConvexResultCallback resultCallback, CollisionObject collisionObject, ConcaveShape triangleMesh, ref IndexedMatrix triangleToWorld)
		{
			base.Initialize(castShape, ref from, ref to, ref triangleToWorld, triangleMesh.GetMargin());
			m_resultCallback = resultCallback;
			m_collisionObject = collisionObject;
			m_triangleMesh = triangleMesh;
		}

		public override float ReportHit(ref IndexedVector3 hitNormalLocal, ref IndexedVector3 hitPointLocal, float hitFraction, int partId, int triangleIndex)
		{
			LocalShapeInfo localShapeInfo = default(LocalShapeInfo);
			localShapeInfo.m_shapePart = partId;
			localShapeInfo.m_triangleIndex = triangleIndex;
			if (hitFraction <= m_resultCallback.m_closestHitFraction)
			{
				LocalConvexResult convexResult = new LocalConvexResult(m_collisionObject, ref localShapeInfo, ref hitNormalLocal, ref hitPointLocal, hitFraction);
				bool normalInWorldSpace = m_triangleMesh is TriangleMeshShape;
				return m_resultCallback.AddSingleResult(ref convexResult, normalInWorldSpace);
			}
			return hitFraction;
		}

		public void Dispose()
		{
			Cleanup();
			BulletGlobals.BridgeTriangleConvexcastCallbackPool.Free(this);
		}
	}
}
