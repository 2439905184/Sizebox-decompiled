using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

namespace BulletXNA.LinearMath
{
	public class ObjectArray<T> : IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable where T : new()
	{
		public struct Enumerator<T> : IEnumerator<T>, IDisposable, IEnumerator where T : new()
		{
			private ObjectArray<T> list;

			private int index;

			private int version;

			private T current;

			public T Current
			{
				get
				{
					return current;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					if (index == 0 || index == list._size + 1)
					{
						throw new Exception("InvalidOperation EnumOpCantHappen");
					}
					return Current;
				}
			}

			internal Enumerator(ObjectArray<T> list)
			{
				this.list = list;
				index = 0;
				version = list._version;
				current = default(T);
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				ObjectArray<T> objectArray = list;
				if (version == objectArray._version && index < objectArray._size)
				{
					current = objectArray._items[index];
					index++;
					return true;
				}
				return MoveNextRare();
			}

			private bool MoveNextRare()
			{
				if (version != list._version)
				{
					throw new Exception("InvalidOperation EnumFailedVersion");
				}
				index = list._size + 1;
				current = default(T);
				return false;
			}

			void IEnumerator.Reset()
			{
				if (version != list._version)
				{
					throw new Exception("InvalidOperation EnumFailedVersion");
				}
				index = 0;
				current = default(T);
			}
		}

		internal sealed class FunctorComparer<T> : IComparer<T>
		{
			private Comparer<T> c;

			private Comparison<T> comparison;

			public FunctorComparer(Comparison<T> comparison)
			{
				c = Comparer<T>.Default;
				this.comparison = comparison;
			}

			public int Compare(T x, T y)
			{
				return comparison(x, y);
			}
		}

		private const int _defaultCapacity = 4;

		private static T[] _emptyArray;

		private T[] _items;

		private int _size;

		private object _syncRoot;

		private int _version;

		public int Capacity
		{
			get
			{
				return _items.Length;
			}
			set
			{
				if (value == _items.Length)
				{
					return;
				}
				if (value < _size)
				{
					throw new Exception("ExceptionResource ArgumentOutOfRange_SmallCapacity");
				}
				if (value > 0)
				{
					T[] array = new T[value];
					if (_size > 0)
					{
						Array.Copy(_items, 0, array, 0, _size);
					}
					_items = array;
				}
				else
				{
					_items = _emptyArray;
				}
			}
		}

		public int Count
		{
			get
			{
				return _size;
			}
		}

		public T this[int index]
		{
			get
			{
				CheckAndGrow(index);
				if (index >= _size)
				{
					throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException()");
				}
				return _items[index];
			}
			set
			{
				CheckAndGrow(index);
				if (index >= _size)
				{
					throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException()");
				}
				_items[index] = value;
				_version++;
			}
		}

		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				if (_syncRoot == null)
				{
					Interlocked.CompareExchange(ref _syncRoot, new object(), null);
				}
				return _syncRoot;
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		object IList.this[int index]
		{
			get
			{
				CheckAndGrow(index);
				return this[index];
			}
			set
			{
				CheckAndGrow(index);
				VerifyValueType(value);
				this[index] = (T)value;
			}
		}

		static ObjectArray()
		{
			_emptyArray = new T[0];
		}

		public ObjectArray()
		{
			_items = _emptyArray;
		}

		public T[] GetRawArray()
		{
			return _items;
		}

		public ObjectArray(IEnumerable<T> collection)
		{
			if (collection == null)
			{
				throw new Exception("ThrowHelper.ThrowArgumentNullException(ExceptionArgument.collection");
			}
			ICollection<T> collection2 = collection as ICollection<T>;
			if (collection2 != null)
			{
				int count = collection2.Count;
				_items = new T[count];
				collection2.CopyTo(_items, 0);
				_size = count;
				return;
			}
			_size = 0;
			_items = new T[4];
			foreach (T item in collection)
			{
				Add(item);
			}
		}

		public ObjectArray(int capacity)
		{
			if (capacity < 0)
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.capacity, ExceptionResource.ArgumentOutOfRange_SmallCapacity");
			}
			_items = new T[capacity];
		}

