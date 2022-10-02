using System.Collections.Generic;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class ShapeHull
	{
		private const int NUM_UNITSPHERE_POINTS = 42;

		public IList<IndexedVector3> m_vertices = new ObjectArray<IndexedVector3>();

		public IList<int> m_indices = new ObjectArray<int>();

		public ConvexShape m_shape;

		private static IndexedVector3[] UnitSpherePoints = new IndexedVector3[62]
		{
			new IndexedVector3(0f, -0f, -1f),
			new IndexedVector3(0.723608f, -0.525725f, -0.447219f),
			new IndexedVector3(-0.276388f, -0.850649f, -0.447219f),
			new IndexedVector3(-0.894426f, -0f, -0.447216f),
			new IndexedVector3(-0.276388f, 0.850649f, -0.44722f),
			new IndexedVector3(0.723608f, 0.525725f, -0.447219f),
			new IndexedVector3(0.276388f, -0.850649f, 0.44722f),
			new IndexedVector3(-0.723608f, -0.525725f, 0.447219f),
			new IndexedVector3(-0.723608f, 0.525725f, 0.447219f),
			new IndexedVector3(0.276388f, 0.850649f, 0.447219f),
			new IndexedVector3(0.894426f, 0f, 0.447216f),
			new IndexedVector3(-0f, 0f, 1f),
			new IndexedVector3(0.425323f, -0.309011f, -0.850654f),
			new IndexedVector3(-0.162456f, -0.499995f, -0.850654f),
			new IndexedVector3(0.262869f, -0.809012f, -0.525738f),
			new IndexedVector3(0.425323f, 0.309011f, -0.850654f),
			new IndexedVector3(0.850648f, -0f, -0.525736f),
			new IndexedVector3(-0.52573f, -0f, -0.850652f),
			new IndexedVector3(-0.68819f, -0.499997f, -0.525736f),
			new IndexedVector3(-0.162456f, 0.499995f, -0.850654f),
			new IndexedVector3(-0.68819f, 0.499997f, -0.525736f),
			new IndexedVector3(0.262869f, 0.809012f, -0.525738f),
			new IndexedVector3(0.951058f, 0.309013f, 0f),
			new IndexedVector3(0.951058f, -0.309013f, 0f),
			new IndexedVector3(0.587786f, -0.809017f, 0f),
			new IndexedVector3(0f, -1f, 0f),
			new IndexedVector3(-0.587786f, -0.809017f, 0f),
			new IndexedVector3(-0.951058f, -0.309013f, -0f),
			new IndexedVector3(-0.951058f, 0.309013f, -0f),
			new IndexedVector3(-0.587786f, 0.809017f, -0f),
			new IndexedVector3(-0f, 1f, -0f),
			new IndexedVector3(0.587786f, 0.809017f, -0f),
			new IndexedVector3(0.68819f, -0.499997f, 0.525736f),
			new IndexedVector3(-0.262869f, -0.809012f, 0.525738f),
			new IndexedVector3(-0.850648f, 0f, 0.525736f),
			new IndexedVector3(-0.262869f, 0.809012f, 0.525738f),
			new IndexedVector3(0.68819f, 0.499997f, 0.525736f),
			new IndexedVector3(0.52573f, 0f, 0.850652f),
			new IndexedVector3(0.162456f, -0.499995f, 0.850654f),
			new IndexedVector3(-0.425323f, -0.309011f, 0.850654f),
			new IndexedVector3(-0.425323f, 0.309011f, 0.850654f),
			new IndexedVector3(0.162456f, 0.499995f, 0.850654f),
			IndexedVector3.Zero,
			IndexedVector3.Zero,
			IndexedVector3.Zero,
			IndexedVector3.Zero,
			IndexedVector3.Zero,
			IndexedVector3.Zero,
			IndexedVector3.Zero,
			IndexedVector3.Zero,
			IndexedVector3.Zero,
			IndexedVector3.Zero,
			IndexedVector3.Zero,
			IndexedVector3.Zero,
			IndexedVector3.Zero,
			IndexedVector3.Zero,
			IndexedVector3.Zero,
			IndexedVector3.Zero,
			IndexedVector3.Zero,
			IndexedVector3.Zero,
			IndexedVector3.Zero,
			IndexedVector3.Zero
		};

		public ShapeHull(ConvexShape convexShape)
		{
			m_shape = convexShape;
		}

		public bool BuildHull(float margin)
		{
			int num = 42;
			int numPreferredPenetrationDirections = m_shape.GetNumPreferredPenetrationDirections();
			if (numPreferredPenetrationDirections != 0)
			{
				for (int i = 0; i < numPreferredPenetrationDirections; i++)
				{
					IndexedVector3 penetrationVector;
					m_shape.GetPreferredPenetrationDirection(i, out penetrationVector);
					UnitSpherePoints[num] = penetrationVector;
					num++;
				}
			}
			IndexedVector3[] array = new IndexedVector3[62];
			for (int j = 0; j < num; j++)
			{
				array[j] = m_shape.LocalGetSupportingVertex(ref UnitSpherePoints[j]);
			}
			HullDesc hullDesc = new HullDesc();
			hullDesc.mFlags = HullFlag.QF_TRIANGLES;
			hullDesc.mVcount = num;
			for (int k = 0; k < num; k++)
			{
				hullDesc.mVertices.Add(array[k]);
			}
			HullLibrary hullLibrary = new HullLibrary();
			HullResult hullResult = new HullResult();
			if (hullLibrary.CreateConvexHull(hullDesc, hullResult) == HullError.QE_FAIL)
			{
				return false;
			}
			for (int l = 0; l < hullResult.mNumOutputVertices; l++)
			{
				m_vertices[l] = hullResult.m_OutputVertices[l];
			}
			int mNumIndices = hullResult.mNumIndices;
			for (int m = 0; m < mNumIndices; m++)
			{
				m_indices[m] = hullResult.m_Indices[m];
			}
			hullLibrary.ReleaseResult(hullResult);
			return true;
		}

		public int NumTriangles()
		{
			return m_indices.Count / 3;
		}

		public int NumVertices()
		{
			return m_vertices.Count;
		}

		public int NumIndices()
		{
			return m_indices.Count;
		}
	}
}
