using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
	public class FBBIKArmBending : MonoBehaviour
	{
		public FullBodyBipedIK ik;

		public Vector3 bendDirectionOffsetLeft;

		public Vector3 bendDirectionOffsetRight;

		public Vector3 characterSpaceBendOffsetLeft;

		public Vector3 characterSpaceBendOffsetRight;

		private Quaternion leftHandTargetRotation;

		private Quaternion rightHandTargetRotation;

		private bool initiated;

		private void LateUpdate()
		{
			if (!(ik == null))
			{
				if (!initiated)
				{
					IKSolverFullBodyBiped solver = ik.solver;
					solver.OnPostUpdate = (IKSolver.UpdateDelegate)Delegate.Combine(solver.OnPostUpdate, new IKSolver.UpdateDelegate(OnPostFBBIK));
					initiated = true;
				}
				if (ik.solver.leftHandEffector.target != null)
				{
					Vector3 left = Vector3.left;
					ik.solver.leftArmChain.bendConstraint.direction = ik.solver.leftHandEffector.target.rotation * left + ik.solver.leftHandEffector.target.rotation * bendDirectionOffsetLeft + ik.transform.rotation * characterSpaceBendOffsetLeft;
					ik.solver.leftArmChain.bendConstraint.weight = 1f;
				}
				if (ik.solver.rightHandEffector.target != null)
				{
					Vector3 right = Vector3.right;
					ik.solver.rightArmChain.bendConstraint.direction = ik.solver.rightHandEffector.target.rotation * right + ik.solver.rightHandEffector.target.rotation * bendDirectionOffsetRight + ik.transform.rotation * characterSpaceBendOffsetRight;
					ik.solver.rightArmChain.bendConstraint.weight = 1f;
				}
			}
		}

		private void OnPostFBBIK()
		{
			if (!(ik == null))
			{
				if (ik.solver.leftHandEffector.target != null)
				{
					ik.references.leftHand.rotation = ik.solver.leftHandEffector.target.rotation;
				}
				if (ik.solver.rightHandEffector.target != null)
				{
					ik.references.rightHand.rotation = ik.solver.rightHandEffector.target.rotation;
				}
			}
		}

		private void OnDestroy()
		{
			if (ik != null)
			{
				IKSolverFullBodyBiped solver = ik.solver;
				solver.OnPostUpdate = (IKSolver.UpdateDelegate)Delegate.Remove(solver.OnPostUpdate, new IKSolver.UpdateDelegate(OnPostFBBIK));
			}
		}
	}
}
