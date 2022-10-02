using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Serialization;
using UnityEngine.SpatialTracking;
using UnityEngine.XR;

public class XRRig : MonoBehaviour
{
	public struct XrRigRevert
	{
		public Transform parent;

		public Vector3 localPos;

		public Quaternion localQuaterion;
	}

	public struct XrRig
	{
		public GameObject gameObject;

		public Transform head;

		public Transform right;

		public Transform left;

		public Transform floor;

		public XrRigRevert revert;
	}

	[FormerlySerializedAs("camera")]
	public Camera myCamera;

	public Transform xrTransform;

	public PostProcessLayer postProcessLayer;

	private XrRig _xrRig;

	private const string StrXrPrefab = "XR Rig/XR Rig";

	private const string StrHead = "FloorOffset/Head";

	private const string StrLeft = "FloorOffset/Left Hand";

	private const string StrRight = "FloorOffset/Right Hand";

	private const string StrFloor = "FloorOffset";

	private const string StrCameraFallback = "MainCamera";

	private void OnEnable()
	{
		_xrRig = (myCamera ? SetupXrRig(myCamera, xrTransform, postProcessLayer) : SetupXrRig(xrTransform, postProcessLayer));
	}

	private void OnDisable()
	{
		if (!base.enabled)
		{
			if ((bool)myCamera)
			{
				DestroyXrRig(myCamera, _xrRig);
			}
			else
			{
				DestroyXrRig(_xrRig);
			}
		}
	}

	public static XrRig SetupXrRig(Transform transform = null, PostProcessLayer postProcessLayer = null)
	{
		return SetupXrRig(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>(), transform, postProcessLayer);
	}

	private static Vector3 TryGround(Transform transform)
	{
		Vector3 position = transform.position;
		Ray ray = default(Ray);
		ray.origin = position.ToWorld();
		ray.direction = Vector3.down.ToWorld();
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity))
		{
			Debug.DrawLine(hitInfo.transform.position, hitInfo.point);
			return hitInfo.point;
		}
		return position;
	}

	public static XrRig SetupXrRig(Camera camera, Transform transform = null, PostProcessLayer postProcessLayer = null)
	{
		Vector3 position = TryGround(transform ? transform : camera.transform);
		GameObject gameObject = Object.Instantiate(Resources.Load<GameObject>("XR Rig/XR Rig"), transform, false);
		Transform transform2 = camera.transform;
		XrRigRevert xrRigRevert = default(XrRigRevert);
		xrRigRevert.localPos = transform2.localPosition;
		xrRigRevert.localQuaterion = transform2.localRotation;
		xrRigRevert.parent = transform2.parent;
		XrRigRevert revert = xrRigRevert;
		XrRig xrRig = default(XrRig);
		xrRig.gameObject = gameObject;
		xrRig.head = gameObject.transform.Find("FloorOffset/Head");
		xrRig.floor = gameObject.transform.Find("FloorOffset");
		xrRig.left = gameObject.transform.Find("FloorOffset/Left Hand");
		xrRig.right = gameObject.transform.Find("FloorOffset/Right Hand");
		xrRig.revert = revert;
		XrRig result = xrRig;
		camera.gameObject.transform.localPosition.Set(0f, 0f, 0f);
		gameObject.gameObject.transform.position = position;
		camera.gameObject.transform.SetParent(result.floor, false);
		TrackedPoseDriver trackedPoseDriver = camera.gameObject.AddComponent<TrackedPoseDriver>();
		trackedPoseDriver.UseRelativeTransform = XRSettings.loadedDeviceName != "OpenVR";
		trackedPoseDriver.SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRDevice, TrackedPoseDriver.TrackedPose.Head);
		result.right.GetComponent<TrackedPoseDriver>().UseRelativeTransform = XRSettings.loadedDeviceName == "OpenVR";
		result.left.GetComponent<TrackedPoseDriver>().UseRelativeTransform = XRSettings.loadedDeviceName == "OpenVR";
		if (postProcessLayer != null)
		{
			postProcessLayer.finalBlitToCameraTarget = false;
		}
		return result;
	}

	public static void DestroyXrRig(XrRig xrRig)
	{
		DestroyXrRig(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>(), xrRig);
	}

	public static void DestroyXrRig(Camera camera, XrRig xrRig, PostProcessLayer postProcessLayer = null)
	{
		Transform obj = camera.transform;
		obj.SetParent(xrRig.revert.parent, false);
		obj.localPosition = xrRig.revert.localPos;
		obj.localRotation = xrRig.revert.localQuaterion;
		if (postProcessLayer != null)
		{
			postProcessLayer.finalBlitToCameraTarget = true;
		}
		TrackedPoseDriver component = camera.GetComponent<TrackedPoseDriver>();
		if ((bool)component)
		{
			Object.Destroy(component);
		}
		Object.Destroy(xrRig.gameObject);
	}
}
