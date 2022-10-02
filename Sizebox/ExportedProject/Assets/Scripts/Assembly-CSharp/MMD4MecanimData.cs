using System;
using System.Collections.Generic;
using UnityEngine;

public static class MMD4MecanimData
{
	public enum FileType
	{
		None = 0,
		PMD = 1,
		PMX = 2
	}

	public enum MorphCategory
	{
		Base = 0,
		EyeBrow = 1,
		Eye = 2,
		Lip = 3,
		Other = 4,
		Max = 5
	}

	public enum ShapeType
	{
		Sphere = 0,
		Box = 1,
		Capsule = 2
	}

	public enum RigidBodyType
	{
		Kinematics = 0,
		Simulated = 1,
		SimulatedAligned = 2
	}

	public enum BoneTransform
	{
		Unknown = -1,
		BDEF1 = 0,
		BDEF2 = 1,
		BDEF4 = 2,
		SDEF = 3,
		QDEF = 4
	}

	public enum MorphType
	{
		Group = 0,
		Vertex = 1,
		Bone = 2,
		UV = 3,
		UVA1 = 4,
		UVA2 = 5,
		UVA3 = 6,
		UVA4 = 7,
		Material = 8
	}

	public enum FullBodyIKGroup
	{
		Body = 0,
		LeftArm = 1,
		RightArm = 2,
		LeftLeg = 3,
		RightLeg = 4,
		LeftWristFinger = 5,
		RightWristFinger = 6,
		Look = 7,
		Max = 8,
		Unknown = -1
	}

	public enum FullBodyIKBodyTarget
	{
		Torso = 0,
		Neck = 1,
		Max = 2
	}

	public enum FullBodyIKArmTarget
	{
		Arm = 0,
		Elbow = 1,
		Wrist = 2,
		WristUpward = 3,
		WristForward = 4,
		Max = 5
	}

	public enum FullBodyIKLegTarget
	{
		Hip = 0,
		Knee = 1,
		Foot = 2,
		FootUpward = 3,
		FootForward = 4,
		Max = 5
	}

	public enum FullBodyIKFingerTarget
	{
		Thumb = 0,
		Index = 1,
		Middle = 2,
		Ring = 3,
		Little = 4,
		Max = 5
	}

	public enum FullBodyIKLookTarget
	{
		Head = 0,
		HeadUpward = 1,
		HeadForward = 2,
		Eyes = 3,
		Max = 4
	}

	public enum MorphAutoLumninousType
	{
		None = 0,
		LightUp = 1,
		LightOff = 2,
		LightBlink = 3,
		LightBS = 4
	}

	public enum PMDBoneType
	{
		Rotate = 0,
		RotateAndMove = 1,
		IKDestination = 2,
		Unknown = 3,
		UnderIK = 4,
		UnderRotate = 5,
		IKTarget = 6,
		NoDisp = 7,
		Twist = 8,
		FollowRotate = 9
	}

	[Flags]
	public enum BoneAdditionalFlags
	{
		None = 0,
		IsKnee = 1,
		BoneTypeMask = -16777216,
		BoneTypeRoot = int.MinValue
	}

	[Flags]
	public enum RigidBodyAdditionalFlags
	{
		None = 0,
		IsFreezed = 1
	}

	[Flags]
	public enum PMXBoneFlags
	{
		None = 0,
		Destination = 1,
		Rotate = 2,
		Translate = 4,
		Visible = 8,
		Controllable = 0x10,
		IK = 0x20,
		IKChild = 0x40,
		InherenceLocal = 0x80,
		InherenceRotate = 0x100,
		InherenceTranslate = 0x200,
		FixedAxis = 0x400,
		LocalAxis = 0x800,
		TransformAfterPhysics = 0x1000,
		TransformExternalParent = 0x2000
	}

	[Flags]
	public enum ModelAdditionalFlags
	{
		None = 0,
		MorphTranslateName = 1
	}

	[Flags]
	public enum IKAdditionalFlags
	{

	}

	[Flags]
	public enum IKLinkFlags
	{
		None = 0,
		HasAngleJoint = 1
	}

	[Flags]
	public enum PMXBoneFlag
	{
		None = 0,
		Destination = 1,
		Rotate = 2,
		Translate = 4,
		Visible = 8,
		Controllable = 0x10,
		IK = 0x20,
		IKChild = 0x40,
		InherenceLocal = 0x80,
		InherenceRotate = 0x100,
		InherenceTranslate = 0x200,
		FixedAxis = 0x400,
		LocalAxis = 0x800,
		TransformAfterPhysics = 0x1000,
		TransformExternalParent = 0x2000
	}

	[Flags]
	public enum MeshFlags
	{
		None = 0,
		BlendShapes = 1,
		VertexMorph = 2,
		XDEF = 4
	}

	[Serializable]
	public class BoneData
	{
		public BoneAdditionalFlags boneAdditionalFlags;

		public string nameJp;

		public string skeletonName;

		public int parentBoneID;

		public int sortedBoneID;

