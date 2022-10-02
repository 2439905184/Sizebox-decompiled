using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem
{
	[InputControlLayout(stateType = typeof(TouchscreenState), isGenericTypeOfDevice = true)]
	[Preserve]
	public class Touchscreen : Pointer, IInputStateCallbackReceiver
	{
		internal static float s_TapTime;

		internal static float s_TapDelayTime;

		internal static float s_TapRadiusSquared;

		public TouchControl primaryTouch { get; private set; }

		public ReadOnlyArray<TouchControl> touches { get; private set; }

		public new static Touchscreen current { get; internal set; }

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
			base.FinishSetup();
			primaryTouch = GetChildControl<TouchControl>("primaryTouch");
			int num = 0;
			foreach (InputControl child in base.children)
			{
				if (child is TouchControl)
				{
					num++;
				}
			}
			if (num >= 1)
			{
				num--;
			}
			TouchControl[] array = new TouchControl[num];
			int num2 = 0;
			foreach (InputControl child2 in base.children)
			{
				TouchControl touchControl;
				if (child2 != primaryTouch && (touchControl = child2 as TouchControl) != null)
				{
					array[num2++] = touchControl;
				}
			}
			touches = new ReadOnlyArray<TouchControl>(array);
		}

		protected new unsafe void OnNextUpdate()
		{
			void* ptr = base.currentStatePtr;
			TouchState* ptr2 = (TouchState*)((byte*)ptr + base.stateBlock.byteOffset + 56);
			int num = 0;
			while (num < touches.Count)
			{
				if (ptr2->delta != default(Vector2))
				{
					InputState.Change(touches[num].delta, Vector2.zero);
				}
				if (ptr2->tapCount > 0 && InputState.currentTime >= ptr2->startTime + (double)s_TapTime + (double)s_TapDelayTime)
				{
					InputState.Change((InputControl)touches[num].tapCount, (byte)0, InputUpdateType.None, default(InputEventPtr));
				}
				num++;
				ptr2++;
			}
			TouchState* ptr3 = (TouchState*)((byte*)ptr + base.stateBlock.byteOffset);
			if (ptr3->delta != default(Vector2))
			{
				InputState.Change(primaryTouch.delta, Vector2.zero);
			}
			if (ptr3->tapCount > 0 && InputState.currentTime >= ptr3->startTime + (double)s_TapTime + (double)s_TapDelayTime)
			{
				InputState.Change((InputControl)primaryTouch.tapCount, (byte)0, InputUpdateType.None, default(InputEventPtr));
			}
		}

		protected new unsafe void OnStateEvent(InputEventPtr eventPtr)
		{
			if (eventPtr.stateFormat != TouchState.Format)
			{
				InputState.Change(this, eventPtr);
			}
			else
			{
				if (eventPtr.IsA<DeltaStateEvent>())
				{
					return;
				}
				StateEvent* ptr = StateEvent.From(eventPtr);
				void* intPtr = base.currentStatePtr;
				TouchState* ptr2 = (TouchState*)((byte*)intPtr + touches[0].stateBlock.byteOffset);
				TouchState* ptr3 = (TouchState*)((byte*)intPtr + primaryTouch.stateBlock.byteOffset);
				int count = touches.Count;
				TouchState output;
				if (ptr->stateSizeInBytes == 56)
				{
					output = *(TouchState*)ptr->state;
				}
				else
				{
					output = default(TouchState);
					UnsafeUtility.MemCpy(UnsafeUtility.AddressOf(ref output), ptr->state, ptr->stateSizeInBytes);
				}
				output.tapCount = 0;
				output.isTap = false;
				if (output.phase != TouchPhase.Began)
				{
					int touchId = output.touchId;
					for (int i = 0; i < count; i++)
					{
						if (ptr2[i].touchId != touchId)
						{
							continue;
						}
						bool flag = (output.isPrimaryTouch = ptr2[i].isPrimaryTouch);
						if (output.delta == default(Vector2))
						{
							output.delta = output.position - ptr2[i].position;
						}
						output.delta += ptr2[i].delta;
						output.startTime = ptr2[i].startTime;
						output.startPosition = ptr2[i].startPosition;
						bool flag2 = output.isNoneEndedOrCanceled && eventPtr.time - output.startTime <= (double)s_TapTime && (output.position - output.startPosition).sqrMagnitude <= s_TapRadiusSquared;
						if (flag2)
						{
							output.tapCount = (byte)(ptr2[i].tapCount + 1);
						}
						else
						{
							output.tapCount = ptr2[i].tapCount;
						}
						if (flag)
						{
							if (output.isNoneEndedOrCanceled)
							{
								output.isPrimaryTouch = false;
								bool flag3 = false;
								for (int j = 0; j < count; j++)
								{
									if (j != i && ptr2[j].isInProgress)
									{
										flag3 = true;
										break;
									}
								}
								if (!flag3)
								{
									if (flag2)
									{
										TriggerTap(primaryTouch, ref output, eventPtr);
									}
									else
									{
										InputState.Change(primaryTouch, output, InputUpdateType.None, eventPtr);
									}
								}
								else
								{
									TouchState state = output;
									state.phase = TouchPhase.Moved;
									state.isOrphanedPrimaryTouch = true;
									InputState.Change(primaryTouch, state, InputUpdateType.None, eventPtr);
								}
							}
							else
							{
								InputState.Change(primaryTouch, output, InputUpdateType.None, eventPtr);
							}
						}
						else if (output.isNoneEndedOrCanceled && ptr3->isOrphanedPrimaryTouch)
						{
							bool flag4 = false;
							for (int k = 0; k < count; k++)
							{
								if (k != i && ptr2[k].isInProgress)
								{
									flag4 = true;
									break;
								}
							}
							if (!flag4)
							{
								ptr3->isOrphanedPrimaryTouch = false;
								InputState.Change((InputControl)primaryTouch.phase, (byte)3, InputUpdateType.None, default(InputEventPtr));
							}
						}
						if (flag2)
						{
							TriggerTap(touches[i], ref output, eventPtr);
						}
						else
						{
							InputState.Change(touches[i], output, InputUpdateType.None, eventPtr);
						}
						break;
					}
					return;
				}
				int num = 0;
				while (num < count)
				{
					if (ptr2->isNoneEndedOrCanceled)
					{
						output.delta = Vector2.zero;
						output.startTime = eventPtr.time;
						output.startPosition = output.position;
						output.isPrimaryTouch = false;
						output.isOrphanedPrimaryTouch = false;
						output.isTap = false;
						output.tapCount = ptr2->tapCount;
						if (ptr3->isNoneEndedOrCanceled)
						{
							output.isPrimaryTouch = true;
							InputState.Change(primaryTouch, output, InputUpdateType.None, eventPtr);
						}
						InputState.Change(touches[num], output, InputUpdateType.None, eventPtr);
						break;
					}
					num++;
					ptr2++;
				}
			}
		}

		void IInputStateCallbackReceiver.OnNextUpdate()
		{
			OnNextUpdate();
		}

		void IInputStateCallbackReceiver.OnStateEvent(InputEventPtr eventPtr)
		{
			OnStateEvent(eventPtr);
		}

		unsafe bool IInputStateCallbackReceiver.GetStateOffsetForEvent(InputControl control, InputEventPtr eventPtr, ref uint offset)
		{
			if (!eventPtr.IsA<StateEvent>() || StateEvent.From(eventPtr)->stateFormat != TouchState.Format)
			{
				return false;
			}
			TouchControl touchControl = control.FindInParentChain<TouchControl>();
			if (touchControl == null || touchControl.parent != this)
			{
				return false;
			}
			if (touchControl != primaryTouch)
			{
				return false;
			}
			offset = touchControl.stateBlock.byteOffset;
			return true;
		}

		private static void TriggerTap(TouchControl control, ref TouchState state, InputEventPtr eventPtr)
		{
			state.isTap = true;
			InputState.Change(control, state, InputUpdateType.None, eventPtr);
			state.isTap = false;
			InputState.Change(control, state, InputUpdateType.None, eventPtr);
		}
	}
}
