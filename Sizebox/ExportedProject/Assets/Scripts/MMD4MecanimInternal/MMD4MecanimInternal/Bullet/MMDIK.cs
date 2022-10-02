using System;
using BulletXNA.LinearMath;
using UnityEngine;

namespace MMD4MecanimInternal.Bullet
{
	public class MMDIK
	{
		private enum IKLinkLimitType
		{
			X = 0,
			Y = 1,
			Z = 2,
			Free = 3
		}

		private enum IKLinkFreeLimitType
		{
			X = 0,
			Y = 1,
			Z = 2
		}

		[Flags]
		public enum IKLinkFlags
		{
			HasLimit = 1
		}

		private struct IKLink
		{
			public MMDBone bone;

			public uint ikLinkFlags;

			public IndexedVector3 lowerLimit;

			public IndexedVector3 upperLimit;

			public IndexedVector3 savedAngle;

			public IndexedVector3 savedDirection;

			public IndexedQuaternion savedRotation;

			public IKLinkLimitType ikLinkLimitType;

			public IKLinkFreeLimitType ikLinkFreeLimitType;

			public bool hasLimit
			{
				get
				{
					return (ikLinkFlags & 1) != 0;
				}
			}
		}

		public const float MMDIK_SKIP_ANGLE = 1.7453292E-06f;

		private const float MaxAngle = (float)System.Math.PI * 2f;

		private const float IKLimitOuterAngle = (float)System.Math.PI;

		private const float IKLimitInnerAngle = 3.0717795f;

		private const uint MinIteration = 6u;

		public MMDModel _model;

		private MMDBone _destBone;

		private MMDBone _targetBone;

		private uint _iteration;

		private float _angleConstraint;

		private IKLink[] _ikLinkList;

		private float _ikWeight = 1f;

		private bool _isDisabled;

		private bool _isKnee;

		private bool _isSavedResults;

		public float ikWeight
		{
			get
			{
				return _ikWeight;
			}
			set
			{
				_ikWeight = Mathf.Clamp01(value);
			}
		}

		public bool isDisabled
		{
			get
			{
				return _isDisabled;
			}
			set
			{
				_isDisabled = value;
			}
		}

		public void Destroy()
		{
			_destBone = null;
			_targetBone = null;
			_ikLinkList = null;
		}

		public void PreUpdate_MarkIKDepended(uint updateFlags, float ikWeight)
		{
			if ((updateFlags & (true ? 1u : 0u)) != 0 && _destBone != null && !(ikWeight <= 0f))
			{
				_destBone.PreUpdate_MarkIKDepended(false, false, true);
				_targetBone.PreUpdate_MarkIKDepended(false, true, false);
				for (int i = 0; i < _ikLinkList.Length; i++)
				{
					_ikLinkList[i].bone.PreUpdate_MarkIKDepended(true, false, false);
				}
			}
		}

		private static void _InnerLockR(ref float lowerAngle, ref float upperAngle, float innerLockScale)
		{
			float num = Mathf.Max((upperAngle - lowerAngle) * innerLockScale, 0f);
			float num2 = lowerAngle + num;
			float num3 = upperAngle - num;
			lowerAngle = num2;
			upperAngle = num3;
		}

		private static void _InnerLockR(ref IndexedVector3 lowerAngle, ref IndexedVector3 upperAngle, float innerLockScale)
		{
			_InnerLockR(ref lowerAngle.X, ref upperAngle.X, innerLockScale);
			_InnerLockR(ref lowerAngle.Y, ref upperAngle.Y, innerLockScale);
			_InnerLockR(ref lowerAngle.Z, ref upperAngle.Z, innerLockScale);
		}

		private static float _ClampEuler(float r, float lower, float upper, bool inverse)
		{
			if (r < lower)
			{
				if (inverse)
				{
					float num = lower * 2f - r;
					if (num <= upper)
					{
						return num;
					}
				}
				return lower;
			}
			if (r > upper)
			{
				if (inverse)
				{
					float num2 = upper * 2f - r;
					if (num2 >= lower)
					{
						return num2;
					}
				}
				return upper;
			}
			return r;
		}

