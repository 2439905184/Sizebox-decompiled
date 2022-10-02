using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public interface IConvexCast
	{
		bool CalcTimeOfImpact(ref IndexedMatrix fromA, ref IndexedMatrix toA, ref IndexedMatrix fromB, ref IndexedMatrix toB, CastResult result);
	}
}
