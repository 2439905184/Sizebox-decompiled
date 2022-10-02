using UnityEngine;

public static class ShaderLoader
{
	public enum BlendMode
	{
		Opaque = 0,
		Cutout = 1,
		Fade = 2,
		Transparent = 3
	}

	private const string standard = "Standard";

	private const string standardCullOff = "StandardCullOff";

	private const string standard2Sided = "Standard2Sided";

	private const string standardSpecular = "Standard (Specular setup)";

	private const string sbStandard = "SB_Standard";

	private const string sbStandard2Sided = "SB_Standard2Sided";

	private const string sbStandardSpecular = "SB_StandardSpecular";

	private const string sMode = "_Mode";

	private const string sSrcBlend = "_SrcBlend";

	private const string sDstBlend = "_DstBlend";

	private const string sAlphaTestOn = "_ALPHATEST_ON";

	private const string sAlphaBlendOn = "_ALPHABLEND_ON";

	private const string sAlphaPremultipyOn = "_ALPHAPREMULTIPLY_ON";

	private const string sZWrite = "_ZWrite";

	public static Shader GetShader(string shaderName)
	{
		if (shaderName.Contains("Standard"))
		{
			switch (shaderName)
			{
			case "SB_Standard":
				shaderName = "Standard";
				break;
			case "StandardCullOff":
			case "SB_Standard2Sided":
				shaderName = "Standard2Sided";
				break;
			case "SB_StandardSpecular":
				shaderName = "Standard (Specular setup)";
				break;
			}
		}
		Shader shader = Shader.Find(shaderName);
		if (shader != null && shader.isSupported)
		{
			return shader;
		}
		return Shader.Find("Standard");
	}

	public static void SetShaderOnMaterial(Material material, string shaderName, string modelName = null)
	{
		if (!material)
		{
			Debug.LogWarning("No material to set" + (string.IsNullOrWhiteSpace(modelName) ? " on object" : (" on " + modelName)));
			return;
		}
		Shader shader = GetShader(shaderName);
		string name = shader.name;
		material.shader = shader;
		if (name.Contains("Standard"))
		{
			BlendMode blendMode = (BlendMode)material.GetFloat("_Mode");
			SetupMaterialWithBlendMode(material, blendMode);
		}
		else if (name.Contains("MMD"))
		{
			Color color = material.GetColor("_Ambient");
			color *= 1.4f;
			material.SetColor("_Ambient", color);
		}
	}

	public static void SetupMaterialWithBlendMode(Material material, BlendMode blendMode)
	{
		switch (blendMode)
		{
		case BlendMode.Opaque:
			material.SetFloat("_Mode", 0f);
			material.SetInt("_SrcBlend", 1);
			material.SetInt("_DstBlend", 0);
			material.DisableKeyword("_ALPHATEST_ON");
			material.DisableKeyword("_ALPHABLEND_ON");
			material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
			material.SetInt("_ZWrite", 1);
			material.renderQueue = -1;
			break;
		case BlendMode.Cutout:
			material.SetFloat("_Mode", 1f);
			material.SetInt("_SrcBlend", 1);
			material.SetInt("_DstBlend", 0);
			material.EnableKeyword("_ALPHATEST_ON");
			material.DisableKeyword("_ALPHABLEND_ON");
			material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
			material.SetInt("_ZWrite", 1);
			material.renderQueue = 2450;
			break;
		case BlendMode.Fade:
			material.SetFloat("_Mode", 2f);
			material.SetInt("_SrcBlend", 5);
			material.SetInt("_DstBlend", 10);
			material.DisableKeyword("_ALPHATEST_ON");
			material.EnableKeyword("_ALPHABLEND_ON");
			material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
			material.SetInt("_ZWrite", 0);
			material.renderQueue = 3000;
			break;
		case BlendMode.Transparent:
			material.SetFloat("_Mode", 3f);
			material.SetInt("_SrcBlend", 1);
			material.SetInt("_DstBlend", 10);
			material.DisableKeyword("_ALPHATEST_ON");
			material.DisableKeyword("_ALPHABLEND_ON");
			material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
			material.SetInt("_ZWrite", 0);
			material.renderQueue = 3000;
			break;
		}
	}

	public static void ApplyShader(GameObject go, Shader shader = null)
	{
		if (shader == null)
		{
			shader = Shader.Find("Standard");
		}
		Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Material[] sharedMaterials = componentsInChildren[i].sharedMaterials;
			foreach (Material material in sharedMaterials)
			{
				string name = material.name;
				string name2 = material.shader.name;
				material.shader = shader;
				int num = (int)material.GetFloat("_Mode");
				if (name2.Contains("Standard"))
				{
					BlendMode blendMode = (BlendMode)num;
					SetupMaterialWithBlendMode(material, blendMode);
					continue;
				}
				if (name.Contains("eyes") && !name.Contains("_"))
				{
					SetupMaterialWithBlendMode(material, BlendMode.Opaque);
					continue;
				}
				if (name.Contains("eye") || name.Contains("matuge") || name.Contains("hitomi") || (name.Contains("matsuge") | name.Contains("shadow")) || name.Contains("shadou") || name.Contains("scar") || name.Contains("kage") || name.Contains("hairaito") || name.Contains("kizu") || name.Contains("mune2"))
				{
					SetupMaterialWithBlendMode(material, BlendMode.Fade);
					continue;
				}
				if (name.Contains("Lens") || name.Contains("renzu"))
				{
					SetupMaterialWithBlendMode(material, BlendMode.Transparent);
					continue;
				}
				if (name.Contains("Face"))
				{
					SetupMaterialWithBlendMode(material, BlendMode.Cutout);
					continue;
				}
				BlendMode blendMode2 = (BlendMode)num;
				bool flag = false;
				if (material.color.a == 0f)
				{
					flag = true;
				}
				else if (blendMode2 == BlendMode.Opaque && material.mainTexture != null)
				{
					flag = TextureLoader.IsTransparent(material.mainTexture);
				}
				if (flag)
				{
					SetupMaterialWithBlendMode(material, BlendMode.Cutout);
				}
				else
				{
					SetupMaterialWithBlendMode(material, blendMode2);
				}
			}
		}
	}

	public static void EnableMaterialMorph(GameObject go)
	{
		Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Material[] sharedMaterials = componentsInChildren[i].sharedMaterials;
			foreach (Material material in sharedMaterials)
			{
				if (material.shader.name.Contains("Standard") && (int)material.GetFloat("_Mode") == 0 && (double)material.color.a > 0.5 && !TextureLoader.IsTransparent(material.mainTexture))
				{
					SetupMaterialWithBlendMode(material, BlendMode.Cutout);
				}
			}
		}
	}
}
