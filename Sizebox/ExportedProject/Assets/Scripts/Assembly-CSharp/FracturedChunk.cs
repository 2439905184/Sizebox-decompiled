using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[ExecuteInEditMode]
public class FracturedChunk : MonoBehaviour
{
	[Serializable]
	public class AdjacencyInfo
	{
		public FracturedChunk chunk;

		public float fArea;

		public AdjacencyInfo(FracturedChunk chunk, float fArea)
		{
			this.chunk = chunk;
			this.fArea = fArea;
		}
	}

	public struct CollisionInfo
	{
		public FracturedChunk chunk;

		public Vector3 collisionPoint;

		public bool bIsMain;

		public bool bCancelCollisionEvent;

		public CollisionInfo(FracturedChunk chunk, Vector3 collisionPoint, bool bIsMain)
		{
			this.chunk = chunk;
			this.collisionPoint = collisionPoint;
			this.bIsMain = bIsMain;
			bCancelCollisionEvent = false;
		}
	}

	private const string ATTACHED_TAG = "AttachedDebris";

	private const string DETACHED_TAG = "DetachedDebris";

	public FracturedObject FracturedObjectSource;

	public int SplitSubMeshIndex = -1;

	public bool DontDeleteAfterBroken;

	public bool IsSupportChunk;

	public bool IsNonSupportedChunk;

	public bool IsDetachedChunk;

	public float RelativeVolume = 0.01f;

	public float Volume;

	public bool HasConcaveCollider;

	public float PreviewDecompositionValue;

	public Color RandomMaterialColor = Color.white;

	public bool Visited;

	public List<AdjacencyInfo> ListAdjacentChunks = new List<AdjacencyInfo>();

	[SerializeField]
	private Vector3 m_v3InitialLocalPosition;

	[SerializeField]
	private Quaternion m_qInitialLocalRotation;

	[SerializeField]
	private Vector3 m_v3InitialLocalScale;

	[SerializeField]
	private bool m_bInitialLocalRotScaleInitialized;

	private List<AdjacencyInfo> ListAdjacentChunksCopy;

	private bool m_bNonSupportedChunkStored;

	private Collider myCollider;

	private const float _minSettleWait = 1f;

	private const float _maxSettleWait = 2.5f;

	private const float _minFallDistance = 2.5f;

	private const int _streakToSettle = 3;

	private const float _sqrSettleThreshold = 0.4f;

	private const float _maxSettleTime = 90f;

	private static Collider[] lodCastResults = new Collider[32];

	public Rigidbody Rigidbody { get; private set; }

	private void Awake()
	{
		if (Application.isPlaying)
		{
			IsDetachedChunk = false;
			base.transform.localPosition = m_v3InitialLocalPosition;
			if (m_bInitialLocalRotScaleInitialized)
			{
				base.transform.localRotation = m_qInitialLocalRotation;
				base.transform.localScale = m_v3InitialLocalScale;
			}
			ListAdjacentChunksCopy = new List<AdjacencyInfo>(ListAdjacentChunks);
		}
		myCollider = GetComponent<Collider>();
		if ((bool)myCollider)
		{
			Rigidbody = myCollider.attachedRigidbody;
		}
		else
		{
			Rigidbody = GetComponent<Rigidbody>();
		}
		m_bNonSupportedChunkStored = IsNonSupportedChunk;
		base.gameObject.tag = "AttachedDebris";
	}

