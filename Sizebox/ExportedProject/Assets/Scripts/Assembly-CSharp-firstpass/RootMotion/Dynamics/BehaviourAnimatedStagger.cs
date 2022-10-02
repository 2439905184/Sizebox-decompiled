using System;
using System.Collections;
using UnityEngine;

namespace RootMotion.Dynamics
{
	[AddComponentMenu("Scripts/RootMotion.Dynamics/PuppetMaster/Behaviours/BehaviourAnimatedStagger")]
	public class BehaviourAnimatedStagger : BehaviourBase
	{
		[Serializable]
		public struct FallParams
		{
			public float startPinWeightMlp;

			public float startMuscleWeightMlp;

			public float losePinSpeed;
		}

		[Serializable]
		public struct FallParamsGroup
		{
			public Muscle.Group[] groups;

			public FallParams fallParams;
		}

		[Header("Master Properties")]
		public LayerMask groundLayers;

		public float animationBlendSpeed = 2f;

		public float animationMag = 5f;

		public float momentumMag = 0.1f;

		public float unbalancedMuscleWeightMlp = 0.05f;

		public float unbalancedMuscleDamperAdd = 1f;

		public bool dropProps;

		public float maxGetUpVelocity = 0.3f;

		public float minHipHeight = 0.3f;

		public SubBehaviourCOM centerOfMass;

		[Header("Muscle Group Properties")]
		public FallParams defaults;

		public FallParamsGroup[] groupOverrides;

		[Header("Events")]
		public PuppetEvent onUngrounded;

		public PuppetEvent onFallOver;

		public PuppetEvent onRest;

		[HideInInspector]
		public Vector3 moveVector;

		[HideInInspector]
		public bool isGrounded = true;

		[HideInInspector]
		public Vector3 forward;

		protected override void OnInitiate()
		{
			centerOfMass.Initiate(this, groundLayers);
		}

		protected override void OnActivate()
		{
			StartCoroutine(LoseBalance());
		}

		public override void OnReactivate()
		{
		}

		private IEnumerator LoseBalance()
		{
			Muscle[] muscles = puppetMaster.muscles;
			foreach (Muscle muscle in muscles)
			{
				FallParams fallParams = GetFallParams(muscle.props.group);
				muscle.state.pinWeightMlp = Mathf.Min(muscle.state.pinWeightMlp, fallParams.startPinWeightMlp);
				muscle.state.muscleWeightMlp = Mathf.Min(muscle.state.muscleWeightMlp, fallParams.startMuscleWeightMlp);
				muscle.state.muscleDamperAdd = 0f - puppetMaster.muscleDamper;
			}
			puppetMaster.internalCollisions = true;
			bool done = false;
			while (!done)
			{
				Vector3 b = Quaternion.Inverse(puppetMaster.targetRoot.rotation) * centerOfMass.direction * animationMag;
				moveVector = Vector3.Lerp(moveVector, b, Time.deltaTime * animationBlendSpeed);
				moveVector = Vector3.ClampMagnitude(moveVector, 2f);
				muscles = puppetMaster.muscles;
				foreach (Muscle muscle2 in muscles)
				{
					FallParams fallParams2 = GetFallParams(muscle2.props.group);
					muscle2.state.pinWeightMlp = Mathf.MoveTowards(muscle2.state.pinWeightMlp, 0f, Time.deltaTime * fallParams2.losePinSpeed);
					muscle2.state.mappingWeightMlp = Mathf.MoveTowards(muscle2.state.mappingWeightMlp, 1f, Time.deltaTime * animationBlendSpeed);
				}
				done = true;
				muscles = puppetMaster.muscles;
				foreach (Muscle muscle3 in muscles)
				{
					if (muscle3.state.pinWeightMlp > 0f || muscle3.state.mappingWeightMlp < 1f)
					{
						done = false;
						break;
					}
				}
				if (puppetMaster.muscles[0].rigidbody.position.y - puppetMaster.targetRoot.position.y < minHipHeight)
				{
					done = true;
				}
				yield return null;
			}
			if (dropProps)
			{
				RemoveMusclesOfGroup(Muscle.Group.Prop);
			}
			if (!isGrounded)
			{
				muscles = puppetMaster.muscles;
				foreach (Muscle obj in muscles)
				{
					obj.state.pinWeightMlp = 0f;
					obj.state.muscleWeightMlp = 1f;
				}
				onUngrounded.Trigger(puppetMaster);
				if (onUngrounded.switchBehaviour)
				{
					yield break;
				}
			}
			moveVector = Vector3.zero;
			puppetMaster.mappingWeight = 1f;
			muscles = puppetMaster.muscles;
			foreach (Muscle obj2 in muscles)
			{
				obj2.state.pinWeightMlp = 0f;
				obj2.state.muscleWeightMlp = unbalancedMuscleWeightMlp;
				obj2.state.muscleDamperAdd = unbalancedMuscleDamperAdd;
			}
			onFallOver.Trigger(puppetMaster);
			if (!onFallOver.switchBehaviour)
			{
				yield return new WaitForSeconds(1f);
				while (puppetMaster.muscles[0].rigidbody.velocity.magnitude > maxGetUpVelocity || !isGrounded)
				{
					yield return null;
				}
				onRest.Trigger(puppetMaster);
				bool switchBehaviour = onRest.switchBehaviour;
			}
		}

		private FallParams GetFallParams(Muscle.Group group)
		{
			FallParamsGroup[] array = groupOverrides;
			for (int i = 0; i < array.Length; i++)
			{
				FallParamsGroup fallParamsGroup = array[i];
				Muscle.Group[] groups = fallParamsGroup.groups;
				for (int j = 0; j < groups.Length; j++)
				{
					if (groups[j] == group)
					{
						return fallParamsGroup.fallParams;
					}
				}
			}
			return defaults;
		}
	}
}
