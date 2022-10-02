using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public abstract class PolyhedralConvexShape : ConvexInternalShape
	{
		protected ConvexPolyhedron m_polyhedron;

		public override IndexedVector3 LocalGetSupportingVertexWithoutMargin(ref IndexedVector3 vec)
		{
			return IndexedVector3.Zero;
		}

		public override void Cleanup()
		{
			base.Cleanup();
		}

		public virtual bool InitializePolyhedralFeatures()
		{
			return true;
		}

		public ConvexPolyhedron GetConvexPolyhedron()
		{
			return m_polyhedron;
		}

		public override void BatchedUnitVectorGetSupportingVertexWithoutMargin(IndexedVector3[] vectors, IndexedVector4[] supportVerticesOut, int numVectors)
		{
			float num = 0f;
			for (int i = 0; i < numVectors; i++)
			{
				IndexedVector4 indexedVector = supportVerticesOut[i];
				indexedVector.W = -1E+18f;
				supportVerticesOut[i] = indexedVector;
			}
			for (int j = 0; j < numVectors; j++)
			{
				IndexedVector3 a = vectors[j];
				for (int i = 0; i < GetNumVertices(); i++)
				{
					IndexedVector3 vtx;
					GetVertex(i, out vtx);
					num = IndexedVector3.Dot(a, vtx);
					if (num > supportVerticesOut[j].W)
					{
						supportVerticesOut[j] = new IndexedVector4(vtx, num);
					}
				}
			}
		}

		public override void CalculateLocalInertia(float mass, out IndexedVector3 inertia)
		{
			float margin = GetMargin();
			IndexedMatrix t = IndexedMatrix.Identity;
			IndexedVector3 aabbMin = IndexedVector3.Zero;
			IndexedVector3 aabbMax = IndexedVector3.Zero;
			GetAabb(ref t, out aabbMin, out aabbMax);
			IndexedVector3 indexedVector = (aabbMax - aabbMin) * 0.5f;
			float num = 2f * (indexedVector.X + margin);
			float num2 = 2f * (indexedVector.Y + margin);
			float num3 = 2f * (indexedVector.Z + margin);
			float num4 = num * num;
			float num5 = num2 * num2;
			float num6 = num3 * num3;
			float num7 = mass * 0.08333333f;
			inertia = num7 * new IndexedVector3(num5 + num6, num4 + num6, num4 + num5);
		}

		public abstract int GetNumVertices();

		public abstract int GetNumEdges();

		public abstract void GetEdge(int i, out IndexedVector3 pa, out IndexedVector3 pb);

		public abstract void GetVertex(int i, out IndexedVector3 vtx);

		public abstract int GetNumPlanes();

		public abstract void GetPlane(out IndexedVector3 planeNormal, out IndexedVector3 planeSupport, int i);

		public abstract bool IsInside(ref IndexedVector3 pt, float tolerance);
	}
}
