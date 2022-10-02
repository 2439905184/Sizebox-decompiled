using System;
using UnityEngine;

namespace RootMotion.Dynamics
{
	public static class PhysXTools
	{
		public static Vector3 GetCenterOfMass(Rigidbody[] rigidbodies)
		{
			Vector3 zero = Vector3.zero;
			float num = 0f;
			for (int i = 0; i < rigidbodies.Length; i++)
			{
				if (rigidbodies[i].gameObject.activeInHierarchy)
				{
					zero += rigidbodies[i].worldCenterOfMass * rigidbodies[i].mass;
					num += rigidbodies[i].mass;
				}
			}
			return zero / num;
		}

		public static Vector3 GetCenterOfMassVelocity(Rigidbody[] rigidbodies)
		{
			Vector3 zero = Vector3.zero;
			float num = 0f;
			for (int i = 0; i < rigidbodies.Length; i++)
			{
				if (rigidbodies[i].gameObject.activeInHierarchy)
				{
					zero += rigidbodies[i].velocity * rigidbodies[i].mass;
					num += rigidbodies[i].mass;
				}
			}
			return zero / num;
		}

		public static void DivByInertia(ref Vector3 v, Quaternion rotation, Vector3 inertiaTensor)
		{
			v = rotation * Div(Quaternion.Inverse(rotation) * v, inertiaTensor);
		}

		public static void ScaleByInertia(ref Vector3 v, Quaternion rotation, Vector3 inertiaTensor)
		{
			v = rotation * Vector3.Scale(Quaternion.Inverse(rotation) * v, inertiaTensor);
		}

		public static Vector3 GetFromToAcceleration(Vector3 fromV, Vector3 toV)
		{
			Quaternion quaternion = Quaternion.FromToRotation(fromV, toV);
			float angle = 0f;
			Vector3 axis = Vector3.zero;
			quaternion.ToAngleAxis(out angle, out axis);
			return angle * axis * ((float)Math.PI / 180f) / Time.fixedDeltaTime;
		}

		public static Vector3 GetAngularAcceleration(Quaternion fromR, Quaternion toR)
		{
			Vector3 vector = Vector3.Cross(fromR * Vector3.forward, toR * Vector3.forward);
			Vector3 vector2 = Vector3.Cross(fromR * Vector3.up, toR * Vector3.up);
			float num = Quaternion.Angle(fromR, toR);
			return Vector3.Normalize(vector + vector2) * num * ((float)Math.PI / 180f) / Time.fixedDeltaTime;
		}

		public static void AddFromToTorque(Rigidbody r, Quaternion toR, ForceMode forceMode)
		{
			Vector3 angularAcceleration = GetAngularAcceleration(r.rotation, toR);
			angularAcceleration -= r.angularVelocity;
			switch (forceMode)
			{
			case ForceMode.Acceleration:
				r.AddTorque(angularAcceleration / Time.fixedDeltaTime, forceMode);
				break;
			case ForceMode.Force:
			{
				Vector3 v2 = angularAcceleration / Time.fixedDeltaTime;
				ScaleByInertia(ref v2, r.rotation, r.inertiaTensor);
				r.AddTorque(v2, forceMode);
				break;
			}
			case ForceMode.Impulse:
			{
				Vector3 v = angularAcceleration;
				ScaleByInertia(ref v, r.rotation, r.inertiaTensor);
				r.AddTorque(v, forceMode);
				break;
			}
			case ForceMode.VelocityChange:
				r.AddTorque(angularAcceleration, forceMode);
				break;
			case (ForceMode)3:
			case (ForceMode)4:
				break;
			}
		}

		public static void AddFromToTorque(Rigidbody r, Vector3 fromV, Vector3 toV, ForceMode forceMode)
		{
			Vector3 fromToAcceleration = GetFromToAcceleration(fromV, toV);
			fromToAcceleration -= r.angularVelocity;
			switch (forceMode)
			{
			case ForceMode.Acceleration:
				r.AddTorque(fromToAcceleration / Time.fixedDeltaTime, forceMode);
				break;
			case ForceMode.Force:
			{
				Vector3 v2 = fromToAcceleration / Time.fixedDeltaTime;
				ScaleByInertia(ref v2, r.rotation, r.inertiaTensor);
				r.AddTorque(v2, forceMode);
				break;
			}
			case ForceMode.Impulse:
			{
				Vector3 v = fromToAcceleration;
				ScaleByInertia(ref v, r.rotation, r.inertiaTensor);
				r.AddTorque(v, forceMode);
				break;
			}
			case ForceMode.VelocityChange:
				r.AddTorque(fromToAcceleration, forceMode);
				break;
			case (ForceMode)3:
			case (ForceMode)4:
				break;
			}
		}

		public static void AddFromToForce(Rigidbody r, Vector3 fromV, Vector3 toV, ForceMode forceMode)
		{
			Vector3 linearAcceleration = GetLinearAcceleration(fromV, toV);
			linearAcceleration -= r.velocity;
			switch (forceMode)
			{
			case ForceMode.Acceleration:
				r.AddForce(linearAcceleration / Time.fixedDeltaTime, forceMode);
				break;
			case ForceMode.Force:
			{
				Vector3 force2 = linearAcceleration / Time.fixedDeltaTime;
				force2 *= r.mass;
				r.AddForce(force2, forceMode);
				break;
			}
			case ForceMode.Impulse:
			{
				Vector3 force = linearAcceleration;
				force *= r.mass;
				r.AddForce(force, forceMode);
				break;
			}
			case ForceMode.VelocityChange:
				r.AddForce(linearAcceleration, forceMode);
				break;
			case (ForceMode)3:
			case (ForceMode)4:
				break;
			}
		}

		public static Vector3 GetLinearAcceleration(Vector3 fromPoint, Vector3 toPoint)
		{
			return (toPoint - fromPoint) / Time.fixedDeltaTime;
		}

		public static Quaternion ToJointSpace(ConfigurableJoint joint)
		{
			Vector3 vector = Vector3.Cross(joint.axis, joint.secondaryAxis);
			Vector3 upwards = Vector3.Cross(vector, joint.axis);
			return Quaternion.LookRotation(vector, upwards);
		}

		public static Vector3 CalculateInertiaTensorCuboid(Vector3 size, float mass)
		{
			float num = size.x * size.x;
			float num2 = size.y * size.y;
			float num3 = size.z * size.z;
			float num4 = 1f / 12f * mass;
			return new Vector3(num4 * (num2 + num3), num4 * (num + num3), num4 * (num + num2));
		}

		public static Vector3 Div(Vector3 v, Vector3 v2)
		{
			return new Vector3(v.x / v2.x, v.y / v2.y, v.z / v2.z);
		}
	}
}
