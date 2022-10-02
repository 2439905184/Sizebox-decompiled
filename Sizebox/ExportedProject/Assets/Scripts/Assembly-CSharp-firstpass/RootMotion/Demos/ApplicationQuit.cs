using UnityEngine;

namespace RootMotion.Demos
{
	public class ApplicationQuit : MonoBehaviour
	{
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Escape))
			{
				Application.Quit();
			}
		}
	}
}
