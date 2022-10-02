using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class GIM_BVH_DATA_ARRAY : ObjectArray<GIM_BVH_DATA>
	{
		public GIM_BVH_DATA_ARRAY()
		{
		}

		public GIM_BVH_DATA_ARRAY(int reserve)
			: base(reserve)
		{
		}
	}
}