		public void Add(T item)
		{
			if (_size == _items.Length)
			{
				EnsureCapacity(_size + 1);
			}
			_items[_size++] = item;
			_version++;
		}

		public void AddRange(IEnumerable<T> collection)
		{
			InsertRange(_size, collection);
		}

		public ReadOnlyCollection<T> AsReadOnly()
		{
			return new ReadOnlyCollection<T>(this);
		}

		public void Swap(int index0, int index1)
		{
			T val = _items[index0];
			_items[index0] = _items[index1];
			_items[index1] = val;
		}

		public void Resize(int newsize)
		{
			Resize(newsize, true);
		}

		public void Resize(int newsize, bool allocateOrReset)
		{
			int count = Count;
			if (newsize < count)
			{
				if (allocateOrReset)
				{
					for (int i = newsize; i < count; i++)
					{
						if (_items[i] == null)
						{
							_items[i] = new T();
						}
					}
				}
				else
				{
					for (int j = newsize; j < count; j++)
					{
						_items[j] = default(T);
					}
				}
			}
			else
			{
				if (newsize > Count)
				{
					EnsureCapacity(newsize);
				}
				if (allocateOrReset)
				{
					for (int k = count; k < newsize; k++)
					{
						if (_items[k] == null)
						{
							_items[k] = new T();
						}
					}
				}
			}
			_size = newsize;
			_version++;
		}

		public int BinarySearch(T item)
		{
			return BinarySearch(0, Count, item, null);
		}

		public int BinarySearch(T item, IComparer<T> comparer)
		{
			return BinarySearch(0, Count, item, comparer);
		}

		public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
		{
			if (index < 0 || count < 0)
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException((index < 0) ? ExceptionArgument.index : ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum");
			}
			if (_size - index < count)
			{
				throw new Exception("ExceptionResource  - Offlen");
			}
			return Array.BinarySearch(_items, index, count, item, comparer);
		}

		public void Clear()
		{
			if (_size > 0)
			{
				Array.Clear(_items, 0, _size);
				_size = 0;
			}
			_version++;
		}

		public void Truncate(int numElements)
		{
			if (_size > 0 && numElements <= _size)
			{
				int num = _size - numElements;
				Array.Clear(_items, num, numElements);
				_size = num;
			}
			_version++;
		}

		public bool Contains(T item)
		{
			if (item == null)
			{
				for (int i = 0; i < _size; i++)
				{
					if (_items[i] == null)
					{
						return true;
					}
				}
				return false;
			}
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			for (int j = 0; j < _size; j++)
			{
				if (@default.Equals(_items[j], item))
				{
					return true;
				}
			}
			return false;
		}

