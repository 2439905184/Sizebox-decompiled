using UnityEngine;

public class WorldNodes
{
	private static PathNode[,] nodeArray;

	private static int instanceID;

	private int arrayLength;

	private int vx;

	private int vz;

	private float tileWidth;

	private float height;

	private float step;

	private float angleMutiplier = 0.5f;

	public int checkCount;

	public int maxChecks = 1000;

	private Vector3 down;

	private Vector3 up;

	private LayerMask pathfindMask;

	public WorldNodes(Vector3 virtualStart, float height, float tilewidth, float step, float angle)
	{
		angleMutiplier = angle;
		tileWidth = tilewidth;
		this.height = height;
		this.step = step;
		PathNode pathNode = NodeFromPoint(virtualStart);
		pathNode.walkable = true;
		checkCount = 0;
		down = Vector3.down;
		up = Vector3.up;
		pathfindMask = Layers.pathfindingMask;
		instanceID++;
		int num = maxChecks + 1;
		arrayLength = num * 2;
		if (nodeArray == null)
		{
			nodeArray = new PathNode[arrayLength, arrayLength];
		}
		pathNode.instanceID = instanceID;
		nodeArray[num, num] = pathNode;
		vx = num - pathNode.x;
		vz = num - pathNode.z;
		Debug.DrawRay(pathNode.realPosition, up * height * 2f, Color.red, 10f);
	}

	public void UpdateNodes(Vector3 start, int radius)
	{
	}

	public PathNode[] GetConnections(PathNode node)
	{
		if (checkCount < maxChecks && !node.visited)
		{
			FindNeighboors(node, 2);
			UpdateWalkability(node);
			node.visited = true;
			checkCount++;
		}
		return node.GetConnections();
	}

	private void FindNeighboors(PathNode node, int levels = 0)
	{
		for (int i = node.x - 1; i <= node.x + 1; i++)
		{
			for (int j = node.z - 1; j <= node.z + 1; j++)
			{
				if ((i == node.x && j == node.z) || (i != node.x && j != node.z))
				{
					continue;
				}
				PathNode pathNode = GetNodeIfExists(i, j);
				bool flag = false;
				if (pathNode == null)
				{
					pathNode = SampleNode(i, node.y, j);
				}
				else
				{
					if (pathNode.visited || pathNode.walkable)
					{
						continue;
					}
					if (NodesAreConnected(node, pathNode))
					{
						flag = true;
					}
				}
				if (pathNode == null)
				{
					continue;
				}
				if (!flag && CheckWalkability(node, pathNode))
				{
					flag = true;
					if (node.x != pathNode.x)
					{
						if (node.x > pathNode.x)
						{
							node.est = pathNode;
							pathNode.west = node;
						}
						else
						{
							node.west = pathNode;
							pathNode.est = node;
						}
					}
					else if (node.z > pathNode.z)
					{
						node.north = pathNode;
						pathNode.south = node;
					}
					else
					{
						node.south = pathNode;
						pathNode.north = node;
					}
				}
				if (flag && levels > 1)
				{
					FindNeighboors(pathNode, levels - 1);
				}
			}
		}
	}

	private void UpdateWalkability(PathNode node)
	{
		for (int i = node.x - 1; i <= node.x + 1; i++)
		{
			for (int j = node.z - 1; j <= node.z + 1; j++)
			{
				if (i == node.x && j == node.z)
				{
					continue;
				}
				PathNode nodeIfExists = GetNodeIfExists(i, j);
				if (nodeIfExists != null)
				{
					if (!nodeIfExists.walkable && i != node.x && j != node.z)
					{
						FindNeighboors(nodeIfExists);
					}
					if (nodeIfExists.CanWalkAllDirections())
					{
						nodeIfExists.walkable = true;
					}
				}
			}
		}
	}

	private bool NodesAreConnected(PathNode a, PathNode b)
	{
		if (a.north != b && a.south != b && a.est != b)
		{
			return a.west == b;
		}
		return true;
	}

	public void DrawLineBetweenNodes(PathNode a, PathNode b, Color color, float time = 0f)
	{
	}

