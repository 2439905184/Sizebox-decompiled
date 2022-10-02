using System;
using UnityEngine;

namespace RootMotion.Dynamics
{
	public abstract class RagdollCreator : MonoBehaviour
	{
		[Serializable]
		public enum ColliderType
		{
			Box = 0,
			Capsule = 1
		}

		[Serializable]
		public enum JointType
		{
			Configurable = 0,
			Character = 1
		}

		[Serializable]
		public enum Direction
		{
			X = 0,
			Y = 1,
			Z = 2
		}

		public struct CreateJointParams
		{
			public struct Limits
			{
				public float minSwing;

				public float maxSwing;

				public float swing2;

				public float twist;

				public Limits(float minSwing, float maxSwing, float swing2, float twist)
				{
					this.minSwing = minSwing;
					this.maxSwing = maxSwing;
					this.swing2 = swing2;
					this.twist = twist;
				}
			}

			public Rigidbody rigidbody;

			public Rigidbody connectedBody;

			public Transform child;

			public Vector3 worldSwingAxis;

			public Limits limits;

			public JointType type;

			public CreateJointParams(Rigidbody rigidbody, Rigidbody connectedBody, Transform child, Vector3 worldSwingAxis, Limits limits, JointType type)
			{
				this.rigidbody = rigidbody;
				this.connectedBody = connectedBody;
				this.child = child;
				this.worldSwingAxis = worldSwingAxis;
				this.limits = limits;
				this.type = type;
			}
		}

		public static void ClearAll(Transform root)
		{
			if (root == null)
			{
				return;
			}
			Transform transform = root;
			Animator componentInChildren = root.GetComponentInChildren<Animator>();
			if (componentInChildren != null && componentInChildren.isHuman)
			{
				Transform boneTransform = componentInChildren.GetBoneTransform(HumanBodyBones.Hips);
				if (boneTransform != null && boneTransform.GetComponentsInChildren<Transform>().Length > 2)
				{
					transform = boneTransform;
				}
			}
			Transform[] componentsInChildren = transform.GetComponentsInChildren<Transform>();
			if (componentsInChildren.Length >= 2)
			{
				for (int i = ((!(componentInChildren != null) || !componentInChildren.isHuman) ? 1 : 0); i < componentsInChildren.Length; i++)
				{
					ClearTransform(componentsInChildren[i]);
				}
			}
		}

		protected static void ClearTransform(Transform transform)
		{
			if (transform == null)
			{
				return;
			}
			Collider[] components = transform.GetComponents<Collider>();
			foreach (Collider collider in components)
			{
				if (collider != null && !collider.isTrigger)
				{
					UnityEngine.Object.DestroyImmediate(collider);
				}
			}
			Joint component = transform.GetComponent<Joint>();
			if (component != null)
			{
				UnityEngine.Object.DestroyImmediate(component);
			}
			Rigidbody component2 = transform.GetComponent<Rigidbody>();
			if (component2 != null && !transform.tag.Equals("Equipment"))
			{
				UnityEngine.Object.DestroyImmediate(component2);
			}
		}

		protected static void CreateCollider(Transform t, Vector3 startPoint, Vector3 endPoint, ColliderType colliderType, float lengthOverlap, float width)
		{
			Vector3 direction = endPoint - startPoint;
			float num = direction.magnitude * (1f + lengthOverlap);
			Vector3 axisVectorToDirection = AxisTools.GetAxisVectorToDirection(t, direction);
			t.gameObject.AddComponent<Rigidbody>();
			float scaleF = GetScaleF(t);
			switch (colliderType)
			{
			case ColliderType.Capsule:
			{
				CapsuleCollider capsuleCollider = t.gameObject.AddComponent<CapsuleCollider>();
				capsuleCollider.height = Mathf.Abs(num / scaleF);
				capsuleCollider.radius = Mathf.Abs(width * 0.75f / scaleF);
				capsuleCollider.direction = DirectionVector3ToInt(axisVectorToDirection);
				capsuleCollider.center = t.InverseTransformPoint(Vector3.Lerp(startPoint, endPoint, 0.5f));
				break;
			}
			case ColliderType.Box:
			{
				Vector3 vector = Vector3.Scale(axisVectorToDirection, new Vector3(num, num, num));
				if (vector.x == 0f)
				{
					vector.x = width;
				}
				if (vector.y == 0f)
				{
					vector.y = width;
				}
				if (vector.z == 0f)
				{
					vector.z = width;
				}
				BoxCollider boxCollider = t.gameObject.AddComponent<BoxCollider>();
				boxCollider.size = vector / scaleF;
				boxCollider.size = new Vector3(Mathf.Abs(boxCollider.size.x), Mathf.Abs(boxCollider.size.y), Mathf.Abs(boxCollider.size.z));
				boxCollider.center = t.InverseTransformPoint(Vector3.Lerp(startPoint, endPoint, 0.5f));
				break;
			}
			}
		}