		private static IndexedVector3 _ClampEuler(ref IndexedVector3 r, ref IndexedVector3 lower, ref IndexedVector3 upper, bool inverse)
		{
			IndexedVector3 zero = IndexedVector3.Zero;
			for (int i = 0; i != 3; i++)
			{
				zero[i] = _ClampEuler(r[i], lower[i], upper[i], inverse);
			}
			return zero;
		}

		private static bool _Normalize(ref IndexedVector3 v)
		{
			float num = v.Length();
			if (num > float.Epsilon)
			{
				v *= 1f / num;
				return true;
			}
			return false;
		}

		private static IndexedVector3 _ComputeEulerZYX(ref IndexedBasisMatrix m)
		{
			float roll = 0f;
			float pitch = 0f;
			float yaw = 0f;
			Math.ComputeEulerZYX(ref m, ref yaw, ref pitch, ref roll);
			return new IndexedVector3(roll, pitch, yaw);
		}

		private static IndexedQuaternion _GetRotationEulerZYX(ref IndexedVector3 r)
		{
			return Math.EulerZYX(r.X, r.Y, r.Z).GetRotation();
		}

		private static bool _GetIKMuscle(MMDModel model, MMDBone ikBone, ref IndexedVector3 xyzMin, ref IndexedVector3 xyzMax)
		{
			if (model._modelProperty.enableIKMuscleFootFlag != 0 && ikBone._isFoot)
			{
				float muscleFootUpperXAngle = model._modelProperty.muscleFootUpperXAngle;
				float muscleFootLowerXAngle = model._modelProperty.muscleFootLowerXAngle;
				float muscleFootInnerYAngle = model._modelProperty.muscleFootInnerYAngle;
				float muscleFootOuterYAngle = model._modelProperty.muscleFootOuterYAngle;
				float muscleFootInnerZAngle = model._modelProperty.muscleFootInnerZAngle;
				float muscleFootOuterZAngle = model._modelProperty.muscleFootOuterZAngle;
				if (ikBone._isLeft)
				{
					xyzMin.X = 0f - muscleFootUpperXAngle;
					xyzMax.X = muscleFootLowerXAngle;
					xyzMin.Y = 0f - muscleFootInnerYAngle;
					xyzMax.Y = muscleFootOuterYAngle;
					xyzMin.Z = 0f - muscleFootInnerZAngle;
					xyzMax.Z = muscleFootOuterZAngle;
				}
				else
				{
					xyzMin.X = 0f - muscleFootUpperXAngle;
					xyzMax.X = muscleFootLowerXAngle;
					xyzMin.Y = 0f - muscleFootOuterYAngle;
					xyzMax.Y = muscleFootInnerYAngle;
					xyzMin.Z = 0f - muscleFootOuterZAngle;
					xyzMax.Z = muscleFootInnerZAngle;
				}
				return true;
			}
			if (model._modelProperty.enableIKMuscleHipFlag != 0 && ikBone._isHip)
			{
				float muscleHipUpperXAngle = model._modelProperty.muscleHipUpperXAngle;
				float muscleHipLowerXAngle = model._modelProperty.muscleHipLowerXAngle;
				float muscleHipInnerYAngle = model._modelProperty.muscleHipInnerYAngle;
				float muscleHipOuterYAngle = model._modelProperty.muscleHipOuterYAngle;
				float muscleHipInnerZAngle = model._modelProperty.muscleHipInnerZAngle;
				float muscleHipOuterZAngle = model._modelProperty.muscleHipOuterZAngle;
				if (ikBone._isLeft)
				{
					xyzMin.X = 0f - muscleHipUpperXAngle;
					xyzMax.X = muscleHipLowerXAngle;
					xyzMin.Y = 0f - muscleHipInnerYAngle;
					xyzMax.Y = muscleHipOuterYAngle;
					xyzMin.Z = 0f - muscleHipInnerZAngle;
					xyzMax.Z = muscleHipOuterZAngle;
				}
				else
				{
					xyzMin.X = 0f - muscleHipUpperXAngle;
					xyzMax.X = muscleHipLowerXAngle;
					xyzMin.Y = 0f - muscleHipOuterYAngle;
					xyzMax.Y = muscleHipInnerYAngle;
					xyzMin.Z = 0f - muscleHipOuterZAngle;
					xyzMax.Z = muscleHipInnerZAngle;
				}
				return true;
			}
			return false;
		}

