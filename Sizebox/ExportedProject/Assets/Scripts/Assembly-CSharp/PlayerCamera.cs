using System;
using UnityEngine;

[Serializable]
public class PlayerCamera
{
	[Header("Camera Controllers")]
	[SerializeField]
	private PlayerMicroCameraController microCamera;

	[SerializeField]
	private PlayerGiantessCameraController giantessCamera;

	public PlayerMicroCameraController MicroCamera
	{
		get
		{
			return microCamera;
		}
	}

	public PlayerGiantessCameraController GiantessCamera
	{
		get
		{
			return giantessCamera;
		}
	}

	public BaseCameraController ActiveCameraController { get; private set; }

	public void SetEntity(EntityBase entity)
	{
		if ((bool)entity)
		{
			if ((bool)(entity as AnimatedMicroNPC))
			{
				MicroCamera.SetTarget(entity.transform);
				ActiveCameraController = MicroCamera;
			}
			else if ((bool)(entity as Giantess))
			{
				GiantessCamera.SetTarget(entity.transform);
				ActiveCameraController = GiantessCamera;
			}
		}
	}

	public void DisableActiveController()
	{
		if ((bool)ActiveCameraController)
		{
			ActiveCameraController.SetTarget(null);
			ActiveCameraController = null;
		}
	}

	public void _DoLateUpdate()
	{
		if ((bool)ActiveCameraController)
		{
			ActiveCameraController.DoLateUpdate();
		}
	}
}
