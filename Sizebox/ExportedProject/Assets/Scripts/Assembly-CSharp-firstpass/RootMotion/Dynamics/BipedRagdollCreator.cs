using System;
using UnityEngine;

namespace RootMotion.Dynamics
{
	[HelpURL("https://www.youtube.com/watch?v=y-luLRVmL7E&index=1&list=PLVxSIA1OaTOuE2SB9NUbckQ9r2hTg4mvL")]
	[AddComponentMenu("Scripts/RootMotion.Dynamics/Ragdoll Manager/Biped Ragdoll Creator")]
	public class BipedRagdollCreator : RagdollCreator
	{
		[Serializable]
		public struct Options
		{
			public float weight;

			[Header("Optional Bones")]
			public bool spine;

			public bool chest;

			public bool hands;

			public bool feet;

			[Header("Joints")]
			public JointType joints;

			public float jointRange;

			[Header("Colliders")]
			public float colliderLengthOverlap;

			public ColliderType torsoColliders;

			public ColliderType headCollider;

			public ColliderType armColliders;

			public ColliderType handColliders;

			public ColliderType legColliders;

			public ColliderType footColliders;

			public static Options Default
			{
				get
				{
					Options result = default(Options);
					result.weight = 75f;
					result.colliderLengthOverlap = 0.1f;
					result.jointRange = 1f;
					result.chest = true;
					result.headCollider = ColliderType.Capsule;
					result.armColliders = ColliderType.Capsule;
					result.hands = true;
					result.handColliders = ColliderType.Capsule;
					result.legColliders = ColliderType.Capsule;
					result.feet = true;
					return result;
				}
			}
		}

		public bool canBuild;

		public BipedRagdollReferences references;

		public Options options = Options.Default;

		[ContextMenu("User Manual")]
		private void OpenUserManual()
		{
			Application.OpenURL("http://root-motion.com/puppetmasterdox/html/page1.html");
		}

		[ContextMenu("Scrpt Reference")]
		private void OpenScriptReference()
		{
			Application.OpenURL("http://root-motion.com/puppetmasterdox/html/class_root_motion_1_1_dynamics_1_1_biped_ragdoll_creator.html#details");
		}

		[ContextMenu("TUTORIAL VIDEO")]
		private void OpenTutorial()
		{
			Application.OpenURL("https://www.youtube.com/watch?v=y-luLRVmL7E&index=1&list=PLVxSIA1OaTOuE2SB9NUbckQ9r2hTg4mvL");
		}

		public static Options AutodetectOptions(BipedRagdollReferences r)
		{
			Options @default = Options.Default;
			if (r.spine == null)
			{
				@default.spine = false;
			}
			if (r.chest == null)
			{
				@default.chest = false;
			}
			if (@default.chest && Vector3.Dot(r.root.up, r.chest.position - GetUpperArmCentroid(r)) > 0f)
			{
				@default.chest = false;
				if (r.spine != null)
				{
					@default.spine = true;
				}
			}
			return @default;
		}

		public static void Create(BipedRagdollReferences r, Options options)
		{
			string msg = string.Empty;
			if (!r.IsValid(ref msg))
			{
				Debug.LogWarning(msg);
				return;
			}
			RagdollCreator.ClearAll(r.root);
			CreateColliders(r, options);
			MassDistribution(r, options);
			CreateJoints(r, options);
		}

