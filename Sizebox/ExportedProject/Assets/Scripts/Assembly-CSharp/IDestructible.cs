using UnityEngine;

public interface IDestructible : IDamagable, IGameObject
{
	bool TryToDestroy(float destructionForce, Vector3 contactPoint, EntityBase entity = null);
}
