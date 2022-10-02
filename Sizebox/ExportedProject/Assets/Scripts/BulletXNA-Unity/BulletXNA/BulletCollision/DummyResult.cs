using System.Runtime.InteropServices;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct DummyResult : IDiscreteCollisionDetectorInterfaceResult
	{
		public void SetShapeIdentifiersA(int partId0, int index0)
		{
		}

		public void SetShapeIdentifiersB(int partId1, int index1)
		{
		}

		public void AddContactPoint(IndexedVector3 normalOnBInWorld, IndexedVector3 pointInWorld, float depth)
		{
		}

		public void AddContactPoint(ref IndexedVector3 normalOnBInWorld, ref IndexedVector3 pointInWorld, float depth)
		{
		}
	}
}
