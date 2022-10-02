using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class Handle : BroadphaseProxy
	{
		public UShortVector3 m_minEdges = default(UShortVector3);

		public UShortVector3 m_maxEdges = default(UShortVector3);

		public BroadphaseProxy m_dbvtProxy;

		public Handle()
		{
			m_minEdges.X = (m_minEdges.Y = (m_minEdges.Z = 52685));
			m_maxEdges.X = (m_maxEdges.Y = (m_maxEdges.Z = 52685));
		}

		public void SetNextFree(ushort next)
		{
			m_minEdges.X = next;
		}

		public ushort GetNextFree()
		{
			return m_minEdges.X;
		}
	}
}
