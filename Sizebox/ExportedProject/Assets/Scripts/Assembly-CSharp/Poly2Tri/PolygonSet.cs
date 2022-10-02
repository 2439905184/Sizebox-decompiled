using System.Collections.Generic;

namespace Poly2Tri
{
	public class PolygonSet
	{
		protected List<Polygon> _polygons = new List<Polygon>();

		public IEnumerable<Polygon> Polygons
		{
			get
			{
				return _polygons;
			}
		}

		public PolygonSet()
		{
		}

		public PolygonSet(Polygon poly)
		{
			_polygons.Add(poly);
		}

		public void Add(Polygon p)
		{
			_polygons.Add(p);
		}
	}
}
