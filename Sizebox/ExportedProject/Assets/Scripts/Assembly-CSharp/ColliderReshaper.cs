using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderReshaper : MonoBehaviour
{
	public SkinnedMeshCollider meshMaster;

	public SkinnedMeshRenderer skinnedMeshRenderer;

	public Giantess giantess;

	private bool _updatingIndividualBoneMeshes;

	public GameObject crushReferenceTargeter;

	public GameObject crushReferenceSubTargeter;

	public Vector3 crushReferenceSubTargeterDistance;

	private float boneDistanceCheckMultiplier = 1.5f;

	private void Update()
	{
		if (!crushReferenceTargeter)
		{
			CreateCrushPositionTargeter();
		}
		if (GlobalPreferences.UseAdvancedCollision.value && meshMaster.boneData != null)
		{
			RepositionStuckTransforms();
		}
		if (!_updatingIndividualBoneMeshes && GlobalPreferences.UseAdvancedCollision.value && meshMaster.hasInit && GameController.Instance.mode != GameMode.Edit)
		{
			StartCoroutine(UpdateIndividualBoneMeshes());
		}
	}

	private IEnumerator UpdateIndividualBoneMeshes()
	{
		List<GiantessBone> list = new List<GiantessBone>();
		foreach (KeyValuePair<int, GiantessBone> giantessBone in meshMaster.giantessBones)
		{
			list.Add(giantessBone.Value);
		}
		if (GameController.Instance.transform.lossyScale.x * GlobalPreferences.MacroColliderUpdateDistance.value < Vector3.Distance(GameController.Instance.transform.position, giantess.transform.position))
		{
			foreach (GiantessBone item in list)
			{
				if (item.rigidBody.collisionDetectionMode != 0)
				{
					item.SetCollisionDetectionMode(CollisionDetectionMode.Discrete);
				}
			}
			_updatingIndividualBoneMeshes = false;
			yield break;
		}
		_updatingIndividualBoneMeshes = true;
		List<GiantessBone> bonesToUpdate = new List<GiantessBone>();
		int proximitiesChecked = 0;
		foreach (GiantessBone item2 in list)
		{
			float radius = item2.farthestExtent * (giantess.transform.lossyScale.y * 20f) * boneDistanceCheckMultiplier;
			if (item2.transform.childCount > 0)
			{
				if (item2.rigidBody.collisionDetectionMode != CollisionDetectionMode.ContinuousSpeculative)
				{
					item2.SetCollisionDetectionMode(CollisionDetectionMode.ContinuousSpeculative);
				}
				bonesToUpdate.Add(item2);
			}
			else if (Physics.CheckSphere(item2.transform.TransformPoint(item2.localCenterOfMesh), radius, Layers.crushableMask) || item2.transform.childCount > 0)
			{
				if (item2.rigidBody.collisionDetectionMode != CollisionDetectionMode.ContinuousSpeculative)
				{
					item2.SetCollisionDetectionMode(CollisionDetectionMode.ContinuousSpeculative);
				}
				bonesToUpdate.Add(item2);
			}
			else if (item2.rigidBody.collisionDetectionMode != 0)
			{
				item2.SetCollisionDetectionMode(CollisionDetectionMode.Discrete);
			}
			proximitiesChecked++;
			if (proximitiesChecked > GlobalPreferences.MeshCheckLimit.value)
			{
				proximitiesChecked = 0;
				yield return null;
			}
		}
		int bonesUpdated = 0;
		foreach (GiantessBone item3 in bonesToUpdate)
		{
			UpdateSingleDynamicMesh(item3);
			bonesUpdated++;
			if (bonesUpdated > GlobalPreferences.MeshUpdateLimit.value)
			{
				bonesUpdated = 0;
				yield return null;
			}
		}
		_updatingIndividualBoneMeshes = false;
		yield return null;
	}

	public void CreateCrushPositionTargeter()
	{
		crushReferenceSubTargeterDistance = new Vector3(0f, 10000f * giantess.AccurateScale, 0f);
		crushReferenceTargeter = Object.Instantiate(new GameObject("Transform_Targeter"), base.transform);
		crushReferenceSubTargeter = Object.Instantiate(new GameObject("Sub_Targeter"), crushReferenceTargeter.transform);
		crushReferenceSubTargeter.transform.localPosition = crushReferenceSubTargeterDistance;
		crushReferenceSubTargeter.transform.localRotation = Quaternion.identity;
	}

	private void UpdateSingleDynamicMesh(GiantessBone boneScript)
	{
		int boneId = boneScript.boneId;
		Material[] materials = skinnedMeshRenderer.materials;
		int num = materials.Length;
		bool[] array = new bool[materials.Length];
		for (int i = 0; i < materials.Length; i++)
		{
			if ((!materials[i].HasProperty("_Color") || materials[i].color.a > 0.05f) && materials[i].shader.name != "Sizebox/Hide")
			{
				array[i] = true;
			}
		}
		SkinnedMeshCollider.Bone bone = meshMaster.boneData[boneId];
		Transform obj = base.transform;
		Vector3 position = obj.position;
		Quaternion rotation = obj.rotation;
		int num2 = 0;
		Transform transform = bone.transform;
		Vector3 position2 = transform.position;
		float num3 = 1f / transform.lossyScale.y;
		Quaternion quaternion = Quaternion.Inverse(transform.rotation);
		int vertexCount = bone.vertexCount;
		skinnedMeshRenderer.BakeMesh(meshMaster.poseMesh);
		Vector3[] vertices = meshMaster.poseMesh.vertices;
		for (int j = 0; j < vertexCount; j++)
		{
			int num4 = bone.vertexArray[j];
			Vector3 vector = rotation * vertices[num4];
			vector.x += position.x - position2.x;
			vector.y += position.y - position2.y;
			vector.z += position.z - position2.z;
			vector = quaternion * vector;
			vector.x *= num3;
			vector.y *= num3;
			vector.z *= num3;
			bone.localVertices[num2] = vector;
			bone.globalToLocalVertex[num4] = num2;
			num2++;
		}
		int num5 = 0;
		for (int k = 0; k < num; k++)
		{
			if (array[k])
			{
				int[] array2 = bone.trianglesArray[k];
				int num6 = array2.Length;
				for (int l = 0; l < num6; l++)
				{
					int num7 = array2[l];
					int num8 = bone.globalToLocalVertex[num7];
					bone.localTriangles[num5] = num8;
					num5++;
				}
			}
		}
		Mesh mesh = new Mesh();
		mesh.MarkDynamic();
		mesh.vertices = bone.localVertices;
		mesh.triangles = bone.localTriangles;
		meshMaster.GetTriangleData(bone, bone.localTriangles);
		GiantessBone giantessBone = meshMaster.giantessBones[boneId];
		giantessBone.colliderFront.sharedMesh = null;
		giantessBone.colliderFront.sharedMesh = mesh;
		foreach (GiantessBoneParenting.CrushCollection crushDataCluster in boneScript.boneParentingScript.crushDataClusters)
		{
			SetRepositionTarget(crushDataCluster, bone);
		}
	}

	private void SetRepositionTarget(GiantessBoneParenting.CrushCollection crushData, SkinnedMeshCollider.Bone bone)
	{
		if (crushData.ready)
		{
			Vector3 vector = bone.localVertices[crushData.parentVertexId];
			Vector3 localCentroidPosition = bone.allTriangleData[crushData.parentCentroidId].localCentroidPosition;
			Vector3 vector2 = Vector3.Lerp(vector, localCentroidPosition, crushData.percentageAwayFromVertex);
			crushData.parentVertexCurrentPosition = vector;
			crushData.parentCentroidCurrentPosition = localCentroidPosition;
			crushData.targetPosition = vector2 - crushData.microLocalOffset;
			Transform transform = crushData.rotationTrackerObject.transform;
			transform.localPosition = vector2;
			transform.up = bone.allTriangleData[crushData.parentCentroidId].worldFacing;
			Vector3 position = transform.InverseTransformPoint(bone.transform.TransformPoint(bone.localVertices[crushData.footAnchorVertex]));
			position.y = 0f;
			transform.LookAt(transform.TransformPoint(position), transform.up);
		}
	}

	private void RepositionStuckTransforms()
	{
		SkinnedMeshCollider.Bone[] boneData = meshMaster.boneData;
		foreach (SkinnedMeshCollider.Bone bone in boneData)
		{
			if (!(bone.gtsBoneReffernce != null) || bone.gtsBoneReffernce.boneParentingScript.crushDataClusters == null)
			{
				continue;
			}
			List<GiantessBoneParenting.CrushCollection> list = new List<GiantessBoneParenting.CrushCollection>();
			for (int j = 0; j < bone.gtsBoneReffernce.boneParentingScript.crushDataClusters.Count; j++)
			{
				GiantessBoneParenting.CrushCollection crushCollection = bone.gtsBoneReffernce.boneParentingScript.crushDataClusters[j];
				if ((bool)crushCollection.micro || (bool)crushCollection.blood)
				{
					if (!GameController.FreezeGts)
					{
						float maxDistanceDelta = GlobalPreferences.MicroRepositionSpeed.value * GameController.macroSpeed * Time.deltaTime;
						if ((bool)crushCollection.micro)
						{
							Transform obj = crushCollection.micro.transform;
							obj.localPosition = Vector3.MoveTowards(crushCollection.micro.transform.localPosition, crushCollection.targetPosition, maxDistanceDelta);
							obj.rotation = crushCollection.rotationSubTracker.transform.rotation;
						}
						if ((bool)crushCollection.blood)
						{
							crushCollection.blood.transform.localPosition = Vector3.MoveTowards(crushCollection.blood.transform.localPosition, crushCollection.targetPosition, maxDistanceDelta);
							crushCollection.blood.transform.rotation = crushCollection.rotationSubTracker.transform.rotation;
						}
					}
					else
					{
						if (crushCollection.micro != null)
						{
							Transform obj2 = crushCollection.micro.transform;
							obj2.localPosition = crushCollection.targetPosition;
							obj2.rotation = crushCollection.rotationSubTracker.transform.rotation;
						}
						if (crushCollection.blood != null)
						{
							crushCollection.blood.transform.localPosition = crushCollection.targetPosition;
							crushCollection.blood.transform.rotation = crushCollection.rotationSubTracker.transform.rotation;
						}
					}
				}
				else
				{
					list.Add(crushCollection);
				}
			}
			for (int k = 0; k < list.Count; k++)
			{
				Object.Destroy(list[k].rotationTrackerObject);
				bone.gtsBoneReffernce.boneParentingScript.crushDataClusters.Remove(list[k]);
			}
		}
	}
}
