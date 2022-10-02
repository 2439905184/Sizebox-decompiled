using System.Runtime.CompilerServices;
using Sizebox.CharacterEditor;
using UnityEngine;
using UnityEngine.UI;

namespace SizeboxUI
{
	public class EditView : MonoBehaviour
	{
		[Header("Required References")]
		[SerializeField]
		private TransformView transformView;

		[SerializeField]
		private SceneTreeView sceneTreeView;

		[SerializeField]
		private CatalogView catalogView;

		[SerializeField]
		private AnimationView animationView;

		[SerializeField]
		private MorphsView morphsView;

		[SerializeField]
		private PoseView poseView;

		[SerializeField]
		private CharacterEditorView characterEditor;

		[Space]
		[SerializeField]
		private CommandView commandView;

		[SerializeField]
		private CtrlDisplay ctrlDisplay;

		[Space]
		[SerializeField]
		private Transform buttonPanel;

		[SerializeField]
		private Button buttonPrefab;

		private GuiManager guiManager;

		public EditPlacement placement { get; private set; }

		public InterfaceControl control { get; private set; }

		public CommandView CommandView
		{
			get
			{
				return commandView;
			}
		}

		public CtrlDisplay CtrlDisplay
		{
			get
			{
				return ctrlDisplay;
			}
		}

		public SceneTreeView SceneTreeView
		{
			get
			{
				return sceneTreeView;
			}
		}

		private void Awake()
		{
			guiManager = GuiManager.Instance;
			placement = guiManager.EditPlacement;
			control = guiManager.InterfaceControl;
			sceneTreeView.gameObject.SetActive(true);
			poseView.gameObject.SetActive(true);
			animationView.gameObject.SetActive(true);
			morphsView.gameObject.SetActive(true);
			characterEditor.gameObject.SetActive(true);
			transformView.gameObject.SetActive(true);
			commandView.gameObject.SetActive(true);
			catalogView.gameObject.SetActive(true);
			ctrlDisplay.gameObject.SetActive(true);
			sceneTreeView.gameObject.SetActive(false);
			poseView.gameObject.SetActive(false);
			animationView.gameObject.SetActive(false);
			morphsView.gameObject.SetActive(false);
			characterEditor.gameObject.SetActive(false);
			ctrlDisplay.gameObject.SetActive(false);
			transformView.gameObject.SetActive(true);
			commandView.gameObject.SetActive(true);
			catalogView.gameObject.SetActive(true);
			catalogView.OnMenuClick(CatalogCategory.Micro);
			AddButton("Play").onClick.AddListener(_003CAwake_003Eb__26_0);
			AddButton("Scene").onClick.AddListener(_003CAwake_003Eb__26_1);
			AddButton("Macro").onClick.AddListener(_003CAwake_003Eb__26_2);
			AddButton("Micro").onClick.AddListener(_003CAwake_003Eb__26_3);
			AddButton("Objects").onClick.AddListener(_003CAwake_003Eb__26_4);
			AddButton("Move").onClick.AddListener(_003CAwake_003Eb__26_5);
			AddButton("Transform").onClick.AddListener(_003CAwake_003Eb__26_6);
			AddButton("Animation").onClick.AddListener(_003CAwake_003Eb__26_7);
			AddButton("Pose").onClick.AddListener(_003CAwake_003Eb__26_8);
			AddButton("Morphs").onClick.AddListener(_003CAwake_003Eb__26_9);
			AddButton("Model").onClick.AddListener(_003CAwake_003Eb__26_10);
			AddButton("Delete").onClick.AddListener(_003CAwake_003Eb__26_11);
			AddButton("Menu").onClick.AddListener(_003CAwake_003Eb__26_12);
		}

		public void OnTransformClick()
		{
			transformView.gameObject.SetActive(!transformView.gameObject.activeSelf);
			if (transformView.gameObject.activeSelf && SceneTreeView.gameObject.activeSelf)
			{
				SceneTreeView.gameObject.SetActive(false);
			}
		}

		public void OnSceneClick()
		{
			SceneTreeView.gameObject.SetActive(!SceneTreeView.gameObject.activeSelf);
			if (transformView.gameObject.activeSelf && SceneTreeView.gameObject.activeSelf)
			{
				transformView.gameObject.SetActive(false);
			}
		}

		public void OnCatalogClick(CatalogCategory category)
		{
			if (category != CatalogCategory.Pose || (!(control.selectedEntity == null) && control.selectedEntity.isGiantess))
			{
				bool activeSelf = catalogView.gameObject.activeSelf;
				DisablePrimaryViews();
				catalogView.gameObject.SetActive(activeSelf);
				catalogView.OnMenuClick(category);
				transformView.gameObject.SetActive(true);
				if (transformView.gameObject.activeSelf && SceneTreeView.gameObject.activeSelf)
				{
					SceneTreeView.gameObject.SetActive(false);
				}
			}
		}

