using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class CollisionObject
	{
		public IndexedMatrix m_worldTransform;

		protected IndexedMatrix m_interpolationWorldTransform = IndexedMatrix.Identity;

		protected IndexedVector3 m_interpolationAngularVelocity;

		protected IndexedVector3 m_interpolationLinearVelocity;

		protected IndexedVector3 m_anisotropicFriction;

		protected bool m_hasAnisotropicFriction;

		protected float m_contactProcessingThreshold;

		protected BroadphaseProxy m_broadphaseHandle;

		protected CollisionShape m_collisionShape;

		protected CollisionShape m_rootCollisionShape;

		protected CollisionFlags m_collisionFlags;

		protected int m_islandTag1;

		protected int m_companionId;

		protected ActivationState m_activationState1;

		protected float m_deactivationTime;

		protected float m_friction;

		protected float m_restitution;

		protected object m_userObjectPointer;

		protected object m_extensionPointer;

		protected CollisionObjectTypes m_internalType;

		protected float m_hitFraction;

		protected float m_ccdSweptSphereRadius;

		protected float m_ccdMotionThreshold;

		protected bool m_checkCollideWith;

		public CollisionObject()
		{
			m_anisotropicFriction = new IndexedVector3(1f);
			m_hasAnisotropicFriction = false;
			m_contactProcessingThreshold = 1E+18f;
			m_broadphaseHandle = null;
			m_collisionShape = null;
			m_rootCollisionShape = null;
			m_collisionFlags = CollisionFlags.CF_STATIC_OBJECT;
			m_islandTag1 = -1;
			m_companionId = -1;
			m_activationState1 = ActivationState.ACTIVE_TAG;
			m_deactivationTime = 0f;
			m_friction = 0.5f;
			m_userObjectPointer = null;
			m_internalType = CollisionObjectTypes.CO_COLLISION_OBJECT;
			m_hitFraction = 1f;
			m_ccdSweptSphereRadius = 0f;
			m_ccdMotionThreshold = 0f;
			m_checkCollideWith = false;
			m_worldTransform = IndexedMatrix.Identity;
		}

		public virtual bool CheckCollideWithOverride(CollisionObject obj)
		{
			return true;
		}

		public bool MergesSimulationIslands()
		{
			CollisionFlags collisionFlags = CollisionFlags.CF_STATIC_OBJECT | CollisionFlags.CF_KINEMATIC_OBJECT | CollisionFlags.CF_NO_CONTACT_RESPONSE;
			return (m_collisionFlags & collisionFlags) == 0;
		}

		public IndexedVector3 GetAnisotropicFriction()
		{
			return m_anisotropicFriction;
		}

		public void SetAnisotropicFriction(ref IndexedVector3 anisotropicFriction)
		{
			m_anisotropicFriction = anisotropicFriction;
			m_hasAnisotropicFriction = anisotropicFriction.X != 1f || anisotropicFriction.Y != 1f || anisotropicFriction.Z != 1f;
		}

		public bool HasAnisotropicFriction()
		{
			return m_hasAnisotropicFriction;
		}

		public void SetContactProcessingThreshold(float contactProcessingThreshold)
		{
			m_contactProcessingThreshold = contactProcessingThreshold;
		}

		public float GetContactProcessingThreshold()
		{
			return m_contactProcessingThreshold;
		}

		public bool IsStaticObject()
		{
			return (m_collisionFlags & CollisionFlags.CF_STATIC_OBJECT) != 0;
		}

		public bool IsKinematicObject()
		{
			return (m_collisionFlags & CollisionFlags.CF_KINEMATIC_OBJECT) != 0;
		}

		public bool IsStaticOrKinematicObject()
		{
			return (m_collisionFlags & (CollisionFlags.CF_STATIC_OBJECT | CollisionFlags.CF_KINEMATIC_OBJECT)) != 0;
		}

		public bool HasContactResponse()
		{
			return (m_collisionFlags & CollisionFlags.CF_NO_CONTACT_RESPONSE) == 0;
		}

		public virtual void SetCollisionShape(CollisionShape collisionShape)
		{
			m_collisionShape = collisionShape;
			m_rootCollisionShape = collisionShape;
		}

		public CollisionShape GetCollisionShape()
		{
			return m_collisionShape;
		}

		public CollisionShape GetRootCollisionShape()
		{
			return m_rootCollisionShape;
		}

		public void InternalSetTemporaryCollisionShape(CollisionShape collisionShape)
		{
			m_collisionShape = collisionShape;
		}

		protected object InternalGetExtensionPointer()
		{
			return m_extensionPointer;
		}

		protected void InternalSetExtensionPointer(object pointer)
		{
			m_extensionPointer = pointer;
		}

		public ActivationState GetActivationState()
		{
			return m_activationState1;
		}

		public void SetActivationState(ActivationState newState)
		{
			if (m_activationState1 != ActivationState.DISABLE_DEACTIVATION && m_activationState1 != ActivationState.DISABLE_SIMULATION)
			{
				m_activationState1 = newState;
			}
		}

		public void SetDeactivationTime(float time)
		{
			m_deactivationTime = time;
		}

		public float GetDeactivationTime()
		{
			return m_deactivationTime;
		}

		public void ForceActivationState(ActivationState newState)
		{
			m_activationState1 = newState;
		}

		public void Activate()
		{
			Activate(false);
		}

		public void Activate(bool forceActivation)
		{
			CollisionFlags collisionFlags = CollisionFlags.CF_STATIC_OBJECT | CollisionFlags.CF_KINEMATIC_OBJECT;
			if (forceActivation || (m_collisionFlags & collisionFlags) == 0)
			{
				SetActivationState(ActivationState.ACTIVE_TAG);
				m_deactivationTime = 0f;
			}
		}

		public bool IsActive()
		{
			ActivationState activationState = GetActivationState();
			if (activationState != ActivationState.ISLAND_SLEEPING)
			{
				return activationState != ActivationState.DISABLE_SIMULATION;
			}
			return false;
		}

		public void SetRestitution(float rest)
		{
			m_restitution = rest;
		}

		public float GetRestitution()
		{
			return m_restitution;
		}

		public void SetFriction(float frict)
		{
			m_friction = frict;
		}

		public virtual float GetFriction()
		{
			return m_friction;
		}

		public CollisionObjectTypes GetInternalType()
		{
			return m_internalType;
		}

		public void SetInternalType(CollisionObjectTypes types)
		{
			m_internalType = types;
		}

		public void GetWorldTransform(out IndexedMatrix m)
		{
			m = m_worldTransform;
		}

		public IndexedMatrix GetWorldTransform()
		{
			return m_worldTransform;
		}

		public void SetWorldTransform(IndexedMatrix worldTrans)
		{
			m_worldTransform = worldTrans;
		}

		public void SetWorldTransform(ref IndexedMatrix worldTrans)
		{
			m_worldTransform = worldTrans;
		}

		public BroadphaseProxy GetBroadphaseHandle()
		{
			return m_broadphaseHandle;
		}

		public void SetBroadphaseHandle(BroadphaseProxy handle)
		{
			m_broadphaseHandle = handle;
		}

		public IndexedMatrix GetInterpolationWorldTransform()
		{
			return m_interpolationWorldTransform;
		}

		public void SetInterpolationWorldTransform(ref IndexedMatrix trans)
		{
			m_interpolationWorldTransform = trans;
		}

		public void SetInterpolationLinearVelocity(ref IndexedVector3 linvel)
		{
			m_interpolationLinearVelocity = linvel;
		}

		public void SetInterpolationAngularVelocity(ref IndexedVector3 angvel)
		{
			m_interpolationAngularVelocity = angvel;
		}

		public IndexedVector3 SetInterpolationLinearVelocity()
		{
			return m_interpolationLinearVelocity;
		}

		public IndexedVector3 GetInterpolationAngularVelocity()
		{
			return m_interpolationAngularVelocity;
		}

		public IndexedVector3 GetInterpolationLinearVelocity()
		{
			return m_interpolationLinearVelocity;
		}

		public int GetIslandTag()
		{
			return m_islandTag1;
		}

		public void SetIslandTag(int tag)
		{
			m_islandTag1 = tag;
		}

		public int GetCompanionId()
		{
			return m_companionId;
		}

		public void SetCompanionId(int id)
		{
			m_companionId = id;
		}

		public float GetHitFraction()
		{
			return m_hitFraction;
		}

		public void SetHitFraction(float hitFraction)
		{
			m_hitFraction = hitFraction;
		}

		public CollisionFlags GetCollisionFlags()
		{
			return m_collisionFlags;
		}

		public void SetCollisionFlags(CollisionFlags flags)
		{
			m_collisionFlags = flags;
		}

		public float GetCcdSweptSphereRadius()
		{
			return m_ccdSweptSphereRadius;
		}

		public void SetCcdSweptSphereRadius(float radius)
		{
			m_ccdSweptSphereRadius = radius;
		}

		public float GetCcdMotionThreshold()
		{
			return m_ccdMotionThreshold;
		}

		public float GetCcdSquareMotionThreshold()
		{
			return m_ccdMotionThreshold * m_ccdMotionThreshold;
		}

		public void SetCcdMotionThreshold(float ccdMotionThreshold)
		{
			m_ccdMotionThreshold = ccdMotionThreshold;
		}

		public object GetUserPointer()
		{
			return m_userObjectPointer;
		}

		public void SetUserPointer(object userPointer)
		{
			m_userObjectPointer = userPointer;
		}

		public bool CheckCollideWith(CollisionObject co)
		{
			if (m_checkCollideWith)
			{
				return CheckCollideWithOverride(co);
			}
			return true;
		}

		public virtual void Cleanup()
		{
		}

		public void Translate(ref IndexedVector3 v)
		{
			m_worldTransform._origin += v;
		}

		public void Translate(IndexedVector3 v)
		{
			m_worldTransform._origin += v;
		}
	}
}
