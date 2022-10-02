using System;
using System.Collections.Generic;

namespace BulletXNA.BulletCollision
{
	public class TriangleInfoMap : Dictionary<int, TriangleInfo>
	{
		public const int TRI_INFO_V0V1_CONVEX = 1;

		public const int TRI_INFO_V1V2_CONVEX = 2;

		public const int TRI_INFO_V2V0_CONVEX = 4;

		public const int TRI_INFO_V0V1_SWAP_NORMALB = 8;

		public const int TRI_INFO_V1V2_SWAP_NORMALB = 16;

		public const int TRI_INFO_V2V0_SWAP_NORMALB = 32;

		public float m_convexEpsilon;

		public float m_planarEpsilon;

		public float m_equalVertexThreshold;

		public float m_edgeDistanceThreshold;

		public float m_maxEdgeAngleThreshold;

		public float m_zeroAreaThreshold;

		public TriangleInfoMap()
		{
			m_convexEpsilon = 0f;
			m_planarEpsilon = 0.0001f;
			m_equalVertexThreshold = 9.999999E-09f;
			m_edgeDistanceThreshold = 0.1f;
			m_zeroAreaThreshold = 9.999999E-09f;
			m_maxEdgeAngleThreshold = (float)Math.PI * 2f;
		}
	}
}
