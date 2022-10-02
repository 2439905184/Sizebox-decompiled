using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sizebox.CharacterEditor
{
	public class MaterialControlGui : MonoBehaviour
	{
		[Header("Required References")]
		[SerializeField]
		private MaterialEditView materialEditView;

		[SerializeField]
		private Button resetButton;

		[SerializeField]
		private Button hideButton;

		[SerializeField]
		private ColorPicker colorPicker;

		[SerializeField]
		private Dropdown colorSelect;

		[SerializeField]
		private Dropdown shaderSelect;

		[Space]
		[SerializeField]
		private List<string> colorPropertyNames;

		[SerializeField]
		private List<Shader> shaders;

		[Space]
		[SerializeField]
		private List<MaterialControlOption> controlOptions;

		private const string Standard = "Standard";

		private const string Standard2Sided = "Standard2Sided";

		private const string StandardSpecular = "Standard (Specular setup)";

		private MaterialEntryGui _activeGui;

		private readonly List<MaterialEntryGui> _selectedMaterials = new List<MaterialEntryGui>();

		private Shader _extraShader;

		private Dropdown.OptionData _extraOption;

		private string _targetColor;

		private void Awake()
		{
			PrepareShaderSelection();
			colorPicker.onValueChanged.AddListener(OnColorChanged);
			colorSelect.onValueChanged.AddListener(OnTargetColorSelect);
			resetButton.onClick.AddListener(ResetSelected);
			hideButton.onClick.AddListener(HideSelected);
		}

		private void PrepareShaderSelection()
		{
			List<Shader> collection = shaders;
			shaders = new List<Shader>(new Shader[3]
			{
				Shader.Find("Standard"),
				Shader.Find("Standard2Sided"),
				Shader.Find("Standard (Specular setup)")
			});
			shaders.AddRange(collection);
			if (shaders.Count > 0)
			{
				List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();
				foreach (Shader shader in shaders)
				{
					Dropdown.OptionData item = new Dropdown.OptionData
					{
						text = shader.name
					};
					list.Add(item);
				}
				shaderSelect.AddOptions(list);
			}
			shaderSelect.onValueChanged.AddListener(OnShaderSelect);
		}

		public void ProcessClickOnMaterial(MaterialEntryGui clickedMaterial, PointerEventData eventData)
		{
			if (!clickedMaterial)
			{
				return;
			}
			if (eventData.button == PointerEventData.InputButton.Right)
			{
				_selectedMaterials.Add(clickedMaterial);
				bool flag = false;
				foreach (MaterialEntryGui selectedMaterial in _selectedMaterials)
				{
					if (!selectedMaterial.MatWrapper.Hidden)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					HideSelected();
				}
				else
				{
					ResetSelected();
				}
				if (_selectedMaterials.Count < 2)
				{
					_selectedMaterials.Clear();
				}
				return;
			}
			if (StateManager.Keyboard.Shift)
			{
				if (!_selectedMaterials.Contains(clickedMaterial))
				{
					clickedMaterial.Select();
					_selectedMaterials.Add(clickedMaterial);
				}
				else if (!clickedMaterial.GetComponentInChildren<MaterialControlGui>())
				{
					_activeGui = clickedMaterial;
					OpenControls();
				}
			}
			else if (!StateManager.Keyboard.Ctrl && (_selectedMaterials.Count > 1 || !_selectedMaterials.Contains(clickedMaterial)))
			{
				ClearSelectedMaterials();
				_activeGui = clickedMaterial;
				clickedMaterial.Select();
				_selectedMaterials.Add(clickedMaterial);
				OpenControls();
			}
			else
			{
				clickedMaterial.Deselect();
				_selectedMaterials.Remove(clickedMaterial);
			}
			if (_selectedMaterials.Count <= 0)
			{
				base.gameObject.SetActive(false);
			}
			else if (!_activeGui || !_activeGui.Selected)
			{
				_activeGui = _selectedMaterials[_selectedMaterials.Count - 1];
				OpenControls();
			}
		}

		public Texture FindTexture(string textureName)
		{
			foreach (MaterialControlOption controlOption in controlOptions)
			{
				MaterialTextureOption materialTextureOption;
				if ((object)(materialTextureOption = controlOption as MaterialTextureOption) != null)
				{
					Texture texture = materialTextureOption.FindTexture(textureName);
					if ((bool)texture)
					{
						return texture;
					}
				}
			}
			return null;
		}

		private void OpenControls()
		{
			base.transform.SetParent(_activeGui.transform);
			base.gameObject.SetActive(true);
			PrepareShader();
		}

		private void PrepareShader()
		{
			if ((bool)_activeGui)
			{
				HandleExtraShaderOption();
				Shader shader = _activeGui.MatWrapper.Shader;
				shaderSelect.onValueChanged.RemoveListener(OnShaderSelect);
				if (shaders.Contains(shader))
				{
					shaderSelect.value = shaders.IndexOf(shader);
				}
				else
				{
					shaderSelect.value = shaderSelect.options.Count - 1;
				}
				shaderSelect.onValueChanged.AddListener(OnShaderSelect);
				PrepareColorOptions();
				UpdateControls();
			}
		}

		private void PrepareColorOptions()
		{
			colorSelect.options.Clear();
			if (_activeGui == null)
			{
				return;
			}
			foreach (string colorPropertyName in colorPropertyNames)
			{
				if (_activeGui.MatWrapper.Material.HasProperty(colorPropertyName))
				{
					colorSelect.options.Add(new Dropdown.OptionData(colorPropertyName));
				}
			}
			if (colorSelect.options.Count == 0)
			{
				_targetColor = null;
				return;
			}
			colorSelect.value = 0;
			OnTargetColorSelect(0);
		}

		private void HandleExtraShaderOption()
		{
			if (_extraOption != null)
			{
				shaderSelect.options.Remove(_extraOption);
				_extraOption = null;
				_extraShader = null;
			}
			if (_activeGui != null)
			{
				Material defaultMaterial = _activeGui.MatWrapper.DefaultMaterial;
				if (!shaders.Contains(defaultMaterial.shader))
				{
					_extraShader = defaultMaterial.shader;
					_extraOption = new Dropdown.OptionData(defaultMaterial.shader.name);
					shaderSelect.options.Add(_extraOption);
				}
			}
		}

		private void UpdateControls()
		{
			UpdateColor();
			foreach (MaterialControlOption controlOption in controlOptions)
			{
				controlOption.ValidateOption(_activeGui, _selectedMaterials);
			}
		}

		private void UpdateColor()
		{
			colorPicker.onValueChanged.RemoveListener(OnColorChanged);
			if (_activeGui.MatWrapper.Material.HasProperty(_targetColor))
			{
				colorPicker.CurrentColor = _activeGui.MatWrapper.Material.GetColor(_targetColor);
			}
			colorPicker.onValueChanged.AddListener(OnColorChanged);
		}

		private void OnShaderSelect(int selection)
		{
			if (_activeGui == null)
			{
				return;
			}
			Shader shader = ((selection < shaders.Count) ? shaders[selection] : _extraShader);
			if ((bool)shader)
			{
				foreach (MaterialEntryGui selectedMaterial in _selectedMaterials)
				{
					selectedMaterial.MatWrapper.Shader = shader;
				}
			}
			PrepareShader();
		}

		private void OnTargetColorSelect(int index)
		{
			_targetColor = colorSelect.options[index].text;
			UpdateColor();
		}

		private void OnColorChanged(Color color)
		{
			if (_selectedMaterials == null || _targetColor == null)
			{
				return;
			}
			foreach (MaterialEntryGui selectedMaterial in _selectedMaterials)
			{
				if (selectedMaterial.MatWrapper.Material.HasProperty(_targetColor))
				{
					selectedMaterial.MatWrapper.Material.SetColor(_targetColor, color);
				}
			}
		}

		private void ResetSelected()
		{
			foreach (MaterialEntryGui selectedMaterial in _selectedMaterials)
			{
				selectedMaterial.MatWrapper.ResetMaterial();
			}
			HandleExtraShaderOption();
			PrepareShader();
		}

		public void ResetAll()
		{
			foreach (MaterialEntryGui materialEntry in materialEditView.MaterialEntries)
			{
				materialEntry.MatWrapper.ResetMaterial();
			}
			if (_selectedMaterials.Count > 0)
			{
				HandleExtraShaderOption();
				PrepareShader();
			}
		}

		private void OnDisable()
		{
			ClearSelectedMaterials();
			base.gameObject.SetActive(false);
		}

		private void ClearSelectedMaterials()
		{
			_activeGui = null;
			foreach (MaterialEntryGui selectedMaterial in _selectedMaterials)
			{
				selectedMaterial.Deselect();
			}
			_selectedMaterials.Clear();
		}

		public void SelectAll()
		{
			ClearSelectedMaterials();
			_selectedMaterials.AddRange(materialEditView.MaterialEntries);
			foreach (MaterialEntryGui selectedMaterial in _selectedMaterials)
			{
				selectedMaterial.Select();
			}
			_activeGui = _selectedMaterials[0];
			OpenControls();
		}

		private void HideSelected()
		{
			foreach (MaterialEntryGui selectedMaterial in _selectedMaterials)
			{
				selectedMaterial.MatWrapper.Hide();
			}
			HandleExtraShaderOption();
			PrepareShader();
		}
	}
}
