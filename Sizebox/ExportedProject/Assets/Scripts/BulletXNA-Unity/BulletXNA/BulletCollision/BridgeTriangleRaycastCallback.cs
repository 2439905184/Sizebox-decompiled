using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class BridgeTriangleRaycastCallback : TriangleRaycastCallback, IDisposable
	{
		public RayResultCallback m_resultCallback;

		public CollisionObject m_collisionObject;

		public TriangleMeshShape m_triangleMesh;

		public IndexedMatrix m_colObjWorldTransform;

		public BridgeTriangleRaycastCallback()
		{
		}

		public BridgeTriangleRaycastCallback(ref IndexedVector3 from, ref IndexedVector3 to, RayResultCallback resultCallback, CollisionObject collisionObject, TriangleMeshShape triangleMesh, ref IndexedMatrix colObjWorldTransform)
			: base(ref from, ref to, resultCallback.m_flags)
		{
			m_resultCallback = resultCallback;
			m_collisionObject = collisionObject;
			m_triangleMesh = triangleMesh;
			m_colObjWorldTransform = colObjWorldTransform;
		}

		public virtual void Initialize(ref IndexedVector3 from, ref IndexedVector3 to, RayResultCallback resultCallback, CollisionObject collisionObject, TriangleMeshShape triangleMesh, ref IndexedMatrix colObjWorldTransform)
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

		public override void Cleanup()
		{
			base.Cleanup();
		}

		public void Dispose()
		{
			Cleanup();
			BulletGlobals.BridgeTriangleRaycastCallbackPool.Free(this);
		}
	}
}