	private PathNode GetNode(int x, int z)
	{
		int num = vx + x;
		int num2 = vz + z;
		if (num < 0 || num >= arrayLength || num2 < 0 || num2 >= arrayLength)
		{
			return new PathNode(x, z);
		}
		PathNode pathNode = nodeArray[num, num2];
		if (pathNode == null)
		{
			pathNode = new PathNode(x, z);
			nodeArray[num, num2] = pathNode;
			pathNode.instanceID = instanceID;
		}
		else if (pathNode.instanceID != instanceID)
		{
			pathNode.ResetValues(x, z);
			pathNode.instanceID = instanceID;
			nodeArray[num, num2] = pathNode;
		}
		return pathNode;
	}

	public PathNode GetNodeIfExists(int x, int z)
	{
		int num = vx + x;
		int num2 = vz + z;
		if (num < 0 || num >= arrayLength || num2 < 0 || num2 >= arrayLength)
		{
			return null;
		}
		PathNode pathNode = nodeArray[num, num2];
		if (pathNode != null && pathNode.instanceID == instanceID)
		{
			return pathNode;
		}
		return null;
	}

	private bool CheckWalkability(PathNode currentNode, PathNode previousNode)
	{
		if (previousNode == null)
		{
			return false;
		}
		float num = currentNode.y - previousNode.y;
		if (num < 0f)
		{
			num = 0f - num;
		}
		if (num > tileWidth * angleMutiplier)
		{
			return false;
		}
		Vector3 realPosition = currentNode.realPosition;
		Vector3 realPosition2 = previousNode.realPosition;
		realPosition.y += step;
		realPosition2.y += step;
		Vector3 direction = Substract(realPosition2, realPosition);
		float magnitude = direction.magnitude;
		if (Physics.Raycast(realPosition, direction, magnitude, pathfindMask))
		{
			return false;
		}
		if (Physics.Raycast(realPosition2, Substract(realPosition, realPosition2), magnitude, pathfindMask))
		{
			return false;
		}
		return true;
	}

	private PathNode SampleNode(int x, float y, int z)
	{
		Vector3 origin = CenterOrigin.VirtualToWorld((float)x * tileWidth, y, (float)z * tileWidth);
		float num = (height + tileWidth) * angleMutiplier;
		origin.y += num;
		RaycastHit hitInfo;
		if (Physics.Raycast(origin, down, out hitInfo, num * 2f, pathfindMask))
		{
			Vector3 point = hitInfo.point;
			float y2 = point.y;
			point.y += step;
			if (Physics.Raycast(point, up, height, pathfindMask))
			{
				return null;
			}
			PathNode node = GetNode(x, z);
			node.y = CenterOrigin.WorldToVirtual(0f, y2, 0f).y;
			node.realPosition.x = point.x;
			node.realPosition.y = y2;
			node.realPosition.z = point.z;
			return node;
		}
		return null;
	}

	private Vector3 Substract(Vector3 a, Vector3 b)
	{
		a.x -= b.x;
		a.y -= b.y;
		a.z -= b.z;
		return a;
	}

	private Vector3 Sum(Vector3 a, Vector3 b)
	{
		a.x += b.x;
		a.y += b.y;
		a.z += b.z;
		return a;
	}

	public Vector3 LocalizeCoordinates(PathNode node)
	{
		return CenterOrigin.VirtualToWorld((float)node.x * tileWidth, node.y, (float)node.z * tileWidth);
	}

	public PathNode PointToNode(Vector3 point)
	{
		int x = QuantizeFloat(point.x);
		int z = QuantizeFloat(point.z);
		PathNode pathNode = SampleNode(x, point.y, z);
		if (pathNode != null)
		{
			return pathNode;
		}
		pathNode = GetNode(x, z);
		pathNode.y = point.y;
		pathNode.walkable = true;
		pathNode.realPosition = LocalizeCoordinates(pathNode);
		return pathNode;
	}

	private PathNode NodeFromPoint(Vector3 point)
	{
		int x = QuantizeFloat(point.x);
		int z = QuantizeFloat(point.z);
		PathNode pathNode = new PathNode(x, z);
		pathNode.y = point.y;
		pathNode.walkable = true;
		pathNode.realPosition = LocalizeCoordinates(pathNode);
		return pathNode;
	}

	private int QuantizeFloat(float f)
	{
		float num = f / tileWidth;
		if (num > 0f)
		{
			return (int)(num + 0.5f);
		}
		return (int)(num - 0.5f);
	}
}
