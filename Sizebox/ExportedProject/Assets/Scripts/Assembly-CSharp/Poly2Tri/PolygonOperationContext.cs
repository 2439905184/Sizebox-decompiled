using System;
using System.Collections.Generic;

namespace Poly2Tri
{
	public class PolygonOperationContext
	{
		public PolygonUtil.PolyOperation mOperations;

		public Point2DList mOriginalPolygon1;

		public Point2DList mOriginalPolygon2;

		public Point2DList mPoly1;

		public Point2DList mPoly2;

		public List<EdgeIntersectInfo> mIntersections;

		public int mStartingIndex;

		public PolygonUtil.PolyUnionError mError;

		public List<int> mPoly1VectorAngles;

		public List<int> mPoly2VectorAngles;

		public Dictionary<uint, Point2DList> mOutput = new Dictionary<uint, Point2DList>();

		public Point2DList Union
		{
			get
			{
				Point2DList value = null;
				if (!mOutput.TryGetValue(1u, out value))
				{
					value = new Point2DList();
					mOutput.Add(1u, value);
				}
				return value;
			}
		}

		public Point2DList Intersect
		{
			get
			{
				Point2DList value = null;
				if (!mOutput.TryGetValue(2u, out value))
				{
					value = new Point2DList();
					mOutput.Add(2u, value);
				}
				return value;
			}
		}

		public Point2DList Subtract
		{
			get
			{
				Point2DList value = null;
				if (!mOutput.TryGetValue(4u, out value))
				{
					value = new Point2DList();
					mOutput.Add(4u, value);
				}
				return value;
			}
		}

		public void Clear()
		{
			mOperations = PolygonUtil.PolyOperation.None;
			mOriginalPolygon1 = null;
			mOriginalPolygon2 = null;
			mPoly1 = null;
			mPoly2 = null;
			mIntersections = null;
			mStartingIndex = -1;
			mError = PolygonUtil.PolyUnionError.None;
			mPoly1VectorAngles = null;
			mPoly2VectorAngles = null;
			mOutput = new Dictionary<uint, Point2DList>();
		}

		public bool Init(PolygonUtil.PolyOperation operations, Point2DList polygon1, Point2DList polygon2)
		{
			Clear();
			mOperations = operations;
			mOriginalPolygon1 = polygon1;
			mOriginalPolygon2 = polygon2;
			mPoly1 = new Point2DList(polygon1);
			mPoly1.WindingOrder = Point2DList.WindingOrderType.CCW;
			mPoly2 = new Point2DList(polygon2);
			mPoly2.WindingOrder = Point2DList.WindingOrderType.CCW;
			if (!VerticesIntersect(mPoly1, mPoly2, out mIntersections))
			{
				mError = PolygonUtil.PolyUnionError.NoIntersections;
				return false;
			}
			int count = mIntersections.Count;
			for (int i = 0; i < count; i++)
			{
				for (int j = i + 1; j < count; j++)
				{
					if (mIntersections[i].EdgeOne.EdgeStart.Equals(mIntersections[j].EdgeOne.EdgeStart) && mIntersections[i].EdgeOne.EdgeEnd.Equals(mIntersections[j].EdgeOne.EdgeEnd))
					{
						mIntersections[j].EdgeOne.EdgeStart = mIntersections[i].IntersectionPoint;
					}
					if (mIntersections[i].EdgeTwo.EdgeStart.Equals(mIntersections[j].EdgeTwo.EdgeStart) && mIntersections[i].EdgeTwo.EdgeEnd.Equals(mIntersections[j].EdgeTwo.EdgeEnd))
					{
						mIntersections[j].EdgeTwo.EdgeStart = mIntersections[i].IntersectionPoint;
					}
				}
			}
			foreach (EdgeIntersectInfo mIntersection in mIntersections)
			{
				if (!mPoly1.Contains(mIntersection.IntersectionPoint))
				{
					mPoly1.Insert(mPoly1.IndexOf(mIntersection.EdgeOne.EdgeStart) + 1, mIntersection.IntersectionPoint);
				}
				if (!mPoly2.Contains(mIntersection.IntersectionPoint))
				{
					mPoly2.Insert(mPoly2.IndexOf(mIntersection.EdgeTwo.EdgeStart) + 1, mIntersection.IntersectionPoint);
				}
			}
			mPoly1VectorAngles = new List<int>();
			for (int k = 0; k < mPoly2.Count; k++)
			{
				mPoly1VectorAngles.Add(-1);
			}
			mPoly2VectorAngles = new List<int>();
			for (int l = 0; l < mPoly1.Count; l++)
			{
				mPoly2VectorAngles.Add(-1);
			}
			int num = 0;
			do
			{
				bool flag = PointInPolygonAngle(mPoly1[num], mPoly2);
				mPoly2VectorAngles[num] = (flag ? 1 : 0);
				if (flag)
				{
					mStartingIndex = num;
					break;
				}
				num = mPoly1.NextIndex(num);
			}
			while (num != 0);
			if (mStartingIndex == -1)
			{
				mError = PolygonUtil.PolyUnionError.Poly1InsidePoly2;
				return false;
			}
			return true;
		}

		private bool VerticesIntersect(Point2DList polygon1, Point2DList polygon2, out List<EdgeIntersectInfo> intersections)
		{
			intersections = new List<EdgeIntersectInfo>();
			double epsilon = Math.Min(polygon1.Epsilon, polygon2.Epsilon);
			for (int i = 0; i < polygon1.Count; i++)
			{
				Point2D point2D = polygon1[i];
				Point2D point2D2 = polygon1[polygon1.NextIndex(i)];
				for (int j = 0; j < polygon2.Count; j++)
				{
					Point2D pIntersectionPt = new Point2D();
					Point2D point2D3 = polygon2[j];
					Point2D point2D4 = polygon2[polygon2.NextIndex(j)];
					if (TriangulationUtil.LinesIntersect2D(point2D, point2D2, point2D3, point2D4, ref pIntersectionPt, epsilon))
					{
						intersections.Add(new EdgeIntersectInfo(new Edge(point2D, point2D2), new Edge(point2D3, point2D4), pIntersectionPt));
					}
				}
			}
			return intersections.Count > 0;
		}

		public bool PointInPolygonAngle(Point2D point, Point2DList polygon)
		{
			double num = 0.0;
			for (int i = 0; i < polygon.Count; i++)
			{
				Point2D p = polygon[i] - point;
				Point2D p2 = polygon[polygon.NextIndex(i)] - point;
				num += VectorAngle(p, p2);
			}
			if (Math.Abs(num) < Math.PI)
			{
				return false;
			}
			return true;
		}

		public double VectorAngle(Point2D p1, Point2D p2)
		{
			double num = Math.Atan2(p1.Y, p1.X);
			double num2;
			for (num2 = Math.Atan2(p2.Y, p2.X) - num; num2 > Math.PI; num2 -= Math.PI * 2.0)
			{
			}
			for (; num2 < -Math.PI; num2 += Math.PI * 2.0)
			{
			}
			return num2;
		}
	}
}
