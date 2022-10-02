using UnityEngine;

public class MicroObstacleDetector : ObstacleDetector
{
	private Vector3 point;

	private Vector3 normal;

	private float scale;

	private float nextCheck;

	private float timeBetweenChecks = 0.5f;

	private bool obstacleDetected;

	public bool debugRays = true;

	private Transform myTransform;

	private Vector3 position;

	private Vector3 up;

	private Vector3 forward;

	private float wallDistance = 2.5f;

	private bool walkingUp;

	private float last_y;

	private float step = 0.05f;

	public override void Initialize(EntityBase entity)
	{
		myTransform = entity.transform;
		base.initialized = true;
	}

	public override bool CheckObstacle()
	{
		float time = Time.time;
		if (time > nextCheck)
		{
			obstacleDetected = DoRaycasts();
			nextCheck = time + timeBetweenChecks;
		}
		return obstacleDetected;
	}

	public override Vector3 GetPoint()
	{
		return point;
	}

	public override Vector3 GetNormal()
	{
		return normal;
	}

	private bool DoRaycasts()
	{
		scale = myTransform.lossyScale.y;
		position = myTransform.position;
		walkingUp = position.y > last_y + step * scale;
		last_y = position.y;
		up = myTransform.up;
		forward = myTransform.forward;
		if (!CheckForFalling())
		{
			return CheckForWall();
		}
		return true;
	}

	private bool CheckForWall()
	{
		Vector3 vector = position + up * 1.4f * scale;
		Vector3 vector2 = forward;
		float num = wallDistance * scale;
		if (walkingUp)
		{
			num *= 0.3f;
		}
		if (debugRays)
		{
			Debug.DrawLine(vector, vector + vector2 * num, Color.white, timeBetweenChecks);
		}
		RaycastHit hitInfo;
		if (Physics.Raycast(vector, vector2, out hitInfo, num))
		{
			point = hitInfo.point;
			normal = hitInfo.normal;
			return true;
		}
		return false;
	}

	private bool CheckForFalling()
	{
		Vector3 vector = position + (up * 1.6f + forward * 1f) * scale;
		Vector3 vector2 = forward - up * 1.2f;
		float num = 4f * scale;
		if (debugRays)
		{
			Debug.DrawLine(vector, vector + vector2 * num, Color.white, timeBetweenChecks);
		}
		if (Physics.Raycast(vector, vector2, num))
		{
			return false;
		}
		point = vector + vector2 * num;
		normal = -forward;
		return true;
	}
}