		public int originalParentBoneID;

		public int originalSortedBoneID;

		public Vector3 baseOrigin;

		public PMDBoneType pmdBoneType;

		public int childBoneID;

		public int targetBoneID;

		public float followCoef;

		public int transformLayerID;

		public PMXBoneFlags pmxBoneFlags;

		public int inherenceParentBoneID;

		public float inherenceWeight;

		public int externalID;

		public bool isKnee
		{
			get
			{
				return (boneAdditionalFlags & BoneAdditionalFlags.IsKnee) != 0;
			}
		}

		public bool isRootBone
		{
			get
			{
				return (boneAdditionalFlags & BoneAdditionalFlags.BoneTypeMask) == BoneAdditionalFlags.BoneTypeRoot;
			}
		}
	}

	[Serializable]
	public class IKLinkData
	{
		public int ikLinkBoneID;

		public IKLinkFlags ikLinkFlags;

		public Vector3 lowerLimit;

		public Vector3 upperLimit;

		public Vector3 lowerLimitAsDegree;

		public Vector3 upperLimitAsDegree;

		public bool hasAngleJoint
		{
			get
			{
				return (ikLinkFlags & IKLinkFlags.HasAngleJoint) != 0;
			}
		}
	}

	[Serializable]
	public class IKData
	{
		public IKAdditionalFlags ikAdditionalFlags;

		public int destBoneID;

		public int targetBoneID;

		public int iteration;

		public float angleConstraint;

		public IKLinkData[] ikLinkDataList;
	}

	[Serializable]
	public class RigidBodyData
	{
		public RigidBodyAdditionalFlags rigidBodyAdditionalFlags;

		public string nameJp;

		public string nameEn;

		public int boneID;

		public int collisionGroupID;

		public int collisionMask;

		public ShapeType shapeType;

		public RigidBodyType rigidBodyType;

		public Vector3 shapeSize;

		public Vector3 position;

		public Vector3 rotation;

		public float mass;

		public float linearDamping;

		public float angularDamping;

		public float restitution;

		public float friction;

		public bool isFreezed
		{
			get
			{
				return (rigidBodyAdditionalFlags & RigidBodyAdditionalFlags.IsFreezed) != 0;
			}
		}
	}

	[Serializable]
	public class JointData
	{
		public int rigidBodyIDA;

		public int rigidBodyIDB;
	}

	public enum MorphMaterialOperation
	{
		Multiply = 0,
		Adding = 1
	}

	public struct MorphMaterialData
	{
		public int materialID;

		public MorphMaterialOperation operation;

		public Color diffuse;

		public Color specular;

		public float shininess;

		public Color ambient;

		public Color edgeColor;

		public float edgeScale;

		public float edgeSize;

		public Color textureColor;

		public Color sphereColor;

		public Color toonTextureColor;

		public float alPower;
	}

	[Serializable]
	public class MorphData
	{
		public string nameJp;

		public string nameEn;

		public string translatedName;

		public MorphCategory morphCategory;

		public MorphType morphType;

		[NonSerialized]
		public bool isMorphBaseVertex;

		[NonSerialized]
		public int[] indices;

		[NonSerialized]
		public float[] weights;

		[NonSerialized]
		public Vector3[] positions;

		[NonSerialized]
		public MorphMaterialData[] materialData;
	}

	public class ModelData
	{
		public FileType fileType;

		public ModelAdditionalFlags modelAdditionalFlags;

		public int vertexCount;

		public float vertexScale;

		public float importScale;

		public BoneData[] boneDataList;

		public Dictionary<string, int> boneDataDictionary;

		public IKData[] ikDataList;

		public MorphData[] morphDataList;

		public Dictionary<string, int> morphDataDictionaryJp;

		public Dictionary<string, int> morphDataDictionaryEn;

		public Dictionary<string, int> translatedMorphDataDictionary;

		public RigidBodyData[] rigidBodyDataList;

		public JointData[] jointDataList;

		public Dictionary<string, int> morphDataDictionary
		{
			get
			{
				return morphDataDictionaryJp;
			}
		}

		public bool isMorphTranslateName
		{
			get
			{
				return (modelAdditionalFlags & ModelAdditionalFlags.MorphTranslateName) != 0;
			}
		}

		public int GetMorphDataIndex(string morphName, bool isStartsWith)
		{
			return GetMorphDataIndexJp(morphName, isStartsWith);
		}

		public int GetMorphDataIndexJp(string morphName, bool isStartsWith)
		{
			if (morphName != null && morphDataList != null && morphDataDictionaryJp != null && morphDataDictionaryJp.Count > 0)
			{
				int value = 0;
				if (morphDataDictionaryJp.TryGetValue(morphName, out value))
				{
					return value;
				}
				if (isStartsWith)
				{
					for (int i = 0; i < morphDataList.Length; i++)
					{
						if (morphDataList[i].nameJp != null && morphDataList[i].nameJp.StartsWith(morphName))
						{
							return i;
						}
					}
				}
			}
			return -1;
		}

