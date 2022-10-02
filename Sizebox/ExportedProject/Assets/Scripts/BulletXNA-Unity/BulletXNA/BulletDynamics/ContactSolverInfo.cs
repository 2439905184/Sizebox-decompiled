namespace BulletXNA.BulletDynamics
{
	public class ContactSolverInfo : ContactSolverInfoData
	{
		public ContactSolverInfo()
		{
			m_tau = 0.6f;
			m_damping = 1f;
			m_friction = 0.3f;
			m_restitution = 0f;
			m_maxErrorReduction = 20f;
			m_numIterations = 10;
			m_erp = 0.2f;
			m_erp2 = 0.1f;
			m_globalCfm = 0f;
			m_sor = 1f;
			m_splitImpulse = false;
			m_splitImpulsePenetrationThreshold = -0.02f;
			m_linearSlop = 0f;
			m_warmstartingFactor = 0.85f;
			m_solverMode = SolverMode.SOLVER_USE_WARMSTARTING | SolverMode.SOLVER_SIMD;
			m_restingContactRestitutionThreshold = 2;
			m_minimumSolverBatchSize = 128;
		}
	}
}
