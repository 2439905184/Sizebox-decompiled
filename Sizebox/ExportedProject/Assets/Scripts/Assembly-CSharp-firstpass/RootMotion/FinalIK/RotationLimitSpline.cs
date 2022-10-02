using UnityEngine;

namespace RootMotion.FinalIK
{
	[HelpURL("http://www.root-motion.com/finalikdox/html/page12.html")]
	[AddComponentMenu("Scripts/RootMotion.FinalIK/Rotation Limits/Rotation Limit Spline")]
	public class RotationLimitSpline : RotationLimit
	{
		[Range(0f, 180f)]
		public float twistLimit = 180f;

		[SerializeField]
		[HideInInspector]
		public AnimationCurve spline;

		[ContextMenu("User Manual")]
		private void OpenUserManual()
		{
			Application.OpenURL("http://www.root-motion.com/finalikdox/html/page12.html");
		}

		[ContextMenu("Scrpt Reference")]
		private void OpenScriptReference()
		{
			Application.OpenURL("http://www.root-motion.com/finalikdox/html/class_root_motion_1_1_final_i_k_1_1_rotation_limit_spline.html");
		}

		[ContextMenu("Support Group")]
		private void SupportGroup()
		{
			Application.OpenURL("https://groups.google.com/forum/#!forum/final-ik");
		}

		[ContextMenu("Asset Store Thread")]
		private void ASThread()
		{
			Application.OpenURL("http://forum.unity3d.com/threads/final-ik-full-body-ik-aim-look-at-fabrik-ccd-ik-1-0-released.222685/");
		}

		public void SetSpline(Keyframe[] keyframes)
		{
			spline.keys = keyframes;
		}

		protected override Quaternion LimitRotation(Quaternion rotation)
		{
			return RotationLimit.LimitTwist(LimitSwing(rotation), axis, base.secondaryAxis, twistLimit);
		}

		public Quaternion LimitSwing(Quaternion rotation)
		{
			if (axis == Vector3.zero)
			{
				return rotation;
			}
			if (rotation == Quaternion.identity)
			{
				return rotation;
			}
			Vector3 vector = rotation * axis;
			float num = RotationLimit.GetOrthogonalAngle(vector, base.secondaryAxis, axis);
			if (Vector3.Dot(vector, base.crossAxis) < 0f)
			{
				num = 180f + (180f - num);
			}
			float maxDegreesDelta = spline.Evaluate(num);
			Quaternion to = Quaternion.FromToRotation(axis, vector);
			Quaternion quaternion = Quaternion.RotateTowards(Quaternion.identity, to, maxDegreesDelta);
			return Quaternion.FromToRotation(vector, quaternion * axis) * rotation;
		}
	}
}
