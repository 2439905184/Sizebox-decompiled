using SaveDataStructures;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SavedScenesManager
{
	private static SavedScenesManager _instance;

	private SaveData _saveData;

	public static UnityAction PreSave;

	public static SavedScenesManager Instance
	{
		get
		{
			return _instance ?? (_instance = new SavedScenesManager());
		}
	}

	public void SaveScene(string filename)
	{
		UnityAction preSave = PreSave;
		if (preSave != null)
		{
			preSave();
		}
		string content = JsonUtility.ToJson(SaveData.Create(), true);
		IOManager.Instance.SaveFile(filename, content);
	}

	public SaveData LoadSaveData(string filename)
	{
		string text = IOManager.Instance.LoadFile(filename);
		if (text == null)
		{
			Debug.LogError("Load scene has failed.");
			return null;
		}
		return JsonUtility.FromJson<SaveData>(text);
	}

	private string GetCurrentScene()
	{
		return SceneManager.GetActiveScene().name;
	}

	public void ReBuildScene()
	{
		Debug.Log("Loading scene contents");
		SaveData data = JsonUtility.FromJson<SaveData>(IOManager.Instance.LoadCachedDataFile());
		LoadEntitiesOnScene(data);
	}

	public void LoadScene(string sceneName)
	{
		if ((bool)GameController.Instance)
		{
			GameController.Instance.SetPausedState(false);
		}
		SceneLoader sceneLoader = Object.FindObjectOfType<SceneLoader>();
		if ((bool)sceneLoader)
		{
			sceneLoader.LoadSceneByName(sceneName);
		}
		else
		{
			SceneManager.LoadScene(sceneName);
		}
	}

	public void RestartScene()
	{
		string currentScene = GetCurrentScene();
		LoadScene(currentScene);
	}

	private void LoadEntitiesOnScene(SaveData data)
	{
		if (data != null)
		{
			_saveData = data;
			_saveData.Load();
		}
	}
}
