using Priority_Queue;

public class NodeRecord : FastPriorityQueueNode
{
	public PathNode node;

	public NodeRecord previous;

	public int costSoFar;

	public int estimatedTotalCost;

	public int heuristic;
}
