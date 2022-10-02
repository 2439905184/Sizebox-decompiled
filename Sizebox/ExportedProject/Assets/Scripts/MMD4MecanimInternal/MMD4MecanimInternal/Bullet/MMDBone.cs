using BulletXNA.LinearMath;
using UnityEngine;

namespace MMD4MecanimInternal.Bullet
{
	public class MMDBone
	{
		public MMDModel _model;

		public int _boneID;

		public int _sortedBoneID;

		public PMDBoneType _pmdBoneType = PMDBoneType.Unknown;

		public MMDBone _modifiedParentBone;

		public MMDBone _originalParentBone;

		public MMDBone _inherenceParentBone;

		public MMDBone _childBone;

		public float _inherenceWeight;

		public IndexedVector3 _baseOrigin = IndexedVector3.Zero;

		public uint _additionalFlags;

		public PMXBoneFlag _pmxBoneFlags;

		public uint _transformLayerID;

		public int _externalID;

		public string _nameJp;

		public string _skeletonName;

		public IndexedVector3 _localAxisX = IndexedVector3.Zero;

		public IndexedVector3 _localAxisZ = IndexedVector3.Zero;

		public IndexedBasisMatrix _localBasis = IndexedBasisMatrix.Identity;

		public IndexedBasisMatrix _localBasisInv = IndexedBasisMatrix.Identity;

		public IndexedVector3 _fixedAxis = IndexedVector3.Zero;

		public IndexedVector3 _offset = IndexedVector3.Zero;

		public IndexedVector3 _unityOffset = IndexedVector3.Zero;

		public float _boneLength;

		public bool _isLeft;

		public bool _isRight;

		public bool _isHip;

		public bool _isKnee;

		public bool _isFoot;

		public bool _isRootBone;

		public MMDIK _ik;

		public IndexedMatrix _worldTransform = IndexedMatrix.Identity;

		public IndexedMatrix _worldTransformBeforePhysics = IndexedMatrix.Identity;

		public IndexedVector3 _localPosition = IndexedVector3.Zero;

		public IndexedQuaternion _localRotation = IndexedQuaternion.Identity;

		public IndexedQuaternion _localRotationBeforeIK = IndexedQuaternion.Identity;

		public bool _isMovingWorldTransform;

		public bool _isComputeTransform;

		public bool _isComputeUnityTransform;

		public bool _isWriteWorldTransform;

		public bool _isWriteLocalPosition;

		public bool _isWriteLocalRotation;

		public bool _isSetUnityWorldTransform;

		public bool _isSetUnityLocalPosition;

		public bool _isSetUnityLocalRotation;

		public bool _isLateUpdatePosition;

		public bool _isLateUpdateRotation;

		public IndexedMatrix _unityWorldTransform;

		public IndexedVector3 _unityLocalPosition;

		public IndexedQuaternion _unityLocalRotation;

		public FastVector3 _userPosition;

		public FastQuaternion _userRotation;

		public FastVector3 _userPosition2;

		public FastQuaternion _userRotation2;

		public FastVector3 _morphPosition;

		public FastQuaternion _morphRotation;

		public FastVector3 _morphPosition2;

		public FastQuaternion _morphRotation2;

		public IndexedBasisMatrix _pphBoneBasis;

		public bool _isMovingOnResetWorld;

		public IndexedMatrix _moveWorldTransform;

		public IndexedVector3 _moveSourcePosition;

		public IndexedVector3 _moveDestPosition;

		public IndexedQuaternion _moveSourceRotation;

		public IndexedQuaternion _moveDestRotation;

		public bool _isKinematicRigidBody;

		public MMDRigidBody _simulatedRigidBody;

		public IndexedMatrix _prevWorldTransform;

		public IndexedMatrix _prevWorldTransform2;

		public bool _isSetPrevWorldTransform;

		public bool _isSetPrevWorldTransform2;

		public uint _updateFlags;

		public uint _updateFlags2;

		public uint _updateBoneFlags;

		public uint _updateBoneFlags2;

		public ulong _updatedBoneTransform;

		public ulong _updateWorldTransform;

		public ulong _updatedWorldTransform;

		public bool _isMorphPositionDepended;

		public bool _isMorphRotationDepended;

		public bool _isMorphPositionDepended2;

		public bool _isMorphRotationDepended2;

		public bool _underIK;

		public bool _underIKLink;

		public uint _preUpdate_updateFlags;

		public uint _preUpdate_updateFlags2;

		public uint _preUpdate_updateBoneFlags;

		public uint _preUpdate_updateBoneFlags2;

		public bool _preUpdate_isCheckUpdated;

		public bool _preUpdate_isCheckUpdated2;

		public bool _preUpdate_isIKLink;

		public bool _preUpdate_isIKTarget;

		public bool _preUpdate_isIKDestination;

		public bool _preUpdate_isXDEFDepended;

		public bool _preUpdate_isMorphPositionDepended;

		public bool _preUpdate_isMorphRotationDepended;

		public bool _preUpdate_isMorphPositionDepended2;

		public bool _preUpdate_isMorphRotationDepended2;

		public bool _preUpdate_isReadLocalPosition;

		public bool _preUpdate_isReadLocalRotation;

		public bool _preUpdate_isReadWorldTransform;

		public bool _preUpdate_isWriteLocalPosition;

		public bool _preUpdate_isWriteLocalRotation;

		public bool _preUpdate_isWriteWorldTransform;

		public bool _preUpdate_isGetLocalPosition;

		public bool _preUpdate_isGetLocalRotation;

		public bool _preUpdate_isGetLocalToWorldMatrix;

		public bool _preUpdate_isGetLocalToWorldMatrixIndirect;

		public bool isLocalAxisEnabled
		{
			get
			{
				if (_model._isLocalAxisEnabled)
				{
					return (_pmxBoneFlags & PMXBoneFlag.LocalAxis) != 0;
				}
				return false;
			}
		}

		public bool isRigidBodyFreezed
		{
			get
			{
				if (_simulatedRigidBody != null)
				{
					return _simulatedRigidBody.isFreezed;
				}
				return false;
			}
		}

		public bool isRigidBodySimulated
		{
			get
			{
				if (_simulatedRigidBody != null)
				{
					return _simulatedRigidBody.isSimulated;
				}
				return false;
			}
		}

		public MMDRigidBodyType rigidBodyType
		{
			get
			{
				if (_simulatedRigidBody == null)
				{
					return MMDRigidBodyType.Kinematics;
				}
				return _simulatedRigidBody.rigidBodyType;
			}
		}

		public bool isRunningRigidBodySimulated
		{
			get
			{
				if (_model._isPhysicsEnabled)
				{
					if (_simulatedRigidBody != null && _simulatedRigidBody.isSimulated)
					{
						return !_simulatedRigidBody.isFreezed;
					}
					return false;
				}
				return false;
			}
		}

		public bool isBoneTranslate
		{
			get
			{
				return (_pmxBoneFlags & PMXBoneFlag.Translate) != 0;
			}
		}

		public bool isTransformAfterPhysics
		{
			get
			{
				return (_pmxBoneFlags & PMXBoneFlag.TransformAfterPhysics) != 0;
			}
		}

		public void Destroy()
		{
			_model = null;
			_modifiedParentBone = null;
			_originalParentBone = null;
			_inherenceParentBone = null;
			_childBone = null;
			_simulatedRigidBody = null;
			_ik = null;
		}

