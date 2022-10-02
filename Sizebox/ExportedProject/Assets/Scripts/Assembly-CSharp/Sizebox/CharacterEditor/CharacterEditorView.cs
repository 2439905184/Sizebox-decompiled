using UnityEngine;

namespace Sizebox.CharacterEditor
{
	public class CharacterEditorView : BaseView
	{
		[Space(15f)]
		[Header("View References")]
		[SerializeField]
		private BaseView startView;

		[SerializeField]
		private HandleManager handleManager;

		protected override void OnEnable()
		{
			base.OnEnable();
			if ((bool)Controller.selectedEntity && Controller.selectedEntity != handleManager.TargetEditor)
			{
				if ((bool)Controller.selectedEntity.GetComponent<CharacterEditor>())
				{
					EnableCharacterEditor();
				}
				else
				{
					DisableTabs();
				}
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
		}

		private void EnableTabs()
		{
			startView.gameObject.SetActive(false);
			foreach (ViewConfig subView in subViews)
			{
				if ((bool)subView.activator)
				{
					subView.activator.interactable = true;
				}
			}
		}

		private void DisableTabs()
		{
			DisableAllSubViews();
			startView.gameObject.SetActive(true);
			foreach (ViewConfig subView in subViews)
			{
				if ((bool)subView.activator)
				{
					subView.activator.interactable = false;
				}
			}
		}

		public void EnableCharacterEditor()
		{
			EntityBase selectedEntity = Controller.selectedEntity;
			if ((bool)selectedEntity && !(selectedEntity == handleManager.TargetEditor))
			{
				CharacterEditor characterEditor = selectedEntity.GetComponent<CharacterEditor>();
				if (!characterEditor)
				{
					characterEditor = selectedEntity.gameObject.AddComponent<CharacterEditor>();
				}
				handleManager.SetTarget(characterEditor);
				EnableTabs();
			}
		}

		protected override void OnSelection()
		{
			if (Controller == null || Controller.selectedEntity == null)
			{
				base.gameObject.SetActive(false);
			}
			else if (!Controller.selectedEntity.GetComponent<CharacterEditor>())
			{
				DisableTabs();
			}
			else
			{
				EnableCharacterEditor();
			}
		}
	}
}
