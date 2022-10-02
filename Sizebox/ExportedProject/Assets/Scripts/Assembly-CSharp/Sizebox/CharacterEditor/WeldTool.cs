using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sizebox.CharacterEditor
{
	[RequireComponent(typeof(CharacterEditor))]
	[RequireComponent(typeof(SkeletonEdit))]
	public class WeldTool : MonoBehaviour
	{
		public const string WELD_SEPERATOR = ";W;";

		private CharacterEditor CharacterEditor;

		private SkeletonEdit SkeletonEdit;

		private MaterialEdit MaterialEdit;

		private Dictionary<string, WeldData> ObjectData = new Dictionary<string, WeldData>();

		public List<string> Keys { get; private set; }

		private void Awake()
		{
			Keys = new List<string>();
			CharacterEditor = GetComponent<CharacterEditor>();
			SkeletonEdit = GetComponent<SkeletonEdit>();
			MaterialEdit = GetComponent<MaterialEdit>();
		}

		public List<string> Weld(EntityBase obj, List<EditBone> bones)
		{
			List<string> list = new List<string>();
			if (obj.isPlayer)
			{
				return list;
			}
			CleanObject(obj);
			string item = Weld(obj, bones[0]);
			Keys.Add(item);
			list.Add(item);
			for (int i = 1; i < bones.Count; i++)
			{
				EntityBase entityBase = UnityEngine.Object.Instantiate(obj);
				CleanObject(entityBase);
				item = Weld(entityBase, bones[i]);
				Keys.Add(item);
				list.Add(item);
			}
			return list;
		}

		private string Weld(EntityBase obj, EditBone parent)
		{
			if (obj.isPlayer)
			{
				return null;
			}
			obj.transform.parent = parent.RealTransform;
			string customKey = AddWeldData(obj.asset.AssetFullName, parent.Key);
			string result = CharacterEditor.RegisterTransform(obj.transform, parent.RealTransform, ";W;", customKey);
			SkeletonEdit.CreateBone(obj.transform, parent);
			if ((bool)MaterialEdit)
			{
				List<Material> list = new List<Material>();
				Renderer[] componentsInChildren = obj.GetComponentsInChildren<Renderer>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					Material[] materials = componentsInChildren[i].materials;
					foreach (Material item in materials)
					{
						list.Add(item);
					}
				}
				MaterialEdit.AddMaterials(list);
			}
			return result;
		}

		public void Unweld(SkeletonEditHandle handle)
		{
			EntityBase component = handle.EditBone.RealTransform.GetComponent<EntityBase>();
			string key = handle.EditBone.Key;
			if (!Keys.Contains(key))
			{
				return;
			}
			if ((bool)component)
			{
				if ((bool)MaterialEdit)
				{
					HashSet<Material> hashSet = new HashSet<Material>();
					Renderer[] componentsInChildren = component.GetComponentsInChildren<Renderer>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						Material[] materials = componentsInChildren[i].materials;
						foreach (Material item in materials)
						{
							hashSet.Add(item);
						}
					}
					MaterialEdit.RemoveMaterials(hashSet);
				}
				RemoveWeldData(component, handle);
			}
			Keys.Remove(key);
		}

		private void CleanObject(EntityBase weldee)
		{
			EntityBase component = GetComponent<EntityBase>();
			Behaviour[] componentsInChildren = weldee.GetComponentsInChildren<Behaviour>();
			foreach (Behaviour behaviour in componentsInChildren)
			{
				if (!IsIgnoredScript(behaviour))
				{
					behaviour.enabled = false;
				}
			}
			if (component.isMicro)
			{
				Collider[] componentsInChildren2 = weldee.GetComponentsInChildren<Collider>();
				foreach (Collider collider in componentsInChildren2)
				{
					if ((bool)(collider as MeshCollider) && !(collider as MeshCollider).convex)
					{
						collider.enabled = false;
					}
					else
					{
						collider.isTrigger = true;
					}
				}
			}
			if (weldee.isGiantess)
			{
				weldee.Rigidbody.gameObject.SetActive(false);
			}
			else
			{
				Rigidbody componentInChildren = weldee.GetComponentInChildren<Rigidbody>();
				if ((bool)componentInChildren)
				{
					componentInChildren.isKinematic = true;
				}
			}
			if (component.isGiantess)
			{
				SetLayerRecursively(weldee.transform, Layers.gtsBodyLayer);
			}
			else
			{
				SetLayerRecursively(weldee.transform, Layers.defaultLayer);
			}
			weldee.gameObject.AddComponent<Weld>();
		}

		private bool IsIgnoredScript(Behaviour script)
		{
			if ((bool)(script as Animator) || (bool)(script as DynamicBone) || (bool)(script as Destructor))
			{
				return true;
			}
			if ((bool)(script as SkeletonEdit) || (bool)(script as MaterialEdit))
			{
				return true;
			}
			return false;
		}

		private void SetLayerRecursively(Transform inTransform, int layer)
		{
			if (inTransform.gameObject.layer != Layers.destroyerLayer)
			{
				inTransform.gameObject.layer = layer;
			}
			for (int i = 0; i < inTransform.childCount; i++)
			{
				SetLayerRecursively(inTransform.GetChild(i), layer);
			}
		}

		private string AddWeldData(string objectName, string parentKey)
		{
			string key = CreateObjectKey(objectName, parentKey);
			if (!ObjectData.ContainsKey(key))
			{
				ObjectData.Add(key, new WeldData(objectName));
			}
			return ObjectData[key].Add();
		}

		private void RemoveWeldData(EntityBase obj, SkeletonEditHandle handle)
		{
			string key = CreateObjectKey(obj.asset.AssetFullName, handle.EditBone.ParentBone.Key);
			string key2 = handle.EditBone.Key;
			if (ObjectData.ContainsKey(key))
			{
				string[] array = key2.Split(new string[1] { ";W;" }, StringSplitOptions.None);
				if (array.Length != 0)
				{
					string weldKey = array[array.Length - 1];
					ObjectData[key].Remove(weldKey);
				}
			}
		}

		private static string CreateObjectKey(string objectName, string parentKey)
		{
			return parentKey + objectName;
		}
	}
}