		private static void CreateColliders(BipedRagdollReferences r, Options options)
		{
			Vector3 upperArmToHeadCentroid = GetUpperArmToHeadCentroid(r);
			if (r.spine == null)
			{
				options.spine = false;
			}
			if (r.chest == null)
			{
				options.chest = false;
			}
			Vector3 widthDirection = r.rightUpperArm.position - r.leftUpperArm.position;
			float magnitude = widthDirection.magnitude;
			float proportionAspect = 0.6f;
			Vector3 vector = r.hips.position;
			float num = Vector3.Distance(r.head.position, r.root.position);
			if (Vector3.Distance(r.hips.position, r.root.position) < num * 0.2f)
			{
				vector = Vector3.Lerp(r.leftUpperLeg.position, r.rightUpperLeg.position, 0.5f);
			}
			Vector3 vector2 = (options.spine ? r.spine.position : (options.chest ? r.chest.position : upperArmToHeadCentroid));
			vector += (vector - upperArmToHeadCentroid) * 0.1f;
			float width = ((options.spine || options.chest) ? (magnitude * 0.8f) : magnitude);
			RagdollCreator.CreateCollider(r.hips, vector, vector2, options.torsoColliders, options.colliderLengthOverlap, width, proportionAspect, widthDirection);
			if (options.spine)
			{
				Vector3 startPoint = vector2;
				vector2 = (options.chest ? r.chest.position : upperArmToHeadCentroid);
				float width2 = (options.chest ? (magnitude * 0.75f) : magnitude);
				RagdollCreator.CreateCollider(r.spine, startPoint, vector2, options.torsoColliders, options.colliderLengthOverlap, width2, proportionAspect, widthDirection);
			}
			if (options.chest)
			{
				Vector3 startPoint2 = vector2;
				vector2 = upperArmToHeadCentroid;
				RagdollCreator.CreateCollider(r.chest, startPoint2, vector2, options.torsoColliders, options.colliderLengthOverlap, magnitude, proportionAspect, widthDirection);
			}
			Vector3 vector3 = vector2;
			Vector3 vector4 = vector3 + (vector3 - vector) * 0.45f;
			Vector3 onNormal = r.head.TransformVector(AxisTools.GetAxisVectorToDirection(r.head, vector4 - vector3));
			vector4 = vector3 + Vector3.Project(vector4 - vector3, onNormal).normalized * (vector4 - vector3).magnitude;
			RagdollCreator.CreateCollider(r.head, vector3, vector4, options.headCollider, options.colliderLengthOverlap, Vector3.Distance(vector3, vector4) * 0.8f);
			float num2 = 0.4f;
			float num3 = Vector3.Distance(r.leftUpperArm.position, r.leftLowerArm.position) * num2;
			RagdollCreator.CreateCollider(r.leftUpperArm, r.leftUpperArm.position, r.leftLowerArm.position, options.armColliders, options.colliderLengthOverlap, num3);
			RagdollCreator.CreateCollider(r.leftLowerArm, r.leftLowerArm.position, r.leftHand.position, options.armColliders, options.colliderLengthOverlap, num3 * 0.9f);
			float num4 = Vector3.Distance(r.rightUpperArm.position, r.rightLowerArm.position) * num2;
			RagdollCreator.CreateCollider(r.rightUpperArm, r.rightUpperArm.position, r.rightLowerArm.position, options.armColliders, options.colliderLengthOverlap, num4);
			RagdollCreator.CreateCollider(r.rightLowerArm, r.rightLowerArm.position, r.rightHand.position, options.armColliders, options.colliderLengthOverlap, num4 * 0.9f);
			float num5 = 0.3f;
			float num6 = Vector3.Distance(r.leftUpperLeg.position, r.leftLowerLeg.position) * num5;
			RagdollCreator.CreateCollider(r.leftUpperLeg, r.leftUpperLeg.position, r.leftLowerLeg.position, options.legColliders, options.colliderLengthOverlap, num6);
			RagdollCreator.CreateCollider(r.leftLowerLeg, r.leftLowerLeg.position, r.leftFoot.position, options.legColliders, options.colliderLengthOverlap, num6 * 0.9f);
			float num7 = Vector3.Distance(r.rightUpperLeg.position, r.rightLowerLeg.position) * num5;
			RagdollCreator.CreateCollider(r.rightUpperLeg, r.rightUpperLeg.position, r.rightLowerLeg.position, options.legColliders, options.colliderLengthOverlap, num7);
			RagdollCreator.CreateCollider(r.rightLowerLeg, r.rightLowerLeg.position, r.rightFoot.position, options.legColliders, options.colliderLengthOverlap, num7 * 0.9f);
			if (options.hands)
			{
				CreateHandCollider(r.leftHand, r.leftLowerArm, r.root, options);
				CreateHandCollider(r.rightHand, r.rightLowerArm, r.root, options);
			}
			if (options.feet)
			{
				CreateFootCollider(r.leftFoot, r.leftLowerLeg, r.leftUpperLeg, r.root, options);
				CreateFootCollider(r.rightFoot, r.rightLowerLeg, r.rightUpperLeg, r.root, options);
			}
		}

