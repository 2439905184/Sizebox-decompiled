using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TerrainTree : MonoBehaviour, IDestructible, IDamagable, IGameObject
{
	private bool fallen;

	public void Damage(float amount)
	{
	}

	public bool TryToDestroy(float destructionForce, Vector3 contactPoint, EntityBase entity = null)
	{
		if (fallen || destructionForce < base.transform.lossyScale.y * 6f)
		{
			return false;
		}
		fallen = true;
		Collider component = GetComponent<Collider>();
		if (component != null)
		{
			component.enabled = false;
		}
		StartCoroutine(FallCoroutine(contactPoint));
		return true;
	}

	public IEnumerator FallCoroutine(Vector3 contactPoint)
	{
		float y = Quaternion.LookRotation(base.transform.position - contactPoint).eulerAngles.y;
		Quaternion targetRotation = Quaternion.Euler(90f, y, 0f);
		while (base.transform.localRotation != targetRotation)
		{
			base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, targetRotation, 8f * Time.deltaTime);
			yield return null;
		}
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
