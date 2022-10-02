using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class BroadphasePairQuickSort : IQSComparer<BroadphasePair>
	{
		public bool Compare(BroadphasePair lhs, BroadphasePair rhs)
		{
			return BroadphasePair.IsLessThen(lhs, rhs);
		}
	}
}
