using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class QuantizedBvhNode
	{
		public UShortVector3 m_quantizedAabbMin;

		public UShortVector3 m_quantizedAabbMax;

		public int m_escapeIndexOrTriangleIndex;

		public int m_leftChildIndex = -1;

		public int m_rightChildIndex = -1;

		public bool IsLeafNode()
		{
			return m_escapeIndexOrTriangleIndex >= 0;
		}

		public int GetEscapeIndex()
		{
			return -m_escapeIndexOrTriangleIndex;
		}

		public int GetTriangleIndex()
		{
			uint num = 4292870144u;
			return (int)(m_escapeIndexOrTriangleIndex & ~num);
		}

		public int GetPartId()
		{
			return m_escapeIndexOrTriangleIndex >> 21;
		}
	}
}
