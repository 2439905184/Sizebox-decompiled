using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public interface IDiscreteCollisionDetectorInterfaceResult
	{
		void SetShapeIdentifiersA(int partId0, int index0);

		void SetShapeIdentifiersB(int partId1, int index1);

		void AddContactPoint(IndexedVector3 normalOnBInWorld, IndexedVector3 pointInWorld, float depth);

		void AddContactPoint(ref IndexedVector3 normalOnBInWorld, ref IndexedVector3 pointInWorld, float depth);
	}
}
