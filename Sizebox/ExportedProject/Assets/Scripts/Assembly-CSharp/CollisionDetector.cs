using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
	public LayerMask layerSphereCast;

	public bool isColliding { get; private set; }

	private void Start()
	{
		layerSphereCast = LayerMask.GetMask("Map");
		isColliding = false;
	}

	public RaycastHit GetCollision(Vector3 position, float distance)
	{
		RaycastHit hitInfo;
		if (Physics.SphereCast(position + Vector3.up * 850f, 200f, base.transform.forward, out hitInfo, distance, layerSphereCast))
		{
			Debug.Log("Hit");
			Debug.Log("Hit point:" + hitInfo.point);
		}
		return hitInfo;
	}

	private void OnTriggerEnter(Collider mapCollider)
	{
		if (mapCollider.gameObject.layer == Layers.mapLayer || mapCollider.gameObject.layer == Layers.defaultLayer)
		{
			mapCollider.bounds.ClosestPoint(base.transform.position);
			isColliding = true;
		}
	}

	private void OnTriggerExit(Collider mapCollider)
	{
		if (mapCollider.gameObject.layer == Layers.mapLayer || mapCollider.gameObject.layer == Layers.defaultLayer)
		{
			isColliding = false;
		}
	}

	public void DoSphereCast()
	{
		RaycastHit hitInfo;
		if (Physics.SphereCast(base.transform.position + Vector3.up * 850f, 1000f, base.transform.forward, out hitInfo, 2000f, layerSphereCast))
		{
			Debug.Log("Hit");
		}
	}
}
