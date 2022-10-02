namespace BulletXNA.BulletCollision
{
	public interface IOverlapFilterCallback
	{
		bool NeedBroadphaseCollision(BroadphaseProxy proxy0, BroadphaseProxy proxy1);
	}
}
