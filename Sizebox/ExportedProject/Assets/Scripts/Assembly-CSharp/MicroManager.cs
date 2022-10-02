using System.Collections.Generic;
using Lua;
using UnityEngine;

public class MicroManager
{
	public static MicroManager Instance;

	public Dictionary<int, Micro> microDictionary = new Dictionary<int, Micro>();

	private Dictionary<int, Entity> luaMicroDictionary = new Dictionary<int, Entity>();

	private Queue<Micro> pool;

	private UnityEngine.Transform cameraTransform;

	private UnityEngine.Vector3 cameraPosition;

	private float maxDistanceBase = 500f;

	private int frame;

	public MicroManager()
	{
		Instance = this;
		cameraTransform = Camera.main.transform;
		pool = new Queue<Micro>();
	}

	public void AddMicro(Micro micro)
	{
		microDictionary.Add(micro.id, micro);
		luaMicroDictionary.Add(micro.id, micro);
	}

	public void RemoveMicro(int id)
	{
		microDictionary.Remove(id);
		luaMicroDictionary.Remove(id);
	}

	public void FrameUpdate()
	{
		frame++;
		if (frame % 60 != 0)
		{
			return;
		}
		cameraPosition = cameraTransform.position;
		foreach (KeyValuePair<int, Micro> item in microDictionary)
		{
			Micro value = item.Value;
			if ((bool)value && !value.isPlayer)
			{
				MicroUpdate(value);
			}
		}
	}

	public void MicroUpdate(Micro micro)
	{
		UnityEngine.Transform transform = micro.transform;
		UnityEngine.Vector3 position = transform.position;
		float num = position.x - cameraPosition.x;
		float num2 = position.y - cameraPosition.y;
		float num3 = position.z - cameraPosition.z;
		float num4 = num * num + num2 * num2 + num3 * num3;
		float num5 = maxDistanceBase * transform.lossyScale.y;
		bool flag = micro.IsVisible();
		bool flag2 = num4 < num5 * num5;
		if (flag && !flag2)
		{
			micro.Render(false);
		}
		else if (!flag && flag2)
		{
			micro.Render(true);
		}
	}

	public IDictionary<int, Entity> GetLuaMicroList()
	{
		return luaMicroDictionary;
	}

	public static Micro GetRandomMicro(EntityBase entity)
	{
		int count = Instance.microDictionary.Count;
		int num = UnityEngine.Random.Range(0, count);
		int num2 = 0;
		Micro micro = null;
		foreach (KeyValuePair<int, Micro> item in Instance.microDictionary)
		{
			if (micro == null && item.Value.gameObject.activeInHierarchy && (!entity.isMicro || item.Value.id != entity.id))
			{
				micro = item.Value;
			}
			if (num2 == num)
			{
				if (item.Value.gameObject.activeInHierarchy && (!entity.isMicro || item.Value.id != entity.id))
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
		return micro;
	}

	public void AddToPool(Micro micro)
	{
		pool.Enqueue(micro);
	}

	public int GetPoolCount()
	{
		return pool.Count;
	}

	public Micro GetFromPool()
	{
		return pool.Dequeue();
	}

	public static Micro FindClosestMicro(EntityBase entity, float scale)
	{
		float num = 0.1f * scale;
		float num2 = 100f * scale;
		List<Micro> list = null;
		while (num < num2 && (list == null || list.Count == 0))
		{
			list = CheckRadius(entity, num);
			num *= 2f;
		}
		Micro result = null;
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

	public static List<Micro> FindMicrosInRadius(EntityBase entity, float radius)
	{
		radius *= entity.Height * 0.625f;
		return CheckRadius(entity, radius);
	}

	private static List<Micro> CheckRadius(EntityBase entity, float radius)
	{
		Collider[] array = Physics.OverlapSphere(entity.transform.position, radius, Layers.crushableMask, QueryTriggerInteraction.Collide);
		List<Micro> list = new List<Micro>();
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			if (GlobalPreferences.IgnorePlayer.value && collider.gameObject.layer == Layers.playerLayer)
			{
				continue;
			}
			Micro componentInParent = collider.gameObject.GetComponentInParent<Micro>();
			if ((bool)componentInParent && !componentInParent.isCrushed && !componentInParent.IsDead && !componentInParent.locked && entity.id != componentInParent.id && (!componentInParent.transform.parent || componentInParent.transform.parent.gameObject.layer != Layers.gtsBodyLayer))
			{
				float num = componentInParent.CheckForOverHeadObjects(entity.Height);
				if ((num == 0f || !(num < entity.Height)) && !componentInParent.IsOnGiantess())
				{
					list.Add(componentInParent);
				}
			}
		}
		return list;
	}
}
