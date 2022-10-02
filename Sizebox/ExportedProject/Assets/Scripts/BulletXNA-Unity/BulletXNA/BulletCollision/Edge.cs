namespace BulletXNA.BulletCollision
{
	public class Edge
	{
		public ushort m_pos;

		public ushort m_handle;

		private static Edge swapEdge = new Edge();

		public void Copy(Edge edge)
		{
			m_pos = edge.m_pos;
			m_handle = edge.m_handle;
		}

		public bool IsMax()
		{
			return (m_pos & 1) != 0;
		}

		public static void Swap(Edge a, Edge b)
		{
			swapEdge.Copy(a);
			a.Copy(b);
			b.Copy(swapEdge);
		}
	}
}
