using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public struct GIM_CONTACT
	{
		public const int NORMAL_CONTACT_AVERAGE = 1;

		public const float CONTACT_DIFF_EPSILON = 1E-05f;

		public IndexedVector3 m_point;

		public IndexedVector3 m_normal;

		public float m_depth;

		public int m_feature1;

		public int m_feature2;

		public GIM_CONTACT(ref GIM_CONTACT contact)
		{
			m_point = contact.m_point;
			m_normal = contact.m_normal;
			m_depth = contact.m_depth;
			m_feature1 = contact.m_feature1;
			m_feature2 = contact.m_feature2;
		}

		public GIM_CONTACT(ref IndexedVector3 point, ref IndexedVector3 normal, float depth, int feature1, int feature2)
		{
			m_point = point;
			m_normal = normal;
			m_depth = depth;
			m_feature1 = feature1;
			m_feature2 = feature2;
		}

		public uint CalcKeyContact()
		{
			int[] array = new int[3]
			{
				(int)(m_point.X * 1000f + 1f),
				(int)(m_point.Y * 1333f),
				(int)(m_point.Z * 2133f + 3f)
			};
			uint num = 0u;
			uint num2 = (uint)array[0];
			num = num2;
			num2 = (uint)array[1];
			num += num2 << 4;
			num2 = (uint)array[2];
			return num + (num2 << 8);
		}

		public void InterpolateNormals(IndexedVector3[] normals, int normal_count)
		{
			IndexedVector3 normal = m_normal;
			for (int i = 0; i < normal_count; i++)
			{
				normal += normals[i];
			}
			float num = normal.LengthSquared();
			if (!(num < 1E-05f))
			{
				m_normal = normal / (float)Math.Sqrt(num);
			}
		}
	}
}
