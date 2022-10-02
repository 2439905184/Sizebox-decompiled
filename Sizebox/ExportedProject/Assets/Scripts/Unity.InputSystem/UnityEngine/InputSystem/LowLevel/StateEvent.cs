using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 25)]
	public struct StateEvent : IInputEventTypeInfo
	{
		public const int Type = 1398030676;

		internal const int kStateDataSizeToSubtract = 1;

		[FieldOffset(0)]
		public InputEvent baseEvent;

		[FieldOffset(20)]
		public FourCC stateFormat;

		[FieldOffset(24)]
		internal unsafe fixed byte stateData[1];

		public uint stateSizeInBytes
		{
			get
			{
				return baseEvent.sizeInBytes - 24;
			}
		}

		public unsafe void* state
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
				return 1398030676;
			}
		}

		public unsafe InputEventPtr ToEventPtr()
		{
			fixed (StateEvent* eventPtr = &this)
			{
				return new InputEventPtr((InputEvent*)eventPtr);
			}
		}

		public static int GetEventSizeWithPayload<TState>() where TState : struct
		{
			return UnsafeUtility.SizeOf<TState>() + 20 + 4;
		}

		public unsafe static StateEvent* From(InputEventPtr ptr)
		{
			if (!ptr.valid)
			{
				throw new ArgumentNullException("ptr");
			}
			if (!ptr.IsA<StateEvent>())
			{
				throw new InvalidCastException(string.Format("Cannot cast event with type '{0}' into StateEvent", ptr.type));
			}
			return (StateEvent*)ptr.data;
		}

		public unsafe static NativeArray<byte> From(InputDevice device, out InputEventPtr eventPtr, Allocator allocator = Allocator.Temp)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			if (!device.added)
			{
				throw new ArgumentException(string.Format("Device '{0}' has not been added to system", device), "device");
			}
			FourCC format = device.m_StateBlock.format;
			uint alignedSizeInBytes = device.m_StateBlock.alignedSizeInBytes;
			uint byteOffset = device.m_StateBlock.byteOffset;
			byte* source = (byte*)device.currentStatePtr + (int)byteOffset;
			uint num = 24 + alignedSizeInBytes;
			NativeArray<byte> nativeArray = new NativeArray<byte>((int)num, allocator);
			StateEvent* unsafePtr = (StateEvent*)nativeArray.GetUnsafePtr();
			unsafePtr->baseEvent = new InputEvent(1398030676, (int)num, device.deviceId, InputRuntime.s_Instance.currentTime);
			unsafePtr->stateFormat = format;
			UnsafeUtility.MemCpy(unsafePtr->state, source, alignedSizeInBytes);
			eventPtr = unsafePtr->ToEventPtr();
			return nativeArray;
		}
	}
}