		public bool Import(int boneID, BinaryReader binaryReader)
		{
			if (!binaryReader.BeginStruct())
			{
				Debug.LogError("BeginStruct() failed.");
				return false;
			}
			_boneID = boneID;
			_additionalFlags = (uint)binaryReader.ReadStructInt();
			int index = binaryReader.ReadStructInt();
			binaryReader.ReadStructInt();
			int index2 = binaryReader.ReadStructInt();
			int boneID2 = binaryReader.ReadStructInt();
			_sortedBoneID = binaryReader.ReadStructInt();
			binaryReader.ReadStructInt();
			int boneID3 = binaryReader.ReadStructInt();
			binaryReader.ReadStructInt();
			_baseOrigin = binaryReader.ReadStructVector3();
			_nameJp = binaryReader.GetName(index);
			_skeletonName = binaryReader.GetName(index2);
			if (_nameJp != null)
			{
				_isLeft = _nameJp.Contains("左");
				_isRight = _nameJp.Contains("右");
				_isHip = _nameJp.EndsWith("左足") || _nameJp.EndsWith("右足");
				_isFoot = _nameJp.EndsWith("左足首") || _nameJp.EndsWith("右足首");
			}
			_originalParentBone = _model.GetBone(boneID3);
			_modifiedParentBone = _model.GetBone(boneID2);
			if (_model._fileType == MMDFileType.PMD)
			{
				_pmdBoneType = (PMDBoneType)binaryReader.ReadStructInt();
				int boneID4 = binaryReader.ReadStructInt();
				int boneID5 = binaryReader.ReadStructInt();
				_inherenceWeight = binaryReader.ReadStructFloat();
				if (_pmdBoneType == PMDBoneType.UnderRotate)
				{
					_inherenceParentBone = _model.GetBone(boneID5);
				}
				else if (_pmdBoneType == PMDBoneType.FollowRotate)
				{
					_inherenceParentBone = _model.GetBone(boneID4);
				}
				else if (_pmdBoneType == PMDBoneType.Twist)
				{
					_childBone = _model.GetBone(boneID4);
				}
				if (_pmdBoneType == PMDBoneType.RotateAndMove || _pmdBoneType == PMDBoneType.IKDestination)
				{
					_pmxBoneFlags |= PMXBoneFlag.Translate;
				}
				if (_pmdBoneType == PMDBoneType.UnderRotate || _pmdBoneType == PMDBoneType.FollowRotate)
				{
					_pmxBoneFlags |= PMXBoneFlag.InherenceRotation;
				}
			}
			else if (_model._fileType == MMDFileType.PMX)
			{
				_transformLayerID = (uint)binaryReader.ReadStructInt();
				_pmxBoneFlags = (PMXBoneFlag)binaryReader.ReadStructInt();
				int boneID6 = binaryReader.ReadStructInt();
				_inherenceWeight = binaryReader.ReadStructFloat();
				_externalID = binaryReader.ReadStructInt();
				_localAxisX = binaryReader.ReadStructVector3();
				_localAxisZ = binaryReader.ReadStructVector3();
				_fixedAxis = binaryReader.ReadStructVector3();
				if ((_pmxBoneFlags & (PMXBoneFlag.InherenceRotation | PMXBoneFlag.InherencePosition)) != 0)
				{
					_inherenceParentBone = _model.GetBone(boneID6);
				}
			}
			if (!binaryReader.EndStruct())
			{
				Debug.LogError("EndStruct() failed.");
				return false;
			}
			_isKnee = (_additionalFlags & 1) != 0;
			_isRootBone = (_additionalFlags & 0xFF000000u) == 2147483648u;
			_baseOrigin *= _model._modelToBulletScale;
			_baseOrigin.Z = 0f - _baseOrigin.Z;
			if ((_pmxBoneFlags & PMXBoneFlag.LocalAxis) != 0)
			{
				if (_localAxisX != IndexedVector3.Zero && _localAxisZ != IndexedVector3.Zero)
				{
					_localAxisX.Z = 0f - _localAxisX.Z;
					_localAxisZ.Z = 0f - _localAxisZ.Z;
					IndexedVector3 localAxisX = _localAxisX;
					IndexedVector3 localAxisZ = _localAxisZ;
					localAxisX = -localAxisX;
					localAxisZ = -localAxisZ;
					IndexedVector3 v = _localAxisZ.Cross(ref _localAxisX);
					localAxisZ = localAxisX.Cross(ref v);
					if (Math.Normalize(ref localAxisX) && Math.Normalize(ref v) && Math.Normalize(ref localAxisZ))
					{
						_localBasis = Math.GetBasis(ref localAxisX, ref v, ref localAxisZ);
						_localBasisInv = _localBasis.Transpose();
					}
				}
				else
				{
					_localAxisX = IndexedVector3.Zero;
					_localAxisZ = IndexedVector3.Zero;
				}
			}
			if ((_pmxBoneFlags & PMXBoneFlag.FixedAxis) != 0 && _fixedAxis != IndexedVector3.Zero)
			{
				_fixedAxis.Z = 0f - _fixedAxis.Z;
				if (!Math.Normalize(ref _fixedAxis))
				{
					_fixedAxis = IndexedVector3.Zero;
				}
			}
			_worldTransform._origin = _baseOrigin;
			return true;
		}

		public void PostfixImport()
		{
			if (_originalParentBone != null)
			{
				_offset = _baseOrigin - _originalParentBone._baseOrigin;
			}
			else
			{
				_offset = _baseOrigin;
			}
			if (_pmdBoneType == PMDBoneType.Twist && _childBone != null)
			{
				_pmxBoneFlags |= PMXBoneFlag.FixedAxis;
				_fixedAxis = _childBone._baseOrigin - _baseOrigin;
				if (!Math.Normalize(ref _fixedAxis))
				{
					_fixedAxis = IndexedVector3.Zero;
				}
			}
			_boneLength = _offset.Length();
			if (_originalParentBone == _modifiedParentBone)
			{
				_unityOffset = Math.BulletToUnityPosition(ref _offset, _model._bulletToLocalScale);
			}
			else if (_modifiedParentBone != null)
			{
				_unityOffset = Math.BulletToUnityPosition(_baseOrigin - _modifiedParentBone._baseOrigin, _model._bulletToLocalScale);
			}
			else
			{
				_unityOffset = Math.BulletToUnityPosition(ref _baseOrigin, _model._bulletToLocalScale);
			}
			_localPosition = _offset;
			_unityLocalPosition = _unityOffset;
		}

		public void PostfixImportIK(ref int ikIndex)
		{
			if (_model._fileType == MMDFileType.PMX && (_pmxBoneFlags & PMXBoneFlag.IK) != 0 && _model._ikList != null && ikIndex < _model._ikList.Length)
			{
				_ik = _model._ikList[ikIndex];
				ikIndex++;
			}
		}

		public void CleanupMoveWorldTransform()
		{
			_isMovingOnResetWorld = false;
		}

		public void FeedbackMoveWorldTransform(bool isMovingOnResetWorld)
		{
			_isMovingOnResetWorld = isMovingOnResetWorld;
			if (_isMovingOnResetWorld && _isSetUnityWorldTransform)
			{
				_moveWorldTransform = _worldTransform;
				_isMovingWorldTransform = false;
			}
		}

