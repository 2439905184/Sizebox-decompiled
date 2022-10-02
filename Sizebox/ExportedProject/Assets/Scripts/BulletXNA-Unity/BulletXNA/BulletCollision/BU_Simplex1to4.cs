using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class BU_Simplex1to4 : PolyhedralConvexAabbCachingShape
	{
		protected int m_numVertices;

		protected IndexedVector3[] m_vertices = new IndexedVector3[4];

		public BU_Simplex1to4()
		{
			m_shapeType = BroadphaseNativeTypes.TETRAHEDRAL_SHAPE_PROXYTYPE;
		}

		public BU_Simplex1to4(ref IndexedVector3 pt0)
		{
			m_shapeType = BroadphaseNativeTypes.TETRAHEDRAL_SHAPE_PROXYTYPE;
			AddVertex(ref pt0);
		}

		public BU_Simplex1to4(ref IndexedVector3 pt0, ref IndexedVector3 pt1)
		{
			m_shapeType = BroadphaseNativeTypes.TETRAHEDRAL_SHAPE_PROXYTYPE;
			AddVertex(ref pt0);
			AddVertex(ref pt1);
		}

		public BU_Simplex1to4(ref IndexedVector3 pt0, ref IndexedVector3 pt1, ref IndexedVector3 pt2)
		{
			m_shapeType = BroadphaseNativeTypes.TETRAHEDRAL_SHAPE_PROXYTYPE;
			AddVertex(ref pt0);
			AddVertex(ref pt1);
			AddVertex(ref pt2);
		}

		public BU_Simplex1to4(ref IndexedVector3 pt0, ref IndexedVector3 pt1, ref IndexedVector3 pt2, ref IndexedVector3 pt3)
		{
			m_shapeType = BroadphaseNativeTypes.TETRAHEDRAL_SHAPE_PROXYTYPE;
			AddVertex(ref pt0);
			AddVertex(ref pt1);
			AddVertex(ref pt2);
			AddVertex(ref pt3);
		}

		public void Reset()
		{
			m_numVertices = 0;
		}

		public override void GetAabb(ref IndexedMatrix t, out IndexedVector3 aabbMin, out IndexedVector3 aabbMax)
		{
			base.GetAabb(ref t, out aabbMin, out aabbMax);
		}

		public void AddVertex(ref IndexedVector3 pt)
		{
			m_vertices[m_numVertices++] = pt;
			RecalcLocalAabb();
		}

		public override int GetNumVertices()
		{
			return m_numVertices;
		}

		public override int GetNumEdges()
		{
			switch (m_numVertices)
			{
			case 0:
				return 0;
			case 1:
				return 0;
			case 2:
				return 1;
			case 3:
				return 3;
			case 4:
				return 6;
			default:
				return 0;
			}
		}

		public override void GetEdge(int i, out IndexedVector3 pa, out IndexedVector3 pb)
		{
			switch (m_numVertices)
			{
			case 2:
				pa = m_vertices[0];
				pb = m_vertices[1];
				return;
			case 3:
				switch (i)
				{
				case 0:
					pa = m_vertices[0];
					pb = m_vertices[1];
					return;
				case 1:
					pa = m_vertices[1];
					pb = m_vertices[2];
					return;
				case 2:
					pa = m_vertices[2];
					pb = m_vertices[0];
					return;
				}
				break;
			case 4:
				switch (i)
				{
				case 0:
					pa = m_vertices[0];
					pb = m_vertices[1];
					return;
				case 1:
					pa = m_vertices[1];
					pb = m_vertices[2];
					return;
				case 2:
					pa = m_vertices[2];
					pb = m_vertices[0];
					return;
				case 3:
					pa = m_vertices[0];
					pb = m_vertices[3];
					return;
				case 4:
					pa = m_vertices[1];
					pb = m_vertices[3];
					return;
				case 5:
					pa = m_vertices[2];
					pb = m_vertices[3];
					return;
				}
				break;
			}
			pa = IndexedVector3.Zero;
			pb = IndexedVector3.Zero;
		}

		public override void GetVertex(int i, out IndexedVector3 vtx)
		{
			vtx = m_vertices[i];
		}

		public override int GetNumPlanes()
		{
			switch (m_numVertices)
			{
			case 0:
				return 0;
			case 1:
				return 0;
			case 2:
				return 0;
			case 3:
				return 2;
			case 4:
				return 4;
			default:
				return 0;
			}
		}

		public override void GetPlane(out IndexedVector3 planeNormal, out IndexedVector3 planeSupport, int i)
		{
			planeNormal = new IndexedVector3(0f, 1f, 0f);
			planeSupport = IndexedVector3.Zero;
		}

		public virtual int GetIndex(int i)
		{
			return 0;
		}

		public override bool IsInside(ref IndexedVector3 pt, float tolerance)
		{
			return false;
		}

		public override string GetName()
		{
			return "btBU_Simplex1to4";
		}
	}
}
