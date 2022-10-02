using System;
using UnityEngine;

[Serializable]
public class PlayerControl
{
	[Header("Movement Controllers")]
	[SerializeField]
	private NewPlayerMicroController microController;

	[SerializeField]
	private PlayerGiantessController giantessController;

	public NewPlayerMicroController MicroController
	{
		get
		{
			return microController;
		}
	}

	public PlayerGiantessController GiantessController
	{
		get
		{
			return giantessController;
		}
	}

	public BaseMovementController ActiveMovementController { get; private set; }

	public float WalkSpeed
	{
		get
		{
			return microController.Settings.walkSpeed;
		}
		set
		{
			microController.Settings.walkSpeed = value;
		}
	}

	public float RunSpeed
	{
		get
		{
			return microController.Settings.runSpeed;
		}
		set
		{
			microController.Settings.runSpeed = value;
		}
	}

	public float SprintSpeed
	{
		get
		{
			return microController.Settings.sprintSpeed;
		}
		set
		{
			microController.Settings.sprintSpeed = value;
		}
	}

	public float FlySpeed
	{
		get
		{
			return microController.Settings.flySpeed;
		}
		set
		{
			microController.Settings.flySpeed = value;
		}
	}

	public float SuperSpeed
	{
		get
		{
			return microController.Settings.superSpeed;
		}
		set
		{
			microController.Settings.superSpeed = value;
		}
	}

	public float ClimbSpeed
	{
		get
		{
			return microController.Settings.climbSpeed;
		}
		set
		{
			microController.Settings.climbSpeed = value;
		}
	}

	public float JumpHeight
	{
		get
		{
			return microController.Settings.jumpHeight;
		}
		set
		{
			microController.Settings.jumpHeight = value;
		}
	}

	public bool IsClimbing
	{
		get
		{
			if (ActiveMovementController == MicroController)
			{
				return MicroController.IsClimbing;
			}
			return false;
		}
	}

	public bool IsAiming
	{
		get
		{
			if (ActiveMovementController == MicroController)
			{
				return MicroController.IsAiming;
			}
			return false;
		}
	}

	public bool AutoWalk
	{
		get
		{
			if (ActiveMovementController == MicroController)
			{
				return MicroController.autoWalk;
			}
			return false;
		}
		set
		{
			if (ActiveMovementController == MicroController)
			{
				MicroController.autoWalk = value;
			}
		}
	}

	public void SetEntity(EntityBase entity)
	{
		LuaManager.Instance.UpdatePlayerEntity(entity);
		if (!entity)
		{
			DisableActiveController();
		}
		else if ((bool)(entity as AnimatedMicroNPC))
		{
			MicroController.SetTarget(entity.transform);
			ActiveMovementController = MicroController;
		}
		else if ((bool)(entity as Giantess))
		{
			GiantessController.SetTarget(entity.transform);
			ActiveMovementController = GiantessController;
		}
	}

	public void DisableActiveController()
	{
		if ((bool)ActiveMovementController)
		{
			ActiveMovementController.SetTarget(null);
			ActiveMovementController = null;
		}
	}

	public void _DoUpdate()
	{
		if ((bool)ActiveMovementController)
		{
			ActiveMovementController.DoUpdate();
		}
	}

	public void _DoLateUpdate()
	{
		if ((bool)ActiveMovementController)
		{
			ActiveMovementController.DoLateUpdate();
		}
	}

	public void _DoFixedUpdate()
	{
		if ((bool)ActiveMovementController)
		{
			ActiveMovementController.DoFixedUpdate();
		}
	}
}
