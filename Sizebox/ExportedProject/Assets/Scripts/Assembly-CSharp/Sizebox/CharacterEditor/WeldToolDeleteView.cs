using UnityEngine;
using UnityEngine.UI;

namespace Sizebox.CharacterEditor
{
	public class WeldToolDeleteView : BaseAdvancedView
	{
		[Space]
		[SerializeField]
		private Button deleteButton;

		private WeldTool welder;

		protected override void Awake()
		{
			base.Awake();
			deleteButton.onClick.AddListener(OnDelete);
		}

		protected override void OnEnable()
		{
			if ((bool)Controller.selectedEntity)
			{
				welder = Controller.selectedEntity.GetComponent<WeldTool>();
				if ((bool)welder)
				{
					base.OnEnable();
					ShowHandles(welder.Keys);
					Controller.commandEnabled = false;
				}
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			Controller.commandEnabled = true;
		}

		private void OnDelete()
		{
			foreach (SkeletonEditHandle targetHandle in handleManager.TargetHandles)
			{
				welder.Unweld(targetHandle);
			}
			handleManager.DeleteTargets(true);
			ShowHandles(welder.Keys);
		}
	}
}
