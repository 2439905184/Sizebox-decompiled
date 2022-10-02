using UnityEngine;

public static class PhysicsHelper
{
	public static bool CapsuleCast(CapsuleCollider capsule, Vector3 direction)
	{
		Vector3 p;
		Vector3 p2;
		float r;
		ProcessCapsule(capsule, out p, out p2, out r);
		return Physics.CapsuleCast(p, p2, r, direction);
	}

	public static bool CapsuleCast(CapsuleCollider capsule, Vector3 direction, float maxDistance)
	{
		Vector3 p;
		Vector3 p2;
		float r;
		ProcessCapsule(capsule, out p, out p2, out r);
		return Physics.CapsuleCast(p, p2, r, direction, maxDistance);
	}

	public static bool CapsuleCast(CapsuleCollider capsule, Vector3 direction, out RaycastHit hitInfo)
	{
		Vector3 p;
		Vector3 p2;
		float r;
		ProcessCapsule(capsule, out p, out p2, out r);
		return Physics.CapsuleCast(p, p2, r, direction, out hitInfo);
	}

	public static bool CapsuleCast(CapsuleCollider capsule, Vector3 direction, float maxDistance, LayerMask layerMask)
	{
		Vector3 p;
		Vector3 p2;
		float r;
		ProcessCapsule(capsule, out p, out p2, out r);
		return Physics.CapsuleCast(p, p2, r, direction, maxDistance, layerMask);
	}

	public static bool CapsuleCast(CapsuleCollider capsule, Vector3 direction, out RaycastHit hitInfo, float maxDistance)
	{
		Vector3 p;
		Vector3 p2;
		float r;
		ProcessCapsule(capsule, out p, out p2, out r);
		return Physics.CapsuleCast(p, p2, r, direction, out hitInfo, maxDistance);
	}

	public static bool CapsuleCast(CapsuleCollider capsule, Vector3 direction, out RaycastHit hitInfo, float maxDistance, LayerMask layerMask)
	{
		Vector3 p;
		Vector3 p2;
		float r;
		ProcessCapsule(capsule, out p, out p2, out r);
		return Physics.CapsuleCast(p, p2, r, direction, out hitInfo, maxDistance, layerMask);
	}

	public static void ProcessCapsule(CapsuleCollider capsule, out Vector3 p1, out Vector3 p2, out float r)
	{
		Vector3 vector = (new Vector3[3]
		{
			Vector3.left,
			Vector3.up,
			Vector3.forward
		})[capsule.direction];
		Transform transform = capsule.transform;
		p1 = transform.position + transform.rotation * (capsule.center + vector * (capsule.height / 2f - capsule.radius - 0.1f)) * transform.lossyScale.y;
		p2 = transform.position + transform.rotation * (capsule.center - vector * (capsule.height / 2f - capsule.radius - 0.1f)) * transform.lossyScale.y;
		r = capsule.radius * transform.lossyScale.y;
	}
}
