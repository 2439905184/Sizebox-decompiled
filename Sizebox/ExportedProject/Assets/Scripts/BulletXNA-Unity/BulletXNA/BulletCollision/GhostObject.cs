using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class GhostObject : CollisionObject
	{
		protected ObjectArray<CollisionObject> m_overlappingObjects = new ObjectArray<CollisionObject>();

		public GhostObject()
		{
			SetInternalType(CollisionObjectTypes.CO_GHOST_OBJECT);
		}

		public override void Cleanup()
		{
		}

		public void ConvexSweepTest(ConvexShape castShape, ref IndexedMatrix convexFromWorld, ref IndexedMatrix convexToWorld, ConvexResultCallback resultCallback, float allowedCcdPenetration)
		{
			IndexedMatrix transform = convexFromWorld;
			IndexedMatrix transform2 = convexToWorld;
			IndexedVector3 linVel;
			IndexedVector3 angVel;
			TransformUtil.CalculateVelocity(ref transform, ref transform2, 1f, out linVel, out angVel);
			IndexedMatrix curTrans = IndexedMatrix.Identity;
			curTrans.SetRotation(transform.GetRotation());
			IndexedVector3 temporalAabbMin;
			IndexedVector3 temporalAabbMax;
			castShape.CalculateTemporalAabb(ref curTrans, ref linVel, ref angVel, 1f, out temporalAabbMin, out temporalAabbMax);
			for (int i = 0; i < m_overlappingObjects.Count; i++)
			{
				CollisionObject collisionObject = m_overlappingObjects[i];
				if (resultCallback.NeedsCollision(collisionObject.GetBroadphaseHandle()))
				{
					IndexedMatrix t = collisionObject.GetWorldTransform();
					IndexedVector3 aabbMin;
					IndexedVector3 aabbMax;
					collisionObject.GetCollisionShape().GetAabb(ref t, out aabbMin, out aabbMax);
					AabbUtil2.AabbExpand(ref aabbMin, ref aabbMax, ref temporalAabbMin, ref temporalAabbMax);
					float param = 1f;
					IndexedVector3 normal;
					if (AabbUtil2.RayAabb(convexFromWorld._origin, convexToWorld._origin, ref aabbMin, ref aabbMax, ref param, out normal))
					{
						IndexedMatrix colObjWorldTransform = collisionObject.GetWorldTransform();
						CollisionWorld.ObjectQuerySingle(castShape, ref transform, ref transform2, collisionObject, collisionObject.GetCollisionShape(), ref colObjWorldTransform, resultCallback, allowedCcdPenetration);
					}
				}
			}
		}

		public void RayTest(ref IndexedVector3 rayFromWorld, ref IndexedVector3 rayToWorld, RayResultCallback resultCallback)
		{
		}

		public virtual void AddOverlappingObjectInternal(BroadphaseProxy otherProxy, BroadphaseProxy thisProxy)
		{
			CollisionObject item = otherProxy.m_clientObject as CollisionObject;
			if (!m_overlappingObjects.Contains(item))
			{
				m_overlappingObjects.Add(item);
			}
		}

		public virtual void RemoveOverlappingObjectInternal(BroadphaseProxy otherProxy, IDispatcher dispatcher, BroadphaseProxy thisProxy)
		{
			CollisionObject item = otherProxy.m_clientObject as CollisionObject;
			m_overlappingObjects.RemoveQuick(item);
		}

		public int GetNumOverlappingObjects()
		{
			return m_overlappingObjects.Count;
		}

		public CollisionObject GetOverlappingObject(int index)
		{
			return m_overlappingObjects[index];
		}

		public ObjectArray<CollisionObject> GetOverlappingPairs()
		{
			return m_overlappingObjects;
		}

		public static GhostObject Upcast(CollisionObject colObj)
		{
			if (colObj.GetInternalType() == CollisionObjectTypes.CO_GHOST_OBJECT)
			{
				return (GhostObject)colObj;
			}
			return null;
		}
	}
}
