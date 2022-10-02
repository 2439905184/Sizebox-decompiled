using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class BoxShape : PolyhedralConvexShape
	{
		public virtual IndexedVector3 GetHalfExtentsWithMargin()
		{
			return GetHalfExtentsWithoutMargin() + new IndexedVector3(GetMargin());
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

		public BoxShape(IndexedVector3 boxHalfExtents)
			: this(ref boxHalfExtents)
		{
		}

		public BoxShape(ref IndexedVector3 boxHalfExtents)
		{
			m_shapeType = BroadphaseNativeTypes.BOX_SHAPE_PROXYTYPE;
			SetSafeMargin(ref boxHalfExtents);
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

		public override void GetAabb(ref IndexedMatrix trans, out IndexedVector3 aabbMin, out IndexedVector3 aabbMax)
		{
			IndexedVector3 halfExtents = GetHalfExtentsWithoutMargin();
			AabbUtil2.TransformAabb(ref halfExtents, GetMargin(), ref trans, out aabbMin, out aabbMax);
		}

		public override void CalculateLocalInertia(float mass, out IndexedVector3 inertia)
		{
			IndexedVector3 halfExtentsWithMargin = GetHalfExtentsWithMargin();
			float num = 2f * halfExtentsWithMargin.X;
			float num2 = 2f * halfExtentsWithMargin.Y;
			float num3 = 2f * halfExtentsWithMargin.Z;
			float num4 = mass / 12f;
			inertia = new IndexedVector3(num4 * (num2 * num2 + num3 * num3), num4 * (num * num + num3 * num3), num4 * (num * num + num2 * num2));
		}

		public override void GetPlane(out IndexedVector3 planeNormal, out IndexedVector3 planeSupport, int i)
		{
			IndexedVector4 plane;
			GetPlaneEquation(out plane, i);
			planeNormal = plane.ToVector3();
			IndexedVector3 vec = -planeNormal;
			planeSupport = LocalGetSupportingVertex(ref vec);
		}

		public override int GetNumPlanes()
		{
			return 6;
		}

		public override int GetNumVertices()
		{
			return 8;
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

		public virtual void GetPlaneEquation(out IndexedVector4 plane, int i)
		{
			IndexedVector3 halfExtentsWithoutMargin = GetHalfExtentsWithoutMargin();
			switch (i)
			{
			case 0:
				plane = new IndexedVector4(IndexedVector3.Right, 0f - halfExtentsWithoutMargin.X);
				break;
			case 1:
				plane = new IndexedVector4(IndexedVector3.Left, 0f - halfExtentsWithoutMargin.X);
				break;
			case 2:
				plane = new IndexedVector4(IndexedVector3.Up, 0f - halfExtentsWithoutMargin.Y);
				break;
			case 3:
				plane = new IndexedVector4(IndexedVector3.Down, 0f - halfExtentsWithoutMargin.Y);
				break;
			case 4:
				plane = new IndexedVector4(IndexedVector3.Backward, 0f - halfExtentsWithoutMargin.Z);
				break;
			case 5:
				plane = new IndexedVector4(IndexedVector3.Forward, 0f - halfExtentsWithoutMargin.Z);
				break;
			default:
				plane = default(IndexedVector4);
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
			return "Box";
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
				penetrationVector = IndexedVector3.Right;
				break;
			case 1:
				penetrationVector = IndexedVector3.Left;
				break;
			case 2:
				penetrationVector = IndexedVector3.Up;
				break;
			case 3:
				penetrationVector = IndexedVector3.Down;
				break;
			case 4:
				penetrationVector = IndexedVector3.Backward;
				break;
			case 5:
				penetrationVector = IndexedVector3.Forward;
				break;
			default:
				penetrationVector = IndexedVector3.Zero;
				break;
			}
		}
	}
}
