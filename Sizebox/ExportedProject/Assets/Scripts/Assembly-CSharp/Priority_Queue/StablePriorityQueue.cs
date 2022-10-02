using System;
using System.Collections;
using System.Collections.Generic;

namespace Priority_Queue
{
	public sealed class StablePriorityQueue<T> : IFixedSizePriorityQueue<T, float>, IPriorityQueue<T, float>, IEnumerable<T>, IEnumerable where T : StablePriorityQueueNode
	{
		private int _numNodes;

		private T[] _nodes;

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

		public T First
		{
			get
			{
				return _nodes[1];
			}
		}

		public StablePriorityQueue(int maxNodes)
		{
			_numNodes = 0;
			_nodes = new T[maxNodes + 1];
			_numNodesEverEnqueued = 0L;
		}

		public void Clear()
		{
			Array.Clear(_nodes, 1, _numNodes);
			_numNodes = 0;
		}

		public bool Contains(T node)
		{
			return _nodes[node.QueueIndex] == node;
		}

		public void Enqueue(T node, float priority)
		{
			node.Priority = priority;
			_numNodes++;
			_nodes[_numNodes] = node;
			node.QueueIndex = _numNodes;
			node.InsertionIndex = _numNodesEverEnqueued++;
			CascadeUp(_nodes[_numNodes]);
		}

		private void Swap(T node1, T node2)
		{
			_nodes[node1.QueueIndex] = node2;
			_nodes[node2.QueueIndex] = node1;
			int queueIndex = node1.QueueIndex;
			node1.QueueIndex = node2.QueueIndex;
			node2.QueueIndex = queueIndex;
		}

		private void CascadeUp(T node)
		{
			int num = node.QueueIndex / 2;
			while (num >= 1)
			{
				T val = _nodes[num];
				if (!HasHigherPriority(val, node))
				{
					Swap(node, val);
					num = node.QueueIndex / 2;
					continue;
				}
				break;
			}
		}

		private void CascadeDown(T node)
		{
			int num = node.QueueIndex;
			while (true)
			{
				T val = node;
				int num2 = 2 * num;
				if (num2 > _numNodes)
				{
					node.QueueIndex = num;
					_nodes[num] = node;
					return;
				}
				T val2 = _nodes[num2];
				if (HasHigherPriority(val2, val))
				{
					val = val2;
				}
				int num3 = num2 + 1;
				if (num3 <= _numNodes)
				{
					T val3 = _nodes[num3];
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

		private bool HasHigherPriority(T higher, T lower)
		{
			if (!(higher.Priority < lower.Priority))
			{
				if (higher.Priority == lower.Priority)
				{
					return higher.InsertionIndex < lower.InsertionIndex;
				}
				return false;
			}
			return true;
		}

		public T Dequeue()
		{
			T val = _nodes[1];
			Remove(val);
			return val;
		}

		public void Resize(int maxNodes)
		{
			T[] array = new T[maxNodes + 1];
			int num = Math.Min(maxNodes, _numNodes);
			for (int i = 1; i <= num; i++)
			{
				array[i] = _nodes[i];
			}
			_nodes = array;
		}

		public void UpdatePriority(T node, float priority)
		{
			node.Priority = priority;
			OnNodeUpdated(node);
		}

		private void OnNodeUpdated(T node)
		{
			int num = node.QueueIndex / 2;
			T lower = _nodes[num];
			if (num > 0 && HasHigherPriority(node, lower))
			{
				CascadeUp(node);
			}
			else
			{
				CascadeDown(node);
			}
		}

		public void Remove(T node)
		{
			if (node.QueueIndex == _numNodes)
			{
				_nodes[_numNodes] = null;
				_numNodes--;
				return;
			}
			T val = _nodes[_numNodes];
			Swap(node, val);
			_nodes[_numNodes] = null;
			_numNodes--;
			OnNodeUpdated(val);
		}

		public IEnumerator<T> GetEnumerator()
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
