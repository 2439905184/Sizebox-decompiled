using System;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class LocalClient : MonoBehaviour
{
	public static LocalClient Instance;

	public Player Player { get; private set; }

	private void Awake()
	{
		Instance = this;
		Player = GetComponent<Player>();
	}

	public void Update()
	{
		if ((bool)Player.Entity)
		{
			base.transform.position = Player.Entity.transform.position;
		}
	}

	public GameObject SpawnGiantess(AssetDescription assetDesc, Vector3 position, Quaternion rotation, float scale, int id = -1)
	{
		Giantess giantess = ObjectManager.Instance.InstantiateGiantess(assetDesc, position, rotation, scale, id);
		if (!giantess)
		{
			return null;
		}
		EventManager.SendEvent(new SpawnEvent(giantess));
		return giantess.gameObject;
	}

	public GameObject SpawnGiantess(AssetDescription assetDesc, float scale)
	{
		Transform transform = Camera.main.transform;
		RaycastHit hitInfo;
		Vector3 vector = ((!Physics.Raycast(transform.transform.position, transform.forward, out hitInfo, 50f, Layers.walkableMask)) ? (transform.position + transform.forward.Flatten() * 50f) : hitInfo.point);
		Quaternion rotation = Quaternion.LookRotation(transform.position - vector);
		return SpawnGiantess(assetDesc, vector, rotation, scale);
	}

	public GameObject SpawnGiantess(AssetDescription assetDesc, Vector3 position, Quaternion rotation, float scale, Action<EntityBase> callback)
	{
		Giantess giantess = ObjectManager.Instance.InstantiateGiantess(assetDesc, position, rotation, scale, callback);
		if (!giantess)
		{
			return null;
		}
		EventManager.SendEvent(new SpawnEvent(giantess));
		return giantess.gameObject;
	}

	public GameObject SpawnObject(AssetDescription assetDesc, Vector3 position, Quaternion rotation, float scale, int id = -1)
	{
		EntityBase entityBase = ObjectManager.Instance.InstantiateObject(assetDesc, position, rotation, scale, id);
		if (!entityBase)
		{
			return null;
		}
		EventManager.SendEvent(new SpawnEvent(entityBase));
		return entityBase.gameObject;
	}

	public GameObject SpawnObject(AssetDescription assetDesc, Vector3 position, Quaternion rotation, float scale, Action<EntityBase> callback)
	{
		EntityBase entityBase = ObjectManager.Instance.InstantiateObject(assetDesc, position, rotation, scale, callback);
		if (!entityBase)
		{
			return null;
		}
		EventManager.SendEvent(new SpawnEvent(entityBase));
		return entityBase.gameObject;
	}

	public GameObject SpawnMicro(AssetDescription assetDesc, Vector3 position, Quaternion rotation, float scale, int id = -1)
	{
		MicroNpc microNpc = ObjectManager.Instance.InstantiateMicro(assetDesc, position, rotation, scale, id);
		if (!microNpc)
		{
			return null;
		}
		EventManager.SendEvent(new SpawnEvent(microNpc));
		return microNpc.gameObject;
	}

	public GameObject SpawnMicro(AssetDescription assetDesc, Vector3 position, Quaternion rotation, float scale, Action<EntityBase> callback)
	{
		MicroNpc microNpc = ObjectManager.Instance.InstantiateMicro(assetDesc, position, rotation, scale, callback);
		if (!microNpc)
		{
			return null;
		}
		EventManager.SendEvent(new SpawnEvent(microNpc));
		return microNpc.gameObject;
	}

	public GameObject SpawnMicro(AssetDescription assetDesc, float scale)
	{
		if ((bool)Player.Entity && Player.Entity.isGiantess)
		{
			return SpawnAroundGiantess(assetDesc, scale);
		}
		Transform transform = Camera.main.transform;
		RaycastHit hitInfo;
		Vector3 position = ((!Physics.Raycast(transform.transform.position, transform.forward, out hitInfo, 8f, Layers.walkableMask)) ? (transform.position + transform.forward * 8f) : hitInfo.point);
		Quaternion rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(-180f, 180f), 0f);
		return SpawnMicro(assetDesc, position, rotation, scale);
	}

	private GameObject SpawnAroundGiantess(AssetDescription assetDesc, float scale)
	{
		Vector3 position = Player.Entity.transform.position;
		float meshHeight = Player.Entity.MeshHeight;
		float num = meshHeight * 0.5f;
		float num2 = Mathf.Sin(UnityEngine.Random.Range(0f, 360f));
		float num3 = Mathf.Sin(UnityEngine.Random.Range(0f, 360f));
		num2 = Mathf.Clamp(num2 * num, 0f - num, num);
		num3 = Mathf.Clamp(num3 * num, 0f - num, num);
		float num4 = num * 0.25f;
		if (Mathf.Abs(num2) < num * 0.25f)
		{
			num2 *= num4 / Mathf.Abs(num2);
		}
		if (Mathf.Abs(num3) < num * 0.25f)
		{
			num3 *= num4 / Mathf.Abs(num3);
		}
		Vector3 vector = position + new Vector3(num2, 0f, num3) + Vector3.up * meshHeight;
		RaycastHit hitInfo;
		if (Physics.Raycast(vector, Vector3.down, out hitInfo))
		{
			vector = hitInfo.point;
		}
		Quaternion rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(-180f, 180f), 0f);
		return SpawnMicro(assetDesc, vector, rotation, scale);
	}

	public GameObject SpawnPlayableGiantess(AssetDescription assetDesc, Vector3 position, Quaternion rotation, float scale)
	{
		GameObject obj = SpawnGiantess(assetDesc, position, rotation, scale);
		IPlayable component = obj.GetComponent<IPlayable>();
		if (component != null)
		{
			Player.PlayAs(component);
		}
		return obj;
	}
}
