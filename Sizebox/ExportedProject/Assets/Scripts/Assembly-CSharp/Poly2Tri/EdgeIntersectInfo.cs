namespace Poly2Tri
{
	public class EdgeIntersectInfo
	{
		public Edge EdgeOne { get; private set; }

		public Edge EdgeTwo { get; private set; }

		public Point2D IntersectionPoint { get; private set; }

		public EdgeIntersectInfo(Edge edgeOne, Edge edgeTwo, Point2D intersectionPoint)
		{
			EdgeOne = edgeOne;
			EdgeTwo = edgeTwo;
			IntersectionPoint = intersectionPoint;
		}
	}
}
