using System;
using BulletXNA.LinearMath;
using UnityEngine;

namespace MMD4MecanimInternal.Bullet;

public class MMDMesh
{
	public MMDModel _model;

	public int meshID = -1;

	public uint meshFlags;

	public bool isMorphChanged;

	public bool isXDEFChanged;

	public Vector3[] vertices;

	public Vector3[] backupVertices;

	public Vector3[] normals;

	public Vector3[] backupNormals;

	public BoneWeight[] boneWeights;

	public IndexedMatrix[] bindposes;

	private int _xdefSDEFVertexIndex;

	private int _xdefSDEFVertexCount;

	private IndexedMatrix[] _xdefBoneTransformCache;

	private IndexedMatrix[] _xdefBoneTransformInvCache;

	private IndexedQuaternion[] _xdefBoneRotationCache;

	public MMDModel model => _model;

	public bool isChanged
	{
		get
		{
			if (!isMorphChanged)
			{
				return isXDEFChanged;
			}
			return true;
		}
	}

	public void Destroy()
	{
		vertices = null;
		backupVertices = null;
		normals = null;
		backupNormals = null;
		boneWeights = null;
		bindposes = null;
		_xdefBoneTransformCache = null;
		_xdefBoneTransformInvCache = null;
		_xdefBoneRotationCache = null;
	}

	public bool UploadMesh(int meshID, Vector3[] vertices, Vector3[] normals, BoneWeight[] boneWeights, Matrix4x4[] bindposes)
	{
		this.meshID = meshID;
		this.vertices = null;
		backupVertices = vertices;
		this.normals = null;
		backupNormals = normals;
		this.boneWeights = boneWeights;
		if (bindposes != null)
		{
			this.bindposes = new IndexedMatrix[bindposes.Length];
			for (int i = 0; i != bindposes.Length; i++)
			{
				ref IndexedMatrix reference = ref this.bindposes[i];
				reference = Math.MakeIndexedMatrix(ref bindposes[i]);
			}
		}
		if (!_PrepareAndValidationCheck())
		{
			Destroy();
			return false;
		}
		return true;
	}

	public void PrepareUpdate()
	{
		isMorphChanged = false;
	}

	public void PrepareMorph()
	{
		isXDEFChanged = false;
		if (backupVertices != null)
		{
			if (vertices == null || vertices.Length != backupVertices.Length)
			{
				vertices = (Vector3[])backupVertices.Clone();
			}
			else
			{
				Array.Copy(backupVertices, vertices, backupVertices.Length);
			}
		}
	}

	public bool PrepareXDEF()
	{
		if (!_model._isVertexMorphEnabled || !_model._isXDEFEnabled)
		{
			return false;
		}
		if ((meshFlags & 4) == 0)
		{
			return false;
		}
		if (_model._isBlendShapesEnabled && (meshFlags & (true ? 1u : 0u)) != 0)
		{
			return false;
		}
		if (_xdefBoneTransformCache == null)
		{
			return false;
		}
		isXDEFChanged = true;
		return true;
	}

	public void ProcessXDEF(ref IndexedMatrix xdefRootTransformInv, ref IndexedQuaternion xdefRootRotationInv)
	{
		if (!isXDEFChanged)
		{
			return;
		}
		if (!isMorphChanged)
		{
			if (vertices == null || vertices.Length != backupVertices.Length)
			{
				vertices = (Vector3[])backupVertices.Clone();
			}
			else
			{
				Array.Copy(backupVertices, vertices, backupVertices.Length);
			}
		}
		if (_model._isXDEFNormalEnabled)
		{
			if (normals == null || normals.Length != backupNormals.Length)
			{
				normals = (Vector3[])backupNormals.Clone();
			}
			else
			{
				Array.Copy(backupNormals, normals, backupNormals.Length);
			}
		}
		if (_xdefBoneTransformCache == null || _xdefBoneTransformInvCache == null || _xdefBoneRotationCache == null || boneWeights == null || vertices == null)
		{
			return;
		}
		AuxData.VertexData vertexData = _model._vertexData;
		if (vertexData == null)
		{
			return;
		}
		AuxData.VertexData.MeshBoneInfo r = default(AuxData.VertexData.MeshBoneInfo);
		vertexData.GetMeshBoneInfo(ref r, meshID);
		for (int i = 0; i != r.count; i++)
		{
			AuxData.VertexData.BoneFlags boneFlags = vertexData.GetBoneFlags(ref r, i);
			if ((boneFlags & AuxData.VertexData.BoneFlags.SDEF) != 0)
			{
				int boneID = vertexData.GetBoneID(ref r, i);
				MMDBone bone = _model.GetBone(boneID);
				IndexedMatrix worldTransform = bone.GetWorldTransform();
				ref IndexedMatrix reference = ref _xdefBoneTransformCache[i];
				reference = xdefRootTransformInv * worldTransform;
				if (bone.isLocalAxisEnabled)
				{
					_xdefBoneTransformCache[i]._basis *= bone._localBasis;
				}
				_BulletToUnityTransform(ref _xdefBoneTransformCache[i]);
				if ((boneFlags & AuxData.VertexData.BoneFlags.OptimizeBindPoses) != 0)
				{
					_xdefBoneTransformCache[i]._origin = _xdefBoneTransformCache[i] * bindposes[i]._origin;
				}
				else
				{
					_xdefBoneTransformCache[i] *= bindposes[i];
				}
				ref IndexedMatrix reference2 = ref _xdefBoneTransformInvCache[i];
				reference2 = _xdefBoneTransformCache[i].Inverse();
				ref IndexedQuaternion reference3 = ref _xdefBoneRotationCache[i];
				reference3 = xdefRootRotationInv * worldTransform.GetRotation();
				_xdefBoneRotationCache[i].Y = 0f - _xdefBoneRotationCache[i].Y;
				_xdefBoneRotationCache[i].Z = 0f - _xdefBoneRotationCache[i].Z;
			}
		}
		if (vertices.Length <= 32)
		{
			_PararellProcessXDEF(0, vertices.Length);
			return;
		}
		object xDEFPararellThreadQueue = MMDModel.GetXDEFPararellThreadQueue();
		if (xDEFPararellThreadQueue != null && Global.bridge != null)
		{
			ThreadQueueHandle threadQueueHandle = Global.bridge.InvokeCachedPararellThreadQueue(xDEFPararellThreadQueue, _PararellProcessXDEF, vertices.Length);
			Global.bridge.WaitEndCachedPararellThreadQueue(xDEFPararellThreadQueue, ref threadQueueHandle);
		}
	}

