using System;

namespace BulletXNA.BulletCollision
{
	public class TriangleInfo
	{
		public int m_flags;

		public float m_edgeV0V1Angle;

		public float m_edgeV1V2Angle;

		public float m_edgeV2V0Angle;

		public TriangleInfo()
		{
			m_edgeV0V1Angle = (float)Math.PI * 2f;
			m_edgeV1V2Angle = (float)Math.PI * 2f;
			m_edgeV2V0Angle = (float)Math.PI * 2f;
			m_flags = 0;
		}
	}
}
