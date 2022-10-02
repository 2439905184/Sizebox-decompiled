using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ThreadSafeCollection<T> : ICollection<T>, IEnumerable<T>, IEnumerable
{
	protected readonly object @lock;

	protected abstract ICollection<T> collection { get; }

	public int Count
	{
		get
		{
			lock (@lock)
			{
				return collection.Count;
			}
		}
	}

	public bool IsReadOnly
	{
		get
		{
			lock (@lock)
			{
				return collection.IsReadOnly;
			}
		}
	}

	protected ThreadSafeCollection()
	{
		@lock = new object();
	}

	public void Add(T item)
	{
		lock (@lock)
		{
			collection.Add(item);
		}
	}

	public void Clear()
	{
		lock (@lock)
		{
			collection.Clear();
		}
	}

	public bool Contains(T item)
	{
		lock (@lock)
		{
			return collection.Contains(item);
		}
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		lock (@lock)
		{
			collection.CopyTo(array, arrayIndex);
		}
	}

	public bool Remove(T item)
	{
		lock (@lock)
		{
			return collection.Remove(item);
		}
	}

	public IEnumerator<T> GetEnumerator()
	{
		lock (@lock)
		{
			return collection.GetEnumerator();
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		lock (@lock)
		{
			return collection.GetEnumerator();
		}
	}

	public ThreadSafeEnumerator<T> GetThreadSafeEnumerator()
	{
		Debug.Log("A use of ThreadSafeEnumerator has occured. This can cause blocking in the main Unity thread.");
		return new ThreadSafeEnumerator<T>(GetEnumerator(), @lock);
	}
}
