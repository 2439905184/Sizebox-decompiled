using UnityEngine;
using UnityEngine.SceneManagement;

public class LogicExplodingFloor : MonoBehaviour
{
	private void OnGUI()
	{
		if (LogicGlobalFracturing.HelpVisible)
		{
			LogicGlobalFracturing.GlobalGUI();
			GUILayout.Label("This scene shows a floor with:");
			GUILayout.Label("-BSP fracturing");
			GUILayout.Label("-Moving explosion source with radius");
			GUILayout.Label("");
			if (GUILayout.Button("Restart"))
			{
				SceneManager.LoadScene("02 Sample Scene - Exploding Floor");
			}
		}
	}
}
