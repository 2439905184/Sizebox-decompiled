using System;
using UnityEngine;

namespace RootMotion.Dynamics
{
	[Serializable]
	public class SubBehaviourCOM : SubBehaviourBase
	{
		[Serializable]
		public enum Mode
		{
			FeetCentroid = 0,
			CenterOfPressure = 1
		}

		public Mode mode;

		public float velocityDamper = 1f;

		public float velocityLerpSpeed = 5f;

		public float velocityMax = 1f;

		public float centerOfPressureSpeed = 5f;

		public Vector3 offset;

		[HideInInspector]
		public bool[] groundContacts;

		[HideInInspector]
		public Vector3[] groundContactPoints;

		private LayerMask groundLayers;

		public Vector3 position { get; private set; }

		public Vector3 direction { get; private set; }

		public float angle { get; private set; }

		public Vector3 velocity { get; private set; }

		public Vector3 centerOfPressure { get; private set; }

		public Quaternion rotation { get; private set; }

		public Quaternion inverseRotation { get; private set; }

		public bool isGrounded { get; private set; }

		public float lastGroundedTime { get; private set; }

		public void Initiate(BehaviourBase behaviour, LayerMask groundLayers)
		{
			base.behaviour = behaviour;
			this.groundLayers = groundLayers;
			rotation = Quaternion.identity;
			groundContacts = new bool[behaviour.puppetMaster.muscles.Length];
			groundContactPoints = new Vector3[groundContacts.Length];
			behaviour.OnPreActivate = (BehaviourBase.BehaviourDelegate)Delegate.Combine(behaviour.OnPreActivate, new BehaviourBase.BehaviourDelegate(OnPreActivate));
			behaviour.OnPreLateUpdate = (BehaviourBase.BehaviourDelegate)Delegate.Combine(behaviour.OnPreLateUpdate, new BehaviourBase.BehaviourDelegate(OnPreLateUpdate));
			behaviour.OnPreDeactivate = (BehaviourBase.BehaviourDelegate)Delegate.Combine(behaviour.OnPreDeactivate, new BehaviourBase.BehaviourDelegate(OnPreDeactivate));
			behaviour.OnPreMuscleCollision = (BehaviourBase.CollisionDelegate)Delegate.Combine(behaviour.OnPreMuscleCollision, new BehaviourBase.CollisionDelegate(OnPreMuscleCollision));
			behaviour.OnPreMuscleCollisionExit = (BehaviourBase.CollisionDelegate)Delegate.Combine(behaviour.OnPreMuscleCollisionExit, new BehaviourBase.CollisionDelegate(OnPreMuscleCollisionExit));
			behaviour.OnHierarchyChanged = (BehaviourBase.BehaviourDelegate)Delegate.Combine(behaviour.OnHierarchyChanged, new BehaviourBase.BehaviourDelegate(OnHierarchyChanged));
		}

		private void OnHierarchyChanged()
		{
			Array.Resize(ref groundContacts, behaviour.puppetMaster.muscles.Length);
			Array.Resize(ref groundContactPoints, behaviour.puppetMaster.muscles.Length);
		}

		private void OnPreMuscleCollision(MuscleCollision c)
		{
			if (LayerMaskExtensions.Contains(groundLayers, c.collision.gameObject.layer) && c.collision.contacts.Length != 0)
			{
				lastGroundedTime = Time.time;
				groundContacts[c.muscleIndex] = true;
				if (mode == Mode.CenterOfPressure)
				{
					groundContactPoints[c.muscleIndex] = GetCollisionCOP(c.collision);
				}
			}
		}

		private void OnPreMuscleCollisionExit(MuscleCollision c)
		{
			if (LayerMaskExtensions.Contains(groundLayers, c.collision.gameObject.layer))
			{
				groundContacts[c.muscleIndex] = false;
				groundContactPoints[c.muscleIndex] = Vector3.zero;
			}
		}

		private void OnPreActivate()
		{
			position = GetCenterOfMass();
			centerOfPressure = GetFeetCentroid();
			direction = position - centerOfPressure;
			angle = Vector3.Angle(direction, Vector3.up);
			velocity = Vector3.zero;
		}

