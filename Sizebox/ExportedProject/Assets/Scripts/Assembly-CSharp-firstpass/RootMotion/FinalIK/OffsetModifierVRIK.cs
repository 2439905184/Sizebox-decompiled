using System;
using System.Collections;
using UnityEngine;

namespace RootMotion.FinalIK
{
	public abstract class OffsetModifierVRIK : MonoBehaviour
	{
		[Tooltip("The master weight")]
		public float weight = 1f;

		[Tooltip("Reference to the VRIK component")]
		public VRIK ik;

		private float lastTime;

		protected float deltaTime
		{
			get
			{
				return Time.time - lastTime;
			}
		}

		protected abstract void OnModifyOffset();

		protected virtual void Start()
		{
			StartCoroutine(Initiate());
		}

		private IEnumerator Initiate()
		{
			while (ik == null)
			{
				yield return null;
			}
			IKSolverVR solver = ik.solver;
			solver.OnPreUpdate = (IKSolver.UpdateDelegate)Delegate.Combine(solver.OnPreUpdate, new IKSolver.UpdateDelegate(ModifyOffset));
			lastTime = Time.time;
		}

		private void ModifyOffset()
		{
			if (base.enabled && !(weight <= 0f) && !(deltaTime <= 0f) && !(ik == null))
			{
				weight = Mathf.Clamp(weight, 0f, 1f);
				OnModifyOffset();
				lastTime = Time.time;
			}
		}

		protected virtual void OnDestroy()
		{
			if (ik != null)
			{
				IKSolverVR solver = ik.solver;
				solver.OnPreUpdate = (IKSolver.UpdateDelegate)Delegate.Remove(solver.OnPreUpdate, new IKSolver.UpdateDelegate(ModifyOffset));
			}
		}
	}
}
