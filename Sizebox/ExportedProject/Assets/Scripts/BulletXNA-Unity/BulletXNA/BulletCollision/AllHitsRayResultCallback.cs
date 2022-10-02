using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class AllHitsRayResultCallback : RayResultCallback
	{
		public ObjectArray<CollisionObject> m_collisionObjects = new ObjectArray<CollisionObject>();

		private IndexedVector3 m_rayFromWorld;

		private IndexedVector3 m_rayToWorld;

		public ObjectArray<IndexedVector3> m_hitNormalWorld = new ObjectArray<IndexedVector3>();

		public ObjectArray<IndexedVector3> m_hitPointWorld = new ObjectArray<IndexedVector3>();

		public ObjectArray<float> m_hitFractions = new ObjectArray<float>();

		public AllHitsRayResultCallback(IndexedVector3 rayFromWorld, IndexedVector3 rayToWorld)
			: this(ref rayFromWorld, ref rayToWorld)
		{
		}

		public AllHitsRayResultCallback(ref IndexedVector3 rayFromWorld, ref IndexedVector3 rayToWorld)
		{
			m_rayFromWorld = rayFromWorld;
			m_rayToWorld = rayToWorld;
		}

		public override float AddSingleResult(ref LocalRayResult rayResult, bool normalInWorldSpace)
		{
			m_collisionObject = rayResult.m_collisionObject;
			m_collisionObjects.Add(rayResult.m_collisionObject);
			IndexedVector3 item = ((!normalInWorldSpace) ? (m_collisionObject.GetWorldTransform()._basis * rayResult.m_hitNormalLocal) : rayResult.m_hitNormalLocal);
			m_hitNormalWorld.Add(item);
			IndexedVector3 item2 = MathUtil.Interpolate3(ref m_rayFromWorld, ref m_rayToWorld, rayResult.m_hitFraction);
			m_hitPointWorld.Add(item2);
			m_hitFractions.Add(rayResult.m_hitFraction);
			return m_closestHitFraction;
		}
	}
}
