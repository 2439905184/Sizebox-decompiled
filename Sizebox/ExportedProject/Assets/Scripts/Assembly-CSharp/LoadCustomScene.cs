using UnityEngine;

public class LoadCustomScene : MonoBehaviour
{
	public SceneLoader sceneLoader;

	public string sceneName;

	private void Start()
	{
		sceneLoader.LoadSceneByName(sceneName);
	}
}