		private static bool _GetSecondPassLimitAngle(MMDModel model, IKLink ikLink, ref IndexedVector3 xyzMin, ref IndexedVector3 xyzMax)
		{
			xyzMin = new IndexedVector3((float)System.Math.PI * -2f, (float)System.Math.PI * -2f, (float)System.Math.PI * -2f);
			xyzMax = new IndexedVector3((float)System.Math.PI * 2f, (float)System.Math.PI * 2f, (float)System.Math.PI * 2f);
			if (model == null)
			{
				return false;
			}
			float secondPassLimitAngle = model._modelProperty.secondPassLimitAngle;
			xyzMin = (xyzMax = ikLink.savedAngle);
			for (int i = 0; i != 3; i++)
			{
				xyzMin[i] -= secondPassLimitAngle;
				xyzMax[i] += secondPassLimitAngle;
			}
			return true;
		}

		private static void _ContainMinMax(ref IndexedVector3 xyzMin, ref IndexedVector3 xyzMax, ref IndexedVector3 xyzMin2, ref IndexedVector3 xyzMax2)
		{
			for (int i = 0; i != 3; i++)
			{
				xyzMin[i] = Mathf.Max(xyzMin[i], xyzMin2[i]);
				xyzMax[i] = Mathf.Min(xyzMax[i], xyzMax2[i]);
				if (xyzMin[i] > xyzMax[i])
				{
					if (Mathf.Abs(xyzMin[i]) > Mathf.Abs(xyzMax[i]))
					{
						xyzMin[i] = xyzMax[i];
					}
					else
					{
						xyzMax[i] = xyzMin[i];
					}
				}
			}
		}

