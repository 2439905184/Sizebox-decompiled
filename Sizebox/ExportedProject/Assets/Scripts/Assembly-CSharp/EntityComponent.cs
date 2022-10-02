using UnityEngine;

public abstract class EntityComponent : MonoBehaviour
{
	public bool initialized { get; protected set; }

	public abstract void Initialize(EntityBase entity);
}
