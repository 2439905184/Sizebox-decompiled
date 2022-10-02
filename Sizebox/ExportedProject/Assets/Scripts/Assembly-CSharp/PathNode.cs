using UnityEngine;

public class PathNode
{
	private static int normalCost = 100;

	private static int diagonalCost = 141;

	public static PathNode[] connections;

	public static int connectionsLength;

	public int instanceID;

	public int x;

	public int z;

	public float y;

	public PathNode north;

	public PathNode south;

	public PathNode est;

	public PathNode west;

	public bool visited;

	public bool walkable;

	public Vector3 realPosition;

	public int cost;

	public NodeRecord openRecord;

	public PathNode(int x, int z)
	{
		if (connections == null)
		{
			connections = new PathNode[8];
		}
		this.x = x;
		this.z = z;
	}

	public void ResetValues(int x, int z)
	{
		this.x = x;
		this.z = z;
		north = null;
		south = null;
		est = null;
		west = null;
		visited = false;
		walkable = false;
		openRecord = null;
	}

	public bool CanWalkAllDirections()
	{
		if (north != null && south != null && est != null)
		{
			return west != null;
		}
		return false;
	}

	public PathNode[] GetConnections()
	{
		int num = 0;
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		if (north != null && north.walkable)
		{
			north.cost = normalCost;
			connections[num] = north;
			num++;
			flag = true;
		}
		if (est != null && est.walkable)
		{
			est.cost = normalCost;
			connections[num] = est;
			num++;
			flag4 = true;
			if (flag && north.est != null && est.north != null && north.est.walkable)
			{
				north.est.cost = diagonalCost;
				connections[num] = north.est;
				num++;
			}
		}
		if (south != null && south.walkable)
		{
			south.cost = normalCost;
			connections[num] = south;
			num++;
			flag2 = true;
			if (flag4 && south.est != null && est.south != null && south.est.walkable)
			{
				south.est.cost = diagonalCost;
				connections[num] = south.est;
				num++;
			}
		}
		if (west != null && west.walkable)
		{
			west.cost = normalCost;
			connections[num] = west;
			num++;
			flag3 = true;
			if (flag2 && south.west != null && west.south != null && south.west.walkable)
			{
				south.west.cost = diagonalCost;
				connections[num] = south.west;
				num++;
			}
		}
		if (flag && flag3 && north.west != null && west.north != null && north.west.walkable)
		{
			north.west.cost = diagonalCost;
			connections[num] = north.west;
			num++;
		}
		connectionsLength = num;
		return connections;
	}
}
