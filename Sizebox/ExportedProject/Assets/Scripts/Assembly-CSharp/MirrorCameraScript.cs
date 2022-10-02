using UnityEngine;

public class MirrorCameraScript : MonoBehaviour
{
	public GameObject MirrorObject;

	private Renderer mirrorRenderer;

	private Material mirrorMaterial;

	private MirrorScript mirrorScript;

	private Camera cameraObject;

	private RenderTexture reflectionTexture;

	private Matrix4x4 reflectionMatrix;

	private int oldReflectionTextureSize;

	private static bool renderingMirror;

	private void Start()
	{
		mirrorScript = GetComponentInParent<MirrorScript>();
		cameraObject = GetComponent<Camera>();
		if (mirrorScript.AddFlareLayer)
		{
			cameraObject.gameObject.AddComponent<FlareLayer>();
		}
		mirrorRenderer = MirrorObject.GetComponent<Renderer>();
		if (Application.isPlaying)
		{
			mirrorRenderer.sharedMaterial = mirrorRenderer.material;
		}
		mirrorMaterial = mirrorRenderer.sharedMaterial;
		CreateRenderTexture();
	}

	private void CreateRenderTexture()
	{
		if (reflectionTexture == null || oldReflectionTextureSize != mirrorScript.TextureSize)
		{
			if ((bool)reflectionTexture)
			{
				Object.DestroyImmediate(reflectionTexture);
			}
			reflectionTexture = new RenderTexture(mirrorScript.TextureSize, mirrorScript.TextureSize, 16);
			reflectionTexture.filterMode = FilterMode.Bilinear;
			reflectionTexture.antiAliasing = 1;
			reflectionTexture.name = "MirrorRenderTexture_" + GetInstanceID();
			reflectionTexture.hideFlags = HideFlags.HideAndDontSave;
			reflectionTexture.autoGenerateMips = false;
			reflectionTexture.wrapMode = TextureWrapMode.Clamp;
			mirrorMaterial.SetTexture("_MainTex", reflectionTexture);
			oldReflectionTextureSize = mirrorScript.TextureSize;
		}
		if (cameraObject.targetTexture != reflectionTexture)
		{
			cameraObject.targetTexture = reflectionTexture;
		}
	}

	private void Update()
	{
		CreateRenderTexture();
	}

	private void UpdateCameraProperties(Camera src, Camera dest)
	{
		dest.clearFlags = src.clearFlags;
		dest.backgroundColor = src.backgroundColor;
		if (src.clearFlags == CameraClearFlags.Skybox)
		{
			Skybox component = src.GetComponent<Skybox>();
			Skybox component2 = dest.GetComponent<Skybox>();
			if (!component || !component.material)
			{
				component2.enabled = false;
			}
			else
			{
				component2.enabled = true;
				component2.material = component.material;
			}
		}
		dest.orthographic = src.orthographic;
		dest.orthographicSize = src.orthographicSize;
		if (mirrorScript.AspectRatio > 0f)
		{
			dest.aspect = mirrorScript.AspectRatio;
		}
		else
		{
			dest.aspect = src.aspect;
		}
		dest.renderingPath = src.renderingPath;
	}

