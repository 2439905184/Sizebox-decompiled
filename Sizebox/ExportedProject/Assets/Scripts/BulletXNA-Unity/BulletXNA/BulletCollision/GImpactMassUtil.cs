using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public static class GImpactMassUtil
	{
		public static IndexedVector3 GimGetPointInertia(ref IndexedVector3 point, float mass)
		{
			float num = point.X * point.X;
			float num2 = point.Y * point.Y;
			float num3 = point.Z * point.Z;
			return new IndexedVector3(mass * (num2 + num3), mass * (num + num3), mass * (num + num2));
		}

		public static IndexedVector3 GimInertiaAddTransformed(ref IndexedVector3 source_inertia, ref IndexedVector3 added_inertia, ref IndexedMatrix transform)
		{
			return IndexedVector3.Zero;
		}
	}
}
