namespace BulletXNA.BulletCollision
{
	public interface IContactAddedCallback
	{
		bool Callback(ref ManifoldPoint cp, CollisionObject colObj0, int partId0, int index0, CollisionObject colObj1, int partId1, int index1);
	}
}
