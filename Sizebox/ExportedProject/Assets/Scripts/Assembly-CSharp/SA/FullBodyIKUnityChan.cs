using System;
using UnityEngine;

namespace SA
{
	[Serializable]
	public class FullBodyIKUnityChan : FullBodyIK
	{
		private Vector3 _headBoneLossyScale = Vector3.one;

		private bool _isHeadBoneLossyScaleFuzzyIdentity = true;

		private Quaternion _headToLeftEyeRotation = Quaternion.identity;

		private Quaternion _headToRightEyeRotation = Quaternion.identity;

		private Vector3 _unityChan_leftEyeDefaultPosition = Vector3.zero;

		private Vector3 _unityChan_rightEyeDefaultPosition = Vector3.zero;

		private static Vector3 _unityChan_leftEyeDefaultLocalPosition = new Vector3(-0.018530998f, 0.048524f, 0.027681999f);

		private static Vector3 _unityChan_rightEyeDefaultLocalPosition = new Vector3(0.018530998f, 0.048524f, 0.027681999f);

		private static readonly float _unityChan_eyesHorzLimitTheta = Mathf.Sin(0.6981317f);

		private static readonly float _unityChan_eyesVertLimitTheta = Mathf.Sin((float)Math.PI / 40f);

		private const float _unityChan_eyesYawRate = 0.796f;

		private const float _unityChan_eyesPitchRate = 0.28f;

		private const float _unityChan_eyesYawOuterRate = 0.096f;

		private const float _unityChan_eyesYawInnerRate = 0.065f;

		private const float _unityChan_eyesMoveXInnerRate = 0.0063f;

		private const float _unityChan_eyesMoveXOuterRate = 0.0063f;

		public override bool _IsHiddenCustomEyes()
		{
			return true;
		}

		public override bool _PrepareCustomEyes(ref Quaternion headToLeftEyeRotation, ref Quaternion headToRightEyeRotation)
		{
			Bone bone = ((headBones != null) ? headBones.head : null);
			Bone bone2 = ((headBones != null) ? headBones.leftEye : null);
			Bone bone3 = ((headBones != null) ? headBones.rightEye : null);
			if (bone != null && bone.transformIsAlive && bone2 != null && bone2.transformIsAlive && bone3 != null && bone3.transformIsAlive)
			{
				_headToLeftEyeRotation = headToLeftEyeRotation;
				_headToRightEyeRotation = headToRightEyeRotation;
				Vector3 ret;
				FullBodyIK.SAFBIKMatMultVec(out ret, ref internalValues.defaultRootBasis, ref _unityChan_leftEyeDefaultLocalPosition);
				Vector3 ret2;
				FullBodyIK.SAFBIKMatMultVec(out ret2, ref internalValues.defaultRootBasis, ref _unityChan_rightEyeDefaultLocalPosition);
				_headBoneLossyScale = bone.transform.lossyScale;
				_isHeadBoneLossyScaleFuzzyIdentity = FullBodyIK.IsFuzzy(_headBoneLossyScale, Vector3.one);
				if (!_isHeadBoneLossyScaleFuzzyIdentity)
				{
					ret = FullBodyIK.Scale(ref ret, ref _headBoneLossyScale);
					ret2 = FullBodyIK.Scale(ref ret2, ref _headBoneLossyScale);
				}
				_unityChan_leftEyeDefaultPosition = bone._defaultPosition + ret;
				_unityChan_rightEyeDefaultPosition = bone._defaultPosition + ret2;
			}
			return true;
		}

		public override void _ResetCustomEyes()
		{
			Bone bone = ((headBones != null) ? headBones.neck : null);
			Bone bone2 = ((headBones != null) ? headBones.head : null);
			Bone bone3 = ((headBones != null) ? headBones.leftEye : null);
			Bone bone4 = ((headBones != null) ? headBones.rightEye : null);
			if (bone != null && bone.transformIsAlive && bone2 != null && bone2.transformIsAlive && bone3 != null && bone3.transformIsAlive && bone4 != null && bone4.transformIsAlive)
			{
				Quaternion lhs = bone.worldRotation;
				Vector3 addVec = bone.worldPosition;
				Matrix3x3 ret;
				FullBodyIK.SAFBIKMatSetRotMultInv1(out ret, ref lhs, ref bone._defaultRotation);
				Vector3 ret2;
				FullBodyIK.SAFBIKMatMultVecPreSubAdd(out ret2, ref ret, ref bone2._defaultPosition, ref bone._defaultPosition, ref addVec);
				Quaternion lhs2 = bone2.worldRotation;
				Matrix3x3 ret3;
				FullBodyIK.SAFBIKMatSetRotMultInv1(out ret3, ref lhs2, ref bone2._defaultRotation);
				Vector3 ret4;
				FullBodyIK.SAFBIKMatMultVecPreSubAdd(out ret4, ref ret3, ref bone3._defaultPosition, ref bone2._defaultPosition, ref ret2);
				bone3.worldPosition = ret4;
				Quaternion ret5;
				FullBodyIK.SAFBIKQuatMult(out ret5, ref lhs2, ref _headToLeftEyeRotation);
				bone3.worldRotation = ret5;
				FullBodyIK.SAFBIKMatMultVecPreSubAdd(out ret4, ref ret3, ref bone4._defaultPosition, ref bone2._defaultPosition, ref ret2);
				bone4.worldPosition = ret4;
				FullBodyIK.SAFBIKQuatMult(out ret5, ref lhs2, ref _headToRightEyeRotation);
				bone4.worldRotation = ret5;
			}
		}

