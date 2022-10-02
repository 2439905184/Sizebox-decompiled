using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class DebugDrawcallback : ITriangleCallback, IInternalTriangleIndexCallback
	{
		public IDebugDraw m_debugDrawer;

		public IndexedVector3 m_color;

		public IndexedMatrix m_worldTrans;

		public DebugDrawcallback(IDebugDraw debugDrawer, ref IndexedMatrix worldTrans, ref IndexedVector3 color)
		{
			m_debugDrawer = debugDrawer;
			m_color = color;
			m_worldTrans = worldTrans;
		}

		public virtual bool graphics()
		{
			return true;
		}

		public virtual void InternalProcessTriangleIndex(IndexedVector3[] triangle, int partId, int triangleIndex)
		{
			ProcessTriangle(triangle, partId, triangleIndex);
		}

		public virtual void ProcessTriangle(IndexedVector3[] triangle, int partId, int triangleIndex)
		{
			IndexedVector3 from = m_worldTrans * triangle[0];
			IndexedVector3 to = m_worldTrans * triangle[1];
			IndexedVector3 to2 = m_worldTrans * triangle[2];
			IndexedVector3 indexedVector = (from + to + to2) * (1f / 3f);
			if ((m_debugDrawer.GetDebugMode() & DebugDrawModes.DBG_DrawNormals) != 0)
			{
				IndexedVector3 indexedVector2 = (to - from).Cross(to2 - from);
				indexedVector2.Normalize();
				IndexedVector3 color = new IndexedVector3(1f, 1f, 0f);
				m_debugDrawer.DrawLine(indexedVector, indexedVector + indexedVector2, color);
				m_debugDrawer.DrawLine(ref from, ref to, ref m_color);
				m_debugDrawer.DrawLine(ref to, ref to2, ref m_color);
				m_debugDrawer.DrawLine(ref to2, ref from, ref m_color);
			}
		}

		public void Cleanup()
		{
		}
	}
}