		protected static void CreateCollider(Transform t, Vector3 startPoint, Vector3 endPoint, ColliderType colliderType, float lengthOverlap, float width, float proportionAspect, Vector3 widthDirection)
		{
			if (colliderType == ColliderType.Capsule)
			{
				CreateCollider(t, startPoint, endPoint, colliderType, lengthOverlap, width * proportionAspect);
				return;
			}
			Vector3 direction = endPoint - startPoint;
			float num = direction.magnitude * (1f + lengthOverlap);
			Vector3 axisVectorToDirection = AxisTools.GetAxisVectorToDirection(t, direction);
			Vector3 vector = AxisTools.GetAxisVectorToDirection(t, widthDirection);
			if (vector == axisVectorToDirection)
			{
				Debug.LogWarning("Width axis = height axis on " + t.name, t);
				vector = new Vector3(axisVectorToDirection.y, axisVectorToDirection.z, axisVectorToDirection.x);
			}
			t.gameObject.AddComponent<Rigidbody>();
			Vector3 vector2 = Vector3.Scale(axisVectorToDirection, new Vector3(num, num, num));
			Vector3 vector3 = Vector3.Scale(vector, new Vector3(width, width, width));
			Vector3 vector4 = vector2 + vector3;
			if (vector4.x == 0f)
			{
				vector4.x = width * proportionAspect;
			}
			if (vector4.y == 0f)
			{
				vector4.y = width * proportionAspect;
			}
			if (vector4.z == 0f)
			{
				vector4.z = width * proportionAspect;
			}
			BoxCollider boxCollider = t.gameObject.AddComponent<BoxCollider>();
			boxCollider.size = vector4 / GetScaleF(t);
			boxCollider.center = t.InverseTransformPoint(Vector3.Lerp(startPoint, endPoint, 0.5f));
		}

		protected static float GetScaleF(Transform t)
		{
			Vector3 lossyScale = t.lossyScale;
			return (lossyScale.x + lossyScale.y + lossyScale.z) / 3f;
		}

		protected static Vector3 Abs(Vector3 v)
		{
			Vector3Abs(ref v);
			return v;
		}

		protected static void Vector3Abs(ref Vector3 v)
		{
			v.x = Mathf.Abs(v.x);
			v.y = Mathf.Abs(v.y);
			v.z = Mathf.Abs(v.z);
		}

		protected static Vector3 DirectionIntToVector3(int dir)
		{
			switch (dir)
			{
			case 0:
				return Vector3.right;
			case 1:
				return Vector3.up;
			default:
				return Vector3.forward;
			}
		}

		protected static Vector3 DirectionToVector3(Direction dir)
		{
			switch (dir)
			{
			case Direction.X:
				return Vector3.right;
			case Direction.Y:
				return Vector3.up;
			default:
				return Vector3.forward;
			}
		}

		protected static int DirectionVector3ToInt(Vector3 dir)
		{
			float f = Vector3.Dot(dir, Vector3.right);
			float f2 = Vector3.Dot(dir, Vector3.up);
			float f3 = Vector3.Dot(dir, Vector3.forward);
			float num = Mathf.Abs(f);
			float num2 = Mathf.Abs(f2);
			float num3 = Mathf.Abs(f3);
			int result = 0;
			if (num2 > num && num2 > num3)
			{
				result = 1;
			}
			if (num3 > num && num3 > num2)
			{
				result = 2;
			}
			return result;
		}

