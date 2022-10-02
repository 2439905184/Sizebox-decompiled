using UnityEngine;
using UnityEngine.Serialization;

public class WorldSpaceCanvas : MonoBehaviour
{
	[FormerlySerializedAs("camera")]
	public Camera myCamera;

	public Canvas canvas;

	private Camera uicam;

	private const float distance = 5f;

	private const float scale = 0.012f;

	public static void calculateRotation(Transform RotationTarget, Transform RotationSource)
	{
		RotationTarget.LookAt(RotationSource);
		RotationTarget.Rotate(0f, 180f, 0f);
	}

	public static void calculatePosition(Transform PositionTarget, Transform PositionSource, float distance)
	{
		PositionTarget.position = PositionSource.transform.position + PositionSource.transform.forward * distance;
	}

	public static void calculateScale(Transform ScaleTarget, float scale)
	{
		ScaleTarget.transform.localScale = new Vector3(scale, scale, scale);
	}

	public static void RepositionCanvas(Transform referenceTransform, Transform canvasTransform)
	{
		calculatePosition(canvasTransform, referenceTransform, 5f);
		calculateRotation(canvasTransform, referenceTransform);
		calculateScale(canvasTransform, 0.012f);
		Canvas.ForceUpdateCanvases();
	}

	public void RepositionCanvas()
	{
		RepositionCanvas(myCamera.transform, canvas.transform);
	}

	public static Camera setupUICamera(Camera kamera)
	{
		GameObject obj = new GameObject("UICamera");
		obj.transform.parent = kamera.transform.parent;
		Camera camera = obj.AddComponent<Camera>();
		camera.CopyFrom(kamera);
		camera.cullingMask = LayerMask.GetMask("UI");
		camera.depth = 100f;
		return camera;
	}

	private bool SetupClass()
	{
		if (!canvas)
		{
			canvas = GetComponent<Canvas>();
		}
		if (!myCamera)
		{
			myCamera = Camera.main;
		}
		if (!myCamera || !canvas)
		{
			Debug.LogWarning("Cannot find essenical components for worldspace canvas");
			return false;
		}
		return true;
	}

	public static void convertToScreenSpace(Camera referenceCamera, Canvas canvas, Camera UICamera)
	{
		if ((bool)UICamera)
		{
			Object.Destroy(UICamera.gameObject);
		}
		if ((bool)referenceCamera)
		{
			referenceCamera.cullingMask |= 1 << LayerMask.NameToLayer("UI");
		}
		if ((bool)canvas)
		{
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		}
	}

	public static void convertToWorldSpace(Camera referenceCamera, Canvas canvas, out Camera UICamera)
	{
		canvas.renderMode = RenderMode.WorldSpace;
		canvas.worldCamera = referenceCamera;
		Canvas.ForceUpdateCanvases();
		RepositionCanvas(referenceCamera.transform, canvas.transform);
		UICamera = setupUICamera(referenceCamera);
		referenceCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));
	}

	private void OnEnable()
	{
		if (SetupClass())
		{
			convertToWorldSpace(myCamera, canvas, out uicam);
		}
	}

	private void OnDisable()
	{
		convertToScreenSpace(myCamera, canvas, uicam);
	}
}
