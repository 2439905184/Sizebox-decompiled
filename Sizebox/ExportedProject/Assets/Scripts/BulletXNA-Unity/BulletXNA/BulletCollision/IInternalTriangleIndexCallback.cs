using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public interface IInternalTriangleIndexCallback
	{
		void InternalProcessTriangleIndex(IndexedVector3[] triangle, int partId, int triangleIndex);

		void Cleanup();
	}
}