		public bool Import(BinaryReader binaryReader)
		{
			Destroy();
			if (!binaryReader.BeginStruct())
			{
				Debug.LogError("BeginStruct() failed.");
				return false;
			}
			int num = 0;
			binaryReader.ReadStructInt();
			_destBone = _model.GetBone(binaryReader.ReadStructInt());
			_targetBone = _model.GetBone(binaryReader.ReadStructInt());
			_iteration = (uint)binaryReader.ReadStructInt();
			_angleConstraint = binaryReader.ReadStructFloat();
			num = Mathf.Max(binaryReader.ReadStructInt(), 0);
			_ikLinkList = new IKLink[num];
			_iteration = ((_iteration > 6) ? _iteration : 6u);
			if (_targetBone == null || _destBone == null)
			{
				Debug.LogError("bone is null.");
				Destroy();
				return false;
			}
			for (int i = 0; i < num; i++)
			{
				_ikLinkList[i] = default(IKLink);
				_ikLinkList[i].bone = _model.GetBone(binaryReader.ReadInt());
				_ikLinkList[i].ikLinkFlags = (uint)binaryReader.ReadInt();
				_ikLinkList[i].lowerLimit = new IndexedVector3((float)System.Math.PI * -2f, (float)System.Math.PI * -2f, (float)System.Math.PI * -2f);
				_ikLinkList[i].upperLimit = new IndexedVector3((float)System.Math.PI * 2f, (float)System.Math.PI * 2f, (float)System.Math.PI * 2f);
				_ikLinkList[i].savedAngle = IndexedVector3.Zero;
				_ikLinkList[i].savedDirection = new IndexedVector3(0f, 0f, 1f);
				_ikLinkList[i].savedRotation = IndexedQuaternion.Identity;
				_ikLinkList[i].ikLinkLimitType = IKLinkLimitType.Free;
				_ikLinkList[i].ikLinkFreeLimitType = IKLinkFreeLimitType.X;
				if (_ikLinkList[i].hasLimit)
				{
					_ikLinkList[i].lowerLimit = binaryReader.ReadVector3();
					_ikLinkList[i].upperLimit = binaryReader.ReadVector3();
					Math.GetAngularLimitFromLeftHand(ref _ikLinkList[i].lowerLimit, ref _ikLinkList[i].upperLimit);
					IndexedVector3 lowerLimit = _ikLinkList[i].lowerLimit;
					IndexedVector3 upperLimit = _ikLinkList[i].upperLimit;
					if (Math.FuzzyZero(lowerLimit.Y) && Math.FuzzyZero(upperLimit.Y) && Math.FuzzyZero(lowerLimit.Z) && Math.FuzzyZero(upperLimit.Z))
					{
						_ikLinkList[i].ikLinkLimitType = IKLinkLimitType.X;
					}
					else if (Math.FuzzyZero(lowerLimit.X) && Math.FuzzyZero(upperLimit.X) && Math.FuzzyZero(lowerLimit.Z) && Math.FuzzyZero(upperLimit.Z))
					{
						_ikLinkList[i].ikLinkLimitType = IKLinkLimitType.Y;
					}
					else if (Math.FuzzyZero(lowerLimit.X) && Math.FuzzyZero(upperLimit.X) && Math.FuzzyZero(lowerLimit.Y) && Math.FuzzyZero(upperLimit.Y))
					{
						_ikLinkList[i].ikLinkLimitType = IKLinkLimitType.Z;
					}
					else
					{
						_ikLinkList[i].ikLinkLimitType = IKLinkLimitType.Free;
						if (lowerLimit.X >= -(float)System.Math.PI && upperLimit.X <= (float)System.Math.PI)
						{
							_ikLinkList[i].ikLinkFreeLimitType = IKLinkFreeLimitType.X;
						}
						else if (lowerLimit.Y >= -(float)System.Math.PI && upperLimit.Y <= (float)System.Math.PI)
						{
							_ikLinkList[i].ikLinkFreeLimitType = IKLinkFreeLimitType.Y;
						}
						else
						{
							_ikLinkList[i].ikLinkFreeLimitType = IKLinkFreeLimitType.Z;
						}
					}
				}
				if (_ikLinkList[i].bone == null)
				{
					Debug.LogError("bone is null.");
					Destroy();
					return false;
				}
				_isKnee |= _ikLinkList[i].bone._isKnee;
				if (_model._fileType == MMDFileType.PMD && _ikLinkList[i].bone._isKnee)
				{
					_ikLinkList[i].ikLinkFlags |= 1u;
					_ikLinkList[i].ikLinkLimitType = IKLinkLimitType.X;
					_ikLinkList[i].lowerLimit = new IndexedVector3((float)System.Math.PI / 360f, 0f, 0f);
					_ikLinkList[i].upperLimit = new IndexedVector3((float)System.Math.PI, 0f, 0f);
				}
			}
			if (!binaryReader.EndStruct())
			{
				Debug.LogError("EndStruct() failed.");
				Destroy();
				return false;
			}
			return true;
		}

