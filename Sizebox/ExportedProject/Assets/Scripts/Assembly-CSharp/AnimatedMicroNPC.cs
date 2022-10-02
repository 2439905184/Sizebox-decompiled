using System.Runtime.CompilerServices;
using SaveDataStructures;
using UnityEngine;

public class AnimatedMicroNPC : MicroNpc, IPlayable, IEntity, IGameObject, ISavable
{
	public MicroLookAtController lookController;

	private float _diffAngle;

	public Player Player { get; private set; }

	public bool IsPlayerControlled { get; private set; }

	protected override void InitializeModel(GameObject modelPrefab)
	{
		base.InitializeModel(modelPrefab);
		if (base.Animator != null && lookController == null && (bool)base.Animator.GetBoneTransform(HumanBodyBones.Head))
		{
			_diffAngle = Vector3.Angle(base.transform.forward, base.Animator.GetBoneTransform(HumanBodyBones.Head).forward);
			if (Mathf.Abs(_diffAngle) > 5f)
			{
				lookController = base.model.AddComponent<ComplexMicroLookAtController>();
			}
			else
			{
				lookController = base.model.AddComponent<MicroLookAtController>();
			}
		}
	}

	protected override void FinishInitialization()
	{
		base.FinishInitialization();
		if ((bool)Player)
		{
			Player.PlayAs(this);
		}
	}

	public void LookAt(EntityBase target)
	{
		if (base.Animator != null && lookController != null && !base.isCrushed)
		{
			if (!lookController.enabled)
			{
				lookController.enabled = true;
			}
			lookController.LookAt(target);
		}
	}

	protected override void OnDestroy()
	{
		ai.DisableAI();
		base.OnDestroy();
	}

	public bool StartPlayerControl(Player player)
	{
		if (IsPlayerControlled || player == null)
		{
			return false;
		}
		Player = player;
		if (!base.Initialized)
		{
			return false;
		}
		ActionManager.ClearAll();
		ai.behaviorController.StopMainBehavior();
		ai.DisableAI();
		if ((bool)base.Animator)
		{
			base.Animator.SetRuntimeController(IOManager.Instance.playerAnimatorController);
		}
		IsPlayerControlled = true;
		isPlayer = true;
		return true;
	}

	public void OnPlayerControlEnd(Player player)
	{
		if (!(Player != player))
		{
			if ((bool)base.Animator)
			{
				base.Animator.SetRuntimeController(IOManager.Instance.microAnimatorController);
			}
			Player = null;
			IsPlayerControlled = false;
			isPlayer = false;
		}
	}

	public override SavableData Save()
	{
		return new MicroSaveData(this);
	}

	public override void Load(SavableData savedData, bool loadPosition = true)
	{
		base.Load(savedData, loadPosition);
	}

	[SpecialName]
	GameObject IGameObject.get_gameObject()
	{
		return base.gameObject;
	}

	[SpecialName]
	Transform IGameObject.get_transform()
	{
		return base.transform;
	}
}
