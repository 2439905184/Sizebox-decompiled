using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	public class InputStateHistory : IDisposable, IEnumerable<InputStateHistory.Record>, IEnumerable, IInputStateChangeMonitor
	{
		private struct Enumerator : IEnumerator<Record>, IEnumerator, IDisposable
		{
			private readonly InputStateHistory m_History;

			private int m_Index;

			public Record Current
			{
				get
				{
					return m_History[m_Index];
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return Current;
				}
			}

			public Enumerator(InputStateHistory history)
			{
				m_History = history;
				m_Index = -1;
			}

			public bool MoveNext()
			{
				if (m_Index + 1 >= m_History.Count)
				{
					return false;
				}
				m_Index++;
				return true;
			}

			public void Reset()
			{
				m_Index = -1;
			}

			public void Dispose()
			{
			}
		}

		[StructLayout(LayoutKind.Explicit)]
		protected internal struct RecordHeader
		{
			[FieldOffset(0)]
			public double time;

			[FieldOffset(8)]
			public uint version;

			[FieldOffset(12)]
			public int controlIndex;

			[FieldOffset(12)]
			private unsafe fixed byte m_StateWithoutControlIndex[1];

			[FieldOffset(16)]
			private unsafe fixed byte m_StateWithControlIndex[1];

			public const int kSizeWithControlIndex = 16;

			public const int kSizeWithoutControlIndex = 12;

			public unsafe byte* statePtrWithControlIndex
			{
				get
				{
					fixed (byte* result = m_StateWithControlIndex)
					{
						return result;
					}
				}
			}

			public unsafe byte* statePtrWithoutControlIndex
			{
				get
				{
					fixed (byte* result = m_StateWithoutControlIndex)
					{
						return result;
					}
				}
			}
		}

		public struct Record : IEquatable<Record>
		{
			private readonly InputStateHistory m_Owner;

			private readonly int m_IndexPlusOne;

			private uint m_Version;

			internal unsafe RecordHeader* header
			{
				get
				{
					return m_Owner.GetRecord(recordIndex);
				}
			}

			internal int recordIndex
			{
				get
				{
					return m_IndexPlusOne - 1;
				}
			}

			internal uint version
			{
				get
				{
					return m_Version;
				}
			}

			public unsafe bool valid
			{
				get
				{
					if (m_Owner != null && m_IndexPlusOne != 0)
					{
						return header->version == m_Version;
					}
					return false;
				}
			}

			public InputStateHistory owner
			{
				get
				{
					return m_Owner;
				}
			}

			public int index
			{
				get
				{
					CheckValid();
					return m_Owner.RecordIndexToUserIndex(recordIndex);
				}
			}

			public unsafe double time
			{
				get
				{
					CheckValid();
					return header->time;
				}
			}

			public unsafe InputControl control
			{
				get
				{
					CheckValid();
					ReadOnlyArray<InputControl> controls = m_Owner.controls;
					if (controls.Count == 1 && !m_Owner.m_AddNewControls)
					{
						return controls[0];
					}
					return controls[header->controlIndex];
				}
			}

			public unsafe Record next
			{
				get
				{
					CheckValid();
					int num = m_Owner.RecordIndexToUserIndex(recordIndex);
					if (num + 1 >= m_Owner.Count)
					{
						return default(Record);
					}
					int num2 = m_Owner.UserIndexToRecordIndex(num + 1);
					return new Record(m_Owner, num2, m_Owner.GetRecord(num2));
				}
			}

			public unsafe Record previous
			{
				get
				{
					CheckValid();
					int num = m_Owner.RecordIndexToUserIndex(recordIndex);
					if (num - 1 < 0)
					{
						return default(Record);
					}
					int num2 = m_Owner.UserIndexToRecordIndex(num - 1);
					return new Record(m_Owner, num2, m_Owner.GetRecord(num2));
				}
			}

			internal unsafe Record(InputStateHistory owner, int index, RecordHeader* header)
			{
				m_Owner = owner;
				m_IndexPlusOne = index + 1;
				m_Version = header->version;
			}

			public unsafe TValue ReadValue<TValue>() where TValue : struct
			{
				CheckValid();
				return m_Owner.ReadValue<TValue>(header);
			}

			public unsafe object ReadValueAsObject()
			{
				CheckValid();
				return m_Owner.ReadValueAsObject(header);
			}

			public unsafe void* GetUnsafeMemoryPtr()
			{
				CheckValid();
				if (m_Owner.controls.Count == 1 && !m_Owner.m_AddNewControls)
				{
					return header->statePtrWithoutControlIndex;
				}
				return header->statePtrWithControlIndex;
			}

			public unsafe void* GetUnsafeExtraMemoryPtr()
			{
				CheckValid();
				if (m_Owner.extraMemoryPerRecord == 0)
				{
					throw new InvalidOperationException("No extra memory has been set up for history records; set extraMemoryPerRecord");
				}
				return (byte*)header + m_Owner.bytesPerRecord - m_Owner.extraMemoryPerRecord;
			}

			public unsafe void CopyFrom(Record record)
			{
				if (!record.valid)
				{
					throw new ArgumentException("Given history record is not valid", "record");
				}
				CheckValid();
				InputControl value = record.control;
				int num = m_Owner.controls.IndexOfReference(value);
				if (num == -1)
				{
					if (!m_Owner.m_AddNewControls)
					{
						throw new InvalidOperationException(string.Format("Control '{0}' is not tracked by target history", record.control));
					}
					num = ArrayHelpers.AppendWithCapacity(ref m_Owner.m_Controls, ref m_Owner.m_ControlCount, value);
				}
				int stateSizeInBytes = m_Owner.m_StateSizeInBytes;
				if (stateSizeInBytes != record.m_Owner.m_StateSizeInBytes)
				{
					throw new InvalidOperationException(string.Format("Cannot copy record from owner with state size '{0}' to owner with state size '{1}'", record.m_Owner.m_StateSizeInBytes, stateSizeInBytes));
				}
				RecordHeader* ptr = header;
				RecordHeader* ptr2 = record.header;
				UnsafeUtility.MemCpy(ptr, ptr2, 12L);
				ptr->version = ++m_Owner.m_CurrentVersion;
				m_Version = ptr->version;
				byte* destination = ptr->statePtrWithoutControlIndex;
				if (m_Owner.controls.Count > 1 || m_Owner.m_AddNewControls)
				{
					ptr->controlIndex = num;
					destination = ptr->statePtrWithControlIndex;
				}
				byte* source = ((record.m_Owner.m_ControlCount > 1 || record.m_Owner.m_AddNewControls) ? ptr2->statePtrWithControlIndex : ptr2->statePtrWithoutControlIndex);
				UnsafeUtility.MemCpy(destination, source, stateSizeInBytes);
				int extraMemoryPerRecord = m_Owner.m_ExtraMemoryPerRecord;
				if (extraMemoryPerRecord > 0 && extraMemoryPerRecord == record.m_Owner.m_ExtraMemoryPerRecord)
				{
					UnsafeUtility.MemCpy(GetUnsafeExtraMemoryPtr(), record.GetUnsafeExtraMemoryPtr(), extraMemoryPerRecord);
				}
				Action<Record> onRecordAdded = m_Owner.onRecordAdded;
				if (onRecordAdded != null)
				{
					onRecordAdded(this);
				}
			}

			internal unsafe void CheckValid()
			{
				if (m_Owner == null || m_IndexPlusOne == 0)
				{
					throw new InvalidOperationException("Value not initialized");
				}
				if (header->version != m_Version)
				{
					throw new InvalidOperationException("Record is no longer valid");
				}
			}

			public bool Equals(Record other)
			{
				if (m_Owner == other.m_Owner && m_IndexPlusOne == other.m_IndexPlusOne)
				{
					return m_Version == other.m_Version;
				}
				return false;
			}

			public override bool Equals(object obj)
			{
				object obj2;
				if ((obj2 = obj) is Record)
				{
					Record other = (Record)obj2;
					return Equals(other);
				}
				return false;
			}

			public override int GetHashCode()
			{
				return (((((m_Owner != null) ? m_Owner.GetHashCode() : 0) * 397) ^ m_IndexPlusOne) * 397) ^ (int)m_Version;
			}

			public override string ToString()
			{
				if (!valid)
				{
					return "<Invalid>";
				}
				return string.Format("{{ control={0} value={1} time={2} }}", control, ReadValueAsObject(), time);
			}
		}

		private const int kDefaultHistorySize = 128;

		private InputControl[] m_Controls;

		private int m_ControlCount;

		private NativeArray<byte> m_RecordBuffer;

		private int m_StateSizeInBytes;

		private int m_RecordCount;

		private int m_HistoryDepth = 128;

		private int m_ExtraMemoryPerRecord;

		internal int m_HeadIndex;

		internal uint m_CurrentVersion;

		private InputUpdateType? m_UpdateMask;

		internal readonly bool m_AddNewControls;

		public int Count
		{
			get
			{
				return m_RecordCount;
			}
		}

		public uint version
		{
			get
			{
				return m_CurrentVersion;
			}
		}

		public int historyDepth
		{
			get
			{
				return m_HistoryDepth;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("History depth cannot be negative", "value");
				}
				if (m_RecordBuffer.IsCreated)
				{
					throw new NotImplementedException();
				}
				m_HistoryDepth = value;
			}
		}

		public int extraMemoryPerRecord
		{
			get
			{
				return m_ExtraMemoryPerRecord;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("Memory size cannot be negative", "value");
				}
				if (m_RecordBuffer.IsCreated)
				{
					throw new NotImplementedException();
				}
				m_ExtraMemoryPerRecord = value;
			}
		}

		public InputUpdateType updateMask
		{
			get
			{
				return m_UpdateMask ?? (InputSystem.s_Manager.updateMask & ~InputUpdateType.Editor);
			}
			set
			{
				if (value == InputUpdateType.None)
				{
					throw new ArgumentException("'InputUpdateType.None' is not a valid update mask", "value");
				}
				m_UpdateMask = value;
			}
		}

		public ReadOnlyArray<InputControl> controls
		{
			get
			{
				return new ReadOnlyArray<InputControl>(m_Controls, 0, m_ControlCount);
			}
		}

		public unsafe Record this[int index]
		{
			get
			{
				if (index < 0 || index >= m_RecordCount)
				{
					throw new ArgumentOutOfRangeException(string.Format("Index {0} is out of range for history with {1} entries", index, m_RecordCount), "index");
				}
				int index2 = UserIndexToRecordIndex(index);
				return new Record(this, index2, GetRecord(index2));
			}
			set
			{
				if (index < 0 || index >= m_RecordCount)
				{
					throw new ArgumentOutOfRangeException(string.Format("Index {0} is out of range for history with {1} entries", index, m_RecordCount), "index");
				}
				int index2 = UserIndexToRecordIndex(index);
				new Record(this, index2, GetRecord(index2)).CopyFrom(value);
			}
		}

		public Action<Record> onRecordAdded { get; set; }

		public Func<InputControl, double, InputEventPtr, bool> onShouldRecordStateChange { get; set; }

		internal int bytesPerRecord
		{
			get
			{
				return m_StateSizeInBytes + m_ExtraMemoryPerRecord + ((m_ControlCount == 1 && !m_AddNewControls) ? 12 : 16);
			}
		}

		public InputStateHistory(int maxStateSizeInBytes)
		{
			if (maxStateSizeInBytes <= 0)
			{
				throw new ArgumentException("State size must be >= 0", "maxStateSizeInBytes");
			}
			m_AddNewControls = true;
			m_StateSizeInBytes = maxStateSizeInBytes.AlignToMultipleOf(4);
		}

		public InputStateHistory(string path)
		{
			using (InputControlList<InputControl> inputControlList = InputSystem.FindControls(path))
			{
				m_Controls = inputControlList.ToArray();
				m_ControlCount = m_Controls.Length;
			}
		}

		public InputStateHistory(InputControl control)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			m_Controls = new InputControl[1] { control };
			m_ControlCount = 1;
		}

		public InputStateHistory(IEnumerable<InputControl> controls)
		{
			if (controls != null)
			{
				m_Controls = controls.ToArray();
				m_ControlCount = m_Controls.Length;
			}
		}

		~InputStateHistory()
		{
			Dispose();
		}

		public void Clear()
		{
			m_HeadIndex = 0;
			m_RecordCount = 0;
			m_CurrentVersion++;
		}

		public unsafe Record AddRecord(Record record)
		{
			int index;
			RecordHeader* header = AllocateRecord(out index);
			Record result = new Record(this, index, header);
			result.CopyFrom(record);
			return result;
		}

		public void StartRecording()
		{
			foreach (InputControl control in controls)
			{
				InputState.AddChangeMonitor(control, this, -1L);
			}
		}

		public void StopRecording()
		{
			foreach (InputControl control in controls)
			{
				InputState.RemoveChangeMonitor(control, this, -1L);
			}
		}

		public unsafe Record RecordStateChange(InputControl control, InputEventPtr eventPtr)
		{
			if (eventPtr.IsA<DeltaStateEvent>())
			{
				throw new NotImplementedException();
			}
			if (!eventPtr.IsA<StateEvent>())
			{
				throw new ArgumentException(string.Format("Event must be a state event but is '{0}' instead", eventPtr), "eventPtr");
			}
			byte* statePtr = (byte*)StateEvent.From(eventPtr)->state - control.device.stateBlock.byteOffset;
			return RecordStateChange(control, statePtr, eventPtr.time);
		}

		public unsafe Record RecordStateChange(InputControl control, void* statePtr, double time)
		{
			int num = m_Controls.IndexOfReference(control, m_ControlCount);
			if (num == -1)
			{
				if (!m_AddNewControls)
				{
					throw new ArgumentException(string.Format("Control '{0}' is not part of InputStateHistory", control), "control");
				}
				if (control.stateBlock.alignedSizeInBytes > m_StateSizeInBytes)
				{
					throw new InvalidOperationException(string.Format("Cannot add control '{0}' with state larger than {1} bytes", control, m_StateSizeInBytes));
				}
				num = ArrayHelpers.AppendWithCapacity(ref m_Controls, ref m_ControlCount, control);
			}
			int index;
			RecordHeader* ptr = AllocateRecord(out index);
			ptr->time = time;
			ptr->version = ++m_CurrentVersion;
			byte* destination = ptr->statePtrWithoutControlIndex;
			if (m_ControlCount > 1 || m_AddNewControls)
			{
				ptr->controlIndex = num;
				destination = ptr->statePtrWithControlIndex;
			}
			uint alignedSizeInBytes = control.stateBlock.alignedSizeInBytes;
			uint byteOffset = control.stateBlock.byteOffset;
			UnsafeUtility.MemCpy(destination, (byte*)statePtr + byteOffset, alignedSizeInBytes);
			Record record = new Record(this, index, ptr);
			Action<Record> action = onRecordAdded;
			if (action != null)
			{
				action(record);
			}
			return record;
		}

		public IEnumerator<Record> GetEnumerator()
		{
			return new Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Dispose()
		{
			StopRecording();
			Destroy();
			GC.SuppressFinalize(this);
		}

		protected void Destroy()
		{
			if (m_RecordBuffer.IsCreated)
			{
				m_RecordBuffer.Dispose();
				m_RecordBuffer = default(NativeArray<byte>);
			}
		}

		private void Allocate()
		{
			if (!m_AddNewControls)
			{
				m_StateSizeInBytes = 0;
				foreach (InputControl control in controls)
				{
					m_StateSizeInBytes = (int)Math.Max((uint)m_StateSizeInBytes, control.stateBlock.alignedSizeInBytes);
				}
			}
			int length = bytesPerRecord * m_HistoryDepth;
			m_RecordBuffer = new NativeArray<byte>(length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
		}

		protected int RecordIndexToUserIndex(int index)
		{
			if (index < m_HeadIndex)
			{
				return m_HistoryDepth - m_HeadIndex + index;
			}
			return index - m_HeadIndex;
		}

		protected int UserIndexToRecordIndex(int index)
		{
			return (m_HeadIndex + index) % m_HistoryDepth;
		}

		protected unsafe RecordHeader* GetRecord(int index)
		{
			if (!m_RecordBuffer.IsCreated)
			{
				throw new InvalidOperationException("History buffer has been disposed");
			}
			if (index < 0 || index >= m_HistoryDepth)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			return (RecordHeader*)((byte*)m_RecordBuffer.GetUnsafePtr() + index * bytesPerRecord);
		}

		protected unsafe RecordHeader* AllocateRecord(out int index)
		{
			if (!m_RecordBuffer.IsCreated)
			{
				Allocate();
			}
			index = (m_HeadIndex + m_RecordCount) % m_HistoryDepth;
			if (m_RecordCount == m_HistoryDepth)
			{
				m_HeadIndex = (m_HeadIndex + 1) % m_HistoryDepth;
			}
			else
			{
				m_RecordCount++;
			}
			return (RecordHeader*)((byte*)m_RecordBuffer.GetUnsafePtr() + bytesPerRecord * index);
		}

		protected unsafe TValue ReadValue<TValue>(RecordHeader* data) where TValue : struct
		{
			int num;
			InputControl inputControl;
			if (m_ControlCount == 1)
			{
				num = ((!m_AddNewControls) ? 1 : 0);
				if (num != 0)
				{
					inputControl = controls[0];
					goto IL_003d;
				}
			}
			else
			{
				num = 0;
			}
			inputControl = controls[data->controlIndex];
			goto IL_003d;
			IL_003d:
			InputControl inputControl2 = inputControl;
			InputControl<TValue> inputControl3;
			if ((inputControl3 = inputControl2 as InputControl<TValue>) == null)
			{
				throw new InvalidOperationException(string.Format("Cannot read value of type '{0}' from control '{1}' with value type '{2}'", TypeHelpers.GetNiceTypeName(typeof(TValue)), inputControl2, TypeHelpers.GetNiceTypeName(inputControl2.valueType)));
			}
			byte* ptr = ((num != 0) ? data->statePtrWithoutControlIndex : data->statePtrWithControlIndex);
			ptr -= inputControl2.stateBlock.byteOffset;
			return inputControl3.ReadValueFromState(ptr);
		}

		protected unsafe object ReadValueAsObject(RecordHeader* data)
		{
			int num;
			InputControl inputControl;
			if (m_ControlCount == 1)
			{
				num = ((!m_AddNewControls) ? 1 : 0);
				if (num != 0)
				{
					inputControl = controls[0];
					goto IL_003d;
				}
			}
			else
			{
				num = 0;
			}
			inputControl = controls[data->controlIndex];
			goto IL_003d;
			IL_003d:
			InputControl inputControl2 = inputControl;
			byte* ptr = ((num != 0) ? data->statePtrWithoutControlIndex : data->statePtrWithControlIndex);
			ptr -= inputControl2.stateBlock.byteOffset;
			return inputControl2.ReadValueFromStateAsObject(ptr);
		}

		unsafe void IInputStateChangeMonitor.NotifyControlStateChanged(InputControl control, double time, InputEventPtr eventPtr, long monitorIndex)
		{
			InputUpdateType currentUpdateType = InputState.currentUpdateType;
			InputUpdateType inputUpdateType = updateMask;
			if (((currentUpdateType & inputUpdateType) != 0 || ((currentUpdateType & (InputUpdateType.Dynamic | InputUpdateType.Fixed)) != 0 && (inputUpdateType & (InputUpdateType.Dynamic | InputUpdateType.Fixed)) != 0 && eventPtr.valid)) && (onShouldRecordStateChange == null || onShouldRecordStateChange(control, time, eventPtr)))
			{
				RecordStateChange(control, control.currentStatePtr, time);
			}
		}

		void IInputStateChangeMonitor.NotifyTimerExpired(InputControl control, double time, long monitorIndex, int timerIndex)
		{
		}
	}
	public class InputStateHistory<TValue> : InputStateHistory, IReadOnlyList<InputStateHistory<TValue>.Record>, IEnumerable<InputStateHistory<TValue>.Record>, IEnumerable, IReadOnlyCollection<InputStateHistory<TValue>.Record> where TValue : struct
	{
		private struct Enumerator : IEnumerator<Record>, IEnumerator, IDisposable
		{
			private readonly InputStateHistory<TValue> m_History;

			private int m_Index;

			public Record Current
			{
				get
				{
					return m_History[m_Index];
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return Current;
				}
			}

			public Enumerator(InputStateHistory<TValue> history)
			{
				m_History = history;
				m_Index = -1;
			}

			public bool MoveNext()
			{
				if (m_Index + 1 >= m_History.Count)
				{
					return false;
				}
				m_Index++;
				return true;
			}

			public void Reset()
			{
				m_Index = -1;
			}

			public void Dispose()
			{
			}
		}

		public new struct Record : IEquatable<Record>
		{
			private readonly InputStateHistory<TValue> m_Owner;

			private readonly int m_IndexPlusOne;

			private uint m_Version;

			internal unsafe RecordHeader* header
			{
				get
				{
					return m_Owner.GetRecord(recordIndex);
				}
			}

			internal int recordIndex
			{
				get
				{
					return m_IndexPlusOne - 1;
				}
			}

			public unsafe bool valid
			{
				get
				{
					if (m_Owner != null && m_IndexPlusOne != 0)
					{
						return header->version == m_Version;
					}
					return false;
				}
			}

			public InputStateHistory<TValue> owner
			{
				get
				{
					return m_Owner;
				}
			}

			public int index
			{
				get
				{
					CheckValid();
					return m_Owner.RecordIndexToUserIndex(recordIndex);
				}
			}

			public unsafe double time
			{
				get
				{
					CheckValid();
					return header->time;
				}
			}

			public unsafe InputControl<TValue> control
			{
				get
				{
					CheckValid();
					ReadOnlyArray<InputControl> controls = m_Owner.controls;
					if (controls.Count == 1 && !m_Owner.m_AddNewControls)
					{
						return (InputControl<TValue>)controls[0];
					}
					return (InputControl<TValue>)controls[header->controlIndex];
				}
			}

			public unsafe Record next
			{
				get
				{
					CheckValid();
					int num = m_Owner.RecordIndexToUserIndex(recordIndex);
					if (num + 1 >= m_Owner.Count)
					{
						return default(Record);
					}
					int num2 = m_Owner.UserIndexToRecordIndex(num + 1);
					return new Record(m_Owner, num2, m_Owner.GetRecord(num2));
				}
			}

			public unsafe Record previous
			{
				get
				{
					CheckValid();
					int num = m_Owner.RecordIndexToUserIndex(recordIndex);
					if (num - 1 < 0)
					{
						return default(Record);
					}
					int num2 = m_Owner.UserIndexToRecordIndex(num - 1);
					return new Record(m_Owner, num2, m_Owner.GetRecord(num2));
				}
			}

			internal unsafe Record(InputStateHistory<TValue> owner, int index, RecordHeader* header)
			{
				m_Owner = owner;
				m_IndexPlusOne = index + 1;
				m_Version = header->version;
			}

			public unsafe TValue ReadValue()
			{
				CheckValid();
				return m_Owner.ReadValue<TValue>(header);
			}

			public unsafe void* GetUnsafeMemoryPtr()
			{
				CheckValid();
				if (m_Owner.controls.Count == 1 && !m_Owner.m_AddNewControls)
				{
					return header->statePtrWithoutControlIndex;
				}
				return header->statePtrWithControlIndex;
			}

			public unsafe void* GetUnsafeExtraMemoryPtr()
			{
				CheckValid();
				if (m_Owner.extraMemoryPerRecord == 0)
				{
					throw new InvalidOperationException("No extra memory has been set up for history records; set extraMemoryPerRecord");
				}
				return (byte*)header + m_Owner.bytesPerRecord - m_Owner.extraMemoryPerRecord;
			}

			public unsafe void CopyFrom(Record record)
			{
				CheckValid();
				if (!record.valid)
				{
					throw new ArgumentException("Given history record is not valid", "record");
				}
				InputStateHistory.Record record2 = new InputStateHistory.Record(m_Owner, recordIndex, header);
				record2.CopyFrom(new InputStateHistory.Record(record.m_Owner, record.recordIndex, record.header));
				m_Version = record2.version;
			}

			private unsafe void CheckValid()
			{
				if (m_Owner == null || m_IndexPlusOne == 0)
				{
					throw new InvalidOperationException("Value not initialized");
				}
				if (header->version != m_Version)
				{
					throw new InvalidOperationException("Record is no longer valid");
				}
			}

			public bool Equals(Record other)
			{
				if (m_Owner == other.m_Owner && m_IndexPlusOne == other.m_IndexPlusOne)
				{
					return m_Version == other.m_Version;
				}
				return false;
			}

			public override bool Equals(object obj)
			{
				object obj2;
				if ((obj2 = obj) is Record)
				{
					Record other = (Record)obj2;
					return Equals(other);
				}
				return false;
			}

			public override int GetHashCode()
			{
				return (((((m_Owner != null) ? m_Owner.GetHashCode() : 0) * 397) ^ m_IndexPlusOne) * 397) ^ (int)m_Version;
			}

			public override string ToString()
			{
				if (!valid)
				{
					return "<Invalid>";
				}
				return string.Format("{{ control={0} value={1} time={2} }}", control, ReadValue(), time);
			}
		}

		public new unsafe Record this[int index]
		{
			get
			{
				if (index < 0 || index >= base.Count)
				{
					throw new ArgumentOutOfRangeException(string.Format("Index {0} is out of range for history with {1} entries", index, base.Count), "index");
				}
				int index2 = UserIndexToRecordIndex(index);
				return new Record(this, index2, GetRecord(index2));
			}
			set
			{
				if (index < 0 || index >= base.Count)
				{
					throw new ArgumentOutOfRangeException(string.Format("Index {0} is out of range for history with {1} entries", index, base.Count), "index");
				}
				int index2 = UserIndexToRecordIndex(index);
				new Record(this, index2, GetRecord(index2)).CopyFrom(value);
			}
		}

		public InputStateHistory(int? maxStateSizeInBytes = null)
			: base(maxStateSizeInBytes ?? UnsafeUtility.SizeOf<TValue>())
		{
			if (maxStateSizeInBytes < UnsafeUtility.SizeOf<TValue>())
			{
				throw new ArgumentException("Max state size cannot be smaller than sizeof(TValue)", "maxStateSizeInBytes");
			}
		}

		public InputStateHistory(InputControl<TValue> control)
			: base(control)
		{
		}

		public InputStateHistory(string path)
			: base(path)
		{
			foreach (InputControl control in base.controls)
			{
				if (!typeof(TValue).IsAssignableFrom(control.valueType))
				{
					throw new ArgumentException(string.Format("Control '{0}' matched by '{1}' has value type '{2}' which is incompatible with '{3}'", control, path, TypeHelpers.GetNiceTypeName(control.valueType), TypeHelpers.GetNiceTypeName(typeof(TValue))));
				}
			}
		}

		~InputStateHistory()
		{
			Destroy();
		}

		public unsafe Record AddRecord(Record record)
		{
			int index;
			RecordHeader* header = AllocateRecord(out index);
			Record result = new Record(this, index, header);
			result.CopyFrom(record);
			return result;
		}

		public unsafe Record RecordStateChange(InputControl<TValue> control, TValue value, double time = -1.0)
		{
			InputEventPtr eventPtr;
			using (StateEvent.From(control.device, out eventPtr))
			{
				byte* statePtr = (byte*)StateEvent.From(eventPtr)->state - control.device.stateBlock.byteOffset;
				control.WriteValueIntoState(value, statePtr);
				if (time >= 0.0)
				{
					eventPtr.time = time;
				}
				InputStateHistory.Record record = RecordStateChange(control, eventPtr);
				return new Record(this, record.recordIndex, record.header);
			}
		}

		public new IEnumerator<Record> GetEnumerator()
		{
			return new Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
