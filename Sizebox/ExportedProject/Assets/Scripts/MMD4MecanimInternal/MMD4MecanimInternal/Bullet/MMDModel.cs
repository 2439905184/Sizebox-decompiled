using System;
using BulletXNA.BulletCollision;
using BulletXNA.BulletDynamics;
using BulletXNA.LinearMath;
using UnityEngine;

namespace MMD4MecanimInternal.Bullet
{
	public class MMDModel : PhysicsEntity
	{
		[Flags]
		public enum MeshFlags
		{
			None = 0,
			BlendShapes = 1,
			VertexMorph = 2,
			XDEF = 4
		}

		public enum UpdateIValues
		{
			AnimationHashName = 0,
			Max = 1
		}

		public enum UpdateFValues
		{
			AnimationTime = 0,
			PPHShoulderFixRate = 1,
			Max = 2
		}

		[Flags]
		public enum LateUpdateMeshFlags
		{
			Vertices = 1,
			Normals = 2
		}

		[Flags]
		public enum UpdateFlags
		{
			IKEnabled = 1,
			BoneInherenceEnabled = 2,
			BoneMorphEnabled = 4,
			PPHShoulderEnabled = 8
		}

		[Flags]
		public enum UpdateBoneFlags
		{
			WorldTransform = 1,
			Position = 2,
			Rotation = 4,
			CheckPosition = 8,
			CheckRotation = 0x10,
			ChangedPosition = 0x20,
			ChangedRotation = 0x40,
			UserPosition = 0x80,
			UserRotation = 0x100,
			SkeletonMask = -16777216,
			SkeletonLeftShoulder = 0x1000000,
			SkeletonLeftUpperArm = 0x2000000,
			SkeletonRightShoulder = 0x3000000,
			SkeletonRightUpperArm = 0x4000000,
			UserTransform = 0x180
		}

		[Flags]
		public enum UpdateRigidBodyFlags
		{
			Freezed = 1
		}

		[Flags]
		public enum LateUpdateFlags
		{
			Bone = 1,
			Mesh = 2
		}

		[Flags]
		public enum LateUpdateBoneFlags
		{
			LateUpdated = 1,
			Position = 2,
			Rotation = 4
		}

		public struct UpdateAnimData
		{
			public uint animationHashName;

			public float animationTime;
		}

		public class LateUpdateMeshData
		{
			public int meshID = -1;

			public uint lateUpdateMeshFlags;

			public int vertexLength;

			public Vector3[] vertices;

			public Vector3[] normals;
		}

		public class UpdateData
		{
			public uint updateFlags;

			public UpdateAnimData updateAnimData = default(UpdateAnimData);

			public float updatePPHShoulderFixRate;

			public Matrix4x4 updateModelTransform;

			public int[] updateBoneFlagsList;

			public Matrix4x4[] updateBoneTransformList;

			public Vector3[] updateBoneLocalPositionList;

			public Quaternion[] updateBoneLocalRotationList;

			public Vector3[] updateBoneUserPositionList;

			public Quaternion[] updateBoneUserRotationList;

			public int[] updateRigidBodyFlagsList;

			public float[] updateIKWeightList;

			public float[] updateMorphWeightList;

			public uint lateUpdateFlags;

			public int[] lateUpdateBoneFlagsList;

			public Vector3[] lateUpdateBonePositionList;

			public Quaternion[] lateUpdateBoneRotationList;

			public int[] lateUpdateMeshFlagsList;

			public LateUpdateMeshData[] lateUpdateMeshDataList;
		}

		public struct ImportProperty
		{
			public bool isPhysicsEnabled;

			public bool isVertexMorphEnabled;

			public bool isBlendShapesEnabled;

			public bool isXDEFEnabled;

			public bool isXDEFNormalEnabled;

			public bool isJoinedLocalWorld;

			public bool useCustomResetTime;

			public float resetWaitTime;

			public float resetMorphTime;

			public bool optimizeSettings;

			public MMDModelProperty mmdModelProperty;
		}

		private static object _vertexMorphThreadQueue;

		private static object _xdefPararellThreadQueue;

		public MMDFileType _fileType;

		public uint _vertexCount;

		public Vector3 _lossyScale = Vector3.one;

		public float _lossyScaleInv = 1f;

		public bool _isLossyScaleIdentity;

		public bool _isLossyScaleSquare;

		public float _importScaleMMDModel;

		public float _modelToBulletScale;

		public float _bulletToLocalScale;

		public float _bulletToWorldScale;

		public float _worldToBulletScale;

		public float _localToBulletScale;

		public bool _optimizeSettings = true;

		public float _resetWaitTime;

		public float _resetMorphTime;

		public MMDBone _rootBone;

		public MMDBone[] _boneList;

		public MMDBone[] _sortedBoneList;

		public MMDMorph[] _morphList;

		public MMDRigidBody[] _rigidBodyList;

		public MMDJoint[] _jointList;

		public MMDIK[] _ikList;

		public MMDMesh[] _meshList;

		public AuxData.IndexData _indexData;

		public AuxData.VertexData _vertexData;

		private MMDRigidBody[] _simulatedRigidBodyList;

		private bool _isJoinedWorld;

		private bool _isJoinedWorldInternal;

		private bool _isResetWorld;

		private bool _processResetWorld;

		private float _processResetRatio;

		public bool _isAfterIK;

		public bool _isAfterPhysics;

		public bool _isMultiThreading;

		public bool _isPhysicsEnabled = true;

		public bool _isMorphRenameEnabled;

		public bool _isLocalAxisEnabled;

		public bool _isJoinedLocalWorld;

		public bool _isVertexMorphEnabled;

		public bool _isBlendShapesEnabled;

		public bool _isIKEnabled;

		public bool _isBoneInherenceEnabled;

		public bool _isBoneMorphEnabled;

		public bool _isXDEFEnabled;

		public bool _isXDEFNormalEnabled;

		public bool _isDestroyed;

		public float _pphShoulderFixRate;

		public float _pphShoulderFixRate2;

		private UpdateData _sync_unusedData;

		private UpdateData _sync_updateData;

		private UpdateData _sync_updateData2;

		private UpdateData _sync_updatedData;

		private UpdateData _sync_lateUpdateData;

		private UpdateData _updateData;

		public IndexedMatrix _modelTransform = IndexedMatrix.Identity;

		public ulong _updateBoneTransform;

		public MMDModelProperty _modelProperty;

		private ThreadQueueHandle _vertexMorphThreadQueueHandle = default(ThreadQueueHandle);

		public static object GetXDEFPararellThreadQueue()
		{
			return _xdefPararellThreadQueue;
		}

		~MMDModel()
		{
			Destroy();
		}

		public void Destroy()
		{
			_isDestroyed = true;
			if (!_isJoinedWorld)
			{
				_DestroyImmediate();
			}
		}

		private static float _SafeDiv(float a, float b)
		{
			if (!(Mathf.Abs(b) > float.Epsilon))
			{
				return 0f;
			}
			return a / b;
		}

