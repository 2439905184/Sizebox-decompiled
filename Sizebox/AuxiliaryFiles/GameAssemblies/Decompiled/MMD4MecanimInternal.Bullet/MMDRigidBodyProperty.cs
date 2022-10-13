using System;
using UnityEngine;

namespace MMD4MecanimInternal.Bullet;

[Serializable]
public class MMDRigidBodyProperty
{
	public int isFreezed = -1;

	public int isUseForceAngularVelocityLimit = -1;

	public int isUseForceAngularAccelerationLimit = -1;

	public float forceAngularVelocityLimit = -1f;

	public float linearVelocityLimit = -1f;

	public float angularVelocityLimit = -1f;

	public float shapeScale = -1f;

	public Vector3 shapeSize = Vector3.zero;

	public float linearDamping = -1f;

	public float angularDamping = -1f;

	public float restitution = -1f;

	public float friction = -1f;

	public void Copy(MMDRigidBodyProperty rhs)
	{
		if (rhs != null)
		{
			if (rhs.isFreezed != -1)
			{
				isFreezed = rhs.isFreezed;
			}
			if (rhs.isUseForceAngularVelocityLimit != -1)
			{
				isUseForceAngularVelocityLimit = rhs.isUseForceAngularVelocityLimit;
			}
			if (rhs.isUseForceAngularAccelerationLimit != -1)
			{
				isUseForceAngularAccelerationLimit = rhs.isUseForceAngularAccelerationLimit;
			}
			if (rhs.forceAngularVelocityLimit >= 0f)
			{
				forceAngularVelocityLimit = rhs.forceAngularVelocityLimit;
			}
			if (rhs.linearVelocityLimit >= 0f)
			{
				linearVelocityLimit = rhs.linearVelocityLimit;
			}
			if (rhs.angularVelocityLimit >= 0f)
			{
				angularVelocityLimit = rhs.angularVelocityLimit;
			}
			if (rhs.shapeScale >= 0f)
			{
				shapeScale = rhs.shapeScale;
			}
			if (rhs.shapeSize != Vector3.zero)
			{
				shapeSize = rhs.shapeSize;
			}
			if (rhs.linearDamping >= 0f)
			{
				linearDamping = rhs.linearDamping;
			}
			if (rhs.angularDamping >= 0f)
			{
				angularDamping = rhs.angularDamping;
			}
			if (rhs.restitution >= 0f)
			{
				restitution = rhs.restitution;
			}
			if (rhs.friction >= 0f)
			{
				friction = rhs.friction;
			}
		}
	}
}
