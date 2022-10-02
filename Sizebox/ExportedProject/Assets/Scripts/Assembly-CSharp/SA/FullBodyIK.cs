using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace SA
{
	[Serializable]
	public class FullBodyIK
	{
		[Serializable]
		public class BodyBones
		{
			public Bone hips;

			public Bone spine;

			public Bone spine2;

			public Bone spine3;

			public Bone spine4;

			public Bone spineU
			{
				get
				{
					return spine4;
				}
			}
		}

		[Serializable]
		public class HeadBones
		{
			public Bone neck;

			public Bone head;

			public Bone leftEye;

			public Bone rightEye;
		}

		[Serializable]
		public class LegBones
		{
			public Bone leg;

			public Bone knee;

			public Bone foot;
		}

		[Serializable]
		public class ArmBones
		{
			public Bone shoulder;

			public Bone arm;

			public Bone[] armRoll;

			public Bone elbow;

			public Bone[] elbowRoll;

			public Bone wrist;

			public void Repair()
			{
				SafeResize(ref armRoll, 4);
				SafeResize(ref elbowRoll, 4);
			}
		}

		[Serializable]
		public class FingersBones
		{
			public Bone[] thumb;

			public Bone[] index;

			public Bone[] middle;

			public Bone[] ring;

			public Bone[] little;

			public void Repair()
			{
				SafeResize(ref thumb, 4);
				SafeResize(ref index, 4);
				SafeResize(ref middle, 4);
				SafeResize(ref ring, 4);
				SafeResize(ref little, 4);
			}
		}

		[Serializable]
		public class BodyEffectors
		{
			public Effector hips;
		}

		[Serializable]
		public class HeadEffectors
		{
			public Effector neck;

			public Effector head;

			public Effector eyes;
		}

		[Serializable]
		public class LegEffectors
		{
			public Effector knee;

			public Effector foot;
		}

		[Serializable]
		public class ArmEffectors
		{
			public Effector arm;

			public Effector elbow;

			public Effector wrist;
		}

		[Serializable]
		public class FingersEffectors
		{
			public Effector thumb;

			public Effector index;

			public Effector middle;

			public Effector ring;

			public Effector little;
		}

		public enum AutomaticBool
		{
			Auto = -1,
			Disable = 0,
			Enable = 1
		}

		public enum SyncDisplacement
		{
			Disable = 0,
			Firstframe = 1,
			Everyframe = 2
		}

		[Serializable]
		public class Settings
		{
			[Serializable]
			public class BodyIK
			{
				public bool forceSolveEnabled = true;

				public bool lowerSolveEnabled = true;

				public bool upperSolveEnabled = true;

				public bool computeWorldTransform = true;

				public bool shoulderSolveEnabled = true;

				public float shoulderSolveBendingRate = 0.25f;

				public bool shoulderLimitEnabled = true;

				public float shoulderLimitAngleYPlus = 30f;

				public float shoulderLimitAngleYMinus = 1f;

				public float shoulderLimitAngleZ = 30f;

				public float spineDirXLegToArmRate = 0.5f;

				public float spineDirXLegToArmToRate = 1f;

				public float spineDirYLerpRate = 0.5f;

				public float upperBodyMovingfixRate = 1f;

				public float upperHeadMovingfixRate = 0.8f;

				public float upperCenterLegTranslateRate = 0.5f;

				public float upperSpineTranslateRate = 0.65f;

				public float upperCenterLegRotateRate = 0.6f;

				public float upperSpineRotateRate = 0.9f;

				public float upperPostTranslateRate = 1f;

				public bool upperSolveHipsEnabled = true;

				public bool upperSolveSpineEnabled = true;

				public bool upperSolveSpine2Enabled = true;

				public bool upperSolveSpine3Enabled = true;

				public bool upperSolveSpine4Enabled = true;

				public float upperCenterLegLerpRate = 1f;

				public float upperSpineLerpRate = 1f;

				public bool upperDirXLimitEnabled = true;

				public float upperDirXLimitAngleY = 20f;

				public bool spineLimitEnabled = true;

				public bool spineAccurateLimitEnabled;

				public float spineLimitAngleX = 40f;

				public float spineLimitAngleY = 25f;

				public float upperContinuousPreTranslateRate = 0.2f;

				public float upperContinuousPreTranslateStableRate = 0.65f;

				public float upperContinuousCenterLegRotationStableRate;

				public float upperContinuousPostTranslateStableRate = 0.01f;

				public float upperContinuousSpineDirYLerpRate = 0.5f;

				public float upperNeckToCenterLegRate = 0.6f;

				public float upperNeckToSpineRate = 0.9f;

				public float upperEyesToCenterLegRate = 0.2f;

				public float upperEyesToSpineRate = 0.5f;

				public float upperEyesYawRate = 0.8f;

				public float upperEyesPitchUpRate = 0.25f;

				public float upperEyesPitchDownRate = 0.5f;

				public float upperEyesLimitYaw = 80f;

				public float upperEyesLimitPitchUp = 10f;

				public float upperEyesLimitPitchDown = 45f;

				public float upperEyesTraceAngle = 160f;
			}

			[Serializable]
			public class LimbIK
			{
				public bool legAlwaysSolveEnabled = true;

				public bool armAlwaysSolveEnabled;

				public float automaticKneeBaseAngle;

				public bool presolveKneeEnabled;

				public bool presolveElbowEnabled;

				public float presolveKneeRate = 1f;

				public float presolveKneeLerpAngle = 10f;

				public float presolveKneeLerpLengthRate = 0.1f;

				public float presolveElbowRate = 1f;

				public float presolveElbowLerpAngle = 10f;

				public float presolveElbowLerpLengthRate = 0.1f;

				public bool prefixLegEffectorEnabled = true;

				public float prefixLegUpperLimitAngle = 60f;

				public float prefixKneeUpperLimitAngle = 45f;

				public float legEffectorMinLengthRate = 0.1f;

				public float legEffectorMaxLengthRate = 0.9999f;

				public float armEffectorMaxLengthRate = 0.9999f;

				public bool armBasisForcefixEnabled = true;

				public float armBasisForcefixEffectorLengthRate = 0.99f;

				public float armBasisForcefixEffectorLengthLerpRate = 0.03f;

				public bool armEffectorBackfixEnabled = true;

				public bool armEffectorInnerfixEnabled = true;

				public float armEffectorBackBeginAngle = 5f;

				public float armEffectorBackCoreBeginAngle = -10f;

				public float armEffectorBackCoreEndAngle = -30f;

				public float armEffectorBackEndAngle = -160f;

				public float armEffectorBackCoreUpperAngle = 8f;

				public float armEffectorBackCoreLowerAngle = -15f;

				public float automaticElbowBaseAngle = 30f;

				public float automaticElbowLowerAngle = 90f;

				public float automaticElbowUpperAngle = 90f;

				public float automaticElbowBackUpperAngle = 180f;

				public float automaticElbowBackLowerAngle = 330f;

				public float elbowFrontInnerLimitAngle = 5f;

				public float elbowBackInnerLimitAngle;

				public bool wristLimitEnabled = true;

				public float wristLimitAngle = 90f;

				public bool footLimitEnabled = true;

				public float footLimitYaw = 45f;

				public float footLimitPitchUp = 45f;

				public float footLimitPitchDown = 60f;

				public float footLimitRoll = 45f;
			}

			[Serializable]
			public class HeadIK
			{
				public float neckLimitPitchUp = 15f;

				public float neckLimitPitchDown = 30f;

				public float neckLimitRoll = 5f;

				public float eyesToNeckPitchRate = 0.4f;

				public float headLimitYaw = 60f;

				public float headLimitPitchUp = 15f;

				public float headLimitPitchDown = 15f;

				public float headLimitRoll = 5f;

				public float eyesToHeadYawRate = 0.8f;

				public float eyesToHeadPitchRate = 0.5f;

				public float eyesTraceAngle = 110f;

				public float eyesLimitYaw = 40f;

				public float eyesLimitPitch = 12f;

				public float eyesYawRate = 0.796f;

				public float eyesPitchRate = 0.729f;

				public float eyesYawOuterRate = 0.356f;

				public float eyesYawInnerRate = 0.212f;
			}

			[Serializable]
			public class FingerIK
			{
			}

			public AutomaticBool animatorEnabled = AutomaticBool.Auto;

			public AutomaticBool resetTransforms = AutomaticBool.Auto;

			public SyncDisplacement syncDisplacement;

			public AutomaticBool shoulderDirYAsNeck = AutomaticBool.Auto;

			public bool automaticPrepareHumanoid = true;

			public bool automaticConfigureSpineEnabled;

			public bool automaticConfigureRollBonesEnabled;

			public bool rollBonesEnabled;

			public bool createEffectorTransform = true;

			public BodyIK bodyIK;

			public LimbIK limbIK;

			public HeadIK headIK;

			public FingerIK fingerIK;

			public void Prefix()
			{
				SafeNew(ref bodyIK);
				SafeNew(ref limbIK);
				SafeNew(ref headIK);
				SafeNew(ref fingerIK);
			}
		}

		[Serializable]
		public class EditorSettings
		{
			public bool isAdvanced;

			public int toolbarSelected;

			public bool isShowEffectorTransform;
		}

		public class InternalValues
		{
			public class BodyIK
			{
				public CachedDegreesToSin shoulderLimitThetaYPlus = CachedDegreesToSin.zero;

				public CachedDegreesToSin shoulderLimitThetaYMinus = CachedDegreesToSin.zero;

				public CachedDegreesToSin shoulderLimitThetaZ = CachedDegreesToSin.zero;

				public CachedRate01 upperCenterLegTranslateRate = CachedRate01.zero;

				public CachedRate01 upperSpineTranslateRate = CachedRate01.zero;

				public CachedRate01 upperPreTranslateRate = CachedRate01.zero;

				public CachedRate01 upperPostTranslateRate = CachedRate01.zero;

				public CachedRate01 upperCenterLegRotateRate = CachedRate01.zero;

				public CachedRate01 upperSpineRotateRate = CachedRate01.zero;

				public bool isFuzzyUpperCenterLegAndSpineRotationRate = true;

				public CachedDegreesToSin upperEyesLimitYaw = CachedDegreesToSin.zero;

				public CachedDegreesToSin upperEyesLimitPitchUp = CachedDegreesToSin.zero;

				public CachedDegreesToSin upperEyesLimitPitchDown = CachedDegreesToSin.zero;

				public CachedDegreesToCos upperEyesTraceTheta = CachedDegreesToCos.zero;

				public CachedDegreesToSin upperDirXLimitThetaY = CachedDegreesToSin.zero;

				public CachedScaledValue spineLimitAngleX = CachedScaledValue.zero;

				public CachedScaledValue spineLimitAngleY = CachedScaledValue.zero;

				public CachedRate01 upperContinuousPreTranslateRate = CachedRate01.zero;

				public CachedRate01 upperContinuousPreTranslateStableRate = CachedRate01.zero;

				public CachedRate01 upperContinuousCenterLegRotationStableRate = CachedRate01.zero;

				public CachedRate01 upperContinuousPostTranslateStableRate = CachedRate01.zero;

				public void Update(Settings.BodyIK settingsBodyIK)
				{
					if (shoulderLimitThetaYPlus._degrees != settingsBodyIK.shoulderLimitAngleYPlus)
					{
						shoulderLimitThetaYPlus._Reset(settingsBodyIK.shoulderLimitAngleYPlus);
					}
					if (shoulderLimitThetaYMinus._degrees != settingsBodyIK.shoulderLimitAngleYMinus)
					{
						shoulderLimitThetaYMinus._Reset(settingsBodyIK.shoulderLimitAngleYMinus);
					}
					if (shoulderLimitThetaZ._degrees != settingsBodyIK.shoulderLimitAngleZ)
					{
						shoulderLimitThetaZ._Reset(settingsBodyIK.shoulderLimitAngleZ);
					}
					if (upperCenterLegTranslateRate._value != settingsBodyIK.upperCenterLegTranslateRate || upperSpineTranslateRate._value != settingsBodyIK.upperSpineTranslateRate)
					{
						upperCenterLegTranslateRate._Reset(settingsBodyIK.upperCenterLegTranslateRate);
						upperSpineTranslateRate._Reset(Mathf.Max(settingsBodyIK.upperCenterLegTranslateRate, settingsBodyIK.upperSpineTranslateRate));
					}
					if (upperPostTranslateRate._value != settingsBodyIK.upperPostTranslateRate)
					{
						upperPostTranslateRate._Reset(settingsBodyIK.upperPostTranslateRate);
					}
					if (upperCenterLegRotateRate._value != settingsBodyIK.upperCenterLegRotateRate || upperSpineRotateRate._value != settingsBodyIK.upperSpineRotateRate)
					{
						upperCenterLegRotateRate._Reset(settingsBodyIK.upperCenterLegRotateRate);
						upperSpineRotateRate._Reset(Mathf.Max(settingsBodyIK.upperCenterLegRotateRate, settingsBodyIK.upperSpineRotateRate));
						isFuzzyUpperCenterLegAndSpineRotationRate = IsFuzzy(upperCenterLegRotateRate.value, upperSpineRotateRate.value);
					}
					if (upperEyesLimitYaw._degrees != settingsBodyIK.upperEyesLimitYaw)
					{
						upperEyesLimitYaw._Reset(settingsBodyIK.upperEyesLimitYaw);
					}
					if (upperEyesLimitPitchUp._degrees != settingsBodyIK.upperEyesLimitPitchUp)
					{
						upperEyesLimitPitchUp._Reset(settingsBodyIK.upperEyesLimitPitchUp);
					}
					if (upperEyesLimitPitchDown._degrees != settingsBodyIK.upperEyesLimitPitchDown)
					{
						upperEyesLimitPitchDown._Reset(settingsBodyIK.upperEyesLimitPitchDown);
					}
					if (upperEyesTraceTheta._degrees != settingsBodyIK.upperEyesTraceAngle)
					{
						upperEyesTraceTheta._Reset(settingsBodyIK.upperEyesTraceAngle);
					}
					if (spineLimitAngleX._a != settingsBodyIK.spineLimitAngleX)
					{
						spineLimitAngleX._Reset(settingsBodyIK.spineLimitAngleX, (float)Math.PI / 180f);
					}
					if (spineLimitAngleY._a != settingsBodyIK.spineLimitAngleY)
					{
						spineLimitAngleY._Reset(settingsBodyIK.spineLimitAngleY, (float)Math.PI / 180f);
					}
					if (upperDirXLimitThetaY._degrees != settingsBodyIK.upperDirXLimitAngleY)
					{
						upperDirXLimitThetaY._Reset(settingsBodyIK.upperDirXLimitAngleY);
					}
					if (upperContinuousPreTranslateRate._value != settingsBodyIK.upperContinuousPreTranslateRate)
					{
						upperContinuousPreTranslateRate._Reset(settingsBodyIK.upperContinuousPreTranslateRate);
					}
					if (upperContinuousPreTranslateStableRate._value != settingsBodyIK.upperContinuousPreTranslateStableRate)
					{
						upperContinuousPreTranslateStableRate._Reset(settingsBodyIK.upperContinuousPreTranslateStableRate);
					}
					if (upperContinuousCenterLegRotationStableRate._value != settingsBodyIK.upperContinuousCenterLegRotationStableRate)
					{
						upperContinuousCenterLegRotationStableRate._Reset(settingsBodyIK.upperContinuousCenterLegRotationStableRate);
					}
					if (upperContinuousPostTranslateStableRate._value != settingsBodyIK.upperContinuousPostTranslateStableRate)
					{
						upperContinuousPostTranslateStableRate._Reset(settingsBodyIK.upperContinuousPostTranslateStableRate);
					}
				}
			}

			public class LimbIK
			{
				public CachedDegreesToSin armEffectorBackBeginTheta = CachedDegreesToSin.zero;

				public CachedDegreesToSin armEffectorBackCoreBeginTheta = CachedDegreesToSin.zero;

				public CachedDegreesToCos armEffectorBackCoreEndTheta = CachedDegreesToCos.zero;

				public CachedDegreesToCos armEffectorBackEndTheta = CachedDegreesToCos.zero;

				public CachedDegreesToSin armEffectorBackCoreUpperTheta = CachedDegreesToSin.zero;

				public CachedDegreesToSin armEffectorBackCoreLowerTheta = CachedDegreesToSin.zero;

				public CachedDegreesToSin elbowFrontInnerLimitTheta = CachedDegreesToSin.zero;

				public CachedDegreesToSin elbowBackInnerLimitTheta = CachedDegreesToSin.zero;

				public CachedDegreesToSin footLimitYawTheta = CachedDegreesToSin.zero;

				public CachedDegreesToSin footLimitPitchUpTheta = CachedDegreesToSin.zero;

				public CachedDegreesToSin footLimitPitchDownTheta = CachedDegreesToSin.zero;

				public CachedDegreesToSin footLimitRollTheta = CachedDegreesToSin.zero;

				public void Update(Settings.LimbIK settingsLimbIK)
				{
					if (armEffectorBackBeginTheta._degrees != settingsLimbIK.armEffectorBackBeginAngle)
					{
						armEffectorBackBeginTheta._Reset(settingsLimbIK.armEffectorBackBeginAngle);
					}
					if (armEffectorBackCoreBeginTheta._degrees != settingsLimbIK.armEffectorBackCoreBeginAngle)
					{
						armEffectorBackCoreBeginTheta._Reset(settingsLimbIK.armEffectorBackCoreBeginAngle);
					}
					if (armEffectorBackCoreEndTheta._degrees != settingsLimbIK.armEffectorBackCoreEndAngle)
					{
						armEffectorBackCoreEndTheta._Reset(settingsLimbIK.armEffectorBackCoreEndAngle);
					}
					if (armEffectorBackEndTheta._degrees != settingsLimbIK.armEffectorBackEndAngle)
					{
						armEffectorBackEndTheta._Reset(settingsLimbIK.armEffectorBackEndAngle);
					}
					if (armEffectorBackCoreUpperTheta._degrees != settingsLimbIK.armEffectorBackCoreUpperAngle)
					{
						armEffectorBackCoreUpperTheta._Reset(settingsLimbIK.armEffectorBackCoreUpperAngle);
					}
					if (armEffectorBackCoreLowerTheta._degrees != settingsLimbIK.armEffectorBackCoreLowerAngle)
					{
						armEffectorBackCoreLowerTheta._Reset(settingsLimbIK.armEffectorBackCoreLowerAngle);
					}
					if (elbowFrontInnerLimitTheta._degrees != settingsLimbIK.elbowFrontInnerLimitAngle)
					{
						elbowFrontInnerLimitTheta._Reset(settingsLimbIK.elbowFrontInnerLimitAngle);
					}
					if (elbowBackInnerLimitTheta._degrees != settingsLimbIK.elbowBackInnerLimitAngle)
					{
						elbowBackInnerLimitTheta._Reset(settingsLimbIK.elbowBackInnerLimitAngle);
					}
					if (footLimitYawTheta._degrees != settingsLimbIK.footLimitYaw)
					{
						footLimitYawTheta._Reset(settingsLimbIK.footLimitYaw);
					}
					if (footLimitPitchUpTheta._degrees != settingsLimbIK.footLimitPitchUp)
					{
						footLimitPitchUpTheta._Reset(settingsLimbIK.footLimitPitchUp);
					}
					if (footLimitPitchDownTheta._degrees != settingsLimbIK.footLimitPitchDown)
					{
						footLimitPitchDownTheta._Reset(settingsLimbIK.footLimitPitchDown);
					}
					if (footLimitRollTheta._degrees != settingsLimbIK.footLimitRoll)
					{
						footLimitRollTheta._Reset(settingsLimbIK.footLimitRoll);
					}
				}
			}

			public class HeadIK
			{
				public CachedDegreesToSin neckLimitPitchUpTheta = CachedDegreesToSin.zero;

				public CachedDegreesToSin neckLimitPitchDownTheta = CachedDegreesToSin.zero;

				public CachedDegreesToSin neckLimitRollTheta = CachedDegreesToSin.zero;

				public CachedDegreesToSin headLimitYawTheta = CachedDegreesToSin.zero;

				public CachedDegreesToSin headLimitPitchUpTheta = CachedDegreesToSin.zero;

				public CachedDegreesToSin headLimitPitchDownTheta = CachedDegreesToSin.zero;

				public CachedDegreesToSin headLimitRollTheta = CachedDegreesToSin.zero;

				public CachedDegreesToCos eyesTraceTheta = CachedDegreesToCos.zero;

				public CachedDegreesToSin eyesLimitYawTheta = CachedDegreesToSin.zero;

				public CachedDegreesToSin eyesLimitPitchTheta = CachedDegreesToSin.zero;

				public void Update(Settings.HeadIK settingsHeadIK)
				{
					if (neckLimitPitchUpTheta._degrees != settingsHeadIK.neckLimitPitchUp)
					{
						neckLimitPitchUpTheta._Reset(settingsHeadIK.neckLimitPitchUp);
					}
					if (neckLimitPitchDownTheta._degrees != settingsHeadIK.neckLimitPitchDown)
					{
						neckLimitPitchDownTheta._Reset(settingsHeadIK.neckLimitPitchDown);
					}
					if (neckLimitRollTheta._degrees != settingsHeadIK.neckLimitRoll)
					{
						neckLimitRollTheta._Reset(settingsHeadIK.neckLimitRoll);
					}
					if (headLimitYawTheta._degrees != settingsHeadIK.headLimitYaw)
					{
						headLimitYawTheta._Reset(settingsHeadIK.headLimitYaw);
					}
					if (headLimitPitchUpTheta._degrees != settingsHeadIK.headLimitPitchUp)
					{
						headLimitPitchUpTheta._Reset(settingsHeadIK.headLimitPitchUp);
					}
					if (headLimitPitchDownTheta._degrees != settingsHeadIK.headLimitPitchDown)
					{
						headLimitPitchDownTheta._Reset(settingsHeadIK.headLimitPitchDown);
					}
					if (headLimitRollTheta._degrees != settingsHeadIK.headLimitRoll)
					{
						headLimitRollTheta._Reset(settingsHeadIK.headLimitRoll);
					}
					if (eyesTraceTheta._degrees != settingsHeadIK.eyesTraceAngle)
					{
						eyesTraceTheta._Reset(settingsHeadIK.eyesTraceAngle);
					}
					if (eyesLimitYawTheta._degrees != settingsHeadIK.eyesLimitYaw)
					{
						eyesLimitYawTheta._Reset(settingsHeadIK.eyesLimitYaw);
					}
					if (eyesLimitPitchTheta._degrees != settingsHeadIK.eyesLimitPitch)
					{
						eyesLimitPitchTheta._Reset(settingsHeadIK.eyesLimitPitch);
					}
				}
			}

			public bool animatorEnabled;

			public bool resetTransforms;

			public bool continuousSolverEnabled;

			public int shoulderDirYAsNeck = -1;

			public Vector3 defaultRootPosition = Vector3.zero;

			public Matrix3x3 defaultRootBasis = Matrix3x3.identity;

			public Matrix3x3 defaultRootBasisInv = Matrix3x3.identity;

			public Quaternion defaultRootRotation = Quaternion.identity;

			public Vector3 baseHipsPos = Vector3.zero;

			public Matrix3x3 baseHipsBasis = Matrix3x3.identity;

			public BodyIK bodyIK = new BodyIK();

			public LimbIK limbIK = new LimbIK();

			public HeadIK headIK = new HeadIK();

			[Conditional("SAFULLBODYIK_DEBUG")]
			public void ClearDegugPoints()
			{
			}

			[Conditional("SAFULLBODYIK_DEBUG")]
			public void AddDebugPoint(Vector3 pos)
			{
			}

			[Conditional("SAFULLBODYIK_DEBUG")]
			public void AddDebugPoint(Vector3 pos, Color color)
			{
			}

			[Conditional("SAFULLBODYIK_DEBUG")]
			public void AddDebugPoint(Vector3 pos, Color color, float size)
			{
			}

			[Conditional("SAFULLBODYIK_DEBUG")]
			public void UpdateDebugValue(string name, ref int v)
			{
			}

			[Conditional("SAFULLBODYIK_DEBUG")]
			public void UpdateDebugValue(string name, ref float v)
			{
			}

			[Conditional("SAFULLBODYIK_DEBUG")]
			public void UpdateDebugValue(string name, ref bool v)
			{
			}
		}

		public class BoneCaches
		{
			public struct HipsToFootLength
			{
				public Vector3 hipsToLeg;

				public Vector3 legToKnee;

				public Vector3 kneeToFoot;

				public Vector3 defaultOffset;
			}

			public HipsToFootLength[] hipsToFootLength = new HipsToFootLength[2];

			public Vector3 defaultHipsPosition = Vector3.zero;

			public Vector3 hipsOffset = Vector3.zero;

			private void _PrepareHipsToFootLength(int index, Bone legBone, Bone kneeBone, Bone footBone, InternalValues internalValues)
			{
				if (legBone != null && kneeBone != null && footBone != null)
				{
					float length = legBone._defaultLocalLength.length;
					float length2 = kneeBone._defaultLocalLength.length;
					float length3 = footBone._defaultLocalLength.length;
					Vector3 v = legBone._defaultLocalDirection;
					Vector3 v2 = kneeBone._defaultLocalDirection;
					Vector3 v3 = footBone._defaultLocalDirection;
					SAFBIKMatMultVec(out hipsToFootLength[index].hipsToLeg, ref internalValues.defaultRootBasisInv, ref v);
					SAFBIKMatMultVec(out hipsToFootLength[index].legToKnee, ref internalValues.defaultRootBasisInv, ref v2);
					SAFBIKMatMultVec(out hipsToFootLength[index].kneeToFoot, ref internalValues.defaultRootBasisInv, ref v3);
					hipsToFootLength[index].defaultOffset = hipsToFootLength[index].hipsToLeg * length + hipsToFootLength[index].legToKnee * length2 + hipsToFootLength[index].kneeToFoot * length3;
				}
			}

			private Vector3 _GetHipsOffset(int index, Bone legBone, Bone kneeBone, Bone footBone)
			{
				if (legBone != null && kneeBone != null && footBone != null)
				{
					float length = legBone._defaultLocalLength.length;
					float length2 = kneeBone._defaultLocalLength.length;
					float length3 = footBone._defaultLocalLength.length;
					return hipsToFootLength[index].hipsToLeg * length + hipsToFootLength[index].legToKnee * length2 + hipsToFootLength[index].kneeToFoot * length3 - hipsToFootLength[index].defaultOffset;
				}
				return Vector3.zero;
			}

			public void Prepare(FullBodyIK fullBodyIK)
			{
				_PrepareHipsToFootLength(0, fullBodyIK.leftLegBones.leg, fullBodyIK.leftLegBones.knee, fullBodyIK.leftLegBones.foot, fullBodyIK.internalValues);
				_PrepareHipsToFootLength(1, fullBodyIK.rightLegBones.leg, fullBodyIK.rightLegBones.knee, fullBodyIK.rightLegBones.foot, fullBodyIK.internalValues);
				if (fullBodyIK.bodyBones.hips != null)
				{
					defaultHipsPosition = fullBodyIK.bodyBones.hips._defaultPosition;
				}
			}

			public void _SyncDisplacement(FullBodyIK fullBodyIK)
			{
				Vector3 vector = _GetHipsOffset(0, fullBodyIK.leftLegBones.leg, fullBodyIK.leftLegBones.knee, fullBodyIK.leftLegBones.foot);
				Vector3 vector2 = _GetHipsOffset(1, fullBodyIK.rightLegBones.leg, fullBodyIK.rightLegBones.knee, fullBodyIK.rightLegBones.foot);
				hipsOffset = (vector + vector2) * 0.5f;
			}
		}

		public class BodyIK
		{
			public class SolverCaches
			{
				public Bone[] armBones;

				public Bone[] shoulderBones;

				public Bone[] nearArmBones;

				public float armToArmLen;

				public float nearArmToNearArmLen;

				public float[] shoulderToArmLength;

				public float[] nearArmToNeckLength = new float[2];

				public float neckToHeadLength;

				public float neckPull;

				public float headPull;

				public float eyesRate;

				public float neckHeadPull;

				public float[] armPull = new float[2];

				public float[] elbowPull = new float[2];

				public float[] wristPull = new float[2];

				public float[] kneePull = new float[2];

				public float[] footPull = new float[2];

				public float[] fullArmPull = new float[2];

				public float[] limbLegPull = new float[2];

				public float[] armToElbowPull = new float[2];

				public float[] armToWristPull = new float[2];

				public float[] neckHeadToFullArmPull = new float[2];

				public float limbArmRate;

				public float limbLegRate;

				public float armToLegRate;

				public Matrix3x3 centerLegToNearArmBasis = Matrix3x3.identity;

				public Matrix3x3 centerLegToNearArmBasisInv = Matrix3x3.identity;

				public Matrix3x3 centerLegToNaerArmBoneToBaseBasis = Matrix3x3.identity;

				public Matrix3x3 centerLegToNaerArmBaseToBoneBasis = Matrix3x3.identity;

				public Vector3 defaultCenterLegPos = Vector3.zero;
			}

			private class SolverInternal
			{
				public class Limb
				{
					public Vector3[] beginPos;

					public float[] _bendingPull;

					public float[] _endPull;

					public FastLength[] _beginToBendingLength;

					public FastLength[] _beginToEndLength;

					public bool[] targetBeginPosEnabled = new bool[2];

					public Vector3[] targetBeginPos = new Vector3[2];

					public bool[] bendingPosEnabled = new bool[2];

					public bool[] endPosEnabled = new bool[2];

					public Vector3[] bendingPos = new Vector3[2];

					public Vector3[] endPos = new Vector3[2];

					public void Prepare(Effector[] bendingEffectors, Effector[] endEffectors)
					{
						for (int i = 0; i < 2; i++)
						{
							targetBeginPos[i] = beginPos[i];
							targetBeginPosEnabled[i] = false;
							if (bendingEffectors[i] != null && bendingEffectors[i].bone != null && bendingEffectors[i].bone.transformIsAlive)
							{
								bendingPosEnabled[i] = true;
								bendingPos[i] = bendingEffectors[i]._hidden_worldPosition;
							}
							else
							{
								bendingPosEnabled[i] = false;
								bendingPos[i] = default(Vector3);
							}
							if (endEffectors[i] != null && endEffectors[i].bone != null && endEffectors[i].bone.transformIsAlive)
							{
								endPosEnabled[i] = true;
								endPos[i] = endEffectors[i]._hidden_worldPosition;
							}
							else
							{
								endPosEnabled[i] = false;
								endPos[i] = default(Vector3);
							}
						}
					}

					public void ClearEnvTargetBeginPos()
					{
						for (int i = 0; i < 2; i++)
						{
							_ClearEnvTargetBeginPos(i, ref beginPos[i]);
						}
					}

					public void ClearEnvTargetBeginPos(int i)
					{
						_ClearEnvTargetBeginPos(i, ref beginPos[i]);
					}

					public void _ClearEnvTargetBeginPos(int i, ref Vector3 beginPos)
					{
						targetBeginPos[i] = beginPos;
						targetBeginPosEnabled[i] = false;
					}

					public bool SolveTargetBeginPos()
					{
						bool flag = false;
						for (int i = 0; i < 2; i++)
						{
							flag |= SolveTargetBeginPos(i, ref beginPos[i]);
						}
						return flag;
					}

					public bool SolveTargetBeginPos(int i)
					{
						return SolveTargetBeginPos(i, ref beginPos[i]);
					}

					public bool SolveTargetBeginPos(int i, ref Vector3 beginPos)
					{
						targetBeginPos[i] = beginPos;
						targetBeginPosEnabled[i] = false;
						if (endPosEnabled[i] && _endPull[i] > 1E-07f)
						{
							targetBeginPosEnabled[i] |= _SolveTargetBeginPos(ref targetBeginPos[i], ref endPos[i], ref _beginToEndLength[i], _endPull[i]);
						}
						if (bendingPosEnabled[i] && _bendingPull[i] > 1E-07f)
						{
							targetBeginPosEnabled[i] |= _SolveTargetBeginPos(ref targetBeginPos[i], ref bendingPos[i], ref _beginToBendingLength[i], _bendingPull[i]);
						}
						return targetBeginPosEnabled[i];
					}

					private static bool _SolveTargetBeginPos(ref Vector3 targetBeginPos, ref Vector3 targetEndPos, ref FastLength targetBeginToEndLength, float endPull)
					{
						Vector3 vector = targetEndPos - targetBeginPos;
						float sqrMagnitude = vector.sqrMagnitude;
						if (sqrMagnitude > targetBeginToEndLength.lengthSq + float.Epsilon)
						{
							float num = SAFBIKSqrt(sqrMagnitude);
							if (num > 1E-07f)
							{
								float num2 = num - targetBeginToEndLength.length;
								num2 /= num;
								if (num2 > 1E-07f)
								{
									if (endPull < 0.9999999f)
									{
										num2 *= endPull;
									}
									targetBeginPos += vector * num2;
									return true;
								}
							}
						}
						return false;
					}
				}

				public class BackupData
				{
					public Vector3 centerArmPos;

					public Vector3 centerLegPos;

					public Matrix3x3 centerLegBasis;

					public Matrix3x3 spineUBasis;

					public Vector3[] spinePos;

					public Vector3 neckPos;

					public Vector3 headPos;

					public Vector3[] shoulderPos;

					public Vector3[] armPos = new Vector3[2];

					public Vector3[] legPos = new Vector3[2];
				}

				private struct _UpperSolverPreArmsTemp
				{
					public Vector3[] shoulderPos;

					public Vector3[] armPos;

					public Vector3[] nearArmPos;

					public Vector3 neckPos;

					public bool shoulderEnabled;

					public static _UpperSolverPreArmsTemp Alloc()
					{
						_UpperSolverPreArmsTemp result = default(_UpperSolverPreArmsTemp);
						result.shoulderPos = new Vector3[2];
						result.armPos = new Vector3[2];
						result.nearArmPos = null;
						result.shoulderEnabled = false;
						return result;
					}
				}

				private struct _UpperSolverArmsTemp
				{
					public Vector3[] shoulderPos;

					public Vector3[] armPos;

					public Vector3[] nearArmPos;

					public bool shoulderEnabled;

					public Vector3 centerArmPos;

					public Vector3 centerArmDir;

					public static _UpperSolverArmsTemp Alloc()
					{
						_UpperSolverArmsTemp result = default(_UpperSolverArmsTemp);
						result.shoulderPos = new Vector3[2];
						result.armPos = new Vector3[2];
						result.nearArmPos = null;
						result.shoulderEnabled = false;
						result.centerArmPos = Vector3.zero;
						result.centerArmDir = Vector3.zero;
						return result;
					}
				}

				private struct _UpperSolverTemp
				{
					public Vector3[] targetArmPos;

					public Vector3 targetNeckPos;

					public Vector3 targetHeadPos;

					public float[] wristToArmRate;

					public float[] neckToWristRate;

					public static _UpperSolverTemp Alloc()
					{
						_UpperSolverTemp result = default(_UpperSolverTemp);
						result.targetArmPos = new Vector3[2];
						result.targetNeckPos = default(Vector3);
						result.targetHeadPos = default(Vector3);
						result.wristToArmRate = new float[2];
						result.neckToWristRate = new float[2];
						return result;
					}
				}

				public Settings settings;

				public InternalValues internalValues;

				public bool[] _shouderLocalAxisYInv;

				public Effector[] _armEffectors;

				public Effector _neckEffector;

				public Effector _headEffector;

				public Bone[] _spineBones;

				public Bone[] _shoulderBones;

				public Bone[] _armBones;

				public Limb arms = new Limb();

				public Limb legs = new Limb();

				public Vector3[] origToBeginDir = new Vector3[2];

				public Vector3[] origToTargetBeginDir = new Vector3[2];

				public float[] origTheta = new float[2];

				public Vector3[] origAxis = new Vector3[2];

				public Vector3[] origTranslate = new Vector3[2];

				public float[] origFeedbackRate = new float[2];

				public Vector3[] spinePos;

				public Vector3 neckPos;

				public Vector3 headPos;

				public bool headEnabled;

				public Vector3[] nearArmPos;

				public Vector3[] shoulderPos;

				public Vector3[] armPos = new Vector3[2];

				public Vector3[] legPos = new Vector3[2];

				public Matrix3x3 _centerLegBoneBasisInv = Matrix3x3.identity;

				public Matrix3x3 _spineUBoneLocalAxisBasisInv = Matrix3x3.identity;

				public Vector3 _centerArmPos = Vector3.zero;

				public Vector3 _centerLegPos = Vector3.zero;

				public Matrix3x3 _centerLegBasis = Matrix3x3.identity;

				public Matrix3x3 _spineUBasis = Matrix3x3.identity;

				private bool _isDirtyCenterArmPos = true;

				private bool _isDirtyCenterLegPos = true;

				private bool _isDirtyCenterLegBasis = true;

				private bool _isDirtySpineUBasis = true;

				private BackupData _backupData = new BackupData();

				public Effector[] _wristEffectors;

				public SolverCaches _solverCaches;

				private _UpperSolverPreArmsTemp _upperSolverPreArmsTemp = _UpperSolverPreArmsTemp.Alloc();

				private _UpperSolverArmsTemp[] _upperSolverArmsTemps = new _UpperSolverArmsTemp[2]
				{
					_UpperSolverArmsTemp.Alloc(),
					_UpperSolverArmsTemp.Alloc()
				};

				private _UpperSolverTemp _upperSolverTemp = _UpperSolverTemp.Alloc();

				public bool targetCenterArmEnabled;

				public Vector3 targetCenterArmPos = Vector3.zero;

				public Vector3 targetCenterArmDir = Vector3.zero;

				private Vector3[] _tempArmPos = new Vector3[2];

				private Vector3[] _tempArmToElbowDir = new Vector3[2];

				private Vector3[] _tempElbowToWristDir = new Vector3[2];

				private bool[] _tempElbowPosEnabled = new bool[2];

				public LimbIK[] _limbIK;

				private Matrix3x3[] _tempParentBasis = new Matrix3x3[2]
				{
					Matrix3x3.identity,
					Matrix3x3.identity
				};

				private Vector3[] _tempArmToElbowDefaultDir = new Vector3[2];

				public Vector3 centerArmPos
				{
					get
					{
						if (_isDirtyCenterArmPos)
						{
							_UpdateCenterArmPos();
						}
						return _centerArmPos;
					}
				}

				public Vector3 centerLegPos
				{
					get
					{
						if (_isDirtyCenterLegPos)
						{
							_UpdateCenterLegPos();
						}
						return _centerLegPos;
					}
				}

				public Matrix3x3 centerLegBasis
				{
					get
					{
						if (_isDirtyCenterLegBasis)
						{
							_UpdateCenterLegBasis();
						}
						return _centerLegBasis;
					}
				}

				public Matrix3x3 spineUBasis
				{
					get
					{
						if (_isDirtySpineUBasis)
						{
							_UpdateSpineUBasis();
						}
						return _spineUBasis;
					}
				}

				public Vector3 spineUPos
				{
					get
					{
						if (spinePos != null && spinePos.Length != 0)
						{
							return spinePos[spinePos.Length - 1];
						}
						return Vector3.zero;
					}
				}

				public Vector3 currentCenterArmPos
				{
					get
					{
						if (shoulderPos != null)
						{
							return (shoulderPos[0] + shoulderPos[1]) * 0.5f;
						}
						if (armPos != null)
						{
							return (armPos[0] + armPos[1]) * 0.5f;
						}
						return Vector3.zero;
					}
				}

				public Vector3 currentCenterArmDir
				{
					get
					{
						if (shoulderPos != null)
						{
							Vector3 v = shoulderPos[1] - shoulderPos[0];
							if (SAFBIKVecNormalize(ref v))
							{
								return v;
							}
						}
						else if (armPos != null)
						{
							Vector3 v2 = armPos[1] - armPos[0];
							if (SAFBIKVecNormalize(ref v2))
							{
								return v2;
							}
						}
						return Vector3.zero;
					}
				}

				public SolverInternal()
				{
					arms.beginPos = armPos;
					legs.beginPos = legPos;
				}

				public void _UpdateCenterArmPos()
				{
					if (_isDirtyCenterArmPos)
					{
						_isDirtyCenterArmPos = false;
						Vector3[] array = shoulderPos;
						if (array == null)
						{
							array = armPos;
						}
						if (array != null)
						{
							_centerArmPos = (array[0] + array[1]) * 0.5f;
						}
					}
				}

				public void _UpdateCenterLegPos()
				{
					if (_isDirtyCenterLegPos)
					{
						_isDirtyCenterLegPos = false;
						Vector3[] array = legPos;
						if (array != null)
						{
							_centerLegPos = (array[0] + array[1]) * 0.5f;
						}
					}
				}

				public void _SetCenterArmPos(ref Vector3 centerArmPos)
				{
					_isDirtyCenterArmPos = false;
					_centerArmPos = centerArmPos;
				}

				public void _SetCenterLegPos(ref Vector3 centerLegPos)
				{
					_isDirtyCenterLegPos = false;
					_centerLegPos = centerLegPos;
				}

				public void _UpdateCenterLegBasis()
				{
					if (!_isDirtyCenterLegBasis)
					{
						return;
					}
					_isDirtyCenterLegBasis = false;
					Vector3[] array = legPos;
					_centerLegBasis = Matrix3x3.identity;
					if (spinePos != null && spinePos.Length != 0 && array != null)
					{
						Vector3 lhs = array[1] - array[0];
						Vector3 v = spinePos[0] - centerLegPos;
						Vector3 v2 = Vector3.Cross(lhs, v);
						lhs = Vector3.Cross(v, v2);
						if (SAFBIKVecNormalize3(ref lhs, ref v, ref v2))
						{
							_centerLegBasis.SetColumn(ref lhs, ref v, ref v2);
							SAFBIKMatMultRet0(ref _centerLegBasis, ref _centerLegBoneBasisInv);
						}
					}
				}

				public void _UpdateSpineUBasis()
				{
					if (_isDirtySpineUBasis)
					{
						_isDirtySpineUBasis = false;
						_spineUBasis = Matrix3x3.identity;
						Vector3 vector = ((shoulderPos != null) ? (shoulderPos[1] + shoulderPos[0]) : (armPos[1] + armPos[0]));
						vector = vector * 0.5f - spineUPos;
						Vector3 lhs = ((shoulderPos != null) ? (shoulderPos[1] - shoulderPos[0]) : (armPos[1] - armPos[0]));
						Vector3 v = Vector3.Cross(lhs, vector);
						lhs = Vector3.Cross(vector, v);
						if (SAFBIKVecNormalize3(ref lhs, ref vector, ref v))
						{
							_spineUBasis.SetColumn(ref lhs, ref vector, ref v);
							SAFBIKMatMultRet0(ref _spineUBasis, ref _spineUBoneLocalAxisBasisInv);
						}
					}
				}

				public void SetDirtyVariables()
				{
					_isDirtyCenterArmPos = true;
					_isDirtyCenterLegPos = true;
					_isDirtyCenterLegBasis = true;
					_isDirtySpineUBasis = true;
				}

				public void Backup()
				{
					_backupData.centerArmPos = centerArmPos;
					_backupData.centerLegPos = centerLegPos;
					_backupData.centerLegBasis = centerLegBasis;
					_backupData.spineUBasis = spineUBasis;
					CloneArray(ref _backupData.spinePos, spinePos);
					_backupData.neckPos = neckPos;
					_backupData.headPos = headPos;
					CloneArray(ref _backupData.shoulderPos, shoulderPos);
					CloneArray(ref _backupData.armPos, arms.beginPos);
					CloneArray(ref _backupData.legPos, legs.beginPos);
				}

				public void Restore()
				{
					_isDirtyCenterArmPos = false;
					_isDirtyCenterLegPos = false;
					_isDirtyCenterLegBasis = false;
					_isDirtySpineUBasis = false;
					_centerArmPos = _backupData.centerArmPos;
					_centerLegPos = _backupData.centerLegPos;
					_centerLegBasis = _backupData.centerLegBasis;
					_spineUBasis = _backupData.spineUBasis;
					CloneArray(ref spinePos, _backupData.spinePos);
					neckPos = _backupData.neckPos;
					headPos = _backupData.headPos;
					CloneArray(ref shoulderPos, _backupData.shoulderPos);
					CloneArray(ref arms.beginPos, _backupData.armPos);
					CloneArray(ref legs.beginPos, _backupData.legPos);
				}

				private void _SolveArmsToArms(ref _UpperSolverArmsTemp armsTemp, float armPull, int idx0)
				{
					Vector3 b = _upperSolverTemp.targetArmPos[idx0];
					armsTemp.armPos[idx0] = Vector3.Lerp(armsTemp.armPos[idx0], b, armPull);
				}

				private void _SolveArmsToNeck(ref _UpperSolverArmsTemp armsTemp, float neckToFullArmPull, int idx0)
				{
					Vector3 posTo = armsTemp.nearArmPos[idx0];
					_KeepLength(ref posTo, ref _upperSolverTemp.targetNeckPos, _solverCaches.nearArmToNeckLength[idx0]);
					armsTemp.nearArmPos[idx0] = Vector3.Lerp(posTo, armsTemp.nearArmPos[idx0], neckToFullArmPull);
				}

				private void _SolveArms(ref _UpperSolverArmsTemp armsTemp, int idx0)
				{
					int num = 1 - idx0;
					float neckHeadPull = _solverCaches.neckHeadPull;
					float[] armPull = _solverCaches.armPull;
					float[] elbowPull = _solverCaches.elbowPull;
					float[] wristPull = _solverCaches.wristPull;
					float[] neckHeadToFullArmPull = _solverCaches.neckHeadToFullArmPull;
					if (!(wristPull[idx0] > 1E-07f) && !(elbowPull[idx0] > 1E-07f) && !(armPull[idx0] > 1E-07f) && !(neckHeadPull > 1E-07f))
					{
						return;
					}
					if (armPull[idx0] > 1E-07f)
					{
						_SolveArmsToArms(ref armsTemp, armPull[idx0], idx0);
					}
					if ((wristPull[idx0] > 1E-07f || elbowPull[idx0] > 1E-07f) && arms.SolveTargetBeginPos(idx0, ref armsTemp.armPos[idx0]))
					{
						armsTemp.armPos[idx0] = arms.targetBeginPos[idx0];
						if (armsTemp.shoulderEnabled)
						{
							_KeepLength(ref armsTemp.shoulderPos[idx0], ref armsTemp.armPos[idx0], _solverCaches.shoulderToArmLength[idx0]);
							if (neckHeadPull > 1E-07f)
							{
								_SolveArmsToNeck(ref armsTemp, neckHeadToFullArmPull[idx0], idx0);
								_KeepLength(ref armsTemp.armPos[idx0], ref armsTemp.shoulderPos[idx0], _solverCaches.shoulderToArmLength[idx0]);
							}
							_KeepLength(ref armsTemp.shoulderPos[num], ref armsTemp.shoulderPos[idx0], _solverCaches.nearArmToNearArmLen);
							_KeepLength(ref armsTemp.armPos[num], ref armsTemp.shoulderPos[num], _solverCaches.shoulderToArmLength[num]);
						}
						else
						{
							if (neckHeadPull > 1E-07f)
							{
								_SolveArmsToNeck(ref armsTemp, neckHeadToFullArmPull[idx0], idx0);
							}
							_KeepLength(ref armsTemp.armPos[num], ref armsTemp.armPos[idx0], _solverCaches.armToArmLen);
						}
					}
					else
					{
						if (!(armPull[idx0] > 1E-07f) && !(neckHeadPull > 1E-07f))
						{
							return;
						}
						if (armPull[idx0] > 1E-07f && armsTemp.shoulderEnabled)
						{
							_KeepLength(ref armsTemp.shoulderPos[idx0], ref armsTemp.armPos[idx0], _solverCaches.shoulderToArmLength[idx0]);
						}
						if (neckHeadPull > 1E-07f)
						{
							_SolveArmsToNeck(ref armsTemp, neckHeadToFullArmPull[idx0], idx0);
							if (armsTemp.shoulderEnabled)
							{
								_KeepLength(ref armsTemp.armPos[idx0], ref armsTemp.shoulderPos[idx0], _solverCaches.shoulderToArmLength[idx0]);
							}
						}
						if (armsTemp.shoulderEnabled)
						{
							_KeepLength(ref armsTemp.shoulderPos[num], ref armsTemp.shoulderPos[idx0], _solverCaches.nearArmToNearArmLen);
							_KeepLength(ref armsTemp.armPos[num], ref armsTemp.shoulderPos[num], _solverCaches.shoulderToArmLength[num]);
						}
						else
						{
							_KeepLength(ref armsTemp.armPos[num], ref armsTemp.armPos[idx0], _solverCaches.armToArmLen);
						}
					}
				}

				public bool UpperSolve()
				{
					targetCenterArmEnabled = false;
					float neckPull = _solverCaches.neckPull;
					float headPull = _solverCaches.headPull;
					float[] armPull = _solverCaches.armPull;
					float[] elbowPull = _solverCaches.elbowPull;
					float[] wristPull = _solverCaches.wristPull;
					if (wristPull[0] <= 1E-07f && wristPull[1] <= 1E-07f && elbowPull[0] <= 1E-07f && elbowPull[1] <= 1E-07f && armPull[0] <= 1E-07f && armPull[1] <= 1E-07f && neckPull <= 1E-07f && headPull <= 1E-07f)
					{
						targetCenterArmPos = currentCenterArmPos;
						targetCenterArmDir = currentCenterArmDir;
						return false;
					}
					_upperSolverTemp.targetNeckPos = ((_neckEffector != null) ? _neckEffector._hidden_worldPosition : neckPos);
					_upperSolverTemp.targetHeadPos = ((_headEffector != null) ? _headEffector._hidden_worldPosition : headPos);
					_upperSolverTemp.targetArmPos[0] = ((_armEffectors != null) ? _armEffectors[0]._hidden_worldPosition : armPos[0]);
					_upperSolverTemp.targetArmPos[1] = ((_armEffectors != null) ? _armEffectors[1]._hidden_worldPosition : armPos[1]);
					_upperSolverPreArmsTemp.neckPos = neckPos;
					_upperSolverPreArmsTemp.armPos[0] = armPos[0];
					_upperSolverPreArmsTemp.armPos[1] = armPos[1];
					_upperSolverPreArmsTemp.shoulderEnabled = shoulderPos != null;
					if (_upperSolverPreArmsTemp.shoulderEnabled)
					{
						_upperSolverPreArmsTemp.shoulderPos[0] = shoulderPos[0];
						_upperSolverPreArmsTemp.shoulderPos[1] = shoulderPos[1];
						_upperSolverPreArmsTemp.nearArmPos = _upperSolverPreArmsTemp.shoulderPos;
					}
					else
					{
						_upperSolverPreArmsTemp.nearArmPos = _upperSolverPreArmsTemp.armPos;
					}
					float upperBodyMovingfixRate = settings.bodyIK.upperBodyMovingfixRate;
					float upperHeadMovingfixRate = settings.bodyIK.upperHeadMovingfixRate;
					if (upperBodyMovingfixRate > 1E-07f || upperHeadMovingfixRate > 1E-07f)
					{
						Vector3 vector = Vector3.zero;
						Vector3 vector2 = Vector3.zero;
						if (headPull > 1E-07f)
						{
							vector = _upperSolverTemp.targetHeadPos - headPos;
							if (upperHeadMovingfixRate < 0.9999999f)
							{
								vector *= headPull * upperHeadMovingfixRate;
							}
							else
							{
								vector *= headPull;
							}
						}
						float num = 0f;
						float num2 = 0f;
						if (neckPull > 1E-07f || armPull[0] > 1E-07f || armPull[1] > 1E-07f)
						{
							num = neckPull + armPull[0] + armPull[1];
							num2 = 1f / num;
							if (neckPull > 1E-07f)
							{
								vector2 = (_upperSolverTemp.targetNeckPos - neckPos) * (neckPull * neckPull);
							}
							if (armPull[0] > 1E-07f)
							{
								vector2 += (_upperSolverTemp.targetArmPos[0] - armPos[0]) * (armPull[0] * armPull[0]);
							}
							if (armPull[1] > 1E-07f)
							{
								vector2 += (_upperSolverTemp.targetArmPos[1] - armPos[1]) * (armPull[1] * armPull[1]);
							}
							if (upperBodyMovingfixRate < 0.9999999f)
							{
								vector2 *= num2 * upperBodyMovingfixRate;
							}
							else
							{
								vector2 *= num2;
							}
						}
						Vector3 vector3 = default(Vector3);
						if (!(headPull > 1E-07f) || !(num > 1E-07f))
						{
							vector3 = ((!(headPull > 1E-07f)) ? vector2 : vector);
						}
						else
						{
							vector3 = vector * headPull + vector2 * num;
							vector3 *= 1f / (headPull + num);
						}
						_upperSolverPreArmsTemp.neckPos += vector3;
						_upperSolverPreArmsTemp.armPos[0] += vector3;
						_upperSolverPreArmsTemp.armPos[1] += vector3;
						if (_upperSolverPreArmsTemp.shoulderEnabled)
						{
							_upperSolverPreArmsTemp.shoulderPos[0] += vector3;
							_upperSolverPreArmsTemp.shoulderPos[1] += vector3;
						}
					}
					if ((upperHeadMovingfixRate < 0.9999999f || upperBodyMovingfixRate < 0.9999999f) && (headPull > 1E-07f || neckPull > 1E-07f))
					{
						if (upperHeadMovingfixRate < 0.9999999f && headPull > 1E-07f)
						{
							Vector3 posTo = _upperSolverPreArmsTemp.neckPos;
							if (_KeepMaxLength(ref posTo, ref _upperSolverTemp.targetHeadPos, _solverCaches.neckToHeadLength))
							{
								_upperSolverPreArmsTemp.neckPos = Vector3.Lerp(_upperSolverPreArmsTemp.neckPos, posTo, headPull);
							}
						}
						for (int i = 0; i != 2; i++)
						{
							if (upperBodyMovingfixRate < 0.9999999f && neckPull > 1E-07f)
							{
								Vector3 posTo2 = _upperSolverPreArmsTemp.nearArmPos[i];
								_KeepLength(ref posTo2, ref _upperSolverTemp.targetNeckPos, _solverCaches.nearArmToNeckLength[i]);
								_upperSolverPreArmsTemp.nearArmPos[i] = Vector3.Lerp(_upperSolverPreArmsTemp.nearArmPos[i], posTo2, neckPull);
							}
							else
							{
								_KeepLength(ref _upperSolverPreArmsTemp.nearArmPos[i], ref _upperSolverPreArmsTemp.neckPos, _solverCaches.nearArmToNeckLength[i]);
							}
							if (_upperSolverPreArmsTemp.shoulderEnabled)
							{
								_KeepLength(ref _upperSolverPreArmsTemp.armPos[i], ref _upperSolverPreArmsTemp.shoulderPos[i], _solverCaches.shoulderToArmLength[i]);
							}
						}
					}
					_upperSolverTemp.targetNeckPos = _upperSolverPreArmsTemp.neckPos;
					for (int j = 0; j != 2; j++)
					{
						_upperSolverArmsTemps[j].armPos[0] = _upperSolverPreArmsTemp.armPos[0];
						_upperSolverArmsTemps[j].armPos[1] = _upperSolverPreArmsTemp.armPos[1];
						_upperSolverArmsTemps[j].shoulderEnabled = _upperSolverPreArmsTemp.shoulderEnabled;
						if (_upperSolverArmsTemps[j].shoulderEnabled)
						{
							_upperSolverArmsTemps[j].shoulderPos[0] = _upperSolverPreArmsTemp.shoulderPos[0];
							_upperSolverArmsTemps[j].shoulderPos[1] = _upperSolverPreArmsTemp.shoulderPos[1];
							_upperSolverArmsTemps[j].nearArmPos = _upperSolverArmsTemps[j].shoulderPos;
						}
						else
						{
							_upperSolverArmsTemps[j].nearArmPos = _upperSolverArmsTemps[j].armPos;
						}
					}
					bool flag = wristPull[0] > 1E-07f || elbowPull[0] > 1E-07f || armPull[0] > 1E-07f;
					bool flag2 = wristPull[1] > 1E-07f || elbowPull[1] > 1E-07f || armPull[1] > 1E-07f;
					float neckHeadPull = _solverCaches.neckHeadPull;
					if ((flag && flag2) || neckHeadPull > 1E-07f)
					{
						for (int k = 0; k != 2; k++)
						{
							int num3 = k;
							int idx = 1 - k;
							_SolveArms(ref _upperSolverArmsTemps[num3], num3);
							_SolveArms(ref _upperSolverArmsTemps[num3], idx);
							_SolveArms(ref _upperSolverArmsTemps[num3], num3);
							if (_upperSolverArmsTemps[num3].shoulderEnabled)
							{
								_upperSolverArmsTemps[num3].centerArmPos = (_upperSolverArmsTemps[num3].shoulderPos[0] + _upperSolverArmsTemps[num3].shoulderPos[1]) * 0.5f;
								_upperSolverArmsTemps[num3].centerArmDir = _upperSolverArmsTemps[num3].shoulderPos[1] - _upperSolverArmsTemps[num3].shoulderPos[0];
							}
							else
							{
								_upperSolverArmsTemps[num3].centerArmPos = (_upperSolverArmsTemps[num3].armPos[0] + _upperSolverArmsTemps[num3].armPos[1]) * 0.5f;
								_upperSolverArmsTemps[num3].centerArmDir = _upperSolverArmsTemps[num3].armPos[1] - _upperSolverArmsTemps[num3].armPos[0];
							}
						}
						if (!SAFBIKVecNormalize2(ref _upperSolverArmsTemps[0].centerArmDir, ref _upperSolverArmsTemps[1].centerArmDir))
						{
							return false;
						}
						float limbArmRate = _solverCaches.limbArmRate;
						targetCenterArmEnabled = true;
						targetCenterArmPos = Vector3.Lerp(_upperSolverArmsTemps[0].centerArmPos, _upperSolverArmsTemps[1].centerArmPos, limbArmRate);
						targetCenterArmDir = _LerpDir(ref _upperSolverArmsTemps[0].centerArmDir, ref _upperSolverArmsTemps[1].centerArmDir, limbArmRate);
					}
					else
					{
						int num4 = ((!flag) ? 1 : 0);
						_SolveArms(ref _upperSolverArmsTemps[num4], num4);
						if (_upperSolverArmsTemps[num4].shoulderEnabled)
						{
							_upperSolverArmsTemps[num4].centerArmPos = (_upperSolverArmsTemps[num4].shoulderPos[0] + _upperSolverArmsTemps[num4].shoulderPos[1]) * 0.5f;
							_upperSolverArmsTemps[num4].centerArmDir = _upperSolverArmsTemps[num4].shoulderPos[1] - _upperSolverArmsTemps[num4].shoulderPos[0];
						}
						else
						{
							_upperSolverArmsTemps[num4].centerArmPos = (_upperSolverArmsTemps[num4].armPos[0] + _upperSolverArmsTemps[num4].armPos[1]) * 0.5f;
							_upperSolverArmsTemps[num4].centerArmDir = _upperSolverArmsTemps[num4].armPos[1] - _upperSolverArmsTemps[num4].armPos[0];
						}
						if (!SAFBIKVecNormalize(ref _upperSolverArmsTemps[num4].centerArmDir))
						{
							return false;
						}
						targetCenterArmEnabled = true;
						targetCenterArmPos = _upperSolverArmsTemps[num4].centerArmPos;
						targetCenterArmDir = _upperSolverArmsTemps[num4].centerArmDir;
					}
					return true;
				}

				public bool ShoulderResolve()
				{
					Bone[] armBones = _solverCaches.armBones;
					Bone[] shoulderBones = _solverCaches.shoulderBones;
					float[] shoulderToArmLength = _solverCaches.shoulderToArmLength;
					if (armBones == null || shoulderBones == null)
					{
						return false;
					}
					if (!_limbIK[2].IsSolverEnabled() && !_limbIK[3].IsSolverEnabled())
					{
						return false;
					}
					for (int i = 0; i != 2; i++)
					{
						int num = ((i == 0) ? 2 : 3);
						if (!_limbIK[num].IsSolverEnabled())
						{
							continue;
						}
						Vector3 vector = armPos[i] - shoulderPos[i];
						Vector3 rhs = ((internalValues.shoulderDirYAsNeck == 0) ? (shoulderPos[i] - spineUPos) : (neckPos - shoulderPos[i]));
						vector = ((i == 0) ? (-vector) : vector);
						Vector3 v = Vector3.Cross(vector, rhs);
						rhs = Vector3.Cross(v, vector);
						if (SAFBIKVecNormalize3(ref vector, ref rhs, ref v))
						{
							Matrix3x3 lhs = Matrix3x3.FromColumn(ref vector, ref rhs, ref v);
							SAFBIKMatMult(out _tempParentBasis[i], ref lhs, ref _shoulderBones[i]._boneToBaseBasis);
						}
						_tempArmPos[i] = armPos[i];
						_tempElbowPosEnabled[i] = _limbIK[num].Presolve(ref _tempParentBasis[i], ref _tempArmPos[i], out _tempArmToElbowDir[i], out _tempElbowToWristDir[i]);
						if (_tempElbowPosEnabled[i])
						{
							SAFBIKMatMultCol0(out _tempArmToElbowDefaultDir[i], ref _tempParentBasis[i], ref _armBones[i]._baseToBoneBasis);
							if (i == 0)
							{
								_tempArmToElbowDefaultDir[i] = -_tempArmToElbowDefaultDir[i];
							}
						}
					}
					if (!_tempElbowPosEnabled[0] && !_tempElbowPosEnabled[1])
					{
						return false;
					}
					float shoulderSolveBendingRate = settings.bodyIK.shoulderSolveBendingRate;
					bool result = false;
					for (int j = 0; j != 2; j++)
					{
						if (_tempElbowPosEnabled[j])
						{
							float theta;
							Vector3 axis;
							_ComputeThetaAxis(ref _tempArmToElbowDefaultDir[j], ref _tempArmToElbowDir[j], out theta, out axis);
							if (!(theta >= -1E-45f) || !(theta <= float.Epsilon))
							{
								result = true;
								theta = SAFBIKCos(SAFBIKAcos(theta) * shoulderSolveBendingRate);
								Matrix3x3 m = default(Matrix3x3);
								SAFBIKMatSetAxisAngle(out m, ref axis, theta);
								Vector3 vector2 = shoulderPos[j];
								Vector3 v2 = _tempArmPos[j] - vector2;
								SAFBIKVecNormalize(ref v2);
								Vector3 ret;
								SAFBIKMatMultVec(out ret, ref m, ref v2);
								Vector3 destArmPos = vector2 + ret * shoulderToArmLength[j];
								SolveShoulderToArmInternal(j, ref destArmPos);
							}
						}
					}
					return result;
				}

				public bool PrepareLowerRotation(int origIndex)
				{
					bool flag = false;
					for (int i = 0; i < 2; i++)
					{
						legs.SolveTargetBeginPos(i);
						flag |= _PrepareLimbRotation(legs, i, origIndex, ref legs.beginPos[i]);
					}
					return flag;
				}

				public bool _PrepareLimbRotation(Limb limb, int i, int origIndex, ref Vector3 beginPos)
				{
					origTheta[i] = 0f;
					origAxis[i] = new Vector3(0f, 0f, 1f);
					if (!limb.targetBeginPosEnabled[i])
					{
						return false;
					}
					Vector3[] targetBeginPos = limb.targetBeginPos;
					Vector3 origPos = ((origIndex == -1) ? centerLegPos : spinePos[origIndex]);
					return _ComputeThetaAxis(ref origPos, ref beginPos, ref targetBeginPos[i], out origTheta[i], out origAxis[i]);
				}

				public void SetSolveFeedbackRate(float feedbackRate)
				{
					for (int i = 0; i < origFeedbackRate.Length; i++)
					{
						origFeedbackRate[i] = feedbackRate;
					}
				}

				public void SetSolveFeedbackRate(int i, float feedbackRate)
				{
					origFeedbackRate[i] = feedbackRate;
				}

				public bool SolveLowerRotation(int origIndex, out Quaternion origRotation)
				{
					return _SolveLimbRotation(legs, origIndex, out origRotation);
				}

				private bool _SolveLimbRotation(Limb limb, int origIndex, out Quaternion origRotation)
				{
					origRotation = Quaternion.identity;
					int num = -1;
					int num2 = 0;
					for (int i = 0; i < 2; i++)
					{
						if (limb.targetBeginPosEnabled[i])
						{
							num = i;
							num2++;
						}
					}
					if (num2 == 0)
					{
						return false;
					}
					float num3 = ((limb == arms) ? _solverCaches.limbArmRate : _solverCaches.limbLegRate);
					if (num2 == 1)
					{
						int num4 = num;
						if (origTheta[num4] == 0f)
						{
							return false;
						}
						if (num4 == 0)
						{
							num3 = 1f - num3;
						}
						origRotation = _GetRotation(ref origAxis[num4], origTheta[num4], origFeedbackRate[num4] * num3);
						return true;
					}
					Quaternion a = _GetRotation(ref origAxis[0], origTheta[0], origFeedbackRate[0] * 0.5f);
					Quaternion b = _GetRotation(ref origAxis[1], origTheta[1], origFeedbackRate[1] * 0.5f);
					origRotation = Quaternion.Lerp(a, b, num3);
					origRotation *= origRotation;
					return true;
				}

				public void UpperRotation(int origIndex, ref Matrix3x3 origBasis)
				{
					Vector3 addVec = ((origIndex == -1) ? centerLegPos : spinePos[origIndex]);
					Vector3[] array = armPos;
					if (array != null)
					{
						for (int i = 0; i < array.Length; i++)
						{
							SAFBIKMatMultVecPreSubAdd(out array[i], ref origBasis, ref array[i], ref addVec, ref addVec);
						}
					}
					if (shoulderPos != null)
					{
						for (int j = 0; j < shoulderPos.Length; j++)
						{
							SAFBIKMatMultVecPreSubAdd(out shoulderPos[j], ref origBasis, ref shoulderPos[j], ref addVec, ref addVec);
						}
					}
					SAFBIKMatMultVecPreSubAdd(out neckPos, ref origBasis, ref neckPos, ref addVec, ref addVec);
					if (headEnabled)
					{
						SAFBIKMatMultVecPreSubAdd(out headPos, ref origBasis, ref headPos, ref addVec, ref addVec);
					}
					if (origIndex == -1)
					{
						Vector3[] array2 = legPos;
						if (array2 != null)
						{
							for (int k = 0; k < array2.Length; k++)
							{
								SAFBIKMatMultVecPreSubAdd(out array2[k], ref origBasis, ref array2[k], ref addVec, ref addVec);
							}
						}
						_isDirtyCenterLegBasis = true;
					}
					for (int l = ((origIndex != -1) ? origIndex : 0); l < spinePos.Length; l++)
					{
						SAFBIKMatMultVecPreSubAdd(out spinePos[l], ref origBasis, ref spinePos[l], ref addVec, ref addVec);
					}
					_isDirtyCenterArmPos = true;
					_isDirtySpineUBasis = true;
				}

				public void LowerRotation(int origIndex, ref Quaternion origRotation, bool bodyRotation)
				{
					Matrix3x3 origBasis = new Matrix3x3(origRotation);
					LowerRotation(origIndex, ref origBasis, bodyRotation);
				}

				public void LowerRotation(int origIndex, ref Matrix3x3 origBasis, bool bodyRotation)
				{
					Vector3 addVec = ((origIndex == -1) ? centerLegPos : spinePos[origIndex]);
					Vector3[] array = legPos;
					if (array != null)
					{
						for (int i = 0; i < 2; i++)
						{
							SAFBIKMatMultVecPreSubAdd(out array[i], ref origBasis, ref array[i], ref addVec, ref addVec);
						}
					}
					if (spinePos != null)
					{
						int num = (bodyRotation ? spinePos.Length : origIndex);
						for (int j = 0; j < num; j++)
						{
							SAFBIKMatMultVecPreSubAdd(out spinePos[j], ref origBasis, ref spinePos[j], ref addVec, ref addVec);
						}
					}
					_isDirtyCenterArmPos = true;
					_isDirtyCenterLegPos = true;
					_isDirtyCenterLegBasis = true;
					if (!bodyRotation && spinePos != null && origIndex + 1 != spinePos.Length)
					{
						return;
					}
					SAFBIKMatMultVecPreSubAdd(out neckPos, ref origBasis, ref neckPos, ref addVec, ref addVec);
					if (headEnabled)
					{
						SAFBIKMatMultVecPreSubAdd(out headPos, ref origBasis, ref headPos, ref addVec, ref addVec);
					}
					Vector3[] array2 = armPos;
					if (array2 != null)
					{
						for (int k = 0; k < 2; k++)
						{
							SAFBIKMatMultVecPreSubAdd(out array2[k], ref origBasis, ref array2[k], ref addVec, ref addVec);
						}
					}
					if (shoulderPos != null)
					{
						for (int l = 0; l < 2; l++)
						{
							SAFBIKMatMultVecPreSubAdd(out shoulderPos[l], ref origBasis, ref shoulderPos[l], ref addVec, ref addVec);
						}
					}
					_isDirtySpineUBasis = true;
				}

				public bool PrepareLowerTranslate()
				{
					bool flag = false;
					for (int i = 0; i < 2; i++)
					{
						legs.SolveTargetBeginPos(i);
						flag |= _PrepareLimbTranslate(legs, i, ref legs.beginPos[i]);
					}
					return flag;
				}

				private bool _PrepareLimbTranslate(Limb limb, int i, ref Vector3 beginPos)
				{
					origTranslate[i] = Vector3.zero;
					if (limb.targetBeginPosEnabled[i])
					{
						origTranslate[i] = limb.targetBeginPos[i] - beginPos;
						return true;
					}
					return false;
				}

				public bool SolveLowerTranslate(out Vector3 translate)
				{
					return _SolveLimbTranslate(legs, out translate);
				}

				private bool _SolveLimbTranslate(Limb limb, out Vector3 origTranslate)
				{
					origTranslate = Vector3.zero;
					float num = ((limb == arms) ? _solverCaches.limbArmRate : _solverCaches.limbLegRate);
					if (limb.targetBeginPosEnabled[0] && limb.targetBeginPosEnabled[1])
					{
						origTranslate = Vector3.Lerp(this.origTranslate[0], this.origTranslate[1], num);
					}
					else if (limb.targetBeginPosEnabled[0] || limb.targetBeginPosEnabled[1])
					{
						int num2 = ((!limb.targetBeginPosEnabled[0]) ? 1 : 0);
						float num3 = (limb.targetBeginPosEnabled[0] ? (1f - num) : num);
						origTranslate = this.origTranslate[num2] * num3;
					}
					return origTranslate != Vector3.zero;
				}

				public void LowerTranslateBeginOnly(ref Vector3 origTranslate)
				{
					_LimbTranslateBeginOnly(legs, ref origTranslate);
					_centerLegPos += origTranslate;
				}

				private void _LimbTranslateBeginOnly(Limb limb, ref Vector3 origTranslate)
				{
					for (int i = 0; i < 2; i++)
					{
						limb.beginPos[i] += origTranslate;
					}
				}

				public void Translate(ref Vector3 origTranslate)
				{
					_centerArmPos += origTranslate;
					_centerLegPos += origTranslate;
					if (spinePos != null)
					{
						for (int i = 0; i != spinePos.Length; i++)
						{
							spinePos[i] += origTranslate;
						}
					}
					neckPos += origTranslate;
					if (headEnabled)
					{
						headPos += origTranslate;
					}
					for (int j = 0; j != 2; j++)
					{
						if (legPos != null)
						{
							legPos[j] += origTranslate;
						}
						if (shoulderPos != null)
						{
							shoulderPos[j] += origTranslate;
						}
						if (armPos != null)
						{
							armPos[j] += origTranslate;
						}
					}
				}

				public void SolveShoulderToArmInternal(int i, ref Vector3 destArmPos)
				{
					if (!settings.bodyIK.shoulderSolveEnabled)
					{
						return;
					}
					Bone[] shoulderBones = _solverCaches.shoulderBones;
					float[] shoulderToArmLength = _solverCaches.shoulderToArmLength;
					float num = internalValues.bodyIK.shoulderLimitThetaYPlus.sin;
					float num2 = internalValues.bodyIK.shoulderLimitThetaYMinus.sin;
					float sin = internalValues.bodyIK.shoulderLimitThetaZ.sin;
					if (shoulderBones == null)
					{
						return;
					}
					if (_shouderLocalAxisYInv[i])
					{
						float num3 = num;
						num = num2;
						num2 = num3;
					}
					if (IsFuzzy(ref armPos[i], ref destArmPos))
					{
						return;
					}
					Vector3 v = destArmPos - shoulderPos[i];
					if (SAFBIKVecNormalize(ref v))
					{
						if (settings.bodyIK.shoulderLimitEnabled)
						{
							Matrix3x3 lhs = spineUBasis;
							SAFBIKMatMultRet0(ref lhs, ref shoulderBones[i]._localAxisBasis);
							SAFBIKMatMultVecInv(out v, ref lhs, ref v);
							_LimitYZ_Square(i != 0, ref v, num2, num, sin, sin);
							SAFBIKMatMultVec(out v, ref lhs, ref v);
						}
						armPos[i] = shoulderPos[i] + v * shoulderToArmLength[i];
					}
				}
			}

			private LimbIK[] _limbIK;

			private Bone _hipsBone;

			private Bone[] _spineBones;

			private bool[] _spineEnabled;

			private Matrix3x3[] _spinePrevCenterArmToChildBasis;

			private Matrix3x3[] _spineCenterArmToChildBasis;

			private Bone _spineBone;

			private Bone _spineUBone;

			private Bone _neckBone;

			private Bone _headBone;

			private Bone[] _kneeBones;

			private Bone[] _elbowBones;

			private Bone[] _legBones;

			private Bone[] _shoulderBones;

			private Bone[] _armBones;

			private Bone[] _nearArmBones;

			private float[] _spineDirXRate;

			private Effector _hipsEffector;

			private Effector _neckEffector;

			private Effector _headEffector;

			private Effector _eyesEffector;

			private Effector[] _armEffectors = new Effector[2];

			private Effector[] _elbowEffectors = new Effector[2];

			private Effector[] _wristEffectors = new Effector[2];

			private Effector[] _kneeEffectors = new Effector[2];

			private Effector[] _footEffectors = new Effector[2];

			private Vector3 _defaultCenterLegPos = Vector3.zero;

			private Matrix3x3 _centerLegBoneBasis = Matrix3x3.identity;

			private Matrix3x3 _centerLegBoneBasisInv = Matrix3x3.identity;

			private Matrix3x3 _centerLegToArmBasis = Matrix3x3.identity;

			private Matrix3x3 _centerLegToArmBasisInv = Matrix3x3.identity;

			private Matrix3x3 _centerLegToArmBoneToBaseBasis = Matrix3x3.identity;

			private Matrix3x3 _centerLegToArmBaseToBoneBasis = Matrix3x3.identity;

			private float[] _shoulderToArmLength = new float[2];

			private bool[] _shouderLocalAxisYInv = new bool[2];

			private FastLength[] _elbowEffectorMaxLength = new FastLength[2];

			private FastLength[] _wristEffectorMaxLength = new FastLength[2];

			private FastLength[] _kneeEffectorMaxLength = new FastLength[2];

			private FastLength[] _footEffectorMaxLength = new FastLength[2];

			private SolverCaches _solverCaches = new SolverCaches();

			private Vector3 _defaultCenterArmPos = Vector3.zero;

			private float _defaultCenterLegLen;

			private float _defaultCenterLegHalfLen;

			private float _defaultNearArmToNearArmLen;

			private float _defaultCenterLegToCeterArmLen;

			private Vector3 _defaultCenterEyePos = Vector3.zero;

			private SolverInternal _solverInternal;

			private Settings _settings;

			private InternalValues _internalValues;

			private bool _isSyncDisplacementAtLeastOnce;

			public BodyIK(FullBodyIK fullBodyIK, LimbIK[] limbIK)
			{
				_limbIK = limbIK;
				_settings = fullBodyIK.settings;
				_internalValues = fullBodyIK.internalValues;
				_hipsBone = _PrepareBone(fullBodyIK.bodyBones.hips);
				_neckBone = _PrepareBone(fullBodyIK.headBones.neck);
				_headBone = _PrepareBone(fullBodyIK.headBones.head);
				_hipsEffector = fullBodyIK.bodyEffectors.hips;
				_neckEffector = fullBodyIK.headEffectors.neck;
				_headEffector = fullBodyIK.headEffectors.head;
				_eyesEffector = fullBodyIK.headEffectors.eyes;
				_armEffectors[0] = fullBodyIK.leftArmEffectors.arm;
				_armEffectors[1] = fullBodyIK.rightArmEffectors.arm;
				_elbowEffectors[0] = fullBodyIK.leftArmEffectors.elbow;
				_elbowEffectors[1] = fullBodyIK.rightArmEffectors.elbow;
				_wristEffectors[0] = fullBodyIK.leftArmEffectors.wrist;
				_wristEffectors[1] = fullBodyIK.rightArmEffectors.wrist;
				_kneeEffectors[0] = fullBodyIK.leftLegEffectors.knee;
				_kneeEffectors[1] = fullBodyIK.rightLegEffectors.knee;
				_footEffectors[0] = fullBodyIK.leftLegEffectors.foot;
				_footEffectors[1] = fullBodyIK.rightLegEffectors.foot;
				_spineBones = _PrepareSpineBones(fullBodyIK.bones);
				if (_spineBones != null && _spineBones.Length != 0)
				{
					int num = _spineBones.Length;
					_spineBone = _spineBones[0];
					_spineUBone = _spineBones[num - 1];
					_spineEnabled = new bool[num];
				}
				_kneeBones = _PrepareBones(fullBodyIK.leftLegBones.knee, fullBodyIK.rightLegBones.knee);
				_elbowBones = _PrepareBones(fullBodyIK.leftArmBones.elbow, fullBodyIK.rightArmBones.elbow);
				_legBones = _PrepareBones(fullBodyIK.leftLegBones.leg, fullBodyIK.rightLegBones.leg);
				_armBones = _PrepareBones(fullBodyIK.leftArmBones.arm, fullBodyIK.rightArmBones.arm);
				_shoulderBones = _PrepareBones(fullBodyIK.leftArmBones.shoulder, fullBodyIK.rightArmBones.shoulder);
				_nearArmBones = ((_shoulderBones != null) ? _shoulderBones : _nearArmBones);
				_Prepare(fullBodyIK);
			}

			private static Bone[] _PrepareSpineBones(Bone[] bones)
			{
				if (bones == null || bones.Length != 79)
				{
					return null;
				}
				int num = 0;
				for (int i = 1; i <= 4; i++)
				{
					if (bones[i] != null && bones[i].transformIsAlive)
					{
						num++;
					}
				}
				if (num == 0)
				{
					return null;
				}
				Bone[] array = new Bone[num];
				int num2 = 0;
				for (int j = 1; j <= 4; j++)
				{
					if (bones[j] != null && bones[j].transformIsAlive)
					{
						array[num2] = bones[j];
						num2++;
					}
				}
				return array;
			}

			private void _Prepare(FullBodyIK fullBodyIK)
			{
				if (_spineBones == null)
				{
					return;
				}
				int num = _spineBones.Length;
				_spineDirXRate = new float[num];
				if (num > 1)
				{
					_spinePrevCenterArmToChildBasis = new Matrix3x3[num - 1];
					_spineCenterArmToChildBasis = new Matrix3x3[num - 1];
					for (int i = 0; i != num - 1; i++)
					{
						_spinePrevCenterArmToChildBasis[i] = Matrix3x3.identity;
						_spineCenterArmToChildBasis[i] = Matrix3x3.identity;
					}
				}
			}

			private void _SyncDisplacement()
			{
				if (_settings.syncDisplacement != SyncDisplacement.Everyframe && _isSyncDisplacementAtLeastOnce)
				{
					return;
				}
				_isSyncDisplacementAtLeastOnce = true;
				if (_shoulderBones != null)
				{
					for (int i = 0; i != 2; i++)
					{
						Vector3 column = _shoulderBones[i]._localAxisBasis.column1;
						_shouderLocalAxisYInv[i] = Vector3.Dot(column, _internalValues.defaultRootBasis.column1) < 0f;
					}
				}
				if (_eyesEffector != null)
				{
					_defaultCenterEyePos = _eyesEffector.defaultPosition;
				}
				if (_legBones != null)
				{
					_defaultCenterLegPos = (_legBones[0]._defaultPosition + _legBones[1]._defaultPosition) * 0.5f;
				}
				if (_nearArmBones != null)
				{
					_defaultCenterArmPos = (_nearArmBones[1]._defaultPosition + _nearArmBones[0]._defaultPosition) * 0.5f;
					Vector3 dirX = _nearArmBones[1]._defaultPosition - _nearArmBones[0]._defaultPosition;
					Vector3 v = _defaultCenterArmPos - _defaultCenterLegPos;
					if (SAFBIKVecNormalize(ref v) && SAFBIKComputeBasisFromXYLockY(out _centerLegToArmBasis, ref dirX, ref v))
					{
						_centerLegToArmBasisInv = _centerLegToArmBasis.transpose;
						SAFBIKMatMult(out _centerLegToArmBoneToBaseBasis, ref _centerLegToArmBasisInv, ref _internalValues.defaultRootBasis);
						_centerLegToArmBaseToBoneBasis = _centerLegToArmBoneToBaseBasis.transpose;
					}
				}
				_solverCaches.armBones = _armBones;
				_solverCaches.shoulderBones = _shoulderBones;
				_solverCaches.nearArmBones = _nearArmBones;
				_solverCaches.centerLegToNearArmBasis = _centerLegToArmBasis;
				_solverCaches.centerLegToNearArmBasisInv = _centerLegToArmBasisInv;
				_solverCaches.centerLegToNaerArmBoneToBaseBasis = _centerLegToArmBoneToBaseBasis;
				_solverCaches.centerLegToNaerArmBaseToBoneBasis = _centerLegToArmBaseToBoneBasis;
				_solverCaches.defaultCenterLegPos = _defaultCenterLegPos;
				_defaultCenterLegToCeterArmLen = SAFBIKVecLength2(ref _defaultCenterLegPos, ref _defaultCenterArmPos);
				if (_footEffectors != null && _footEffectors[0].bone != null && _footEffectors[1].bone != null)
				{
					_defaultCenterLegLen = SAFBIKVecLength2(ref _footEffectors[0].bone._defaultPosition, ref _footEffectors[1].bone._defaultPosition);
					_defaultCenterLegHalfLen = _defaultCenterLegLen * 0.5f;
				}
				if (_spineBone != null && _legBones != null && _ComputeCenterLegBasis(out _centerLegBoneBasis, ref _spineBone._defaultPosition, ref _legBones[0]._defaultPosition, ref _legBones[1]._defaultPosition))
				{
					_centerLegBoneBasisInv = _centerLegBoneBasis.transpose;
				}
				if (_armBones != null)
				{
					if (_shoulderBones != null)
					{
						for (int j = 0; j != 2; j++)
						{
							_shoulderToArmLength[j] = _armBones[j]._defaultLocalLength.length;
						}
					}
					_solverCaches.armToArmLen = SAFBIKVecLength2(ref _armBones[0]._defaultPosition, ref _armBones[1]._defaultPosition);
				}
				if (_nearArmBones != null)
				{
					_defaultNearArmToNearArmLen = SAFBIKVecLength2(ref _nearArmBones[0]._defaultPosition, ref _nearArmBones[1]._defaultPosition);
					if (_neckBone != null && _neckBone.transformIsAlive)
					{
						_solverCaches.nearArmToNeckLength[0] = SAFBIKVecLength2(ref _neckBone._defaultPosition, ref _nearArmBones[0]._defaultPosition);
						_solverCaches.nearArmToNeckLength[1] = SAFBIKVecLength2(ref _neckBone._defaultPosition, ref _nearArmBones[1]._defaultPosition);
					}
				}
				if (_neckBone != null && _headBone != null)
				{
					_solverCaches.neckToHeadLength = SAFBIKVecLength2(ref _neckBone._defaultPosition, ref _headBone._defaultPosition);
				}
				_solverCaches.shoulderToArmLength = _shoulderToArmLength;
				_solverCaches.nearArmToNearArmLen = _defaultNearArmToNearArmLen;
				if (_kneeBones != null && _footEffectors != null)
				{
					for (int k = 0; k != 2; k++)
					{
						Bone bone = _kneeBones[k];
						Bone bone2 = _footEffectors[k].bone;
						_kneeEffectorMaxLength[k] = bone._defaultLocalLength;
						_footEffectorMaxLength[k] = FastLength.FromLength(bone._defaultLocalLength.length + bone2._defaultLocalLength.length);
					}
				}
				if (_elbowBones != null && _wristEffectors != null)
				{
					for (int l = 0; l != 2; l++)
					{
						Bone bone3 = _elbowBones[l];
						Bone bone4 = _wristEffectors[l].bone;
						_elbowEffectorMaxLength[l] = bone3._defaultLocalLength;
						_wristEffectorMaxLength[l] = FastLength.FromLength(bone3._defaultLocalLength.length + bone4._defaultLocalLength.length);
					}
				}
				if (_spineBones == null || (_nearArmBones == null && _legBones == null) || _neckBone == null || !_neckBone.transformIsAlive)
				{
					return;
				}
				Vector3 dirX2 = _internalValues.defaultRootBasis.column0;
				int num = _spineBones.Length;
				for (int m = 0; m < num - 1; m++)
				{
					Vector3 vector = ((m != 0) ? _spineBones[m - 1]._defaultPosition : _defaultCenterLegPos);
					Vector3 v2 = _defaultCenterArmPos - vector;
					Vector3 v3 = _defaultCenterArmPos - _spineBones[m]._defaultPosition;
					Matrix3x3 basis;
					Matrix3x3 basis2;
					if (SAFBIKVecNormalize2(ref v2, ref v3) && SAFBIKComputeBasisFromXYLockY(out basis, ref dirX2, ref v2) && SAFBIKComputeBasisFromXYLockY(out basis2, ref dirX2, ref v3))
					{
						SAFBIKMatMultInv0(out _spinePrevCenterArmToChildBasis[m], ref basis, ref _spineBones[m]._localAxisBasis);
						SAFBIKMatMultInv0(out _spineCenterArmToChildBasis[m], ref basis2, ref _spineBones[m]._localAxisBasis);
					}
				}
			}

			public bool Solve()
			{
				if (!_IsEffectorEnabled() && !_settings.bodyIK.forceSolveEnabled)
				{
					return false;
				}
				_SyncDisplacement();
				if (!_PrepareSolverInternal())
				{
					return false;
				}
				SolverInternal solverInternal = _solverInternal;
				if (!_internalValues.resetTransforms)
				{
					if (solverInternal.spinePos != null)
					{
						for (int i = 0; i != _spineBones.Length; i++)
						{
							if (_spineBones[i] != null)
							{
								solverInternal.spinePos[i] = _spineBones[i].worldPosition;
							}
						}
					}
					if (_neckBone != null)
					{
						solverInternal.neckPos = _neckBone.worldPosition;
					}
					if (_headBone != null)
					{
						solverInternal.headPos = _headBone.worldPosition;
					}
					if (solverInternal.shoulderPos != null)
					{
						for (int j = 0; j < 2; j++)
						{
							solverInternal.shoulderPos[j] = _shoulderBones[j].worldPosition;
						}
					}
					if (solverInternal.armPos != null)
					{
						for (int k = 0; k != 2; k++)
						{
							solverInternal.armPos[k] = _armBones[k].worldPosition;
						}
					}
					if (solverInternal.legPos != null)
					{
						for (int l = 0; l != 2; l++)
						{
							solverInternal.legPos[l] = _legBones[l].worldPosition;
						}
					}
					solverInternal.SetDirtyVariables();
				}
				if (_internalValues.resetTransforms)
				{
					_ResetTransforms();
				}
				else if (_internalValues.animatorEnabled)
				{
					_PresolveHips();
				}
				if (!_internalValues.resetTransforms && _settings.bodyIK.shoulderSolveEnabled)
				{
					_ResetShoulderTransform();
				}
				_solverInternal.arms.Prepare(_elbowEffectors, _wristEffectors);
				_solverInternal.legs.Prepare(_kneeEffectors, _footEffectors);
				if (_settings.bodyIK.lowerSolveEnabled)
				{
					_LowerSolve(true);
				}
				if (_settings.bodyIK.upperSolveEnabled)
				{
					_UpperSolve();
				}
				if (_settings.bodyIK.lowerSolveEnabled)
				{
					_LowerSolve(false);
				}
				if (_settings.bodyIK.shoulderSolveEnabled)
				{
					_ShoulderResolve();
				}
				if (_settings.bodyIK.computeWorldTransform)
				{
					_ComputeWorldTransform();
				}
				return true;
			}

			private bool _UpperSolve()
			{
				SolverInternal solverInternal = _solverInternal;
				float num = (_hipsEffector.positionEnabled ? _hipsEffector.pull : 0f);
				float neckPull = _solverCaches.neckPull;
				float headPull = _solverCaches.headPull;
				float eyesRate = _solverCaches.eyesRate;
				float[] armPull = _solverCaches.armPull;
				float[] elbowPull = _solverCaches.elbowPull;
				float[] wristPull = _solverCaches.wristPull;
				if (!_settings.bodyIK.forceSolveEnabled && num <= 1E-07f && neckPull <= 1E-07f && headPull <= 1E-07f && eyesRate <= 1E-07f && armPull[0] <= 1E-07f && armPull[1] <= 1E-07f && elbowPull[0] <= 1E-07f && elbowPull[1] <= 1E-07f && wristPull[0] <= 1E-07f && wristPull[1] <= 1E-07f)
				{
					return false;
				}
				Vector3 stableCenterLegPos = Vector3.zero;
				bool continuousSolverEnabled = _internalValues.continuousSolverEnabled;
				if (continuousSolverEnabled)
				{
					Matrix3x3 centerLegBasis;
					_UpperSolve_PresolveBaseCenterLegTransform(out stableCenterLegPos, out centerLegBasis);
					solverInternal.Backup();
					if (_spineBones != null)
					{
						for (int i = 0; i < _spineBones.Length; i++)
						{
							SAFBIKMatMultVecPreSubAdd(out solverInternal.spinePos[i], ref centerLegBasis, ref _spineBones[i]._defaultPosition, ref _defaultCenterLegPos, ref stableCenterLegPos);
						}
					}
					if (_neckBone != null)
					{
						SAFBIKMatMultVecPreSubAdd(out solverInternal.neckPos, ref centerLegBasis, ref _neckBone._defaultPosition, ref _defaultCenterLegPos, ref stableCenterLegPos);
					}
					if (_headBone != null && solverInternal.headEnabled)
					{
						SAFBIKMatMultVecPreSubAdd(out solverInternal.headPos, ref centerLegBasis, ref _headBone._defaultPosition, ref _defaultCenterLegPos, ref stableCenterLegPos);
					}
					for (int j = 0; j < 2; j++)
					{
						if (_shoulderBones != null)
						{
							SAFBIKMatMultVecPreSubAdd(out solverInternal.shoulderPos[j], ref centerLegBasis, ref _shoulderBones[j]._defaultPosition, ref _defaultCenterLegPos, ref stableCenterLegPos);
						}
						if (_armBones != null)
						{
							SAFBIKMatMultVecPreSubAdd(out solverInternal.armPos[j], ref centerLegBasis, ref _armBones[j]._defaultPosition, ref _defaultCenterLegPos, ref stableCenterLegPos);
						}
						if (_legBones != null)
						{
							SAFBIKMatMultVecPreSubAdd(out solverInternal.legPos[j], ref centerLegBasis, ref _legBones[j]._defaultPosition, ref _defaultCenterLegPos, ref stableCenterLegPos);
						}
					}
					solverInternal.SetDirtyVariables();
				}
				solverInternal.UpperSolve();
				float value = _internalValues.bodyIK.upperCenterLegRotateRate.value;
				float value2 = _internalValues.bodyIK.upperSpineRotateRate.value;
				float value3 = _internalValues.bodyIK.upperCenterLegTranslateRate.value;
				float value4 = _internalValues.bodyIK.upperSpineTranslateRate.value;
				Vector3 targetCenterArmPos = solverInternal.targetCenterArmPos;
				Vector3 dst = solverInternal.targetCenterArmDir;
				Vector3 currentCenterArmPos = solverInternal.currentCenterArmPos;
				Vector3 src = solverInternal.currentCenterArmDir;
				Vector3 dir = _LerpDir(ref src, ref dst, value);
				Vector3 dir2 = _LerpDir(ref src, ref dst, value2);
				Vector3 vector = Vector3.Lerp(currentCenterArmPos, targetCenterArmPos, value3);
				Vector3 vector2 = Vector3.Lerp(currentCenterArmPos, targetCenterArmPos, value4);
				Vector3 v = vector - solverInternal.centerLegPos;
				Vector3 v2 = vector2 - solverInternal.centerLegPos;
				if (!SAFBIKVecNormalize2(ref v, ref v2))
				{
					return false;
				}
				if (_settings.bodyIK.upperDirXLimitEnabled)
				{
					if (_internalValues.bodyIK.upperDirXLimitThetaY.sin <= 1E-07f)
					{
						if (!_FitToPlaneDir(ref dir, v) || !_FitToPlaneDir(ref dir2, v2))
						{
							return false;
						}
					}
					else if (!_LimitToPlaneDirY(ref dir, v, _internalValues.bodyIK.upperDirXLimitThetaY.sin) || !_LimitToPlaneDirY(ref dir2, v2, _internalValues.bodyIK.upperDirXLimitThetaY.sin))
					{
						return false;
					}
				}
				if (_settings.bodyIK.spineLimitEnabled)
				{
					float value5 = _internalValues.bodyIK.spineLimitAngleX.value;
					float value6 = _internalValues.bodyIK.spineLimitAngleY.value;
					if (_settings.bodyIK.spineAccurateLimitEnabled)
					{
						if (SAFBIKAcos(Vector3.Dot(dir, dir2)) > value5)
						{
							Vector3 v3 = Vector3.Cross(dir, dir2);
							if (SAFBIKVecNormalize(ref v3))
							{
								Quaternion q = Quaternion.AngleAxis(_settings.bodyIK.spineLimitAngleX, v3);
								Matrix3x3 m;
								SAFBIKMatSetRot(out m, ref q);
								SAFBIKMatMultVec(out dir2, ref m, ref dir);
							}
						}
						if (SAFBIKAcos(Vector3.Dot(v, v2)) > value6)
						{
							Vector3 v4 = Vector3.Cross(v, v2);
							if (SAFBIKVecNormalize(ref v4))
							{
								Quaternion q2 = Quaternion.AngleAxis(_settings.bodyIK.spineLimitAngleY, v4);
								Matrix3x3 m2;
								SAFBIKMatSetRot(out m2, ref q2);
								SAFBIKMatMultVec(out v2, ref m2, ref v);
							}
						}
					}
					else
					{
						float num2 = SAFBIKAcos(Vector3.Dot(dir, dir2));
						if (num2 > value5 && num2 > 1E-07f)
						{
							float t = value5 / num2;
							Vector3 v5 = Vector3.Lerp(dir, dir2, t);
							if (SAFBIKVecNormalize(ref v5))
							{
								dir2 = v5;
							}
						}
						float num3 = SAFBIKAcos(Vector3.Dot(v, v2));
						if (num3 > value6 && num3 > 1E-07f)
						{
							float t2 = value6 / num3;
							Vector3 v6 = Vector3.Lerp(v, v2, t2);
							if (SAFBIKVecNormalize(ref v6))
							{
								v2 = v6;
							}
						}
					}
				}
				Vector3 centerLegPos = solverInternal.centerLegPos;
				Vector3 addVec = solverInternal.centerLegPos;
				if (eyesRate > 1E-07f)
				{
					Vector3 vector3 = solverInternal.centerLegPos + v2 * _defaultCenterLegToCeterArmLen;
					addVec += solverInternal.targetCenterArmPos - vector3;
				}
				Matrix3x3 basis;
				if (eyesRate > 1E-07f && SAFBIKComputeBasisFromXYLockY(out basis, ref dir2, ref v2))
				{
					Matrix3x3 ret;
					SAFBIKMatMult(out ret, ref basis, ref _centerLegToArmBasisInv);
					Matrix3x3 lhs = basis;
					SAFBIKMatMultRet0(ref basis, ref _centerLegToArmBoneToBaseBasis);
					Vector3 ret2;
					SAFBIKMatMultVecPreSubAdd(out ret2, ref ret, ref _defaultCenterEyePos, ref _defaultCenterLegPos, ref addVec);
					Vector3 ret3 = _eyesEffector.worldPosition - ret2;
					float sin = _internalValues.bodyIK.upperEyesLimitYaw.sin;
					float sin2 = _internalValues.bodyIK.upperEyesLimitPitchUp.sin;
					float sin3 = _internalValues.bodyIK.upperEyesLimitPitchDown.sin;
					SAFBIKMatMultVecInv(out ret3, ref basis, ref ret3);
					ret3.x *= _settings.bodyIK.upperEyesYawRate;
					if (ret3.y >= 0f)
					{
						ret3.y *= _settings.bodyIK.upperEyesPitchUpRate;
					}
					else
					{
						ret3.y *= _settings.bodyIK.upperEyesPitchDownRate;
					}
					SAFBIKVecNormalize(ref ret3);
					if (_ComputeEyesRange(ref ret3, _internalValues.bodyIK.upperEyesTraceTheta.cos))
					{
						_LimitXY(ref ret3, sin, sin, sin3, sin2);
					}
					SAFBIKMatMultVec(out ret3, ref basis, ref ret3);
					Vector3 dirX = basis.column0;
					Vector3 dirY = basis.column1;
					Vector3 dirZ = ret3;
					SAFBIKComputeBasisLockZ(out basis, ref dirX, ref dirY, ref dirZ);
					SAFBIKMatMultRet0(ref basis, ref _centerLegToArmBaseToBoneBasis);
					float num4 = _settings.bodyIK.upperEyesToCenterLegRate * eyesRate;
					float num5 = _settings.bodyIK.upperEyesToSpineRate * eyesRate;
					Matrix3x3 ret4;
					if (num5 > 1E-07f)
					{
						SAFBIKMatFastLerp(out ret4, ref lhs, ref basis, num5);
						dir2 = ret4.column0;
						v2 = ret4.column1;
					}
					if (num4 > 1E-07f && SAFBIKComputeBasisFromXYLockY(out lhs, ref dir, ref v))
					{
						SAFBIKMatFastLerp(out ret4, ref lhs, ref basis, num4);
						dir = ret4.column0;
						v = ret4.column1;
					}
				}
				if (continuousSolverEnabled)
				{
					solverInternal.Restore();
					solverInternal.UpperSolve();
				}
				int num6 = ((_spineBones != null) ? _spineBones.Length : 0);
				float upperContinuousCenterLegRotationStableRate = _settings.bodyIK.upperContinuousCenterLegRotationStableRate;
				Matrix3x3 basis2;
				if (_settings.bodyIK.upperSolveHipsEnabled && SAFBIKComputeBasisFromXYLockY(out basis2, ref dir, ref v))
				{
					Matrix3x3 ret5 = Matrix3x3.identity;
					if (_internalValues.animatorEnabled || _internalValues.resetTransforms)
					{
						if (continuousSolverEnabled && upperContinuousCenterLegRotationStableRate > 1E-07f)
						{
							Matrix3x3 basis3 = Matrix3x3.identity;
							Vector3 v7 = vector - centerLegPos;
							Vector3 dirX2 = dir;
							if (SAFBIKVecNormalize(ref v7) && SAFBIKComputeBasisFromXYLockY(out basis3, ref dirX2, ref v7))
							{
								Matrix3x3 ret6;
								SAFBIKMatFastLerp(out ret6, ref basis2, ref basis3, upperContinuousCenterLegRotationStableRate);
								basis2 = ret6;
							}
						}
						Vector3 dirX3 = solverInternal.nearArmPos[1] - solverInternal.nearArmPos[0];
						Vector3 v8 = (solverInternal.nearArmPos[1] + solverInternal.nearArmPos[0]) * 0.5f - solverInternal.centerLegPos;
						Matrix3x3 basis4;
						if (SAFBIKVecNormalize(ref v8) && SAFBIKComputeBasisFromXYLockY(out basis4, ref dirX3, ref v8))
						{
							SAFBIKMatMultInv1(out ret5, ref basis2, ref basis4);
						}
					}
					else
					{
						SAFBIKMatMultRet0(ref basis2, ref _centerLegToArmBasisInv);
						if (continuousSolverEnabled && upperContinuousCenterLegRotationStableRate > 1E-07f)
						{
							Matrix3x3 basis5 = Matrix3x3.identity;
							Vector3 v9 = vector - centerLegPos;
							Vector3 dirX4 = dir;
							if (SAFBIKVecNormalize(ref v9) && SAFBIKComputeBasisFromXYLockY(out basis5, ref dirX4, ref v9))
							{
								SAFBIKMatMultRet0(ref basis5, ref _centerLegToArmBasisInv);
								Matrix3x3 ret7;
								SAFBIKMatFastLerp(out ret7, ref basis2, ref basis5, upperContinuousCenterLegRotationStableRate);
								basis2 = ret7;
							}
						}
						Matrix3x3 rhs = solverInternal.centerLegBasis;
						SAFBIKMatMultInv1(out ret5, ref basis2, ref rhs);
					}
					if (_settings.bodyIK.upperCenterLegLerpRate < 0.9999999f)
					{
						SAFBIKMatFastLerpToIdentity(ref ret5, 1f - _settings.bodyIK.upperCenterLegLerpRate);
					}
					solverInternal.UpperRotation(-1, ref ret5);
				}
				float defaultCenterLegToCeterArmLen = _defaultCenterLegToCeterArmLen;
				Vector3 column = solverInternal.centerLegBasis.column0;
				Vector3 vector4 = v2 * defaultCenterLegToCeterArmLen + solverInternal.centerLegPos;
				float upperSpineLerpRate = _settings.bodyIK.upperSpineLerpRate;
				for (int k = 0; k != num6; k++)
				{
					if (!_spineEnabled[k])
					{
						continue;
					}
					Vector3 vector5 = solverInternal.spinePos[k];
					if (k + 1 == num6)
					{
						Vector3 dirX5 = solverInternal.nearArmPos[1] - solverInternal.nearArmPos[0];
						Vector3 v10 = solverInternal.centerArmPos - vector5;
						Vector3 dirX6 = dir2;
						Vector3 v11 = vector4 - vector5;
						if (SAFBIKVecNormalize2(ref v10, ref v11))
						{
							Matrix3x3 basis6;
							SAFBIKComputeBasisFromXYLockY(out basis6, ref dirX6, ref v11);
							Matrix3x3 basis7;
							SAFBIKComputeBasisFromXYLockY(out basis7, ref dirX5, ref v10);
							Matrix3x3 ret8;
							SAFBIKMatMultInv1(out ret8, ref basis6, ref basis7);
							if (upperSpineLerpRate < 0.9999999f)
							{
								SAFBIKMatFastLerpToIdentity(ref ret8, 1f - upperSpineLerpRate);
							}
							solverInternal.UpperRotation(k, ref ret8);
						}
						continue;
					}
					Vector3 obj = ((k + 1 == num6) ? solverInternal.neckPos : solverInternal.spinePos[k + 1]);
					Vector3 vector6 = ((k != 0) ? solverInternal.spinePos[k - 1] : solverInternal.centerLegPos);
					Vector3 v12 = solverInternal.nearArmPos[1] - solverInternal.nearArmPos[0];
					Vector3 v13 = obj - vector5;
					Vector3 b = dir2;
					Vector3 v14 = vector4 - vector6;
					Vector3 v15 = vector4 - vector5;
					Matrix3x3 lhs2;
					Matrix3x3 lhs3;
					if (!SAFBIKVecNormalize4(ref v12, ref v13, ref v14, ref v15) || !SAFBIKComputeBasisFromXYLockY(out lhs2, ref v12, ref v14) || !SAFBIKComputeBasisFromXYLockY(out lhs3, ref v12, ref v15))
					{
						continue;
					}
					SAFBIKMatMultCol1(out v14, ref lhs2, ref _spinePrevCenterArmToChildBasis[k]);
					SAFBIKMatMultCol1(out v15, ref lhs3, ref _spineCenterArmToChildBasis[k]);
					float t3 = _spineDirXRate[k];
					v12 = Vector3.Lerp(column, v12, t3);
					b = Vector3.Lerp(column, b, t3);
					if (k + 1 != num6)
					{
						v14 = Vector3.Lerp(v14, v15, _settings.bodyIK.spineDirYLerpRate);
						if (!SAFBIKVecNormalize(ref v14))
						{
							v14 = v13;
						}
					}
					Matrix3x3 basis8;
					SAFBIKComputeBasisFromXYLockY(out basis8, ref b, ref v14);
					Matrix3x3 basis9;
					SAFBIKComputeBasisFromXYLockY(out basis9, ref v12, ref v13);
					Matrix3x3 ret9;
					SAFBIKMatMultInv1(out ret9, ref basis8, ref basis9);
					if (upperSpineLerpRate < 0.9999999f)
					{
						SAFBIKMatFastLerpToIdentity(ref ret9, 1f - upperSpineLerpRate);
					}
					solverInternal.UpperRotation(k, ref ret9);
				}
				_UpperSolve_Translate2(ref _internalValues.bodyIK.upperPostTranslateRate, ref _internalValues.bodyIK.upperContinuousPostTranslateStableRate, ref stableCenterLegPos);
				return true;
			}

			private void _ShoulderResolve()
			{
				_solverInternal.ShoulderResolve();
			}

			private void _UpperSolve_PresolveBaseCenterLegTransform(out Vector3 centerLegPos, out Matrix3x3 centerLegBasis)
			{
				SolverInternal solverInternal = _solverInternal;
				SolverCaches solverCaches = _solverCaches;
				_GetBaseCenterLegTransform(out centerLegPos, out centerLegBasis);
				if (_legBones != null && _legBones[0].transformIsAlive && _legBones[1].transformIsAlive && (!(solverCaches.limbLegPull[0] <= 1E-07f) || !(solverCaches.limbLegPull[1] <= 1E-07f)))
				{
					Vector3 ret;
					SAFBIKMatMultVecPreSubAdd(out ret, ref centerLegBasis, ref _legBones[0]._defaultPosition, ref _defaultCenterLegPos, ref centerLegPos);
					Vector3 ret2;
					SAFBIKMatMultVecPreSubAdd(out ret2, ref centerLegBasis, ref _legBones[1]._defaultPosition, ref _defaultCenterLegPos, ref centerLegPos);
					if (false | solverInternal.legs.SolveTargetBeginPos(0, ref ret) | solverInternal.legs.SolveTargetBeginPos(1, ref ret2))
					{
						Vector3 vector = centerLegBasis.column0 * _defaultCenterLegHalfLen;
						centerLegPos = Vector3.Lerp(ret + vector, ret2 - vector, solverCaches.limbLegRate);
					}
				}
			}

			private void _UpperSolve_Transform(int origIndex, ref Matrix3x3 transformBasis)
			{
				SolverInternal solverInternal = _solverInternal;
				Vector3 subVec = ((origIndex == -1) ? solverInternal.centerLegPos : solverInternal.spinePos[origIndex]);
				for (int i = 0; i != 2; i++)
				{
					SAFBIKMatMultVecPreSubAdd(out solverInternal.armPos[i], ref transformBasis, ref solverInternal.armPos[i], ref subVec, ref subVec);
					if (_shoulderBones != null)
					{
						SAFBIKMatMultVecPreSubAdd(out solverInternal.shoulderPos[i], ref transformBasis, ref solverInternal.shoulderPos[i], ref subVec, ref subVec);
					}
				}
				int num = ((_spineBones != null) ? _spineBones.Length : 0);
				for (int j = origIndex + 1; j < num; j++)
				{
					SAFBIKMatMultVecPreSubAdd(out solverInternal.spinePos[j], ref transformBasis, ref solverInternal.spinePos[j], ref subVec, ref subVec);
				}
				if (_neckBone != null)
				{
					SAFBIKMatMultVecPreSubAdd(out solverInternal.neckPos, ref transformBasis, ref solverInternal.neckPos, ref subVec, ref subVec);
				}
				if (origIndex == -1 && solverInternal.legPos != null)
				{
					for (int k = 0; k < 2; k++)
					{
						SAFBIKMatMultVecPreSubAdd(out solverInternal.legPos[k], ref transformBasis, ref solverInternal.legPos[k], ref subVec, ref subVec);
					}
				}
				solverInternal.SetDirtyVariables();
			}

			private bool _UpperSolve_PreTranslate2(out Vector3 translate, ref CachedRate01 translateRate, ref CachedRate01 stableRate, ref Vector3 stableCenterLegPos)
			{
				translate = Vector3.zero;
				if (_hipsEffector.positionEnabled && _hipsEffector.positionWeight <= 1E-07f && _hipsEffector.pull >= 0.9999999f)
				{
					return false;
				}
				SolverInternal solverInternal = _solverInternal;
				bool continuousSolverEnabled = _internalValues.continuousSolverEnabled;
				bool flag = continuousSolverEnabled && stableRate.isGreater0;
				if (solverInternal.targetCenterArmEnabled)
				{
					translate = solverInternal.targetCenterArmPos - solverInternal.currentCenterArmPos;
					flag = true;
				}
				if (flag)
				{
					if (translateRate.isLess1)
					{
						translate *= translateRate.value;
					}
					if (continuousSolverEnabled && stableRate.isGreater0)
					{
						Vector3 b = stableCenterLegPos - solverInternal.centerLegPos;
						translate = Vector3.Lerp(translate, b, stableRate.value);
					}
					if (_hipsEffector.positionEnabled && _hipsEffector.pull > 1E-07f)
					{
						Vector3 b2 = _hipsEffector._hidden_worldPosition - solverInternal.centerLegPos;
						translate = Vector3.Lerp(translate, b2, _hipsEffector.pull);
					}
					return true;
				}
				return false;
			}

			private void _UpperSolve_Translate2(ref CachedRate01 translateRate, ref CachedRate01 stableRate, ref Vector3 stableCenterLegPos)
			{
				Vector3 translate;
				if (_UpperSolve_PreTranslate2(out translate, ref translateRate, ref stableRate, ref stableCenterLegPos))
				{
					_solverInternal.Translate(ref translate);
				}
			}

			private void _LowerSolve(bool firstPass)
			{
				SolverInternal solverInternal = _solverInternal;
				SolverCaches solverCaches = _solverCaches;
				if (solverInternal.spinePos == null || solverInternal.spinePos.Length == 0)
				{
					return;
				}
				float num = (firstPass ? 1f : solverCaches.armToLegRate);
				if (solverInternal.PrepareLowerRotation(0))
				{
					Vector3 column = solverInternal.centerLegBasis.column1;
					for (int i = 0; i < 2; i++)
					{
						if (solverInternal.legs.endPosEnabled[i] && solverInternal.legs.targetBeginPosEnabled[i])
						{
							Vector3 v = solverInternal.legs.targetBeginPos[i] - solverInternal.legs.beginPos[i];
							if (SAFBIKVecNormalize(ref v))
							{
								float value = Vector3.Dot(column, -v);
								value = Mathf.Clamp01(value);
								value = 1f - value;
								solverInternal.SetSolveFeedbackRate(i, value * num);
							}
						}
					}
					Quaternion origRotation;
					if (solverInternal.SolveLowerRotation(0, out origRotation))
					{
						solverInternal.LowerRotation(0, ref origRotation, false);
					}
				}
				Vector3 translate;
				if ((!_hipsEffector.positionEnabled || !(_hipsEffector.positionWeight <= 1E-07f) || !(_hipsEffector.pull >= 0.9999999f)) && solverInternal.PrepareLowerTranslate() && solverInternal.SolveLowerTranslate(out translate))
				{
					if (num < 0.9999999f)
					{
						translate *= num;
					}
					if (_hipsEffector.positionEnabled && _hipsEffector.pull > 1E-07f)
					{
						Vector3 b = _hipsEffector._hidden_worldPosition - solverInternal.centerLegPos;
						translate = Vector3.Lerp(translate, b, _hipsEffector.pull);
					}
					solverInternal.Translate(ref translate);
				}
			}

			private void _ComputeWorldTransform()
			{
				SolverInternal solverInternal = _solverInternal;
				if (solverInternal == null || solverInternal.spinePos == null || solverInternal.spinePos.Length == 0 || _hipsBone == null || !_hipsBone.transformIsAlive || solverInternal.spinePos == null || solverInternal.spinePos.Length == 0 || _neckBone == null || !_neckBone.transformIsAlive)
				{
					return;
				}
				Vector3 vector = new Vector3(1f, 0f, 0f);
				Vector3 dirX = solverInternal.legs.beginPos[1] - solverInternal.legs.beginPos[0];
				Vector3 v = solverInternal.spinePos[0] - (solverInternal.legs.beginPos[1] + solverInternal.legs.beginPos[0]) * 0.5f;
				Matrix3x3 basis = default(Matrix3x3);
				if (SAFBIKVecNormalize(ref v) && SAFBIKComputeBasisFromXYLockY(out basis, ref dirX, ref v))
				{
					Matrix3x3 ret;
					SAFBIKMatMult(out ret, ref basis, ref _centerLegBoneBasisInv);
					vector = basis.column0;
					Quaternion ret2;
					SAFBIKMatMultGetRot(out ret2, ref ret, ref _hipsBone._defaultBasis);
					_hipsBone.worldRotation = ret2;
					if (_hipsBone.isWritebackWorldPosition)
					{
						Vector3 vec = -_spineBone._defaultLocalTranslate;
						Vector3 ret3;
						SAFBIKMatMultVecAdd(out ret3, ref ret, ref vec, ref solverInternal.spinePos[0]);
						_hipsBone.worldPosition = ret3;
					}
				}
				else if (SAFBIKVecNormalize(ref dirX))
				{
					vector = dirX;
				}
				int num = solverInternal.spinePos.Length;
				for (int i = 0; i != num; i++)
				{
					if (!_spineEnabled[i])
					{
						continue;
					}
					if (i + 1 == num)
					{
						v = solverInternal.neckPos - solverInternal.spinePos[i];
						dirX = ((solverInternal.nearArmPos == null) ? vector : (solverInternal.nearArmPos[1] - solverInternal.nearArmPos[0]));
					}
					else
					{
						v = solverInternal.spinePos[i + 1] - solverInternal.spinePos[i];
						dirX = vector;
						if (solverInternal.nearArmPos != null)
						{
							Vector3 v2 = solverInternal.nearArmPos[1] - solverInternal.nearArmPos[0];
							if (SAFBIKVecNormalize(ref v2))
							{
								dirX = Vector3.Lerp(dirX, v2, _settings.bodyIK.spineDirXLegToArmRate);
							}
						}
					}
					if (SAFBIKVecNormalize(ref v) && SAFBIKComputeBasisFromXYLockY(out basis, ref dirX, ref v))
					{
						vector = basis.column0;
						Quaternion ret4;
						SAFBIKMatMultGetRot(out ret4, ref basis, ref _spineBones[i]._boneToWorldBasis);
						_spineBones[i].worldRotation = ret4;
						if (_spineBones[i].isWritebackWorldPosition)
						{
							_spineBones[i].worldPosition = solverInternal.spinePos[i];
						}
					}
				}
				if (_shoulderBones == null)
				{
					return;
				}
				for (int j = 0; j != 2; j++)
				{
					Vector3 vector2 = solverInternal.armPos[j] - solverInternal.shoulderPos[j];
					Vector3 rhs = ((_internalValues.shoulderDirYAsNeck == 0) ? (solverInternal.shoulderPos[j] - solverInternal.spineUPos) : (solverInternal.neckPos - solverInternal.shoulderPos[j]));
					vector2 = ((j == 0) ? (-vector2) : vector2);
					Vector3 v3 = Vector3.Cross(vector2, rhs);
					rhs = Vector3.Cross(v3, vector2);
					if (SAFBIKVecNormalize3(ref vector2, ref rhs, ref v3))
					{
						basis.SetColumn(ref vector2, ref rhs, ref v3);
						Quaternion ret5;
						SAFBIKMatMultGetRot(out ret5, ref basis, ref _shoulderBones[j]._boneToWorldBasis);
						_shoulderBones[j].worldRotation = ret5;
					}
				}
			}

			private bool _IsEffectorEnabled()
			{
				if ((_hipsEffector.positionEnabled && _hipsEffector.pull > 1E-07f) || (_hipsEffector.rotationEnabled && _hipsEffector.rotationWeight > 1E-07f) || (_neckEffector.positionEnabled && _neckEffector.pull > 1E-07f) || (_eyesEffector.positionEnabled && _eyesEffector.positionWeight > 1E-07f && _eyesEffector.pull > 1E-07f) || (_armEffectors[0].positionEnabled && _armEffectors[0].pull > 1E-07f) || (_armEffectors[1].positionEnabled && _armEffectors[1].pull > 1E-07f))
				{
					return true;
				}
				if ((_elbowEffectors[0].positionEnabled && _elbowEffectors[0].pull > 1E-07f) || (_elbowEffectors[1].positionEnabled && _elbowEffectors[1].pull > 1E-07f) || (_kneeEffectors[0].positionEnabled && _kneeEffectors[0].pull > 1E-07f) || (_kneeEffectors[1].positionEnabled && _kneeEffectors[1].pull > 1E-07f) || (_wristEffectors[0].positionEnabled && _wristEffectors[0].pull > 1E-07f) || (_wristEffectors[1].positionEnabled && _wristEffectors[1].pull > 1E-07f) || (_footEffectors[0].positionEnabled && _footEffectors[0].pull > 1E-07f) || (_footEffectors[1].positionEnabled && _footEffectors[1].pull > 1E-07f))
				{
					return true;
				}
				return false;
			}

			private bool _PrepareSolverInternal()
			{
				if (_armBones == null || _legBones == null)
				{
					_solverInternal = null;
					return false;
				}
				if (_neckEffector != null)
				{
					_solverCaches.neckPull = (_neckEffector.positionEnabled ? _neckEffector.pull : 0f);
				}
				if (_headEffector != null)
				{
					_solverCaches.headPull = (_headEffector.positionEnabled ? _headEffector.pull : 0f);
				}
				if (_eyesEffector != null)
				{
					_solverCaches.eyesRate = (_eyesEffector.positionEnabled ? (_eyesEffector.pull * _eyesEffector.positionWeight) : 0f);
				}
				for (int i = 0; i != 2; i++)
				{
					if (_armEffectors[i] != null)
					{
						_solverCaches.armPull[i] = (_armEffectors[i].positionEnabled ? _armEffectors[i].pull : 0f);
					}
					if (_elbowEffectors[i] != null)
					{
						_solverCaches.elbowPull[i] = (_elbowEffectors[i].positionEnabled ? _elbowEffectors[i].pull : 0f);
					}
					if (_wristEffectors[i] != null)
					{
						_solverCaches.wristPull[i] = (_wristEffectors[i].positionEnabled ? _wristEffectors[i].pull : 0f);
					}
					if (_kneeEffectors[i] != null)
					{
						_solverCaches.kneePull[i] = (_kneeEffectors[i].positionEnabled ? _kneeEffectors[i].pull : 0f);
					}
					if (_footEffectors[i] != null)
					{
						_solverCaches.footPull[i] = (_footEffectors[i].positionEnabled ? _footEffectors[i].pull : 0f);
					}
				}
				_solverCaches.neckHeadPull = _ConcatPull(_solverCaches.neckPull, _solverCaches.headPull);
				float num = _solverCaches.neckHeadPull;
				float num2 = 0f;
				for (int j = 0; j != 2; j++)
				{
					float num3 = _solverCaches.armPull[j];
					num3 = ((num3 != 0f) ? _ConcatPull(num3, _solverCaches.elbowPull[j]) : _solverCaches.elbowPull[j]);
					num3 = ((num3 != 0f) ? _ConcatPull(num3, _solverCaches.wristPull[j]) : _solverCaches.wristPull[j]);
					float num4 = _solverCaches.kneePull[j];
					if (num4 == 0f)
					{
						num4 = _solverCaches.wristPull[j];
					}
					else
					{
						num4 = _ConcatPull(num4, _solverCaches.wristPull[j]);
					}
					float num5 = _solverCaches.kneePull[j];
					num5 = ((num5 != 0f) ? _ConcatPull(num5, _solverCaches.footPull[j]) : _solverCaches.footPull[j]);
					_solverCaches.fullArmPull[j] = num3;
					_solverCaches.limbLegPull[j] = num5;
					_solverCaches.armToElbowPull[j] = _GetBalancedPullLockFrom(_solverCaches.armPull[j], _solverCaches.elbowPull[j]);
					_solverCaches.armToWristPull[j] = _GetBalancedPullLockFrom(_solverCaches.armPull[j], _solverCaches.wristPull[j]);
					_solverCaches.neckHeadToFullArmPull[j] = _GetBalancedPullLockTo(_solverCaches.neckHeadPull, num3);
					num += num3;
					num2 += num5;
				}
				_solverCaches.limbArmRate = _GetLerpRateFromPull2(_solverCaches.fullArmPull[0], _solverCaches.fullArmPull[1]);
				_solverCaches.limbLegRate = _GetLerpRateFromPull2(_solverCaches.limbLegPull[0], _solverCaches.limbLegPull[1]);
				_solverCaches.armToLegRate = _GetLerpRateFromPull2(num, num2);
				if (_spineBones != null)
				{
					int num6 = _spineBones.Length;
					float num7 = Mathf.Clamp01(_settings.bodyIK.spineDirXLegToArmRate);
					float num8 = Mathf.Max(_settings.bodyIK.spineDirXLegToArmToRate, num7);
					for (int k = 0; k != num6; k++)
					{
						if (k == 0)
						{
							_spineDirXRate[k] = num7;
						}
						else if (k + 1 == num6)
						{
							_spineDirXRate[k] = num8;
						}
						else
						{
							_spineDirXRate[k] = num7 + (num8 - num7) * ((float)k / (float)(num6 - 1));
						}
					}
					if (num6 > 0)
					{
						_spineEnabled[0] = _settings.bodyIK.upperSolveSpineEnabled;
					}
					if (num6 > 1)
					{
						_spineEnabled[1] = _settings.bodyIK.upperSolveSpine2Enabled;
					}
					if (num6 > 2)
					{
						_spineEnabled[2] = _settings.bodyIK.upperSolveSpine3Enabled;
					}
					if (num6 > 3)
					{
						_spineEnabled[3] = _settings.bodyIK.upperSolveSpine4Enabled;
					}
				}
				if (_solverInternal == null)
				{
					_solverInternal = new SolverInternal();
					_solverInternal.settings = _settings;
					_solverInternal.internalValues = _internalValues;
					_solverInternal._solverCaches = _solverCaches;
					_solverInternal.arms._bendingPull = _solverCaches.armToElbowPull;
					_solverInternal.arms._endPull = _solverCaches.armToWristPull;
					_solverInternal.arms._beginToBendingLength = _elbowEffectorMaxLength;
					_solverInternal.arms._beginToEndLength = _wristEffectorMaxLength;
					_solverInternal.legs._bendingPull = _solverCaches.kneePull;
					_solverInternal.legs._endPull = _solverCaches.footPull;
					_solverInternal.legs._beginToBendingLength = _kneeEffectorMaxLength;
					_solverInternal.legs._beginToEndLength = _footEffectorMaxLength;
					_solverInternal._shouderLocalAxisYInv = _shouderLocalAxisYInv;
					_solverInternal._armEffectors = _armEffectors;
					_solverInternal._wristEffectors = _wristEffectors;
					_solverInternal._neckEffector = _neckEffector;
					_solverInternal._headEffector = _headEffector;
					_solverInternal._spineBones = _spineBones;
					_solverInternal._shoulderBones = _shoulderBones;
					_solverInternal._armBones = _armBones;
					_solverInternal._limbIK = _limbIK;
					_solverInternal._centerLegBoneBasisInv = _centerLegBoneBasisInv;
					PrepareArray(ref _solverInternal.shoulderPos, _shoulderBones);
					PrepareArray(ref _solverInternal.spinePos, _spineBones);
					_solverInternal.nearArmPos = ((_shoulderBones != null) ? _solverInternal.shoulderPos : _solverInternal.armPos);
					if (_spineUBone != null && (_shoulderBones != null || _armBones != null))
					{
						Bone[] array = ((_shoulderBones != null) ? _shoulderBones : _armBones);
						Vector3 vector = array[1]._defaultPosition + array[0]._defaultPosition;
						Vector3 lhs = array[1]._defaultPosition - array[0]._defaultPosition;
						vector = vector * 0.5f - _spineUBone._defaultPosition;
						Vector3 v = Vector3.Cross(lhs, vector);
						lhs = Vector3.Cross(vector, v);
						if (SAFBIKVecNormalize3(ref lhs, ref vector, ref v))
						{
							Matrix3x3 matrix3x = Matrix3x3.FromColumn(ref lhs, ref vector, ref v);
							_solverInternal._spineUBoneLocalAxisBasisInv = matrix3x.transpose;
						}
					}
				}
				_solverInternal.headEnabled = _headBone != null && _solverCaches.headPull > 1E-07f;
				return true;
			}

			private void _PresolveHips()
			{
				SolverInternal solverInternal = _solverInternal;
				if (_hipsEffector == null)
				{
					return;
				}
				bool flag = _hipsEffector.rotationEnabled && _hipsEffector.rotationWeight > 1E-07f;
				bool flag2 = _hipsEffector.positionEnabled && _hipsEffector.pull > 1E-07f;
				if (!flag && !flag2)
				{
					return;
				}
				Matrix3x3 m = solverInternal.centerLegBasis;
				if (flag)
				{
					Quaternion quaternion = _hipsEffector.worldRotation * Inverse(_hipsEffector._defaultRotation);
					Quaternion q;
					SAFBIKMatGetRot(out q, ref m);
					Quaternion origRotation = quaternion * Inverse(q);
					if (_hipsEffector.rotationWeight < 0.9999999f)
					{
						origRotation = Quaternion.Lerp(Quaternion.identity, origRotation, _hipsEffector.rotationWeight);
					}
					solverInternal.LowerRotation(-1, ref origRotation, true);
					m = solverInternal.centerLegBasis;
				}
				if (flag2)
				{
					Vector3 addVec = _hipsEffector._hidden_worldPosition;
					Vector3 ret;
					SAFBIKMatMultVecPreSubAdd(out ret, ref m, ref _defaultCenterLegPos, ref _hipsEffector._defaultPosition, ref addVec);
					Vector3 origTranslate = ret - solverInternal.centerLegPos;
					if (_hipsEffector.pull < 0.9999999f)
					{
						origTranslate *= _hipsEffector.pull;
					}
					solverInternal.Translate(ref origTranslate);
				}
			}

			private void _ResetTransforms()
			{
				Matrix3x3 centerLegBasis = Matrix3x3.identity;
				Vector3 centerLegPos = Vector3.zero;
				_GetBaseCenterLegTransform(out centerLegPos, out centerLegBasis);
				_ResetCenterLegTransform(ref centerLegPos, ref centerLegBasis);
			}

			private void _GetBaseCenterLegTransform(out Vector3 centerLegPos, out Matrix3x3 centerLegBasis)
			{
				centerLegBasis = _internalValues.baseHipsBasis;
				if (_hipsEffector != null)
				{
					SAFBIKMatMultVecPreSubAdd(out centerLegPos, ref _internalValues.baseHipsBasis, ref _defaultCenterLegPos, ref _hipsEffector._defaultPosition, ref _internalValues.baseHipsPos);
				}
				else
				{
					centerLegPos = default(Vector3);
				}
			}

			private void _ResetCenterLegTransform(ref Vector3 centerLegPos, ref Matrix3x3 centerLegBasis)
			{
				SolverInternal solverInternal = _solverInternal;
				Vector3 subVec = _defaultCenterLegPos;
				if (_legBones != null)
				{
					for (int i = 0; i != 2; i++)
					{
						SAFBIKMatMultVecPreSubAdd(out solverInternal.legPos[i], ref centerLegBasis, ref _legBones[i]._defaultPosition, ref subVec, ref centerLegPos);
					}
				}
				if (_spineBones != null)
				{
					for (int j = 0; j != _spineBones.Length; j++)
					{
						SAFBIKMatMultVecPreSubAdd(out solverInternal.spinePos[j], ref centerLegBasis, ref _spineBones[j]._defaultPosition, ref subVec, ref centerLegPos);
					}
				}
				if (_shoulderBones != null)
				{
					for (int k = 0; k != 2; k++)
					{
						SAFBIKMatMultVecPreSubAdd(out solverInternal.shoulderPos[k], ref centerLegBasis, ref _shoulderBones[k]._defaultPosition, ref subVec, ref centerLegPos);
					}
				}
				if (_armBones != null)
				{
					for (int l = 0; l != 2; l++)
					{
						SAFBIKMatMultVecPreSubAdd(out solverInternal.armPos[l], ref centerLegBasis, ref _armBones[l]._defaultPosition, ref subVec, ref centerLegPos);
					}
				}
				if (_neckBone != null)
				{
					SAFBIKMatMultVecPreSubAdd(out solverInternal.neckPos, ref centerLegBasis, ref _neckBone._defaultPosition, ref subVec, ref centerLegPos);
				}
				if (_headBone != null && solverInternal.headEnabled)
				{
					SAFBIKMatMultVecPreSubAdd(out solverInternal.headPos, ref centerLegBasis, ref _headBone._defaultPosition, ref subVec, ref centerLegPos);
				}
				solverInternal.SetDirtyVariables();
				solverInternal._SetCenterLegPos(ref centerLegPos);
			}

			private void _ResetShoulderTransform()
			{
				SolverInternal solverInternal = _solverInternal;
				if (_armBones == null || _shoulderBones == null)
				{
					return;
				}
				if (_spineUBone != null && _spineUBone.transformIsAlive && _neckBone != null)
				{
					bool transformIsAlive = _neckBone.transformIsAlive;
				}
				if (!_limbIK[2].IsSolverEnabled() && !_limbIK[3].IsSolverEnabled())
				{
					return;
				}
				Vector3 v = solverInternal.neckPos - solverInternal.spineUPos;
				Vector3 dirX = solverInternal.nearArmPos[1] - solverInternal.nearArmPos[0];
				Matrix3x3 lhs;
				if (!SAFBIKVecNormalize(ref v) || !SAFBIKComputeBasisFromXYLockY(out lhs, ref dirX, ref v))
				{
					return;
				}
				Matrix3x3 ret;
				SAFBIKMatMult(out ret, ref lhs, ref _spineUBone._localAxisBasisInv);
				Vector3 addVec = solverInternal.spineUPos;
				int num = 0;
				while (true)
				{
					int num2;
					switch (num)
					{
					default:
						num2 = 3;
						break;
					case 0:
						num2 = 2;
						break;
					case 2:
						return;
					}
					int num3 = num2;
					if (_limbIK[num3].IsSolverEnabled())
					{
						SAFBIKMatMultVecPreSubAdd(out solverInternal.armPos[num], ref ret, ref _armBones[num]._defaultPosition, ref _spineUBone._defaultPosition, ref addVec);
					}
					num++;
				}
			}

			private static bool _ComputeCenterLegBasis(out Matrix3x3 centerLegBasis, ref Vector3 spinePos, ref Vector3 leftLegPos, ref Vector3 rightLegPos)
			{
				Vector3 dirX = rightLegPos - leftLegPos;
				Vector3 v = spinePos - (rightLegPos + leftLegPos) * 0.5f;
				if (SAFBIKVecNormalize(ref v))
				{
					return SAFBIKComputeBasisFromXYLockY(out centerLegBasis, ref dirX, ref v);
				}
				centerLegBasis = Matrix3x3.identity;
				return false;
			}

			private static bool _KeepMaxLength(ref Vector3 posTo, ref Vector3 posFrom, float keepLength)
			{
				Vector3 v = posTo - posFrom;
				float num = SAFBIKVecLength(ref v);
				if (num > 1E-07f && num > keepLength)
				{
					v *= keepLength / num;
					posTo = posFrom + v;
					return true;
				}
				return false;
			}

			private static bool _KeepMaxLength(ref Vector3 posTo, ref Vector3 posFrom, ref FastLength keepLength)
			{
				Vector3 v = posTo - posFrom;
				float num = SAFBIKVecLength(ref v);
				if (num > 1E-07f && num > keepLength.length)
				{
					v *= keepLength.length / num;
					posTo = posFrom + v;
					return true;
				}
				return false;
			}

			private static bool _KeepLength(ref Vector3 posTo, ref Vector3 posFrom, float keepLength)
			{
				Vector3 v = posTo - posFrom;
				float num = SAFBIKVecLength(ref v);
				if (num > 1E-07f)
				{
					v *= keepLength / num;
					posTo = posFrom + v;
					return true;
				}
				return false;
			}

			private static Quaternion _GetRotation(ref Vector3 axisDir, float theta, float rate)
			{
				if ((theta >= -1E-07f && theta <= 1E-07f) || (rate >= -1E-07f && rate <= 1E-07f))
				{
					return Quaternion.identity;
				}
				return Quaternion.AngleAxis(SAFBIKAcos(theta) * rate * 57.29578f, axisDir);
			}

			private static float _ConcatPull(float pull, float effectorPull)
			{
				if (pull >= 0.9999999f)
				{
					return 1f;
				}
				if (pull <= 1E-07f)
				{
					return effectorPull;
				}
				if (effectorPull > 1E-07f)
				{
					if (effectorPull >= 0.9999999f)
					{
						return 1f;
					}
					return pull + (1f - pull) * effectorPull;
				}
				return pull;
			}

			private static float _GetBalancedPullLockTo(float pullFrom, float pullTo)
			{
				if (pullTo <= 1E-07f)
				{
					return 1f - pullFrom;
				}
				if (pullFrom <= 1E-07f)
				{
					return 1f;
				}
				return pullTo / (pullFrom + pullTo);
			}

			private static float _GetBalancedPullLockFrom(float pullFrom, float pullTo)
			{
				if (pullFrom <= 1E-07f)
				{
					return pullTo;
				}
				if (pullTo <= 1E-07f)
				{
					return 0f;
				}
				return pullTo / (pullFrom + pullTo);
			}

			private static float _GetLerpRateFromPull2(float pull0, float pull1)
			{
				if (pull0 > float.Epsilon && pull1 > float.Epsilon)
				{
					return Mathf.Clamp01(pull1 / (pull0 + pull1));
				}
				if (pull0 > float.Epsilon)
				{
					return 0f;
				}
				if (pull1 > float.Epsilon)
				{
					return 1f;
				}
				return 0.5f;
			}
		}

		public enum _LocalAxisFrom
		{
			None = 0,
			Parent = 1,
			Child = 2,
			Max = 3,
			Unknown = 3
		}

		[Serializable]
		public class Bone
		{
			public Transform transform;

			[SerializeField]
			private bool _isPresetted;

			[SerializeField]
			private BoneLocation _boneLocation = BoneLocation.Max;

			[SerializeField]
			private BoneType _boneType = BoneType.Max;

			[SerializeField]
			private Side _boneSide = Side.Max;

			[SerializeField]
			private FingerType _fingerType = FingerType.Max;

			[SerializeField]
			private int _fingerIndex = -1;

			[SerializeField]
			private _LocalAxisFrom _localAxisFrom = _LocalAxisFrom.Max;

			[SerializeField]
			private _DirectionAs _localDirectionAs = _DirectionAs.Max;

			private Bone _parentBone;

			private Bone _parentBoneLocationBased;

			public Vector3 _defaultPosition = Vector3.zero;

			public Quaternion _defaultRotation = Quaternion.identity;

			public Matrix3x3 _defaultBasis = Matrix3x3.identity;

			public Vector3 _defaultLocalTranslate = Vector3.zero;

			public Vector3 _defaultLocalDirection = Vector3.zero;

			public FastLength _defaultLocalLength;

			public Matrix3x3 _localAxisBasis = Matrix3x3.identity;

			public Matrix3x3 _localAxisBasisInv = Matrix3x3.identity;

			public Quaternion _localAxisRotation = Quaternion.identity;

			public Quaternion _localAxisRotationInv = Quaternion.identity;

			public Matrix3x3 _worldToBoneBasis = Matrix3x3.identity;

			public Matrix3x3 _boneToWorldBasis = Matrix3x3.identity;

			public Matrix3x3 _worldToBaseBasis = Matrix3x3.identity;

			public Matrix3x3 _baseToWorldBasis = Matrix3x3.identity;

			public Quaternion _worldToBoneRotation = Quaternion.identity;

			public Quaternion _boneToWorldRotation = Quaternion.identity;

			public Quaternion _worldToBaseRotation = Quaternion.identity;

			public Quaternion _baseToWorldRotation = Quaternion.identity;

			public Matrix3x3 _baseToBoneBasis = Matrix3x3.identity;

			public Matrix3x3 _boneToBaseBasis = Matrix3x3.identity;

			[SerializeField]
			private bool _isWritebackWorldPosition;

			[NonSerialized]
			public Vector3 _worldPosition = Vector3.zero;

			[NonSerialized]
			public Quaternion _worldRotation = Quaternion.identity;

			private bool _isReadWorldPosition;

			private bool _isReadWorldRotation;

			private bool _isWrittenWorldPosition;

			private bool _isWrittenWorldRotation;

			private int _transformIsAlive = -1;

			public BoneLocation boneLocation
			{
				get
				{
					return _boneLocation;
				}
			}

			public BoneType boneType
			{
				get
				{
					return _boneType;
				}
			}

			public Side boneSide
			{
				get
				{
					return _boneSide;
				}
			}

			public FingerType fingerType
			{
				get
				{
					return _fingerType;
				}
			}

			public int fingerIndex
			{
				get
				{
					return _fingerIndex;
				}
			}

			public _LocalAxisFrom localAxisFrom
			{
				get
				{
					return _localAxisFrom;
				}
			}

			public _DirectionAs localDirectionAs
			{
				get
				{
					return _localDirectionAs;
				}
			}

			public Bone parentBone
			{
				get
				{
					return _parentBone;
				}
			}

			public Bone parentBoneLocationBased
			{
				get
				{
					return _parentBoneLocationBased;
				}
			}

			public bool isWritebackWorldPosition
			{
				get
				{
					return _isWritebackWorldPosition;
				}
			}

			public string name
			{
				get
				{
					return _boneType.ToString();
				}
			}

			public bool transformIsAlive
			{
				get
				{
					if (_transformIsAlive == -1)
					{
						_transformIsAlive = (CheckAlive(ref transform) ? 1 : 0);
					}
					return _transformIsAlive != 0;
				}
			}

			public Transform parentTransform
			{
				get
				{
					if (_parentBone != null)
					{
						return _parentBone.transform;
					}
					return null;
				}
			}

			public Vector3 worldPosition
			{
				get
				{
					if (!_isReadWorldPosition && !_isWrittenWorldPosition)
					{
						_isReadWorldPosition = true;
						if (transformIsAlive)
						{
							_worldPosition = transform.position;
						}
					}
					return _worldPosition;
				}
				set
				{
					_isWrittenWorldPosition = true;
					_worldPosition = value;
				}
			}

			public Quaternion worldRotation
			{
				get
				{
					if (!_isReadWorldRotation && !_isWrittenWorldRotation)
					{
						_isReadWorldRotation = true;
						if (transformIsAlive)
						{
							_worldRotation = transform.rotation;
						}
					}
					return _worldRotation;
				}
				set
				{
					_isWrittenWorldRotation = true;
					_worldRotation = value;
				}
			}

			public static Bone Preset(BoneLocation boneLocation)
			{
				Bone bone = new Bone();
				bone._PresetBoneLocation(boneLocation);
				return bone;
			}

			private void _PresetBoneLocation(BoneLocation boneLocation)
			{
				_isPresetted = true;
				_boneLocation = boneLocation;
				_boneType = ToBoneType(boneLocation);
				_boneSide = ToBoneSide(boneLocation);
				if (_boneType == BoneType.HandFinger)
				{
					_fingerType = ToFingerType(boneLocation);
					_fingerIndex = ToFingerIndex(boneLocation);
				}
				else
				{
					_fingerType = FingerType.Max;
					_fingerIndex = -1;
				}
				_PresetLocalAxis();
			}

			private void _PresetLocalAxis()
			{
				switch (_boneType)
				{
				case BoneType.Hips:
					_PresetLocalAxis(_LocalAxisFrom.Child, _DirectionAs.YPlus);
					return;
				case BoneType.Spine:
					_PresetLocalAxis(_LocalAxisFrom.Child, _DirectionAs.YPlus);
					return;
				case BoneType.Neck:
					_PresetLocalAxis(_LocalAxisFrom.Child, _DirectionAs.YPlus);
					return;
				case BoneType.Head:
					_PresetLocalAxis(_LocalAxisFrom.None, _DirectionAs.None);
					return;
				case BoneType.Eye:
					_PresetLocalAxis(_LocalAxisFrom.None, _DirectionAs.None);
					return;
				case BoneType.Leg:
					_PresetLocalAxis(_LocalAxisFrom.Child, _DirectionAs.YMinus);
					return;
				case BoneType.Knee:
					_PresetLocalAxis(_LocalAxisFrom.Child, _DirectionAs.YMinus);
					return;
				case BoneType.Foot:
					_PresetLocalAxis(_LocalAxisFrom.Parent, _DirectionAs.YMinus);
					return;
				case BoneType.Shoulder:
					_PresetLocalAxis(_LocalAxisFrom.Child, (_boneSide != 0) ? _DirectionAs.XPlus : _DirectionAs.XMinus);
					return;
				case BoneType.Arm:
					_PresetLocalAxis(_LocalAxisFrom.Child, (_boneSide != 0) ? _DirectionAs.XPlus : _DirectionAs.XMinus);
					return;
				case BoneType.ArmRoll:
					_PresetLocalAxis(_LocalAxisFrom.Parent, (_boneSide != 0) ? _DirectionAs.XPlus : _DirectionAs.XMinus);
					return;
				case BoneType.Elbow:
					_PresetLocalAxis(_LocalAxisFrom.Child, (_boneSide != 0) ? _DirectionAs.XPlus : _DirectionAs.XMinus);
					return;
				case BoneType.ElbowRoll:
					_PresetLocalAxis(_LocalAxisFrom.Parent, (_boneSide != 0) ? _DirectionAs.XPlus : _DirectionAs.XMinus);
					return;
				case BoneType.Wrist:
					_PresetLocalAxis(_LocalAxisFrom.Parent, (_boneSide != 0) ? _DirectionAs.XPlus : _DirectionAs.XMinus);
					return;
				}
				if (_boneType == BoneType.HandFinger)
				{
					_LocalAxisFrom localAxisFrom = ((_fingerIndex + 1 == 4) ? _LocalAxisFrom.Parent : _LocalAxisFrom.Child);
					_PresetLocalAxis(localAxisFrom, (_boneSide != 0) ? _DirectionAs.XPlus : _DirectionAs.XMinus);
				}
			}

			private void _PresetLocalAxis(_LocalAxisFrom localAxisFrom, _DirectionAs localDirectionAs)
			{
				_localAxisFrom = localAxisFrom;
				_localDirectionAs = localDirectionAs;
			}

			public static void Prefix(Bone[] bones, ref Bone bone, BoneLocation boneLocation, Bone parentBoneLocationBased = null)
			{
				if (bone == null)
				{
					bone = new Bone();
				}
				if (!bone._isPresetted || bone._boneLocation != boneLocation || bone._boneType < BoneType.Hips || bone._boneType >= BoneType.Max || bone._localAxisFrom == _LocalAxisFrom.Max || bone._localDirectionAs == _DirectionAs.Max)
				{
					bone._PresetBoneLocation(boneLocation);
				}
				bone._parentBoneLocationBased = parentBoneLocationBased;
				if (bones != null)
				{
					bones[(int)boneLocation] = bone;
				}
			}

			public void Prepare(FullBodyIK fullBodyIK)
			{
				_transformIsAlive = -1;
				_localAxisBasis = Matrix3x3.identity;
				_isWritebackWorldPosition = false;
				_parentBone = null;
				if (transformIsAlive)
				{
					for (Bone bone = _parentBoneLocationBased; bone != null; bone = bone._parentBoneLocationBased)
					{
						if (bone.transformIsAlive)
						{
							_parentBone = bone;
							break;
						}
					}
				}
				if (_boneLocation == BoneLocation.Hips)
				{
					if (transformIsAlive)
					{
						_isWritebackWorldPosition = true;
					}
				}
				else if (_boneLocation == BoneLocation.Spine && transformIsAlive && _parentBone != null && _parentBone.transformIsAlive && IsParentOfRecusively(_parentBone.transform, transform))
				{
					_isWritebackWorldPosition = true;
				}
				if (_boneType == BoneType.Eye && fullBodyIK._IsHiddenCustomEyes())
				{
					_isWritebackWorldPosition = true;
				}
				if (transformIsAlive)
				{
					_defaultPosition = transform.position;
					_defaultRotation = transform.rotation;
					SAFBIKMatSetRot(out _defaultBasis, ref _defaultRotation);
					if (_parentBone != null)
					{
						_defaultLocalTranslate = _defaultPosition - _parentBone._defaultPosition;
						_defaultLocalLength = FastLength.FromVector3(ref _defaultLocalTranslate);
						if (_defaultLocalLength.length > float.Epsilon)
						{
							float num = 1f / _defaultLocalLength.length;
							_defaultLocalDirection.x = _defaultLocalTranslate.x * num;
							_defaultLocalDirection.y = _defaultLocalTranslate.y * num;
							_defaultLocalDirection.z = _defaultLocalTranslate.z * num;
						}
					}
					SAFBIKMatMultInv0(out _worldToBaseBasis, ref _defaultBasis, ref fullBodyIK.internalValues.defaultRootBasis);
					_baseToWorldBasis = _worldToBaseBasis.transpose;
					SAFBIKMatGetRot(out _worldToBaseRotation, ref _worldToBaseBasis);
					_baseToWorldRotation = Inverse(_worldToBaseRotation);
				}
				else
				{
					_defaultPosition = Vector3.zero;
					_defaultRotation = Quaternion.identity;
					_defaultBasis = Matrix3x3.identity;
					_defaultLocalTranslate = Vector3.zero;
					_defaultLocalLength = default(FastLength);
					_defaultLocalDirection = Vector3.zero;
					_worldToBaseBasis = Matrix3x3.identity;
					_baseToWorldBasis = Matrix3x3.identity;
					_worldToBaseRotation = Quaternion.identity;
					_baseToWorldRotation = Quaternion.identity;
				}
				_ComputeLocalAxis(fullBodyIK);
			}

			private void _ComputeLocalAxis(FullBodyIK fullBodyIK)
			{
				if (!transformIsAlive || _parentBone == null || !_parentBone.transformIsAlive || (_localAxisFrom != _LocalAxisFrom.Parent && _parentBone._localAxisFrom != _LocalAxisFrom.Child))
				{
					return;
				}
				Vector3 dir = _defaultLocalDirection;
				if (dir.x == 0f && dir.y == 0f && dir.z == 0f)
				{
					return;
				}
				if (_localAxisFrom == _LocalAxisFrom.Parent)
				{
					SAFBIKComputeBasisFrom(out _localAxisBasis, ref fullBodyIK.internalValues.defaultRootBasis, ref dir, _localDirectionAs);
				}
				if (_parentBone._localAxisFrom != _LocalAxisFrom.Child)
				{
					return;
				}
				if (_parentBone._boneType == BoneType.Shoulder)
				{
					Bone bone = _parentBone;
					Bone bone2 = _parentBone._parentBone;
					Bone bone3 = ((fullBodyIK.headBones != null) ? fullBodyIK.headBones.neck : null);
					if (bone3 != null && !bone3.transformIsAlive)
					{
						bone3 = null;
					}
					if (fullBodyIK.internalValues.shoulderDirYAsNeck == -1)
					{
						if (fullBodyIK.settings.shoulderDirYAsNeck == AutomaticBool.Auto)
						{
							if (bone2 != null && bone3 != null)
							{
								Vector3 defaultLocalDirection = bone._defaultLocalDirection;
								Vector3 v = bone3._defaultPosition - bone._defaultPosition;
								if (SAFBIKVecNormalize(ref v))
								{
									float num = Mathf.Abs(Vector3.Dot(dir, defaultLocalDirection));
									float num2 = Mathf.Abs(Vector3.Dot(dir, v));
									if (num < num2)
									{
										fullBodyIK.internalValues.shoulderDirYAsNeck = 0;
									}
									else
									{
										fullBodyIK.internalValues.shoulderDirYAsNeck = 1;
									}
								}
								else
								{
									fullBodyIK.internalValues.shoulderDirYAsNeck = 0;
								}
							}
							else
							{
								fullBodyIK.internalValues.shoulderDirYAsNeck = 0;
							}
						}
						else
						{
							fullBodyIK.internalValues.shoulderDirYAsNeck = ((fullBodyIK.settings.shoulderDirYAsNeck != 0) ? 1 : 0);
						}
					}
					Vector3 c = ((_parentBone._localDirectionAs == _DirectionAs.XMinus) ? (-dir) : dir);
					Vector3 v2 = Vector3.Cross(rhs: (fullBodyIK.internalValues.shoulderDirYAsNeck == 0 || bone3 == null) ? bone._defaultLocalDirection : (bone3._defaultPosition - bone._defaultPosition), lhs: c);
					Vector3 v3 = Vector3.Cross(v2, c);
					if (SAFBIKVecNormalize2(ref v3, ref v2))
					{
						_parentBone._localAxisBasis.SetColumn(ref c, ref v3, ref v2);
					}
				}
				else
				{
					if ((_parentBone._boneType == BoneType.Spine && _boneType != BoneType.Spine && _boneType != BoneType.Neck) || (_parentBone._boneType == BoneType.Hips && _boneType != BoneType.Spine))
					{
						return;
					}
					if (_parentBone._boneType == BoneType.Hips)
					{
						Vector3 dirX = fullBodyIK.internalValues.defaultRootBasis.column0;
						SAFBIKComputeBasisFromXYLockY(out _parentBone._localAxisBasis, ref dirX, ref dir);
					}
					else if (_parentBone._boneType == BoneType.Spine || _parentBone._boneType == BoneType.Neck)
					{
						if (_parentBone._parentBone != null)
						{
							Vector3 dirX2 = _parentBone._parentBone._localAxisBasis.column0;
							SAFBIKComputeBasisFromXYLockY(out _parentBone._localAxisBasis, ref dirX2, ref dir);
						}
					}
					else if (_localAxisFrom == _LocalAxisFrom.Parent && _localDirectionAs == _parentBone._localDirectionAs)
					{
						_parentBone._localAxisBasis = _localAxisBasis;
					}
					else
					{
						SAFBIKComputeBasisFrom(out _parentBone._localAxisBasis, ref fullBodyIK.internalValues.defaultRootBasis, ref dir, _parentBone._localDirectionAs);
					}
				}
			}

			public void PostPrepare()
			{
				if (_localAxisFrom != 0)
				{
					_localAxisBasisInv = _localAxisBasis.transpose;
					SAFBIKMatGetRot(out _localAxisRotation, ref _localAxisBasis);
					_localAxisRotationInv = Inverse(_localAxisRotation);
					SAFBIKMatMultInv0(out _worldToBoneBasis, ref _defaultBasis, ref _localAxisBasis);
					_boneToWorldBasis = _worldToBoneBasis.transpose;
					SAFBIKMatGetRot(out _worldToBoneRotation, ref _worldToBoneBasis);
					_boneToWorldRotation = Inverse(_worldToBoneRotation);
				}
				else
				{
					_localAxisBasis = Matrix3x3.identity;
					_localAxisBasisInv = Matrix3x3.identity;
					_localAxisRotation = Quaternion.identity;
					_localAxisRotationInv = Quaternion.identity;
					_worldToBoneBasis = _defaultBasis.transpose;
					_boneToWorldBasis = _defaultBasis;
					_worldToBoneRotation = Inverse(_defaultRotation);
					_boneToWorldRotation = _defaultRotation;
				}
				SAFBIKMatMultInv0(out _baseToBoneBasis, ref _worldToBaseBasis, ref _worldToBoneBasis);
				_boneToBaseBasis = _baseToBoneBasis.transpose;
			}

			public void PrepareUpdate()
			{
				_transformIsAlive = -1;
				_isReadWorldPosition = false;
				_isReadWorldRotation = false;
				_isWrittenWorldPosition = false;
				_isWrittenWorldRotation = false;
			}

			public void SyncDisplacement()
			{
				if (_parentBone == null || !_parentBone.transformIsAlive || !transformIsAlive)
				{
					return;
				}
				Vector3 v = worldPosition - _parentBone.worldPosition;
				_defaultLocalLength = FastLength.FromVector3(ref v);
				if (_parentBone.transform == transform.parent)
				{
					Vector3 v2 = transform.localPosition;
					if (SAFBIKVecNormalize(ref v2))
					{
						Vector3 ret;
						SAFBIKMatMultVec(out ret, ref _parentBone._defaultBasis, ref v2);
						_defaultLocalDirection = ret;
						_defaultLocalTranslate = ret * _defaultLocalLength.length;
					}
					else
					{
						_defaultLocalDirection = Vector3.zero;
						_defaultLocalTranslate = Vector3.zero;
					}
				}
				else
				{
					_defaultLocalTranslate = _defaultLocalDirection * _defaultLocalLength.length;
				}
			}

			public void PostSyncDisplacement(FullBodyIK fullBodyIK)
			{
				if (_boneLocation == BoneLocation.Hips)
				{
					_defaultPosition = fullBodyIK.boneCaches.defaultHipsPosition + fullBodyIK.boneCaches.hipsOffset;
				}
				else if (_parentBone != null)
				{
					_defaultPosition = _parentBone._defaultPosition + _defaultLocalTranslate;
				}
				_ComputeLocalAxis(fullBodyIK);
			}

			public void forcefix_worldRotation()
			{
				if (transformIsAlive)
				{
					if (!_isReadWorldRotation)
					{
						_isReadWorldRotation = true;
						_worldRotation = transform.rotation;
					}
					_isWrittenWorldRotation = true;
					if (_parentBone != null && _parentBone.transformIsAlive)
					{
						Quaternion lhs = _parentBone.worldRotation;
						Matrix3x3 ret;
						SAFBIKMatSetRotMultInv1(out ret, ref lhs, ref parentBone._defaultRotation);
						Vector3 addVec = parentBone.worldPosition;
						Vector3 ret2;
						SAFBIKMatMultVecPreSubAdd(out ret2, ref ret, ref _defaultPosition, ref parentBone._defaultPosition, ref addVec);
						_isWrittenWorldPosition = true;
						_isWritebackWorldPosition = true;
						_worldPosition = ret2;
					}
				}
			}

			public void WriteToTransform()
			{
				if (_isWrittenWorldPosition)
				{
					_isWrittenWorldPosition = false;
					if (_isWritebackWorldPosition && transformIsAlive)
					{
						transform.position = _worldPosition;
					}
				}
				if (_isWrittenWorldRotation)
				{
					_isWrittenWorldRotation = false;
					if (transformIsAlive)
					{
						transform.rotation = _worldRotation;
					}
				}
			}
		}

		[Serializable]
		public class Effector
		{
			[Flags]
			private enum _EffectorFlags
			{
				None = 0,
				RotationContained = 1,
				PullContained = 2
			}

			public Transform transform;

			public bool positionEnabled;

			public bool rotationEnabled;

			public float positionWeight = 1f;

			public float rotationWeight = 1f;

			public float pull;

			[NonSerialized]
			public Vector3 _hidden_worldPosition = Vector3.zero;

			[SerializeField]
			private bool _isPresetted;

			[SerializeField]
			private EffectorLocation _effectorLocation = EffectorLocation.Max;

			[SerializeField]
			private EffectorType _effectorType = EffectorType.Max;

			[SerializeField]
			private _EffectorFlags _effectorFlags;

			private Effector _parentEffector;

			private Bone _bone;

			private Bone _leftBone;

			private Bone _rightBone;

			[SerializeField]
			private Transform _createdTransform;

			[SerializeField]
			public Vector3 _defaultPosition = Vector3.zero;

			[SerializeField]
			public Quaternion _defaultRotation = Quaternion.identity;

			public bool _isSimulateFingerTips;

			[NonSerialized]
			public Vector3 _worldPosition = Vector3.zero;

			[NonSerialized]
			public Quaternion _worldRotation = Quaternion.identity;

			private bool _isReadWorldPosition;

			private bool _isReadWorldRotation;

			private bool _isWrittenWorldPosition;

			private bool _isWrittenWorldRotation;

			private bool _isHiddenEyes;

			private int _transformIsAlive = -1;

			public bool effectorEnabled
			{
				get
				{
					if (!positionEnabled)
					{
						if (rotationContained)
						{
							return rotationContained;
						}
						return false;
					}
					return true;
				}
			}

			public bool rotationContained
			{
				get
				{
					return (_effectorFlags & _EffectorFlags.RotationContained) != 0;
				}
			}

			public bool pullContained
			{
				get
				{
					return (_effectorFlags & _EffectorFlags.PullContained) != 0;
				}
			}

			public EffectorLocation effectorLocation
			{
				get
				{
					return _effectorLocation;
				}
			}

			public EffectorType effectorType
			{
				get
				{
					return _effectorType;
				}
			}

			public Effector parentEffector
			{
				get
				{
					return _parentEffector;
				}
			}

			public Bone bone
			{
				get
				{
					return _bone;
				}
			}

			public Bone leftBone
			{
				get
				{
					return _leftBone;
				}
			}

			public Bone rightBone
			{
				get
				{
					return _rightBone;
				}
			}

			public Vector3 defaultPosition
			{
				get
				{
					return _defaultPosition;
				}
			}

			public Quaternion defaultRotation
			{
				get
				{
					return _defaultRotation;
				}
			}

			public string name
			{
				get
				{
					return GetEffectorName(_effectorLocation);
				}
			}

			public bool transformIsAlive
			{
				get
				{
					if (_transformIsAlive == -1)
					{
						_transformIsAlive = (CheckAlive(ref transform) ? 1 : 0);
					}
					return _transformIsAlive != 0;
				}
			}

			private bool _defaultLocalBasisIsIdentity
			{
				get
				{
					if ((_effectorFlags & _EffectorFlags.RotationContained) != 0 && _bone != null && _bone.localAxisFrom != 0 && _bone.boneType != 0)
					{
						return false;
					}
					return true;
				}
			}

			public Vector3 worldPosition
			{
				get
				{
					if (!_isReadWorldPosition && !_isWrittenWorldPosition)
					{
						_isReadWorldPosition = true;
						if (transformIsAlive)
						{
							_worldPosition = transform.position;
						}
					}
					return _worldPosition;
				}
				set
				{
					_isWrittenWorldPosition = true;
					_worldPosition = value;
				}
			}

			public Vector3 bone_worldPosition
			{
				get
				{
					if (_effectorType == EffectorType.Eyes)
					{
						if (!_isHiddenEyes && _bone != null && _bone.transformIsAlive && _leftBone != null && _leftBone.transformIsAlive && _rightBone != null && _rightBone.transformIsAlive)
						{
							return (_leftBone.worldPosition + _rightBone.worldPosition) * 0.5f;
						}
						if (_bone != null && _bone.transformIsAlive)
						{
							Vector3 result = _bone.worldPosition;
							if (_bone.parentBone != null && _bone.parentBone.transformIsAlive && _bone.parentBone.boneType == BoneType.Neck)
							{
								Vector3 vector = _bone._defaultPosition - _bone.parentBone._defaultPosition;
								float num = ((vector.y > 0f) ? vector.y : 0f);
								Quaternion q = _bone.parentBone.worldRotation * _bone.parentBone._worldToBaseRotation;
								Matrix3x3 m;
								SAFBIKMatSetRot(out m, ref q);
								result += m.column1 * num;
								result += m.column2 * num;
							}
							return result;
						}
					}
					else if (_isSimulateFingerTips)
					{
						if (_bone != null && _bone.parentBoneLocationBased != null && _bone.parentBoneLocationBased.transformIsAlive && _bone.parentBoneLocationBased.parentBoneLocationBased != null && _bone.parentBoneLocationBased.parentBoneLocationBased.transformIsAlive)
						{
							Vector3 vector2 = _bone.parentBoneLocationBased.worldPosition;
							Vector3 vector3 = _bone.parentBoneLocationBased.parentBoneLocationBased.worldPosition;
							return vector2 + (vector2 - vector3);
						}
					}
					else if (_bone != null && _bone.transformIsAlive)
					{
						return _bone.worldPosition;
					}
					return worldPosition;
				}
			}

			public Quaternion worldRotation
			{
				get
				{
					if (!_isReadWorldRotation && !_isWrittenWorldRotation)
					{
						_isReadWorldRotation = true;
						if (transformIsAlive)
						{
							_worldRotation = transform.rotation;
						}
					}
					return _worldRotation;
				}
				set
				{
					_isWrittenWorldRotation = true;
					_worldRotation = value;
				}
			}

			public void Prefix()
			{
				positionEnabled = _GetPresetPositionEnabled(_effectorType);
				positionWeight = _GetPresetPositionWeight(_effectorType);
				pull = _GetPresetPull(_effectorType);
			}

			private void _PresetEffectorLocation(EffectorLocation effectorLocation)
			{
				_isPresetted = true;
				_effectorLocation = effectorLocation;
				_effectorType = ToEffectorType(effectorLocation);
				_effectorFlags = _GetEffectorFlags(_effectorType);
			}

			public static void Prefix(Effector[] effectors, ref Effector effector, EffectorLocation effectorLocation, bool createEffectorTransform, Transform parentTransform, Effector parentEffector = null, Bone bone = null, Bone leftBone = null, Bone rightBone = null)
			{
				if (effector == null)
				{
					effector = new Effector();
				}
				if (!effector._isPresetted || effector._effectorLocation != effectorLocation || effector._effectorType < EffectorType.Root || effector._effectorType >= EffectorType.Max)
				{
					effector._PresetEffectorLocation(effectorLocation);
				}
				effector._parentEffector = parentEffector;
				effector._bone = bone;
				effector._leftBone = leftBone;
				effector._rightBone = rightBone;
				effector._PrefixTransform(createEffectorTransform, parentTransform);
				if (effectors != null)
				{
					effectors[(int)effectorLocation] = effector;
				}
			}

			private static bool _GetPresetPositionEnabled(EffectorType effectorType)
			{
				switch (effectorType)
				{
				case EffectorType.Wrist:
					return true;
				case EffectorType.Foot:
					return true;
				default:
					return false;
				}
			}

			private static float _GetPresetPositionWeight(EffectorType effectorType)
			{
				if (effectorType == EffectorType.Arm)
				{
					return 0f;
				}
				return 1f;
			}

			private static float _GetPresetPull(EffectorType effectorType)
			{
				switch (effectorType)
				{
				case EffectorType.Hips:
					return 1f;
				case EffectorType.Eyes:
					return 1f;
				case EffectorType.Arm:
					return 1f;
				case EffectorType.Wrist:
					return 1f;
				case EffectorType.Foot:
					return 1f;
				default:
					return 0f;
				}
			}

			private static _EffectorFlags _GetEffectorFlags(EffectorType effectorType)
			{
				switch (effectorType)
				{
				case EffectorType.Hips:
					return _EffectorFlags.RotationContained | _EffectorFlags.PullContained;
				case EffectorType.Neck:
					return _EffectorFlags.PullContained;
				case EffectorType.Head:
					return _EffectorFlags.RotationContained | _EffectorFlags.PullContained;
				case EffectorType.Eyes:
					return _EffectorFlags.PullContained;
				case EffectorType.Arm:
					return _EffectorFlags.PullContained;
				case EffectorType.Wrist:
					return _EffectorFlags.RotationContained | _EffectorFlags.PullContained;
				case EffectorType.Foot:
					return _EffectorFlags.RotationContained | _EffectorFlags.PullContained;
				case EffectorType.Elbow:
					return _EffectorFlags.PullContained;
				case EffectorType.Knee:
					return _EffectorFlags.PullContained;
				default:
					return _EffectorFlags.None;
				}
			}

			private void _PrefixTransform(bool createEffectorTransform, Transform parentTransform)
			{
				if (createEffectorTransform)
				{
					if (transform == null || transform != _createdTransform)
					{
						if (transform == null)
						{
							GameObject gameObject = new GameObject(GetEffectorName(_effectorLocation));
							if (parentTransform != null)
							{
								gameObject.transform.SetParent(parentTransform, false);
							}
							else if (_parentEffector != null && _parentEffector.transformIsAlive)
							{
								gameObject.transform.SetParent(_parentEffector.transform, false);
							}
							transform = gameObject.transform;
							_createdTransform = gameObject.transform;
						}
						else
						{
							DestroyImmediate(ref _createdTransform, true);
						}
					}
					else
					{
						CheckAlive(ref _createdTransform);
					}
				}
				else
				{
					if (_createdTransform != null)
					{
						if (transform == _createdTransform)
						{
							transform = null;
						}
						UnityEngine.Object.DestroyImmediate(_createdTransform.gameObject, true);
					}
					_createdTransform = null;
				}
				_transformIsAlive = (CheckAlive(ref transform) ? 1 : 0);
			}

			public void Prepare(FullBodyIK fullBodyIK)
			{
				_ClearInternal();
				_ComputeDefaultTransform(fullBodyIK);
				if (transformIsAlive)
				{
					if (_effectorType == EffectorType.Eyes)
					{
						transform.position = _defaultPosition + fullBodyIK.internalValues.defaultRootBasis.column2 * 1f;
					}
					else
					{
						transform.position = _defaultPosition;
					}
					if (!_defaultLocalBasisIsIdentity)
					{
						transform.rotation = _defaultRotation;
					}
					else
					{
						transform.localRotation = Quaternion.identity;
					}
					transform.localScale = Vector3.one;
				}
				_worldPosition = _defaultPosition;
				_worldRotation = _defaultRotation;
				if (_effectorType == EffectorType.Eyes)
				{
					_worldPosition += fullBodyIK.internalValues.defaultRootBasis.column2 * 1f;
				}
			}

			public void _ComputeDefaultTransform(FullBodyIK fullBodyIK)
			{
				if (_parentEffector != null)
				{
					_defaultRotation = _parentEffector._defaultRotation;
				}
				if (_effectorType == EffectorType.Root)
				{
					_defaultPosition = fullBodyIK.internalValues.defaultRootPosition;
					_defaultRotation = fullBodyIK.internalValues.defaultRootRotation;
				}
				else if (_effectorType == EffectorType.HandFinger)
				{
					if (_bone != null)
					{
						if (_bone.transformIsAlive)
						{
							_defaultPosition = bone._defaultPosition;
						}
						else if (_bone.parentBoneLocationBased != null && _bone.parentBoneLocationBased.parentBoneLocationBased != null)
						{
							Vector3 vector = bone.parentBoneLocationBased._defaultPosition - bone.parentBoneLocationBased.parentBoneLocationBased._defaultPosition;
							_defaultPosition = bone.parentBoneLocationBased._defaultPosition + vector;
							_isSimulateFingerTips = true;
						}
					}
				}
				else if (_effectorType == EffectorType.Eyes)
				{
					_isHiddenEyes = fullBodyIK._IsHiddenCustomEyes();
					if (!_isHiddenEyes && _bone != null && _bone.transformIsAlive && _leftBone != null && _leftBone.transformIsAlive && _rightBone != null && _rightBone.transformIsAlive)
					{
						_defaultPosition = (_leftBone._defaultPosition + _rightBone._defaultPosition) * 0.5f;
					}
					else if (_bone != null && _bone.transformIsAlive)
					{
						_defaultPosition = _bone._defaultPosition;
						if (_bone.parentBone != null && _bone.parentBone.transformIsAlive && _bone.parentBone.boneType == BoneType.Neck)
						{
							Vector3 vector2 = _bone._defaultPosition - _bone.parentBone._defaultPosition;
							float num = ((vector2.y > 0f) ? vector2.y : 0f);
							_defaultPosition += fullBodyIK.internalValues.defaultRootBasis.column1 * num;
							_defaultPosition += fullBodyIK.internalValues.defaultRootBasis.column2 * num;
						}
					}
				}
				else if (_effectorType == EffectorType.Hips)
				{
					if (_bone != null && _leftBone != null && _rightBone != null)
					{
						_defaultPosition = (_leftBone._defaultPosition + _rightBone._defaultPosition) * 0.5f;
					}
				}
				else if (_bone != null)
				{
					_defaultPosition = bone._defaultPosition;
					if (!_defaultLocalBasisIsIdentity)
					{
						_defaultRotation = bone._localAxisRotation;
					}
				}
			}

			private void _ClearInternal()
			{
				_transformIsAlive = -1;
				_defaultPosition = Vector3.zero;
				_defaultRotation = Quaternion.identity;
			}

			public void PrepareUpdate()
			{
				_transformIsAlive = -1;
				_isReadWorldPosition = false;
				_isReadWorldRotation = false;
				_isWrittenWorldPosition = false;
				_isWrittenWorldRotation = false;
			}

			public void WriteToTransform()
			{
				if (_isWrittenWorldPosition)
				{
					_isWrittenWorldPosition = false;
					if (transformIsAlive)
					{
						transform.position = _worldPosition;
					}
				}
				if (_isWrittenWorldRotation)
				{
					_isWrittenWorldRotation = false;
					if (transformIsAlive)
					{
						transform.rotation = _worldRotation;
					}
				}
			}
		}

		public class FingerIK
		{
			public class _FingerLink
			{
				public Bone bone;

				public Matrix3x3 boneToSolvedBasis = Matrix3x3.identity;

				public Matrix3x3 solvedToBoneBasis = Matrix3x3.identity;

				public Matrix3x4 boneTransform = Matrix3x4.identity;

				public float childToLength;

				public float childToLengthSq;
			}

			public struct _FingerIKParams
			{
				public float lengthD0;

				public float lengthABCDInv;

				public float beginLink_endCosTheta;
			}

			public class _FingerBranch
			{
				public Effector effector;

				public _FingerLink[] fingerLinks;

				public Matrix3x3 boneToSolvedBasis = Matrix3x3.identity;

				public Matrix3x3 solvedToBoneBasis = Matrix3x3.identity;

				public FastAngle notThumb1BaseAngle;

				public FastAngle notThumb2BaseAngle;

				public float link0ToEffectorLength;

				public float link0ToEffectorLengthSq;

				public _FingerIKParams fingerIKParams;
			}

			private class _ThumbLink
			{
				public Matrix3x3 thumb_boneToSolvedBasis = Matrix3x3.identity;

				public Matrix3x3 thumb_solvedToBoneBasis = Matrix3x3.identity;
			}

			private class _ThumbBranch
			{
				public _ThumbLink[] thumbLinks;

				public Vector3 thumbSolveY = Vector3.zero;

				public Vector3 thumbSolveZ = Vector3.zero;

				public bool thumb0_isLimited;

				public float thumb0_lowerLimit;

				public float thumb0_upperLimit;

				public float thumb0_innerLimit;

				public float thumb0_outerLimit;

				public float linkLength0to1Sq;

				public float linkLength0to1;

				public float linkLength1to3Sq;

				public float linkLength1to3;

				public float linkLength1to2Sq;

				public float linkLength1to2;

				public float linkLength2to3Sq;

				public float linkLength2to3;

				public float thumb1_baseThetaAtoB = 1f;

				public float thumb1_Acos_baseThetaAtoB;
			}

			private const float _positionLerpRate = 1.15f;

			private FingerIKType _fingerIKType;

			private Settings _settings;

			private InternalValues _internalValues;

			private Bone _parentBone;

			private _FingerBranch[] _fingerBranches = new _FingerBranch[5];

			private _ThumbBranch _thumbBranch;

			private FastAngle _notThumbYawThetaLimit = new FastAngle(0.17453292f);

			private FastAngle _notThumbPitchUThetaLimit = new FastAngle((float)Math.PI / 3f);

			private FastAngle _notThumbPitchLThetaLimit = new FastAngle(2.7925267f);

			private FastAngle _notThumb0FingerIKLimit = new FastAngle((float)Math.PI / 3f);

			private FastAngle _notThumb1PitchUTrace = new FastAngle(0.08726646f);

			private FastAngle _notThumb1PitchUSmooth = new FastAngle(0.08726646f);

			private FastAngle _notThumb1PitchUTraceSmooth = new FastAngle(0.17453292f);

			private FastAngle _notThumb1PitchLTrace = new FastAngle(0.17453292f);

			private FastAngle _notThumb1PitchLLimit = new FastAngle(1.3962634f);

			private bool _isSyncDisplacementAtLeastOnce;

			public FingerIK(FullBodyIK fullBodyIK, FingerIKType fingerIKType)
			{
				_fingerIKType = fingerIKType;
				_settings = fullBodyIK.settings;
				_internalValues = fullBodyIK.internalValues;
				FingersBones fingersBones = null;
				FingersEffectors fingersEffectors = null;
				switch (fingerIKType)
				{
				case FingerIKType.LeftWrist:
					_parentBone = fullBodyIK.leftArmBones.wrist;
					fingersBones = fullBodyIK.leftHandFingersBones;
					fingersEffectors = fullBodyIK.leftHandFingersEffectors;
					break;
				case FingerIKType.RightWrist:
					_parentBone = fullBodyIK.rightArmBones.wrist;
					fingersBones = fullBodyIK.rightHandFingersBones;
					fingersEffectors = fullBodyIK.rightHandFingersEffectors;
					break;
				}
				_notThumb1PitchUTraceSmooth = new FastAngle(_notThumb1PitchUTrace.angle + _notThumb1PitchUSmooth.angle);
				if (fingersBones == null || fingersEffectors == null)
				{
					return;
				}
				for (int i = 0; i < 5; i++)
				{
					Bone[] array = null;
					Effector effector = null;
					switch (i)
					{
					case 0:
						array = fingersBones.thumb;
						effector = fingersEffectors.thumb;
						break;
					case 1:
						array = fingersBones.index;
						effector = fingersEffectors.index;
						break;
					case 2:
						array = fingersBones.middle;
						effector = fingersEffectors.middle;
						break;
					case 3:
						array = fingersBones.ring;
						effector = fingersEffectors.ring;
						break;
					case 4:
						array = fingersBones.little;
						effector = fingersEffectors.little;
						break;
					}
					if (array != null && effector != null)
					{
						_PrepareBranch(i, array, effector);
					}
				}
			}

			private void _PrepareBranch(int fingerType, Bone[] bones, Effector effector)
			{
				if (_parentBone == null || bones == null || effector == null)
				{
					return;
				}
				int num = bones.Length;
				if (num == 0)
				{
					return;
				}
				if (effector.bone != null && bones[num - 1] == effector.bone)
				{
					num--;
					if (num == 0)
					{
						return;
					}
				}
				if (num != 0 && (bones[num - 1] == null || bones[num - 1].transform == null))
				{
					num--;
					if (num == 0)
					{
						return;
					}
				}
				_FingerBranch fingerBranch = new _FingerBranch();
				fingerBranch.effector = effector;
				fingerBranch.fingerLinks = new _FingerLink[num];
				for (int i = 0; i < num; i++)
				{
					if (bones[i] == null || bones[i].transform == null)
					{
						return;
					}
					_FingerLink fingerLink = new _FingerLink();
					fingerLink.bone = bones[i];
					fingerBranch.fingerLinks[i] = fingerLink;
				}
				_fingerBranches[fingerType] = fingerBranch;
				if (fingerType == 0)
				{
					_thumbBranch = new _ThumbBranch();
					_thumbBranch.thumbLinks = new _ThumbLink[num];
					for (int j = 0; j != num; j++)
					{
						_thumbBranch.thumbLinks[j] = new _ThumbLink();
					}
				}
			}

			private static bool _SolveThumbYZ(ref Matrix3x3 middleBoneToSolvedBasis, ref Vector3 thumbSolveY, ref Vector3 thumbSolveZ)
			{
				if (SAFBIKVecNormalize2(ref thumbSolveY, ref thumbSolveZ))
				{
					if (Mathf.Abs(thumbSolveY.z) > Mathf.Abs(thumbSolveZ.z))
					{
						Vector3 vector = thumbSolveY;
						thumbSolveY = thumbSolveZ;
						thumbSolveZ = vector;
					}
					if (thumbSolveY.y < 0f)
					{
						thumbSolveY = -thumbSolveY;
					}
					if (thumbSolveZ.z < 0f)
					{
						thumbSolveZ = -thumbSolveZ;
					}
					SAFBIKMatMultVec(out thumbSolveY, ref middleBoneToSolvedBasis, ref thumbSolveY);
					SAFBIKMatMultVec(out thumbSolveZ, ref middleBoneToSolvedBasis, ref thumbSolveZ);
					return true;
				}
				thumbSolveY = Vector3.zero;
				thumbSolveZ = Vector3.zero;
				return false;
			}

			private void _PrepareBranch2(int fingerType)
			{
				_FingerBranch fingerBranch = _fingerBranches[fingerType];
				if (_parentBone == null || fingerBranch == null)
				{
					return;
				}
				Effector effector = fingerBranch.effector;
				int num = fingerBranch.fingerLinks.Length;
				bool flag = _fingerIKType == FingerIKType.RightWrist;
				if (fingerBranch.fingerLinks != null && fingerBranch.fingerLinks.Length != 0 && fingerBranch.fingerLinks[0].bone != null)
				{
					Vector3 vector = effector.defaultPosition - fingerBranch.fingerLinks[0].bone._defaultPosition;
					vector = (flag ? vector : (-vector));
					if (SAFBIKVecNormalize(ref vector) && SAFBIKComputeBasisFromXZLockX(out fingerBranch.boneToSolvedBasis, vector, _internalValues.defaultRootBasis.column2))
					{
						fingerBranch.solvedToBoneBasis = fingerBranch.boneToSolvedBasis.transpose;
					}
					fingerBranch.link0ToEffectorLength = SAFBIKVecLengthAndLengthSq2(out fingerBranch.link0ToEffectorLengthSq, ref effector._defaultPosition, ref fingerBranch.fingerLinks[0].bone._defaultPosition);
				}
				if (fingerType == 0)
				{
					_FingerBranch fingerBranch2 = _fingerBranches[2];
					if (fingerBranch2 == null)
					{
						return;
					}
					if (fingerBranch2.fingerLinks.Length >= 1)
					{
						_FingerLink obj = fingerBranch2.fingerLinks[0];
						Matrix3x3 basis = Matrix3x3.identity;
						Matrix3x3 m = Matrix3x3.identity;
						Vector3 v = obj.bone._defaultPosition - _parentBone._defaultPosition;
						if (SAFBIKVecNormalize(ref v))
						{
							v = (flag ? v : (-v));
							if (SAFBIKComputeBasisFromXZLockX(out basis, v, _internalValues.defaultRootBasis.column2))
							{
								m = basis.transpose;
							}
						}
						bool flag2 = false;
						if (num >= 2 && !effector._isSimulateFingerTips)
						{
							_FingerLink fingerLink = fingerBranch.fingerLinks[num - 2];
							_FingerLink obj2 = fingerBranch.fingerLinks[num - 1];
							Vector3 defaultPosition = fingerLink.bone._defaultPosition;
							Vector3 defaultPosition2 = obj2.bone._defaultPosition;
							Vector3 defaultPosition3 = effector._defaultPosition;
							Vector3 ret = defaultPosition2 - defaultPosition;
							Vector3 ret2 = defaultPosition3 - defaultPosition2;
							SAFBIKMatMultVec(out ret, ref m, ref ret);
							SAFBIKMatMultVec(out ret2, ref m, ref ret2);
							Vector3 vector2 = Vector3.Cross(ret, ret2);
							_thumbBranch.thumbSolveY = vector2;
							_thumbBranch.thumbSolveZ = Vector3.Cross(ret2, vector2);
							flag2 = _SolveThumbYZ(ref basis, ref _thumbBranch.thumbSolveY, ref _thumbBranch.thumbSolveZ);
						}
						if (!flag2 && num >= 3)
						{
							_FingerLink fingerLink2 = fingerBranch.fingerLinks[num - 3];
							_FingerLink fingerLink3 = fingerBranch.fingerLinks[num - 2];
							_FingerLink obj3 = fingerBranch.fingerLinks[num - 1];
							Vector3 defaultPosition4 = fingerLink2.bone._defaultPosition;
							Vector3 defaultPosition5 = fingerLink3.bone._defaultPosition;
							Vector3 defaultPosition6 = obj3.bone._defaultPosition;
							Vector3 ret3 = defaultPosition5 - defaultPosition4;
							Vector3 ret4 = defaultPosition6 - defaultPosition5;
							SAFBIKMatMultVec(out ret3, ref m, ref ret3);
							SAFBIKMatMultVec(out ret4, ref m, ref ret4);
							Vector3 vector3 = Vector3.Cross(ret3, ret4);
							_thumbBranch.thumbSolveY = vector3;
							_thumbBranch.thumbSolveZ = Vector3.Cross(ret4, vector3);
							flag2 = _SolveThumbYZ(ref basis, ref _thumbBranch.thumbSolveY, ref _thumbBranch.thumbSolveZ);
						}
						if (!flag2)
						{
							_thumbBranch.thumbSolveZ = new Vector3(0f, 1f, 2f);
							_thumbBranch.thumbSolveY = new Vector3(0f, 2f, -1f);
							SAFBIKVecNormalize2(ref _thumbBranch.thumbSolveZ, ref _thumbBranch.thumbSolveY);
						}
					}
				}
				for (int i = 0; i != fingerBranch.fingerLinks.Length; i++)
				{
					_FingerLink fingerLink4 = fingerBranch.fingerLinks[i];
					Vector3 defaultPosition7 = fingerLink4.bone._defaultPosition;
					FastLength fastLength;
					Vector3 vector4;
					if (i + 1 != fingerBranch.fingerLinks.Length)
					{
						Vector3 defaultPosition8 = fingerBranch.fingerLinks[i + 1].bone._defaultPosition;
						fastLength = fingerBranch.fingerLinks[i + 1].bone._defaultLocalLength;
						vector4 = fingerBranch.fingerLinks[i + 1].bone._defaultLocalDirection;
					}
					else
					{
						Vector3 defaultPosition8 = fingerBranch.effector._defaultPosition;
						if (!fingerBranch.effector._isSimulateFingerTips)
						{
							fastLength = fingerBranch.effector.bone._defaultLocalLength;
							vector4 = fingerBranch.effector.bone._defaultLocalDirection;
						}
						else
						{
							Vector3 v2 = defaultPosition8 - defaultPosition7;
							fastLength = FastLength.FromVector3(ref v2);
							vector4 = ((!(fastLength.length > float.Epsilon)) ? Vector3.zero : (v2 * (1f / fastLength.length)));
						}
					}
					if (fingerType != 0)
					{
						fingerLink4.childToLength = fastLength.length;
						fingerLink4.childToLengthSq = fastLength.lengthSq;
					}
					Vector3 vector5 = vector4;
					if (vector5.x != 0f || vector5.y != 0f || vector5.z != 0f)
					{
						vector5 = (flag ? vector5 : (-vector5));
						if (SAFBIKComputeBasisFromXZLockX(out fingerLink4.boneToSolvedBasis, vector5, _internalValues.defaultRootBasis.column2))
						{
							fingerLink4.solvedToBoneBasis = fingerLink4.boneToSolvedBasis.transpose;
						}
					}
					if (fingerType != 0)
					{
						continue;
					}
					_ThumbLink thumbLink = _thumbBranch.thumbLinks[i];
					Vector3 v3 = fingerBranch.effector._defaultPosition - defaultPosition7;
					if (SAFBIKVecNormalize(ref v3))
					{
						v3 = (flag ? v3 : (-v3));
						if (SAFBIKComputeBasisFromXYLockX(out thumbLink.thumb_boneToSolvedBasis, ref v3, ref _thumbBranch.thumbSolveY))
						{
							thumbLink.thumb_solvedToBoneBasis = thumbLink.thumb_boneToSolvedBasis.transpose;
						}
					}
				}
				if (fingerType == 0 || fingerBranch.fingerLinks.Length != 3)
				{
					return;
				}
				fingerBranch.notThumb1BaseAngle = new FastAngle(_ComputeJointBaseAngle(ref _internalValues.defaultRootBasis, ref fingerBranch.fingerLinks[0].bone._defaultPosition, ref fingerBranch.fingerLinks[1].bone._defaultPosition, ref fingerBranch.effector._defaultPosition, flag));
				fingerBranch.notThumb2BaseAngle = new FastAngle(_ComputeJointBaseAngle(ref _internalValues.defaultRootBasis, ref fingerBranch.fingerLinks[1].bone._defaultPosition, ref fingerBranch.fingerLinks[2].bone._defaultPosition, ref fingerBranch.effector._defaultPosition, flag));
				float childToLength = fingerBranch.fingerLinks[0].childToLength;
				float childToLength2 = fingerBranch.fingerLinks[1].childToLength;
				float childToLength3 = fingerBranch.fingerLinks[2].childToLength;
				float num2 = Mathf.Abs(childToLength - childToLength3);
				float num3 = SAFBIKSqrt(childToLength2 * childToLength2 - num2 * num2);
				float beginLink_endCosTheta = 0f;
				if (childToLength > childToLength3)
				{
					float cos = _notThumb0FingerIKLimit.cos;
					float sin = _notThumb0FingerIKLimit.sin;
					if (num3 * (1f / childToLength2) < cos)
					{
						num3 = cos * childToLength2;
						num2 = sin * childToLength2;
						float num4 = Mathf.Clamp01((childToLength3 + num2) * (1f / childToLength));
						beginLink_endCosTheta = SAFBIKSqrtClamp01(1f - num4 * num4);
					}
				}
				float num5 = childToLength2 - num3;
				float num6 = childToLength + childToLength3 + num5;
				num6 = ((num6 > 1E-07f) ? (1f / num6) : 0f);
				fingerBranch.fingerIKParams.lengthD0 = num3;
				fingerBranch.fingerIKParams.lengthABCDInv = num6;
				fingerBranch.fingerIKParams.beginLink_endCosTheta = beginLink_endCosTheta;
			}

			private void _PrepareThumb()
			{
				_FingerBranch fingerBranch = _fingerBranches[0];
				_FingerBranch fingerBranch2 = _fingerBranches[1];
				if (fingerBranch != null && fingerBranch.fingerLinks.Length == 3 && fingerBranch2 != null && fingerBranch2.fingerLinks.Length != 0)
				{
					_FingerLink fingerLink = fingerBranch.fingerLinks[0];
					_FingerLink fingerLink2 = fingerBranch.fingerLinks[1];
					_FingerLink fingerLink3 = fingerBranch.fingerLinks[2];
					Vector3 v = fingerBranch2.fingerLinks[0].bone._defaultPosition - fingerLink.bone._defaultPosition;
					Vector3 ret;
					SAFBIKMatMultVec(out ret, ref fingerBranch.solvedToBoneBasis, ref v);
					if (SAFBIKVecNormalize(ref ret))
					{
						_thumbBranch.thumb0_isLimited = true;
						_thumbBranch.thumb0_innerLimit = Mathf.Max(0f - ret.z, 0f);
						_thumbBranch.thumb0_outerLimit = (float)Math.Sin(Mathf.Max(0f - (SAFBIKAsin(_thumbBranch.thumb0_innerLimit) - 0.6981317f), 0f));
						_thumbBranch.thumb0_upperLimit = Mathf.Max(ret.y, 0f);
						_thumbBranch.thumb0_lowerLimit = (float)Math.Sin(Mathf.Max(0f - (SAFBIKAsin(_thumbBranch.thumb0_upperLimit) - (float)Math.PI / 4f), 0f));
					}
					_thumbBranch.linkLength0to1 = SAFBIKVecLengthAndLengthSq2(out _thumbBranch.linkLength0to1Sq, ref fingerLink2.bone._defaultPosition, ref fingerLink.bone._defaultPosition);
					_thumbBranch.linkLength1to2 = SAFBIKVecLengthAndLengthSq2(out _thumbBranch.linkLength1to2Sq, ref fingerLink3.bone._defaultPosition, ref fingerLink2.bone._defaultPosition);
					_thumbBranch.linkLength2to3 = SAFBIKVecLengthAndLengthSq2(out _thumbBranch.linkLength2to3Sq, ref fingerBranch.effector._defaultPosition, ref fingerLink3.bone._defaultPosition);
					_thumbBranch.linkLength1to3 = SAFBIKVecLengthAndLengthSq2(out _thumbBranch.linkLength1to3Sq, ref fingerBranch.effector._defaultPosition, ref fingerLink2.bone._defaultPosition);
					_thumbBranch.thumb1_baseThetaAtoB = _ComputeTriangleTheta(_thumbBranch.linkLength1to2, _thumbBranch.linkLength1to3, _thumbBranch.linkLength2to3, _thumbBranch.linkLength1to2Sq, _thumbBranch.linkLength1to3Sq, _thumbBranch.linkLength2to3Sq);
					_thumbBranch.thumb1_Acos_baseThetaAtoB = SAFBIKAcos(_thumbBranch.thumb1_baseThetaAtoB);
				}
			}

			private void _SyncDisplacement()
			{
				if (_settings.syncDisplacement == SyncDisplacement.Everyframe || !_isSyncDisplacementAtLeastOnce)
				{
					_isSyncDisplacementAtLeastOnce = true;
					for (int i = 0; i != 5; i++)
					{
						_PrepareBranch2(i);
					}
					_PrepareThumb();
				}
			}

			private static float _ComputeJointBaseAngle(ref Matrix3x3 rootBaseBasis, ref Vector3 beginPosition, ref Vector3 nextPosition, ref Vector3 endPosition, bool isRight)
			{
				Vector3 v = endPosition - beginPosition;
				Vector3 v2 = nextPosition - beginPosition;
				if (SAFBIKVecNormalize2(ref v, ref v2))
				{
					Vector3 dirX = (isRight ? v : (-v));
					Matrix3x3 basis;
					SAFBIKComputeBasisFromXZLockX(out basis, dirX, rootBaseBasis.column2);
					dirX = (isRight ? v2 : (-v2));
					Vector3 dirZ = basis.column2;
					Matrix3x3 basis2;
					SAFBIKComputeBasisFromXZLockZ(out basis2, ref dirX, ref dirZ);
					float cos = Vector3.Dot(basis.column0, basis2.column0);
					float num = Vector3.Dot(basis.column1, basis2.column0);
					float num2 = SAFBIKAcos(cos);
					if (num < 0f)
					{
						num2 = 0f - num2;
					}
					return num2;
				}
				return 0f;
			}

			private static bool _SolveInDirect(bool isRight, ref Vector3 solvedDirY, ref Vector3 solvedDirZ, ref Matrix3x3 rootBasis, ref Matrix3x3 linkBoneToSolvedBasis, ref Vector3 effectorDirection)
			{
				Vector3 dirX = (isRight ? effectorDirection : (-effectorDirection));
				Vector3 ret;
				SAFBIKMatMultVec(out ret, ref rootBasis, ref linkBoneToSolvedBasis.column2);
				Matrix3x3 basis;
				if (!SAFBIKComputeBasisFromXZLockX(out basis, ref dirX, ref ret))
				{
					return false;
				}
				solvedDirY = basis.column1;
				solvedDirZ = basis.column2;
				return true;
			}

			private static float _ComputeTriangleTheta(float lenA, float lenB, float lenC, float lenASq, float lenBSq, float lenCSq)
			{
				float num = lenA * lenB;
				if (num >= 1E-07f)
				{
					return (lenASq + lenBSq - lenCSq) / (2f * num);
				}
				return 1f;
			}

			private static void _LerpEffectorLength(ref float effectorLength, ref Vector3 effectorDirection, ref Vector3 effectorTranslate, ref Vector3 effectorPosition, ref Vector3 effectorOrigin, float minLength, float maxLength, float lerpLength)
			{
				if (lerpLength > 1E-07f)
				{
					float num = (effectorLength - minLength) / lerpLength;
					effectorLength = minLength + num * (maxLength - minLength);
				}
				else
				{
					effectorLength = minLength;
				}
				effectorTranslate = effectorLength * effectorDirection;
				effectorPosition = effectorOrigin + effectorTranslate;
			}

			private static Vector3 SolveFingerIK(ref Vector3 beginPosition, ref Vector3 endPosition, ref Vector3 bendingDirection, float linkLength0, float linkLength1, float linkLength2, ref _FingerIKParams fingerIKParams)
			{
				float num = linkLength0 + linkLength1 + linkLength2;
				float magnitude = (endPosition - beginPosition).magnitude;
				if (magnitude <= 1E-07f)
				{
					return Vector3.zero;
				}
				Vector3 vector = endPosition - beginPosition;
				vector *= 1f / magnitude;
				if (magnitude >= num - 1E-07f)
				{
					return vector;
				}
				if (linkLength0 <= 1E-07f || linkLength1 <= 1E-07f || linkLength2 <= 1E-07f)
				{
					return Vector3.zero;
				}
				Vector3 lhs = Vector3.Cross(vector, bendingDirection);
				lhs = Vector3.Cross(lhs, vector);
				float magnitude2 = lhs.magnitude;
				if (magnitude2 <= 1E-07f)
				{
					return Vector3.zero;
				}
				lhs *= 1f / magnitude2;
				float num2 = Mathf.Lerp(fingerIKParams.beginLink_endCosTheta, 1f, Mathf.Clamp01((magnitude - fingerIKParams.lengthD0) * fingerIKParams.lengthABCDInv));
				float num3 = SAFBIKSqrtClamp01(1f - num2 * num2);
				Vector3 v = vector * num2 + lhs * num3;
				if (!SAFBIKVecNormalize(ref v))
				{
					return Vector3.zero;
				}
				return v;
			}

			private static Vector3 SolveLimbIK(ref Vector3 beginPosition, ref Vector3 endPosition, float beginToInterBaseLength, float beginToInterBaseLengthSq, float interToEndBaseLength, float interToEndBaseLengthSq, ref Vector3 bendingDirection)
			{
				float num = beginToInterBaseLength + interToEndBaseLength;
				if (num <= 1E-07f)
				{
					return Vector3.zero;
				}
				float sqrMagnitude = (endPosition - beginPosition).sqrMagnitude;
				float num2 = SAFBIKSqrt(sqrMagnitude);
				if (num2 <= 1E-07f)
				{
					return Vector3.zero;
				}
				Vector3 vector = (endPosition - beginPosition) * (1f / num2);
				if (num2 >= num - 1E-07f)
				{
					return vector;
				}
				Vector3 lhs = Vector3.Cross(vector, bendingDirection);
				lhs = Vector3.Cross(lhs, vector);
				float magnitude = lhs.magnitude;
				if (magnitude <= 1E-07f)
				{
					return Vector3.zero;
				}
				lhs *= 1f / magnitude;
				float num3 = 1f;
				float num4 = num2;
				float num5 = sqrMagnitude;
				if (num2 < num)
				{
					float num6 = 2f * beginToInterBaseLength * num4;
					if (num6 > 1E-07f)
					{
						num3 = (interToEndBaseLengthSq - beginToInterBaseLengthSq - num5) / (0f - num6);
					}
				}
				float num7 = SAFBIKSqrtClamp01(1f - num3 * num3);
				Vector3 vector2 = vector * num3 * beginToInterBaseLength + lhs * num7 * beginToInterBaseLength;
				float magnitude2 = vector2.magnitude;
				if (magnitude2 <= 1E-07f)
				{
					return Vector3.zero;
				}
				return vector2 * (1f / magnitude2);
			}

			public bool Solve()
			{
				if (_parentBone == null)
				{
					return false;
				}
				_SyncDisplacement();
				bool flag = false;
				Matrix3x4 parentTransform = Matrix3x4.identity;
				parentTransform.origin = _parentBone.worldPosition;
				Quaternion lhs = _parentBone.worldRotation;
				SAFBIKMatSetRotMultInv1(out parentTransform.basis, ref lhs, ref _parentBone._defaultRotation);
				for (int i = 0; i != 5; i++)
				{
					_FingerBranch fingerBranch = _fingerBranches[i];
					if (fingerBranch != null && fingerBranch.effector != null && fingerBranch.effector.positionEnabled)
					{
						flag = ((i != 0) ? (flag | _SolveNotThumb(i, ref parentTransform)) : (flag | _SolveThumb(ref parentTransform)));
					}
				}
				return flag;
			}

			private static Vector3 _GetEffectorPosition(InternalValues internalValues, Bone rootBone, Bone beginLinkBone, Effector effector, float link0ToEffectorLength, ref Matrix3x4 parentTransform)
			{
				if (rootBone != null && beginLinkBone != null && effector != null)
				{
					Vector3 worldPosition = effector.worldPosition;
					if (effector.positionWeight < 0.9999999f)
					{
						Vector3 vector = ((!internalValues.continuousSolverEnabled && !internalValues.resetTransforms) ? effector.bone_worldPosition : (parentTransform * (effector._defaultPosition - rootBone._defaultPosition)));
						Vector3 vector2 = parentTransform * (beginLinkBone._defaultPosition - rootBone._defaultPosition);
						Vector3 vector3 = vector - vector2;
						Vector3 vector4 = worldPosition - vector2;
						float magnitude = vector4.magnitude;
						if (link0ToEffectorLength > 1E-07f && magnitude > 1E-07f)
						{
							Vector3 src = vector3 * (1f / link0ToEffectorLength);
							Vector3 dst = vector4 * (1f / magnitude);
							Vector3 vector5 = _LerpDir(ref src, ref dst, effector.positionWeight);
							float num = Mathf.Lerp(link0ToEffectorLength, magnitude, Mathf.Clamp01(1f - (1f - effector.positionWeight) * 1.15f));
							return vector5 * num + vector2;
						}
					}
					return worldPosition;
				}
				return Vector3.zero;
			}

			private bool _SolveNotThumb(int fingerType, ref Matrix3x4 parentTransform)
			{
				_FingerBranch fingerBranch = _fingerBranches[fingerType];
				if (fingerBranch == null || fingerBranch.fingerLinks.Length != 3)
				{
					return false;
				}
				bool flag = _fingerIKType == FingerIKType.RightWrist;
				_FingerLink fingerLink = fingerBranch.fingerLinks[0];
				_FingerLink fingerLink2 = fingerBranch.fingerLinks[1];
				_FingerLink fingerLink3 = fingerBranch.fingerLinks[2];
				Effector effector = fingerBranch.effector;
				float childToLength = fingerLink.childToLength;
				float childToLength2 = fingerLink2.childToLength;
				float childToLengthSq = fingerLink2.childToLengthSq;
				float childToLength3 = fingerLink3.childToLength;
				float childToLengthSq2 = fingerLink3.childToLengthSq;
				float link0ToEffectorLength = fingerBranch.link0ToEffectorLength;
				Vector3 effectorOrigin = parentTransform * (fingerLink.bone._defaultPosition - _parentBone._defaultPosition);
				Vector3 effectorPosition = _GetEffectorPosition(_internalValues, _parentBone, fingerLink.bone, effector, fingerBranch.link0ToEffectorLength, ref parentTransform);
				Vector3 effectorTranslate = effectorPosition - effectorOrigin;
				float effectorLength = effectorTranslate.magnitude;
				if (effectorLength <= 1E-07f || link0ToEffectorLength <= 1E-07f)
				{
					return false;
				}
				Vector3 vec = effectorTranslate * (1f / effectorLength);
				bool flag2 = (flag ? (fingerBranch.notThumb1BaseAngle.angle <= 1E-07f) : (fingerBranch.notThumb1BaseAngle.angle >= -1E-07f));
				float num = (flag2 ? link0ToEffectorLength : (childToLength + childToLength2 + childToLength3));
				if (effectorLength > num)
				{
					effectorLength = num;
					effectorTranslate = vec * effectorLength;
					effectorPosition = effectorOrigin + effectorTranslate;
				}
				else if (effectorLength < childToLength2)
				{
					effectorLength = childToLength2;
					effectorTranslate = vec * effectorLength;
					effectorPosition = effectorOrigin + effectorTranslate;
				}
				bool flag3 = false;
				Matrix3x3 ret;
				SAFBIKMatMult(out ret, ref parentTransform.basis, ref fingerBranch.boneToSolvedBasis);
				Vector3 ret2;
				SAFBIKMatMultVecInv(out ret2, ref ret, ref vec);
				flag3 = ret2.y >= 0f;
				if (_LimitFingerNotThumb(flag, ref ret2, ref _notThumbPitchUThetaLimit, ref _notThumbPitchLThetaLimit, ref _notThumbYawThetaLimit))
				{
					SAFBIKMatMultVec(out vec, ref ret, ref ret2);
					effectorTranslate = vec * effectorLength;
					effectorPosition = effectorOrigin + effectorTranslate;
				}
				Vector3 solvedDirY = Vector3.zero;
				Vector3 solvedDirZ = Vector3.zero;
				if (!_SolveInDirect(flag, ref solvedDirY, ref solvedDirZ, ref parentTransform.basis, ref fingerLink.boneToSolvedBasis, ref vec))
				{
					return false;
				}
				bool flag4 = !flag2;
				if (flag2)
				{
					bool flag5 = false;
					float num2 = 0f;
					Vector3 vector = parentTransform * (fingerLink2.bone._defaultPosition - _parentBone._defaultPosition);
					Vector3 vector2 = parentTransform * (fingerLink3.bone._defaultPosition - _parentBone._defaultPosition);
					Vector3 vector3 = parentTransform * (effector._defaultPosition - _parentBone._defaultPosition);
					Vector3 src = Vector3.zero;
					Vector3 v = vector - effectorOrigin;
					Vector3 v2 = vector3 - effectorOrigin;
					Matrix3x3 basis;
					Matrix3x3 basis2;
					Matrix3x3 basis3;
					if (SAFBIKVecNormalize2(ref v, ref v2) && SAFBIKComputeBasisFromXZLockX(out basis, flag ? vec : (-vec), solvedDirZ) && SAFBIKComputeBasisFromXZLockZ(out basis2, flag ? v : (-v), basis.column2) && SAFBIKComputeBasisFromXZLockZ(out basis3, flag ? v2 : (-v2), basis.column2))
					{
						Vector3 dirX = (flag ? basis.column0 : (-basis.column0));
						Vector3 dirY = basis.column1;
						Vector3 column = basis.column2;
						Vector3 dirX2 = (flag ? basis2.column0 : (-basis2.column0));
						Vector3 dirY2 = basis2.column1;
						Vector3 vector4 = (flag ? basis3.column0 : (-basis3.column0));
						Vector3 column2 = basis3.column1;
						float cosR = Vector3.Dot(dirX2, vector4);
						float sinR = Vector3.Dot(dirX2, column2);
						Vector3 dirX3 = _Rotate(ref dirX, ref dirY, cosR, sinR);
						flag5 = Vector3.Dot(column2, dirX) >= 0f;
						bool flag6 = false;
						float num3 = Vector3.Dot(vector4, dirX);
						if (flag5)
						{
							if (flag2)
							{
								float angle = _notThumb1PitchUTraceSmooth.angle;
								float cos = _notThumb1PitchUTraceSmooth.cos;
								if (angle <= 1E-07f || num3 < cos)
								{
									Vector3 v3 = Vector3.Cross(column, dirX3);
									if (SAFBIKVecNormalize(ref v3))
									{
										float cos2 = _notThumb1PitchUTrace.cos;
										float sin = _notThumb1PitchUTrace.sin;
										src = _Rotate(ref dirX3, ref v3, cos2, flag ? (0f - sin) : sin);
									}
								}
								else
								{
									float num4 = SAFBIKAcos(num3);
									num4 /= angle;
									num4 = _notThumb1PitchUTrace.angle * num4;
									src = _Rotate(ref dirX2, ref dirY2, num4);
								}
							}
							else
							{
								flag4 = true;
							}
						}
						else if (flag2)
						{
							float num5 = Mathf.Abs(fingerBranch.notThumb1BaseAngle.angle);
							float num6 = Mathf.Max(num5, _notThumb1PitchLTrace.angle);
							float num7 = Mathf.Min(fingerBranch.notThumb1BaseAngle.cos, _notThumb1PitchLTrace.cos);
							if (num3 < num7)
							{
								flag4 = true;
								float num8 = childToLength3 * 0.25f;
								if (effectorLength >= link0ToEffectorLength - num8)
								{
									_LerpEffectorLength(ref effectorLength, ref vec, ref effectorTranslate, ref effectorPosition, ref effectorOrigin, link0ToEffectorLength - num8, childToLength + childToLength2 + childToLength3, num8);
								}
							}
							else if (num6 <= 1E-07f || num6 == num5)
							{
								src = dirX2;
								num2 = ((!(num6 <= 1E-07f)) ? (SAFBIKAcos(num3) / num6) : 1f);
							}
							else
							{
								float num9 = SAFBIKAcos(num3);
								num9 /= num6;
								num2 = num9;
								num9 = (_notThumb1PitchLTrace.angle - num5) * num9;
								src = _Rotate(ref dirX2, ref dirY2, 0f - num9);
							}
						}
						else
						{
							flag4 = true;
						}
						if (flag2 && !flag4 && effectorLength < link0ToEffectorLength - 1E-07f)
						{
							float num10 = 0f;
							if (!flag6)
							{
								num10 = Vector3.Dot(src, dirX);
								num10 = SAFBIKSqrt(1f - num10 * num10);
								num10 *= childToLength;
							}
							float num11 = childToLength3 * 0.25f;
							if (num10 > 1E-07f && effectorLength >= link0ToEffectorLength - num10)
							{
								float num12 = 1f - (effectorLength - (link0ToEffectorLength - num10)) / num10;
								src = _FastLerpDir(ref src, ref dirX, num12);
								num2 += (1f - num2) * num12;
							}
							else
							{
								flag4 = true;
								if (effectorLength >= link0ToEffectorLength - (num10 + num11))
								{
									_LerpEffectorLength(ref effectorLength, ref vec, ref effectorTranslate, ref effectorPosition, ref effectorOrigin, link0ToEffectorLength - (num10 + num11), childToLength + childToLength2 + childToLength3, num11);
								}
							}
						}
					}
					if (!flag4)
					{
						if (src == Vector3.zero)
						{
							return false;
						}
						if (!SAFBIKComputeBasisFromXZLockX(out fingerLink.boneTransform.basis, flag ? src : (-src), solvedDirZ))
						{
							return false;
						}
						fingerLink.boneTransform.origin = effectorOrigin;
						SAFBIKMatMultRet0(ref fingerLink.boneTransform.basis, ref fingerLink.solvedToBoneBasis);
						vector = fingerLink.boneTransform * (fingerLink2.bone._defaultPosition - fingerLink.bone._defaultPosition);
						vector2 = fingerLink.boneTransform * (fingerLink3.bone._defaultPosition - fingerLink.bone._defaultPosition);
						Vector3 vector5 = fingerLink.boneTransform * (effector._defaultPosition - fingerLink.bone._defaultPosition);
						Vector3 vector6 = effectorOrigin + vec * link0ToEffectorLength;
						Vector3 v4 = vector6 - vector;
						Vector3 v5 = vector2 - vector;
						Vector3 v6 = vector5 - vector;
						if (!SAFBIKVecNormalize3(ref v4, ref v5, ref v6))
						{
							return false;
						}
						Vector3 zero = Vector3.zero;
						Matrix3x3 basis4;
						if (!SAFBIKComputeBasisFromXZLockX(out basis4, flag ? v4 : (-v4), solvedDirZ))
						{
							return false;
						}
						Matrix3x3 basis5;
						Matrix3x3 basis6;
						if (!SAFBIKComputeBasisFromXZLockZ(out basis5, flag ? v5 : (-v5), basis4.column2) || !SAFBIKComputeBasisFromXZLockZ(out basis6, flag ? v6 : (-v6), basis4.column2))
						{
							return false;
						}
						Vector3 dirX4 = (flag ? basis4.column0 : (-basis4.column0));
						Vector3 dirY3 = basis4.column1;
						Vector3 src2 = (flag ? basis5.column0 : (-basis5.column0));
						Vector3 rhs = (flag ? basis6.column0 : (-basis6.column0));
						Vector3 column3 = basis6.column1;
						float cosR2 = Vector3.Dot(src2, rhs);
						float sinR2 = Vector3.Dot(src2, column3);
						Vector3 src3 = _Rotate(ref dirX4, ref dirY3, cosR2, sinR2);
						zero = ((!flag5) ? _FastLerpDir(ref src2, ref dirX4, num2) : _FastLerpDir(ref src3, ref dirX4, num2));
						if (!SAFBIKComputeBasisFromXZLockX(out fingerLink2.boneTransform.basis, flag ? zero : (-zero), solvedDirZ))
						{
							return false;
						}
						fingerLink2.boneTransform.origin = vector;
						SAFBIKMatMultRet0(ref fingerLink2.boneTransform.basis, ref fingerLink2.solvedToBoneBasis);
						vector2 = fingerLink2.boneTransform * (fingerLink3.bone._defaultPosition - fingerLink2.bone._defaultPosition);
						Vector3 v7 = vector6 - vector2;
						if (!SAFBIKVecNormalize(ref v7))
						{
							return false;
						}
						Vector3 ret3;
						SAFBIKMatMultVec(out ret3, ref fingerLink2.boneTransform.basis, ref fingerLink3.boneToSolvedBasis.column2);
						if (!SAFBIKComputeBasisFromXZLockX(out fingerLink3.boneTransform.basis, flag ? v7 : (-v7), ret3))
						{
							return false;
						}
						fingerLink3.boneTransform.origin = vector2;
						SAFBIKMatMultRet0(ref fingerLink3.boneTransform.basis, ref fingerLink3.solvedToBoneBasis);
					}
				}
				if (flag4)
				{
					Vector3 vec2 = SolveFingerIK(ref effectorOrigin, ref effectorPosition, ref solvedDirY, childToLength, childToLength2, childToLength3, ref fingerBranch.fingerIKParams);
					if (vec2 == Vector3.zero)
					{
						return false;
					}
					if (!flag3)
					{
						Vector3 ret4;
						SAFBIKMatMultVec(out ret4, ref parentTransform.basis, ref fingerLink.boneToSolvedBasis.column0);
						Matrix3x3 basis7;
						if (SAFBIKComputeBasisFromXZLockZ(out basis7, ret4, solvedDirZ))
						{
							Vector3 ret5;
							SAFBIKMatMultVecInv(out ret5, ref basis7, ref vec2);
							float x = ret5.x;
							float y = ret5.y;
							float z = ret5.z;
							float cos3 = _notThumb1PitchLLimit.cos;
							if ((flag && x < cos3) || (!flag && x > 0f - cos3))
							{
								float num13 = SAFBIKSqrt(1f - (cos3 * cos3 + z * z));
								ret5.x = (flag ? cos3 : (0f - cos3));
								ret5.y = ((y >= 0f) ? num13 : (0f - num13));
								SAFBIKMatMultVec(out vec2, ref basis7, ref ret5);
							}
						}
					}
					if (!SAFBIKComputeBasisFromXZLockX(out fingerLink.boneTransform.basis, flag ? vec2 : (-vec2), solvedDirZ))
					{
						return false;
					}
					fingerLink.boneTransform.origin = effectorOrigin;
					SAFBIKMatMultRet0(ref fingerLink.boneTransform.basis, ref fingerLink.solvedToBoneBasis);
					Vector3 beginPosition = fingerLink.boneTransform * (fingerLink2.bone._defaultPosition - fingerLink.bone._defaultPosition);
					Vector3 rhs2 = effectorPosition - beginPosition;
					Vector3 ret6;
					SAFBIKMatMultVec(out ret6, ref fingerLink.boneTransform.basis, ref _internalValues.defaultRootBasis.column2);
					solvedDirY = Vector3.Cross(ret6, rhs2);
					if (!SAFBIKVecNormalize(ref solvedDirY))
					{
						return false;
					}
					solvedDirY = (flag ? solvedDirY : (-solvedDirY));
					Vector3 vector7 = SolveLimbIK(ref beginPosition, ref effectorPosition, childToLength2, childToLengthSq, childToLength3, childToLengthSq2, ref solvedDirY);
					if (vector7 == Vector3.zero)
					{
						return false;
					}
					SAFBIKMatMultVec(out ret6, ref fingerLink.boneTransform.basis, ref fingerLink2.boneToSolvedBasis.column2);
					if (!SAFBIKComputeBasisFromXZLockX(out fingerLink2.boneTransform.basis, flag ? vector7 : (-vector7), ret6))
					{
						return false;
					}
					fingerLink2.boneTransform.origin = beginPosition;
					SAFBIKMatMultRet0(ref fingerLink2.boneTransform.basis, ref fingerLink2.solvedToBoneBasis);
					Vector3 vector8 = fingerLink2.boneTransform * (fingerLink3.bone._defaultPosition - fingerLink2.bone._defaultPosition);
					Vector3 v8 = effectorPosition - vector8;
					if (!SAFBIKVecNormalize(ref v8))
					{
						return false;
					}
					Vector3 ret7;
					SAFBIKMatMultVec(out ret7, ref fingerLink2.boneTransform.basis, ref fingerLink3.boneToSolvedBasis.column2);
					if (!SAFBIKComputeBasisFromXZLockX(out fingerLink3.boneTransform.basis, flag ? v8 : (-v8), ret7))
					{
						return false;
					}
					fingerLink3.boneTransform.origin = vector8;
					SAFBIKMatMultRet0(ref fingerLink3.boneTransform.basis, ref fingerLink3.solvedToBoneBasis);
				}
				Quaternion ret8;
				SAFBIKMatMultGetRot(out ret8, ref fingerLink.boneTransform.basis, ref fingerLink.bone._defaultBasis);
				fingerLink.bone.worldRotation = ret8;
				SAFBIKMatMultGetRot(out ret8, ref fingerLink2.boneTransform.basis, ref fingerLink2.bone._defaultBasis);
				fingerLink2.bone.worldRotation = ret8;
				SAFBIKMatMultGetRot(out ret8, ref fingerLink3.boneTransform.basis, ref fingerLink3.bone._defaultBasis);
				fingerLink3.bone.worldRotation = ret8;
				return true;
			}

			private bool _SolveThumb(ref Matrix3x4 parentTransform)
			{
				_FingerBranch fingerBranch = _fingerBranches[0];
				if (fingerBranch == null || fingerBranch.fingerLinks.Length != 3)
				{
					return false;
				}
				_FingerLink fingerLink = fingerBranch.fingerLinks[0];
				_FingerLink fingerLink2 = fingerBranch.fingerLinks[1];
				_FingerLink fingerLink3 = fingerBranch.fingerLinks[2];
				_ThumbLink thumbLink = _thumbBranch.thumbLinks[0];
				_ThumbLink thumbLink2 = _thumbBranch.thumbLinks[1];
				_ThumbLink thumbLink3 = _thumbBranch.thumbLinks[2];
				bool flag = _fingerIKType == FingerIKType.RightWrist;
				Vector3 vector = parentTransform * (fingerLink.bone._defaultPosition - _parentBone._defaultPosition);
				Effector effector = fingerBranch.effector;
				Vector3 vector2 = _GetEffectorPosition(_internalValues, _parentBone, fingerLink.bone, effector, fingerBranch.link0ToEffectorLength, ref parentTransform);
				Vector3 vector3 = vector2 - vector;
				float magnitude = vector3.magnitude;
				if (magnitude < 1E-07f || fingerBranch.link0ToEffectorLength < 1E-07f)
				{
					return false;
				}
				Vector3 vector4 = vector3 * (1f / magnitude);
				if (magnitude > fingerBranch.link0ToEffectorLength)
				{
					magnitude = fingerBranch.link0ToEffectorLength;
					vector3 = vector4 * fingerBranch.link0ToEffectorLength;
					vector2 = vector + vector3;
				}
				Vector3 vec = vector4;
				if (_thumbBranch.thumb0_isLimited)
				{
					Matrix3x3 ret;
					SAFBIKMatMult(out ret, ref parentTransform.basis, ref fingerBranch.boneToSolvedBasis);
					Vector3 ret2;
					SAFBIKMatMultVecInv(out ret2, ref ret, ref vec);
					if (_LimitYZ(flag, ref ret2, _thumbBranch.thumb0_lowerLimit, _thumbBranch.thumb0_upperLimit, _thumbBranch.thumb0_innerLimit, _thumbBranch.thumb0_outerLimit))
					{
						SAFBIKMatMultVec(out vec, ref ret, ref ret2);
					}
				}
				Vector3 ret3;
				SAFBIKMatMultVec(out ret3, ref parentTransform.basis, ref thumbLink.thumb_boneToSolvedBasis.column1);
				if (!SAFBIKComputeBasisFromXYLockX(out fingerLink.boneTransform.basis, flag ? vec : (-vec), ret3))
				{
					return false;
				}
				fingerLink.boneTransform.origin = vector;
				SAFBIKMatMultRet0(ref fingerLink.boneTransform.basis, ref thumbLink.thumb_solvedToBoneBasis);
				Vector3 vector5 = fingerLink.boneTransform * (fingerLink2.bone._defaultPosition - fingerLink.bone._defaultPosition);
				float magnitude2 = (vector2 - vector5).magnitude;
				if (magnitude2 < _thumbBranch.linkLength1to3 - 1E-07f)
				{
					Vector3 v = vector2 - fingerLink.boneTransform.origin;
					float lengthSq;
					float num = SAFBIKVecLengthAndLengthSq(out lengthSq, ref v);
					float num2 = 1f;
					if (num > 1E-07f)
					{
						Vector3 v2 = vector5 - fingerLink.boneTransform.origin;
						if (SAFBIKVecNormalize(ref v2))
						{
							num2 = Vector3.Dot(v * (1f / num), v2);
						}
					}
					float linkLength0to = _thumbBranch.linkLength0to1;
					float linkLength0to1Sq = _thumbBranch.linkLength0to1Sq;
					float lenB = num;
					float lenBSq = lengthSq;
					float num3 = magnitude2 + (_thumbBranch.linkLength1to3 - magnitude2) * 0.5f;
					float lenCSq = num3 * num3;
					float num4 = _ComputeTriangleTheta(linkLength0to, lenB, num3, linkLength0to1Sq, lenBSq, lenCSq);
					if (num4 < num2)
					{
						float num5 = SAFBIKAcos(num4) - SAFBIKAcos(num2);
						if (num5 > 0.00017453292f)
						{
							float num6 = SAFBIKSqrt(linkLength0to1Sq * 2f * (1f - SAFBIKCos(num5)));
							if (num6 > 1E-07f)
							{
								Vector3 ret4;
								SAFBIKMatMultVec(out ret4, ref fingerLink.boneTransform.basis, ref _thumbBranch.thumbSolveZ);
								vector5 += ret4 * num6;
								Vector3 v3 = vector5 - fingerLink.boneTransform.origin;
								if (SAFBIKVecNormalize(ref v3))
								{
									Vector3 ret5;
									SAFBIKMatMultVec(out ret5, ref fingerLink.boneTransform.basis, ref fingerLink.boneToSolvedBasis.column1);
									Matrix3x3 basis;
									if (SAFBIKComputeBasisFromXYLockX(out basis, flag ? v3 : (-v3), ret5))
									{
										SAFBIKMatMult(out fingerLink.boneTransform.basis, ref basis, ref fingerLink.solvedToBoneBasis);
									}
								}
							}
						}
					}
				}
				Vector3 vector6 = fingerLink.boneTransform * (fingerLink2.bone._defaultPosition - fingerLink.bone._defaultPosition);
				Vector3 v4 = vector2 - vector6;
				if (!SAFBIKVecNormalize(ref v4))
				{
					return false;
				}
				Vector3 ret6;
				SAFBIKMatMultVec(out ret6, ref fingerLink.boneTransform.basis, ref thumbLink2.thumb_boneToSolvedBasis.column1);
				if (!SAFBIKComputeBasisFromXYLockX(out fingerLink2.boneTransform.basis, flag ? v4 : (-v4), ret6))
				{
					return false;
				}
				fingerLink2.boneTransform.origin = vector6;
				SAFBIKMatMultRet0(ref fingerLink2.boneTransform.basis, ref thumbLink2.thumb_solvedToBoneBasis);
				float sqrMagnitude = (vector2 - fingerLink2.boneTransform.origin).sqrMagnitude;
				float num7 = SAFBIKSqrt(sqrMagnitude);
				float linkLength1to = _thumbBranch.linkLength1to2;
				float linkLength1to2Sq = _thumbBranch.linkLength1to2Sq;
				float lenB2 = num7;
				float lenBSq2 = sqrMagnitude;
				float linkLength2to = _thumbBranch.linkLength2to3;
				float linkLength2to3Sq = _thumbBranch.linkLength2to3Sq;
				float num8 = _ComputeTriangleTheta(linkLength1to, lenB2, linkLength2to, linkLength1to2Sq, lenBSq2, linkLength2to3Sq);
				if (num8 < _thumbBranch.thumb1_baseThetaAtoB)
				{
					float num9 = SAFBIKAcos(num8) - _thumbBranch.thumb1_Acos_baseThetaAtoB;
					if (num9 > 0.00017453292f)
					{
						float num10 = linkLength1to2Sq * 2f;
						float num11 = SAFBIKSqrt(num10 - num10 * SAFBIKCos(num9));
						Vector3 ret7;
						SAFBIKMatMultVec(out ret7, ref fingerLink2.boneTransform.basis, ref _thumbBranch.thumbSolveZ);
						Vector3 v5 = fingerLink2.boneTransform * (fingerLink3.bone._defaultPosition - fingerLink2.bone._defaultPosition) + ret7 * num11 - fingerLink2.boneTransform.origin;
						if (SAFBIKVecNormalize(ref v5))
						{
							Vector3 ret8;
							SAFBIKMatMultVec(out ret8, ref fingerLink2.boneTransform.basis, ref fingerLink2.boneToSolvedBasis.column1);
							Matrix3x3 basis2;
							if (SAFBIKComputeBasisFromXYLockX(out basis2, flag ? v5 : (-v5), ret8))
							{
								SAFBIKMatMult(out fingerLink2.boneTransform.basis, ref basis2, ref fingerLink2.solvedToBoneBasis);
							}
						}
					}
				}
				Vector3 vector7 = fingerLink2.boneTransform * (fingerLink3.bone._defaultPosition - fingerLink2.bone._defaultPosition);
				Vector3 v6 = vector2 - vector7;
				if (!SAFBIKVecNormalize(ref v6))
				{
					return false;
				}
				Vector3 ret9;
				SAFBIKMatMultVec(out ret9, ref fingerLink2.boneTransform.basis, ref thumbLink3.thumb_boneToSolvedBasis.column1);
				if (!SAFBIKComputeBasisFromXYLockX(out fingerLink3.boneTransform.basis, flag ? v6 : (-v6), ret9))
				{
					return false;
				}
				fingerLink3.boneTransform.origin = vector7;
				SAFBIKMatMultRet0(ref fingerLink3.boneTransform.basis, ref thumbLink3.thumb_solvedToBoneBasis);
				Quaternion ret10;
				SAFBIKMatMultGetRot(out ret10, ref fingerLink.boneTransform.basis, ref fingerLink.bone._defaultBasis);
				fingerLink.bone.worldRotation = ret10;
				SAFBIKMatMultGetRot(out ret10, ref fingerLink2.boneTransform.basis, ref fingerLink2.bone._defaultBasis);
				fingerLink2.bone.worldRotation = ret10;
				SAFBIKMatMultGetRot(out ret10, ref fingerLink3.boneTransform.basis, ref fingerLink3.bone._defaultBasis);
				fingerLink3.bone.worldRotation = ret10;
				return true;
			}
		}

		public class HeadIK
		{
			private Settings _settings;

			private InternalValues _internalValues;

			private Bone _neckBone;

			private Bone _headBone;

			private Bone _leftEyeBone;

			private Bone _rightEyeBone;

			private Effector _headEffector;

			private Effector _eyesEffector;

			private Quaternion _headEffectorToWorldRotation = Quaternion.identity;

			private Quaternion _headToLeftEyeRotation = Quaternion.identity;

			private Quaternion _headToRightEyeRotation = Quaternion.identity;

			private bool _isSyncDisplacementAtLeastOnce;

			private bool _isEnabledCustomEyes;

			public HeadIK(FullBodyIK fullBodyIK)
			{
				_settings = fullBodyIK.settings;
				_internalValues = fullBodyIK.internalValues;
				_neckBone = _PrepareBone(fullBodyIK.headBones.neck);
				_headBone = _PrepareBone(fullBodyIK.headBones.head);
				_leftEyeBone = _PrepareBone(fullBodyIK.headBones.leftEye);
				_rightEyeBone = _PrepareBone(fullBodyIK.headBones.rightEye);
				_headEffector = fullBodyIK.headEffectors.head;
				_eyesEffector = fullBodyIK.headEffectors.eyes;
			}

			private void _SyncDisplacement(FullBodyIK fullBodyIK)
			{
				if (_settings.syncDisplacement != SyncDisplacement.Everyframe && _isSyncDisplacementAtLeastOnce)
				{
					return;
				}
				_isSyncDisplacementAtLeastOnce = true;
				if (_headBone != null && _headBone.transformIsAlive)
				{
					if (_headEffector != null)
					{
						SAFBIKQuatMultInv0(out _headEffectorToWorldRotation, ref _headEffector._defaultRotation, ref _headBone._defaultRotation);
					}
					if (_leftEyeBone != null && _leftEyeBone.transformIsAlive)
					{
						SAFBIKQuatMultInv0(out _headToLeftEyeRotation, ref _headBone._defaultRotation, ref _leftEyeBone._defaultRotation);
					}
					if (_rightEyeBone != null && _rightEyeBone.transformIsAlive)
					{
						SAFBIKQuatMultInv0(out _headToRightEyeRotation, ref _headBone._defaultRotation, ref _rightEyeBone._defaultRotation);
					}
				}
				_isEnabledCustomEyes = fullBodyIK._PrepareCustomEyes(ref _headToLeftEyeRotation, ref _headToRightEyeRotation);
			}

			public bool Solve(FullBodyIK fullBodyIK)
			{
				if (_neckBone == null || !_neckBone.transformIsAlive || _headBone == null || !_headBone.transformIsAlive || _headBone.parentBone == null || !_headBone.parentBone.transformIsAlive)
				{
					return false;
				}
				_SyncDisplacement(fullBodyIK);
				float num = (_headEffector.positionEnabled ? _headEffector.positionWeight : 0f);
				float num2 = (_eyesEffector.positionEnabled ? _eyesEffector.positionWeight : 0f);
				if (num <= 1E-07f && num2 <= 1E-07f)
				{
					Quaternion q = _neckBone.parentBone.worldRotation;
					Quaternion ret;
					SAFBIKQuatMult(out ret, ref q, ref _neckBone.parentBone._worldToBaseRotation);
					if (_internalValues.resetTransforms)
					{
						Quaternion ret2;
						SAFBIKQuatMult(out ret2, ref ret, ref _neckBone._baseToWorldRotation);
						_neckBone.worldRotation = ret2;
					}
					float num3 = (_headEffector.rotationEnabled ? _headEffector.rotationWeight : 0f);
					if (num3 > 1E-07f)
					{
						Quaternion q2 = _headEffector.worldRotation;
						Quaternion ret3;
						SAFBIKQuatMult(out ret3, ref q2, ref _headEffectorToWorldRotation);
						if (num3 < 0.9999999f)
						{
							Quaternion ret4;
							if (_internalValues.resetTransforms)
							{
								SAFBIKQuatMult(out ret4, ref ret, ref _headBone._baseToWorldRotation);
							}
							else
							{
								ret4 = _headBone.worldRotation;
							}
							_headBone.worldRotation = Quaternion.Lerp(ret4, ret3, num3);
						}
						else
						{
							_headBone.worldRotation = ret3;
						}
						_HeadRotationLimit();
					}
					else if (_internalValues.resetTransforms)
					{
						Quaternion ret5;
						SAFBIKQuatMult(out ret5, ref ret, ref _headBone._baseToWorldRotation);
						_headBone.worldRotation = ret5;
					}
					if (_internalValues.resetTransforms)
					{
						if (_isEnabledCustomEyes)
						{
							fullBodyIK._ResetCustomEyes();
						}
						else
						{
							_ResetEyes();
						}
					}
					if (!_internalValues.resetTransforms)
					{
						return num3 > 1E-07f;
					}
					return true;
				}
				_Solve(fullBodyIK);
				return true;
			}

			private void _HeadRotationLimit()
			{
				Quaternion q = _headBone.worldRotation;
				Quaternion ret;
				SAFBIKQuatMult(out ret, ref q, ref _headBone._worldToBaseRotation);
				q = _neckBone.worldRotation;
				Quaternion ret2;
				SAFBIKQuatMult(out ret2, ref q, ref _neckBone._worldToBaseRotation);
				Quaternion ret3;
				SAFBIKQuatMultInv0(out ret3, ref ret2, ref ret);
				Matrix3x3 m;
				SAFBIKMatSetRot(out m, ref ret3);
				Vector3 dir = m.column1;
				Vector3 dir2 = m.column2;
				if ((false | _LimitXZ_Square(ref dir, _internalValues.headIK.headLimitRollTheta.sin, _internalValues.headIK.headLimitRollTheta.sin, _internalValues.headIK.headLimitPitchUpTheta.sin, _internalValues.headIK.headLimitPitchDownTheta.sin) | _LimitXY_Square(ref dir2, _internalValues.headIK.headLimitYawTheta.sin, _internalValues.headIK.headLimitYawTheta.sin, _internalValues.headIK.headLimitPitchDownTheta.sin, _internalValues.headIK.headLimitPitchUpTheta.sin)) && SAFBIKComputeBasisFromYZLockZ(out m, ref dir, ref dir2))
				{
					SAFBIKMatGetRot(out ret3, ref m);
					SAFBIKQuatMultNorm3(out ret, ref ret2, ref ret3, ref _headBone._baseToWorldRotation);
					_headBone.worldRotation = ret;
				}
			}

			private void _Solve(FullBodyIK fullBodyIK)
			{
				Quaternion lhs = _neckBone.parentBone.worldRotation;
				Matrix3x3 ret;
				SAFBIKMatSetRotMultInv1(out ret, ref lhs, ref _neckBone.parentBone._defaultRotation);
				Matrix3x3 ret2;
				SAFBIKMatMult(out ret2, ref ret, ref _internalValues.defaultRootBasis);
				Quaternion ret3;
				SAFBIKQuatMult(out ret3, ref lhs, ref _neckBone.parentBone._worldToBaseRotation);
				float num = (_headEffector.positionEnabled ? _headEffector.positionWeight : 0f);
				float num2 = (_eyesEffector.positionEnabled ? _eyesEffector.positionWeight : 0f);
				Quaternion q = Quaternion.identity;
				Quaternion headPrevRotation = Quaternion.identity;
				Quaternion leftEyePrevRotation = Quaternion.identity;
				Quaternion rightEyePrevRotation = Quaternion.identity;
				if (!_internalValues.resetTransforms)
				{
					q = _neckBone.worldRotation;
					headPrevRotation = _headBone.worldRotation;
					if (_leftEyeBone != null && _leftEyeBone.transformIsAlive)
					{
						leftEyePrevRotation = _leftEyeBone.worldRotation;
					}
					if (_rightEyeBone != null && _rightEyeBone.transformIsAlive)
					{
						rightEyePrevRotation = _rightEyeBone.worldRotation;
					}
				}
				if (num > 1E-07f)
				{
					Matrix3x3 ret4;
					SAFBIKMatMult(out ret4, ref ret, ref _neckBone._localAxisBasis);
					Vector3 v = _headEffector.worldPosition - _neckBone.worldPosition;
					if (SAFBIKVecNormalize(ref v))
					{
						Vector3 ret5;
						SAFBIKMatMultVecInv(out ret5, ref ret4, ref v);
						if (_LimitXZ_Square(ref ret5, _internalValues.headIK.neckLimitRollTheta.sin, _internalValues.headIK.neckLimitRollTheta.sin, _internalValues.headIK.neckLimitPitchDownTheta.sin, _internalValues.headIK.neckLimitPitchUpTheta.sin))
						{
							SAFBIKMatMultVec(out v, ref ret4, ref ret5);
						}
						Vector3 dirX = ret2.column0;
						Vector3 dirZ = ret2.column2;
						if (SAFBIKComputeBasisLockY(out ret4, ref dirX, ref v, ref dirZ))
						{
							Quaternion ret6;
							SAFBIKMatMultGetRot(out ret6, ref ret4, ref _neckBone._boneToWorldBasis);
							if (num < 0.9999999f)
							{
								Quaternion ret7;
								if (_internalValues.resetTransforms)
								{
									SAFBIKQuatMult(out ret7, ref ret3, ref _neckBone._baseToWorldRotation);
								}
								else
								{
									ret7 = q;
								}
								_neckBone.worldRotation = Quaternion.Lerp(ret7, ret6, num);
							}
							else
							{
								_neckBone.worldRotation = ret6;
							}
						}
					}
				}
				else if (_internalValues.resetTransforms)
				{
					Quaternion ret8;
					SAFBIKQuatMult(out ret8, ref ret3, ref _neckBone._baseToWorldRotation);
					_neckBone.worldRotation = ret8;
				}
				if (num2 <= 1E-07f)
				{
					float num3 = (_headEffector.rotationEnabled ? _headEffector.rotationWeight : 0f);
					if (num3 > 1E-07f)
					{
						Quaternion q2 = _headEffector.worldRotation;
						Quaternion ret9;
						SAFBIKQuatMult(out ret9, ref q2, ref _headEffectorToWorldRotation);
						if (num3 < 0.9999999f)
						{
							Quaternion q3 = _neckBone.worldRotation;
							Quaternion ret10;
							if (_internalValues.resetTransforms)
							{
								SAFBIKQuatMult3(out ret10, ref q3, ref _neckBone._worldToBaseRotation, ref _headBone._baseToWorldRotation);
							}
							else
							{
								SAFBIKQuatMultNorm3Inv1(out ret10, ref q3, ref q, ref headPrevRotation);
							}
							_headBone.worldRotation = Quaternion.Lerp(ret10, ret9, num3);
						}
						else
						{
							_headBone.worldRotation = ret9;
						}
					}
					else if (_internalValues.resetTransforms)
					{
						Quaternion q4 = _neckBone.worldRotation;
						Quaternion ret11;
						SAFBIKQuatMult3(out ret11, ref q4, ref _neckBone._worldToBaseRotation, ref _headBone._baseToWorldRotation);
						_headBone.worldRotation = ret11;
					}
					_HeadRotationLimit();
					if (_internalValues.resetTransforms)
					{
						if (_isEnabledCustomEyes)
						{
							fullBodyIK._ResetCustomEyes();
						}
						else
						{
							_ResetEyes();
						}
					}
					return;
				}
				Vector3 addVec = _neckBone.parentBone.worldPosition;
				Vector3 ret12;
				SAFBIKMatMultVecPreSubAdd(out ret12, ref ret, ref _eyesEffector._defaultPosition, ref _neckBone.parentBone._defaultPosition, ref addVec);
				Vector3 vec = _eyesEffector.worldPosition - ret12;
				Matrix3x3 basis = ret2;
				Vector3 ret13;
				SAFBIKMatMultVecInv(out ret13, ref ret2, ref vec);
				ret13.y *= _settings.headIK.eyesToNeckPitchRate;
				SAFBIKVecNormalize(ref ret13);
				if (_ComputeEyesRange(ref ret13, _internalValues.headIK.eyesTraceTheta.cos))
				{
					if (ret13.y < 0f - _internalValues.headIK.neckLimitPitchDownTheta.sin)
					{
						ret13.y = 0f - _internalValues.headIK.neckLimitPitchDownTheta.sin;
					}
					else if (ret13.y > _internalValues.headIK.neckLimitPitchUpTheta.sin)
					{
						ret13.y = _internalValues.headIK.neckLimitPitchUpTheta.sin;
					}
					ret13.x = 0f;
					ret13.z = SAFBIKSqrt(1f - ret13.y * ret13.y);
				}
				SAFBIKMatMultVec(out vec, ref ret2, ref ret13);
				Vector3 dirX2 = ret2.column0;
				Vector3 dirY = ret2.column1;
				Vector3 dirZ2 = vec;
				if (!SAFBIKComputeBasisLockZ(out basis, ref dirX2, ref dirY, ref dirZ2))
				{
					basis = ret2;
				}
				Quaternion ret14;
				SAFBIKMatMultGetRot(out ret14, ref basis, ref _neckBone._baseToWorldBasis);
				if (_eyesEffector.positionWeight < 0.9999999f)
				{
					Quaternion lhs2 = Quaternion.Lerp(_neckBone.worldRotation, ret14, _eyesEffector.positionWeight);
					_neckBone.worldRotation = lhs2;
					SAFBIKMatSetRotMult(out basis, ref lhs2, ref _neckBone._worldToBaseRotation);
				}
				else
				{
					_neckBone.worldRotation = ret14;
				}
				Matrix3x3 ret15;
				SAFBIKMatMult(out ret15, ref basis, ref _internalValues.defaultRootBasisInv);
				Vector3 addVec2 = _neckBone.worldPosition;
				SAFBIKMatMultVecPreSubAdd(out ret12, ref ret15, ref _eyesEffector._defaultPosition, ref _neckBone._defaultPosition, ref addVec2);
				vec = _eyesEffector.worldPosition - ret12;
				Matrix3x3 basis2 = basis;
				Vector3 ret16;
				SAFBIKMatMultVecInv(out ret16, ref basis, ref vec);
				ret16.x *= _settings.headIK.eyesToHeadYawRate;
				ret16.y *= _settings.headIK.eyesToHeadPitchRate;
				SAFBIKVecNormalize(ref ret16);
				if (_ComputeEyesRange(ref ret16, _internalValues.headIK.eyesTraceTheta.cos))
				{
					_LimitXY_Square(ref ret16, _internalValues.headIK.headLimitYawTheta.sin, _internalValues.headIK.headLimitYawTheta.sin, _internalValues.headIK.headLimitPitchDownTheta.sin, _internalValues.headIK.headLimitPitchUpTheta.sin);
				}
				SAFBIKMatMultVec(out vec, ref basis, ref ret16);
				Vector3 dirX3 = basis.column0;
				Vector3 dirY2 = basis.column1;
				Vector3 dirZ3 = vec;
				if (!SAFBIKComputeBasisLockZ(out basis2, ref dirX3, ref dirY2, ref dirZ3))
				{
					basis2 = basis;
				}
				Quaternion ret17;
				SAFBIKMatMultGetRot(out ret17, ref basis2, ref _headBone._baseToWorldBasis);
				if (_eyesEffector.positionWeight < 0.9999999f)
				{
					Quaternion q5 = _neckBone.worldRotation;
					Quaternion ret18;
					SAFBIKQuatMultNorm3Inv1(out ret18, ref q5, ref q, ref headPrevRotation);
					Quaternion lhs3 = Quaternion.Lerp(ret18, ret17, _eyesEffector.positionWeight);
					_headBone.worldRotation = lhs3;
					SAFBIKMatSetRotMult(out basis2, ref lhs3, ref _headBone._worldToBaseRotation);
				}
				else
				{
					_headBone.worldRotation = ret17;
				}
				Matrix3x3 ret19;
				SAFBIKMatMult(out ret19, ref basis2, ref _internalValues.defaultRootBasisInv);
				if (_isEnabledCustomEyes)
				{
					fullBodyIK._SolveCustomEyes(ref ret15, ref ret19, ref basis2);
				}
				else
				{
					_SolveEyes(ref ret15, ref ret19, ref basis2, ref headPrevRotation, ref leftEyePrevRotation, ref rightEyePrevRotation);
				}
			}

			private void _ResetEyes()
			{
				if (_headBone != null && _headBone.transformIsAlive)
				{
					Quaternion q = _headBone.worldRotation;
					Quaternion ret;
					if (_leftEyeBone != null && _leftEyeBone.transformIsAlive)
					{
						SAFBIKQuatMultNorm(out ret, ref q, ref _headToLeftEyeRotation);
						_leftEyeBone.worldRotation = ret;
					}
					if (_rightEyeBone != null && _rightEyeBone.transformIsAlive)
					{
						SAFBIKQuatMultNorm(out ret, ref q, ref _headToRightEyeRotation);
						_rightEyeBone.worldRotation = ret;
					}
				}
			}

			private void _SolveEyes(ref Matrix3x3 neckBasis, ref Matrix3x3 headBasis, ref Matrix3x3 headBaseBasis, ref Quaternion headPrevRotation, ref Quaternion leftEyePrevRotation, ref Quaternion rightEyePrevRotation)
			{
				if (_headBone == null || !_headBone.transformIsAlive || ((_leftEyeBone == null || !_leftEyeBone.transformIsAlive) && (_rightEyeBone == null || !_rightEyeBone.transformIsAlive)))
				{
					return;
				}
				Vector3 addVec = _neckBone.worldPosition;
				Vector3 ret;
				SAFBIKMatMultVecPreSubAdd(out ret, ref neckBasis, ref _headBone._defaultPosition, ref _neckBone._defaultPosition, ref addVec);
				Vector3 ret2;
				SAFBIKMatMultVecPreSubAdd(out ret2, ref headBasis, ref _eyesEffector._defaultPosition, ref _headBone._defaultPosition, ref ret);
				Vector3 ret3 = _eyesEffector.worldPosition - ret2;
				SAFBIKMatMultVecInv(out ret3, ref headBaseBasis, ref ret3);
				SAFBIKVecNormalize(ref ret3);
				if (_internalValues.resetTransforms && _eyesEffector.positionWeight < 0.9999999f)
				{
					Vector3 v = Vector3.Lerp(new Vector3(0f, 0f, 1f), ret3, _eyesEffector.positionWeight);
					if (SAFBIKVecNormalize(ref v))
					{
						ret3 = v;
					}
				}
				_LimitXY_Square(ref ret3, _internalValues.headIK.eyesLimitYawTheta.sin, _internalValues.headIK.eyesLimitYawTheta.sin, _internalValues.headIK.eyesLimitPitchTheta.sin, _internalValues.headIK.eyesLimitPitchTheta.sin);
				ret3.x *= _settings.headIK.eyesYawRate;
				ret3.y *= _settings.headIK.eyesPitchRate;
				Vector3 v2 = ret3;
				Vector3 v3 = ret3;
				if (ret3.x >= 0f)
				{
					v2.x *= _settings.headIK.eyesYawInnerRate;
					v3.x *= _settings.headIK.eyesYawOuterRate;
				}
				else
				{
					v2.x *= _settings.headIK.eyesYawOuterRate;
					v3.x *= _settings.headIK.eyesYawInnerRate;
				}
				SAFBIKVecNormalize2(ref v2, ref v3);
				SAFBIKMatMultVec(out v2, ref headBaseBasis, ref v2);
				SAFBIKMatMultVec(out v3, ref headBaseBasis, ref v3);
				Quaternion q = _headBone.worldRotation;
				Quaternion ret4;
				if (_leftEyeBone != null && _leftEyeBone.transformIsAlive)
				{
					Matrix3x3 basis;
					SAFBIKComputeBasisLockZ(out basis, ref headBasis.column0, ref headBasis.column1, ref v2);
					SAFBIKMatMultGetRot(out ret4, ref basis, ref _leftEyeBone._baseToWorldBasis);
					if (!_internalValues.resetTransforms && _eyesEffector.positionWeight < 0.9999999f)
					{
						Quaternion ret5;
						SAFBIKQuatMultNorm3Inv1(out ret5, ref q, ref headPrevRotation, ref leftEyePrevRotation);
						_leftEyeBone.worldRotation = Quaternion.Lerp(ret5, ret4, _eyesEffector.positionWeight);
					}
					else
					{
						_leftEyeBone.worldRotation = ret4;
					}
				}
				if (_rightEyeBone != null && _rightEyeBone.transformIsAlive)
				{
					Matrix3x3 basis2;
					SAFBIKComputeBasisLockZ(out basis2, ref headBasis.column0, ref headBasis.column1, ref v3);
					SAFBIKMatMultGetRot(out ret4, ref basis2, ref _rightEyeBone._baseToWorldBasis);
					if (!_internalValues.resetTransforms && _eyesEffector.positionWeight < 0.9999999f)
					{
						Quaternion ret6;
						SAFBIKQuatMultNorm3Inv1(out ret6, ref q, ref headPrevRotation, ref rightEyePrevRotation);
						_rightEyeBone.worldRotation = Quaternion.Lerp(ret6, ret4, _eyesEffector.positionWeight);
					}
					else
					{
						_rightEyeBone.worldRotation = ret4;
					}
				}
			}
		}

		public class LimbIK
		{
			private struct RollBone
			{
				public Bone bone;

				public float rate;
			}

			private Settings _settings;

			private InternalValues _internalValues;

			public LimbIKLocation _limbIKLocation;

			private LimbIKType _limbIKType;

			private Side _limbIKSide;

			private Bone _beginBone;

			private Bone _bendingBone;

			private Bone _endBone;

			private Effector _bendingEffector;

			private Effector _endEffector;

			private RollBone[] _armRollBones;

			private RollBone[] _elbowRollBones;

			public float _beginToBendingLength;

			public float _beginToBendingLengthSq;

			public float _bendingToEndLength;

			public float _bendingToEndLengthSq;

			private Matrix3x3 _beginToBendingBoneBasis = Matrix3x3.identity;

			private Quaternion _endEffectorToWorldRotation = Quaternion.identity;

			private Matrix3x3 _effectorToBeginBoneBasis = Matrix3x3.identity;

			private float _defaultSinTheta;

			private float _defaultCosTheta = 1f;

			private float _beginToEndMaxLength;

			private CachedScaledValue _effectorMaxLength = CachedScaledValue.zero;

			private CachedScaledValue _effectorMinLength = CachedScaledValue.zero;

			private float _leg_upperLimitNearCircleZ;

			private float _leg_upperLimitNearCircleY;

			private CachedScaledValue _arm_elbowBasisForcefixEffectorLengthBegin = CachedScaledValue.zero;

			private CachedScaledValue _arm_elbowBasisForcefixEffectorLengthEnd = CachedScaledValue.zero;

			private Matrix3x3 _arm_bendingToBeginBoneBasis = Matrix3x3.identity;

			private Quaternion _arm_bendingWorldToBeginBoneRotation = Quaternion.identity;

			private Quaternion _arm_endWorldToBendingBoneRotation = Quaternion.identity;

			private bool _arm_isSolvedLimbIK;

			private Matrix3x3 _arm_solvedBeginBoneBasis = Matrix3x3.identity;

			private Matrix3x3 _arm_solvedBendingBoneBasis = Matrix3x3.identity;

			private bool _isSyncDisplacementAtLeastOnce;

			private float _cache_legUpperLimitAngle;

			private float _cache_kneeUpperLimitAngle;

			private bool _isPresolvedBending;

			private Matrix3x3 _presolvedBendingBasis = Matrix3x3.identity;

			private Vector3 _presolvedEffectorDir = Vector3.zero;

			private float _presolvedEffectorLength;

			private const float _LocalDirMaxTheta = 0.99f;

			private const float _LocalDirLerpTheta = 0.01f;

			private CachedDegreesToCos _presolvedLerpTheta = CachedDegreesToCos.zero;

			private CachedDegreesToCos _automaticKneeBaseTheta = CachedDegreesToCos.zero;

			private CachedDegreesToCosSin _automaticArmElbowTheta = CachedDegreesToCosSin.zero;

			public LimbIK(FullBodyIK fullBodyIK, LimbIKLocation limbIKLocation)
			{
				if (fullBodyIK != null)
				{
					_settings = fullBodyIK.settings;
					_internalValues = fullBodyIK.internalValues;
					_limbIKLocation = limbIKLocation;
					_limbIKType = ToLimbIKType(limbIKLocation);
					_limbIKSide = ToLimbIKSide(limbIKLocation);
					if (_limbIKType == LimbIKType.Leg)
					{
						LegBones legBones = ((_limbIKSide == Side.Left) ? fullBodyIK.leftLegBones : fullBodyIK.rightLegBones);
						LegEffectors legEffectors = ((_limbIKSide == Side.Left) ? fullBodyIK.leftLegEffectors : fullBodyIK.rightLegEffectors);
						_beginBone = legBones.leg;
						_bendingBone = legBones.knee;
						_endBone = legBones.foot;
						_bendingEffector = legEffectors.knee;
						_endEffector = legEffectors.foot;
					}
					else if (_limbIKType == LimbIKType.Arm)
					{
						ArmBones armBones = ((_limbIKSide == Side.Left) ? fullBodyIK.leftArmBones : fullBodyIK.rightArmBones);
						ArmEffectors armEffectors = ((_limbIKSide == Side.Left) ? fullBodyIK.leftArmEffectors : fullBodyIK.rightArmEffectors);
						_beginBone = armBones.arm;
						_bendingBone = armBones.elbow;
						_endBone = armBones.wrist;
						_bendingEffector = armEffectors.elbow;
						_endEffector = armEffectors.wrist;
						_PrepareRollBones(ref _armRollBones, armBones.armRoll);
						_PrepareRollBones(ref _elbowRollBones, armBones.elbowRoll);
					}
					_Prepare(fullBodyIK);
				}
			}

			private void _Prepare(FullBodyIK fullBodyIK)
			{
				SAFBIKQuatMultInv0(out _endEffectorToWorldRotation, ref _endEffector._defaultRotation, ref _endBone._defaultRotation);
				_beginToBendingLength = _bendingBone._defaultLocalLength.length;
				_beginToBendingLengthSq = _bendingBone._defaultLocalLength.lengthSq;
				_bendingToEndLength = _endBone._defaultLocalLength.length;
				_bendingToEndLengthSq = _endBone._defaultLocalLength.lengthSq;
				float lengthSq;
				float lenB = SAFBIKVecLengthAndLengthSq2(out lengthSq, ref _endBone._defaultPosition, ref _beginBone._defaultPosition);
				_defaultCosTheta = ComputeCosTheta(_bendingToEndLengthSq, lengthSq, _beginToBendingLengthSq, lenB, _beginToBendingLength);
				_defaultSinTheta = SAFBIKSqrtClamp01(1f - _defaultCosTheta * _defaultCosTheta);
			}

			private void _SyncDisplacement()
			{
				if (_settings.syncDisplacement != SyncDisplacement.Everyframe && _isSyncDisplacementAtLeastOnce)
				{
					return;
				}
				_isSyncDisplacementAtLeastOnce = true;
				SAFBIKMatMult(out _beginToBendingBoneBasis, ref _beginBone._localAxisBasisInv, ref _bendingBone._localAxisBasis);
				if (_armRollBones != null && _beginBone != null && _bendingBone != null)
				{
					SAFBIKMatMult(out _arm_bendingToBeginBoneBasis, ref _bendingBone._boneToBaseBasis, ref _beginBone._baseToBoneBasis);
					SAFBIKMatMultGetRot(out _arm_bendingWorldToBeginBoneRotation, ref _bendingBone._worldToBaseBasis, ref _beginBone._baseToBoneBasis);
				}
				if (_elbowRollBones != null && _endBone != null && _bendingBone != null)
				{
					SAFBIKMatMultGetRot(out _arm_endWorldToBendingBoneRotation, ref _endBone._worldToBaseBasis, ref _bendingBone._baseToBoneBasis);
				}
				_beginToBendingLength = _bendingBone._defaultLocalLength.length;
				_beginToBendingLengthSq = _bendingBone._defaultLocalLength.lengthSq;
				_bendingToEndLength = _endBone._defaultLocalLength.length;
				_bendingToEndLengthSq = _endBone._defaultLocalLength.lengthSq;
				_beginToEndMaxLength = _beginToBendingLength + _bendingToEndLength;
				Vector3 v = _endBone._defaultPosition - _beginBone._defaultPosition;
				if (SAFBIKVecNormalize(ref v))
				{
					if (_limbIKType == LimbIKType.Arm)
					{
						if (_limbIKSide == Side.Left)
						{
							v = -v;
						}
						Vector3 dirY = _internalValues.defaultRootBasis.column1;
						Vector3 dirZ = _internalValues.defaultRootBasis.column2;
						if (SAFBIKComputeBasisLockX(out _effectorToBeginBoneBasis, ref v, ref dirY, ref dirZ))
						{
							_effectorToBeginBoneBasis = _effectorToBeginBoneBasis.transpose;
						}
					}
					else
					{
						v = -v;
						Vector3 dirX = _internalValues.defaultRootBasis.column0;
						Vector3 dirZ2 = _internalValues.defaultRootBasis.column2;
						if (SAFBIKComputeBasisLockY(out _effectorToBeginBoneBasis, ref dirX, ref v, ref dirZ2))
						{
							_effectorToBeginBoneBasis = _effectorToBeginBoneBasis.transpose;
						}
					}
					SAFBIKMatMultRet0(ref _effectorToBeginBoneBasis, ref _beginBone._localAxisBasis);
				}
				if (_limbIKType == LimbIKType.Leg)
				{
					_leg_upperLimitNearCircleZ = 0f;
					_leg_upperLimitNearCircleY = _beginToEndMaxLength;
				}
				_SyncDisplacement_UpdateArgs();
			}

			private void _UpdateArgs()
			{
				if (_limbIKType == LimbIKType.Leg)
				{
					float legEffectorMinLengthRate = _settings.limbIK.legEffectorMinLengthRate;
					if (_effectorMinLength._b != legEffectorMinLengthRate)
					{
						_effectorMinLength._Reset(_beginToEndMaxLength, legEffectorMinLengthRate);
					}
					if (_cache_kneeUpperLimitAngle != _settings.limbIK.prefixKneeUpperLimitAngle || _cache_legUpperLimitAngle != _settings.limbIK.prefixLegUpperLimitAngle)
					{
						_cache_kneeUpperLimitAngle = _settings.limbIK.prefixKneeUpperLimitAngle;
						_cache_legUpperLimitAngle = _settings.limbIK.prefixLegUpperLimitAngle;
						CachedDegreesToCosSin cachedDegreesToCosSin = new CachedDegreesToCosSin(_settings.limbIK.prefixKneeUpperLimitAngle);
						CachedDegreesToCosSin cachedDegreesToCosSin2 = new CachedDegreesToCosSin(_settings.limbIK.prefixLegUpperLimitAngle);
						_leg_upperLimitNearCircleZ = _beginToBendingLength * cachedDegreesToCosSin2.cos + _bendingToEndLength * cachedDegreesToCosSin.cos;
						_leg_upperLimitNearCircleY = _beginToBendingLength * cachedDegreesToCosSin2.sin + _bendingToEndLength * cachedDegreesToCosSin.sin;
					}
				}
				if (_limbIKType == LimbIKType.Arm)
				{
					float num = _settings.limbIK.armBasisForcefixEffectorLengthRate - _settings.limbIK.armBasisForcefixEffectorLengthLerpRate;
					float armBasisForcefixEffectorLengthRate = _settings.limbIK.armBasisForcefixEffectorLengthRate;
					if (_arm_elbowBasisForcefixEffectorLengthBegin._b != num)
					{
						_arm_elbowBasisForcefixEffectorLengthBegin._Reset(_beginToEndMaxLength, num);
					}
					if (_arm_elbowBasisForcefixEffectorLengthEnd._b != armBasisForcefixEffectorLengthRate)
					{
						_arm_elbowBasisForcefixEffectorLengthEnd._Reset(_beginToEndMaxLength, armBasisForcefixEffectorLengthRate);
					}
				}
				float num2 = ((_limbIKType == LimbIKType.Leg) ? _settings.limbIK.legEffectorMaxLengthRate : _settings.limbIK.armEffectorMaxLengthRate);
				if (_effectorMaxLength._b != num2)
				{
					_effectorMaxLength._Reset(_beginToEndMaxLength, num2);
				}
			}

			private void _SyncDisplacement_UpdateArgs()
			{
				if (_limbIKType == LimbIKType.Leg)
				{
					float legEffectorMinLengthRate = _settings.limbIK.legEffectorMinLengthRate;
					_effectorMinLength._Reset(_beginToEndMaxLength, legEffectorMinLengthRate);
					CachedDegreesToCosSin cachedDegreesToCosSin = new CachedDegreesToCosSin(_settings.limbIK.prefixKneeUpperLimitAngle);
					CachedDegreesToCosSin cachedDegreesToCosSin2 = new CachedDegreesToCosSin(_settings.limbIK.prefixLegUpperLimitAngle);
					_leg_upperLimitNearCircleZ = _beginToBendingLength * cachedDegreesToCosSin2.cos + _bendingToEndLength * cachedDegreesToCosSin.cos;
					_leg_upperLimitNearCircleY = _beginToBendingLength * cachedDegreesToCosSin2.sin + _bendingToEndLength * cachedDegreesToCosSin.sin;
				}
				float b = ((_limbIKType == LimbIKType.Leg) ? _settings.limbIK.legEffectorMaxLengthRate : _settings.limbIK.armEffectorMaxLengthRate);
				_effectorMaxLength._Reset(_beginToEndMaxLength, b);
			}

			private void _SolveBaseBasis(out Matrix3x3 baseBasis, ref Matrix3x3 parentBaseBasis, ref Vector3 effectorDir)
			{
				if (_limbIKType == LimbIKType.Arm)
				{
					Vector3 dirX = ((_limbIKSide == Side.Left) ? (-effectorDir) : effectorDir);
					Vector3 dirY = parentBaseBasis.column1;
					Vector3 dirZ = parentBaseBasis.column2;
					if (SAFBIKComputeBasisLockX(out baseBasis, ref dirX, ref dirY, ref dirZ))
					{
						SAFBIKMatMultRet0(ref baseBasis, ref _effectorToBeginBoneBasis);
					}
					else
					{
						SAFBIKMatMult(out baseBasis, ref parentBaseBasis, ref _beginBone._localAxisBasis);
					}
				}
				else
				{
					Vector3 dirY2 = -effectorDir;
					Vector3 dirX2 = parentBaseBasis.column0;
					Vector3 dirZ2 = parentBaseBasis.column2;
					if (SAFBIKComputeBasisLockY(out baseBasis, ref dirX2, ref dirY2, ref dirZ2))
					{
						SAFBIKMatMultRet0(ref baseBasis, ref _effectorToBeginBoneBasis);
					}
					else
					{
						SAFBIKMatMult(out baseBasis, ref parentBaseBasis, ref _beginBone._localAxisBasis);
					}
				}
			}

			private static void _PrepareRollBones(ref RollBone[] rollBones, Bone[] bones)
			{
				if (bones != null && bones.Length != 0)
				{
					int num = bones.Length;
					float num2 = 1f / (float)(num + 1);
					float num3 = num2;
					rollBones = new RollBone[num];
					int num4 = 0;
					while (num4 < num)
					{
						rollBones[num4].bone = bones[num4];
						rollBones[num4].rate = num3;
						num4++;
						num3 += num2;
					}
				}
				else
				{
					rollBones = null;
				}
			}

			public void PresolveBeinding()
			{
				_SyncDisplacement();
				if (!((_limbIKType == LimbIKType.Leg) ? _settings.limbIK.presolveKneeEnabled : _settings.limbIK.presolveElbowEnabled))
				{
					return;
				}
				_isPresolvedBending = false;
				if (_beginBone == null || !_beginBone.transformIsAlive || _beginBone.parentBone == null || !_beginBone.parentBone.transformIsAlive || _bendingEffector == null || _bendingEffector.bone == null || !_bendingEffector.bone.transformIsAlive || _endEffector == null || _endEffector.bone == null || !_endEffector.bone.transformIsAlive || !_internalValues.animatorEnabled || _bendingEffector.positionEnabled)
				{
					return;
				}
				if (_limbIKType == LimbIKType.Leg)
				{
					if (_settings.limbIK.presolveKneeRate < 1E-07f)
					{
						return;
					}
				}
				else if (_settings.limbIK.presolveElbowRate < 1E-07f)
				{
					return;
				}
				Vector3 worldPosition = _beginBone.worldPosition;
				Vector3 worldPosition2 = _bendingEffector.bone.worldPosition;
				Vector3 vector = _endEffector.bone.worldPosition - worldPosition;
				Vector3 vector2 = worldPosition2 - worldPosition;
				float magnitude = vector.magnitude;
				float magnitude2 = vector2.magnitude;
				if (magnitude <= 1E-07f || magnitude2 <= 1E-07f)
				{
					return;
				}
				Vector3 effectorDir = vector * (1f / magnitude);
				Vector3 vector3 = vector2 * (1f / magnitude2);
				Quaternion lhs = _beginBone.parentBone.worldRotation;
				Matrix3x3 ret;
				SAFBIKMatSetRotMult(out ret, ref lhs, ref _beginBone.parentBone._worldToBaseRotation);
				Matrix3x3 baseBasis;
				_SolveBaseBasis(out baseBasis, ref ret, ref effectorDir);
				_presolvedEffectorDir = effectorDir;
				_presolvedEffectorLength = magnitude;
				Matrix3x3 basis;
				if (_limbIKType == LimbIKType.Arm)
				{
					Vector3 dirX = ((_limbIKSide == Side.Left) ? (-vector3) : vector3);
					Vector3 dirY = ret.column1;
					Vector3 dirZ = ret.column2;
					if (SAFBIKComputeBasisLockX(out basis, ref dirX, ref dirY, ref dirZ))
					{
						SAFBIKMatMultInv1(out _presolvedBendingBasis, ref basis, ref baseBasis);
						_isPresolvedBending = true;
					}
				}
				else
				{
					Vector3 dirY2 = -vector3;
					Vector3 dirX2 = ret.column0;
					Vector3 dirZ2 = ret.column2;
					if (SAFBIKComputeBasisLockY(out basis, ref dirX2, ref dirY2, ref dirZ2))
					{
						SAFBIKMatMultInv1(out _presolvedBendingBasis, ref basis, ref baseBasis);
						_isPresolvedBending = true;
					}
				}
			}

			private bool _PrefixLegEffectorPos_UpperNear(ref Vector3 localEffectorTrans)
			{
				float num = localEffectorTrans.y - _leg_upperLimitNearCircleY;
				float z = localEffectorTrans.z;
				float leg_upperLimitNearCircleZ = _leg_upperLimitNearCircleZ;
				float num2 = _leg_upperLimitNearCircleY + _effectorMinLength.value;
				if (leg_upperLimitNearCircleZ > 1E-07f && num2 > 1E-07f)
				{
					bool flag = false;
					z /= leg_upperLimitNearCircleZ;
					if (num > _leg_upperLimitNearCircleY)
					{
						flag = true;
					}
					else
					{
						num /= num2;
						if (SAFBIKSqrt(num * num + z * z) < 1f)
						{
							flag = true;
						}
					}
					if (flag)
					{
						float num3 = SAFBIKSqrt(1f - z * z);
						if (num3 > 1E-07f)
						{
							localEffectorTrans.y = (0f - num3) * num2 + _leg_upperLimitNearCircleY;
						}
						else
						{
							localEffectorTrans.z = 0f;
							localEffectorTrans.y = 0f - _effectorMinLength.value;
						}
						return true;
					}
				}
				return false;
			}

			private static bool _PrefixLegEffectorPos_Circular_Far(ref Vector3 localEffectorTrans, float effectorLength)
			{
				return _PrefixLegEffectorPos_Circular(ref localEffectorTrans, effectorLength, true);
			}

			private static bool _PrefixLegEffectorPos_Circular(ref Vector3 localEffectorTrans, float effectorLength, bool isFar)
			{
				float y = localEffectorTrans.y;
				float z = localEffectorTrans.z;
				float num = SAFBIKSqrt(y * y + z * z);
				if ((isFar && num > effectorLength) || (!isFar && num < effectorLength))
				{
					float num2 = SAFBIKSqrt(effectorLength * effectorLength - localEffectorTrans.z * localEffectorTrans.z);
					if (num2 > 1E-07f)
					{
						localEffectorTrans.y = 0f - num2;
					}
					else
					{
						localEffectorTrans.z = 0f;
						localEffectorTrans.y = 0f - effectorLength;
					}
					return true;
				}
				return false;
			}

			private static bool _PrefixLegEffectorPos_Upper_Circular_Far(ref Vector3 localEffectorTrans, float centerPositionZ, float effectorLengthZ, float effectorLengthY)
			{
				if (effectorLengthY > 1E-07f && effectorLengthZ > 1E-07f)
				{
					float y = localEffectorTrans.y;
					float num = localEffectorTrans.z - centerPositionZ;
					float num2 = y / effectorLengthY;
					num /= effectorLengthZ;
					if (SAFBIKSqrt(num2 * num2 + num * num) > 1f)
					{
						float num3 = SAFBIKSqrt(1f - num * num);
						if (num3 > 1E-07f)
						{
							localEffectorTrans.y = num3 * effectorLengthY;
						}
						else
						{
							localEffectorTrans.z = centerPositionZ;
							localEffectorTrans.y = effectorLengthY;
						}
						return true;
					}
				}
				return false;
			}

			private static void _ComputeLocalDirXZ(ref Vector3 localDir, out Vector3 localDirXZ)
			{
				if (localDir.y >= 0.9899999f)
				{
					localDirXZ = new Vector3(1f, 0f, 0f);
				}
				else if (localDir.y > 0.9799999f)
				{
					float num = (localDir.y - 0.98f) * 100f;
					localDirXZ = new Vector3(localDir.x + (1f - localDir.x) * num, 0f, localDir.z - localDir.z * num);
					if (!SAFBIKVecNormalizeXZ(ref localDirXZ))
					{
						localDirXZ = new Vector3(1f, 0f, 0f);
					}
				}
				else if (localDir.y <= -0.9899999f)
				{
					localDirXZ = new Vector3(-1f, 0f, 0f);
				}
				else if (localDir.y < -0.9799999f)
				{
					float num2 = (-0.98f - localDir.y) * 100f;
					localDirXZ = new Vector3(localDir.x + (-1f - localDir.x) * num2, 0f, localDir.z - localDir.z * num2);
					if (!SAFBIKVecNormalizeXZ(ref localDirXZ))
					{
						localDirXZ = new Vector3(-1f, 0f, 0f);
					}
				}
				else
				{
					localDirXZ = new Vector3(localDir.x, 0f, localDir.z);
					if (!SAFBIKVecNormalizeXZ(ref localDirXZ))
					{
						localDirXZ = new Vector3(1f, 0f, 0f);
					}
				}
			}

			private static void _ComputeLocalDirYZ(ref Vector3 localDir, out Vector3 localDirYZ)
			{
				if (localDir.x >= 0.9899999f)
				{
					localDirYZ = new Vector3(0f, 0f, -1f);
				}
				else if (localDir.x > 0.9799999f)
				{
					float num = (localDir.x - 0.98f) * 100f;
					localDirYZ = new Vector3(0f, localDir.y - localDir.y * num, localDir.z + (-1f - localDir.z) * num);
					if (!SAFBIKVecNormalizeYZ(ref localDirYZ))
					{
						localDirYZ = new Vector3(0f, 0f, -1f);
					}
				}
				else if (localDir.x <= -0.9899999f)
				{
					localDirYZ = new Vector3(0f, 0f, 1f);
				}
				else if (localDir.x < -0.9799999f)
				{
					float num2 = (-0.98f - localDir.x) * 100f;
					localDirYZ = new Vector3(0f, localDir.y - localDir.y * num2, localDir.z + (1f - localDir.z) * num2);
					if (!SAFBIKVecNormalizeYZ(ref localDirYZ))
					{
						localDirYZ = new Vector3(0f, 0f, 1f);
					}
				}
				else
				{
					localDirYZ = new Vector3(0f, localDir.y, localDir.z);
					if (!SAFBIKVecNormalizeYZ(ref localDirYZ))
					{
						localDirYZ = new Vector3(0f, 0f, (localDir.x >= 0f) ? (-1f) : 1f);
					}
				}
			}

			public bool IsSolverEnabled()
			{
				if (!_endEffector.positionEnabled && (!_bendingEffector.positionEnabled || !(_bendingEffector.pull > 1E-07f)))
				{
					if (_limbIKType == LimbIKType.Arm)
					{
						if (!_settings.limbIK.armAlwaysSolveEnabled)
						{
							return false;
						}
					}
					else if (_limbIKType == LimbIKType.Leg && !_settings.limbIK.legAlwaysSolveEnabled)
					{
						return false;
					}
				}
				return true;
			}

			public bool Presolve(ref Matrix3x3 parentBaseBasis, ref Vector3 beginPos, out Vector3 solvedBeginToBendingDir, out Vector3 solvedBendingToEndDir)
			{
				float effectorLen;
				Matrix3x3 baseBasis;
				return PresolveInternal(ref parentBaseBasis, ref beginPos, out effectorLen, out baseBasis, out solvedBeginToBendingDir, out solvedBendingToEndDir);
			}

			public bool PresolveInternal(ref Matrix3x3 parentBaseBasis, ref Vector3 beginPos, out float effectorLen, out Matrix3x3 baseBasis, out Vector3 solvedBeginToBendingDir, out Vector3 solvedBendingToEndDir)
			{
				solvedBeginToBendingDir = Vector3.zero;
				solvedBendingToEndDir = Vector3.zero;
				Vector3 vector = _bendingEffector._hidden_worldPosition;
				Vector3 vector2 = _endEffector._hidden_worldPosition;
				if (_bendingEffector.positionEnabled && _bendingEffector.pull > 1E-07f)
				{
					Vector3 vector3 = vector - beginPos;
					float sqrMagnitude = vector3.sqrMagnitude;
					if (sqrMagnitude > _bendingBone._defaultLocalLength.length)
					{
						float num = SAFBIKSqrt(sqrMagnitude);
						float num2 = num - _bendingBone._defaultLocalLength.length;
						if (num2 < -1E-07f && num > 1E-07f)
						{
							vector += vector3 * (num2 / num);
						}
					}
				}
				if (_bendingEffector.positionEnabled && _bendingEffector.pull > 1E-07f)
				{
					Vector3 vector4 = vector2 - vector;
					float magnitude = vector4.magnitude;
					if (magnitude > 1E-07f)
					{
						float num3 = _endBone._defaultLocalLength.length - magnitude;
						if (num3 > 1E-07f || num3 < -1E-07f)
						{
							float num4 = ((!_endEffector.positionEnabled || !(_endEffector.pull > 1E-07f)) ? _bendingEffector.pull : (_bendingEffector.pull / (_bendingEffector.pull + _endEffector.pull)));
							vector2 += vector4 * (num3 * num4 / magnitude);
						}
					}
				}
				Matrix3x3 m = parentBaseBasis.transpose;
				Vector3 v = vector2 - beginPos;
				effectorLen = v.magnitude;
				if (effectorLen <= 1E-07f)
				{
					baseBasis = Matrix3x3.identity;
					return false;
				}
				if (_effectorMaxLength.value <= 1E-07f)
				{
					baseBasis = Matrix3x3.identity;
					return false;
				}
				Vector3 v2 = v * (1f / effectorLen);
				if (effectorLen > _effectorMaxLength.value)
				{
					v = v2 * _effectorMaxLength.value;
					vector2 = beginPos + v;
					effectorLen = _effectorMaxLength.value;
				}
				Vector3 ret = new Vector3(0f, 0f, 1f);
				if (_limbIKType == LimbIKType.Arm)
				{
					SAFBIKMatMultVec(out ret, ref m, ref v2);
				}
				if (_limbIKType == LimbIKType.Leg && _settings.limbIK.prefixLegEffectorEnabled)
				{
					Vector3 ret2;
					SAFBIKMatMultVec(out ret2, ref m, ref v);
					bool flag = false;
					bool flag2 = false;
					if (ret2.z >= 0f)
					{
						if (ret2.z >= _beginToBendingLength + _bendingToEndLength)
						{
							flag = true;
							ret2.z = _beginToBendingLength + _bendingToEndLength;
							ret2.y = 0f;
						}
						if (!flag && ret2.y >= 0f - _effectorMinLength.value && ret2.z <= _leg_upperLimitNearCircleZ)
						{
							flag = true;
							flag2 = _PrefixLegEffectorPos_UpperNear(ref ret2);
						}
						if (!flag && ret2.y >= 0f && ret2.z > _leg_upperLimitNearCircleZ)
						{
							flag = true;
							_PrefixLegEffectorPos_Upper_Circular_Far(ref ret2, _leg_upperLimitNearCircleZ, _beginToBendingLength + _bendingToEndLength - _leg_upperLimitNearCircleZ, _leg_upperLimitNearCircleY);
						}
						if (!flag)
						{
							flag = true;
							flag2 = _PrefixLegEffectorPos_Circular_Far(ref ret2, _beginToBendingLength + _bendingToEndLength);
						}
					}
					else if (ret2.y >= 0f - _effectorMinLength.value)
					{
						flag2 = true;
						ret2.y = 0f - _effectorMinLength.value;
					}
					else
					{
						flag2 = _PrefixLegEffectorPos_Circular_Far(ref ret2, _beginToBendingLength + _bendingToEndLength);
					}
					if (flag2)
					{
						SAFBIKMatMultVec(out v, ref parentBaseBasis, ref ret2);
						effectorLen = v.magnitude;
						vector2 = beginPos + v;
						if (effectorLen > 1E-07f)
						{
							v2 = v * (1f / effectorLen);
						}
					}
				}
				_SolveBaseBasis(out baseBasis, ref parentBaseBasis, ref v2);
				if (!_bendingEffector.positionEnabled)
				{
					bool num5 = ((_limbIKType == LimbIKType.Leg) ? _settings.limbIK.presolveKneeEnabled : _settings.limbIK.presolveElbowEnabled);
					float num6 = ((_limbIKType == LimbIKType.Leg) ? _settings.limbIK.presolveKneeRate : _settings.limbIK.presolveElbowRate);
					float num7 = ((_limbIKType == LimbIKType.Leg) ? _settings.limbIK.presolveKneeLerpAngle : _settings.limbIK.presolveElbowLerpAngle);
					float num8 = ((_limbIKType == LimbIKType.Leg) ? _settings.limbIK.presolveKneeLerpLengthRate : _settings.limbIK.presolveElbowLerpLengthRate);
					Vector3 vector5 = Vector3.zero;
					if (num5 && _isPresolvedBending)
					{
						if (_presolvedEffectorLength > 1E-07f)
						{
							float num9 = _presolvedEffectorLength * num8;
							if (num9 > 1E-07f)
							{
								float num10 = Mathf.Abs(_presolvedEffectorLength - effectorLen);
								num6 = ((!(num10 < num9)) ? 0f : (num6 * (1f - num10 / num9)));
							}
							else
							{
								num6 = 0f;
							}
						}
						else
						{
							num6 = 0f;
						}
						if (num6 > 1E-07f)
						{
							if (_presolvedLerpTheta._degrees != num7)
							{
								_presolvedLerpTheta._Reset(num7);
							}
							if (_presolvedLerpTheta.cos < 0.9999999f)
							{
								float num11 = Vector3.Dot(v2, _presolvedEffectorDir);
								if (num11 > _presolvedLerpTheta.cos + 1E-07f)
								{
									float num12 = (num11 - _presolvedLerpTheta.cos) / (1f - _presolvedLerpTheta.cos);
									num6 *= num12;
								}
								else
								{
									num6 = 0f;
								}
							}
							else
							{
								num6 = 0f;
							}
						}
						if (num6 > 1E-07f)
						{
							Matrix3x3 ret3;
							SAFBIKMatMult(out ret3, ref baseBasis, ref _presolvedBendingBasis);
							Vector3 vector6 = ((_limbIKType != LimbIKType.Arm) ? (-ret3.column1) : ((_limbIKSide == Side.Left) ? (-ret3.column0) : ret3.column0));
							vector5 = beginPos + vector6 * _beginToBendingLength;
							vector = vector5;
						}
					}
					else
					{
						num6 = 0f;
					}
					if (num6 < 0.9999999f)
					{
						float num13 = ComputeCosTheta(_bendingToEndLengthSq, effectorLen * effectorLen, _beginToBendingLengthSq, effectorLen, _beginToBendingLength);
						float num14 = SAFBIKSqrtClamp01(1f - num13 * num13);
						float num15 = _beginToBendingLength * (1f - Mathf.Max(_defaultCosTheta - num13, 0f));
						float num16 = _beginToBendingLength * Mathf.Max(num14 - _defaultSinTheta, 0f);
						if (_limbIKType == LimbIKType.Arm)
						{
							Vector3 vector7 = ((_limbIKSide == Side.Left) ? (-baseBasis.column0) : baseBasis.column0);
							float automaticElbowBaseAngle = _settings.limbIK.automaticElbowBaseAngle;
							float automaticElbowLowerAngle = _settings.limbIK.automaticElbowLowerAngle;
							float automaticElbowUpperAngle = _settings.limbIK.automaticElbowUpperAngle;
							float a = automaticElbowBaseAngle;
							Vector3 localDir = ((_limbIKSide == Side.Left) ? ret : new Vector3(0f - ret.x, ret.y, ret.z));
							a = ((!(localDir.y < 0f)) ? Mathf.Lerp(a, automaticElbowUpperAngle, localDir.y) : Mathf.Lerp(a, automaticElbowLowerAngle, 0f - localDir.y));
							if (_settings.limbIK.armEffectorBackfixEnabled)
							{
								float automaticElbowBackUpperAngle = _settings.limbIK.automaticElbowBackUpperAngle;
								float automaticElbowBackLowerAngle = _settings.limbIK.automaticElbowBackLowerAngle;
								float sin = _internalValues.limbIK.armEffectorBackBeginTheta.sin;
								float sin2 = _internalValues.limbIK.armEffectorBackCoreBeginTheta.sin;
								float cos = _internalValues.limbIK.armEffectorBackCoreEndTheta.cos;
								float cos2 = _internalValues.limbIK.armEffectorBackEndTheta.cos;
								float sin3 = _internalValues.limbIK.armEffectorBackCoreUpperTheta.sin;
								float sin4 = _internalValues.limbIK.armEffectorBackCoreLowerTheta.sin;
								Vector3 localDirXZ;
								_ComputeLocalDirXZ(ref localDir, out localDirXZ);
								Vector3 localDirYZ;
								_ComputeLocalDirYZ(ref localDir, out localDirYZ);
								if (localDirXZ.z < sin && localDirXZ.x > cos2)
								{
									float num17;
									if (localDirYZ.y >= sin3)
									{
										num17 = automaticElbowBackUpperAngle;
									}
									else if (localDirYZ.y <= sin4)
									{
										num17 = automaticElbowBackLowerAngle;
									}
									else
									{
										float num18 = sin3 - sin4;
										if (num18 > 1E-07f)
										{
											float t = (localDirYZ.y - sin4) / num18;
											num17 = Mathf.Lerp(automaticElbowBackLowerAngle, automaticElbowBackUpperAngle, t);
										}
										else
										{
											num17 = automaticElbowBackLowerAngle;
										}
									}
									if (localDirXZ.x < cos)
									{
										float num19 = cos - cos2;
										if (num19 > 1E-07f)
										{
											float t2 = (localDirXZ.x - cos2) / num19;
											if (localDirYZ.y <= sin4)
											{
												a = Mathf.Lerp(a, num17, t2);
											}
											else if (localDirYZ.y >= sin3)
											{
												a = Mathf.Lerp(a, num17 - 360f, t2);
											}
											else
											{
												float num20 = Mathf.Lerp(a, num17, t2);
												float num21 = Mathf.Lerp(a, num17 - 360f, t2);
												float num22 = sin3 - sin4;
												if (num22 > 1E-07f)
												{
													float t3 = (localDirYZ.y - sin4) / num22;
													if (num20 - num21 > 180f)
													{
														num21 += 360f;
													}
													a = Mathf.Lerp(num20, num21, t3);
												}
												else
												{
													a = num20;
												}
											}
										}
									}
									else if (localDirXZ.z > sin2)
									{
										float num23 = sin - sin2;
										if (num23 > 1E-07f)
										{
											float t4 = (sin - localDirXZ.z) / num23;
											a = ((!(localDir.y >= 0f)) ? Mathf.Lerp(a, num17 - 360f, t4) : Mathf.Lerp(a, num17, t4));
										}
										else
										{
											a = num17;
										}
									}
									else
									{
										a = num17;
									}
								}
							}
							Vector3 column = parentBaseBasis.column1;
							Vector3 v3 = Vector3.Cross(baseBasis.column0, column);
							column = Vector3.Cross(v3, baseBasis.column0);
							if (!SAFBIKVecNormalize2(ref column, ref v3))
							{
								column = parentBaseBasis.column1;
								v3 = parentBaseBasis.column2;
							}
							if (_automaticArmElbowTheta._degrees != a)
							{
								_automaticArmElbowTheta._Reset(a);
							}
							vector = beginPos + vector7 * num15 + -column * num16 * _automaticArmElbowTheta.cos + -v3 * num16 * _automaticArmElbowTheta.sin;
						}
						else
						{
							float automaticKneeBaseAngle = _settings.limbIK.automaticKneeBaseAngle;
							if (automaticKneeBaseAngle >= -1E-07f && automaticKneeBaseAngle <= 1E-07f)
							{
								vector = beginPos + -baseBasis.column1 * num15 + baseBasis.column2 * num16;
							}
							else
							{
								if (_automaticKneeBaseTheta._degrees != automaticKneeBaseAngle)
								{
									_automaticKneeBaseTheta._Reset(automaticKneeBaseAngle);
								}
								float cos3 = _automaticKneeBaseTheta.cos;
								float num24 = SAFBIKSqrt(1f - cos3 * cos3);
								if (_limbIKSide == Side.Right)
								{
									if (automaticKneeBaseAngle >= 0f)
									{
										num24 = 0f - num24;
									}
								}
								else if (automaticKneeBaseAngle < 0f)
								{
									num24 = 0f - num24;
								}
								vector = beginPos + -baseBasis.column1 * num15 + baseBasis.column0 * num16 * num24 + baseBasis.column2 * num16 * cos3;
							}
						}
					}
					if (num6 > 1E-07f)
					{
						vector = Vector3.Lerp(vector, vector5, num6);
					}
				}
				bool flag3 = false;
				Vector3 vector8 = vector - beginPos;
				Vector3 vector9 = vector8 - v2 * Vector3.Dot(v2, vector8);
				float magnitude2 = vector9.magnitude;
				if (magnitude2 > 1E-07f)
				{
					Vector3 vector10 = vector9 * (1f / magnitude2);
					float num25 = 2f * _beginToBendingLength * effectorLen;
					if (num25 > 1E-07f)
					{
						float num26 = (_beginToBendingLengthSq + effectorLen * effectorLen - _bendingToEndLengthSq) / num25;
						float num27 = SAFBIKSqrtClamp01(1f - num26 * num26);
						Vector3 v4 = v2 * num26 * _beginToBendingLength + vector10 * num27 * _beginToBendingLength;
						Vector3 v5 = vector2 - (beginPos + v4);
						if (SAFBIKVecNormalize2(ref v4, ref v5))
						{
							flag3 = true;
							solvedBeginToBendingDir = v4;
							solvedBendingToEndDir = v5;
						}
					}
				}
				if (flag3 && _limbIKType == LimbIKType.Arm && _settings.limbIK.armEffectorInnerfixEnabled)
				{
					float sin5 = _internalValues.limbIK.elbowFrontInnerLimitTheta.sin;
					float sin6 = _internalValues.limbIK.elbowBackInnerLimitTheta.sin;
					Vector3 ret4;
					SAFBIKMatMultVec(out ret4, ref m, ref solvedBeginToBendingDir);
					bool flag4 = ret4.z < 0f;
					float num28 = (flag4 ? sin6 : sin5);
					if (((_limbIKSide == Side.Left) ? ret4.x : (0f - ret4.x)) > num28)
					{
						ret4.x = ((_limbIKSide == Side.Left) ? num28 : (0f - num28));
						ret4.z = SAFBIKSqrt(1f - (ret4.x * ret4.x + ret4.y * ret4.y));
						if (flag4)
						{
							ret4.z = 0f - ret4.z;
						}
						Vector3 ret5;
						SAFBIKMatMultVec(out ret5, ref parentBaseBasis, ref ret4);
						Vector3 vector11 = beginPos + ret5 * _beginToBendingLength;
						Vector3 v6 = vector2 - vector11;
						if (SAFBIKVecNormalize(ref v6))
						{
							solvedBeginToBendingDir = ret5;
							solvedBendingToEndDir = v6;
							if (_settings.limbIK.armBasisForcefixEnabled)
							{
								effectorLen = (vector2 - beginPos).magnitude;
							}
						}
					}
				}
				if (!flag3)
				{
					Vector3 v7 = vector - beginPos;
					if (SAFBIKVecNormalize(ref v7))
					{
						Vector3 vector12 = beginPos + v7 * _beginToBendingLength;
						Vector3 v8 = vector2 - vector12;
						if (SAFBIKVecNormalize(ref v8))
						{
							flag3 = true;
							solvedBeginToBendingDir = v7;
							solvedBendingToEndDir = v8;
						}
					}
				}
				if (!flag3)
				{
					return false;
				}
				return true;
			}

			public bool Solve()
			{
				_UpdateArgs();
				_arm_isSolvedLimbIK = false;
				Quaternion bendingBonePrevRotation = Quaternion.identity;
				Quaternion endBonePrevRotation = Quaternion.identity;
				if (!_internalValues.resetTransforms)
				{
					float num = (_endEffector.rotationEnabled ? _endEffector.rotationWeight : 0f);
					if (num > 1E-07f && num < 0.9999999f)
					{
						bendingBonePrevRotation = _bendingBone.worldRotation;
						endBonePrevRotation = _endBone.worldRotation;
					}
				}
				bool flag = _SolveInternal();
				flag |= _SolveEndRotation(flag, ref bendingBonePrevRotation, ref endBonePrevRotation);
				return flag | _RollInternal();
			}

			public bool _SolveInternal()
			{
				if (!IsSolverEnabled())
				{
					return false;
				}
				if (_beginBone.parentBone == null || !_beginBone.parentBone.transformIsAlive)
				{
					return false;
				}
				Quaternion lhs = _beginBone.parentBone.worldRotation;
				Matrix3x3 ret;
				SAFBIKMatSetRotMult(out ret, ref lhs, ref _beginBone.parentBone._worldToBaseRotation);
				Vector3 beginPos = _beginBone.worldPosition;
				float effectorLen;
				Matrix3x3 baseBasis;
				Vector3 solvedBeginToBendingDir;
				Vector3 solvedBendingToEndDir;
				if (!PresolveInternal(ref ret, ref beginPos, out effectorLen, out baseBasis, out solvedBeginToBendingDir, out solvedBendingToEndDir))
				{
					return false;
				}
				Matrix3x3 basis = Matrix3x3.identity;
				Matrix3x3 basis2 = Matrix3x3.identity;
				if (_limbIKType == LimbIKType.Arm)
				{
					if (_limbIKSide == Side.Left)
					{
						solvedBeginToBendingDir = -solvedBeginToBendingDir;
						solvedBendingToEndDir = -solvedBendingToEndDir;
					}
					Vector3 dirY = ret.column1;
					Vector3 dirZ = ret.column2;
					if (!SAFBIKComputeBasisLockX(out basis, ref solvedBeginToBendingDir, ref dirY, ref dirZ))
					{
						return false;
					}
					bool armBasisForcefixEnabled = _settings.limbIK.armBasisForcefixEnabled;
					if (armBasisForcefixEnabled && effectorLen > _arm_elbowBasisForcefixEffectorLengthEnd.value)
					{
						SAFBIKMatMultCol1(out dirY, ref basis, ref _beginToBendingBoneBasis);
					}
					else
					{
						dirY = Vector3.Cross(-solvedBeginToBendingDir, solvedBendingToEndDir);
						if (_limbIKSide == Side.Left)
						{
							dirY = -dirY;
						}
						if (armBasisForcefixEnabled && effectorLen > _arm_elbowBasisForcefixEffectorLengthBegin.value)
						{
							float num = _arm_elbowBasisForcefixEffectorLengthEnd.value - _arm_elbowBasisForcefixEffectorLengthBegin.value;
							if (num > 1E-07f)
							{
								float t = (effectorLen - _arm_elbowBasisForcefixEffectorLengthBegin.value) / num;
								Vector3 ret2;
								SAFBIKMatMultCol1(out ret2, ref basis, ref _beginToBendingBoneBasis);
								dirY = Vector3.Lerp(dirY, ret2, t);
							}
						}
					}
					if (!SAFBIKComputeBasisFromXYLockX(out basis2, ref solvedBendingToEndDir, ref dirY))
					{
						return false;
					}
				}
				else
				{
					solvedBeginToBendingDir = -solvedBeginToBendingDir;
					solvedBendingToEndDir = -solvedBendingToEndDir;
					Vector3 dirX = baseBasis.column0;
					Vector3 dirZ2 = baseBasis.column2;
					if (!SAFBIKComputeBasisLockY(out basis, ref dirX, ref solvedBeginToBendingDir, ref dirZ2))
					{
						return false;
					}
					SAFBIKMatMultCol0(out dirX, ref basis, ref _beginToBendingBoneBasis);
					if (!SAFBIKComputeBasisFromXYLockY(out basis2, ref dirX, ref solvedBendingToEndDir))
					{
						return false;
					}
				}
				if (_limbIKType == LimbIKType.Arm)
				{
					_arm_isSolvedLimbIK = true;
					_arm_solvedBeginBoneBasis = basis;
					_arm_solvedBendingBoneBasis = basis2;
				}
				Quaternion ret3;
				SAFBIKMatMultGetRot(out ret3, ref basis, ref _beginBone._boneToWorldBasis);
				_beginBone.worldRotation = ret3;
				SAFBIKMatMultGetRot(out ret3, ref basis2, ref _bendingBone._boneToWorldBasis);
				_bendingBone.worldRotation = ret3;
				return true;
			}

			private bool _SolveEndRotation(bool isSolved, ref Quaternion bendingBonePrevRotation, ref Quaternion endBonePrevRotation)
			{
				float num = (_endEffector.rotationEnabled ? _endEffector.rotationWeight : 0f);
				if (num > 1E-07f)
				{
					Quaternion q = _endEffector.worldRotation;
					Quaternion ret;
					SAFBIKQuatMult(out ret, ref q, ref _endEffectorToWorldRotation);
					if (num < 0.9999999f)
					{
						Quaternion ret2;
						if (_internalValues.resetTransforms)
						{
							Quaternion q2 = _bendingBone.worldRotation;
							SAFBIKQuatMult3(out ret2, ref q2, ref _bendingBone._worldToBaseRotation, ref _endBone._baseToWorldRotation);
						}
						else if (isSolved)
						{
							Quaternion q3 = _bendingBone.worldRotation;
							SAFBIKQuatMultNorm3Inv1(out ret2, ref q3, ref bendingBonePrevRotation, ref endBonePrevRotation);
						}
						else
						{
							ret2 = endBonePrevRotation;
						}
						_endBone.worldRotation = Quaternion.Lerp(ret2, ret, num);
					}
					else
					{
						_endBone.worldRotation = ret;
					}
					_EndRotationLimit();
					return true;
				}
				if (_internalValues.resetTransforms)
				{
					Quaternion q4 = _bendingBone.worldRotation;
					Quaternion ret3;
					SAFBIKQuatMult3(out ret3, ref q4, ref _bendingBone._worldToBaseRotation, ref _endBone._baseToWorldRotation);
					_endBone.worldRotation = ret3;
					return true;
				}
				return false;
			}

			private void _EndRotationLimit()
			{
				if (_limbIKType == LimbIKType.Arm)
				{
					if (!_settings.limbIK.wristLimitEnabled)
					{
						return;
					}
				}
				else if (_limbIKType == LimbIKType.Leg && !_settings.limbIK.footLimitEnabled)
				{
					return;
				}
				Quaternion q = _endBone.worldRotation;
				Quaternion ret;
				SAFBIKQuatMult(out ret, ref q, ref _endBone._worldToBaseRotation);
				q = _bendingBone.worldRotation;
				Quaternion ret2;
				SAFBIKQuatMult(out ret2, ref q, ref _bendingBone._worldToBaseRotation);
				Quaternion ret3;
				SAFBIKQuatMultInv0(out ret3, ref ret2, ref ret);
				if (_limbIKType == LimbIKType.Arm)
				{
					bool flag = false;
					float wristLimitAngle = _settings.limbIK.wristLimitAngle;
					float angle;
					Vector3 axis;
					ret3.ToAngleAxis(out angle, out axis);
					if (angle < 0f - wristLimitAngle)
					{
						angle = 0f - wristLimitAngle;
						flag = true;
					}
					else if (angle > wristLimitAngle)
					{
						angle = wristLimitAngle;
						flag = true;
					}
					if (flag)
					{
						ret3 = Quaternion.AngleAxis(angle, axis);
						SAFBIKQuatMultNorm3(out ret, ref ret2, ref ret3, ref _endBone._baseToWorldRotation);
						_endBone.worldRotation = ret;
					}
				}
				else if (_limbIKType == LimbIKType.Leg)
				{
					Matrix3x3 m;
					SAFBIKMatSetRot(out m, ref ret3);
					Vector3 dir = m.column1;
					Vector3 dir2 = m.column2;
					if ((false | _LimitXZ_Square(ref dir, _internalValues.limbIK.footLimitRollTheta.sin, _internalValues.limbIK.footLimitRollTheta.sin, _internalValues.limbIK.footLimitPitchUpTheta.sin, _internalValues.limbIK.footLimitPitchDownTheta.sin) | _LimitXY_Square(ref dir2, _internalValues.limbIK.footLimitYawTheta.sin, _internalValues.limbIK.footLimitYawTheta.sin, _internalValues.limbIK.footLimitPitchDownTheta.sin, _internalValues.limbIK.footLimitPitchUpTheta.sin)) && SAFBIKComputeBasisFromYZLockZ(out m, ref dir, ref dir2))
					{
						SAFBIKMatGetRot(out ret3, ref m);
						SAFBIKQuatMultNorm3(out ret, ref ret2, ref ret3, ref _endBone._baseToWorldRotation);
						_endBone.worldRotation = ret;
					}
				}
			}

			private bool _RollInternal()
			{
				if (_limbIKType != LimbIKType.Arm || !_settings.rollBonesEnabled)
				{
					return false;
				}
				bool result = false;
				if (_armRollBones != null && _armRollBones.Length != 0)
				{
					int num = _armRollBones.Length;
					Matrix3x3 lhs;
					Matrix3x3 ret;
					if (_arm_isSolvedLimbIK)
					{
						lhs = _arm_solvedBeginBoneBasis;
						SAFBIKMatMult(out ret, ref _arm_solvedBendingBoneBasis, ref _arm_bendingToBeginBoneBasis);
					}
					else
					{
						lhs = new Matrix3x3(_beginBone.worldRotation * _beginBone._worldToBoneRotation);
						ret = new Matrix3x3(_bendingBone.worldRotation * _arm_bendingWorldToBeginBoneRotation);
					}
					Vector3 dirX = lhs.column0;
					Vector3 dirY = ret.column1;
					Vector3 dirZ = ret.column2;
					Matrix3x3 basis;
					if (SAFBIKComputeBasisLockX(out basis, ref dirX, ref dirY, ref dirZ))
					{
						Matrix3x3 ret2;
						SAFBIKMatMult(out ret2, ref lhs, ref _beginBone._boneToBaseBasis);
						Matrix3x3 ret3;
						SAFBIKMatMult(out ret3, ref basis, ref _beginBone._boneToBaseBasis);
						for (int i = 0; i < num; i++)
						{
							if (_armRollBones[i].bone != null && _armRollBones[i].bone.transformIsAlive)
							{
								float rate = _armRollBones[i].rate;
								Matrix3x3 ret4;
								SAFBIKMatFastLerp(out ret4, ref ret2, ref ret3, rate);
								Quaternion ret5;
								SAFBIKMatMultGetRot(out ret5, ref ret4, ref _elbowRollBones[i].bone._baseToWorldBasis);
								_armRollBones[i].bone.worldRotation = ret5;
								result = true;
							}
						}
					}
				}
				if (_elbowRollBones != null && _elbowRollBones.Length != 0)
				{
					int num2 = _elbowRollBones.Length;
					Matrix3x3 lhs2 = ((!_arm_isSolvedLimbIK) ? new Matrix3x3(_bendingBone.worldRotation * _bendingBone._worldToBoneRotation) : _arm_solvedBendingBoneBasis);
					Vector3 column = new Matrix3x3(_endBone.worldRotation * _arm_endWorldToBendingBoneRotation).column2;
					Vector3 column2 = lhs2.column0;
					Vector3 v = Vector3.Cross(column, column2);
					column = Vector3.Cross(column2, v);
					if (SAFBIKVecNormalize2(ref v, ref column))
					{
						Matrix3x3 lhs3 = Matrix3x3.FromColumn(ref column2, ref v, ref column);
						Matrix3x3 ret6;
						SAFBIKMatMult(out ret6, ref lhs2, ref _bendingBone._boneToBaseBasis);
						SAFBIKMatMultRet0(ref lhs3, ref _bendingBone._boneToBaseBasis);
						for (int j = 0; j < num2; j++)
						{
							if (_elbowRollBones[j].bone != null && _elbowRollBones[j].bone.transformIsAlive)
							{
								float rate2 = _elbowRollBones[j].rate;
								Matrix3x3 ret7;
								SAFBIKMatFastLerp(out ret7, ref ret6, ref lhs3, rate2);
								Quaternion ret8;
								SAFBIKMatMultGetRot(out ret8, ref ret7, ref _elbowRollBones[j].bone._baseToWorldBasis);
								_elbowRollBones[j].bone.worldRotation = ret8;
								result = true;
							}
						}
					}
				}
				return result;
			}
		}

		[Serializable]
		[StructLayout(LayoutKind.Sequential, Pack = 4)]
		public struct Float2
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
			public float[] v;
		}

		[Serializable]
		[StructLayout(LayoutKind.Sequential, Pack = 4)]
		public struct Matrix3x3
		{
			public Vector3 column0;

			public Vector3 column1;

			public Vector3 column2;

			public static readonly Matrix3x3 identity = new Matrix3x3(1f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 1f);

			public Vector3 row0
			{
				get
				{
					return new Vector3(column0.x, column1.x, column2.x);
				}
			}

			public Vector3 row1
			{
				get
				{
					return new Vector3(column0.y, column1.y, column2.y);
				}
			}

			public Vector3 row2
			{
				get
				{
					return new Vector3(column0.z, column1.z, column2.z);
				}
			}

			public bool isFuzzyIdentity
			{
				get
				{
					if (column0.x < 0.9999999f || column1.x < -1E-07f || column2.x < -1E-07f || column0.y < -1E-07f || column1.y < 0.9999999f || column2.y < -1E-07f || column0.z < -1E-07f || column1.z < -1E-07f || column2.z < 0.9999999f || column0.x > 1.0000001f || column1.x > 1E-07f || column2.x > 1E-07f || column0.y > 1E-07f || column1.y > 1.0000001f || column2.y > 1E-07f || column0.z > 1E-07f || column1.z > 1E-07f || column2.z > 1.0000001f)
					{
						return false;
					}
					return true;
				}
			}

			public Matrix3x3 transpose
			{
				get
				{
					return new Matrix3x3(column0.x, column0.y, column0.z, column1.x, column1.y, column1.z, column2.x, column2.y, column2.z);
				}
			}

			public Matrix3x3(float _11, float _12, float _13, float _21, float _22, float _23, float _31, float _32, float _33)
			{
				column0 = new Vector3(_11, _21, _31);
				column1 = new Vector3(_12, _22, _32);
				column2 = new Vector3(_13, _23, _33);
			}

			public Matrix3x3(Vector3 axis, float cos)
			{
				SAFBIKMatSetAxisAngle(out this, ref axis, cos);
			}

			public Matrix3x3(ref Vector3 axis, float cos)
			{
				SAFBIKMatSetAxisAngle(out this, ref axis, cos);
			}

			public Matrix3x3(Matrix4x4 m)
			{
				column0 = new Vector3(m.m00, m.m10, m.m20);
				column1 = new Vector3(m.m01, m.m11, m.m21);
				column2 = new Vector3(m.m02, m.m12, m.m22);
			}

			public Matrix3x3(ref Matrix4x4 m)
			{
				column0 = new Vector3(m.m00, m.m10, m.m20);
				column1 = new Vector3(m.m01, m.m11, m.m21);
				column2 = new Vector3(m.m02, m.m12, m.m22);
			}

			public Matrix3x3(Quaternion q)
			{
				SAFBIKMatSetRot(out this, ref q);
			}

			public Matrix3x3(ref Quaternion q)
			{
				SAFBIKMatSetRot(out this, ref q);
			}

			public static Matrix3x3 FromColumn(Vector3 column0, Vector3 column1, Vector3 column2)
			{
				Matrix3x3 result = default(Matrix3x3);
				result.SetColumn(ref column0, ref column1, ref column2);
				return result;
			}

			public static Matrix3x3 FromColumn(ref Vector3 column0, ref Vector3 column1, ref Vector3 column2)
			{
				Matrix3x3 result = default(Matrix3x3);
				result.SetColumn(ref column0, ref column1, ref column2);
				return result;
			}

			public void SetValue(float _11, float _12, float _13, float _21, float _22, float _23, float _31, float _32, float _33)
			{
				column0.x = _11;
				column1.x = _12;
				column2.x = _13;
				column0.y = _21;
				column1.y = _22;
				column2.y = _23;
				column0.z = _31;
				column1.z = _32;
				column2.z = _33;
			}

			public void SetValue(Matrix4x4 m)
			{
				column0.x = m.m00;
				column1.x = m.m01;
				column2.x = m.m02;
				column0.y = m.m10;
				column1.y = m.m11;
				column2.y = m.m12;
				column0.z = m.m20;
				column1.z = m.m21;
				column2.z = m.m22;
			}

			public void SetValue(ref Matrix4x4 m)
			{
				column0.x = m.m00;
				column1.x = m.m01;
				column2.x = m.m02;
				column0.y = m.m10;
				column1.y = m.m11;
				column2.y = m.m12;
				column0.z = m.m20;
				column1.z = m.m21;
				column2.z = m.m22;
			}

			public void SetColumn(Vector3 c0, Vector3 c1, Vector3 c2)
			{
				column0 = c0;
				column1 = c1;
				column2 = c2;
			}

			public void SetColumn(ref Vector3 c0, ref Vector3 c1, ref Vector3 c2)
			{
				column0 = c0;
				column1 = c1;
				column2 = c2;
			}

			public static implicit operator Matrix4x4(Matrix3x3 m)
			{
				Matrix4x4 result = Matrix4x4.identity;
				result.m00 = m.column0.x;
				result.m01 = m.column1.x;
				result.m02 = m.column2.x;
				result.m10 = m.column0.y;
				result.m11 = m.column1.y;
				result.m12 = m.column2.y;
				result.m20 = m.column0.z;
				result.m21 = m.column1.z;
				result.m22 = m.column2.z;
				return result;
			}

			public static implicit operator Matrix3x3(Matrix4x4 m)
			{
				return new Matrix3x3(ref m);
			}

			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(row0.ToString());
				stringBuilder.Append(" : ");
				stringBuilder.Append(row1.ToString());
				stringBuilder.Append(" : ");
				stringBuilder.Append(row2.ToString());
				return stringBuilder.ToString();
			}

			public string ToString(string format)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(row0.ToString(format));
				stringBuilder.Append(" : ");
				stringBuilder.Append(row1.ToString(format));
				stringBuilder.Append(" : ");
				stringBuilder.Append(row2.ToString(format));
				return stringBuilder.ToString();
			}

			public string ToStringColumn()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(column0.ToString());
				stringBuilder.Append("(");
				stringBuilder.Append(column0.magnitude);
				stringBuilder.Append(") : ");
				stringBuilder.Append(column1.ToString());
				stringBuilder.Append("(");
				stringBuilder.Append(column1.magnitude);
				stringBuilder.Append(") : ");
				stringBuilder.Append(column2.ToString());
				stringBuilder.Append("(");
				stringBuilder.Append(column2.magnitude);
				stringBuilder.Append(")");
				return stringBuilder.ToString();
			}

			public string ToStringColumn(string format)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(column0.ToString(format));
				stringBuilder.Append("(");
				stringBuilder.Append(column0.magnitude);
				stringBuilder.Append(") : ");
				stringBuilder.Append(column1.ToString(format));
				stringBuilder.Append("(");
				stringBuilder.Append(column1.magnitude);
				stringBuilder.Append(") : ");
				stringBuilder.Append(column2.ToString(format));
				stringBuilder.Append("(");
				stringBuilder.Append(column2.magnitude);
				stringBuilder.Append(")");
				return stringBuilder.ToString();
			}

			public bool Normalize()
			{
				float num = SAFBIKSqrt(column0.x * column0.x + column0.y * column0.y + column0.z * column0.z);
				float num2 = SAFBIKSqrt(column1.x * column1.x + column1.y * column1.y + column1.z * column1.z);
				float num3 = SAFBIKSqrt(column2.x * column2.x + column2.y * column2.y + column2.z * column2.z);
				bool result = true;
				if (num > 1E-07f)
				{
					num = 1f / num;
					column0.x *= num;
					column0.y *= num;
					column0.z *= num;
				}
				else
				{
					result = false;
					column0.x = 1f;
					column0.y = 0f;
					column0.z = 0f;
				}
				if (num2 > 1E-07f)
				{
					num2 = 1f / num2;
					column1.x *= num2;
					column1.y *= num2;
					column1.z *= num2;
				}
				else
				{
					result = false;
					column1.x = 0f;
					column1.y = 1f;
					column1.z = 0f;
				}
				if (num3 > 1E-07f)
				{
					num3 = 1f / num3;
					column2.x *= num3;
					column2.y *= num3;
					column2.z *= num3;
				}
				else
				{
					result = false;
					column2.x = 0f;
					column2.y = 0f;
					column2.z = 1f;
				}
				return result;
			}
		}

		[Serializable]
		public struct Matrix3x4
		{
			public Matrix3x3 basis;

			public Vector3 origin;

			public static readonly Matrix3x4 identity = new Matrix3x4(Matrix3x3.identity, Vector3.zero);

			public Matrix3x4 inverse
			{
				get
				{
					Matrix3x3 m = basis.transpose;
					Vector3 v = -origin;
					Vector3 ret;
					SAFBIKMatMultVec(out ret, ref m, ref v);
					return new Matrix3x4(ref m, ref ret);
				}
			}

			public Matrix3x4(Matrix3x3 _basis, Vector3 _origin)
			{
				basis = _basis;
				origin = _origin;
			}

			public Matrix3x4(ref Matrix3x3 _basis, ref Vector3 _origin)
			{
				basis = _basis;
				origin = _origin;
			}

			public Matrix3x4(Quaternion _q, Vector3 _origin)
			{
				SAFBIKMatSetRot(out basis, ref _q);
				origin = _origin;
			}

			public Matrix3x4(ref Quaternion _q, ref Vector3 _origin)
			{
				SAFBIKMatSetRot(out basis, ref _q);
				origin = _origin;
			}

			public Matrix3x4(Matrix4x4 m)
			{
				basis = new Matrix3x3(ref m);
				origin = new Vector3(m.m03, m.m13, m.m23);
			}

			public Matrix3x4(ref Matrix4x4 m)
			{
				basis = new Matrix3x3(ref m);
				origin = new Vector3(m.m03, m.m13, m.m23);
			}

			public static implicit operator Matrix4x4(Matrix3x4 t)
			{
				Matrix4x4 result = Matrix4x4.identity;
				result.m00 = t.basis.column0.x;
				result.m01 = t.basis.column1.x;
				result.m02 = t.basis.column2.x;
				result.m10 = t.basis.column0.y;
				result.m11 = t.basis.column1.y;
				result.m12 = t.basis.column2.y;
				result.m20 = t.basis.column0.z;
				result.m21 = t.basis.column1.z;
				result.m22 = t.basis.column2.z;
				result.m03 = t.origin.x;
				result.m13 = t.origin.y;
				result.m23 = t.origin.z;
				return result;
			}

			public static implicit operator Matrix3x4(Matrix4x4 m)
			{
				return new Matrix3x4(ref m);
			}

			public Vector3 Multiply(Vector3 v)
			{
				Vector3 ret;
				SAFBIKMatMultVecAdd(out ret, ref basis, ref v, ref origin);
				return ret;
			}

			public Vector3 Multiply(ref Vector3 v)
			{
				Vector3 ret;
				SAFBIKMatMultVecAdd(out ret, ref basis, ref v, ref origin);
				return ret;
			}

			public static Vector3 operator *(Matrix3x4 t, Vector3 v)
			{
				Vector3 ret;
				SAFBIKMatMultVecAdd(out ret, ref t.basis, ref v, ref t.origin);
				return ret;
			}

			public Matrix3x4 Multiply(Matrix3x4 t)
			{
				Matrix3x3 ret;
				SAFBIKMatMult(out ret, ref basis, ref t.basis);
				Vector3 ret2;
				SAFBIKMatMultVecAdd(out ret2, ref basis, ref t.origin, ref origin);
				return new Matrix3x4(ref ret, ref ret2);
			}

			public Matrix3x4 Multiply(ref Matrix3x4 t)
			{
				Matrix3x3 ret;
				SAFBIKMatMult(out ret, ref basis, ref t.basis);
				Vector3 ret2;
				SAFBIKMatMultVecAdd(out ret2, ref basis, ref t.origin, ref origin);
				return new Matrix3x4(ref ret, ref ret2);
			}

			public static Matrix3x4 operator *(Matrix3x4 t1, Matrix3x4 t2)
			{
				Matrix3x3 ret;
				SAFBIKMatMult(out ret, ref t1.basis, ref t2.basis);
				Vector3 ret2;
				SAFBIKMatMultVecAdd(out ret2, ref t1.basis, ref t2.origin, ref t1.origin);
				return new Matrix3x4(ref ret, ref ret2);
			}

			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("basis: ");
				stringBuilder.Append(basis.ToString());
				stringBuilder.Append(" origin: ");
				stringBuilder.Append(origin.ToString());
				return stringBuilder.ToString();
			}

			public string ToString(string format)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("basis: ");
				stringBuilder.Append(basis.ToString(format));
				stringBuilder.Append(" origin: ");
				stringBuilder.Append(origin.ToString(format));
				return stringBuilder.ToString();
			}
		}

		[Serializable]
		public struct FastLength
		{
			public float length;

			public float lengthSq;

			private FastLength(float length_)
			{
				length = length_;
				lengthSq = length_ * length_;
			}

			private FastLength(float length_, float lengthSq_)
			{
				length = length_;
				lengthSq = lengthSq_;
			}

			public static FastLength FromLength(float length)
			{
				return new FastLength(length);
			}

			public static FastLength FromLengthSq(float lengthSq)
			{
				return new FastLength(SAFBIKSqrt(lengthSq), lengthSq);
			}

			public static FastLength FromVector3(Vector3 v)
			{
				float lengthSq_;
				return new FastLength(SAFBIKVecLengthAndLengthSq(out lengthSq_, ref v), lengthSq_);
			}

			public static FastLength FromVector3(Vector3 v0, Vector3 v1)
			{
				float lengthSq_;
				return new FastLength(SAFBIKVecLengthAndLengthSq2(out lengthSq_, ref v0, ref v1), lengthSq_);
			}

			public static FastLength FromVector3(ref Vector3 v)
			{
				float lengthSq_;
				return new FastLength(SAFBIKVecLengthAndLengthSq(out lengthSq_, ref v), lengthSq_);
			}

			public static FastLength FromVector3(ref Vector3 v0, ref Vector3 v1)
			{
				float lengthSq_;
				return new FastLength(SAFBIKVecLengthAndLengthSq2(out lengthSq_, ref v0, ref v1), lengthSq_);
			}
		}

		[Serializable]
		public struct FastAngle
		{
			public float angle;

			public float cos;

			public float sin;

			public static readonly FastAngle zero = new FastAngle(0f, 1f, 0f);

			public FastAngle(float angle_)
			{
				angle = angle_;
				cos = (float)Math.Cos(angle_);
				sin = (float)Math.Sin(angle_);
			}

			public FastAngle(float angle_, float cos_, float sin_)
			{
				angle = angle_;
				cos = cos_;
				sin = sin_;
			}

			public void Reset()
			{
				angle = 0f;
				cos = 1f;
				sin = 0f;
			}

			public void Reset(float angle_)
			{
				angle = angle_;
				cos = (float)Math.Cos(angle_);
				sin = (float)Math.Sin(angle_);
			}

			public void Reset(float angle_, float cos_, float sin_)
			{
				angle = angle_;
				cos = cos_;
				sin = sin_;
			}
		}

		public struct CachedRate01
		{
			public float _value;

			public float value;

			public bool isGreater0;

			public bool isLess1;

			public static readonly CachedRate01 zero = new CachedRate01(0f);

			public CachedRate01(float v)
			{
				_value = v;
				value = Mathf.Clamp01(v);
				isGreater0 = value > 1E-07f;
				isLess1 = value < 0.9999999f;
			}

			public void _Reset(float v)
			{
				_value = v;
				value = Mathf.Clamp01(v);
				isGreater0 = value > 1E-07f;
				isLess1 = value < 0.9999999f;
			}
		}

		public struct CachedDegreesToSin
		{
			public float _degrees;

			public float sin;

			public static readonly CachedDegreesToSin zero = new CachedDegreesToSin(0f, 0f);

			public CachedDegreesToSin(float degrees)
			{
				_degrees = degrees;
				sin = (float)Math.Sin(degrees * ((float)Math.PI / 180f));
			}

			public CachedDegreesToSin(float degrees, float sin_)
			{
				_degrees = degrees;
				sin = sin_;
			}

			public void _Reset(float degrees)
			{
				_degrees = degrees;
				sin = (float)Math.Sin(degrees * ((float)Math.PI / 180f));
			}
		}

		public struct CachedDegreesToCos
		{
			public float _degrees;

			public float cos;

			public static readonly CachedDegreesToCos zero = new CachedDegreesToCos(0f, 1f);

			public CachedDegreesToCos(float degrees)
			{
				_degrees = degrees;
				cos = (float)Math.Cos(degrees * ((float)Math.PI / 180f));
			}

			public CachedDegreesToCos(float degrees, float cos_)
			{
				_degrees = degrees;
				cos = cos_;
			}

			public void _Reset(float degrees)
			{
				_degrees = degrees;
				cos = (float)Math.Cos(degrees * ((float)Math.PI / 180f));
			}
		}

		public struct CachedDegreesToCosSin
		{
			public float _degrees;

			public float cos;

			public float sin;

			public static readonly CachedDegreesToCosSin zero = new CachedDegreesToCosSin(0f, 1f, 0f);

			public CachedDegreesToCosSin(float degrees)
			{
				_degrees = degrees;
				cos = (float)Math.Cos(degrees * ((float)Math.PI / 180f));
				sin = (float)Math.Sin(degrees * ((float)Math.PI / 180f));
			}

			public CachedDegreesToCosSin(float degrees, float cos_, float sin_)
			{
				_degrees = degrees;
				cos = cos_;
				sin = sin_;
			}

			public void _Reset(float degrees)
			{
				_degrees = degrees;
				cos = (float)Math.Cos(degrees * ((float)Math.PI / 180f));
				sin = (float)Math.Sin(degrees * ((float)Math.PI / 180f));
			}
		}

		public struct CachedScaledValue
		{
			public float _a;

			public float _b;

			public float value;

			public static readonly CachedScaledValue zero = new CachedScaledValue(0f, 0f, 0f);

			public CachedScaledValue(float a, float b)
			{
				_a = a;
				_b = b;
				value = a * b;
			}

			public CachedScaledValue(float a, float b, float value_)
			{
				_a = a;
				_b = b;
				value = value_;
			}

			public void Reset(float a, float b)
			{
				if (_a != a || _b != b)
				{
					_a = a;
					_b = b;
					value = a * b;
				}
			}

			public void _Reset(float a, float b)
			{
				_a = a;
				_b = b;
				value = a * b;
			}
		}

		public struct CachedDeg2RadScaledValue
		{
			public float _a;

			public float _b;

			public float value;

			public static readonly CachedDeg2RadScaledValue zero = new CachedDeg2RadScaledValue(0f, 0f, 0f);

			public CachedDeg2RadScaledValue(float a, float b)
			{
				_a = a;
				_b = b;
				value = a * b * ((float)Math.PI / 180f);
			}

			public CachedDeg2RadScaledValue(float a, float b, float value_)
			{
				_a = a;
				_b = b;
				value = value_;
			}

			public void Reset(float a, float b)
			{
				if (_a != a || _b != b)
				{
					_a = a;
					_b = b;
					value = a * b * ((float)Math.PI / 180f);
				}
			}

			public void _Reset(float a, float b)
			{
				_a = a;
				_b = b;
				value = a * b * ((float)Math.PI / 180f);
			}
		}

		public enum EyesType
		{
			Normal = 0,
			LegacyMove = 1
		}

		public enum Side
		{
			Left = 0,
			Right = 1,
			Max = 2,
			None = 2
		}

		public enum LimbIKType
		{
			Leg = 0,
			Arm = 1,
			Max = 2,
			Unknown = 2
		}

		public enum LimbIKLocation
		{
			LeftLeg = 0,
			RightLeg = 1,
			LeftArm = 2,
			RightArm = 3,
			Max = 4,
			Unknown = 4
		}

		public enum FingerIKType
		{
			LeftWrist = 0,
			RightWrist = 1,
			Max = 2,
			None = 2
		}

		public enum BoneType
		{
			Hips = 0,
			Spine = 1,
			Neck = 2,
			Head = 3,
			Eye = 4,
			Leg = 5,
			Knee = 6,
			Foot = 7,
			Shoulder = 8,
			Arm = 9,
			ArmRoll = 10,
			Elbow = 11,
			ElbowRoll = 12,
			Wrist = 13,
			HandFinger = 14,
			Max = 15,
			Unknown = 15
		}

		public enum BoneLocation
		{
			Hips = 0,
			Spine = 1,
			Spine2 = 2,
			Spine3 = 3,
			Spine4 = 4,
			Neck = 5,
			Head = 6,
			LeftEye = 7,
			RightEye = 8,
			LeftLeg = 9,
			RightLeg = 10,
			LeftKnee = 11,
			RightKnee = 12,
			LeftFoot = 13,
			RightFoot = 14,
			LeftShoulder = 15,
			RightShoulder = 16,
			LeftArm = 17,
			RightArm = 18,
			LeftArmRoll0 = 19,
			LeftArmRoll1 = 20,
			LeftArmRoll2 = 21,
			LeftArmRoll3 = 22,
			RightArmRoll0 = 23,
			RightArmRoll1 = 24,
			RightArmRoll2 = 25,
			RightArmRoll3 = 26,
			LeftElbow = 27,
			RightElbow = 28,
			LeftElbowRoll0 = 29,
			LeftElbowRoll1 = 30,
			LeftElbowRoll2 = 31,
			LeftElbowRoll3 = 32,
			RightElbowRoll0 = 33,
			RightElbowRoll1 = 34,
			RightElbowRoll2 = 35,
			RightElbowRoll3 = 36,
			LeftWrist = 37,
			RightWrist = 38,
			LeftHandThumb0 = 39,
			LeftHandThumb1 = 40,
			LeftHandThumb2 = 41,
			LeftHandThumbTip = 42,
			LeftHandIndex0 = 43,
			LeftHandIndex1 = 44,
			LeftHandIndex2 = 45,
			LeftHandIndexTip = 46,
			LeftHandMiddle0 = 47,
			LeftHandMiddle1 = 48,
			LeftHandMiddle2 = 49,
			LeftHandMiddleTip = 50,
			LeftHandRing0 = 51,
			LeftHandRing1 = 52,
			LeftHandRing2 = 53,
			LeftHandRingTip = 54,
			LeftHandLittle0 = 55,
			LeftHandLittle1 = 56,
			LeftHandLittle2 = 57,
			LeftHandLittleTip = 58,
			RightHandThumb0 = 59,
			RightHandThumb1 = 60,
			RightHandThumb2 = 61,
			RightHandThumbTip = 62,
			RightHandIndex0 = 63,
			RightHandIndex1 = 64,
			RightHandIndex2 = 65,
			RightHandIndexTip = 66,
			RightHandMiddle0 = 67,
			RightHandMiddle1 = 68,
			RightHandMiddle2 = 69,
			RightHandMiddleTip = 70,
			RightHandRing0 = 71,
			RightHandRing1 = 72,
			RightHandRing2 = 73,
			RightHandRingTip = 74,
			RightHandLittle0 = 75,
			RightHandLittle1 = 76,
			RightHandLittle2 = 77,
			RightHandLittleTip = 78,
			Max = 79,
			Unknown = 79,
			SpineU = 4
		}

		public enum EffectorType
		{
			Root = 0,
			Hips = 1,
			Neck = 2,
			Head = 3,
			Eyes = 4,
			Knee = 5,
			Foot = 6,
			Arm = 7,
			Elbow = 8,
			Wrist = 9,
			HandFinger = 10,
			Max = 11,
			Unknown = 11
		}

		public enum EffectorLocation
		{
			Root = 0,
			Hips = 1,
			Neck = 2,
			Head = 3,
			Eyes = 4,
			LeftKnee = 5,
			RightKnee = 6,
			LeftFoot = 7,
			RightFoot = 8,
			LeftArm = 9,
			RightArm = 10,
			LeftElbow = 11,
			RightElbow = 12,
			LeftWrist = 13,
			RightWrist = 14,
			LeftHandThumb = 15,
			LeftHandIndex = 16,
			LeftHandMiddle = 17,
			LeftHandRing = 18,
			LeftHandLittle = 19,
			RightHandThumb = 20,
			RightHandIndex = 21,
			RightHandMiddle = 22,
			RightHandRing = 23,
			RightHandLittle = 24,
			Max = 25,
			Unknown = 25
		}

		public enum FingerType
		{
			Thumb = 0,
			Index = 1,
			Middle = 2,
			Ring = 3,
			Little = 4,
			Max = 5,
			Unknown = 5
		}

		public enum _DirectionAs
		{
			None = 0,
			XPlus = 1,
			XMinus = 2,
			YPlus = 3,
			YMinus = 4,
			Max = 5,
			Uknown = 5
		}

		public Transform rootTransform;

		[NonSerialized]
		public InternalValues internalValues = new InternalValues();

		[NonSerialized]
		public BoneCaches boneCaches = new BoneCaches();

		public Settings settings;

		public EditorSettings editorSettings;

		public BodyBones bodyBones;

		public HeadBones headBones;

		public LegBones leftLegBones;

		public LegBones rightLegBones;

		public ArmBones leftArmBones;

		public ArmBones rightArmBones;

		public FingersBones leftHandFingersBones;

		public FingersBones rightHandFingersBones;

		public Effector rootEffector;

		public BodyEffectors bodyEffectors;

		public HeadEffectors headEffectors;

		public LegEffectors leftLegEffectors;

		public LegEffectors rightLegEffectors;

		public ArmEffectors leftArmEffectors;

		public ArmEffectors rightArmEffectors;

		public FingersEffectors leftHandFingersEffectors;

		public FingersEffectors rightHandFingersEffectors;

		private Bone[] _bones = new Bone[15];

		private Effector[] _effectors = new Effector[25];

		private BodyIK _bodyIK;

		private LimbIK[] _limbIK = new LimbIK[4];

		private HeadIK _headIK;

		private FingerIK[] _fingerIK = new FingerIK[2];

		private bool _isNeedFixShoulderWorldTransform;

		private bool _isPrefixed;

		private bool _isPrepared;

		[SerializeField]
		private bool _isPrefixedAtLeastOnce;

		private static readonly string[] _LeftKeywords = new string[2] { "left", "_l" };

		private static readonly string[] _RightKeywords = new string[2] { "right", "_r" };

		private bool _isAnimatorCheckedAtLeastOnce;

		private bool _isSyncDisplacementAtLeastOnce;

		public const float FLOAT_EPSILON = float.Epsilon;

		public const float IKEpsilon = 1E-07f;

		public const float IKMoveEpsilon = 1E-05f;

		public const float IKWritebackEpsilon = 0.01f;

		public const int MaxArmRollLength = 4;

		public const int MaxElbowRollLength = 4;

		public const int MaxHandFingerLength = 4;

		public const float Eyes_DefaultDistance = 1f;

		public const float Eyes_MinDistance = 0.5f;

		public const float SimualteEys_NeckHeadDistanceScale = 1f;

		public Bone[] bones
		{
			get
			{
				return _bones;
			}
		}

		public Effector[] effectors
		{
			get
			{
				return _effectors;
			}
		}

		public void Awake(Transform rootTransorm_)
		{
			if (rootTransform != rootTransorm_)
			{
				rootTransform = rootTransorm_;
			}
			_Prefix();
			ConfigureBoneTransforms();
			Prepare();
		}

		public void Destroy()
		{
		}

		private static void _SetBoneTransform(ref Bone bone, Transform transform)
		{
			if (bone == null)
			{
				bone = new Bone();
			}
			bone.transform = transform;
		}

		private static void _SetFingerBoneTransform(ref Bone[] bones, Transform[,] transforms, int index)
		{
			if (bones == null || bones.Length != 4)
			{
				bones = new Bone[4];
			}
			for (int i = 0; i != 4; i++)
			{
				if (bones[i] == null)
				{
					bones[i] = new Bone();
				}
				bones[i].transform = transforms[index, i];
			}
		}

		private static bool _IsSpine(Transform trn)
		{
			if (trn != null)
			{
				string name = trn.name;
				if (name.Contains("Spine") || name.Contains("spine") || name.Contains("SPINE"))
				{
					return true;
				}
				if (name.Contains("Torso") || name.Contains("torso") || name.Contains("TORSO"))
				{
					return true;
				}
			}
			return false;
		}

		private static bool _IsNeck(Transform trn)
		{
			if (trn != null)
			{
				string name = trn.name;
				if (name != null)
				{
					if (name.Contains("Neck") || name.Contains("neck") || name.Contains("NECK"))
					{
						return true;
					}
					if (name.Contains("Kubi") || name.Contains("kubi") || name.Contains("KUBI"))
					{
						return true;
					}
					if (name.Contains("くび"))
					{
						return true;
					}
					if (name.Contains("クビ"))
					{
						return true;
					}
					if (name.Contains("首"))
					{
						return true;
					}
				}
			}
			return false;
		}

		public void Prefix(Transform rootTransform_)
		{
			if (rootTransform != rootTransform_)
			{
				rootTransform = rootTransform_;
			}
			_Prefix();
		}

		private void _Prefix()
		{
			if (_isPrefixed)
			{
				return;
			}
			_isPrefixed = true;
			SafeNew(ref bodyBones);
			SafeNew(ref headBones);
			SafeNew(ref leftLegBones);
			SafeNew(ref rightLegBones);
			SafeNew(ref leftArmBones);
			leftArmBones.Repair();
			SafeNew(ref rightArmBones);
			rightArmBones.Repair();
			SafeNew(ref leftHandFingersBones);
			leftHandFingersBones.Repair();
			SafeNew(ref rightHandFingersBones);
			rightHandFingersBones.Repair();
			SafeNew(ref bodyEffectors);
			SafeNew(ref headEffectors);
			SafeNew(ref leftArmEffectors);
			SafeNew(ref rightArmEffectors);
			SafeNew(ref leftLegEffectors);
			SafeNew(ref rightLegEffectors);
			SafeNew(ref leftHandFingersEffectors);
			SafeNew(ref rightHandFingersEffectors);
			SafeNew(ref settings);
			SafeNew(ref editorSettings);
			SafeNew(ref internalValues);
			settings.Prefix();
			if (_bones == null || _bones.Length != 79)
			{
				_bones = new Bone[79];
			}
			if (_effectors == null || _effectors.Length != 25)
			{
				_effectors = new Effector[25];
			}
			_Prefix(ref bodyBones.hips, BoneLocation.Hips, null);
			_Prefix(ref bodyBones.spine, BoneLocation.Spine, bodyBones.hips);
			_Prefix(ref bodyBones.spine2, BoneLocation.Spine2, bodyBones.spine);
			_Prefix(ref bodyBones.spine3, BoneLocation.Spine3, bodyBones.spine2);
			_Prefix(ref bodyBones.spine4, BoneLocation.Spine4, bodyBones.spine3);
			_Prefix(ref headBones.neck, BoneLocation.Neck, bodyBones.spineU);
			_Prefix(ref headBones.head, BoneLocation.Head, headBones.neck);
			_Prefix(ref headBones.leftEye, BoneLocation.LeftEye, headBones.head);
			_Prefix(ref headBones.rightEye, BoneLocation.RightEye, headBones.head);
			for (int i = 0; i != 2; i++)
			{
				LegBones legBones = ((i == 0) ? leftLegBones : rightLegBones);
				_Prefix(ref legBones.leg, (i == 0) ? BoneLocation.LeftLeg : BoneLocation.RightLeg, bodyBones.hips);
				_Prefix(ref legBones.knee, (i == 0) ? BoneLocation.LeftKnee : BoneLocation.RightKnee, legBones.leg);
				_Prefix(ref legBones.foot, (i == 0) ? BoneLocation.LeftFoot : BoneLocation.RightFoot, legBones.knee);
				ArmBones armBones = ((i == 0) ? leftArmBones : rightArmBones);
				_Prefix(ref armBones.shoulder, (i == 0) ? BoneLocation.LeftShoulder : BoneLocation.RightShoulder, bodyBones.spineU);
				_Prefix(ref armBones.arm, (i == 0) ? BoneLocation.LeftArm : BoneLocation.RightArm, armBones.shoulder);
				_Prefix(ref armBones.elbow, (i == 0) ? BoneLocation.LeftElbow : BoneLocation.RightElbow, armBones.arm);
				_Prefix(ref armBones.wrist, (i == 0) ? BoneLocation.LeftWrist : BoneLocation.RightWrist, armBones.elbow);
				for (int j = 0; j != 4; j++)
				{
					BoneLocation boneLocation = ((i == 0) ? BoneLocation.LeftArmRoll0 : BoneLocation.RightArmRoll0);
					_Prefix(ref armBones.armRoll[j], boneLocation + j, armBones.arm);
				}
				for (int k = 0; k != 4; k++)
				{
					BoneLocation boneLocation2 = ((i == 0) ? BoneLocation.LeftElbowRoll0 : BoneLocation.RightElbowRoll0);
					_Prefix(ref armBones.elbowRoll[k], boneLocation2 + k, armBones.elbow);
				}
				FingersBones fingersBones = ((i == 0) ? leftHandFingersBones : rightHandFingersBones);
				for (int l = 0; l != 4; l++)
				{
					BoneLocation boneLocation3 = ((i == 0) ? BoneLocation.LeftHandThumb0 : BoneLocation.RightHandThumb0);
					BoneLocation boneLocation4 = ((i == 0) ? BoneLocation.LeftHandIndex0 : BoneLocation.RightHandIndex0);
					BoneLocation boneLocation5 = ((i == 0) ? BoneLocation.LeftHandMiddle0 : BoneLocation.RightHandMiddle0);
					BoneLocation boneLocation6 = ((i == 0) ? BoneLocation.LeftHandRing0 : BoneLocation.RightHandRing0);
					BoneLocation boneLocation7 = ((i == 0) ? BoneLocation.LeftHandLittle0 : BoneLocation.RightHandLittle0);
					_Prefix(ref fingersBones.thumb[l], boneLocation3 + l, (l == 0) ? armBones.wrist : fingersBones.thumb[l - 1]);
					_Prefix(ref fingersBones.index[l], boneLocation4 + l, (l == 0) ? armBones.wrist : fingersBones.index[l - 1]);
					_Prefix(ref fingersBones.middle[l], boneLocation5 + l, (l == 0) ? armBones.wrist : fingersBones.middle[l - 1]);
					_Prefix(ref fingersBones.ring[l], boneLocation6 + l, (l == 0) ? armBones.wrist : fingersBones.ring[l - 1]);
					_Prefix(ref fingersBones.little[l], boneLocation7 + l, (l == 0) ? armBones.wrist : fingersBones.little[l - 1]);
				}
			}
			_Prefix(ref rootEffector, EffectorLocation.Root);
			_Prefix(ref bodyEffectors.hips, EffectorLocation.Hips, rootEffector, bodyBones.hips, leftLegBones.leg, rightLegBones.leg);
			_Prefix(ref headEffectors.neck, EffectorLocation.Neck, bodyEffectors.hips, headBones.neck);
			_Prefix(ref headEffectors.head, EffectorLocation.Head, headEffectors.neck, headBones.head);
			_Prefix(ref headEffectors.eyes, EffectorLocation.Eyes, rootEffector, headBones.head, headBones.leftEye, headBones.rightEye);
			_Prefix(ref leftLegEffectors.knee, EffectorLocation.LeftKnee, rootEffector, leftLegBones.knee);
			_Prefix(ref leftLegEffectors.foot, EffectorLocation.LeftFoot, rootEffector, leftLegBones.foot);
			_Prefix(ref rightLegEffectors.knee, EffectorLocation.RightKnee, rootEffector, rightLegBones.knee);
			_Prefix(ref rightLegEffectors.foot, EffectorLocation.RightFoot, rootEffector, rightLegBones.foot);
			_Prefix(ref leftArmEffectors.arm, EffectorLocation.LeftArm, bodyEffectors.hips, leftArmBones.arm);
			_Prefix(ref leftArmEffectors.elbow, EffectorLocation.LeftElbow, bodyEffectors.hips, leftArmBones.elbow);
			_Prefix(ref leftArmEffectors.wrist, EffectorLocation.LeftWrist, bodyEffectors.hips, leftArmBones.wrist);
			_Prefix(ref rightArmEffectors.arm, EffectorLocation.RightArm, bodyEffectors.hips, rightArmBones.arm);
			_Prefix(ref rightArmEffectors.elbow, EffectorLocation.RightElbow, bodyEffectors.hips, rightArmBones.elbow);
			_Prefix(ref rightArmEffectors.wrist, EffectorLocation.RightWrist, bodyEffectors.hips, rightArmBones.wrist);
			_Prefix(ref leftHandFingersEffectors.thumb, EffectorLocation.LeftHandThumb, leftArmEffectors.wrist, leftHandFingersBones.thumb);
			_Prefix(ref leftHandFingersEffectors.index, EffectorLocation.LeftHandIndex, leftArmEffectors.wrist, leftHandFingersBones.index);
			_Prefix(ref leftHandFingersEffectors.middle, EffectorLocation.LeftHandMiddle, leftArmEffectors.wrist, leftHandFingersBones.middle);
			_Prefix(ref leftHandFingersEffectors.ring, EffectorLocation.LeftHandRing, leftArmEffectors.wrist, leftHandFingersBones.ring);
			_Prefix(ref leftHandFingersEffectors.little, EffectorLocation.LeftHandLittle, leftArmEffectors.wrist, leftHandFingersBones.little);
			_Prefix(ref rightHandFingersEffectors.thumb, EffectorLocation.RightHandThumb, rightArmEffectors.wrist, rightHandFingersBones.thumb);
			_Prefix(ref rightHandFingersEffectors.index, EffectorLocation.RightHandIndex, rightArmEffectors.wrist, rightHandFingersBones.index);
			_Prefix(ref rightHandFingersEffectors.middle, EffectorLocation.RightHandMiddle, rightArmEffectors.wrist, rightHandFingersBones.middle);
			_Prefix(ref rightHandFingersEffectors.ring, EffectorLocation.RightHandRing, rightArmEffectors.wrist, rightHandFingersBones.ring);
			_Prefix(ref rightHandFingersEffectors.little, EffectorLocation.RightHandLittle, rightArmEffectors.wrist, rightHandFingersBones.little);
			if (!_isPrefixedAtLeastOnce)
			{
				_isPrefixedAtLeastOnce = true;
				for (int m = 0; m != _effectors.Length; m++)
				{
					_effectors[m].Prefix();
				}
			}
		}

		public void CleanupBoneTransforms()
		{
			_Prefix();
			if (_bones == null)
			{
				return;
			}
			for (int i = 0; i < _bones.Length; i++)
			{
				if (_bones[i] != null)
				{
					_bones[i].transform = null;
				}
			}
		}

		private static Transform _FindEye(Transform head, bool isRight)
		{
			if (head != null)
			{
				string[] array = (isRight ? _RightKeywords : _LeftKeywords);
				int childCount = head.childCount;
				for (int i = 0; i < childCount; i++)
				{
					Transform child = head.GetChild(i);
					if (!(child != null))
					{
						continue;
					}
					string name = child.name;
					if (name == null)
					{
						continue;
					}
					name = name.ToLower();
					if (name == null || !name.Contains("eye"))
					{
						continue;
					}
					for (int j = 0; j < array.Length; j++)
					{
						if (name.Contains(array[j]))
						{
							return child;
						}
					}
				}
			}
			return null;
		}

		public void ConfigureBoneTransforms()
		{
			_Prefix();
			if (settings.automaticPrepareHumanoid && rootTransform != null)
			{
				Animator component = rootTransform.GetComponent<Animator>();
				if (component != null && component.isHuman)
				{
					Transform boneTransform = component.GetBoneTransform(HumanBodyBones.Hips);
					Transform boneTransform2 = component.GetBoneTransform(HumanBodyBones.Spine);
					Transform transform = component.GetBoneTransform(HumanBodyBones.Chest);
					Transform transform2 = null;
					Transform transform3 = null;
					Transform transform4 = component.GetBoneTransform(HumanBodyBones.Neck);
					Transform boneTransform3 = component.GetBoneTransform(HumanBodyBones.Head);
					Transform transform5 = component.GetBoneTransform(HumanBodyBones.LeftEye);
					Transform transform6 = component.GetBoneTransform(HumanBodyBones.RightEye);
					Transform boneTransform4 = component.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
					Transform boneTransform5 = component.GetBoneTransform(HumanBodyBones.RightUpperLeg);
					Transform boneTransform6 = component.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
					Transform boneTransform7 = component.GetBoneTransform(HumanBodyBones.RightLowerLeg);
					Transform boneTransform8 = component.GetBoneTransform(HumanBodyBones.LeftFoot);
					Transform boneTransform9 = component.GetBoneTransform(HumanBodyBones.RightFoot);
					Transform boneTransform10 = component.GetBoneTransform(HumanBodyBones.LeftShoulder);
					Transform boneTransform11 = component.GetBoneTransform(HumanBodyBones.RightShoulder);
					Transform boneTransform12 = component.GetBoneTransform(HumanBodyBones.LeftUpperArm);
					Transform boneTransform13 = component.GetBoneTransform(HumanBodyBones.RightUpperArm);
					Transform boneTransform14 = component.GetBoneTransform(HumanBodyBones.LeftLowerArm);
					Transform boneTransform15 = component.GetBoneTransform(HumanBodyBones.RightLowerArm);
					Transform boneTransform16 = component.GetBoneTransform(HumanBodyBones.LeftHand);
					Transform boneTransform17 = component.GetBoneTransform(HumanBodyBones.RightHand);
					Transform[,] array = new Transform[5, 4];
					Transform[,] array2 = new Transform[5, 4];
					for (int i = 0; i != 2; i++)
					{
						int num = ((i == 0) ? 24 : 39);
						Transform[,] array3 = ((i == 0) ? array : array2);
						for (int j = 0; j != 5; j++)
						{
							int num2 = 0;
							while (num2 != 3)
							{
								array3[j, num2] = component.GetBoneTransform((HumanBodyBones)num);
								num2++;
								num++;
							}
							if (array3[j, 2] != null && array3[j, 2].childCount != 0)
							{
								array3[j, 3] = array3[j, 2].GetChild(0);
							}
						}
					}
					if (transform4 == null && boneTransform3 != null)
					{
						Transform parent = boneTransform3.parent;
						transform4 = ((!(parent != null) || !_IsNeck(parent)) ? boneTransform3 : parent);
					}
					if (transform5 == null)
					{
						transform5 = _FindEye(boneTransform3, false);
					}
					if (transform6 == null)
					{
						transform6 = _FindEye(boneTransform3, true);
					}
					if (settings.automaticConfigureSpineEnabled && boneTransform2 != null && transform4 != null)
					{
						List<Transform> list = new List<Transform>();
						Transform parent2 = transform4.parent;
						while (parent2 != null && parent2 != boneTransform2)
						{
							if (_IsSpine(parent2))
							{
								list.Insert(0, parent2);
							}
							parent2 = parent2.parent;
						}
						list.Insert(0, boneTransform2);
						int num3 = 4;
						if (list.Count > num3)
						{
							list.RemoveRange(num3, list.Count - num3);
						}
						transform = ((list.Count >= 2) ? list[1] : null);
						transform2 = ((list.Count >= 3) ? list[2] : null);
						transform3 = ((list.Count >= 4) ? list[3] : null);
					}
					_SetBoneTransform(ref bodyBones.hips, boneTransform);
					_SetBoneTransform(ref bodyBones.spine, boneTransform2);
					_SetBoneTransform(ref bodyBones.spine2, transform);
					_SetBoneTransform(ref bodyBones.spine3, transform2);
					_SetBoneTransform(ref bodyBones.spine4, transform3);
					_SetBoneTransform(ref headBones.neck, transform4);
					_SetBoneTransform(ref headBones.head, boneTransform3);
					_SetBoneTransform(ref headBones.leftEye, transform5);
					_SetBoneTransform(ref headBones.rightEye, transform6);
					_SetBoneTransform(ref leftLegBones.leg, boneTransform4);
					_SetBoneTransform(ref leftLegBones.knee, boneTransform6);
					_SetBoneTransform(ref leftLegBones.foot, boneTransform8);
					_SetBoneTransform(ref rightLegBones.leg, boneTransform5);
					_SetBoneTransform(ref rightLegBones.knee, boneTransform7);
					_SetBoneTransform(ref rightLegBones.foot, boneTransform9);
					_SetBoneTransform(ref leftArmBones.shoulder, boneTransform10);
					_SetBoneTransform(ref leftArmBones.arm, boneTransform12);
					_SetBoneTransform(ref leftArmBones.elbow, boneTransform14);
					_SetBoneTransform(ref leftArmBones.wrist, boneTransform16);
					_SetBoneTransform(ref rightArmBones.shoulder, boneTransform11);
					_SetBoneTransform(ref rightArmBones.arm, boneTransform13);
					_SetBoneTransform(ref rightArmBones.elbow, boneTransform15);
					_SetBoneTransform(ref rightArmBones.wrist, boneTransform17);
					_SetFingerBoneTransform(ref leftHandFingersBones.thumb, array, 0);
					_SetFingerBoneTransform(ref leftHandFingersBones.index, array, 1);
					_SetFingerBoneTransform(ref leftHandFingersBones.middle, array, 2);
					_SetFingerBoneTransform(ref leftHandFingersBones.ring, array, 3);
					_SetFingerBoneTransform(ref leftHandFingersBones.little, array, 4);
					_SetFingerBoneTransform(ref rightHandFingersBones.thumb, array2, 0);
					_SetFingerBoneTransform(ref rightHandFingersBones.index, array2, 1);
					_SetFingerBoneTransform(ref rightHandFingersBones.middle, array2, 2);
					_SetFingerBoneTransform(ref rightHandFingersBones.ring, array2, 3);
					_SetFingerBoneTransform(ref rightHandFingersBones.little, array2, 4);
				}
			}
			if (!settings.automaticConfigureRollBonesEnabled)
			{
				return;
			}
			List<Transform> tempBones = new List<Transform>();
			int num4 = 0;
			while (true)
			{
				ArmBones armBones;
				switch (num4)
				{
				default:
					armBones = rightArmBones;
					break;
				case 0:
					armBones = leftArmBones;
					break;
				case 2:
					return;
				}
				ArmBones armBones2 = armBones;
				if (armBones2 != null && armBones2.arm != null && armBones2.arm.transform != null && armBones2.elbow != null && armBones2.elbow.transform != null && armBones2.wrist != null && armBones2.wrist.transform != null)
				{
					_ConfigureRollBones(armBones2.armRoll, tempBones, armBones2.arm.transform, armBones2.elbow.transform, (Side)num4, true);
					_ConfigureRollBones(armBones2.elbowRoll, tempBones, armBones2.elbow.transform, armBones2.wrist.transform, (Side)num4, false);
				}
				num4++;
			}
		}

		private void _ConfigureRollBones(Bone[] bones, List<Transform> tempBones, Transform transform, Transform excludeTransform, Side side, bool isArm)
		{
			bool flag = false;
			string text = null;
			text = ((!isArm) ? ((side == Side.Left) ? "LeftElbowRoll" : "RightElbowRoll") : ((side == Side.Left) ? "LeftArmRoll" : "RightArmRoll"));
			int childCount = transform.childCount;
			for (int i = 0; i != childCount; i++)
			{
				string name = transform.GetChild(i).name;
				if (name != null && name.Contains(text))
				{
					flag = true;
					break;
				}
			}
			tempBones.Clear();
			for (int j = 0; j != childCount; j++)
			{
				Transform child = transform.GetChild(j);
				string name2 = child.name;
				if (name2 == null || !(excludeTransform != child) || excludeTransform.IsChildOf(child))
				{
					continue;
				}
				if (flag)
				{
					if (name2.Contains(text))
					{
						char c = name2[name2.Length - 1];
						if (c >= '0' && c <= '9')
						{
							tempBones.Add(child);
						}
					}
				}
				else
				{
					tempBones.Add(child);
				}
			}
			childCount = Mathf.Min(tempBones.Count, bones.Length);
			for (int k = 0; k != childCount; k++)
			{
				_SetBoneTransform(ref bones[k], tempBones[k]);
			}
		}

		public bool Prepare()
		{
			_Prefix();
			if (_isPrepared)
			{
				return false;
			}
			_isPrepared = true;
			if (rootTransform != null)
			{
				internalValues.defaultRootPosition = rootTransform.position;
				internalValues.defaultRootBasis = Matrix3x3.FromColumn(rootTransform.right, rootTransform.up, rootTransform.forward);
				internalValues.defaultRootBasisInv = internalValues.defaultRootBasis.transpose;
				internalValues.defaultRootRotation = rootTransform.rotation;
			}
			if (_bones != null)
			{
				int num = _bones.Length;
				for (int i = 0; i != num; i++)
				{
					if (_bones[i] != null)
					{
						_bones[i].Prepare(this);
					}
				}
				for (int j = 0; j != num; j++)
				{
					if (_bones[j] != null)
					{
						_bones[j].PostPrepare();
					}
				}
			}
			boneCaches.Prepare(this);
			if (_effectors != null)
			{
				int num2 = _effectors.Length;
				for (int k = 0; k != num2; k++)
				{
					if (_effectors[k] != null)
					{
						_effectors[k].Prepare(this);
					}
				}
			}
			if (_limbIK == null || _limbIK.Length != 4)
			{
				_limbIK = new LimbIK[4];
			}
			for (int l = 0; l != 4; l++)
			{
				_limbIK[l] = new LimbIK(this, (LimbIKLocation)l);
			}
			_bodyIK = new BodyIK(this, _limbIK);
			_headIK = new HeadIK(this);
			if (_fingerIK == null || _fingerIK.Length != 2)
			{
				_fingerIK = new FingerIK[2];
			}
			for (int m = 0; m != 2; m++)
			{
				_fingerIK[m] = new FingerIK(this, (FingerIKType)m);
			}
			Bone neck = headBones.neck;
			Bone shoulder = leftArmBones.shoulder;
			Bone shoulder2 = rightArmBones.shoulder;
			if (shoulder != null && shoulder.transformIsAlive && shoulder2 != null && shoulder2.transformIsAlive && neck != null && neck.transformIsAlive && shoulder.transform.parent == neck.transform && shoulder2.transform.parent == neck.transform)
			{
				_isNeedFixShoulderWorldTransform = true;
			}
			return true;
		}

		private void _UpdateInternalValues()
		{
			if (settings.animatorEnabled == AutomaticBool.Auto)
			{
				if (!_isAnimatorCheckedAtLeastOnce)
				{
					_isAnimatorCheckedAtLeastOnce = true;
					internalValues.animatorEnabled = false;
					if (rootTransform != null)
					{
						Animator component = rootTransform.GetComponent<Animator>();
						if (component != null && component.enabled)
						{
							RuntimeAnimatorController runtimeAnimatorController = component.runtimeAnimatorController;
							internalValues.animatorEnabled = runtimeAnimatorController != null;
						}
						if (component == null)
						{
							Animation component2 = rootTransform.GetComponent<Animation>();
							if (component2 != null && component2.enabled && component2.GetClipCount() > 0)
							{
								internalValues.animatorEnabled = true;
							}
						}
					}
				}
			}
			else
			{
				internalValues.animatorEnabled = settings.animatorEnabled != AutomaticBool.Disable;
				_isAnimatorCheckedAtLeastOnce = false;
			}
			if (settings.resetTransforms == AutomaticBool.Auto)
			{
				internalValues.resetTransforms = !internalValues.animatorEnabled;
			}
			else
			{
				internalValues.resetTransforms = settings.resetTransforms != AutomaticBool.Disable;
			}
			internalValues.continuousSolverEnabled = !internalValues.animatorEnabled && !internalValues.resetTransforms;
			internalValues.bodyIK.Update(settings.bodyIK);
			internalValues.limbIK.Update(settings.limbIK);
			internalValues.headIK.Update(settings.headIK);
		}

		private void _Bones_SyncDisplacement()
		{
			if (settings.syncDisplacement == SyncDisplacement.Disable || (settings.syncDisplacement != SyncDisplacement.Everyframe && _isSyncDisplacementAtLeastOnce))
			{
				return;
			}
			_isSyncDisplacementAtLeastOnce = true;
			if (_bones != null)
			{
				int num = _bones.Length;
				for (int i = 0; i != num; i++)
				{
					if (_bones[i] != null)
					{
						_bones[i].SyncDisplacement();
					}
				}
				boneCaches._SyncDisplacement(this);
				for (int j = 0; j != num; j++)
				{
					if (_bones[j] != null)
					{
						_bones[j].PostSyncDisplacement(this);
					}
				}
				for (int k = 0; k != num; k++)
				{
					if (_bones[k] != null)
					{
						_bones[k].PostPrepare();
					}
				}
			}
			if (_effectors == null)
			{
				return;
			}
			int num2 = _effectors.Length;
			for (int l = 0; l != num2; l++)
			{
				if (_effectors[l] != null)
				{
					_effectors[l]._ComputeDefaultTransform(this);
				}
			}
		}

		private void _ComputeBaseHipsTransform()
		{
			if (bodyEffectors == null)
			{
				return;
			}
			Effector hips = bodyEffectors.hips;
			if (hips == null || rootEffector == null)
			{
				return;
			}
			if (hips.rotationEnabled && hips.rotationWeight > 1E-07f)
			{
				Quaternion q = hips.worldRotation * Inverse(hips._defaultRotation);
				if (hips.rotationWeight < 0.9999999f)
				{
					Quaternion q2 = Quaternion.Lerp(rootEffector.worldRotation * Inverse(rootEffector._defaultRotation), q, hips.rotationWeight);
					SAFBIKMatSetRot(out internalValues.baseHipsBasis, ref q2);
				}
				else
				{
					SAFBIKMatSetRot(out internalValues.baseHipsBasis, ref q);
				}
			}
			else
			{
				Quaternion lhs = rootEffector.worldRotation;
				SAFBIKMatSetRotMultInv1(out internalValues.baseHipsBasis, ref lhs, ref rootEffector._defaultRotation);
			}
			if (hips.positionEnabled && hips.positionWeight > 1E-07f)
			{
				Vector3 addVec = hips.worldPosition;
				SAFBIKMatMultVecPreSubAdd(out internalValues.baseHipsPos, ref internalValues.baseHipsBasis, ref rootEffector._defaultPosition, ref hips._defaultPosition, ref addVec);
				if (hips.positionWeight < 0.9999999f)
				{
					Vector3 addVec2 = rootEffector.worldPosition;
					Vector3 ret;
					SAFBIKMatMultVecPreSubAdd(out ret, ref internalValues.baseHipsBasis, ref hips._defaultPosition, ref rootEffector._defaultPosition, ref addVec2);
					internalValues.baseHipsPos = Vector3.Lerp(ret, internalValues.baseHipsPos, hips.positionWeight);
				}
			}
			else
			{
				Vector3 addVec3 = rootEffector.worldPosition;
				SAFBIKMatMultVecPreSubAdd(out internalValues.baseHipsPos, ref internalValues.baseHipsBasis, ref hips._defaultPosition, ref rootEffector._defaultPosition, ref addVec3);
			}
		}

		public void Update()
		{
			_UpdateInternalValues();
			if (_effectors != null)
			{
				int num = _effectors.Length;
				for (int i = 0; i != num; i++)
				{
					if (_effectors[i] != null)
					{
						_effectors[i].PrepareUpdate();
					}
				}
			}
			_Bones_PrepareUpdate();
			_Bones_SyncDisplacement();
			if (internalValues.resetTransforms || internalValues.continuousSolverEnabled)
			{
				_ComputeBaseHipsTransform();
			}
			if (_effectors != null)
			{
				int num2 = _effectors.Length;
				for (int j = 0; j != num2; j++)
				{
					Effector effector = _effectors[j];
					if (effector == null || effector.effectorType == EffectorType.Eyes || effector.effectorType == EffectorType.HandFinger)
					{
						continue;
					}
					float num3 = (effector.positionEnabled ? effector.positionWeight : 0f);
					Vector3 vector = ((num3 > 1E-07f) ? effector.worldPosition : default(Vector3));
					if (num3 < 0.9999999f)
					{
						Vector3 ret = vector;
						if (!internalValues.animatorEnabled && (internalValues.resetTransforms || internalValues.continuousSolverEnabled))
						{
							if (effector.effectorLocation == EffectorLocation.Hips)
							{
								ret = internalValues.baseHipsPos;
							}
							else
							{
								Effector effector2 = ((bodyEffectors != null) ? bodyEffectors.hips : null);
								if (effector2 != null)
								{
									SAFBIKMatMultVecPreSubAdd(out ret, ref internalValues.baseHipsBasis, ref effector._defaultPosition, ref effector2._defaultPosition, ref internalValues.baseHipsPos);
								}
							}
						}
						else if (effector.bone != null && effector.bone.transformIsAlive)
						{
							ret = effector.bone.worldPosition;
						}
						if (num3 > 1E-07f)
						{
							effector._hidden_worldPosition = Vector3.Lerp(ret, vector, num3);
						}
						else
						{
							effector._hidden_worldPosition = ret;
						}
					}
					else
					{
						effector._hidden_worldPosition = vector;
					}
				}
			}
			if (_limbIK != null)
			{
				int num4 = _limbIK.Length;
				for (int k = 0; k != num4; k++)
				{
					if (_limbIK[k] != null)
					{
						_limbIK[k].PresolveBeinding();
					}
				}
			}
			if (_bodyIK != null && _bodyIK.Solve())
			{
				_Bones_WriteToTransform();
			}
			if (_limbIK != null || _headIK != null)
			{
				_Bones_PrepareUpdate();
				bool flag = false;
				bool flag2 = false;
				if (_limbIK != null)
				{
					int num5 = _limbIK.Length;
					for (int l = 0; l != num5; l++)
					{
						if (_limbIK[l] != null)
						{
							flag |= _limbIK[l].Solve();
						}
					}
				}
				if (_headIK != null)
				{
					flag2 = _headIK.Solve(this);
					flag = flag || flag2;
				}
				if (flag2 && _isNeedFixShoulderWorldTransform)
				{
					if (leftArmBones.shoulder != null)
					{
						leftArmBones.shoulder.forcefix_worldRotation();
					}
					if (rightArmBones.shoulder != null)
					{
						rightArmBones.shoulder.forcefix_worldRotation();
					}
				}
				if (flag)
				{
					_Bones_WriteToTransform();
				}
			}
			if (_fingerIK == null)
			{
				return;
			}
			_Bones_PrepareUpdate();
			bool flag3 = false;
			int num6 = _fingerIK.Length;
			for (int m = 0; m != num6; m++)
			{
				if (_fingerIK[m] != null)
				{
					flag3 |= _fingerIK[m].Solve();
				}
			}
			if (flag3)
			{
				_Bones_WriteToTransform();
			}
		}

		private void _Bones_PrepareUpdate()
		{
			if (_bones == null)
			{
				return;
			}
			int num = _bones.Length;
			for (int i = 0; i != num; i++)
			{
				if (_bones[i] != null)
				{
					_bones[i].PrepareUpdate();
				}
			}
		}

		private void _Bones_WriteToTransform()
		{
			if (_bones == null)
			{
				return;
			}
			int num = _bones.Length;
			for (int i = 0; i != num; i++)
			{
				if (_bones[i] != null)
				{
					_bones[i].WriteToTransform();
				}
			}
		}

		private void _Prefix(ref Bone bone, BoneLocation boneLocation, Bone parentBoneLocationBased)
		{
			Bone.Prefix(_bones, ref bone, boneLocation, parentBoneLocationBased);
		}

		private void _Prefix(ref Effector effector, EffectorLocation effectorLocation)
		{
			bool createEffectorTransform = settings.createEffectorTransform;
			Effector.Prefix(_effectors, ref effector, effectorLocation, createEffectorTransform, rootTransform);
		}

		private void _Prefix(ref Effector effector, EffectorLocation effectorLocation, Effector parentEffector, Bone[] bones)
		{
			_Prefix(ref effector, effectorLocation, parentEffector, (bones != null && bones.Length != 0) ? bones[bones.Length - 1] : null);
		}

		private void _Prefix(ref Effector effector, EffectorLocation effectorLocation, Effector parentEffector, Bone bone, Bone leftBone = null, Bone rightBone = null)
		{
			bool createEffectorTransform = settings.createEffectorTransform;
			Effector.Prefix(_effectors, ref effector, effectorLocation, createEffectorTransform, null, parentEffector, bone, leftBone, rightBone);
		}

		public virtual bool _IsHiddenCustomEyes()
		{
			return false;
		}

		public virtual bool _PrepareCustomEyes(ref Quaternion headToLeftEyeRotation, ref Quaternion headToRightEyeRotation)
		{
			return false;
		}

		public virtual void _ResetCustomEyes()
		{
		}

		public virtual void _SolveCustomEyes(ref Matrix3x3 neckBasis, ref Matrix3x3 headBasis, ref Matrix3x3 headBaseBasis)
		{
		}

		public static float SAFBIKSqrt(float a)
		{
			if (a <= float.Epsilon)
			{
				return 0f;
			}
			return (float)Math.Sqrt(a);
		}

		public static float SAFBIKSqrtClamp01(float a)
		{
			if (a <= float.Epsilon)
			{
				return 0f;
			}
			if (a >= 1f)
			{
				return 1f;
			}
			return (float)Math.Sqrt(a);
		}

		public static float SAFBIKSin(float a)
		{
			return (float)Math.Sin(a);
		}

		public static float SAFBIKCos(float a)
		{
			return (float)Math.Cos(a);
		}

		public static void SAFBIKCosSin(out float cos, out float sin, float a)
		{
			cos = (float)Math.Cos(a);
			sin = (float)Math.Sin(a);
		}

		public static float SAFBIKTan(float a)
		{
			return (float)Math.Tan(a);
		}

		public static float SAFBIKAcos(float cos)
		{
			if (cos >= 1f)
			{
				return 0f;
			}
			if (cos <= -1f)
			{
				return (float)Math.PI;
			}
			return (float)Math.Acos(cos);
		}

		public static float SAFBIKAsin(float sin)
		{
			if (sin >= 1f)
			{
				return (float)Math.PI / 2f;
			}
			if (sin <= -1f)
			{
				return -(float)Math.PI / 2f;
			}
			return (float)Math.Asin(sin);
		}

		public static void SAFBIKVecCross(out Vector3 ret, ref Vector3 lhs, ref Vector3 rhs)
		{
			ret = new Vector3(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);
		}

		public static float SAFBIKVecLength(ref Vector3 v)
		{
			float num = v.x * v.x + v.y * v.y + v.z * v.z;
			if (num > float.Epsilon)
			{
				return (float)Math.Sqrt(num);
			}
			return 0f;
		}

		public static float SAFBIKVecLengthAndLengthSq(out float lengthSq, ref Vector3 v)
		{
			lengthSq = v.x * v.x + v.y * v.y + v.z * v.z;
			if (lengthSq > float.Epsilon)
			{
				return (float)Math.Sqrt(lengthSq);
			}
			return 0f;
		}

		public static float SAFBIKVecLength2(ref Vector3 lhs, ref Vector3 rhs)
		{
			float num = lhs.x - rhs.x;
			float num2 = lhs.y - rhs.y;
			float num3 = lhs.z - rhs.z;
			float num4 = num * num + num2 * num2 + num3 * num3;
			if (num4 > float.Epsilon)
			{
				return (float)Math.Sqrt(num4);
			}
			return 0f;
		}

		public static float SAFBIKVecLengthSq2(ref Vector3 lhs, ref Vector3 rhs)
		{
			float num = lhs.x - rhs.x;
			float num2 = lhs.y - rhs.y;
			float num3 = lhs.z - rhs.z;
			return num * num + num2 * num2 + num3 * num3;
		}

		public static float SAFBIKVecLengthAndLengthSq2(out float lengthSq, ref Vector3 lhs, ref Vector3 rhs)
		{
			float num = lhs.x - rhs.x;
			float num2 = lhs.y - rhs.y;
			float num3 = lhs.z - rhs.z;
			lengthSq = num * num + num2 * num2 + num3 * num3;
			if (lengthSq > float.Epsilon)
			{
				return (float)Math.Sqrt(lengthSq);
			}
			return 0f;
		}

		public static bool SAFBIKVecNormalize(ref Vector3 v0)
		{
			float num = v0.x * v0.x + v0.y * v0.y + v0.z * v0.z;
			if (num > float.Epsilon)
			{
				float num2 = (float)Math.Sqrt(num);
				if (num2 > 1E-07f)
				{
					num2 = 1f / num2;
					v0.x *= num2;
					v0.y *= num2;
					v0.z *= num2;
					return true;
				}
			}
			v0.x = (v0.y = (v0.z = 0f));
			return false;
		}

		public static bool SAFBIKVecNormalizeXZ(ref Vector3 v0)
		{
			float num = v0.x * v0.x + v0.z * v0.z;
			if (num > float.Epsilon)
			{
				float num2 = (float)Math.Sqrt(num);
				if (num2 > 1E-07f)
				{
					num2 = 1f / num2;
					v0.x *= num2;
					v0.z *= num2;
					return true;
				}
			}
			v0.x = (v0.z = 0f);
			return false;
		}

		public static bool SAFBIKVecNormalizeYZ(ref Vector3 v0)
		{
			float num = v0.y * v0.y + v0.z * v0.z;
			if (num > float.Epsilon)
			{
				float num2 = (float)Math.Sqrt(num);
				if (num2 > 1E-07f)
				{
					num2 = 1f / num2;
					v0.y *= num2;
					v0.z *= num2;
					return true;
				}
			}
			v0.y = (v0.z = 0f);
			return false;
		}

		public static bool SAFBIKVecNormalize2(ref Vector3 v0, ref Vector3 v1)
		{
			bool num = SAFBIKVecNormalize(ref v0);
			bool flag = SAFBIKVecNormalize(ref v1);
			return num && flag;
		}

		public static bool SAFBIKVecNormalize3(ref Vector3 v0, ref Vector3 v1, ref Vector3 v2)
		{
			bool num = SAFBIKVecNormalize(ref v0);
			bool flag = SAFBIKVecNormalize(ref v1);
			bool flag2 = SAFBIKVecNormalize(ref v2);
			return num && flag && flag2;
		}

		public static bool SAFBIKVecNormalize4(ref Vector3 v0, ref Vector3 v1, ref Vector3 v2, ref Vector3 v3)
		{
			bool num = SAFBIKVecNormalize(ref v0);
			bool flag = SAFBIKVecNormalize(ref v1);
			bool flag2 = SAFBIKVecNormalize(ref v2);
			bool flag3 = SAFBIKVecNormalize(ref v3);
			return num && flag && flag2 && flag3;
		}

		public static void SAFBIKMatMult(out Matrix3x3 ret, ref Matrix3x3 lhs, ref Matrix3x3 rhs)
		{
			ret = new Matrix3x3(lhs.column0.x * rhs.column0.x + lhs.column1.x * rhs.column0.y + lhs.column2.x * rhs.column0.z, lhs.column0.x * rhs.column1.x + lhs.column1.x * rhs.column1.y + lhs.column2.x * rhs.column1.z, lhs.column0.x * rhs.column2.x + lhs.column1.x * rhs.column2.y + lhs.column2.x * rhs.column2.z, lhs.column0.y * rhs.column0.x + lhs.column1.y * rhs.column0.y + lhs.column2.y * rhs.column0.z, lhs.column0.y * rhs.column1.x + lhs.column1.y * rhs.column1.y + lhs.column2.y * rhs.column1.z, lhs.column0.y * rhs.column2.x + lhs.column1.y * rhs.column2.y + lhs.column2.y * rhs.column2.z, lhs.column0.z * rhs.column0.x + lhs.column1.z * rhs.column0.y + lhs.column2.z * rhs.column0.z, lhs.column0.z * rhs.column1.x + lhs.column1.z * rhs.column1.y + lhs.column2.z * rhs.column1.z, lhs.column0.z * rhs.column2.x + lhs.column1.z * rhs.column2.y + lhs.column2.z * rhs.column2.z);
		}

		public static void SAFBIKMatMultRet0(ref Matrix3x3 lhs, ref Matrix3x3 rhs)
		{
			lhs = new Matrix3x3(lhs.column0.x * rhs.column0.x + lhs.column1.x * rhs.column0.y + lhs.column2.x * rhs.column0.z, lhs.column0.x * rhs.column1.x + lhs.column1.x * rhs.column1.y + lhs.column2.x * rhs.column1.z, lhs.column0.x * rhs.column2.x + lhs.column1.x * rhs.column2.y + lhs.column2.x * rhs.column2.z, lhs.column0.y * rhs.column0.x + lhs.column1.y * rhs.column0.y + lhs.column2.y * rhs.column0.z, lhs.column0.y * rhs.column1.x + lhs.column1.y * rhs.column1.y + lhs.column2.y * rhs.column1.z, lhs.column0.y * rhs.column2.x + lhs.column1.y * rhs.column2.y + lhs.column2.y * rhs.column2.z, lhs.column0.z * rhs.column0.x + lhs.column1.z * rhs.column0.y + lhs.column2.z * rhs.column0.z, lhs.column0.z * rhs.column1.x + lhs.column1.z * rhs.column1.y + lhs.column2.z * rhs.column1.z, lhs.column0.z * rhs.column2.x + lhs.column1.z * rhs.column2.y + lhs.column2.z * rhs.column2.z);
		}

		public static void SAFBIKMatMultCol0(out Vector3 ret, ref Matrix3x3 lhs, ref Matrix3x3 rhs)
		{
			ret = new Vector3(lhs.column0.x * rhs.column0.x + lhs.column1.x * rhs.column0.y + lhs.column2.x * rhs.column0.z, lhs.column0.y * rhs.column0.x + lhs.column1.y * rhs.column0.y + lhs.column2.y * rhs.column0.z, lhs.column0.z * rhs.column0.x + lhs.column1.z * rhs.column0.y + lhs.column2.z * rhs.column0.z);
		}

		public static void SAFBIKMatMultCol1(out Vector3 ret, ref Matrix3x3 lhs, ref Matrix3x3 rhs)
		{
			ret = new Vector3(lhs.column0.x * rhs.column1.x + lhs.column1.x * rhs.column1.y + lhs.column2.x * rhs.column1.z, lhs.column0.y * rhs.column1.x + lhs.column1.y * rhs.column1.y + lhs.column2.y * rhs.column1.z, lhs.column0.z * rhs.column1.x + lhs.column1.z * rhs.column1.y + lhs.column2.z * rhs.column1.z);
		}

		public static void SAFBIKMatMultCol2(out Vector3 ret, ref Matrix3x3 lhs, ref Matrix3x3 rhs)
		{
			ret = new Vector3(lhs.column0.x * rhs.column2.x + lhs.column1.x * rhs.column2.y + lhs.column2.x * rhs.column2.z, lhs.column0.y * rhs.column2.x + lhs.column1.y * rhs.column2.y + lhs.column2.y * rhs.column2.z, lhs.column0.z * rhs.column2.x + lhs.column1.z * rhs.column2.y + lhs.column2.z * rhs.column2.z);
		}

		public static void SAFBIKMatMultVec(out Vector3 ret, ref Matrix3x3 m, ref Vector3 v)
		{
			ret = new Vector3(m.column0.x * v.x + m.column1.x * v.y + m.column2.x * v.z, m.column0.y * v.x + m.column1.y * v.y + m.column2.y * v.z, m.column0.z * v.x + m.column1.z * v.y + m.column2.z * v.z);
		}

		public static void SAFBIKMatGetRot(out Quaternion q, ref Matrix3x3 m)
		{
			q = default(Quaternion);
			float num = m.column0.x + m.column1.y + m.column2.z;
			if (num > 0f)
			{
				float num2 = (float)Math.Sqrt(num + 1f);
				q.w = num2 * 0.5f;
				num2 = 0.5f / num2;
				q.x = (m.column1.z - m.column2.y) * num2;
				q.y = (m.column2.x - m.column0.z) * num2;
				q.z = (m.column0.y - m.column1.x) * num2;
			}
			else if (m.column0.x > m.column1.y && m.column0.x > m.column2.z)
			{
				float num3 = m.column0.x - m.column1.y - m.column2.z + 1f;
				if (num3 <= float.Epsilon)
				{
					q = Quaternion.identity;
					return;
				}
				num3 = (float)Math.Sqrt(num3);
				q.x = num3 * 0.5f;
				num3 = 0.5f / num3;
				q.w = (m.column1.z - m.column2.y) * num3;
				q.y = (m.column0.y + m.column1.x) * num3;
				q.z = (m.column0.z + m.column2.x) * num3;
			}
			else if (m.column1.y > m.column2.z)
			{
				float num4 = m.column1.y - m.column0.x - m.column2.z + 1f;
				if (num4 <= float.Epsilon)
				{
					q = Quaternion.identity;
					return;
				}
				num4 = (float)Math.Sqrt(num4);
				q.y = num4 * 0.5f;
				num4 = 0.5f / num4;
				q.w = (m.column2.x - m.column0.z) * num4;
				q.z = (m.column1.z + m.column2.y) * num4;
				q.x = (m.column1.x + m.column0.y) * num4;
			}
			else
			{
				float num5 = m.column2.z - m.column0.x - m.column1.y + 1f;
				if (num5 <= float.Epsilon)
				{
					q = Quaternion.identity;
					return;
				}
				num5 = (float)Math.Sqrt(num5);
				q.z = num5 * 0.5f;
				num5 = 0.5f / num5;
				q.w = (m.column0.y - m.column1.x) * num5;
				q.x = (m.column2.x + m.column0.z) * num5;
				q.y = (m.column2.y + m.column1.z) * num5;
			}
		}

		public static void SAFBIKMatSetRot(out Matrix3x3 m, ref Quaternion q)
		{
			float num = q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w;
			float num2 = ((num > float.Epsilon) ? (2f / num) : 0f);
			float num3 = q.x * num2;
			float num4 = q.y * num2;
			float num5 = q.z * num2;
			float num6 = q.w * num3;
			float num7 = q.w * num4;
			float num8 = q.w * num5;
			float num9 = q.x * num3;
			float num10 = q.x * num4;
			float num11 = q.x * num5;
			float num12 = q.y * num4;
			float num13 = q.y * num5;
			float num14 = q.z * num5;
			m.column0.x = 1f - (num12 + num14);
			m.column1.x = num10 - num8;
			m.column2.x = num11 + num7;
			m.column0.y = num10 + num8;
			m.column1.y = 1f - (num9 + num14);
			m.column2.y = num13 - num6;
			m.column0.z = num11 - num7;
			m.column1.z = num13 + num6;
			m.column2.z = 1f - (num9 + num12);
		}

		public static void SAFBIKMatSetAxisAngle(out Matrix3x3 m, ref Vector3 axis, float cos)
		{
			if (cos >= -1E-45f && cos <= float.Epsilon)
			{
				m = Matrix3x3.identity;
				return;
			}
			m = default(Matrix3x3);
			float num = 1f - cos * cos;
			num = ((num <= float.Epsilon) ? 0f : ((num >= 1f) ? 1f : ((float)Math.Sqrt(num))));
			float num2 = axis.x * num;
			float num3 = axis.y * num;
			float num4 = axis.z * num;
			m.column0.x = cos;
			m.column0.y = num4;
			m.column0.z = 0f - num3;
			m.column1.x = 0f - num4;
			m.column1.y = cos;
			m.column1.z = num2;
			m.column2.x = num3;
			m.column2.y = 0f - num2;
			m.column2.z = cos;
			float num5 = 1f - cos;
			float num6 = axis.x * num5;
			float num7 = axis.y * num5;
			float num8 = axis.z * num5;
			m.column0.x += axis.x * num6;
			m.column0.y += axis.y * num6;
			m.column0.z += axis.z * num6;
			m.column1.x += axis.x * num7;
			m.column1.y += axis.y * num7;
			m.column1.z += axis.z * num7;
			m.column2.x += axis.x * num8;
			m.column2.y += axis.y * num8;
			m.column2.z += axis.z * num8;
		}

		public static void SAFBIKMatFastLerp(out Matrix3x3 ret, ref Matrix3x3 lhs, ref Matrix3x3 rhs, float rate)
		{
			if (rate <= 1E-07f)
			{
				ret = lhs;
				return;
			}
			if (rate >= 0.9999999f)
			{
				ret = rhs;
				return;
			}
			Vector3 column = lhs.column0;
			Vector3 v = lhs.column1;
			column += (rhs.column0 - column) * rate;
			v += (rhs.column1 - v) * rate;
			Vector3 v2 = Vector3.Cross(column, v);
			column = Vector3.Cross(v, v2);
			if (SAFBIKVecNormalize3(ref column, ref v, ref v2))
			{
				ret = Matrix3x3.FromColumn(ref column, ref v, ref v2);
			}
			else
			{
				ret = lhs;
			}
		}

		public static void SAFBIKMatFastLerpToIdentity(ref Matrix3x3 m, float rate)
		{
			if (rate <= 1E-07f)
			{
				return;
			}
			if (rate >= 0.9999999f)
			{
				m = Matrix3x3.identity;
				return;
			}
			Vector3 column = m.column0;
			Vector3 v = m.column1;
			column += (new Vector3(1f, 0f, 0f) - column) * rate;
			v += (new Vector3(0f, 1f, 0f) - v) * rate;
			Vector3 v2 = Vector3.Cross(column, v);
			column = Vector3.Cross(v, v2);
			if (SAFBIKVecNormalize3(ref column, ref v, ref v2))
			{
				m = Matrix3x3.FromColumn(ref column, ref v, ref v2);
			}
		}

		public static void SAFBIKMatMultVecInv(out Vector3 ret, ref Matrix3x3 mat, ref Vector3 vec)
		{
			Matrix3x3 m = mat.transpose;
			SAFBIKMatMultVec(out ret, ref m, ref vec);
		}

		public static void SAFBIKMatMultVecAdd(out Vector3 ret, ref Matrix3x3 mat, ref Vector3 vec, ref Vector3 addVec)
		{
			SAFBIKMatMultVec(out ret, ref mat, ref vec);
			ret += addVec;
		}

		public static void SAFBIKMatMultVecPreSub(out Vector3 ret, ref Matrix3x3 mat, ref Vector3 vec, ref Vector3 subVec)
		{
			Vector3 v = vec - subVec;
			SAFBIKMatMultVec(out ret, ref mat, ref v);
		}

		public static void SAFBIKMatMultVecPreSubAdd(out Vector3 ret, ref Matrix3x3 mat, ref Vector3 vec, ref Vector3 subVec, ref Vector3 addVec)
		{
			Vector3 v = vec - subVec;
			SAFBIKMatMultVec(out ret, ref mat, ref v);
			ret += addVec;
		}

		public static void SAFBIKMatMultInv0(out Matrix3x3 ret, ref Matrix3x3 lhs, ref Matrix3x3 rhs)
		{
			ret = new Matrix3x3(lhs.column0.x * rhs.column0.x + lhs.column0.y * rhs.column0.y + lhs.column0.z * rhs.column0.z, lhs.column0.x * rhs.column1.x + lhs.column0.y * rhs.column1.y + lhs.column0.z * rhs.column1.z, lhs.column0.x * rhs.column2.x + lhs.column0.y * rhs.column2.y + lhs.column0.z * rhs.column2.z, lhs.column1.x * rhs.column0.x + lhs.column1.y * rhs.column0.y + lhs.column1.z * rhs.column0.z, lhs.column1.x * rhs.column1.x + lhs.column1.y * rhs.column1.y + lhs.column1.z * rhs.column1.z, lhs.column1.x * rhs.column2.x + lhs.column1.y * rhs.column2.y + lhs.column1.z * rhs.column2.z, lhs.column2.x * rhs.column0.x + lhs.column2.y * rhs.column0.y + lhs.column2.z * rhs.column0.z, lhs.column2.x * rhs.column1.x + lhs.column2.y * rhs.column1.y + lhs.column2.z * rhs.column1.z, lhs.column2.x * rhs.column2.x + lhs.column2.y * rhs.column2.y + lhs.column2.z * rhs.column2.z);
		}

		public static void SAFBIKMatMultInv1(out Matrix3x3 ret, ref Matrix3x3 lhs, ref Matrix3x3 rhs)
		{
			ret = new Matrix3x3(lhs.column0.x * rhs.column0.x + lhs.column1.x * rhs.column1.x + lhs.column2.x * rhs.column2.x, lhs.column0.x * rhs.column0.y + lhs.column1.x * rhs.column1.y + lhs.column2.x * rhs.column2.y, lhs.column0.x * rhs.column0.z + lhs.column1.x * rhs.column1.z + lhs.column2.x * rhs.column2.z, lhs.column0.y * rhs.column0.x + lhs.column1.y * rhs.column1.x + lhs.column2.y * rhs.column2.x, lhs.column0.y * rhs.column0.y + lhs.column1.y * rhs.column1.y + lhs.column2.y * rhs.column2.y, lhs.column0.y * rhs.column0.z + lhs.column1.y * rhs.column1.z + lhs.column2.y * rhs.column2.z, lhs.column0.z * rhs.column0.x + lhs.column1.z * rhs.column1.x + lhs.column2.z * rhs.column2.x, lhs.column0.z * rhs.column0.y + lhs.column1.z * rhs.column1.y + lhs.column2.z * rhs.column2.y, lhs.column0.z * rhs.column0.z + lhs.column1.z * rhs.column1.z + lhs.column2.z * rhs.column2.z);
		}

		public static void SAFBIKMatMultGetRot(out Quaternion ret, ref Matrix3x3 lhs, ref Matrix3x3 rhs)
		{
			Matrix3x3 ret2;
			SAFBIKMatMult(out ret2, ref lhs, ref rhs);
			SAFBIKMatGetRot(out ret, ref ret2);
		}

		public static void SAFBIKMatSetRotMult(out Matrix3x3 ret, ref Quaternion lhs, ref Quaternion rhs)
		{
			Quaternion q = lhs * rhs;
			SAFBIKMatSetRot(out ret, ref q);
		}

		public static void SAFBIKMatSetRotMultInv1(out Matrix3x3 ret, ref Quaternion lhs, ref Quaternion rhs)
		{
			Quaternion q = lhs * Inverse(rhs);
			SAFBIKMatSetRot(out ret, ref q);
		}

		public static void SAFBIKQuatMult(out Quaternion ret, ref Quaternion q0, ref Quaternion q1)
		{
			ret = q0 * q1;
		}

		public static void SAFBIKQuatMultInv0(out Quaternion ret, ref Quaternion q0, ref Quaternion q1)
		{
			ret = Inverse(q0) * q1;
		}

		public static void SAFBIKQuatMultNorm(out Quaternion ret, ref Quaternion q0, ref Quaternion q1)
		{
			ret = Normalize(q0 * q1);
		}

		public static void SAFBIKQuatMultNormInv0(out Quaternion ret, ref Quaternion q0, ref Quaternion q1)
		{
			ret = Normalize(Inverse(q0) * q1);
		}

		public static void SAFBIKQuatMult3(out Quaternion ret, ref Quaternion q0, ref Quaternion q1, ref Quaternion q2)
		{
			ret = q0 * q1 * q2;
		}

		public static void SAFBIKQuatMult3Inv0(out Quaternion ret, ref Quaternion q0, ref Quaternion q1, ref Quaternion q2)
		{
			ret = Inverse(q0) * q1 * q2;
		}

		public static void SAFBIKQuatMult3Inv1(out Quaternion ret, ref Quaternion q0, ref Quaternion q1, ref Quaternion q2)
		{
			ret = q0 * Inverse(q1) * q2;
		}

		public static void SAFBIKQuatMultNorm3(out Quaternion ret, ref Quaternion q0, ref Quaternion q1, ref Quaternion q2)
		{
			ret = Normalize(q0 * q1 * q2);
		}

		public static void SAFBIKQuatMultNorm3Inv0(out Quaternion ret, ref Quaternion q0, ref Quaternion q1, ref Quaternion q2)
		{
			ret = Normalize(Inverse(q0) * q1 * q2);
		}

		public static void SAFBIKQuatMultNorm3Inv1(out Quaternion ret, ref Quaternion q0, ref Quaternion q1, ref Quaternion q2)
		{
			ret = Normalize(q0 * Inverse(q1) * q2);
		}

		public static bool SAFBIKComputeBasisFromXZLockX(out Matrix3x3 basis, ref Vector3 dirX, ref Vector3 dirZ)
		{
			Vector3 v = Vector3.Cross(dirZ, dirX);
			Vector3 v2 = Vector3.Cross(dirX, v);
			if (SAFBIKVecNormalize2(ref v, ref v2))
			{
				basis = Matrix3x3.FromColumn(ref dirX, ref v, ref v2);
				return true;
			}
			basis = Matrix3x3.identity;
			return false;
		}

		public static bool SAFBIKComputeBasisFromXYLockX(out Matrix3x3 basis, ref Vector3 dirX, ref Vector3 dirY)
		{
			Vector3 v = Vector3.Cross(dirX, dirY);
			Vector3 v2 = Vector3.Cross(v, dirX);
			if (SAFBIKVecNormalize2(ref v2, ref v))
			{
				basis = Matrix3x3.FromColumn(ref dirX, ref v2, ref v);
				return true;
			}
			basis = Matrix3x3.identity;
			return false;
		}

		public static bool SAFBIKComputeBasisFromXYLockY(out Matrix3x3 basis, ref Vector3 dirX, ref Vector3 dirY)
		{
			Vector3 v = Vector3.Cross(dirX, dirY);
			Vector3 v2 = Vector3.Cross(dirY, v);
			if (SAFBIKVecNormalize2(ref v2, ref v))
			{
				basis = Matrix3x3.FromColumn(ref v2, ref dirY, ref v);
				return true;
			}
			basis = Matrix3x3.identity;
			return false;
		}

		public static bool SAFBIKComputeBasisFromXZLockZ(out Matrix3x3 basis, ref Vector3 dirX, ref Vector3 dirZ)
		{
			Vector3 v = Vector3.Cross(dirZ, dirX);
			Vector3 v2 = Vector3.Cross(v, dirZ);
			if (SAFBIKVecNormalize2(ref v2, ref v))
			{
				basis = Matrix3x3.FromColumn(ref v2, ref v, ref dirZ);
				return true;
			}
			basis = Matrix3x3.identity;
			return false;
		}

		public static bool SAFBIKComputeBasisFromYZLockY(out Matrix3x3 basis, ref Vector3 dirY, ref Vector3 dirZ)
		{
			Vector3 v = Vector3.Cross(dirY, dirZ);
			Vector3 v2 = Vector3.Cross(v, dirY);
			if (SAFBIKVecNormalize2(ref v, ref v2))
			{
				basis = Matrix3x3.FromColumn(ref v, ref dirY, ref v2);
				return true;
			}
			basis = Matrix3x3.identity;
			return false;
		}

		public static bool SAFBIKComputeBasisFromYZLockZ(out Matrix3x3 basis, ref Vector3 dirY, ref Vector3 dirZ)
		{
			Vector3 v = Vector3.Cross(dirY, dirZ);
			Vector3 v2 = Vector3.Cross(dirZ, v);
			if (SAFBIKVecNormalize2(ref v, ref v2))
			{
				basis = Matrix3x3.FromColumn(ref v, ref v2, ref dirZ);
				return true;
			}
			basis = Matrix3x3.identity;
			return false;
		}

		public static bool SAFBIKComputeBasisLockX(out Matrix3x3 basis, ref Vector3 dirX, ref Vector3 dirY, ref Vector3 dirZ)
		{
			Matrix3x3 basis2;
			bool flag = SAFBIKComputeBasisFromXYLockX(out basis2, ref dirX, ref dirY);
			Matrix3x3 basis3;
			bool flag2 = SAFBIKComputeBasisFromXZLockX(out basis3, ref dirX, ref dirZ);
			if (flag && flag2)
			{
				float num = Mathf.Abs(Vector3.Dot(dirX, dirY));
				float num2 = Mathf.Abs(Vector3.Dot(dirX, dirZ));
				if (num2 <= 1E-07f)
				{
					basis = basis3;
					return true;
				}
				if (num <= 1E-07f)
				{
					basis = basis2;
					return true;
				}
				SAFBIKMatFastLerp(out basis, ref basis2, ref basis3, num / (num + num2));
				return true;
			}
			if (flag)
			{
				basis = basis2;
				return true;
			}
			if (flag2)
			{
				basis = basis3;
				return true;
			}
			basis = Matrix3x3.identity;
			return false;
		}

		public static bool SAFBIKComputeBasisLockY(out Matrix3x3 basis, ref Vector3 dirX, ref Vector3 dirY, ref Vector3 dirZ)
		{
			Matrix3x3 basis2;
			bool flag = SAFBIKComputeBasisFromXYLockY(out basis2, ref dirX, ref dirY);
			Matrix3x3 basis3;
			bool flag2 = SAFBIKComputeBasisFromYZLockY(out basis3, ref dirY, ref dirZ);
			if (flag && flag2)
			{
				float num = Mathf.Abs(Vector3.Dot(dirY, dirX));
				float num2 = Mathf.Abs(Vector3.Dot(dirY, dirZ));
				if (num2 <= 1E-07f)
				{
					basis = basis3;
					return true;
				}
				if (num <= 1E-07f)
				{
					basis = basis2;
					return true;
				}
				SAFBIKMatFastLerp(out basis, ref basis2, ref basis3, num / (num + num2));
				return true;
			}
			if (flag)
			{
				basis = basis2;
				return true;
			}
			if (flag2)
			{
				basis = basis3;
				return true;
			}
			basis = Matrix3x3.identity;
			return false;
		}

		public static bool SAFBIKComputeBasisLockZ(out Matrix3x3 basis, ref Vector3 dirX, ref Vector3 dirY, ref Vector3 dirZ)
		{
			Matrix3x3 basis2;
			bool flag = SAFBIKComputeBasisFromXZLockZ(out basis2, ref dirX, ref dirZ);
			Matrix3x3 basis3;
			bool flag2 = SAFBIKComputeBasisFromYZLockZ(out basis3, ref dirY, ref dirZ);
			if (flag && flag2)
			{
				float num = Mathf.Abs(Vector3.Dot(dirZ, dirX));
				float num2 = Mathf.Abs(Vector3.Dot(dirZ, dirY));
				if (num2 <= 1E-07f)
				{
					basis = basis3;
					return true;
				}
				if (num <= 1E-07f)
				{
					basis = basis2;
					return true;
				}
				SAFBIKMatFastLerp(out basis, ref basis2, ref basis3, num / (num + num2));
				return true;
			}
			if (flag)
			{
				basis = basis2;
				return true;
			}
			if (flag2)
			{
				basis = basis3;
				return true;
			}
			basis = Matrix3x3.identity;
			return false;
		}

		public static bool IsFuzzy(float lhs, float rhs, float epsilon = 1E-07f)
		{
			float num = lhs - rhs;
			if (num >= 0f - epsilon)
			{
				return num <= epsilon;
			}
			return false;
		}

		public static bool IsFuzzy(Vector3 lhs, Vector3 rhs, float epsilon = 1E-07f)
		{
			float num = lhs.x - rhs.x;
			if (num >= 0f - epsilon && num <= epsilon)
			{
				num = lhs.y - rhs.y;
				if (num >= 0f - epsilon && num <= epsilon)
				{
					num = lhs.z - rhs.z;
					if (num >= 0f - epsilon && num <= epsilon)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static bool IsFuzzy(ref Vector3 lhs, ref Vector3 rhs, float epsilon = 1E-07f)
		{
			float num = lhs.x - rhs.x;
			if (num >= 0f - epsilon && num <= epsilon)
			{
				num = lhs.y - rhs.y;
				if (num >= 0f - epsilon && num <= epsilon)
				{
					num = lhs.z - rhs.z;
					if (num >= 0f - epsilon && num <= epsilon)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static bool _IsNear(ref Vector3 lhs, ref Vector3 rhs)
		{
			return IsFuzzy(ref lhs, ref rhs, 1E-05f);
		}

		public static Vector3 _Rotate(ref Vector3 dirX, ref Vector3 dirY, float cosR, float sinR)
		{
			return dirX * cosR + dirY * sinR;
		}

		public static Vector3 _Rotate(ref Vector3 dirX, ref Vector3 dirY, float r)
		{
			float num = Mathf.Cos(r);
			float num2 = Mathf.Sin(r);
			return dirX * num + dirY * num2;
		}

		public static Vector3 _Rotate(ref Vector3 dirX, ref Vector3 dirY, ref FastAngle angle)
		{
			return dirX * angle.cos + dirY * angle.sin;
		}

		public static bool _NormalizedTranslate(ref Vector3 dir, ref Vector3 fr, ref Vector3 to)
		{
			Vector3 vector = to - fr;
			float magnitude = vector.magnitude;
			if (magnitude > 1E-07f)
			{
				dir = vector * (1f / magnitude);
				return true;
			}
			dir = Vector3.zero;
			return false;
		}

		public static Quaternion Inverse(Quaternion q)
		{
			return new Quaternion(0f - q.x, 0f - q.y, 0f - q.z, q.w);
		}

		public static Quaternion Normalize(Quaternion q)
		{
			float num = q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w;
			if (num > 1E-07f)
			{
				if (num >= 0.9999999f && (double)num <= 1.0000001000000012)
				{
					return q;
				}
				float num2 = 1f / Mathf.Sqrt(num);
				return new Quaternion(q.x * num2, q.y * num2, q.z * num2, q.w * num2);
			}
			return q;
		}

		public static bool SafeNormalize(ref Quaternion q)
		{
			float num = q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w;
			if (num > 1E-07f)
			{
				if (num >= 0.9999999f && (double)num <= 1.0000001000000012)
				{
					return true;
				}
				float num2 = 1f / Mathf.Sqrt(num);
				q.x *= num2;
				q.y *= num2;
				q.z *= num2;
				q.w *= num2;
				return true;
			}
			return false;
		}

		public static bool IsFuzzyIdentity(Quaternion q)
		{
			return IsFuzzyIdentity(ref q);
		}

		public static bool IsFuzzyIdentity(ref Quaternion q)
		{
			if (q.x >= -1E-07f && q.x <= 1E-07f && q.y >= -1E-07f && q.y <= 1E-07f && q.z >= -1E-07f && q.z <= 1E-07f && q.w >= 0.9999999f)
			{
				return q.w <= 1.0000001f;
			}
			return false;
		}

		public static bool SAFBIKComputeBasisFromXZLockX(out Matrix3x3 basis, Vector3 dirX, Vector3 dirZ)
		{
			return SAFBIKComputeBasisFromXZLockX(out basis, ref dirX, ref dirZ);
		}

		public static bool SAFBIKComputeBasisFromXYLockX(out Matrix3x3 basis, Vector3 dirX, Vector3 dirY)
		{
			return SAFBIKComputeBasisFromXYLockX(out basis, ref dirX, ref dirY);
		}

		public static bool SAFBIKComputeBasisFromXYLockY(out Matrix3x3 basis, Vector3 dirX, Vector3 dirY)
		{
			return SAFBIKComputeBasisFromXYLockY(out basis, ref dirX, ref dirY);
		}

		public static bool SAFBIKComputeBasisFromXZLockZ(out Matrix3x3 basis, Vector3 dirX, Vector3 dirZ)
		{
			return SAFBIKComputeBasisFromXZLockZ(out basis, ref dirX, ref dirZ);
		}

		public static bool SAFBIKComputeBasisFromYZLockY(out Matrix3x3 basis, Vector3 dirY, Vector3 dirZ)
		{
			return SAFBIKComputeBasisFromYZLockY(out basis, ref dirY, ref dirZ);
		}

		public static bool SAFBIKComputeBasisFromYZLockZ(out Matrix3x3 basis, Vector3 dirY, Vector3 dirZ)
		{
			return SAFBIKComputeBasisFromYZLockZ(out basis, ref dirY, ref dirZ);
		}

		public static bool SAFBIKComputeBasisFrom(out Matrix3x3 basis, ref Matrix3x3 rootBasis, ref Vector3 dir, _DirectionAs directionAs)
		{
			switch (directionAs)
			{
			case _DirectionAs.XPlus:
				return SAFBIKComputeBasisFromXYLockX(out basis, dir, rootBasis.column1);
			case _DirectionAs.XMinus:
				return SAFBIKComputeBasisFromXYLockX(out basis, -dir, rootBasis.column1);
			case _DirectionAs.YPlus:
				return SAFBIKComputeBasisFromXYLockY(out basis, rootBasis.column0, dir);
			case _DirectionAs.YMinus:
				return SAFBIKComputeBasisFromXYLockY(out basis, rootBasis.column0, -dir);
			default:
				basis = Matrix3x3.identity;
				return false;
			}
		}

		public static float ComputeCosTheta(float lenASq, float lenBSq, float lenCSq, float lenB, float lenC)
		{
			float num = lenB * lenC * 2f;
			if (num > 1E-07f)
			{
				return (lenBSq + lenCSq - lenASq) / num;
			}
			return 1f;
		}

		public static float ComputeCosTheta(FastLength lenA, FastLength lenB, FastLength lenC)
		{
			float num = lenB.length * lenC.length * 2f;
			if (num > 1E-07f)
			{
				return (lenB.lengthSq + lenC.lengthSq - lenA.lengthSq) / num;
			}
			return 0f;
		}

		public static float ComputeSinTheta(float lenASq, float lenBSq, float lenCSq, float lenB, float lenC)
		{
			float num = lenB * lenC * 2f;
			if (num > 1E-07f)
			{
				float num2 = (lenBSq + lenCSq - lenASq) / num;
				return SAFBIKSqrtClamp01(1f - num2 * num2);
			}
			return 0f;
		}

		public static float ComputeSinTheta(FastLength lenA, FastLength lenB, FastLength lenC)
		{
			float num = lenB.length * lenC.length * 2f;
			if (num > 1E-07f)
			{
				float num2 = (lenB.lengthSq + lenC.lengthSq - lenA.lengthSq) / num;
				return SAFBIKSqrtClamp01(1f - num2 * num2);
			}
			return 0f;
		}

		private static bool _ComputeThetaAxis(ref Vector3 origPos, ref Vector3 fromPos, ref Vector3 toPos, out float theta, out Vector3 axis)
		{
			Vector3 v = fromPos - origPos;
			Vector3 v2 = toPos - origPos;
			if (!SAFBIKVecNormalize2(ref v, ref v2))
			{
				theta = 0f;
				axis = new Vector3(0f, 0f, 1f);
				return false;
			}
			return _ComputeThetaAxis(ref v, ref v2, out theta, out axis);
		}

		private static bool _ComputeThetaAxis(ref Vector3 dirFrom, ref Vector3 dirTo, out float theta, out Vector3 axis)
		{
			axis = Vector3.Cross(dirFrom, dirTo);
			if (!SAFBIKVecNormalize(ref axis))
			{
				theta = 0f;
				axis = new Vector3(0f, 0f, 1f);
				return false;
			}
			theta = Vector3.Dot(dirFrom, dirTo);
			return true;
		}

		public static Vector3 Scale(Vector3 lhs, Vector3 rhs)
		{
			return new Vector3(lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z);
		}

		public static Vector3 Scale(ref Vector3 lhs, ref Vector3 rhs)
		{
			return new Vector3(lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z);
		}

		public static bool _LimitXY_Square(ref Vector3 dir, float limitXMinus, float limitXPlus, float limitYMinus, float limitYPlus)
		{
			bool flag = false;
			bool flag2 = false;
			if (dir.x < 0f - limitXMinus)
			{
				dir.x = 0f - limitXMinus;
				flag = true;
			}
			else if (dir.x > limitXPlus)
			{
				dir.x = limitXPlus;
				flag = true;
			}
			if (dir.y < 0f - limitYMinus)
			{
				dir.y = 0f - limitYMinus;
				flag2 = true;
			}
			else if (dir.y > limitYPlus)
			{
				dir.y = limitYPlus;
				flag2 = true;
			}
			if (flag || flag2)
			{
				dir.z = SAFBIKSqrt(1f - (dir.x * dir.x + dir.y * dir.y));
				return true;
			}
			if (dir.z < 0f)
			{
				dir.z = 0f - dir.z;
				return true;
			}
			return false;
		}

		public static bool _LimitYZ_Square(bool isRight, ref Vector3 dir, float limitYMinus, float limitYPlus, float limitZMinus, float limitZPlus)
		{
			bool flag = false;
			bool flag2 = false;
			if (dir.y < 0f - limitYMinus)
			{
				dir.y = 0f - limitYMinus;
				flag = true;
			}
			else if (dir.y > limitYPlus)
			{
				dir.y = limitYPlus;
				flag = true;
			}
			if (dir.z < 0f - limitZMinus)
			{
				dir.z = 0f - limitZMinus;
				flag2 = true;
			}
			else if (dir.z > limitZPlus)
			{
				dir.z = limitZPlus;
				flag2 = true;
			}
			if (flag || flag2)
			{
				dir.x = SAFBIKSqrt(1f - (dir.y * dir.y + dir.z * dir.z));
				if (!isRight)
				{
					dir.x = 0f - dir.x;
				}
				return true;
			}
			if (isRight)
			{
				if (dir.x < 0f)
				{
					dir.x = 0f - dir.x;
					return true;
				}
			}
			else if (dir.x >= 0f)
			{
				dir.x = 0f - dir.x;
				return true;
			}
			return false;
		}

		public static bool _LimitXZ_Square(ref Vector3 dir, float limitXMinus, float limitXPlus, float limitZMinus, float limitZPlus)
		{
			bool flag = false;
			bool flag2 = false;
			if (dir.x < 0f - limitXMinus)
			{
				dir.x = 0f - limitXMinus;
				flag = true;
			}
			else if (dir.x > limitXPlus)
			{
				dir.x = limitXPlus;
				flag = true;
			}
			if (dir.z < 0f - limitZMinus)
			{
				dir.z = 0f - limitZMinus;
				flag2 = true;
			}
			else if (dir.z > limitZPlus)
			{
				dir.z = limitZPlus;
				flag2 = true;
			}
			if (flag || flag2)
			{
				dir.y = SAFBIKSqrt(1f - (dir.x * dir.x + dir.z * dir.z));
				return true;
			}
			if (dir.y < 0f)
			{
				dir.y = 0f - dir.y;
				return true;
			}
			return false;
		}

		public static bool _LimitXY(ref Vector3 dir, float limitXMinus, float limitXPlus, float limitYMinus, float limitYPlus)
		{
			bool num = dir.x >= 0f;
			bool flag = dir.y >= 0f;
			float num2 = (num ? limitXPlus : limitXMinus);
			float num3 = (flag ? limitYPlus : limitYMinus);
			bool flag2 = false;
			if (num2 <= 1E-07f && num3 <= 1E-07f)
			{
				Vector3 vector = new Vector3(0f, 0f, 1f);
				Vector3 vector2 = vector - dir;
				if (Mathf.Abs(vector2.x) > 1E-07f || Mathf.Abs(vector2.y) > 1E-07f || Mathf.Abs(vector2.z) > 1E-07f)
				{
					dir = vector;
					flag2 = true;
				}
			}
			else
			{
				float num4 = ((num2 >= 1E-07f) ? (1f / num2) : 0f);
				float num5 = ((num3 >= 1E-07f) ? (1f / num3) : 0f);
				float num6 = dir.x * num4;
				float num7 = dir.y * num5;
				float num8 = SAFBIKSqrt(num6 * num6 + num7 * num7 + dir.z * dir.z);
				float num9 = ((num8 > 1E-07f) ? (1f / num8) : 0f);
				float num10 = num6 * num9;
				float num11 = num7 * num9;
				if (num8 > 1f && !flag2)
				{
					flag2 = true;
					num6 = num10;
					num7 = num11;
				}
				float num12 = (flag2 ? (num6 * num2) : dir.x);
				float num13 = (flag2 ? (num7 * num3) : dir.y);
				bool flag3 = dir.z < 0f;
				if (flag2)
				{
					float num14 = SAFBIKSqrt(num12 * num12 + num13 * num13);
					float z = SAFBIKSqrt(1f - num14 * num14);
					dir.x = num12;
					dir.y = num13;
					dir.z = z;
				}
				else if (flag3)
				{
					flag2 = true;
					dir.z = 0f - dir.z;
				}
			}
			return flag2;
		}

		public static bool _LimitXZ(ref Vector3 dir, float limiXMinus, float limiXPlus, float limiZMinus, float limiZPlus)
		{
			bool num = dir.x >= 0f;
			bool flag = dir.z >= 0f;
			float num2 = (num ? limiXPlus : limiXMinus);
			float num3 = (flag ? limiZPlus : limiZMinus);
			bool flag2 = false;
			if (num2 <= 1E-07f && num3 <= 1E-07f)
			{
				Vector3 vector = new Vector3(0f, 1f, 0f);
				Vector3 vector2 = vector - dir;
				if (Mathf.Abs(vector2.x) > 1E-07f || Mathf.Abs(vector2.y) > 1E-07f || Mathf.Abs(vector2.z) > 1E-07f)
				{
					dir = vector;
					flag2 = true;
				}
			}
			else
			{
				float num4 = ((num2 >= 1E-07f) ? (1f / num2) : 0f);
				float num5 = ((num3 >= 1E-07f) ? (1f / num3) : 0f);
				float num6 = dir.x * num4;
				float num7 = dir.z * num5;
				float num8 = SAFBIKSqrt(num6 * num6 + num7 * num7 + dir.y * dir.y);
				float num9 = ((num8 > 1E-07f) ? (1f / num8) : 0f);
				float num10 = num6 * num9;
				float num11 = num7 * num9;
				if (num8 > 1f && !flag2)
				{
					flag2 = true;
					num6 = num10;
					num7 = num11;
				}
				float num12 = (flag2 ? (num6 * num2) : dir.x);
				float num13 = (flag2 ? (num7 * num3) : dir.z);
				bool flag3 = dir.y < 0f;
				if (flag2)
				{
					float num14 = SAFBIKSqrt(num12 * num12 + num13 * num13);
					float y = SAFBIKSqrt(1f - num14 * num14);
					dir.x = num12;
					dir.y = y;
					dir.z = num13;
				}
				else if (flag3)
				{
					flag2 = true;
					dir.y = 0f - dir.y;
				}
			}
			return flag2;
		}

		public static bool _LimitYZ(bool isRight, ref Vector3 dir, float limitYMinus, float limitYPlus, float limitZMinus, float limitZPlus)
		{
			bool num = dir.y >= 0f;
			bool flag = dir.z >= 0f;
			float num2 = (num ? limitYPlus : limitYMinus);
			float num3 = (flag ? limitZPlus : limitZMinus);
			bool flag2 = false;
			if (num2 <= 1E-07f && num3 <= 1E-07f)
			{
				Vector3 vector = (isRight ? new Vector3(1f, 0f, 0f) : new Vector3(-1f, 0f, 0f));
				Vector3 vector2 = vector - dir;
				if (Mathf.Abs(vector2.x) > 1E-07f || Mathf.Abs(vector2.y) > 1E-07f || Mathf.Abs(vector2.z) > 1E-07f)
				{
					dir = vector;
					flag2 = true;
				}
			}
			else
			{
				float num4 = ((num2 >= 1E-07f) ? (1f / num2) : 0f);
				float num5 = ((num3 >= 1E-07f) ? (1f / num3) : 0f);
				float num6 = dir.y * num4;
				float num7 = dir.z * num5;
				float num8 = SAFBIKSqrt(dir.x * dir.x + num6 * num6 + num7 * num7);
				float num9 = ((num8 > 1E-07f) ? (1f / num8) : 0f);
				float num10 = num6 * num9;
				float num11 = num7 * num9;
				if (num8 > 1f && !flag2)
				{
					flag2 = true;
					num6 = num10;
					num7 = num11;
				}
				float num12 = (flag2 ? (num6 * num2) : dir.y);
				float num13 = (flag2 ? (num7 * num3) : dir.z);
				bool flag3 = dir.x >= 0f != isRight;
				if (flag2)
				{
					float num14 = SAFBIKSqrt(num12 * num12 + num13 * num13);
					float num15 = SAFBIKSqrt(1f - num14 * num14);
					dir.x = (isRight ? num15 : (0f - num15));
					dir.y = num12;
					dir.z = num13;
				}
				else if (flag3)
				{
					flag2 = true;
					dir.x = 0f - dir.x;
				}
			}
			return flag2;
		}

		public static Vector3 _FitToPlane(Vector3 pos, Vector3 planeDir)
		{
			float num = Vector3.Dot(pos, planeDir);
			if (num <= 1E-07f && num >= -1E-07f)
			{
				return pos;
			}
			return pos - planeDir * num;
		}

		public static bool _FitToPlaneDir(ref Vector3 dir, Vector3 planeDir)
		{
			float num = Vector3.Dot(dir, planeDir);
			if (num <= 1E-07f && num >= -1E-07f)
			{
				return false;
			}
			Vector3 v = dir - planeDir * num;
			if (!SAFBIKVecNormalize(ref v))
			{
				return false;
			}
			dir = v;
			return true;
		}

		public static bool _LimitToPlaneDirY(ref Vector3 dir, Vector3 planeDir, float thetaY)
		{
			float num = Vector3.Dot(dir, planeDir);
			if (num <= 1E-07f && num >= -1E-07f)
			{
				return false;
			}
			if (num <= thetaY && num >= 0f - thetaY)
			{
				return true;
			}
			Vector3 v = dir - planeDir * num;
			float num2 = SAFBIKVecLength(ref v);
			if (num2 <= float.Epsilon)
			{
				return false;
			}
			float num3 = SAFBIKSqrt(1f - thetaY * thetaY);
			v *= num3 / num2;
			dir = v;
			if (num >= 0f)
			{
				dir += planeDir * thetaY;
			}
			else
			{
				dir -= planeDir * thetaY;
			}
			return true;
		}

		private static void _LerpRotateBasis(out Matrix3x3 basis, ref Vector3 axis, float cos, float rate)
		{
			if (rate <= 1E-07f)
			{
				basis = Matrix3x3.identity;
				return;
			}
			if (rate <= 0.9999999f)
			{
				cos = (float)Math.Cos(((cos >= 0.9999999f) ? 0f : ((cos <= -1.0000001f) ? ((float)Math.PI) : ((float)Math.Acos(cos)))) * rate);
			}
			SAFBIKMatSetAxisAngle(out basis, ref axis, cos);
		}

		public static Vector3 _LerpDir(ref Vector3 src, ref Vector3 dst, float r)
		{
			float theta;
			Vector3 axis;
			if (_ComputeThetaAxis(ref src, ref dst, out theta, out axis))
			{
				Matrix3x3 basis;
				_LerpRotateBasis(out basis, ref axis, theta, r);
				Vector3 ret;
				SAFBIKMatMultVec(out ret, ref basis, ref src);
				return ret;
			}
			return dst;
		}

		public static Vector3 _FastLerpDir(ref Vector3 src, ref Vector3 dst, float r)
		{
			if (r <= 1E-07f)
			{
				return src;
			}
			if (r >= 0.9999999f)
			{
				return dst;
			}
			Vector3 vector = src + (dst - src) * r;
			float magnitude = vector.magnitude;
			if (magnitude > 1E-07f)
			{
				return vector * (1f / magnitude);
			}
			return dst;
		}

		public static bool _LimitFingerNotThumb(bool isRight, ref Vector3 dir, ref FastAngle limitYPlus, ref FastAngle limitYMinus, ref FastAngle limitZ)
		{
			bool result = false;
			if (limitZ.cos > 1E-07f && (dir.z < 0f - limitZ.sin || dir.z > limitZ.sin))
			{
				result = true;
				bool flag = dir.z >= 0f;
				float num = SAFBIKSqrt(dir.x * dir.x + dir.y * dir.y);
				if (limitZ.sin <= 1E-07f)
				{
					if (num > 1E-07f)
					{
						dir.z = 0f;
						dir *= 1f / num;
					}
					else
					{
						dir.Set(isRight ? limitZ.cos : (0f - limitZ.cos), 0f, flag ? limitZ.sin : (0f - limitZ.sin));
					}
				}
				else
				{
					float num2 = limitZ.sin * num / limitZ.cos;
					dir.z = (flag ? num2 : (0f - num2));
					float magnitude = dir.magnitude;
					if (magnitude > 1E-07f)
					{
						dir *= 1f / magnitude;
					}
					else
					{
						dir.Set(isRight ? limitZ.cos : (0f - limitZ.cos), 0f, flag ? limitZ.sin : (0f - limitZ.sin));
					}
				}
			}
			bool flag2 = dir.y >= 0f;
			float num3 = (flag2 ? limitYPlus.cos : limitYMinus.cos);
			if ((isRight && dir.x < num3) || (!isRight && dir.x > 0f - num3))
			{
				float num4 = SAFBIKSqrt(1f - (num3 * num3 + dir.z * dir.z));
				dir.x = (isRight ? num3 : (0f - num3));
				dir.y = (flag2 ? num4 : (0f - num4));
			}
			return result;
		}

		public static LimbIKType ToLimbIKType(LimbIKLocation limbIKLocation)
		{
			switch (limbIKLocation)
			{
			case LimbIKLocation.LeftLeg:
				return LimbIKType.Leg;
			case LimbIKLocation.RightLeg:
				return LimbIKType.Leg;
			case LimbIKLocation.LeftArm:
				return LimbIKType.Arm;
			case LimbIKLocation.RightArm:
				return LimbIKType.Arm;
			default:
				return LimbIKType.Max;
			}
		}

		public static Side ToLimbIKSide(LimbIKLocation limbIKLocation)
		{
			switch (limbIKLocation)
			{
			case LimbIKLocation.LeftLeg:
				return Side.Left;
			case LimbIKLocation.RightLeg:
				return Side.Right;
			case LimbIKLocation.LeftArm:
				return Side.Left;
			case LimbIKLocation.RightArm:
				return Side.Right;
			default:
				return Side.Max;
			}
		}

		public static BoneType ToBoneType(BoneLocation boneLocation)
		{
			switch (boneLocation)
			{
			case BoneLocation.Hips:
				return BoneType.Hips;
			case BoneLocation.Neck:
				return BoneType.Neck;
			case BoneLocation.Head:
				return BoneType.Head;
			case BoneLocation.LeftEye:
				return BoneType.Eye;
			case BoneLocation.RightEye:
				return BoneType.Eye;
			case BoneLocation.LeftLeg:
				return BoneType.Leg;
			case BoneLocation.RightLeg:
				return BoneType.Leg;
			case BoneLocation.LeftKnee:
				return BoneType.Knee;
			case BoneLocation.RightKnee:
				return BoneType.Knee;
			case BoneLocation.LeftFoot:
				return BoneType.Foot;
			case BoneLocation.RightFoot:
				return BoneType.Foot;
			case BoneLocation.LeftShoulder:
				return BoneType.Shoulder;
			case BoneLocation.RightShoulder:
				return BoneType.Shoulder;
			case BoneLocation.LeftArm:
				return BoneType.Arm;
			case BoneLocation.RightArm:
				return BoneType.Arm;
			case BoneLocation.LeftElbow:
				return BoneType.Elbow;
			case BoneLocation.RightElbow:
				return BoneType.Elbow;
			case BoneLocation.LeftWrist:
				return BoneType.Wrist;
			case BoneLocation.RightWrist:
				return BoneType.Wrist;
			default:
				if (boneLocation >= BoneLocation.Spine && boneLocation <= BoneLocation.Spine4)
				{
					return BoneType.Spine;
				}
				if (boneLocation >= BoneLocation.LeftArmRoll0 && boneLocation <= BoneLocation.RightArmRoll3)
				{
					return BoneType.ArmRoll;
				}
				if (boneLocation >= BoneLocation.LeftElbowRoll0 && boneLocation <= BoneLocation.RightElbowRoll3)
				{
					return BoneType.ElbowRoll;
				}
				if (boneLocation >= BoneLocation.LeftHandThumb0 && boneLocation <= BoneLocation.RightHandLittleTip)
				{
					return BoneType.HandFinger;
				}
				return BoneType.Max;
			}
		}

		public static Side ToBoneSide(BoneLocation boneLocation)
		{
			switch (boneLocation)
			{
			case BoneLocation.LeftEye:
				return Side.Left;
			case BoneLocation.RightEye:
				return Side.Right;
			case BoneLocation.LeftLeg:
				return Side.Left;
			case BoneLocation.RightLeg:
				return Side.Right;
			case BoneLocation.LeftKnee:
				return Side.Left;
			case BoneLocation.RightKnee:
				return Side.Right;
			case BoneLocation.LeftFoot:
				return Side.Left;
			case BoneLocation.RightFoot:
				return Side.Right;
			case BoneLocation.LeftShoulder:
				return Side.Left;
			case BoneLocation.RightShoulder:
				return Side.Right;
			case BoneLocation.LeftArm:
				return Side.Left;
			case BoneLocation.RightArm:
				return Side.Right;
			case BoneLocation.LeftElbow:
				return Side.Left;
			case BoneLocation.RightElbow:
				return Side.Right;
			case BoneLocation.LeftWrist:
				return Side.Left;
			case BoneLocation.RightWrist:
				return Side.Right;
			default:
				if (boneLocation >= BoneLocation.LeftHandThumb0 && boneLocation <= BoneLocation.LeftHandLittleTip)
				{
					return Side.Left;
				}
				if (boneLocation >= BoneLocation.LeftArmRoll0 && boneLocation <= BoneLocation.LeftArmRoll3)
				{
					return Side.Left;
				}
				if (boneLocation >= BoneLocation.RightArmRoll0 && boneLocation <= BoneLocation.RightArmRoll3)
				{
					return Side.Right;
				}
				if (boneLocation >= BoneLocation.LeftElbowRoll0 && boneLocation <= BoneLocation.LeftElbowRoll3)
				{
					return Side.Left;
				}
				if (boneLocation >= BoneLocation.RightElbowRoll0 && boneLocation <= BoneLocation.RightElbowRoll3)
				{
					return Side.Right;
				}
				if (boneLocation >= BoneLocation.RightHandThumb0 && boneLocation <= BoneLocation.RightHandLittleTip)
				{
					return Side.Right;
				}
				return Side.Max;
			}
		}

		public static FingerType ToFingerType(BoneLocation boneLocation)
		{
			if (boneLocation >= BoneLocation.LeftHandThumb0 && boneLocation <= BoneLocation.LeftHandLittleTip)
			{
				return (FingerType)((int)(boneLocation - 39) / 4);
			}
			if (boneLocation >= BoneLocation.RightHandThumb0 && boneLocation <= BoneLocation.RightHandLittleTip)
			{
				return (FingerType)((int)(boneLocation - 59) / 4);
			}
			return FingerType.Max;
		}

		public static int ToFingerIndex(BoneLocation boneLocation)
		{
			if (boneLocation >= BoneLocation.LeftHandThumb0 && boneLocation <= BoneLocation.LeftHandLittleTip)
			{
				return (int)(boneLocation - 39) % 4;
			}
			if (boneLocation >= BoneLocation.RightHandThumb0 && boneLocation <= BoneLocation.RightHandLittleTip)
			{
				return (int)(boneLocation - 59) % 4;
			}
			return -1;
		}

		public static EffectorType ToEffectorType(EffectorLocation effectorLocation)
		{
			switch (effectorLocation)
			{
			case EffectorLocation.Root:
				return EffectorType.Root;
			case EffectorLocation.Hips:
				return EffectorType.Hips;
			case EffectorLocation.Neck:
				return EffectorType.Neck;
			case EffectorLocation.Head:
				return EffectorType.Head;
			case EffectorLocation.Eyes:
				return EffectorType.Eyes;
			case EffectorLocation.LeftKnee:
				return EffectorType.Knee;
			case EffectorLocation.RightKnee:
				return EffectorType.Knee;
			case EffectorLocation.LeftFoot:
				return EffectorType.Foot;
			case EffectorLocation.RightFoot:
				return EffectorType.Foot;
			case EffectorLocation.LeftArm:
				return EffectorType.Arm;
			case EffectorLocation.RightArm:
				return EffectorType.Arm;
			case EffectorLocation.LeftElbow:
				return EffectorType.Elbow;
			case EffectorLocation.RightElbow:
				return EffectorType.Elbow;
			case EffectorLocation.LeftWrist:
				return EffectorType.Wrist;
			case EffectorLocation.RightWrist:
				return EffectorType.Wrist;
			case EffectorLocation.LeftHandThumb:
			case EffectorLocation.LeftHandIndex:
			case EffectorLocation.LeftHandMiddle:
			case EffectorLocation.LeftHandRing:
			case EffectorLocation.LeftHandLittle:
			case EffectorLocation.RightHandThumb:
			case EffectorLocation.RightHandIndex:
			case EffectorLocation.RightHandMiddle:
			case EffectorLocation.RightHandRing:
			case EffectorLocation.RightHandLittle:
				return EffectorType.HandFinger;
			default:
				return EffectorType.Max;
			}
		}

		public static Side ToEffectorSide(EffectorLocation effectorLocation)
		{
			switch (effectorLocation)
			{
			case EffectorLocation.LeftKnee:
				return Side.Left;
			case EffectorLocation.RightKnee:
				return Side.Right;
			case EffectorLocation.LeftFoot:
				return Side.Left;
			case EffectorLocation.RightFoot:
				return Side.Right;
			case EffectorLocation.LeftArm:
				return Side.Left;
			case EffectorLocation.RightArm:
				return Side.Right;
			case EffectorLocation.LeftElbow:
				return Side.Left;
			case EffectorLocation.RightElbow:
				return Side.Right;
			case EffectorLocation.LeftWrist:
				return Side.Left;
			case EffectorLocation.RightWrist:
				return Side.Right;
			case EffectorLocation.LeftHandThumb:
			case EffectorLocation.LeftHandIndex:
			case EffectorLocation.LeftHandMiddle:
			case EffectorLocation.LeftHandRing:
			case EffectorLocation.LeftHandLittle:
				return Side.Left;
			default:
				if (effectorLocation >= EffectorLocation.RightHandThumb && effectorLocation <= EffectorLocation.RightHandLittle)
				{
					return Side.Right;
				}
				return Side.Max;
			}
		}

		public static string GetEffectorName(EffectorLocation effectorLocation)
		{
			if (effectorLocation == EffectorLocation.Root)
			{
				return "FullBodyIK";
			}
			if (IsHandFingerEffectors(effectorLocation))
			{
				return ToFingerType(effectorLocation).ToString();
			}
			return effectorLocation.ToString();
		}

		public static bool IsHandFingerEffectors(EffectorLocation effectorLocation)
		{
			if (effectorLocation >= EffectorLocation.LeftHandThumb)
			{
				return effectorLocation <= EffectorLocation.RightHandLittle;
			}
			return false;
		}

		public static FingerType ToFingerType(EffectorLocation effectorLocation)
		{
			if (IsHandFingerEffectors(effectorLocation))
			{
				return (FingerType)((int)(effectorLocation - 15) % 5);
			}
			return FingerType.Max;
		}

		public static void SafeNew<TYPE_>(ref TYPE_ obj) where TYPE_ : class, new()
		{
			if (obj == null)
			{
				obj = new TYPE_();
			}
		}

		public static void SafeResize<TYPE_>(ref TYPE_[] objArray, int length)
		{
			if (objArray == null)
			{
				objArray = new TYPE_[length];
			}
			else
			{
				Array.Resize(ref objArray, length);
			}
		}

		public static void PrepareArray<TypeA, TypeB>(ref TypeA[] dstArray, TypeB[] srcArray)
		{
			if (srcArray != null)
			{
				if (dstArray == null || dstArray.Length != srcArray.Length)
				{
					dstArray = new TypeA[srcArray.Length];
				}
			}
			else
			{
				dstArray = null;
			}
		}

		public static void CloneArray<Type>(ref Type[] dstArray, Type[] srcArray)
		{
			if (srcArray != null)
			{
				if (dstArray == null || dstArray.Length != srcArray.Length)
				{
					dstArray = new Type[srcArray.Length];
				}
				for (int i = 0; i < srcArray.Length; i++)
				{
					dstArray[i] = srcArray[i];
				}
			}
			else
			{
				dstArray = null;
			}
		}

		public static void DestroyImmediate(ref Transform transform)
		{
			if (transform != null)
			{
				UnityEngine.Object.DestroyImmediate(transform.gameObject);
				transform = null;
			}
			else
			{
				transform = null;
			}
		}

		public static void DestroyImmediate(ref Transform transform, bool allowDestroyingAssets)
		{
			if (transform != null)
			{
				UnityEngine.Object.DestroyImmediate(transform.gameObject, allowDestroyingAssets);
				transform = null;
			}
			else
			{
				transform = null;
			}
		}

		public static bool CheckAlive<Type>(ref Type obj) where Type : UnityEngine.Object
		{
			if ((UnityEngine.Object)obj != (UnityEngine.Object)null)
			{
				return true;
			}
			obj = null;
			return false;
		}

		public static bool IsParentOfRecusively(Transform parent, Transform child)
		{
			while (child != null)
			{
				if (child.parent == parent)
				{
					return true;
				}
				child = child.parent;
			}
			return false;
		}

		private static Bone _PrepareBone(Bone bone)
		{
			if (bone == null || !bone.transformIsAlive)
			{
				return null;
			}
			return bone;
		}

		private static Bone[] _PrepareBones(Bone leftBone, Bone rightBone)
		{
			if (leftBone != null && rightBone != null && leftBone.transformIsAlive && rightBone.transformIsAlive)
			{
				return new Bone[2] { leftBone, rightBone };
			}
			return null;
		}

		private static bool _ComputeEyesRange(ref Vector3 eyesDir, float rangeTheta)
		{
			if (rangeTheta >= -1E-07f)
			{
				if (eyesDir.z < 0f)
				{
					eyesDir.z = 0f - eyesDir.z;
				}
				return true;
			}
			if (rangeTheta >= -0.9999999f)
			{
				float num = 0f - rangeTheta;
				eyesDir.z += num;
				if (eyesDir.z < 0f)
				{
					eyesDir.z *= 1f / (1f - num);
				}
				else
				{
					eyesDir.z *= 1f / (1f + num);
				}
				float num2 = SAFBIKSqrt(eyesDir.x * eyesDir.x + eyesDir.y * eyesDir.y);
				if (num2 > float.Epsilon)
				{
					float num3 = SAFBIKSqrt(1f - eyesDir.z * eyesDir.z) / num2;
					eyesDir.x *= num3;
					eyesDir.y *= num3;
					return true;
				}
				eyesDir.x = 0f;
				eyesDir.y = 0f;
				eyesDir.z = 1f;
				return false;
			}
			return true;
		}

		public static string _GetAvatarName(Transform rootTransform)
		{
			if (rootTransform != null)
			{
				Animator component = rootTransform.GetComponent<Animator>();
				if (component != null)
				{
					Avatar avatar = component.avatar;
					if (avatar != null)
					{
						return avatar.name;
					}
				}
			}
			return null;
		}

		[Conditional("SAFULLBODYIK_DEBUG")]
		public static void DebugLog(object msg)
		{
			UnityEngine.Debug.Log(msg);
		}

		[Conditional("SAFULLBODYIK_DEBUG")]
		public static void DebugLogWarning(object msg)
		{
			UnityEngine.Debug.LogWarning(msg);
		}

		[Conditional("SAFULLBODYIK_DEBUG")]
		public static void DebugLogError(object msg)
		{
			UnityEngine.Debug.LogError(msg);
		}

		[Conditional("SAFULLBODYIK_DEBUG")]
		public static void Assert(bool cmp)
		{
			if (!cmp)
			{
				UnityEngine.Debug.LogError("Assert");
				UnityEngine.Debug.Break();
			}
		}

		[Conditional("SAFULLBODYIK_DEBUG_CHECKEVAL")]
		public static void CheckNormalized(Vector3 v)
		{
			float num = 0.0001f;
			float num2 = v.x * v.x + v.y * v.y + v.z * v.z;
			if (num2 < 1f - num || num2 > 1f + num)
			{
				UnityEngine.Debug.LogError("CheckNormalized:" + num2.ToString("F6"));
				UnityEngine.Debug.Break();
			}
		}

		[Conditional("SAFULLBODYIK_DEBUG_CHECKEVAL")]
		public static void CheckNaN(float f)
		{
			if (float.IsNaN(f))
			{
				UnityEngine.Debug.LogError("NaN");
			}
		}

		[Conditional("SAFULLBODYIK_DEBUG_CHECKEVAL")]
		public static void CheckNaN(Vector3 v)
		{
			if (float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z))
			{
				UnityEngine.Debug.LogError("NaN:" + v);
			}
		}

		[Conditional("SAFULLBODYIK_DEBUG_CHECKEVAL")]
		public static void CheckNaN(Quaternion q)
		{
			if (float.IsNaN(q.x) || float.IsNaN(q.y) || float.IsNaN(q.z) || float.IsNaN(q.w))
			{
				UnityEngine.Debug.LogError("NaN:" + q);
			}
		}
	}
}
