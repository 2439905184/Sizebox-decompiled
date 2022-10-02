using BulletXNA.LinearMath;
using UnityEngine;

namespace MMD4MecanimInternal.Bullet
{
	public class MMDMorph
	{
		public const int MeshCountBitShift = 24;

		public const int VertexIndexBitMask = 16777215;

		public const int HeaderSize = 2;

		public MMDModel _model;

		public float preUpdate_weight;

		public float preUpdate_appendWeight;

		public float weight;

		public float appendWeight;

		public float _backupWeight;

		private int _precheckedDependMesh = -1;

		private uint _additionalFlags;

		private MMDMorphCategory _morphCategory;

		private MMDMorphType _morphType;

		private uint _indexCount;

		private uint[] _indices;

		private float[] _weights;

		private Vector3[] _vertices;

		private IndexedVector3[] _bonePositions;

		private IndexedQuaternion[] _boneRotations;

		private bool[] _bonePositionsIsZero;

		private bool[] _boneRotationsIsIdentity;

		private MMDBone[] _bones;

		private MMDMorph[] _morphs;

		private bool[] _dependMesh;

		public MMDModel model
		{
			get
			{
				return _model;
			}
		}

		public bool isMorphBaseVertex
		{
			get
			{
				return (_additionalFlags & 1) != 0;
			}
		}

		public MMDMorphCategory morphCategory
		{
			get
			{
				return _morphCategory;
			}
		}

		public bool Import(BinaryReader binaryReader)
		{
			if (!binaryReader.BeginStruct())
			{
				return false;
			}
			_additionalFlags = (uint)binaryReader.ReadStructInt();
			binaryReader.ReadStructInt();
			binaryReader.ReadStructInt();
			_morphCategory = (MMDMorphCategory)binaryReader.ReadStructInt();
			_morphType = (MMDMorphType)binaryReader.ReadStructInt();
			_indexCount = (uint)binaryReader.ReadStructInt();
			switch (_morphType)
			{
			case MMDMorphType.Vertex:
			{
				_indices = new uint[_indexCount];
				_vertices = new Vector3[_indexCount];
				for (uint num2 = 0u; num2 != _indexCount; num2++)
				{
					_indices[num2] = (uint)binaryReader.ReadInt();
					_vertices[num2] = binaryReader.ReadVector3();
					if (_indices[num2] >= _model._vertexCount)
					{
						return false;
					}
				}
				if (Mathf.Abs(_model._importScaleMMDModel - _model._modelProperty.importScale) > float.Epsilon && _model._importScaleMMDModel > float.Epsilon)
				{
					float num3 = _model._modelProperty.importScale / _model._importScaleMMDModel;
					for (int i = 0; i != _indexCount; i++)
					{
						_vertices[i] *= num3;
					}
				}
				break;
			}
			case MMDMorphType.Group:
			{
				_morphs = new MMDMorph[_indexCount];
				_weights = new float[_indexCount];
				for (uint num4 = 0u; num4 != _indexCount; num4++)
				{
					int morphID = binaryReader.ReadInt();
					_weights[num4] = binaryReader.ReadFloat();
					_morphs[num4] = _model.GetMorph(morphID);
				}
				break;
			}
			case MMDMorphType.Bone:
			{
				_bones = new MMDBone[_indexCount];
				_bonePositions = new IndexedVector3[_indexCount];
				_boneRotations = new IndexedQuaternion[_indexCount];
				_bonePositionsIsZero = new bool[_indexCount];
				_boneRotationsIsIdentity = new bool[_indexCount];
				for (uint num = 0u; num != _indexCount; num++)
				{
					int boneID = binaryReader.ReadInt();
					_bonePositions[num] = binaryReader.ReadVector3();
					_boneRotations[num] = binaryReader.ReadQuaternion();
					_bonePositionsIsZero[num] = _bonePositions[num] == IndexedVector3.Zero;
					_boneRotationsIsIdentity[num] = _boneRotations[num] == IndexedQuaternion.Identity;
					_bonePositions[num].Z = 0f - _bonePositions[num].Z;
					_boneRotations[num].X = 0f - _boneRotations[num].X;
					_boneRotations[num].Y = 0f - _boneRotations[num].Y;
					_bonePositions[num] *= _model._modelToBulletScale;
					_bones[num] = _model.GetBone(boneID);
				}
				break;
			}
			}
			if (!binaryReader.EndStruct())
			{
				return false;
			}
			return true;
		}

