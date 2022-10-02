namespace Assets.Scripts.ProceduralCityGenerator
{
	internal class DestructionStatus
	{
		private FracturedObject _chunks;

		private bool _animationCalled;

		public DestructionStatus(FracturedObject chunks)
		{
			_chunks = chunks;
		}

		public bool IsBroken()
		{
			if (_chunks != null)
			{
				return _chunks.enabled;
			}
			return _animationCalled;
		}

		public void AnimationCalled()
		{
			_animationCalled = true;
		}
	}
}
