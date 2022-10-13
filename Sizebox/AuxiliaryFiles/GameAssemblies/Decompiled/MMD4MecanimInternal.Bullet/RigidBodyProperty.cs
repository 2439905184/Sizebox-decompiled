using System;

namespace MMD4MecanimInternal.Bullet;

[Serializable]
public class RigidBodyProperty
{
	public bool isKinematic = true;

	public bool isFreezed;

	public bool isAdditionalDamping = true;

	public float mass = 1f;

	public float linearDamping = 0.5f;

	public float angularDamping = 0.5f;

	public float restitution = 0.5f;

	public float friction = 0.5f;
}
