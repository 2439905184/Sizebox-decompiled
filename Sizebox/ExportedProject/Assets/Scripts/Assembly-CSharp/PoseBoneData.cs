using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class PoseBoneData
{
	[FormerlySerializedAs("localPostion")]
	public Vector3 localPosition;

	public Quaternion localRotation;

	public PoseBoneData(EntityBase entity, Vector3 position, Quaternion rotation)
	{
		Transform transform = entity.model.transform;
		localPosition = transform.InverseTransformPoint(position);
		localRotation = Quaternion.Inverse(transform.rotation) * rotation;
	}
}