		public override void _SolveCustomEyes(ref Matrix3x3 neckBasis, ref Matrix3x3 headBasis, ref Matrix3x3 headBaseBasis)
		{
			Bone bone = ((headBones != null) ? headBones.neck : null);
			Bone bone2 = ((headBones != null) ? headBones.head : null);
			Bone bone3 = ((headBones != null) ? headBones.leftEye : null);
			Bone bone4 = ((headBones != null) ? headBones.rightEye : null);
			Effector effector = ((headEffectors != null) ? headEffectors.eyes : null);
			if (bone == null || !bone.transformIsAlive || bone2 == null || !bone2.transformIsAlive || bone3 == null || !bone3.transformIsAlive || bone4 == null || !bone4.transformIsAlive || effector == null)
			{
				return;
			}
			Vector3 addVec = bone.worldPosition;
			Vector3 ret;
			FullBodyIK.SAFBIKMatMultVecPreSubAdd(out ret, ref neckBasis, ref bone2._defaultPosition, ref bone._defaultPosition, ref addVec);
			Vector3 ret2;
			FullBodyIK.SAFBIKMatMultVecPreSubAdd(out ret2, ref headBasis, ref effector._defaultPosition, ref bone2._defaultPosition, ref ret);
			Vector3 ret3 = effector.worldPosition - ret2;
			Matrix3x3 basis = headBaseBasis;
			Matrix3x3 basis2 = headBaseBasis;
			FullBodyIK.SAFBIKMatMultVecInv(out ret3, ref headBaseBasis, ref ret3);
			if (!FullBodyIK.SAFBIKVecNormalize(ref ret3))
			{
				ret3 = new Vector3(0f, 0f, 1f);
			}
			if (effector.positionWeight < 0.9999999f)
			{
				Vector3 v = Vector3.Lerp(new Vector3(0f, 0f, 1f), ret3, effector.positionWeight);
				if (FullBodyIK.SAFBIKVecNormalize(ref v))
				{
					ret3 = v;
				}
			}
			FullBodyIK._LimitXY_Square(ref ret3, _unityChan_eyesHorzLimitTheta, _unityChan_eyesHorzLimitTheta, _unityChan_eyesVertLimitTheta, _unityChan_eyesVertLimitTheta);
			float num = ret3.x * 0.796f;
			if (num < 0f - _unityChan_eyesHorzLimitTheta)
			{
				num = 0f - _unityChan_eyesHorzLimitTheta;
			}
			else if (num > _unityChan_eyesHorzLimitTheta)
			{
				num = _unityChan_eyesHorzLimitTheta;
			}
			ret3.x *= 0.796f;
			ret3.y *= 0.28f;
			Vector3 v2 = ret3;
			Vector3 v3 = ret3;
			if (ret3.x >= 0f)
			{
				v2.x *= 0.065f;
				v3.x *= 0.096f;
			}
			else
			{
				v2.x *= 0.096f;
				v3.x *= 0.065f;
			}
			FullBodyIK.SAFBIKVecNormalize2(ref v2, ref v3);
			FullBodyIK.SAFBIKMatMultVec(out v2, ref headBaseBasis, ref v2);
			FullBodyIK.SAFBIKMatMultVec(out v3, ref headBaseBasis, ref v3);
			float num2 = ((num >= 0f) ? 0.0063f : 0.0063f);
			float num3 = ((num >= 0f) ? 0.0063f : 0.0063f);
			FullBodyIK.SAFBIKComputeBasisLockZ(out basis, ref headBasis.column0, ref headBasis.column1, ref v2);
			FullBodyIK.SAFBIKComputeBasisLockZ(out basis2, ref headBasis.column0, ref headBasis.column1, ref v3);
			Vector3 lhs = headBaseBasis.column0 * (num2 * num);
			Vector3 lhs2 = headBaseBasis.column0 * (num3 * num);
			if (!_isHeadBoneLossyScaleFuzzyIdentity)
			{
				lhs = FullBodyIK.Scale(ref lhs, ref _headBoneLossyScale);
				lhs2 = FullBodyIK.Scale(ref lhs2, ref _headBoneLossyScale);
			}
			Vector3 ret4;
			FullBodyIK.SAFBIKMatMultVecPreSubAdd(out ret4, ref headBasis, ref _unityChan_leftEyeDefaultPosition, ref bone2._defaultPosition, ref ret);
			lhs += ret4;
			FullBodyIK.SAFBIKMatMultVecPreSubAdd(out ret4, ref headBasis, ref _unityChan_rightEyeDefaultPosition, ref bone2._defaultPosition, ref ret);
			lhs2 += ret4;
			Matrix3x3 ret5;
			FullBodyIK.SAFBIKMatMult(out ret5, ref basis, ref internalValues.defaultRootBasisInv);
			Matrix3x3 ret6;
			FullBodyIK.SAFBIKMatMult(out ret6, ref basis2, ref internalValues.defaultRootBasisInv);
			Vector3 ret7;
			FullBodyIK.SAFBIKMatMultVecPreSubAdd(out ret7, ref ret5, ref bone3._defaultPosition, ref _unityChan_leftEyeDefaultPosition, ref lhs);
			bone3.worldPosition = ret7;
			Quaternion ret8;
			FullBodyIK.SAFBIKMatMultGetRot(out ret8, ref basis, ref bone3._baseToWorldBasis);
			bone3.worldRotation = ret8;
			FullBodyIK.SAFBIKMatMultVecPreSubAdd(out ret7, ref ret6, ref bone4._defaultPosition, ref _unityChan_rightEyeDefaultPosition, ref lhs2);
			bone4.worldPosition = ret7;
			FullBodyIK.SAFBIKMatMultGetRot(out ret8, ref basis2, ref bone4._baseToWorldBasis);
			bone4.worldRotation = ret8;
		}
	}
}
