using System.Collections.Generic;
using UnityEngine;

public class DynamicMeshCreator : MonoBehaviour
{
	private SkinnedMeshRenderer skinnedMeshRenderer;

	public Transform[] bones;

	private void Start()
	{
		skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
		Mesh sharedMesh = skinnedMeshRenderer.sharedMesh;
		Mesh mesh = new Mesh();
		skinnedMeshRenderer.BakeMesh(mesh);
		bones = skinnedMeshRenderer.bones;
		int num = bones.Length;
		int[] triangles = mesh.triangles;
		Vector3[] vertices = mesh.vertices;
		BoneWeight[] boneWeights = sharedMesh.boneWeights;
		int num2 = triangles.Length / 3;
		List<int>[] array = new List<int>[num];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new List<int>();
		}
		bool[,] array2 = new bool[num, vertices.Length];
		for (int j = 0; j < num; j++)
		{
			for (int k = 0; k < vertices.Length; k++)
			{
				array2[j, k] = false;
			}
		}
		int num3 = 0;
		for (int l = 0; l < num2; l++)
		{
			int num4 = l * 3;
			int num5 = triangles[num4];
			int num6 = triangles[num4 + 1];
			int num7 = triangles[num4 + 2];
			int boneIndex = boneWeights[num5].boneIndex0;
			int boneIndex2 = boneWeights[num6].boneIndex0;
			int boneIndex3 = boneWeights[num7].boneIndex0;
			int[] array3 = new int[3] { boneIndex, -1, -1 };
			if (boneIndex != boneIndex2)
			{
				array3[1] = boneIndex2;
				if (boneIndex2 != boneIndex3)
				{
					array3[2] = boneIndex3;
				}
			}
			else if (boneIndex != boneIndex3)
			{
				array3[1] = boneIndex3;
			}
			for (int m = 0; m < array3.Length && array3[m] > -1; m++)
			{
				int num8 = array3[m];
				array[num8].Add(num5);
				array[num8].Add(num6);
				array[num8].Add(num7);
				array2[num8, num5] = true;
				array2[num8, num6] = true;
				array2[num8, num7] = true;
				num3++;
			}
		}
		Debug.Log(num3 + " faces found");
		int[] array4 = new int[num];
		for (int n = 0; n < num; n++)
		{
			array4[n] = 0;
		}
		int[,] array5 = new int[num, vertices.Length];
		for (int num9 = 0; num9 < num; num9++)
		{
			for (int num10 = 0; num10 < vertices.Length; num10++)
			{
				array5[num9, num10] = -1;
				if (array2[num9, num10])
				{
					array4[num9]++;
				}
			}
		}
		List<Vector3[]> list = new List<Vector3[]>();
		List<int[]> list2 = new List<int[]>();
		int[] array6 = new int[num];
		for (int num11 = 0; num11 < num; num11++)
		{
			list.Add(new Vector3[array4[num11]]);
			array6[num11] = 0;
			list2.Add(new int[array[num11].Count]);
		}
		Debug.Log("bone count: " + num);
		for (int num12 = 0; num12 < num; num12++)
		{
			int num13 = 0;
			for (int num14 = 0; num14 < vertices.Length; num14++)
			{
				if (array2[num12, num14])
				{
					if (num13 == 0)
					{
						Debug.Log("Initialized bone: " + num12);
					}
					else if (num13 >= list[num12].Length - 1)
					{
						Debug.Log("Completed Bone: " + num12);
					}
					list[num12][num13] = bones[num12].InverseTransformPoint(base.transform.position + base.transform.rotation * vertices[num14]);
					array5[num12, num14] = num13;
					num13++;
				}
			}
		}
		float y = base.transform.localScale.y;
		base.transform.localScale = Vector3.one;
		for (int num15 = 0; num15 < num; num15++)
		{
			if (array4[num15] < 1)
			{
				continue;
			}
			Mesh mesh2 = new Mesh();
			for (int num16 = 0; num16 < array[num15].Count; num16++)
			{
				int num17 = array[num15][num16];
				int num18 = array5[num15, num17];
				if (num18 > list[num15].Length)
				{
					Debug.Log("Error in bone: " + num15 + " new value: " + num18 + " is bigger than the vertex count: " + list[num15].Length);
				}
				list2[num15][num16] = num18;
			}
			mesh2.vertices = list[num15];
			mesh2.triangles = list2[num15];
			mesh2.name = "Mesh " + num15;
			MeshCollider meshCollider = bones[num15].gameObject.AddComponent<MeshCollider>();
			meshCollider.sharedMesh = null;
			meshCollider.sharedMesh = mesh2;
		}
		base.transform.localScale = Vector3.one * y;
	}
}
