using UnityEngine;

namespace RootMotion.Dynamics
{
	[AddComponentMenu("Scripts/RootMotion.Dynamics/PuppetMaster/Behaviours/BehaviourTemplate")]
	public class BehaviourTemplate : BehaviourBase
	{
		public SubBehaviourCOM centerOfMass;

		public LayerMask groundLayers;

		public PuppetEvent onLoseBalance;

		public float loseBalanceAngle = 60f;

		protected override void OnInitiate()
		{
			centerOfMass.Initiate(this, groundLayers);
		}

		protected override void OnActivate()
		{
		}

		public override void OnReactivate()
		{
		}

		protected override void OnDeactivate()
		{
		}

		protected override void OnFixedUpdate()
		{
			if (centerOfMass.angle > loseBalanceAngle)
			{
				onLoseBalance.Trigger(puppetMaster);
			}
		}

		protected override void OnLateUpdate()
		{
		}

		protected override void OnMuscleHitBehaviour(MuscleHit hit)
		{
			bool enabled2 = base.enabled;
		}

		protected override void OnMuscleCollisionBehaviour(MuscleCollision m)
		{
			bool enabled2 = base.enabled;
		}
	}
}
