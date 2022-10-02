using System;

namespace BulletXNA.BulletCollision
{
	public class GIM_ShapeRetriever : IDisposable
	{
		public class ChildShapeRetriever
		{
			public GIM_ShapeRetriever m_parent;

			public virtual CollisionShape GetChildShape(int index)
			{
				return m_parent.m_gim_shape.GetChildShape(index);
			}

			public virtual void Cleanup()
			{
			}
		}

		public class TriangleShapeRetriever : ChildShapeRetriever
		{
			public override CollisionShape GetChildShape(int index)
			{
				m_parent.m_gim_shape.GetBulletTriangle(index, m_parent.m_trishape);
				return m_parent.m_trishape;
			}

			public override void Cleanup()
			{
			}
		}

		public class TetraShapeRetriever : ChildShapeRetriever
		{
			public override CollisionShape GetChildShape(int index)
			{
				m_parent.m_gim_shape.GetBulletTetrahedron(index, m_parent.m_tetrashape);
				return m_parent.m_tetrashape;
			}
		}

		public GImpactShapeInterface m_gim_shape;

		public TriangleShapeEx m_trishape = new TriangleShapeEx();

		public TetrahedronShapeEx m_tetrashape = new TetrahedronShapeEx();

		public ChildShapeRetriever m_child_retriever = new ChildShapeRetriever();

		public TriangleShapeRetriever m_tri_retriever = new TriangleShapeRetriever();

		public TetraShapeRetriever m_tetra_retriever = new TetraShapeRetriever();

		public ChildShapeRetriever m_current_retriever;

		public GIM_ShapeRetriever()
		{
		}

		public GIM_ShapeRetriever(GImpactShapeInterface gim_shape)
		{
			m_gim_shape = gim_shape;
			if (m_gim_shape.NeedsRetrieveTriangles())
			{
				m_current_retriever = m_tri_retriever;
			}
			else if (m_gim_shape.NeedsRetrieveTetrahedrons())
			{
				m_current_retriever = m_tetra_retriever;
			}
			else
			{
				m_current_retriever = m_child_retriever;
			}
			m_current_retriever.m_parent = this;
		}

		public void Initialize(GImpactShapeInterface gim_shape)
		{
			m_gim_shape = gim_shape;
			if (m_gim_shape.NeedsRetrieveTriangles())
			{
				m_current_retriever = m_tri_retriever;
			}
			else if (m_gim_shape.NeedsRetrieveTetrahedrons())
			{
				m_current_retriever = m_tetra_retriever;
			}
			else
			{
				m_current_retriever = m_child_retriever;
			}
			m_current_retriever.m_parent = this;
		}

		public CollisionShape GetChildShape(int index)
		{
			return m_current_retriever.GetChildShape(index);
		}

		public void Dispose()
		{
			BulletGlobals.GIM_ShapeRetrieverPool.Free(this);
		}
	}
}
