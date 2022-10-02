using UnityEngine;

public class LogicArcsAndColumns : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
		if (LogicGlobalFracturing.HelpVisible)
		{
			LogicGlobalFracturing.GlobalGUI();
			GUILayout.Label("This scene shows a simulated use case scenario with:");
			GUILayout.Label("-BSP fracturing");
			GUILayout.Label("-Chunk structural interconnection");
			GUILayout.Label("-Use of support planes to link pillars to the ground");
			GUILayout.Label("-Collision particles");
			GUILayout.Label("-Collision sounds");
			GUILayout.Label("-Raycasting to trigger explosions with the weapon");
			GUILayout.Label("");
			GUILayout.Label("Hold down the left mouse button and move to aim.");
			GUILayout.Label("Press spacebar to fire.");
			GUILayout.Space(20f);
		}
	}
}
