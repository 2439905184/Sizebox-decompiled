using System.Collections;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class SubSimplexClosestResult
	{
		public IndexedVector3 m_closestPointOnSimplex;

		public BitArray m_usedVertices = new BitArray(4);

		public IndexedVector4 m_barycentricCoords = default(IndexedVector4);

		public bool m_degenerate;

		public void Reset()
		{
			m_degenerate = false;
			SetBarycentricCoordinates(0f, 0f, 0f, 0f);
			m_usedVertices.SetAll(false);
		}

		public bool IsValid()
		{
			return m_barycentricCoords.X >= 0f && m_barycentricCoords.Y >= 0f && m_barycentricCoords.Z >= 0f && m_barycentricCoords.W >= 0f;
		}

		public void SetBarycentricCoordinates(float a, float b, float c, float d)
		{
			m_barycentricCoords.X = a;
			m_barycentricCoords.Y = b;
			m_barycentricCoords.Z = c;
			m_barycentricCoords.W = d;
		}
	}
}
