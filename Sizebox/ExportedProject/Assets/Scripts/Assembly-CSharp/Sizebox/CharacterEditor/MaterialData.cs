using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sizebox.CharacterEditor
{
	[Serializable]
	public struct MaterialData
	{
		public string id;

		public string shaderName;

		public float renderMode;

		public List<FloatData> floatData;

		public List<ColorData> colorData;

		public List<TextureData> textureData;

		public bool removeTexture;

		public bool useEmission;

		public MaterialData(MaterialWrapper wrapper, string[] floats, string[] colors, string[] textures)
		{
			id = wrapper.Id;
			Material material = wrapper.Material;
			shaderName = (material.shader ? material.shader.name : "");
			renderMode = wrapper.RenderMode;
			floatData = new List<FloatData>();
			colorData = new List<ColorData>();
			textureData = new List<TextureData>();
			string[] array = floats;
			foreach (string text in array)
			{
				if (material.HasProperty(text))
				{
					floatData.Add(new FloatData(text, material.GetFloat(text)));
				}
			}
			array = colors;
			foreach (string text2 in array)
			{
				if (material.HasProperty(text2))
				{
					colorData.Add(new ColorData(text2, material.GetColor(text2)));
				}
			}
			array = textures;
			foreach (string text3 in array)
			{
				if (material.HasProperty(text3))
				{
					Texture texture = material.GetTexture(text3);
					textureData.Add(texture ? new TextureData(text3, texture.name, material.GetTextureScale(text3)) : new TextureData(text3, "SB_NO_TEX", material.GetTextureScale(text3)));
				}
			}
			removeTexture = wrapper.RemoveTexture;
			useEmission = wrapper.UseEmission;
		}

		public void LoadMaterialData(MaterialWrapper wrapper)
		{
			Material material = wrapper.Material;
			Shader shader = ShaderLoader.GetShader(shaderName);
			if ((bool)shader)
			{
				material.shader = shader;
			}
			wrapper.RenderMode = renderMode;
			foreach (FloatData floatDatum in floatData)
			{
				if (material.HasProperty(floatDatum.propertyName))
				{
					material.SetFloat(floatDatum.propertyName, floatDatum.value);
				}
			}
			foreach (ColorData colorDatum in colorData)
			{
				if (material.HasProperty(colorDatum.propertyName))
				{
					material.SetColor(colorDatum.propertyName, colorDatum.color);
				}
			}
			foreach (TextureData textureDatum in textureData)
			{
				if (material.HasProperty(textureDatum.propertyName))
				{
					wrapper.SetTexture(textureDatum.propertyName, textureDatum.textureName);
					material.SetTextureScale(textureDatum.propertyName, textureDatum.tiling);
				}
			}
			wrapper.RemoveTexture = removeTexture;
			wrapper.UseEmission = useEmission;
		}
	}
}