	internal void RenderMirror()
	{
		Camera current;
		if (renderingMirror || !base.enabled || (current = Camera.current) == null || mirrorRenderer == null || mirrorMaterial == null || !mirrorRenderer.enabled)
		{
			return;
		}
		renderingMirror = true;
		int pixelLightCount = QualitySettings.pixelLightCount;
		if (QualitySettings.pixelLightCount != mirrorScript.MaximumPerPixelLights)
		{
			QualitySettings.pixelLightCount = mirrorScript.MaximumPerPixelLights;
		}
		try
		{
			UpdateCameraProperties(current, cameraObject);
			if (mirrorScript.MirrorRecursion)
			{
				mirrorMaterial.EnableKeyword("MIRROR_RECURSION");
				cameraObject.ResetWorldToCameraMatrix();
				cameraObject.ResetProjectionMatrix();
				cameraObject.projectionMatrix *= Matrix4x4.Scale(new Vector3(-1f, 1f, 1f));
				cameraObject.cullingMask = -17 & mirrorScript.ReflectLayers.value;
				GL.invertCulling = true;
				cameraObject.Render();
				GL.invertCulling = false;
				return;
			}
			mirrorMaterial.DisableKeyword("MIRROR_RECURSION");
			Vector3 pos = base.transform.position;
			Vector3 normal = (mirrorScript.NormalIsForward ? base.transform.forward : base.transform.up);
			float w = 0f - Vector3.Dot(normal, pos) - mirrorScript.ClipPlaneOffset;
			Vector4 plane = new Vector4(normal.x, normal.y, normal.z, w);
			CalculateReflectionMatrix(ref plane);
			Vector3 position = cameraObject.transform.position;
			float farClipPlane = cameraObject.farClipPlane;
			Vector3 position2 = reflectionMatrix.MultiplyPoint(position);
			Matrix4x4 worldToCameraMatrix = current.worldToCameraMatrix * reflectionMatrix;
			cameraObject.worldToCameraMatrix = worldToCameraMatrix;
			Vector4 clipPlane = CameraSpacePlane(ref worldToCameraMatrix, ref pos, ref normal, 1f);
			cameraObject.projectionMatrix = current.CalculateObliqueMatrix(clipPlane);
			GL.invertCulling = true;
			cameraObject.transform.position = position2;
			cameraObject.farClipPlane = mirrorScript.FarClipPlane;
			cameraObject.cullingMask = -17 & mirrorScript.ReflectLayers.value;
			cameraObject.Render();
			cameraObject.transform.position = position;
			cameraObject.farClipPlane = farClipPlane;
			GL.invertCulling = false;
		}
		finally
		{
			renderingMirror = false;
			if (QualitySettings.pixelLightCount != pixelLightCount)
			{
				QualitySettings.pixelLightCount = pixelLightCount;
			}
		}
	}

	private void OnDisable()
	{
		if ((bool)reflectionTexture)
		{
			Object.DestroyImmediate(reflectionTexture);
			reflectionTexture = null;
		}
	}

	private Vector4 CameraSpacePlane(ref Matrix4x4 worldToCameraMatrix, ref Vector3 pos, ref Vector3 normal, float sideSign)
	{
		Vector3 point = pos + normal * mirrorScript.ClipPlaneOffset;
		Vector3 lhs = worldToCameraMatrix.MultiplyPoint(point);
		Vector3 rhs = worldToCameraMatrix.MultiplyVector(normal).normalized * sideSign;
		return new Vector4(rhs.x, rhs.y, rhs.z, 0f - Vector3.Dot(lhs, rhs));
	}

	private void CalculateReflectionMatrix(ref Vector4 plane)
	{
		reflectionMatrix.m00 = 1f - 2f * plane[0] * plane[0];
		reflectionMatrix.m01 = -2f * plane[0] * plane[1];
		reflectionMatrix.m02 = -2f * plane[0] * plane[2];
		reflectionMatrix.m03 = -2f * plane[3] * plane[0];
		reflectionMatrix.m10 = -2f * plane[1] * plane[0];
		reflectionMatrix.m11 = 1f - 2f * plane[1] * plane[1];
		reflectionMatrix.m12 = -2f * plane[1] * plane[2];
		reflectionMatrix.m13 = -2f * plane[3] * plane[1];
		reflectionMatrix.m20 = -2f * plane[2] * plane[0];
		reflectionMatrix.m21 = -2f * plane[2] * plane[1];
		reflectionMatrix.m22 = 1f - 2f * plane[2] * plane[2];
		reflectionMatrix.m23 = -2f * plane[3] * plane[2];
		reflectionMatrix.m30 = 0f;
		reflectionMatrix.m31 = 0f;
		reflectionMatrix.m32 = 0f;
		reflectionMatrix.m33 = 1f;
	}

	private static void CalculateObliqueMatrix(ref Matrix4x4 projection, ref Vector4 clipPlane)
	{
		Vector4 b = projection.inverse * new Vector4(Sign(clipPlane.x), Sign(clipPlane.y), 1f, 1f);
		Vector4 vector = clipPlane * (2f / Vector4.Dot(clipPlane, b));
		projection[2] = vector.x - projection[3];
		projection[6] = vector.y - projection[7];
		projection[10] = vector.z - projection[11];
		projection[14] = vector.w - projection[15];
	}

	private static float Sign(float a)
	{
		if (a > 0f)
		{
			return 1f;
		}
		if (a < 0f)
		{
			return -1f;
		}
		return 0f;
	}
}
