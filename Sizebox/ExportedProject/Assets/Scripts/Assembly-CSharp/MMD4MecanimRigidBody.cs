using MMD4MecanimInternal.Bullet;
using UnityEngine;

public class MMD4MecanimRigidBody : MonoBehaviour
{
	public RigidBodyProperty bulletPhysicsRigidBodyProperty;

	private MMD4MecanimBulletPhysics.RigidBody _bulletPhysicsRigidBody;

	private void Start()
	{
		MMD4MecanimBulletPhysics instance = MMD4MecanimBulletPhysics.instance;
		if (instance != null)
		{
			_bulletPhysicsRigidBody = instance.CreateRigidBody(this);
		}
	}

	private void OnDestroy()
	{
		if (_bulletPhysicsRigidBody != null && !_bulletPhysicsRigidBody.isExpired)
		{
			MMD4MecanimBulletPhysics instance = MMD4MecanimBulletPhysics.instance;
			if (instance != null)
			{
				instance.DestroyRigidBody(_bulletPhysicsRigidBody);
			}
		}
		_bulletPhysicsRigidBody = null;
	}
}
