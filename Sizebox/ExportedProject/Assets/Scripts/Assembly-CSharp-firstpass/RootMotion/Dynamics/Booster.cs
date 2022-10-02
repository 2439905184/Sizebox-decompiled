using System;
using UnityEngine;

namespace RootMotion.Dynamics
{
	[Serializable]
	public class Booster
	{
		[Tooltip("If true, all the muscles will be boosted and the 'Muscles' and 'Groups' properties below will be ignored.")]
		public bool fullBody;

		[Tooltip("Muscles to boost. Used only when 'Full Body' is false.")]
		public ConfigurableJoint[] muscles = new ConfigurableJoint[0];

		[Tooltip("Muscle groups to boost. Used only when 'Full Body' is false.")]
		public Muscle.Group[] groups = new Muscle.Group[0];

		[Tooltip("Immunity to apply to the muscles. If muscle immunity is 1, it can not be damaged.")]
		[Range(0f, 1f)]
		public float immunity;

		[Tooltip("Impulse multiplier to be applied to the muscles. This makes them deal more damage to other puppets.")]
		public float impulseMlp;

		[Tooltip("Falloff for parent muscles (power of kinship degree).")]
		public float boostParents;

		[Tooltip("Falloff for child muscles (power of kinship degree).")]
		public float boostChildren;

		[Tooltip("This does nothing on it's own, you can use it in a 'yield return new WaitForseconds(delay);' call.")]
		public float delay;

		public void Boost(BehaviourPuppet puppet)
		{
			if (fullBody)
			{
				puppet.Boost(immunity, impulseMlp);
				return;
			}
			ConfigurableJoint[] array = muscles;
			foreach (ConfigurableJoint configurableJoint in array)
			{
				for (int j = 0; j < puppet.puppetMaster.muscles.Length; j++)
				{
					if (puppet.puppetMaster.muscles[j].joint == configurableJoint)
					{
						puppet.Boost(j, immunity, impulseMlp, boostParents, boostChildren);
						break;
					}
				}
			}
			Muscle.Group[] array2 = groups;
			foreach (Muscle.Group group in array2)
			{
				for (int k = 0; k < puppet.puppetMaster.muscles.Length; k++)
				{
					if (puppet.puppetMaster.muscles[k].props.group == group)
					{
						puppet.Boost(k, immunity, impulseMlp, boostParents, boostChildren);
					}
				}
			}
		}
	}
}