		public void OnAnimationClick()
		{
			if (!(control.humanoid == null) && control.selectedEntity.Initialized)
			{
				bool activeSelf = animationView.gameObject.activeSelf;
				DisablePrimaryViews();
				animationView.gameObject.SetActive(!activeSelf);
				transformView.gameObject.SetActive(true);
				if (transformView.gameObject.activeSelf && SceneTreeView.gameObject.activeSelf)
				{
					SceneTreeView.gameObject.SetActive(false);
				}
			}
		}

		public void OnPoseClick()
		{
			if (!(control.selectedEntity == null) && control.selectedEntity.Initialized && control.selectedEntity.GetComponent<IPosable>() != null)
			{
				bool activeSelf = poseView.gameObject.activeSelf;
				DisablePrimaryViews();
				poseView.gameObject.SetActive(!activeSelf);
				transformView.gameObject.SetActive(true);
				if (transformView.gameObject.activeSelf && SceneTreeView.gameObject.activeSelf)
				{
					SceneTreeView.gameObject.SetActive(false);
				}
			}
		}

		public void OnMorphsClick()
		{
			if (!(control.selectedEntity == null) && control.selectedEntity.GetComponent<IMorphable>() != null && control.selectedEntity.Initialized)
			{
				bool activeSelf = morphsView.gameObject.activeSelf;
				DisablePrimaryViews();
				morphsView.gameObject.SetActive(!activeSelf);
			}
		}

		public void OnCharacterClick()
		{
			if (!(control.selectedEntity == null) && control.selectedEntity.Initialized)
			{
				bool activeSelf = characterEditor.gameObject.activeSelf;
				DisablePrimaryViews();
				characterEditor.gameObject.SetActive(!activeSelf);
			}
		}

		private void DisablePrimaryViews()
		{
			animationView.gameObject.SetActive(false);
			catalogView.gameObject.SetActive(false);
			poseView.gameObject.SetActive(false);
			morphsView.gameObject.SetActive(false);
			characterEditor.gameObject.SetActive(false);
		}

		public Button AddButton(string label)
		{
			Button button = Object.Instantiate(buttonPrefab, buttonPanel);
			button.name = label;
			button.gameObject.GetComponent<Text>().text = label;
			return button;
		}

		public void OnDeleteClick()
		{
			if (placement.control != null)
			{
				placement.Delete();
			}
		}

		private void OnEnable()
		{
			StateManager.Mouse.Add();
			if (GlobalPreferences.SlOpenOnEditor.value && !transformView.gameObject.activeSelf && !SceneTreeView.gameObject.activeSelf)
			{
				OnSceneClick();
			}
		}

		private void OnDisable()
		{
			StateManager.Mouse.Remove();
		}

		[CompilerGenerated]
		private void _003CAwake_003Eb__26_0()
		{
			guiManager.ToggleEditMode();
		}

		[CompilerGenerated]
		private void _003CAwake_003Eb__26_1()
		{
			OnSceneClick();
		}

		[CompilerGenerated]
		private void _003CAwake_003Eb__26_2()
		{
			OnCatalogClick(CatalogCategory.Giantess);
		}

		[CompilerGenerated]
		private void _003CAwake_003Eb__26_3()
		{
			OnCatalogClick(CatalogCategory.Micro);
		}

		[CompilerGenerated]
		private void _003CAwake_003Eb__26_4()
		{
			OnCatalogClick(CatalogCategory.Object);
		}

		[CompilerGenerated]
		private void _003CAwake_003Eb__26_5()
		{
			placement.MoveCurrentGO();
		}

		[CompilerGenerated]
		private void _003CAwake_003Eb__26_6()
		{
			OnTransformClick();
		}

		[CompilerGenerated]
		private void _003CAwake_003Eb__26_7()
		{
			OnAnimationClick();
		}

		[CompilerGenerated]
		private void _003CAwake_003Eb__26_8()
		{
			OnPoseClick();
		}

		[CompilerGenerated]
		private void _003CAwake_003Eb__26_9()
		{
			OnMorphsClick();
		}

		[CompilerGenerated]
		private void _003CAwake_003Eb__26_10()
		{
			OnCharacterClick();
		}

		[CompilerGenerated]
		private void _003CAwake_003Eb__26_11()
		{
			placement.Delete();
		}

		[CompilerGenerated]
		private void _003CAwake_003Eb__26_12()
		{
			guiManager.OpenPauseMenu();
		}
	}
}
