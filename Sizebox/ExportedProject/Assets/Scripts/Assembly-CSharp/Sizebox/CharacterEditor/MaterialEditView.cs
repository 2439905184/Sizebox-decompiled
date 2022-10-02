using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SizeboxUI;
using UnityEngine;
using UnityEngine.UI;

namespace Sizebox.CharacterEditor
{
	public class MaterialEditView : BaseView
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Predicate<MaterialEntryGui> _003C_003E9__8_0;

			internal bool _003Cget_MaterialEntries_003Eb__8_0(MaterialEntryGui mat)
			{
				return mat.gameObject.activeSelf;
			}
		}

		[Header("Prefabs")]
		[SerializeField]
		private MaterialEntryGui materialEntryPrefab;

		[Header("Required References")]
		[SerializeField]
		private MaterialControlGui materialControls;

		[SerializeField]
		private RectTransform controlsGuiParent;

		[SerializeField]
		private Button resetButton;

		[SerializeField]
		private Button selectAllButton;

		private MaterialEdit _materialEdit;

		private readonly List<MaterialEntryGui> _materialEntries = new List<MaterialEntryGui>();

		private InterfaceControl _control;

		public List<MaterialEntryGui> MaterialEntries
		{
			get
			{
				return _materialEntries.FindAll(_003C_003Ec._003C_003E9__8_0 ?? (_003C_003Ec._003C_003E9__8_0 = _003C_003Ec._003C_003E9._003Cget_MaterialEntries_003Eb__8_0));
			}
		}

		protected override void Awake()
		{
			base.Awake();
			_control = GuiManager.Instance.InterfaceControl;
			materialControls.gameObject.SetActive(false);
			resetButton.onClick.AddListener(OnResetAll);
			selectAllButton.onClick.AddListener(OnSelectAll);
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if ((bool)_control.selectedEntity)
			{
				_materialEdit = _control.selectedEntity.GetComponent<MaterialEdit>();
				if ((bool)_materialEdit)
				{
					EnableMaterialEntries();
				}
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			DisableMaterialEntries();
			InterfaceControl.instance.UpdateCollider();
		}

		private void EnableMaterialEntries()
		{
			if (_materialEdit == null)
			{
				return;
			}
			DisableMaterialEntries();
			EnsureMaterialEntries(_materialEdit.Materials);
			int num = 0;
			foreach (MaterialWrapper material in _materialEdit.Materials)
			{
				_materialEntries[num].gameObject.SetActive(true);
				_materialEntries[num].RegisterMaterial(material, this);
				num++;
			}
		}

		private void DisableMaterialEntries()
		{
			foreach (MaterialEntryGui materialEntry in _materialEntries)
			{
				materialEntry.gameObject.SetActive(false);
			}
		}

		private void EnsureMaterialEntries(ICollection<MaterialWrapper> materialWrappers)
		{
			int count = materialWrappers.Count;
			for (int i = _materialEntries.Count; i < count; i++)
			{
				MaterialEntryGui materialEntryGui = UnityEngine.Object.Instantiate(materialEntryPrefab, controlsGuiParent);
				_materialEntries.Add(materialEntryGui);
				materialEntryGui.gameObject.SetActive(false);
			}
		}

		public MaterialControlGui GetControls()
		{
			return materialControls;
		}

		private void OnResetAll()
		{
			if ((bool)_materialEdit)
			{
				materialControls.ResetAll();
			}
		}

		private void OnSelectAll()
		{
			materialControls.SelectAll();
		}
	}
}
