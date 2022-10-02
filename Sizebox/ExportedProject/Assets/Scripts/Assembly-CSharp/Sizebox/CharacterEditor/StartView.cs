using UnityEngine;
using UnityEngine.UI;

namespace Sizebox.CharacterEditor
{
	public class StartView : BaseView
	{
		[Space(15f)]
		[SerializeField]
		private CharacterEditorView characterEditorView;

		[SerializeField]
		private Button startCharacterEditor;

		protected override void Awake()
		{
			base.Awake();
			startCharacterEditor.onClick.AddListener(OnStartButton);
		}

		private void OnStartButton()
		{
			characterEditorView.EnableCharacterEditor();
		}
	}
}
