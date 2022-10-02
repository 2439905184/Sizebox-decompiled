using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public interface ITriangleCallback
	{
		void ProcessTriangle(IndexedVector3[] triangle, int partId, int triangleIndex);

		void Cleanup();
	}
}
