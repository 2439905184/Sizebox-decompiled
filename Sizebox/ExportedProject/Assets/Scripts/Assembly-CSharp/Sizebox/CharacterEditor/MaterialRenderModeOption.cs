using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sizebox.CharacterEditor
{
	public class MaterialRenderModeOption : MaterialControlOption
	{
		[SerializeField]
		private Dropdown dropdown;

		[SerializeField]
		private string[] options;

		private const string MODE = "_Mode";

		private void Awake()
		{
			dropdown.ClearOptions();
			string[] array = options;
			foreach (string text in array)
			{
				dropdown.options.Add(new Dropdown.OptionData(text));
			}
			dropdown.onValueChanged.AddListener(OnSelection);
		}

		public override void ValidateOption(MaterialEntryGui selected, List<MaterialEntryGui> newWrappers)
		{
			base.ValidateOption(selected, newWrappers);
			if (!(selectedGui == null))
			{
				bool flag = selectedGui.MatWrapper.Material.HasProperty("_Mode");
				base.gameObject.SetActive(flag);
				dropdown.onValueChanged.RemoveListener(OnSelection);
				if (flag)
				{
					dropdown.value = Mathf.RoundToInt(selectedGui.MatWrapper.Material.GetFloat("_Mode"));
				}
				dropdown.onValueChanged.AddListener(OnSelection);
			}
		}

		private void OnSelection(int index)
		{
			if (selectedGuis == null)
			{
				return;
			}
			foreach (MaterialEntryGui selectedGui in selectedGuis)
			{
				selectedGui.MatWrapper.RenderMode = index;
			}
		}
	}
}
