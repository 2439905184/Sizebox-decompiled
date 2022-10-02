using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class PoseTargetData
{
	[FormerlySerializedAs("localPostion")]
	public Vector3 localPosition;

	public PoseTargetData(EntityBase entity, Vector3 position)
	{
		Transform transform = entity.model.transform;
		localPosition = transform.InverseTransformPoint(position);
	}
}
