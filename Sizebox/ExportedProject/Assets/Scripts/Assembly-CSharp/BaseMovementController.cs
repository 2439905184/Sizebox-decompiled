using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public abstract class BaseMovementController : MonoBehaviour
{
	protected Transform target;

	protected bool inputsConnected;

	public InputAction spawnMicroAction;

	internal float horizontalInput;

	internal float verticalInput;

	internal bool sprint;

	[FormerlySerializedAs("autowalk")]
	public bool autoWalk;

	internal static readonly WaitForSeconds MicroSpawnInterval = new WaitForSeconds(0.05f);

	protected virtual void Awake()
	{
		base.enabled = false;
	}

	private void OnDestroy()
	{
		DisconnectInputActions();
	}

	public abstract void SetTarget(Transform target);

	public abstract void DoUpdate();

	public abstract void DoFixedUpdate();

	public virtual void DoLateUpdate()
	{
	}

	protected virtual void ConnectInputActions()
	{
		if (!inputsConnected)
		{
			Inputs.PlayerActions player = InputManager.inputs.Player;
			player.AutoWalk.performed += OnAutoWalkPerformed;
			player.Move.canceled += OnMoveCancelled;
			player.Move.performed += OnMovePerformed;
			player.Sprint.canceled += OnSprintCancelled;
			player.Sprint.performed += OnSprintPerformed;
			InputManager.inputs.Misc.SpawnMicro.started += OnSpawnMicroStart;
			inputsConnected = true;
		}
	}

	protected virtual void DisconnectInputActions()
	{
		Inputs.PlayerActions player = InputManager.inputs.Player;
		player.AutoWalk.performed -= OnAutoWalkPerformed;
		player.Move.canceled -= OnMoveCancelled;
		player.Move.performed -= OnMovePerformed;
		player.Sprint.canceled -= OnSprintCancelled;
		player.Sprint.performed -= OnSprintPerformed;
		InputManager.inputs.Misc.SpawnMicro.started -= OnSpawnMicroStart;
		inputsConnected = false;
	}

	private void OnSpawnMicroStart(InputAction.CallbackContext obj)
	{
		StartCoroutine(SpawnMicro());
	}

	protected virtual IEnumerator SpawnMicro()
	{
		while ((bool)target)
		{
			bool female;
			bool male;
			GetMicroSpawnButtonState(out female, out male);
			if (female || male)
			{
				float num = target.lossyScale.x * 20f;
				if (StateManager.Keyboard.Shift)
				{
					num *= 0.24f + 3.76f * GlobalPreferences.MicroSpawnSize.value;
				}
				num = Mathf.Clamp(num, MapSettingInternal.minPlayerSize, MapSettingInternal.maxPlayerSize);
				MicroGender gender = ((!female) ? MicroGender.Male : MicroGender.Female);
				AssetDescription randomMicroAsset = AssetManager.Instance.GetRandomMicroAsset(gender);
				if (randomMicroAsset != null)
				{
					GameController.LocalClient.SpawnMicro(randomMicroAsset, num);
				}
				yield return MicroSpawnInterval;
				continue;
			}
			break;
		}
	}

	internal void GetMicroSpawnButtonState(out bool female, out bool male)
	{
		float num = InputManager.inputs.Misc.SpawnMicro.ReadValue<float>();
		if (num > float.Epsilon)
		{
			male = !(female = true);
		}
		else if (num < -1E-45f)
		{
			female = !(male = true);
		}
		else
		{
			female = (male = false);
		}
	}

	private void OnAutoWalkPerformed(InputAction.CallbackContext obj)
	{
		if (autoWalk)
		{
			autoWalk = false;
			verticalInput = 0f;
		}
		else
		{
			autoWalk = true;
		}
	}

	protected virtual void OnSprintCancelled(InputAction.CallbackContext obj)
	{
		sprint = false;
	}

	protected virtual void OnSprintPerformed(InputAction.CallbackContext obj)
	{
		sprint = true;
	}

	private void OnMoveCancelled(InputAction.CallbackContext obj)
	{
		horizontalInput = (verticalInput = 0f);
	}

	internal virtual void OnMovePerformed(InputAction.CallbackContext obj)
	{
		Vector2 vector = obj.ReadValue<Vector2>();
		horizontalInput = vector.x;
		verticalInput = vector.y;
	}
}
