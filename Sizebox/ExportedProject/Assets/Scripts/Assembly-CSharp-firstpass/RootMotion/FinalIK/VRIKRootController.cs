using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
	public class VRIKRootController : MonoBehaviour
	{
		public Transform pelvisTarget;

		public Transform leftFootTarget;

		public Transform rightFootTarget;

		private Vector3 pelvisTargetRight;

		private VRIK ik;

		private void Awake()
		{
			ik = GetComponent<VRIK>();
			IKSolverVR solver = ik.solver;
			solver.OnPreUpdate = (IKSolver.UpdateDelegate)Delegate.Combine(solver.OnPreUpdate, new IKSolver.UpdateDelegate(OnPreUpdate));
		}

		public void Calibrate()
		{
			if (pelvisTarget != null)
			{
				pelvisTargetRight = Quaternion.Inverse(pelvisTarget.rotation) * ik.references.root.right;
			}
		}

		private void OnPreUpdate()
		{
			if (base.enabled)
			{
				if (pelvisTarget != null)
				{
					ik.references.root.position = new Vector3(pelvisTarget.position.x, ik.references.root.position.y, pelvisTarget.position.z);
					Vector3 forward = Vector3.Cross(pelvisTarget.rotation * pelvisTargetRight, ik.references.root.up);
					forward.y = 0f;
					ik.references.root.rotation = Quaternion.LookRotation(forward);
					ik.references.pelvis.position = pelvisTarget.position;
					ik.references.pelvis.rotation = pelvisTarget.rotation;
				}
				else if (leftFootTarget != null && rightFootTarget != null)
				{
					ik.references.root.position = Vector3.Lerp(leftFootTarget.position, rightFootTarget.position, 0.5f);
				}
			}
		}

		private void OnDestroy()
		{
			if (ik != null)
			{
				IKSolverVR solver = ik.solver;
				solver.OnPreUpdate = (IKSolver.UpdateDelegate)Delegate.Remove(solver.OnPreUpdate, new IKSolver.UpdateDelegate(OnPreUpdate));
			}
		}
	}
}
