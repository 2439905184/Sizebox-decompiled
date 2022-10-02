using System.Collections.Generic;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class ConvexHullShape : PolyhedralConvexAabbCachingShape
	{
		private IList<IndexedVector3> m_unscaledPoints;

		public ConvexHullShape(IList<IndexedVector3> points, int numPoints)
		{
			m_unscaledPoints = new List<IndexedVector3>(numPoints);
			m_shapeType = BroadphaseNativeTypes.CONVEX_HULL_SHAPE_PROXYTYPE;
			for (int i = 0; i < numPoints; i++)
			{
				m_unscaledPoints.Add(points[i]);
			}
			RecalcLocalAabb();
		}

		public ConvexHullShape(IList<float> points, int numPoints)
		{
			m_unscaledPoints = new List<IndexedVector3>(numPoints);
			m_shapeType = BroadphaseNativeTypes.CONVEX_HULL_SHAPE_PROXYTYPE;
			for (int i = 0; i < numPoints / 3; i++)
			{
				m_unscaledPoints.Add(new IndexedVector3(points[i * 3], points[i * 3] + 1f, points[i * 3] + 2f));
			}
			RecalcLocalAabb();
		}

		public void AddPoint(ref IndexedVector3 point)
		{
			m_unscaledPoints.Add(point);
			RecalcLocalAabb();
		}

		public IList<IndexedVector3> GetUnscaledPoints()
		{
			return m_unscaledPoints;
		}

		public IndexedVector3 GetScaledPoint(int i)
		{
			return m_unscaledPoints[i] * m_localScaling;
		}

		public int GetNumPoints()
		{
			return m_unscaledPoints.Count;
		}

		public override IndexedVector3 LocalGetSupportingVertexWithoutMargin(ref IndexedVector3 vec)
		{
			IndexedVector3 result = IndexedVector3.Zero;
			float num = float.MinValue;
			for (int i = 0; i < m_unscaledPoints.Count; i++)
			{
				IndexedVector3 indexedVector = m_unscaledPoints[i] * m_localScaling;
				float num2 = IndexedVector3.Dot(vec, indexedVector);
				if (num2 > num)
				{
					num = num2;
					result = indexedVector;
				}
			}
			return result;
		}

		public override IndexedVector3 LocalGetSupportingVertex(ref IndexedVector3 vec)
		{
			IndexedVector3 result = LocalGetSupportingVertexWithoutMargin(ref vec);
			if (GetMargin() != 0f)
			{
				IndexedVector3 indexedVector = vec;
				if (indexedVector.LengthSquared() < 1.4210855E-14f)
				{
					indexedVector = new IndexedVector3(-1f);
				}
				indexedVector.Normalize();
				result += GetMargin() * indexedVector;
			}
			return result;
		}

		public override void BatchedUnitVectorGetSupportingVertexWithoutMargin(IndexedVector3[] vectors, IndexedVector4[] supportVerticesOut, int numVectors)
		{
			for (int i = 0; i < numVectors; i++)
			{
				IndexedVector4 indexedVector = supportVerticesOut[i];
				indexedVector.W = -1E+18f;
				supportVerticesOut[i] = indexedVector;
			}
			for (int j = 0; j < m_unscaledPoints.Count; j++)
			{
				IndexedVector3 scaledPoint = GetScaledPoint(j);
				for (int k = 0; k < numVectors; k++)
				{
					IndexedVector3 a = vectors[k];
					float num = IndexedVector3.Dot(a, scaledPoint);
					if (num > supportVerticesOut[k].W)
					{
						supportVerticesOut[k] = new IndexedVector4(scaledPoint, num);
					}
				}
			}
		}

		public override string GetName()
		{
			return "Convex";
		}

		public override int GetNumVertices()
		{
			return m_unscaledPoints.Count;
		}

		public override int GetNumEdges()
		{
			return m_unscaledPoints.Count;
		}

		public override void GetEdge(int i, out IndexedVector3 pa, out IndexedVector3 pb)
		{
			int i2 = i % m_unscaledPoints.Count;
			int i3 = (i + 1) % m_unscaledPoints.Count;
			pa = GetScaledPoint(i2);
			pb = GetScaledPoint(i3);
		}

		public override void GetVertex(int i, out IndexedVector3 vtx)
		{
			vtx = GetScaledPoint(i);
		}

		public override int GetNumPlanes()
		{
			return 0;
		}

		public override void GetPlane(out IndexedVector3 planeNormal, out IndexedVector3 planeSupport, int i)
		{
			planeNormal = IndexedVector3.Zero;
			planeSupport = IndexedVector3.Zero;
		}

		public override bool IsInside(ref IndexedVector3 pt, float tolerance)
		{
			return false;
		}

		public override void SetLocalScaling(ref IndexedVector3 scaling)
		{
			m_localScaling = scaling;
			RecalcLocalAabb();
		}

		public new void Project(ref IndexedMatrix trans, ref IndexedVector3 dir, ref float min, ref float max)
		{
			min = float.MaxValue;
			max = float.MinValue;
			int count = m_unscaledPoints.Count;
			for (int i = 0; i < count; i++)
			{
				IndexedVector3 indexedVector = m_unscaledPoints[i] * m_localScaling;
				float num = (trans * indexedVector).Dot(dir);
				if (num < min)
				{
					min = num;
				}
				if (num > max)
				{
					max = num;
				}
			}
			if (min > max)
			{
				float num2 = min;
				min = max;
				max = num2;
			}
		}
	}
}
