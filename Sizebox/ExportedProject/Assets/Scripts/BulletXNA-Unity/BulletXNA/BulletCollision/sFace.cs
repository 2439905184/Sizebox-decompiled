using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class sFace
	{
		public IndexedVector3 n;

		public float d;

		public float p;

		public sSV[] c = new sSV[3];

		public sFace[] f = new sFace[3];

		public uint[] e = new uint[3];

		public uint pass;
	}
}