		public int GetMorphDataIndexEn(string morphName, bool isStartsWith)
		{
			if (morphName != null && morphDataList != null && morphDataDictionaryEn != null && morphDataDictionaryEn.Count > 0)
			{
				int value = 0;
				if (morphDataDictionaryEn.TryGetValue(morphName, out value))
				{
					return value;
				}
				if (isStartsWith)
				{
					for (int i = 0; i < morphDataList.Length; i++)
					{
						if (morphDataList[i].nameEn != null && morphDataList[i].nameEn.StartsWith(morphName))
						{
							return i;
						}
					}
				}
			}
			return -1;
		}

		public int GetTranslatedMorphDataIndex(string morphName, bool isStartsWith)
		{
			if (morphName != null && morphDataList != null && translatedMorphDataDictionary != null && translatedMorphDataDictionary.Count > 0)
			{
				int value = 0;
				if (translatedMorphDataDictionary.TryGetValue(morphName, out value))
				{
					return value;
				}
				if (isStartsWith)
				{
					for (int i = 0; i < morphDataList.Length; i++)
					{
						if (morphDataList[i].translatedName != null && morphDataList[i].translatedName.StartsWith(morphName))
						{
							return i;
						}
					}
				}
			}
			return -1;
		}

		public MorphData GetMorphData(string morphName, bool isStartsWith)
		{
			int morphDataIndex = GetMorphDataIndex(morphName, isStartsWith);
			if (morphDataIndex != -1)
			{
				return morphDataList[morphDataIndex];
			}
			return null;
		}

		public MorphData GetMorphData(string morphName)
		{
			return GetMorphData(morphName, false);
		}
	}

	[Flags]
	public enum VertexFlags
	{
		None = 0,
		NoEdge = 1
	}

	public struct VertexData
	{
		public VertexFlags flags;

		public Vector3 position;

		public Vector3 normal;

		public Vector2 uv;

		public float edgeScale;

		public BoneTransform boneTransform;

		public BoneWeight boneWeight;

		public Vector3 sdefC;

		public Vector3 sdefR0;

		public Vector3 sdefR1;
	}

	public class ExtraData
	{
		public int vertexCount;

		public float vertexScale;

		public float importScale;

		public VertexData[] vertexDataList;
	}

	[Flags]
	public enum MorphMotionAdditionalFlags
	{
		None = 0,
		NameIsFull = 1
	}

	public class MorphMotionData
	{
		public string name;

		public MorphMotionAdditionalFlags morphMotionAdditionalFlags;

		public int[] frameNos;

		public float[] f_frameNos;

		public float[] weights;

		public bool nameIsFull
		{
			get
			{
				return (morphMotionAdditionalFlags & MorphMotionAdditionalFlags.NameIsFull) != 0;
			}
		}
	}

	[Flags]
	public enum AnimAdditionalFlags
	{
		None = 0,
		SupportNameIsFull = 1
	}

	public class AnimData
	{
		public AnimAdditionalFlags animAdditionalFlags;

		public int maxFrame;

		public MorphMotionData[] morphMotionDataList;

		public bool supportNameIsFull
		{
			get
			{
				return (animAdditionalFlags & AnimAdditionalFlags.SupportNameIsFull) != 0;
			}
		}
	}

	public const int FullBodyIKTransformElements = 4;

	public const int FullBodyIKTargetMax = 36;

	private static bool _Decl(ref int index, int max)
	{
		if (index < max)
		{
			return true;
		}
		index -= max;
		return false;
	}

	public static FullBodyIKGroup GetFullBodyIKGroup(int fullBodyIKTargetIndex)
	{
		return GetFullBodyIKGroup(ref fullBodyIKTargetIndex);
	}

	public static FullBodyIKGroup GetFullBodyIKGroup(ref int fullBodyIKTargetIndex)
	{
		if (fullBodyIKTargetIndex >= 0 && fullBodyIKTargetIndex < 36)
		{
			if (_Decl(ref fullBodyIKTargetIndex, 2))
			{
				return FullBodyIKGroup.Body;
			}
			if (_Decl(ref fullBodyIKTargetIndex, 5))
			{
				return FullBodyIKGroup.LeftArm;
			}
			if (_Decl(ref fullBodyIKTargetIndex, 5))
			{
				return FullBodyIKGroup.RightArm;
			}
			if (_Decl(ref fullBodyIKTargetIndex, 5))
			{
				return FullBodyIKGroup.LeftLeg;
			}
			if (_Decl(ref fullBodyIKTargetIndex, 5))
			{
				return FullBodyIKGroup.RightLeg;
			}
			if (_Decl(ref fullBodyIKTargetIndex, 5))
			{
				return FullBodyIKGroup.LeftWristFinger;
			}
			if (_Decl(ref fullBodyIKTargetIndex, 5))
			{
				return FullBodyIKGroup.RightWristFinger;
			}
			if (_Decl(ref fullBodyIKTargetIndex, 4))
			{
				return FullBodyIKGroup.Look;
			}
		}
		return FullBodyIKGroup.Unknown;
	}

