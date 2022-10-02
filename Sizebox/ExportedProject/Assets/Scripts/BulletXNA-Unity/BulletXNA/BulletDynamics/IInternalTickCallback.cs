namespace BulletXNA.BulletDynamics
{
	public interface IInternalTickCallback
	{
		void InternalTickCallback(DynamicsWorld world, float timeStep);
	}
}
