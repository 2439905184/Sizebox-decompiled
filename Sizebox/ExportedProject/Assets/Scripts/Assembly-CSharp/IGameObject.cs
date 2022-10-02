using UnityEngine;

public interface IGameObject
{
	GameObject gameObject { get; }

	Transform transform { get; }
}
