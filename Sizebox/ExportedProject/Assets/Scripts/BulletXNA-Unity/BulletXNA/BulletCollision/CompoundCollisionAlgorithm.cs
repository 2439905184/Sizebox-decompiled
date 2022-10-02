using System.Collections.Generic;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class CompoundCollisionAlgorithm : ActivatingCollisionAlgorithm
	{
		private IList<CollisionAlgorithm> m_childCollisionAlgorithms = new List<CollisionAlgorithm>();

		private PersistentManifoldArray m_manifoldArray = new PersistentManifoldArray();

		private bool m_isSwapped;

		private PersistentManifold m_sharedManifold;

		private bool m_ownsManifold;

		private int m_compoundShapeRevision;

		public CompoundCollisionAlgorithm()
		{
		}

		public CompoundCollisionAlgorithm(CollisionAlgorithmConstructionInfo ci, CollisionObject body0, CollisionObject body1, bool isSwapped)
			: base(ci, body0, body1)
		{
			m_isSwapped = isSwapped;
			m_sharedManifold = ci.GetManifold();
			m_ownsManifold = false;
			CollisionObject collisionObject = (m_isSwapped ? body1 : body0);
			CompoundShape compoundShape = (CompoundShape)collisionObject.GetCollisionShape();
			m_compoundShapeRevision = compoundShape.GetUpdateRevision();
			PreallocateChildAlgorithms(body0, body1);
		}

		public virtual void Initialize(CollisionAlgorithmConstructionInfo ci, CollisionObject body0, CollisionObject body1, bool isSwapped)
		{
			base.Initialize(ci, body0, body1);
			m_isSwapped = isSwapped;
			m_sharedManifold = ci.GetManifold();
			m_ownsManifold = false;
			CollisionObject collisionObject = (m_isSwapped ? body1 : body0);
			CompoundShape compoundShape = (CompoundShape)collisionObject.GetCollisionShape();
			m_compoundShapeRevision = compoundShape.GetUpdateRevision();
			PreallocateChildAlgorithms(body0, body1);
		}

		public override void Cleanup()
		{
			RemoveChildAlgorithms();
			m_compoundShapeRevision = 0;
			BulletGlobals.CompoundCollisionAlgorithmPool.Free(this);
		}

		private void RemoveChildAlgorithms()
		{
			int count = m_childCollisionAlgorithms.Count;
			for (int i = 0; i < count; i++)
			{
				if (m_childCollisionAlgorithms[i] != null)
				{
					m_dispatcher.FreeCollisionAlgorithm(m_childCollisionAlgorithms[i]);
					m_childCollisionAlgorithms[i] = null;
				}
			}
			m_childCollisionAlgorithms.Clear();
		}

		private void PreallocateChildAlgorithms(CollisionObject body0, CollisionObject body1)
		{
			CollisionObject collisionObject = (m_isSwapped ? body1 : body0);
			CollisionObject body2 = (m_isSwapped ? body0 : body1);
			CompoundShape compoundShape = (CompoundShape)collisionObject.GetCollisionShape();
			int numChildShapes = compoundShape.GetNumChildShapes();
			m_childCollisionAlgorithms.Clear();
			for (int i = 0; i < numChildShapes; i++)
			{
				if (compoundShape.GetDynamicAabbTree() != null)
				{
					m_childCollisionAlgorithms.Add(null);
					continue;
				}
				CollisionShape collisionShape = collisionObject.GetCollisionShape();
				CollisionShape childShape = compoundShape.GetChildShape(i);
				collisionObject.InternalSetTemporaryCollisionShape(childShape);
				CollisionAlgorithm item = m_dispatcher.FindAlgorithm(collisionObject, body2, m_sharedManifold);
				m_childCollisionAlgorithms.Add(item);
				collisionObject.InternalSetTemporaryCollisionShape(collisionShape);
			}
		}

		public override void ProcessCollision(CollisionObject body0, CollisionObject body1, DispatcherInfo dispatchInfo, ManifoldResult resultOut)
		{
			CollisionObject collisionObject = (m_isSwapped ? body1 : body0);
			CollisionObject collisionObject2 = (m_isSwapped ? body0 : body1);
			CompoundShape compoundShape = (CompoundShape)collisionObject.GetCollisionShape();
			if (compoundShape.GetUpdateRevision() != m_compoundShapeRevision)
			{
				RemoveChildAlgorithms();
				PreallocateChildAlgorithms(body0, body1);
			}
			Dbvt dynamicAabbTree = compoundShape.GetDynamicAabbTree();
			using (CompoundLeafCallback compoundLeafCallback = BulletGlobals.CompoundLeafCallbackPool.Get())
			{
				compoundLeafCallback.Initialize(collisionObject, collisionObject2, m_dispatcher, dispatchInfo, resultOut, this, m_childCollisionAlgorithms, m_sharedManifold);
				m_manifoldArray.Clear();
				for (int i = 0; i < m_childCollisionAlgorithms.Count; i++)
				{
					if (m_childCollisionAlgorithms[i] == null)
					{
						continue;
					}
					m_childCollisionAlgorithms[i].GetAllContactManifolds(m_manifoldArray);
					for (int j = 0; j < m_manifoldArray.Count; j++)
					{
						if (m_manifoldArray[j].GetNumContacts() > 0)
						{
							resultOut.SetPersistentManifold(m_manifoldArray[j]);
							resultOut.RefreshContactPoints();
							resultOut.SetPersistentManifold(null);
						}
					}
					m_manifoldArray.Clear();
				}
				if (dynamicAabbTree != null)
				{
					IndexedMatrix t = collisionObject.GetWorldTransform().Inverse() * collisionObject2.GetWorldTransform();
					IndexedVector3 aabbMin;
					IndexedVector3 aabbMax;
					collisionObject2.GetCollisionShape().GetAabb(ref t, out aabbMin, out aabbMax);
					DbvtAabbMm volume = DbvtAabbMm.FromMM(ref aabbMin, ref aabbMax);
					Dbvt.CollideTV(dynamicAabbTree.m_root, ref volume, compoundLeafCallback);
				}
				else
				{
					int count = m_childCollisionAlgorithms.Count;
					for (int k = 0; k < count; k++)
					{
						compoundLeafCallback.ProcessChildShape(compoundShape.GetChildShape(k), k);
					}
				}
				int count2 = m_childCollisionAlgorithms.Count;
				m_manifoldArray.Clear();
				CollisionShape collisionShape = null;
				for (int l = 0; l < count2; l++)
				{
					if (m_childCollisionAlgorithms[l] != null)
					{
						collisionShape = compoundShape.GetChildShape(l);
						IndexedMatrix worldTransform = collisionObject.GetWorldTransform();
						collisionObject.GetInterpolationWorldTransform();
						IndexedMatrix childTransform = compoundShape.GetChildTransform(l);
						IndexedMatrix t2 = worldTransform * childTransform;
						IndexedVector3 aabbMin2;
						IndexedVector3 aabbMax2;
						collisionShape.GetAabb(ref t2, out aabbMin2, out aabbMax2);
						IndexedVector3 aabbMin3;
						IndexedVector3 aabbMax3;
						collisionObject2.GetCollisionShape().GetAabb(collisionObject2.GetWorldTransform(), out aabbMin3, out aabbMax3);
						if (!AabbUtil2.TestAabbAgainstAabb2(ref aabbMin2, ref aabbMax2, ref aabbMin3, ref aabbMax3))
						{
							m_dispatcher.FreeCollisionAlgorithm(m_childCollisionAlgorithms[l]);
							m_childCollisionAlgorithms[l] = null;
						}
					}
				}
			}
		}

		public override void GetAllContactManifolds(PersistentManifoldArray manifoldArray)
		{
			for (int i = 0; i < m_childCollisionAlgorithms.Count; i++)
			{
				if (m_childCollisionAlgorithms[i] != null)
				{
					CollisionAlgorithm collisionAlgorithm = m_childCollisionAlgorithms[i];
					m_childCollisionAlgorithms[i].GetAllContactManifolds(manifoldArray);
				}
			}
		}

		public override float CalculateTimeOfImpact(CollisionObject body0, CollisionObject body1, DispatcherInfo dispatchInfo, ManifoldResult resultOut)
		{
			resultOut = null;
			CollisionObject collisionObject = (m_isSwapped ? body1 : body0);
			CollisionObject body2 = (m_isSwapped ? body0 : body1);
			CompoundShape compoundShape = (CompoundShape)collisionObject.GetCollisionShape();
			float num = 1f;
			int count = m_childCollisionAlgorithms.Count;
			for (int i = 0; i < count; i++)
			{
				CollisionShape childShape = compoundShape.GetChildShape(i);
				IndexedMatrix worldTrans = collisionObject.GetWorldTransform();
				IndexedMatrix childTransform = compoundShape.GetChildTransform(i);
				IndexedMatrix worldTrans2 = worldTrans * childTransform;
				collisionObject.SetWorldTransform(ref worldTrans2);
				CollisionShape collisionShape = collisionObject.GetCollisionShape();
				collisionObject.InternalSetTemporaryCollisionShape(childShape);
				float num2 = m_childCollisionAlgorithms[i].CalculateTimeOfImpact(collisionObject, body2, dispatchInfo, resultOut);
				if (num2 < num)
				{
					num = num2;
				}
				collisionObject.InternalSetTemporaryCollisionShape(collisionShape);
				collisionObject.SetWorldTransform(ref worldTrans);
			}
			return num;
		}
	}
}
