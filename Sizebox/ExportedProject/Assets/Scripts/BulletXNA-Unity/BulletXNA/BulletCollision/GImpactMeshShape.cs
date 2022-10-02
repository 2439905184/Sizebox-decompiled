using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class GImpactMeshShape : GImpactShapeInterface
	{
		private StridingMeshInterface m_meshInterface;

		protected ObjectArray<GImpactMeshShapePart> m_mesh_parts = new ObjectArray<GImpactMeshShapePart>();

		protected void BuildMeshParts(StridingMeshInterface meshInterface)
		{
			for (int i = 0; i < meshInterface.GetNumSubParts(); i++)
			{
				GImpactMeshShapePart item = new GImpactMeshShapePart(meshInterface, i);
				m_mesh_parts.Add(item);
			}
		}

		protected override void CalcLocalAABB()
		{
			m_localAABB.Invalidate();
			int count = m_mesh_parts.Count;
			while (count-- != 0)
			{
				m_mesh_parts[count].UpdateBound();
				m_localAABB.Merge(m_mesh_parts[count].GetLocalBox());
			}
		}

		public GImpactMeshShape(StridingMeshInterface meshInterface)
		{
			m_meshInterface = meshInterface;
			BuildMeshParts(meshInterface);
		}

		public override void Cleanup()
		{
			base.Cleanup();
			m_mesh_parts.Clear();
		}

		public StridingMeshInterface GetMeshInterface()
		{
			return m_meshInterface;
		}

		public int GetMeshPartCount()
		{
			return m_mesh_parts.Count;
		}

		public GImpactMeshShapePart GetMeshPart(int index)
		{
			return m_mesh_parts[index];
		}

		public override void SetLocalScaling(ref IndexedVector3 scaling)
		{
			localScaling = scaling;
			int count = m_mesh_parts.Count;
			while (count-- != 0)
			{
				GImpactMeshShapePart gImpactMeshShapePart = m_mesh_parts[count];
				gImpactMeshShapePart.SetLocalScaling(ref scaling);
			}
			m_needs_update = true;
		}

		public override void SetMargin(float margin)
		{
			m_collisionMargin = margin;
			int count = m_mesh_parts.Count;
			while (count-- != 0)
			{
				GImpactMeshShapePart gImpactMeshShapePart = m_mesh_parts[count];
				gImpactMeshShapePart.SetMargin(margin);
			}
			m_needs_update = true;
		}

		public override void PostUpdate()
		{
			int count = m_mesh_parts.Count;
			while (count-- != 0)
			{
				GImpactMeshShapePart gImpactMeshShapePart = m_mesh_parts[count];
				gImpactMeshShapePart.PostUpdate();
			}
			m_needs_update = true;
		}

		public override void CalculateLocalInertia(float mass, out IndexedVector3 inertia)
		{
			inertia = IndexedVector3.Zero;
			int meshPartCount = GetMeshPartCount();
			float mass2 = mass / (float)meshPartCount;
			while (meshPartCount-- != 0)
			{
				IndexedVector3 inertia2 = IndexedVector3.Zero;
				GetMeshPart(meshPartCount).CalculateLocalInertia(mass2, ref inertia2);
				inertia += inertia2;
			}
		}

		public override IPrimitiveManagerBase GetPrimitiveManager()
		{
			return null;
		}

		public override int GetNumChildShapes()
		{
			return 0;
		}

		public override bool ChildrenHasTransform()
		{
			return false;
		}

		public override bool NeedsRetrieveTriangles()
		{
			return false;
		}

		public override bool NeedsRetrieveTetrahedrons()
		{
			return false;
		}

		public override void GetBulletTriangle(int prim_index, TriangleShapeEx triangle)
		{
		}

		public override void GetBulletTetrahedron(int prim_index, TetrahedronShapeEx tetrahedron)
		{
		}

		public override void LockChildShapes()
		{
		}

		public override void UnlockChildShapes()
		{
		}

		public override void GetChildAabb(int child_index, ref IndexedMatrix t, out IndexedVector3 aabbMin, out IndexedVector3 aabbMax)
		{
			aabbMin = IndexedVector3.Zero;
			aabbMax = IndexedVector3.Zero;
		}

		public override CollisionShape GetChildShape(int index)
		{
			return null;
		}

		public override IndexedMatrix GetChildTransform(int index)
		{
			return IndexedMatrix.Identity;
		}

		public override void SetChildTransform(int index, ref IndexedMatrix transform)
		{
		}

		public override GIMPACT_SHAPE_TYPE GetGImpactShapeType()
		{
			return GIMPACT_SHAPE_TYPE.CONST_GIMPACT_TRIMESH_SHAPE;
		}

		public override string GetName()
		{
			return "GImpactMesh";
		}

		public void RayTest(IndexedVector3 rayFrom, ref IndexedVector3 rayTo, RayResultCallback resultCallback)
		{
		}

		public override void ProcessAllTriangles(ITriangleCallback callback, ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax)
		{
			int count = m_mesh_parts.Count;
			while (count-- != 0)
			{
				m_mesh_parts[count].ProcessAllTriangles(callback, ref aabbMin, ref aabbMax);
			}
		}
	}
}
