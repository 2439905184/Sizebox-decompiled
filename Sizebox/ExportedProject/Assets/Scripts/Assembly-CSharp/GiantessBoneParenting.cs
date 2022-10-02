using System.Collections.Generic;
using UnityEngine;

public class GiantessBoneParenting : MonoBehaviour
{
	public class CrushCollection
	{
		public Micro micro;

		public GameObject blood;

		public GameObject rotationTrackerObject;

		public GameObject rotationSubTracker;

		public Vector3 targetPosition;

		public Vector3 parentVertexCurrentPosition;

		public Vector3 parentCentroidCurrentPosition;

		public Vector3 startLocalPosition;

		public Vector3 microLocalOffset;

		public int parentVertexId;

		public int parentCentroidId;

		public int footAnchorVertex;

		public float percentageAwayFromVertex;

		public bool ready;
	}

	public int boneId;

	public SkinnedMeshCollider boneMaster;

	public ColliderReshaper colliderReshaper;

	public List<CrushCollection> crushDataClusters = new List<CrushCollection>();

	public void BeginParentingStuckObject(Transform startLocation, Micro micro = null, GameObject blood = null)
	{
		CrushCollection crushCollection = new CrushCollection();
		crushCollection.startLocalPosition = startLocation.localPosition;
		if ((bool)blood)
		{
			crushCollection.blood = blood;
		}
		Transform targetTransform;
		if ((bool)micro)
		{
			crushCollection.micro = micro;
			targetTransform = micro.transform;
			micro.CrushData = crushCollection;
		}
		else
		{
			if (!blood)
			{
				return;
			}
			targetTransform = blood.transform;
		}
		GetParentingPositioningReferences(crushCollection, targetTransform);
		crushCollection.ready = true;
		crushDataClusters.Add(crushCollection);
	}

	private void GetParentingPositioningReferences(CrushCollection crushData, Transform targetTransform)
	{
		SkinnedMeshCollider.Bone bone = boneMaster.boneData[boneId];
		crushData.targetPosition = targetTransform.localPosition;
		crushData.parentVertexId = FindClosestVector3Index(crushData.targetPosition, bone.localVertices);
		crushData.parentVertexCurrentPosition = bone.localVertices[crushData.parentVertexId];
		List<SkinnedMeshCollider.TriangleData> listOfTrianglesToSort = boneMaster.FindAllCentriodsConectedToVertex(bone, crushData.parentVertexId);
		PlaceCentroidTransformObject(targetTransform, crushData, listOfTrianglesToSort, bone);
		FindPercentageOfMicroDistanceFromVertex(targetTransform, crushData);
		Vector3 vector = Vector3.Lerp(crushData.parentVertexCurrentPosition, crushData.parentCentroidCurrentPosition, crushData.percentageAwayFromVertex);
		crushData.microLocalOffset = vector - crushData.startLocalPosition;
	}

	private int FindClosestVector3Index(Vector3 target, Vector3[] arrayOfVectorsToSort)
	{
		float num = float.PositiveInfinity;
		int result = 0;
		int num2 = 0;
		foreach (Vector3 b in arrayOfVectorsToSort)
		{
			float num3 = Vector3.Distance(target, b);
			if (num3 < num)
			{
				num = num3;
				result = num2;
			}
			num2++;
		}
		return result;
	}

	private int FindFurthestVector3(Vector3 target, List<Vector3> arrayOfVectorsToSort)
	{
		float num = 0f;
		int result = 0;
		int num2 = 0;
		foreach (Vector3 item in arrayOfVectorsToSort)
		{
			float num3 = Vector3.Distance(target, item);
			if (num3 > num)
			{
				num = num3;
				result = num2;
			}
			num2++;
		}
		return result;
	}

	private void PlaceCentroidTransformObject(Transform stuckTransform, CrushCollection crushData, List<SkinnedMeshCollider.TriangleData> listOfTrianglesToSort, SkinnedMeshCollider.Bone bone)
	{
		if (!colliderReshaper.crushReferenceTargeter)
		{
			colliderReshaper.CreateCrushPositionTargeter();
		}
		Transform transform = colliderReshaper.crushReferenceTargeter.transform;
		Transform transform2 = colliderReshaper.crushReferenceSubTargeter.transform;
		transform.parent = stuckTransform;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		transform2.localPosition = colliderReshaper.crushReferenceSubTargeterDistance;
		Vector3 position = transform2.position;
		transform.parent = stuckTransform.parent;
		List<Vector3> list = new List<Vector3>();
		foreach (SkinnedMeshCollider.TriangleData item in listOfTrianglesToSort)
		{
			transform.localPosition = item.localCentroidPosition;
			transform.up = item.worldFacing;
			transform2.localPosition = colliderReshaper.crushReferenceSubTargeterDistance;
			list.Add(transform2.position);
		}
		int index = FindClosestVector3Index(position, list.ToArray());
		SkinnedMeshCollider.TriangleData triangleData = listOfTrianglesToSort[index];
		List<Vector3> list2 = new List<Vector3>();
		foreach (int triangleVertexId in triangleData.triangleVertexIds)
		{
			list2.Add(bone.localVertices[triangleVertexId]);
		}
		crushData.footAnchorVertex = FindFurthestVector3(stuckTransform.localPosition, list2);
		GameObject gameObject = Object.Instantiate(new GameObject("Rotation_Tracker" + triangleData.index), bone.gtsBoneReffernce.transform);
		gameObject.transform.localPosition = triangleData.localCentroidPosition;
		gameObject.transform.up = triangleData.worldFacing;
		Vector3 position2 = gameObject.transform.InverseTransformPoint(base.transform.TransformPoint(bone.localVertices[crushData.footAnchorVertex]));
		position2.y = 0f;
		gameObject.transform.LookAt(gameObject.transform.TransformPoint(position2), gameObject.transform.up);
		crushData.rotationTrackerObject = gameObject;
		crushData.parentCentroidCurrentPosition = gameObject.transform.localPosition;
		crushData.parentCentroidId = triangleData.index;
		GameObject gameObject2 = Object.Instantiate(new GameObject("Sub_Rotation_Tracker"), gameObject.transform);
		gameObject2.transform.localPosition = Vector3.zero;
		gameObject2.transform.rotation = stuckTransform.rotation;
		crushData.rotationSubTracker = gameObject2;
		transform.parent = boneMaster.transform;
	}

	private void FindPercentageOfMicroDistanceFromVertex(Transform stuckTransform, CrushCollection crushData)
	{
		Vector3 parentVertexCurrentPosition = crushData.parentVertexCurrentPosition;
		Vector3 localPosition = stuckTransform.localPosition;
		Vector3 localPosition2 = crushData.rotationTrackerObject.transform.localPosition;
		float num = Vector3.Distance(parentVertexCurrentPosition, localPosition);
		float num2 = Vector3.Distance(parentVertexCurrentPosition, localPosition2);
		float percentageAwayFromVertex = ((!(num > num2)) ? (num / num2) : 0f);
		crushData.percentageAwayFromVertex = percentageAwayFromVertex;
	}
}
