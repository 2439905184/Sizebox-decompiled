using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Sizebox.CharacterEditor
{
	[RequireComponent(typeof(EntityBase))]
	public class CharacterEditor : MonoBehaviour
	{
		private EntityBase entity;

		private Dictionary<Transform, string> transformKeyMap = new Dictionary<Transform, string>();

		private Dictionary<string, Transform> keyTransformMap = new Dictionary<string, Transform>();

		public SkeletonEdit SkeletonEdit { get; private set; }

		public MaterialEdit MaterialEdit { get; private set; }

		public WeldTool WeldTool { get; private set; }

		public JiggleEdit JiggleEdit { get; private set; }

		public Animator Animator { get; private set; }

		public string SkeletonHashCode { get; private set; }

		public string FolderName { get; private set; }

		public string FolderPath { get; private set; }

		public Dictionary<string, Transform>.ValueCollection Transforms
		{
			get
			{
				return keyTransformMap.Values;
			}
		}

		private void Awake()
		{
			entity = GetComponent<EntityBase>();
			Animator = entity.GetComponentInChildren<Animator>();
			GenerateFileNames();
			GeneratePath();
			if ((bool)Animator && Animator.GetBoneTransform(HumanBodyBones.Hips) != null)
			{
				CreateTransformMap(Animator.GetBoneTransform(HumanBodyBones.Hips));
			}
			else
			{
				for (int i = 0; i < base.transform.childCount; i++)
				{
					CreateTransformMap(base.transform.GetChild(i), i.ToString());
				}
			}
			GenerateSkeletonHashCode();
			MaterialEdit = base.gameObject.AddComponent<MaterialEdit>();
			SkeletonEdit = base.gameObject.AddComponent<SkeletonEdit>();
			WeldTool = base.gameObject.AddComponent<WeldTool>();
			JiggleEdit = base.gameObject.AddComponent<JiggleEdit>();
		}

		private void GenerateFileNames()
		{
			string assetFullName = entity.asset.AssetFullName;
			string text = "";
			string[] array = assetFullName.Split('/', '\\');
			foreach (string text2 in array)
			{
				text += text2;
			}
			text = text.Replace(".micro", "-");
			text = text.Replace(".gts", "-");
			FolderName = text;
		}

		private void GeneratePath()
		{
			string text = Sbox.StringPreferenceOrArgument(GlobalPreferences.PathData, "-path-data");
			text = Path.Combine(string.IsNullOrEmpty(text) ? Application.persistentDataPath : text, "Character");
			FolderPath = text + Path.DirectorySeparatorChar + FolderName + Path.DirectorySeparatorChar;
		}

		private void CreateTransformMap(Transform inTransform, string id = "0")
		{
			if (!inTransform.GetComponent<EntityBase>() && !inTransform.GetComponent<MeshCollider>() && !inTransform.GetComponent<NotABone>())
			{
				keyTransformMap.Add(id, inTransform);
				transformKeyMap.Add(inTransform, id);
				for (int i = 0; i < inTransform.childCount; i++)
				{
					CreateTransformMap(inTransform.GetChild(i), id + "," + i);
				}
			}
		}

		public string RegisterTransform(Transform newTransform, Transform parent, string seperator = ",", string customKey = null)
		{
			string transformKey = GetTransformKey(parent);
			if (transformKey == null)
			{
				return null;
			}
			SkeletonEdit.Disable();
			string text;
			if (customKey == null)
			{
				text = transformKey + seperator + parent.childCount;
				int num = 1;
				while (keyTransformMap.ContainsKey(text))
				{
					text = transformKey + seperator + (parent.childCount + num);
					num++;
				}
			}
			else
			{
				text = transformKey + seperator + customKey;
			}
			SkeletonEdit.Enable();
			keyTransformMap.Add(text, newTransform);
			transformKeyMap.Add(newTransform, text);
			return text;
		}

		public void DeleteTransform(string key)
		{
			if (keyTransformMap.ContainsKey(key))
			{
				Transform transform = keyTransformMap[key];
				keyTransformMap.Remove(key);
				transformKeyMap.Remove(transform);
				if ((bool)transform.GetComponent<EntityBase>())
				{
					transform.GetComponent<EntityBase>().DestroyObject();
				}
				else
				{
					Object.Destroy(transform.gameObject);
				}
			}
		}

		private void GenerateSkeletonHashCode()
		{
			string s = GenerateTransformMapString();
			byte[] array = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(s));
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString("x2"));
			}
			SkeletonHashCode = stringBuilder.ToString();
		}

		private string GenerateTransformMapString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string key in keyTransformMap.Keys)
			{
				stringBuilder.Append(key);
				stringBuilder.Append(':');
			}
			return stringBuilder.ToString();
		}

		public Transform GetTransform(string key)
		{
			if (keyTransformMap.ContainsKey(key))
			{
				return keyTransformMap[key];
			}
			return null;
		}

		public string GetTransformKey(Transform inTransform)
		{
			if (transformKeyMap.ContainsKey(inTransform))
			{
				return transformKeyMap[inTransform];
			}
			return null;
		}

		private void OnDestroy()
		{
			Object.Destroy(WeldTool);
			Object.Destroy(MaterialEdit);
			Object.Destroy(SkeletonEdit);
			Object.Destroy(JiggleEdit);
		}

		public void Load(CharacterEditorSaveData saveData)
		{
			SkeletonEdit.Load(saveData.skeletonData);
			MaterialEdit.Load(saveData.materialData);
			JiggleEdit.Load(saveData.jiggleData);
		}

		public CharacterEditorSaveData Save()
		{
			return new CharacterEditorSaveData
			{
				skeletonData = SkeletonEdit.Save(),
				materialData = MaterialEdit.Save(),
				jiggleData = JiggleEdit.Save()
			};
		}
	}
}