		public bool PrepareDependMesh()
		{
			if (_morphType == MMDMorphType.Vertex)
			{
				MMDMesh[] meshList = _model._meshList;
				AuxData.IndexData indexData = _model._indexData;
				if (indexData != null && meshList.Length == indexData.meshCount)
				{
					uint num = (uint)meshList.Length;
					_dependMesh = new bool[num];
					int[] indexValues = indexData.indexValues;
					int num2 = indexValues.Length;
					int num3 = _indices.Length;
					for (int i = 0; i != num3; i++)
					{
						uint num4 = _indices[i];
						if (2 + num4 + 1 < (uint)num2)
						{
							uint num5 = (uint)indexValues[2 + num4];
							uint num6 = (uint)indexValues[2 + num4 + 1];
							if (num5 >= (uint)num2 || num6 > (uint)num2)
							{
								continue;
							}
							for (; num5 != num6; num5++)
							{
								uint num7 = (uint)indexValues[num5] >> 24;
								if (num7 < num && meshList[num7] != null)
								{
									_dependMesh[num7] = true;
									meshList[num7].meshFlags |= 2u;
									continue;
								}
								_dependMesh = null;
								return false;
							}
							continue;
						}
						_dependMesh = null;
						return false;
					}
				}
			}
			return true;
		}

		public void PreUpdate_ApplyGroupMorph()
		{
		}

		public void PreUpdate_ApplyMorph(uint updateFlags)
		{
			if (isMorphBaseVertex)
			{
				return;
			}
			float f = preUpdate_weight;
			switch (_morphType)
			{
			case MMDMorphType.Bone:
			{
				if ((updateFlags & 4) == 0 || _bones == null || _bonePositionsIsZero == null || _boneRotationsIsIdentity == null)
				{
					break;
				}
				if (Mathf.Abs(f) <= float.Epsilon)
				{
					for (int i = 0; i != _bones.Length; i++)
					{
						MMDBone mMDBone = _bones[i];
						if (mMDBone != null)
						{
							mMDBone._preUpdate_isMorphPositionDepended = false;
							mMDBone._preUpdate_isMorphRotationDepended = false;
						}
					}
					break;
				}
				for (int j = 0; j != _bones.Length; j++)
				{
					MMDBone mMDBone2 = _bones[j];
					if (mMDBone2 != null)
					{
						if (_bonePositionsIsZero[j])
						{
							mMDBone2._preUpdate_isMorphPositionDepended = false;
						}
						else
						{
							mMDBone2._preUpdate_isMorphPositionDepended = true;
						}
						if (_boneRotationsIsIdentity[j])
						{
							mMDBone2._preUpdate_isMorphRotationDepended = false;
						}
						else
						{
							mMDBone2._preUpdate_isMorphRotationDepended = true;
						}
					}
				}
				break;
			}
			case MMDMorphType.Vertex:
				break;
			}
		}

		public void PrepareUpdate()
		{
			_backupWeight = weight;
		}

		public void ApplyGroupMorph()
		{
		}

		public void ApplyMorph()
		{
			if (isMorphBaseVertex)
			{
				return;
			}
			float num = weight;
			switch (_morphType)
			{
			case MMDMorphType.Vertex:
				if (num != _backupWeight)
				{
					_MarkChangedDependMesh();
				}
				break;
			case MMDMorphType.Bone:
			{
				if (!_model._isBoneMorphEnabled || _bones == null || _bonePositions == null || _boneRotations == null || _bonePositionsIsZero == null || _boneRotationsIsIdentity == null || Mathf.Abs(num) <= float.Epsilon)
				{
					break;
				}
				if (Mathf.Abs(num - 1f) <= float.Epsilon)
				{
					for (int i = 0; i != _bones.Length; i++)
					{
						MMDBone mMDBone = _bones[i];
						if (mMDBone != null)
						{
							if (!_bonePositionsIsZero[i])
							{
								mMDBone._morphPosition += _bonePositions[i];
								mMDBone._isMorphPositionDepended = true;
							}
							if (!_boneRotationsIsIdentity[i])
							{
								mMDBone._morphRotation *= _boneRotations[i];
								mMDBone._isMorphRotationDepended = true;
							}
						}
					}
					break;
				}
				for (int j = 0; j != _bones.Length; j++)
				{
					MMDBone mMDBone2 = _bones[j];
					if (mMDBone2 != null)
					{
						if (!_bonePositionsIsZero[j])
						{
							mMDBone2._morphPosition += _bonePositions[j] * num;
							mMDBone2._isMorphPositionDepended = true;
						}
						if (!_boneRotationsIsIdentity[j])
						{
							mMDBone2._morphRotation *= Math.SlerpFromIdentity(ref _boneRotations[j], num);
							mMDBone2._isMorphRotationDepended = true;
						}
					}
				}
				break;
			}
			}
		}

