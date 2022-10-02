using System.Collections;
using System.Collections.Generic;

public class ThreadSafeList<T> : ThreadSafeCollection<T>, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
	private readonly IList<T> _list;

	protected override ICollection<T> collection
	{
		get
		{
			return _list;
		}
	}

	public T this[int index]
	{
		get
		{
			lock (@lock)
			{
				return _list[index];
			}
		}
		set
		{
			lock (@lock)
			{
				_list[index] = value;
			}
		}
	}

	public ThreadSafeList()
	{
		_list = new List<T>();
	}

	public ThreadSafeList(int size)
	{
		lock (@lock)
		{
			_list = new List<T>(size);
		}
	}

	public ThreadSafeList(IList<T> list)
	{
		lock (@lock)
		{
			_list = new List<T>(list);
		}
	}

	public int IndexOf(T item)
	{
		lock (@lock)
		{
			return _list.IndexOf(item);
		}
	}

	public void Insert(int index, T item)
	{
		lock (@lock)
		{
			_list.Insert(index, item);
		}
	}

	public void RemoveAt(int index)
	{
		lock (@lock)
		{
			_list.RemoveAt(index);
		}
	}
}
