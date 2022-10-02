using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class MyNodeOverlapCallback : INodeOverlapCallback, IDisposable
	{
		public StridingMeshInterface m_meshInterface;

		public ITriangleCallback m_callback;

		private IndexedVector3[] m_triangle = new IndexedVector3[3];

		public MyNodeOverlapCallback()
		{
		}

		public MyNodeOverlapCallback(ITriangleCallback callback, StridingMeshInterface meshInterface)
		{
			m_meshInterface = meshInterface;
			m_callback = callback;
		}

		public void Initialize(ITriangleCallback callback, StridingMeshInterface meshInterface)
		{
			m_meshInterface = meshInterface;
			m_callback = callback;
		}

		public virtual void ProcessNode(int nodeSubPart, int nodeTriangleIndex)
		{
			object vertexbase;
			int numverts;
			PHY_ScalarType type;
			int stride;
			object indexbase;
			int numfaces;
			PHY_ScalarType indicestype;
			int indexstride;
			m_meshInterface.GetLockedReadOnlyVertexIndexBase(out vertexbase, out numverts, out type, out stride, out indexbase, out indexstride, out numfaces, out indicestype, nodeSubPart);
			indexstride = 3;
			int num = nodeTriangleIndex * indexstride;
			IndexedVector3 scaling = m_meshInterface.GetScaling();
			int[] rawArray = ((ObjectArray<int>)indexbase).GetRawArray();
			if (vertexbase is ObjectArray<IndexedVector3>)
			{
				IndexedVector3[] rawArray2 = ((ObjectArray<IndexedVector3>)vertexbase).GetRawArray();
				for (int num2 = 2; num2 >= 0; num2--)
				{
					m_triangle[num2] = rawArray2[rawArray[num + num2]];
					m_triangle[num2] *= scaling;
				}
			}
			else if (vertexbase is ObjectArray<float>)
			{
				float[] rawArray3 = ((ObjectArray<float>)vertexbase).GetRawArray();
				for (int num3 = 2; num3 >= 0; num3--)
				{
					int num4 = rawArray[num + num3] * 3;
					m_triangle[num3] = new IndexedVector3(rawArray3[num4] * scaling.X, rawArray3[num4 + 1] * scaling.Y, rawArray3[num4 + 2] * scaling.Z);
				}
			}
			m_callback.ProcessTriangle(m_triangle, nodeSubPart, nodeTriangleIndex);
			m_meshInterface.UnLockReadOnlyVertexBase(nodeSubPart);
		}

		public virtual void Cleanup()
		{
		}

		public void Dispose()
		{
			Cleanup();
			BulletGlobals.MyNodeOverlapCallbackPool.Free(this);
		}
	}
}