		public void ProcessVertexMorph()
		{
			if (_morphType == MMDMorphType.Vertex)
			{
				if (isMorphBaseVertex)
				{
					_ProcessVertexBaseMorph();
				}
				else
				{
					_ProcessVertexMorph();
				}
			}
		}

		private void _MarkChangedDependMesh()
		{
			MMDMesh[] meshList = _model._meshList;
			if (meshList == null || _dependMesh == null || meshList.Length != _dependMesh.Length)
			{
				return;
			}
			for (int i = 0; i != _dependMesh.Length; i++)
			{
				if (_dependMesh[i])
				{
					meshList[i].isMorphChanged = true;
				}
			}
		}

		private bool _PrecheckDependMesh()
		{
			if (_precheckedDependMesh != -1)
			{
				return _precheckedDependMesh != 0;
			}
			_precheckedDependMesh = 0;
			MMDMesh[] meshList = _model._meshList;
			AuxData.IndexData indexData = _model._indexData;
			if (meshList != null && indexData != null && _dependMesh != null)
			{
				int num = meshList.Length;
				if (_indices != null && _vertices != null && _indices.Length == _vertices.Length)
				{
					int[] indexValues = indexData.indexValues;
					if (indexValues != null)
					{
						int num2 = indexValues.Length;
						int num3 = _vertices.Length;
						for (int i = 0; i != num3; i++)
						{
							uint num4 = _indices[i];
							if (2 + num4 + 1 < (uint)num2)
							{
								uint num5 = (uint)indexValues[2 + num4];
								uint num6 = (uint)indexValues[2 + num4 + 1];
								if (num5 < (uint)num2 && num6 <= (uint)num2)
								{
									for (; num5 != num6; num5++)
									{
										uint num7 = (uint)indexValues[num5] >> 24;
										uint num8 = (uint)indexValues[num5] & 0xFFFFFFu;
										if (num7 < (uint)num)
										{
											if (meshList[num7] == null || meshList[num7].backupVertices == null || num8 >= meshList[num7].backupVertices.Length)
											{
												return false;
											}
											continue;
										}
										return false;
									}
									continue;
								}
								return false;
							}
							return false;
						}
						_precheckedDependMesh = 1;
						return true;
					}
				}
			}
			return false;
		}

		private void _ProcessVertexBaseMorph()
		{
			if (!_PrecheckDependMesh())
			{
				return;
			}
			MMDMesh[] meshList = _model._meshList;
			AuxData.IndexData indexData = _model._indexData;
			if (meshList == null || indexData == null)
			{
				return;
			}
			int[] indexValues = indexData.indexValues;
			int num = _vertices.Length;
			for (int i = 0; i != num; i++)
			{
				uint num2 = _indices[i];
				uint num3 = (uint)indexValues[2 + num2];
				for (uint num4 = (uint)indexValues[2 + num2 + 1]; num3 != num4; num3++)
				{
					uint num5 = (uint)indexValues[num3] >> 24;
					uint num6 = (uint)indexValues[num3] & 0xFFFFFFu;
					if (meshList[num5].isMorphChanged)
					{
						meshList[num5].vertices[num6] += _vertices[i];
					}
				}
			}
		}

		private void _ProcessVertexMorph()
		{
			if (!_PrecheckDependMesh())
			{
				return;
			}
			float num = weight;
			MMDMesh[] meshList = _model._meshList;
			AuxData.IndexData indexData = _model._indexData;
			if (meshList == null || indexData == null)
			{
				return;
			}
			int[] indexValues = indexData.indexValues;
			if (!(num > float.Epsilon))
			{
				return;
			}
			int num2 = _vertices.Length;
			if (Mathf.Abs(1f - num) <= float.Epsilon)
			{
				for (int i = 0; i != num2; i++)
				{
					uint num3 = _indices[i];
					uint num4 = (uint)indexValues[2 + num3];
					for (uint num5 = (uint)indexValues[2 + num3 + 1]; num4 != num5; num4++)
					{
						uint num6 = (uint)indexValues[num4] >> 24;
						uint num7 = (uint)indexValues[num4] & 0xFFFFFFu;
						meshList[num6].vertices[num7] += _vertices[i];
					}
				}
				return;
			}
			for (int j = 0; j != num2; j++)
			{
				uint num8 = _indices[j];
				uint num9 = (uint)indexValues[2 + num8];
				for (uint num10 = (uint)indexValues[2 + num8 + 1]; num9 != num10; num9++)
				{
					uint num11 = (uint)indexValues[num9] >> 24;
					uint num12 = (uint)indexValues[num9] & 0xFFFFFFu;
					meshList[num11].vertices[num12] += _vertices[j] * num;
				}
			}
		}
	}
}