		public bool Import(byte[] bytes, byte[] indexData, byte[] vertexData, int[] meshFlags, ref ImportProperty importProperty)
		{
			BinaryReader binaryReader = new BinaryReader(bytes);
			if (!binaryReader.Preparse())
			{
				return false;
			}
			uint fourCC = (uint)binaryReader.GetFourCC();
			if (fourCC != BinaryReader.MakeFourCC("MDL1"))
			{
				char c = (char)fourCC;
				char c2 = (char)((fourCC >> 8) & 0xFFu);
				char c3 = (char)((fourCC >> 16) & 0xFFu);
				char c4 = (char)((fourCC >> 24) & 0xFFu);
				Debug.LogError("Not supported file. " + c + c2 + c3 + c4);
				return false;
			}
			_isPhysicsEnabled = importProperty.isPhysicsEnabled;
			_isVertexMorphEnabled = importProperty.isVertexMorphEnabled;
			_isBlendShapesEnabled = importProperty.isBlendShapesEnabled;
			_isXDEFEnabled = importProperty.isXDEFEnabled;
			_isXDEFNormalEnabled = importProperty.isXDEFNormalEnabled;
			_isJoinedLocalWorld = importProperty.isJoinedLocalWorld;
			_modelProperty = importProperty.mmdModelProperty;
			if (_modelProperty == null)
			{
				_modelProperty = new MMDModelProperty();
			}
			else
			{
				_modelProperty = _modelProperty.Clone();
			}
			if (_modelProperty.rigidBodyLinearDampingLossRate < 0f)
			{
				_modelProperty.rigidBodyLinearDampingLossRate = 0.05f;
			}
			if (_modelProperty.rigidBodyAngularDampingLossRate < 0f)
			{
				_modelProperty.rigidBodyAngularDampingLossRate = 0.05f;
			}
			if (_modelProperty.rigidBodyLinearVelocityLimit < 0f)
			{
				_modelProperty.rigidBodyLinearVelocityLimit = 10f;
			}
			if (_modelProperty.jointRootAdditionalLimitAngle < 0f)
			{
				_modelProperty.jointRootAdditionalLimitAngle = 135f;
			}
			_modelProperty.Postfix();
			_lossyScale = _modelProperty.lossyScale;
			if (Mathf.Abs(_lossyScale.x) > float.Epsilon)
			{
				_lossyScaleInv = 1f / _lossyScale.x;
			}
			else
			{
				_lossyScaleInv = 0f;
			}
			_isLossyScaleSquare = (_isLossyScaleIdentity = Mathf.Abs(_lossyScale.x - 1f) <= float.Epsilon && Mathf.Abs(_lossyScale.y - 1f) <= float.Epsilon && Mathf.Abs(_lossyScale.z - 1f) <= float.Epsilon);
			if (!_isLossyScaleSquare)
			{
				_isLossyScaleSquare = Mathf.Abs(_lossyScale.x - _lossyScale.y) <= float.Epsilon && Mathf.Abs(_lossyScale.x - _lossyScale.z) <= float.Epsilon;
			}
			_optimizeSettings = importProperty.optimizeSettings;
			_resetMorphTime = 1.8f;
			_resetWaitTime = 1.2f;
			if (importProperty.useCustomResetTime)
			{
				_resetMorphTime = importProperty.resetMorphTime;
				_resetWaitTime = importProperty.resetWaitTime;
			}
			binaryReader.BeginHeader();
			_fileType = (MMDFileType)binaryReader.ReadHeaderInt();
			binaryReader.ReadHeaderFloat();
			binaryReader.ReadHeaderInt();
			int num = binaryReader.ReadHeaderInt();
			_vertexCount = (uint)binaryReader.ReadHeaderInt();
			binaryReader.ReadHeaderInt();
			float num2 = binaryReader.ReadHeaderFloat();
			float num3 = binaryReader.ReadHeaderFloat();
			binaryReader.EndHeader();
			_importScaleMMDModel = num3;
			_isMorphRenameEnabled = (num & 1) != 0;
			_isLocalAxisEnabled = (num & 2) != 0;
			if (_modelProperty.importScale > float.Epsilon)
			{
				num3 = _modelProperty.importScale;
			}
			float num4 = num2 * num3;
			_modelToBulletScale = 1f;
			_bulletToLocalScale = 1f;
			_bulletToWorldScale = 1f;
			_worldToBulletScale = 1f;
			float b = num4;
			if (_modelProperty.worldScale > float.Epsilon)
			{
				b = _modelProperty.worldScale;
			}
			if (_isJoinedLocalWorld)
			{
				_modelToBulletScale = _SafeDiv(num4, b);
			}
			else
			{
				_modelToBulletScale = _lossyScale.x * _SafeDiv(num4, b);
			}
			_bulletToLocalScale = num4 * _SafeDiv(1f, _modelToBulletScale);
			_localToBulletScale = _SafeDiv(1f, _bulletToLocalScale);
			_worldToBulletScale = _SafeDiv(_localToBulletScale, _lossyScale.x);
			_bulletToWorldScale = _SafeDiv(1f, _worldToBulletScale);
			int num5 = BinaryReader.MakeFourCC("BONE");
			int num6 = BinaryReader.MakeFourCC("IK__");
			int num7 = BinaryReader.MakeFourCC("MRPH");
			int num8 = BinaryReader.MakeFourCC("RGBD");
			int num9 = BinaryReader.MakeFourCC("JOIN");
			int structListLength = binaryReader.structListLength;
			for (int i = 0; i < structListLength; i++)
			{
				if (!binaryReader.BeginStructList())
				{
					Debug.LogError("BeginStructList() failed.");
					return false;
				}
				int currentStructFourCC = binaryReader.currentStructFourCC;
				if (currentStructFourCC == num5)
				{
					_boneList = new MMDBone[binaryReader.currentStructLength];
					for (int j = 0; j < binaryReader.currentStructLength; j++)
					{
						_boneList[j] = new MMDBone();
						_boneList[j]._model = this;
					}
					for (int k = 0; k < binaryReader.currentStructLength; k++)
					{
						MMDBone mMDBone = _boneList[k];
						if (!mMDBone.Import(k, binaryReader))
						{
							Debug.LogError("PMXBone parse error.");
							_boneList = null;
							return false;
						}
						if (mMDBone._isRootBone)
						{
							_rootBone = mMDBone;
						}
					}
					if (_rootBone == null && _boneList.Length > 0)
					{
						_rootBone = _boneList[0];
						_rootBone._isRootBone = true;
					}
					for (int l = 0; l < _boneList.Length; l++)
					{
						_boneList[l].PostfixImport();
					}
				}
				else if (currentStructFourCC == num6)
				{
					_ikList = new MMDIK[binaryReader.currentStructLength];
					for (int m = 0; m < binaryReader.currentStructLength; m++)
					{
						MMDIK mMDIK = new MMDIK();
						_ikList[m] = mMDIK;
						mMDIK._model = this;
						if (!mMDIK.Import(binaryReader))
						{
							Debug.LogError("PMXIK parse error.");
							_ikList = null;
							return false;
						}
					}
				}
				else if (currentStructFourCC == num7)
				{
					_morphList = new MMDMorph[binaryReader.currentStructLength];
					for (int n = 0; n != binaryReader.currentStructLength; n++)
					{
						MMDMorph mMDMorph = new MMDMorph();
						_morphList[n] = mMDMorph;
						mMDMorph._model = this;
					}
					for (int num10 = 0; num10 != binaryReader.currentStructLength; num10++)
					{
						MMDMorph mMDMorph2 = _morphList[num10];
						if (!mMDMorph2.Import(binaryReader))
						{
							Debug.LogError("PMXIK parse error.");
							_morphList = null;
							return false;
						}
					}
				}
				else if (currentStructFourCC == num8)
				{
					_rigidBodyList = new MMDRigidBody[binaryReader.currentStructLength];
					for (int num11 = 0; num11 < binaryReader.currentStructLength; num11++)
					{
						MMDRigidBody mMDRigidBody = new MMDRigidBody();
						_rigidBodyList[num11] = mMDRigidBody;
						mMDRigidBody._model = this;
						if (!mMDRigidBody.Import(binaryReader))
						{
							Debug.LogError("PMXRigidBody parse error.");
							_rigidBodyList = null;
							return false;
						}
					}
				}
				else if (currentStructFourCC == num9)
				{
					_jointList = new MMDJoint[binaryReader.currentStructLength];
					for (int num12 = 0; num12 < binaryReader.currentStructLength; num12++)
					{
						MMDJoint mMDJoint = new MMDJoint();
						_jointList[num12] = mMDJoint;
						mMDJoint._model = this;
						if (!mMDJoint.Import(binaryReader))
						{
							Debug.LogError("PMXJoint parse error.");
							_jointList = null;
							return false;
						}
					}
				}
				if (!binaryReader.EndStructList())
				{
					Debug.LogError("EndStructList() failed.");
					return false;
				}
			}
			if (_boneList != null)
			{
				int num13 = 0;
				int ikIndex = 0;
				for (; num13 < _boneList.Length; num13++)
				{
					_boneList[num13].PostfixImportIK(ref ikIndex);
				}
				_sortedBoneList = new MMDBone[_boneList.Length];
				for (int num14 = 0; num14 < _boneList.Length; num14++)
				{
					int sortedBoneID = _boneList[num14]._sortedBoneID;
					if (sortedBoneID >= 0 && sortedBoneID < _boneList.Length)
					{
						_sortedBoneList[sortedBoneID] = _boneList[num14];
					}
				}
				for (int num15 = 0; num15 < _boneList.Length; num15++)
				{
					if (_sortedBoneList[num15] == null)
					{
						Debug.LogError("SortedBoneList is invalid. boneID = " + num15);
						_sortedBoneList = null;
						break;
					}
				}
			}
			if (_isPhysicsEnabled)
			{
				_MakeSimulatedRigidBodyList();
				_isResetWorld = true;
			}
			if (meshFlags != null && meshFlags.Length != 0)
			{
				bool flag = false;
				for (int num16 = 0; num16 != meshFlags.Length; num16++)
				{
					if (flag)
					{
						break;
					}
					flag |= (meshFlags[num16] & 1) != 0;
				}
				_isBlendShapesEnabled = _isVertexMorphEnabled && _isBlendShapesEnabled && flag;
			}
			else
			{
				_isVertexMorphEnabled = false;
				_isBlendShapesEnabled = false;
				_isXDEFEnabled = false;
				_isXDEFNormalEnabled = false;
			}
			if ((_isVertexMorphEnabled && !_isBlendShapesEnabled) || _isXDEFEnabled)
			{
				if (indexData != null && indexData.Length != 0)
				{
					_indexData = AuxData.BuildIndexData(indexData);
					if (_indexData != null && (meshFlags == null || meshFlags.Length != _indexData.meshCount))
					{
						_indexData = null;
					}
				}
				if (_indexData != null && _isXDEFEnabled && vertexData != null && vertexData.Length != 0)
				{
					_vertexData = AuxData.BuildVertexData(vertexData);
					if (_vertexData != null && (meshFlags == null || meshFlags.Length != _vertexData.meshCount))
					{
						_vertexData = null;
					}
				}
				if (_indexData != null && meshFlags != null && meshFlags.Length != 0)
				{
					_meshList = new MMDMesh[meshFlags.Length];
					for (int num17 = 0; num17 != _meshList.Length; num17++)
					{
						_meshList[num17] = new MMDMesh();
						_meshList[num17]._model = this;
						_meshList[num17].meshFlags = (uint)meshFlags[num17];
					}
					if (_vertexData != null && _isXDEFEnabled)
					{
						bool flag2 = false;
						AuxData.VertexData.MeshBoneInfo r = default(AuxData.VertexData.MeshBoneInfo);
						for (int num18 = 0; num18 != _meshList.Length; num18++)
						{
							if (!_isBlendShapesEnabled || (meshFlags[num18] & 1) == 0)
							{
								_vertexData.GetMeshBoneInfo(ref r, num18);
								if (r.isSDEF)
								{
									_meshList[num18].meshFlags |= 4u;
									flag2 = true;
								}
							}
						}
						if (!flag2)
						{
							_isXDEFEnabled = false;
							_isXDEFNormalEnabled = false;
						}
					}
					if (_indexData != null && _isVertexMorphEnabled && !_isBlendShapesEnabled)
					{
						bool flag3 = false;
						for (int num19 = 0; num19 != _morphList.Length; num19++)
						{
							if (!_morphList[num19].PrepareDependMesh())
							{
								flag3 = true;
								break;
							}
						}
						if (flag3)
						{
							_indexData = null;
							_meshList = null;
						}
					}
				}
			}
			if (_indexData != null)
			{
				if (meshFlags != null && _meshList != null && _meshList.Length == meshFlags.Length)
				{
					for (int num20 = 0; num20 != meshFlags.Length; num20++)
					{
						meshFlags[num20] = (int)_meshList[num20].meshFlags;
					}
				}
			}
			else
			{
				_meshList = null;
				if (meshFlags != null)
				{
					for (int num21 = 0; num21 != meshFlags.Length; num21++)
					{
						meshFlags[num21] &= -7;
					}
				}
				_isVertexMorphEnabled = false;
				_isBlendShapesEnabled = false;
				_isXDEFEnabled = false;
				_isXDEFNormalEnabled = false;
			}
			if (_isVertexMorphEnabled && !_isBlendShapesEnabled && _vertexMorphThreadQueue == null)
			{
				_vertexMorphThreadQueue = Global.bridge.CreateCachedThreadQueue(0);
			}
			if (_isVertexMorphEnabled && _isXDEFEnabled && _xdefPararellThreadQueue == null && Global.bridge != null)
			{
				_xdefPararellThreadQueue = Global.bridge.CreatePararellCachedThreadQueue(0);
			}
			return true;
		}

