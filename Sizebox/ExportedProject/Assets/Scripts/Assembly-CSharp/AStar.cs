using System.Collections.Generic;
using Priority_Queue;

public class AStar
{
	public class PriorityList
	{
		private FastPriorityQueue<NodeRecord> priorityQueue;

		public int count;

		public PriorityList(int max)
		{
			priorityQueue = new FastPriorityQueue<NodeRecord>(max);
		}

		public void Add(NodeRecord record)
		{
			priorityQueue.Enqueue(record, record.estimatedTotalCost);
			count++;
		}

		public NodeRecord GetSmallestElement()
		{
			count--;
			return priorityQueue.Dequeue();
		}
	}

	public List<PathNode> PathfindAStart(WorldNodes nodes, PathNode start, PathNode end)
	{
		NodeRecord nodeRecord = new NodeRecord();
		nodeRecord.node = start;
		nodeRecord.estimatedTotalCost = CalculateEstimatedCost(start, end);
		nodeRecord.heuristic = nodeRecord.estimatedTotalCost;
		NodeRecord nodeRecord2 = nodeRecord;
		PriorityList priorityList = new PriorityList(nodes.maxChecks * 10);
		priorityList.Add(nodeRecord);
		NodeRecord nodeRecord3 = null;
		NodeRecord nodeRecord4 = null;
		NodeRecord[] array = new NodeRecord[8];
		while (nodes.checkCount < nodes.maxChecks && (priorityList.count > 0 || nodeRecord4 != null))
		{
			if (nodeRecord4 != null)
			{
				nodeRecord3 = nodeRecord4;
			}
			else
			{
				nodeRecord3 = priorityList.GetSmallestElement();
				nodeRecord3.node.openRecord = null;
			}
			if (nodeRecord3.node.x == end.x && nodeRecord3.node.z == end.z)
			{
				nodeRecord3.node = end;
				break;
			}
			nodeRecord4 = null;
			PathNode[] connections = nodes.GetConnections(nodeRecord3.node);
			for (int i = 0; i < PathNode.connectionsLength; i++)
			{
				array[i] = null;
				PathNode pathNode = connections[i];
				if (pathNode.visited)
				{
					continue;
				}
				PathNode pathNode2 = pathNode;
				int num = nodeRecord3.costSoFar + pathNode.cost;
				NodeRecord nodeRecord5;
				if (pathNode2.openRecord != null)
				{
					nodeRecord5 = pathNode2.openRecord;
					if (nodeRecord5.costSoFar <= num)
					{
						continue;
					}
				}
				else
				{
					nodeRecord5 = new NodeRecord();
					nodeRecord5.node = pathNode2;
					nodeRecord5.heuristic = CalculateEstimatedCost(pathNode2, end);
				}
				nodeRecord5.costSoFar = num;
				nodeRecord5.previous = nodeRecord3;
				nodeRecord5.estimatedTotalCost = num + nodeRecord5.heuristic;
				if (nodeRecord5.heuristic < nodeRecord2.heuristic)
				{
					nodeRecord2 = nodeRecord5;
				}
				if (nodeRecord4 == null)
				{
					if (nodeRecord5.estimatedTotalCost < nodeRecord3.estimatedTotalCost)
					{
						nodeRecord4 = nodeRecord5;
					}
				}
				else if (nodeRecord5.estimatedTotalCost < nodeRecord4.estimatedTotalCost)
				{
					nodeRecord4 = nodeRecord5;
				}
				if (pathNode2.openRecord == null)
				{
					array[i] = nodeRecord5;
				}
			}
			for (int j = 0; j < PathNode.connectionsLength; j++)
			{
				NodeRecord nodeRecord6 = array[j];
				if (nodeRecord6 != null && nodeRecord6 != nodeRecord4)
				{
					priorityList.Add(nodeRecord6);
					nodeRecord6.node.openRecord = nodeRecord6;
				}
			}
		}
		if (nodeRecord3.node != end)
		{
			nodeRecord3 = nodeRecord2;
		}
		List<PathNode> list = new List<PathNode>();
		while (nodeRecord3.node != start)
		{
			list.Add(nodeRecord3.previous.node);
			nodeRecord3 = nodeRecord3.previous;
		}
		list.Reverse();
		return list;
	}

	public int CalculateEstimatedCost(PathNode thisNode, PathNode goalNode)
	{
		int num = thisNode.x - goalNode.x;
		int num2 = thisNode.z - goalNode.z;
		if (num < 0)
		{
			num = -num;
		}
		if (num2 < 0)
		{
			num2 = -num2;
		}
		int num3;
		int num4;
		if (num > num2)
		{
			num3 = num;
			num4 = num2;
		}
		else
		{
			num3 = num2;
			num4 = num;
		}
		return (num3 * 100 + num4 * 41) * 2;
	}

	private int BetterAproximation(PathNode thisNode, PathNode goalNode)
	{
		int num = thisNode.x - goalNode.x;
		int num2 = thisNode.z - goalNode.z;
		if (num < 0)
		{
			num = -num;
		}
		if (num2 < 0)
		{
			num2 = -num2;
		}
		int num3;
		int num4;
		if (num > num2)
		{
			num3 = num;
			num4 = num2;
		}
		else
		{
			num3 = num2;
			num4 = num;
		}
		int num5 = num3 * 94 + num4 * 41;
		if (num3 < num4 * 16)
		{
			num5 -= num3 * 4;
		}
		return num5 * 2;
	}
}
