namespace BulletXNA.BulletDynamics
{
	public class AngularLimit
	{
		private float m_center;

		private float m_halfRange;

		private float m_softness;

		private float m_biasFactor;

		private float m_relaxationFactor;

		private float m_correction;

		private float m_sign;

		private bool m_solveLimit;

		public AngularLimit()
		{
			m_center = 0f;
			m_halfRange = -1f;
			m_softness = 0.9f;
			m_biasFactor = 0.3f;
			m_relaxationFactor = 1f;
			m_correction = 0f;
			m_sign = 0f;
			m_solveLimit = false;
		}

		public void Set(float low, float high)
		{
			Set(low, high, 0.9f, 0.3f, 1f);
		}

		public void Set(float low, float high, float _softness, float _biasFactor, float _relaxationFactor)
		{
			m_halfRange = (high - low) / 2f;
			m_center = MathUtil.NormalizeAngle(low + m_halfRange);
			m_softness = _softness;
			m_biasFactor = _biasFactor;
			m_relaxationFactor = _relaxationFactor;
		}

		public void Test(float angle)
		{
			m_correction = 0f;
			m_sign = 0f;
			m_solveLimit = false;
			if (m_halfRange >= 0f)
			{
				float num = MathUtil.NormalizeAngle(angle - m_center);
				if (num < 0f - m_halfRange)
				{
					m_solveLimit = true;
					m_correction = 0f - (num + m_halfRange);
					m_sign = 1f;
				}
				else if (num > m_halfRange)
				{
					m_solveLimit = true;
					m_correction = m_halfRange - num;
					m_sign = -1f;
				}
			}
		}

		public float GetSoftness()
		{
			return m_softness;
		}

		public float GetBiasFactor()
		{
			return m_biasFactor;
		}

		public float GetRelaxationFactor()
		{
			return m_relaxationFactor;
		}

		public float GetCorrection()
		{
			return m_correction;
		}

		public float GetSign()
		{
			return m_sign;
		}

		public float GetHalfRange()
		{
			return m_halfRange;
		}

		public bool IsLimit()
		{
			return m_solveLimit;
		}

		public void Fit(ref float angle)
		{
			if (!(m_halfRange > 0f))
			{
				return;
			}
			float num = MathUtil.NormalizeAngle(angle - m_center);
			if (!MathUtil.CompareFloat(num, m_halfRange))
			{
				if (num > 0f)
				{
					angle = GetHigh();
				}
				else
				{
					angle = GetLow();
				}
			}
		}

		public float GetError()
		{
			return m_correction * m_sign;
		}

		public float GetLow()
		{
			return MathUtil.NormalizeAngle(m_center - m_halfRange);
		}

		public float GetHigh()
		{
			return MathUtil.NormalizeAngle(m_center + m_halfRange);
		}
	}
}
