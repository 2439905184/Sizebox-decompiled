using System;
using UnityEngine;

namespace MMD4MecanimInternal
{
	public static class AuxData
	{
		public class IndexData
		{
			public const int VertexIndexBitMask = 16777215;

			public const int MeshCountBitShift = 24;

			public const int HeaderSize = 2;

			public int[] indexValues;

			public int vertexCount
			{
				get
				{
					if (indexValues != null && 0 < indexValues.Length)
					{
						return indexValues[0] & 0xFFFFFF;
					}
					return 0;
				}
			}

			public int meshCount
			{
				get
				{
					if (indexValues != null && 1 < indexValues.Length)
					{
						return (int)((uint)indexValues[1] >> 24);
					}
					return 0;
				}
			}

			public int meshVertexCount
			{
				get
				{
					if (indexValues != null && 1 < indexValues.Length)
					{
						return indexValues[1] & 0xFFFFFF;
					}
					return 0;
				}
			}
		}

		public class VertexData
		{
			public struct MeshBoneInfo
			{
				public bool isSDEF;

				public bool isQDEF;

				public bool isXDEF;

				public int index;

				public int count;
			}

			public struct MeshVertexInfo
			{
				public int index;

				public int count;
			}

			[Flags]
			public enum MeshFlags
			{
				None = 0,
				SDEF = int.MinValue,
				QDEF = 0x40000000,
				XDEF = -1073741824
			}

			[Flags]
			public enum BoneFlags
			{
				None = 0,
				SDEF = 1,
				QDEF = 2,
				XDEF = 3,
				OptimizeBindPoses = 4
			}

			[Flags]
			public enum VertexFlags
			{
				None = 0,
				SDEF = 1,
				QDEF = 2,
				XDEF = 3,
				SDEFSwapIndex = 0x80
			}

			public const int VertexIndexBitMask = 16777215;

			public const int MeshCountBitShift = 24;

			public const int MeshBoneOffsetBitMask = 16777215;

			public const int MeshVertexOffsetBitMask = 16777215;

			public const int HeaderSize = 5;

			public const int FloatHeaderSize = 2;

			public int[] intValues;

			public float[] floatValues;

			public byte[] byteValues;

			private int _meshCount;

			private int _meshBoneIDOffset;

			public int vertexCount
			{
				get
				{
					if (intValues != null && 0 < intValues.Length)
					{
						return intValues[0] & 0xFFFFFF;
					}
					return 0;
				}
			}

			public int meshCount
			{
				get
				{
					return _meshCount;
				}
			}

			public int meshVertexCount
			{
				get
				{
					if (intValues != null && 1 < intValues.Length)
					{
						return intValues[1] & 0xFFFFFF;
					}
					return 0;
				}
			}

			public float vertexScale
			{
				get
				{
					if (floatValues != null && 0 < floatValues.Length)
					{
						return floatValues[0];
					}
					return 0f;
				}
			}

			public float importScale
			{
				get
				{
					if (floatValues != null && 1 < floatValues.Length)
					{
						return floatValues[1];
					}
					return 0f;
				}
			}

			public static int GetIntValueCount(byte[] bytes)
			{
				return 5 + ReadInt(bytes, 2);
			}

			public static int GetFloatValueCount(byte[] bytes)
			{
				return ReadInt(bytes, 3);
			}

			public static int GetByteValueCount(byte[] bytes)
			{
				return ReadInt(bytes, 4);
			}

			public void PostfixBuild()
			{
				if (intValues != null && 1 < intValues.Length)
				{
					_meshCount = (int)((uint)intValues[1] >> 24);
				}
				_meshBoneIDOffset = 5 + (_meshCount + 1 << 1);
			}

			public void GetMeshBoneInfo(ref MeshBoneInfo r, int meshIndex)
			{
				int num = meshCount;
				if ((uint)meshIndex < (uint)num)
				{
					int num2 = 5 + meshIndex;
					if (intValues != null && num2 + 1 < intValues.Length)
					{
						r.isXDEF = (intValues[num2] & -1073741824) != 0;
						r.isSDEF = (intValues[num2] & int.MinValue) != 0;
						r.isQDEF = (intValues[num2] & 0x40000000) != 0;
						r.index = intValues[num2] & 0xFFFFFF;
						r.count = (intValues[num2 + 1] & 0xFFFFFF) - r.index;
					}
				}
			}

			public void GetMeshVertexInfo(ref MeshVertexInfo r, int meshIndex)
			{
				int num = meshCount;
				if ((uint)meshIndex < (uint)num)
				{
					int num2 = 5 + num + 1 + meshIndex;
					if (intValues != null && num2 + 1 < intValues.Length)
					{
						r.index = intValues[num2] & 0xFFFFFF;
						r.count = (intValues[num2 + 1] & 0xFFFFFF) - r.index;
					}
				}
			}

			public bool PrecheckMeshBoneInfo(ref MeshBoneInfo meshBoneInfo)
			{
				int meshBoneIDOffset = _meshBoneIDOffset;
				if (byteValues != null && (uint)(meshBoneInfo.index + meshBoneInfo.count) <= byteValues.Length && intValues != null)
				{
					return (uint)(meshBoneIDOffset + meshBoneInfo.index + meshBoneInfo.count) <= intValues.Length;
				}
				return false;
			}

			public BoneFlags GetBoneFlags(ref MeshBoneInfo meshBoneInfo, int boneIndex)
			{
				return (BoneFlags)byteValues[meshBoneInfo.index + boneIndex];
			}