		public void PrepareMoveWorldTransform()
		{
			if (_isMovingOnResetWorld && !_isMovingWorldTransform)
			{
				_isMovingWorldTransform = true;
				IndexedVector3 zero = IndexedVector3.Zero;
				zero = ((_isRootBone || _model._rootBone == this) ? _moveWorldTransform._origin : ((_model._rootBone == null) ? _baseOrigin : (_baseOrigin + _model._rootBone._worldTransform._origin)));
				if (_originalParentBone != null && _originalParentBone._isMovingOnResetWorld)
				{
					_originalParentBone.PrepareMoveWorldTransform();
					IndexedMatrix indexedMatrix = _originalParentBone._moveWorldTransform.Inverse() * _moveWorldTransform;
					_moveSourcePosition = _offset;
					_moveDestPosition = indexedMatrix._origin;
					_moveSourceRotation = IndexedQuaternion.Identity;
					_moveDestRotation = indexedMatrix.GetRotation();
				}
				else
				{
					_moveSourcePosition = zero;
					_moveDestPosition = _moveWorldTransform._origin;
					_moveSourceRotation = IndexedQuaternion.Identity;
					_moveDestRotation = _moveWorldTransform.GetRotation();
				}
				_worldTransform._basis = IndexedBasisMatrix.Identity;
				_worldTransform._origin = zero;
			}
		}

		public void ResetWorldTransformOnMoving()
		{
			if (_isMovingOnResetWorld)
			{
				_isMovingWorldTransform = false;
			}
		}

		public void PerformMoveWorldTransform(float r)
		{
			if (_isMovingOnResetWorld && !_isMovingWorldTransform)
			{
				_isMovingWorldTransform = true;
				if (r >= 1f)
				{
					_worldTransform = _moveWorldTransform;
				}
				else if (_originalParentBone != null && _originalParentBone._isMovingOnResetWorld)
				{
					_originalParentBone.PerformMoveWorldTransform(r);
					IndexedMatrix identity = IndexedMatrix.Identity;
					identity._origin = IndexedVector3.Lerp(ref _moveSourcePosition, ref _moveDestPosition, r);
					identity._basis.SetRotation(Math.Slerp(ref _moveSourceRotation, ref _moveDestRotation, r));
					_worldTransform = _originalParentBone._worldTransform * identity;
				}
				else
				{
					_worldTransform._origin = IndexedVector3.Lerp(ref _moveSourcePosition, ref _moveDestPosition, r);
					_worldTransform._basis.SetRotation(Math.Slerp(ref _moveSourceRotation, ref _moveDestRotation, r));
				}
			}
		}

		public void PreUpdate_Prepare(uint updateFlags, uint updateBoneFlags)
		{
			_preUpdate_updateFlags2 = _preUpdate_updateFlags;
			_preUpdate_updateFlags = updateFlags;
			_preUpdate_updateBoneFlags2 = _preUpdate_updateBoneFlags;
			_preUpdate_updateBoneFlags = updateBoneFlags;
			_preUpdate_isCheckUpdated = false;
			_preUpdate_isCheckUpdated2 = false;
			_preUpdate_isIKLink = false;
			_preUpdate_isIKTarget = false;
			_preUpdate_isIKDestination = false;
			_preUpdate_isXDEFDepended = false;
			_preUpdate_isMorphPositionDepended2 = _preUpdate_isMorphPositionDepended;
			_preUpdate_isMorphPositionDepended = false;
			_preUpdate_isMorphRotationDepended2 = _preUpdate_isMorphRotationDepended;
			_preUpdate_isMorphRotationDepended = false;
			_preUpdate_isReadLocalPosition = false;
			_preUpdate_isReadLocalRotation = false;
			_preUpdate_isReadWorldTransform = false;
			_preUpdate_isWriteLocalPosition = false;
			_preUpdate_isWriteLocalRotation = false;
			_preUpdate_isWriteWorldTransform = false;
			_preUpdate_isGetLocalPosition = false;
			_preUpdate_isGetLocalRotation = false;
			_preUpdate_isGetLocalToWorldMatrix = false;
			_preUpdate_isGetLocalToWorldMatrixIndirect = false;
		}

		public void PreUpdate_MarkIKDepended(bool isIKLink, bool isIKTarget, bool isIKDestination)
		{
			_preUpdate_isIKLink |= isIKLink;
			_preUpdate_isIKTarget |= isIKTarget;
			_preUpdate_isIKDestination |= isIKDestination;
		}

		public void PreUpdate_MarkXDEFDepended()
		{
			_preUpdate_isXDEFDepended = true;
		}

		public bool PreUpdate_IsTargetBoneAsUnderIK()
		{
			if (_pmdBoneType == PMDBoneType.UnderRotate && _inherenceParentBone != null)
			{
				return _inherenceParentBone._preUpdate_isIKLink;
			}
			return false;
		}

		public bool PreUpdate_IsParentBoneAsUnderIK()
		{
			for (MMDBone originalParentBone = _originalParentBone; originalParentBone != null; originalParentBone = originalParentBone._originalParentBone)
			{
				if (originalParentBone._pmdBoneType == PMDBoneType.UnderRotate && originalParentBone._inherenceParentBone != null && originalParentBone._inherenceParentBone._preUpdate_isIKLink)
				{
					return true;
				}
			}
			return false;
		}

		public void PreUpdate_MarkMorphPositionDepended()
		{
			_preUpdate_isMorphPositionDepended = true;
		}

		public bool PreUpdate_IsSimulatedPhysics()
		{
			if (_model._isPhysicsEnabled && _simulatedRigidBody != null)
			{
				return (_simulatedRigidBody.preUpdate_updateRigidBodyFlags & 1) == 0;
			}
			return false;
		}

		public void PreUpdate_MarkMorphRotationDepended()
		{
			_preUpdate_isMorphRotationDepended = true;
		}

