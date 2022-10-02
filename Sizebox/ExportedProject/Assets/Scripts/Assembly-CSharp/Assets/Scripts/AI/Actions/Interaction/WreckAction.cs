using Lua;

namespace Assets.Scripts.AI.Actions.Interaction
{
	public class WreckAction : AgentAction
	{
		private bool mComplete;

		private readonly Animation mAnimationController;

		public WreckAction(Animation animationController)
		{
			mAnimationController = animationController;
		}

		public override void StartAction()
		{
			mAnimationController.SetAndWait("Stomping");
			mComplete = true;
		}

		public override bool IsCompleted()
		{
			return mComplete;
		}
	}
}