		public int UploadMesh(int meshID, Vector3[] vertices, Vector3[] normals, BoneWeight[] boneWeights, Matrix4x4[] bindposes)
		{
			if (_meshList == null || meshID < 0 || meshID >= _meshList.Length)
			{
				return 0;
			}
			if (!_meshList[meshID].UploadMesh(meshID, vertices, normals, boneWeights, bindposes))
			{
				return 0;
			}
			return 1;
		}

		private static bool _IsParentBone(MMDBone targetBone, MMDBone bone)
		{
			while (bone != null)
			{
				if (bone == targetBone)
				{
					return true;
				}
				bone = bone._originalParentBone;
			}
			return false;
		}

		private static void _Swap(ref MMDRigidBody lhs, ref MMDRigidBody rhs)
		{
			MMDRigidBody mMDRigidBody = lhs;
			lhs = rhs;
			rhs = mMDRigidBody;
		}

		public int PreUpdate(uint updateFlags, int[] iBoneValues, int[] iRigidBodyValues, float[] ikWeights, float[] morphWeights)
		{
			if (iBoneValues == null || _boneList == null)
			{
				return 0;
			}
			if (_boneList.Length == iBoneValues.Length)
			{
				for (int i = 0; i != _boneList.Length; i++)
				{
					_boneList[i].PreUpdate_Prepare(updateFlags, (uint)iBoneValues[i]);
				}
			}
			if (_rigidBodyList != null && iRigidBodyValues != null && _rigidBodyList.Length == iRigidBodyValues.Length)
			{
				for (int j = 0; j != _rigidBodyList.Length; j++)
				{
					_rigidBodyList[j].preUpdate_updateRigidBodyFlags = (uint)iRigidBodyValues[j];
				}
			}
			if ((updateFlags & (true ? 1u : 0u)) != 0 && ikWeights != null && _ikList != null && ikWeights.Length == _ikList.Length)
			{
				for (int k = 0; k != _ikList.Length; k++)
				{
					_ikList[k].PreUpdate_MarkIKDepended(updateFlags, ikWeights[k]);
				}
			}
			if (_isVertexMorphEnabled && _isXDEFEnabled && _vertexData != null && _meshList != null && _vertexData.meshCount == _meshList.Length)
			{
				int meshCount = _vertexData.meshCount;
				AuxData.VertexData.MeshBoneInfo r = default(AuxData.VertexData.MeshBoneInfo);
				for (int l = 0; l != meshCount; l++)
				{
					if (_isBlendShapesEnabled && (_meshList[l].meshFlags & (true ? 1u : 0u)) != 0)
					{
						continue;
					}
					_vertexData.GetMeshBoneInfo(ref r, l);
					if (!r.isSDEF)
					{
						continue;
					}
					for (int m = 0; m != r.count; m++)
					{
						AuxData.VertexData.BoneFlags boneFlags = _vertexData.GetBoneFlags(ref r, m);
						if ((boneFlags & AuxData.VertexData.BoneFlags.SDEF) != 0)
						{
							int boneID = _vertexData.GetBoneID(ref r, m);
							if ((uint)boneID < (uint)_boneList.Length)
							{
								_boneList[l]._preUpdate_isXDEFDepended = true;
							}
						}
					}
				}
			}
			if (morphWeights != null && _morphList != null && morphWeights.Length == _morphList.Length)
			{
				for (int n = 0; n != _morphList.Length; n++)
				{
					_morphList[n].preUpdate_weight = morphWeights[n];
					_morphList[n].preUpdate_appendWeight = 0f;
				}
				for (int num = 0; num != _morphList.Length; num++)
				{
					_morphList[num].PreUpdate_ApplyGroupMorph();
				}
				for (int num2 = 0; num2 != _morphList.Length; num2++)
				{
					_morphList[num2].PreUpdate_ApplyMorph(updateFlags);
				}
			}
			for (int num3 = 0; num3 != _boneList.Length; num3++)
			{
				_boneList[num3].PreUpdate_CheckUpdated();
			}
			for (int num4 = 0; num4 != _boneList.Length; num4++)
			{
				_boneList[num4].PreUpdate_CheckUpdated2();
			}
			for (int num5 = 0; num5 != _boneList.Length; num5++)
			{
				if (_boneList[num5]._preUpdate_isGetLocalToWorldMatrix)
				{
					iBoneValues[num5] |= 1;
				}
				if (_boneList[num5]._preUpdate_isGetLocalPosition)
				{
					iBoneValues[num5] |= 2;
				}
				if (_boneList[num5]._preUpdate_isGetLocalRotation)
				{
					iBoneValues[num5] |= 4;
				}
			}
			return 1;
		}

