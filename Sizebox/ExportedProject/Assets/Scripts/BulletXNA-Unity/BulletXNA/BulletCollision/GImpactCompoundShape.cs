using System.Collections.Generic;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class GImpactCompoundShape : GImpactShapeInterface
	{
		protected CompoundPrimitiveManager m_primitive_manager;

		protected ObjectArray<IndexedMatrix> m_childTransforms = new ObjectArray<IndexedMatrix>();

		protected List<CollisionShape> m_childShapes = new List<CollisionShape>();

		public GImpactCompoundShape()
			: this(true)
		{
		}

		public GImpactCompoundShape(bool children_has_transform)
		{
			m_primitive_manager.m_compoundShape = this;
			m_box_set.SetPrimitiveManager(m_primitive_manager);
		}

		public override void Cleanup()
		{
		}

		public override bool ChildrenHasTransform()
		{
			if (m_childTransforms.Count == 0)
			{
				return false;
			}
			return true;
		}

		public override IPrimitiveManagerBase GetPrimitiveManager()
		{
			return m_primitive_manager;
		}

		public CompoundPrimitiveManager GetCompoundPrimitiveManager()
		{
			return m_primitive_manager;
		}

		public override int GetNumChildShapes()
		{
			return m_childShapes.Count;
		}

		public void AddChildShape(ref IndexedMatrix localTransform, CollisionShape shape)
		{
			m_childTransforms.Add(localTransform);
			m_childShapes.Add(shape);
		}

		public void AddChildShape(CollisionShape shape)
		{
			m_childShapes.Add(shape);
		}

		public override CollisionShape GetChildShape(int index)
		{
			return m_childShapes[index];
		}

		public override void GetChildAabb(int child_index, ref IndexedMatrix t, out IndexedVector3 aabbMin, out IndexedVector3 aabbMax)
		{
			if (ChildrenHasTransform())
			{
				IndexedMatrix t2 = t * m_childTransforms[child_index];
				m_childShapes[child_index].GetAabb(ref t2, out aabbMin, out aabbMax);
			}
			else
			{
				m_childShapes[child_index].GetAabb(ref t, out aabbMin, out aabbMax);
			}
		}

		public override IndexedMatrix GetChildTransform(int index)
		{
			return m_childTransforms[index];
		}

		public override void SetChildTransform(int index, ref IndexedMatrix transform)
		{
			m_childTransforms[index] = transform;
			PostUpdate();
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

		public virtual void CalculateLocalInertia(float mass, ref IndexedVector3 inertia)
		{
			LockChildShapes();
			inertia = IndexedVector3.Zero;
			int numChildShapes = GetNumChildShapes();
			float mass2 = mass / (float)numChildShapes;
			while (numChildShapes-- != 0)
			{
				IndexedVector3 inertia2;
				m_childShapes[numChildShapes].CalculateLocalInertia(mass2, out inertia2);
				if (ChildrenHasTransform())
				{
					inertia = GImpactMassUtil.GimInertiaAddTransformed(ref inertia, ref inertia2, ref m_childTransforms.GetRawArray()[numChildShapes]);
					continue;
				}
				IndexedMatrix transform = IndexedMatrix.Identity;
				inertia = GImpactMassUtil.GimInertiaAddTransformed(ref inertia, ref inertia2, ref transform);
			}
		}

		public override string GetName()
		{
			return "GImpactCompound";
		}

		public override GIMPACT_SHAPE_TYPE GetGImpactShapeType()
		{
			return GIMPACT_SHAPE_TYPE.CONST_GIMPACT_COMPOUND_SHAPE;
		}
	}
}
