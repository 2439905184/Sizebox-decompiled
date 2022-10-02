using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Sizebox.CharacterEditor
{
	[RequireComponent(typeof(CharacterEditor))]
	public class SkeletonEdit : MonoBehaviour
	{
		private CharacterEditor characterEditor;

		private EditBone skeletonBonePrefab;

		private SkeletonEditConfigData configData = new SkeletonEditConfigData();

		private const string FILE_EXTENSION = ".skeletondata";

		public SkeletonEditSkeleton Skeleton { get; private set; }

		public List<EditBone> SkeletonBones
		{
			get
			{
				return Skeleton.Bones;
			}
		}

		public Dictionary<string, EditBone> BoneMap { get; private set; }

		public Dictionary<EditBone, string> KeyMap { get; private set; }

		private void Awake()
		{
			characterEditor = GetComponent<CharacterEditor>();
			skeletonBonePrefab = (Resources.Load("SkeletonEdit/Bone") as GameObject).GetComponent<EditBone>();
			BoneMap = new Dictionary<string, EditBone>();
			KeyMap = new Dictionary<EditBone, string>();
			CreateEditBones();
			Skeleton = new SkeletonEditSkeleton();
			Skeleton.Initialize(characterEditor.Animator);
			if (!LoadConfigDataFromFile())
			{
				configData = new SkeletonEditConfigData();
				Skeleton.GeneratePairings(this);
				ApplyConfig(configData);
			}
		}

		public void SaveConfig()
		{
			string filePath = characterEditor.FolderPath + characterEditor.FolderName + ".skeletondata";
			SaveConfigFile(filePath, configData);
		}

		private void SaveConfigFile(string filePath, SkeletonEditConfigData data)
		{
			StreamWriter streamWriter = new StreamWriter(filePath);
			string value = JsonUtility.ToJson(data);
			streamWriter.Write(value);
			streamWriter.Close();
		}

		public bool LoadConfigDataFromFile()
		{
			string folderPath = characterEditor.FolderPath;
			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}
			string text = folderPath + characterEditor.FolderName + ".skeletondata";
			if (File.Exists(text))
			{
				try
				{
					configData = LoadConfigFile(text);
					ApplyConfig(configData);
					return true;
				}
				catch
				{
					return false;
				}
			}
			return false;
		}

		private SkeletonEditConfigData LoadConfigFile(string filePath)
		{
			if (File.Exists(filePath))
			{
				return JsonUtility.FromJson<SkeletonEditConfigData>(File.ReadAllText(filePath));
			}
			return new SkeletonEditConfigData();
		}

		public void ApplyConfig(SkeletonEditConfigData data)
		{
			if (data == null)
			{
				return;
			}
			ClearAllBoneSiblingData();
			foreach (KeyPair sibling in data.siblings)
			{
				if (BoneMap.ContainsKey(sibling.key) && BoneMap.ContainsKey(sibling.key2))
				{
					EditBone editBone = BoneMap[sibling.key];
					EditBone editBone2 = BoneMap[sibling.key2];
					editBone.SetSibling(editBone2);
					editBone2.SetSibling(editBone);
				}
			}
		}

		private void ClearAllBoneSiblingData()
		{
			foreach (EditBone value in BoneMap.Values)
			{
				value.SetSibling(null);
			}
		}

		private void CreateEditBones()
		{
			BoneMap.Clear();
			KeyMap.Clear();
			foreach (Transform transform in characterEditor.Transforms)
			{
				CreateEditBone(transform);
			}
		}

		private void CreateEditBone(Transform inTransform)
		{
			string transformKey = characterEditor.GetTransformKey(inTransform);
			if (transformKey != null)
			{
				EditBone componentInParent = inTransform.GetComponentInParent<EditBone>();
				EditBone editBone = Object.Instantiate(skeletonBonePrefab);
				editBone.Initialize(transformKey, this, inTransform, componentInParent);
				if (!BoneMap.ContainsKey(transformKey))
				{
					BoneMap.Add(transformKey, editBone);
				}
				if (!KeyMap.ContainsKey(editBone))
				{
					KeyMap.Add(editBone, transformKey);
				}
			}
		}

		public void CreateBone(Transform inTransform, EditBone parent)
		{
			string transformKey = characterEditor.GetTransformKey(inTransform);
			if (transformKey != null)
			{
				EditBone editBone = Object.Instantiate(skeletonBonePrefab);
				editBone.Initialize(transformKey, this, inTransform, parent);
				inTransform.localPosition = Vector3.zero;
				inTransform.localRotation = Quaternion.identity;
				inTransform.localScale = Vector3.one;
				if (!BoneMap.ContainsKey(transformKey))
				{
					BoneMap.Add(transformKey, editBone);
				}
				if (!KeyMap.ContainsKey(editBone))
				{
					KeyMap.Add(editBone, transformKey);
				}
			}
		}

		public void Enable()
		{
			foreach (EditBone value in BoneMap.Values)
			{
				value.Enable();
			}
			if ((bool)Skeleton.hips)
			{
				Skeleton.hips.UpdateTransformationRecursive();
				return;
			}
			EditBone componentInChildren = GetComponentInChildren<EditBone>();
			if ((bool)componentInChildren)
			{
				componentInChildren.UpdateTransformationRecursive();
			}
		}

		public void Disable()
		{
			foreach (EditBone value in BoneMap.Values)
			{
				value.Disable();
			}
		}

		public EditBone GetBone(string key)
		{
			if (BoneMap.ContainsKey(key))
			{
				return BoneMap[key];
			}
			return null;
		}

		public EditBone GetBone(SkeletonEditBones bone)
		{
			SkeletonEditSkeleton skeleton = Skeleton;
			switch (bone)
			{
			case SkeletonEditBones.None:
				return null;
			case SkeletonEditBones.Head:
				return skeleton.head;
			case SkeletonEditBones.Neck:
				return skeleton.neck;
			case SkeletonEditBones.UpperSpine:
				return skeleton.upperSpine;
			case SkeletonEditBones.MidSpine:
				return skeleton.midSpine;
			case SkeletonEditBones.LowerSpine:
				return skeleton.lowerSpine;
			case SkeletonEditBones.Hips:
				return skeleton.hips;
			case SkeletonEditBones.LeftUpperArm:
				return skeleton.leftUpperArm;
			case SkeletonEditBones.LeftLowerArm:
				return skeleton.leftLowerArm;
			case SkeletonEditBones.LeftHand:
				return skeleton.leftHand;
			case SkeletonEditBones.RightUpperArm:
				return skeleton.rightUpperArm;
			case SkeletonEditBones.RightLowerArm:
				return skeleton.rightLowerArm;
			case SkeletonEditBones.RightHand:
				return skeleton.rightHand;
			case SkeletonEditBones.LeftShoulder:
				return skeleton.leftShoulder;
			case SkeletonEditBones.RightShoulder:
				return skeleton.rightShoulder;
			case SkeletonEditBones.LeftUpperLeg:
				return skeleton.leftUpperLeg;
			case SkeletonEditBones.LeftLowerLeg:
				return skeleton.leftLowerLeg;
			case SkeletonEditBones.LeftFoot:
				return skeleton.leftFoot;
			case SkeletonEditBones.RightUpperLeg:
				return skeleton.rightUpperLeg;
			case SkeletonEditBones.RightLowerLeg:
				return skeleton.rightLowerLeg;
			case SkeletonEditBones.RightFoot:
				return skeleton.rightFoot;
			case SkeletonEditBones.LeftBreast:
				return skeleton.leftBreast;
			case SkeletonEditBones.RightBreast:
				return skeleton.rightBreast;
			default:
				return null;
			}
		}

		public static List<EditBone> FindFirstChildEditBones(Transform bone)
		{
			List<EditBone> childBones = new List<EditBone>();
			for (int i = 0; i < bone.transform.childCount; i++)
			{
				FindFirstChildBonesLoop(bone.transform.GetChild(i), ref childBones);
			}
			return childBones;
		}

		private static void FindFirstChildBonesLoop(Transform transform, ref List<EditBone> childBones)
		{
			EditBone component = transform.GetComponent<EditBone>();
			if ((bool)component)
			{
				childBones.Add(component);
				return;
			}
			for (int i = 0; i < transform.childCount; i++)
			{
				FindFirstChildBonesLoop(transform.GetChild(i), ref childBones);
			}
		}

		public void LoadPreset(SkeletonPresetData preset, string dataId)
		{
			foreach (EditBone value in BoneMap.Values)
			{
				value.SetData(dataId, BoneTransformData.Default);
			}
			BoneData[] boneData;
			if (preset.hashCode.Equals(characterEditor.SkeletonHashCode))
			{
				boneData = preset.boneData;
				for (int i = 0; i < boneData.Length; i++)
				{
					BoneData boneData2 = boneData[i];
					if (BoneMap.ContainsKey(boneData2.key))
					{
						BoneMap[boneData2.key].SetData(dataId, boneData2.data);
					}
				}
				return;
			}
			Debug.Log("Using generic loading!");
			Dictionary<string, BoneTransformData> dictionary = new Dictionary<string, BoneTransformData>();
			boneData = preset.boneData;
			for (int i = 0; i < boneData.Length; i++)
			{
				BoneData boneData3 = boneData[i];
				dictionary.Add(boneData3.key, boneData3.data);
			}
			SkeletonDataMap[] skeletonMap = preset.skeletonMap;
			for (int i = 0; i < skeletonMap.Length; i++)
			{
				SkeletonDataMap skeletonDataMap = skeletonMap[i];
				string key = skeletonDataMap.key;
				if (dictionary.ContainsKey(key))
				{
					Skeleton.SetData(skeletonDataMap.bone, dataId, dictionary[key]);
				}
			}
		}

		public void CreatePair(string key, string key2)
		{
			EditBone bone = null;
			EditBone bone2 = null;
			if (BoneMap.ContainsKey(key))
			{
				bone = BoneMap[key];
			}
			if (BoneMap.ContainsKey(key2))
			{
				bone2 = BoneMap[key2];
			}
			CreatePair(bone, bone2);
		}

		public void CreatePair(EditBone bone, EditBone bone2)
		{
			if ((bool)bone && (bool)bone2)
			{
				bone.SetSibling(bone2);
				bone2.SetSibling(bone);
				string key = bone.Key;
				string key2 = bone2.Key;
				KeyPair pair = new KeyPair(key, key2);
				configData.AddPair(pair);
			}
		}

		public void RemovePair(string key, string key2)
		{
			EditBone bone = null;
			EditBone bone2 = null;
			if (BoneMap.ContainsKey(key))
			{
				bone = BoneMap[key];
			}
			if (BoneMap.ContainsKey(key2))
			{
				bone2 = BoneMap[key2];
			}
			RemovePair(bone, bone2);
		}

		public void RemovePair(EditBone bone, EditBone bone2)
		{
			if ((bool)bone && (bool)bone2)
			{
				bone.SetSibling(null);
				bone2.SetSibling(null);
				string key = bone.Key;
				string key2 = bone2.Key;
				KeyPair pair = new KeyPair(key, key2);
				configData.RemovePair(pair);
			}
		}

		public void Load(SkeletonEditSaveData skeletonData)
		{
			LoadPreset(skeletonData.bodyPreset, "body");
			LoadPreset(skeletonData.hairPreset, "hair");
			LoadPreset(skeletonData.extraPreset, "extra");
		}

		public SkeletonEditSaveData Save()
		{
			return new SkeletonEditSaveData
			{
				bodyPreset = new SkeletonPresetData("body", characterEditor),
				hairPreset = new SkeletonPresetData("hair", characterEditor),
				extraPreset = new SkeletonPresetData("extra", characterEditor)
			};
		}
	}
}
