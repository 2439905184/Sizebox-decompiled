using System.Collections.Generic;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class ConvexPointCloudShape : PolyhedralConvexAabbCachingShape
	{
		private IList<IndexedVector3> m_unscaledPoints;

		private int m_numPoints;

		public ConvexPointCloudShape()
		{
			m_localScaling = new IndexedVector3(1f);
			m_shapeType = BroadphaseNativeTypes.CONVEX_POINT_CLOUD_SHAPE_PROXYTYPE;
			m_unscaledPoints = null;
			m_numPoints = 0;
		}

		public ConvexPointCloudShape(IList<IndexedVector3> points, int numPoints, ref IndexedVector3 localScaling, bool computeAabb)
		{
			m_localScaling = localScaling;
			m_shapeType = BroadphaseNativeTypes.CONVEX_POINT_CLOUD_SHAPE_PROXYTYPE;
			m_unscaledPoints = points;
			m_numPoints = numPoints;
			if (computeAabb)
			{
				RecalcLocalAabb();
			}
		}

		public void SetPoints(IList<IndexedVector3> points, int numPoints, bool computeAabb)
		{
			IndexedVector3 localScaling = new IndexedVector3(1f);
			SetPoints(points, numPoints, computeAabb, ref localScaling);
		}

		public void SetPoints(IList<IndexedVector3> points, int numPoints, bool computeAabb, ref IndexedVector3 localScaling)
		{
			m_unscaledPoints = points;
			m_numPoints = numPoints;
			m_localScaling = localScaling;
			if (computeAabb)
			{
				RecalcLocalAabb();
			}
		}

		public IList<IndexedVector3> GetUnscaledPoints()
		{
			return m_unscaledPoints;
		}

		public int GetNumPoints()
		{
			return m_numPoints;
		}

		public IndexedVector3 GetScaledPoint(int index)
		{
			return m_unscaledPoints[index] * m_localScaling;
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

		public override IndexedVector3 LocalGetSupportingVertexWithoutMargin(ref IndexedVector3 vec0)
		{
			IndexedVector3 result = IndexedVector3.Zero;
			float num = float.MinValue;
			IndexedVector3 a = vec0;
			float num2 = a.LengthSquared();
			if (num2 < 0.0001f)
			{
				a = new IndexedVector3(1f, 0f, 0f);
			}
			else
			{
				a.Normalize();
			}
			for (int i = 0; i < m_numPoints; i++)
			{
				IndexedVector3 scaledPoint = GetScaledPoint(i);
				float num3 = IndexedVector3.Dot(a, scaledPoint);
				if (num3 > num)
				{
					num = num3;
					result = scaledPoint;
				}
			}
			return result;
		}

		public override void BatchedUnitVectorGetSupportingVertexWithoutMargin(IndexedVector3[] vectors, IndexedVector4[] supportVerticesOut, int numVectors)
		{
			for (int i = 0; i < numVectors; i++)
			{
				IndexedVector4 indexedVector = supportVerticesOut[i];
				indexedVector.W = float.MinValue;
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
			return "ConvexPointCloud";
		}

		public override int GetNumVertices()
		{
			return m_unscaledPoints.Count;
		}

		public override int GetNumEdges()
		{
			return 0;
		}

		public override void GetEdge(int i, out IndexedVector3 pa, out IndexedVector3 pb)
		{
			pa = IndexedVector3.Zero;
			pb = IndexedVector3.Zero;
		}

		public override void GetVertex(int i, out IndexedVector3 vtx)
		{
			vtx = m_unscaledPoints[i] * m_localScaling;
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
	}
}
