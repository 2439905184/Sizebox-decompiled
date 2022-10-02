using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class TriangleMesh : TriangleIndexVertexArray
	{
		private ObjectArray<IndexedVector3> m_4componentVertices;

		private ObjectArray<float> m_3componentVertices;

		private ObjectArray<int> m_32bitIndices;

		private ObjectArray<short> m_16bitIndices;

		private bool m_use32bitIndices;

		private bool m_use4componentVertices;

		public float m_weldingThreshold;

		public TriangleMesh()
			: this(true, true)
		{
		}

		public TriangleMesh(bool use32bitIndices, bool use4componentVertices)
		{
			m_use32bitIndices = use32bitIndices;
			m_use4componentVertices = use4componentVertices;
			IndexedMesh item = new IndexedMesh();
			m_indexedMeshes.Add(item);
			if (m_use32bitIndices)
			{
				m_32bitIndices = new ObjectArray<int>();
				m_indexedMeshes[0].m_numTriangles = m_32bitIndices.Count / 3;
				m_indexedMeshes[0].m_indexType = PHY_ScalarType.PHY_INTEGER;
				m_indexedMeshes[0].m_triangleIndexStride = 3;
			}
			else
			{
				m_16bitIndices = new ObjectArray<short>();
				m_indexedMeshes[0].m_numTriangles = m_16bitIndices.Count / 3;
				m_indexedMeshes[0].m_triangleIndexBase = null;
				m_indexedMeshes[0].m_indexType = PHY_ScalarType.PHY_SHORT;
				m_indexedMeshes[0].m_triangleIndexStride = 3;
			}
			if (m_use4componentVertices)
			{
				m_4componentVertices = new ObjectArray<IndexedVector3>();
				m_indexedMeshes[0].m_numVertices = m_4componentVertices.Count;
				m_indexedMeshes[0].m_vertexStride = 1;
				m_indexedMeshes[0].m_vertexBase = m_4componentVertices;
			}
			else
			{
				m_3componentVertices = new ObjectArray<float>();
				m_indexedMeshes[0].m_numVertices = m_3componentVertices.Count / 3;
				m_indexedMeshes[0].m_vertexStride = 3;
				m_indexedMeshes[0].m_vertexBase = m_3componentVertices;
			}
		}

		public int FindOrAddVertex(ref IndexedVector3 vertex, bool removeDuplicateVertices)
		{
			if (m_use4componentVertices)
			{
				if (removeDuplicateVertices)
				{
					IndexedVector3[] rawArray = m_4componentVertices.GetRawArray();
					for (int i = 0; i < m_4componentVertices.Count; i++)
					{
						if ((rawArray[i] - vertex).LengthSquared() <= m_weldingThreshold)
						{
							return i;
						}
					}
				}
				m_indexedMeshes[0].m_numVertices++;
				m_4componentVertices.Add(vertex);
				(m_indexedMeshes[0].m_vertexBase as ObjectArray<IndexedVector3>).Add(vertex);
				return m_4componentVertices.Count - 1;
			}
			if (removeDuplicateVertices)
			{
				float[] rawArray2 = m_3componentVertices.GetRawArray();
				for (int j = 0; j < m_3componentVertices.Count; j += 3)
				{
					IndexedVector3 indexedVector = new IndexedVector3(rawArray2[j], rawArray2[j + 1], rawArray2[j + 2]);
					if ((indexedVector - vertex).LengthSquared() <= m_weldingThreshold)
					{
						return j / 3;
					}
				}
			}
			m_3componentVertices.Add(vertex.X);
			m_3componentVertices.Add(vertex.Y);
			m_3componentVertices.Add(vertex.Z);
			m_indexedMeshes[0].m_numVertices++;
			(m_indexedMeshes[0].m_vertexBase as ObjectArray<float>).Add(vertex.X);
			(m_indexedMeshes[0].m_vertexBase as ObjectArray<float>).Add(vertex.Y);
			(m_indexedMeshes[0].m_vertexBase as ObjectArray<float>).Add(vertex.Z);
			return m_3componentVertices.Count / 3 - 1;
		}

		public void AddIndex(int index)
		{
			if (m_use32bitIndices)
			{
				m_32bitIndices.Add(index);
				m_indexedMeshes[0].m_triangleIndexBase = m_32bitIndices;
			}
			else
			{
				m_16bitIndices.Add((short)index);
				m_indexedMeshes[0].m_triangleIndexBase = m_16bitIndices;
			}
		}

		public bool GetUse32bitIndices()
		{
			return m_use32bitIndices;
		}

		public bool GetUse4componentVertices()
		{
			return m_use4componentVertices;
		}

		public int GetNumTriangles()
		{
			if (m_use32bitIndices)
			{
				return m_32bitIndices.Count / 3;
			}
			return m_16bitIndices.Count / 3;
		}

		public override void PreallocateVertices(int numverts)
		{
		}

		public override void PreallocateIndices(int numindices)
		{
		}
	}
}
