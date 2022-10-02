using System;
using System.Collections;
using SizeboxUI;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainInput : MonoBehaviour
{
	private GameController controller;

	private CameraEffectsSettings cameraEffects;

	private GuiManager guiManager;

	private float startPushMale;

	private float startPushFemale;

	private bool gtsIsPaused;

	private float gtsPausedSpeed;

	private float gtsSpeedBeforeBulletTime;

	private float microSpeedBeforeBulletTime;

	private InputAction _hardcodedPause;

	private Toast _gameSpeedToast;

	private void Start()
	{
		controller = GetComponent<GameController>();
		guiManager = GuiManager.Instance;
		cameraEffects = GetComponent<CameraEffectsSettings>();
		_gameSpeedToast = new Toast("_GameSpeed");
		gtsSpeedBeforeBulletTime = GameController.macroSpeed;
		microSpeedBeforeBulletTime = GameController.microSpeed;
		_hardcodedPause = new InputAction("HardPause", InputActionType.Button, "<Keyboard>/escape");
		_hardcodedPause.performed += PausePressed;
		_hardcodedPause.Enable();
		Inputs.MiscActions misc = InputManager.inputs.Misc;
		misc.AlternativePause.performed += PausePressed;
		misc.Console.performed += ConsolePressed;
		misc.FreezeMacros.performed += FreezeMacros;
		misc.MacroSpeed.canceled += MacroSpeedCancel;
		misc.MacroSpeed.started += MacroSpeedStart;
		misc.ResetMacroSpeed.performed += ResetMacroSpeed;
		misc.BulletTimeToggle.performed += BulletTimeTogglePressed;
		misc.BulletTimeHold.canceled += BulletTimeOnHoldCanceled;
		misc.BulletTimeHold.started += BulletTimeOnHoldStarted;
		misc.Screenshot.performed += ScreenShotPressed;
		misc.SwitchMode.performed += ToggleEditMode;
		Inputs.EditModeActions editMode = InputManager.inputs.EditMode;
		editMode.DeleteSelection.performed += DeletePressed;
		editMode.GotoSelection.started += GotoPressed;
		editMode.Macro.performed += MacroPressed;
		editMode.Micro.performed += MicroPressed;
		editMode.MoveObject.performed += MovePressed;
		editMode.Object.performed += ObjectsPressed;
		editMode.Pose.performed += PosePressed;
	}

	private void PosePressed(InputAction.CallbackContext obj)
	{
		guiManager.EditMode.OnPoseClick();
	}

	private void ObjectsPressed(InputAction.CallbackContext obj)
	{
		guiManager.EditMode.OnCatalogClick(CatalogCategory.Object);
	}

	private void MovePressed(InputAction.CallbackContext obj)
	{
		guiManager.EditMode.placement.MoveCurrentGO();
	}

	private void MicroPressed(InputAction.CallbackContext obj)
	{
		guiManager.EditMode.OnCatalogClick(CatalogCategory.Micro);
	}

	private void MacroPressed(InputAction.CallbackContext obj)
	{
		guiManager.EditMode.OnCatalogClick(CatalogCategory.Giantess);
	}

	private void DeletePressed(InputAction.CallbackContext obj)
	{
		guiManager.EditMode.OnDeleteClick();
	}

	private void PausePressed(InputAction.CallbackContext obj)
	{
		guiManager.TogglePauseMenu();
	}

	private void ToggleEditMode(InputAction.CallbackContext obj)
	{
		if (!GameController.Instance.paused)
		{
			guiManager.ToggleEditMode();
		}
	}

	private void ConsolePressed(InputAction.CallbackContext obj)
	{
		guiManager.Console.visible = !guiManager.Console.visible;
	}

	private void ScreenShotPressed(InputAction.CallbackContext obj)
	{
		guiManager.TakeScreenshot();
	}

	private void ResetMacroSpeed(InputAction.CallbackContext obj)
	{
		CancelMacroSpeed();
		GameController.ChangeSpeed(1f);
	}

	private IEnumerator ChangingMacroSpeed()
	{
		float v;
		do
		{
			v = InputManager.inputs.Misc.MacroSpeed.ReadValue<float>();
			GameController.ChangeSpeed(GameController.macroSpeed + v * 0.05f);
			_gameSpeedToast.Print("Macro Speed:\t" + GameController.macroSpeed.ToString("P0"));
			yield return null;
		}
		while (Math.Abs(v - float.Epsilon) > 0.01f);
	}

	private void MacroSpeedCancel(InputAction.CallbackContext obj)
	{
		CancelMacroSpeed();
	}

	private void CancelMacroSpeed()
	{
		if (ChangingMacroSpeed().Current != null)
		{
			StopCoroutine(ChangingMacroSpeed());
		}
	}

	private void MacroSpeedStart(InputAction.CallbackContext obj)
	{
		if (GameController.FreezeGts)
		{
			ToggleMacroFreeze();
		}
		if (GameController.bulletTimeActive)
		{
			BulletTimeOff();
		}
		StartCoroutine(ChangingMacroSpeed());
	}

	private void FreezeMacros(InputAction.CallbackContext obj)
	{
		ToggleMacroFreeze();
	}

	private void BulletTimeTogglePressed(InputAction.CallbackContext obj)
	{
		if (GameController.bulletTimeActive)
		{
			BulletTimeOff();
			_gameSpeedToast.Print("Bullet Time:\t Disabled");
		}
		else
		{
			BulletTimeOn();
			_gameSpeedToast.Print("Bullet Time:\t Enabled");
		}
	}

	private void BulletTimeOnHoldCanceled(InputAction.CallbackContext obj)
	{
		BulletTimeOff();
	}

	private void BulletTimeOnHoldStarted(InputAction.CallbackContext obj)
	{
		BulletTimeOn();
	}

	private void GotoPressed(InputAction.CallbackContext obj)
	{
		StartCoroutine(GoTo());
	}

	private IEnumerator GoTo()
	{
		EntityBase entity = guiManager.InterfaceControl.selectedEntity;
		if (!entity)
		{
			yield break;
		}
		Camera main = Camera.main;
		if (main != null)
		{
			Transform cameraTransform = main.transform;
			Transform entityTransform = entity.transform;
			bool flag;
			do
			{
				float num = entityTransform.lossyScale.y * entity.meshHeight;
				cameraTransform.parent.SetPositionAndRotation(entityTransform.position + new Vector3(0f, num / 2f, 0f) - cameraTransform.forward.normalized * (num * 1.4f), cameraTransform.rotation);
				yield return null;
				flag = Mathf.Abs(InputManager.inputs.EditMode.GotoSelection.ReadValue<float>()) > float.Epsilon;
			}
			while ((bool)entity && GameController.Instance.mode == GameMode.Edit && flag);
		}
	}

	private void ToggleLookAtPlayer()
	{
		GlobalPreferences.LookAtPlayer.value = !GlobalPreferences.LookAtPlayer.value;
	}

	private void ToggleMacroFreeze()
	{
		CancelMacroSpeed();
		GameController.FreezeGts = !GameController.FreezeGts;
		ObjectManager.Instance.UpdateGiantessSpeeds();
		_gameSpeedToast.Print(GameController.FreezeGts ? "Frozen" : "Unfrozen");
	}

	private void BulletTimeOn()
	{
		if (GameController.FreezeGts)
		{
			ToggleMacroFreeze();
		}
		GameController.bulletTimeActive = true;
		ObjectManager.Instance.UpdateGiantessSpeeds();
		if (GameController.microsAffectedByBulletTime)
		{
			ObjectManager.Instance.UpdateMicroSpeeds();
		}
		ObjectManager.UpdatePlayerSpeed();
	}

	private void BulletTimeOff()
	{
		GameController.bulletTimeActive = false;
		ObjectManager.Instance.UpdateGiantessSpeeds();
		if (GameController.microsAffectedByBulletTime)
		{
			ObjectManager.Instance.UpdateMicroSpeeds();
		}
		ObjectManager.UpdatePlayerSpeed();
	}

	private void OnDestroy()
	{
		if (gtsIsPaused)
		{
			if (gtsPausedSpeed > 0f)
			{
				GameController.macroSpeed = gtsPausedSpeed;
			}
			else
			{
				GameController.macroSpeed = 0.1f;
			}
			if (GameController.bulletTimeActive)
			{
				GameController.macroSpeed = gtsSpeedBeforeBulletTime;
			}
			GameController.bulletTimeActive = false;
			gtsIsPaused = false;
			ObjectManager.Instance.UpdateGiantessSpeeds();
		}
		if ((bool)StateManager.cachedInstance)
		{
			CancelMacroSpeed();
		}
		Inputs.MiscActions misc = InputManager.inputs.Misc;
		misc.AlternativePause.performed -= PausePressed;
		misc.Console.performed -= ConsolePressed;
		misc.FreezeMacros.performed -= FreezeMacros;
		misc.MacroSpeed.canceled -= MacroSpeedCancel;
		misc.MacroSpeed.started -= MacroSpeedStart;
		misc.ResetMacroSpeed.performed -= ResetMacroSpeed;
		misc.BulletTimeHold.canceled -= BulletTimeOnHoldCanceled;
		misc.BulletTimeHold.started -= BulletTimeOnHoldStarted;
		misc.Screenshot.performed -= ScreenShotPressed;
		misc.SwitchMode.performed -= ToggleEditMode;
		misc.Disable();
		Inputs.EditModeActions editMode = InputManager.inputs.EditMode;
		editMode.DeleteSelection.performed -= DeletePressed;
		editMode.GotoSelection.started -= GotoPressed;
		editMode.Macro.performed -= MacroPressed;
		editMode.Micro.performed -= MicroPressed;
		editMode.MoveObject.performed -= MovePressed;
		editMode.Object.performed -= ObjectsPressed;
		editMode.Pose.performed -= PosePressed;
		_hardcodedPause.performed -= PausePressed;
	}
}
