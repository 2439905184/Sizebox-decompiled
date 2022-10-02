using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletDynamics
{
	public class Solve2LinearConstraint
	{
		private float m_tau;

		private float m_damping;

		public Solve2LinearConstraint(float tau, float damping)
		{
			m_tau = tau;
			m_damping = damping;
		}

		public void ResolveUnilateralPairConstraint(RigidBody body0, RigidBody body1, ref IndexedBasisMatrix world2A, ref IndexedBasisMatrix world2B, ref IndexedVector3 invInertiaADiag, float invMassA, ref IndexedVector3 linvelA, ref IndexedVector3 angvelA, ref IndexedVector3 rel_posA1, ref IndexedVector3 invInertiaBDiag, float invMassB, ref IndexedVector3 linvelB, ref IndexedVector3 angvelB, ref IndexedVector3 rel_posA2, float depthA, ref IndexedVector3 normalA, ref IndexedVector3 rel_posB1, ref IndexedVector3 rel_posB2, float depthB, ref IndexedVector3 normalB, out float imp0, out float imp1)
		{
			imp0 = 0f;
			imp1 = 0f;
			float value = Math.Abs(normalA.Length()) - 1f;
			if (!(Math.Abs(value) >= 1.1920929E-07f))
			{
				JacobianEntry jacobianEntry = new JacobianEntry(ref world2A, ref world2B, ref rel_posA1, ref rel_posA2, ref normalA, ref invInertiaADiag, invMassA, ref invInertiaBDiag, invMassB);
				JacobianEntry jacobianEntry2 = new JacobianEntry(ref world2A, ref world2B, ref rel_posB1, ref rel_posB2, ref normalB, ref invInertiaADiag, invMassA, ref invInertiaBDiag, invMassB);
				float num = IndexedVector3.Dot(normalA, body0.GetVelocityInLocalPoint(ref rel_posA1) - body1.GetVelocityInLocalPoint(ref rel_posA1));
				float num2 = IndexedVector3.Dot(normalB, body0.GetVelocityInLocalPoint(ref rel_posB1) - body1.GetVelocityInLocalPoint(ref rel_posB1));
				float num3 = 1f / (invMassA + invMassB);
				float num4 = depthA * m_tau * num3 - num * m_damping;
				float num5 = depthB * m_tau * num3 - num2 * m_damping;
				float nonDiagonal = jacobianEntry.GetNonDiagonal(jacobianEntry2, invMassA, invMassB);
				float num6 = 1f / (jacobianEntry.GetDiagonal() * jacobianEntry2.GetDiagonal() - nonDiagonal * nonDiagonal);
				imp0 = num4 * jacobianEntry.GetDiagonal() * num6 + num5 * (0f - nonDiagonal) * num6;
				imp1 = num5 * jacobianEntry2.GetDiagonal() * num6 + num4 * (0f - nonDiagonal) * num6;
			}
		}

		private void ResolveBilateralPairraint(RigidBody body0, RigidBody body1, ref IndexedBasisMatrix world2A, ref IndexedBasisMatrix world2B, ref IndexedVector3 invInertiaADiag, float invMassA, ref IndexedVector3 linvelA, ref IndexedVector3 angvelA, ref IndexedVector3 rel_posA1, ref IndexedVector3 invInertiaBDiag, float invMassB, ref IndexedVector3 linvelB, ref IndexedVector3 angvelB, ref IndexedVector3 rel_posA2, float depthA, ref IndexedVector3 normalA, ref IndexedVector3 rel_posB1, ref IndexedVector3 rel_posB2, float depthB, ref IndexedVector3 normalB, ref float imp0, ref float imp1)
		{
			imp0 = 0f;
			imp1 = 0f;
			float value = Math.Abs(normalA.Length()) - 1f;
			if (Math.Abs(value) >= 1.1920929E-07f)
			{
				return;
			}
			JacobianEntry jacobianEntry = new JacobianEntry(ref world2A, ref world2B, ref rel_posA1, ref rel_posA2, ref normalA, ref invInertiaADiag, invMassA, ref invInertiaBDiag, invMassB);
			JacobianEntry jacobianEntry2 = new JacobianEntry(ref world2A, ref world2B, ref rel_posB1, ref rel_posB2, ref normalB, ref invInertiaADiag, invMassA, ref invInertiaBDiag, invMassB);
			float num = IndexedVector3.Dot(normalA, body0.GetVelocityInLocalPoint(ref rel_posA1) - body1.GetVelocityInLocalPoint(ref rel_posA1));
			float num2 = IndexedVector3.Dot(normalB, body0.GetVelocityInLocalPoint(ref rel_posB1) - body1.GetVelocityInLocalPoint(ref rel_posB1));
			float num3 = depthA * m_tau - num * m_damping;
			float num4 = depthB * m_tau - num2 * m_damping;
			float nonDiagonal = jacobianEntry.GetNonDiagonal(jacobianEntry2, invMassA, invMassB);
			float num5 = 1f / (jacobianEntry.GetDiagonal() * jacobianEntry2.GetDiagonal() - nonDiagonal * nonDiagonal);
			imp0 = num3 * jacobianEntry.GetDiagonal() * num5 + num4 * (0f - nonDiagonal) * num5;
			imp1 = num4 * jacobianEntry2.GetDiagonal() * num5 + num3 * (0f - nonDiagonal) * num5;
			if (imp0 > 0f)
			{
				if (!(imp1 > 0f))
				{
					imp1 = 0f;
					imp0 = num3 / jacobianEntry.GetDiagonal();
					if (!(imp0 > 0f))
					{
						imp0 = 0f;
					}
				}
				return;
			}
			imp0 = 0f;
			imp1 = num4 / jacobianEntry2.GetDiagonal();
			if (imp1 <= 0f)
			{
				imp1 = 0f;
				imp0 = num3 / jacobianEntry.GetDiagonal();
				if (!(imp0 > 0f))
				{
					imp0 = 0f;
				}
			}
		}
	}
}
