using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	public struct InputEventBuffer : IEnumerable<InputEventPtr>, IEnumerable, IDisposable, ICloneable
	{
		private struct Enumerator : IEnumerator<InputEventPtr>, IEnumerator, IDisposable
		{
			private unsafe readonly InputEvent* m_Buffer;

			private readonly int m_EventCount;

			private unsafe InputEvent* m_CurrentEvent;

			private int m_CurrentIndex;

			public unsafe InputEventPtr Current
			{
				get
				{
					return m_CurrentEvent;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return Current;
				}
			}

			public unsafe Enumerator(InputEventBuffer buffer)
			{
				m_Buffer = buffer.bufferPtr;
				m_EventCount = buffer.m_EventCount;
				m_CurrentEvent = null;
				m_CurrentIndex = 0;
			}

			public unsafe bool MoveNext()
			{
				if (m_CurrentIndex == m_EventCount)
				{
					return false;
				}
				if (m_CurrentEvent == null)
				{
					m_CurrentEvent = m_Buffer;
					return m_CurrentEvent != null;
				}
				m_CurrentIndex++;
				if (m_CurrentIndex == m_EventCount)
				{
					return false;
				}
				m_CurrentEvent = InputEvent.GetNextInMemory(m_CurrentEvent);
				return true;
			}

			public unsafe void Reset()
			{
				m_CurrentEvent = null;
				m_CurrentIndex = 0;
			}

			public void Dispose()
			{
			}
		}

		public const long BufferSizeUnknown = -1L;

		private NativeArray<byte> m_Buffer;

		private long m_SizeInBytes;

		private int m_EventCount;

		private bool m_WeOwnTheBuffer;

		public int eventCount
		{
			get
			{
				return m_EventCount;
			}
		}

		public long sizeInBytes
		{
			get
			{
				return m_SizeInBytes;
			}
		}

		public long capacityInBytes
		{
			get
			{
				if (!m_Buffer.IsCreated)
				{
					return 0L;
				}
				return m_Buffer.Length;
			}
		}

		public NativeArray<byte> data
		{
			get
			{
				return m_Buffer;
			}
		}

		public unsafe InputEventPtr bufferPtr
		{
			get
			{
				return (InputEvent*)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(m_Buffer);
			}
		}

		public unsafe InputEventBuffer(InputEvent* eventPtr, int eventCount, int sizeInBytes = -1, int capacityInBytes = -1)
		{
			this = default(InputEventBuffer);
			if (eventPtr == null && eventCount != 0)
			{
				throw new ArgumentException("eventPtr is NULL but eventCount is != 0", "eventCount");
			}
			if (capacityInBytes != 0 && capacityInBytes < sizeInBytes)
			{
				throw new ArgumentException(string.Format("capacity({0}) cannot be smaller than size({1})", capacityInBytes, sizeInBytes), "capacityInBytes");
			}
			if (eventPtr != null)
			{
				if (capacityInBytes < 0)
				{
					capacityInBytes = sizeInBytes;
				}
				m_Buffer = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>(eventPtr, (capacityInBytes > 0) ? capacityInBytes : 0, Allocator.None);
				m_SizeInBytes = ((sizeInBytes >= 0) ? sizeInBytes : (-1));
				m_EventCount = eventCount;
				m_WeOwnTheBuffer = false;
			}
		}

		public InputEventBuffer(NativeArray<byte> buffer, int eventCount, int sizeInBytes = -1)
		{
			if (eventCount > 0 && !buffer.IsCreated)
			{
				throw new ArgumentException("buffer has no data but eventCount is > 0", "eventCount");
			}
			if (sizeInBytes > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("sizeInBytes");
			}
			m_Buffer = buffer;
			m_WeOwnTheBuffer = false;
			m_SizeInBytes = ((sizeInBytes >= 0) ? sizeInBytes : buffer.Length);
			m_EventCount = eventCount;
		}

		public unsafe void AppendEvent(InputEvent* eventPtr, int capacityIncrementInBytes = 2048)
		{
			if (eventPtr == null)
			{
				throw new ArgumentNullException("eventPtr");
			}
			uint num = eventPtr->sizeInBytes;
			UnsafeUtility.MemCpy(AllocateEvent((int)num, capacityIncrementInBytes), eventPtr, num);
		}

		public unsafe InputEvent* AllocateEvent(int sizeInBytes, int capacityIncrementInBytes = 2048)
		{
			if (sizeInBytes < 20)
			{
				throw new ArgumentException(string.Format("sizeInBytes must be >= sizeof(InputEvent) == {0} (was {1})", 20, sizeInBytes), "sizeInBytes");
			}
			int num = sizeInBytes.AlignToMultipleOf(4);
			long num2 = capacityInBytes;
			if (num2 < num)
			{
				long num3 = Math.Max(num2 + capacityIncrementInBytes, num2 + num);
				long num4 = this.sizeInBytes + num3;
				if (num4 > int.MaxValue)
				{
					throw new NotImplementedException("NativeArray long support");
				}
				NativeArray<byte> nativeArray = new NativeArray<byte>((int)num4, Allocator.Persistent);
				if (m_Buffer.IsCreated)
				{
					UnsafeUtility.MemCpy(nativeArray.GetUnsafePtr(), m_Buffer.GetUnsafeReadOnlyPtr(), this.sizeInBytes);
				}
				else
				{
					m_SizeInBytes = 0L;
				}
				if (m_WeOwnTheBuffer)
				{
					m_Buffer.Dispose();
				}
				m_Buffer = nativeArray;
				m_WeOwnTheBuffer = true;
			}
			InputEvent* ptr = (InputEvent*)((byte*)m_Buffer.GetUnsafePtr() + m_SizeInBytes);
			ptr->sizeInBytes = (uint)sizeInBytes;
			m_SizeInBytes += num;
			m_EventCount++;
			return ptr;
		}

		public unsafe bool Contains(InputEvent* eventPtr)
		{
			if (eventPtr == null)
			{
				return false;
			}
			if (sizeInBytes == 0L)
			{
				return false;
			}
			void* unsafeBufferPointerWithoutChecks = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(data);
			if (eventPtr < unsafeBufferPointerWithoutChecks)
			{
				return false;
			}
			if (sizeInBytes != -1 && eventPtr >= (byte*)unsafeBufferPointerWithoutChecks + sizeInBytes)
			{
				return false;
			}
			return true;
		}

		public void Reset()
		{
			m_EventCount = 0;
			if (m_SizeInBytes != -1)
			{
				m_SizeInBytes = 0L;
			}
		}

		internal unsafe void AdvanceToNextEvent(ref InputEvent* currentReadPos, ref InputEvent* currentWritePos, ref int numEventsRetainedInBuffer, ref int numRemainingEvents, bool leaveEventInBuffer)
		{
			InputEvent* ptr = currentReadPos;
			if (numRemainingEvents > 1)
			{
				ptr = InputEvent.GetNextInMemoryChecked(currentReadPos, ref this);
			}
			if (leaveEventInBuffer)
			{
				uint num = currentReadPos->sizeInBytes;
				if (currentReadPos != currentWritePos)
				{
					UnsafeUtility.MemMove(currentWritePos, currentReadPos, num);
				}
				currentWritePos = (InputEvent*)((byte*)currentWritePos + num.AlignToMultipleOf(4u));
				numEventsRetainedInBuffer++;
			}
			currentReadPos = ptr;
			numRemainingEvents--;
		}

		public IEnumerator<InputEventPtr> GetEnumerator()
		{
			return new Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Dispose()
		{
			if (m_WeOwnTheBuffer)
			{
				m_Buffer.Dispose();
				m_WeOwnTheBuffer = false;
				m_SizeInBytes = 0L;
				m_EventCount = 0;
			}
		}

		public InputEventBuffer Clone()
		{
			InputEventBuffer result = default(InputEventBuffer);
			if (m_Buffer.IsCreated)
			{
				result.m_Buffer = new NativeArray<byte>(m_Buffer.Length, Allocator.Persistent);
				result.m_Buffer.CopyFrom(m_Buffer);
				result.m_WeOwnTheBuffer = true;
			}
			result.m_SizeInBytes = m_SizeInBytes;
			result.m_EventCount = m_EventCount;
			return result;
		}

		object ICloneable.Clone()
		{
			return Clone();
		}
	}
}
