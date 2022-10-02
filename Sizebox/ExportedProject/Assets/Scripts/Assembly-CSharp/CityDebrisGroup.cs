using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityDebrisGroup : MonoBehaviour
{
	[SerializeField]
	private LODGroup lodGroup;

	[SerializeField]
	private MeshFilter meshFilter;

	[SerializeField]
	private int maxMeshes = 20;

	private const float _combineDelay = 5f;

	private Mesh _mesh;

	private List<MeshFilter> _meshFilters = new List<MeshFilter>();

	private List<MeshRenderer> _meshRenderers = new List<MeshRenderer>();

	private bool _isCombiningMeshes;

	private CombineInstance[] combine;

	public bool IsFull
	{
		get
		{
			return _meshFilters.Count >= maxMeshes;
		}
	}

	private void Awake()
	{
		combine = new CombineInstance[maxMeshes * 2];
		for (int i = 0; i < combine.Length; i++)
		{
			combine[i].mesh = new Mesh();
			combine[i].transform = Matrix4x4.identity;
		}
	}

	public void SetCity(CityBuilder city)
	{
		base.transform.parent = city.transform;
	}

	public void Register(MeshRenderer renderer, MeshFilter meshFilter)
	{
		if (!IsFull)
		{
			_meshFilters.Add(meshFilter);
			_meshRenderers.Add(renderer);
			int num = (_meshFilters.Count - 1) * 2;
			Mesh sharedMesh = meshFilter.sharedMesh;
			Matrix4x4 worldToLocalMatrix = base.transform.worldToLocalMatrix;
			Matrix4x4 localToWorldMatrix = meshFilter.transform.localToWorldMatrix;
			combine[num].mesh = sharedMesh;
			combine[num].transform = worldToLocalMatrix * localToWorldMatrix;
			combine[num].subMeshIndex = 0;
			combine[num + 1].mesh = sharedMesh;
			combine[num + 1].transform = worldToLocalMatrix * localToWorldMatrix;
			combine[num + 1].subMeshIndex = 0;
			if (!_isCombiningMeshes)
			{
				StartCoroutine(DelayedMeshCombine());
			}
		}
	}

	private IEnumerator DelayedMeshCombine()
	{
		int startingCount = _meshFilters.Count;
		_isCombiningMeshes = true;
		yield return new WaitForSeconds(5f);
		if (!_mesh)
		{
			_mesh = new Mesh();
			meshFilter.mesh = _mesh;
		}
		_mesh.Clear();
		_mesh.CombineMeshes(combine, true, true);
		lodGroup.RecalculateBounds();
		for (int i = 0; i < startingCount; i++)
		{
			_meshRenderers[i].enabled = false;
		}
		_isCombiningMeshes = false;
		if (_meshFilters.Count != startingCount)
		{
			StartCoroutine(DelayedMeshCombine());
		}
	}
}
