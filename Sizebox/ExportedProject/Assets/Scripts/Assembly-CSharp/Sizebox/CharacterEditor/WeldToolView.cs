using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Sizebox.CharacterEditor
{
	public class WeldToolView : BaseAdvancedView
	{
		[Space]
		[Header("Weld Tool Required References")]
		[SerializeField]
		private ExtraView extraView;

		[SerializeField]
		private GameObject selectionPanel;

		[SerializeField]
		private GameObject weldPanel;

		[Space]
		[SerializeField]
		private Button confirmBonesButton;

		[SerializeField]
		private Button backButton;

		[SerializeField]
		private Button weldButton;

		[Space]
		[SerializeField]
		private Text selectObjectText;

		private EntityBase selectedObject;

		private List<EditBone> selectedBones = new List<EditBone>();

		private WeldToolViewState _state;

		private WeldToolViewState State
		{
			get
			{
				return _state;
			}
			set
			{
				bool flag = value == WeldToolViewState.SelectingBones;
				selectionPanel.SetActive(flag);
				weldPanel.SetActive(!flag);
				if (flag)
				{
					ShowHandles();
				}
				else
				{
					handleManager.DisableHandlesMode();
					Controller.commandEnabled = false;
				}
				_state = value;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			confirmBonesButton.onClick.AddListener(OnConfirmBones);
			backButton.onClick.AddListener(OnBack);
			weldButton.onClick.AddListener(OnWeld);
		}

		protected override void OnEnable()
		{
			if (!Controller.selectedEntity || !Controller.selectedEntity.GetComponent<WeldTool>())
			{
				base.gameObject.SetActive(false);
				return;
			}
			base.OnEnable();
			selectedBones.Clear();
			State = WeldToolViewState.SelectingBones;
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			Controller.commandEnabled = true;
		}

		private void Update()
		{
			if (State == WeldToolViewState.SelectingBones)
			{
				confirmBonesButton.interactable = handleManager.TargetHandles.Count > 0;
				return;
			}
			if (Mouse.current.leftButton.wasPressedThisFrame)
			{
				RaycastHit[] array = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), float.PositiveInfinity);
				if (array.Length != 0)
				{
					RaycastHit[] array2 = array;
					foreach (RaycastHit raycastHit in array2)
					{
						EntityBase componentInParent = raycastHit.collider.transform.GetComponentInParent<EntityBase>();
						if (SelectObject(componentInParent))
						{
							break;
						}
					}
				}
			}
			weldButton.interactable = selectedObject;
		}

		private bool SelectObject(EntityBase obj)
		{
			EntityBase component = handleManager.TargetEditor.GetComponent<EntityBase>();
			if (!obj || obj == component || (bool)obj.GetComponent<CityBuilder>() || obj.isPlayer)
			{
				return false;
			}
			selectedObject = obj;
			gizmo.SetTarget(selectedObject.transform);
			selectObjectText.text = selectedObject.name;
			return true;
		}

		private void OnWeld()
		{
			if (selectedBones.Count <= 0 || !selectedObject)
			{
				return;
			}
			List<string> list = Controller.selectedEntity.GetComponent<WeldTool>().Weld(selectedObject, selectedBones);
			extraView.UpdateKeys();
			foreach (string item in list)
			{
				if (!extraView.Keys.Contains(item))
				{
					extraView.Keys.Add(item);
				}
			}
			extraView.SaveKeys(Controller.selectedEntity.GetComponent<CharacterEditor>());
			OnBack();
		}

		private void OnConfirmBones()
		{
			selectedBones.Clear();
			foreach (SkeletonEditHandle targetHandle in handleManager.TargetHandles)
			{
				selectedBones.Add(targetHandle.EditBone);
			}
			State = WeldToolViewState.SelectingObject;
		}

		private void OnBack()
		{
			State = WeldToolViewState.SelectingBones;
			selectedBones.Clear();
			selectedObject = null;
			selectObjectText.text = "";
			gizmo.SetTarget(null);
		}
	}
}
