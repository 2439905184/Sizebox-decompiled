using UnityEngine;

public class FaceCamera : MonoBehaviour
{
	private Transform cameraT;

	private void Start()
	{
		cameraT = Camera.main.transform;
	}

	private void Update()
	{
		base.transform.LookAt(cameraT);
	}
}
