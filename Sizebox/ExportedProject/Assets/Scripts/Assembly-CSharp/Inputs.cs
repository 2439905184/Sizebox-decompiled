using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class Inputs : IInputActionCollection, IEnumerable<InputAction>, IEnumerable, IDisposable
{
	public struct EditModeActions
	{
		private Inputs m_Wrapper;

		public InputAction MoveCamera
		{
			get
			{
				return m_Wrapper.m_EditMode_MoveCamera;
			}
		}

		public InputAction MoveCameraUpDown
		{
			get
			{
				return m_Wrapper.m_EditMode_MoveCameraUpDown;
			}
		}

		public InputAction MoveFaster
		{
			get
			{
				return m_Wrapper.m_EditMode_MoveFaster;
			}
		}

		public InputAction ChangeCameraMode
		{
			get
			{
				return m_Wrapper.m_EditMode_ChangeCameraMode;
			}
		}

		public InputAction Look
		{
			get
			{
				return m_Wrapper.m_EditMode_Look;
			}
		}

		public InputAction Zoom
		{
			get
			{
				return m_Wrapper.m_EditMode_Zoom;
			}
		}

		public InputAction Pose
		{
			get
			{
				return m_Wrapper.m_EditMode_Pose;
			}
		}

		public InputAction Object
		{
			get
			{
				return m_Wrapper.m_EditMode_Object;
			}
		}

		public InputAction MoveObject
		{
			get
			{
				return m_Wrapper.m_EditMode_MoveObject;
			}
		}

		public InputAction Micro
		{
			get
			{
				return m_Wrapper.m_EditMode_Micro;
			}
		}

		public InputAction Macro
		{
			get
			{
				return m_Wrapper.m_EditMode_Macro;
			}
		}

		public InputAction GotoSelection
		{
			get
			{
				return m_Wrapper.m_EditMode_GotoSelection;
			}
		}

		public InputAction DeleteSelection
		{
			get
			{
				return m_Wrapper.m_EditMode_DeleteSelection;
			}
		}

		public bool enabled
		{
			get
			{
				return Get().enabled;
			}
		}

		public EditModeActions(Inputs wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_EditMode;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(EditModeActions set)
		{
			return set.Get();
		}

		public void SetCallbacks(IEditModeActions instance)
		{
			if (m_Wrapper.m_EditModeActionsCallbackInterface != null)
			{
				MoveCamera.started -= m_Wrapper.m_EditModeActionsCallbackInterface.OnMoveCamera;
				MoveCamera.performed -= m_Wrapper.m_EditModeActionsCallbackInterface.OnMoveCamera;
				MoveCamera.canceled -= m_Wrapper.m_EditModeActionsCallbackInterface.OnMoveCamera;
				MoveCameraUpDown.started -= m_Wrapper.m_EditModeActionsCallbackInterface.OnMoveCameraUpDown;
				MoveCameraUpDown.performed -= m_Wrapper.m_EditModeActionsCallbackInterface.OnMoveCameraUpDown;
				MoveCameraUpDown.canceled -= m_Wrapper.m_EditModeActionsCallbackInterface.OnMoveCameraUpDown;
				MoveFaster.started -= m_Wrapper.m_EditModeActionsCallbackInterface.OnMoveFaster;
				MoveFaster.performed -= m_Wrapper.m_EditModeActionsCallbackInterface.OnMoveFaster;
				MoveFaster.canceled -= m_Wrapper.m_EditModeActionsCallbackInterface.OnMoveFaster;
				ChangeCameraMode.started -= m_Wrapper.m_EditModeActionsCallbackInterface.OnChangeCameraMode;
				ChangeCameraMode.performed -= m_Wrapper.m_EditModeActionsCallbackInterface.OnChangeCameraMode;
				ChangeCameraMode.canceled -= m_Wrapper.m_EditModeActionsCallbackInterface.OnChangeCameraMode;
				Look.started -= m_Wrapper.m_EditModeActionsCallbackInterface.OnLook;
				Look.performed -= m_Wrapper.m_EditModeActionsCallbackInterface.OnLook;
				Look.canceled -= m_Wrapper.m_EditModeActionsCallbackInterface.OnLook;
				Zoom.started -= m_Wrapper.m_EditModeActionsCallbackInterface.OnZoom;
				Zoom.performed -= m_Wrapper.m_EditModeActionsCallbackInterface.OnZoom;
				Zoom.canceled -= m_Wrapper.m_EditModeActionsCallbackInterface.OnZoom;
				Pose.started -= m_Wrapper.m_EditModeActionsCallbackInterface.OnPose;
				Pose.performed -= m_Wrapper.m_EditModeActionsCallbackInterface.OnPose;
				Pose.canceled -= m_Wrapper.m_EditModeActionsCallbackInterface.OnPose;
				Object.started -= m_Wrapper.m_EditModeActionsCallbackInterface.OnObject;
				Object.performed -= m_Wrapper.m_EditModeActionsCallbackInterface.OnObject;
				Object.canceled -= m_Wrapper.m_EditModeActionsCallbackInterface.OnObject;
				MoveObject.started -= m_Wrapper.m_EditModeActionsCallbackInterface.OnMoveObject;
				MoveObject.performed -= m_Wrapper.m_EditModeActionsCallbackInterface.OnMoveObject;
				MoveObject.canceled -= m_Wrapper.m_EditModeActionsCallbackInterface.OnMoveObject;
				Micro.started -= m_Wrapper.m_EditModeActionsCallbackInterface.OnMicro;
				Micro.performed -= m_Wrapper.m_EditModeActionsCallbackInterface.OnMicro;
				Micro.canceled -= m_Wrapper.m_EditModeActionsCallbackInterface.OnMicro;
				Macro.started -= m_Wrapper.m_EditModeActionsCallbackInterface.OnMacro;
				Macro.performed -= m_Wrapper.m_EditModeActionsCallbackInterface.OnMacro;
				Macro.canceled -= m_Wrapper.m_EditModeActionsCallbackInterface.OnMacro;
				GotoSelection.started -= m_Wrapper.m_EditModeActionsCallbackInterface.OnGotoSelection;
				GotoSelection.performed -= m_Wrapper.m_EditModeActionsCallbackInterface.OnGotoSelection;
				GotoSelection.canceled -= m_Wrapper.m_EditModeActionsCallbackInterface.OnGotoSelection;
				DeleteSelection.started -= m_Wrapper.m_EditModeActionsCallbackInterface.OnDeleteSelection;
				DeleteSelection.performed -= m_Wrapper.m_EditModeActionsCallbackInterface.OnDeleteSelection;
				DeleteSelection.canceled -= m_Wrapper.m_EditModeActionsCallbackInterface.OnDeleteSelection;
			}
			m_Wrapper.m_EditModeActionsCallbackInterface = instance;
			if (instance != null)
			{
				MoveCamera.started += instance.OnMoveCamera;
				MoveCamera.performed += instance.OnMoveCamera;
				MoveCamera.canceled += instance.OnMoveCamera;
				MoveCameraUpDown.started += instance.OnMoveCameraUpDown;
				MoveCameraUpDown.performed += instance.OnMoveCameraUpDown;
				MoveCameraUpDown.canceled += instance.OnMoveCameraUpDown;
				MoveFaster.started += instance.OnMoveFaster;
				MoveFaster.performed += instance.OnMoveFaster;
				MoveFaster.canceled += instance.OnMoveFaster;
				ChangeCameraMode.started += instance.OnChangeCameraMode;
				ChangeCameraMode.performed += instance.OnChangeCameraMode;
				ChangeCameraMode.canceled += instance.OnChangeCameraMode;
				Look.started += instance.OnLook;
				Look.performed += instance.OnLook;
				Look.canceled += instance.OnLook;
				Zoom.started += instance.OnZoom;
				Zoom.performed += instance.OnZoom;
				Zoom.canceled += instance.OnZoom;
				Pose.started += instance.OnPose;
				Pose.performed += instance.OnPose;
				Pose.canceled += instance.OnPose;
				Object.started += instance.OnObject;
				Object.performed += instance.OnObject;
				Object.canceled += instance.OnObject;
				MoveObject.started += instance.OnMoveObject;
				MoveObject.performed += instance.OnMoveObject;
				MoveObject.canceled += instance.OnMoveObject;
				Micro.started += instance.OnMicro;
				Micro.performed += instance.OnMicro;
				Micro.canceled += instance.OnMicro;
				Macro.started += instance.OnMacro;
				Macro.performed += instance.OnMacro;
				Macro.canceled += instance.OnMacro;
				GotoSelection.started += instance.OnGotoSelection;
				GotoSelection.performed += instance.OnGotoSelection;
				GotoSelection.canceled += instance.OnGotoSelection;
				DeleteSelection.started += instance.OnDeleteSelection;
				DeleteSelection.performed += instance.OnDeleteSelection;
				DeleteSelection.canceled += instance.OnDeleteSelection;
			}
		}
	}

	public struct PlayerActions
	{
		private Inputs m_Wrapper;

		public InputAction Move
		{
			get
			{
				return m_Wrapper.m_Player_Move;
			}
		}

		public InputAction Sprint
		{
			get
			{
				return m_Wrapper.m_Player_Sprint;
			}
		}

		public InputAction ChangeCamera
		{
			get
			{
				return m_Wrapper.m_Player_ChangeCamera;
			}
		}

		public InputAction ChangeSize
		{
			get
			{
				return m_Wrapper.m_Player_ChangeSize;
			}
		}

		public InputAction Look
		{
			get
			{
				return m_Wrapper.m_Player_Look;
			}
		}

		public InputAction Zoom
		{
			get
			{
				return m_Wrapper.m_Player_Zoom;
			}
		}

		public InputAction LookBack
		{
			get
			{
				return m_Wrapper.m_Player_LookBack;
			}
		}

		public InputAction AutoWalk
		{
			get
			{
				return m_Wrapper.m_Player_AutoWalk;
			}
		}

		public bool enabled
		{
			get
			{
				return Get().enabled;
			}
		}

		public PlayerActions(Inputs wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_Player;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(PlayerActions set)
		{
			return set.Get();
		}

		public void SetCallbacks(IPlayerActions instance)
		{
			if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
			{
				Move.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
				Move.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
				Move.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
				Sprint.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSprint;
				Sprint.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSprint;
				Sprint.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSprint;
				ChangeCamera.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnChangeCamera;
				ChangeCamera.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnChangeCamera;
				ChangeCamera.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnChangeCamera;
				ChangeSize.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnChangeSize;
				ChangeSize.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnChangeSize;
				ChangeSize.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnChangeSize;
				Look.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLook;
				Look.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLook;
				Look.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLook;
				Zoom.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnZoom;
				Zoom.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnZoom;
				Zoom.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnZoom;
				LookBack.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLookBack;
				LookBack.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLookBack;
				LookBack.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLookBack;
				AutoWalk.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAutoWalk;
				AutoWalk.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAutoWalk;
				AutoWalk.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAutoWalk;
			}
			m_Wrapper.m_PlayerActionsCallbackInterface = instance;
			if (instance != null)
			{
				Move.started += instance.OnMove;
				Move.performed += instance.OnMove;
				Move.canceled += instance.OnMove;
				Sprint.started += instance.OnSprint;
				Sprint.performed += instance.OnSprint;
				Sprint.canceled += instance.OnSprint;
				ChangeCamera.started += instance.OnChangeCamera;
				ChangeCamera.performed += instance.OnChangeCamera;
				ChangeCamera.canceled += instance.OnChangeCamera;
				ChangeSize.started += instance.OnChangeSize;
				ChangeSize.performed += instance.OnChangeSize;
				ChangeSize.canceled += instance.OnChangeSize;
				Look.started += instance.OnLook;
				Look.performed += instance.OnLook;
				Look.canceled += instance.OnLook;
				Zoom.started += instance.OnZoom;
				Zoom.performed += instance.OnZoom;
				Zoom.canceled += instance.OnZoom;
				LookBack.started += instance.OnLookBack;
				LookBack.performed += instance.OnLookBack;
				LookBack.canceled += instance.OnLookBack;
				AutoWalk.started += instance.OnAutoWalk;
				AutoWalk.performed += instance.OnAutoWalk;
				AutoWalk.canceled += instance.OnAutoWalk;
			}
		}
	}

	public struct MacroActions
	{
		private Inputs m_Wrapper;

		public InputAction Stomp
		{
			get
			{
				return m_Wrapper.m_Macro_Stomp;
			}
		}

		public InputAction Cancel
		{
			get
			{
				return m_Wrapper.m_Macro_Cancel;
			}
		}

		public bool enabled
		{
			get
			{
				return Get().enabled;
			}
		}

		public MacroActions(Inputs wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_Macro;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(MacroActions set)
		{
			return set.Get();
		}

		public void SetCallbacks(IMacroActions instance)
		{
			if (m_Wrapper.m_MacroActionsCallbackInterface != null)
			{
				Stomp.started -= m_Wrapper.m_MacroActionsCallbackInterface.OnStomp;
				Stomp.performed -= m_Wrapper.m_MacroActionsCallbackInterface.OnStomp;
				Stomp.canceled -= m_Wrapper.m_MacroActionsCallbackInterface.OnStomp;
				Cancel.started -= m_Wrapper.m_MacroActionsCallbackInterface.OnCancel;
				Cancel.performed -= m_Wrapper.m_MacroActionsCallbackInterface.OnCancel;
				Cancel.canceled -= m_Wrapper.m_MacroActionsCallbackInterface.OnCancel;
			}
			m_Wrapper.m_MacroActionsCallbackInterface = instance;
			if (instance != null)
			{
				Stomp.started += instance.OnStomp;
				Stomp.performed += instance.OnStomp;
				Stomp.canceled += instance.OnStomp;
				Cancel.started += instance.OnCancel;
				Cancel.performed += instance.OnCancel;
				Cancel.canceled += instance.OnCancel;
			}
		}
	}

	public struct MicroActions
	{
		private Inputs m_Wrapper;

		public InputAction Crouch
		{
			get
			{
				return m_Wrapper.m_Micro_Crouch;
			}
		}

		public InputAction Fly
		{
			get
			{
				return m_Wrapper.m_Micro_Fly;
			}
		}

		public InputAction Interact
		{
			get
			{
				return m_Wrapper.m_Micro_Interact;
			}
		}

		public InputAction Jump
		{
			get
			{
				return m_Wrapper.m_Micro_Jump;
			}
		}

		public InputAction Walk
		{
			get
			{
				return m_Wrapper.m_Micro_Walk;
			}
		}

		public InputAction WeaponAim
		{
			get
			{
				return m_Wrapper.m_Micro_WeaponAim;
			}
		}

		public InputAction WeaponFire
		{
			get
			{
				return m_Wrapper.m_Micro_WeaponFire;
			}
		}

		public InputAction WeaponMode
		{
			get
			{
				return m_Wrapper.m_Micro_WeaponMode;
			}
		}

		public InputAction WeaponRange
		{
			get
			{
				return m_Wrapper.m_Micro_WeaponRange;
			}
		}

		public InputAction SuperFly
		{
			get
			{
				return m_Wrapper.m_Micro_SuperFly;
			}
		}

		public InputAction FlyingPunch
		{
			get
			{
				return m_Wrapper.m_Micro_FlyingPunch;
			}
		}

		public InputAction FlyDown
		{
			get
			{
				return m_Wrapper.m_Micro_FlyDown;
			}
		}

		public bool enabled
		{
			get
			{
				return Get().enabled;
			}
		}

		public MicroActions(Inputs wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_Micro;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(MicroActions set)
		{
			return set.Get();
		}

		public void SetCallbacks(IMicroActions instance)
		{
			if (m_Wrapper.m_MicroActionsCallbackInterface != null)
			{
				Crouch.started -= m_Wrapper.m_MicroActionsCallbackInterface.OnCrouch;
				Crouch.performed -= m_Wrapper.m_MicroActionsCallbackInterface.OnCrouch;
				Crouch.canceled -= m_Wrapper.m_MicroActionsCallbackInterface.OnCrouch;
				Fly.started -= m_Wrapper.m_MicroActionsCallbackInterface.OnFly;
				Fly.performed -= m_Wrapper.m_MicroActionsCallbackInterface.OnFly;
				Fly.canceled -= m_Wrapper.m_MicroActionsCallbackInterface.OnFly;
				Interact.started -= m_Wrapper.m_MicroActionsCallbackInterface.OnInteract;
				Interact.performed -= m_Wrapper.m_MicroActionsCallbackInterface.OnInteract;
				Interact.canceled -= m_Wrapper.m_MicroActionsCallbackInterface.OnInteract;
				Jump.started -= m_Wrapper.m_MicroActionsCallbackInterface.OnJump;
				Jump.performed -= m_Wrapper.m_MicroActionsCallbackInterface.OnJump;
				Jump.canceled -= m_Wrapper.m_MicroActionsCallbackInterface.OnJump;
				Walk.started -= m_Wrapper.m_MicroActionsCallbackInterface.OnWalk;
				Walk.performed -= m_Wrapper.m_MicroActionsCallbackInterface.OnWalk;
				Walk.canceled -= m_Wrapper.m_MicroActionsCallbackInterface.OnWalk;
				WeaponAim.started -= m_Wrapper.m_MicroActionsCallbackInterface.OnWeaponAim;
				WeaponAim.performed -= m_Wrapper.m_MicroActionsCallbackInterface.OnWeaponAim;
				WeaponAim.canceled -= m_Wrapper.m_MicroActionsCallbackInterface.OnWeaponAim;
				WeaponFire.started -= m_Wrapper.m_MicroActionsCallbackInterface.OnWeaponFire;
				WeaponFire.performed -= m_Wrapper.m_MicroActionsCallbackInterface.OnWeaponFire;
				WeaponFire.canceled -= m_Wrapper.m_MicroActionsCallbackInterface.OnWeaponFire;
				WeaponMode.started -= m_Wrapper.m_MicroActionsCallbackInterface.OnWeaponMode;
				WeaponMode.performed -= m_Wrapper.m_MicroActionsCallbackInterface.OnWeaponMode;
				WeaponMode.canceled -= m_Wrapper.m_MicroActionsCallbackInterface.OnWeaponMode;
				WeaponRange.started -= m_Wrapper.m_MicroActionsCallbackInterface.OnWeaponRange;
				WeaponRange.performed -= m_Wrapper.m_MicroActionsCallbackInterface.OnWeaponRange;
				WeaponRange.canceled -= m_Wrapper.m_MicroActionsCallbackInterface.OnWeaponRange;
				SuperFly.started -= m_Wrapper.m_MicroActionsCallbackInterface.OnSuperFly;
				SuperFly.performed -= m_Wrapper.m_MicroActionsCallbackInterface.OnSuperFly;
				SuperFly.canceled -= m_Wrapper.m_MicroActionsCallbackInterface.OnSuperFly;
				FlyingPunch.started -= m_Wrapper.m_MicroActionsCallbackInterface.OnFlyingPunch;
				FlyingPunch.performed -= m_Wrapper.m_MicroActionsCallbackInterface.OnFlyingPunch;
				FlyingPunch.canceled -= m_Wrapper.m_MicroActionsCallbackInterface.OnFlyingPunch;
				FlyDown.started -= m_Wrapper.m_MicroActionsCallbackInterface.OnFlyDown;
				FlyDown.performed -= m_Wrapper.m_MicroActionsCallbackInterface.OnFlyDown;
				FlyDown.canceled -= m_Wrapper.m_MicroActionsCallbackInterface.OnFlyDown;
			}
			m_Wrapper.m_MicroActionsCallbackInterface = instance;
			if (instance != null)
			{
				Crouch.started += instance.OnCrouch;
				Crouch.performed += instance.OnCrouch;
				Crouch.canceled += instance.OnCrouch;
				Fly.started += instance.OnFly;
				Fly.performed += instance.OnFly;
				Fly.canceled += instance.OnFly;
				Interact.started += instance.OnInteract;
				Interact.performed += instance.OnInteract;
				Interact.canceled += instance.OnInteract;
				Jump.started += instance.OnJump;
				Jump.performed += instance.OnJump;
				Jump.canceled += instance.OnJump;
				Walk.started += instance.OnWalk;
				Walk.performed += instance.OnWalk;
				Walk.canceled += instance.OnWalk;
				WeaponAim.started += instance.OnWeaponAim;
				WeaponAim.performed += instance.OnWeaponAim;
				WeaponAim.canceled += instance.OnWeaponAim;
				WeaponFire.started += instance.OnWeaponFire;
				WeaponFire.performed += instance.OnWeaponFire;
				WeaponFire.canceled += instance.OnWeaponFire;
				WeaponMode.started += instance.OnWeaponMode;
				WeaponMode.performed += instance.OnWeaponMode;
				WeaponMode.canceled += instance.OnWeaponMode;
				WeaponRange.started += instance.OnWeaponRange;
				WeaponRange.performed += instance.OnWeaponRange;
				WeaponRange.canceled += instance.OnWeaponRange;
				SuperFly.started += instance.OnSuperFly;
				SuperFly.performed += instance.OnSuperFly;
				SuperFly.canceled += instance.OnSuperFly;
				FlyingPunch.started += instance.OnFlyingPunch;
				FlyingPunch.performed += instance.OnFlyingPunch;
				FlyingPunch.canceled += instance.OnFlyingPunch;
				FlyDown.started += instance.OnFlyDown;
				FlyDown.performed += instance.OnFlyDown;
				FlyDown.canceled += instance.OnFlyDown;
			}
		}
	}

	public struct MiscActions
	{
		private Inputs m_Wrapper;

		public InputAction AlternativePause
		{
			get
			{
				return m_Wrapper.m_Misc_AlternativePause;
			}
		}

		public InputAction BulletTimeHold
		{
			get
			{
				return m_Wrapper.m_Misc_BulletTimeHold;
			}
		}

		public InputAction BulletTimeToggle
		{
			get
			{
				return m_Wrapper.m_Misc_BulletTimeToggle;
			}
		}

		public InputAction Console
		{
			get
			{
				return m_Wrapper.m_Misc_Console;
			}
		}

		public InputAction FreezeMacros
		{
			get
			{
				return m_Wrapper.m_Misc_FreezeMacros;
			}
		}

		public InputAction MacroSpeed
		{
			get
			{
				return m_Wrapper.m_Misc_MacroSpeed;
			}
		}

		public InputAction ResetMacroSpeed
		{
			get
			{
				return m_Wrapper.m_Misc_ResetMacroSpeed;
			}
		}

		public InputAction Screenshot
		{
			get
			{
				return m_Wrapper.m_Misc_Screenshot;
			}
		}

		public InputAction SwitchMode
		{
			get
			{
				return m_Wrapper.m_Misc_SwitchMode;
			}
		}

		public InputAction SpawnMicro
		{
			get
			{
				return m_Wrapper.m_Misc_SpawnMicro;
			}
		}

		public bool enabled
		{
			get
			{
				return Get().enabled;
			}
		}

		public MiscActions(Inputs wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_Misc;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(MiscActions set)
		{
			return set.Get();
		}

		public void SetCallbacks(IMiscActions instance)
		{
			if (m_Wrapper.m_MiscActionsCallbackInterface != null)
			{
				AlternativePause.started -= m_Wrapper.m_MiscActionsCallbackInterface.OnAlternativePause;
				AlternativePause.performed -= m_Wrapper.m_MiscActionsCallbackInterface.OnAlternativePause;
				AlternativePause.canceled -= m_Wrapper.m_MiscActionsCallbackInterface.OnAlternativePause;
				BulletTimeHold.started -= m_Wrapper.m_MiscActionsCallbackInterface.OnBulletTimeHold;
				BulletTimeHold.performed -= m_Wrapper.m_MiscActionsCallbackInterface.OnBulletTimeHold;
				BulletTimeHold.canceled -= m_Wrapper.m_MiscActionsCallbackInterface.OnBulletTimeHold;
				BulletTimeToggle.started -= m_Wrapper.m_MiscActionsCallbackInterface.OnBulletTimeToggle;
				BulletTimeToggle.performed -= m_Wrapper.m_MiscActionsCallbackInterface.OnBulletTimeToggle;
				BulletTimeToggle.canceled -= m_Wrapper.m_MiscActionsCallbackInterface.OnBulletTimeToggle;
				Console.started -= m_Wrapper.m_MiscActionsCallbackInterface.OnConsole;
				Console.performed -= m_Wrapper.m_MiscActionsCallbackInterface.OnConsole;
				Console.canceled -= m_Wrapper.m_MiscActionsCallbackInterface.OnConsole;
				FreezeMacros.started -= m_Wrapper.m_MiscActionsCallbackInterface.OnFreezeMacros;
				FreezeMacros.performed -= m_Wrapper.m_MiscActionsCallbackInterface.OnFreezeMacros;
				FreezeMacros.canceled -= m_Wrapper.m_MiscActionsCallbackInterface.OnFreezeMacros;
				MacroSpeed.started -= m_Wrapper.m_MiscActionsCallbackInterface.OnMacroSpeed;
				MacroSpeed.performed -= m_Wrapper.m_MiscActionsCallbackInterface.OnMacroSpeed;
				MacroSpeed.canceled -= m_Wrapper.m_MiscActionsCallbackInterface.OnMacroSpeed;
				ResetMacroSpeed.started -= m_Wrapper.m_MiscActionsCallbackInterface.OnResetMacroSpeed;
				ResetMacroSpeed.performed -= m_Wrapper.m_MiscActionsCallbackInterface.OnResetMacroSpeed;
				ResetMacroSpeed.canceled -= m_Wrapper.m_MiscActionsCallbackInterface.OnResetMacroSpeed;
				Screenshot.started -= m_Wrapper.m_MiscActionsCallbackInterface.OnScreenshot;
				Screenshot.performed -= m_Wrapper.m_MiscActionsCallbackInterface.OnScreenshot;
				Screenshot.canceled -= m_Wrapper.m_MiscActionsCallbackInterface.OnScreenshot;
				SwitchMode.started -= m_Wrapper.m_MiscActionsCallbackInterface.OnSwitchMode;
				SwitchMode.performed -= m_Wrapper.m_MiscActionsCallbackInterface.OnSwitchMode;
				SwitchMode.canceled -= m_Wrapper.m_MiscActionsCallbackInterface.OnSwitchMode;
				SpawnMicro.started -= m_Wrapper.m_MiscActionsCallbackInterface.OnSpawnMicro;
				SpawnMicro.performed -= m_Wrapper.m_MiscActionsCallbackInterface.OnSpawnMicro;
				SpawnMicro.canceled -= m_Wrapper.m_MiscActionsCallbackInterface.OnSpawnMicro;
			}
			m_Wrapper.m_MiscActionsCallbackInterface = instance;
			if (instance != null)
			{
				AlternativePause.started += instance.OnAlternativePause;
				AlternativePause.performed += instance.OnAlternativePause;
				AlternativePause.canceled += instance.OnAlternativePause;
				BulletTimeHold.started += instance.OnBulletTimeHold;
				BulletTimeHold.performed += instance.OnBulletTimeHold;
				BulletTimeHold.canceled += instance.OnBulletTimeHold;
				BulletTimeToggle.started += instance.OnBulletTimeToggle;
				BulletTimeToggle.performed += instance.OnBulletTimeToggle;
				BulletTimeToggle.canceled += instance.OnBulletTimeToggle;
				Console.started += instance.OnConsole;
				Console.performed += instance.OnConsole;
				Console.canceled += instance.OnConsole;
				FreezeMacros.started += instance.OnFreezeMacros;
				FreezeMacros.performed += instance.OnFreezeMacros;
				FreezeMacros.canceled += instance.OnFreezeMacros;
				MacroSpeed.started += instance.OnMacroSpeed;
				MacroSpeed.performed += instance.OnMacroSpeed;
				MacroSpeed.canceled += instance.OnMacroSpeed;
				ResetMacroSpeed.started += instance.OnResetMacroSpeed;
				ResetMacroSpeed.performed += instance.OnResetMacroSpeed;
				ResetMacroSpeed.canceled += instance.OnResetMacroSpeed;
				Screenshot.started += instance.OnScreenshot;
				Screenshot.performed += instance.OnScreenshot;
				Screenshot.canceled += instance.OnScreenshot;
				SwitchMode.started += instance.OnSwitchMode;
				SwitchMode.performed += instance.OnSwitchMode;
				SwitchMode.canceled += instance.OnSwitchMode;
				SpawnMicro.started += instance.OnSpawnMicro;
				SpawnMicro.performed += instance.OnSpawnMicro;
				SpawnMicro.canceled += instance.OnSpawnMicro;
			}
		}
	}

	public struct InterfaceActions
	{
		private Inputs m_Wrapper;

		public InputAction Move
		{
			get
			{
				return m_Wrapper.m_Interface_Move;
			}
		}

		public InputAction Select
		{
			get
			{
				return m_Wrapper.m_Interface_Select;
			}
		}

		public InputAction Back
		{
			get
			{
				return m_Wrapper.m_Interface_Back;
			}
		}

		public InputAction Scroll
		{
			get
			{
				return m_Wrapper.m_Interface_Scroll;
			}
		}

		public InputAction Pointer
		{
			get
			{
				return m_Wrapper.m_Interface_Pointer;
			}
		}

		public InputAction LeftClick
		{
			get
			{
				return m_Wrapper.m_Interface_LeftClick;
			}
		}

		public InputAction RightClick
		{
			get
			{
				return m_Wrapper.m_Interface_RightClick;
			}
		}

		public InputAction MiddleClick
		{
			get
			{
				return m_Wrapper.m_Interface_MiddleClick;
			}
		}

		public InputAction Position
		{
			get
			{
				return m_Wrapper.m_Interface_Position;
			}
		}

		public InputAction Orientation
		{
			get
			{
				return m_Wrapper.m_Interface_Orientation;
			}
		}

		public bool enabled
		{
			get
			{
				return Get().enabled;
			}
		}

		public InterfaceActions(Inputs wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_Interface;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(InterfaceActions set)
		{
			return set.Get();
		}

		public void SetCallbacks(IInterfaceActions instance)
		{
			if (m_Wrapper.m_InterfaceActionsCallbackInterface != null)
			{
				Move.started -= m_Wrapper.m_InterfaceActionsCallbackInterface.OnMove;
				Move.performed -= m_Wrapper.m_InterfaceActionsCallbackInterface.OnMove;
				Move.canceled -= m_Wrapper.m_InterfaceActionsCallbackInterface.OnMove;
				Select.started -= m_Wrapper.m_InterfaceActionsCallbackInterface.OnSelect;
				Select.performed -= m_Wrapper.m_InterfaceActionsCallbackInterface.OnSelect;
				Select.canceled -= m_Wrapper.m_InterfaceActionsCallbackInterface.OnSelect;
				Back.started -= m_Wrapper.m_InterfaceActionsCallbackInterface.OnBack;
				Back.performed -= m_Wrapper.m_InterfaceActionsCallbackInterface.OnBack;
				Back.canceled -= m_Wrapper.m_InterfaceActionsCallbackInterface.OnBack;
				Scroll.started -= m_Wrapper.m_InterfaceActionsCallbackInterface.OnScroll;
				Scroll.performed -= m_Wrapper.m_InterfaceActionsCallbackInterface.OnScroll;
				Scroll.canceled -= m_Wrapper.m_InterfaceActionsCallbackInterface.OnScroll;
				Pointer.started -= m_Wrapper.m_InterfaceActionsCallbackInterface.OnPointer;
				Pointer.performed -= m_Wrapper.m_InterfaceActionsCallbackInterface.OnPointer;
				Pointer.canceled -= m_Wrapper.m_InterfaceActionsCallbackInterface.OnPointer;
				LeftClick.started -= m_Wrapper.m_InterfaceActionsCallbackInterface.OnLeftClick;
				LeftClick.performed -= m_Wrapper.m_InterfaceActionsCallbackInterface.OnLeftClick;
				LeftClick.canceled -= m_Wrapper.m_InterfaceActionsCallbackInterface.OnLeftClick;
				RightClick.started -= m_Wrapper.m_InterfaceActionsCallbackInterface.OnRightClick;
				RightClick.performed -= m_Wrapper.m_InterfaceActionsCallbackInterface.OnRightClick;
				RightClick.canceled -= m_Wrapper.m_InterfaceActionsCallbackInterface.OnRightClick;
				MiddleClick.started -= m_Wrapper.m_InterfaceActionsCallbackInterface.OnMiddleClick;
				MiddleClick.performed -= m_Wrapper.m_InterfaceActionsCallbackInterface.OnMiddleClick;
				MiddleClick.canceled -= m_Wrapper.m_InterfaceActionsCallbackInterface.OnMiddleClick;
				Position.started -= m_Wrapper.m_InterfaceActionsCallbackInterface.OnPosition;
				Position.performed -= m_Wrapper.m_InterfaceActionsCallbackInterface.OnPosition;
				Position.canceled -= m_Wrapper.m_InterfaceActionsCallbackInterface.OnPosition;
				Orientation.started -= m_Wrapper.m_InterfaceActionsCallbackInterface.OnOrientation;
				Orientation.performed -= m_Wrapper.m_InterfaceActionsCallbackInterface.OnOrientation;
				Orientation.canceled -= m_Wrapper.m_InterfaceActionsCallbackInterface.OnOrientation;
			}
			m_Wrapper.m_InterfaceActionsCallbackInterface = instance;
			if (instance != null)
			{
				Move.started += instance.OnMove;
				Move.performed += instance.OnMove;
				Move.canceled += instance.OnMove;
				Select.started += instance.OnSelect;
				Select.performed += instance.OnSelect;
				Select.canceled += instance.OnSelect;
				Back.started += instance.OnBack;
				Back.performed += instance.OnBack;
				Back.canceled += instance.OnBack;
				Scroll.started += instance.OnScroll;
				Scroll.performed += instance.OnScroll;
				Scroll.canceled += instance.OnScroll;
				Pointer.started += instance.OnPointer;
				Pointer.performed += instance.OnPointer;
				Pointer.canceled += instance.OnPointer;
				LeftClick.started += instance.OnLeftClick;
				LeftClick.performed += instance.OnLeftClick;
				LeftClick.canceled += instance.OnLeftClick;
				RightClick.started += instance.OnRightClick;
				RightClick.performed += instance.OnRightClick;
				RightClick.canceled += instance.OnRightClick;
				MiddleClick.started += instance.OnMiddleClick;
				MiddleClick.performed += instance.OnMiddleClick;
				MiddleClick.canceled += instance.OnMiddleClick;
				Position.started += instance.OnPosition;
				Position.performed += instance.OnPosition;
				Position.canceled += instance.OnPosition;
				Orientation.started += instance.OnOrientation;
				Orientation.performed += instance.OnOrientation;
				Orientation.canceled += instance.OnOrientation;
			}
		}
	}

	public struct LuaActions
	{
		private Inputs m_Wrapper;

		public bool enabled
		{
			get
			{
				return Get().enabled;
			}
		}

		public LuaActions(Inputs wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_Lua;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(LuaActions set)
		{
			return set.Get();
		}

		public void SetCallbacks(ILuaActions instance)
		{
			ILuaActions luaActionsCallbackInterface = m_Wrapper.m_LuaActionsCallbackInterface;
			m_Wrapper.m_LuaActionsCallbackInterface = instance;
		}
	}

	public interface IEditModeActions
	{
		void OnMoveCamera(InputAction.CallbackContext context);

		void OnMoveCameraUpDown(InputAction.CallbackContext context);

		void OnMoveFaster(InputAction.CallbackContext context);

		void OnChangeCameraMode(InputAction.CallbackContext context);

		void OnLook(InputAction.CallbackContext context);

		void OnZoom(InputAction.CallbackContext context);

		void OnPose(InputAction.CallbackContext context);

		void OnObject(InputAction.CallbackContext context);

		void OnMoveObject(InputAction.CallbackContext context);

		void OnMicro(InputAction.CallbackContext context);

		void OnMacro(InputAction.CallbackContext context);

		void OnGotoSelection(InputAction.CallbackContext context);

		void OnDeleteSelection(InputAction.CallbackContext context);
	}

	public interface IPlayerActions
	{
		void OnMove(InputAction.CallbackContext context);

		void OnSprint(InputAction.CallbackContext context);

		void OnChangeCamera(InputAction.CallbackContext context);

		void OnChangeSize(InputAction.CallbackContext context);

		void OnLook(InputAction.CallbackContext context);

		void OnZoom(InputAction.CallbackContext context);

		void OnLookBack(InputAction.CallbackContext context);

		void OnAutoWalk(InputAction.CallbackContext context);
	}

	public interface IMacroActions
	{
		void OnStomp(InputAction.CallbackContext context);

		void OnCancel(InputAction.CallbackContext context);
	}

	public interface IMicroActions
	{
		void OnCrouch(InputAction.CallbackContext context);

		void OnFly(InputAction.CallbackContext context);

		void OnInteract(InputAction.CallbackContext context);

		void OnJump(InputAction.CallbackContext context);

		void OnWalk(InputAction.CallbackContext context);

		void OnWeaponAim(InputAction.CallbackContext context);

		void OnWeaponFire(InputAction.CallbackContext context);

		void OnWeaponMode(InputAction.CallbackContext context);

		void OnWeaponRange(InputAction.CallbackContext context);

		void OnSuperFly(InputAction.CallbackContext context);

		void OnFlyingPunch(InputAction.CallbackContext context);

		void OnFlyDown(InputAction.CallbackContext context);
	}

	public interface IMiscActions
	{
		void OnAlternativePause(InputAction.CallbackContext context);

		void OnBulletTimeHold(InputAction.CallbackContext context);

		void OnBulletTimeToggle(InputAction.CallbackContext context);

		void OnConsole(InputAction.CallbackContext context);

		void OnFreezeMacros(InputAction.CallbackContext context);

		void OnMacroSpeed(InputAction.CallbackContext context);

		void OnResetMacroSpeed(InputAction.CallbackContext context);

		void OnScreenshot(InputAction.CallbackContext context);

		void OnSwitchMode(InputAction.CallbackContext context);

		void OnSpawnMicro(InputAction.CallbackContext context);
	}

	public interface IInterfaceActions
	{
		void OnMove(InputAction.CallbackContext context);

		void OnSelect(InputAction.CallbackContext context);

		void OnBack(InputAction.CallbackContext context);

		void OnScroll(InputAction.CallbackContext context);

		void OnPointer(InputAction.CallbackContext context);

		void OnLeftClick(InputAction.CallbackContext context);

		void OnRightClick(InputAction.CallbackContext context);

		void OnMiddleClick(InputAction.CallbackContext context);

		void OnPosition(InputAction.CallbackContext context);

		void OnOrientation(InputAction.CallbackContext context);
	}

	public interface ILuaActions
	{
	}

	[CompilerGenerated]
	private readonly InputActionAsset _003Casset_003Ek__BackingField;

	private readonly InputActionMap m_EditMode;

	private IEditModeActions m_EditModeActionsCallbackInterface;

	private readonly InputAction m_EditMode_MoveCamera;

	private readonly InputAction m_EditMode_MoveCameraUpDown;

	private readonly InputAction m_EditMode_MoveFaster;

	private readonly InputAction m_EditMode_ChangeCameraMode;

	private readonly InputAction m_EditMode_Look;

	private readonly InputAction m_EditMode_Zoom;

	private readonly InputAction m_EditMode_Pose;

	private readonly InputAction m_EditMode_Object;

	private readonly InputAction m_EditMode_MoveObject;

	private readonly InputAction m_EditMode_Micro;

	private readonly InputAction m_EditMode_Macro;

	private readonly InputAction m_EditMode_GotoSelection;

	private readonly InputAction m_EditMode_DeleteSelection;

	private readonly InputActionMap m_Player;

	private IPlayerActions m_PlayerActionsCallbackInterface;

	private readonly InputAction m_Player_Move;

	private readonly InputAction m_Player_Sprint;

	private readonly InputAction m_Player_ChangeCamera;

	private readonly InputAction m_Player_ChangeSize;

	private readonly InputAction m_Player_Look;

	private readonly InputAction m_Player_Zoom;

	private readonly InputAction m_Player_LookBack;

	private readonly InputAction m_Player_AutoWalk;

	private readonly InputActionMap m_Macro;

	private IMacroActions m_MacroActionsCallbackInterface;

	private readonly InputAction m_Macro_Stomp;

	private readonly InputAction m_Macro_Cancel;

	private readonly InputActionMap m_Micro;

	private IMicroActions m_MicroActionsCallbackInterface;

	private readonly InputAction m_Micro_Crouch;

	private readonly InputAction m_Micro_Fly;

	private readonly InputAction m_Micro_Interact;

	private readonly InputAction m_Micro_Jump;

	private readonly InputAction m_Micro_Walk;

	private readonly InputAction m_Micro_WeaponAim;

	private readonly InputAction m_Micro_WeaponFire;

	private readonly InputAction m_Micro_WeaponMode;

	private readonly InputAction m_Micro_WeaponRange;

	private readonly InputAction m_Micro_SuperFly;

	private readonly InputAction m_Micro_FlyingPunch;

	private readonly InputAction m_Micro_FlyDown;

	private readonly InputActionMap m_Misc;

	private IMiscActions m_MiscActionsCallbackInterface;

	private readonly InputAction m_Misc_AlternativePause;

	private readonly InputAction m_Misc_BulletTimeHold;

	private readonly InputAction m_Misc_BulletTimeToggle;

	private readonly InputAction m_Misc_Console;

	private readonly InputAction m_Misc_FreezeMacros;

	private readonly InputAction m_Misc_MacroSpeed;

	private readonly InputAction m_Misc_ResetMacroSpeed;

	private readonly InputAction m_Misc_Screenshot;

	private readonly InputAction m_Misc_SwitchMode;

	private readonly InputAction m_Misc_SpawnMicro;

	private readonly InputActionMap m_Interface;

	private IInterfaceActions m_InterfaceActionsCallbackInterface;

	private readonly InputAction m_Interface_Move;

	private readonly InputAction m_Interface_Select;

	private readonly InputAction m_Interface_Back;

	private readonly InputAction m_Interface_Scroll;

	private readonly InputAction m_Interface_Pointer;

	private readonly InputAction m_Interface_LeftClick;

	private readonly InputAction m_Interface_RightClick;

	private readonly InputAction m_Interface_MiddleClick;

	private readonly InputAction m_Interface_Position;

	private readonly InputAction m_Interface_Orientation;

	private readonly InputActionMap m_Lua;

	private ILuaActions m_LuaActionsCallbackInterface;

	private int m_KBMSchemeIndex = -1;

	private int m_GamepadSchemeIndex = -1;

	public InputActionAsset asset
	{
		[CompilerGenerated]
		get
		{
			return _003Casset_003Ek__BackingField;
		}
	}

	public InputBinding? bindingMask
	{
		get
		{
			return asset.bindingMask;
		}
		set
		{
			asset.bindingMask = value;
		}
	}

	public ReadOnlyArray<InputDevice>? devices
	{
		get
		{
			return asset.devices;
		}
		set
		{
			asset.devices = value;
		}
	}

	public ReadOnlyArray<InputControlScheme> controlSchemes
	{
		get
		{
			return asset.controlSchemes;
		}
	}

	public EditModeActions EditMode
	{
		get
		{
			return new EditModeActions(this);
		}
	}

	public PlayerActions Player
	{
		get
		{
			return new PlayerActions(this);
		}
	}

	public MacroActions Macro
	{
		get
		{
			return new MacroActions(this);
		}
	}

	public MicroActions Micro
	{
		get
		{
			return new MicroActions(this);
		}
	}

	public MiscActions Misc
	{
		get
		{
			return new MiscActions(this);
		}
	}

	public InterfaceActions Interface
	{
		get
		{
			return new InterfaceActions(this);
		}
	}

	public LuaActions Lua
	{
		get
		{
			return new LuaActions(this);
		}
	}

	public InputControlScheme KBMScheme
	{
		get
		{
			if (m_KBMSchemeIndex == -1)
			{
				m_KBMSchemeIndex = asset.FindControlSchemeIndex("KBM");
			}
			return asset.controlSchemes[m_KBMSchemeIndex];
		}
	}

	public InputControlScheme GamepadScheme
	{
		get
		{
			if (m_GamepadSchemeIndex == -1)
			{
				m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
			}
			return asset.controlSchemes[m_GamepadSchemeIndex];
		}
	}

	public Inputs()
	{
		_003Casset_003Ek__BackingField = InputActionAsset.FromJson("{\n    \"name\": \"Inputs\",\n    \"maps\": [\n        {\n            \"name\": \"EditMode\",\n            \"id\": \"ab6cd5a8-04ed-4939-96ec-f48fcbdf8ee9\",\n            \"actions\": [\n                {\n                    \"name\": \"Move Camera\",\n                    \"type\": \"Value\",\n                    \"id\": \"6bdfc79e-d9a0-48c3-b843-80e63c59d711\",\n                    \"expectedControlType\": \"Vector2\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Move Camera Up/Down\",\n                    \"type\": \"Value\",\n                    \"id\": \"22a4d0c2-3853-4c5c-aacf-34fda7445aa3\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Move Faster\",\n                    \"type\": \"Button\",\n                    \"id\": \"51ab6ee4-d4a7-4a9c-882f-1aa91e9c13ad\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Change Camera Mode\",\n                    \"type\": \"Button\",\n                    \"id\": \"7dcd9899-59f4-47c8-a9ce-df126f537f29\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Look\",\n                    \"type\": \"Value\",\n                    \"id\": \"c3af7e49-5a3c-40f2-b2ba-1d0c1f67ae66\",\n                    \"expectedControlType\": \"\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Zoom\",\n                    \"type\": \"Value\",\n                    \"id\": \"32dcd59d-63de-461e-8f25-75ec96df369b\",\n                    \"expectedControlType\": \"Axis\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Pose\",\n                    \"type\": \"Button\",\n                    \"id\": \"479c27e7-7cfd-402a-b354-877e3c97d77b\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Object\",\n                    \"type\": \"Button\",\n                    \"id\": \"ddd555d8-df99-49b4-b9e4-60a25d404b27\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Move Object\",\n                    \"type\": \"Button\",\n                    \"id\": \"6ca2997b-4387-42a0-9a74-3cb8c54fc0b9\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Micro\",\n                    \"type\": \"Button\",\n                    \"id\": \"c03128f4-7c9a-44f9-bb28-f6fa00205501\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Macro\",\n                    \"type\": \"Button\",\n                    \"id\": \"eda31bbc-557d-4cd6-9393-c60077cd8949\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Go to Selection\",\n                    \"type\": \"Button\",\n                    \"id\": \"110a29fe-851d-44f7-b52e-899c9388482d\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Delete Selection\",\n                    \"type\": \"Button\",\n                    \"id\": \"7085de9b-9512-4077-a224-dcb7eb194b2d\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                }\n            ],\n            \"bindings\": [\n                {\n                    \"name\": \"Primary\",\n                    \"id\": \"c6a47569-f233-4379-8566-3d5ac40cee86\",\n                    \"path\": \"2DVector(mode=2)\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Move Camera\",\n                    \"isComposite\": true,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"up\",\n                    \"id\": \"3d29da28-bb0e-4ab4-8f48-81170a160561\",\n                    \"path\": \"<Keyboard>/w\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KBM\",\n                    \"action\": \"Move Camera\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"down\",\n                    \"id\": \"3b6aec1e-00cd-487e-90fe-858befc653ee\",\n                    \"path\": \"<Keyboard>/s\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KBM\",\n                    \"action\": \"Move Camera\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"left\",\n                    \"id\": \"17b54a5f-e2fb-49cd-84bc-bb9d9929d46e\",\n                    \"path\": \"<Keyboard>/a\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KBM\",\n                    \"action\": \"Move Camera\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"right\",\n                    \"id\": \"1c4ab8a4-4c99-4bd6-a014-96b9740e2601\",\n                    \"path\": \"<Keyboard>/d\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KBM\",\n                    \"action\": \"Move Camera\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"Secondary\",\n                    \"id\": \"9e3d6487-b27b-4d1a-8fee-373b108e5bbe\",\n                    \"path\": \"2DVector(mode=2)\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Move Camera\",\n                    \"isComposite\": true,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"up\",\n                    \"id\": \"a9bc771a-b7e9-4152-9b7e-23586ad098bb\",\n                    \"path\": \"<Gamepad>/leftStick/up\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KBM\",\n                    \"action\": \"Move Camera\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"down\",\n                    \"id\": \"b77e04f5-65ae-4a25-8062-860fccd4e74e\",\n                    \"path\": \"<Gamepad>/leftStick/down\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KBM\",\n                    \"action\": \"Move Camera\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"left\",\n                    \"id\": \"b3582579-dc51-48e6-995e-9bb742a9ed04\",\n                    \"path\": \"<Gamepad>/leftStick/left\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KBM\",\n                    \"action\": \"Move Camera\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"right\",\n                    \"id\": \"6a14c7a5-0eab-4895-9ec2-428d9bd36245\",\n                    \"path\": \"<Gamepad>/leftStick/right\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KBM\",\n                    \"action\": \"Move Camera\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"ce044b93-9e1c-4db4-a52e-e8a1694f527c\",\n                    \"path\": \"<Keyboard>/leftShift\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KBM\",\n                    \"action\": \"Move Faster\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"ff5e2657-0156-4f99-adc0-dbe345ec8301\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Move Faster\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"1ef014c5-f2cf-49cb-8e24-2c922ba45128\",\n                    \"path\": \"<Keyboard>/delete\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Delete Selection\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"a96763ae-a127-4dd0-a63b-82a897176a60\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Delete Selection\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"ec8c7a2a-8e57-416c-848c-5e61e07b200d\",\n                    \"path\": \"<Keyboard>/f\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Go to Selection\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"4eace4f4-6ce3-4781-a08e-d7da5dc5f8c1\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Go to Selection\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"b7bbcdd0-766f-4155-b2ad-f7a493c03dd3\",\n                    \"path\": \"<Keyboard>/g\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Macro\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"887a20a9-7290-4139-b5f6-0c8cf30ac41f\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Macro\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"9da2b392-6b5a-4ba9-9351-2e12502210aa\",\n                    \"path\": \"<Keyboard>/n\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Micro\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"e93ab7a6-f8ef-44d9-b12e-b61ea9a9c46b\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Micro\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"76efc63e-4954-4f43-b2aa-f10db1009e89\",\n                    \"path\": \"<Keyboard>/m\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Move Object\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"270a883e-e6de-4bd5-832b-c70dbcae9047\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Move Object\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"e0bbec44-b0f6-486e-ae23-c16370a151a9\",\n                    \"path\": \"<Keyboard>/o\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Object\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"8659866b-e8ca-4bd3-9bdc-3c1fcf81b851\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Object\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"0a3c6121-4dca-4dde-990d-b3ae7b4f8706\",\n                    \"path\": \"<Keyboard>/p\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Pose\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"37fd0bbd-47e4-4dfb-9d32-c0e7644a6c3a\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Pose\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"Primary\",\n                    \"id\": \"0a75d096-1258-45f9-9822-c087af5d0256\",\n                    \"path\": \"1DAxis\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Move Camera Up/Down\",\n                    \"isComposite\": true,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"negative\",\n                    \"id\": \"6be275cc-1070-4299-bd4b-bd282cd64681\",\n                    \"path\": \"<Keyboard>/q\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Move Camera Up/Down\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"positive\",\n                    \"id\": \"b86ee986-54e3-45e0-b1dd-1a33bc83da4b\",\n                    \"path\": \"<Keyboard>/e\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Move Camera Up/Down\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"Secondary\",\n                    \"id\": \"8a11bba4-5963-43f1-a4eb-d20aae64bdb5\",\n                    \"path\": \"1DAxis\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Move Camera Up/Down\",\n                    \"isComposite\": true,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"negative\",\n                    \"id\": \"5cf23d8f-0ddb-4e95-8414-2eeac7bca0b3\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Move Camera Up/Down\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"positive\",\n                    \"id\": \"6ed51bc1-97a5-4b9a-a73a-a698a60c7c44\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Move Camera Up/Down\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"2d89df76-459d-4e65-86d1-f7d300877411\",\n                    \"path\": \"<Keyboard>/v\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KBM\",\n                    \"action\": \"Change Camera Mode\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"b8ed5da4-d2a0-4205-ac0d-880bea7db6bd\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Change Camera Mode\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"Primary\",\n                    \"id\": \"e2a7478c-c803-459d-af39-6235707eb5ad\",\n                    \"path\": \"1DAxis(minValue=-0.1,maxValue=0.1)\",\n                    \"interactions\": \"\",\n                    \"processors\": \"Clamp(min=-0.1,max=0.1)\",\n                    \"groups\": \"\",\n                    \"action\": \"Zoom\",\n                    \"isComposite\": true,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"negative\",\n                    \"id\": \"e45608e7-5485-4113-a674-392ff6778f08\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Zoom\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"positive\",\n                    \"id\": \"000dd4ae-cf2a-456e-8a22-b5301fae2e5d\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Zoom\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"Secondary\",\n                    \"id\": \"98f344b5-a823-4edd-aa7f-112e1c9787ab\",\n                    \"path\": \"1DAxis(minValue=-0.1,maxValue=0.1)\",\n                    \"interactions\": \"\",\n                    \"processors\": \"Clamp(min=-0.1,max=0.1)\",\n                    \"groups\": \"\",\n                    \"action\": \"Zoom\",\n                    \"isComposite\": true,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"negative\",\n                    \"id\": \"6ed1a8ce-0ac5-4636-bae4-0dfbadc8b405\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Zoom\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"positive\",\n                    \"id\": \"a77f278a-aefe-40c9-a1aa-88ada7a8fe43\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Zoom\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"4b76fca5-3356-4da2-8b01-56b7c736011b\",\n                    \"path\": \"<Mouse>/scroll/y\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Zoom\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"Primary\",\n                    \"id\": \"0f8bf8a0-176c-4815-af2f-a2c03bbb32a3\",\n                    \"path\": \"2DVector(mode=2)\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Look\",\n                    \"isComposite\": true,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"up\",\n                    \"id\": \"69748089-873d-4115-af44-678b0dd09c96\",\n                    \"path\": \"<Gamepad>/rightStick/up\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Look\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"down\",\n                    \"id\": \"68f51cc9-d536-4095-8ead-60a27e20b885\",\n                    \"path\": \"<Gamepad>/rightStick/down\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Look\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"left\",\n                    \"id\": \"57f4bf0e-7edc-4474-832b-bbb8e30f58a0\",\n                    \"path\": \"<Gamepad>/rightStick/left\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Look\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"right\",\n                    \"id\": \"297de944-d64b-4698-a7da-4276308f2191\",\n                    \"path\": \"<Gamepad>/rightStick/right\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Look\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"Secondary\",\n                    \"id\": \"5594b482-4151-46ad-8a30-eb21e35ea4df\",\n                    \"path\": \"2DVector(mode=2)\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Look\",\n                    \"isComposite\": true,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"up\",\n                    \"id\": \"7e892c00-b36e-4b1f-906c-1b9bea62c6a8\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Look\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"down\",\n                    \"id\": \"9cf8d529-12f8-4953-9ddc-394cfd452155\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Look\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"left\",\n                    \"id\": \"e23a9f8b-a5f4-45e4-9eda-5231596d1548\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Look\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"right\",\n                    \"id\": \"46adc970-f2c2-4163-b8a2-f99559b88be0\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Look\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"2c655903-5672-40ba-a55a-8f0ae3ed4e2c\",\n                    \"path\": \"<Mouse>/delta\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KBM\",\n                    \"action\": \"Look\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"923d4b39-db1a-4aa1-8f5e-6e94c8d962b9\",\n                    \"path\": \"<Touchscreen>/delta\",\n                    \"interactions\": \"\",\n                    \"processors\": \"InvertVector2\",\n                    \"groups\": \"KBM\",\n                    \"action\": \"Look\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                }\n            ]\n        },\n        {\n            \"name\": \"Player\",\n            \"id\": \"12073173-23ea-4888-9c32-036ce1b1e86a\",\n            \"actions\": [\n                {\n                    \"name\": \"Move\",\n                    \"type\": \"Value\",\n                    \"id\": \"5436bc99-bf9a-424c-a5af-af309eb0632f\",\n                    \"expectedControlType\": \"Vector2\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Sprint\",\n                    \"type\": \"Button\",\n                    \"id\": \"f59d3b44-40ef-4dff-bf7b-e9e148a4bf10\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Change Camera\",\n                    \"type\": \"Button\",\n                    \"id\": \"100c4c85-18ff-4b1d-9d7c-aadbf0529019\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Change Size\",\n                    \"type\": \"Value\",\n                    \"id\": \"63d58c36-063d-4ce5-b86e-bd8fb7c01b6d\",\n                    \"expectedControlType\": \"Axis\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Look\",\n                    \"type\": \"Value\",\n                    \"id\": \"27eaf064-304e-49c9-b94d-4b7dff50c286\",\n                    \"expectedControlType\": \"\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Zoom\",\n                    \"type\": \"Value\",\n                    \"id\": \"7dc3676c-6f7b-4521-927a-68761c8aeea3\",\n                    \"expectedControlType\": \"Axis\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Look Back\",\n                    \"type\": \"Button\",\n                    \"id\": \"b94cbb3d-5b0d-4210-bdb0-c08d3bd5bac7\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Auto Walk\",\n                    \"type\": \"Button\",\n                    \"id\": \"1e347342-f4d5-4bef-85bb-66fcff14500c\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                }\n            ],\n            \"bindings\": [\n                {\n                    \"name\": \"Primary\",\n                    \"id\": \"7c5ac118-a4f1-48ba-b59a-b4794e6a8999\",\n                    \"path\": \"2DVector(mode=2)\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Move\",\n                    \"isComposite\": true,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"up\",\n                    \"id\": \"d9af1bb5-ed81-4b6d-85fb-396b04c16db9\",\n                    \"path\": \"<Keyboard>/w\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KBM\",\n                    \"action\": \"Move\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"down\",\n                    \"id\": \"51dff5bf-576b-4552-8ce4-41af810b6fd8\",\n                    \"path\": \"<Keyboard>/s\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KBM\",\n                    \"action\": \"Move\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"left\",\n                    \"id\": \"fae50e6d-a5d0-4ec4-b1d2-7ea36b422ec1\",\n                    \"path\": \"<Keyboard>/a\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KBM\",\n                    \"action\": \"Move\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"right\",\n                    \"id\": \"64cc5f07-a1a9-4ff3-ad8f-7dbf292ece3e\",\n                    \"path\": \"<Keyboard>/d\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KBM\",\n                    \"action\": \"Move\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"Secondary\",\n                    \"id\": \"caf069f3-7ad7-4922-8206-3d328f7a7241\",\n                    \"path\": \"2DVector(mode=2)\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Move\",\n                    \"isComposite\": true,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"up\",\n                    \"id\": \"46816443-54e9-4711-ad32-d92905b5e703\",\n                    \"path\": \"<Gamepad>/leftStick/up\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KBM\",\n                    \"action\": \"Move\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"down\",\n                    \"id\": \"f9967b2d-82f3-4002-b209-fbfcebd97a56\",\n                    \"path\": \"<Gamepad>/leftStick/down\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KBM\",\n                    \"action\": \"Move\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"left\",\n                    \"id\": \"7c267b25-be3f-4b9d-95b4-6a5f4688a189\",\n                    \"path\": \"<Gamepad>/leftStick/left\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KBM\",\n                    \"action\": \"Move\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"right\",\n                    \"id\": \"d525bdfe-d19b-454d-aa77-0b5d4de9256d\",\n                    \"path\": \"<Gamepad>/leftStick/right\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KBM\",\n                    \"action\": \"Move\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"dbc164fa-d9d0-4695-9e2c-d7d56425c4d5\",\n                    \"path\": \"<Keyboard>/leftShift\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KBM\",\n                    \"action\": \"Sprint\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"1af3a0a3-7d5a-4081-a884-4123c47c9134\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Sprint\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"71d99310-b753-4482-b318-44e309bf4fdf\",\n                    \"path\": \"<Keyboard>/v\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KBM\",\n                    \"action\": \"Change Camera\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"3524157b-8afd-4ed7-91a9-edf490e1a185\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Change Camera\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"Primary\",\n                    \"id\": \"01bef77f-0e13-4be2-867b-ffe32b3f0d74\",\n                    \"path\": \"1DAxis\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Change Size\",\n                    \"isComposite\": true,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"negative\",\n                    \"id\": \"a8cb7cea-be23-44da-bdfe-8334cff48046\",\n                    \"path\": \"<Keyboard>/z\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Change Size\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"positive\",\n                    \"id\": \"8d4d1308-ba25-4fec-ac33-69689a5f3547\",\n                    \"path\": \"<Keyboard>/x\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Change Size\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"Secondary\",\n                    \"id\": \"c33fdc5c-1f15-4b42-a9f2-79e71cf6bd85\",\n                    \"path\": \"1DAxis\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Change Size\",\n                    \"isComposite\": true,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"negative\",\n                    \"id\": \"c64ce426-9f2a-47a7-afff-c3a5d9b6c50f\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Change Size\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"positive\",\n                    \"id\": \"30e3fa13-dda7-4598-b3c5-eb56dea2cc7d\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Change Size\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"Primary\",\n                    \"id\": \"381ebaf0-94c4-435f-a87d-67cb1258bb35\",\n                    \"path\": \"2DVector(mode=2)\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Look\",\n                    \"isComposite\": true,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"up\",\n                    \"id\": \"ba032a9c-74bd-4891-ad1b-f3a976e7bbaa\",\n                    \"path\": \"<Gamepad>/rightStick/up\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Look\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"down\",\n                    \"id\": \"3c863e3a-e756-44ad-9528-b36c3be70d04\",\n                    \"path\": \"<Gamepad>/rightStick/down\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Look\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"left\",\n                    \"id\": \"3b08b1a0-36cc-498e-8d36-53c3935fae64\",\n                    \"path\": \"<Gamepad>/rightStick/left\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Look\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"right\",\n                    \"id\": \"dd7e2142-992c-439b-9d69-080ee22a89a5\",\n                    \"path\": \"<Gamepad>/rightStick/right\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Look\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"Secondary\",\n                    \"id\": \"788cf94b-056a-40a8-b09b-cae2a91c66a8\",\n                    \"path\": \"2DVector(mode=2)\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Look\",\n                    \"isComposite\": true,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"up\",\n                    \"id\": \"6d3c7d5d-b1f0-42f8-b2eb-22f91250bb0b\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Look\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"down\",\n                    \"id\": \"097a6eb7-e8cc-4c0f-b80c-7216bcd925df\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Look\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"left\",\n                    \"id\": \"ee8f6eea-4f80-48d5-abfb-0bc0ac850352\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Look\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"right\",\n                    \"id\": \"1efd5e03-f924-442c-baef-e921f00388ac\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Look\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"8540e8e9-f26e-4ee0-820a-3cd06ecd7673\",\n                    \"path\": \"<Mouse>/delta\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KBM\",\n                    \"action\": \"Look\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"a5673fba-eb18-45b8-ae7e-0c83f89ead10\",\n                    \"path\": \"<Touchscreen>/delta\",\n                    \"interactions\": \"\",\n                    \"processors\": \"InvertVector2\",\n                    \"groups\": \"KBM\",\n                    \"action\": \"Look\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"Primary\",\n                    \"id\": \"d30b99c2-106d-42f6-9911-08acbb06be54\",\n                    \"path\": \"1DAxis(minValue=-0.1,maxValue=0.1)\",\n                    \"interactions\": \"\",\n                    \"processors\": \"Clamp(min=-0.1,max=0.1)\",\n                    \"groups\": \"\",\n                    \"action\": \"Zoom\",\n                    \"isComposite\": true,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"negative\",\n                    \"id\": \"b870a697-b9dd-4d38-a7c2-7ecc8b4a60bc\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Zoom\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"positive\",\n                    \"id\": \"a4230871-cfc9-4f3c-a0d9-d6847e39d904\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Zoom\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"Secondary\",\n                    \"id\": \"a4838848-2f18-47ac-9bef-9d572f76c15f\",\n                    \"path\": \"1DAxis(minValue=-0.1,maxValue=0.1)\",\n                    \"interactions\": \"\",\n                    \"processors\": \"Clamp(min=-0.1,max=0.1)\",\n                    \"groups\": \"\",\n                    \"action\": \"Zoom\",\n                    \"isComposite\": true,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"negative\",\n                    \"id\": \"9dba74ea-90a8-427b-8441-6fbb4770335a\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Zoom\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"positive\",\n                    \"id\": \"01504ad1-a77f-455d-92b0-4fcd0d336150\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Zoom\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"e354db56-0a11-4c81-bf27-4d702448aedb\",\n                    \"path\": \"<Mouse>/scroll/y\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Zoom\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"8015ee0b-9ed5-4bcb-865c-6c8551cf5b24\",\n                    \"path\": \"<Mouse>/middleButton\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Look Back\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"53e1323a-5401-4c7b-a5a9-6505b56ab750\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Look Back\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"072c3cc2-a13a-4c6f-97ba-dbffc7cb10b6\",\n                    \"path\": \"<Keyboard>/rightShift\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Auto Walk\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"1d1f9c88-2b1c-4da1-bc1a-4c2f2b158912\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Auto Walk\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                }\n            ]\n        },\n        {\n            \"name\": \"Macro\",\n            \"id\": \"9f18619a-9c68-4201-8b02-63e1f9fcce97\",\n            \"actions\": [\n                {\n                    \"name\": \"Stomp\",\n                    \"type\": \"Button\",\n                    \"id\": \"dd4c2f3c-f5ab-416a-b2f9-2e0c3a0f650f\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Cancel\",\n                    \"type\": \"Button\",\n                    \"id\": \"f7ea651c-a0ed-4cae-b8ca-d6d60afc30f3\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                }\n            ],\n            \"bindings\": [\n                {\n                    \"name\": \"\",\n                    \"id\": \"ee13cfce-e2dd-4d29-9355-cb15f39efa97\",\n                    \"path\": \"<Mouse>/leftButton\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Stomp\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"499bdc4a-f853-4b84-bb85-13889a7d24d0\",\n                    \"path\": \"<Gamepad>/leftStickPress\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Stomp\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"75850fcd-b8c7-4fad-9b19-46b700cb542e\",\n                    \"path\": \"<Mouse>/rightButton\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Cancel\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"c0f8df4f-9343-4952-8057-0974fcdaa89f\",\n                    \"path\": \"<Gamepad>/rightStickPress\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Cancel\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                }\n            ]\n        },\n        {\n            \"name\": \"Micro\",\n            \"id\": \"052640b5-6ea6-421f-b2c5-32851ff06f3d\",\n            \"actions\": [\n                {\n                    \"name\": \"Crouch\",\n                    \"type\": \"Button\",\n                    \"id\": \"59bc8c52-9155-4def-a83a-44f012368b33\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Fly\",\n                    \"type\": \"Button\",\n                    \"id\": \"bf853018-bbdf-4e31-8080-7075bba29797\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Interact\",\n                    \"type\": \"Button\",\n                    \"id\": \"98e420bb-435b-4779-8b55-5addb837888e\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Jump\",\n                    \"type\": \"Button\",\n                    \"id\": \"e4592b68-963b-4df2-80da-6cc2064f6db3\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Walk\",\n                    \"type\": \"Button\",\n                    \"id\": \"1eb87da8-3257-4211-85fb-12594d9a0a87\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Weapon Aim\",\n                    \"type\": \"Button\",\n                    \"id\": \"fb338a9b-24a0-491f-9139-55bd96b03d30\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Weapon Fire\",\n                    \"type\": \"Button\",\n                    \"id\": \"703d4cc1-c718-4068-9370-d55d68d9c1c0\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Weapon Mode\",\n                    \"type\": \"Button\",\n                    \"id\": \"baa45ecd-9493-4390-92bb-93ca25cbd8fb\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Weapon Range\",\n                    \"type\": \"Value\",\n                    \"id\": \"f84e4c7e-de8f-483e-8e19-973c0698f96e\",\n                    \"expectedControlType\": \"\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Super Fly\",\n                    \"type\": \"Button\",\n                    \"id\": \"cfd52d35-9d99-4a7b-bd75-8fba49d4c0a8\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Flying Punch\",\n                    \"type\": \"Button\",\n                    \"id\": \"7e4f1f4b-d745-4eb0-a00f-29a46dfcbcfa\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Fly Down\",\n                    \"type\": \"Button\",\n                    \"id\": \"05661b31-8fad-4756-afc8-8a6b6c15568b\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                }\n            ],\n            \"bindings\": [\n                {\n                    \"name\": \"\",\n                    \"id\": \"423a1d17-3021-4a25-8d9e-042970199775\",\n                    \"path\": \"<Keyboard>/space\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Jump\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"e467d851-836f-4f6c-8055-e58d96940a5a\",\n                    \"path\": \"<Gamepad>/buttonSouth\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad;KBM\",\n                    \"action\": \"Jump\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"1d180e00-eccb-46ea-b299-e3a84e2df627\",\n                    \"path\": \"<Keyboard>/e\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KBM\",\n                    \"action\": \"Fly\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"a004bab3-47c8-4766-b406-8506eaea7dc6\",\n                    \"path\": \"<Gamepad>/rightStickPress\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Fly\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"c722ff2c-3a56-4bbc-8f7a-149accf84e65\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Walk\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"6cb67a00-409c-418e-8161-3de486a9e618\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Walk\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"09b6ccca-8fc8-4c84-a4f4-81d28a33ec06\",\n                    \"path\": \"<Mouse>/rightButton\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Weapon Aim\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"3c5875a3-9663-4031-a2f4-74b1e23e8aad\",\n                    \"path\": \"<Gamepad>/rightTriggerButton\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"Weapon Aim\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"59e82532-939c-4965-9cae-e32174793487\",\n                    \"path\": \"<Mouse>/leftButton\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KBM\",\n                    \"action\": \"Weapon Fire\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"da007472-ab2e-4b5a-8192-1d39ec4bb2cd\",\n                    \"path\": \"<Gamepad>/rightTrigger\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"Weapon Fire\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"Primary\",\n                    \"id\": \"cc02a9d2-b1dc-42f0-a39a-8be9e5aa815d\",\n                    \"path\": \"1DAxis\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Weapon Range\",\n                    \"isComposite\": true,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"negative\",\n                    \"id\": \"35a270a2-9e36-41ed-8e44-bf597e9ca8b6\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Weapon Range\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"positive\",\n                    \"id\": \"56377fe4-8176-447d-b2e8-916ce7085e54\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Weapon Range\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"Secondary\",\n                    \"id\": \"b708fea2-0989-4b14-954b-267bb226c5ec\",\n                    \"path\": \"1DAxis\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Weapon Range\",\n                    \"isComposite\": true,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"negative\",\n                    \"id\": \"8384a610-298f-4021-9a2c-97da391c8738\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Weapon Range\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"positive\",\n                    \"id\": \"c7a4b3d1-5c7d-4dbb-9228-2150509ddf79\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Weapon Range\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"213b689d-ddd0-465e-86e3-2802bd27657b\",\n                    \"path\": \"<Mouse>/scroll/y\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Weapon Range\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"e7921f2a-e2a7-4ea7-a3a1-7113f5da26d2\",\n                    \"path\": \"<Mouse>/middleButton\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Weapon Mode\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"23d00ed2-a011-4491-bae1-9ca7794355ee\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Weapon Mode\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"f98037d3-01aa-4a23-a010-2ae6fd257d17\",\n                    \"path\": \"<Keyboard>/leftAlt\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KBM\",\n                    \"action\": \"Super Fly\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"0fcfe928-be6f-446e-b070-9004412a89c8\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Super Fly\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"c540a99e-8170-4a33-9ef7-5fcdb8064113\",\n                    \"path\": \"<Mouse>/leftButton\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KBM\",\n                    \"action\": \"Flying Punch\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"7796b490-4fd6-4ca2-a07c-091d9023056a\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Flying Punch\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"c66ec358-c97e-4fee-8687-4e929ec4f849\",\n                    \"path\": \"<Keyboard>/c\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KBM\",\n                    \"action\": \"Crouch\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"4c025540-9968-42d6-bbd0-d454b6511908\",\n                    \"path\": \"<Gamepad>/leftStickPress\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Crouch\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"c426d288-a50c-40db-8820-cc0f667565fc\",\n                    \"path\": \"<Keyboard>/leftCtrl\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KBM\",\n                    \"action\": \"Fly Down\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"4480f0ee-2107-4d58-a1a1-6900e06c9a52\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Fly Down\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"46dcc00d-aa6b-4b38-b70d-feae4481512c\",\n                    \"path\": \"<Keyboard>/enter\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Interact\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"38953e79-2688-43c2-8d20-4ee129b5f171\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Interact\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                }\n            ]\n        },\n        {\n            \"name\": \"Misc\",\n            \"id\": \"2432e284-a936-4a49-a8b8-e96dc6dc1984\",\n            \"actions\": [\n                {\n                    \"name\": \"Alternative Pause\",\n                    \"type\": \"Value\",\n                    \"id\": \"fdec07bd-23d3-49ed-bdbb-4a20bbab8d5c\",\n                    \"expectedControlType\": \"Axis\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Bullet Time (Hold)\",\n                    \"type\": \"Button\",\n                    \"id\": \"82bafbf7-c47d-498c-b2a4-e03944fc4a5b\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Bullet Time (Toggle)\",\n                    \"type\": \"Button\",\n                    \"id\": \"6d7fdeac-733d-4a8f-a318-209a6b8dcf40\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Console\",\n                    \"type\": \"Button\",\n                    \"id\": \"db19b532-abbd-4f3a-a961-5191be10a561\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Freeze Macros\",\n                    \"type\": \"Button\",\n                    \"id\": \"fc4389a4-f9d0-403f-b857-863c178f3a13\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Macro Speed\",\n                    \"type\": \"Value\",\n                    \"id\": \"4a38b4bd-50a7-4c92-8fab-b027227ebeee\",\n                    \"expectedControlType\": \"\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Reset Macro Speed\",\n                    \"type\": \"Button\",\n                    \"id\": \"d552428c-9a75-43d8-a7eb-2a620f09a34d\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Screenshot\",\n                    \"type\": \"Button\",\n                    \"id\": \"2e4508f0-4700-4f94-ac74-592b3e23245c\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Switch Mode\",\n                    \"type\": \"Button\",\n                    \"id\": \"22f9f1e5-4c76-4f6d-ad44-1bb2de735d0d\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Spawn Micro\",\n                    \"type\": \"Value\",\n                    \"id\": \"c5de175b-26c8-4334-a252-7db1fe1ed16a\",\n                    \"expectedControlType\": \"Axis\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                }\n            ],\n            \"bindings\": [\n                {\n                    \"name\": \"\",\n                    \"id\": \"e63dec0f-8068-4daa-b729-539f18eb6018\",\n                    \"path\": \"<Keyboard>/printScreen\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KBM\",\n                    \"action\": \"Screenshot\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"4825e614-a649-407d-93fd-89bf9e89ef10\",\n                    \"path\": \"<Keyboard>/f11\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Screenshot\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"dfdf9107-bed9-49b8-89cb-143c474a5d04\",\n                    \"path\": \"<Keyboard>/backquote\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KBM\",\n                    \"action\": \"Console\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"3e2beb39-bc90-4aee-9816-2686e4177f3b\",\n                    \"path\": \"<Keyboard>/f12\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Console\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"6171ed87-6403-4b67-9abb-867b777d680b\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Alternative Pause\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"3f62a3bd-28db-4935-bcfe-1788840efb6e\",\n                    \"path\": \"<Gamepad>/start\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Alternative Pause\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"1eb9b571-b2ce-47bc-bcab-f8d7af6f894c\",\n                    \"path\": \"<Keyboard>/tab\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Switch Mode\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"85a01211-870a-4b14-9f74-791a8a3e97d9\",\n                    \"path\": \"<Gamepad>/select\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Switch Mode\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"Primary\",\n                    \"id\": \"97185e50-b10b-4a3b-8552-6632b7d6ec0f\",\n                    \"path\": \"1DAxis\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Macro Speed\",\n                    \"isComposite\": true,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"negative\",\n                    \"id\": \"b54a35a7-e339-4949-95d6-af25355f5cae\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Macro Speed\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"positive\",\n                    \"id\": \"daf012b5-5290-46cc-b17b-bfe65af8113e\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Macro Speed\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"Secondary\",\n                    \"id\": \"11bb9dcc-7110-4348-abfe-e18bb2e1c645\",\n                    \"path\": \"1DAxis\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Macro Speed\",\n                    \"isComposite\": true,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"negative\",\n                    \"id\": \"5bf08ba0-a5fd-4cc6-9310-c3b737f30283\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Macro Speed\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"positive\",\n                    \"id\": \"7fe973cc-c90b-48a7-9bee-afd517320076\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Macro Speed\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"4c3de300-cfae-4ce9-afcb-e0519babbde1\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Reset Macro Speed\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"2b79de88-e9b9-48bd-a489-726bbb172912\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Reset Macro Speed\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"d0502dae-c757-4eba-8a76-1f1d0c096080\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Freeze Macros\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"ac529f2a-38d6-429c-8c1d-c5edf42a7b21\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Freeze Macros\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"74dea218-a22b-4036-912d-5cbf18cd7465\",\n                    \"path\": \"<Keyboard>/b\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Bullet Time (Toggle)\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"2b913a10-cade-42c3-b8b1-22aee9520abd\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Bullet Time (Toggle)\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"147001ab-8c1a-4c0f-8fbd-8b8bbda520b5\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Bullet Time (Hold)\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"9851fd94-1e0c-4d89-88c4-5855baa26490\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Bullet Time (Hold)\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"Primary\",\n                    \"id\": \"59f20b00-34c8-48f3-ad14-f344e7275893\",\n                    \"path\": \"1DAxis\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Spawn Micro\",\n                    \"isComposite\": true,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"negative\",\n                    \"id\": \"48c33ba3-b408-4310-9467-2655e3067cd6\",\n                    \"path\": \"<Keyboard>/o\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Spawn Micro\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"positive\",\n                    \"id\": \"42acc9e7-b1a0-492a-8c92-c00f50dec5c5\",\n                    \"path\": \"<Keyboard>/p\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Spawn Micro\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"Secondary\",\n                    \"id\": \"c8b65b00-f7b6-4259-8459-2c1858235264\",\n                    \"path\": \"1DAxis\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Spawn Micro\",\n                    \"isComposite\": true,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"negative\",\n                    \"id\": \"b75ef52d-d1e6-4b8a-8fdc-68107096e5d5\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Spawn Micro\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"positive\",\n                    \"id\": \"630ea48f-1879-4785-9712-baf63b9944bc\",\n                    \"path\": \"\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Spawn Micro\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                }\n            ]\n        },\n        {\n            \"name\": \"Interface\",\n            \"id\": \"79455639-cdeb-4800-bf6c-c7979ad1be79\",\n            \"actions\": [\n                {\n                    \"name\": \"Move\",\n                    \"type\": \"Value\",\n                    \"id\": \"533ede30-61db-457e-91a2-726c1d81e62e\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Select\",\n                    \"type\": \"Button\",\n                    \"id\": \"c19d0b4e-1a4f-402b-bbce-19933805c00f\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Back\",\n                    \"type\": \"Button\",\n                    \"id\": \"547a2036-e3e9-4568-bee0-e0e59b1832ce\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Scroll\",\n                    \"type\": \"Value\",\n                    \"id\": \"1c5c1ae4-cb48-4df2-bda9-dcd88cf5fb8e\",\n                    \"expectedControlType\": \"\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Pointer\",\n                    \"type\": \"Value\",\n                    \"id\": \"e0f382f4-eb2a-4355-bc3f-0e0abac627df\",\n                    \"expectedControlType\": \"\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Left Click\",\n                    \"type\": \"Button\",\n                    \"id\": \"87589013-2ce3-47ad-9fcb-a50a991abff7\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Right Click\",\n                    \"type\": \"Button\",\n                    \"id\": \"b79cd49a-bebd-478a-b5f2-65c5fe413c85\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Middle Click\",\n                    \"type\": \"Button\",\n                    \"id\": \"5fab4bf1-ac21-4cf9-8546-53ad18ccf182\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Position\",\n                    \"type\": \"Button\",\n                    \"id\": \"7f8e767a-7ff4-48ca-9586-2b986f53321a\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                },\n                {\n                    \"name\": \"Orientation\",\n                    \"type\": \"Button\",\n                    \"id\": \"47190629-ac9f-4cd4-917e-2592840fe345\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\"\n                }\n            ],\n            \"bindings\": [\n                {\n                    \"name\": \"Keyboard Arrows\",\n                    \"id\": \"d0ed8de8-de5a-44d9-845c-7cd5f31bc23e\",\n                    \"path\": \"2DVector(mode=1)\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Move\",\n                    \"isComposite\": true,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"up\",\n                    \"id\": \"5e6577b0-03b4-45dc-8b0c-4d53c40d19d0\",\n                    \"path\": \"<Keyboard>/upArrow\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Move\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"down\",\n                    \"id\": \"e70e4446-cb08-410d-aa8f-f7d65e5ca118\",\n                    \"path\": \"<Keyboard>/downArrow\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Move\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"left\",\n                    \"id\": \"1ee7e6a6-26e5-44e2-8275-a1f5f28f7a68\",\n                    \"path\": \"<Keyboard>/leftArrow\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Move\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"right\",\n                    \"id\": \"bd08efbc-955c-4fbf-8775-67ac2ef815f3\",\n                    \"path\": \"<Keyboard>/rightArrow\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Move\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"d800cf85-5d9c-403d-94ca-cb1f55e18847\",\n                    \"path\": \"<Gamepad>/dpad\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Move\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"27a143b5-a183-40b9-9875-47e4c7b174b2\",\n                    \"path\": \"<Gamepad>/buttonSouth\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Select\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"7aaa5209-c837-445c-827e-4818d871f2b6\",\n                    \"path\": \"<Keyboard>/enter\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Select\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"d6256c1f-acf2-46ae-937e-46723b118d2f\",\n                    \"path\": \"<Gamepad>/buttonEast\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Back\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"33e69f62-7ec6-4587-a265-7714c01124c1\",\n                    \"path\": \"<Keyboard>/backspace\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Back\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"2b8750f5-9a04-4186-84dc-c65b00d10445\",\n                    \"path\": \"<Mouse>/scroll\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Scroll\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"cb848daa-8086-4b53-a93d-aefe951c022b\",\n                    \"path\": \"<Mouse>/position\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Pointer\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"59b87576-6393-4279-8bdb-e5235a282f0f\",\n                    \"path\": \"<Mouse>/leftButton\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Left Click\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"679ccddd-7830-4a5a-bbc4-2fcd630ab4ac\",\n                    \"path\": \"<Mouse>/rightButton\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Right Click\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"58924c78-4097-489e-a16c-4c82e8b44068\",\n                    \"path\": \"<Mouse>/middleButton\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Middle Click\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"b009834f-d6b7-407c-a882-08c5f58f1c9d\",\n                    \"path\": \"<TrackedDevice>/devicePosition\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Position\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"a83badc5-4c0b-482b-a87b-ddc8332cef24\",\n                    \"path\": \"<TrackedDevice>/deviceRotation\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Orientation\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                }\n            ]\n        },\n        {\n            \"name\": \"Lua\",\n            \"id\": \"f6910b02-8c5c-41cb-8803-6a17772b1e71\",\n            \"actions\": [],\n            \"bindings\": []\n        }\n    ],\n    \"controlSchemes\": [\n        {\n            \"name\": \"KBM\",\n            \"bindingGroup\": \"KBM\",\n            \"devices\": []\n        },\n        {\n            \"name\": \"Gamepad\",\n            \"bindingGroup\": \"Gamepad\",\n            \"devices\": []\n        }\n    ]\n}");
		m_EditMode = asset.FindActionMap("EditMode", true);
		m_EditMode_MoveCamera = m_EditMode.FindAction("Move Camera", true);
		m_EditMode_MoveCameraUpDown = m_EditMode.FindAction("Move Camera Up/Down", true);
		m_EditMode_MoveFaster = m_EditMode.FindAction("Move Faster", true);
		m_EditMode_ChangeCameraMode = m_EditMode.FindAction("Change Camera Mode", true);
		m_EditMode_Look = m_EditMode.FindAction("Look", true);
		m_EditMode_Zoom = m_EditMode.FindAction("Zoom", true);
		m_EditMode_Pose = m_EditMode.FindAction("Pose", true);
		m_EditMode_Object = m_EditMode.FindAction("Object", true);
		m_EditMode_MoveObject = m_EditMode.FindAction("Move Object", true);
		m_EditMode_Micro = m_EditMode.FindAction("Micro", true);
		m_EditMode_Macro = m_EditMode.FindAction("Macro", true);
		m_EditMode_GotoSelection = m_EditMode.FindAction("Go to Selection", true);
		m_EditMode_DeleteSelection = m_EditMode.FindAction("Delete Selection", true);
		m_Player = asset.FindActionMap("Player", true);
		m_Player_Move = m_Player.FindAction("Move", true);
		m_Player_Sprint = m_Player.FindAction("Sprint", true);
		m_Player_ChangeCamera = m_Player.FindAction("Change Camera", true);
		m_Player_ChangeSize = m_Player.FindAction("Change Size", true);
		m_Player_Look = m_Player.FindAction("Look", true);
		m_Player_Zoom = m_Player.FindAction("Zoom", true);
		m_Player_LookBack = m_Player.FindAction("Look Back", true);
		m_Player_AutoWalk = m_Player.FindAction("Auto Walk", true);
		m_Macro = asset.FindActionMap("Macro", true);
		m_Macro_Stomp = m_Macro.FindAction("Stomp", true);
		m_Macro_Cancel = m_Macro.FindAction("Cancel", true);
		m_Micro = asset.FindActionMap("Micro", true);
		m_Micro_Crouch = m_Micro.FindAction("Crouch", true);
		m_Micro_Fly = m_Micro.FindAction("Fly", true);
		m_Micro_Interact = m_Micro.FindAction("Interact", true);
		m_Micro_Jump = m_Micro.FindAction("Jump", true);
		m_Micro_Walk = m_Micro.FindAction("Walk", true);
		m_Micro_WeaponAim = m_Micro.FindAction("Weapon Aim", true);
		m_Micro_WeaponFire = m_Micro.FindAction("Weapon Fire", true);
		m_Micro_WeaponMode = m_Micro.FindAction("Weapon Mode", true);
		m_Micro_WeaponRange = m_Micro.FindAction("Weapon Range", true);
		m_Micro_SuperFly = m_Micro.FindAction("Super Fly", true);
		m_Micro_FlyingPunch = m_Micro.FindAction("Flying Punch", true);
		m_Micro_FlyDown = m_Micro.FindAction("Fly Down", true);
		m_Misc = asset.FindActionMap("Misc", true);
		m_Misc_AlternativePause = m_Misc.FindAction("Alternative Pause", true);
		m_Misc_BulletTimeHold = m_Misc.FindAction("Bullet Time (Hold)", true);
		m_Misc_BulletTimeToggle = m_Misc.FindAction("Bullet Time (Toggle)", true);
		m_Misc_Console = m_Misc.FindAction("Console", true);
		m_Misc_FreezeMacros = m_Misc.FindAction("Freeze Macros", true);
		m_Misc_MacroSpeed = m_Misc.FindAction("Macro Speed", true);
		m_Misc_ResetMacroSpeed = m_Misc.FindAction("Reset Macro Speed", true);
		m_Misc_Screenshot = m_Misc.FindAction("Screenshot", true);
		m_Misc_SwitchMode = m_Misc.FindAction("Switch Mode", true);
		m_Misc_SpawnMicro = m_Misc.FindAction("Spawn Micro", true);
		m_Interface = asset.FindActionMap("Interface", true);
		m_Interface_Move = m_Interface.FindAction("Move", true);
		m_Interface_Select = m_Interface.FindAction("Select", true);
		m_Interface_Back = m_Interface.FindAction("Back", true);
		m_Interface_Scroll = m_Interface.FindAction("Scroll", true);
		m_Interface_Pointer = m_Interface.FindAction("Pointer", true);
		m_Interface_LeftClick = m_Interface.FindAction("Left Click", true);
		m_Interface_RightClick = m_Interface.FindAction("Right Click", true);
		m_Interface_MiddleClick = m_Interface.FindAction("Middle Click", true);
		m_Interface_Position = m_Interface.FindAction("Position", true);
		m_Interface_Orientation = m_Interface.FindAction("Orientation", true);
		m_Lua = asset.FindActionMap("Lua", true);
	}

	public void Dispose()
	{
		UnityEngine.Object.Destroy(asset);
	}

	public bool Contains(InputAction action)
	{
		return asset.Contains(action);
	}

	public IEnumerator<InputAction> GetEnumerator()
	{
		return asset.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public void Enable()
	{
		asset.Enable();
	}

	public void Disable()
	{
		asset.Disable();
	}
}
