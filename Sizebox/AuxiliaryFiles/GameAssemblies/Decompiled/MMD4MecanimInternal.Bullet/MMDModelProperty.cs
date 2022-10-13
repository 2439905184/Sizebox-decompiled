using System;
using UnityEngine;

namespace MMD4MecanimInternal.Bullet;

[Serializable]
public class MMDModelProperty
{
	public float worldScale;

	public float importScale;

	public Vector3 lossyScale = Vector3.one;

	public int keepIKTargetBoneFlag = -1;

	public int forceIKResetBoneFlag = -1;

	public int enableIKSecondPassFlag = -1;

	public float secondPassLimitAngle = -1f;

	public int enableIKInnerLockFlag = -1;

	public int enableIKInnerLockKneeFlag = -1;

	public float innerLockKneeClamp = -1f;

	public float innerLockKneeRatioL = -1f;

	public float innerLockKneeRatioU = -1f;

	public float innerLockKneeScale = -1f;

	public int enableIKMuscleFlag = -1;

	public int enableIKMuscleHipFlag = -1;

	public int enableIKMuscleFootFlag = -1;

	public float muscleHipUpperXAngle = -1f;

	public float muscleHipLowerXAngle = -1f;

	public float muscleHipInnerYAngle = -1f;

	public float muscleHipOuterYAngle = -1f;

	public float muscleHipInnerZAngle = -1f;

	public float muscleHipOuterZAngle = -1f;

	public float muscleFootUpperXAngle = -1f;

	public float muscleFootLowerXAngle = -1f;

	public float muscleFootInnerYAngle = -1f;

	public float muscleFootOuterYAngle = -1f;

	public float muscleFootInnerZAngle = -1f;

	public float muscleFootOuterZAngle = -1f;

	public bool rigidBodyIsAdditionalDamping = true;

	public bool rigidBodyIsEnableSleeping;

	public bool rigidBodyIsUseCcd = true;

	public float rigidBodyCcdMotionThreshold = 0.1f;

	public float rigidBodyShapeScale = 1f;

	public float rigidBodyMassRate = 1f;

	public float rigidBodyLinearDampingRate = 1f;

	public float rigidBodyAngularDampingRate = 1f;

	public float rigidBodyRestitutionRate = 1f;

	public float rigidBodyFrictionRate = 1f;

	public float rigidBodyAntiJitterRate = 1f;

	public float rigidBodyAntiJitterRateOnKinematic = 1f;

	public float rigidBodyPreBoneAlignmentLimitLength = 0.01f;

	public float rigidBodyPreBoneAlignmentLossRate = 0.01f;

	public float rigidBodyPostBoneAlignmentLimitLength = 0.01f;

	public float rigidBodyPostBoneAlignmentLossRate = 0.01f;

	public float rigidBodyLinearDampingLossRate = -1f;

	public float rigidBodyLinearDampingLimit = -1f;

	public float rigidBodyAngularDampingLossRate = -1f;

	public float rigidBodyAngularDampingLimit = -1f;

	public float rigidBodyLinearVelocityLimit = -1f;

	public float rigidBodyAngularVelocityLimit = -1f;

	public bool rigidBodyIsUseForceAngularVelocityLimit = true;

	public bool rigidBodyIsUseForceAngularAccelerationLimit = true;

	public float rigidBodyForceAngularVelocityLimit = 600f;

	public bool rigidBodyIsAdditionalCollider;

	public float rigidBodyAdditionalColliderBias = 0.99f;

	public bool rigidBodyIsForceTranslate = true;

	public float jointRootAdditionalLimitAngle = -1f;

	public MMDModelProperty Clone()
	{
		return (MMDModelProperty)MemberwiseClone();
	}

	public void Postfix()
	{
		if (keepIKTargetBoneFlag == -1)
		{
			keepIKTargetBoneFlag = 0;
		}
		if (forceIKResetBoneFlag == -1)
		{
			forceIKResetBoneFlag = 0;
		}
		if (enableIKSecondPassFlag == -1)
		{
			enableIKSecondPassFlag = 0;
		}
		if (secondPassLimitAngle < 0f)
		{
			secondPassLimitAngle = 20f;
		}
		if (enableIKInnerLockFlag == -1)
		{
			enableIKInnerLockFlag = 0;
		}
		if (enableIKInnerLockKneeFlag == -1)
		{
			enableIKInnerLockKneeFlag = 1;
		}
		if (innerLockKneeClamp < 0f)
		{
			innerLockKneeClamp = 0.0625f;
		}
		if (innerLockKneeRatioL < 0f)
		{
			innerLockKneeRatioL = 0.4f;
		}
		if (innerLockKneeRatioU < 0f)
		{
			innerLockKneeRatioU = 0.1f;
		}
		if (innerLockKneeScale < 0f)
		{
			innerLockKneeScale = 8f;
		}
		if (enableIKMuscleFlag == -1)
		{
			enableIKMuscleFlag = 0;
		}
		if (enableIKMuscleHipFlag == -1)
		{
			enableIKMuscleHipFlag = 1;
		}
		if (enableIKMuscleFootFlag == -1)
		{
			enableIKMuscleFootFlag = 1;
		}
		if (muscleHipUpperXAngle < 0f)
		{
			muscleHipUpperXAngle = 176f;
		}
		if (muscleHipLowerXAngle < 0f)
		{
			muscleHipLowerXAngle = 86f;
		}
		if (muscleHipInnerYAngle < 0f)
		{
			muscleHipInnerYAngle = 45f;
		}
		if (muscleHipOuterYAngle < 0f)
		{
			muscleHipOuterYAngle = 90f;
		}
		if (muscleHipInnerZAngle < 0f)
		{
			muscleHipInnerZAngle = 30f;
		}
		if (muscleHipOuterZAngle < 0f)
		{
			muscleHipOuterZAngle = 90f;
		}
		if (muscleFootUpperXAngle < 0f)
		{
			muscleFootUpperXAngle = 70f;
		}
		if (muscleFootLowerXAngle < 0f)
		{
			muscleFootLowerXAngle = 90f;
		}
		if (muscleFootInnerYAngle < 0f)
		{
			muscleFootInnerYAngle = 25f;
		}
		if (muscleFootOuterYAngle < 0f)
		{
			muscleFootOuterYAngle = 25f;
		}
		if (muscleFootInnerZAngle < 0f)
		{
			muscleFootInnerZAngle = 12.5f;
		}
		if (muscleFootOuterZAngle < 0f)
		{
			muscleFootOuterZAngle = 0f;
		}
		secondPassLimitAngle *= (float)System.Math.PI / 180f;
		muscleHipUpperXAngle *= (float)System.Math.PI / 180f;
		muscleHipLowerXAngle *= (float)System.Math.PI / 180f;
		muscleHipInnerYAngle *= (float)System.Math.PI / 180f;
		muscleHipOuterYAngle *= (float)System.Math.PI / 180f;
		muscleHipInnerZAngle *= (float)System.Math.PI / 180f;
		muscleHipOuterZAngle *= (float)System.Math.PI / 180f;
		muscleFootUpperXAngle *= (float)System.Math.PI / 180f;
		muscleFootLowerXAngle *= (float)System.Math.PI / 180f;
		muscleFootInnerYAngle *= (float)System.Math.PI / 180f;
		muscleFootOuterYAngle *= (float)System.Math.PI / 180f;
		muscleFootInnerZAngle *= (float)System.Math.PI / 180f;
		muscleFootOuterZAngle *= (float)System.Math.PI / 180f;
	}
}