		public ObjectArray<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter) where TOutput : new()
		{
			if (converter == null)
			{
				throw new Exception("ThrowHelper.ThrowArgumentNullException(ExceptionArgument.converter");
			}
			ObjectArray<TOutput> objectArray = new ObjectArray<TOutput>(_size);
			for (int i = 0; i < _size; i++)
			{
				objectArray._items[i] = converter(_items[i]);
			}
			objectArray._size = _size;
			return objectArray;
		}

		public void CopyTo(T[] array)
		{
			CopyTo(array, 0);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			Array.Copy(_items, 0, array, arrayIndex, _size);
		}

		public void CopyTo(int index, T[] array, int arrayIndex, int count)
		{
			if (_size - index < count)
			{
				throw new Exception("ExceptionResource  - Offlen");
			}
			Array.Copy(_items, index, array, arrayIndex, count);
		}

		public void EnsureCapacity(int min)
		{
			if (_items.Length < min)
			{
				int num = ((_items.Length == 0) ? 4 : (_items.Length * 2));
				if (num < min)
				{
					num = min;
				}
				Capacity = num;
			}
		}

		public bool Exists(Predicate<T> match)
		{
			return FindIndex(match) != -1;
		}

		public T Find(Predicate<T> match)
		{
			if (match == null)
			{
				throw new Exception("ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match");
			}
			for (int i = 0; i < _size; i++)
			{
				if (match(_items[i]))
				{
					return _items[i];
				}
			}
			return default(T);
		}

		public ObjectArray<T> FindAll(Predicate<T> match)
		{
			if (match == null)
			{
				throw new Exception("ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match");
			}
			ObjectArray<T> objectArray = new ObjectArray<T>();
			for (int i = 0; i < _size; i++)
			{
				if (match(_items[i]))
				{
					objectArray.Add(_items[i]);
				}
			}
			return objectArray;
		}

		public int FindIndex(Predicate<T> match)
		{
			return FindIndex(0, _size, match);
		}

		public int FindIndex(int startIndex, Predicate<T> match)
		{
			return FindIndex(startIndex, _size - startIndex, match);
		}

		public int FindIndex(int startIndex, int count, Predicate<T> match)
		{
			if (startIndex > _size)
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.startIndex, ExceptionResource.ArgumentOutOfRange_Index");
			}
			if (count < 0 || startIndex > _size - count)
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_Count");
			}
			if (match == null)
			{
				throw new Exception("ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match");
			}
			int num = startIndex + count;
			for (int i = startIndex; i < num; i++)
			{
				if (match(_items[i]))
				{
					return i;
				}
			}
			return -1;
		}

		public T FindLast(Predicate<T> match)
		{
			if (match == null)
			{
				throw new Exception("ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match");
			}
			for (int num = _size - 1; num >= 0; num--)
			{
				if (match(_items[num]))
				{
					return _items[num];
				}
			}
			return default(T);
		}

		public int FindLastIndex(Predicate<T> match)
		{
			return FindLastIndex(_size - 1, _size, match);
		}

		public int FindLastIndex(int startIndex, Predicate<T> match)
		{
			return FindLastIndex(startIndex, startIndex + 1, match);
		}

		public int FindLastIndex(int startIndex, int count, Predicate<T> match)
		{
			if (match == null)
			{
				throw new Exception("ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match");
			}
			if (_size == 0)
			{
				if (startIndex != -1)
				{
					throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.startIndex, ExceptionResource.ArgumentOutOfRange_Index");
				}
			}
			else if (startIndex >= _size)
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.startIndex, ExceptionResource.ArgumentOutOfRange_Index");
			}
			if (count < 0 || startIndex - count + 1 < 0)
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_Count");
			}
			int num = startIndex - count;
			for (int num2 = startIndex; num2 > num; num2--)
			{
				if (match(_items[num2]))
				{
					return num2;
				}
			}
			return -1;
		}

		public void ForEach(Action<T> action)
		{
			if (action == null)
			{
				throw new Exception("ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match");
			}
			for (int i = 0; i < _size; i++)
			{
				action(_items[i]);
			}
		}

		public ObjectArray<T> GetRange(int index, int count)
		{
			if (index < 0 || count < 0)
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException((index < 0) ? ExceptionArgument.index : ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum");
			}
			if (_size - index < count)
			{
				throw new Exception("ExceptionResource  - Offlen");
			}
			ObjectArray<T> objectArray = new ObjectArray<T>(count);
			Array.Copy(_items, index, objectArray._items, 0, count);
			objectArray._size = count;
			return objectArray;
		}

		public int IndexOf(T item)
		{
			return Array.IndexOf(_items, item, 0, _size);
		}

		public int IndexOf(T item, int index)
		{
			if (index > _size)
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_Index");
			}
			return Array.IndexOf(_items, item, index, _size - index);
		}

		public int IndexOf(T item, int index, int count)
		{
			if (index > _size)
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_Index");
			}
			if (count < 0 || index > _size - count)
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_Count");
			}
			return Array.IndexOf(_items, item, index, count);
		}

		public void Insert(int index, T item)
		{
			if (index > _size)
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_ListInsert");
			}
			if (_size == _items.Length)
			{
				EnsureCapacity(_size + 1);
			}
			if (index < _size)
			{
				Array.Copy(_items, index, _items, index + 1, _size - index);
			}
			_items[index] = item;
			_size++;
			_version++;
		}

		public void InsertRange(int index, IEnumerable<T> collection)
		{
			if (collection == null)
			{
				throw new Exception("ThrowHelper.ThrowArgumentNullException(ExceptionArgument.collection");
			}
			if (index > _size)
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_Index");
			}
			ICollection<T> collection2 = collection as ICollection<T>;
			if (collection2 != null)
			{
				int count = collection2.Count;
				if (count > 0)
				{
					EnsureCapacity(_size + count);
					if (index < _size)
					{
						Array.Copy(_items, index, _items, index + count, _size - index);
					}
					if (this == collection2)
					{
						Array.Copy(_items, 0, _items, index, index);
						Array.Copy(_items, index + count, _items, index * 2, _size - index);
					}
					else
					{
						T[] array = new T[count];
						collection2.CopyTo(array, 0);
						array.CopyTo(_items, index);
					}
					_size += count;
				}
			}
			else
			{
				using (IEnumerator<T> enumerator = collection.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Insert(index++, enumerator.Current);
					}
				}
			}
			_version++;
		}

		private static bool IsCompatibleObject(object value)
		{
			if (!(value is T) && (value != null || typeof(T).IsValueType))
			{
				return false;
			}
			return true;
		}

		public int LastIndexOf(T item)
		{
			return LastIndexOf(item, _size - 1, _size);
		}

		public int LastIndexOf(T item, int index)
		{
			if (index >= _size)
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_Index");
			}
			return LastIndexOf(item, index, index + 1);
		}

		public int LastIndexOf(T item, int index, int count)
		{
			if (_size == 0)
			{
				return -1;
			}
			if (index < 0 || count < 0)
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException((index < 0) ? ExceptionArgument.index : ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum");
			}
			if (index >= _size || count > index + 1)
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException((index >= this._size) ? ExceptionArgument.index : ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_BiggerThanCollection");
			}
			return Array.LastIndexOf(_items, item, index, count);
		}

		public bool RemoveQuick(T item)
		{
			int num = IndexOf(item);
			if (num >= 0)
			{
				if (_size > 0)
				{
					_items[num] = _items[_size - 1];
				}
				_size--;
				return true;
			}
			return false;
		}

		public bool RemoveAtQuick(int index)
		{
			if (index >= 0)
			{
				if (_size > 0)
				{
					_items[index] = _items[_size - 1];
					_items[_size - 1] = default(T);
				}
				_size--;
				return true;
			}
			return false;
		}

		public bool Remove(T item)
		{
			int num = IndexOf(item);
			if (num >= 0)
			{
				RemoveAt(num);
				return true;
			}
			return false;
		}

		public int RemoveAll(Predicate<T> match)
		{
			if (match == null)
			{
				throw new Exception("ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match");
			}
			int i;
			for (i = 0; i < _size && !match(_items[i]); i++)
			{
			}
			if (i >= _size)
			{
				return 0;
			}
			int j = i + 1;
			while (j < _size)
			{
				for (; j < _size && match(_items[j]); j++)
				{
				}
				if (j < _size)
				{
					_items[i++] = _items[j++];
				}
			}
			Array.Clear(_items, i, _size - i);
			int result = _size - i;
			_size = i;
			_version++;
			return result;
		}

		public void RemoveAt(int index)
		{
			if (index >= _size)
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException");
			}
			_size--;
			if (index < _size)
			{
				Array.Copy(_items, index + 1, _items, index, _size - index);
			}
			_items[_size] = default(T);
			_version++;
		}

		public void RemoveRange(int index, int count)
		{
			if (index < 0 || count < 0)
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException((index < 0) ? ExceptionArgument.index : ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum");
			}
			if (_size - index < count)
			{
				throw new Exception("ExceptionResource  - Offlen");
			}
			if (count > 0)
			{
				_size -= count;
				if (index < _size)
				{
					Array.Copy(_items, index + count, _items, index, _size - index);
				}
				Array.Clear(_items, _size, count);
				_version++;
			}
		}

		public void PopBack()
		{
			RemoveAtQuick(Count - 1);
		}

		public void Reverse()
		{
			Reverse(0, Count);
		}

		public void Reverse(int index, int count)
		{
			if (index < 0 || count < 0)
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException((index < 0) ? ExceptionArgument.index : ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum");
			}
			if (_size - index < count)
			{
				throw new Exception("ExceptionResource  - Offlen");
			}
			Array.Reverse((Array)_items, index, count);
			_version++;
		}

		public void Sort()
		{
			Sort(0, Count, null);
		}

		public void Sort(IComparer<T> comparer)
		{
			Sort(0, Count, comparer);
		}

		public void Sort(Comparison<T> comparison)
		{
			if (comparison == null)
			{
				throw new Exception("ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match");
			}
			if (_size > 0)
			{
				IComparer<T> comparer = new FunctorComparer<T>(comparison);
				Array.Sort(_items, 0, _size, comparer);
			}
		}

		public void Sort(int index, int count, IComparer<T> comparer)
		{
			if (index < 0 || count < 0)
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException((index < 0) ? ExceptionArgument.index : ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum");
			}
			if (_size - index < count)
			{
				throw new Exception("ExceptionResource  - Offlen");
			}
			Array.Sort(_items, index, count, comparer);
			_version++;
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return new Enumerator<T>(this);
		}

		void ICollection.CopyTo(Array array, int arrayIndex)
		{
			if (array != null && array.Rank != 1)
			{
				throw new Exception("ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported");
			}
			try
			{
				Array.Copy(_items, 0, array, arrayIndex, _size);
			}
			catch (ArrayTypeMismatchException)
			{
				throw new Exception("ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArrayType");
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator<T>(this);
		}

		int IList.Add(object item)
		{
			VerifyValueType(item);
			Add((T)item);
			return Count - 1;
		}

		bool IList.Contains(object item)
		{
			if (IsCompatibleObject(item))
			{
				return Contains((T)item);
			}
			return false;
		}

		int IList.IndexOf(object item)
		{
			if (IsCompatibleObject(item))
			{
				return IndexOf((T)item);
			}
			return -1;
		}

		void IList.Insert(int index, object item)
		{
			VerifyValueType(item);
			Insert(index, (T)item);
		}

		void IList.Remove(object item)
		{
			if (IsCompatibleObject(item))
			{
				Remove((T)item);
			}
		}

		public T[] ToArray()
		{
			T[] array = new T[_size];
			Array.Copy(_items, 0, array, 0, _size);
			return array;
		}

		public void TrimExcess()
		{
			int num = (int)((double)_items.Length * 0.9);
			if (_size < num)
			{
				Capacity = _size;
			}
		}

		public bool TrueForAll(Predicate<T> match)
		{
			if (match == null)
			{
				throw new Exception("ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match");
			}
			for (int i = 0; i < _size; i++)
			{
				if (!match(_items[i]))
				{
					return false;
				}
			}
			return true;
		}

		private static void VerifyValueType(object value)
		{
			if (!IsCompatibleObject(value))
			{
				throw new Exception("ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(T)");
			}
		}

		private void CheckAndGrow(int newSize)
		{
			if (newSize >= _items.Length)
			{
				EnsureCapacity(newSize + 1);
			}
			int num = newSize + 1 - _size;
			for (int i = 0; i < num; i++)
			{
				if (_items[_size] == null)
				{
					_items[_size] = new T();
				}
				_size++;
			}
			_version++;
		}

		public void QuickSort(IQSComparer<T> comparer)
		{
			if (Count > 1)
			{
				QuickSortInternal(comparer, 0, Count - 1);
			}
		}

		private void QuickSortInternal(IQSComparer<T> comparer, int lo, int hi)
		{
			int num = lo;
			int num2 = hi;
			T val = _items[(lo + hi) / 2];
			while (true)
			{
				if (comparer.Compare(_items[num], val))
				{
					num++;
					continue;
				}
				while (comparer.Compare(val, _items[num2]))
				{
					num2--;
				}
				if (num <= num2)
				{
					Swap(num, num2);
					num++;
					num2--;
				}
				if (num > num2)
				{
					break;
				}
			}
			if (lo < num2)
			{
				QuickSortInternal(comparer, lo, num2);
			}
			if (num < hi)
			{
				QuickSortInternal(comparer, num, hi);
			}
		}
	}
}
