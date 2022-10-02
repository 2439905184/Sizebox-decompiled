using RootMotion.Dynamics;
using UnityEngine;

namespace RootMotion.Demos
{
	public class CharacterPuppet : CharacterThirdPerson
	{
		[Header("Puppet")]
		public PropRoot propRoot;

		public BehaviourPuppet puppet { get; private set; }

		protected override void Start()
		{
			base.Start();
			puppet = base.transform.parent.GetComponentInChildren<BehaviourPuppet>();
		}

		public override void Move(Vector3 deltaPosition, Quaternion deltaRotation)
		{
			if (puppet.state != 0)
			{
				userControl.state.move = Vector3.zero;
			}
			else
			{
				base.Move(deltaPosition, deltaRotation);
			}
		}

		protected override void Rotate()
		{
			if (puppet.state != 0)
			{
				if (gravityTarget != null)
				{
					base.transform.rotation = Quaternion.FromToRotation(base.transform.up, base.transform.position - gravityTarget.position) * base.transform.rotation;
				}
			}
			else
			{
				base.Rotate();
			}
		}

		protected override bool Jump()
		{
			if (puppet.state != 0)
			{
				return false;
			}
			return base.Jump();
		}
	}
}
