using UnityEngine;

public interface IAnimated : IEntity, IGameObject
{
	Animator Animator { get; }

	GiantessIK Ik { get; }

	RuntimeAnimatorController DefaultAnimatorController { get; }
}
