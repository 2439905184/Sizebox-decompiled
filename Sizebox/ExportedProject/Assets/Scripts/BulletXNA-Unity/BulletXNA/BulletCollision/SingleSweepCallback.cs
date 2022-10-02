using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class SingleSweepCallback : BroadphaseRayCallback
	{
		private IndexedMatrix m_convexFromTrans;

		private IndexedMatrix m_convexToTrans;

		private IndexedVector3 m_hitNormal;

		private CollisionWorld m_world;

		private ConvexResultCallback m_resultCallback;

		private float m_allowedCcdPenetration;

		private ConvexShape m_castShape;

		public SingleSweepCallback()
		{
		}

		public SingleSweepCallback(ConvexShape castShape, ref IndexedMatrix convexFromTrans, ref IndexedMatrix convexToTrans, CollisionWorld world, ConvexResultCallback resultCallback, float allowedPenetration)
		{
			Initialize(castShape, ref convexFromTrans, ref convexToTrans, world, resultCallback, allowedPenetration);
		}

		public void Initialize(ConvexShape castShape, ref IndexedMatrix convexFromTrans, ref IndexedMatrix convexToTrans, CollisionWorld world, ConvexResultCallback resultCallback, float allowedPenetration)
		{
			m_convexFromTrans = convexFromTrans;
			m_convexToTrans = convexToTrans;
			m_world = world;
			m_resultCallback = resultCallback;
			m_allowedCcdPenetration = allowedPenetration;
			m_castShape = castShape;
			IndexedVector3 v = m_convexToTrans._origin - m_convexFromTrans._origin;
			IndexedVector3 indexedVector = v;
			indexedVector.Normalize();
			m_rayDirectionInverse.X = (MathUtil.CompareFloat(indexedVector.X, 0f) ? float.MaxValue : (1f / indexedVector.X));
			m_rayDirectionInverse.Y = (MathUtil.CompareFloat(indexedVector.Y, 0f) ? float.MaxValue : (1f / indexedVector.Y));
			m_rayDirectionInverse.Z = (MathUtil.CompareFloat(indexedVector.Z, 0f) ? float.MaxValue : (1f / indexedVector.Z));
			m_signs[0] = (double)m_rayDirectionInverse.X < 0.0;
			m_signs[1] = (double)m_rayDirectionInverse.Y < 0.0;
			m_signs[2] = (double)m_rayDirectionInverse.Z < 0.0;
			m_lambda_max = indexedVector.Dot(ref v);
		}

		public override bool Process(BroadphaseProxy proxy)
		{
			if (MathUtil.FuzzyZero(m_resultCallback.m_closestHitFraction))
			{
				return false;
			}
			CollisionObject collisionObject = proxy.m_clientObject as CollisionObject;
			if (m_resultCallback.NeedsCollision(collisionObject.GetBroadphaseHandle()))
			{
				IndexedMatrix colObjWorldTransform = collisionObject.GetWorldTransform();
				CollisionWorld.ObjectQuerySingle(m_castShape, ref m_convexFromTrans, ref m_convexToTrans, collisionObject, collisionObject.GetCollisionShape(), ref colObjWorldTransform, m_resultCallback, m_allowedCcdPenetration);
			}
			return true;
		}
	}
}
