using System;
using UnityEngine;

public class CameraDynamicGround : MonoBehaviour
{
	private const float FailSafe = 2E+09f;

	private Camera followCamera;

	public Collider ground;

	[Range(1f, 1E+09f)]
	public float scaleMin = 1f;

	[Range(1.1f, 2E+09f)]
	public float scaleMax = 2E+09f;

	[Range(1f, 2000f)]
	public float multiply = 20f;

	private void Awake()
	{
		followCamera = Camera.main;
	}

	private void Update()
	{
		Vector3 position = followCamera.transform.position;
		Bounds bounds = ground.bounds;
		base.transform.position = new Vector3(position.x, bounds.max.y, position.z);
		float num = Mathf.Abs(position.y - bounds.max.y);
		float num2 = 2f * num * Mathf.Tan(followCamera.fieldOfView * 0.5f * ((float)Math.PI / 180f)) * 0.5f / Mathf.Tan(followCamera.fieldOfView * 0.5f * ((float)Math.PI / 180f)) * followCamera.aspect * multiply;
		if (num2 < scaleMin)
		{
			num2 = scaleMin;
		}
		else if (num2 > scaleMax || num2 > 2E+09f)
		{
			num2 = scaleMax;
		}
		base.transform.localScale = new Vector3(num2, base.transform.localScale.y, num2);
	}
}
