using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 29)]
	public struct DeltaStateEvent : IInputEventTypeInfo
	{
		public const int Type = 1145852993;

		[FieldOffset(0)]
		public InputEvent baseEvent;

		[FieldOffset(20)]
		public FourCC stateFormat;

		[FieldOffset(24)]
		public uint stateOffset;

		[FieldOffset(28)]
		internal unsafe fixed byte stateData[1];

		public uint deltaStateSizeInBytes
		{
			get
			{
				return baseEvent.sizeInBytes - 28;
			}
		}

		public unsafe void* deltaState
		{
			get
			{
				fixed (byte* result = stateData)
				{
					return result;
				}
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return 1145852993;
			}
		}

		public unsafe InputEventPtr ToEventPtr()
		{
			fixed (DeltaStateEvent* eventPtr = &this)
			{
				return new InputEventPtr((InputEvent*)eventPtr);
			}
		}

		public unsafe static DeltaStateEvent* From(InputEventPtr ptr)
		{
			if (!ptr.valid)
			{
				throw new ArgumentNullException("ptr");
			}
			if (!ptr.IsA<DeltaStateEvent>())
			{
				throw new InvalidCastException(string.Format("Cannot cast event with type '{0}' into DeltaStateEvent", ptr.type));
			}
			return (DeltaStateEvent*)ptr.data;
		}

		public unsafe static NativeArray<byte> From(InputControl control, out InputEventPtr eventPtr, Allocator allocator = Allocator.Temp)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			InputDevice device = control.device;
			if (!device.added)
			{
				throw new ArgumentException(string.Format("Device for control '{0}' has not been added to system", control), "control");
			}
			ref InputStateBlock stateBlock = ref device.m_StateBlock;
			ref InputStateBlock stateBlock2 = ref control.m_StateBlock;
			FourCC format = stateBlock.format;
			uint alignedSizeInBytes = stateBlock2.alignedSizeInBytes;
			alignedSizeInBytes += stateBlock2.bitOffset / 8u;
			uint byteOffset = stateBlock2.byteOffset;
			byte* source = (byte*)control.currentStatePtr + (int)byteOffset;
			uint num = 28 + alignedSizeInBytes;
			NativeArray<byte> nativeArray = new NativeArray<byte>((int)num, allocator);
			DeltaStateEvent* unsafePtr = (DeltaStateEvent*)nativeArray.GetUnsafePtr();
			unsafePtr->baseEvent = new InputEvent(1145852993, (int)num, device.deviceId, InputRuntime.s_Instance.currentTime);
			unsafePtr->stateFormat = format;
			unsafePtr->stateOffset = stateBlock2.byteOffset - stateBlock.byteOffset;
			UnsafeUtility.MemCpy(unsafePtr->deltaState, source, alignedSizeInBytes);
			eventPtr = unsafePtr->ToEventPtr();
			return nativeArray;
		}
	}
}
