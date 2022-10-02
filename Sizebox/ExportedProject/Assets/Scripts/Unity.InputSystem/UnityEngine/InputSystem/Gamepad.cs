using System.ComponentModel;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Haptics;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem
{
	[InputControlLayout(stateType = typeof(GamepadState), isGenericTypeOfDevice = true)]
	[Preserve]
	public class Gamepad : InputDevice, IDualMotorRumble, IHaptics
	{
		private DualMotorRumble m_Rumble;

		private static int s_GamepadCount;

		private static Gamepad[] s_Gamepads;

		public ButtonControl buttonWest { get; private set; }

		public ButtonControl buttonNorth { get; private set; }

		public ButtonControl buttonSouth { get; private set; }

		public ButtonControl buttonEast { get; private set; }

		public ButtonControl leftStickButton { get; private set; }

		public ButtonControl rightStickButton { get; private set; }

		public ButtonControl startButton { get; private set; }

		public ButtonControl selectButton { get; private set; }

		public DpadControl dpad { get; private set; }

		public ButtonControl leftShoulder { get; private set; }

		public ButtonControl rightShoulder { get; private set; }

		public StickControl leftStick { get; private set; }

		public StickControl rightStick { get; private set; }

		public ButtonControl leftTrigger { get; private set; }

		public ButtonControl rightTrigger { get; private set; }

		public ButtonControl aButton
		{
			get
			{
				return buttonSouth;
			}
		}

		public ButtonControl bButton
		{
			get
			{
				return buttonEast;
			}
		}

		public ButtonControl xButton
		{
			get
			{
				return buttonWest;
			}
		}

		public ButtonControl yButton
		{
			get
			{
				return buttonNorth;
			}
		}

		public ButtonControl triangleButton
		{
			get
			{
				return buttonNorth;
			}
		}

		public ButtonControl squareButton
		{
			get
			{
				return buttonWest;
			}
		}

		public ButtonControl circleButton
		{
			get
			{
				return buttonEast;
			}
		}

		public ButtonControl crossButton
		{
			get
			{
				return buttonSouth;
			}
		}

		public ButtonControl this[GamepadButton button]
		{
			get
			{
				switch (button)
				{
				case GamepadButton.North:
					return buttonNorth;
				case GamepadButton.South:
					return buttonSouth;
				case GamepadButton.East:
					return buttonEast;
				case GamepadButton.West:
					return buttonWest;
				case GamepadButton.Start:
					return startButton;
				case GamepadButton.Select:
					return selectButton;
				case GamepadButton.LeftShoulder:
					return leftShoulder;
				case GamepadButton.RightShoulder:
					return rightShoulder;
				case GamepadButton.LeftTrigger:
					return leftTrigger;
				case GamepadButton.RightTrigger:
					return rightTrigger;
				case GamepadButton.LeftStick:
					return leftStickButton;
				case GamepadButton.RightStick:
					return rightStickButton;
				case GamepadButton.DpadUp:
					return dpad.up;
				case GamepadButton.DpadDown:
					return dpad.down;
				case GamepadButton.DpadLeft:
					return dpad.left;
				case GamepadButton.DpadRight:
					return dpad.right;
				default:
					throw new InvalidEnumArgumentException("button", (int)button, typeof(GamepadButton));
				}
			}
		}

		public static Gamepad current { get; private set; }

		public new static ReadOnlyArray<Gamepad> all
		{
			get
			{
				return new ReadOnlyArray<Gamepad>(s_Gamepads, 0, s_GamepadCount);
			}
		}

		protected override void FinishSetup()
		{
			buttonWest = GetChildControl<ButtonControl>("buttonWest");
			buttonNorth = GetChildControl<ButtonControl>("buttonNorth");
			buttonSouth = GetChildControl<ButtonControl>("buttonSouth");
			buttonEast = GetChildControl<ButtonControl>("buttonEast");
			startButton = GetChildControl<ButtonControl>("start");
			selectButton = GetChildControl<ButtonControl>("select");
			leftStickButton = GetChildControl<ButtonControl>("leftStickPress");
			rightStickButton = GetChildControl<ButtonControl>("rightStickPress");
			dpad = GetChildControl<DpadControl>("dpad");
			leftShoulder = GetChildControl<ButtonControl>("leftShoulder");
			rightShoulder = GetChildControl<ButtonControl>("rightShoulder");
			leftStick = GetChildControl<StickControl>("leftStick");
			rightStick = GetChildControl<StickControl>("rightStick");
			leftTrigger = GetChildControl<ButtonControl>("leftTrigger");
			rightTrigger = GetChildControl<ButtonControl>("rightTrigger");
			base.FinishSetup();
		}

		public override void MakeCurrent()
		{
			base.MakeCurrent();
			current = this;
		}

		protected override void OnAdded()
		{
			ArrayHelpers.AppendWithCapacity(ref s_Gamepads, ref s_GamepadCount, this);
		}

		protected override void OnRemoved()
		{
			if (current == this)
			{
				current = null;
			}
			int num = s_Gamepads.IndexOfReference(this, s_GamepadCount);
			if (num != -1)
			{
				ArrayHelpers.EraseAtWithCapacity(s_Gamepads, ref s_GamepadCount, num);
			}
		}

		public virtual void PauseHaptics()
		{
			m_Rumble.PauseHaptics(this);
		}

		public virtual void ResumeHaptics()
		{
			m_Rumble.ResumeHaptics(this);
		}

		public virtual void ResetHaptics()
		{
			m_Rumble.ResetHaptics(this);
		}

		public virtual void SetMotorSpeeds(float lowFrequency, float highFrequency)
		{
			m_Rumble.SetMotorSpeeds(this, lowFrequency, highFrequency);
		}
	}
}
