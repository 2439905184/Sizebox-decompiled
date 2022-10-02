using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class TriangleMeshShape : ConcaveShape
	{
		private bool m_inConstructor;

		protected IndexedVector3 m_localAabbMin;

		protected IndexedVector3 m_localAabbMax;

		protected StridingMeshInterface m_meshInterface;

		public TriangleMeshShape(StridingMeshInterface meshInterface)
		{
			m_inConstructor = true;
			m_meshInterface = meshInterface;
			m_shapeType = BroadphaseNativeTypes.TRIANGLE_MESH_SHAPE_PROXYTYPE;
			if (meshInterface.HasPremadeAabb())
			{
				meshInterface.GetPremadeAabb(out m_localAabbMin, out m_localAabbMax);
			}
			else
			{
				RecalcLocalAabb();
			}
			m_inConstructor = false;
		}

		public virtual IndexedVector3 LocalGetSupportingVertex(ref IndexedVector3 vec)
		{
			IndexedMatrix trans = IndexedMatrix.Identity;
			SupportVertexCallback supportVertexCallback = new SupportVertexCallback(ref vec, ref trans);
			IndexedVector3 aabbMax = MathUtil.MAX_VECTOR;
			IndexedVector3 aabbMin = MathUtil.MIN_VECTOR;
			if (m_inConstructor)
			{
				ProcessAllTrianglesCtor(supportVertexCallback, ref aabbMin, ref aabbMax);
			}
			else
			{
				ProcessAllTriangles(supportVertexCallback, ref aabbMin, ref aabbMax);
			}
			IndexedVector3 supportVertexLocal = supportVertexCallback.GetSupportVertexLocal();
			supportVertexCallback.Cleanup();
			return supportVertexLocal;
		}

		public virtual IndexedVector3 LocalGetSupportingVertexWithoutMargin(ref IndexedVector3 vec)
		{
			return LocalGetSupportingVertex(ref vec);
		}

		public void RecalcLocalAabb()
		{
			IndexedVector3 vec = new IndexedVector3(1f, 0f, 0f);
			IndexedVector3 indexedVector = LocalGetSupportingVertex(ref vec);
			m_localAabbMax.X = indexedVector.X + m_collisionMargin;
			vec = new IndexedVector3(-1f, 0f, 0f);
			indexedVector = LocalGetSupportingVertex(ref vec);
			m_localAabbMin.X = indexedVector.X - m_collisionMargin;
			vec = new IndexedVector3(0f, 1f, 0f);
			indexedVector = LocalGetSupportingVertex(ref vec);
			m_localAabbMax.Y = indexedVector.Y + m_collisionMargin;
			vec = new IndexedVector3(0f, -1f, 0f);
			indexedVector = LocalGetSupportingVertex(ref vec);
			m_localAabbMin.Y = indexedVector.Y - m_collisionMargin;
			vec = new IndexedVector3(0f, 0f, 1f);
			indexedVector = LocalGetSupportingVertex(ref vec);
			m_localAabbMax.Z = indexedVector.Z + m_collisionMargin;
			vec = new IndexedVector3(0f, 0f, -1f);
			indexedVector = LocalGetSupportingVertex(ref vec);
			m_localAabbMin.Z = indexedVector.Z - m_collisionMargin;
		}

		public override void GetAabb(ref IndexedMatrix trans, out IndexedVector3 aabbMin, out IndexedVector3 aabbMax)
		{
			IndexedVector3 v = 0.5f * (m_localAabbMax - m_localAabbMin);
			float margin = GetMargin();
			v += new IndexedVector3(margin);
			IndexedVector3 indexedVector = 0.5f * (m_localAabbMax + m_localAabbMin);
			IndexedBasisMatrix indexedBasisMatrix = trans._basis.Absolute();
			IndexedVector3 indexedVector2 = trans * indexedVector;
			IndexedVector3 indexedVector3 = new IndexedVector3(indexedBasisMatrix._el0.Dot(ref v), indexedBasisMatrix._el1.Dot(ref v), indexedBasisMatrix._el2.Dot(ref v));
			aabbMin = indexedVector2 - indexedVector3;
			aabbMax = indexedVector2 + indexedVector3;
		}

		public void ProcessAllTrianglesCtor(ITriangleCallback callback, ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax)
		{
			FilteredCallback filteredCallback = new FilteredCallback(callback, ref aabbMin, ref aabbMax);
			m_meshInterface.InternalProcessAllTriangles(filteredCallback, ref aabbMin, ref aabbMax);
			filteredCallback.Cleanup();
		}

		public override void ProcessAllTriangles(ITriangleCallback callback, ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax)
		{
			FilteredCallback filteredCallback = new FilteredCallback(callback, ref aabbMin, ref aabbMax);
			m_meshInterface.InternalProcessAllTriangles(filteredCallback, ref aabbMin, ref aabbMax);
			filteredCallback.Cleanup();
		}

		public override void CalculateLocalInertia(float mass, out IndexedVector3 inertia)
		{
			inertia = IndexedVector3.Zero;
		}

		public override void SetLocalScaling(ref IndexedVector3 scaling)
		{
			m_meshInterface.SetScaling(ref scaling);
			RecalcLocalAabb();
		}

		public override IndexedVector3 GetLocalScaling()
		{
			return m_meshInterface.GetScaling();
		}

		public StridingMeshInterface GetMeshInterface()
		{
			return m_meshInterface;
		}

		public IndexedVector3 GetLocalAabbMin()
		{
			return m_localAabbMin;
		}

		public IndexedVector3 GetLocalAabbMax()
		{
			return m_localAabbMax;
		}

		public override string GetName()
		{
			return "TRIANGLEMESH";
		}
	}
}
