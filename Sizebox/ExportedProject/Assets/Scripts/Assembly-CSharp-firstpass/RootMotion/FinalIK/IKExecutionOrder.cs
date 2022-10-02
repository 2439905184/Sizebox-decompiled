using UnityEngine;

namespace RootMotion.FinalIK
{
	public class IKExecutionOrder : MonoBehaviour
	{
		[Tooltip("The IK components, assign in the order in which you wish to update them.")]
		public IK[] IKComponents;

		[Tooltip("Optional. Assign it if you are using 'Animate Physics' as the Update Mode.")]
		public Animator animator;

		private bool fixedFrame;

		private bool animatePhysics
		{
			get
			{
				if (animator == null)
				{
					return false;
				}
				return animator.updateMode == AnimatorUpdateMode.AnimatePhysics;
			}
		}

		private void Start()
		{
			for (int i = 0; i < IKComponents.Length; i++)
			{
				IKComponents[i].enabled = false;
			}
		}

		private void Update()
		{
			if (!animatePhysics)
			{
				FixTransforms();
			}
		}

		private void FixedUpdate()
		{
			fixedFrame = true;
			if (animatePhysics)
			{
				FixTransforms();
			}
		}

		private void LateUpdate()
		{
			if (!animatePhysics || fixedFrame)
			{
				for (int i = 0; i < IKComponents.Length; i++)
				{
					IKComponents[i].GetIKSolver().Update();
				}
				fixedFrame = false;
			}
		}

		private void FixTransforms()
		{
			for (int i = 0; i < IKComponents.Length; i++)
			{
				if (IKComponents[i].fixTransforms)
				{
					IKComponents[i].GetIKSolver().FixTransforms();
				}
			}
		}
	}
}