		public void Solve()
		{
			if (!_model._isIKEnabled || _destBone == null || _isDisabled || _ikWeight == 0f)
			{
				return;
			}
			bool flag = _model._modelProperty.keepIKTargetBoneFlag != 0;
			bool flag2 = _model._modelProperty.enableIKSecondPassFlag != 0;
			IndexedQuaternion localRotation = IndexedQuaternion.Identity;
			if (flag)
			{
				localRotation = _targetBone._localRotation;
			}
			_PrepareIKTransform();
			_SolveInternal(false);
			if (flag2)
			{
				if (_isSavedResults)
				{
					float secondPassLimitAngle = _model._modelProperty.secondPassLimitAngle;
					bool flag3 = false;
					int num = _ikLinkList.Length - 1;
					while (num >= 0 && !flag3)
					{
						if (_ikLinkList[num].bone != null)
						{
							IndexedVector3 savedDirection = _ikLinkList[num].savedDirection;
							IndexedVector3 v = _ikLinkList[num].bone.GetWorldTransform()._basis.GetColumn(2);
							if (savedDirection.Dot(ref v) < Mathf.Cos(secondPassLimitAngle))
							{
								flag3 = true;
							}
						}
						num--;
					}
					if (flag3)
					{
						for (int num2 = _ikLinkList.Length - 1; num2 >= 0; num2--)
						{
							if (_ikLinkList[num2].bone != null)
							{
								_ikLinkList[num2].bone._WriteLocalRotation(ref _ikLinkList[num2].savedRotation);
							}
						}
						for (int num3 = _ikLinkList.Length - 1; num3 >= 0; num3--)
						{
							if (_ikLinkList[num3].bone != null)
							{
								_ikLinkList[num3].bone.PerformTransformFromIK();
							}
						}
						_targetBone.PerformTransformFromIK();
						_SolveInternal(true);
					}
				}
				_isSavedResults = true;
				for (int num4 = _ikLinkList.Length - 1; num4 >= 0; num4--)
				{
					if (_ikLinkList[num4].bone != null)
					{
						IndexedBasisMatrix m = IndexedBasisMatrix.Identity;
						IndexedQuaternion q = _ikLinkList[num4].bone._localRotation;
						m.SetRotation(ref q);
						_ikLinkList[num4].savedAngle = _ComputeEulerZYX(ref m);
						_ikLinkList[num4].savedDirection = _ikLinkList[num4].bone.GetWorldTransform()._basis.GetColumn(2);
						_ikLinkList[num4].savedRotation = q;
					}
				}
			}
			if (flag)
			{
				_targetBone._WriteLocalRotation(ref localRotation);
				_targetBone.PerformTransformFromIK();
			}
			_PostfixIKTransform();
		}

		private void _PrepareIKTransform()
		{
			for (int num = _ikLinkList.Length - 1; num >= 0; num--)
			{
				_ikLinkList[num].bone.PrepareIKTransform(true, _ikWeight);
			}
			_targetBone.PrepareIKTransform(false, _ikWeight);
		}

		private void _PostfixIKTransform()
		{
			for (int num = _ikLinkList.Length - 1; num >= 0; num--)
			{
				_ikLinkList[num].bone.PostfixIKTransform(_ikWeight);
			}
			_targetBone.PostfixIKTransform(_ikWeight);
		}

