using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class Face
	{
		public ObjectArray<int> m_indices = new ObjectArray<int>();

		public float[] m_plane = new float[4];
	}
}
