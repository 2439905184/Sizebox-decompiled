using System.Collections.Generic;

namespace BulletXNA.LinearMath
{
	public class ConvexH
	{
		public class HalfEdge
		{
			public short ea;

			public byte v;

			public byte p;

			public HalfEdge()
			{
			}

			private HalfEdge(short _ea, byte _v, byte _p)
			{
				ea = _ea;
				v = _v;
				p = _p;
			}
		}

		public IList<IndexedVector3> vertices = new ObjectArray<IndexedVector3>();

		public IList<HalfEdge> edges = new ObjectArray<HalfEdge>();

		public IList<IndexedVector4> facets = new ObjectArray<IndexedVector4>();

		private ConvexH()
		{
		}

		public ConvexH(int vertices_size, int edges_size, int facets_size)
		{
		}
	}
}