		protected static Vector3 GetLocalOrthoDirection(Transform transform, Vector3 worldDir)
		{
			worldDir = worldDir.normalized;
			float f = Vector3.Dot(worldDir, transform.right);
			float f2 = Vector3.Dot(worldDir, transform.up);
			float f3 = Vector3.Dot(worldDir, transform.forward);
			float num = Mathf.Abs(f);
			float num2 = Mathf.Abs(f2);
			float num3 = Mathf.Abs(f3);
			Vector3 vector = Vector3.right;
			if (num2 > num && num2 > num3)
			{
				vector = Vector3.up;
			}
			if (num3 > num && num3 > num2)
			{
				vector = Vector3.forward;
			}
			if (Vector3.Dot(worldDir, transform.rotation * vector) < 0f)
			{
				vector = -vector;
			}
			return vector;
		}

		protected static Rigidbody GetConnectedBody(Transform bone, ref Transform[] bones)
		{
			if (bone.parent == null)
			{
				return null;
			}
			Transform[] array = bones;
			foreach (Transform transform in array)
			{
				if (bone.parent == transform && transform.GetComponent<Rigidbody>() != null)
				{
					return transform.GetComponent<Rigidbody>();
				}
			}
			return GetConnectedBody(bone.parent, ref bones);
		}

		protected static void CreateJoint(CreateJointParams p)
		{
			Vector3 localOrthoDirection = GetLocalOrthoDirection(p.rigidbody.transform, p.worldSwingAxis);
			Vector3 rhs = Vector3.forward;
			if (p.child != null)
			{
				rhs = GetLocalOrthoDirection(p.rigidbody.transform, p.child.position - p.rigidbody.transform.position);
			}
			else if (p.connectedBody != null)
			{
				rhs = GetLocalOrthoDirection(p.rigidbody.transform, p.rigidbody.transform.position - p.connectedBody.transform.position);
			}
			Vector3 vector = Vector3.Cross(localOrthoDirection, rhs);
			if (p.type == JointType.Configurable)
			{
				ConfigurableJoint configurableJoint = p.rigidbody.gameObject.AddComponent<ConfigurableJoint>();
				configurableJoint.enablePreprocessing = false;
				configurableJoint.connectedBody = p.connectedBody;
				ConfigurableJointMotion configurableJointMotion = ((!(p.connectedBody != null)) ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Locked);
				ConfigurableJointMotion configurableJointMotion2 = ((p.connectedBody != null) ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Free);
				configurableJoint.xMotion = configurableJointMotion;
				configurableJoint.yMotion = configurableJointMotion;
				configurableJoint.zMotion = configurableJointMotion;
				configurableJoint.angularXMotion = configurableJointMotion2;
				configurableJoint.angularYMotion = configurableJointMotion2;
				configurableJoint.angularZMotion = configurableJointMotion2;
				if (p.connectedBody != null)
				{
					configurableJoint.axis = localOrthoDirection;
					configurableJoint.secondaryAxis = vector;
					configurableJoint.lowAngularXLimit = ToSoftJointLimit(p.limits.minSwing);
					configurableJoint.highAngularXLimit = ToSoftJointLimit(p.limits.maxSwing);
					configurableJoint.angularYLimit = ToSoftJointLimit(p.limits.swing2);
					configurableJoint.angularZLimit = ToSoftJointLimit(p.limits.twist);
				}
				configurableJoint.anchor = Vector3.zero;
			}
			else if (!(p.connectedBody == null))
			{
				CharacterJoint characterJoint = p.rigidbody.gameObject.AddComponent<CharacterJoint>();
				characterJoint.enablePreprocessing = false;
				characterJoint.connectedBody = p.connectedBody;
				characterJoint.axis = localOrthoDirection;
				characterJoint.swingAxis = vector;
				characterJoint.lowTwistLimit = ToSoftJointLimit(p.limits.minSwing);
				characterJoint.highTwistLimit = ToSoftJointLimit(p.limits.maxSwing);
				characterJoint.swing1Limit = ToSoftJointLimit(p.limits.swing2);
				characterJoint.swing2Limit = ToSoftJointLimit(p.limits.twist);
				characterJoint.anchor = Vector3.zero;
			}
		}

		private static SoftJointLimit ToSoftJointLimit(float limit)
		{
			SoftJointLimit result = default(SoftJointLimit);
			result.limit = limit;
			return result;
		}
	}
}
