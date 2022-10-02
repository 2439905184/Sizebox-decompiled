using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class ClosestRayResultCallback : RayResultCallback, IDisposable
	{
		public IndexedVector3 m_rayFromWorld;

		public IndexedVector3 m_rayToWorld;

		public IndexedVector3 m_hitNormalWorld;

		public IndexedVector3 m_hitPointWorld;

		public ClosestRayResultCallback()
		{
		}

		public ClosestRayResultCallback(IndexedVector3 rayFromWorld, IndexedVector3 rayToWorld)
		{
			m_rayFromWorld = rayFromWorld;
			m_rayToWorld = rayToWorld;
		}

		public ClosestRayResultCallback(ref IndexedVector3 rayFromWorld, ref IndexedVector3 rayToWorld)
		{
			m_rayFromWorld = rayFromWorld;
			m_rayToWorld = rayToWorld;
		}

		public void Initialize(IndexedVector3 rayFromWorld, IndexedVector3 rayToWorld)
		{
			m_rayFromWorld = rayFromWorld;
			m_rayToWorld = rayToWorld;
			m_closestHitFraction = 1f;
			m_collisionObject = null;
		}

		public void Initialize(ref IndexedVector3 rayFromWorld, ref IndexedVector3 rayToWorld)
		{
			m_rayFromWorld = rayFromWorld;
			m_rayToWorld = rayToWorld;
			m_closestHitFraction = 1f;
			m_collisionObject = null;
		}

		public override float AddSingleResult(ref LocalRayResult rayResult, bool normalInWorldSpace)
		{
			m_closestHitFraction = rayResult.m_hitFraction;
			m_collisionObject = rayResult.m_collisionObject;
			if (normalInWorldSpace)
			{
				m_hitNormalWorld = rayResult.m_hitNormalLocal;
			}
			else
			{
				m_hitNormalWorld = m_collisionObject.GetWorldTransform()._basis * rayResult.m_hitNormalLocal;
			}
			m_hitPointWorld = MathUtil.Interpolate3(ref m_rayFromWorld, ref m_rayToWorld, rayResult.m_hitFraction);
			return rayResult.m_hitFraction;
		}

		public void Dispose()
		{
			Cleanup();
			BulletGlobals.ClosestRayResultCallbackPool.Free(this);
		}
	}
}
