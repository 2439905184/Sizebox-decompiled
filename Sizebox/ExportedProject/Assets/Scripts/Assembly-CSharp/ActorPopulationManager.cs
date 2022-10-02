using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Assets.Scripts.ProceduralCityGenerator;
using UnityEngine;

internal class ActorPopulationManager
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Predicate<Micro> _003C_003E9__13_0;

		public static Predicate<CityBuilding> _003C_003E9__16_0;

		internal bool _003CActorUpdate_003Eb__13_0(Micro x)
		{
			return !x;
		}

		internal bool _003CCheckMicroStatus_003Eb__16_0(CityBuilding x)
		{
			return !x;
		}
	}

	private static readonly WaitForSeconds Wait = new WaitForSeconds(1f);

	private readonly EntityBase _entity;

	private readonly float _spawnDistance;

	public readonly List<Micro> ActiveMicros;

	private readonly CityPopulationManager _parent;

	private readonly bool _enable;

	private const float SpawnDistancePlayer = 100f;

	private const float SpawnDistanceGts = 300f;

	private readonly int _cMaxMicros = GlobalPreferences.CityPopulation.value;

	private const int CMaxSpawnPerCycle = 1;

	private readonly List<CityBuilding> _buildingOptions = new List<CityBuilding>();

	private ActorPopulationManager(CityPopulationManager parent)
	{
		_parent = parent;
		ActiveMicros = new List<Micro>();
		_enable = true;
	}

	public ActorPopulationManager(CityPopulationManager parent, EntityBase entity)
		: this(parent)
	{
		float num = (entity.isPlayer ? 100f : 300f);
		_spawnDistance = num * parent.CityScale;
		_entity = entity;
	}

	public IEnumerator ActorUpdate(int actorCount, int microCount)
	{
		if (!_enable || !_entity)
		{
			yield break;
		}
		EntityBase target = _entity;
		ActiveMicros.RemoveAll(_003C_003Ec._003C_003E9__13_0 ?? (_003C_003Ec._003C_003E9__13_0 = _003C_003Ec._003C_003E9._003CActorUpdate_003Eb__13_0));
		yield return null;
		if (microCount < _cMaxMicros - 1)
		{
			yield return SpawnMicros(actorCount, microCount);
			if (microCount < Mathf.Min(_cMaxMicros, 15))
			{
				yield return CheckMicroStatus(target, true);
			}
		}
		else
		{
			yield return CheckMicroStatus(target);
		}
	}

	public void Destroy()
	{
		foreach (Micro activeMicro in ActiveMicros)
		{
			if ((bool)activeMicro)
			{
				UnityEngine.Object.Destroy(activeMicro.gameObject);
			}
		}
		ActiveMicros.Clear();
	}

	private static Micro[] GetInvalidMicros(List<Micro> activeMicros, Vector3 targetPosition, float distance)
	{
		List<Micro> list = new List<Micro>();
		foreach (Micro activeMicro in activeMicros)
		{
			if ((bool)activeMicro && Sbox.DistanceHorizontal(activeMicro.transform.position, targetPosition) > distance)
			{
				list.Add(activeMicro);
			}
		}
		return list.ToArray();
	}

	private IEnumerator CheckMicroStatus(EntityBase target, bool close = false)
	{
		Vector3 targetPosition = target.transform.position;
		Micro[] invalidMicros = GetInvalidMicros(ActiveMicros, targetPosition, _spawnDistance * _parent.CityScale * 1.2f);
		yield return null;
		if (invalidMicros.Length < 1)
		{
			yield break;
		}
		GetSpawnBuildingsInRange(targetPosition);
		yield return null;
		int num = (close ? 1 : 10);
		if (_buildingOptions.Count < num)
		{
			_buildingOptions.Clear();
			yield break;
		}
		_buildingOptions.RemoveAll(_003C_003Ec._003C_003E9__16_0 ?? (_003C_003Ec._003C_003E9__16_0 = _003C_003Ec._003C_003E9._003CCheckMicroStatus_003Eb__16_0));
		int maxBuildingOptionsRange = _buildingOptions.Count - 1;
		for (int i = 0; i < invalidMicros.Length; i++)
		{
			int index = UnityEngine.Random.Range(0, maxBuildingOptionsRange);
			CityBuilding cityBuilding = _buildingOptions[index];
			if ((bool)cityBuilding)
			{
				Micro micro = invalidMicros[i];
				if ((bool)micro)
				{
					micro.transform.position = GetBuildingSpawnSpot(cityBuilding);
					yield return Wait;
				}
			}
			else
			{
				yield return null;
			}
		}
	}

	private IEnumerator SpawnMicros(int actorCount, int microCount)
	{
		int num = (_cMaxMicros - microCount) / actorCount;
		if (num <= 0)
		{
			yield break;
		}
		int toSpawn = ((microCount == 0) ? num : Mathf.Min(1, num));
		int max = _parent.Buildings.Count - 1;
		if (max <= 1)
		{
			yield break;
		}
		for (int i = 0; i < toSpawn; i++)
		{
			CityBuilding cityBuilding = null;
			if ((bool)_parent)
			{
				cityBuilding = _parent.Buildings[UnityEngine.Random.Range(0, max)];
			}
			if (!cityBuilding)
			{
				yield return null;
				continue;
			}
			Vector3 buildingSpawnSpot = GetBuildingSpawnSpot(cityBuilding);
			buildingSpawnSpot.x += i;
			Micro micro = SpawnMicro(buildingSpawnSpot);
			if ((bool)micro)
			{
				ActiveMicros.Add(micro);
			}
			yield return null;
		}
	}

	private void GetSpawnBuildingsInRange(Vector3 actorPosition)
	{
		for (int i = 0; i < _parent.Buildings.Count; i++)
		{
			CityBuilding cityBuilding = _parent.Buildings[i];
			if ((bool)cityBuilding && Sbox.DistanceHorizontal(cityBuilding.transform.position, actorPosition) <= _spawnDistance)
			{
				_buildingOptions.Add(cityBuilding);
			}
		}
	}

	private Vector3 GetBuildingSpawnSpot(CityBuilding target)
	{
		Renderer componentInChildren = target.GetComponentInChildren<Renderer>();
		if (!componentInChildren)
		{
			Debug.LogWarning("Building renderer couldn't be found");
			return Vector3.zero;
		}
		Bounds bounds = componentInChildren.bounds;
		Vector3 center = bounds.center;
		switch (UnityEngine.Random.Range(0, 3))
		{
		default:
			center.x = bounds.max.x;
			break;
		case 1:
			center.x = bounds.min.x;
			break;
		case 2:
			center.y = bounds.min.y;
			break;
		case 3:
			center.y = bounds.max.y;
			break;
		}
		Vector3 start = center;
		Vector3 end = center;
		start.y = bounds.max.y;
		end.y = bounds.min.y - bounds.extents.y;
		RaycastHit hitInfo;
		if (Physics.Linecast(start, end, out hitInfo))
		{
			center.y = hitInfo.point.y;
		}
		return center;
	}

	private Micro SpawnMicro(Vector3 position)
	{
		AssetDescription randomMicroAsset = AssetManager.Instance.GetRandomMicroAsset();
		if (randomMicroAsset == null)
		{
			return null;
		}
		MicroNpc microNpc = ObjectManager.Instance.InstantiateMicro(randomMicroAsset, position, Quaternion.AngleAxis(180f, Vector3.up), _parent.CityScale);
		if (!microNpc)
		{
			return null;
		}
		microNpc.ChangeScale(_parent.CityScale);
		EventManager.SendEvent(new SpawnEvent(microNpc.GetComponent<EntityBase>()));
		return microNpc;
	}
}
