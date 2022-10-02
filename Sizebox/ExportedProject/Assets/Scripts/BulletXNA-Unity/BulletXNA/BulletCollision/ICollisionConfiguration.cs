namespace BulletXNA.BulletCollision
{
	public interface ICollisionConfiguration
	{
		CollisionAlgorithmCreateFunc GetCollisionAlgorithmCreateFunc(BroadphaseNativeTypes proxyType0, BroadphaseNativeTypes proxyType1);
	}
}
