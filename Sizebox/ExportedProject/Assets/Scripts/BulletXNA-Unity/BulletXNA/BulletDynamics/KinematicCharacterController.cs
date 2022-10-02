using System;
using BulletXNA.BulletCollision;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletDynamics
{
	public class KinematicCharacterController : ICharacterControllerInterface, IActionInterface
	{
		protected float m_halfHeight;

		protected PairCachingGhostObject m_ghostObject;

		protected ConvexShape m_convexShape;

		protected float m_verticalVelocity;

		protected float m_verticalOffset;

		protected float m_fallSpeed;

		protected float m_jumpSpeed;

		protected float m_maxJumpHeight;

		protected float m_maxSlopeRadians;

		protected float m_maxSlopeCosine;

		protected float m_gravity;

		protected float m_turnAngle;

		protected float m_stepHeight;

		protected float m_addedMargin;

		protected IndexedVector3 m_walkDirection;

		protected IndexedVector3 m_normalizedDirection;

		protected IndexedVector3 m_currentPosition;

		private float m_currentStepOffset;

		protected IndexedVector3 m_targetPosition;

		protected PersistentManifoldArray m_manifoldArray = new PersistentManifoldArray();

		protected bool m_touchingContact;

		protected IndexedVector3 m_touchingNormal;

		protected bool m_wasOnGround;

		protected bool m_wasJumping;

		protected bool m_useGhostObjectSweepTest;

		protected int m_upAxis;

		protected bool m_useWalkDirection;

		protected float m_velocityTimeInterval;

		protected static IndexedVector3[] upAxisDirection = new IndexedVector3[3]
		{
			new IndexedVector3(1f, 0f, 0f),
			new IndexedVector3(0f, 1f, 0f),
			new IndexedVector3(0f, 0f, 1f)
		};

		protected IndexedVector3 ComputeReflectionDirection(ref IndexedVector3 direction, ref IndexedVector3 normal)
		{
			return direction - 2f * direction.Dot(ref normal) * normal;
		}

		protected IndexedVector3 ParallelComponent(ref IndexedVector3 direction, ref IndexedVector3 normal)
		{
			float num = IndexedVector3.Dot(direction, normal);
			return normal * num;
		}

		protected IndexedVector3 PerpindicularComponent(ref IndexedVector3 direction, ref IndexedVector3 normal)
		{
			return direction - ParallelComponent(ref direction, ref normal);
		}

		protected bool RecoverFromPenetration(CollisionWorld collisionWorld)
		{
			bool result = false;
			collisionWorld.GetDispatcher().DispatchAllCollisionPairs(m_ghostObject.GetOverlappingPairCache(), collisionWorld.GetDispatchInfo(), collisionWorld.GetDispatcher());
			m_currentPosition = m_ghostObject.GetWorldTransform()._origin;
			float num = 0f;
			for (int i = 0; i < m_ghostObject.GetOverlappingPairCache().GetNumOverlappingPairs(); i++)
			{
				m_manifoldArray.Clear();
				BroadphasePair broadphasePair = m_ghostObject.GetOverlappingPairCache().GetOverlappingPairArray()[i];
				if (broadphasePair.m_algorithm != null)
				{
					broadphasePair.m_algorithm.GetAllContactManifolds(m_manifoldArray);
				}
				for (int j = 0; j < m_manifoldArray.Count; j++)
				{
					PersistentManifold persistentManifold = m_manifoldArray[j];
					float num2 = ((persistentManifold.GetBody0() == m_ghostObject) ? (-1f) : 1f);
					for (int k = 0; k < persistentManifold.GetNumContacts(); k++)
					{
						ManifoldPoint contactPoint = persistentManifold.GetContactPoint(k);
						float distance = contactPoint.GetDistance();
						if ((double)distance < 0.0)
						{
							if (distance < num)
							{
								num = distance;
								m_touchingNormal = contactPoint.m_normalWorldOnB * num2;
							}
							m_currentPosition += contactPoint.m_normalWorldOnB * num2 * distance * 0.2f;
							result = true;
						}
					}
				}
			}
			IndexedMatrix worldTrans = m_ghostObject.GetWorldTransform();
			worldTrans._origin = m_currentPosition;
			m_ghostObject.SetWorldTransform(ref worldTrans);
			return result;
		}

		protected void StepUp(CollisionWorld collisionWorld)
		{
			IndexedMatrix convexFromWorld = IndexedMatrix.Identity;
			IndexedMatrix convexToWorld = IndexedMatrix.Identity;
			m_targetPosition = m_currentPosition + upAxisDirection[m_upAxis] * (m_stepHeight + ((m_verticalOffset > 0f) ? m_verticalOffset : 0f));
			convexFromWorld._origin = m_currentPosition + upAxisDirection[m_upAxis] * (m_convexShape.GetMargin() + m_addedMargin);
			convexToWorld._origin = m_targetPosition;
			KinematicClosestNotMeConvexResultCallback kinematicClosestNotMeConvexResultCallback = new KinematicClosestNotMeConvexResultCallback(m_ghostObject, -upAxisDirection[m_upAxis], 0.7071f);
			kinematicClosestNotMeConvexResultCallback.m_collisionFilterGroup = GetGhostObject().GetBroadphaseHandle().m_collisionFilterGroup;
			kinematicClosestNotMeConvexResultCallback.m_collisionFilterMask = GetGhostObject().GetBroadphaseHandle().m_collisionFilterMask;
			if (m_useGhostObjectSweepTest)
			{
				m_ghostObject.ConvexSweepTest(m_convexShape, ref convexFromWorld, ref convexToWorld, kinematicClosestNotMeConvexResultCallback, collisionWorld.GetDispatchInfo().GetAllowedCcdPenetration());
			}
			else
			{
				collisionWorld.ConvexSweepTest(m_convexShape, ref convexFromWorld, ref convexToWorld, kinematicClosestNotMeConvexResultCallback, 0f);
			}
			if (kinematicClosestNotMeConvexResultCallback.HasHit())
			{
				if ((double)IndexedVector3.Dot(kinematicClosestNotMeConvexResultCallback.m_hitNormalWorld, upAxisDirection[m_upAxis]) > 0.0)
				{
					m_currentStepOffset = m_stepHeight * kinematicClosestNotMeConvexResultCallback.m_closestHitFraction;
					m_currentPosition = MathUtil.Interpolate3(ref m_currentPosition, ref m_targetPosition, kinematicClosestNotMeConvexResultCallback.m_closestHitFraction);
				}
				m_verticalVelocity = 0f;
				m_verticalOffset = 0f;
			}
			else
			{
				m_currentStepOffset = m_stepHeight;
				m_currentPosition = m_targetPosition;
			}
		}

		protected void UpdateTargetPositionBasedOnCollision(ref IndexedVector3 hitNormal, float tangentMag, float normalMag)
		{
			IndexedVector3 direction = m_targetPosition - m_currentPosition;
			float num = direction.Length();
			if (num > 1.1920929E-07f)
			{
				direction.Normalize();
				IndexedVector3 direction2 = ComputeReflectionDirection(ref direction, ref hitNormal);
				direction2.Normalize();
				ParallelComponent(ref direction2, ref hitNormal);
				IndexedVector3 indexedVector = PerpindicularComponent(ref direction2, ref hitNormal);
				m_targetPosition = m_currentPosition;
				if (normalMag != 0f)
				{
					IndexedVector3 indexedVector2 = indexedVector * (normalMag * num);
					m_targetPosition += indexedVector2;
				}
			}
		}

		protected void StepForwardAndStrafe(CollisionWorld collisionWorld, ref IndexedVector3 walkMove)
		{
			IndexedMatrix convexFromWorld = IndexedMatrix.Identity;
			IndexedMatrix convexToWorld = IndexedMatrix.Identity;
			m_targetPosition = m_currentPosition + walkMove;
			float num = 1f;
			float num2 = (m_currentPosition - m_targetPosition).LengthSquared();
			if (m_touchingContact && IndexedVector3.Dot(m_normalizedDirection, m_touchingNormal) > 0f)
			{
				UpdateTargetPositionBasedOnCollision(ref m_touchingNormal, 0f, 1f);
			}
			int num3 = 10;
			while (num > 0.01f && num3-- > 0)
			{
				convexFromWorld._origin = m_currentPosition;
				convexToWorld._origin = m_targetPosition;
				IndexedVector3 up = m_currentPosition - m_targetPosition;
				KinematicClosestNotMeConvexResultCallback kinematicClosestNotMeConvexResultCallback = new KinematicClosestNotMeConvexResultCallback(m_ghostObject, up, 0f);
				kinematicClosestNotMeConvexResultCallback.m_collisionFilterGroup = GetGhostObject().GetBroadphaseHandle().m_collisionFilterGroup;
				kinematicClosestNotMeConvexResultCallback.m_collisionFilterMask = GetGhostObject().GetBroadphaseHandle().m_collisionFilterMask;
				float margin = m_convexShape.GetMargin();
				m_convexShape.SetMargin(margin + m_addedMargin);
				if (m_useGhostObjectSweepTest)
				{
					m_ghostObject.ConvexSweepTest(m_convexShape, ref convexFromWorld, ref convexToWorld, kinematicClosestNotMeConvexResultCallback, collisionWorld.GetDispatchInfo().GetAllowedCcdPenetration());
				}
				else
				{
					collisionWorld.ConvexSweepTest(m_convexShape, ref convexFromWorld, ref convexToWorld, kinematicClosestNotMeConvexResultCallback, collisionWorld.GetDispatchInfo().GetAllowedCcdPenetration());
				}
				m_convexShape.SetMargin(margin);
				num -= kinematicClosestNotMeConvexResultCallback.m_closestHitFraction;
				if (kinematicClosestNotMeConvexResultCallback.HasHit())
				{
					(kinematicClosestNotMeConvexResultCallback.m_hitPointWorld - m_currentPosition).Length();
					UpdateTargetPositionBasedOnCollision(ref kinematicClosestNotMeConvexResultCallback.m_hitNormalWorld, 0f, 1f);
					IndexedVector3 a = m_targetPosition - m_currentPosition;
					num2 = a.LengthSquared();
					if (!(num2 > 1.1920929E-07f))
					{
						break;
					}
					a.Normalize();
					if (IndexedVector3.Dot(a, m_normalizedDirection) <= 0f)
					{
						break;
					}
				}
				else
				{
					m_currentPosition = m_targetPosition;
				}
			}
		}

		protected void StepDown(CollisionWorld collisionWorld, float dt)
		{
			IndexedMatrix convexFromWorld = IndexedMatrix.Identity;
			IndexedMatrix convexToWorld = IndexedMatrix.Identity;
			float num = ((m_verticalVelocity < 0f) ? (0f - m_verticalVelocity) : 0f) * dt;
			if ((double)num > 0.0 && num < m_stepHeight && (m_wasOnGround || !m_wasJumping))
			{
				num = m_stepHeight;
			}
			IndexedVector3 indexedVector = upAxisDirection[m_upAxis] * (m_currentStepOffset + num);
			m_targetPosition -= indexedVector;
			convexFromWorld._origin = m_currentPosition;
			convexToWorld._origin = m_targetPosition;
			KinematicClosestNotMeConvexResultCallback kinematicClosestNotMeConvexResultCallback = new KinematicClosestNotMeConvexResultCallback(m_ghostObject, upAxisDirection[m_upAxis], m_maxSlopeCosine);
			kinematicClosestNotMeConvexResultCallback.m_collisionFilterGroup = GetGhostObject().GetBroadphaseHandle().m_collisionFilterGroup;
			kinematicClosestNotMeConvexResultCallback.m_collisionFilterMask = GetGhostObject().GetBroadphaseHandle().m_collisionFilterMask;
			if (m_useGhostObjectSweepTest)
			{
				m_ghostObject.ConvexSweepTest(m_convexShape, ref convexFromWorld, ref convexToWorld, kinematicClosestNotMeConvexResultCallback, collisionWorld.GetDispatchInfo().GetAllowedCcdPenetration());
			}
			else
			{
				collisionWorld.ConvexSweepTest(m_convexShape, convexFromWorld, convexToWorld, kinematicClosestNotMeConvexResultCallback, collisionWorld.GetDispatchInfo().GetAllowedCcdPenetration());
			}
			if (kinematicClosestNotMeConvexResultCallback.HasHit())
			{
				m_currentPosition = MathUtil.Interpolate3(ref m_currentPosition, ref m_targetPosition, kinematicClosestNotMeConvexResultCallback.m_closestHitFraction);
				m_verticalVelocity = 0f;
				m_verticalOffset = 0f;
				m_wasJumping = false;
			}
			else
			{
				m_currentPosition = m_targetPosition;
			}
		}

		public KinematicCharacterController(PairCachingGhostObject ghostObject, ConvexShape convexShape, float stepHeight, int upAxis)
		{
			m_upAxis = upAxis;
			m_addedMargin = 0.02f;
			m_walkDirection = IndexedVector3.Zero;
			m_useGhostObjectSweepTest = true;
			m_ghostObject = ghostObject;
			m_stepHeight = stepHeight;
			m_turnAngle = 0f;
			m_convexShape = convexShape;
			m_useWalkDirection = true;
			m_velocityTimeInterval = 0f;
			m_verticalVelocity = 0f;
			m_verticalOffset = 0f;
			m_gravity = 29.400002f;
			m_fallSpeed = 55f;
			m_jumpSpeed = 10f;
			m_wasOnGround = false;
			m_wasJumping = false;
			SetMaxSlope(MathUtil.DegToRadians(45f));
		}

		public virtual void UpdateAction(CollisionWorld collisionWorld, float deltaTime)
		{
			PreStep(collisionWorld);
			PlayerStep(collisionWorld, deltaTime);
		}

		public void DebugDraw(IDebugDraw debugDrawer)
		{
		}

		public void SetUpAxis(int axis)
		{
			if (axis < 0)
			{
				axis = 0;
			}
			if (axis > 2)
			{
				axis = 2;
			}
			m_upAxis = axis;
		}

		public virtual void SetWalkDirection(ref IndexedVector3 walkDirection)
		{
			m_useWalkDirection = true;
			m_walkDirection = walkDirection;
			m_normalizedDirection = GetNormalizedVector(ref m_walkDirection);
		}

		public void SetVelocityForTimeInterval(ref IndexedVector3 velocity, float timeInterval)
		{
			m_useWalkDirection = false;
			m_walkDirection = velocity;
			m_normalizedDirection = GetNormalizedVector(ref m_walkDirection);
			m_velocityTimeInterval = timeInterval;
		}

		public void Reset()
		{
		}

		public void Warp(ref IndexedVector3 origin)
		{
			IndexedMatrix worldTrans = IndexedMatrix.CreateTranslation(origin);
			m_ghostObject.SetWorldTransform(ref worldTrans);
		}

		public void PreStep(CollisionWorld collisionWorld)
		{
			int num = 0;
			m_touchingContact = false;
			while (RecoverFromPenetration(collisionWorld))
			{
				num++;
				m_touchingContact = true;
				if (num > 4)
				{
					break;
				}
			}
			m_currentPosition = m_ghostObject.GetWorldTransform()._origin;
			m_targetPosition = m_currentPosition;
		}

		public void PlayerStep(CollisionWorld collisionWorld, float dt)
		{
			if (m_useWalkDirection || !((double)m_velocityTimeInterval <= 0.0))
			{
				m_wasOnGround = OnGround();
				m_verticalVelocity -= m_gravity * dt;
				if (m_verticalVelocity > 0f && m_verticalVelocity > m_jumpSpeed)
				{
					m_verticalVelocity = m_jumpSpeed;
				}
				if (m_verticalVelocity < 0f && Math.Abs(m_verticalVelocity) > Math.Abs(m_fallSpeed))
				{
					m_verticalVelocity = 0f - Math.Abs(m_fallSpeed);
				}
				m_verticalOffset = m_verticalVelocity * dt;
				IndexedMatrix worldTrans = m_ghostObject.GetWorldTransform();
				StepUp(collisionWorld);
				if (m_useWalkDirection)
				{
					StepForwardAndStrafe(collisionWorld, ref m_walkDirection);
				}
				else
				{
					float num = ((dt < m_velocityTimeInterval) ? dt : m_velocityTimeInterval);
					m_velocityTimeInterval -= dt;
					IndexedVector3 walkMove = m_walkDirection * num;
					StepForwardAndStrafe(collisionWorld, ref walkMove);
				}
				StepDown(collisionWorld, dt);
				worldTrans._origin = m_currentPosition;
				m_ghostObject.SetWorldTransform(ref worldTrans);
			}
		}

		public void SetFallSpeed(float fallSpeed)
		{
			m_fallSpeed = fallSpeed;
		}

		public void SetJumpSpeed(float jumpSpeed)
		{
			m_jumpSpeed = jumpSpeed;
		}

		public void SetMaxJumpHeight(float maxJumpHeight)
		{
			m_maxJumpHeight = maxJumpHeight;
		}

		public bool CanJump()
		{
			return OnGround();
		}

		public void Jump()
		{
			if (CanJump())
			{
				m_verticalVelocity = m_jumpSpeed;
				m_wasJumping = true;
			}
		}

		public void SetGravity(float gravity)
		{
			m_gravity = gravity;
		}

		public float GetGravity()
		{
			return m_gravity;
		}

		public void SetMaxSlope(float slopeRadians)
		{
			m_maxSlopeRadians = slopeRadians;
			m_maxSlopeCosine = (float)Math.Cos(slopeRadians);
		}

		public float GetMaxSlope()
		{
			return m_maxSlopeRadians;
		}

		public PairCachingGhostObject GetGhostObject()
		{
			return m_ghostObject;
		}

		public void SetUseGhostSweepTest(bool useGhostObjectSweepTest)
		{
			m_useGhostObjectSweepTest = useGhostObjectSweepTest;
		}

		public bool OnGround()
		{
			if (m_verticalVelocity == 0f)
			{
				return m_verticalOffset == 0f;
			}
			return false;
		}

		public static IndexedVector3 GetNormalizedVector(ref IndexedVector3 v)
		{
			IndexedVector3 result = IndexedVector3.Normalize(v);
			if (result.Length() < 1.1920929E-07f)
			{
				result = IndexedVector3.Zero;
			}
			return result;
		}
	}
}
