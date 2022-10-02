using UnityEngine;

public abstract class MicroNpc : Micro
{
	private AIShooterController _shooterController;

	protected override void Awake()
	{
		base.Awake();
		if (GlobalPreferences.MicroAI.value)
		{
			ai.EnableAI();
		}
	}

	protected override void InitializeModel(GameObject modelPrefab)
	{
		base.InitializeModel(modelPrefab);
		if ((bool)base.Animator)
		{
			base.Animator.SetRuntimeController(IOManager.Instance.microAnimatorController);
		}
	}

	protected override void OnDestroy()
	{
		Object.Destroy(GetComponent<MicroObstacleDetector>());
		Object.Destroy(GetComponent<LODGroup>());
		Object.Destroy(GetComponent<MovementCharacter>());
		if ((bool)_shooterController)
		{
			Object.Destroy(_shooterController);
		}
		base.OnDestroy();
	}

	public void Crush()
	{
		Crush(null, null, null);
	}

	protected override void Crush(Collision collisionData = null, EntityBase entity = null, Collider crushingCollider = null)
	{
		if (GlobalPreferences.CrushNpcEnabled.value || isPlayer)
		{
			base.Movement.enabled = false;
			ai.enabled = false;
			if ((bool)_shooterController)
			{
				_shooterController.UnequipGun();
			}
			base.Crush(collisionData, entity, crushingCollider);
		}
	}

	public override void StandUp()
	{
		base.Movement.enabled = true;
		ai.enabled = true;
		base.StandUp();
	}

	private void Update()
	{
		if (!base.isCrushed && !isPlayer && !Dead && !base.RagDollEnabled)
		{
			Transform obj = base.transform;
			Quaternion rotation = obj.rotation;
			Quaternion b = Quaternion.FromToRotation(obj.up, Vector3.up) * rotation;
			obj.rotation = Quaternion.Slerp(rotation, b, Time.deltaTime * 3f);
		}
	}

	private void AddShooterController()
	{
		_shooterController = base.gameObject.AddComponent<AIShooterController>();
	}

	private bool CanEquipGun()
	{
		if (!base.Animator || !base.Animator.GetBoneTransform(HumanBodyBones.RightHand))
		{
			Debug.Log("Micro doesn't have enough bones to equip gun.");
			return false;
		}
		return base.Animator.GetBoneTransform(HumanBodyBones.RightHand) != null;
	}

	public AIShooterController GetShooterController(bool createIfNecessary)
	{
		if (!CanEquipGun())
		{
			return null;
		}
		if (_shooterController == null && createIfNecessary)
		{
			AddShooterController();
		}
		return _shooterController;
	}

	public void EquipRaygun()
	{
		if (CanEquipGun())
		{
			if (_shooterController == null)
			{
				AddShooterController();
			}
			_shooterController.EquipRaygun();
		}
	}

	public void EquipSmg()
	{
		if (CanEquipGun())
		{
			if (_shooterController == null)
			{
				AddShooterController();
			}
			_shooterController.EquipSMG();
		}
	}

	public void HolsterGun()
	{
		if ((bool)_shooterController)
		{
			_shooterController.UnequipGun();
		}
	}
}