		public static void ArrayCopy<Type>(Type[] src, Type[] dst)
		{
			if (src != null && dst != null && dst.Length == src.Length)
			{
				Array.Copy(src, dst, src.Length);
			}
		}

		public static void PrimArrayClone<Type>(Type[] src, ref Type[] dst)
		{
			if (src != null)
			{
				if (dst == null || dst.Length != src.Length)
				{
					dst = new Type[src.Length];
				}
				Buffer.BlockCopy(src, 0, dst, 0, Buffer.ByteLength(src));
			}
			else
			{
				dst = null;
			}
		}

		public static void ArrayClone<Type>(Type[] src, ref Type[] dst)
		{
			if (src != null)
			{
				if (dst == null || dst.Length != src.Length)
				{
					dst = new Type[src.Length];
				}
				Array.Copy(src, dst, src.Length);
			}
			else
			{
				dst = null;
			}
		}

		public void Update(uint updateFlags, int[] iValues, float[] fValues, ref Matrix4x4 modelTransform, int[] iBoneValues, Matrix4x4[] boneTransforms, Vector3[] bonePositions, Quaternion[] boneRotations, Vector3[] boneUserPositions, Quaternion[] boneUserRotations, int[] iRigidBodyValues, float[] ikWights, float[] morphWeights)
		{
			UpdateData updateData = null;
			if (_isMultiThreading)
			{
				lock (this)
				{
					updateData = _sync_unusedData;
					_sync_unusedData = null;
				}
			}
			else
			{
				updateData = _sync_unusedData;
				_sync_unusedData = null;
			}
			if (updateData == null)
			{
				updateData = new UpdateData();
			}
			updateData.updateFlags = updateFlags;
			if (iValues != null && iValues.Length >= 1)
			{
				updateData.updateAnimData.animationHashName = (uint)iValues[0];
			}
			else
			{
				updateData.updateAnimData.animationHashName = 0u;
			}
			if (fValues != null && fValues.Length >= 2)
			{
				updateData.updateAnimData.animationTime = fValues[0];
				updateData.updatePPHShoulderFixRate = fValues[1];
			}
			else
			{
				updateData.updateAnimData.animationTime = 0f;
				updateData.updatePPHShoulderFixRate = 0f;
			}
			updateData.updateModelTransform = modelTransform;
			if (_isMultiThreading)
			{
				PrimArrayClone(iBoneValues, ref updateData.updateBoneFlagsList);
				ArrayClone(boneTransforms, ref updateData.updateBoneTransformList);
				ArrayClone(bonePositions, ref updateData.updateBoneLocalPositionList);
				ArrayClone(boneRotations, ref updateData.updateBoneLocalRotationList);
				ArrayClone(boneUserPositions, ref updateData.updateBoneUserPositionList);
				ArrayClone(boneUserRotations, ref updateData.updateBoneUserRotationList);
				PrimArrayClone(iRigidBodyValues, ref updateData.updateRigidBodyFlagsList);
				PrimArrayClone(ikWights, ref updateData.updateIKWeightList);
				PrimArrayClone(morphWeights, ref updateData.updateMorphWeightList);
			}
			else
			{
				updateData.updateBoneFlagsList = iBoneValues;
				updateData.updateBoneTransformList = boneTransforms;
				updateData.updateBoneLocalPositionList = bonePositions;
				updateData.updateBoneLocalRotationList = boneRotations;
				updateData.updateBoneUserPositionList = boneUserPositions;
				updateData.updateBoneUserRotationList = boneUserRotations;
				updateData.updateRigidBodyFlagsList = iRigidBodyValues;
				updateData.updateIKWeightList = ikWights;
				updateData.updateMorphWeightList = morphWeights;
			}
			if (_isMultiThreading)
			{
				lock (this)
				{
					if (_sync_updateData != null)
					{
						_sync_updateData2 = updateData;
					}
					else
					{
						_sync_updateData = updateData;
					}
					return;
				}
			}
			if (_sync_updateData != null)
			{
				_sync_updateData2 = updateData;
			}
			else
			{
				_sync_updateData = updateData;
			}
		}

