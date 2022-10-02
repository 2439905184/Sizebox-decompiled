using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sizebox.CharacterEditor
{
	public class MaterialTextureOption : MaterialControlOption
	{
		[SerializeField]
		private string texturePropertyName = "_DetailNormalMap";

		[SerializeField]
		private string textureTilingPropertyName = "_DetailAlbedoMap";

		[Header("Textures")]
		[SerializeField]
		private Texture[] textures;

		[Header("Required References")]
		[SerializeField]
		private Dropdown detailMapDropdown;

		[SerializeField]
		private InputField xScale;

		[SerializeField]
		private InputField yScale;

		private void Awake()
		{
			PrepareDetailMapDropdown();
			xScale.onValueChanged.AddListener(OnXScale);
			yScale.onValueChanged.AddListener(OnYScale);
		}

		private void PrepareDetailMapDropdown()
		{
			detailMapDropdown.options.Clear();
			detailMapDropdown.options.Add(new Dropdown.OptionData("Default"));
			detailMapDropdown.options.Add(new Dropdown.OptionData("None"));
			Texture[] array = textures;
			for (int i = 0; i < array.Length; i++)
			{
				Dropdown.OptionData item = new Dropdown.OptionData(array[i].name);
				detailMapDropdown.options.Add(item);
			}
			detailMapDropdown.onValueChanged.AddListener(OnSelection);
		}

		public override void ValidateOption(MaterialEntryGui newGui, List<MaterialEntryGui> newGuis)
		{
			base.ValidateOption(newGui, newGuis);
			if (selectedGui == null)
			{
				return;
			}
			bool flag = selectedGui.MatWrapper.Material.HasProperty(texturePropertyName);
			base.gameObject.SetActive(flag);
			if (!flag)
			{
				return;
			}
			Material defaultMaterial = selectedGui.MatWrapper.DefaultMaterial;
			Texture texture = selectedGui.MatWrapper.Material.GetTexture(texturePropertyName);
			Texture texture2 = null;
			if (defaultMaterial.HasProperty(texturePropertyName))
			{
				texture2 = defaultMaterial.GetTexture(texturePropertyName);
			}
			if ((bool)texture2 && texture == texture2)
			{
				detailMapDropdown.value = 0;
			}
			else if (!texture)
			{
				detailMapDropdown.value = 1;
			}
			else
			{
				detailMapDropdown.value = 1;
				List<Dropdown.OptionData> options = detailMapDropdown.options;
				for (int i = 2; i < options.Count; i++)
				{
					if (options[i].text == texture.name)
					{
						detailMapDropdown.value = i;
					}
				}
			}
			Vector2 textureScale = selectedGui.MatWrapper.Material.GetTextureScale(textureTilingPropertyName);
			xScale.text = textureScale.x.ToString();
			yScale.text = textureScale.y.ToString();
		}

		private void OnSelection(int index)
		{
			if (selectedGuis == null)
			{
				return;
			}
			bool reset = false;
			if (index == 0)
			{
				reset = true;
			}
			Texture texture = null;
			if (index > 1)
			{
				texture = textures[index - 2];
			}
			foreach (MaterialEntryGui selectedGui in selectedGuis)
			{
				selectedGui.MatWrapper.SetTexture(texturePropertyName, texture, reset);
			}
		}

		private void OnXScale(string text)
		{
			if (selectedGuis == null)
			{
				return;
			}
			float x = float.Parse(text);
			foreach (MaterialEntryGui selectedGui in selectedGuis)
			{
				Vector2 textureScale = selectedGui.MatWrapper.Material.GetTextureScale(textureTilingPropertyName);
				selectedGui.MatWrapper.Material.SetTextureScale(textureTilingPropertyName, new Vector2(x, textureScale.y));
			}
		}

		private void OnYScale(string text)
		{
			if (selectedGuis == null)
			{
				return;
			}
			float y = float.Parse(text);
			foreach (MaterialEntryGui selectedGui in selectedGuis)
			{
				Vector2 textureScale = selectedGui.MatWrapper.Material.GetTextureScale(textureTilingPropertyName);
				selectedGui.MatWrapper.Material.SetTextureScale(textureTilingPropertyName, new Vector2(textureScale.x, y));
			}
		}

		public Texture FindTexture(string name)
		{
			Texture[] array = textures;
			foreach (Texture texture in array)
			{
				if (texture.name.Equals(name))
				{
					return texture;
				}
			}
			return null;
		}
	}
}
