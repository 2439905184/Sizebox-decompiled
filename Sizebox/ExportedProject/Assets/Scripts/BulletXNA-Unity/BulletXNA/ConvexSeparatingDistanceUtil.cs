using BulletXNA.LinearMath;

namespace BulletXNA
{
	public class ConvexSeparatingDistanceUtil
	{
		private IndexedQuaternion m_ornA;

		private IndexedQuaternion m_ornB;

		private IndexedVector3 m_posA;

		private IndexedVector3 m_posB;

		private IndexedVector3 m_separatingNormal;

		private float m_boundingRadiusA;

		private float m_boundingRadiusB;

		private float m_separatingDistance;

		public ConvexSeparatingDistanceUtil(float boundingRadiusA, float boundingRadiusB)
		{
			m_boundingRadiusA = boundingRadiusA;
			m_boundingRadiusB = boundingRadiusB;
			m_separatingDistance = 0f;
		}

		public float GetConservativeSeparatingDistance()
		{
			return m_separatingDistance;
		}

		public void UpdateSeparatingDistance(ref IndexedMatrix transA, ref IndexedMatrix transB)
		{
			IndexedVector3 pos = transA._origin;
			IndexedVector3 pos2 = transB._origin;
			IndexedQuaternion orn = transA.GetRotation();
			IndexedQuaternion orn2 = transB.GetRotation();
			if (m_separatingDistance > 0f)
			{
				IndexedVector3 linVel;
				IndexedVector3 angVel;
				TransformUtil.CalculateVelocityQuaternion(ref m_posA, ref pos, ref m_ornA, ref orn, 1f, out linVel, out angVel);
				IndexedVector3 linVel2;
				IndexedVector3 angVel2;
				TransformUtil.CalculateVelocityQuaternion(ref m_posB, ref pos2, ref m_ornB, ref orn2, 1f, out linVel2, out angVel2);
				float num = angVel.Length() * m_boundingRadiusA + angVel2.Length() * m_boundingRadiusB;
				IndexedVector3 indexedVector = linVel2 - linVel;
				float num2 = IndexedVector3.Dot(linVel2 - linVel, m_separatingNormal);
				if (num2 < 0f)
				{
					num2 = 0f;
				}
				float num3 = num + num2;
				m_separatingDistance -= num3;
			}
			m_posA = pos;
			m_posB = pos2;
			m_ornA = orn;
			m_ornB = orn2;
		}

		private void InitSeparatingDistance(ref IndexedVector3 separatingVector, float separatingDistance, ref IndexedMatrix transA, ref IndexedMatrix transB)
		{
			m_separatingNormal = separatingVector;
			m_separatingDistance = separatingDistance;
			IndexedVector3 origin = transA._origin;
			IndexedVector3 origin2 = transB._origin;
			IndexedQuaternion rotation = transA.GetRotation();
			IndexedQuaternion rotation2 = transB.GetRotation();
			m_posA = origin;
			m_posB = origin2;
			m_ornA = rotation;
			m_ornB = rotation2;
		}
	}
}