		public UpdateData LateUpdate()
		{
			UpdateData updateData = null;
			if (_isMultiThreading)
			{
				lock (this)
				{
					return _sync_lateUpdateData;
				}
			}
			return _sync_lateUpdateData;
		}

		public void _ResetWorldTransformOnMoving()
		{
			if (_boneList != null)
			{
				for (int i = 0; i < _boneList.Length; i++)
				{
					_boneList[i].ResetWorldTransformOnMoving();
				}
			}
		}

		public MMDBone GetBone(int boneID)
		{
			if (_boneList != null && (uint)boneID < (uint)_boneList.Length)
			{
				return _boneList[boneID];
			}
			return null;
		}

		public MMDMorph GetMorph(int morphID)
		{
			if (_morphList != null && (uint)morphID < (uint)_morphList.Length)
			{
				return _morphList[morphID];
			}
			return null;
		}

		public MMDRigidBody GetRigidBody(int rigidBodyID)
		{
			if (_rigidBodyList != null && (uint)rigidBodyID < (uint)_rigidBodyList.Length)
			{
				return _rigidBodyList[rigidBodyID];
			}
			return null;
		}

		public override bool _JoinWorld()
		{
			_isJoinedWorld = true;
			_isMultiThreading = base.physicsWorld != null && base.physicsWorld.isMultiThreading;
			return true;
		}

		public override void _LeaveWorld()
		{
			if (_isPhysicsEnabled)
			{
				if (_jointList != null)
				{
					for (int i = 0; i < _jointList.Length; i++)
					{
						_jointList[i].LeaveWorld();
					}
				}
				if (_rigidBodyList != null)
				{
					for (int j = 0; j < _rigidBodyList.Length; j++)
					{
						_rigidBodyList[j].LeaveWorld();
					}
				}
			}
			_isJoinedWorld = false;
			_isJoinedWorldInternal = false;
			_isMultiThreading = false;
			if (_isDestroyed)
			{
				_DestroyImmediate();
			}
		}

		public override float _GetResetWorldTime()
		{
			if (!_isPhysicsEnabled || !_isResetWorld)
			{
				return 0f;
			}
			return _resetMorphTime + _resetWaitTime;
		}

		public override void _PreResetWorld()
		{
			if (!_isPhysicsEnabled || !_isResetWorld)
			{
				return;
			}
			if (_isMultiThreading)
			{
				lock (this)
				{
					_updateData = _sync_updateData;
				}
			}
			else
			{
				_updateData = _sync_updateData;
			}
			_FeedbackUpdateData();
			_PerformTransform();
			_PerformTransformAfterPhysics();
			_PrepareMoveWorldTransform();
			_ProcessJoinWorld();
			_processResetWorld = true;
			_processResetRatio = 0f;
			_isResetWorld = false;
		}

		public override void _StepResetWorld(float elapsedTime)
		{
			if (!_isPhysicsEnabled || !_processResetWorld)
			{
				return;
			}
			if (elapsedTime < _resetMorphTime)
			{
				if (elapsedTime > 0f && _resetMorphTime > 0f)
				{
					_processResetRatio = elapsedTime / _resetMorphTime;
					_ResetWorldTransformOnMoving();
					_PerformMoveWorldTransform(_processResetRatio);
				}
			}
			else if (_processResetRatio != 1f)
			{
				_processResetRatio = 1f;
				_ResetWorldTransformOnMoving();
				_PerformMoveWorldTransform(1f);
			}
		}

		public override void _PostResetWorld()
		{
			if (!_isPhysicsEnabled || !_processResetWorld)
			{
				return;
			}
			if (_boneList != null)
			{
				for (int i = 0; i < _boneList.Length; i++)
				{
					_boneList[i].CleanupMoveWorldTransform();
				}
			}
			_processResetWorld = false;
			_processResetRatio = 0f;
		}

		public override void _PreUpdate()
		{
			if (!_processResetWorld)
			{
				_LockUpdateData();
				_FeedbackUpdateData();
				_PerformTransform();
			}
		}

		public override void _PostUpdate()
		{
			if (!_processResetWorld)
			{
				_PerformTransformAfterPhysics();
				_ProcessXDEF();
				_FeedbackLateUpdateData();
				_UnlockUpdateData();
			}
			_isResetWorld = false;
		}

		public override void _PreUpdateWorld(float deltaTime)
		{
			if (!_isPhysicsEnabled)
			{
				return;
			}
			_ProcessJoinWorld();
			if (_rigidBodyList != null)
			{
				for (int i = 0; i < _rigidBodyList.Length; i++)
				{
					_rigidBodyList[i].FeedbackBoneToBodyTransform();
				}
			}
			if (_simulatedRigidBodyList != null)
			{
				for (int j = 0; j < _simulatedRigidBodyList.Length; j++)
				{
					_simulatedRigidBodyList[j].ProcessPreBoneAlignment();
				}
			}
		}

