using System;
using System.Collections;
using System.Collections.Generic;

namespace Priority_Queue
{
	public class SimplePriorityQueue<TItem, TPriority> : IPriorityQueue<TItem, TPriority>, IEnumerable<TItem>, IEnumerable where TPriority : IComparable<TPriority>
	{
		private class SimpleNode : GenericPriorityQueueNode<TPriority>
		{
			public TItem Data { get; private set; }

			public SimpleNode(TItem data)
			{
				Data = data;
			}
		}

		private const int INITIAL_QUEUE_SIZE = 10;

		private readonly GenericPriorityQueue<SimpleNode, TPriority> _queue;

		public int Count
		{
			get
			{
				lock (_queue)
				{
					return _queue.Count;
				}
			}
		}

		public TItem First
		{
			get
			{
				lock (_queue)
				{
					if (_queue.Count <= 0)
					{
						throw new InvalidOperationException("Cannot call .First on an empty queue");
					}
					SimpleNode first = _queue.First;
					return (first != null) ? first.Data : default(TItem);
				}
			}
		}

		public SimplePriorityQueue()
		{
			_queue = new GenericPriorityQueue<SimpleNode, TPriority>(10);
		}

		private SimpleNode GetExistingNode(TItem item)
		{
			EqualityComparer<TItem> @default = EqualityComparer<TItem>.Default;
			foreach (SimpleNode item2 in _queue)
			{
				if (@default.Equals(item2.Data, item))
				{
					return item2;
				}
			}
			throw new InvalidOperationException("Item cannot be found in queue: " + item);
		}

		public void Clear()
		{
			lock (_queue)
			{
				_queue.Clear();
			}
		}

		public bool Contains(TItem item)
		{
			lock (_queue)
			{
				EqualityComparer<TItem> @default = EqualityComparer<TItem>.Default;
				foreach (SimpleNode item2 in _queue)
				{
					if (@default.Equals(item2.Data, item))
					{
						return true;
					}
				}
				return false;
			}
		}

		public TItem Dequeue()
		{
			lock (_queue)
			{
				if (_queue.Count <= 0)
				{
					throw new InvalidOperationException("Cannot call Dequeue() on an empty queue");
				}
				return _queue.Dequeue().Data;
			}
		}

		public void Enqueue(TItem item, TPriority priority)
		{
			lock (_queue)
			{
				SimpleNode node = new SimpleNode(item);
				if (_queue.Count == _queue.MaxSize)
				{
					_queue.Resize(_queue.MaxSize * 2 + 1);
				}
				_queue.Enqueue(node, priority);
			}
		}

		public void Remove(TItem item)
		{
			lock (_queue)
			{
				try
				{
					_queue.Remove(GetExistingNode(item));
				}
				catch (InvalidOperationException innerException)
				{
					throw new InvalidOperationException("Cannot call Remove() on a node which is not enqueued: " + item, innerException);
				}
			}
		}

		public void UpdatePriority(TItem item, TPriority priority)
		{
			lock (_queue)
			{
				try
				{
					SimpleNode existingNode = GetExistingNode(item);
					_queue.UpdatePriority(existingNode, priority);
				}
				catch (InvalidOperationException innerException)
				{
					throw new InvalidOperationException("Cannot call UpdatePriority() on a node which is not enqueued: " + item, innerException);
				}
			}
		}

		public IEnumerator<TItem> GetEnumerator()
		{
			List<TItem> list = new List<TItem>();
			lock (_queue)
			{
				foreach (SimpleNode item in _queue)
				{
					list.Add(item.Data);
				}
			}
			return list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public bool IsValidQueue()
		{
			lock (_queue)
			{
				return _queue.IsValidQueue();
			}
		}
	}
	public class SimplePriorityQueue<TItem> : SimplePriorityQueue<TItem, float>
	{
	}
}