		public void PreUpdate_CheckUpdated()
		{
			if (_preUpdate_isCheckUpdated)
			{
				return;
			}
			_preUpdate_isCheckUpdated = true;
			if (_originalParentBone != null && !_originalParentBone._preUpdate_isCheckUpdated)
			{
				_originalParentBone.PreUpdate_CheckUpdated();
			}
			uint preUpdate_updateFlags = _preUpdate_updateFlags;
			bool isPhysicsEnabled = _model._isPhysicsEnabled;
			bool flag = (preUpdate_updateFlags & 1) != 0;
			bool flag2 = (preUpdate_updateFlags & 2) != 0;
			bool flag3 = (preUpdate_updateFlags & 4) != 0;
			bool isXDEFEnabled = _model._isXDEFEnabled;
			bool flag4 = (preUpdate_updateFlags & 8) != 0;
			if (flag2 && _inherenceParentBone != null)
			{
				bool flag5 = (_pmxBoneFlags & PMXBoneFlag.InherencePosition) != 0;
				bool flag6 = (_pmxBoneFlags & PMXBoneFlag.InherenceRotation) != 0;
				if (flag5 || flag6)
				{
					_preUpdate_isWriteLocalPosition |= flag5;
					_preUpdate_isWriteLocalRotation |= flag6;
					if (!_inherenceParentBone.PreUpdate_IsSimulatedPhysics())
					{
						if ((_pmxBoneFlags & PMXBoneFlag.InherenceLocal) == 0)
						{
							_inherenceParentBone._preUpdate_isReadLocalPosition |= flag5;
							_inherenceParentBone._preUpdate_isReadLocalRotation |= flag6;
						}
						else
						{
							_inherenceParentBone._preUpdate_isReadWorldTransform = true;
						}
					}
				}
			}
			if (flag)
			{
				if (_preUpdate_isIKDestination)
				{
					_preUpdate_isReadWorldTransform = true;
				}
				if (_preUpdate_isIKLink || _preUpdate_isIKTarget)
				{
					_preUpdate_isReadLocalPosition = true;
					_preUpdate_isReadLocalRotation = true;
					_preUpdate_isWriteLocalRotation = true;
				}
			}
			if (isPhysicsEnabled)
			{
				if (_isKinematicRigidBody || (_simulatedRigidBody != null && (_simulatedRigidBody.preUpdate_updateRigidBodyFlags & (true ? 1u : 0u)) != 0))
				{
					_preUpdate_isReadWorldTransform = true;
				}
				else if (_simulatedRigidBody != null && _originalParentBone != null && !_originalParentBone.PreUpdate_IsSimulatedPhysics())
				{
					_originalParentBone._preUpdate_isReadWorldTransform = true;
				}
			}
			bool flag7 = (flag3 && _preUpdate_isMorphPositionDepended) || _preUpdate_isMorphPositionDepended2;
			bool flag8 = (flag3 && _preUpdate_isMorphRotationDepended) || _preUpdate_isMorphRotationDepended2;
			flag7 |= ((_preUpdate_updateBoneFlags | _preUpdate_updateBoneFlags2) & 0x80) != 0;
			flag8 |= ((_preUpdate_updateBoneFlags | _preUpdate_updateBoneFlags2) & 0x100) != 0;
			if (flag7 || flag8)
			{
				_preUpdate_isWriteLocalPosition |= flag7;
				_preUpdate_isWriteLocalRotation |= flag8;
			}
			if (isXDEFEnabled && _preUpdate_isXDEFDepended)
			{
				_preUpdate_isReadWorldTransform = true;
			}
			if (flag4 || (_preUpdate_updateFlags2 | 8u) != 0)
			{
				uint num = _preUpdate_updateBoneFlags & 0xFF000000u;
				if (num == 16777216 || num == 50331648)
				{
					_preUpdate_isReadLocalRotation = true;
					_preUpdate_isWriteLocalRotation = true;
				}
				if (num == 33554432 || num == 67108864)
				{
					_preUpdate_isReadWorldTransform = true;
					_preUpdate_isWriteLocalRotation = true;
					if (_originalParentBone != null)
					{
						_originalParentBone._preUpdate_isReadWorldTransform = true;
					}
				}
			}
			if (_model._rootBone == this)
			{
				_preUpdate_isReadWorldTransform = true;
			}
			if (_model._fileType != MMDFileType.PMD || PreUpdate_IsSimulatedPhysics() || _originalParentBone == null)
			{
				return;
			}
			bool flag9 = false;
			if (_originalParentBone.PreUpdate_IsSimulatedPhysics())
			{
				flag9 = true;
			}
			if ((_originalParentBone._preUpdate_isIKLink || _originalParentBone._preUpdate_isIKTarget) && !PreUpdate_IsTargetBoneAsUnderIK())
			{
				flag9 = true;
			}
			if (_originalParentBone._sortedBoneID > _sortedBoneID && (_originalParentBone._preUpdate_isWriteLocalPosition || _originalParentBone._preUpdate_isWriteLocalRotation))
			{
				flag9 = true;
			}
			if (flag9)
			{
				_preUpdate_isReadLocalPosition = true;
				_preUpdate_isReadLocalRotation = true;
				_preUpdate_isWriteLocalPosition = true;
				_preUpdate_isWriteLocalRotation = true;
				if (_originalParentBone != null)
				{
					_originalParentBone._preUpdate_isReadWorldTransform = true;
				}
			}
		}

		public void PreUpdate_CheckUpdated2()
		{
			if (_preUpdate_isCheckUpdated2)
			{
				return;
			}
			_preUpdate_isCheckUpdated2 = true;
			if (_originalParentBone != null && !_originalParentBone._preUpdate_isCheckUpdated2)
			{
				_originalParentBone.PreUpdate_CheckUpdated2();
			}
			if (_modifiedParentBone != null && !_modifiedParentBone._preUpdate_isCheckUpdated2)
			{
				_modifiedParentBone.PreUpdate_CheckUpdated2();
			}
			if (_preUpdate_isWriteLocalPosition || _preUpdate_isWriteLocalRotation)
			{
				_preUpdate_isWriteWorldTransform = true;
			}
			if (_originalParentBone != null && _originalParentBone._preUpdate_isWriteWorldTransform)
			{
				_preUpdate_isWriteWorldTransform = true;
			}
			if (_modifiedParentBone != null && _modifiedParentBone._preUpdate_isWriteWorldTransform)
			{
				_preUpdate_isWriteWorldTransform = true;
			}
			if ((!_model._isLocalAxisEnabled || ((_pmxBoneFlags & PMXBoneFlag.LocalAxis) == 0 && (_originalParentBone == null || (_originalParentBone._pmxBoneFlags & PMXBoneFlag.LocalAxis) == 0))) && _originalParentBone == _modifiedParentBone)
			{
				_preUpdate_isGetLocalToWorldMatrix = _preUpdate_isReadWorldTransform;
				_preUpdate_isGetLocalPosition = _preUpdate_isReadLocalPosition;
				_preUpdate_isGetLocalRotation = _preUpdate_isReadLocalRotation;
				if (!_preUpdate_isWriteLocalPosition || !_preUpdate_isWriteLocalRotation)
				{
					if (_preUpdate_isWriteLocalPosition)
					{
						_preUpdate_isGetLocalRotation = true;
					}
					else if (_preUpdate_isWriteLocalRotation)
					{
						_preUpdate_isGetLocalPosition = true;
					}
				}
				if (_originalParentBone != null && _originalParentBone._preUpdate_isWriteWorldTransform)
				{
					_preUpdate_isGetLocalPosition = true;
					_preUpdate_isGetLocalRotation = true;
				}
				if ((_preUpdate_isWriteWorldTransform || PreUpdate_IsSimulatedPhysics()) && _originalParentBone != null && !_originalParentBone._preUpdate_isGetLocalToWorldMatrix && !_originalParentBone._preUpdate_isGetLocalToWorldMatrixIndirect)
				{
					_originalParentBone._preUpdate_isGetLocalToWorldMatrix = true;
				}
			}
			else
			{
				if (_preUpdate_isReadWorldTransform || _preUpdate_isReadLocalPosition || _preUpdate_isReadLocalRotation || _preUpdate_isWriteWorldTransform)
				{
					_preUpdate_isGetLocalToWorldMatrix = true;
				}
				if (_originalParentBone != null && _originalParentBone._preUpdate_isWriteWorldTransform)
				{
					_preUpdate_isGetLocalToWorldMatrix = true;
				}
				if (_modifiedParentBone != null && _modifiedParentBone._preUpdate_isWriteWorldTransform)
				{
					_preUpdate_isGetLocalToWorldMatrix = true;
				}
				if (_preUpdate_isWriteWorldTransform || PreUpdate_IsSimulatedPhysics())
				{
					if (_originalParentBone != null && !_originalParentBone._preUpdate_isGetLocalToWorldMatrix && !_originalParentBone._preUpdate_isGetLocalToWorldMatrixIndirect)
					{
						_originalParentBone._preUpdate_isGetLocalToWorldMatrix = true;
					}
					if (_modifiedParentBone != null && !_modifiedParentBone._preUpdate_isGetLocalToWorldMatrix && !_modifiedParentBone._preUpdate_isGetLocalToWorldMatrixIndirect)
					{
						_modifiedParentBone._preUpdate_isGetLocalToWorldMatrix = true;
					}
				}
			}
			if (_modifiedParentBone != null && (_modifiedParentBone._preUpdate_isGetLocalToWorldMatrix || _modifiedParentBone._preUpdate_isGetLocalToWorldMatrixIndirect) && (_preUpdate_isGetLocalPosition || _preUpdate_isGetLocalRotation || _preUpdate_isGetLocalToWorldMatrix))
			{
				_preUpdate_isGetLocalPosition = true;
				_preUpdate_isGetLocalRotation = true;
				_preUpdate_isGetLocalToWorldMatrix = false;
				_preUpdate_isGetLocalToWorldMatrixIndirect = true;
			}
		}

