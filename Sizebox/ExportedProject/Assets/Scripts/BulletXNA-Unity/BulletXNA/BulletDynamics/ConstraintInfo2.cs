namespace BulletXNA.BulletDynamics
{
	public class ConstraintInfo2
	{
		public const int maxConstraints = 6;

		public SolverConstraint[] m_solverConstraints = new SolverConstraint[6];

		public int m_numRows;

		public float fps;

		public float erp;

		public int findex;

		public int m_numIterations;

		public float m_damping;

		public void Reset()
		{
			for (int i = 0; i < 6; i++)
			{
				m_solverConstraints[i] = null;
			}
			m_numRows = 0;
			fps = 0f;
			erp = 0f;
			findex = 0;
			m_numIterations = 0;
			m_damping = 0f;
		}

		~ConstraintInfo2()
		{
		}
	}
}