			public int GetBoneID(ref MeshBoneInfo meshBoneInfo, int boneIndex)
			{
				int meshBoneIDOffset = _meshBoneIDOffset;
				return intValues[meshBoneIDOffset + meshBoneInfo.index + boneIndex];
			}

			public bool PrecheckMeshVertexInfo(ref MeshVertexInfo meshVertexInfo)
			{
				if (byteValues != null)
				{
					return (uint)(meshVertexInfo.index + meshVertexInfo.count) <= byteValues.Length;
				}
				return false;
			}

			public VertexFlags GetVertexFlags(ref MeshVertexInfo meshVertexInfo, int vertexIndex)
			{
				return (VertexFlags)byteValues[meshVertexInfo.index + vertexIndex];
			}

			public bool PrecheckGetSDEFParams(int sdefIndex, int sdefCount)
			{
				uint num = (uint)(2 + (sdefIndex + sdefCount) * 9);
				return num <= (uint)floatValues.Length;
			}

			public int SDEFIndexToSDEFOffset(int sdefIndex)
			{
				return 2 + sdefIndex * 9;
			}
		}

		public static int ReadInt(byte[] bytes, int index)
		{
			if (bytes != null && index * 4 + 3 < bytes.Length)
			{
				return bytes[index * 4] | (bytes[index * 4 + 1] << 8) | (bytes[index * 4 + 2] << 16) | (bytes[index * 4 + 3] << 24);
			}
			return 0;
		}

		public static IndexData BuildIndexData(TextAsset indexFile)
		{
			if (indexFile == null)
			{
				Debug.LogError("BuildIndexData: indexFile is norhing.");
				return null;
			}
			return BuildIndexData(indexFile.bytes);
		}

		public static IndexData BuildIndexData(byte[] indexBytes)
		{
			if (indexBytes == null || indexBytes.Length == 0)
			{
				return null;
			}
			int num = indexBytes.Length / 4;
			if (num < 2)
			{
				return null;
			}
			int[] array = new int[num];
			Buffer.BlockCopy(indexBytes, 0, array, 0, num * 4);
			IndexData indexData = new IndexData();
			indexData.indexValues = array;
			return indexData;
		}

		public static bool ValidateIndexData(IndexData indexData, SkinnedMeshRenderer[] skinnedMeshRenderers)
		{
			if (indexData == null || skinnedMeshRenderers == null)
			{
				return false;
			}
			if (indexData.meshCount != skinnedMeshRenderers.Length)
			{
				Debug.LogError("ValidateIndexData: FBX reimported. Disabled morph, please recreate index file.");
				return false;
			}
			int num = 0;
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshRenderers)
			{
				if (skinnedMeshRenderer.sharedMesh != null)
				{
					num += skinnedMeshRenderer.sharedMesh.vertexCount;
				}
			}
			if (indexData.meshVertexCount != num)
			{
				Debug.LogError("ValidateIndexData: FBX reimported. Disabled morph, please recreate index file.");
				return false;
			}
			return true;
		}

		public static VertexData BuildVertexData(TextAsset vertexFile)
		{
			if (vertexFile == null)
			{
				Debug.LogError("BuildVertexData: xdefFile is norhing.");
				return null;
			}
			return BuildVertexData(vertexFile.bytes);
		}

		public static VertexData BuildVertexData(byte[] vertexBytes)
		{
			if (vertexBytes == null)
			{
				return null;
			}
			if (vertexBytes.Length == 0)
			{
				return null;
			}
			int intValueCount = VertexData.GetIntValueCount(vertexBytes);
			int floatValueCount = VertexData.GetFloatValueCount(vertexBytes);
			int byteValueCount = VertexData.GetByteValueCount(vertexBytes);
			if (intValueCount <= 0 || floatValueCount < 0 || byteValueCount < 0)
			{
				return null;
			}
			if (vertexBytes.Length != intValueCount * 4 + floatValueCount * 4 + byteValueCount)
			{
				return null;
			}
			VertexData vertexData = new VertexData();
			int num = 0;
			vertexData.intValues = new int[intValueCount];
			Buffer.BlockCopy(vertexBytes, num, vertexData.intValues, 0, intValueCount * 4);
			num += intValueCount * 4;
			if (floatValueCount > 0)
			{
				vertexData.floatValues = new float[floatValueCount];
				Buffer.BlockCopy(vertexBytes, num, vertexData.floatValues, 0, floatValueCount * 4);
				num += floatValueCount * 4;
			}
			if (byteValueCount > 0)
			{
				vertexData.byteValues = new byte[byteValueCount];
				Buffer.BlockCopy(vertexBytes, num, vertexData.byteValues, 0, byteValueCount);
			}
			vertexData.PostfixBuild();
			return vertexData;
		}

		public static bool ValidateVertexData(VertexData vertexData, SkinnedMeshRenderer[] skinnedMeshRenderers)
		{
			if (vertexData == null || skinnedMeshRenderers == null)
			{
				return false;
			}
			if (vertexData.meshCount != skinnedMeshRenderers.Length)
			{
				return false;
			}
			int num = 0;
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshRenderers)
			{
				if (skinnedMeshRenderer.sharedMesh != null)
				{
					num += skinnedMeshRenderer.sharedMesh.vertexCount;
				}
			}
			if (vertexData.meshVertexCount != num)
			{
				return false;
			}
			return true;
		}
	}
}
