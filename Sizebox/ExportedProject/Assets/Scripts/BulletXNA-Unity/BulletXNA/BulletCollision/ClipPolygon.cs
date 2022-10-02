using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class ClipPolygon
	{
		public static float DistancePointPlane(ref IndexedVector4 plane, ref IndexedVector3 point)
		{
			return point.Dot(new IndexedVector3(plane.X, plane.Y, plane.Z)) - plane.W;
		}

		public static void VecBlend(ref IndexedVector3 vr, ref IndexedVector3 va, ref IndexedVector3 vb, float blend_factor)
		{
			vr = (1f - blend_factor) * va + blend_factor * vb;
		}

		public static void PlaneClipPolygonCollect(ref IndexedVector3 point0, ref IndexedVector3 point1, float dist0, float dist1, IndexedVector3[] clipped, ref int clipped_count)
		{
			bool flag = dist0 > 1.1920929E-07f;
			bool flag2 = dist1 > 1.1920929E-07f;
			if (flag2 != flag)
			{
				float blend_factor = (0f - dist0) / (dist1 - dist0);
				VecBlend(ref clipped[clipped_count], ref point0, ref point1, blend_factor);
				clipped_count++;
			}
			if (!flag2)
			{
				clipped[clipped_count] = point1;
				clipped_count++;
			}
		}

		public static int PlaneClipPolygon(ref IndexedVector4 plane, IndexedVector3[] polygon_points, int polygon_point_count, IndexedVector3[] clipped)
		{
			int clipped_count = 0;
			float num = DistancePointPlane(ref plane, ref polygon_points[0]);
			if (!(num > 1.1920929E-07f))
			{
				clipped[clipped_count] = polygon_points[0];
				clipped_count++;
			}
			float dist = num;
			for (int i = 1; i < polygon_point_count; i++)
			{
				float num2 = DistancePointPlane(ref plane, ref polygon_points[i]);
				PlaneClipPolygonCollect(ref polygon_points[i - 1], ref polygon_points[i], dist, num2, clipped, ref clipped_count);
				dist = num2;
			}
			PlaneClipPolygonCollect(ref polygon_points[polygon_point_count - 1], ref polygon_points[0], dist, num, clipped, ref clipped_count);
			return clipped_count;
		}

		public static int PlaneClipTriangle(ref IndexedVector4 plane, ref IndexedVector3 point0, ref IndexedVector3 point1, ref IndexedVector3 point2, IndexedVector3[] clipped)
		{
			int clipped_count = 0;
			float num = DistancePointPlane(ref plane, ref point0);
			if (!(num > 1.1920929E-07f))
			{
				clipped[clipped_count] = point0;
				clipped_count++;
			}
			float dist = num;
			float num2 = DistancePointPlane(ref plane, ref point1);
			PlaneClipPolygonCollect(ref point0, ref point1, dist, num2, clipped, ref clipped_count);
			dist = num2;
			num2 = DistancePointPlane(ref plane, ref point2);
			PlaneClipPolygonCollect(ref point1, ref point2, dist, num2, clipped, ref clipped_count);
			dist = num2;
			PlaneClipPolygonCollect(ref point2, ref point0, dist, num, clipped, ref clipped_count);
			return clipped_count;
		}
	}
}
