using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class SingleRayCallback : BroadphaseRayCallback, IDisposable
	{
		public IndexedVector3 m_rayFromWorld;

		public IndexedVector3 m_rayToWorld;

		public IndexedMatrix m_rayFromTrans;

		public IndexedMatrix m_rayToTrans;

		public IndexedVector3 m_hitNormal;

		public CollisionWorld m_world;

		public RayResultCallback m_resultCallback;

		public SingleRayCallback()
		{
		}

		public SingleRayCallback(ref IndexedVector3 rayFromWorld, ref IndexedVector3 rayToWorld, CollisionWorld world, RayResultCallback resultCallback)
		{
			Initialize(ref rayFromWorld, ref rayToWorld, world, resultCallback);
		}

		public void Initialize(ref IndexedVector3 rayFromWorld, ref IndexedVector3 rayToWorld, CollisionWorld world, RayResultCallback resultCallback)
		{
			m_rayFromWorld = rayFromWorld;
			m_rayToWorld = rayToWorld;
			m_world = world;
			m_resultCallback = resultCallback;
			m_rayFromTrans = IndexedMatrix.CreateTranslation(m_rayFromWorld);
			m_rayToTrans = IndexedMatrix.CreateTranslation(m_rayToWorld);
			IndexedVector3 indexedVector = rayToWorld - rayFromWorld;
			indexedVector.Normalize();
			m_rayDirectionInverse.X = (MathUtil.FuzzyZero(indexedVector.X) ? float.MaxValue : (1f / indexedVector.X));
			m_rayDirectionInverse.Y = (MathUtil.FuzzyZero(indexedVector.Y) ? float.MaxValue : (1f / indexedVector.Y));
			m_rayDirectionInverse.Z = (MathUtil.FuzzyZero(indexedVector.Z) ? float.MaxValue : (1f / indexedVector.Z));
			m_signs[0] = m_rayDirectionInverse.X < 0f;
			m_signs[1] = m_rayDirectionInverse.Y < 0f;
			m_signs[2] = m_rayDirectionInverse.Z < 0f;
			m_lambda_max = indexedVector.Dot(m_rayToWorld - m_rayFromWorld);
		}

		public override bool Process(BroadphaseProxy proxy)
		{
			if (MathUtil.FuzzyZero(m_resultCallback.m_closestHitFraction))
			{
				return false;
			}
			CollisionObject collisionObject = proxy.m_clientObject as CollisionObject;
			if (m_resultCallback.NeedsCollision(collisionObject.GetBroadphaseHandle()))
			{
				IndexedVector3 aabbMin = collisionObject.GetBroadphaseHandle().m_aabbMin;
				IndexedVector3 aabbMax = collisionObject.GetBroadphaseHandle().m_aabbMax;
				IndexedMatrix colObjWorldTransform = collisionObject.GetWorldTransform();
				CollisionWorld.RayTestSingle(ref m_rayFromTrans, ref m_rayToTrans, collisionObject, collisionObject.GetCollisionShape(), ref colObjWorldTransform, m_resultCallback);
			}
			return true;
		}

		public override void Cleanup()
		{
			base.Cleanup();
		}

		public void Dispose()
		{
			BulletGlobals.SingleRayCallbackPool.Free(this);
		}
	}
}
