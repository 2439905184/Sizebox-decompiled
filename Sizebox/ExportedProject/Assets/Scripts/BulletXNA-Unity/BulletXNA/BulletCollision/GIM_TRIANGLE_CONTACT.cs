using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class GIM_TRIANGLE_CONTACT
	{
		public const int MAX_TRI_CLIPPING = 16;

		public float m_penetration_depth;

		public int m_point_count;

		public IndexedVector4 m_separating_normal;

		public IndexedVector3[] m_points = new IndexedVector3[16];

		private int[] point_indices = new int[16];

		public void CopyFrom(GIM_TRIANGLE_CONTACT other)
		{
			m_penetration_depth = other.m_penetration_depth;
			m_separating_normal = other.m_separating_normal;
			m_point_count = other.m_point_count;
			int point_count = m_point_count;
			while (point_count-- != 0)
			{
				m_points[point_count] = other.m_points[point_count];
			}
		}

		public GIM_TRIANGLE_CONTACT()
		{
		}

		public GIM_TRIANGLE_CONTACT(GIM_TRIANGLE_CONTACT other)
		{
			CopyFrom(other);
		}

		public void MergePoints(ref IndexedVector4 plane, float margin, IndexedVector3[] points, int point_count)
		{
			m_point_count = 0;
			m_penetration_depth = -1000f;
			for (int i = 0; i < point_count; i++)
			{
				float num = 0f - ClipPolygon.DistancePointPlane(ref plane, ref points[i]) + margin;
				if (num >= 0f)
				{
					if (num > m_penetration_depth)
					{
						m_penetration_depth = num;
						point_indices[0] = i;
						m_point_count = 1;
					}
					else if (num + 1.1920929E-07f >= m_penetration_depth)
					{
						point_indices[m_point_count] = i;
						m_point_count++;
					}
				}
			}
			for (int i = 0; i < m_point_count; i++)
			{
				m_points[i] = points[point_indices[i]];
			}
		}
	}
}
