using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class DispatcherInfo
	{
		private float m_timeStep;

		private int m_stepCount;

		private DispatchFunc m_dispatchFunc;

		private float m_timeOfImpact;

		public bool m_useContinuous;

		public IDebugDraw m_debugDraw;

		public bool m_enableSatConvex;

		private bool m_enableSPU;

		private bool m_useEpa;

		private float m_allowedCcdPenetration;

		private bool m_useConvexConservativeDistanceUtil;

		private float m_convexConservativeDistanceThreshold;

		public DispatcherInfo()
		{
			m_timeStep = 0f;
			m_stepCount = 0;
			m_dispatchFunc = DispatchFunc.DISPATCH_DISCRETE;
			m_timeOfImpact = 1f;
			m_useContinuous = true;
			m_debugDraw = null;
			m_enableSatConvex = false;
			m_enableSPU = true;
			m_useEpa = true;
			m_allowedCcdPenetration = 0.04f;
			m_useConvexConservativeDistanceUtil = false;
			m_convexConservativeDistanceThreshold = 0f;
		}

		public DispatchFunc GetDispatchFunc()
		{
			return m_dispatchFunc;
		}

		public void SetDispatchFunc(DispatchFunc func)
		{
			m_dispatchFunc = func;
		}

		public float GetTimeOfImpact()
		{
			return m_timeOfImpact;
		}

		public void SetTimeOfImpact(float toi)
		{
			m_timeOfImpact = toi;
		}

		public float GetTimeStep()
		{
			return m_timeStep;
		}

		public void SetTimeStep(float value)
		{
			m_timeStep = value;
		}

		public int GetStepCount()
		{
			return m_stepCount;
		}

		public void SetStepCount(int count)
		{
			m_stepCount = count;
		}

		public IDebugDraw getDebugDraw()
		{
			return m_debugDraw;
		}

		public void SetDebugDraw(IDebugDraw debugDraw)
		{
			m_debugDraw = debugDraw;
		}

		public float GetAllowedCcdPenetration()
		{
			return m_allowedCcdPenetration;
		}

		public void SetAllowedCcdPenetration(float ccd)
		{
			m_allowedCcdPenetration = ccd;
		}

		public float GetConvexConservativeDistanceThreshold()
		{
			return m_convexConservativeDistanceThreshold;
		}
	}
}
