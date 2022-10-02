using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class TriangleIndexVertexArray : StridingMeshInterface
	{
		protected ObjectArray<IndexedMesh> m_indexedMeshes = new ObjectArray<IndexedMesh>();

		protected bool m_hasAabb;

		protected IndexedVector3 m_aabbMin;

		protected IndexedVector3 m_aabbMax;

		public TriangleIndexVertexArray()
		{
			m_hasAabb = false;
		}

		public TriangleIndexVertexArray(int numTriangles, ObjectArray<int> triangleIndexBase, int triangleIndexStride, int numVertices, object vertexBase, int vertexStride)
		{
			IndexedMesh indexedMesh = new IndexedMesh
			{
				m_numTriangles = numTriangles,
				m_triangleIndexBase = triangleIndexBase,
				m_triangleIndexStride = 3,
				m_numVertices = numVertices,
				m_vertexBase = vertexBase
			};
			if (vertexBase is ObjectArray<IndexedVector3>)
			{
				indexedMesh.m_vertexStride = 1;
			}
			else if (vertexBase is ObjectArray<float>)
			{
				indexedMesh.m_vertexStride = 3;
			}
			AddIndexedMesh(indexedMesh, PHY_ScalarType.PHY_INTEGER);
		}

		public override void Cleanup()
		{
		}

		public void AddIndexedMesh(IndexedMesh mesh, PHY_ScalarType indexType)
		{
			m_indexedMeshes.Add(mesh);
			m_indexedMeshes[m_indexedMeshes.Count - 1].m_indexType = indexType;
		}

		public override void GetLockedVertexIndexBase(out object vertexbase, out int numverts, out PHY_ScalarType type, out int vertexStride, out object indexbase, out int indexstride, out int numfaces, out PHY_ScalarType indicestype, int subpart)
		{
			IndexedMesh indexedMesh = m_indexedMeshes[subpart];
			numverts = indexedMesh.m_numVertices;
			vertexbase = indexedMesh.m_vertexBase;
			type = indexedMesh.m_vertexType;
			vertexStride = indexedMesh.m_vertexStride;
			numfaces = indexedMesh.m_numTriangles;
			indexbase = indexedMesh.m_triangleIndexBase;
			indexstride = indexedMesh.m_triangleIndexStride;
			indicestype = indexedMesh.m_indexType;
		}

		public override void GetLockedReadOnlyVertexIndexBase(out object vertexbase, out int numverts, out PHY_ScalarType type, out int vertexStride, out object indexbase, out int indexstride, out int numfaces, out PHY_ScalarType indicestype, int subpart)
		{
			IndexedMesh indexedMesh = m_indexedMeshes[subpart];
			numverts = indexedMesh.m_numVertices;
			vertexbase = indexedMesh.m_vertexBase;
			type = indexedMesh.m_vertexType;
			vertexStride = indexedMesh.m_vertexStride;
			numfaces = indexedMesh.m_numTriangles;
			indexbase = indexedMesh.m_triangleIndexBase;
			indexstride = indexedMesh.m_triangleIndexStride;
			indicestype = indexedMesh.m_indexType;
		}

		public override void UnLockVertexBase(int subpart)
		{
		}

		public override void UnLockReadOnlyVertexBase(int subpart)
		{
		}

		public override int GetNumSubParts()
		{
			return m_indexedMeshes.Count;
		}

		public ObjectArray<IndexedMesh> getIndexedMeshArray()
		{
			return m_indexedMeshes;
		}

		public override void PreallocateVertices(int numverts)
		{
		}

		public override void PreallocateIndices(int numindices)
		{
		}

		public override bool HasPremadeAabb()
		{
			return m_hasAabb;
		}

		public override void SetPremadeAabb(ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax)
		{
			m_aabbMin = aabbMin;
			m_aabbMax = aabbMax;
			m_hasAabb = true;
		}

		public override void GetPremadeAabb(out IndexedVector3 aabbMin, out IndexedVector3 aabbMax)
		{
			aabbMin = m_aabbMin;
			aabbMax = m_aabbMax;
		}
	}
}
