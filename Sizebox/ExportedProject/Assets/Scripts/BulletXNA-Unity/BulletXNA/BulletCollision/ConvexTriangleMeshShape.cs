using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class ConvexTriangleMeshShape : PolyhedralConvexAabbCachingShape
	{
		private StridingMeshInterface m_stridingMesh;

		public ConvexTriangleMeshShape(StridingMeshInterface meshInterface, bool calcAabb)
		{
			m_shapeType = BroadphaseNativeTypes.CONVEX_TRIANGLEMESH_SHAPE_PROXYTYPE;
			m_stridingMesh = meshInterface;
			if (calcAabb)
			{
				RecalcLocalAabb();
			}
		}

		public override void Cleanup()
		{
			base.Cleanup();
		}

		public StridingMeshInterface GetMeshInterface()
		{
			return m_stridingMesh;
		}

		public override IndexedVector3 LocalGetSupportingVertex(ref IndexedVector3 vec)
		{
			IndexedVector3 result = LocalGetSupportingVertexWithoutMargin(ref vec);
			if (GetMargin() != 0f)
			{
				IndexedVector3 indexedVector = vec;
				if (indexedVector.LengthSquared() < 1.4210855E-14f)
				{
					indexedVector = new IndexedVector3(-1f);
				}
				indexedVector.Normalize();
				result += GetMargin() * indexedVector;
			}
			return result;
		}

		public override IndexedVector3 LocalGetSupportingVertexWithoutMargin(ref IndexedVector3 vec0)
		{
			IndexedVector3 zero = IndexedVector3.Zero;
			IndexedVector3 supportVecLocal = vec0;
			float num = supportVecLocal.LengthSquared();
			if (num < 0.0001f)
			{
				supportVecLocal = new IndexedVector3(1f, 0f, 0f);
			}
			else
			{
				float num2 = 1f / (float)Math.Sqrt(num);
				supportVecLocal *= num2;
			}
			LocalSupportVertexCallback localSupportVertexCallback = new LocalSupportVertexCallback(ref supportVecLocal);
			IndexedVector3 aabbMax = new IndexedVector3(float.MaxValue);
			IndexedVector3 aabbMin = -aabbMax;
			m_stridingMesh.InternalProcessAllTriangles(localSupportVertexCallback, ref aabbMin, ref aabbMax);
			return localSupportVertexCallback.GetSupportVertexLocal();
		}

		public override void BatchedUnitVectorGetSupportingVertexWithoutMargin(IndexedVector3[] vectors, IndexedVector4[] supportVerticesOut, int numVectors)
		{
			for (int i = 0; i < numVectors; i++)
			{
				IndexedVector3 supportVecLocal = vectors[i];
				LocalSupportVertexCallback localSupportVertexCallback = new LocalSupportVertexCallback(ref supportVecLocal);
				IndexedVector3 aabbMax = MathUtil.MAX_VECTOR;
				IndexedVector3 aabbMin = MathUtil.MIN_VECTOR;
				m_stridingMesh.InternalProcessAllTriangles(localSupportVertexCallback, ref aabbMin, ref aabbMax);
				supportVerticesOut[i] = new IndexedVector4(localSupportVertexCallback.GetSupportVertexLocal(), 0f);
			}
		}

		public override string GetName()
		{
			return "ConvexTrimesh";
		}

		public override int GetNumVertices()
		{
			return 0;
		}

		public override int GetNumEdges()
		{
			return 0;
		}

		public override void GetEdge(int i, out IndexedVector3 pa, out IndexedVector3 pb)
		{
			pa = IndexedVector3.Zero;
			pb = IndexedVector3.Zero;
		}

		public override void GetVertex(int i, out IndexedVector3 vtx)
		{
			vtx = IndexedVector3.Zero;
		}

		public override int GetNumPlanes()
		{
			return 0;
		}

		public override void GetPlane(out IndexedVector3 planeNormal, out IndexedVector3 planeSupport, int i)
		{
			planeNormal = IndexedVector3.Zero;
			planeSupport = IndexedVector3.Zero;
		}

		public override bool IsInside(ref IndexedVector3 pt, float tolerance)
		{
			return false;
		}

		public override void SetLocalScaling(ref IndexedVector3 scaling)
		{
			m_stridingMesh.SetScaling(ref scaling);
			RecalcLocalAabb();
		}

		public override IndexedVector3 GetLocalScaling()
		{
			return m_stridingMesh.GetScaling();
		}

		public void CalculatePrincipalAxisTransform(ref IndexedMatrix principal, out IndexedVector3 inertia, float volume)
		{
			CenterCallback centerCallback = new CenterCallback();
			IndexedVector3 aabbMax = MathUtil.MAX_VECTOR;
			IndexedVector3 aabbMin = MathUtil.MIN_VECTOR;
			m_stridingMesh.InternalProcessAllTriangles(centerCallback, ref aabbMin, ref aabbMax);
			IndexedVector3 center = (principal._origin = centerCallback.GetCenter());
			volume = centerCallback.GetVolume();
			InertiaCallback inertiaCallback = new InertiaCallback(ref center);
			m_stridingMesh.InternalProcessAllTriangles(inertiaCallback, ref aabbMax, ref aabbMax);
			IndexedBasisMatrix inertia2 = inertiaCallback.GetInertia();
			inertia2.Diagonalize(out principal, 1E-05f, 20);
			inertia = new IndexedVector3(inertia2[0, 0], inertia2[1, 1], inertia2[2, 2]);
			inertia /= volume;
		}
	}
}
