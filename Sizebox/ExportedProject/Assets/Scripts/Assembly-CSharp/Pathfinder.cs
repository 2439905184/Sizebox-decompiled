using System.Collections.Generic;
using SteeringBehaviors;
using UnityEngine;

public class Pathfinder
{
	private AStar astar;

	public static Pathfinder instance
	{
		get
		{
			return GameController.Instance.pathfinder;
		}
	}

	public List<Kinematic> PlanRoute(Vector3 realStart, Vector3 realEnd, float height, float tileWidth, float step, float angle)
	{
		List<Kinematic> list = new List<Kinematic>();
		Vector3 vector = CenterOrigin.WorldToVirtual(realStart);
		Vector3 point = CenterOrigin.WorldToVirtual(realEnd);
		WorldNodes worldNodes = new WorldNodes(vector, height, tileWidth, step, angle);
		if (astar == null)
		{
			astar = new AStar();
		}
		worldNodes.UpdateNodes(vector, 50);
		PathNode pathNode = worldNodes.PointToNode(vector);
		if (pathNode == null)
		{
			return list;
		}
		Debug.DrawRay(realStart, Vector3.up * height * 1.5f, Color.yellow, 10f);
		Debug.DrawRay(pathNode.realPosition, Vector3.up * height, Color.green, 10f);
		PathNode pathNode2 = worldNodes.PointToNode(point);
		if (pathNode2 == null)
		{
			return list;
		}
		Debug.DrawRay(pathNode2.realPosition, Vector3.up * height, Color.green, 10f);
		List<PathNode> list2 = astar.PathfindAStart(worldNodes, pathNode, pathNode2);
		PathNode pathNode3 = null;
		foreach (PathNode item in list2)
		{
			list.Add(new VectorKinematic(item.realPosition));
			if (pathNode3 != null)
			{
				worldNodes.DrawLineBetweenNodes(pathNode3, item, Color.blue, 120f);
			}
			pathNode3 = item;
		}
		return list;
	}
}
