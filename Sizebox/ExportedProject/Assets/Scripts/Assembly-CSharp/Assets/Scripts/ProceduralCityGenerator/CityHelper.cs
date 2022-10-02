using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Lua;
using UnityEngine;

namespace Assets.Scripts.ProceduralCityGenerator
{
	public class CityHelper
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<CityBuilding, bool> _003C_003E9__1_0;

			internal bool _003CGetBuilding_003Eb__1_0(CityBuilding x)
			{
				return !x.broken;
			}
		}

		public static CityBuilding FindRandomStructure(Entity agent)
		{
			return GetBuilding();
		}

		private static CityBuilding GetBuilding()
		{
			IList<CityBuilding> list = UnityEngine.Object.FindObjectsOfType<CityBuilding>().Where(_003C_003Ec._003C_003E9__1_0 ?? (_003C_003Ec._003C_003E9__1_0 = _003C_003Ec._003C_003E9._003CGetBuilding_003Eb__1_0)).ToList();
			if (!list.Any())
			{
				return null;
			}
			int index = UnityEngine.Random.Range(0, list.Count - 1);
			return list[index];
		}

		private static bool CheckRadiusForBuildings(UnityEngine.Vector3 position, float radius, ref IList<CityBuilding> buildings)
		{
			bool result = false;
			Collider[] array = Physics.OverlapSphere(position, radius, Layers.buildingLayer);
			for (int i = 0; i < array.Length; i++)
			{
				CityBuilding building;
				if (TryFindBuilding(array[i].gameObject, out building))
				{
					buildings.Add(building);
					result = true;
				}
			}
			return result;
		}

		private static bool TryFindBuilding(GameObject go, out CityBuilding building)
		{
			building = null;
			UnityEngine.Transform transform = go.transform;
			while (transform != null)
			{
				CityBuilding component = transform.gameObject.GetComponent<CityBuilding>();
				if (component != null)
				{
					building = component;
					return true;
				}
				transform = transform.parent;
			}
			return false;
		}
	}
}
