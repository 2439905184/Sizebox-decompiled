using System;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
	[Header("Required References")]
	[SerializeField]
	private ResizeCharacter resizer;

	[SerializeField]
	private PlayerControl moveControl = new PlayerControl();

	[SerializeField]
	private PlayerCamera cameraControl = new PlayerCamera();

	public float minSize;

	public float maxSize;

	public PlayerControl Control
	{
		get
		{
			return moveControl;
		}
	}

	public PlayerCamera Camera
	{
		get
		{
			return cameraControl;
		}
	}

	public EntityBase Entity { get; private set; }

	public float Scale
	{
		get
		{
			if ((bool)Entity)
			{
				return Entity.Scale;
			}
			return 1.65f;
		}
	}

	private void OnEntityDeleted(EntityBase entity)
	{
		if (entity == Entity)
		{
			StopPlayingEntity();
		}
	}

	private void Start()
	{
		InputManager.Instance.EnableControls(InputManager.inputs.Player.Get(), GameMode.Play);
		EntityBase.EntityDeleted = (UnityAction<EntityBase>)Delegate.Combine(EntityBase.EntityDeleted, new UnityAction<EntityBase>(OnEntityDeleted));
	}

	private void OnDestroy()
	{
		EntityBase.EntityDeleted = (UnityAction<EntityBase>)Delegate.Remove(EntityBase.EntityDeleted, new UnityAction<EntityBase>(OnEntityDeleted));
		InputManager.Instance.DisableControls(InputManager.inputs.Player.Get(), GameMode.Play);
	}

	public PlayerRaygun GetRaygun()
	{
		return moveControl.MicroController.GetRaygun();
	}

	public bool PlayAs(IPlayable playable)
	{
		if (playable == null || playable.IsPlayerControlled)
		{
			return false;
		}
		if (playable.StartPlayerControl(this))
		{
			StopPlayingEntity();
			SetEntity(playable.Entity);
			return true;
		}
		return false;
	}

	public void StopPlayingEntity()
	{
		if ((bool)Entity)
		{
			Entity.GetComponent<IPlayable>().OnPlayerControlEnd(this);
		}
		base.transform.parent = null;
		SetEntity(null);
	}

	private void SetEntity(EntityBase entity)
	{
		moveControl.DisableActiveController();
		cameraControl.DisableActiveController();
		Entity = entity;
		resizer.SetEntity(Entity);
		moveControl.SetEntity(entity);
		cameraControl.SetEntity(entity);
		GameController.Instance.SetMode();
	}

	private void Update()
	{
		moveControl._DoUpdate();
	}

	private void LateUpdate()
	{
		cameraControl._DoLateUpdate();
		moveControl._DoLateUpdate();
	}

	private void FixedUpdate()
	{
		moveControl._DoFixedUpdate();
	}
}
