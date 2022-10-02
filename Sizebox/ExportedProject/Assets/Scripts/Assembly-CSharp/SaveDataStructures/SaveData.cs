using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SaveDataStructures
{
	[Serializable]
	public class SaveData
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Converter<SavableObject, ISavable> _003C_003E9__11_0;

			internal ISavable _003C_002Ector_003Eb__11_0(SavableObject obj)
			{
				return obj.Savable;
			}
		}

		public static bool IsLoading;

		public string scene;

		public string version;

		public PlayerSaveData playerSaveData;

		public List<EntitySaveData> entityData = new List<EntitySaveData>();

		public List<MicroSaveData> microData = new List<MicroSaveData>();

		public List<MacroSaveData> macroData = new List<MacroSaveData>();

		public List<CitySaveData> cityData = new List<CitySaveData>();

		private int _highestId;

		private void UpdateHighestId(int id)
		{
			if (id > _highestId)
			{
				_highestId = id;
			}
		}

		public static SaveData Create()
		{
			return new SaveData(GameController.LocalClient.Player);
		}

		private SaveData(Player player)
		{
			version = Version.GetText();
			scene = SceneManager.GetActiveScene().name;
			EntityBase entity = player.Entity;
			playerSaveData = new PlayerSaveData(entity);
			foreach (ISavable item in new List<SavableObject>(UnityEngine.Object.FindObjectsOfType<SavableObject>()).ConvertAll(_003C_003Ec._003C_003E9__11_0 ?? (_003C_003Ec._003C_003E9__11_0 = _003C_003Ec._003C_003E9._003C_002Ector_003Eb__11_0)))
			{
				Save(item);
			}
		}

		private void Save(ISavable savable)
		{
			SavableData savableData = savable.Save();
			if (savableData != null)
			{
				switch (savableData.DataType)
				{
				case SavableDataType.EntityData:
					entityData.Add((EntitySaveData)savableData);
					break;
				case SavableDataType.MicroData:
					microData.Add((MicroSaveData)savableData);
					break;
				case SavableDataType.MacroData:
					macroData.Add((MacroSaveData)savableData);
					break;
				case SavableDataType.CityData:
					cityData.Add((CitySaveData)savableData);
					break;
				}
			}
		}

		public void Load()
		{
			IsLoading = true;
			_highestId = 0;
			try
			{
				PreloadAssets();
				SpawnCities();
				Dictionary<int, EntityBase> entities = new Dictionary<int, EntityBase>();
				Dictionary<int, AnimatedMicroNPC> micros = new Dictionary<int, AnimatedMicroNPC>();
				Dictionary<int, Giantess> macros = new Dictionary<int, Giantess>();
				SpawnEntities(entities, micros, macros);
				LoadData(entities, micros, macros);
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
			EntityBase.ForceNextId(_highestId + 1);
			IsLoading = false;
		}

		private void PreloadAssets()
		{
			AssetManager instance = AssetManager.Instance;
			foreach (EntitySaveData entityDatum in entityData)
			{
				AssetDescription assetDescriptionByName = instance.GetAssetDescriptionByName(entityDatum.model);
				if (assetDescriptionByName != null)
				{
					AssetLoader.LoadModelAssetSyncronous(assetDescriptionByName);
				}
			}
			foreach (MicroSaveData microDatum in microData)
			{
				AssetDescription assetDescriptionByName2 = instance.GetAssetDescriptionByName(microDatum.model);
				if (assetDescriptionByName2 != null)
				{
					AssetLoader.LoadModelAssetSyncronous(assetDescriptionByName2);
				}
			}
			foreach (MacroSaveData macroDatum in macroData)
			{
				AssetDescription assetDescriptionByName3 = instance.GetAssetDescriptionByName(macroDatum.model);
				if (assetDescriptionByName3 != null)
				{
					AssetLoader.LoadModelAssetSyncronous(assetDescriptionByName3);
				}
			}
		}

		private void SpawnCities()
		{
			AssetManager instance = AssetManager.Instance;
			foreach (CitySaveData cityDatum in cityData)
			{
				AssetDescription assetDescriptionByName = instance.GetAssetDescriptionByName(cityDatum.model);
				LocalClient.Instance.SpawnObject(assetDescriptionByName, cityDatum.virtualPosition.ToWorld(), cityDatum.rotation, cityDatum.scale, cityDatum.id).GetComponent<CityBuilder>().Load(cityDatum);
				UpdateHighestId(cityDatum.id);
			}
		}

		private void SpawnEntities(Dictionary<int, EntityBase> entities, Dictionary<int, AnimatedMicroNPC> micros, Dictionary<int, Giantess> macros)
		{
			AssetManager instance = AssetManager.Instance;
			foreach (EntitySaveData entityDatum in entityData)
			{
				AssetDescription assetDescriptionByName = instance.GetAssetDescriptionByName(entityDatum.model);
				if (assetDescriptionByName != null)
				{
					entities.Add(entityDatum.id, LocalClient.Instance.SpawnObject(assetDescriptionByName, entityDatum.virtualPosition.ToWorld(), entityDatum.rotation, entityDatum.scale, entityDatum.id).GetComponent<EntityBase>());
				}
				UpdateHighestId(entityDatum.id);
			}
			foreach (MicroSaveData microDatum in microData)
			{
				AssetDescription assetDescriptionByName2 = instance.GetAssetDescriptionByName(microDatum.model);
				if (assetDescriptionByName2 != null)
				{
					micros.Add(microDatum.id, LocalClient.Instance.SpawnMicro(assetDescriptionByName2, microDatum.virtualPosition.ToWorld(), microDatum.rotation, microDatum.scale, microDatum.id).GetComponent<AnimatedMicroNPC>());
				}
				UpdateHighestId(microDatum.id);
			}
			foreach (MacroSaveData macroDatum in macroData)
			{
				AssetDescription assetDescriptionByName3 = instance.GetAssetDescriptionByName(macroDatum.model);
				if (assetDescriptionByName3 != null)
				{
					macros.Add(macroDatum.id, LocalClient.Instance.SpawnGiantess(assetDescriptionByName3, macroDatum.virtualPosition.ToWorld(), macroDatum.rotation, macroDatum.scale, macroDatum.id).GetComponent<Giantess>());
				}
				UpdateHighestId(macroDatum.id);
			}
		}

		private void LoadData(Dictionary<int, EntityBase> entities, Dictionary<int, AnimatedMicroNPC> micros, Dictionary<int, Giantess> macros)
		{
			if (playerSaveData != null)
			{
				int controlledEntityId = playerSaveData.controlledEntityId;
				GameController.LocalClient.Player.PlayAs(ObjectManager.Instance.GetById(controlledEntityId) as IPlayable);
				Transform transform = GameController.Instance.transform;
				transform.position = playerSaveData.cameraVirtualPosition.ToWorld();
				transform.rotation = playerSaveData.cameraRotation;
			}
			foreach (EntitySaveData entityDatum in entityData)
			{
				entities[entityDatum.id].Load(entityDatum);
			}
			foreach (MicroSaveData microDatum in microData)
			{
				micros[microDatum.id].Load(microDatum);
			}
			foreach (MacroSaveData macroDatum in macroData)
			{
				macros[macroDatum.id].Load(macroDatum);
			}
		}
	}
}
