using UnityEngine;
using UnityEngine.UI;

public class RaygunReticule : MonoBehaviour
{
	private Texture2D crosshairTexture;

	private GameObject crosshair;

	private Image crosshairImage;

	private GameObject polarityBar;

	private Image polarityBarImage;

	private GameObject firingModeBar;

	private Image firingModeBarImage;

	private Color auxiliaryUIColor;

	private float fadeTimer = 2f;

	private bool isFiringModeFaded;

	private bool isPolarityBarFaded;

	private bool shouldFadeUI;

	private float polarityVal;

	private int firingModeVal;

	private bool isHidden;

	private void Start()
	{
		polarityVal = 0.5f;
		firingModeVal = 0;
		crosshair = base.transform.GetChild(0).gameObject;
		crosshairImage = crosshair.GetComponent<Image>();
		SetupUI();
	}

	public void SetupUI()
	{
		SetupCrosshair();
		SetupPolarityBar();
		SetupFiringModeBar();
		auxiliaryUIColor = new Color(GlobalPreferences.AuxiliaryUIColorR.value, GlobalPreferences.AuxiliaryUIColorG.value, GlobalPreferences.AuxiliaryUIColorB.value);
		ChangeAuxiliaryColor(auxiliaryUIColor);
		shouldFadeUI = GlobalPreferences.AuxiliaryFade.value;
	}

	private void SetupCrosshair()
	{
		string crosshairImageName = GetCrosshairImageName();
		crosshairTexture = Object.Instantiate(Resources.Load<Texture2D>("Raygun/UI/" + crosshairImageName));
		Color[] pixels = crosshairTexture.GetPixels();
		Color color = new Color(GlobalPreferences.CrossHairColorR.value, GlobalPreferences.CrossHairColorG.value, GlobalPreferences.CrossHairColorB.value);
		Color color2 = Color.clear;
		int num = 1;
		switch (GlobalPreferences.CrossHairOutline.value)
		{
		case 0:
			color2 = Color.clear;
			num = 0;
			break;
		case 1:
			color2 = Color.black;
			break;
		case 2:
			color2 = Color.white;
			break;
		case 3:
			color2 = new Color(1f - color.r, 1f - color.g, 1f - color.b, 1f);
			break;
		case 4:
			color2 = color * 0.6f;
			break;
		case 5:
			color2 = color * 1.4f;
			break;
		}
		for (int i = 0; i < pixels.Length; i++)
		{
			if (pixels[i].a > 0f)
			{
				if (pixels[i].r != 0f)
				{
					pixels[i] = new Color(color.r, color.g, color.b, pixels[i].a);
				}
				else
				{
					pixels[i] = new Color(color2.r, color2.g, color2.b, pixels[i].a * (float)num);
				}
			}
		}
		crosshairTexture.SetPixels(pixels);
		crosshairTexture.Apply();
		crosshairImage.sprite = Sprite.Create(crosshairTexture, new Rect(0f, 0f, crosshairTexture.width, crosshairTexture.height), new Vector2(0.5f, 0.5f), 100f);
		crosshair.transform.localScale = Vector3.one * GlobalPreferences.UiCrossHairScale.value;
	}

	private string GetCrosshairImageName()
	{
		switch (GlobalPreferences.CrossHairImage.value)
		{
		case 0:
			return "reticule_0";
		case 1:
			return "reticule_1";
		case 2:
			return "reticule_2";
		case 3:
			return "reticule_3";
		case 4:
			return "reticule_4";
		case 5:
			return "reticule_5";
		case 6:
			return "scifi_reticule_1";
		case 7:
			return "scifi_reticule_2";
		case 8:
			return "arc_reticule_1";
		case 9:
			return "arc_reticule_2";
		default:
			return "reticule_0";
		}
	}

