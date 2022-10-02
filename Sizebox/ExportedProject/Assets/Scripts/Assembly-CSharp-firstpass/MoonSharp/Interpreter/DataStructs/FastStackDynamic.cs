using System.Collections.Generic;

namespace MoonSharp.Interpreter.DataStructs
{
	internal class FastStackDynamic<T> : List<T>
	{
		public FastStackDynamic(int startingCapacity)
			: base(startingCapacity)
		{
		}

		public void Set(int idxofs, T item)
		{
			base[base.Count - 1 - idxofs] = item;
		}

		public T Push(T item)
		{
			Add(item);
			return item;
		}

		public void Expand(int size)
		{
			for (int i = 0; i < size; i++)
			{
				Add(default(T));
			}
		}

		public void Zero(int index)
		{
			base[index] = default(T);
		}

		public T Peek(int idxofs = 0)
		{
			return base[base.Count - 1 - idxofs];
		}

		public void CropAtCount(int p)
		{
			RemoveLast(base.Count - p);
		}

		public void RemoveLast(int cnt = 1)
		{
			if (cnt == 1)
			{
				RemoveAt(base.Count - 1);
			}
			else
			{
				RemoveRange(base.Count - cnt, cnt);
			}
		}

		public T Pop()
		{
			T result = base[base.Count - 1];
			RemoveAt(base.Count - 1);
			return result;
		}
	}
}
