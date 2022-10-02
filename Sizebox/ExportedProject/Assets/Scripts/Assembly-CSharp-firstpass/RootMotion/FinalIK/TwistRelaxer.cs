using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
	public class TwistRelaxer : MonoBehaviour
	{
		public IK ik;

		[Tooltip("The weight of relaxing the twist of this Transform")]
		[Range(0f, 1f)]
		public float weight = 1f;

		[Tooltip("If 0.5, this Transform will be twisted half way from parent to child. If 1, the twist angle will be locked to the child and will rotate with along with it.")]
		[Range(0f, 1f)]
		public float parentChildCrossfade = 0.5f;

		[Tooltip("Rotation offset around the twist axis.")]
		[Range(-180f, 180f)]
		public float twistAngleOffset;

		private Vector3 twistAxis = Vector3.right;

		private Vector3 axis = Vector3.forward;

		private Vector3 axisRelativeToParentDefault;

		private Vector3 axisRelativeToChildDefault;

		private Transform parent;

		private Transform child;

		public void Relax()
		{
			if (!(weight <= 0f))
			{
				Quaternion rotation = base.transform.rotation;
				Quaternion quaternion = Quaternion.AngleAxis(twistAngleOffset, rotation * twistAxis);
				rotation = quaternion * rotation;
				Vector3 a = quaternion * parent.rotation * axisRelativeToParentDefault;
				Vector3 b = quaternion * child.rotation * axisRelativeToChildDefault;
				Vector3 vector = Vector3.Slerp(a, b, parentChildCrossfade);
				vector = Quaternion.Inverse(Quaternion.LookRotation(rotation * axis, rotation * twistAxis)) * vector;
				float num = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
				Quaternion rotation2 = child.rotation;
				base.transform.rotation = Quaternion.AngleAxis(num * weight, rotation * twistAxis) * rotation;
				child.rotation = rotation2;
			}
		}

		private void Start()
		{
			parent = base.transform.parent;
			if (base.transform.childCount == 0)
			{
				Transform[] componentsInChildren = parent.GetComponentsInChildren<Transform>();
				for (int i = 1; i < componentsInChildren.Length; i++)
				{
					if (componentsInChildren[i] != base.transform)
					{
						child = componentsInChildren[i];
						break;
					}
				}
			}
			else
			{
				child = base.transform.GetChild(0);
			}
			twistAxis = base.transform.InverseTransformDirection(child.position - base.transform.position);
			axis = new Vector3(twistAxis.y, twistAxis.z, twistAxis.x);
			Vector3 vector = base.transform.rotation * axis;
			axisRelativeToParentDefault = Quaternion.Inverse(parent.rotation) * vector;
			axisRelativeToChildDefault = Quaternion.Inverse(child.rotation) * vector;
			if (ik != null)
			{
				IKSolver iKSolver = ik.GetIKSolver();
				iKSolver.OnPostUpdate = (IKSolver.UpdateDelegate)Delegate.Combine(iKSolver.OnPostUpdate, new IKSolver.UpdateDelegate(OnPostUpdate));
			}
		}

		private void OnPostUpdate()
		{
			if (ik != null)
			{
				Relax();
			}
		}

		private void LateUpdate()
		{
			if (ik == null)
			{
				Relax();
			}
		}

		private void OnDestroy()
		{
			if (ik != null)
			{
				IKSolver iKSolver = ik.GetIKSolver();
				iKSolver.OnPostUpdate = (IKSolver.UpdateDelegate)Delegate.Remove(iKSolver.OnPostUpdate, new IKSolver.UpdateDelegate(OnPostUpdate));
			}
		}
	}
}
