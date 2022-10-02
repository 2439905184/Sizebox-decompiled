using System;
using BulletXNA.BulletCollision;
using BulletXNA.LinearMath;

namespace BulletXNA
{
	public class DebugDrawcallback : ITriangleCallback, IInternalTriangleIndexCallback, IDisposable
	{
		private IDebugDraw m_debugDrawer;

		private IndexedVector3 m_color;

		private IndexedMatrix m_worldTrans;

		public virtual bool graphics()
		{
			return true;
		}

		public DebugDrawcallback()
		{
		}

		public DebugDrawcallback(IDebugDraw debugDrawer, ref IndexedMatrix worldTrans, ref IndexedVector3 color)
		{
			m_debugDrawer = debugDrawer;
			m_color = color;
			m_worldTrans = worldTrans;
		}

		public void Initialise(IDebugDraw debugDrawer, ref IndexedMatrix worldTrans, ref IndexedVector3 color)
		{
			m_debugDrawer = debugDrawer;
			m_color = color;
			m_worldTrans = worldTrans;
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
			if ((m_debugDrawer.GetDebugMode() & DebugDrawModes.DBG_DrawNormals) != 0)
			{
				IndexedVector3 indexedVector = (from + to + to2) * (1f / 3f);
				(to - from).Cross(to2 - from).Normalize();
				new IndexedVector3(1f, 1f, 0f);
			}
			m_debugDrawer.DrawLine(ref from, ref to, ref m_color);
			m_debugDrawer.DrawLine(ref to, ref to2, ref m_color);
			m_debugDrawer.DrawLine(ref to2, ref from, ref m_color);
		}

		public virtual void Cleanup()
		{
		}

		public void Dispose()
		{
			BulletGlobals.DebugDrawcallbackPool.Free(this);
		}
	}
}
