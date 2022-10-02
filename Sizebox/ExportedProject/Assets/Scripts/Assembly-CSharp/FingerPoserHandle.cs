using UnityEngine;

public class FingerPoserHandle : MonoBehaviour
{
	private Quaternion initialRot;

	public Vector3 realPosition;

	private void Start()
	{
		initialRot = base.transform.localRotation;
	}

	private void LateUpdate()
	{
		base.transform.localPosition = Vector3.zero;
		realPosition = base.transform.position;
	}

	public void Reset()
	{
		base.transform.localRotation = initialRot;
	}
}
