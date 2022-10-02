using System.Collections;
using RootMotion.Dynamics;
using UnityEngine;

namespace RootMotion.Demos
{
	public class Dying : MonoBehaviour
	{
		[Tooltip("Reference to the PuppetMaster component.")]
		public PuppetMaster puppetMaster;

		[Tooltip("The speed of fading out PuppetMaster.pinWeight.")]
		public float fadeOutPinWeightSpeed = 5f;

		[Tooltip("The speed of fading out PuppetMaster.muscleWeight.")]
		public float fadeOutMuscleWeightSpeed = 5f;

		[Tooltip("The muscle weight to fade out to.")]
		public float deadMuscleWeight = 0.3f;

		private Animator animator;

		private Vector3 defaultPosition;

		private Quaternion defaultRotation = Quaternion.identity;

		private bool isDead;

		private void Start()
		{
			animator = GetComponent<Animator>();
			defaultPosition = base.transform.position;
			defaultRotation = base.transform.rotation;
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.D) && !isDead)
			{
				animator.CrossFadeInFixedTime("Die Backwards", 0.2f);
				if (puppetMaster != null)
				{
					StopAllCoroutines();
					StartCoroutine(FadeOutPinWeight());
					StartCoroutine(FadeOutMuscleWeight());
				}
				isDead = true;
			}
			if (Input.GetKeyDown(KeyCode.R) && isDead)
			{
				base.transform.position = defaultPosition;
				base.transform.rotation = defaultRotation;
				animator.Play("Idle", 0, 0f);
				if (puppetMaster != null)
				{
					StopAllCoroutines();
					puppetMaster.pinWeight = 1f;
					puppetMaster.muscleWeight = 1f;
				}
				isDead = false;
			}
		}

		private IEnumerator FadeOutPinWeight()
		{
			while (puppetMaster.pinWeight > 0f)
			{
				puppetMaster.pinWeight = Mathf.MoveTowards(puppetMaster.pinWeight, 0f, Time.deltaTime * fadeOutPinWeightSpeed);
				yield return null;
			}
		}

		private IEnumerator FadeOutMuscleWeight()
		{
			while (puppetMaster.muscleWeight > 0f)
			{
				puppetMaster.muscleWeight = Mathf.MoveTowards(puppetMaster.muscleWeight, deadMuscleWeight, Time.deltaTime * fadeOutMuscleWeightSpeed);
				yield return null;
			}
		}
	}
}
