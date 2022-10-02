using System.Collections.Generic;
using Lua;
using UnityEngine;

public class GiantessManager
{
	public static GiantessManager Instance;

	private Dictionary<int, Giantess> gtsDictionary = new Dictionary<int, Giantess>();

	private Dictionary<int, Entity> luaGtsDictionary = new Dictionary<int, Entity>();

	public GiantessManager()
	{
		Instance = this;
	}

	public void AddGiantess(Giantess gts)
	{
		gtsDictionary.Add(gts.id, gts);
		luaGtsDictionary.Add(gts.id, gts);
	}

	public void RemoveGiantess(int id)
	{
		gtsDictionary.Remove(id);
		luaGtsDictionary.Remove(id);
	}

	public IDictionary<int, Entity> GetLuaGtsList()
	{
		return luaGtsDictionary;
	}

	public static Giantess GetRandomGiantess(EntityBase entity)
	{
		int count = Instance.gtsDictionary.Count;
		int num = UnityEngine.Random.Range(0, count);
		int num2 = 0;
		Giantess giantess = null;
		foreach (KeyValuePair<int, Giantess> item in Instance.gtsDictionary)
		{
			if (giantess == null && item.Value.gameObject.activeInHierarchy && (!entity.isGiantess || item.Value.id != entity.id))
			{
				giantess = item.Value;
			}
			if (num2 == num)
			{
				if (item.Value.gameObject.activeInHierarchy && (!entity.isGiantess || item.Value.id != entity.id))
				{
					return item.Value;
				}
				num2++;
			}
			else
			{
				num2++;
			}
		}
		return giantess;
	}

	public static Giantess FindClosestGiantess(EntityBase entity, float scale)
	{
		float num = 0.1f * scale;
		float num2 = 100f * scale;
		List<Giantess> list = null;
		while (num < num2 && (list == null || list.Count == 0))
		{
			list = CheckRadius(entity, num);
			num *= 2f;
		}
		Giantess result = null;
		float num3 = num2;
		for (int i = 0; i < list.Count; i++)
		{
			float magnitude = (entity.transform.position - list[i].transform.position).magnitude;
			if (magnitude < num3)
			{
				result = list[i];
				num3 = magnitude;
			}
		}
		return result;
	}

	public static List<Giantess> FindGiantessesInRadius(EntityBase entity, float radius)
	{
		radius *= entity.Height;
		return CheckRadius(entity, radius);
	}

	private static List<Giantess> CheckRadius(EntityBase entity, float radius)
	{
		Collider[] array = Physics.OverlapSphere(entity.transform.position, radius, Layers.gtsCapsuleMask);
		List<Giantess> list = new List<Giantess>();
		Collider[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			Giantess giantess = array2[i].gameObject.GetComponent<GTSMovement>().giantess;
			if ((bool)giantess && !giantess.locked && entity.id != giantess.id)
			{
				list.Add(giantess);
			}
		}
		return list;
	}
}