	public static string GetFullBodyIKTargetName(int fullBodyIKTargetIndex)
	{
		FullBodyIKGroup fullBodyIKGroup;
		return GetFullBodyIKTargetName(out fullBodyIKGroup, fullBodyIKTargetIndex);
	}

	public static string GetFullBodyIKTargetName(out FullBodyIKGroup fullBodyIKGroup, int fullBodyIKTargetIndex)
	{
		fullBodyIKGroup = FullBodyIKGroup.Unknown;
		if (fullBodyIKTargetIndex >= 0 && fullBodyIKTargetIndex < 36)
		{
			fullBodyIKGroup = GetFullBodyIKGroup(ref fullBodyIKTargetIndex);
			switch (fullBodyIKGroup)
			{
			case FullBodyIKGroup.Body:
			{
				FullBodyIKBodyTarget fullBodyIKBodyTarget = (FullBodyIKBodyTarget)fullBodyIKTargetIndex;
				return fullBodyIKBodyTarget.ToString();
			}
			case FullBodyIKGroup.LeftArm:
			{
				FullBodyIKArmTarget fullBodyIKArmTarget = (FullBodyIKArmTarget)fullBodyIKTargetIndex;
				return fullBodyIKArmTarget.ToString();
			}
			case FullBodyIKGroup.RightArm:
			{
				FullBodyIKArmTarget fullBodyIKArmTarget = (FullBodyIKArmTarget)fullBodyIKTargetIndex;
				return fullBodyIKArmTarget.ToString();
			}
			case FullBodyIKGroup.LeftLeg:
			{
				FullBodyIKLegTarget fullBodyIKLegTarget = (FullBodyIKLegTarget)fullBodyIKTargetIndex;
				return fullBodyIKLegTarget.ToString();
			}
			case FullBodyIKGroup.RightLeg:
			{
				FullBodyIKLegTarget fullBodyIKLegTarget = (FullBodyIKLegTarget)fullBodyIKTargetIndex;
				return fullBodyIKLegTarget.ToString();
			}
			case FullBodyIKGroup.LeftWristFinger:
			{
				FullBodyIKFingerTarget fullBodyIKFingerTarget = (FullBodyIKFingerTarget)fullBodyIKTargetIndex;
				return fullBodyIKFingerTarget.ToString();
			}
			case FullBodyIKGroup.RightWristFinger:
			{
				FullBodyIKFingerTarget fullBodyIKFingerTarget = (FullBodyIKFingerTarget)fullBodyIKTargetIndex;
				return fullBodyIKFingerTarget.ToString();
			}
			case FullBodyIKGroup.Look:
			{
				FullBodyIKLookTarget fullBodyIKLookTarget = (FullBodyIKLookTarget)fullBodyIKTargetIndex;
				return fullBodyIKLookTarget.ToString();
			}
			}
		}
		return "";
	}

	public static ModelData BuildModelData(TextAsset modelFile)
	{
		if (modelFile == null)
		{
			Debug.LogError("BuildModelData: modelFile is norhing.");
			return null;
		}
		byte[] bytes = modelFile.bytes;
		if (bytes == null || bytes.Length == 0)
		{
			Debug.LogError("BuildModelData: Nothing modelBytes.");
			return null;
		}
		MMD4MecanimCommon.BinaryReader binaryReader = new MMD4MecanimCommon.BinaryReader(bytes);
		if (!binaryReader.Preparse())
		{
			Debug.LogError("BuildModelData:modelFile is unsupported fomart.");
			return null;
		}
		ModelData modelData = new ModelData();
		binaryReader.BeginHeader();
		modelData.fileType = (FileType)binaryReader.ReadHeaderInt();
		binaryReader.ReadHeaderFloat();
		binaryReader.ReadHeaderInt();
		modelData.modelAdditionalFlags = (ModelAdditionalFlags)binaryReader.ReadHeaderInt();
		modelData.vertexCount = binaryReader.ReadHeaderInt();
		binaryReader.ReadHeaderInt();
		modelData.vertexScale = binaryReader.ReadHeaderFloat();
		modelData.importScale = binaryReader.ReadHeaderFloat();
		binaryReader.EndHeader();
		int structListLength = binaryReader.structListLength;
		for (int i = 0; i < structListLength; i++)
		{
			if (!binaryReader.BeginStructList())
			{
				Debug.LogError("BuildModelData: Parse error.");
				return null;
			}
			int currentStructFourCC = binaryReader.currentStructFourCC;
			if (currentStructFourCC == MMD4MecanimCommon.BinaryReader.MakeFourCC("BONE"))
			{
				if (!_ParseBoneData(modelData, binaryReader))
				{
					Debug.LogError("BuildModelData: Parse error.");
					return null;
				}
			}
			else if (currentStructFourCC == MMD4MecanimCommon.BinaryReader.MakeFourCC("IK__"))
			{
				if (!_ParseIKData(modelData, binaryReader))
				{
					Debug.LogError("BuildModelData: Parse error.");
					return null;
				}
			}
			else if (currentStructFourCC == MMD4MecanimCommon.BinaryReader.MakeFourCC("MRPH"))
			{
				if (!_ParseMorphData(modelData, binaryReader))
				{
					Debug.LogError("BuildModelData: Parse error.");
					return null;
				}
			}
			else if (currentStructFourCC == MMD4MecanimCommon.BinaryReader.MakeFourCC("RGBD"))
			{
				if (!_ParseRigidBodyData(modelData, binaryReader))
				{
					Debug.LogError("BuildModelData: Parse error.");
					return null;
				}
			}
			else if (currentStructFourCC == MMD4MecanimCommon.BinaryReader.MakeFourCC("JOIN") && !_ParseJointData(modelData, binaryReader))
			{
				Debug.LogError("BuildModelData: Parse error.");
				return null;
			}
			if (!binaryReader.EndStructList())
			{
				Debug.LogError("BuildModelData: Parse error.");
				return null;
			}
		}
		return modelData;
	}

