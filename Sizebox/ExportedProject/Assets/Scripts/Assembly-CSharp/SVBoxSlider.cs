using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxSlider), typeof(RawImage))]
[ExecuteInEditMode]
public class SVBoxSlider : MonoBehaviour
{
	public ColorPicker picker;

	private BoxSlider slider;

	private RawImage image;

	private ComputeShader compute;

	private int kernelID;

	private RenderTexture renderTexture;

	private int textureWidth = 100;

	private int textureHeight = 100;

	private float lastH = -1f;

	private bool listen = true;

	[SerializeField]
	private bool overrideComputeShader;

	private bool supportsComputeShaders;

	public RectTransform rectTransform
	{
		get
		{
			return base.transform as RectTransform;
		}
	}

	private void Awake()
	{
		slider = GetComponent<BoxSlider>();
		image = GetComponent<RawImage>();
		if (Application.isPlaying)
		{
			supportsComputeShaders = SystemInfo.supportsComputeShaders;
			if (overrideComputeShader)
			{
				supportsComputeShaders = false;
			}
			if (supportsComputeShaders)
			{
				InitializeCompute();
			}
			RegenerateSVTexture();
		}
	}

	private void InitializeCompute()
	{
		if (renderTexture == null)
		{
			renderTexture = new RenderTexture(textureWidth, textureHeight, 0, RenderTextureFormat.RGB111110Float);
			renderTexture.enableRandomWrite = true;
			renderTexture.Create();
		}
		compute = Resources.Load<ComputeShader>("Shaders/Compute/GenerateSVTexture");
		kernelID = compute.FindKernel("CSMain");
		image.texture = renderTexture;
	}

	private void OnEnable()
	{
		if (Application.isPlaying && picker != null)
		{
			slider.onValueChanged.AddListener(SliderChanged);
			picker.onHSVChanged.AddListener(HSVChanged);
		}
	}

	private void OnDisable()
	{
		if (picker != null)
		{
			slider.onValueChanged.RemoveListener(SliderChanged);
			picker.onHSVChanged.RemoveListener(HSVChanged);
		}
	}

	private void OnDestroy()
	{
		if (image.texture != null)
		{
			if (supportsComputeShaders)
			{
				renderTexture.Release();
			}
			else
			{
				Object.DestroyImmediate(image.texture);
			}
		}
	}

	private void SliderChanged(float saturation, float value)
	{
		if (listen)
		{
			picker.AssignColor(ColorValues.Saturation, saturation);
			picker.AssignColor(ColorValues.Value, value);
		}
		listen = true;
	}

	private void HSVChanged(float h, float s, float v)
	{
		if (!lastH.Equals(h))
		{
			lastH = h;
			RegenerateSVTexture();
		}
		if (!s.Equals(slider.normalizedValue))
		{
			listen = false;
			slider.normalizedValue = s;
		}
		if (!v.Equals(slider.normalizedValueY))
		{
			listen = false;
			slider.normalizedValueY = v;
		}
	}

	private void RegenerateSVTexture()
	{
		if (supportsComputeShaders)
		{
			float val = ((picker != null) ? picker.H : 0f);
			compute.SetTexture(kernelID, "Texture", renderTexture);
			compute.SetFloats("TextureSize", textureWidth, textureHeight);
			compute.SetFloat("Hue", val);
			int threadGroupsX = Mathf.CeilToInt((float)textureWidth / 32f);
			int threadGroupsY = Mathf.CeilToInt((float)textureHeight / 32f);
			compute.Dispatch(kernelID, threadGroupsX, threadGroupsY, 1);
			return;
		}
		double h = ((picker != null) ? (picker.H * 360f) : 0f);
		if (image.texture != null)
		{
			Object.DestroyImmediate(image.texture);
		}
		Texture2D texture2D = new Texture2D(textureWidth, textureHeight);
		texture2D.hideFlags = HideFlags.DontSave;
		for (int i = 0; i < textureWidth; i++)
		{
			Color32[] array = new Color32[textureHeight];
			for (int j = 0; j < textureHeight; j++)
			{
				array[j] = HSVUtil.ConvertHsvToRgb(h, (float)i / 100f, (float)j / 100f, 1f);
			}
			texture2D.SetPixels32(i, 0, 1, textureHeight, array);
		}
		texture2D.Apply();
		image.texture = texture2D;
	}
}
