using RootMotion.Dynamics;
using UnityEngine;

namespace RootMotion.Demos
{
	public class Killing : MonoBehaviour
	{
		[Tooltip("Reference to the PuppetMaster component.")]
		public PuppetMaster puppetMaster;

		[Tooltip("Settings for killing and freezing the puppet.")]
		public PuppetMaster.StateSettings stateSettings = PuppetMaster.StateSettings.Default;

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.K))
			{
				puppetMaster.Kill(stateSettings);
			}
			if (Input.GetKeyDown(KeyCode.F))
			{
				puppetMaster.Freeze(stateSettings);
			}
			if (Input.GetKeyDown(KeyCode.R))
			{
				puppetMaster.Resurrect();
			}
		}
	}
}
