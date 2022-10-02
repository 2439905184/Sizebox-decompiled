using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem
{
	[InputControlLayout(stateType = typeof(PointerState), isGenericTypeOfDevice = true)]
	[Preserve]
	public class Pointer : InputDevice, IInputStateCallbackReceiver
	{
		public Vector2Control position { get; private set; }

		public Vector2Control delta { get; private set; }

		public Vector2Control radius { get; private set; }

		public AxisControl pressure { get; private set; }

		public ButtonControl press { get; private set; }

		public static Pointer current { get; internal set; }

		public override void MakeCurrent()
		{
			base.MakeCurrent();
			current = this;
		}

		protected override void OnRemoved()
		{
			base.OnRemoved();
			if (current == this)
			{
				current = null;
			}
		}

		protected override void FinishSetup()
		{
			position = GetChildControl<Vector2Control>("position");
			delta = GetChildControl<Vector2Control>("delta");
			radius = GetChildControl<Vector2Control>("radius");
			pressure = GetChildControl<AxisControl>("pressure");
			press = GetChildControl<ButtonControl>("press");
			base.FinishSetup();
		}

		protected void OnNextUpdate()
		{
			InputState.Change(delta, Vector2.zero);
		}

		protected unsafe void OnStateEvent(InputEventPtr eventPtr)
		{
			void* ptr = base.currentStatePtr;
			delta.x.AccumulateValueInEvent(ptr, eventPtr);
			delta.y.AccumulateValueInEvent(ptr, eventPtr);
			InputState.Change(this, eventPtr);
		}

		void IInputStateCallbackReceiver.OnNextUpdate()
		{
			OnNextUpdate();
		}

		void IInputStateCallbackReceiver.OnStateEvent(InputEventPtr eventPtr)
		{
			OnStateEvent(eventPtr);
		}

		bool IInputStateCallbackReceiver.GetStateOffsetForEvent(InputControl control, InputEventPtr eventPtr, ref uint offset)
		{
			return false;
		}
	}
}