		private static void CreateHandCollider(Transform hand, Transform lowerArm, Transform root, Options options)
		{
			Vector3 onNormal = hand.TransformVector(AxisTools.GetAxisVectorToPoint(hand, GetChildCentroid(hand, lowerArm.position)));
			Vector3 vector = hand.position - (lowerArm.position - hand.position) * 0.75f;
			vector = hand.position + Vector3.Project(vector - hand.position, onNormal).normalized * (vector - hand.position).magnitude;
			RagdollCreator.CreateCollider(hand, hand.position, vector, options.handColliders, options.colliderLengthOverlap, Vector3.Distance(vector, hand.position) * 0.5f);
		}

		private static void CreateFootCollider(Transform foot, Transform lowerLeg, Transform upperLeg, Transform root, Options options)
		{
			float magnitude = (upperLeg.position - foot.position).magnitude;
			Vector3 onNormal = foot.TransformVector(AxisTools.GetAxisVectorToPoint(foot, GetChildCentroid(foot, foot.position + root.forward) + root.forward * magnitude * 0.2f));
			Vector3 vector = foot.position + root.forward * magnitude * 0.25f;
			vector = foot.position + Vector3.Project(vector - foot.position, onNormal).normalized * (vector - foot.position).magnitude;
			float num = Vector3.Distance(vector, foot.position) * 0.5f;
			Vector3 position = foot.position;
			Vector3 vector2 = ((Vector3.Dot(root.up, foot.position - root.position) < 0f) ? Vector3.zero : Vector3.Project(position - root.up * num * 0.5f - root.position, root.up));
			Vector3 vector3 = vector - position;
			position -= vector3 * 0.2f;
			RagdollCreator.CreateCollider(foot, position - vector2, vector - vector2, options.footColliders, options.colliderLengthOverlap, num);
		}

		private static Vector3 GetChildCentroid(Transform t, Vector3 fallback)
		{
			if (t.childCount == 0)
			{
				return fallback;
			}
			Vector3 zero = Vector3.zero;
			for (int i = 0; i < t.childCount; i++)
			{
				zero += t.GetChild(i).position;
			}
			return zero / t.childCount;
		}

		private static void MassDistribution(BipedRagdollReferences r, Options o)
		{
			int num = 3;
			if (r.spine == null)
			{
				o.spine = false;
				num--;
			}
			if (r.chest == null)
			{
				o.chest = false;
				num--;
			}
			float num2 = 0.508f / (float)num;
			float num3 = 0.0732f;
			float num4 = 0.027f;
			float num5 = 0.016f;
			float num6 = 0.0066f;
			float num7 = 0.0988f;
			float num8 = 0.0465f;
			float num9 = 0.0145f;
			r.hips.GetComponent<Rigidbody>().mass = num2 * o.weight;
			if (o.spine)
			{
				r.spine.GetComponent<Rigidbody>().mass = num2 * o.weight;
			}
			if (o.chest)
			{
				r.chest.GetComponent<Rigidbody>().mass = num2 * o.weight;
			}
			r.head.GetComponent<Rigidbody>().mass = num3 * o.weight;
			r.leftUpperArm.GetComponent<Rigidbody>().mass = num4 * o.weight;
			r.rightUpperArm.GetComponent<Rigidbody>().mass = r.leftUpperArm.GetComponent<Rigidbody>().mass;
			r.leftLowerArm.GetComponent<Rigidbody>().mass = num5 * o.weight;
			r.rightLowerArm.GetComponent<Rigidbody>().mass = r.leftLowerArm.GetComponent<Rigidbody>().mass;
			if (o.hands)
			{
				r.leftHand.GetComponent<Rigidbody>().mass = num6 * o.weight;
				r.rightHand.GetComponent<Rigidbody>().mass = r.leftHand.GetComponent<Rigidbody>().mass;
			}
			r.leftUpperLeg.GetComponent<Rigidbody>().mass = num7 * o.weight;
			r.rightUpperLeg.GetComponent<Rigidbody>().mass = r.leftUpperLeg.GetComponent<Rigidbody>().mass;
			r.leftLowerLeg.GetComponent<Rigidbody>().mass = num8 * o.weight;
			r.rightLowerLeg.GetComponent<Rigidbody>().mass = r.leftLowerLeg.GetComponent<Rigidbody>().mass;
			if (o.feet)
			{
				r.leftFoot.GetComponent<Rigidbody>().mass = num9 * o.weight;
				r.rightFoot.GetComponent<Rigidbody>().mass = r.leftFoot.GetComponent<Rigidbody>().mass;
			}
		}

