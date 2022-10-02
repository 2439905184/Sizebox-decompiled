using UnityEngine;

public class LockLocalPosition : MonoBehaviour
{
	public bool isLocked;

	public Vector3 localPositionTarget;

	public Rigidbody rigid;

	private void Update()
	{
		if (isLocked)
		{
			base.transform.localPosition = localPositionTarget;
		}
	}
}
