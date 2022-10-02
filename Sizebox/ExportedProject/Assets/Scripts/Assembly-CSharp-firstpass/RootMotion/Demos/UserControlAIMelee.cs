using RootMotion.Dynamics;
using UnityEngine;

namespace RootMotion.Demos
{
	public class UserControlAIMelee : UserControlThirdPerson
	{
		public BehaviourPuppet targetPuppet;

		public float stoppingDistance = 0.5f;

		public float stoppingThreshold = 1.5f;

		private bool isAttacking;

		private float attackTimer;

		private Transform moveTarget
		{
			get
			{
				return targetPuppet.puppetMaster.muscles[0].joint.transform;
			}
		}

		protected override void Update()
		{
			float num = (walkByDefault ? 0.5f : 1f);
			Vector3 vector = moveTarget.position - base.transform.position;
			vector.y = 0f;
			float num2 = ((state.move != Vector3.zero) ? stoppingDistance : (stoppingDistance * stoppingThreshold));
			state.move = ((vector.magnitude > num2) ? (vector.normalized * num) : Vector3.zero);
			state.lookPos = moveTarget.position + base.transform.right * -0.2f;
			if (CanAttack())
			{
				attackTimer += Time.deltaTime;
			}
			else
			{
				attackTimer = 0f;
			}
			state.actionIndex = ((attackTimer > 0.5f) ? 1 : 0);
		}

		private bool CanAttack()
		{
			if (targetPuppet.state == BehaviourPuppet.State.Unpinned)
			{
				return false;
			}
			Vector3 vector = state.lookPos - base.transform.position;
			vector = Quaternion.Inverse(base.transform.rotation) * vector;
			if (Mathf.Atan2(vector.x, vector.z) * 57.29578f > 20f)
			{
				return false;
			}
			return state.move == Vector3.zero;
		}
	}
}
