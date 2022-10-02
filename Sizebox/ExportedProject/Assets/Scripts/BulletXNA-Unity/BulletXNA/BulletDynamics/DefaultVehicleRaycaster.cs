using BulletXNA.BulletCollision;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletDynamics
{
	public class DefaultVehicleRaycaster : IVehicleRaycaster
	{
		private struct DataCopy
		{
			private IndexedVector3 m_from;

			private IndexedVector3 m_to;

			private VehicleRaycasterResult m_result;

			public DataCopy(IndexedVector3 from, IndexedVector3 to, VehicleRaycasterResult result)
			{
				m_from = from;
				m_to = to;
				m_result = result;
			}
		}

		private DynamicsWorld m_dynamicsWorld;

		public DefaultVehicleRaycaster(DynamicsWorld world)
		{
			m_dynamicsWorld = world;
		}

		public virtual object CastRay(ref IndexedVector3 from, ref IndexedVector3 to, ref VehicleRaycasterResult result)
		{
			ClosestRayResultCallback closestRayResultCallback = new ClosestRayResultCallback(ref from, ref to);
			m_dynamicsWorld.RayTest(ref from, ref to, closestRayResultCallback);
			if (closestRayResultCallback.HasHit())
			{
				RigidBody rigidBody = RigidBody.Upcast(closestRayResultCallback.m_collisionObject);
				if (rigidBody != null && rigidBody.HasContactResponse())
				{
					result.m_hitPointInWorld = closestRayResultCallback.m_hitPointWorld;
					result.m_hitNormalInWorld = closestRayResultCallback.m_hitNormalWorld;
					result.m_hitNormalInWorld.Normalize();
					result.m_distFraction = closestRayResultCallback.m_closestHitFraction;
					return rigidBody;
				}
			}
			else
			{
				ClosestRayResultCallback resultCallback = new ClosestRayResultCallback(ref from, ref to);
				m_dynamicsWorld.RayTest(ref from, ref to, resultCallback);
			}
			closestRayResultCallback.Cleanup();
			return null;
		}
	}
}
