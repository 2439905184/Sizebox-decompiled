using BulletXNA.BulletDynamics;

namespace MMD4MecanimInternal.Bullet
{
	public class PhysicsEntity
	{
		public PhysicsWorld _physicsWorld;

		public bool _isUpdateAtLeastOnce;

		public PhysicsWorld physicsWorld
		{
			get
			{
				return _physicsWorld;
			}
		}

		public DiscreteDynamicsWorld bulletWorld
		{
			get
			{
				if (_physicsWorld != null)
				{
					return _physicsWorld.bulletWorld;
				}
				return null;
			}
		}

		public void LeaveWorld()
		{
			_LeaveWorld();
			if (_physicsWorld != null)
			{
				_physicsWorld.WaitEndThreading();
				_physicsWorld._RemoveEntity(this);
				_physicsWorld = null;
				_isUpdateAtLeastOnce = false;
			}
		}

		public virtual bool _JoinWorld()
		{
			return false;
		}

		public virtual void _LeaveWorld()
		{
		}

		public virtual void _PreUpdate()
		{
		}

		public virtual void _PreUpdateWorld(float deltaTime)
		{
		}

		public virtual void _PostUpdateWorld(float deltaTime)
		{
		}

		public virtual void _NoUpdateWorld()
		{
		}

		public virtual void _PostUpdate()
		{
		}

		public virtual void _PrepareLateUpdate()
		{
		}

		public virtual float _GetResetWorldTime()
		{
			return 0f;
		}

		public virtual void _PreResetWorld()
		{
		}

		public virtual void _StepResetWorld(float elapsedTime)
		{
		}

		public virtual void _PostResetWorld()
		{
		}
	}
}