		private static void CreateJoints(BipedRagdollReferences r, Options o)
		{
			if (r.spine == null)
			{
				o.spine = false;
			}
			if (r.chest == null)
			{
				o.chest = false;
			}
			float minSwing = -30f * o.jointRange;
			float maxSwing = 10f * o.jointRange;
			float swing = 25f * o.jointRange;
			float twist = 25f * o.jointRange;
			RagdollCreator.CreateJoint(new CreateJointParams(r.hips.GetComponent<Rigidbody>(), null, o.spine ? r.spine : (o.chest ? r.chest : r.head), r.root.right, new CreateJointParams.Limits(0f, 0f, 0f, 0f), o.joints));
			if (o.spine)
			{
				RagdollCreator.CreateJoint(new CreateJointParams(r.spine.GetComponent<Rigidbody>(), r.hips.GetComponent<Rigidbody>(), o.chest ? r.chest : r.head, r.root.right, new CreateJointParams.Limits(minSwing, maxSwing, swing, twist), o.joints));
			}
			if (o.chest)
			{
				RagdollCreator.CreateJoint(new CreateJointParams(r.chest.GetComponent<Rigidbody>(), o.spine ? r.spine.GetComponent<Rigidbody>() : r.hips.GetComponent<Rigidbody>(), r.head, r.root.right, new CreateJointParams.Limits(minSwing, maxSwing, swing, twist), o.joints));
			}
			Transform transform = (o.chest ? r.chest : (o.spine ? r.spine : r.hips));
			RagdollCreator.CreateJoint(new CreateJointParams(r.head.GetComponent<Rigidbody>(), transform.GetComponent<Rigidbody>(), null, r.root.right, new CreateJointParams.Limits(-30f, 30f, 30f, 85f), o.joints));
			CreateJointParams.Limits limits = new CreateJointParams.Limits(-35f * o.jointRange, 120f * o.jointRange, 85f * o.jointRange, 45f * o.jointRange);
			CreateJointParams.Limits limits2 = new CreateJointParams.Limits(0f, 140f * o.jointRange, 10f * o.jointRange, 45f * o.jointRange);
			CreateJointParams.Limits limits3 = new CreateJointParams.Limits(-50f * o.jointRange, 50f * o.jointRange, 50f * o.jointRange, 25f * o.jointRange);
			CreateLimbJoints(transform, r.leftUpperArm, r.leftLowerArm, r.leftHand, r.root, -r.root.right, o.joints, limits, limits2, limits3);
			CreateLimbJoints(transform, r.rightUpperArm, r.rightLowerArm, r.rightHand, r.root, r.root.right, o.joints, limits, limits2, limits3);
			CreateJointParams.Limits limits4 = new CreateJointParams.Limits(-120f * o.jointRange, 35f * o.jointRange, 85f * o.jointRange, 45f * o.jointRange);
			CreateJointParams.Limits limits5 = new CreateJointParams.Limits(0f, 140f * o.jointRange, 10f * o.jointRange, 45f * o.jointRange);
			CreateJointParams.Limits limits6 = new CreateJointParams.Limits(-50f * o.jointRange, 50f * o.jointRange, 50f * o.jointRange, 25f * o.jointRange);
			CreateLimbJoints(r.hips, r.leftUpperLeg, r.leftLowerLeg, r.leftFoot, r.root, -r.root.up, o.joints, limits4, limits5, limits6);
			CreateLimbJoints(r.hips, r.rightUpperLeg, r.rightLowerLeg, r.rightFoot, r.root, -r.root.up, o.joints, limits4, limits5, limits6);
		}

