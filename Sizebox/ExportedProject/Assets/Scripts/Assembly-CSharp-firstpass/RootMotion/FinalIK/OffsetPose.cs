using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
	public class OffsetPose : MonoBehaviour
	{
		[Serializable]
		public class EffectorLink
		{
			public FullBodyBipedEffector effector;

			public Vector3 offset;

			public Vector3 pin;

			public Vector3 pinWeight;

			public void Apply(IKSolverFullBodyBiped solver, float weight, Quaternion rotation)
			{
				solver.GetEffector(effector).positionOffset += rotation * offset * weight;
				Vector3 vector = solver.GetRoot().position + rotation * pin - solver.GetEffector(effector).bone.position;
				Vector3 vector2 = pinWeight * Mathf.Abs(weight);
				solver.GetEffector(effector).positionOffset = new Vector3(Mathf.Lerp(solver.GetEffector(effector).positionOffset.x, vector.x, vector2.x), Mathf.Lerp(solver.GetEffector(effector).positionOffset.y, vector.y, vector2.y), Mathf.Lerp(solver.GetEffector(effector).positionOffset.z, vector.z, vector2.z));
			}
		}

		public EffectorLink[] effectorLinks = new EffectorLink[0];

		public void Apply(IKSolverFullBodyBiped solver, float weight)
		{
			for (int i = 0; i < effectorLinks.Length; i++)
			{
				effectorLinks[i].Apply(solver, weight, solver.GetRoot().rotation);
			}
		}

		public void Apply(IKSolverFullBodyBiped solver, float weight, Quaternion rotation)
		{
			for (int i = 0; i < effectorLinks.Length; i++)
			{
				effectorLinks[i].Apply(solver, weight, rotation);
			}
		}
	}
}