		public void PrepareUpdate(uint updateFlags, uint updateBoneFlags)
		{
			_isComputeTransform = false;
			_isComputeUnityTransform = false;
			_isWriteWorldTransform = false;
			_isWriteLocalPosition = false;
			_isWriteLocalRotation = false;
			_isSetUnityWorldTransform = false;
			_isSetUnityLocalPosition = false;
			_isSetUnityLocalRotation = false;
			_unityWorldTransform = IndexedMatrix.Identity;
			_unityLocalPosition = _unityOffset;
			_unityLocalRotation = IndexedQuaternion.Identity;
			_userPosition2 = _userPosition;
			_userRotation2 = _userRotation;
			_userPosition = IndexedVector3.Zero;
			_userRotation = IndexedQuaternion.Identity;
			_morphPosition2 = _morphPosition;
			_morphRotation2 = _morphRotation;
			_morphPosition = IndexedVector3.Zero;
			_morphRotation = IndexedQuaternion.Identity;
			_pphBoneBasis = IndexedBasisMatrix.Identity;
			_isMorphPositionDepended2 = _isMorphPositionDepended;
			_isMorphRotationDepended2 = _isMorphRotationDepended;
			_isMorphPositionDepended = false;
			_isMorphRotationDepended = false;
			_underIK = false;
			_underIKLink = false;
			_updateFlags2 = _updateFlags;
			_updateBoneFlags2 = _updateBoneFlags;
			_updateFlags = updateFlags;
			_updateBoneFlags = updateBoneFlags;
			_updatedBoneTransform = 0uL;
			_updateWorldTransform = 0uL;
			_updatedWorldTransform = 0uL;
			_isLateUpdatePosition = false;
			_isLateUpdateRotation = false;
		}

		public bool _IsTargetBoneAsUnderIK()
		{
			if (_pmdBoneType == PMDBoneType.UnderRotate && _inherenceParentBone != null)
			{
				return _inherenceParentBone._underIKLink;
			}
			return false;
		}

		public bool _IsParentBoneAsUnderIK()
		{
			for (MMDBone originalParentBone = _originalParentBone; originalParentBone != null; originalParentBone = originalParentBone._originalParentBone)
			{
				if (originalParentBone._pmdBoneType == PMDBoneType.UnderRotate && originalParentBone._inherenceParentBone != null && originalParentBone._inherenceParentBone._underIKLink)
				{
					return true;
				}
			}
			return false;
		}

		public void SetUnityWorldTransform(ref IndexedMatrix unityWorldTransform)
		{
			_isSetUnityWorldTransform = true;
			_unityWorldTransform = unityWorldTransform;
			if (!_model._isPhysicsEnabled || _simulatedRigidBody == null || !_simulatedRigidBody.isSimulated || _simulatedRigidBody.isFreezed)
			{
				_worldTransform = unityWorldTransform;
				if (_model._isLocalAxisEnabled && (_pmxBoneFlags & PMXBoneFlag.LocalAxis) != 0)
				{
					_worldTransform._basis *= _localBasisInv;
				}
			}
		}

		public void SetUnityLocalPosition(ref IndexedVector3 unityLocalPosition)
		{
			_isSetUnityLocalPosition = true;
			_unityLocalPosition = unityLocalPosition;
		}

		public void SetUnityLocalRotation(ref IndexedQuaternion unityLocalRotation)
		{
			_isSetUnityLocalRotation = true;
			_unityLocalRotation = unityLocalRotation;
		}

		public void SetUserTransform(ref IndexedVector3 userPosition, ref IndexedQuaternion userRotation)
		{
			_userPosition = userPosition;
			_userRotation = userRotation;
		}

		public void AppendMorphPosition(ref FastVector3 morphPosition)
		{
			_morphPosition += morphPosition;
		}

		public void AppendMorphRotation(ref FastQuaternion morphRotation)
		{
			_morphRotation *= morphRotation;
		}

		public void MarkMorphPositionDepended()
		{
			_isMorphPositionDepended = true;
		}

		public void MarkMorphRotationDepended()
		{
			_isMorphRotationDepended = true;
		}

		public void _UpdateWorldTransform()
		{
			_worldTransform.SetRotation(ref _localRotation);
			_worldTransform._origin = _localPosition;
			if (_originalParentBone != null)
			{
				_worldTransform = _originalParentBone.GetWorldTransform() * _worldTransform;
			}
		}

		public void _AddUpdateWorldTransform()
		{
			if (_model._fileType != MMDFileType.PMD)
			{
				_model._updateBoneTransform++;
				_updateWorldTransform++;
			}
		}

		public ulong _GetUpdateWorldTransform()
		{
			if (_model._fileType == MMDFileType.PMD)
			{
				return 0uL;
			}
			if (_originalParentBone != null)
			{
				return _updateWorldTransform + _originalParentBone._GetUpdateWorldTransform();
			}
			return _updateWorldTransform;
		}

		public void _CheckUpdateWorldTransform()
		{
			if (_model._fileType != MMDFileType.PMD && _updatedBoneTransform != _model._updateBoneTransform)
			{
				_updatedBoneTransform = _model._updateBoneTransform;
				ulong num = _GetUpdateWorldTransform();
				if (_updatedWorldTransform != num)
				{
					_updatedWorldTransform = num;
					_UpdateWorldTransform();
				}
			}
		}

		public IndexedMatrix GetWorldTransform()
		{
			if (_model._fileType != MMDFileType.PMD && _updatedBoneTransform != _model._updateBoneTransform)
			{
				_updatedBoneTransform = _model._updateBoneTransform;
				ulong num = _GetUpdateWorldTransform();
				if (_updatedWorldTransform != num)
				{
					_updatedWorldTransform = num;
					_UpdateWorldTransform();
				}
			}
			return _worldTransform;
		}

		public void _WriteTransform(ref IndexedMatrix worldTransform, ref IndexedVector3 localPosition, ref IndexedQuaternion localRotation)
		{
			_worldTransform = worldTransform;
			_localPosition = localPosition;
			_localRotation = localRotation;
			_isWriteWorldTransform = true;
			_isWriteLocalPosition = true;
			_isWriteLocalRotation = true;
		}