	private static bool _ParseBoneData(ModelData modelData, MMD4MecanimCommon.BinaryReader binaryReader)
	{
		modelData.boneDataDictionary = new Dictionary<string, int>();
		modelData.boneDataList = new BoneData[binaryReader.currentStructLength];
		for (int i = 0; i < binaryReader.currentStructLength; i++)
		{
			if (!binaryReader.BeginStruct())
			{
				return false;
			}
			BoneData boneData = new BoneData();
			boneData.boneAdditionalFlags = (BoneAdditionalFlags)binaryReader.ReadStructInt();
			boneData.nameJp = binaryReader.GetName(binaryReader.ReadStructInt());
			binaryReader.ReadStructInt();
			boneData.skeletonName = binaryReader.GetName(binaryReader.ReadStructInt());
			boneData.parentBoneID = binaryReader.ReadStructInt();
			boneData.sortedBoneID = binaryReader.ReadStructInt();
			binaryReader.ReadStructInt();
			boneData.originalParentBoneID = binaryReader.ReadStructInt();
			boneData.originalSortedBoneID = binaryReader.ReadStructInt();
			boneData.baseOrigin = binaryReader.ReadStructVector3();
			boneData.baseOrigin.x = 0f - boneData.baseOrigin.x;
			boneData.baseOrigin.z = 0f - boneData.baseOrigin.z;
			if (modelData.fileType == FileType.PMD)
			{
				boneData.pmdBoneType = (PMDBoneType)binaryReader.ReadStructInt();
				boneData.childBoneID = binaryReader.ReadStructInt();
				boneData.targetBoneID = binaryReader.ReadStructInt();
				boneData.followCoef = binaryReader.ReadStructFloat();
			}
			else if (modelData.fileType == FileType.PMX)
			{
				boneData.transformLayerID = binaryReader.ReadStructInt();
				boneData.pmxBoneFlags = (PMXBoneFlags)binaryReader.ReadStructInt();
				boneData.inherenceParentBoneID = binaryReader.ReadStructInt();
				boneData.inherenceWeight = binaryReader.ReadStructFloat();
				boneData.externalID = binaryReader.ReadStructInt();
			}
			if (!binaryReader.EndStruct())
			{
				return false;
			}
			modelData.boneDataList[i] = boneData;
			if (!string.IsNullOrEmpty(boneData.skeletonName))
			{
				modelData.boneDataDictionary[boneData.skeletonName] = i;
			}
		}
		return true;
	}

	private static Vector3 _ToDegree(Vector3 radian)
	{
		return new Vector3(radian.x * 57.29578f, radian.y * 57.29578f, radian.z * 57.29578f);
	}

	private static bool _ParseIKData(ModelData modelData, MMD4MecanimCommon.BinaryReader binaryReader)
	{
		modelData.ikDataList = new IKData[binaryReader.currentStructLength];
		for (int i = 0; i < binaryReader.currentStructLength; i++)
		{
			if (!binaryReader.BeginStruct())
			{
				return false;
			}
			IKData iKData = new IKData();
			iKData.ikAdditionalFlags = (IKAdditionalFlags)binaryReader.ReadStructInt();
			iKData.destBoneID = binaryReader.ReadStructInt();
			iKData.targetBoneID = binaryReader.ReadStructInt();
			iKData.iteration = binaryReader.ReadStructInt();
			iKData.angleConstraint = binaryReader.ReadStructFloat();
			int num = binaryReader.ReadStructInt();
			iKData.ikLinkDataList = new IKLinkData[num];
			for (int j = 0; j < num; j++)
			{
				IKLinkData iKLinkData = new IKLinkData();
				iKLinkData.ikLinkBoneID = binaryReader.ReadInt();
				iKLinkData.ikLinkFlags = (IKLinkFlags)binaryReader.ReadInt();
				if ((iKLinkData.ikLinkFlags & IKLinkFlags.HasAngleJoint) != 0)
				{
					Vector3 lowerLimit = binaryReader.ReadVector3();
					Vector3 upperLimit = binaryReader.ReadVector3();
					iKLinkData.lowerLimit = lowerLimit;
					iKLinkData.upperLimit = upperLimit;
					iKLinkData.lowerLimit = new Vector3(0f - upperLimit[0], lowerLimit[1], 0f - upperLimit[2]);
					iKLinkData.upperLimit = new Vector3(0f - lowerLimit[0], upperLimit[1], 0f - lowerLimit[2]);
					iKLinkData.lowerLimitAsDegree = _ToDegree(iKLinkData.lowerLimit);
					iKLinkData.upperLimitAsDegree = _ToDegree(iKLinkData.upperLimit);
				}
				iKData.ikLinkDataList[j] = iKLinkData;
			}
			if (!binaryReader.EndStruct())
			{
				return false;
			}
			modelData.ikDataList[i] = iKData;
		}
		return true;
	}

