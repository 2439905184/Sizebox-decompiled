using RootMotion.Dynamics;
using UnityEngine;

namespace RootMotion.Demos
{
	public class PuppetScaling : MonoBehaviour
	{
		public PuppetMaster puppetMaster;

		[Range(0.01f, 10f)]
		public float masterScale = 1f;

		public int muscleIndex;

		[Range(0.01f, 10f)]
		public float muscleScale = 1f;

		private float defaultMuscleSpring;

		private void Start()
		{
			puppetMaster.updateJointAnchors = true;
			puppetMaster.supportTranslationAnimation = true;
			defaultMuscleSpring = puppetMaster.muscleSpring;
		}

		private void Update()
		{
			puppetMaster.transform.parent.localScale = Vector3.one * masterScale;
			puppetMaster.muscleSpring = defaultMuscleSpring * Mathf.Pow(masterScale, 2f);
			muscleIndex = Mathf.Clamp(muscleIndex, 0, puppetMaster.muscles.Length - 1);
			for (int i = 0; i < puppetMaster.muscles.Length; i++)
			{
				if (i == muscleIndex)
				{
					puppetMaster.muscles[i].target.localScale = Vector3.one * muscleScale;
					puppetMaster.muscles[i].transform.localScale = Vector3.one * muscleScale;
				}
				else
				{
					puppetMaster.muscles[i].target.localScale = Vector3.one;
					puppetMaster.muscles[i].transform.localScale = Vector3.one;
				}
			}
			if (puppetMaster.muscles[1].transform.parent == puppetMaster.transform)
			{
				for (int j = 0; j < puppetMaster.muscles[muscleIndex].childIndexes.Length; j++)
				{
					puppetMaster.muscles[puppetMaster.muscles[muscleIndex].childIndexes[j]].transform.localScale = Vector3.one * muscleScale;
				}
			}
		}
	}
}
