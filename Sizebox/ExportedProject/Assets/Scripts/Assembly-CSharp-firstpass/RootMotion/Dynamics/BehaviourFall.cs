using System.Collections;
using UnityEngine;

namespace RootMotion.Dynamics
{
	[HelpURL("http://root-motion.com/puppetmasterdox/html/page11.html")]
	[AddComponentMenu("Scripts/RootMotion.Dynamics/PuppetMaster/Behaviours/BehaviourFall")]
	public class BehaviourFall : BehaviourBase
	{
		[LargeHeader("Animation State")]
		[Tooltip("Animation State to crossfade to when this behaviour is activated.")]
		public string stateName = "Falling";

		[Tooltip("The duration of crossfading to 'State Name'. Value is in seconds.")]
		public float transitionDuration = 0.4f;

		[Tooltip("Layer index containing the destination state. If no layer is specified or layer is -1, the first state that is found with the given name or hash will be played.")]
		public int layer;

		[Tooltip("Start time of the current destination state. Value is in seconds. If no explicit fixedTime is specified or fixedTime value is float.NegativeInfinity, the state will either be played from the start if it's not already playing, or will continue playing from its current time and no transition will happen.")]
		public float fixedTime;

		[LargeHeader("Blending")]
		[Tooltip("The layers that will be raycasted against to find colliding objects.")]
		public LayerMask raycastLayers;

		[Tooltip("The parameter in the Animator that blends between catch fall and writhe animations.")]
		public string blendParameter = "FallBlend";

		[Tooltip("The height of the pelvis from the ground at which will blend to writhe animation.")]
		public float writheHeight = 4f;

		[Tooltip("The vertical velocity of the pelvis at which will blend to writhe animation.")]
		public float writheYVelocity = 1f;

		[Tooltip("The speed of blendig between the two falling animations.")]
		public float blendSpeed = 3f;

		[Tooltip("The speed of blending in mapping on activation.")]
		public float blendMappingSpeed = 1f;

		[LargeHeader("Ending")]
		[Tooltip("If false, this behaviour will never end.")]
		public bool canEnd;

		[Tooltip("The minimum time since this behaviour activated before it can end.")]
		public float minTime = 1.5f;

		[Tooltip("If the velocity of the pelvis falls below this value, can end the behaviour.")]
		public float maxEndVelocity = 0.5f;

		[Tooltip("Event triggered when all end conditions are met.")]
		public PuppetEvent onEnd;

		private float timer;

		private bool endTriggered;

		[ContextMenu("User Manual")]
		private void OpenUserManual()
		{
			Application.OpenURL("http://root-motion.com/puppetmasterdox/html/page11.html");
		}

		[ContextMenu("Scrpt Reference")]
		private void OpenScriptReference()
		{
			Application.OpenURL("http://root-motion.com/puppetmasterdox/html/class_root_motion_1_1_dynamics_1_1_behaviour_fall.html");
		}

		protected override void OnActivate()
		{
			base.forceActive = true;
			StopAllCoroutines();
			StartCoroutine(SmoothActivate());
		}

		protected override void OnDeactivate()
		{
			base.forceActive = false;
		}

		public override void OnReactivate()
		{
			timer = 0f;
			endTriggered = false;
		}

		private IEnumerator SmoothActivate()
		{
			timer = 0f;
			endTriggered = false;
			puppetMaster.targetAnimator.CrossFadeInFixedTime(stateName, transitionDuration, layer, fixedTime);
			Muscle[] muscles = puppetMaster.muscles;
			foreach (Muscle muscle in muscles)
			{
				muscle.state.pinWeightMlp = 0f;
				muscle.rigidbody.velocity = muscle.mappedVelocity;
				muscle.rigidbody.angularVelocity = muscle.mappedAngularVelocity;
			}
			float fader = 0f;
			while (fader < 1f)
			{
				fader += Time.deltaTime;
				muscles = puppetMaster.muscles;
				foreach (Muscle obj in muscles)
				{
					obj.state.pinWeightMlp -= Time.deltaTime;
					obj.state.mappingWeightMlp += Time.deltaTime * blendMappingSpeed;
				}
				yield return null;
			}
		}

		protected override void OnFixedUpdate()
		{
			if ((int)raycastLayers == -1)
			{
				Debug.LogWarning("BehaviourFall has no layers to raycast to.", base.transform);
			}
			float blendTarget = GetBlendTarget(GetGroundHeight());
			float value = Mathf.MoveTowards(puppetMaster.targetAnimator.GetFloat(blendParameter), blendTarget, Time.deltaTime * blendSpeed);
			puppetMaster.targetAnimator.SetFloat(blendParameter, value);
			timer += Time.deltaTime;
			if (!endTriggered && canEnd && timer >= minTime && !puppetMaster.isBlending && puppetMaster.muscles[0].rigidbody.velocity.magnitude < maxEndVelocity)
			{
				endTriggered = true;
				onEnd.Trigger(puppetMaster);
			}
		}

		protected override void OnLateUpdate()
		{
			puppetMaster.targetRoot.position += puppetMaster.muscles[0].transform.position - puppetMaster.muscles[0].target.position;
			GroundTarget(raycastLayers);
		}

		public override void Resurrect()
		{
			Muscle[] muscles = puppetMaster.muscles;
			for (int i = 0; i < muscles.Length; i++)
			{
				muscles[i].state.pinWeightMlp = 0f;
			}
		}

		private float GetBlendTarget(float groundHeight)
		{
			if (groundHeight > writheHeight)
			{
				return 1f;
			}
			Vector3 lhs = V3Tools.ExtractVertical(puppetMaster.muscles[0].rigidbody.velocity, puppetMaster.targetRoot.up, 1f);
			float num = lhs.magnitude;
			if (Vector3.Dot(lhs, puppetMaster.targetRoot.up) < 0f)
			{
				num = 0f - num;
			}
			if (num > writheYVelocity)
			{
				return 1f;
			}
			return 0f;
		}

		private float GetGroundHeight()
		{
			RaycastHit hitInfo = default(RaycastHit);
			if (Physics.Raycast(puppetMaster.muscles[0].rigidbody.position, -puppetMaster.targetRoot.up, out hitInfo, 100f, raycastLayers))
			{
				return hitInfo.distance;
			}
			return float.PositiveInfinity;
		}
	}
}
