using UnityEngine;
using UnityEngine.SceneManagement;

public class LogicGlobalFracturing : MonoBehaviour
{
	[HideInInspector]
	public static bool HelpVisible = true;

	private void Start()
	{
		HelpVisible = true;
	}

	public static void GlobalGUI()
	{
		GUI.Box(new Rect(0f, 0f, 400f, 420f), "");
		GUI.Box(new Rect(0f, 0f, 400f, 420f), "-----Ultimate Fracturing & Destruction Tool-----");
		GUILayout.Space(40f);
		GUILayout.Label("Press F1 to show/hide this help window");
		GUILayout.Label("Press 1-" + SceneManager.sceneCountInBuildSettings + " to select different sample scenes");
		GUILayout.Space(20f);
	}

	private void Update()
	{
		for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
		{
			if (Input.GetKeyDown((KeyCode)(49 + i)))
			{
				SceneManager.LoadScene(i);
			}
		}
		if (Input.GetKeyDown(KeyCode.F1))
		{
			HelpVisible = !HelpVisible;
		}
	}
}
