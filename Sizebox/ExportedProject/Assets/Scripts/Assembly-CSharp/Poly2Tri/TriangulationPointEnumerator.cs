using System;
using System.Collections;
using System.Collections.Generic;

namespace Poly2Tri
{
	public class TriangulationPointEnumerator : IEnumerator<TriangulationPoint>, IEnumerator, IDisposable
	{
		protected IList<Point2D> mPoints;

		protected int position = -1;

		object IEnumerator.Current
		{
			get
			{
				return Current;
			}
		}

		public TriangulationPoint Current
		{
			get
			{
				if (position < 0 || position >= mPoints.Count)
				{
					return null;
				}
				return mPoints[position] as TriangulationPoint;
			}
		}

		public TriangulationPointEnumerator(IList<Point2D> points)
		{
			mPoints = points;
		}

		public bool MoveNext()
		{
			position++;
			return position < mPoints.Count;
		}

		public void Reset()
		{
			position = -1;
		}

		void IDisposable.Dispose()
		{
		}
	}
}
