using System.Collections.Generic;
using SizeboxUI;
using UnityEngine;
using UnityEngine.UI;

namespace Sizebox.CharacterEditor
{
	public class MaterialSliderOption : MaterialControlOption
	{
		[SerializeField]
		private Slider slider;

		[SerializeField]
		private string propertyName = "";

		[SerializeField]
		private bool correctForModelScale;

		private void Awake()
		{
			slider.onValueChanged.AddListener(OnSlider);
		}

		public override void ValidateOption(MaterialEntryGui newGui, List<MaterialEntryGui> newGuis)
		{
			base.ValidateOption(newGui, newGuis);
			if (selectedGui == null)
			{
				base.gameObject.SetActive(false);
				return;
			}
			Material material = selectedGui.MatWrapper.Material;
			bool flag = material.HasProperty(propertyName);
			base.gameObject.SetActive(flag);
			if (flag)
			{
				slider.onValueChanged.RemoveListener(OnSlider);
				slider.value = UndoCorrections(material.GetFloat(propertyName));
				slider.onValueChanged.AddListener(OnSlider);
			}
		}

		private void OnSlider(float value)
		{
			if (selectedGuis == null)
			{
				return;
			}
			value = ApplyCorrections(value);
			foreach (MaterialEntryGui selectedGui in selectedGuis)
			{
				if (selectedGui.MatWrapper.Material.HasProperty(propertyName))
				{
					selectedGui.MatWrapper.Material.SetFloat(propertyName, value);
				}
			}
		}

		private float ApplyCorrections(float value)
		{
			EntityBase selectedEntity = GuiManager.Instance.InterfaceControl.selectedEntity;
			if (correctForModelScale)
			{
				value /= selectedEntity.ModelScale;
				if (selectedEntity is Giantess)
				{
					value *= 1000f;
				}
			}
			return value;
		}

		private float UndoCorrections(float value)
		{
			EntityBase selectedEntity = GuiManager.Instance.InterfaceControl.selectedEntity;
			if (correctForModelScale)
			{
				value *= selectedEntity.ModelScale;
				if (selectedEntity is Giantess)
				{
					value /= 1000f;
				}
			}
			return value;
		}
	}
}
