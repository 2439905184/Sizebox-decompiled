using UnityEngine;

namespace BulletXNA.BulletCollision.CollisionShapes.GImpact
{
	public static class GeometeryOperations
	{
		public static void bt_edge_plane(ref Vector3 e1, ref Vector3 e2, ref Vector3 normal, out Plane plane)
		{
			Vector3 vector = Vector3.Cross(e2 - e1, normal);
			vector.Normalize();
			plane = new Plane(vector, Vector3.Dot(e2, vector));
		}
	}
}
