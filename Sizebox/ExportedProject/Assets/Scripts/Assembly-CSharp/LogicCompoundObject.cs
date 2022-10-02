using UnityEngine;

public class LogicCompoundObject : MonoBehaviour
{
	private void OnGUI()
	{
		if (LogicGlobalFracturing.HelpVisible)
		{
			LogicGlobalFracturing.GlobalGUI();
			GUILayout.Label("This scene shows a simulated use case scenario with:");
			GUILayout.Label("-Use of compound objects (through the included MeshCombiner tool)");
			GUILayout.Label("-Voronoi fracturing");
			GUILayout.Label("-Chunk structural interconnection");
			GUILayout.Label("-Use of support plane to link pillars to the ground");
			GUILayout.Label("-Chunk lifetime and offscreen lifetime for optimization");
			GUILayout.Label("-Collision particles");
			GUILayout.Label("-Collision sounds");
			GUILayout.Label("-Raycasting to trigger explosions with the weapon");
			GUILayout.Label("");
			GUILayout.Label("Hold down the left mouse button and move to aim.");
			GUILayout.Label("Press spacebar to fire.");
		}
	}
}
