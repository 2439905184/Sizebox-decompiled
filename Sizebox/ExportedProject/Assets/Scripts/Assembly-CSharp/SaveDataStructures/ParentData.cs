using System;
using UnityEngine;

namespace SaveDataStructures
{
	[Serializable]
	public class ParentData
	{
		public int parentEntityId = -1;

		public string transformName;

		public Vector3 localPosition;

		public Quaternion localRotation;

		public ParentData(Transform me, Transform parent)
		{
			if ((bool)parent)
			{
				EntityBase componentInParent = parent.GetComponentInParent<EntityBase>();
				if ((bool)componentInParent)
				{
					transformName = parent.name;
					parentEntityId = componentInParent.id;
					localPosition = me.localPosition;
					localRotation = me.localRotation;
				}
			}
		}
	}
}
