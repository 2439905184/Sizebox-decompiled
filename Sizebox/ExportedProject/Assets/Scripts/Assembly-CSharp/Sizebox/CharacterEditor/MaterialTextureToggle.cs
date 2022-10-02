using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sizebox.CharacterEditor
{
	public class MaterialTextureToggle : MaterialControlOption
	{
		[SerializeField]
		private Toggle toggle;

		private void Awake()
		{
			toggle.onValueChanged.AddListener(OnTextureToggle);
		}

		public override void ValidateOption(MaterialEntryGui selected, List<MaterialEntryGui> newGuis)
		{
			base.ValidateOption(selected, newGuis);
			if (!(selectedGui == null))
			{
				toggle.isOn = selectedGui.MatWrapper.RemoveTexture;
			}
		}

		private void OnTextureToggle(bool value)
		{
			if (selectedGuis == null)
			{
				return;
			}
			foreach (MaterialEntryGui selectedGui in selectedGuis)
			{
				selectedGui.MatWrapper.RemoveTexture = value;
			}
		}
	}
}
