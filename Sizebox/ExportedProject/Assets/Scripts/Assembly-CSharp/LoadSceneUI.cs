using System.IO;
using System.Runtime.CompilerServices;
using SaveDataStructures;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LoadSceneUI : MonoBehaviour
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass2_0
	{
		public LoadSceneUI _003C_003E4__this;

		public string path;

		internal void _003CCreateButton_003Eb__0()
		{
			_003C_003E4__this.OnClickLoadScene(path);
		}
	}

	public Button buttonPrefab;

	public Canvas menuUI;

	private Button CreateButton(string path, Transform thisTransform)
	{
		_003C_003Ec__DisplayClass2_0 _003C_003Ec__DisplayClass2_ = new _003C_003Ec__DisplayClass2_0();
		_003C_003Ec__DisplayClass2_._003C_003E4__this = this;
		_003C_003Ec__DisplayClass2_.path = path;
		Button button = Object.Instantiate(buttonPrefab, thisTransform, false);
		Text componentInChildren = button.GetComponentInChildren<Text>();
		string[] array = _003C_003Ec__DisplayClass2_.path.Split(Path.DirectorySeparatorChar);
		string text = array[array.Length - 1];
		text = text.Replace(".save", "");
		componentInChildren.text = text.Replace(".json", "");
		button.onClick.AddListener(_003C_003Ec__DisplayClass2_._003CCreateButton_003Eb__0);
		return button;
	}

	private void Start()
	{
		string[] listLoadableSaveFiles = IOManager.GetListLoadableSaveFiles();
		if (listLoadableSaveFiles.Length != 0)
		{
			Transform thisTransform = GetComponentInChildren<GridLayoutGroup>().gameObject.transform;
			CreateButton(listLoadableSaveFiles[0], thisTransform).Select();
			for (int i = 1; i < listLoadableSaveFiles.Length; i++)
			{
				CreateButton(listLoadableSaveFiles[i], thisTransform);
			}
			return;
		}
		Transform obj = base.transform.Find("Cancel");
		if ((object)obj != null)
		{
			Button component = obj.GetComponent<Button>();
			if ((object)component != null)
			{
				component.Select();
			}
		}
	}

	private void OnEnable()
	{
		InputManager.inputs.Interface.Back.performed += OnKeyBack;
	}

	private void OnKeyBack(InputAction.CallbackContext obj)
	{
		base.gameObject.SetActive(false);
		menuUI.gameObject.SetActive(true);
		Button componentInChildren = menuUI.GetComponentInChildren<Button>();
		if ((object)componentInChildren != null)
		{
			componentInChildren.Select();
		}
	}

	private void OnDisable()
	{
		if ((bool)StateManager.cachedInstance)
		{
			InputManager.inputs.Interface.Back.performed -= OnKeyBack;
		}
	}

	private void OnClickLoadScene(string scene)
	{
		Debug.Log("Load Scene: " + scene);
		SaveData saveData = SavedScenesManager.Instance.LoadSaveData(scene);
		if (saveData == null)
		{
			UiMessageBox uiMessageBox = UiMessageBox.Create("Unable to load save. Its scene might be missing or the game save might be corrupt.", "Load Error");
			uiMessageBox.ToLogError();
			uiMessageBox.Popup();
		}
		else
		{
			SavedScenesManager.Instance.LoadScene(saveData.scene);
		}
	}
}