	private static bool _ParseMorphData(ModelData modelData, MMD4MecanimCommon.BinaryReader binaryReader)
	{
		modelData.morphDataDictionaryJp = new Dictionary<string, int>();
		modelData.morphDataDictionaryEn = new Dictionary<string, int>();
		if (modelData.isMorphTranslateName)
		{
			modelData.translatedMorphDataDictionary = new Dictionary<string, int>();
		}
		modelData.morphDataList = new MorphData[binaryReader.currentStructLength];
		for (int i = 0; i < binaryReader.currentStructLength; i++)
		{
			if (!binaryReader.BeginStruct())
			{
				return false;
			}
			MorphData morphData = new MorphData();
			int num = binaryReader.ReadStructInt();
			int index = binaryReader.ReadStructInt();
			int index2 = binaryReader.ReadStructInt();
			int morphCategory = binaryReader.ReadStructInt();
			int num2 = binaryReader.ReadStructInt();
			int num3 = binaryReader.ReadStructInt();
			int index3 = binaryReader.ReadStructInt();
			morphData.nameJp = binaryReader.GetName(index);
			morphData.nameEn = binaryReader.GetName(index2);
			if (modelData.isMorphTranslateName)
			{
				morphData.translatedName = binaryReader.GetName(index3);
			}
			morphData.morphCategory = (MorphCategory)morphCategory;
			morphData.morphType = (MorphType)num2;
			if (((uint)num & (true ? 1u : 0u)) != 0)
			{
				morphData.isMorphBaseVertex = true;
			}
			switch (num2)
			{
			case 0:
			{
				morphData.indices = new int[num3];
				morphData.weights = new float[num3];
				for (int k = 0; k < num3; k++)
				{
					morphData.indices[k] = binaryReader.ReadInt();
					morphData.weights[k] = binaryReader.ReadFloat();
				}
				break;
			}
			case 8:
			{
				morphData.materialData = new MorphMaterialData[num3];
				for (int j = 0; j < num3; j++)
				{
					MorphMaterialData morphMaterialData = default(MorphMaterialData);
					morphMaterialData.materialID = binaryReader.ReadInt();
					morphMaterialData.operation = (MorphMaterialOperation)binaryReader.ReadInt();
					morphMaterialData.diffuse = binaryReader.ReadColor();
					morphMaterialData.specular = binaryReader.ReadColorRGB();
					morphMaterialData.shininess = binaryReader.ReadFloat();
					morphMaterialData.ambient = binaryReader.ReadColorRGB();
					morphMaterialData.edgeColor = binaryReader.ReadColor();
					morphMaterialData.edgeSize = binaryReader.ReadFloat();
					morphMaterialData.textureColor = binaryReader.ReadColor();
					morphMaterialData.sphereColor = binaryReader.ReadColor();
					morphMaterialData.toonTextureColor = binaryReader.ReadColor();
					if (morphMaterialData.operation == MorphMaterialOperation.Adding)
					{
						morphMaterialData.specular.a = 0f;
						morphMaterialData.ambient.a = 0f;
					}
					morphData.materialData[j] = morphMaterialData;
				}
				break;
			}
			}
			if (!binaryReader.EndStruct())
			{
				return false;
			}
			modelData.morphDataList[i] = morphData;
			if (!string.IsNullOrEmpty(morphData.nameJp))
			{
				modelData.morphDataDictionaryJp[morphData.nameJp] = i;
			}
			if (!string.IsNullOrEmpty(morphData.nameEn))
			{
				modelData.morphDataDictionaryEn[morphData.nameEn] = i;
			}
			if (modelData.isMorphTranslateName && !string.IsNullOrEmpty(morphData.translatedName))
			{
				modelData.translatedMorphDataDictionary[morphData.translatedName] = i;
			}
		}
		return true;
	}

