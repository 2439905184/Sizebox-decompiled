using System.Collections.Generic;
using UnityEngine;

namespace Sizebox.CharacterEditor
{
	public class MaterialWrapper
	{
		private static readonly string[] ShaderFloats = new string[10] { "_Mode", "_Cutoff", "_Metallic", "_Glossiness", "_AmbientRate", "_ShadowLum", "_FurLength", "_FurThinness", "_FurDensity", "_FurShading" };

		private static readonly string[] ShaderColors = new string[4] { "_Color", "_Ambient", "_Specular", "_EmissionColor" };

		private static readonly string[] ShaderTextures = new string[3] { "_DetailNormalMap", "_DetailAlbedoMap", "_FurTex" };

		private const int RenderQueueGeometry = 2000;

		private const int RenderQueueCutout = 2450;

		private const int RenderQueueTransparent = 3000;

		public const string NoTexture = "SB_NO_TEX";

		private const string EmissionKeyword = "_EMISSION";

		private const string Mode = "_Mode";

		private const string CutoutKeyword = "_ALPHATEST_ON";

		private const string FadeKeyword = "_ALPHABLEND_ON";

		private const string TransparentKeyword = "_ALPHAPREMULTIPLY_ON";

		private const string NormalMapKeyword = "_NORMALMAP";

		private const string DetailMapKeyword = "_DETAIL_MULX2";

		private const string HideShader = "Sizebox/Hide";

		private readonly MaterialControlGui _controls;

		private readonly Dictionary<string, Texture> _originalTextureMap;

		private bool _removeTexture;

		private bool _useEmission;

		private float _renderMode;

		private static readonly int ModeID = Shader.PropertyToID("_Mode");

		public string Id { get; private set; }

		public Material Material { get; private set; }

		public Material DefaultMaterial { get; private set; }

		private MaterialData DefaultData { get; set; }

		public Shader Shader
		{
			get
			{
				if ((bool)Material)
				{
					return Material.shader;
				}
				return null;
			}
			set
			{
				Material.shader = value;
				if (Material.HasProperty("_Mode"))
				{
					SetRenderMode(Material.GetFloat(ModeID));
				}
			}
		}

		public bool RemoveTexture
		{
			get
			{
				return _removeTexture;
			}
			set
			{
				_removeTexture = value;
				Material.mainTexture = (_removeTexture ? null : DefaultMaterial.mainTexture);
			}
		}

		public bool UseEmission
		{
			get
			{
				return _useEmission;
			}
			set
			{
				_useEmission = value;
				if (_useEmission)
				{
					Material.EnableKeyword("_EMISSION");
				}
				else
				{
					Material.DisableKeyword("_EMISSION");
				}
			}
		}

		public float RenderMode
		{
			get
			{
				return _renderMode;
			}
			set
			{
				SetRenderMode(value);
			}
		}

		public bool Hidden
		{
			get
			{
				return Shader.name == "Sizebox/Hide";
			}
		}

		public void SetTexture(string propertyName, Texture texture, bool reset)
		{
			if (Material.HasProperty(propertyName))
			{
				if (reset)
				{
					ResetTexture(propertyName);
					return;
				}
				Material.EnableKeyword("_NORMALMAP");
				Material.EnableKeyword("_DETAIL_MULX2");
				Material.SetTexture(propertyName, texture);
			}
		}

		public void SetTexture(string propertyName, string textureName)
		{
			if (!Material.HasProperty(propertyName))
			{
				return;
			}
			bool reset = false;
			Texture texture = null;
			if (!textureName.Equals("SB_NO_TEX"))
			{
				texture = _controls.FindTexture(textureName);
				if (texture == null)
				{
					reset = true;
				}
			}
			SetTexture(propertyName, texture, reset);
		}

		private void ResetTexture(string textureName)
		{
			if (Material.HasProperty(textureName))
			{
				Texture value;
				_originalTextureMap.TryGetValue(textureName, out value);
				Material.SetTexture(textureName, value);
			}
		}

		public MaterialWrapper(Material material, int count, MaterialControlGui controls)
		{
			DefaultMaterial = new Material(material);
			Material = material;
			if (material.HasProperty("_Mode"))
			{
				RenderMode = material.GetFloat(ModeID);
			}
			Id = count + material.name;
			DefaultData = GenerateMaterialData();
			_controls = controls;
			_originalTextureMap = new Dictionary<string, Texture>();
			string[] shaderTextures = ShaderTextures;
			foreach (string text in shaderTextures)
			{
				if (Material.HasProperty(text))
				{
					_originalTextureMap.Add(text, Material.GetTexture(text));
				}
			}
		}

		public void ResetMaterial()
		{
			Material.shader = DefaultMaterial.shader;
			Material.CopyPropertiesFromMaterial(DefaultMaterial);
			UseEmission = DefaultData.useEmission;
			RemoveTexture = DefaultData.removeTexture;
		}

		public MaterialData GenerateMaterialData()
		{
			return new MaterialData(this, ShaderFloats, ShaderColors, ShaderTextures);
		}

		private int SetRenderQueue(int mode)
		{
			int num = Material.renderQueue;
			switch (mode)
			{
			default:
				num = Mathf.Clamp(num, 2000, 2449);
				break;
			case 1:
				num = Mathf.Clamp(num, 2450, 2999);
				break;
			case 2:
			case 3:
				if (num < 3000)
				{
					num = 3000;
				}
				break;
			}
			return num;
		}

		private void SetRenderMode(float value)
		{
			int num = Mathf.RoundToInt(value);
			if (Material.HasProperty("_Mode") && num >= 0 && num <= 3)
			{
				Material.DisableKeyword("_ALPHATEST_ON");
				Material.DisableKeyword("_ALPHABLEND_ON");
				Material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				switch (num)
				{
				case 1:
					Material.EnableKeyword("_ALPHATEST_ON");
					break;
				case 2:
					Material.EnableKeyword("_ALPHABLEND_ON");
					break;
				case 3:
					Material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
					break;
				}
				Material.SetFloat(ModeID, num);
				Material.renderQueue = SetRenderQueue(num);
				_renderMode = num;
			}
		}

		public void Hide()
		{
			Shader = Shader.Find("Sizebox/Hide");
		}
	}
}
