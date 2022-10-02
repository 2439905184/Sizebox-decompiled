using System.Runtime.CompilerServices;
using UnityEngine;

public class AssetDestructible : MonoBehaviour, ICrushable, IDamagable, IGameObject, IDestructible
{
	private EntityBase entity;

	private CustomDestructible customDestructible;

	private float referenceScale = 1f;

	public void Damage(float amount)
	{
	}

	public bool TryToCrush(float mass, Vector3 velocity, Collision collisionData = null, EntityBase crusher = null, Collider crushingCollider = null)
	{
		if (Mathf.Pow(mass, 1f / 3f) > customDestructible.minimumDestructionHeight * getScale())
		{
			doDestroy();
			return true;
		}
		return false;
	}

	public bool TryToCrush(Vector3 force, EntityBase crusher = null)
	{
		if (customDestructible.destructibleType == CustomDestructible.DestructibleType.Dynamic)
		{
			return false;
		}
		if (Mathf.Pow(force.magnitude, 0.25f) > customDestructible.minimumDestructionHeight * getScale())
		{
			doDestroy();
			return true;
		}
		return false;
	}

	public bool TryToDestroy(float destructionForce, Vector3 contactPoint, EntityBase entity = null)
	{
		if (customDestructible.destructibleType == CustomDestructible.DestructibleType.Dynamic)
		{
			return false;
		}
		if (destructionForce / 2000f > customDestructible.minimumDestructionHeight * getScale())
		{
			doDestroy();
			return true;
		}
		return false;
	}

	protected void doDestroy()
	{
		if (entity.model.activeInHierarchy)
		{
			entity.model.SetActive(false);
			GameObject go = Object.Instantiate(customDestructible.destroyedPrefab, base.transform);
			if (customDestructible.destructibleType == CustomDestructible.DestructibleType.Dynamic)
			{
				go.SetLayerRecursively(base.gameObject.layer);
			}
			else
			{
				go.SetLayerRecursively(Layers.debrisSimpleLayer);
			}
			if (customDestructible.destroyedTimeToLiveSeconds > 0f)
			{
				Invoke("destroyAfterTtl", customDestructible.destroyedTimeToLiveSeconds);
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	public void destroyAfterTtl()
	{
		entity.DestroyObject();
	}

	protected float getScale()
	{
		return base.transform.lossyScale.y / GameController.ReferenceScale;
	}

	private void Start()
	{
		entity = GetComponent<EntityBase>();
		customDestructible = entity.model.GetComponent<CustomDestructible>();
	}

	[SpecialName]
	GameObject IGameObject.get_gameObject()
	{
		return base.gameObject;
	}

	[SpecialName]
	Transform IGameObject.get_transform()
	{
		return base.transform;
	}
}
