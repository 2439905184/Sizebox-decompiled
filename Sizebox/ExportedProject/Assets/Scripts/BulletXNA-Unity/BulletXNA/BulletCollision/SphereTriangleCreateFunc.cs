namespace BulletXNA.BulletCollision
{
	public class SphereTriangleCreateFunc : CollisionAlgorithmCreateFunc
	{
		public override CollisionAlgorithm CreateCollisionAlgorithm(CollisionAlgorithmConstructionInfo ci, CollisionObject body0, CollisionObject body1)
		{
			SphereTriangleCollisionAlgorithm sphereTriangleCollisionAlgorithm = BulletGlobals.SphereTriangleCollisionAlgorithmPool.Get();
			sphereTriangleCollisionAlgorithm.Initialize(ci.GetManifold(), ci, body0, body1, m_swapped);
			return sphereTriangleCollisionAlgorithm;
		}
	}
}
