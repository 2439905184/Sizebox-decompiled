using System;
using System.Collections.Generic;
using Sizebox.CharacterEditor;
using UnityEngine;

namespace SaveDataStructures
{
	[Serializable]
	public class EntitySaveData : SavableData
	{
		public int id;

		public string name;

		public string model;

		public ParentData parentingData;

		public Vector3 virtualPosition;

		public Quaternion rotation;

		public float scale;

		public float floorOffset;

		public MorphSaveData[] morphs;

		public CharacterEditorSaveData characterEditorSaveData;

		public override SavableDataType DataType
		{
			get
			{
				return SavableDataType.EntityData;
			}
		}

		public EntitySaveData(EntityBase entity)
		{
			if (entity == null)
			{
				Debug.LogError("Null entity in save data, empty save data created");
				return;
			}
			id = entity.id;
			name = entity.name;
			model = entity.asset.AssetFullName;
			Transform transform;
			virtualPosition = (transform = entity.transform).position.ToVirtual();
			floorOffset = entity.offset;
			rotation = transform.rotation;
			scale = entity.Scale;
			parentingData = new ParentData(transform, transform.parent);
			if (entity.MorphsInitialized)
			{
				List<MorphSaveData> list = new List<MorphSaveData>();
				for (int i = 0; i < entity.Morphs.Count; i++)
				{
					float weight = entity.Morphs[i].Weight;
					if (weight > 0f)
					{
						MorphSaveData item = new MorphSaveData
						{
							index = i,
							value = weight
						};
						list.Add(item);
					}
				}
				morphs = list.ToArray();
			}
			CharacterEditor component = entity.GetComponent<CharacterEditor>();
			if ((bool)component)
			{
				characterEditorSaveData = component.Save();
			}
		}
	}
}
