using System;
using UnityEngine.Serialization;

namespace SaveDataStructures
{
	[Serializable]
	public class AnimationData
	{
		public string name;

		public bool isPose;

		public float speed;

		public float time;

		[FormerlySerializedAs("dontMoveGts")]
		public bool doNotMoveGts;

		public AnimationData(Humanoid humanoid)
		{
			name = humanoid.animationManager.nameAnimation;
			isPose = humanoid.IsPosed;
			speed = humanoid.animationManager.speedMultiplier;
			time = humanoid.animationManager.GetAnimationTime();
			if (humanoid.isGiantess)
			{
				GTSMovement gtsMovement = humanoid.GetComponentInChildren<Giantess>().gtsMovement;
				doNotMoveGts = gtsMovement.doNotMoveGts;
			}
		}
	}
}