		private void _SolveInternal(bool secondPass)
		{
			float innerLockKneeAngleR = 0f;
			bool flag = _GetInnerLockKneeAngleR(ref innerLockKneeAngleR);
			IndexedVector3 origin = _destBone.GetWorldTransform()._origin;
			uint num = ((_iteration > 6) ? _iteration : 6u);
			for (uint num2 = 0u; num2 != num; num2++)
			{
				bool flag2 = false;
				for (int i = 0; i != _ikLinkList.Length; i++)
				{
					IndexedVector3 origin2 = _targetBone.GetWorldTransform()._origin;
					MMDBone bone = _ikLinkList[i].bone;
					bool flag3 = _ikLinkList[i].hasLimit;
					IndexedVector3 lowerAngle = _ikLinkList[i].lowerLimit;
					IndexedVector3 upperAngle = _ikLinkList[i].upperLimit;
					bool flag4 = false;
					bool flag5 = false;
					bool flag6 = false;
					if (flag && num2 == 0 && bone._isKnee)
					{
						if (!flag3)
						{
							flag3 = true;
							lowerAngle = new IndexedVector3((float)System.Math.PI / 360f, 0f, 0f);
							upperAngle = new IndexedVector3((float)System.Math.PI, 0f, 0f);
						}
						_InnerLockR(ref lowerAngle, ref upperAngle, innerLockKneeAngleR);
					}
					IndexedVector3 xyzMin = IndexedVector3.Zero;
					IndexedVector3 xyzMax = IndexedVector3.Zero;
					if (_GetIKMuscle(_model, bone, ref xyzMin, ref xyzMax))
					{
						flag3 = true;
						_ContainMinMax(ref lowerAngle, ref upperAngle, ref xyzMin, ref xyzMax);
					}
					if (secondPass)
					{
						IndexedVector3 xyzMin2 = IndexedVector3.Zero;
						IndexedVector3 xyzMax2 = IndexedVector3.Zero;
						if (_GetSecondPassLimitAngle(_model, _ikLinkList[i], ref xyzMin2, ref xyzMax2))
						{
							flag3 = true;
							_ContainMinMax(ref lowerAngle, ref upperAngle, ref xyzMin2, ref xyzMax2);
						}
					}
					if (flag3)
					{
						flag4 = Math.FuzzyZero(lowerAngle[0]) && Math.FuzzyZero(upperAngle[0]);
						flag5 = Math.FuzzyZero(lowerAngle[1]) && Math.FuzzyZero(upperAngle[1]);
						flag6 = Math.FuzzyZero(lowerAngle[2]) && Math.FuzzyZero(upperAngle[2]);
					}
					IndexedVector3 indexedVector = origin;
					IndexedVector3 indexedVector2 = origin2;
					IndexedMatrix indexedMatrix = bone.GetWorldTransform().Inverse();
					indexedVector = indexedMatrix * indexedVector;
					indexedVector2 = indexedMatrix * indexedVector2;
					if (!_Normalize(ref indexedVector) || !_Normalize(ref indexedVector2))
					{
						continue;
					}
					IndexedVector3 v = indexedVector2.Cross(ref indexedVector);
					float value = indexedVector2.Dot(ref indexedVector);
					float num3 = Mathf.Acos(Mathf.Clamp(value, -1f, 1f));
					if (!_Normalize(ref v))
					{
						continue;
					}
					bool flag7 = false;
					if (flag3)
					{
						if (flag5 && flag6)
						{
							num3 *= Mathf.Abs(v.X);
							v = ((!(v.X >= 0f)) ? new IndexedVector3(-1f, 0f, 0f) : new IndexedVector3(1f, 0f, 0f));
							flag7 = true;
						}
						else if (flag4 && flag6)
						{
							num3 *= Mathf.Abs(v.Y);
							v = ((!(v.Y >= 0f)) ? new IndexedVector3(0f, -1f, 0f) : new IndexedVector3(0f, 1f, 0f));
							flag7 = true;
						}
						else if (flag4 && flag5)
						{
							num3 *= Mathf.Abs(v.Z);
							v = ((!(v.Z >= 0f)) ? new IndexedVector3(0f, 0f, -1f) : new IndexedVector3(0f, 0f, 1f));
							flag7 = true;
						}
					}
					if (!flag7 && (bone._pmxBoneFlags & PMXBoneFlag.FixedAxis) != 0)
					{
						IndexedVector3 fixedAxis = bone._fixedAxis;
						if (fixedAxis != IndexedVector3.Zero)
						{
							float num4 = fixedAxis.Dot(ref v);
							num3 *= Mathf.Abs(num4);
							v = ((!(num4 >= 0f)) ? (-fixedAxis) : fixedAxis);
						}
					}
					if (num3 < 1.7453292E-06f)
					{
						continue;
					}
					flag2 = true;
					num3 = Mathf.Min(num3, _angleConstraint);
					IndexedQuaternion rotation = new IndexedQuaternion(v, num3);
					bool inverse = num2 == 0;
					if (flag3)
					{
						IndexedQuaternion q = bone._localRotation * rotation;
						q.Normalize();
						IndexedBasisMatrix m = new IndexedBasisMatrix(q);
						if (flag5 && flag6)
						{
							float r = Math.ComputeEulerX(ref m);
							r = _ClampEuler(r, lowerAngle[0], upperAngle[0], inverse);
							q = Math.QuaternionX(r);
						}
						else if (flag4 && flag6)
						{
							float r2 = Math.ComputeEulerY(ref m);
							r2 = _ClampEuler(r2, lowerAngle[1], upperAngle[1], inverse);
							q = Math.QuaternionY(r2);
						}
						else if (flag4 && flag5)
						{
							float r3 = Math.ComputeEulerZ(ref m);
							r3 = _ClampEuler(r3, lowerAngle[2], upperAngle[2], inverse);
							q = Math.QuaternionZ(r3);
						}
						else
						{
							IndexedVector3 r4 = _ComputeEulerZYX(ref m);
							if (lowerAngle.X >= -(float)System.Math.PI && upperAngle.X <= (float)System.Math.PI)
							{
								r4[0] = Mathf.Clamp(r4[0], -3.0717795f, 3.0717795f);
							}
							if (lowerAngle.Y >= -(float)System.Math.PI && upperAngle.Y <= (float)System.Math.PI)
							{
								r4[1] = Mathf.Clamp(r4[1], -3.0717795f, 3.0717795f);
							}
							if (lowerAngle.Z >= -(float)System.Math.PI && upperAngle.Z <= (float)System.Math.PI)
							{
								r4[2] = Mathf.Clamp(r4[2], -3.0717795f, 3.0717795f);
							}
							r4 = _ClampEuler(ref r4, ref lowerAngle, ref upperAngle, inverse);
							q = _GetRotationEulerZYX(ref r4);
						}
						bone._WriteLocalRotation(ref q);
					}
					else
					{
						bone.ApplyLocalRotationFromIK(ref rotation);
					}
					for (int num5 = i; num5 >= 0; num5--)
					{
						_ikLinkList[num5].bone.PerformTransformFromIK();
					}
					_targetBone.PerformTransformFromIK();
				}
				if (!flag2)
				{
					break;
				}
			}
		}