		public void _WriteWorldTransform(ref IndexedMatrix worldTransform)
		{
			_worldTransform = worldTransform;
			if (_originalParentBone != null)
			{
				IndexedMatrix worldTransform2 = _originalParentBone.GetWorldTransform();
				IndexedBasisMatrix indexedBasisMatrix = worldTransform2._basis.Transpose();
				_localRotation = (indexedBasisMatrix * _worldTransform._basis).GetRotation();
				_localPosition = indexedBasisMatrix * (_worldTransform._origin - worldTransform2._origin);
			}
			else
			{
				IndexedBasisMatrix indexedBasisMatrix2 = _model._modelTransform._basis.Transpose();
				_localRotation = (indexedBasisMatrix2 * _worldTransform._basis).GetRotation();
				_localPosition = indexedBasisMatrix2 * (_worldTransform._origin - _model._modelTransform._origin);
			}
			_isWriteWorldTransform = true;
			_isWriteLocalPosition = true;
			_isWriteLocalRotation = true;
			if (_model._fileType == MMDFileType.PMX)
			{
				_model._updateBoneTransform++;
				_updateWorldTransform++;
			}
		}

		public void _WriteWorldBasis(ref IndexedBasisMatrix worldBasis)
		{
			_worldTransform._basis = worldBasis;
			if (_originalParentBone != null)
			{
				IndexedBasisMatrix indexedBasisMatrix = _originalParentBone.GetWorldTransform()._basis.Transpose();
				_localRotation = (indexedBasisMatrix * _worldTransform._basis).GetRotation();
			}
			else
			{
				IndexedBasisMatrix indexedBasisMatrix2 = _model._modelTransform._basis.Transpose();
				_localRotation = (indexedBasisMatrix2 * _worldTransform._basis).GetRotation();
			}
			_isWriteWorldTransform = true;
			_isWriteLocalPosition = true;
			_isWriteLocalRotation = true;
			if (_model._fileType == MMDFileType.PMX)
			{
				_model._updateBoneTransform++;
				_updateWorldTransform++;
			}
		}

		public void _WriteLocalTransform(ref IndexedVector3 localPosition, ref IndexedQuaternion localRotation)
		{
			_localPosition = localPosition;
			_localRotation = localRotation;
			_isWriteLocalPosition = true;
			_isWriteLocalRotation = true;
			_isWriteWorldTransform = true;
			if (_model._fileType == MMDFileType.PMD)
			{
				_UpdateWorldTransform();
			}
			else if (_model._fileType == MMDFileType.PMX)
			{
				_model._updateBoneTransform++;
				_updateWorldTransform++;
			}
		}

		public void _WriteLocalPosition(ref IndexedVector3 localPosition)
		{
			_localPosition = localPosition;
			_isWriteLocalPosition = true;
			_isWriteWorldTransform = true;
			if (_model._fileType == MMDFileType.PMD)
			{
				_UpdateWorldTransform();
			}
			else if (_model._fileType == MMDFileType.PMX)
			{
				_model._updateBoneTransform++;
				_updateWorldTransform++;
			}
		}

		public void _WriteLocalRotation(ref IndexedQuaternion localRotation)
		{
			_localRotation = localRotation;
			_isWriteLocalRotation = true;
			_isWriteWorldTransform = true;
			if (_model._fileType == MMDFileType.PMD)
			{
				_UpdateWorldTransform();
			}
			else if (_model._fileType == MMDFileType.PMX)
			{
				_model._updateBoneTransform++;
				_updateWorldTransform++;
			}
		}

		public void _PrepareTransform()
		{
			if ((_updateFlags & 8u) != 0 && _model._pphShoulderFixRate > float.Epsilon)
			{
				uint num = _updateBoneFlags & 0xFF000000u;
				if (num == 33554432 || num == 67108864)
				{
					_pphBoneBasis = GetWorldTransform()._basis;
				}
			}
			if (_model._isPhysicsEnabled && _simulatedRigidBody != null && _simulatedRigidBody.isSimulated && !_simulatedRigidBody.isFreezed)
			{
				_worldTransformBeforePhysics = _worldTransform;
			}
		}

		public void _PerformTransform()
		{
			if (_model._fileType == MMDFileType.PMD)
			{
				if (_underIK || (_model._isAfterIK && !_IsTargetBoneAsUnderIK() && !_IsParentBoneAsUnderIK()))
				{
					return;
				}
			}
			else if (_model._fileType == MMDFileType.PMX)
			{
				bool flag = (_pmxBoneFlags & PMXBoneFlag.TransformAfterPhysics) != 0;
				if (_model._isAfterPhysics != flag)
				{
					return;
				}
			}
			if (_model._isPhysicsEnabled && _simulatedRigidBody != null && _simulatedRigidBody.isSimulated && !_simulatedRigidBody.isFreezed)
			{
				return;
			}
			uint updateFlags = _updateFlags;
			bool flag2 = (updateFlags & 2) != 0;
			bool flag3 = (updateFlags & 4) != 0;
			bool flag4 = (updateFlags & 8) != 0;
			bool flag5 = (flag3 && _isMorphPositionDepended) || _isMorphPositionDepended2;
			bool flag6 = (flag3 && _isMorphRotationDepended) || _isMorphRotationDepended2;
			flag5 |= ((_updateBoneFlags | _updateBoneFlags2) & 0x80) != 0;
			flag6 |= ((_updateBoneFlags | _updateBoneFlags2) & 0x100) != 0;
			bool flag7 = false;
			bool flag8 = false;
			FastVector3 fastVector = _offset;
			FastQuaternion fastQuaternion = FastQuaternion.Identity;
			if (flag5)
			{
				flag7 = true;
				fastVector += _userPosition;
				fastVector += _morphPosition;
			}
			if (flag6)
			{
				flag8 = true;
				fastQuaternion = _userRotation;
				fastQuaternion *= _morphRotation;
			}
			if (flag2 && _inherenceParentBone != null)
			{
				if (_model._fileType == MMDFileType.PMD)
				{
					if (_pmdBoneType == PMDBoneType.UnderRotate)
					{
						flag8 = true;
						fastQuaternion *= _inherenceParentBone._localRotation;
					}
					else if (_pmdBoneType == PMDBoneType.FollowRotate)
					{
						IndexedQuaternion rhs = _inherenceParentBone._localRotation;
						if (Mathf.Abs(_inherenceWeight - 1f) > float.Epsilon)
						{
							rhs = Math.SlerpFromIdentity(ref rhs, Mathf.Abs(_inherenceWeight));
							if (_inherenceWeight < 0f)
							{
								rhs.X = 0f - rhs.X;
								rhs.Y = 0f - rhs.Y;
								rhs.Z = 0f - rhs.Z;
							}
						}
						flag8 = true;
						fastQuaternion = rhs;
					}
				}
				else
				{
					if ((_pmxBoneFlags & PMXBoneFlag.InherencePosition) != 0)
					{
						IndexedVector3 indexedVector = (((_pmxBoneFlags & PMXBoneFlag.InherenceLocal) == 0) ? (_inherenceParentBone._localPosition - _inherenceParentBone._offset) : (_inherenceParentBone.GetWorldTransform()._origin - _inherenceParentBone._baseOrigin));
						if (Mathf.Abs(_inherenceWeight - 1f) > float.Epsilon)
						{
							indexedVector *= _inherenceWeight;
						}
						flag7 = true;
						fastVector += indexedVector;
					}
					if ((_pmxBoneFlags & PMXBoneFlag.InherenceRotation) != 0)
					{
						IndexedQuaternion rhs2 = (((_pmxBoneFlags & PMXBoneFlag.InherenceLocal) == 0) ? _inherenceParentBone._localRotation : _inherenceParentBone.GetWorldTransform().GetRotation());
						if (Mathf.Abs(_inherenceWeight - 1f) > float.Epsilon)
						{
							float num = Mathf.Abs(_inherenceWeight);
							if (num < 1f)
							{
								rhs2 = Math.SlerpFromIdentity(ref rhs2, num);
							}
							else if (num > 1f)
							{
								rhs2 = new IndexedQuaternion(Math.GetAxis(ref rhs2), Math.GetAngle(ref rhs2) * num);
							}
							if (_inherenceWeight < 0f)
							{
								rhs2.X = 0f - rhs2.X;
								rhs2.Y = 0f - rhs2.Y;
								rhs2.Z = 0f - rhs2.Z;
							}
						}
						flag8 = true;
						fastQuaternion *= rhs2;
					}
				}
			}
			if (flag4)
			{
				uint num2 = _updateBoneFlags & 0xFF000000u;
				if ((num2 == 16777216 || num2 == 50331648) && _model._pphShoulderFixRate > float.Epsilon)
				{
					if (!flag8)
					{
						fastQuaternion = _localRotation;
					}
					fastQuaternion = Math.SlerpFromIdentity(ref fastQuaternion.q, 1f - _model._pphShoulderFixRate);
					flag8 = true;
				}
				if ((num2 == 33554432 || num2 == 67108864) && _model._pphShoulderFixRate > float.Epsilon)
				{
					fastQuaternion = ((_originalParentBone == null) ? ((FastQuaternion)(_model._modelTransform._basis.Transpose() * _pphBoneBasis).GetRotation()) : ((FastQuaternion)(_originalParentBone.GetWorldTransform()._basis.Transpose() * _pphBoneBasis).GetRotation()));
					flag8 = true;
				}
			}
			if (flag7 && flag8)
			{
				_WriteLocalTransform(ref fastVector.v, ref fastQuaternion.q);
			}
			else if (flag7)
			{
				_WriteLocalPosition(ref fastVector.v);
			}
			else if (flag8)
			{
				_WriteLocalRotation(ref fastQuaternion.q);
			}
			else if (_originalParentBone != null && _originalParentBone._isWriteWorldTransform && _isSetUnityLocalPosition && _isSetUnityLocalRotation)
			{
				_isWriteWorldTransform = true;
				if (_model._fileType == MMDFileType.PMD)
				{
					_UpdateWorldTransform();
				}
			}
			if (_ik != null)
			{
				_ik.Solve();
			}
		}

