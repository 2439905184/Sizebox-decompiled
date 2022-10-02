using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class BoxCollision
	{
		public const float BOX_PLANE_EPSILON = 1E-06f;

		public static bool BT_GREATER(float x, float y)
		{
			return Math.Abs(x) > y;
		}

		public static float BT_MAX(float a, float b)
		{
			return Math.Max(a, b);
		}

		public static float BT_MIN(float a, float b)
		{
			return Math.Min(a, b);
		}

		public static float BT_MAX3(float a, float b, float c)
		{
			return Math.Max(a, Math.Max(b, c));
		}

		public static float BT_MIN3(float a, float b, float c)
		{
			return Math.Min(a, Math.Min(b, c));
		}

		public static bool TEST_CROSS_EDGE_BOX_MCR(ref IndexedVector3 edge, ref IndexedVector3 absolute_edge, ref IndexedVector3 pointa, ref IndexedVector3 pointb, ref IndexedVector3 _extend, int i_dir_0, int i_dir_1, int i_comp_0, int i_comp_1)
		{
			float num = 0f - edge[i_dir_0];
			float num2 = edge[i_dir_1];
			float num3 = pointa[i_comp_0] * num + pointa[i_comp_1] * num2;
			float num4 = pointb[i_comp_0] * num + pointb[i_comp_1] * num2;
			if (num3 > num4)
			{
				num3 += num4;
				num4 = num3 - num4;
				num3 -= num4;
			}
			float num5 = absolute_edge[i_dir_0];
			float num6 = absolute_edge[i_dir_1];
			float num7 = _extend[i_comp_0] * num5 + _extend[i_comp_1] * num6;
			if (num3 > num7 || 0f - num7 > num4)
			{
				return false;
			}
			return true;
		}

		public static bool TEST_CROSS_EDGE_BOX_X_AXIS_MCR(ref IndexedVector3 edge, ref IndexedVector3 absolute_edge, ref IndexedVector3 pointa, ref IndexedVector3 pointb, ref IndexedVector3 _extend)
		{
			return TEST_CROSS_EDGE_BOX_MCR(ref edge, ref absolute_edge, ref pointa, ref pointb, ref _extend, 2, 1, 1, 2);
		}

		public static bool TEST_CROSS_EDGE_BOX_Y_AXIS_MCR(ref IndexedVector3 edge, ref IndexedVector3 absolute_edge, ref IndexedVector3 pointa, ref IndexedVector3 pointb, ref IndexedVector3 _extend)
		{
			return TEST_CROSS_EDGE_BOX_MCR(ref edge, ref absolute_edge, ref pointa, ref pointb, ref _extend, 0, 2, 2, 0);
		}

		public static bool TEST_CROSS_EDGE_BOX_Z_AXIS_MCR(ref IndexedVector3 edge, ref IndexedVector3 absolute_edge, ref IndexedVector3 pointa, ref IndexedVector3 pointb, ref IndexedVector3 _extend)
		{
			return TEST_CROSS_EDGE_BOX_MCR(ref edge, ref absolute_edge, ref pointa, ref pointb, ref _extend, 1, 0, 0, 1);
		}

		private bool CompareTransformsEqual(ref IndexedMatrix t1, ref IndexedMatrix t2)
		{
			return t1.Equals(t2);
		}
	}
}