	private void SetupPolarityBar()
	{
		if (polarityBar != null)
		{
			Object.Destroy(polarityBar);
		}
		float value = GlobalPreferences.UIAuxiliaryScale.value;
		switch (GlobalPreferences.PolarityBarLocation.value)
		{
		case 0:
			polarityBar = Object.Instantiate(Resources.Load<GameObject>("Raygun/UI/polarity_bar_center"));
			polarityBar.transform.SetParent(base.gameObject.transform);
			polarityBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -30f * value);
			break;
		case 1:
		{
			polarityBar = Object.Instantiate(Resources.Load<GameObject>("Raygun/UI/polarity_bar_corner"));
			polarityBar.transform.SetParent(base.gameObject.transform);
			RectTransform component2 = polarityBar.GetComponent<RectTransform>();
			component2.anchorMin = new Vector2(1f, 0f);
			component2.anchorMax = new Vector2(1f, 0f);
			component2.anchoredPosition = new Vector2(-100f * value, 100f * value);
			break;
		}
		case 2:
		{
			polarityBar = Object.Instantiate(Resources.Load<GameObject>("Raygun/UI/polarity_bar_corner"));
			polarityBar.transform.SetParent(base.gameObject.transform);
			RectTransform component = polarityBar.GetComponent<RectTransform>();
			component.anchorMin = new Vector2(0f, 0f);
			component.anchorMax = new Vector2(0f, 0f);
			component.anchoredPosition = new Vector2(100f * value, 100f * value);
			break;
		}
		}
		polarityBar.transform.localScale = Vector3.one * GlobalPreferences.UIAuxiliaryScale.value;
		polarityBarImage = polarityBar.GetComponent<Image>();
		polarityBarImage.material.SetFloat("_magnitude_value", polarityVal);
		polarityBarImage.material.SetFloat("_alpha", 1f);
	}

	private void SetupFiringModeBar()
	{
		if (firingModeBar != null)
		{
			Object.Destroy(firingModeBar);
		}
		float value = GlobalPreferences.UIAuxiliaryScale.value;
		switch (GlobalPreferences.FiringModeBarLocation.value)
		{
		case 0:
		{
			firingModeBar = Object.Instantiate(Resources.Load<GameObject>("Raygun/UI/firing_mode_bar"));
			firingModeBar.transform.SetParent(base.gameObject.transform);
			RectTransform component4 = firingModeBar.GetComponent<RectTransform>();
			component4.anchorMin = new Vector2(0.5f, 0.5f);
			component4.anchorMax = new Vector2(0.5f, 0.5f);
			component4.anchoredPosition = new Vector2(0f, 70f * value);
			break;
		}
		case 1:
		{
			firingModeBar = Object.Instantiate(Resources.Load<GameObject>("Raygun/UI/firing_mode_bar"));
			firingModeBar.transform.SetParent(base.gameObject.transform);
			RectTransform component3 = firingModeBar.GetComponent<RectTransform>();
			component3.anchorMin = new Vector2(0.5f, 0.5f);
			component3.anchorMax = new Vector2(0.5f, 0.5f);
			component3.anchoredPosition = new Vector2(0f, (float)((GlobalPreferences.PolarityBarLocation.value == 0) ? (-130) : (-80)) * value);
			break;
		}
		case 2:
		{
			firingModeBar = Object.Instantiate(Resources.Load<GameObject>("Raygun/UI/firing_mode_bar"));
			firingModeBar.transform.SetParent(base.gameObject.transform);
			RectTransform component2 = firingModeBar.GetComponent<RectTransform>();
			component2.anchorMin = new Vector2(1f, 0f);
			component2.anchorMax = new Vector2(1f, 0f);
			component2.anchoredPosition = new Vector2(-100f * value, 30f * value);
			break;
		}
		case 3:
		{
			firingModeBar = Object.Instantiate(Resources.Load<GameObject>("Raygun/UI/firing_mode_bar"));
			firingModeBar.transform.SetParent(base.gameObject.transform);
			RectTransform component = firingModeBar.GetComponent<RectTransform>();
			component.anchorMin = new Vector2(0f, 0f);
			component.anchorMax = new Vector2(0f, 0f);
			component.anchoredPosition = new Vector2(100f * value, 30f * value);
			break;
		}
		}
		firingModeBar.transform.localScale = Vector3.one * GlobalPreferences.UIAuxiliaryScale.value;
		firingModeBarImage = firingModeBar.GetComponent<Image>();
		firingModeBarImage.material.SetFloat("_mode", firingModeVal);
		firingModeBarImage.material.SetFloat("_alpha", 1f);
	}

	private void Update()
	{
		if (GameController.Instance.paused && !isHidden)
		{
			HideUI();
		}
		else if (!GameController.Instance.paused && isHidden)
		{
			UnhideUI();
		}
		if (shouldFadeUI)
		{
			UpdateFade();
		}
	}

	private void HideUI()
	{
		GetComponent<Canvas>().enabled = false;
		isHidden = true;
	}

	private void UnhideUI()
	{
		GetComponent<Canvas>().enabled = true;
		isHidden = false;
	}

	private void UpdateFade()
	{
		if (fadeTimer < 0f)
		{
			isFiringModeFaded = FadeFiringModeBar();
			isPolarityBarFaded = FadePolarityBar();
			shouldFadeUI = !isFiringModeFaded || !isPolarityBarFaded;
		}
		else
		{
			fadeTimer -= Time.deltaTime;
		}
	}

	public void ResetFade()
	{
		shouldFadeUI = GlobalPreferences.AuxiliaryFade.value;
		fadeTimer = GlobalPreferences.AuxiliaryFadeDelay.value;
		if (!isFiringModeFaded || !shouldFadeUI)
		{
			UnfadeFiringModeBar();
		}
		if (!isPolarityBarFaded || !shouldFadeUI)
		{
			UnfadePolarityBar();
		}
	}

	public void ChangeAuxiliaryColor(Color color)
	{
		Material material = polarityBarImage.material;
		material.SetColor("_UI_Color", color);
		polarityBarImage.material = material;
		material = firingModeBarImage.material;
		material.SetColor("_UI_Color", color);
		firingModeBarImage.material = material;
	}

	public void ChangePolarityValue(float val)
	{
		polarityVal = val;
		polarityBarImage.material.SetFloat("_magnitude_value", val);
		isPolarityBarFaded = false;
		ResetFade();
	}

	public void ChangeFiringMode(int val)
	{
		firingModeVal = val;
		firingModeBarImage.material.SetFloat("_mode", val);
		isFiringModeFaded = false;
		ResetFade();
	}

	private bool FadePolarityBar()
	{
		float @float = polarityBarImage.material.GetFloat("_alpha");
		if (@float == 0f)
		{
			isPolarityBarFaded = true;
			return true;
		}
		@float = Mathf.Clamp01(@float - Time.deltaTime);
		polarityBarImage.material.SetFloat("_alpha", @float);
		return false;
	}

	public void UnfadePolarityBar()
	{
		polarityBarImage.material.SetFloat("_alpha", 1f);
	}

	private bool FadeFiringModeBar()
	{
		float @float = firingModeBarImage.material.GetFloat("_alpha");
		if (@float == 0f)
		{
			isFiringModeFaded = true;
			return true;
		}
		@float = Mathf.Clamp01(@float - Time.deltaTime);
		firingModeBarImage.material.SetFloat("_alpha", @float);
		return false;
	}

	public void UnfadeFiringModeBar()
	{
		firingModeBarImage.material.SetFloat("_alpha", 1f);
	}
}
