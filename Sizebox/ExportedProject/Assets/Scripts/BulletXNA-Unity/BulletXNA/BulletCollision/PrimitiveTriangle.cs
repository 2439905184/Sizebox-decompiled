using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class PrimitiveTriangle
	{
		public IndexedVector3[] m_vertices = new IndexedVector3[3];

		public IndexedVector4 m_plane;

		public float m_margin;

		private GIM_TRIANGLE_CONTACT m_contacts1 = new GIM_TRIANGLE_CONTACT();

		public static IndexedVector3[] temp_points = new IndexedVector3[16];

		public static IndexedVector3[] temp_points1 = new IndexedVector3[16];

		private static IndexedVector3[] clipped_points = new IndexedVector3[16];

		public PrimitiveTriangle()
		{
			m_margin = 0.01f;
		}

		public void BuildTriPlane()
		{
			IndexedVector3 indexedVector = IndexedVector3.Cross(m_vertices[1] - m_vertices[0], m_vertices[2] - m_vertices[0]);
			indexedVector.Normalize();
			m_plane = new IndexedVector4(indexedVector, IndexedVector3.Dot(m_vertices[0], indexedVector));
		}

		public bool OverlapTestConservative(PrimitiveTriangle other)
		{
			float num = m_margin + other.m_margin;
			float num2 = ClipPolygon.DistancePointPlane(ref m_plane, ref other.m_vertices[0]) - num;
			float num3 = ClipPolygon.DistancePointPlane(ref m_plane, ref other.m_vertices[1]) - num;
			float num4 = ClipPolygon.DistancePointPlane(ref m_plane, ref other.m_vertices[2]) - num;
			if (num2 > 0f && num3 > 0f && num4 > 0f)
			{
				return false;
			}
			num2 = ClipPolygon.DistancePointPlane(ref other.m_plane, ref m_vertices[0]) - num;
			num3 = ClipPolygon.DistancePointPlane(ref other.m_plane, ref m_vertices[1]) - num;
			num4 = ClipPolygon.DistancePointPlane(ref other.m_plane, ref m_vertices[2]) - num;
			if (num2 > 0f && num3 > 0f && num4 > 0f)
			{
				return false;
			}
			return true;
		}

		public void GetEdgePlane(int edge_index, out IndexedVector4 plane)
		{
			IndexedVector3 e = m_vertices[edge_index];
			IndexedVector3 e2 = m_vertices[(edge_index + 1) % 3];
			IndexedVector3 normal = new IndexedVector3(m_plane.X, m_plane.Y, m_plane.Z);
			GeometeryOperations.bt_edge_plane(ref e, ref e2, ref normal, out plane);
		}

		public void ApplyTransform(ref IndexedMatrix t)
		{
			IndexedVector3.Transform(m_vertices, ref t, m_vertices);
		}

		public int ClipTriangle(PrimitiveTriangle other, IndexedVector3[] clipped_points)
		{
			IndexedVector4 plane;
			GetEdgePlane(0, out plane);
			int num = ClipPolygon.PlaneClipTriangle(ref plane, ref other.m_vertices[0], ref other.m_vertices[1], ref other.m_vertices[2], temp_points);
			if (num == 0)
			{
				return 0;
			}
			GetEdgePlane(1, out plane);
			num = ClipPolygon.PlaneClipPolygon(ref plane, temp_points, num, temp_points1);
			if (num == 0)
			{
				return 0;
			}
			GetEdgePlane(2, out plane);
			return ClipPolygon.PlaneClipPolygon(ref plane, temp_points1, num, clipped_points);
		}

		public bool FindTriangleCollisionClipMethod(PrimitiveTriangle other, GIM_TRIANGLE_CONTACT contacts)
		{
			float margin = m_margin + other.m_margin;
			m_contacts1.m_separating_normal = m_plane;
			int num = ClipTriangle(other, clipped_points);
			if (num == 0)
			{
				return false;
			}
			m_contacts1.MergePoints(ref m_contacts1.m_separating_normal, margin, clipped_points, num);
			if (m_contacts1.m_point_count == 0)
			{
				return false;
			}
			m_contacts1.m_separating_normal *= -1f;
			GIM_TRIANGLE_CONTACT gIM_TRIANGLE_CONTACT = new GIM_TRIANGLE_CONTACT();
			gIM_TRIANGLE_CONTACT.m_separating_normal = other.m_plane;
			num = other.ClipTriangle(this, clipped_points);
			if (num == 0)
			{
				return false;
			}
			gIM_TRIANGLE_CONTACT.MergePoints(ref gIM_TRIANGLE_CONTACT.m_separating_normal, margin, clipped_points, num);
			if (gIM_TRIANGLE_CONTACT.m_point_count == 0)
			{
				return false;
			}
			if (gIM_TRIANGLE_CONTACT.m_penetration_depth < m_contacts1.m_penetration_depth)
			{
				contacts.CopyFrom(gIM_TRIANGLE_CONTACT);
			}
			else
			{
				contacts.CopyFrom(m_contacts1);
			}
			return true;
		}
	}
}