	private static bool _ParseRigidBodyData(ModelData modelData, MMD4MecanimCommon.BinaryReader binaryReader)
	{
		modelData.rigidBodyDataList = new RigidBodyData[binaryReader.currentStructLength];
		for (int i = 0; i < binaryReader.currentStructLength; i++)
		{
			if (!binaryReader.BeginStruct())
			{
				return false;
			}
			RigidBodyData rigidBodyData = new RigidBodyData();
			int rigidBodyAdditionalFlags = binaryReader.ReadStructInt();
			rigidBodyData.nameJp = binaryReader.GetName(binaryReader.ReadStructInt());
			rigidBodyData.nameEn = binaryReader.GetName(binaryReader.ReadStructInt());
			rigidBodyData.boneID = binaryReader.ReadStructInt();
			rigidBodyData.collisionGroupID = binaryReader.ReadStructInt();
			rigidBodyData.collisionMask = binaryReader.ReadStructInt();
			rigidBodyData.shapeType = (ShapeType)binaryReader.ReadStructInt();
			rigidBodyData.rigidBodyType = (RigidBodyType)binaryReader.ReadStructInt();
			rigidBodyData.shapeSize = binaryReader.ReadStructVector3();
			rigidBodyData.position = binaryReader.ReadStructVector3();
			rigidBodyData.rotation = binaryReader.ReadStructVector3();
			rigidBodyData.mass = binaryReader.ReadStructFloat();
			rigidBodyData.linearDamping = binaryReader.ReadStructFloat();
			rigidBodyData.angularDamping = binaryReader.ReadStructFloat();
			rigidBodyData.restitution = binaryReader.ReadStructFloat();
			rigidBodyData.friction = binaryReader.ReadStructFloat();
			rigidBodyData.rigidBodyAdditionalFlags = (RigidBodyAdditionalFlags)rigidBodyAdditionalFlags;
			modelData.rigidBodyDataList[i] = rigidBodyData;
			if (!binaryReader.EndStruct())
			{
				return false;
			}
		}
		return true;
	}

	private static bool _ParseJointData(ModelData modelData, MMD4MecanimCommon.BinaryReader binaryReader)
	{
		modelData.jointDataList = new JointData[binaryReader.currentStructLength];
		for (int i = 0; i < binaryReader.currentStructLength; i++)
		{
			if (!binaryReader.BeginStruct())
			{
				return false;
			}
			JointData jointData = new JointData();
			binaryReader.ReadStructInt();
			binaryReader.ReadStructInt();
			binaryReader.ReadStructInt();
			binaryReader.ReadStructInt();
			jointData.rigidBodyIDA = binaryReader.ReadStructInt();
			jointData.rigidBodyIDB = binaryReader.ReadStructInt();
			modelData.jointDataList[i] = jointData;
			if (!binaryReader.EndStruct())
			{
				return false;
			}
		}
		return true;
	}

	public static ExtraData BuildExtraData(TextAsset extraFile)
	{
		if (extraFile == null)
		{
			Debug.LogError("BuildExtraData: extraFile is norhing.");
			return null;
		}
		byte[] bytes = extraFile.bytes;
		if (bytes == null || bytes.Length == 0)
		{
			Debug.LogError("BuildModelData: Nothing extraBytes.");
			return null;
		}
		MMD4MecanimCommon.BinaryReader binaryReader = new MMD4MecanimCommon.BinaryReader(bytes);
		if (!binaryReader.Preparse())
		{
			Debug.LogError("BuildModelData:extraFile is unsupported fomart.");
			return null;
		}
		ExtraData extraData = new ExtraData();
		binaryReader.BeginHeader();
		binaryReader.ReadHeaderInt();
		binaryReader.ReadHeaderFloat();
		binaryReader.ReadHeaderInt();
		binaryReader.ReadHeaderInt();
		extraData.vertexCount = binaryReader.ReadHeaderInt();
		binaryReader.ReadHeaderInt();
		extraData.vertexScale = binaryReader.ReadHeaderFloat();
		extraData.importScale = binaryReader.ReadHeaderFloat();
		binaryReader.EndHeader();
		int structListLength = binaryReader.structListLength;
		for (int i = 0; i < structListLength; i++)
		{
			if (!binaryReader.BeginStructList())
			{
				Debug.LogError("BuildExtraData: Parse error.");
				return null;
			}
			if (binaryReader.currentStructFourCC == MMD4MecanimCommon.BinaryReader.MakeFourCC("VTX_") && !_ParseVertexData(extraData, binaryReader))
			{
				Debug.LogError("BuildExtraData: Parse error.");
				return null;
			}
			if (!binaryReader.EndStructList())
			{
				Debug.LogError("BuildExtraData: Parse error.");
				return null;
			}
		}
		return extraData;
	}