		private void OnPreLateUpdate()
		{
			isGrounded = IsGrounded();
			if (mode == Mode.FeetCentroid || !isGrounded)
			{
				centerOfPressure = GetFeetCentroid();
			}
			else
			{
				Vector3 vector = (isGrounded ? GetCenterOfPressure() : GetFeetCentroid());
				centerOfPressure = ((centerOfPressureSpeed <= 2f) ? vector : Vector3.Lerp(centerOfPressure, vector, Time.deltaTime * centerOfPressureSpeed));
			}
			position = GetCenterOfMass();
			Vector3 vector2 = GetCenterOfMassVelocity() - position;
			vector2.y = 0f;
			vector2 = Vector3.ClampMagnitude(vector2, velocityMax);
			velocity = ((velocityLerpSpeed <= 0f) ? vector2 : Vector3.Lerp(velocity, vector2, Time.deltaTime * velocityLerpSpeed));
			position += velocity * velocityDamper;
			position += behaviour.puppetMaster.targetRoot.rotation * offset;
			direction = position - centerOfPressure;
			rotation = Quaternion.FromToRotation(Vector3.up, direction);
			inverseRotation = Quaternion.Inverse(rotation);
			angle = Quaternion.Angle(Quaternion.identity, rotation);
		}

		private void OnPreDeactivate()
		{
			velocity = Vector3.zero;
		}

		private Vector3 GetCollisionCOP(Collision collision)
		{
			Vector3 zero = Vector3.zero;
			for (int i = 0; i < collision.contacts.Length; i++)
			{
				zero += collision.contacts[i].point;
			}
			return zero / collision.contacts.Length;
		}

		private bool IsGrounded()
		{
			for (int i = 0; i < groundContacts.Length; i++)
			{
				if (groundContacts[i])
				{
					return true;
				}
			}
			return false;
		}

		private Vector3 GetCenterOfMass()
		{
			Vector3 zero = Vector3.zero;
			float num = 0f;
			Muscle[] muscles = behaviour.puppetMaster.muscles;
			foreach (Muscle muscle in muscles)
			{
				zero += muscle.rigidbody.worldCenterOfMass * muscle.rigidbody.mass;
				num += muscle.rigidbody.mass;
			}
			return zero /= num;
		}

		private Vector3 GetCenterOfMassVelocity()
		{
			Vector3 zero = Vector3.zero;
			float num = 0f;
			Muscle[] muscles = behaviour.puppetMaster.muscles;
			foreach (Muscle muscle in muscles)
			{
				zero += muscle.rigidbody.worldCenterOfMass * muscle.rigidbody.mass;
				zero += muscle.rigidbody.velocity * muscle.rigidbody.mass;
				num += muscle.rigidbody.mass;
			}
			return zero /= num;
		}

		private Vector3 GetMomentum()
		{
			Vector3 zero = Vector3.zero;
			for (int i = 0; i < behaviour.puppetMaster.muscles.Length; i++)
			{
				zero += behaviour.puppetMaster.muscles[i].rigidbody.velocity * behaviour.puppetMaster.muscles[i].rigidbody.mass;
			}
			return zero;
		}

		private Vector3 GetCenterOfPressure()
		{
			Vector3 zero = Vector3.zero;
			int num = 0;
			for (int i = 0; i < groundContacts.Length; i++)
			{
				if (groundContacts[i])
				{
					zero += groundContactPoints[i];
					num++;
				}
			}
			return zero / num;
		}

		private Vector3 GetFeetCentroid()
		{
			Vector3 zero = Vector3.zero;
			int num = 0;
			for (int i = 0; i < behaviour.puppetMaster.muscles.Length; i++)
			{
				if (behaviour.puppetMaster.muscles[i].props.group == Muscle.Group.Foot)
				{
					zero += behaviour.puppetMaster.muscles[i].rigidbody.worldCenterOfMass;
					num++;
				}
			}
			return zero / num;
		}
	}
}