	private void _PararellProcessXDEF(int vertexIndex, int vertexCount)
	{
		AuxData.VertexData vertexData = _model._vertexData;
		bool isXDEFNormalEnabled = _model._isXDEFNormalEnabled;
		AuxData.VertexData.MeshVertexInfo r = default(AuxData.VertexData.MeshVertexInfo);
		vertexData.GetMeshVertexInfo(ref r, meshID);
		int num = _xdefSDEFVertexIndex;
		for (int i = 0; i != vertexIndex; i++)
		{
			AuxData.VertexData.VertexFlags vertexFlags = vertexData.GetVertexFlags(ref r, i);
			if ((vertexFlags & AuxData.VertexData.VertexFlags.SDEF) != 0)
			{
				num++;
			}
		}
		float[] floatValues = vertexData.floatValues;
		int num2 = vertexData.SDEFIndexToSDEFOffset(num);
		int num3 = vertexIndex + vertexCount;
		while (vertexIndex != num3)
		{
			AuxData.VertexData.VertexFlags vertexFlags2 = vertexData.GetVertexFlags(ref r, vertexIndex);
			if ((vertexFlags2 & AuxData.VertexData.VertexFlags.SDEF) != 0)
			{
				int boneIndex = boneWeights[vertexIndex].boneIndex0;
				int boneIndex2 = boneWeights[vertexIndex].boneIndex1;
				float weight = boneWeights[vertexIndex].weight0;
				float weight2 = boneWeights[vertexIndex].weight1;
				IndexedVector3 indexedVector = new IndexedVector3(floatValues[num2], floatValues[num2 + 1], floatValues[num2 + 2]);
				IndexedVector3 indexedVector2 = new IndexedVector3(floatValues[num2 + 3], floatValues[num2 + 4], floatValues[num2 + 5]);
				IndexedVector3 indexedVector3 = new IndexedVector3(floatValues[num2 + 6], floatValues[num2 + 7], floatValues[num2 + 8]);
				num2 += 9;
				IndexedVector3 indexedVector4 = _xdefBoneTransformCache[boneIndex] * indexedVector2 * weight;
				IndexedVector3 indexedVector5 = (_xdefBoneTransformCache[boneIndex] * indexedVector - indexedVector) * weight;
				IndexedQuaternion lhs = _xdefBoneRotationCache[boneIndex];
				IndexedVector3 indexedVector6 = _xdefBoneTransformCache[boneIndex2] * indexedVector3 * weight2;
				IndexedVector3 indexedVector7 = (_xdefBoneTransformCache[boneIndex2] * indexedVector - indexedVector) * weight2;
				IndexedQuaternion rhs = _xdefBoneRotationCache[boneIndex2];
				IndexedVector3 indexedVector8 = (indexedVector4 + indexedVector6 + (indexedVector5 + indexedVector7 + indexedVector)) * 0.5f;
				IndexedQuaternion q = Math.Slerp(ref lhs, ref rhs, weight2);
				IndexedBasisMatrix indexedBasisMatrix = new IndexedBasisMatrix(ref q);
				IndexedVector3 indexedVector9 = vertices[vertexIndex];
				IndexedVector3 indexedVector10 = indexedBasisMatrix * (indexedVector9 - indexedVector) + indexedVector8;
				indexedVector9 = _xdefBoneTransformInvCache[boneIndex] * indexedVector10 * weight;
				indexedVector9 += _xdefBoneTransformInvCache[boneIndex2] * indexedVector10 * weight2;
				ref Vector3 reference = ref vertices[vertexIndex];
				reference = indexedVector9;
				if (isXDEFNormalEnabled)
				{
					IndexedVector3 indexedVector11 = normals[vertexIndex];
					IndexedVector3 indexedVector12 = indexedBasisMatrix * indexedVector11;
					indexedVector11 = _xdefBoneTransformInvCache[boneIndex]._basis * indexedVector12 * weight;
					indexedVector11 += _xdefBoneTransformInvCache[boneIndex2]._basis * indexedVector12 * weight2;
					float num4 = indexedVector11.Length();
					if (Mathf.Abs(num4) > float.Epsilon)
					{
						ref Vector3 reference2 = ref normals[vertexIndex];
						reference2 = indexedVector11 * (1f / num4);
					}
				}
			}
			vertexIndex++;
		}
	}

