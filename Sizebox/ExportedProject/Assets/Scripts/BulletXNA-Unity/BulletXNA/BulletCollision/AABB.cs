using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public struct AABB
	{
		public IndexedVector3 m_min;

		public IndexedVector3 m_max;

		public AABB(ref IndexedVector3 V1, ref IndexedVector3 V2, ref IndexedVector3 V3)
		{
			m_min = default(IndexedVector3);
			m_max = default(IndexedVector3);
			m_min.X = BoxCollision.BT_MIN3(V1.X, V2.X, V3.X);
			m_min.Y = BoxCollision.BT_MIN3(V1.Y, V2.Y, V3.Y);
			m_min.Z = BoxCollision.BT_MIN3(V1.Z, V2.Z, V3.Z);
			m_max.X = BoxCollision.BT_MAX3(V1.X, V2.X, V3.X);
			m_max.Y = BoxCollision.BT_MAX3(V1.Y, V2.Y, V3.Y);
			m_max.Z = BoxCollision.BT_MAX3(V1.Z, V2.Z, V3.Z);
		}

		public AABB(ref IndexedVector3 V1, ref IndexedVector3 V2, ref IndexedVector3 V3, float margin)
		{
			m_min = default(IndexedVector3);
			m_max = default(IndexedVector3);
			m_min.X = BoxCollision.BT_MIN3(V1.X, V2.X, V3.X);
			m_min.Y = BoxCollision.BT_MIN3(V1.Y, V2.Y, V3.Y);
			m_min.Z = BoxCollision.BT_MIN3(V1.Z, V2.Z, V3.Z);
			m_max.X = BoxCollision.BT_MAX3(V1.X, V2.X, V3.X);
			m_max.Y = BoxCollision.BT_MAX3(V1.Y, V2.Y, V3.Y);
			m_max.Z = BoxCollision.BT_MAX3(V1.Z, V2.Z, V3.Z);
			m_min.X -= margin;
			m_min.Y -= margin;
			m_min.Z -= margin;
			m_max.X += margin;
			m_max.Y += margin;
			m_max.Z += margin;
		}

		public AABB(ref AABB other)
		{
			m_max = other.m_max;
			m_min = other.m_min;
		}

		public AABB(ref AABB other, float margin)
		{
			m_max = other.m_max;
			m_min = other.m_min;
			m_min.X -= margin;
			m_min.Y -= margin;
			m_min.Z -= margin;
			m_max.X += margin;
			m_max.Y += margin;
			m_max.Z += margin;
		}

		public void Invalidate()
		{
			m_min.X = float.MaxValue;
			m_min.Y = float.MaxValue;
			m_min.Z = float.MaxValue;
			m_max.X = float.MinValue;
			m_max.Y = float.MinValue;
			m_max.Z = float.MinValue;
		}

		public void IncrementMargin(float margin)
		{
			m_min.X -= margin;
			m_min.Y -= margin;
			m_min.Z -= margin;
			m_max.X += margin;
			m_max.Y += margin;
			m_max.Z += margin;
		}

		public void CopyWithMargin(ref AABB other, float margin)
		{
			m_min.X = other.m_min.X - margin;
			m_min.Y = other.m_min.Y - margin;
			m_min.Z = other.m_min.Z - margin;
			m_max.X = other.m_max.X + margin;
			m_max.Y = other.m_max.Y + margin;
			m_max.Z = other.m_max.Z + margin;
		}

		public void CalcFromTriangle(ref IndexedVector3 V1, ref IndexedVector3 V2, ref IndexedVector3 V3)
		{
			m_min.X = BoxCollision.BT_MIN3(V1.X, V2.X, V3.X);
			m_min.Y = BoxCollision.BT_MIN3(V1.Y, V2.Y, V3.Y);
			m_min.Z = BoxCollision.BT_MIN3(V1.Z, V2.Z, V3.Z);
			m_max.X = BoxCollision.BT_MAX3(V1.X, V2.X, V3.X);
			m_max.Y = BoxCollision.BT_MAX3(V1.Y, V2.Y, V3.Y);
			m_max.Z = BoxCollision.BT_MAX3(V1.Z, V2.Z, V3.Z);
		}

		public void CalcFromTriangleMargin(ref IndexedVector3 V1, ref IndexedVector3 V2, ref IndexedVector3 V3, float margin)
		{
			m_min.X = BoxCollision.BT_MIN3(V1.X, V2.X, V3.X);
			m_min.Y = BoxCollision.BT_MIN3(V1.Y, V2.Y, V3.Y);
			m_min.Z = BoxCollision.BT_MIN3(V1.Z, V2.Z, V3.Z);
			m_max.X = BoxCollision.BT_MAX3(V1.X, V2.X, V3.X);
			m_max.Y = BoxCollision.BT_MAX3(V1.Y, V2.Y, V3.Y);
			m_max.Z = BoxCollision.BT_MAX3(V1.Z, V2.Z, V3.Z);
			m_min.X -= margin;
			m_min.Y -= margin;
			m_min.Z -= margin;
			m_max.X += margin;
			m_max.Y += margin;
			m_max.Z += margin;
		}

		public void ApplyTransform(ref IndexedMatrix trans)
		{
			IndexedVector3 indexedVector = (m_max + m_min) * 0.5f;
			IndexedVector3 indexedVector2 = m_max - indexedVector;
			indexedVector = trans * indexedVector;
			IndexedVector3 indexedVector3 = new IndexedVector3(indexedVector2.Dot(trans._basis.GetRow(0).Absolute()), indexedVector2.Dot(trans._basis.GetRow(1).Absolute()), indexedVector2.Dot(trans._basis.GetRow(2).Absolute()));
			m_min = indexedVector - indexedVector3;
			m_max = indexedVector + indexedVector3;
		}

		public void ApplyTransformTransCache(ref BT_BOX_BOX_TRANSFORM_CACHE trans)
		{
			IndexedVector3 point = (m_max + m_min) * 0.5f;
			IndexedVector3 indexedVector = m_max - point;
			point = trans.Transform(ref point);
			trans.m_R1to0.Absolute();
			IndexedVector3 indexedVector2 = new IndexedVector3(indexedVector.Dot(trans.m_R1to0.GetRow(0).Absolute()), indexedVector.Dot(trans.m_R1to0.GetRow(1).Absolute()), indexedVector.Dot(trans.m_R1to0.GetRow(2).Absolute()));
			m_min = point - indexedVector2;
			m_max = point + indexedVector2;
		}

		public void Merge(AABB box)
		{
			Merge(ref box);
		}

		public void Merge(ref AABB box)
		{
			m_min.X = BoxCollision.BT_MIN(m_min.X, box.m_min.X);
			m_min.Y = BoxCollision.BT_MIN(m_min.Y, box.m_min.Y);
			m_min.Z = BoxCollision.BT_MIN(m_min.Z, box.m_min.Z);
			m_max.X = BoxCollision.BT_MAX(m_max.X, box.m_max.X);
			m_max.Y = BoxCollision.BT_MAX(m_max.Y, box.m_max.Y);
			m_max.Z = BoxCollision.BT_MAX(m_max.Z, box.m_max.Z);
		}

		public void MergePoint(ref IndexedVector3 point)
		{
			m_min.X = BoxCollision.BT_MIN(m_min.X, point.X);
			m_min.Y = BoxCollision.BT_MIN(m_min.Y, point.Y);
			m_min.Z = BoxCollision.BT_MIN(m_min.Z, point.Z);
			m_max.X = BoxCollision.BT_MAX(m_max.X, point.X);
			m_max.Y = BoxCollision.BT_MAX(m_max.Y, point.Y);
			m_max.Z = BoxCollision.BT_MAX(m_max.Z, point.Z);
		}

		public void GetCenterExtend(out IndexedVector3 center, out IndexedVector3 extend)
		{
			center = new IndexedVector3((m_max + m_min) * 0.5f);
			extend = new IndexedVector3(m_max - center);
		}

		public void FindIntersection(ref AABB other, ref AABB intersection)
		{
			intersection.m_min.X = BoxCollision.BT_MAX(other.m_min.X, m_min.X);
			intersection.m_min.Y = BoxCollision.BT_MAX(other.m_min.Y, m_min.Y);
			intersection.m_min.Z = BoxCollision.BT_MAX(other.m_min.Z, m_min.Z);
			intersection.m_max.X = BoxCollision.BT_MIN(other.m_max.X, m_max.X);
			intersection.m_max.Y = BoxCollision.BT_MIN(other.m_max.Y, m_max.Y);
			intersection.m_max.Z = BoxCollision.BT_MIN(other.m_max.Z, m_max.Z);
		}

		public bool HasCollision(ref AABB other)
		{
			if (m_min.X > other.m_max.X || m_max.X < other.m_min.X || m_min.Y > other.m_max.Y || m_max.Y < other.m_min.Y || m_min.Z > other.m_max.Z || m_max.Z < other.m_min.Z)
			{
				return false;
			}
			return true;
		}

		public bool CollideRay(ref IndexedVector3 vorigin, ref IndexedVector3 vdir)
		{
			IndexedVector3 center;
			IndexedVector3 extend;
			GetCenterExtend(out center, out extend);
			float num = vorigin.X - center.X;
			if (BoxCollision.BT_GREATER(num, extend.X) && num * vdir.X >= 0f)
			{
				return false;
			}
			float num2 = vorigin.Y - center.Y;
			if (BoxCollision.BT_GREATER(num2, extend.Y) && num2 * vdir.Y >= 0f)
			{
				return false;
			}
			float num3 = vorigin.Z - center.Z;
			if (BoxCollision.BT_GREATER(num3, extend.Z) && num3 * vdir.Z >= 0f)
			{
				return false;
			}
			float value = vdir.Y * num3 - vdir.Z * num2;
			if (Math.Abs(value) > extend.Y * Math.Abs(vdir.Z) + extend.Z * Math.Abs(vdir.Y))
			{
				return false;
			}
			value = vdir.Z * num - vdir.X * num3;
			if (Math.Abs(value) > extend.X * Math.Abs(vdir.Z) + extend.Z * Math.Abs(vdir.X))
			{
				return false;
			}
			value = vdir.X * num2 - vdir.Y * num;
			if (Math.Abs(value) > extend.X * Math.Abs(vdir.Y) + extend.Y * Math.Abs(vdir.X))
			{
				return false;
			}
			return true;
		}

		public void ProjectionInterval(ref IndexedVector4 direction, out float vmin, out float vmax)
		{
			IndexedVector3 direction2 = new IndexedVector3(direction.X, direction.Y, direction.Z);
			ProjectionInterval(ref direction2, out vmin, out vmax);
		}

		public void ProjectionInterval(ref IndexedVector3 direction, out float vmin, out float vmax)
		{
			IndexedVector3 v = (m_max + m_min) * 0.5f;
			IndexedVector3 indexedVector = m_max - v;
			float num = direction.Dot(ref v);
			float num2 = indexedVector.Dot(direction.Absolute());
			vmin = num - num2;
			vmax = num + num2;
		}

		public BT_PLANE_INTERSECTION_TYPE PlaneClassify(ref IndexedVector4 plane)
		{
			float vmin;
			float vmax;
			ProjectionInterval(ref plane, out vmin, out vmax);
			if (plane.W > vmax + 1E-06f)
			{
				return BT_PLANE_INTERSECTION_TYPE.BT_CONST_BACK_PLANE;
			}
			if (plane.W + 1E-06f >= vmin)
			{
				return BT_PLANE_INTERSECTION_TYPE.BT_CONST_COLLIDE_PLANE;
			}
			return BT_PLANE_INTERSECTION_TYPE.BT_CONST_FRONT_PLANE;
		}

		public bool OverlappingTransConservative(ref AABB box, ref IndexedMatrix trans1_to_0)
		{
			AABB other = box;
			other.ApplyTransform(ref trans1_to_0);
			return HasCollision(ref other);
		}

		public bool OverlappingTransConservative2(ref AABB box, BT_BOX_BOX_TRANSFORM_CACHE trans1_to_0)
		{
			AABB other = box;
			other.ApplyTransformTransCache(ref trans1_to_0);
			return HasCollision(ref other);
		}

		public bool OverlappingTransCache(ref AABB box, ref BT_BOX_BOX_TRANSFORM_CACHE transcache, bool fulltest)
		{
			IndexedVector3 center;
			IndexedVector3 extend;
			GetCenterExtend(out center, out extend);
			IndexedVector3 center2;
			IndexedVector3 extend2;
			box.GetCenterExtend(out center2, out extend2);
			IndexedVector3 vec = new IndexedVector3(0f, 0f, 0f);
			for (int i = 0; i < 3; i++)
			{
				vec[i] = transcache.m_R1to0[i].Dot(ref center2) + transcache.m_T1to0[i] - center[i];
				float y = transcache.m_AR[i].Dot(ref extend2) + extend[i];
				if (BoxCollision.BT_GREATER(vec[i], y))
				{
					return false;
				}
			}
			for (int i = 0; i < 3; i++)
			{
				float y = Mat3DotCol(ref transcache.m_R1to0, ref vec, i);
				float y2 = Mat3DotCol(ref transcache.m_AR, ref extend, i) + extend2[i];
				if (BoxCollision.BT_GREATER(y, y2))
				{
					return false;
				}
			}
			if (fulltest)
			{
				float[,] array = MathUtil.BasisMatrixToFloatArray(ref transcache.m_R1to0);
				float[,] array2 = MathUtil.BasisMatrixToFloatArray(ref transcache.m_AR);
				for (int i = 0; i < 3; i++)
				{
					int num = (i + 1) % 3;
					int num2 = (i + 2) % 3;
					int num3 = ((i == 0) ? 1 : 0);
					int num4 = ((i == 2) ? 1 : 2);
					for (int j = 0; j < 3; j++)
					{
						int num5 = ((j == 2) ? 1 : 2);
						int num6 = ((j == 0) ? 1 : 0);
						float y = vec[num2] * array[num, j] - vec[num] * array[num2, j];
						float y2 = extend[num3] * array2[num4, j] + extend[num4] * array2[num3, j] + extend2[num6] * array2[i, num5] + extend2[num5] * array2[i, num6];
						if (BoxCollision.BT_GREATER(y, y2))
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		public static float Mat3DotCol(ref IndexedBasisMatrix mat, ref IndexedVector3 vec3, int colindex)
		{
			return vec3.X * mat[0, colindex] + vec3.Y * mat[1, colindex] + vec3.Z * mat[2, colindex];
		}

		public static float Mat3DotCol(IndexedBasisMatrix mat, ref IndexedVector3 vec3, int colindex)
		{
			return vec3.X * mat[0, colindex] + vec3.Y * mat[1, colindex] + vec3.Z * mat[2, colindex];
		}

		public bool CollidePlane(ref IndexedVector4 plane)
		{
			BT_PLANE_INTERSECTION_TYPE bT_PLANE_INTERSECTION_TYPE = PlaneClassify(ref plane);
			return bT_PLANE_INTERSECTION_TYPE == BT_PLANE_INTERSECTION_TYPE.BT_CONST_COLLIDE_PLANE;
		}

		public bool CollideTriangleExact(ref IndexedVector3 p1, ref IndexedVector3 p2, ref IndexedVector3 p3, ref IndexedVector4 triangle_plane)
		{
			if (!CollidePlane(ref triangle_plane))
			{
				return false;
			}
			IndexedVector3 center;
			IndexedVector3 extend;
			GetCenterExtend(out center, out extend);
			IndexedVector3 pointa = p1 - center;
			IndexedVector3 pointa2 = p2 - center;
			IndexedVector3 pointb = p3 - center;
			IndexedVector3 edge = pointa2 - pointa;
			IndexedVector3 absolute_edge = edge.Absolute();
			BoxCollision.TEST_CROSS_EDGE_BOX_X_AXIS_MCR(ref edge, ref absolute_edge, ref pointa, ref pointb, ref extend);
			BoxCollision.TEST_CROSS_EDGE_BOX_Y_AXIS_MCR(ref edge, ref absolute_edge, ref pointa, ref pointb, ref extend);
			BoxCollision.TEST_CROSS_EDGE_BOX_Z_AXIS_MCR(ref edge, ref absolute_edge, ref pointa, ref pointb, ref extend);
			edge = pointb - pointa2;
			absolute_edge = edge.Absolute();
			BoxCollision.TEST_CROSS_EDGE_BOX_X_AXIS_MCR(ref edge, ref absolute_edge, ref pointa2, ref pointa, ref extend);
			BoxCollision.TEST_CROSS_EDGE_BOX_Y_AXIS_MCR(ref edge, ref absolute_edge, ref pointa2, ref pointa, ref extend);
			BoxCollision.TEST_CROSS_EDGE_BOX_Z_AXIS_MCR(ref edge, ref absolute_edge, ref pointa2, ref pointa, ref extend);
			edge = pointa - pointb;
			absolute_edge = edge.Absolute();
			BoxCollision.TEST_CROSS_EDGE_BOX_X_AXIS_MCR(ref edge, ref absolute_edge, ref pointb, ref pointa2, ref extend);
			BoxCollision.TEST_CROSS_EDGE_BOX_Y_AXIS_MCR(ref edge, ref absolute_edge, ref pointb, ref pointa2, ref extend);
			BoxCollision.TEST_CROSS_EDGE_BOX_Z_AXIS_MCR(ref edge, ref absolute_edge, ref pointb, ref pointa2, ref extend);
			return true;
		}
	}
}
