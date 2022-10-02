using System.Collections.Generic;
using SizeboxUI;
using UnityEngine;

namespace Sizebox.CharacterEditor
{
	[RequireComponent(typeof(CharacterEditor))]
	public class MaterialEdit : MonoBehaviour
	{
		private int _count;

		private MaterialControlGui _controls;

		public HashSet<MaterialWrapper> Materials { get; private set; }

		private void Awake()
		{
			HashSet<Renderer> renderers = GetRenderers();
			Materials = new HashSet<MaterialWrapper>();
			_controls = GuiManager.Instance.MainCanvasGameObject.GetComponentInChildren<MaterialControlGui>(true);
			foreach (Renderer item in renderers)
			{
				Material[] materials = item.materials;
				foreach (Material material in materials)
				{
					Materials.Add(new MaterialWrapper(material, _count, _controls));
					_count++;
				}
			}
		}

		private HashSet<Renderer> GetRenderers()
		{
			EntityBase component = GetComponent<EntityBase>();
			if (!component)
			{
				return null;
			}
			List<EntityBase> list = new List<EntityBase>();
			GetComponentsInChildren(list);
			if (list.Contains(component))
			{
				list.Remove(component);
			}
			HashSet<Renderer> hashSet = new HashSet<Renderer>(component.GetComponentsInChildren<Renderer>());
			foreach (EntityBase item2 in list)
			{
				Renderer[] componentsInChildren = item2.GetComponentsInChildren<Renderer>();
				foreach (Renderer item in componentsInChildren)
				{
					if (hashSet.Contains(item))
					{
						hashSet.Remove(item);
					}
				}
			}
			return hashSet;
		}

		public void AddMaterials(ICollection<Material> materials)
		{
			foreach (Material material in materials)
			{
				Materials.Add(new MaterialWrapper(material, _count, _controls));
				_count++;
			}
		}

		public void RemoveMaterials(HashSet<Material> matsToRemove)
		{
			List<MaterialWrapper> list = new List<MaterialWrapper>();
			foreach (MaterialWrapper material in Materials)
			{
				if (matsToRemove.Contains(material.Material))
				{
					list.Add(material);
				}
			}
			foreach (MaterialWrapper item in list)
			{
				Materials.Remove(item);
			}
		}

		public void LoadPreset(MaterialDataSet materialData)
		{
			foreach (MaterialWrapper material in Materials)
			{
				material.ResetMaterial();
			}
			foreach (MaterialData matDatum in materialData.matData)
			{
				foreach (MaterialWrapper material2 in Materials)
				{
					if (material2.Id.Equals(matDatum.id))
					{
						matDatum.LoadMaterialData(material2);
						break;
					}
				}
			}
		}

		public MaterialDataSet SavePreset()
		{
			MaterialDataSet materialDataSet = new MaterialDataSet();
			foreach (MaterialWrapper material in Materials)
			{
				MaterialData data = material.GenerateMaterialData();
				materialDataSet.Add(data);
			}
			return materialDataSet;
		}

		public void Load(MaterialEditSaveData materialEditData)
		{
			LoadPreset(materialEditData.materialDataset);
		}

		public MaterialEditSaveData Save()
		{
			return new MaterialEditSaveData
			{
				materialDataset = SavePreset()
			};
		}
	}
}
