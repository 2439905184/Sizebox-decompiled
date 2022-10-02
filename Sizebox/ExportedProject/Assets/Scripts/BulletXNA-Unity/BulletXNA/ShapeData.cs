using System.Collections.Generic;

namespace BulletXNA
{
	public class ShapeData
	{
		public VertexPositionColor[] m_verticesArray;

		public IList<VertexPositionColor> m_verticesList;

		public short[] m_indexArray;

		public IList<short> m_indexList;

		public ShapeData()
		{
			m_verticesList = new List<VertexPositionColor>();
			m_indexList = new List<short>();
		}

		public ShapeData(int numVert, int numIndices)
		{
			m_verticesArray = new VertexPositionColor[numVert];
			m_indexArray = new short[numIndices];
		}
	}
}
