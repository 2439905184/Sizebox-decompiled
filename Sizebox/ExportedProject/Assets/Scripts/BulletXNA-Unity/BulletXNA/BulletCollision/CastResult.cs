using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class CastResult
	{
		public IndexedMatrix m_hitTransformA;

		public IndexedMatrix m_hitTransformB;

		public IndexedVector3 m_normal;

		public IndexedVector3 m_hitPoint;

		public float m_fraction;

		public IDebugDraw m_debugDrawer;

		public float m_allowedPenetration;

		public virtual void DebugDraw(float fraction)
		{
		}

		public virtual void DrawCoordSystem(ref IndexedMatrix trans)
		{
		}

		public virtual void ReportFailure(int errNo, int numIterations)
		{
		}

		public CastResult()
		{
			m_fraction = float.MaxValue;
			m_debugDrawer = null;
			m_allowedPenetration = 0f;
		}

		public virtual void Cleanup()
		{
			m_fraction = float.MaxValue;
			m_debugDrawer = null;
			m_allowedPenetration = 0f;
			BulletGlobals.CastResultPool.Free(this);
		}
	}
}