		public override void _PostUpdateWorld(float deltaTime)
		{
			if (!_isPhysicsEnabled || !(deltaTime > 0f))
			{
				return;
			}
			if (_rigidBodyList != null)
			{
				for (int i = 0; i < _rigidBodyList.Length; i++)
				{
					_rigidBodyList[i].PrepareTransform();
				}
			}
			if (_simulatedRigidBodyList != null)
			{
				for (int j = 0; j < _simulatedRigidBodyList.Length; j++)
				{
					_simulatedRigidBodyList[j].ApplyTransformToBone(deltaTime, _processResetWorld);
				}
			}
			if (_processResetWorld || _optimizeSettings)
			{
				return;
			}
			if (_modelProperty != null && _modelProperty.rigidBodyIsAdditionalCollider)
			{
				DiscreteDynamicsWorld discreteDynamicsWorld = base.bulletWorld;
				if (discreteDynamicsWorld != null && discreteDynamicsWorld.GetDispatcher() != null)
				{
					IDispatcher dispatcher = discreteDynamicsWorld.GetDispatcher();
					int numManifolds = dispatcher.GetNumManifolds();
					for (int k = 0; k < numManifolds; k++)
					{
						PersistentManifold manifoldByIndexInternal = dispatcher.GetManifoldByIndexInternal(k);
						BulletXNA.BulletDynamics.RigidBody rigidBody = manifoldByIndexInternal.GetBody0() as BulletXNA.BulletDynamics.RigidBody;
						BulletXNA.BulletDynamics.RigidBody rigidBody2 = manifoldByIndexInternal.GetBody1() as BulletXNA.BulletDynamics.RigidBody;
						if (rigidBody != null && rigidBody2 != null)
						{
							MMDRigidBody mMDRigidBody = rigidBody.GetUserPointer() as MMDRigidBody;
							MMDRigidBody mMDRigidBody2 = rigidBody2.GetUserPointer() as MMDRigidBody;
							if (mMDRigidBody != null && mMDRigidBody2 != null)
							{
								mMDRigidBody.ProcessCollider(mMDRigidBody2);
							}
						}
					}
				}
			}
			if (_simulatedRigidBodyList != null)
			{
				for (int l = 0; l < _simulatedRigidBodyList.Length; l++)
				{
					_simulatedRigidBodyList[l].FeedbackTransform();
				}
				for (int m = 0; m < _simulatedRigidBodyList.Length; m++)
				{
					_simulatedRigidBodyList[m].AntiJitterTransform();
				}
			}
		}

		public override void _NoUpdateWorld()
		{
			if (!_isPhysicsEnabled || _simulatedRigidBodyList == null)
			{
				return;
			}
			for (int i = 0; i < _simulatedRigidBodyList.Length; i++)
			{
				if (_simulatedRigidBodyList[i].isSimulated && !_simulatedRigidBodyList[i].isFreezed && _simulatedRigidBodyList[i]._bone != null)
				{
					_simulatedRigidBodyList[i]._bone._WriteWorldTransform(ref _simulatedRigidBodyList[i]._bone._worldTransformBeforePhysics);
				}
			}
		}

		public override void _PrepareLateUpdate()
		{
			if (_isMultiThreading)
			{
				lock (this)
				{
					_sync_unusedData = _sync_lateUpdateData;
					_sync_lateUpdateData = _sync_updatedData;
					_sync_updatedData = null;
					return;
				}
			}
			_sync_unusedData = _sync_lateUpdateData;
			_sync_lateUpdateData = _sync_updatedData;
			_sync_updatedData = null;
		}

		private void _ProcessJoinWorld()
		{
			if (!_isPhysicsEnabled || _isJoinedWorldInternal)
			{
				return;
			}
			_isJoinedWorldInternal = true;
			if (_rigidBodyList != null)
			{
				for (int i = 0; i < _rigidBodyList.Length; i++)
				{
					_rigidBodyList[i].JoinWorld();
				}
			}
			if (_jointList != null)
			{
				for (int j = 0; j < _jointList.Length; j++)
				{
					_jointList[j].JoinWorld();
				}
			}
		}

		private void _MakeSimulatedRigidBodyList()
		{
			if (_rigidBodyList == null)
			{
				return;
			}
			int num = 0;
			for (int i = 0; i < _rigidBodyList.Length; i++)
			{
				if (_rigidBodyList[i].bone != null && _rigidBodyList[i].isSimulated)
				{
					num++;
				}
			}
			_simulatedRigidBodyList = new MMDRigidBody[num];
			int j = 0;
			int num2 = 0;
			for (; j < _rigidBodyList.Length; j++)
			{
				if (_rigidBodyList[j].bone != null && _rigidBodyList[j].isSimulated)
				{
					_simulatedRigidBodyList[num2] = _rigidBodyList[j];
					num2++;
				}
			}
			for (int k = 0; k + 1 < _simulatedRigidBodyList.Length; k++)
			{
				if (_simulatedRigidBodyList[k].originalParentBoneID < 0)
				{
					continue;
				}
				for (int l = k + 1; l < _simulatedRigidBodyList.Length; l++)
				{
					if (_simulatedRigidBodyList[l].originalParentBoneID < 0)
					{
						_Swap(ref _simulatedRigidBodyList[k], ref _simulatedRigidBodyList[l]);
						break;
					}
					if (_IsParentBone(_simulatedRigidBodyList[l].bone, _simulatedRigidBodyList[k].bone))
					{
						_Swap(ref _simulatedRigidBodyList[k], ref _simulatedRigidBodyList[l]);
					}
				}
			}
		}

		private void _PrepareMoveWorldTransform()
		{
			if (_isPhysicsEnabled && _boneList != null)
			{
				for (int i = 0; i < _boneList.Length; i++)
				{
					bool isMovingOnResetWorld = _boneList[i]._isKinematicRigidBody || _boneList[i].isRigidBodyFreezed;
					_boneList[i].FeedbackMoveWorldTransform(isMovingOnResetWorld);
				}
				for (int j = 0; j < _boneList.Length; j++)
				{
					_boneList[j].PrepareMoveWorldTransform();
				}
			}
		}

		private void _PerformMoveWorldTransform(float r)
		{
			if (_isPhysicsEnabled && _boneList != null)
			{
				for (int i = 0; i < _boneList.Length; i++)
				{
					_boneList[i].PerformMoveWorldTransform(r);
				}
			}
		}

		private void _PerformTransform()
		{
			_isAfterIK = false;
			_isAfterPhysics = false;
			if (_sortedBoneList == null)
			{
				return;
			}
			for (int i = 0; i < _sortedBoneList.Length; i++)
			{
				_sortedBoneList[i]._PrepareTransform();
			}
			for (int j = 0; j < _sortedBoneList.Length; j++)
			{
				_sortedBoneList[j]._PerformTransform();
			}
			if (_fileType == MMDFileType.PMD && _ikList != null)
			{
				for (int k = 0; k < _ikList.Length; k++)
				{
					_ikList[k].Solve();
				}
				_isAfterIK = true;
				for (int l = 0; l < _sortedBoneList.Length; l++)
				{
					_sortedBoneList[l]._PerformTransform();
				}
			}
		}

		private void _PerformTransformAfterPhysics()
		{
			_isAfterPhysics = true;
			if (_fileType == MMDFileType.PMX)
			{
				for (int i = 0; i < _sortedBoneList.Length; i++)
				{
					_sortedBoneList[i]._PerformTransform();
				}
			}
		}

		private void _ProcessXDEF()
		{
			if (!_isXDEFEnabled || _meshList == null)
			{
				return;
			}
			bool flag = false;
			for (int i = 0; i != _meshList.Length; i++)
			{
				flag |= _meshList[i].PrepareXDEF();
			}
			if (flag)
			{
				if (_isVertexMorphEnabled && !_isBlendShapesEnabled)
				{
					_WaitVertexMorph();
				}
				IndexedMatrix xdefRootTransformInv = _modelTransform.Inverse();
				IndexedQuaternion xdefRootRotationInv = _modelTransform.GetRotation().Inverse();
				for (int j = 0; j != _meshList.Length; j++)
				{
					_meshList[j].ProcessXDEF(ref xdefRootTransformInv, ref xdefRootRotationInv);
				}
			}
		}

