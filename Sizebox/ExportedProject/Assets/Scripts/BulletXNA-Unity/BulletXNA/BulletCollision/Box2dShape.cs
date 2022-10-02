using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class Box2dShape : PolyhedralConvexShape
	{
		private IndexedVector3 m_centroid;

		private IndexedVector3[] m_vertices = new IndexedVector3[4];

		private IndexedVector3[] m_normals = new IndexedVector3[4];

		public virtual IndexedVector3 GetHalfExtentsWithMargin()
		{
			IndexedVector3 halfExtentsWithoutMargin = GetHalfExtentsWithoutMargin();
			IndexedVector3 indexedVector = new IndexedVector3(GetMargin());
			return halfExtentsWithoutMargin + indexedVector;
		}

		public virtual IndexedVector3 GetHalfExtentsWithoutMargin()
		{
			return m_implicitShapeDimensions;
		}

		public override IndexedVector3 LocalGetSupportingVertex(ref IndexedVector3 vec)
		{
			IndexedVector3 halfExtentsWithoutMargin = GetHalfExtentsWithoutMargin();
			IndexedVector3 indexedVector = new IndexedVector3(GetMargin());
			halfExtentsWithoutMargin += indexedVector;
			return new IndexedVector3(MathUtil.FSel(vec.X, halfExtentsWithoutMargin.X, 0f - halfExtentsWithoutMargin.X), MathUtil.FSel(vec.Y, halfExtentsWithoutMargin.Y, 0f - halfExtentsWithoutMargin.Y), MathUtil.FSel(vec.Z, halfExtentsWithoutMargin.Z, 0f - halfExtentsWithoutMargin.Z));
		}

		public override IndexedVector3 LocalGetSupportingVertexWithoutMargin(ref IndexedVector3 vec)
		{
			IndexedVector3 halfExtentsWithoutMargin = GetHalfExtentsWithoutMargin();
			return new IndexedVector3(MathUtil.FSel(vec.X, halfExtentsWithoutMargin.X, 0f - halfExtentsWithoutMargin.X), MathUtil.FSel(vec.Y, halfExtentsWithoutMargin.Y, 0f - halfExtentsWithoutMargin.Y), MathUtil.FSel(vec.Z, halfExtentsWithoutMargin.Z, 0f - halfExtentsWithoutMargin.Z));
		}

		public override void BatchedUnitVectorGetSupportingVertexWithoutMargin(IndexedVector3[] vectors, IndexedVector4[] supportVerticesOut, int numVectors)
		{
			IndexedVector3 halfExtentsWithoutMargin = GetHalfExtentsWithoutMargin();
			for (int i = 0; i < numVectors; i++)
			{
				IndexedVector3 indexedVector = vectors[i];
				supportVerticesOut[i] = new IndexedVector4(MathUtil.FSel(indexedVector.X, halfExtentsWithoutMargin.X, 0f - halfExtentsWithoutMargin.X), MathUtil.FSel(indexedVector.Y, halfExtentsWithoutMargin.Y, 0f - halfExtentsWithoutMargin.Y), MathUtil.FSel(indexedVector.Z, halfExtentsWithoutMargin.Z, 0f - halfExtentsWithoutMargin.Z), 0f);
			}
		}

		public Box2dShape(ref IndexedVector3 boxHalfExtents)
		{
			m_centroid = IndexedVector3.Zero;
			m_vertices[0] = new IndexedVector3(0f - boxHalfExtents.X, 0f - boxHalfExtents.Y, 0f);
			m_vertices[1] = new IndexedVector3(boxHalfExtents.X, 0f - boxHalfExtents.Y, 0f);
			m_vertices[2] = new IndexedVector3(boxHalfExtents.X, boxHalfExtents.Y, 0f);
			m_vertices[3] = new IndexedVector3(0f - boxHalfExtents.X, boxHalfExtents.Y, 0f);
			m_normals[0] = new IndexedVector3(0f, -1f, 0f);
			m_normals[1] = new IndexedVector3(1f, 0f, 0f);
			m_normals[2] = new IndexedVector3(0f, 1f, 0f);
			m_normals[3] = new IndexedVector3(-1f, 0f, 0f);
			float num = boxHalfExtents.X;
			if (num > boxHalfExtents.Y)
			{
				num = boxHalfExtents.Y;
			}
			SetSafeMargin(num);
			m_shapeType = BroadphaseNativeTypes.BOX_2D_SHAPE_PROXYTYPE;
			IndexedVector3 indexedVector = new IndexedVector3(GetMargin());
			m_implicitShapeDimensions = boxHalfExtents * m_localScaling - indexedVector;
		}

		public override void SetMargin(float collisionMargin)
		{
			IndexedVector3 indexedVector = new IndexedVector3(GetMargin());
			IndexedVector3 indexedVector2 = m_implicitShapeDimensions + indexedVector;
			base.SetMargin(collisionMargin);
			IndexedVector3 indexedVector3 = new IndexedVector3(GetMargin());
			m_implicitShapeDimensions = indexedVector2 - indexedVector3;
		}

		public override void SetLocalScaling(ref IndexedVector3 scaling)
		{
			IndexedVector3 indexedVector = new IndexedVector3(GetMargin());
			IndexedVector3 indexedVector2 = m_implicitShapeDimensions + indexedVector;
			IndexedVector3 indexedVector3 = indexedVector2 / m_localScaling;
			base.SetLocalScaling(ref scaling);
			m_implicitShapeDimensions = indexedVector3 * m_localScaling - indexedVector;
		}

		public override void GetAabb(ref IndexedMatrix t, out IndexedVector3 aabbMin, out IndexedVector3 aabbMax)
		{
			AabbUtil2.TransformAabb(GetHalfExtentsWithoutMargin(), GetMargin(), ref t, out aabbMin, out aabbMax);
		}

		public override void CalculateLocalInertia(float mass, out IndexedVector3 inertia)
		{
			IndexedVector3 halfExtentsWithMargin = GetHalfExtentsWithMargin();
			float num = 2f * halfExtentsWithMargin.X;
			float num2 = 2f * halfExtentsWithMargin.Y;
			float num3 = 2f * halfExtentsWithMargin.Z;
			inertia = new IndexedVector3(mass / 12f * (num2 * num2 + num3 * num3), mass / 12f * (num * num + num3 * num3), mass / 12f * (num * num + num2 * num2));
		}

		public virtual int GetVertexCount()
		{
			return 4;
		}

		public override int GetNumVertices()
		{
			return 4;
		}

		public IndexedVector3[] GetVertices()
		{
			return m_vertices;
		}

		public IndexedVector3[] GetNormals()
		{
			return m_normals;
		}

		public override void GetPlane(out IndexedVector3 planeNormal, out IndexedVector3 planeSupport, int i)
		{
			IndexedVector4 plane;
			GetPlaneEquation(out plane, i);
			planeNormal = new IndexedVector3(plane.X, plane.Y, plane.Z);
			planeSupport = LocalGetSupportingVertex(-planeNormal);
		}

		public IndexedVector3 GetCentroid()
		{
			return m_centroid;
		}

		public override int GetNumPlanes()
		{
			return 6;
		}

		public override int GetNumEdges()
		{
			return 12;
		}

		public override void GetVertex(int i, out IndexedVector3 vtx)
		{
			IndexedVector3 halfExtentsWithoutMargin = GetHalfExtentsWithoutMargin();
			vtx = new IndexedVector3(halfExtentsWithoutMargin.X * (float)(1 - (i & 1)) - halfExtentsWithoutMargin.X * (float)(i & 1), halfExtentsWithoutMargin.Y * (float)(1 - ((i & 2) >> 1)) - halfExtentsWithoutMargin.Y * (float)((i & 2) >> 1), halfExtentsWithoutMargin.Z * (float)(1 - ((i & 4) >> 2)) - halfExtentsWithoutMargin.Z * (float)((i & 4) >> 2));
		}

		public void GetPlaneEquation(out IndexedVector4 plane, int i)
		{
			IndexedVector3 halfExtentsWithoutMargin = GetHalfExtentsWithoutMargin();
			switch (i)
			{
			case 0:
				plane = new IndexedVector4(1f, 0f, 0f, 0f - halfExtentsWithoutMargin.X);
				break;
			case 1:
				plane = new IndexedVector4(-1f, 0f, 0f, 0f - halfExtentsWithoutMargin.X);
				break;
			case 2:
				plane = new IndexedVector4(0f, 1f, 0f, 0f - halfExtentsWithoutMargin.Y);
				break;
			case 3:
				plane = new IndexedVector4(0f, -1f, 0f, 0f - halfExtentsWithoutMargin.Y);
				break;
			case 4:
				plane = new IndexedVector4(0f, 0f, 1f, 0f - halfExtentsWithoutMargin.Z);
				break;
			case 5:
				plane = new IndexedVector4(0f, 0f, -1f, 0f - halfExtentsWithoutMargin.Z);
				break;
			default:
				plane = IndexedVector4.Zero;
				break;
			}
		}

		public override void GetEdge(int i, out IndexedVector3 pa, out IndexedVector3 pb)
		{
			int i2 = 0;
			int i3 = 0;
			switch (i)
			{
			case 0:
				i2 = 0;
				i3 = 1;
				break;
			case 1:
				i2 = 0;
				i3 = 2;
				break;
			case 2:
				i2 = 1;
				i3 = 3;
				break;
			case 3:
				i2 = 2;
				i3 = 3;
				break;
			case 4:
				i2 = 0;
				i3 = 4;
				break;
			case 5:
				i2 = 1;
				i3 = 5;
				break;
			case 6:
				i2 = 2;
				i3 = 6;
				break;
			case 7:
				i2 = 3;
				i3 = 7;
				break;
			case 8:
				i2 = 4;
				i3 = 5;
				break;
			case 9:
				i2 = 4;
				i3 = 6;
				break;
			case 10:
				i2 = 5;
				i3 = 7;
				break;
			case 11:
				i2 = 6;
				i3 = 7;
				break;
			}
			GetVertex(i2, out pa);
			GetVertex(i3, out pb);
		}

		public override bool IsInside(ref IndexedVector3 pt, float tolerance)
		{
			IndexedVector3 halfExtentsWithoutMargin = GetHalfExtentsWithoutMargin();
			return pt.X <= halfExtentsWithoutMargin.X + tolerance && pt.X >= 0f - halfExtentsWithoutMargin.X - tolerance && pt.Y <= halfExtentsWithoutMargin.Y + tolerance && pt.Y >= 0f - halfExtentsWithoutMargin.Y - tolerance && pt.Z <= halfExtentsWithoutMargin.Z + tolerance && pt.Z >= 0f - halfExtentsWithoutMargin.Z - tolerance;
		}

		public override string GetName()
		{
			return "Box2d";
		}

		public override int GetNumPreferredPenetrationDirections()
		{
			return 6;
		}

		public override void GetPreferredPenetrationDirection(int index, out IndexedVector3 penetrationVector)
		{
			switch (index)
			{
			case 0:
				penetrationVector = new IndexedVector3(1f, 0f, 0f);
				break;
			case 1:
				penetrationVector = new IndexedVector3(-1f, 0f, 0f);
				break;
			case 2:
				penetrationVector = new IndexedVector3(0f, 1f, 0f);
				break;
			case 3:
				penetrationVector = new IndexedVector3(0f, -1f, 0f);
				break;
			case 4:
				penetrationVector = new IndexedVector3(0f, 0f, 1f);
				break;
			case 5:
				penetrationVector = new IndexedVector3(0f, 0f, -1f);
				break;
			default:
				penetrationVector = IndexedVector3.Zero;
				break;
			}
		}
	}
}