	private static bool _ParseVertexData(ExtraData extraData, MMD4MecanimCommon.BinaryReader binaryReader)
	{
		extraData.vertexDataList = new VertexData[binaryReader.currentStructLength];
		for (int i = 0; i < binaryReader.currentStructLength; i++)
		{
			if (!binaryReader.BeginStruct())
			{
				return false;
			}
			VertexData vertexData = default(VertexData);
			vertexData.flags = (VertexFlags)binaryReader.ReadStructInt();
			vertexData.position = binaryReader.ReadStructVector3();
			vertexData.normal = binaryReader.ReadStructVector3();
			vertexData.uv = binaryReader.ReadStructVector2();
			vertexData.edgeScale = binaryReader.ReadStructFloat();
			vertexData.boneTransform = (BoneTransform)binaryReader.ReadStructByte();
			vertexData.boneWeight = default(BoneWeight);
			switch (vertexData.boneTransform)
			{
			case BoneTransform.BDEF1:
				vertexData.boneWeight.boneIndex0 = binaryReader.ReadInt();
				vertexData.boneWeight.weight0 = 1f;
				break;
			case BoneTransform.BDEF2:
			case BoneTransform.SDEF:
				vertexData.boneWeight.boneIndex0 = binaryReader.ReadInt();
				vertexData.boneWeight.boneIndex1 = binaryReader.ReadInt();
				vertexData.boneWeight.weight0 = binaryReader.ReadFloat();
				vertexData.boneWeight.weight1 = 1f - vertexData.boneWeight.weight0;
				break;
			case BoneTransform.BDEF4:
			case BoneTransform.QDEF:
				vertexData.boneWeight.boneIndex0 = binaryReader.ReadInt();
				vertexData.boneWeight.boneIndex1 = binaryReader.ReadInt();
				vertexData.boneWeight.boneIndex2 = binaryReader.ReadInt();
				vertexData.boneWeight.boneIndex3 = binaryReader.ReadInt();
				vertexData.boneWeight.weight0 = binaryReader.ReadFloat();
				vertexData.boneWeight.weight1 = binaryReader.ReadFloat();
				vertexData.boneWeight.weight2 = binaryReader.ReadFloat();
				vertexData.boneWeight.weight3 = binaryReader.ReadFloat();
				break;
			}
			if (vertexData.boneTransform == BoneTransform.SDEF)
			{
				vertexData.sdefC = binaryReader.ReadVector3();
				vertexData.sdefR0 = binaryReader.ReadVector3();
				vertexData.sdefR1 = binaryReader.ReadVector3();
			}
			if (!binaryReader.EndStruct())
			{
				return false;
			}
			extraData.vertexDataList[i] = vertexData;
		}
		return true;
	}

	public static AnimData BuildAnimData(TextAsset animFile)
	{
		if (animFile == null)
		{
			Debug.LogError("BuildAnimData: animFile is norhing.");
			return null;
		}
		byte[] bytes = animFile.bytes;
		if (bytes == null || bytes.Length == 0)
		{
			Debug.LogError("BuildAnimData: Nothing animBytes.");
			return null;
		}
		MMD4MecanimCommon.BinaryReader binaryReader = new MMD4MecanimCommon.BinaryReader(bytes);
		if (!binaryReader.Preparse())
		{
			Debug.LogError("BuildAnimData:animFile is unsupported fomart.");
			return null;
		}
		AnimData animData = new AnimData();
		binaryReader.BeginHeader();
		binaryReader.ReadHeaderInt();
		binaryReader.ReadHeaderInt();
		animData.maxFrame = binaryReader.ReadHeaderInt();
		binaryReader.EndHeader();
		int structListLength = binaryReader.structListLength;
		for (int i = 0; i < structListLength; i++)
		{
			if (!binaryReader.BeginStructList())
			{
				Debug.LogError("BuildAnimData: Parse error.");
				return null;
			}
			if (binaryReader.currentStructFourCC == MMD4MecanimCommon.BinaryReader.MakeFourCC("MRPH") && !_ParseMorphMotionData(animData, binaryReader))
			{
				Debug.LogError("BuildAnimData: Parse error.");
				return null;
			}
			if (!binaryReader.EndStructList())
			{
				Debug.LogError("BuildAnimData: Parse error.");
				return null;
			}
		}
		return animData;
	}

	private static bool _ParseMorphMotionData(AnimData animData, MMD4MecanimCommon.BinaryReader binaryReader)
	{
		animData.morphMotionDataList = new MorphMotionData[binaryReader.currentStructLength];
		for (int i = 0; i < binaryReader.currentStructLength; i++)
		{
			if (!binaryReader.BeginStruct())
			{
				return false;
			}
			MorphMotionData morphMotionData = new MorphMotionData();
			binaryReader.ReadStructInt();
			morphMotionData.name = binaryReader.GetName(binaryReader.ReadStructInt());
			int num = binaryReader.ReadStructInt();
			if (num < 0)
			{
				return false;
			}
			morphMotionData.frameNos = new int[num];
			morphMotionData.f_frameNos = new float[num];
			morphMotionData.weights = new float[num];
			for (int j = 0; j < num; j++)
			{
				morphMotionData.frameNos[j] = binaryReader.ReadInt();
				morphMotionData.weights[j] = binaryReader.ReadFloat();
				morphMotionData.f_frameNos[j] = morphMotionData.frameNos[j];
			}
			animData.morphMotionDataList[i] = morphMotionData;
			if (!binaryReader.EndStruct())
			{
				return false;
			}
		}
		return true;
	}
}
