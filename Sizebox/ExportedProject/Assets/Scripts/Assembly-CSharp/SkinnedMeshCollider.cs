using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class SkinnedMeshCollider : MonoBehaviour
{
	public class TriangleData
	{
		public int index;

		public Vector3 localCentroidPosition;

		public Vector3 worldFacing;

		public List<int> triangleVertexIds = new List<int>();
	}

	public class Bone
	{
		public Transform transform;

		public List<int>[] sharedTriangles;

		public int[][] trianglesArray;

		public int[] localTriangles;

		public Vector3[] localVertices;

		public List<int> vertexList;

		public int[] vertexArray;

		public int vertexCount;

		public bool[] hasVertex;

		public int[] globalToLocalVertex;

		public List<TriangleData> allTriangleData;

		public GiantessBone gtsBoneReffernce;
	}

	private ColliderReshaper colliderReshaper;

	private SkinnedMeshRenderer skinnedMeshRenderer;

	public Mesh poseMesh;

	private Giantess giantess;

	public Dictionary<int, GiantessBone> giantessBones;

	public bool update;

	private int[] boneIndex0;

	public Bone[] boneData;

	private bool meshEnabled = true;

	public bool hasInit;

	private LayerMask layer;

	private void Update()
	{
		if (hasInit && update && meshEnabled && skinnedMeshRenderer.isVisible)
		{
			StartCoroutine(UpdateDynamicMesh());
		}
	}

	public void Init(Giantess giantess, bool syncronous)
	{
		layer = Layers.gtsBodyLayer;
		this.giantess = giantess;
		skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
		giantessBones = new Dictionary<int, GiantessBone>();
		base.gameObject.layer = Layers.objectLayer;
		poseMesh = new Mesh();
		poseMesh.MarkDynamic();
		colliderReshaper = base.gameObject.AddComponent<ColliderReshaper>();
		colliderReshaper.meshMaster = this;
		colliderReshaper.skinnedMeshRenderer = skinnedMeshRenderer;
		colliderReshaper.giantess = giantess;
		StartCoroutine(InitializeDynamicMesh(syncronous));
	}

	public void UpdateCollider()
	{
		update = true;
	}

	public void UpdateLayer(int layer)
	{
		foreach (GiantessBone value in giantessBones.Values)
		{
			value.gameObject.layer = layer;
		}
	}

	public void EnableCollision(bool value)
	{
		meshEnabled = value;
		foreach (KeyValuePair<int, GiantessBone> giantessBone in giantessBones)
		{
			giantessBone.Value.colliderFront.enabled = value;
		}
	}

	public void GetTriangleData(Bone bone, int[] targetVertArray)
	{
		bone.allTriangleData = new List<TriangleData>();
		for (int i = 0; i * 3 < targetVertArray.Length; i++)
		{
			int num = i * 3;
			Vector3 vector = bone.localVertices[targetVertArray[num]];
			Vector3 vector2 = bone.localVertices[targetVertArray[num + 1]];
			Vector3 vector3 = bone.localVertices[targetVertArray[num + 2]];
			Vector3 direction = Vector3.Cross(vector - vector2, vector - vector3);
			float x = (vector.x + vector2.x + vector3.x) / 3f;
			float y = (vector.y + vector2.y + vector3.y) / 3f;
			float z = (vector.z + vector2.z + vector3.z) / 3f;
			TriangleData item = new TriangleData
			{
				localCentroidPosition = new Vector3(x, y, z),
				worldFacing = bone.transform.TransformDirection(direction).normalized,
				index = i,
				triangleVertexIds = new List<int>
				{
					targetVertArray[num],
					targetVertArray[num + 1],
					targetVertArray[num + 2]
				}
			};
			bone.allTriangleData.Add(item);
		}
	}

	public List<TriangleData> FindAllCentriodsConectedToVertex(Bone bone, int targetVertex)
	{
		List<TriangleData> list = new List<TriangleData>();
		foreach (TriangleData allTriangleDatum in bone.allTriangleData)
		{
			if (allTriangleDatum.triangleVertexIds.Contains(targetVertex))
			{
				list.Add(allTriangleDatum);
			}
		}
		return list;
	}

	private IEnumerator InitializeDynamicMesh(bool syncronous)
	{
		Mesh skinnedMesh = skinnedMeshRenderer.sharedMesh;
		BoneWeight[] boneWeights = skinnedMesh.boneWeights;
		Vector3[] vertices = skinnedMesh.vertices;
		Material[] materials = skinnedMeshRenderer.materials;
		int subMeshCount = Mathf.Min(materials.Length, skinnedMesh.subMeshCount);
		Transform[] bones = skinnedMeshRenderer.bones;
		boneIndex0 = new int[boneWeights.Length];
		for (int j = 0; j < boneIndex0.Length; j++)
		{
			boneIndex0[j] = boneWeights[j].boneIndex0;
		}
		boneData = new Bone[bones.Length];
		for (int k = 0; k < boneData.Length; k++)
		{
			Bone bone = new Bone();
			bone.transform = bones[k];
			bone.sharedTriangles = new List<int>[subMeshCount];
			bone.globalToLocalVertex = new int[vertices.Length];
			bone.vertexList = new List<int>();
			bone.trianglesArray = new int[subMeshCount][];
			for (int l = 0; l < subMeshCount; l++)
			{
				bone.sharedTriangles[l] = new List<int>();
			}
			bone.hasVertex = new bool[vertices.Length];
			boneData[k] = bone;
		}
		int[] boneAux = new int[3];
		for (int i = 0; i < subMeshCount; i++)
		{
			int[] triangles = skinnedMesh.GetTriangles(i);
			int num = triangles.Length / 3;
			for (int m = 0; m < num; m++)
			{
				int num2 = m * 3;
				if (num2 + 2 >= triangles.Length)
				{
					break;
				}
				int num3 = triangles[num2];
				int num4 = triangles[num2 + 1];
				int num5 = triangles[num2 + 2];
				int num6 = boneIndex0.Length;
				if (num3 >= num6 || num4 >= num6 || num5 >= num6)
				{
					break;
				}
				int num7 = boneIndex0[num3];
				int num8 = boneIndex0[num4];
				int num9 = boneIndex0[num5];
				int num10 = 1;
				boneAux[0] = num7;
				if (num7 != num8)
				{
					boneAux[1] = num8;
					if (num8 != num9)
					{
						boneAux[2] = num9;
						num10 = 3;
					}
					else
					{
						num10 = 2;
					}
				}
				else if (num7 != num9)
				{
					boneAux[1] = num9;
					num10 = 2;
				}
				for (int n = 0; n < num10; n++)
				{
					Bone bone2 = boneData[boneAux[n]];
					bone2.sharedTriangles[i].Add(num3);
					bone2.sharedTriangles[i].Add(num4);
					bone2.sharedTriangles[i].Add(num5);
					if (!bone2.hasVertex[num3])
					{
						bone2.hasVertex[num3] = true;
						bone2.vertexList.Add(num3);
					}
					if (!bone2.hasVertex[num4])
					{
						bone2.hasVertex[num4] = true;
						bone2.vertexList.Add(num4);
					}
					if (!bone2.hasVertex[num5])
					{
						bone2.hasVertex[num5] = true;
						bone2.vertexList.Add(num5);
					}
				}
			}
			if (!syncronous)
			{
				yield return null;
			}
		}
		for (int num11 = 0; num11 < boneData.Length; num11++)
		{
			Bone bone3 = boneData[num11];
			bone3.vertexCount = bone3.vertexList.Count;
			bone3.vertexArray = bone3.vertexList.ToArray();
			bone3.localVertices = new Vector3[bone3.vertexCount];
			for (int num12 = 0; num12 < materials.Length; num12++)
			{
				bone3.trianglesArray[num12] = bone3.sharedTriangles[num12].ToArray();
			}
		}
		hasInit = true;
		if (!syncronous)
		{
			UpdateCollider();
		}
		else
		{
			StartCoroutine(UpdateDynamicMesh(true));
		}
	}

	private IEnumerator UpdateDynamicMesh(bool syncronous = false)
	{
		update = false;
		if (!syncronous)
		{
			yield return new WaitForSeconds(0.2f);
		}
		Material[] materials = skinnedMeshRenderer.materials;
		int materialCount = materials.Length;
		bool[] isValidMaterial = new bool[materials.Length];
		for (int i = 0; i < materials.Length; i++)
		{
			if ((!materials[i].HasProperty("_Color") || materials[i].color.a > 0.05f) && materials[i].shader.name != "Sizebox/Hide")
			{
				isValidMaterial[i] = true;
			}
		}
		int bonesCount = boneData.Length;
		for (int j = 0; j < bonesCount; j++)
		{
			Bone bone = boneData[j];
			int num = 0;
			for (int k = 0; k < materialCount; k++)
			{
				if (isValidMaterial[k])
				{
					num += bone.sharedTriangles[k].Count;
				}
			}
			bone.localTriangles = new int[num];
		}
		for (int boneIndex = 0; boneIndex < bonesCount; boneIndex++)
		{
			if (!meshEnabled)
			{
				break;
			}
			Bone bone2 = boneData[boneIndex];
			if (bone2.vertexCount == 0)
			{
				continue;
			}
			Vector3 position = base.transform.position;
			Quaternion rotation = base.transform.rotation;
			int num2 = 0;
			Transform transform = bone2.transform;
			Vector3 position2 = transform.position;
			float num3 = 1f / transform.lossyScale.y;
			Quaternion quaternion = Quaternion.Inverse(transform.rotation);
			int vertexCount = bone2.vertexCount;
			skinnedMeshRenderer.BakeMesh(poseMesh);
			Vector3[] vertices = poseMesh.vertices;
			for (int l = 0; l < vertexCount; l++)
			{
				int num4 = bone2.vertexArray[l];
				Vector3 vector = rotation * vertices[num4];
				vector.x += position.x - position2.x;
				vector.y += position.y - position2.y;
				vector.z += position.z - position2.z;
				vector = quaternion * vector;
				vector.x *= num3;
				vector.y *= num3;
				vector.z *= num3;
				bone2.localVertices[num2] = vector;
				bone2.globalToLocalVertex[num4] = num2;
				num2++;
			}
			int num5 = 0;
			for (int m = 0; m < materialCount; m++)
			{
				if (isValidMaterial[m])
				{
					int[] array = bone2.trianglesArray[m];
					int num6 = array.Length;
					for (int n = 0; n < num6; n++)
					{
						int num7 = array[n];
						int num8 = bone2.globalToLocalVertex[num7];
						bone2.localTriangles[num5] = num8;
						num5++;
					}
				}
			}
			Mesh mesh = new Mesh();
			mesh.MarkDynamic();
			mesh.vertices = bone2.localVertices;
			mesh.triangles = bone2.localTriangles;
			mesh.name = "Mesh " + boneIndex;
			GetTriangleData(bone2, bone2.localTriangles);
			if (!giantessBones.ContainsKey(boneIndex))
			{
				Transform transform2 = bone2.transform;
				Transform transform3 = new GameObject("Collider_" + transform2.name).transform;
				transform3.gameObject.layer = layer;
				transform3.SetParent(transform2, false);
				GiantessBone giantessBone = transform3.gameObject.AddComponent<GiantessBone>();
				giantessBone.Initialize(giantess);
				giantessBone.colliderFront = transform3.gameObject.AddComponent<MeshCollider>();
				transform3.gameObject.AddComponent<FootCollision>();
				giantessBone.colliderFront.sharedMesh = null;
				giantessBone.colliderFront.sharedMesh = mesh;
				giantessBone.FindFarthestExtent();
				giantessBone.boneId = boneIndex;
				giantessBone.boneMaster = this;
				giantessBone.colliderReshaper = colliderReshaper;
				bone2.gtsBoneReffernce = giantessBone;
				giantessBones[boneIndex] = giantessBone;
			}
			else
			{
				GiantessBone giantessBone2 = giantessBones[boneIndex];
				giantessBone2.colliderFront.sharedMesh = null;
				giantessBone2.colliderFront.sharedMesh = mesh;
			}
			if (!syncronous)
			{
				yield return null;
			}
		}
		if (!syncronous)
		{
			yield return null;
		}
	}
}
