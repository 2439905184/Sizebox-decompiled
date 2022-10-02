using System;
using UnityEngine;

namespace SaveDataStructures
{
	[Serializable]
	public class PlayerSaveData
	{
		public int controlledEntityId;

		public GameMode cameraState;

		public Vector3 cameraVirtualPosition;

		public Quaternion cameraRotation;

		public PlayerSaveData(EntityBase playerEntity)
		{
			cameraState = GameController.Instance.mode;
			Transform transform = GameController.Instance.transform;
			cameraVirtualPosition = transform.position.ToVirtual();
			cameraRotation = transform.rotation;
			if ((bool)playerEntity)
			{
				controlledEntityId = playerEntity.id;
			}
			else
			{
				controlledEntityId = -1;
			}
		}
	}
}
