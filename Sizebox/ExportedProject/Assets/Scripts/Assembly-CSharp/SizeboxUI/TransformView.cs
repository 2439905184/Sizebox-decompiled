using System;
using System.Runtime.CompilerServices;
using RuntimeGizmos;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SizeboxUI
{
	public class TransformView : MonoBehaviour
	{
		[Header("Sliders")]
		[SerializeField]
		private Slider sliderVerticalOffset;

		[SerializeField]
		private Slider sliderGrounder;

		[SerializeField]
		private Slider sliderRotX;

		[SerializeField]
		private Slider sliderRotY;

		[SerializeField]
		private Slider sliderRotZ;

		[SerializeField]
		private Slider sliderScale;

		[Header("Buttons")]
		[SerializeField]
		private Button translateToolButton;

		[SerializeField]
		private Button rotateToolButton;

		[SerializeField]
		private Button scaleToolButton;

		[SerializeField]
		private Button spaceButton;

		[Header("Toggles")]
		[SerializeField]
		private Toggle toggleRotation;

		[SerializeField]
		private Toggle hideHandlesToggle;

		[Header("Text")]
		[SerializeField]
		private InputField scaleInput;

		private const string ScaleString = "Size: ";

		private bool _applyChanges;

		private bool _isFocused;

		private InterfaceControl _control;

		private TransformGizmo _gizmo;

		private void Awake()
		{
			Sbox.AddSBoxInputFieldEvents(scaleInput);
			_control = GuiManager.Instance.InterfaceControl;
			_gizmo = GuiManager.Instance.TransformGizmo;
			UpdateScaleText(1f);
			translateToolButton.onClick.AddListener(OnTranslateClick);
			rotateToolButton.onClick.AddListener(OnRotateClick);
			scaleToolButton.onClick.AddListener(OnScaleClick);
			spaceButton.onClick.AddListener(OnSpaceClick);
			sliderVerticalOffset.onValueChanged.AddListener(_control.SetYAxisOffset);
			sliderGrounder.onValueChanged.AddListener(_003CAwake_003Eb__18_0);
			sliderRotX.onValueChanged.AddListener(_control.RotateXAxis);
			sliderRotY.onValueChanged.AddListener(_control.RotateYAxis);
			sliderRotZ.onValueChanged.AddListener(_control.RotateZAxis);
			sliderScale.maxValue = Mathf.Log10(MapSettingInternal.maxGtsSize) * 100f;
			sliderScale.minValue = Mathf.Log10(MapSettingInternal.minGtsSize) * 100f;
			sliderScale.onValueChanged.AddListener(OnScaleChanged);
			scaleInput.onEndEdit.AddListener(OnScaleInputChanged);
			InterfaceControl control = _control;
			control.onSelected = (UnityAction)Delegate.Combine(control.onSelected, new UnityAction(OnSelect));
			InterfaceControl control2 = _control;
			control2.onDeselect = (UnityAction<EntityBase>)Delegate.Combine(control2.onDeselect, new UnityAction<EntityBase>(OnDeselect));
			toggleRotation.onValueChanged.AddListener(_003CAwake_003Eb__18_1);
			hideHandlesToggle.isOn = EditPlacement.showHandles;
			hideHandlesToggle.onValueChanged.AddListener(ShowHandles);
		}

		private void OnTranslateClick()
		{
			_gizmo.SetToolMode(TransformType.Move);
		}

		private void OnRotateClick()
		{
			_gizmo.SetToolMode(TransformType.Rotate);
		}

		private void OnScaleClick()
		{
			_gizmo.SetToolMode(TransformType.Scale);
		}

		private void OnSpaceClick()
		{
			_gizmo.ToggleSpaceMode();
		}

		private void Update()
		{
			UpdateButtonDisplay();
		}

		private void UpdateButtonDisplay()
		{
			spaceButton.gameObject.SetActive(!_gizmo.overrideSpace);
			if (_gizmo.allowScaling && _gizmo.overrideSpace)
			{
				scaleToolButton.gameObject.SetActive(true);
			}
			else
			{
				scaleToolButton.gameObject.SetActive(false);
			}
		}

		private void OnScaleInputChanged(string text)
		{
			if (_control.selectedEntity == null)
			{
				EventSystem.current.SetSelectedGameObject(sliderScale.gameObject);
				return;
			}
			float num = GameController.ConvertHumanReadableToScale(text, true);
			if (!float.IsNegativeInfinity(num))
			{
				_control.selectedEntity.MeshHeight = num;
			}
			UpdateScaleText(_control.selectedEntity.MeshHeight);
		}

		private void OnScaleChanged(float scale)
		{
			if (!(_control.selectedEntity == null))
			{
				if (_applyChanges)
				{
					_control.SetScale(scale);
				}
				UpdateScaleText(_control.selectedEntity.MeshHeight);
			}
		}

		private void OnScaleUpdated(float oldScale, float newScale)
		{
			OnScaleUpdated();
		}

		private void OnScaleUpdated()
		{
			if ((bool)_control.selectedEntity)
			{
				_applyChanges = false;
				sliderScale.value = _control.GetScale();
				_applyChanges = true;
				UpdateScaleText(_control.selectedEntity.MeshHeight);
			}
		}

		private void UpdateScaleText(float scale)
		{
			scaleInput.text = "Size: " + ((Math.Abs(scale) < float.Epsilon) ? "Unknown" : GameController.ConvertScaleToHumanReadable(scale));
		}

		private void OnSelect()
		{
			if (!base.gameObject.activeSelf)
			{
				return;
			}
			ReloadValues();
			if ((bool)_control.selectedEntity)
			{
				if (_control.selectedEntity.isGiantess)
				{
					sliderScale.maxValue = Mathf.Log10(MapSettingInternal.maxGtsSize) * 100f;
					sliderScale.minValue = Mathf.Log10(MapSettingInternal.minGtsSize) * 100f;
				}
				else
				{
					sliderScale.maxValue = Mathf.Log10(MapSettingInternal.maxPlayerSize) * 100f;
					sliderScale.minValue = Mathf.Log10(MapSettingInternal.minPlayerSize) * 100f;
				}
				ReloadValues();
				EntityBase selectedEntity = _control.selectedEntity;
				selectedEntity.SizeChanging = (UnityAction<float, float>)Delegate.Combine(selectedEntity.SizeChanging, new UnityAction<float, float>(OnScaleUpdated));
			}
		}

		private void OnDeselect(EntityBase entityBase)
		{
			if ((bool)entityBase)
			{
				entityBase.SizeChanging = (UnityAction<float, float>)Delegate.Remove(entityBase.SizeChanging, new UnityAction<float, float>(OnScaleUpdated));
			}
		}

		private void OnEnable()
		{
			ReloadValues();
		}

		private void ReloadValues()
		{
			if ((bool)_control && (bool)_control.selectedEntity)
			{
				_applyChanges = false;
				sliderVerticalOffset.value = _control.GetYAxisOffset();
				sliderGrounder.value = _control.Grounder;
				sliderRotX.value = _control.GetXRotation();
				sliderRotY.value = _control.GetYRotation();
				sliderRotZ.value = _control.GetZRotation();
				_applyChanges = true;
				OnScaleUpdated();
			}
		}

		private void ShowHandles(bool show)
		{
			EditPlacement.showHandles = show;
		}

		[CompilerGenerated]
		private void _003CAwake_003Eb__18_0(float x)
		{
			_control.Grounder = x;
		}

		[CompilerGenerated]
		private void _003CAwake_003Eb__18_1(bool val)
		{
			_control.LockRotation(val);
		}
	}
}
