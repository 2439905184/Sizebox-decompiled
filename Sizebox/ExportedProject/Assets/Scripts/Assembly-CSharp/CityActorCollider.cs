using UnityEngine;

internal class CityActorCollider : MonoBehaviour
{
	private CityPopulationManager _cityManager;

	private void OnTriggerEnter(Collider other)
	{
		Humanoid componentInParent = other.GetComponentInParent<Humanoid>();
		if ((bool)componentInParent)
		{
			_cityManager.RegisterActor(componentInParent);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		Humanoid component = other.GetComponent<Humanoid>();
		if ((bool)component)
		{
			_cityManager.UnregisterActor(component);
		}
	}

	public void RegisterCityHandler(CityPopulationManager popManager)
	{
		_cityManager = popManager;
	}
}
