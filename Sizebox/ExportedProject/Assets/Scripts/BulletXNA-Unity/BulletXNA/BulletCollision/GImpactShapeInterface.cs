using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public abstract class GImpactShapeInterface : ConcaveShape
	{
		protected AABB m_localAABB;

		protected bool m_needs_update;

		protected IndexedVector3 localScaling;

		protected GImpactQuantizedBvh m_box_set = new GImpactQuantizedBvh();

		protected virtual void CalcLocalAABB()
		{
			LockChildShapes();
			if (m_box_set.GetNodeCount() == 0)
			{
				m_box_set.BuildSet();
			}
			else
			{
				m_box_set.Update();
			}
			UnlockChildShapes();
			m_localAABB = m_box_set.GetGlobalBox();
		}

		public GImpactShapeInterface()
		{
			m_shapeType = BroadphaseNativeTypes.GIMPACT_SHAPE_PROXYTYPE;
			m_localAABB.Invalidate();
			m_needs_update = true;
			localScaling = IndexedVector3.One;
		}

		public void UpdateBound()
		{
			if (m_needs_update)
			{
				CalcLocalAABB();
				m_needs_update = false;
			}
		}

		public override void GetAabb(ref IndexedMatrix t, out IndexedVector3 aabbMin, out IndexedVector3 aabbMax)
		{
			AABB localAABB = m_localAABB;
			localAABB.ApplyTransform(ref t);
			aabbMin = localAABB.m_min;
			aabbMax = localAABB.m_max;
		}

		public virtual void PostUpdate()
		{
			m_needs_update = true;
		}

		public AABB GetLocalBox()
		{
			return m_localAABB;
		}

		public override void SetLocalScaling(ref IndexedVector3 scaling)
		{
			localScaling = scaling;
			PostUpdate();
		}

		public override IndexedVector3 GetLocalScaling()
		{
			return localScaling;
		}

		public override void SetMargin(float margin)
		{
			m_collisionMargin = margin;
			int numChildShapes = GetNumChildShapes();
			while (numChildShapes-- != 0)
			{
				CollisionShape childShape = GetChildShape(numChildShapes);
				childShape.SetMargin(margin);
			}
			m_needs_update = true;
		}

		public abstract GIMPACT_SHAPE_TYPE GetGImpactShapeType();

		public GImpactQuantizedBvh GetBoxSet()
		{
			return m_box_set;
		}

		public bool HasBoxSet()
		{
			if (m_box_set.GetNodeCount() == 0)
			{
				return false;
			}
			return true;
		}

		public abstract IPrimitiveManagerBase GetPrimitiveManager();

		public abstract int GetNumChildShapes();

		public abstract bool ChildrenHasTransform();

		public abstract bool NeedsRetrieveTriangles();

		public abstract bool NeedsRetrieveTetrahedrons();

		public abstract void GetBulletTriangle(int prim_index, TriangleShapeEx triangle);

		public abstract void GetBulletTetrahedron(int prim_index, TetrahedronShapeEx tetrahedron);

		public virtual void LockChildShapes()
		{
		}

		public virtual void UnlockChildShapes()
		{
		}

		public void GetPrimitiveTriangle(int index, PrimitiveTriangle triangle)
		{
			GetPrimitiveManager().GetPrimitiveTriangle(index, triangle);
		}

		public virtual void GetChildAabb(int child_index, ref IndexedMatrix t, out IndexedVector3 aabbMin, out IndexedVector3 aabbMax)
		{
			AABB primbox;
			GetPrimitiveManager().GetPrimitiveBox(child_index, out primbox);
			primbox.ApplyTransform(ref t);
			aabbMin = primbox.m_min;
			aabbMax = primbox.m_max;
		}

		public abstract CollisionShape GetChildShape(int index);

		public abstract IndexedMatrix GetChildTransform(int index);

		public abstract void SetChildTransform(int index, ref IndexedMatrix transform);

		public virtual void RayTest(ref IndexedVector3 rayFrom, ref IndexedVector3 rayTo, RayResultCallback resultCallback)
		{
		}

		public override void ProcessAllTriangles(ITriangleCallback callback, ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax)
		{
		}
	}
}
