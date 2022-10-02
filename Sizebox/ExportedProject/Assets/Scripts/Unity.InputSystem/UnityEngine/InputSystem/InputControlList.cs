using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
	[DebuggerDisplay("Count = {Count}")]
	public struct InputControlList<TControl> : IList<TControl>, ICollection<TControl>, IEnumerable<TControl>, IEnumerable, IReadOnlyList<TControl>, IReadOnlyCollection<TControl>, IDisposable where TControl : InputControl
	{
		private struct Enumerator : IEnumerator<TControl>, IEnumerator, IDisposable
		{
			private unsafe readonly ulong* m_Indices;

			private readonly int m_Count;

			private int m_Current;

			public unsafe TControl Current
			{
				get
				{
					if (m_Indices == null)
					{
						throw new InvalidOperationException("Enumerator is not valid");
					}
					return InputControlList<TControl>.FromIndex(m_Indices[m_Current]);
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return Current;
				}
			}

			public unsafe Enumerator(InputControlList<TControl> list)
			{
				m_Count = list.m_Count;
				m_Current = -1;
				m_Indices = (ulong*)((m_Count > 0) ? list.m_Indices.GetUnsafeReadOnlyPtr() : null);
			}

			public bool MoveNext()
			{
				if (m_Current >= m_Count)
				{
					return false;
				}
				m_Current++;
				return m_Current != m_Count;
			}

			public void Reset()
			{
				m_Current = -1;
			}

			public void Dispose()
			{
			}
		}

		private int m_Count;

		private NativeArray<ulong> m_Indices;

		private readonly Allocator m_Allocator;

		private const ulong kInvalidIndex = ulong.MaxValue;

		public int Count
		{
			get
			{
				return m_Count;
			}
		}

		public int Capacity
		{
			get
			{
				if (!m_Indices.IsCreated)
				{
					return 0;
				}
				return m_Indices.Length;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("Capacity cannot be negative", "value");
				}
				if (value == 0)
				{
					if (m_Count != 0)
					{
						m_Indices.Dispose();
					}
					m_Count = 0;
				}
				else
				{
					Allocator allocator = ((m_Allocator != 0) ? m_Allocator : Allocator.Persistent);
					ArrayHelpers.Resize(ref m_Indices, value, allocator);
				}
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public TControl this[int index]
		{
			get
			{
				if (index < 0 || index >= m_Count)
				{
					throw new ArgumentOutOfRangeException("index", string.Format("Index {0} is out of range in list with {1} entries", index, m_Count));
				}
				return FromIndex(m_Indices[index]);
			}
			set
			{
				if (index < 0 || index >= m_Count)
				{
					throw new ArgumentOutOfRangeException("index", string.Format("Index {0} is out of range in list with {1} entries", index, m_Count));
				}
				m_Indices[index] = ToIndex(value);
			}
		}

		public InputControlList(Allocator allocator, int initialCapacity = 0)
		{
			m_Allocator = allocator;
			m_Indices = default(NativeArray<ulong>);
			m_Count = 0;
			if (initialCapacity != 0)
			{
				Capacity = initialCapacity;
			}
		}

		public InputControlList(IEnumerable<TControl> values, Allocator allocator = Allocator.Persistent)
			: this(allocator)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			foreach (TControl value in values)
			{
				Add(value);
			}
		}

		public InputControlList(params TControl[] values)
		{
			this = default(InputControlList<TControl>);
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			int num = values.Length;
			Capacity = Mathf.Max(num, 10);
			for (int i = 0; i < num; i++)
			{
				Add(values[i]);
			}
		}

		public void Add(TControl item)
		{
			ulong value = ToIndex(item);
			Allocator allocator = ((m_Allocator != 0) ? m_Allocator : Allocator.Persistent);
			ArrayHelpers.AppendWithCapacity(ref m_Indices, ref m_Count, value, 10, allocator);
		}

		public void AddSlice<TList>(TList list, int count = -1, int destinationIndex = -1, int sourceIndex = 0) where TList : IReadOnlyList<TControl>
		{
			if (count < 0)
			{
				count = list.Count;
			}
			if (destinationIndex < 0)
			{
				destinationIndex = Count;
			}
			if (count != 0)
			{
				if (sourceIndex + count > list.Count)
				{
					throw new ArgumentOutOfRangeException("count", string.Format("Count of {0} elements starting at index {1} exceeds length of list of {2}", count, sourceIndex, list.Count));
				}
				if (Capacity < m_Count + count)
				{
					Capacity = Math.Max(m_Count + count, 10);
				}
				if (destinationIndex < Count)
				{
					NativeArray<ulong>.Copy(m_Indices, destinationIndex, m_Indices, destinationIndex + count, Count - destinationIndex);
				}
				for (int i = 0; i < count; i++)
				{
					m_Indices[destinationIndex + i] = ToIndex(list[sourceIndex + i]);
				}
				m_Count += count;
			}
		}

		public void AddRange(IEnumerable<TControl> list, int count = -1, int destinationIndex = -1)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (count < 0)
			{
				count = list.Count();
			}
			if (destinationIndex < 0)
			{
				destinationIndex = Count;
			}
			if (count == 0)
			{
				return;
			}
			if (Capacity < m_Count + count)
			{
				Capacity = Math.Max(m_Count + count, 10);
			}
			if (destinationIndex < Count)
			{
				NativeArray<ulong>.Copy(m_Indices, destinationIndex, m_Indices, destinationIndex + count, Count - destinationIndex);
			}
			foreach (TControl item in list)
			{
				m_Indices[destinationIndex++] = ToIndex(item);
				m_Count++;
				count--;
				if (count == 0)
				{
					break;
				}
			}
		}

		public bool Remove(TControl item)
		{
			if (m_Count == 0)
			{
				return false;
			}
			ulong num = ToIndex(item);
			for (int i = 0; i < m_Count; i++)
			{
				if (m_Indices[i] == num)
				{
					ArrayHelpers.EraseAtWithCapacity(m_Indices, ref m_Count, i);
					return true;
				}
			}
			return false;
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || index >= m_Count)
			{
				throw new ArgumentOutOfRangeException("index", string.Format("Index {0} is out of range in list with {1} elements", index, m_Count));
			}
			ArrayHelpers.EraseAtWithCapacity(m_Indices, ref m_Count, index);
		}

		public void CopyTo(TControl[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public unsafe int IndexOf(TControl item)
		{
			if (m_Count == 0)
			{
				return -1;
			}
			ulong num = ToIndex(item);
			ulong* unsafeReadOnlyPtr = (ulong*)m_Indices.GetUnsafeReadOnlyPtr();
			for (int i = 0; i < m_Count; i++)
			{
				if (unsafeReadOnlyPtr[i] == num)
				{
					return i;
				}
			}
			return -1;
		}

		public void Insert(int index, TControl item)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			m_Count = 0;
		}

		public bool Contains(TControl item)
		{
			return IndexOf(item) != -1;
		}

		public void SwapElements(int index1, int index2)
		{
			if (index1 < 0 || index1 >= m_Count)
			{
				throw new ArgumentOutOfRangeException("index1");
			}
			if (index2 < 0 || index2 >= m_Count)
			{
				throw new ArgumentOutOfRangeException("index2");
			}
			if (index1 != index2)
			{
				m_Indices.SwapElements(index1, index2);
			}
		}

		public void Sort<TCompare>(int startIndex, int count, TCompare comparer) where TCompare : IComparer<TControl>
		{
			if (startIndex < 0 || startIndex >= Count)
			{
				throw new ArgumentOutOfRangeException("startIndex");
			}
			if (startIndex + count >= Count)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			for (int i = 1; i < count; i++)
			{
				int num = i;
				while (num > 0 && comparer.Compare(this[num - 1], this[num]) < 0)
				{
					SwapElements(num, num - 1);
					num--;
				}
			}
		}

		public TControl[] ToArray(bool dispose = false)
		{
			TControl[] array = new TControl[m_Count];
			for (int i = 0; i < m_Count; i++)
			{
				array[i] = this[i];
			}
			if (dispose)
			{
				Dispose();
			}
			return array;
		}

		internal void AppendTo(ref TControl[] array, ref int count)
		{
			for (int i = 0; i < m_Count; i++)
			{
				ArrayHelpers.AppendWithCapacity(ref array, ref count, this[i]);
			}
		}

		public void Dispose()
		{
			if (m_Indices.IsCreated)
			{
				m_Indices.Dispose();
			}
		}

		public IEnumerator<TControl> GetEnumerator()
		{
			return new Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public override string ToString()
		{
			if (Count == 0)
			{
				return "()";
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('(');
			for (int i = 0; i < Count; i++)
			{
				if (i != 0)
				{
					stringBuilder.Append(',');
				}
				stringBuilder.Append(this[i]);
			}
			stringBuilder.Append(')');
			return stringBuilder.ToString();
		}

		private static ulong ToIndex(TControl control)
		{
			if (control == null)
			{
				return ulong.MaxValue;
			}
			InputDevice device = control.device;
			int deviceIndex = device.m_DeviceIndex;
			int num = ((device != control) ? (device.m_ChildrenForEachControl.IndexOfReference(control) + 1) : 0);
			long num2 = (long)deviceIndex << 32;
			ulong num3 = (ulong)num;
			return (ulong)num2 | num3;
		}

		private static TControl FromIndex(ulong index)
		{
			if (index == ulong.MaxValue)
			{
				return null;
			}
			int index2 = (int)(index >> 32);
			int num = (int)(index & 0xFFFFFFFFu);
			InputDevice inputDevice = InputSystem.devices[index2];
			if (num == 0)
			{
				return (TControl)(InputControl)inputDevice;
			}
			return (TControl)inputDevice.m_ChildrenForEachControl[num - 1];
		}
	}
}
