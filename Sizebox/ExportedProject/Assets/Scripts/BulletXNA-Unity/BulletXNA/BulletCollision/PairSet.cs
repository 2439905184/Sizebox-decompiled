using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class PairSet : ObjectArray<GIM_PAIR>
	{
		public PairSet()
			: base(32)
		{
		}

		public void PushPair(int index1, int index2)
		{
			Add(new GIM_PAIR(index1, index2));
		}

		public void PushPairInv(int index1, int index2)
		{
			Add(new GIM_PAIR(index2, index1));
		}
	}
}
