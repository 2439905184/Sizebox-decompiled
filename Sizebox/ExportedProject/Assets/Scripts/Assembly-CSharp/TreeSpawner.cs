using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
	public class Patch
	{
		public int x;

		public int z;

		public GameObject[] trees;

		public int treeCount;

		public bool IsVisible()
		{
			if (treeCount == 0)
			{
				return false;
			}
			Vector3 vector = Camera.main.WorldToViewportPoint(trees[0].transform.position);
			if (vector.x < 0f || vector.x > 1f)
			{
				return false;
			}
			if (vector.y < 0f || vector.y > 1f)
			{
				return false;
			}
			if (vector.z < 0f)
			{
				return false;
			}
			return true;
		}
	}

	private GameObject[] treePrefabs;

	private int maxTrees = 2000;

	private int totalTreeCount;

	private int treesPerPatch = 20;

	private float patchSize = 300f;

	private int patchX;

	private int patchZ;

	private int depth = 3;

	private List<Patch> patchs;

	private Patch poolingPatch;

	private Queue<Patch> patchsToFill;

	private Vector3 highestPoint = new Vector3(0f, 1000f, 0f);

	private float lowestPoint = 200f;

	private Transform root;

	private bool editorMode;

	private void Start()
	{
		treePrefabs = Resources.LoadAll<GameObject>("Nature/Tree");
		patchs = new List<Patch>();
		patchsToFill = new Queue<Patch>();
	}

	private void PlaceTree(Vector3 position, Patch patch)
	{
		Vector3 origin = CenterOrigin.VirtualToWorld(highestPoint);
		origin.x = position.x;
		origin.z = position.z;
		RaycastHit hitInfo;
		if (Physics.Raycast(origin, Vector3.down, out hitInfo, highestPoint.y - lowestPoint, Layers.walkableMask) && hitInfo.collider.gameObject.layer == Layers.mapLayer && !(Vector3.Angle(Vector3.up, hitInfo.normal) > 30f))
		{
			SpawnTree(hitInfo.point, patch);
		}
	}

	private void SpawnTree(Vector3 point, Patch patch)
	{
		if (totalTreeCount >= maxTrees)
		{
			PoolTree(point, patch);
		}
		else
		{
			CreateTree(point, patch);
		}
	}

	private void PoolTree(Vector3 point, Patch patch)
	{
		GameObject treeFromPool = GetTreeFromPool();
		if (!(treeFromPool == null))
		{
			patch.trees[patch.treeCount] = treeFromPool;
			treeFromPool.transform.position = point;
			patch.treeCount++;
		}
	}

	private GameObject GetTreeFromPool()
	{
		if (poolingPatch == null || poolingPatch.treeCount == 0)
		{
			PoolPatch();
		}
		if (poolingPatch == null || poolingPatch.treeCount == 0)
		{
			return null;
		}
		poolingPatch.treeCount--;
		GameObject result = poolingPatch.trees[poolingPatch.treeCount];
		poolingPatch.trees[poolingPatch.treeCount] = null;
		return result;
	}

	private void PoolPatch()
	{
		if (patchs.Count == 0)
		{
			return;
		}
		int index = 0;
		int num = 0;
		for (int i = 0; i < patchs.Count; i++)
		{
			Patch patch = patchs[i];
			int num2 = Mathf.Abs(patch.x - patchX) + Mathf.Abs(patch.z - patchZ);
			if (num2 > num && !patch.IsVisible())
			{
				index = i;
				num = num2;
			}
		}
		poolingPatch = patchs[index];
		patchs.RemoveAt(index);
		if (poolingPatch.treeCount == 0)
		{
			PoolPatch();
		}
	}

	private void CreateTree(Vector3 point, Patch patch)
	{
		patch.trees[patch.treeCount] = Object.Instantiate(treePrefabs[Random.Range(0, treePrefabs.Length)], point, Quaternion.identity);
		if (editorMode)
		{
			patch.trees[patch.treeCount].transform.SetParent(root);
		}
		patch.treeCount++;
		totalTreeCount++;
	}

	private void Update()
	{
		CheckPatch();
		FillPatchs();
	}

	private void CheckPatch()
	{
		if ((bool)GameController.LocalClient.Player.Entity)
		{
			Vector3 vector = CenterOrigin.WorldToVirtual(GameController.LocalClient.Player.Entity.transform.position);
			int num = (int)Mathf.Floor(vector.x / patchSize);
			int num2 = (int)Mathf.Floor(vector.z / patchSize);
			if (num != patchX || num2 != patchZ)
			{
				int num3 = num - patchX;
				int num4 = num2 - patchZ;
				patchX = num;
				patchZ = num2;
				RenderNeighboors(num + num3 * depth, num2 + num4 * depth);
			}
		}
	}

	private void FillPatchs()
	{
		if (patchsToFill.Count != 0)
		{
			Patch patch = patchsToFill.Dequeue();
			RenderPatch(patch);
			patchs.Add(patch);
		}
	}

	private void RenderNeighboors(int x, int z)
	{
		int num = 4;
		for (int i = -num; i <= num; i++)
		{
			for (int j = -num; j <= num; j++)
			{
				int x2 = x + i;
				int z2 = z + j;
				if (!PatchExists(x2, z2))
				{
					Patch item = CreatePatch(x2, z2);
					patchsToFill.Enqueue(item);
				}
			}
		}
	}

	private void RenderPatch(Patch patch)
	{
		Vector3 vector = CenterOrigin.VirtualToWorld(new Vector3((float)patch.x * patchSize, 0f, (float)patch.z * patchSize));
		for (int i = 0; i < treesPerPatch; i++)
		{
			float x = Random.Range(0f, patchSize);
			float z = Random.Range(0f, patchSize);
			Vector3 vector2 = new Vector3(x, 0f, z);
			Vector3 position = vector + vector2;
			PlaceTree(position, patch);
		}
	}

	private Patch CreatePatch(int x, int z)
	{
		return new Patch
		{
			x = x,
			z = z,
			trees = new GameObject[treesPerPatch],
			treeCount = 0
		};
	}

	private bool PatchExists(int x, int z)
	{
		for (int i = 0; i < patchs.Count; i++)
		{
			Patch patch = patchs[i];
			if (patch.x == x && patch.z == z)
			{
				return true;
			}
		}
		foreach (Patch item in patchsToFill)
		{
			if (item.x == x && item.z == z)
			{
				return true;
			}
		}
		return false;
	}
}
