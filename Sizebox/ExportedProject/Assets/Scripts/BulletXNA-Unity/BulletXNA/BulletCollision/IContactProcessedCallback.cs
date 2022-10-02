namespace BulletXNA.BulletCollision
{
	public interface IContactProcessedCallback
	{
		bool Callback(ManifoldPoint point, object body0, object body1);
	}
}
