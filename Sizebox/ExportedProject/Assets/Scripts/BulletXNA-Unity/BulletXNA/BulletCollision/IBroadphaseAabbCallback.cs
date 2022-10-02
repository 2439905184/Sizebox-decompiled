namespace BulletXNA.BulletCollision
{
	public interface IBroadphaseAabbCallback
	{
		void Cleanup();

		bool Process(BroadphaseProxy proxy);
	}
}