		private void _DestroyImmediate()
		{
			if (_isVertexMorphEnabled && !_isBlendShapesEnabled)
			{
				_WaitVertexMorph();
			}
			if (_ikList != null)
			{
				for (int i = 0; i < _ikList.Length; i++)
				{
					_ikList[i].Destroy();
				}
			}
			if (_jointList != null)
			{
				for (int j = 0; j < _jointList.Length; j++)
				{
					_jointList[j].Destroy();
				}
			}
			if (_rigidBodyList != null)
			{
				for (int k = 0; k < _rigidBodyList.Length; k++)
				{
					_rigidBodyList[k].Destroy();
				}
			}
			if (_boneList != null)
			{
				for (int l = 0; l < _boneList.Length; l++)
				{
					_boneList[l].Destroy();
				}
			}
			_simulatedRigidBodyList = null;
			_sortedBoneList = null;
			_ikList = null;
			_jointList = null;
			_rigidBodyList = null;
			_boneList = null;
			_rootBone = null;
		}

		private void _LockUpdateData()
		{
			if (_isMultiThreading)
			{
				lock (this)
				{
					_updateData = _sync_updateData;
					_sync_updateData = _sync_updateData2;
					_sync_updateData2 = null;
					return;
				}
			}
			_updateData = _sync_updateData;
			_sync_updateData = _sync_updateData2;
			_sync_updateData2 = null;
		}

		private void _UnlockUpdateData()
		{
			if (_isMultiThreading)
			{
				lock (this)
				{
					_sync_updatedData = _updateData;
				}
			}
			else
			{
				_sync_updatedData = _updateData;
			}
			_updateData = null;
		}

		private void _GetWorldTransformToBone(ref IndexedMatrix transform, ref Matrix4x4 t)
		{
			if (_isLossyScaleIdentity)
			{
				transform._basis.SetValue(t.m00, 0f - t.m01, 0f - t.m02, 0f - t.m10, t.m11, t.m12, 0f - t.m20, t.m21, t.m22);
			}
			else if (_isLossyScaleSquare)
			{
				transform._basis.SetValue(t.m00 * _lossyScaleInv, (0f - t.m01) * _lossyScaleInv, (0f - t.m02) * _lossyScaleInv, (0f - t.m10) * _lossyScaleInv, t.m11 * _lossyScaleInv, t.m12 * _lossyScaleInv, (0f - t.m20) * _lossyScaleInv, t.m21 * _lossyScaleInv, t.m22 * _lossyScaleInv);
			}
			else
			{
				float num = Mathf.Sqrt(t.m00 * t.m00 + t.m01 * t.m01 + t.m02 * t.m02);
				float num2 = Mathf.Sqrt(t.m10 * t.m10 + t.m11 * t.m11 + t.m12 * t.m12);
				float num3 = Mathf.Sqrt(t.m20 * t.m20 + t.m21 * t.m21 + t.m22 * t.m22);
				num = ((Mathf.Abs(num) > float.Epsilon) ? (1f / num) : 0f);
				num2 = ((Mathf.Abs(num2) > float.Epsilon) ? (1f / num2) : 0f);
				num3 = ((Mathf.Abs(num3) > float.Epsilon) ? (1f / num3) : 0f);
				transform._basis.SetValue(t.m00 * num, (0f - t.m01) * num, (0f - t.m02) * num, (0f - t.m10) * num2, t.m11 * num2, t.m12 * num2, (0f - t.m20) * num3, t.m21 * num3, t.m22 * num3);
			}
			transform._origin = new IndexedVector3(0f - t.m03, t.m13, t.m23) * _worldToBulletScale;
		}

		private void _FeedbackUpdateData()
		{
			if (_updateData == null || _boneList == null)
			{
				return;
			}
			int num = _boneList.Length;
			_isIKEnabled = (_updateData.updateFlags & 1) != 0;
			_isBoneInherenceEnabled = (_updateData.updateFlags & 2) != 0;
			_isBoneMorphEnabled = (_updateData.updateFlags & 4) != 0;
			_pphShoulderFixRate2 = _pphShoulderFixRate;
			_pphShoulderFixRate = _updateData.updatePPHShoulderFixRate;
			_GetWorldTransformToBone(ref _modelTransform, ref _updateData.updateModelTransform);
			_updateBoneTransform = 0uL;
			if (_updateData.updateRigidBodyFlagsList != null && _rigidBodyList != null && _updateData.updateRigidBodyFlagsList.Length == _rigidBodyList.Length)
			{
				for (int i = 0; i < _rigidBodyList.Length; i++)
				{
					bool freezed = ((ulong)_updateData.updateRigidBodyFlagsList[i] & 1uL) != 0;
					_rigidBodyList[i].SetFreezed(freezed);
				}
			}
			if (_updateData.updateBoneFlagsList != null && _updateData.updateBoneFlagsList.Length == num && _updateData.updateBoneTransformList != null && _updateData.updateBoneTransformList.Length == num && _updateData.updateBoneLocalPositionList != null && _updateData.updateBoneLocalPositionList.Length == num && _updateData.updateBoneLocalRotationList != null && _updateData.updateBoneLocalRotationList.Length == num)
			{
				uint updateFlags = _updateData.updateFlags;
				IndexedMatrix transform = IndexedMatrix.Identity;
				for (int j = 0; j != num; j++)
				{
					uint num2 = (uint)_updateData.updateBoneFlagsList[j];
					_boneList[j].PrepareUpdate(updateFlags, num2);
					if ((num2 & (true ? 1u : 0u)) != 0)
					{
						_GetWorldTransformToBone(ref transform, ref _updateData.updateBoneTransformList[j]);
						_boneList[j].SetUnityWorldTransform(ref transform);
					}
					if ((num2 & 2u) != 0)
					{
						IndexedVector3 unityLocalPosition = _updateData.updateBoneLocalPositionList[j];
						_boneList[j].SetUnityLocalPosition(ref unityLocalPosition);
					}
					if ((num2 & 4u) != 0)
					{
						IndexedQuaternion unityLocalRotation = _updateData.updateBoneLocalRotationList[j];
						_boneList[j].SetUnityLocalRotation(ref unityLocalRotation);
					}
				}
			}
			for (int k = 0; k != num; k++)
			{
				_boneList[k].ComputeTransform();
			}
			if (_updateData.updateBoneUserPositionList != null && _updateData.updateBoneUserPositionList.Length == num && _updateData.updateBoneUserRotationList != null && _updateData.updateBoneUserRotationList.Length == num)
			{
				for (int l = 0; l < num; l++)
				{
					float num3 = ((_boneList[l]._modifiedParentBone != null) ? _localToBulletScale : _worldToBulletScale);
					_boneList[l]._userPosition = _updateData.updateBoneUserPositionList[l] * num3;
					_boneList[l]._userRotation = _updateData.updateBoneUserRotationList[l];
					_boneList[l]._userPosition.v.X = 0f - _boneList[l]._userPosition.v.X;
					_boneList[l]._userRotation.q.Y = 0f - _boneList[l]._userRotation.q.Y;
					_boneList[l]._userRotation.q.Z = 0f - _boneList[l]._userRotation.q.Z;
				}
			}
			if (_isVertexMorphEnabled && !_isBlendShapesEnabled)
			{
				_WaitVertexMorph();
			}
			if (_meshList != null)
			{
				for (int m = 0; m != _meshList.Length; m++)
				{
					_meshList[m].PrepareUpdate();
				}
			}
			if (_updateData.updateMorphWeightList != null && _morphList != null && _updateData.updateMorphWeightList.Length == _morphList.Length)
			{
				for (int n = 0; n != _morphList.Length; n++)
				{
					_morphList[n]._backupWeight = _morphList[n].weight + _morphList[n].appendWeight;
					_morphList[n].weight = _updateData.updateMorphWeightList[n];
					_morphList[n].appendWeight = 0f;
				}
				for (int num4 = 0; num4 != _morphList.Length; num4++)
				{
					_morphList[num4].ApplyGroupMorph();
				}
				for (int num5 = 0; num5 != _morphList.Length; num5++)
				{
					_morphList[num5].ApplyMorph();
				}
			}
			if (_meshList == null || !_isVertexMorphEnabled || _isBlendShapesEnabled)
			{
				return;
			}
			bool flag = false;
			for (int num6 = 0; num6 != _meshList.Length; num6++)
			{
				if (flag)
				{
					break;
				}
				flag |= _meshList[num6].isMorphChanged;
			}
			if (!flag)
			{
				return;
			}
			if (_updateData.lateUpdateMeshDataList != null && _updateData.lateUpdateMeshDataList.Length == _meshList.Length)
			{
				for (int num7 = 0; num7 != _meshList.Length; num7++)
				{
					if (_updateData.lateUpdateMeshDataList[num7] != null)
					{
						_meshList[num7].vertices = _updateData.lateUpdateMeshDataList[num7].vertices;
					}
				}
			}
			_RunVertexMorph();
		}

