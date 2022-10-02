using UnityEngine;
using UnityEngine.Events;

public class InterfaceControl : MonoBehaviour
{
	public static InterfaceControl instance;

	public bool commandEnabled = true;

	private float _lastRotationX;

	private float _lastRotationY;

	private float _lastRotationZ;

	public UnityAction onSelected;

	public UnityAction<EntityBase> onDeselect;

	private float giantessOffsetDivisor = 3f;

	public EntityBase selectedEntity { get; private set; }

	public Humanoid humanoid { get; private set; }

	public Giantess giantess { get; private set; }

	public string[] animations { get; private set; }

	public bool lockRotation { get; private set; }

	public float lastMicroScale { get; private set; }

	public float lastMacroScale { get; private set; }

	private IOManager modelManager { get; set; }

	public float Grounder
	{
		get
		{
			if ((bool)selectedEntity)
			{
				return selectedEntity.GetGrounderWeight();
			}
			return 0f;
		}
		set
		{
			if ((bool)selectedEntity)
			{
				selectedEntity.ChangeGrounderWeight(value);
			}
		}
	}

	private void Awake()
	{
		instance = this;
		modelManager = IOManager.Instance;
		lastMacroScale = MapSettingInternal.gtsStartingScale;
		lastMicroScale = MapSettingInternal.startingSize;
		lockRotation = false;
		SetSelectedObject(null);
		animations = modelManager.GetAnimationList();
	}

	public void SetSelectedObject(EntityBase obj)
	{
		if (!(obj == selectedEntity))
		{
			if ((bool)selectedEntity)
			{
				onDeselect(selectedEntity);
			}
			selectedEntity = obj;
			if (!selectedEntity)
			{
				humanoid = null;
				giantess = null;
			}
			else
			{
				humanoid = selectedEntity.GetComponent<Humanoid>();
				giantess = selectedEntity.GetComponent<Giantess>();
			}
			if (onSelected != null)
			{
				onSelected();
				EventManager.SendEvent(new LocalSelectionChanged());
			}
		}
	}

	public float GetYRotation()
	{
		if ((bool)selectedEntity)
		{
			_lastRotationY = 0f;
		}
		return 0f;
	}

	public float GetXRotation()
	{
		if ((bool)selectedEntity)
		{
			_lastRotationX = 0f;
		}
		return 0f;
	}

	public float GetZRotation()
	{
		if ((bool)selectedEntity)
		{
			_lastRotationZ = 0f;
		}
		return 0f;
	}

	public void RotateYAxis(float angle)
	{
		if ((bool)selectedEntity)
		{
			selectedEntity.ChangeRotation(new Vector3(0f, _lastRotationY - angle, 0f));
			_lastRotationY = angle;
		}
	}

	public void RotateXAxis(float angle)
	{
		if ((bool)selectedEntity)
		{
			selectedEntity.ChangeRotation(new Vector3(_lastRotationX - angle, 0f, 0f));
			_lastRotationX = angle;
		}
	}

	public void RotateZAxis(float angle)
	{
		if ((bool)selectedEntity)
		{
			selectedEntity.ChangeRotation(new Vector3(0f, 0f, _lastRotationZ - angle));
			_lastRotationZ = angle;
		}
	}

	public void SetScale(float scale)
	{
		if ((bool)selectedEntity)
		{
			scale /= 100f;
			float newScale = Mathf.Pow(10f, scale);
			selectedEntity.ChangeScale(newScale);
			if (selectedEntity.isGiantess)
			{
				lastMacroScale = selectedEntity.Scale;
			}
			else
			{
				lastMicroScale = selectedEntity.Scale;
			}
		}
	}

	public float GetScale()
	{
		if ((bool)selectedEntity)
		{
			return Mathf.Log10(selectedEntity.transform.lossyScale.y) * 100f;
		}
		return 1f;
	}

	public float GetYAxisOffset()
	{
		float num = 0f;
		if ((bool)selectedEntity)
		{
			num = selectedEntity.offset * 300f;
			if ((bool)giantess)
			{
				num *= giantessOffsetDivisor;
			}
		}
		else
		{
			Debug.Log("object null");
		}
		return num;
	}

	public void SetYAxisOffset(float offset)
	{
		if ((bool)selectedEntity)
		{
			offset /= 300f;
			if ((bool)giantess)
			{
				offset /= giantessOffsetDivisor;
			}
			selectedEntity.ChangeVerticalOffset(offset);
		}
	}

	public void SetAnimation(string animationName, bool disableAutoRepositioning)
	{
		if (!(humanoid == null))
		{
			AgentAction action = new AnimationAction(animationName, false);
			humanoid.ai.DisableAI();
			humanoid.ai.behaviorController.StopMainBehavior();
			humanoid.ActionManager.ClearAll();
			humanoid.ActionManager.ScheduleAction(action);
			if (humanoid.isGiantess)
			{
				giantess.InvokeColliderUpdate();
				humanoid.Movement.move = true;
				giantess.gtsMovement.doNotMoveGts = disableAutoRepositioning;
				giantess.DestroyFingerPosers();
			}
		}
	}

	public void UpdateCollider()
	{
		if (!(giantess == null))
		{
			giantess.ForceColliderUpdate();
		}
	}

	public void ChangeAnimationSpeed(float speed)
	{
		if (giantess == null)
		{
			if (humanoid != null)
			{
				humanoid.animationManager.ChangeSpeed(speed);
			}
		}
		else
		{
			giantess.animationManager.ChangeSpeed(speed);
		}
	}

	public float GetAnimationSpeed()
	{
		if (giantess == null)
		{
			if (humanoid != null)
			{
				return humanoid.animationManager.speedMultiplier;
			}
			return 0f;
		}
		return giantess.animationManager.speedMultiplier;
	}

	public bool GetDoNotMoveMacroSetting()
	{
		if (selectedEntity.isGiantess)
		{
			return selectedEntity.GetComponentInChildren<Giantess>().gtsMovement.doNotMoveGts;
		}
		return false;
	}

	public void SetDoNotMoveMacro(bool value)
	{
		if (selectedEntity.isGiantess)
		{
			selectedEntity.GetComponentInChildren<Giantess>().gtsMovement.moveState = (value ? GTSMovement.MacroMoveState.DoNotMove : GTSMovement.MacroMoveState.ResetTransformPosition);
		}
	}

	public void DeleteObject()
	{
		if ((bool)selectedEntity && !selectedEntity.GetComponent<Weld>())
		{
			selectedEntity.DestroyObject();
			SetSelectedObject(null);
		}
	}

	public void LockRotation(bool value)
	{
		lockRotation = value;
	}
}