	private void OnCollisionEnter(Collision collision)
	{
		HandleCollision(collision.collider, collision.GetContact(0).point, collision.relativeVelocity.sqrMagnitude);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!(FracturedObjectSource == null) && !(other == null))
		{
			Rigidbody attachedRigidbody = other.attachedRigidbody;
			HandleCollision(other, other.transform.position, (attachedRigidbody != null) ? attachedRigidbody.velocity.sqrMagnitude : 0f);
		}
	}

	private void HandleCollision(Collider other, Vector3 v3CollisionPos, float relativeSpeedSquared)
	{
		if (FracturedObjectSource == null || other == null || other.gameObject.tag == "AttachedDebris")
		{
			return;
		}
		Rigidbody attachedRigidbody = other.attachedRigidbody;
		float num = float.PositiveInfinity;
		if ((bool)attachedRigidbody)
		{
			num = attachedRigidbody.mass;
		}
		if (!IsDetachedChunk)
		{
			FracturedChunk component = other.GetComponent<FracturedChunk>();
			Transform parent = other.transform.parent;
			if (!component && (bool)parent)
			{
				component = parent.GetComponent<FracturedChunk>();
			}
			bool flag = false;
			if ((bool)component && component.IsDetachedChunk && component.FracturedObjectSource == FracturedObjectSource)
			{
				flag = true;
			}
			if (flag || !(relativeSpeedSquared > Mathf.Pow(FracturedObjectSource.EventDetachMinVelocity, 2f)) || !(num > FracturedObjectSource.EventDetachMinMass) || !Rigidbody || !IsDestructibleChunk())
			{
				return;
			}
			CollisionInfo collisionInfo = new CollisionInfo(this, v3CollisionPos, true);
			FracturedObjectSource.NotifyDetachChunkCollision(collisionInfo);
			if (collisionInfo.bCancelCollisionEvent)
			{
				return;
			}
			new List<FracturedChunk>();
			List<FracturedChunk> list = ComputeRandomConnectionBreaks();
			list.Add(this);
			DetachFromObject();
			{
				foreach (FracturedChunk item in list)
				{
					FracturedChunk fracturedChunk = (collisionInfo.chunk = item);
					collisionInfo.bIsMain = false;
					collisionInfo.bCancelCollisionEvent = false;
					if (fracturedChunk != this)
					{
						FracturedObjectSource.NotifyDetachChunkCollision(collisionInfo);
					}
					if (!collisionInfo.bCancelCollisionEvent)
					{
						fracturedChunk.DetachFromObject();
						fracturedChunk.Rigidbody.AddExplosionForce(relativeSpeedSquared * Mathf.Pow(FracturedObjectSource.EventDetachExitForce, 2f), attachedRigidbody.transform.position, 0f, FracturedObjectSource.EventDetachUpwardsModifier);
					}
				}
				return;
			}
		}
		Vector3 vector = ((attachedRigidbody != null) ? attachedRigidbody.velocity : Vector3.zero);
		if ((Rigidbody ? (vector - Rigidbody.velocity).magnitude : float.MaxValue) > FracturedObjectSource.EventDetachedMinVelocity && num > FracturedObjectSource.EventDetachedMinMass)
		{
			FracturedObjectSource.NotifyFreeChunkCollision(new CollisionInfo(this, v3CollisionPos, true));
		}
	}

	public bool IsDestructibleChunk()
	{
		if (FracturedObjectSource != null)
		{
			if (FracturedObjectSource.SupportChunksAreIndestructible)
			{
				return !IsSupportChunk;
			}
			if (!FracturedObjectSource.SupportChunksAreIndestructible)
			{
				return true;
			}
		}
		return !IsSupportChunk;
	}

	public void ResetChunk(FracturedObject fracturedObjectSource)
	{
		base.transform.parent = fracturedObjectSource.transform;
		GetComponent<Rigidbody>().isKinematic = true;
		GetComponent<Collider>().isTrigger = FracturedObjectSource.ChunkColliderType == FracturedObject.ColliderType.Trigger;
		IsNonSupportedChunk = m_bNonSupportedChunkStored;
		FracturedObjectSource = fracturedObjectSource;
		IsDetachedChunk = false;
		base.transform.localPosition = m_v3InitialLocalPosition;
		if (m_bInitialLocalRotScaleInitialized)
		{
			base.transform.localRotation = m_qInitialLocalRotation;
			base.transform.localScale = m_v3InitialLocalScale;
		}
		ListAdjacentChunks = new List<AdjacencyInfo>(ListAdjacentChunksCopy);
	}

	public void Impact(Vector3 v3Position, float fExplosionForce, float fRadius, bool bAlsoImpactFreeChunks)
	{
		if (GetComponent<Rigidbody>() != null && IsDestructibleChunk())
		{
			new List<FracturedChunk>();
			if (!IsDetachedChunk)
			{
				List<FracturedChunk> list = ComputeRandomConnectionBreaks();
				list.Add(this);
				DetachFromObject();
				foreach (FracturedChunk item in list)
				{
					item.DetachFromObject();
					item.GetComponent<Rigidbody>().AddExplosionForce(fExplosionForce, v3Position, 0f, 0f);
				}
			}
			foreach (FracturedChunk item2 in FracturedObjectSource.GetDestructibleChunksInRadius(v3Position, fRadius, bAlsoImpactFreeChunks))
			{
				item2.DetachFromObject();
				item2.GetComponent<Rigidbody>().AddExplosionForce(fExplosionForce, v3Position, 0f, FracturedObjectSource.EventDetachUpwardsModifier);
			}
		}
		FracturedObjectSource.NotifyImpact(v3Position);
	}

	public void OnCreateFromFracturedObject(FracturedObject fracturedComponent, int nSplitSubMeshIndex)
	{
		FracturedObjectSource = fracturedComponent;
		SplitSubMeshIndex = nSplitSubMeshIndex;
		RandomMaterialColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 0.7f);
		m_v3InitialLocalPosition = base.transform.localPosition;
		m_qInitialLocalRotation = base.transform.localRotation;
		m_v3InitialLocalScale = base.transform.localScale;
		m_bInitialLocalRotScaleInitialized = true;
	}

	public void UpdatePreviewDecompositionPosition()
	{
		float num = 5f;
		float num2 = 1f;
		if (FracturedObjectSource != null)
		{
			num2 = m_v3InitialLocalPosition.magnitude / FracturedObjectSource.DecomposeRadius;
		}
		Vector3 normalized = m_v3InitialLocalPosition.normalized;
		base.transform.localPosition = m_v3InitialLocalPosition + normalized * (PreviewDecompositionValue * num2 * num);
	}

	public void ConnectTo(FracturedChunk chunk, float fArea)
	{
		if ((bool)chunk && !chunk.IsConnectedTo(this))
		{
			ListAdjacentChunks.Add(new AdjacencyInfo(chunk, fArea));
			chunk.ListAdjacentChunks.Add(new AdjacencyInfo(this, fArea));
		}
	}

	public void DisconnectFrom(FracturedChunk chunk)
	{
		if (!chunk || !chunk.IsConnectedTo(this))
		{
			return;
		}
		for (int i = 0; i < ListAdjacentChunks.Count; i++)
		{
			if (ListAdjacentChunks[i].chunk == chunk)
			{
				ListAdjacentChunks.RemoveAt(i);
				break;
			}
		}
		for (int j = 0; j < chunk.ListAdjacentChunks.Count; j++)
		{
			if (chunk.ListAdjacentChunks[j].chunk == this)
			{
				chunk.ListAdjacentChunks.RemoveAt(j);
				break;
			}
		}
	}

	public bool IsConnectedTo(FracturedChunk chunk)
	{
		foreach (AdjacencyInfo listAdjacentChunk in ListAdjacentChunks)
		{
			bool result = true;
			if ((bool)listAdjacentChunk.chunk.FracturedObjectSource)
			{
				result = listAdjacentChunk.fArea > listAdjacentChunk.chunk.FracturedObjectSource.ChunkConnectionMinArea;
			}
			if (listAdjacentChunk.chunk == chunk)
			{
				return result;
			}
		}
		return false;
	}

	public void DetachFromObject(bool bCheckStructureIntegrity = true)
	{
		if (!IsDestructibleChunk() || IsDetachedChunk || !GetComponent<Rigidbody>())
		{
			return;
		}
		m_bNonSupportedChunkStored = IsNonSupportedChunk;
		base.transform.parent = null;
		GetComponent<Rigidbody>().isKinematic = false;
		GetComponent<Collider>().isTrigger = false;
		IsDetachedChunk = true;
		IsNonSupportedChunk = true;
		base.gameObject.layer = Layers.debrisLayer;
		base.gameObject.tag = "DetachedDebris";
		RemoveConnectionInfo();
		if ((bool)FracturedObjectSource)
		{
			FracturedObjectSource.NotifyChunkDetach(this);
			if (bCheckStructureIntegrity)
			{
				FracturedObjectSource.CheckDetachNonSupportedChunks();
			}
		}
		StartCoroutine(TryToSettle());
	}

	private IEnumerator TryToSettle()
	{
		Vector3 startPos = base.transform.position.ToVirtual();
		Vector3 previousPos = startPos;
		int streak = 0;
		float timer = 0f;
		while (true)
		{
			float waitTime = UnityEngine.Random.Range(1f, 2.5f);
			yield return new WaitForSeconds(waitTime);
			Vector3 vector = base.transform.position.ToVirtual();
			if ((startPos - vector).y >= 2.5f && (previousPos - vector).sqrMagnitude < 0.4f)
			{
				streak++;
				if (streak > 3)
				{
					break;
				}
			}
			else
			{
				streak = 0;
				startPos.y += 0.1f;
			}
			previousPos = vector;
			timer += waitTime;
			if (timer > 90f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
		Settle();
	}

	private void Settle()
	{
		UnityEngine.Object.Destroy(Rigidbody);
		if (GlobalPreferences.KeepDebris.value)
		{
			JoinLodGroup();
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void JoinLodGroup()
	{
		MeshRenderer component = GetComponent<MeshRenderer>();
		MeshFilter component2 = GetComponent<MeshFilter>();
		int num = Physics.OverlapSphereNonAlloc(base.transform.position, 10f, lodCastResults, Layers.auxMask);
		for (int i = 0; i < num; i++)
		{
			CityDebrisGroup component3 = lodCastResults[i].GetComponent<CityDebrisGroup>();
			if ((bool)component3 && !component3.IsFull)
			{
				base.transform.parent = component3.transform;
				component3.Register(component, component2);
				return;
			}
		}
		if (!FracturedObjectSource)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		CityDebrisGroup cityDebrisGroup = UnityEngine.Object.Instantiate(CityBuilder.DebrisGroupPrefab, base.transform.position, Quaternion.identity);
		cityDebrisGroup.SetCity(FracturedObjectSource.GetComponentInParent<CityBuilder>());
		base.transform.parent = cityDebrisGroup.transform;
		cityDebrisGroup.Register(component, component2);
	}

	private void RemoveConnectionInfo()
	{
		foreach (AdjacencyInfo listAdjacentChunk in ListAdjacentChunks)
		{
			if (!listAdjacentChunk.chunk)
			{
				continue;
			}
			foreach (AdjacencyInfo listAdjacentChunk2 in listAdjacentChunk.chunk.ListAdjacentChunks)
			{
				if (listAdjacentChunk2.chunk == this)
				{
					listAdjacentChunk.chunk.ListAdjacentChunks.Remove(listAdjacentChunk2);
					break;
				}
			}
		}
		ListAdjacentChunks.Clear();
	}

	public List<FracturedChunk> ComputeRandomConnectionBreaks()
	{
		List<FracturedChunk> list = new List<FracturedChunk>();
		if (FracturedObjectSource == null)
		{
			return list;
		}
		FracturedObjectSource.ResetAllChunkVisitedFlags();
		ComputeRandomConnectionBreaksRecursive(this, list, 1);
		return list;
	}

	private static void ComputeRandomConnectionBreaksRecursive(FracturedChunk chunk, List<FracturedChunk> listBreaksOut, int nLevel)
	{
		if (chunk.Visited)
		{
			return;
		}
		chunk.Visited = true;
		foreach (AdjacencyInfo listAdjacentChunk in chunk.ListAdjacentChunks)
		{
			if ((bool)listAdjacentChunk.chunk && chunk.FracturedObjectSource != null && !listAdjacentChunk.chunk.Visited && listAdjacentChunk.chunk.IsDestructibleChunk() && listAdjacentChunk.fArea > chunk.FracturedObjectSource.ChunkConnectionMinArea && UnityEngine.Random.value > chunk.FracturedObjectSource.ChunkConnectionStrength * (float)nLevel)
			{
				ComputeRandomConnectionBreaksRecursive(listAdjacentChunk.chunk, listBreaksOut, nLevel + 1);
				listBreaksOut.Add(listAdjacentChunk.chunk);
			}
		}
	}

	public static FracturedChunk ChunkRaycast(Vector3 v3Pos, Vector3 v3Forward, out RaycastHit hitInfo)
	{
		FracturedChunk fracturedChunk = null;
		if (Physics.Raycast(v3Pos, v3Forward, out hitInfo))
		{
			fracturedChunk = hitInfo.collider.GetComponent<FracturedChunk>();
			if (fracturedChunk == null && hitInfo.collider.transform.parent != null)
			{
				fracturedChunk = hitInfo.collider.transform.parent.GetComponent<FracturedChunk>();
			}
		}
		return fracturedChunk;
	}
}
