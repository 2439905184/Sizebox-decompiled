using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class BridgeTriangleConcaveRaycastCallback : TriangleRaycastCallback, IDisposable
	{
		public IndexedMatrix m_colObjWorldTransform;

		public RayResultCallback m_resultCallback;

		public CollisionObject m_collisionObject;

		public ConcaveShape m_triangleMesh;

		public BridgeTriangleConcaveRaycastCallback()
		{
		}

		public BridgeTriangleConcaveRaycastCallback(ref IndexedVector3 from, ref IndexedVector3 to, RayResultCallback resultCallback, CollisionObject collisionObject, ConcaveShape triangleMesh, ref IndexedMatrix colObjWorldTransform)
			: base(ref from, ref to, resultCallback.m_flags)
		{
			m_resultCallback = resultCallback;
			m_collisionObject = collisionObject;
			m_triangleMesh = triangleMesh;
			m_colObjWorldTransform = colObjWorldTransform;
		}

		public void Initialize(ref IndexedVector3 from, ref IndexedVector3 to, RayResultCallback resultCallback, CollisionObject collisionObject, ConcaveShape triangleMesh, ref IndexedMatrix colObjWorldTransform)
		{
			base.Initialize(ref from, ref to, resultCallback.m_flags);
			m_resultCallback = resultCallback;
			m_collisionObject = collisionObject;
			m_triangleMesh = triangleMesh;
			m_colObjWorldTransform = colObjWorldTransform;
		}

		public override float ReportHit(ref IndexedVector3 hitNormalLocal, float hitFraction, int partId, int triangleIndex)
		{
			LocalShapeInfo localShapeInfo = default(LocalShapeInfo);
			localShapeInfo.m_shapePart = partId;
			localShapeInfo.m_triangleIndex = triangleIndex;
			IndexedVector3 hitNormalLocal2 = m_colObjWorldTransform._basis * hitNormalLocal;
			LocalRayResult rayResult = new LocalRayResult(m_collisionObject, ref localShapeInfo, ref hitNormalLocal2, hitFraction);
			bool normalInWorldSpace = true;
			return m_resultCallback.AddSingleResult(ref rayResult, normalInWorldSpace);
		}

		public void Dispose()
		{
			Cleanup();
			BulletGlobals.BridgeTriangleConcaveRaycastCallbackPool.Free(this);
		}
	}
}
