using UnityEngine;

public interface ICrushable : IDamagable, IGameObject
{
	bool TryToCrush(float mass, Vector3 velocity, Collision collisionData = null, EntityBase crusher = null, Collider crushingCollider = null);

	bool TryToCrush(Vector3 force, EntityBase crusher = null);
}
