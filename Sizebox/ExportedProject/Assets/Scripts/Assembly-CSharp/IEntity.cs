using UnityEngine;

public interface IEntity : IGameObject
{
	EntityBase Entity { get; }

	Rigidbody Rigidbody { get; }

	float Scale { get; }

	float AccurateScale { get; }
}
