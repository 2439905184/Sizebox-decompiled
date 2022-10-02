using System.Collections.Generic;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class CompoundShape : CollisionShape
	{
		private ObjectArray<CompoundShapeChild> m_children;

		private IndexedVector3 m_localAabbMin;

		private IndexedVector3 m_localAabbMax;

		private Dbvt m_dynamicAabbTree;

		private int m_updateRevision;

		private float m_collisionMargin;

		protected IndexedVector3 m_localScaling;

		public CompoundShape()
			: this(true)
		{
		}

		public CompoundShape(bool enableDynamicAabbTree)
		{
			m_children = new ObjectArray<CompoundShapeChild>();
			m_localAabbMax = new IndexedVector3(float.MinValue);
			m_localAabbMin = new IndexedVector3(float.MaxValue);
			m_collisionMargin = 0f;
			m_localScaling = new IndexedVector3(1f);
			m_dynamicAabbTree = null;
			m_updateRevision = 1;
			m_shapeType = BroadphaseNativeTypes.COMPOUND_SHAPE_PROXYTYPE;
			if (enableDynamicAabbTree)
			{
				m_dynamicAabbTree = new Dbvt();
			}
		}

		public override void Cleanup()
		{
			base.Cleanup();
			if (m_dynamicAabbTree != null)
			{
				m_dynamicAabbTree.Cleanup();
				m_dynamicAabbTree = null;
			}
		}

		public void AddChildShape(ref IndexedMatrix localTransform, CollisionShape shape)
		{
			m_updateRevision++;
			CompoundShapeChild compoundShapeChild = new CompoundShapeChild();
			compoundShapeChild.m_transform = localTransform;
			compoundShapeChild.m_childShape = shape;
			compoundShapeChild.m_childShapeType = shape.GetShapeType();
			compoundShapeChild.m_childMargin = shape.GetMargin();
			IndexedVector3 aabbMin;
			IndexedVector3 aabbMax;
			shape.GetAabb(ref localTransform, out aabbMin, out aabbMax);
			MathUtil.VectorMin(ref aabbMin, ref m_localAabbMin);
			MathUtil.VectorMax(ref aabbMax, ref m_localAabbMax);
			if (m_dynamicAabbTree != null)
			{
				DbvtAabbMm box = DbvtAabbMm.FromMM(ref aabbMin, ref aabbMax);
				int count = m_children.Count;
				compoundShapeChild.m_treeNode = m_dynamicAabbTree.Insert(ref box, (object)count);
			}
			m_children.Add(compoundShapeChild);
		}

		public virtual void RemoveChildShape(CollisionShape shape)
		{
			m_updateRevision++;
			for (int num = m_children.Count - 1; num >= 0; num--)
			{
				if (m_children[num].m_childShape == shape)
				{
					RemoveChildShapeByIndex(num);
				}
			}
			RecalculateLocalAabb();
		}

		public void RemoveChildShapeByIndex(int childShapeIndex)
		{
			m_updateRevision++;
			if (m_dynamicAabbTree != null)
			{
				m_dynamicAabbTree.Remove(m_children[childShapeIndex].m_treeNode);
			}
			m_children.RemoveAtQuick(childShapeIndex);
			if (m_dynamicAabbTree != null && m_children.Count > childShapeIndex)
			{
				m_children[childShapeIndex].m_treeNode.dataAsInt = childShapeIndex;
			}
		}

		public int GetNumChildShapes()
		{
			return m_children.Count;
		}

		public CollisionShape GetChildShape(int index)
		{
			return m_children[index].m_childShape;
		}

		public IndexedMatrix GetChildTransform(int index)
		{
			return m_children[index].m_transform;
		}

		public void UpdateChildTransform(int childIndex, ref IndexedMatrix newChildTransform)
		{
			UpdateChildTransform(childIndex, ref newChildTransform, true);
		}

		public void UpdateChildTransform(int childIndex, ref IndexedMatrix newChildTransform, bool shouldRecalculateLocalAabb)
		{
			m_children[childIndex].m_transform = newChildTransform;
			if (m_dynamicAabbTree != null)
			{
				IndexedVector3 aabbMin;
				IndexedVector3 aabbMax;
				m_children[childIndex].m_childShape.GetAabb(ref newChildTransform, out aabbMin, out aabbMax);
				DbvtAabbMm volume = DbvtAabbMm.FromMM(ref aabbMin, ref aabbMax);
				m_dynamicAabbTree.Update(m_children[childIndex].m_treeNode, ref volume);
			}
			if (shouldRecalculateLocalAabb)
			{
				RecalculateLocalAabb();
			}
		}

		public override void SetLocalScaling(ref IndexedVector3 scaling)
		{
			for (int i = 0; i < m_children.Count; i++)
			{
				IndexedMatrix newChildTransform = GetChildTransform(i);
				IndexedVector3 localScaling = m_children[i].m_childShape.GetLocalScaling();
				localScaling = localScaling * scaling / m_localScaling;
				m_children[i].m_childShape.SetLocalScaling(ref localScaling);
				newChildTransform._origin *= scaling;
				UpdateChildTransform(i, ref newChildTransform, false);
			}
			m_localScaling = scaling;
			RecalculateLocalAabb();
		}

		public void CreateAabbTreeFromChildren()
		{
			if (m_dynamicAabbTree == null)
			{
				m_dynamicAabbTree = new Dbvt();
				for (int i = 0; i < m_children.Count; i++)
				{
					CompoundShapeChild compoundShapeChild = m_children[i];
					IndexedVector3 aabbMin;
					IndexedVector3 aabbMax;
					compoundShapeChild.m_childShape.GetAabb(ref compoundShapeChild.m_transform, out aabbMin, out aabbMax);
					DbvtAabbMm box = DbvtAabbMm.FromMM(ref aabbMin, ref aabbMax);
					compoundShapeChild.m_treeNode = m_dynamicAabbTree.Insert(ref box, (object)i);
				}
			}
		}

		public IList<CompoundShapeChild> GetChildList()
		{
			return m_children;
		}

		public override void GetAabb(ref IndexedMatrix trans, out IndexedVector3 aabbMin, out IndexedVector3 aabbMax)
		{
			IndexedVector3 v = 0.5f * (m_localAabbMax - m_localAabbMin);
			IndexedVector3 indexedVector = 0.5f * (m_localAabbMax + m_localAabbMin);
			if (m_children.Count == 0)
			{
				v = IndexedVector3.Zero;
				indexedVector = IndexedVector3.Zero;
			}
			float margin = GetMargin();
			v += new IndexedVector3(margin);
			IndexedBasisMatrix indexedBasisMatrix = trans._basis.Absolute();
			IndexedVector3 indexedVector2 = trans * indexedVector;
			IndexedVector3 indexedVector3 = new IndexedVector3(indexedBasisMatrix._el0.Dot(ref v), indexedBasisMatrix._el1.Dot(ref v), indexedBasisMatrix._el2.Dot(ref v));
			aabbMin = indexedVector2 - indexedVector3;
			aabbMax = indexedVector2 + indexedVector3;
		}

		public virtual void RecalculateLocalAabb()
		{
			m_localAabbMin = new IndexedVector3(float.MaxValue);
			m_localAabbMax = new IndexedVector3(float.MinValue);
			for (int i = 0; i < m_children.Count; i++)
			{
				IndexedMatrix t = m_children[i].m_transform;
				IndexedVector3 aabbMin;
				IndexedVector3 aabbMax;
				m_children[i].m_childShape.GetAabb(ref t, out aabbMin, out aabbMax);
				MathUtil.VectorMin(ref aabbMin, ref m_localAabbMin);
				MathUtil.VectorMax(ref aabbMax, ref m_localAabbMax);
			}
		}

		public override IndexedVector3 GetLocalScaling()
		{
			return m_localScaling;
		}

		public override void CalculateLocalInertia(float mass, out IndexedVector3 inertia)
		{
			IndexedMatrix t = IndexedMatrix.Identity;
			IndexedVector3 aabbMin;
			IndexedVector3 aabbMax;
			GetAabb(ref t, out aabbMin, out aabbMax);
			IndexedVector3 indexedVector = (aabbMax - aabbMin) * 0.5f;
			float num = 2f * indexedVector.X;
			float num2 = 2f * indexedVector.Y;
			float num3 = 2f * indexedVector.Z;
			inertia = new IndexedVector3(mass / 12f * (num2 * num2 + num3 * num3), mass / 12f * (num * num + num3 * num3), mass / 12f * (num * num + num2 * num2));
		}

		public override void SetMargin(float margin)
		{
			m_collisionMargin = margin;
		}

		public override float GetMargin()
		{
			return m_collisionMargin;
		}

		public override string GetName()
		{
			return "Compound";
		}

		public Dbvt GetDynamicAabbTree()
		{
			return m_dynamicAabbTree;
		}

		public void CalculatePrincipalAxisTransform(IList<float> masses, ref IndexedMatrix principal, out IndexedVector3 inertia)
		{
			int count = m_children.Count;
			float num = 0f;
			IndexedVector3 zero = IndexedVector3.Zero;
			for (int i = 0; i < count; i++)
			{
				zero += m_children[i].m_transform._origin * masses[i];
				num += masses[i];
			}
			zero /= num;
			principal._origin = zero;
			IndexedBasisMatrix indexedBasisMatrix = default(IndexedBasisMatrix);
			for (int j = 0; j < count; j++)
			{
				IndexedVector3 inertia2;
				m_children[j].m_childShape.CalculateLocalInertia(masses[j], out inertia2);
				IndexedMatrix transform = m_children[j].m_transform;
				IndexedVector3 indexedVector = transform._origin - zero;
				IndexedBasisMatrix indexedBasisMatrix2 = transform._basis.Transpose();
				indexedBasisMatrix2._el0 *= inertia2.X;
				indexedBasisMatrix2._el1 *= inertia2.Y;
				indexedBasisMatrix2._el2 *= inertia2.Z;
				indexedBasisMatrix2 = transform._basis * indexedBasisMatrix2;
				indexedBasisMatrix._el0 += indexedBasisMatrix2._el0;
				indexedBasisMatrix._el1 += indexedBasisMatrix2._el1;
				indexedBasisMatrix._el2 += indexedBasisMatrix2._el2;
				float num2 = indexedVector.LengthSquared();
				indexedBasisMatrix2._el0 = new IndexedVector3(num2, 0f, 0f);
				indexedBasisMatrix2._el1 = new IndexedVector3(0f, num2, 0f);
				indexedBasisMatrix2._el2 = new IndexedVector3(0f, 0f, num2);
				indexedBasisMatrix2._el0 += indexedVector * (0f - indexedVector.X);
				indexedBasisMatrix2._el1 += indexedVector * (0f - indexedVector.Y);
				indexedBasisMatrix2._el2 += indexedVector * (0f - indexedVector.Z);
				indexedBasisMatrix._el0 += masses[j] * indexedBasisMatrix2._el0;
				indexedBasisMatrix._el1 += masses[j] * indexedBasisMatrix2._el1;
				indexedBasisMatrix._el2 += masses[j] * indexedBasisMatrix2._el2;
			}
			indexedBasisMatrix.Diagonalize(out principal, 1E-05f, 20);
			inertia = new IndexedVector3(indexedBasisMatrix._el0.X, indexedBasisMatrix._el1.Y, indexedBasisMatrix._el2.Z);
		}

		public int GetUpdateRevision()
		{
			return m_updateRevision;
		}
	}
}
