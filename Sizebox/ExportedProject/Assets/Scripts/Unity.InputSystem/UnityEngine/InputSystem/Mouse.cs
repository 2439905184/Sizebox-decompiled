using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem
{
	[InputControlLayout(stateType = typeof(MouseState), isGenericTypeOfDevice = true)]
	[Preserve]
	public class Mouse : Pointer, IInputStateCallbackReceiver
	{
		internal static Mouse s_PlatformMouseDevice;

		public Vector2Control scroll { get; private set; }

		public ButtonControl leftButton { get; private set; }

		public ButtonControl middleButton { get; private set; }

		public ButtonControl rightButton { get; private set; }

		public ButtonControl backButton { get; private set; }

		public ButtonControl forwardButton { get; private set; }

		public IntegerControl clickCount { get; private set; }

		public new static Mouse current { get; private set; }

		public override void MakeCurrent()
		{
			base.MakeCurrent();
			current = this;
		}

		protected override void OnAdded()
		{
			base.OnAdded();
			if (base.native && s_PlatformMouseDevice == null)
			{
				s_PlatformMouseDevice = this;
			}
		}

		protected override void OnRemoved()
		{
			base.OnRemoved();
			if (current == this)
			{
				current = null;
			}
		}

		public void WarpCursorPosition(Vector2 position)
		{
			WarpMousePositionCommand command = WarpMousePositionCommand.Create(position);
			ExecuteCommand(ref command);
		}

		protected override void FinishSetup()
		{
			scroll = GetChildControl<Vector2Control>("scroll");
			leftButton = GetChildControl<ButtonControl>("leftButton");
			middleButton = GetChildControl<ButtonControl>("middleButton");
			rightButton = GetChildControl<ButtonControl>("rightButton");
			forwardButton = GetChildControl<ButtonControl>("forwardButton");
			backButton = GetChildControl<ButtonControl>("backButton");
			clickCount = GetChildControl<IntegerControl>("clickCount");
			base.FinishSetup();
		}

		protected new void OnNextUpdate()
		{
			base.OnNextUpdate();
			InputState.Change(scroll, Vector2.zero);
		}

		protected new unsafe void OnStateEvent(InputEventPtr eventPtr)
		{
			void* ptr = base.currentStatePtr;
			scroll.x.AccumulateValueInEvent(ptr, eventPtr);
			scroll.y.AccumulateValueInEvent(ptr, eventPtr);
			base.OnStateEvent(eventPtr);
		}

		void IInputStateCallbackReceiver.OnNextUpdate()
		{
			OnNextUpdate();
		}

		void IInputStateCallbackReceiver.OnStateEvent(InputEventPtr eventPtr)
		{
			OnStateEvent(eventPtr);
		}
	}
}
