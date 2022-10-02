using UnityEngine;

public class GiantessObstacleDetector : ObstacleDetector
{
	private CollisionDetector collisionDetector;

	private bool debugRays = true;

	private float lookAhead = 400f;

	private bool raycastDetected;

	private float nextCheck;

	private float timeBetweenChecks = 1f;

	private float scale;

	private Vector3 raycastPoint;

	private Vector3 raycastNormal;

	private bool walkingUp;

	private float last_y;

	private float step = 0.05f;

	private Transform myTransform;

	public override void Initialize(EntityBase entity)
	{
		myTransform = entity.transform;
		collisionDetector = new GameObject("Collision Detector").AddComponent<CollisionDetector>();
		collisionDetector.transform.SetParent(myTransform, false);
		collisionDetector.transform.localPosition = Vector3.forward * lookAhead;
		base.initialized = true;
	}

	public override bool CheckObstacle()
	{
		if (walkingUp)
		{
			return false;
		}
		float time = Time.time;
		if (collisionDetector.isColliding)
		{
			return true;
		}
		if (time > nextCheck)
		{
			Vector3 position = myTransform.position;
			walkingUp = position.y > last_y + step * scale;
			last_y = position.y;
			bool walkingUp2 = walkingUp;
			scale = myTransform.localScale.y;
			raycastDetected = CheckForWall();
			nextCheck = time + timeBetweenChecks;
		}
		return raycastDetected;
	}

	public override Vector3 GetPoint()
	{
		if (collisionDetector.isColliding)
		{
			return collisionDetector.transform.position;
		}
		return raycastPoint;
	}

	public override Vector3 GetNormal()
	{
		if (collisionDetector.isColliding)
		{
			return -myTransform.forward;
		}
		return raycastNormal;
	}

	private bool CheckForWall()
	{
		Vector3 vector = myTransform.position + myTransform.up * 200f * scale;
		Vector3 forward = myTransform.forward;
		float num = 1000f * scale;
		if (debugRays)
		{
			Debug.DrawLine(vector, vector + forward * num, Color.white, timeBetweenChecks);
		}
		RaycastHit hitInfo;
		if (Physics.Raycast(vector, forward, out hitInfo, num, Layers.gtsCollisionCheckMask))
		{
			raycastPoint = hitInfo.point;
			raycastNormal = hitInfo.normal;
			if (walkingUp)
			{
				return false;
			}
			return true;
		}
		return false;
	}
}
