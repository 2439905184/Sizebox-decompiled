using BulletXNA.BulletCollision;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletDynamics
{
	public class RigidBodyConstructionInfo
	{
		public float m_mass;

		public IMotionState m_motionState;

		public IndexedMatrix m_startWorldTransform;

		public CollisionShape m_collisionShape;

		public IndexedVector3 m_localInertia;

		public float m_linearDamping;

		public float m_angularDamping;

		public float m_friction;

		public float m_restitution;

		public float m_linearSleepingThreshold;

		public float m_angularSleepingThreshold;

		public bool m_additionalDamping;

		public float m_additionalDampingFactor;

		public float m_additionalLinearDampingThresholdSqr;

		public float m_additionalAngularDampingThresholdSqr;

		public float m_additionalAngularDampingFactor;

		public RigidBodyConstructionInfo(float mass, IMotionState motionState, CollisionShape collisionShape)
			: this(mass, motionState, collisionShape, new IndexedVector3(0f))
		{
		}

		public RigidBodyConstructionInfo(float mass, IMotionState motionState, CollisionShape collisionShape, IndexedVector3 localInertia)
		{
			m_mass = mass;
			m_motionState = motionState;
			m_collisionShape = collisionShape;
			m_localInertia = localInertia;
			m_linearDamping = 0f;
			m_angularDamping = 0f;
			m_friction = 0.5f;
			m_restitution = 0f;
			m_linearSleepingThreshold = 0.8f;
			m_angularSleepingThreshold = 1f;
			m_additionalDamping = false;
			m_additionalDampingFactor = 0.005f;
			m_additionalLinearDampingThresholdSqr = 0.01f;
			m_additionalAngularDampingThresholdSqr = 0.01f;
			m_additionalAngularDampingFactor = 0.01f;
			m_startWorldTransform = IndexedMatrix.Identity;
		}
	}
}
