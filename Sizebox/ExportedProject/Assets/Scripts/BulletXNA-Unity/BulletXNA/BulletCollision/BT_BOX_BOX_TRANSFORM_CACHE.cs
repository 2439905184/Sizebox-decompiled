using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public struct BT_BOX_BOX_TRANSFORM_CACHE
	{
		public IndexedVector3 m_T1to0;

		public IndexedBasisMatrix m_R1to0;

		public IndexedBasisMatrix m_AR;

		public void CalcAbsoluteMatrix()
		{
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					m_AR[i, j] = 1E-06f + Math.Abs(m_R1to0[i, j]);
				}
			}
		}

		public void CalcFromHomogenic(ref IndexedMatrix trans0, ref IndexedMatrix trans1)
		{
			IndexedMatrix indexedMatrix = trans0.Inverse();
			indexedMatrix *= trans1;
			m_T1to0 = indexedMatrix._origin;
			m_R1to0 = indexedMatrix._basis;
			CalcAbsoluteMatrix();
		}

		public void CalcFromFullInvert(ref IndexedMatrix trans0, ref IndexedMatrix trans1)
		{
			m_R1to0 = trans0._basis.Inverse();
			m_T1to0 = m_R1to0 * -trans0._origin;
			m_T1to0 += m_R1to0 * trans1._origin;
			m_R1to0 *= trans1._basis;
			CalcAbsoluteMatrix();
		}

		public IndexedVector3 Transform(ref IndexedVector3 point)
		{
			return new IndexedVector3(m_R1to0._el0.Dot(ref point) + m_T1to0.X, m_R1to0._el1.Dot(ref point) + m_T1to0.Y, m_R1to0._el2.Dot(ref point) + m_T1to0.Z);
		}
	}
}
