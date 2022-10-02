using UnityEngine;

namespace RootMotion.Demos
{
	public class UserControlMelee : UserControlThirdPerson
	{
		public KeyCode hitKey;

		protected override void Update()
		{
			base.Update();
			state.actionIndex = (Input.GetKey(hitKey) ? 1 : 0);
		}
	}
}
