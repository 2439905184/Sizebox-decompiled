using UnityEngine;

namespace RootMotion.Dynamics
{
	public static class JointConverter
	{
		public static void ToConfigurable(GameObject root)
		{
			int num = 0;
			CharacterJoint[] componentsInChildren = root.GetComponentsInChildren<CharacterJoint>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				CharacterToConfigurable(componentsInChildren[i]);
				num++;
			}
			HingeJoint[] componentsInChildren2 = root.GetComponentsInChildren<HingeJoint>();
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				HingeToConfigurable(componentsInChildren2[j]);
				num++;
			}
			FixedJoint[] componentsInChildren3 = root.GetComponentsInChildren<FixedJoint>();
			for (int k = 0; k < componentsInChildren3.Length; k++)
			{
				FixedToConfigurable(componentsInChildren3[k]);
				num++;
			}
			SpringJoint[] componentsInChildren4 = root.GetComponentsInChildren<SpringJoint>();
			for (int l = 0; l < componentsInChildren4.Length; l++)
			{
				SpringToConfigurable(componentsInChildren4[l]);
				num++;
			}
			if (num > 0)
			{
				Debug.Log(num + " joints were successfully converted to ConfigurableJoints.");
			}
			else
			{
				Debug.Log("No joints found in the children of " + root.name + " to convert to ConfigurableJoints.");
			}
		}

		public static void HingeToConfigurable(HingeJoint src)
		{
			ConfigurableJoint conf = src.gameObject.AddComponent<ConfigurableJoint>();
			ConvertJoint(ref conf, src);
			conf.secondaryAxis = Vector3.zero;
			conf.xMotion = ConfigurableJointMotion.Locked;
			conf.yMotion = ConfigurableJointMotion.Locked;
			conf.zMotion = ConfigurableJointMotion.Locked;
			conf.angularXMotion = (src.useLimits ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Free);
			conf.angularYMotion = ConfigurableJointMotion.Locked;
			conf.angularZMotion = ConfigurableJointMotion.Locked;
			conf.highAngularXLimit = ConvertToHighSoftJointLimit(src.limits, src.spring, src.useSpring);
			conf.angularXLimitSpring = ConvertToSoftJointLimitSpring(src.limits, src.spring, src.useSpring);
			conf.lowAngularXLimit = ConvertToLowSoftJointLimit(src.limits, src.spring, src.useSpring);
			if (src.useMotor)
			{
				Debug.LogWarning("Can not convert HingeJoint Motor to ConfigurableJoint.");
			}
			Object.DestroyImmediate(src);
		}

		public static void FixedToConfigurable(FixedJoint src)
		{
			ConfigurableJoint conf = src.gameObject.AddComponent<ConfigurableJoint>();
			ConvertJoint(ref conf, src);
			conf.secondaryAxis = Vector3.zero;
			conf.xMotion = ConfigurableJointMotion.Locked;
			conf.yMotion = ConfigurableJointMotion.Locked;
			conf.zMotion = ConfigurableJointMotion.Locked;
			conf.angularXMotion = ConfigurableJointMotion.Locked;
			conf.angularYMotion = ConfigurableJointMotion.Locked;
			conf.angularZMotion = ConfigurableJointMotion.Locked;
			Object.DestroyImmediate(src);
		}

		public static void SpringToConfigurable(SpringJoint src)
		{
			ConfigurableJoint conf = src.gameObject.AddComponent<ConfigurableJoint>();
			ConvertJoint(ref conf, src);
			conf.xMotion = ConfigurableJointMotion.Limited;
			conf.yMotion = ConfigurableJointMotion.Limited;
			conf.zMotion = ConfigurableJointMotion.Limited;
			conf.angularXMotion = ConfigurableJointMotion.Free;
			conf.angularYMotion = ConfigurableJointMotion.Free;
			conf.angularZMotion = ConfigurableJointMotion.Free;
			SoftJointLimit linearLimit = default(SoftJointLimit);
			linearLimit.bounciness = 0f;
			linearLimit.limit = src.maxDistance;
			conf.linearLimit = linearLimit;
			SoftJointLimitSpring linearLimitSpring = default(SoftJointLimitSpring);
			linearLimitSpring.damper = src.damper;
			linearLimitSpring.spring = src.spring;
			conf.linearLimitSpring = linearLimitSpring;
			Object.DestroyImmediate(src);
		}

		public static void CharacterToConfigurable(CharacterJoint src)
		{
			ConfigurableJoint conf = src.gameObject.AddComponent<ConfigurableJoint>();
			ConvertJoint(ref conf, src);
			conf.secondaryAxis = src.swingAxis;
			conf.xMotion = ConfigurableJointMotion.Locked;
			conf.yMotion = ConfigurableJointMotion.Locked;
			conf.zMotion = ConfigurableJointMotion.Locked;
			conf.angularXMotion = ConfigurableJointMotion.Limited;
			conf.angularYMotion = ConfigurableJointMotion.Limited;
			conf.angularZMotion = ConfigurableJointMotion.Limited;
			conf.highAngularXLimit = CopyLimit(src.highTwistLimit);
			conf.lowAngularXLimit = CopyLimit(src.lowTwistLimit);
			conf.angularYLimit = CopyLimit(src.swing1Limit);
			conf.angularZLimit = CopyLimit(src.swing2Limit);
			conf.angularXLimitSpring = CopyLimitSpring(src.twistLimitSpring);
			conf.angularYZLimitSpring = CopyLimitSpring(src.swingLimitSpring);
			conf.enableCollision = src.enableCollision;
			conf.projectionMode = (src.enableProjection ? JointProjectionMode.PositionAndRotation : JointProjectionMode.None);
			conf.projectionAngle = src.projectionAngle;
			conf.projectionDistance = src.projectionDistance;
			Object.DestroyImmediate(src);
		}

		private static void ConvertJoint(ref ConfigurableJoint conf, Joint src)
		{
			conf.anchor = src.anchor;
			conf.autoConfigureConnectedAnchor = src.autoConfigureConnectedAnchor;
			conf.axis = src.axis;
			conf.breakForce = src.breakForce;
			conf.breakTorque = src.breakTorque;
			conf.connectedAnchor = src.connectedAnchor;
			conf.connectedBody = src.connectedBody;
			conf.enableCollision = src.enableCollision;
		}

		private static SoftJointLimit ConvertToHighSoftJointLimit(JointLimits src, JointSpring spring, bool useSpring)
		{
			SoftJointLimit result = default(SoftJointLimit);
			result.limit = 0f - src.max;
			result.bounciness = src.bounciness;
			return result;
		}

		private static SoftJointLimit ConvertToLowSoftJointLimit(JointLimits src, JointSpring spring, bool useSpring)
		{
			SoftJointLimit result = default(SoftJointLimit);
			result.limit = 0f - src.min;
			result.bounciness = src.bounciness;
			return result;
		}

		private static SoftJointLimitSpring ConvertToSoftJointLimitSpring(JointLimits src, JointSpring spring, bool useSpring)
		{
			SoftJointLimitSpring result = default(SoftJointLimitSpring);
			result.damper = (useSpring ? spring.damper : 0f);
			result.spring = (useSpring ? spring.spring : 0f);
			return result;
		}

		private static SoftJointLimit CopyLimit(SoftJointLimit src)
		{
			SoftJointLimit result = default(SoftJointLimit);
			result.limit = src.limit;
			result.bounciness = src.bounciness;
			return result;
		}

		private static SoftJointLimitSpring CopyLimitSpring(SoftJointLimitSpring src)
		{
			SoftJointLimitSpring result = default(SoftJointLimitSpring);
			result.damper = src.damper;
			result.spring = src.spring;
			return result;
		}
	}
}
