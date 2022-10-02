using System;
using System.Collections;
using System.Collections.Generic;

namespace Priority_Queue
{
	public sealed class GenericPriorityQueue<TItem, TPriority> : IFixedSizePriorityQueue<TItem, TPriority>, IPriorityQueue<TItem, TPriority>, IEnumerable<TItem>, IEnumerable where TItem : GenericPriorityQueueNode<TPriority> where TPriority : IComparable<TPriority>
	{
		private int _numNodes;

		private TItem[] _nodes;

		private long _numNodesEverEnqueued;

		public int Count
		{
			get
			{
				return _numNodes;
			}
		}

		public int MaxSize
		{
			get
			{
				return _nodes.Length - 1;
			}
		}

		public TItem First
		{
			get
			{
				return _nodes[1];
			}
		}

		public GenericPriorityQueue(int maxNodes)
		{
			_numNodes = 0;
			_nodes = new TItem[maxNodes + 1];
			_numNodesEverEnqueued = 0L;
		}

		public void Clear()
		{
			Array.Clear(_nodes, 1, _numNodes);
			_numNodes = 0;
		}

		public bool Contains(TItem node)
		{
			return _nodes[node.QueueIndex] == node;
		}

		public void Enqueue(TItem node, TPriority priority)
		{
			node.Priority = priority;
			_numNodes++;
			_nodes[_numNodes] = node;
			node.QueueIndex = _numNodes;
			node.InsertionIndex = _numNodesEverEnqueued++;
			CascadeUp(_nodes[_numNodes]);
		}

		private void Swap(TItem node1, TItem node2)
		{
			_nodes[node1.QueueIndex] = node2;
			_nodes[node2.QueueIndex] = node1;
			int queueIndex = node1.QueueIndex;
			node1.QueueIndex = node2.QueueIndex;
			node2.QueueIndex = queueIndex;
		}

		private void CascadeUp(TItem node)
		{
			int num = node.QueueIndex / 2;
			while (num >= 1)
			{
				TItem val = _nodes[num];
				if (!HasHigherPriority(val, node))
				{
					Swap(node, val);
					num = node.QueueIndex / 2;
					continue;
				}
				break;
			}
		}

		private void CascadeDown(TItem node)
		{
			int num = node.QueueIndex;
			while (true)
			{
				TItem val = node;
				int num2 = 2 * num;
				if (num2 > _numNodes)
				{
					node.QueueIndex = num;
					_nodes[num] = node;
					return;
				}
				TItem val2 = _nodes[num2];
				if (HasHigherPriority(val2, val))
				{
					val = val2;
				}
				int num3 = num2 + 1;
				if (num3 <= _numNodes)
				{
					TItem val3 = _nodes[num3];
					if (HasHigherPriority(val3, val))
					{
						val = val3;
					}
				}
				if (val == node)
				{
					break;
				}
				_nodes[num] = val;
				int queueIndex = val.QueueIndex;
				val.QueueIndex = num;
				num = queueIndex;
			}
			node.QueueIndex = num;
			_nodes[num] = node;
		}

		private bool HasHigherPriority(TItem higher, TItem lower)
		{
			int num = higher.Priority.CompareTo(lower.Priority);
			if (num >= 0)
			{
				if (num == 0)
				{
					return higher.InsertionIndex < lower.InsertionIndex;
				}
				return false;
			}
			return true;
		}

		public TItem Dequeue()
		{
			TItem val = _nodes[1];
			Remove(val);
			return val;
		}

		public void Resize(int maxNodes)
		{
			TItem[] array = new TItem[maxNodes + 1];
			int num = Math.Min(maxNodes, _numNodes);
			for (int i = 1; i <= num; i++)
			{
				array[i] = _nodes[i];
			}
			_nodes = array;
		}

		public void UpdatePriority(TItem node, TPriority priority)
		{
			node.Priority = priority;
			OnNodeUpdated(node);
		}

		private void OnNodeUpdated(TItem node)
		{
			int num = node.QueueIndex / 2;
			TItem lower = _nodes[num];
			if (num > 0 && HasHigherPriority(node, lower))
			{
				CascadeUp(node);
			}
			else
			{
				CascadeDown(node);
			}
		}

		public void Remove(TItem node)
		{
			if (node.QueueIndex == _numNodes)
			{
				_nodes[_numNodes] = null;
				_numNodes--;
				return;
			}
			TItem val = _nodes[_numNodes];
			Swap(node, val);
			_nodes[_numNodes] = null;
			_numNodes--;
			OnNodeUpdated(val);
		}

		public IEnumerator<TItem> GetEnumerator()
		{
			for (int i = 1; i <= _numNodes; i++)
			{
				yield return _nodes[i];
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public bool IsValidQueue()
		{
			for (int i = 1; i < _nodes.Length; i++)
			{
				if (_nodes[i] != null)
				{
					int num = 2 * i;
					if (num < _nodes.Length && _nodes[num] != null && HasHigherPriority(_nodes[num], _nodes[i]))
					{
						return false;
					}
					int num2 = num + 1;
					if (num2 < _nodes.Length && _nodes[num2] != null && HasHigherPriority(_nodes[num2], _nodes[i]))
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}
