using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
	[AddComponentMenu("Scripts/RootMotion.FinalIK/Grounder/Grounder VRIK")]
	public class GrounderVRIK : Grounder
	{
		[Tooltip("Reference to the VRIK componet.")]
		public VRIK ik;

		private Transform[] feet = new Transform[2];

		[ContextMenu("TUTORIAL VIDEO")]
		private void OpenTutorial()
		{
		}

		[ContextMenu("User Manual")]
		protected override void OpenUserManual()
		{
		}

		[ContextMenu("Scrpt Reference")]
		protected override void OpenScriptReference()
		{
		}

		public override void ResetPosition()
		{
			solver.Reset();
		}

		private bool IsReadyToInitiate()
		{
			if (ik == null)
			{
				return false;
			}
			if (!ik.solver.initiated)
			{
				return false;
			}
			return true;
		}

		private void Update()
		{
			weight = Mathf.Clamp(weight, 0f, 1f);
			if (!(weight <= 0f) && !initiated && IsReadyToInitiate())
			{
				Initiate();
			}
		}

		private void Initiate()
		{
			feet = new Transform[2];
			feet[0] = ik.references.leftFoot;
			feet[1] = ik.references.rightFoot;
			IKSolverVR iKSolverVR = ik.solver;
			iKSolverVR.OnPreUpdate = (IKSolver.UpdateDelegate)Delegate.Combine(iKSolverVR.OnPreUpdate, new IKSolver.UpdateDelegate(OnSolverUpdate));
			IKSolverVR iKSolverVR2 = ik.solver;
			iKSolverVR2.OnPostUpdate = (IKSolver.UpdateDelegate)Delegate.Combine(iKSolverVR2.OnPostUpdate, new IKSolver.UpdateDelegate(OnPostSolverUpdate));
			solver.Initiate(ik.references.root, feet);
			initiated = true;
		}

		private void OnSolverUpdate()
		{
			if (base.enabled && !(weight <= 0f))
			{
				if (OnPreGrounder != null)
				{
					OnPreGrounder();
				}
				solver.Update();
				ik.references.pelvis.position += solver.pelvis.IKOffset * weight;
				ik.solver.AddPositionOffset(IKSolverVR.PositionOffset.LeftFoot, (solver.legs[0].IKPosition - ik.references.leftFoot.position) * weight);
				ik.solver.AddPositionOffset(IKSolverVR.PositionOffset.RightFoot, (solver.legs[1].IKPosition - ik.references.rightFoot.position) * weight);
				if (OnPostGrounder != null)
				{
					OnPostGrounder();
				}
			}
		}

		private void SetLegIK(IKSolverVR.PositionOffset positionOffset, Transform bone, Grounding.Leg leg)
		{
			ik.solver.AddPositionOffset(positionOffset, (leg.IKPosition - bone.position) * weight);
		}

		private void OnPostSolverUpdate()
		{
			ik.references.leftFoot.rotation = Quaternion.Slerp(Quaternion.identity, solver.legs[0].rotationOffset, weight) * ik.references.leftFoot.rotation;
			ik.references.rightFoot.rotation = Quaternion.Slerp(Quaternion.identity, solver.legs[1].rotationOffset, weight) * ik.references.rightFoot.rotation;
		}

		private void OnDrawGizmosSelected()
		{
			if (ik == null)
			{
				ik = GetComponent<VRIK>();
			}
			if (ik == null)
			{
				ik = GetComponentInParent<VRIK>();
			}
			if (ik == null)
			{
				ik = GetComponentInChildren<VRIK>();
			}
		}

		private void OnDestroy()
		{
			if (initiated && ik != null)
			{
				IKSolverVR iKSolverVR = ik.solver;
				iKSolverVR.OnPreUpdate = (IKSolver.UpdateDelegate)Delegate.Remove(iKSolverVR.OnPreUpdate, new IKSolver.UpdateDelegate(OnSolverUpdate));
				IKSolverVR iKSolverVR2 = ik.solver;
				iKSolverVR2.OnPostUpdate = (IKSolver.UpdateDelegate)Delegate.Remove(iKSolverVR2.OnPostUpdate, new IKSolver.UpdateDelegate(OnPostSolverUpdate));
			}
		}
	}
}
