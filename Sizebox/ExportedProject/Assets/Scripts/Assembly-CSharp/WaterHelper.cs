using UnityEngine;

public class WaterHelper : MonoBehaviour
{
	private const string cEnableReflections = "_EnableReflections";

	private const string cUnderwaterMode = "_UnderwaterMode";

	private AQUAS_Reflection aquasReflection;

	private Material mat;

	private bool reflexionsEnabled = true;

	private bool neverUseReflections;

	private bool underWater;

	private Transform cam;

	private Transform waterPlane;

	private float maxScale = 80f;

	private void Start()
	{
		mat = GetComponent<MeshRenderer>().material;
		cam = Camera.main.transform.parent;
		waterPlane = base.transform;
		aquasReflection = GetComponent<AQUAS_Reflection>();
		UpdateGraphicSettings();
	}

	private void Update()
	{
		if (!neverUseReflections)
		{
			float y = cam.localScale.y;
			if (y > maxScale && reflexionsEnabled)
			{
				SetRelections(false);
				reflexionsEnabled = false;
			}
			else if (y < maxScale && !reflexionsEnabled)
			{
				SetRelections(true);
				reflexionsEnabled = true;
			}
		}
		float y2 = cam.position.y;
		float y3 = waterPlane.position.y;
		if (y2 > y3 && underWater)
		{
			mat.SetFloat("_UnderwaterMode", 0f);
			underWater = false;
		}
		else if (y2 < y3 && !underWater)
		{
			mat.SetFloat("_UnderwaterMode", 0.6f);
			underWater = true;
		}
	}

	public void UpdateGraphicSettings()
	{
		aquasReflection.m_ReflectLayers = Layers.reflectionMask;
		switch (QualitySettings.GetQualityLevel())
		{
		case 1:
			SetTextureSize(128);
			return;
		case 2:
		case 3:
			SetTextureSize(256);
			return;
		case 4:
			SetTextureSize(512);
			return;
		case 5:
			SetTextureSize(1024);
			return;
		}
		neverUseReflections = true;
		SetRelections(false);
		SetTextureSize(128);
	}

	private void SetRelections(bool enabled)
	{
		if (enabled)
		{
			mat.SetFloat("_EnableReflections", 0.6f);
		}
		else
		{
			mat.SetFloat("_EnableReflections", 0f);
		}
	}

	private void SetTextureSize(int size)
	{
		aquasReflection.m_TextureSize = size;
	}
}
