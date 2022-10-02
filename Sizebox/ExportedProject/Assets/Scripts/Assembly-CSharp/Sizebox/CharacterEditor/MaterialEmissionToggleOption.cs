using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sizebox.CharacterEditor
{
	public class MaterialEmissionToggleOption : MaterialControlOption
	{
		[SerializeField]
		private Toggle toggle;

		private void Awake()
		{
			toggle.onValueChanged.AddListener(ToggleKeyword);
		}

		public override void ValidateOption(MaterialEntryGui selected, List<MaterialEntryGui> newGuis)
		{
			base.ValidateOption(selected, newGuis);
			if (!(selectedGui == null))
			{
				toggle.isOn = selectedGui.MatWrapper.UseEmission;
			}
		}

		private void ToggleKeyword(bool value)
		{
			if (selectedGuis == null)
			{
				return;
			}
			foreach (MaterialEntryGui selectedGui in selectedGuis)
			{
				selectedGui.MatWrapper.UseEmission = value;
			}
		}
	}
}
