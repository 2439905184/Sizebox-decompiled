using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public interface IIslandCallback
	{
		void ProcessIsland(ObjectArray<CollisionObject> bodies, int numBodies, PersistentManifoldArray manifolds, int startManifold, int numManifolds, int islandId);
	}
}
