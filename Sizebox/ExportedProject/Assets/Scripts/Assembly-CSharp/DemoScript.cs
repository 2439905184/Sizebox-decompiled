using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoScript : MonoBehaviour
{
	private enum RotationAxes
	{
		MouseXAndY = 0,
		MouseX = 1,
		MouseY = 2
	}

	public List<GameObject> Mirrors;

	public GameObject LightBulb;

	public Toggle RecursionToggle;

	private float rotationModifier = -1f;

	private float moveModifier = 1f;

	private Material lightBulbMaterial;

	private RotationAxes axes;

	private float sensitivityX = 15f;

	private float sensitivityY = 15f;

	private float minimumX = -360f;

	private float maximumX = 360f;

	private float minimumY = -60f;

	private float maximumY = 60f;

	private float rotationX;

	private float rotationY;

	private Quaternion originalRotation;

	private void Start()
	{
		originalRotation = base.transform.localRotation;
		Renderer component = LightBulb.GetComponent<Renderer>();
		if (Application.isPlaying)
		{
			component.sharedMaterial = component.material;
		}
		lightBulbMaterial = component.sharedMaterial;
	}

	private void Update()
	{
		RotateMirror();
		MoveLightBulb();
		UpdateMouseLook();
		UpdateMovement();
	}

	public void MirrorRecursionToggled()
	{
		ChangeMirrorRecursion();
	}

	public void ChangeMirrorRecursion()
	{
		foreach (GameObject mirror in Mirrors)
		{
			mirror.GetComponent<MirrorScript>().MirrorRecursion = RecursionToggle.isOn;
		}
	}

	private void UpdateMovement()
	{
		float num = 4f * Time.deltaTime;
		if (Input.GetKey(KeyCode.W))
		{
			Camera.main.transform.Translate(0f, 0f, num);
		}
		else if (Input.GetKey(KeyCode.S))
		{
			Camera.main.transform.Translate(0f, 0f, 0f - num);
		}
		if (Input.GetKey(KeyCode.A))
		{
			Camera.main.transform.Translate(0f - num, 0f, 0f);
		}
		else if (Input.GetKey(KeyCode.D))
		{
			Camera.main.transform.Translate(num, 0f, 0f);
		}
		if (Input.GetKeyDown(KeyCode.M))
		{
			RecursionToggle.isOn = !RecursionToggle.isOn;
		}
	}

	private void RotateMirror()
	{
		GameObject gameObject = Mirrors[0];
		float y = gameObject.transform.rotation.eulerAngles.y;
		if (y > 65f && y < 100f)
		{
			rotationModifier = 0f - rotationModifier;
			y -= 65f;
			gameObject.transform.Rotate(0f, 0f - y, 0f);
		}
		else if (y > 100f && y < 295f)
		{
			rotationModifier = 0f - rotationModifier;
			y = 295f - y;
			gameObject.transform.Rotate(0f, y, 0f);
		}
		else
		{
			gameObject.transform.Rotate(0f, rotationModifier * Time.deltaTime * 20f, 0f);
		}
	}

	private void MoveLightBulb()
	{
		float x = LightBulb.transform.position.x;
		if (x > 5f)
		{
			moveModifier = 0f - moveModifier;
			x = 5f;
		}
		else if (x < -5f)
		{
			moveModifier = 0f - moveModifier;
			x = -5f;
		}
		else
		{
			x += Time.deltaTime * moveModifier;
		}
		Light component = LightBulb.GetComponent<Light>();
		LightBulb.transform.position = new Vector3(x, LightBulb.transform.position.y, LightBulb.transform.position.z);
		float num = Mathf.Min(1f, component.intensity);
		lightBulbMaterial.SetColor("_EmissionColor", new Color(num, num, num));
	}

	private void UpdateMouseLook()
	{
		if (axes == RotationAxes.MouseXAndY)
		{
			rotationX += Input.GetAxis("Mouse X") * sensitivityX;
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationX = ClampAngle(rotationX, minimumX, maximumX);
			rotationY = ClampAngle(rotationY, minimumY, maximumY);
			Quaternion quaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
			Quaternion quaternion2 = Quaternion.AngleAxis(rotationY, -Vector3.right);
			base.transform.localRotation = originalRotation * quaternion * quaternion2;
		}
		else if (axes == RotationAxes.MouseX)
		{
			rotationX += Input.GetAxis("Mouse X") * sensitivityX;
			rotationX = ClampAngle(rotationX, minimumX, maximumX);
			Quaternion quaternion3 = Quaternion.AngleAxis(rotationX, Vector3.up);
			base.transform.localRotation = originalRotation * quaternion3;
		}
		else
		{
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = ClampAngle(rotationY, minimumY, maximumY);
			Quaternion quaternion4 = Quaternion.AngleAxis(0f - rotationY, Vector3.right);
			base.transform.localRotation = originalRotation * quaternion4;
		}
	}

	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}
}
