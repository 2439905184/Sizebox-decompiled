using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class GeometeryOperations
	{
		public static void bt_edge_plane(ref IndexedVector3 e1, ref IndexedVector3 e2, ref IndexedVector3 normal, out IndexedVector4 plane)
		{
			IndexedVector3 v = (e2 - e1).Cross(ref normal);
			v.Normalize();
			plane = new IndexedVector4(v, e2.Dot(ref v));
		}
	}
}
