namespace RootMotion.Demos
{
	public class CharacterAnimationMeleeDemo : CharacterAnimationThirdPerson
	{
		private CharacterMeleeDemo melee
		{
			get
			{
				return characterController as CharacterMeleeDemo;
			}
		}

		protected override void Update()
		{
			base.Update();
			animator.SetInteger("ActionIndex", -1);
			if (melee.currentAction != null)
			{
				animator.SetInteger("ActionIndex", melee.currentActionIndex);
				CharacterMeleeDemo.Action.Anim anim = melee.currentAction.anim;
				animator.CrossFadeInFixedTime(anim.stateName, anim.transitionDuration, anim.layer, anim.fixedTime);
				melee.currentActionIndex = -1;
			}
		}
	}
}
