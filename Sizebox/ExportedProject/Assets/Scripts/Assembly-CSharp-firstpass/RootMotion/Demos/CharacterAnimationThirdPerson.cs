using UnityEngine;

namespace RootMotion.Demos
{
	[RequireComponent(typeof(Animator))]
	public class CharacterAnimationThirdPerson : CharacterAnimationBase
	{
		public CharacterThirdPerson characterController;

		[SerializeField]
		private float turnSensitivity = 0.2f;

		[SerializeField]
		private float turnSpeed = 5f;

		[SerializeField]
		private float runCycleLegOffset = 0.2f;

		[Range(0.1f, 3f)]
		[SerializeField]
		private float animSpeedMultiplier = 1f;

		protected Animator animator;

		private Vector3 lastForward;

		private const string groundedDirectional = "Grounded Directional";

		private const string groundedStrafe = "Grounded Strafe";

		private float deltaAngle;

		public override bool animationGrounded
		{
			get
			{
				if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded Directional"))
				{
					return animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded Strafe");
				}
				return true;
			}
		}

		protected override void Start()
		{
			base.Start();
			animator = GetComponent<Animator>();
			lastForward = base.transform.forward;
		}

		public override Vector3 GetPivotPoint()
		{
			return animator.pivotPosition;
		}

		protected virtual void Update()
		{
			if (Time.deltaTime != 0f)
			{
				animatePhysics = animator.updateMode == AnimatorUpdateMode.AnimatePhysics;
				if (characterController.animState.jump)
				{
					float value = (float)((Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(0).normalizedTime + runCycleLegOffset, 1f) < 0f) ? 1 : (-1)) * characterController.animState.moveDirection.z;
					animator.SetFloat("JumpLeg", value);
				}
				float num = 0f - GetAngleFromForward(lastForward) - deltaAngle;
				deltaAngle = 0f;
				lastForward = base.transform.forward;
				num *= turnSensitivity * 0.01f;
				num = Mathf.Clamp(num / Time.deltaTime, -1f, 1f);
				animator.SetFloat("Turn", Mathf.Lerp(animator.GetFloat("Turn"), num, Time.deltaTime * turnSpeed));
				animator.SetFloat("Forward", characterController.animState.moveDirection.z);
				animator.SetFloat("Right", characterController.animState.moveDirection.x);
				animator.SetBool("Crouch", characterController.animState.crouch);
				animator.SetBool("OnGround", characterController.animState.onGround);
				animator.SetBool("IsStrafing", characterController.animState.isStrafing);
				if (!characterController.animState.onGround)
				{
					animator.SetFloat("Jump", characterController.animState.yVelocity);
				}
				if (characterController.animState.onGround && characterController.animState.moveDirection.z > 0f)
				{
					animator.speed = animSpeedMultiplier;
				}
				else
				{
					animator.speed = 1f;
				}
			}
		}

		private void OnAnimatorMove()
		{
			Vector3 vector = animator.deltaRotation * Vector3.forward;
			deltaAngle += Mathf.Atan2(vector.x, vector.z) * 57.29578f;
			characterController.Move(animator.deltaPosition, animator.deltaRotation);
		}
	}
}
