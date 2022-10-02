using UnityEngine;

[AddComponentMenu("Dynamic Bone/Dynamic Bone Plane Collider")]
public class DynamicBonePlaneCollider : DynamicBoneColliderBase
{
	private void OnValidate()
	{
	}

	public override void Collide(ref Vector3 particlePosition, float particleRadius)
	{
		Vector3 vector = Vector3.up;
		switch (m_Direction)
		{
		case Direction.X:
			vector = base.transform.right;
			break;
		case Direction.Y:
			vector = base.transform.up;
			break;
		case Direction.Z:
			vector = base.transform.forward;
			break;
		}
		Vector3 inPoint = base.transform.TransformPoint(m_Center);
		float distanceToPoint = new Plane(vector, inPoint).GetDistanceToPoint(particlePosition);
		if (m_Bound == Bound.Outside)
		{
			if (distanceToPoint < 0f)
			{
				particlePosition -= vector * distanceToPoint;
			}
		}
		else if (distanceToPoint > 0f)
		{
			particlePosition -= vector * distanceToPoint;
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (base.enabled)
		{
			if (m_Bound == Bound.Outside)
			{
				Gizmos.color = Color.yellow;
			}
			else
			{
				Gizmos.color = Color.magenta;
			}
			Vector3 vector = Vector3.up;
			switch (m_Direction)
			{
			case Direction.X:
				vector = base.transform.right;
				break;
			case Direction.Y:
				vector = base.transform.up;
				break;
			case Direction.Z:
				vector = base.transform.forward;
				break;
			}
			Vector3 vector2 = base.transform.TransformPoint(m_Center);
			Gizmos.DrawLine(vector2, vector2 + vector);
		}
	}
}