		private static void CreateLimbJoints(Transform connectedBone, Transform bone1, Transform bone2, Transform bone3, Transform root, Vector3 defaultWorldDirection, JointType jointType, CreateJointParams.Limits limits1, CreateJointParams.Limits limits2, CreateJointParams.Limits limits3)
		{
			Quaternion localRotation = bone1.localRotation;
			bone1.rotation = Quaternion.FromToRotation(bone1.rotation * (bone2.position - bone1.position), defaultWorldDirection) * bone1.rotation;
			Vector3 normalized = (bone2.position - bone1.position).normalized;
			Vector3 normalized2 = (bone3.position - bone2.position).normalized;
			Vector3 worldSwingAxis = -Vector3.Cross(normalized, normalized2);
			float num = Vector3.Angle(normalized, normalized2);
			bool flag = Mathf.Abs(Vector3.Dot(normalized, root.up)) > 0.5f;
			float num2 = (flag ? 100f : 1f);
			if (num < 0.01f * num2)
			{
				worldSwingAxis = ((!flag) ? ((Vector3.Dot(normalized, root.right) > 0f) ? root.up : (-root.up)) : ((Vector3.Dot(normalized, root.up) > 0f) ? root.right : (-root.right)));
			}
			RagdollCreator.CreateJoint(new CreateJointParams(bone1.GetComponent<Rigidbody>(), connectedBone.GetComponent<Rigidbody>(), bone2, worldSwingAxis, limits1, jointType));
			RagdollCreator.CreateJoint(new CreateJointParams(bone2.GetComponent<Rigidbody>(), bone1.GetComponent<Rigidbody>(), bone3, worldSwingAxis, new CreateJointParams.Limits(limits2.minSwing - num, limits2.maxSwing - num, limits2.swing2, limits2.twist), jointType));
			if (bone3.GetComponent<Rigidbody>() != null)
			{
				RagdollCreator.CreateJoint(new CreateJointParams(bone3.GetComponent<Rigidbody>(), bone2.GetComponent<Rigidbody>(), null, worldSwingAxis, limits3, jointType));
			}
			bone1.localRotation = localRotation;
		}

		public static void ClearBipedRagdoll(BipedRagdollReferences r)
		{
			Transform[] ragdollTransforms = r.GetRagdollTransforms();
			for (int i = 0; i < ragdollTransforms.Length; i++)
			{
				RagdollCreator.ClearTransform(ragdollTransforms[i]);
			}
		}

		public static bool IsClear(BipedRagdollReferences r)
		{
			Transform[] ragdollTransforms = r.GetRagdollTransforms();
			for (int i = 0; i < ragdollTransforms.Length; i++)
			{
				if (ragdollTransforms[i].GetComponent<Rigidbody>() != null)
				{
					return false;
				}
			}
			return true;
		}

		private static Vector3 GetUpperArmToHeadCentroid(BipedRagdollReferences r)
		{
			return Vector3.Lerp(GetUpperArmCentroid(r), r.head.position, 0.5f);
		}

		private static Vector3 GetUpperArmCentroid(BipedRagdollReferences r)
		{
			return Vector3.Lerp(r.leftUpperArm.position, r.rightUpperArm.position, 0.5f);
		}
	}
}
