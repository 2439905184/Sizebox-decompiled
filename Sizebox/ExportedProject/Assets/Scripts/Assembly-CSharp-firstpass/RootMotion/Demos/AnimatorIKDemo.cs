using UnityEngine;

namespace RootMotion.Demos
{
	[RequireComponent(typeof(Animator))]
	public class AnimatorIKDemo : MonoBehaviour
	{
		public Transform leftHandIKTarget;

		private Animator animator;

		private void Start()
		{
			animator = GetComponent<Animator>();
		}

		private void OnAnimatorIK(int layer)
		{
			animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandIKTarget.position);
			animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
		}
	}
}
