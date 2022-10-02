using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class ConvexPlaneCollisionAlgorithm : CollisionAlgorithm
	{
		private bool m_ownManifold;

		private PersistentManifold m_manifoldPtr;

		private bool m_isSwapped;

		private int m_numPerturbationIterations;

		private int m_minimumPointsPerturbationThreshold;

		public ConvexPlaneCollisionAlgorithm()
		{
		}

		public ConvexPlaneCollisionAlgorithm(PersistentManifold mf, CollisionAlgorithmConstructionInfo ci, CollisionObject col0, CollisionObject col1, bool isSwapped, int numPerturbationIterations, int minimumPointsPerturbationThreshold)
			: base(ci)
		{
			m_manifoldPtr = mf;
			m_ownManifold = false;
			m_isSwapped = isSwapped;
			m_numPerturbationIterations = numPerturbationIterations;
			m_minimumPointsPerturbationThreshold = minimumPointsPerturbationThreshold;
			CollisionObject body = (m_isSwapped ? col1 : col0);
			CollisionObject body2 = (m_isSwapped ? col0 : col1);
			if (m_manifoldPtr == null && m_dispatcher.NeedsCollision(body, body2))
			{
				m_manifoldPtr = m_dispatcher.GetNewManifold(body, body2);
				m_ownManifold = true;
			}
		}

		public void Initialize(PersistentManifold mf, CollisionAlgorithmConstructionInfo ci, CollisionObject col0, CollisionObject col1, bool isSwapped, int numPerturbationIterations, int minimumPointsPerturbationThreshold)
		{
			base.Initialize(ci);
			m_manifoldPtr = mf;
			m_ownManifold = false;
			m_isSwapped = isSwapped;
			m_numPerturbationIterations = numPerturbationIterations;
			m_minimumPointsPerturbationThreshold = minimumPointsPerturbationThreshold;
			CollisionObject body = (m_isSwapped ? col1 : col0);
			CollisionObject body2 = (m_isSwapped ? col0 : col1);
			if (m_manifoldPtr == null && m_dispatcher.NeedsCollision(body, body2))
			{
				m_manifoldPtr = m_dispatcher.GetNewManifold(body, body2);
				m_ownManifold = true;
			}
		}

		public override void Cleanup()
		{
			if (m_ownManifold)
			{
				if (m_manifoldPtr != null)
				{
					m_dispatcher.ReleaseManifold(m_manifoldPtr);
					m_manifoldPtr = null;
				}
				m_ownManifold = false;
			}
			BulletGlobals.ConvexPlaneAlgorithmPool.Free(this);
		}

		public override void ProcessCollision(CollisionObject body0, CollisionObject body1, DispatcherInfo dispatchInfo, ManifoldResult resultOut)
		{
			if (m_manifoldPtr == null)
			{
				return;
			}
			CollisionObject collisionObject = (m_isSwapped ? body1 : body0);
			CollisionObject collisionObject2 = (m_isSwapped ? body0 : body1);
			ConvexShape convexShape = collisionObject.GetCollisionShape() as ConvexShape;
			StaticPlaneShape staticPlaneShape = collisionObject2.GetCollisionShape() as StaticPlaneShape;
			bool flag = false;
			IndexedVector3 n = staticPlaneShape.GetPlaneNormal();
			float planeConstant = staticPlaneShape.GetPlaneConstant();
			IndexedMatrix indexedMatrix = collisionObject.GetWorldTransform().Inverse() * collisionObject2.GetWorldTransform();
			IndexedMatrix indexedMatrix2 = collisionObject2.GetWorldTransform().Inverse() * collisionObject.GetWorldTransform();
			IndexedVector3 indexedVector = convexShape.LocalGetSupportingVertex(indexedMatrix._basis * -n);
			IndexedVector3 indexedVector2 = indexedMatrix2 * indexedVector;
			float num = n.Dot(indexedVector2) - planeConstant;
			IndexedVector3 indexedVector3 = indexedVector2 - num * n;
			IndexedVector3 indexedVector4 = collisionObject2.GetWorldTransform() * indexedVector3;
			flag = num < m_manifoldPtr.GetContactBreakingThreshold();
			resultOut.SetPersistentManifold(m_manifoldPtr);
			if (flag)
			{
				IndexedVector3 normalOnBInWorld = collisionObject2.GetWorldTransform()._basis * n;
				IndexedVector3 pointInWorld = indexedVector4;
				resultOut.AddContactPoint(normalOnBInWorld, pointInWorld, num);
			}
			if (convexShape.IsPolyhedral() && resultOut.GetPersistentManifold().GetNumContacts() < m_minimumPointsPerturbationThreshold)
			{
				IndexedVector3 p;
				IndexedVector3 q;
				TransformUtil.PlaneSpace1(ref n, out p, out q);
				float num2 = (float)Math.PI / 8f;
				float angularMotionDisc = convexShape.GetAngularMotionDisc();
				float num3 = BulletGlobals.gContactBreakingThreshold / angularMotionDisc;
				if (num3 > num2)
				{
					num3 = num2;
				}
				IndexedQuaternion indexedQuaternion = new IndexedQuaternion(p, num3);
				for (int i = 0; i < m_numPerturbationIterations; i++)
				{
					float angle = (float)i * ((float)Math.PI * 2f / (float)m_numPerturbationIterations);
					IndexedQuaternion indexedQuaternion2 = new IndexedQuaternion(n, angle);
					indexedQuaternion2 = IndexedQuaternion.Inverse(indexedQuaternion2) * indexedQuaternion * indexedQuaternion2;
					CollideSingleContact(ref indexedQuaternion2, body0, body1, dispatchInfo, resultOut);
				}
			}
			if (m_ownManifold && m_manifoldPtr.GetNumContacts() > 0)
			{
				resultOut.RefreshContactPoints();
			}
		}

		public virtual void CollideSingleContact(ref IndexedQuaternion perturbeRot, CollisionObject body0, CollisionObject body1, DispatcherInfo dispatchInfo, ManifoldResult resultOut)
		{
			CollisionObject collisionObject = (m_isSwapped ? body1 : body0);
			CollisionObject collisionObject2 = (m_isSwapped ? body0 : body1);
			ConvexShape convexShape = collisionObject.GetCollisionShape() as ConvexShape;
			StaticPlaneShape staticPlaneShape = collisionObject2.GetCollisionShape() as StaticPlaneShape;
			bool flag = false;
			IndexedVector3 planeNormal = staticPlaneShape.GetPlaneNormal();
			float planeConstant = staticPlaneShape.GetPlaneConstant();
			IndexedMatrix worldTransform = collisionObject.GetWorldTransform();
			IndexedMatrix indexedMatrix = collisionObject2.GetWorldTransform().Inverse() * worldTransform;
			worldTransform._basis *= new IndexedBasisMatrix(ref perturbeRot);
			IndexedVector3 indexedVector = convexShape.LocalGetSupportingVertex((worldTransform.Inverse() * collisionObject2.GetWorldTransform())._basis * -planeNormal);
			IndexedVector3 indexedVector2 = (indexedVector2 = indexedMatrix * indexedVector);
			float num = IndexedVector3.Dot(planeNormal, indexedVector2) - planeConstant;
			IndexedVector3 indexedVector3 = indexedVector2 - num * planeNormal;
			IndexedVector3 indexedVector4 = collisionObject2.GetWorldTransform() * indexedVector3;
			flag = num < m_manifoldPtr.GetContactBreakingThreshold();
			resultOut.SetPersistentManifold(m_manifoldPtr);
			if (flag)
			{
				IndexedVector3 normalOnBInWorld = collisionObject2.GetWorldTransform()._basis * planeNormal;
				IndexedVector3 pointInWorld = indexedVector4;
				resultOut.AddContactPoint(ref normalOnBInWorld, ref pointInWorld, num);
			}
		}

		public override float CalculateTimeOfImpact(CollisionObject body0, CollisionObject body1, DispatcherInfo dispatchInfo, ManifoldResult resultOut)
		{
			return 1f;
		}

		public override void GetAllContactManifolds(PersistentManifoldArray manifoldArray)
		{
			if (m_manifoldPtr != null && m_ownManifold)
			{
				manifoldArray.Add(m_manifoldPtr);
			}
		}
	}
}