		private bool _GetInnerLockKneeAngleR(ref float innerLockKneeAngleR)
		{
			if (_model._modelProperty.enableIKInnerLockKneeFlag != 0 && _isKnee)
			{
				float innerLockKneeClamp = _model._modelProperty.innerLockKneeClamp;
				float innerLockKneeRatioL = _model._modelProperty.innerLockKneeRatioL;
				float innerLockKneeRatioU = _model._modelProperty.innerLockKneeRatioU;
				float innerLockKneeScale = _model._modelProperty.innerLockKneeScale;
				innerLockKneeAngleR = _GetInnerLockKneeAngleR(innerLockKneeRatioL, innerLockKneeRatioU, innerLockKneeScale) * innerLockKneeClamp;
				return true;
			}
			innerLockKneeAngleR = 0f;
			return false;
		}

		private float _GetInnerLockKneeAngleR(float innerLockRatioL, float innerLockRatioU, float innerLockScale)
		{
			int num = _ikLinkList.Length;
			if (num == 0)
			{
				return 0f;
			}
			IndexedVector3 origin = _destBone.GetWorldTransform()._origin;
			IndexedVector3 origin2 = _targetBone.GetWorldTransform()._origin;
			IndexedVector3 origin3 = _ikLinkList[num - 1].bone.GetWorldTransform()._origin;
			float num2 = (origin - origin3).LengthSquared();
			float num3 = (origin2 - origin3).LengthSquared();
			float num4 = 0f;
			if (num3 > float.Epsilon)
			{
				if (num2 < num3)
				{
					float num5 = num2 / num3;
					if (num5 > 1f - innerLockRatioL)
					{
						num4 = (num5 - (1f - innerLockRatioL)) / innerLockRatioL;
					}
				}
				else if (num2 - num3 < num3 * innerLockRatioU)
				{
					num4 = 1f - (num2 - num3) / (num3 * innerLockRatioU);
				}
				num4 = Mathf.Clamp(num4 * innerLockScale, 0f, 1f);
			}
			return num4;
		}
	}
}
