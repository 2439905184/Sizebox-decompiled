using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Assets.Scripts.ProceduralCityGenerator;
using UnityEngine;

public class CityPopulationManager : MonoBehaviour
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Predicate<CityBuilding> _003C_003E9__18_0;

		internal bool _003CRescanBuildings_003Eb__18_0(CityBuilding x)
		{
			return x.broken;
		}
	}

	private Dictionary<Humanoid, ActorPopulationManager> _managers;

	private CityActorCollider _cityCollider;

	private IEnumerator _routine;

	public List<CityBuilding> Buildings { get; private set; }

	public float CityScale { get; private set; }

	public void SetParameters(Vector3 cityCenter, Vector3 citySize, GameObject buildingsRoot, float cityScale)
	{
		_managers = new Dictionary<Humanoid, ActorPopulationManager>();
		Buildings = new List<CityBuilding>();
		CityScale = cityScale;
		Debug.Log("Parameters are set for city auto population");
	}

	public void RemoveBuilding(CityBuilding building)
	{
		Buildings.Remove(building);
	}

	public void SetTriggerZone(GameObject zone)
	{
		_cityCollider = zone.GetComponent<CityActorCollider>();
		_cityCollider.RegisterCityHandler(this);
	}

	public void AddBuilding(CityBuilding building)
	{
		Buildings.Add(building);
	}

	public void PopStart()
	{
		_routine = UpdateCity();
		StartCoroutine(_routine);
	}

	public void PopStop()
	{
		StopCoroutine(_routine);
		_routine = null;
	}

	private IEnumerator UpdateCity()
	{
		while (true)
		{
			List<CityBuilding> buildings = Buildings;
			if (buildings == null || buildings.Count <= 0)
			{
				break;
			}
			RescanBuildings();
			yield return null;
			ActorPopulationManager[] actors = new ActorPopulationManager[_managers.Count];
			_managers.Values.CopyTo(actors, 0);
			yield return null;
			int pop = 0;
			foreach (ActorPopulationManager actorPopulationManager in actors)
			{
				if (actorPopulationManager != null)
				{
					pop += actorPopulationManager.ActiveMicros.Count;
					yield return null;
				}
			}
			foreach (ActorPopulationManager actorPopulationManager2 in actors)
			{
				if (actorPopulationManager2 != null)
				{
					yield return actorPopulationManager2.ActorUpdate(_managers.Count + 1, pop);
				}
			}
			yield return null;
		}
	}

	private void RescanBuildings()
	{
		Buildings.RemoveAll(_003C_003Ec._003C_003E9__18_0 ?? (_003C_003Ec._003C_003E9__18_0 = _003C_003Ec._003C_003E9._003CRescanBuildings_003Eb__18_0));
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
		foreach (ActorPopulationManager value in _managers.Values)
		{
			value.Destroy();
		}
		UnityEngine.Object.Destroy(_cityCollider);
	}

	public void RegisterActor(Humanoid actor)
	{
		if ((actor.isGiantess || actor.isPlayer) && !_managers.ContainsKey(actor))
		{
			ActorPopulationManager value = new ActorPopulationManager(this, actor);
			_managers.Add(actor, value);
		}
	}

	public void UnregisterActor(Humanoid actor)
	{
		ActorPopulationManager value;
		if (_managers.TryGetValue(actor, out value))
		{
			PopStop();
			value.Destroy();
			_managers.Remove(actor);
			PopStart();
		}
	}
}