		public void PrepareIKTransform(bool underIKLink, float ikWeight)
		{
			_underIK = true;
			_underIKLink |= underIKLink;
			if (ikWeight != 1f)
			{
				_localRotationBeforeIK = _localRotation;
			}
			if (_model._modelProperty.forceIKResetBoneFlag != 0)
			{
				IndexedQuaternion localRotation = IndexedQuaternion.Identity;
				_WriteLocalRotation(ref localRotation);
			}
		}

		public void ApplyLocalRotationFromIK(ref IndexedQuaternion rotation)
		{
			IndexedQuaternion localRotation = _localRotation * rotation;
			localRotation.Normalize();
			_WriteLocalRotation(ref localRotation);
		}

		public void PerformTransformFromIK()
		{
			if (_model._fileType == MMDFileType.PMD)
			{
				_UpdateWorldTransform();
				return;
			}
			MMDFileType fileType = _model._fileType;
			int num = 2;
		}

		public void PostfixIKTransform(float ikWeight)
		{
			_isWriteWorldTransform = true;
			if (ikWeight != 1f)
			{
				IndexedQuaternion localRotation = Math.Slerp(ref _localRotationBeforeIK, ref _localRotation, ikWeight);
				_WriteLocalRotation(ref localRotation);
			}
		}

		public void AntiJitterWorldTransform(bool isTouchKinematic)
		{
			if (_originalParentBone == null)
			{
				return;
			}
			IndexedMatrix lhs = GetWorldTransform();
			if (!_isSetPrevWorldTransform)
			{
				_isSetPrevWorldTransform = true;
				_prevWorldTransform = lhs;
				return;
			}
			_prevWorldTransform2 = _prevWorldTransform;
			_isSetPrevWorldTransform2 = true;
			float num = (isTouchKinematic ? _model._modelProperty.rigidBodyAntiJitterRateOnKinematic : _model._modelProperty.rigidBodyAntiJitterRate);
			if (num > 0f)
			{
				float lhsRate = Mathf.Max(1f - num * 0.5f, 0.5f);
				IndexedMatrix m = IndexedMatrix.Identity;
				Math.BlendTransform(ref m, ref lhs, ref _prevWorldTransform, lhsRate);
				_prevWorldTransform = lhs;
				_WriteWorldTransform(ref m);
			}
			else
			{
				_prevWorldTransform = lhs;
			}
		}

		public void AntiJitterWorldTransformOnDisabled()
		{
			if (_originalParentBone != null)
			{
				IndexedMatrix worldTransform = GetWorldTransform();
				if (!_isSetPrevWorldTransform)
				{
					_isSetPrevWorldTransform = true;
					_prevWorldTransform = worldTransform;
				}
				else
				{
					_prevWorldTransform2 = _prevWorldTransform;
					_isSetPrevWorldTransform2 = true;
					_prevWorldTransform = worldTransform;
				}
			}
		}