	private bool _PrepareAndValidationCheck()
	{
		if (meshID < 0)
		{
			return false;
		}
		int meshIndex = meshID;
		if (_model._isVertexMorphEnabled && _model._isXDEFEnabled)
		{
			AuxData.VertexData vertexData = _model._vertexData;
			if (vertexData != null && backupVertices != null && bindposes != null && boneWeights != null && (meshFlags & 4u) != 0)
			{
				int num = backupVertices.Length;
				int num2 = bindposes.Length;
				bool flag = false;
				AuxData.VertexData.MeshBoneInfo r = default(AuxData.VertexData.MeshBoneInfo);
				AuxData.VertexData.MeshVertexInfo r2 = default(AuxData.VertexData.MeshVertexInfo);
				vertexData.GetMeshBoneInfo(ref r, meshIndex);
				vertexData.GetMeshVertexInfo(ref r2, meshIndex);
				if (!vertexData.PrecheckMeshBoneInfo(ref r))
				{
					flag = true;
				}
				if (!vertexData.PrecheckMeshVertexInfo(ref r2))
				{
					flag = true;
				}
				if (r2.count != num)
				{
					flag = true;
				}
				if (r2.count != boneWeights.Length)
				{
					flag = true;
				}
				if (r.count != num2)
				{
					flag = true;
				}
				if (!flag)
				{
					for (int i = 0; i != r2.count; i++)
					{
						AuxData.VertexData.VertexFlags vertexFlags = vertexData.GetVertexFlags(ref r2, i);
						if ((vertexFlags & AuxData.VertexData.VertexFlags.SDEF) != 0)
						{
							int boneIndex = boneWeights[i].boneIndex0;
							int boneIndex2 = boneWeights[i].boneIndex1;
							int boneID = vertexData.GetBoneID(ref r, boneIndex);
							int boneID2 = vertexData.GetBoneID(ref r, boneIndex2);
							if (_model.GetBone(boneID) == null || _model.GetBone(boneID2) == null)
							{
								flag = true;
								break;
							}
							if ((vertexFlags & AuxData.VertexData.VertexFlags.SDEFSwapIndex) != 0)
							{
								boneWeights[i].boneIndex0 = boneIndex2;
								boneWeights[i].boneIndex1 = boneIndex;
								float weight = boneWeights[i].weight0;
								boneWeights[i].weight0 = boneWeights[i].weight1;
								boneWeights[i].weight1 = weight;
							}
						}
					}
				}
				if (!flag)
				{
					for (int j = 0; j != r.count; j++)
					{
						int boneID3 = vertexData.GetBoneID(ref r, j);
						if (_model.GetBone(boneID3) == null)
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					_xdefSDEFVertexIndex = 0;
					_xdefSDEFVertexCount = 0;
					for (int k = 0; k != r2.count; k++)
					{
						AuxData.VertexData.VertexFlags vertexFlags2 = vertexData.GetVertexFlags(ref r2, k);
						if ((vertexFlags2 & AuxData.VertexData.VertexFlags.SDEF) != 0)
						{
							_xdefSDEFVertexCount++;
						}
					}
					for (int l = 0; l != meshID; l++)
					{
						if (_model._meshList[l] != null)
						{
							_xdefSDEFVertexIndex += _model._meshList[l]._xdefSDEFVertexCount;
						}
					}
					if (!vertexData.PrecheckGetSDEFParams(_xdefSDEFVertexIndex, _xdefSDEFVertexCount))
					{
						flag = true;
					}
				}
				if (flag)
				{
					return false;
				}
				if (!flag && _xdefSDEFVertexCount > 3 && !flag && r.count != 0)
				{
					_xdefBoneTransformCache = new IndexedMatrix[r.count];
					_xdefBoneTransformInvCache = new IndexedMatrix[r.count];
					_xdefBoneRotationCache = new IndexedQuaternion[r.count];
				}
			}
		}
		return true;
	}

	private void _BulletToUnityTransform(ref IndexedMatrix transform)
	{
		transform._basis._el0.Y = 0f - transform._basis._el0.Y;
		transform._basis._el0.Z = 0f - transform._basis._el0.Z;
		transform._basis._el1.X = 0f - transform._basis._el1.X;
		transform._basis._el2.X = 0f - transform._basis._el2.X;
		transform._origin.X = 0f - transform._origin.X;
		transform._origin *= _model._bulletToWorldScale;
	}
}