		private void _FeedbackLateUpdateData()
		{
			if (_updateData == null || _boneList == null || _sortedBoneList == null)
			{
				return;
			}
			_updateData.lateUpdateFlags = 1u;
			int num = _boneList.Length;
			if (_updateData.lateUpdateBoneFlagsList == null || _updateData.lateUpdateBoneFlagsList.Length != num)
			{
				_updateData.lateUpdateBoneFlagsList = new int[num];
			}
			else
			{
				for (int i = 0; i != num; i++)
				{
					_updateData.lateUpdateBoneFlagsList[i] = 0;
				}
			}
			if (_updateData.lateUpdateBonePositionList == null || _updateData.lateUpdateBonePositionList.Length != num)
			{
				_updateData.lateUpdateBonePositionList = new Vector3[num];
			}
			if (_updateData.lateUpdateBoneRotationList == null || _updateData.lateUpdateBoneRotationList.Length != num)
			{
				_updateData.lateUpdateBoneRotationList = new Quaternion[num];
			}
			for (int j = 0; j != num; j++)
			{
				MMDBone mMDBone = _boneList[j];
				mMDBone.ComputeUnityTransform();
				if (mMDBone._isLateUpdatePosition || mMDBone._isLateUpdateRotation)
				{
					_updateData.lateUpdateBoneFlagsList[j] = 1;
					if (mMDBone._isLateUpdatePosition)
					{
						_updateData.lateUpdateBoneFlagsList[j] |= 2;
						_updateData.lateUpdateBonePositionList[j] = mMDBone._unityLocalPosition;
					}
					if (mMDBone._isLateUpdateRotation)
					{
						_updateData.lateUpdateBoneFlagsList[j] |= 4;
						_updateData.lateUpdateBoneRotationList[j] = mMDBone._unityLocalRotation;
					}
				}
			}
			if (_isVertexMorphEnabled && !_isBlendShapesEnabled)
			{
				_WaitVertexMorph();
			}
			bool flag = false;
			if (_meshList != null)
			{
				if (_updateData.lateUpdateMeshFlagsList == null || _updateData.lateUpdateMeshFlagsList.Length != _meshList.Length)
				{
					_updateData.lateUpdateMeshFlagsList = new int[_meshList.Length];
				}
				for (int k = 0; k != _meshList.Length; k++)
				{
					_updateData.lateUpdateMeshFlagsList[k] = 0;
				}
				for (int l = 0; l != _meshList.Length; l++)
				{
					if (_meshList[l].isChanged)
					{
						_updateData.lateUpdateMeshFlagsList[l] = 1;
						flag = true;
					}
				}
			}
			else
			{
				_updateData.lateUpdateMeshFlagsList = null;
			}
			if (!flag)
			{
				return;
			}
			_updateData.lateUpdateFlags |= 2u;
			if (_updateData.lateUpdateMeshDataList == null || _updateData.lateUpdateMeshDataList.Length != _meshList.Length)
			{
				_updateData.lateUpdateMeshDataList = new LateUpdateMeshData[_meshList.Length];
			}
			LateUpdateMeshData[] lateUpdateMeshDataList = _updateData.lateUpdateMeshDataList;
			for (int m = 0; m != _meshList.Length; m++)
			{
				if (lateUpdateMeshDataList[m] == null)
				{
					lateUpdateMeshDataList[m] = new LateUpdateMeshData();
				}
				LateUpdateMeshData lateUpdateMeshData = lateUpdateMeshDataList[m];
				if (_meshList[m].isChanged)
				{
					lateUpdateMeshData.lateUpdateMeshFlags = 1u;
					lateUpdateMeshData.vertices = _meshList[m].vertices;
					_meshList[m].vertices = null;
				}
				else
				{
					lateUpdateMeshData.lateUpdateMeshFlags = 0u;
				}
			}
		}

		private void _RunVertexMorph()
		{
			if (_isVertexMorphEnabled && !_isBlendShapesEnabled)
			{
				_WaitVertexMorph();
				if (_vertexMorphThreadQueue != null && Global.bridge != null)
				{
					_vertexMorphThreadQueueHandle = Global.bridge.InvokeCachedThreadQueue(_vertexMorphThreadQueue, _VertexMorph);
				}
			}
		}

		private void _WaitVertexMorph()
		{
			if (_isVertexMorphEnabled && !_isBlendShapesEnabled && _vertexMorphThreadQueue != null && Global.bridge != null)
			{
				Global.bridge.WaitEndCachedThreadQueue(_vertexMorphThreadQueue, ref _vertexMorphThreadQueueHandle);
			}
		}

		public void _VertexMorph()
		{
			if (_meshList == null)
			{
				return;
			}
			for (int i = 0; i != _meshList.Length; i++)
			{
				if (_meshList[i].isMorphChanged)
				{
					_meshList[i].PrepareMorph();
				}
			}
			for (int j = 0; j != _morphList.Length; j++)
			{
				_morphList[j].ProcessVertexMorph();
			}
		}
	}
}
