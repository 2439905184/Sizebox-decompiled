using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class TetrahedronShapeEx : BU_Simplex1to4
	{
		public TetrahedronShapeEx()
		{
			m_numVertices = 4;
		}

		public void SetVertices(ref IndexedVector3 v0, ref IndexedVector3 v1, ref IndexedVector3 v2, ref IndexedVector3 v3)
		{
			m_vertices[0] = v0;
			m_vertices[1] = v1;
			m_vertices[2] = v2;
			m_vertices[3] = v3;
			RecalcLocalAabb();
		}
	}
}