		public void ComputeTransform()
		{
			if (_isComputeTransform)
			{
				return;
			}
			_isComputeTransform = true;
			if (!_isSetUnityWorldTransform && _isSetUnityLocalPosition && _isSetUnityLocalRotation)
			{
				if (_modifiedParentBone != null && !_modifiedParentBone._isComputeTransform)
				{
					_modifiedParentBone.ComputeTransform();
				}
				_isSetUnityWorldTransform = true;
				_unityWorldTransform._origin = Math.UnityToBulletPosition(ref _unityLocalPosition, _model._localToBulletScale);
				_unityWorldTransform.SetRotation(Math.UnityToBulletRotation(ref _unityLocalRotation));
				if (_modifiedParentBone != null)
				{
					_unityWorldTransform = _modifiedParentBone._unityWorldTransform * _unityWorldTransform;
				}
				else
				{
					_unityWorldTransform = _model._modelTransform * _unityWorldTransform;
				}
				if (!_model._isPhysicsEnabled || _simulatedRigidBody == null || !_simulatedRigidBody.isSimulated || _simulatedRigidBody.isFreezed)
				{
					_worldTransform = _unityWorldTransform;
					if (_model._isLocalAxisEnabled && (_pmxBoneFlags & PMXBoneFlag.LocalAxis) != 0)
					{
						_worldTransform._basis *= _localBasisInv;
					}
				}
			}
			if (_model._isPhysicsEnabled && _simulatedRigidBody != null && _simulatedRigidBody.isSimulated && !_simulatedRigidBody.isFreezed)
			{
				return;
			}
			bool rigidBodyIsForceTranslate = _model._modelProperty.rigidBodyIsForceTranslate;
			bool flag = _model._isLocalAxisEnabled && _originalParentBone != null && (_originalParentBone._pmxBoneFlags & PMXBoneFlag.LocalAxis) != 0;
			bool flag2 = _model._isLocalAxisEnabled && ((_pmxBoneFlags & PMXBoneFlag.LocalAxis) != 0 || (_originalParentBone != null && (_originalParentBone._pmxBoneFlags & PMXBoneFlag.LocalAxis) != 0));
			if (!flag && _originalParentBone == _modifiedParentBone)
			{
				if (_isSetUnityLocalPosition)
				{
					if (_originalParentBone != null)
					{
						if (!rigidBodyIsForceTranslate && (_pmxBoneFlags & PMXBoneFlag.Translate) == 0)
						{
							_localPosition = _offset;
						}
						else
						{
							_localPosition = Math.UnityToBulletPosition(ref _unityLocalPosition, _model._localToBulletScale);
						}
					}
					else
					{
						_localPosition = Math.UnityToBulletPosition(ref _unityLocalPosition, _model._localToBulletScale);
					}
				}
			}
			else if (_originalParentBone != null)
			{
				if (!_originalParentBone._isComputeTransform)
				{
					_originalParentBone.ComputeTransform();
				}
				if (_isSetUnityWorldTransform || _originalParentBone._isSetUnityWorldTransform)
				{
					if (!rigidBodyIsForceTranslate && (_pmxBoneFlags & PMXBoneFlag.Translate) == 0)
					{
						_localPosition = _offset;
					}
					else
					{
						IndexedVector3 origin = _worldTransform._origin;
						IndexedVector3 origin2 = _originalParentBone._worldTransform._origin;
						_localPosition = _originalParentBone._worldTransform._basis.Transpose() * (origin - origin2);
					}
				}
			}
			else if (_isSetUnityWorldTransform)
			{
				IndexedVector3 origin3 = _worldTransform._origin;
				IndexedVector3 origin4 = _model._modelTransform._origin;
				_localPosition = _model._modelTransform._basis.Transpose() * (origin3 - origin4);
			}
			if (!flag2 && _originalParentBone == _modifiedParentBone)
			{
				if (_isSetUnityLocalRotation)
				{
					_localRotation = Math.UnityToBulletRotation(ref _unityLocalRotation);
				}
			}
			else if (_originalParentBone != null)
			{
				if (!_originalParentBone._isComputeTransform)
				{
					_originalParentBone.ComputeTransform();
				}
				if (_isSetUnityWorldTransform || _originalParentBone._isSetUnityWorldTransform)
				{
					IndexedMatrix worldTransform = _originalParentBone._worldTransform;
					_localRotation = (worldTransform._basis.Transpose() * _worldTransform._basis).GetRotation();
				}
			}
			else if (_isSetUnityWorldTransform)
			{
				IndexedMatrix modelTransform = _model._modelTransform;
				_localRotation = (modelTransform._basis.Transpose() * _worldTransform._basis).GetRotation();
			}
		}

		public void ComputeUnityTransform()
		{
			if (_isComputeUnityTransform)
			{
				return;
			}
			_isComputeUnityTransform = true;
			if (_originalParentBone != null && !_originalParentBone._isComputeUnityTransform)
			{
				_originalParentBone.ComputeUnityTransform();
			}
			if (_modifiedParentBone != null && !_modifiedParentBone._isComputeUnityTransform)
			{
				_modifiedParentBone.ComputeUnityTransform();
			}
			if (_isWriteWorldTransform)
			{
				_CheckUpdateWorldTransform();
				_isSetUnityWorldTransform = true;
				_unityWorldTransform = _worldTransform;
				if (_model._isLocalAxisEnabled && (_pmxBoneFlags & PMXBoneFlag.LocalAxis) != 0)
				{
					_unityWorldTransform._basis *= _localBasis;
				}
			}
			bool flag = false;
			if (_model._fileType == MMDFileType.PMD && _originalParentBone != null && _originalParentBone._sortedBoneID > _sortedBoneID)
			{
				flag = true;
			}
			bool rigidBodyIsForceTranslate = _model._modelProperty.rigidBodyIsForceTranslate;
			bool flag2 = _model._isLocalAxisEnabled && _modifiedParentBone != null && (_modifiedParentBone._pmxBoneFlags & PMXBoneFlag.LocalAxis) != 0;
			bool flag3 = _model._isLocalAxisEnabled && ((_pmxBoneFlags & PMXBoneFlag.LocalAxis) != 0 || (_modifiedParentBone != null && (_modifiedParentBone._pmxBoneFlags & PMXBoneFlag.LocalAxis) != 0));
			if (!flag2 && !flag && _originalParentBone == _modifiedParentBone)
			{
				if (_isWriteLocalPosition)
				{
					_isLateUpdatePosition = true;
					_isSetUnityLocalPosition = true;
					if (_originalParentBone != null && !rigidBodyIsForceTranslate && (_pmxBoneFlags & PMXBoneFlag.Translate) == 0)
					{
						_unityLocalPosition = _unityOffset;
					}
					else
					{
						_unityLocalPosition = Math.BulletToUnityPosition(ref _localPosition, _model._bulletToLocalScale);
					}
				}
			}
			else
			{
				bool flag4 = _originalParentBone != null && _originalParentBone._isWriteWorldTransform;
				bool flag5 = _modifiedParentBone != null && _modifiedParentBone._isWriteWorldTransform;
				if (_isWriteLocalPosition || flag4 || flag5)
				{
					_isLateUpdatePosition = true;
					_isSetUnityLocalPosition = true;
					IndexedMatrix indexedMatrix = ((_modifiedParentBone != null) ? _modifiedParentBone._unityWorldTransform : _model._modelTransform);
					IndexedVector3 origin = _worldTransform._origin;
					IndexedVector3 origin2 = indexedMatrix._origin;
					_unityLocalPosition = indexedMatrix._basis.Transpose() * (origin - origin2);
					_unityLocalPosition = Math.BulletToUnityPosition(ref _unityLocalPosition, _model._bulletToLocalScale);
				}
			}
			if (!flag3 && !flag && _originalParentBone == _modifiedParentBone)
			{
				if (_isWriteLocalRotation)
				{
					_isLateUpdateRotation = true;
					_isSetUnityLocalRotation = true;
					_unityLocalRotation = Math.BulletToUnityRotation(ref _localRotation);
				}
				return;
			}
			bool flag6 = _originalParentBone != null && _originalParentBone._isWriteWorldTransform;
			bool flag7 = _modifiedParentBone != null && _modifiedParentBone._isWriteWorldTransform;
			if (_isWriteWorldTransform || flag6 || flag7)
			{
				_isLateUpdateRotation = true;
				_isSetUnityLocalRotation = true;
				_unityLocalRotation = (((_modifiedParentBone != null) ? _modifiedParentBone._unityWorldTransform : _model._modelTransform)._basis.Transpose() * _unityWorldTransform._basis).GetRotation();
				_unityLocalRotation = Math.BulletToUnityRotation(ref _unityLocalRotation);
			}
		}
	}
}
