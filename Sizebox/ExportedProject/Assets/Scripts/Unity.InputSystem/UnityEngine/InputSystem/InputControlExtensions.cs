using System;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
	public static class InputControlExtensions
	{
		public static TControl FindInParentChain<TControl>(this InputControl control) where TControl : InputControl
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			for (InputControl inputControl = control; inputControl != null; inputControl = inputControl.parent)
			{
				TControl result;
				if ((result = inputControl as TControl) != null)
				{
					return result;
				}
			}
			return null;
		}

		public static bool IsPressed(this InputControl control, float buttonPressPoint = 0f)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (Mathf.Approximately(0f, buttonPressPoint))
			{
				ButtonControl buttonControl;
				buttonPressPoint = (((buttonControl = control as ButtonControl) == null) ? ButtonControl.s_GlobalDefaultButtonPressPoint : buttonControl.pressPointOrDefault);
			}
			return control.IsActuated(buttonPressPoint);
		}

		public static bool IsActuated(this InputControl control, float threshold = 0f)
		{
			if (control.CheckStateIsAtDefault())
			{
				return false;
			}
			float num = control.EvaluateMagnitude();
			if (num < 0f)
			{
				return true;
			}
			if (Mathf.Approximately(threshold, 0f))
			{
				return num > 0f;
			}
			return num >= threshold;
		}

		public unsafe static object ReadValueAsObject(this InputControl control)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			return control.ReadValueFromStateAsObject(control.currentStatePtr);
		}

		public unsafe static void ReadValueIntoBuffer(this InputControl control, void* buffer, int bufferSize)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			control.ReadValueFromStateIntoBuffer(control.currentStatePtr, buffer, bufferSize);
		}

		public unsafe static object ReadDefaultValueAsObject(this InputControl control)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			return control.ReadValueFromStateAsObject(control.defaultStatePtr);
		}

		public static TValue ReadValueFromEvent<TValue>(this InputControl<TValue> control, InputEventPtr inputEvent) where TValue : struct
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			TValue value;
			if (!control.ReadValueFromEvent(inputEvent, out value))
			{
				return default(TValue);
			}
			return value;
		}

		public unsafe static bool ReadValueFromEvent<TValue>(this InputControl<TValue> control, InputEventPtr inputEvent, out TValue value) where TValue : struct
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			void* statePtrFromStateEvent = control.GetStatePtrFromStateEvent(inputEvent);
			if (statePtrFromStateEvent == null)
			{
				value = control.ReadDefaultValue();
				return false;
			}
			value = control.ReadValueFromState(statePtrFromStateEvent);
			return true;
		}

		public static TValue ReadUnprocessedValueFromEvent<TValue>(this InputControl<TValue> control, InputEventPtr eventPtr) where TValue : struct
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			TValue value = default(TValue);
			control.ReadUnprocessedValueFromEvent(eventPtr, out value);
			return value;
		}

		public unsafe static bool ReadUnprocessedValueFromEvent<TValue>(this InputControl<TValue> control, InputEventPtr inputEvent, out TValue value) where TValue : struct
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			void* statePtrFromStateEvent = control.GetStatePtrFromStateEvent(inputEvent);
			if (statePtrFromStateEvent == null)
			{
				value = control.ReadDefaultValue();
				return false;
			}
			value = control.ReadUnprocessedValueFromState(statePtrFromStateEvent);
			return true;
		}

		public unsafe static void WriteValueFromObjectIntoEvent(this InputControl control, InputEventPtr eventPtr, object value)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			void* statePtrFromStateEvent = control.GetStatePtrFromStateEvent(eventPtr);
			if (statePtrFromStateEvent != null)
			{
				control.WriteValueFromObjectIntoState(value, statePtrFromStateEvent);
			}
		}

		public unsafe static void WriteValueIntoState(this InputControl control, void* statePtr)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (statePtr == null)
			{
				throw new ArgumentNullException("statePtr");
			}
			int valueSizeInBytes = control.valueSizeInBytes;
			void* ptr = UnsafeUtility.Malloc(valueSizeInBytes, 8, Allocator.Temp);
			try
			{
				control.ReadValueFromStateIntoBuffer(control.currentStatePtr, ptr, valueSizeInBytes);
				control.WriteValueFromBufferIntoState(ptr, valueSizeInBytes, statePtr);
			}
			finally
			{
				UnsafeUtility.Free(ptr, Allocator.Temp);
			}
		}

		public unsafe static void WriteValueIntoState<TValue>(this InputControl control, TValue value, void* statePtr) where TValue : struct
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			InputControl<TValue> inputControl;
			if ((inputControl = control as InputControl<TValue>) == null)
			{
				throw new ArgumentException("Expecting control of type '" + typeof(TValue).Name + "' but got '" + control.GetType().Name + "'");
			}
			inputControl.WriteValueIntoState(value, statePtr);
		}

		public unsafe static void WriteValueIntoState<TValue>(this InputControl<TValue> control, TValue value, void* statePtr) where TValue : struct
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (statePtr == null)
			{
				throw new ArgumentNullException("statePtr");
			}
			void* bufferPtr = UnsafeUtility.AddressOf(ref value);
			int bufferSize = UnsafeUtility.SizeOf<TValue>();
			control.WriteValueFromBufferIntoState(bufferPtr, bufferSize, statePtr);
		}

		public unsafe static void WriteValueIntoState<TValue>(this InputControl<TValue> control, void* statePtr) where TValue : struct
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			control.WriteValueIntoState(control.ReadValue(), statePtr);
		}

		public unsafe static void WriteValueIntoState<TValue, TState>(this InputControl<TValue> control, TValue value, ref TState state) where TValue : struct where TState : struct, IInputStateTypeInfo
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			int num = UnsafeUtility.SizeOf<TState>();
			if (control.stateOffsetRelativeToDeviceRoot + control.m_StateBlock.alignedSizeInBytes >= num)
			{
				throw new ArgumentException(string.Format("Control {0} with offset {1} and size of {2} bits is out of bounds for state of type {3} with size {4}", control.path, control.stateOffsetRelativeToDeviceRoot, control.m_StateBlock.sizeInBits, typeof(TState).Name, num), "state");
			}
			byte* statePtr = (byte*)UnsafeUtility.AddressOf(ref state);
			control.WriteValueIntoState(value, statePtr);
		}

		public static void WriteValueIntoEvent<TValue>(this InputControl control, TValue value, InputEventPtr eventPtr) where TValue : struct
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (!eventPtr.valid)
			{
				throw new ArgumentNullException("eventPtr");
			}
			InputControl<TValue> control2;
			if ((control2 = control as InputControl<TValue>) == null)
			{
				throw new ArgumentException("Expecting control of type '" + typeof(TValue).Name + "' but got '" + control.GetType().Name + "'");
			}
			control2.WriteValueIntoEvent(value, eventPtr);
		}

		public unsafe static void WriteValueIntoEvent<TValue>(this InputControl<TValue> control, TValue value, InputEventPtr eventPtr) where TValue : struct
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (!eventPtr.valid)
			{
				throw new ArgumentNullException("eventPtr");
			}
			void* statePtrFromStateEvent = control.GetStatePtrFromStateEvent(eventPtr);
			if (statePtrFromStateEvent != null)
			{
				control.WriteValueIntoState(value, statePtrFromStateEvent);
			}
		}

		public unsafe static void CopyState(this InputDevice device, void* buffer, int bufferSizeInBytes)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			if (bufferSizeInBytes <= 0)
			{
				throw new ArgumentException("bufferSizeInBytes must be positive", "bufferSizeInBytes");
			}
			InputStateBlock stateBlock = device.m_StateBlock;
			long size = Math.Min(bufferSizeInBytes, stateBlock.alignedSizeInBytes);
			UnsafeUtility.MemCpy(buffer, (byte*)device.currentStatePtr + stateBlock.byteOffset, size);
		}

		public unsafe static void CopyState<TState>(this InputDevice device, out TState state) where TState : struct, IInputStateTypeInfo
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			state = default(TState);
			if (device.stateBlock.format != state.format)
			{
				throw new ArgumentException(string.Format("Struct '{0}' has state format '{1}' which doesn't match device '{2}' with state format '{3}'", typeof(TState).Name, state.format, device, device.stateBlock.format), "TState");
			}
			int bufferSizeInBytes = UnsafeUtility.SizeOf<TState>();
			void* buffer = UnsafeUtility.AddressOf(ref state);
			device.CopyState(buffer, bufferSizeInBytes);
		}

		public unsafe static bool CheckStateIsAtDefault(this InputControl control)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			return control.CheckStateIsAtDefault(control.currentStatePtr, null);
		}

		public unsafe static bool CheckStateIsAtDefault(this InputControl control, void* statePtr, void* maskPtr = null)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (statePtr == null)
			{
				throw new ArgumentNullException("statePtr");
			}
			return control.CompareState(statePtr, control.defaultStatePtr, maskPtr);
		}

		public unsafe static bool CheckStateIsAtDefaultIgnoringNoise(this InputControl control)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			return control.CheckStateIsAtDefaultIgnoringNoise(control.currentStatePtr);
		}

		public unsafe static bool CheckStateIsAtDefaultIgnoringNoise(this InputControl control, void* statePtr)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (statePtr == null)
			{
				throw new ArgumentNullException("statePtr");
			}
			return control.CheckStateIsAtDefault(statePtr, InputStateBuffers.s_NoiseMaskBuffer);
		}

		public unsafe static bool CompareStateIgnoringNoise(this InputControl control, void* statePtr)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (statePtr == null)
			{
				throw new ArgumentNullException("statePtr");
			}
			return control.CompareState(control.currentStatePtr, statePtr, control.noiseMaskPtr);
		}

		public unsafe static bool CompareState(this InputControl control, void* firstStatePtr, void* secondStatePtr, void* maskPtr = null)
		{
			byte* ptr = (byte*)firstStatePtr + (int)control.m_StateBlock.byteOffset;
			byte* ptr2 = (byte*)secondStatePtr + (int)control.m_StateBlock.byteOffset;
			byte* ptr3 = ((maskPtr != null) ? ((byte*)maskPtr + (int)control.m_StateBlock.byteOffset) : null);
			if (control.m_StateBlock.sizeInBits == 1)
			{
				if (ptr3 != null && MemoryHelpers.ReadSingleBit(ptr3, control.m_StateBlock.bitOffset))
				{
					return true;
				}
				return MemoryHelpers.ReadSingleBit(ptr2, control.m_StateBlock.bitOffset) == MemoryHelpers.ReadSingleBit(ptr, control.m_StateBlock.bitOffset);
			}
			return MemoryHelpers.MemCmpBitRegion(ptr, ptr2, control.m_StateBlock.bitOffset, control.m_StateBlock.sizeInBits, ptr3);
		}

		public unsafe static bool CompareState(this InputControl control, void* statePtr, void* maskPtr = null)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (statePtr == null)
			{
				throw new ArgumentNullException("statePtr");
			}
			return control.CompareState(control.currentStatePtr, statePtr, maskPtr);
		}

		public unsafe static bool HasValueChangeInState(this InputControl control, void* statePtr)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (statePtr == null)
			{
				throw new ArgumentNullException("statePtr");
			}
			return control.CompareValue(control.currentStatePtr, statePtr);
		}

		public unsafe static bool HasValueChangeInEvent(this InputControl control, InputEventPtr eventPtr)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (!eventPtr.valid)
			{
				throw new ArgumentNullException("eventPtr");
			}
			return control.CompareValue(control.currentStatePtr, control.GetStatePtrFromStateEvent(eventPtr));
		}

		public unsafe static void* GetStatePtrFromStateEvent(this InputControl control, InputEventPtr eventPtr)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (!eventPtr.valid)
			{
				throw new ArgumentNullException("eventPtr");
			}
			FourCC stateFormat;
			uint num;
			void* ptr;
			uint offset;
			if (eventPtr.IsA<DeltaStateEvent>())
			{
				DeltaStateEvent* intPtr = DeltaStateEvent.From(eventPtr);
				offset = intPtr->stateOffset;
				stateFormat = intPtr->stateFormat;
				num = intPtr->deltaStateSizeInBytes;
				ptr = intPtr->deltaState;
			}
			else
			{
				if (!eventPtr.IsA<StateEvent>())
				{
					throw new ArgumentException("Event must be a state or delta state event", "eventPtr");
				}
				StateEvent* intPtr2 = StateEvent.From(eventPtr);
				offset = 0u;
				stateFormat = intPtr2->stateFormat;
				num = intPtr2->stateSizeInBytes;
				ptr = intPtr2->state;
			}
			InputDevice device = control.device;
			if (stateFormat != device.m_StateBlock.format && (!device.hasStateCallbacks || !((IInputStateCallbackReceiver)device).GetStateOffsetForEvent(control, eventPtr, ref offset)))
			{
				return null;
			}
			offset += device.m_StateBlock.byteOffset;
			long num2 = (int)control.m_StateBlock.byteOffset - offset;
			if (num2 < 0 || num2 + control.m_StateBlock.alignedSizeInBytes > num)
			{
				return null;
			}
			return (byte*)ptr - (int)offset;
		}

		public static void QueueValueChange<TValue>(this InputControl<TValue> control, TValue value, double time = -1.0) where TValue : struct
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			InputEventPtr eventPtr;
			using (StateEvent.From(control.device, out eventPtr))
			{
				if (time >= 0.0)
				{
					eventPtr.time = time;
				}
				control.WriteValueIntoEvent(value, eventPtr);
				InputSystem.QueueEvent(eventPtr);
			}
		}

		public unsafe static void AccumulateValueInEvent(this InputControl<float> control, void* currentStatePtr, InputEventPtr newState)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			float value;
			if (control.ReadUnprocessedValueFromEvent(newState, out value))
			{
				float num = control.ReadUnprocessedValueFromState(currentStatePtr);
				control.WriteValueIntoEvent(num + value, newState);
			}
		}

		public static void FindControlsRecursive<TControl>(this InputControl parent, IList<TControl> controls, Func<TControl, bool> predicate) where TControl : InputControl
		{
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}
			if (controls == null)
			{
				throw new ArgumentNullException("controls");
			}
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			TControl val;
			if ((val = parent as TControl) != null && predicate(val))
			{
				controls.Add(val);
			}
			int count = parent.children.Count;
			for (int i = 0; i < count; i++)
			{
				parent.children[i].FindControlsRecursive(controls, predicate);
			}
		}

		internal static string BuildPath(this InputControl control, string deviceLayout, StringBuilder builder = null)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (string.IsNullOrEmpty(deviceLayout))
			{
				throw new ArgumentNullException("deviceLayout");
			}
			if (builder == null)
			{
				builder = new StringBuilder();
			}
			InputDevice device = control.device;
			builder.Append('<');
			builder.Append(deviceLayout);
			builder.Append('>');
			ReadOnlyArray<InternedString> usages = device.usages;
			for (int i = 0; i < usages.Count; i++)
			{
				builder.Append('{');
				builder.Append(usages[i]);
				builder.Append('}');
			}
			builder.Append('/');
			string path = device.path;
			string path2 = control.path;
			builder.Append(path2, path.Length + 1, path2.Length - path.Length - 1);
			return builder.ToString();
		}
	}
}
