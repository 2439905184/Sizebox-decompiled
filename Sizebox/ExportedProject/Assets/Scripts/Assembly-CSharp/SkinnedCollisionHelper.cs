using System.Collections;
using UnityEngine;

public class SkinnedCollisionHelper : MonoBehaviour
{
	private class CVertexWeight
	{
		public int index;

		public Vector3 localPosition;

		public Vector3 localNormal;

		public float weight;

		public CVertexWeight(int i, Vector3 p, Vector3 n, float w)
		{
			index = i;
			localPosition = p;
			localNormal = n;
			weight = w;
		}
	}

	private class CWeightList
	{
		public Transform transform;

		public ArrayList weights;

		public CWeightList()
		{
			weights = new ArrayList();
		}
	}

	private bool IsInit;

	private CWeightList[] nodeWeights;

	private SkinnedMeshRenderer skinnedMeshRenderer;

	private MeshCollider meshCollider;

	private Mesh meshCalc;

	public bool Init()
	{
		if (IsInit)
		{
			return true;
		}
		skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
		meshCollider = GetComponent<MeshCollider>();
		if (meshCollider != null && skinnedMeshRenderer != null)
		{
			meshCalc = Object.Instantiate(skinnedMeshRenderer.sharedMesh);
			meshCalc.name = skinnedMeshRenderer.sharedMesh.name + "_calc";
			meshCollider.sharedMesh = meshCalc;
			meshCalc.MarkDynamic();
			Vector3[] vertices = skinnedMeshRenderer.sharedMesh.vertices;
			Vector3[] normals = skinnedMeshRenderer.sharedMesh.normals;
			Matrix4x4[] bindposes = skinnedMeshRenderer.sharedMesh.bindposes;
			BoneWeight[] boneWeights = skinnedMeshRenderer.sharedMesh.boneWeights;
			nodeWeights = new CWeightList[skinnedMeshRenderer.bones.Length];
			for (int i = 0; i < skinnedMeshRenderer.bones.Length; i++)
			{
				nodeWeights[i] = new CWeightList();
				nodeWeights[i].transform = skinnedMeshRenderer.bones[i];
			}
			for (int j = 0; j < vertices.Length; j++)
			{
				BoneWeight boneWeight = boneWeights[j];
				if (boneWeight.weight0 != 0f)
				{
					Vector3 p = bindposes[boneWeight.boneIndex0].MultiplyPoint3x4(vertices[j]);
					Vector3 n = bindposes[boneWeight.boneIndex0].MultiplyPoint3x4(normals[j]);
					nodeWeights[boneWeight.boneIndex0].weights.Add(new CVertexWeight(j, p, n, boneWeight.weight0));
				}
				if (boneWeight.weight1 != 0f)
				{
					Vector3 p2 = bindposes[boneWeight.boneIndex1].MultiplyPoint3x4(vertices[j]);
					Vector3 n2 = bindposes[boneWeight.boneIndex1].MultiplyPoint3x4(normals[j]);
					nodeWeights[boneWeight.boneIndex1].weights.Add(new CVertexWeight(j, p2, n2, boneWeight.weight1));
				}
				if (boneWeight.weight2 != 0f)
				{
					Vector3 p3 = bindposes[boneWeight.boneIndex2].MultiplyPoint3x4(vertices[j]);
					Vector3 n3 = bindposes[boneWeight.boneIndex2].MultiplyPoint3x4(normals[j]);
					nodeWeights[boneWeight.boneIndex2].weights.Add(new CVertexWeight(j, p3, n3, boneWeight.weight2));
				}
				if (boneWeight.weight3 != 0f)
				{
					Vector3 p4 = bindposes[boneWeight.boneIndex3].MultiplyPoint3x4(vertices[j]);
					Vector3 n4 = bindposes[boneWeight.boneIndex3].MultiplyPoint3x4(normals[j]);
					nodeWeights[boneWeight.boneIndex3].weights.Add(new CVertexWeight(j, p4, n4, boneWeight.weight3));
				}
			}
			UpdateCollisionMesh(false);
			IsInit = true;
			return true;
		}
		return false;
	}

	public bool Release()
	{
		Object.Destroy(meshCalc);
		return true;
	}

	public void UpdateCollisionMesh(bool _bRelease = true)
	{
		Vector3[] vertices = meshCalc.vertices;
		for (int i = 0; i < vertices.Length; i++)
		{
			vertices[i] = Vector3.zero;
		}
		CWeightList[] array = nodeWeights;
		foreach (CWeightList obj in array)
		{
			Matrix4x4 localToWorldMatrix = obj.transform.localToWorldMatrix;
			foreach (CVertexWeight weight in obj.weights)
			{
				vertices[weight.index] += localToWorldMatrix.MultiplyPoint3x4(weight.localPosition) * weight.weight;
			}
		}
		for (int k = 0; k < vertices.Length; k++)
		{
			vertices[k] = base.transform.InverseTransformPoint(vertices[k]);
		}
		meshCalc.vertices = vertices;
		meshCollider.enabled = false;
		meshCollider.enabled = true;
	}
}
