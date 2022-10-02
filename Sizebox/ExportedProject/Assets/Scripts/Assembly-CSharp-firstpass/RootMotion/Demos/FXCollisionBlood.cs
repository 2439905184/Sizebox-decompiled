using System;
using RootMotion.Dynamics;
using UnityEngine;

namespace RootMotion.Demos
{
	[RequireComponent(typeof(ParticleSystem))]
	public class FXCollisionBlood : MonoBehaviour
	{
		public BehaviourPuppet puppet;

		public float minCollisionImpulse = 100f;

		public int emission = 2;

		public float emissionImpulseAdd = 0.01f;

		public int maxEmission = 7;

		private ParticleSystem particles;

		private void Start()
		{
			particles = GetComponent<ParticleSystem>();
			BehaviourPuppet behaviourPuppet = puppet;
			behaviourPuppet.OnCollisionImpulse = (BehaviourPuppet.CollisionImpulseDelegate)Delegate.Combine(behaviourPuppet.OnCollisionImpulse, new BehaviourPuppet.CollisionImpulseDelegate(OnCollisionImpulse));
		}

		private void OnCollisionImpulse(MuscleCollision m, float impulse)
		{
			if (m.collision.contacts.Length != 0 && !(impulse < minCollisionImpulse))
			{
				base.transform.position = m.collision.contacts[0].point;
				base.transform.rotation = Quaternion.LookRotation(m.collision.contacts[0].normal);
				particles.Emit(Mathf.Min(emission + (int)(emissionImpulseAdd * impulse), maxEmission));
			}
		}

		private void OnDestroy()
		{
			if (puppet != null)
			{
				BehaviourPuppet behaviourPuppet = puppet;
				behaviourPuppet.OnCollisionImpulse = (BehaviourPuppet.CollisionImpulseDelegate)Delegate.Remove(behaviourPuppet.OnCollisionImpulse, new BehaviourPuppet.CollisionImpulseDelegate(OnCollisionImpulse));
			}
		}
	}
}
