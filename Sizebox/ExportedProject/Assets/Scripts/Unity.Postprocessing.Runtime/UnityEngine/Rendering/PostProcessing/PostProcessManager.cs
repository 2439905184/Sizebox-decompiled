using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace UnityEngine.Rendering.PostProcessing
{
	public sealed class PostProcessManager
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<Type, bool> _003C_003E9__12_0;

			internal bool _003CReloadBaseTypes_003Eb__12_0(Type t)
			{
				if (t.IsDefined(typeof(PostProcessAttribute), false))
				{
					return !t.IsAbstract;
				}
				return false;
			}
		}

		private static PostProcessManager s_Instance;

		private const int k_MaxLayerCount = 32;

		private readonly Dictionary<int, List<PostProcessVolume>> m_SortedVolumes;

		private readonly List<PostProcessVolume> m_Volumes;

		private readonly Dictionary<int, bool> m_SortNeeded;

		private readonly List<PostProcessEffectSettings> m_BaseSettings;

		private readonly List<Collider> m_TempColliders;

		public readonly Dictionary<Type, PostProcessAttribute> settingsTypes;

		public static PostProcessManager instance
		{
			get
			{
				if (s_Instance == null)
				{
					s_Instance = new PostProcessManager();
				}
				return s_Instance;
			}
		}

		private PostProcessManager()
		{
			m_SortedVolumes = new Dictionary<int, List<PostProcessVolume>>();
			m_Volumes = new List<PostProcessVolume>();
			m_SortNeeded = new Dictionary<int, bool>();
			m_BaseSettings = new List<PostProcessEffectSettings>();
			m_TempColliders = new List<Collider>(5);
			settingsTypes = new Dictionary<Type, PostProcessAttribute>();
			ReloadBaseTypes();
		}

		private void CleanBaseTypes()
		{
			settingsTypes.Clear();
			foreach (PostProcessEffectSettings baseSetting in m_BaseSettings)
			{
				RuntimeUtilities.Destroy(baseSetting);
			}
			m_BaseSettings.Clear();
		}

		private void ReloadBaseTypes()
		{
			CleanBaseTypes();
			foreach (Type item in RuntimeUtilities.GetAllTypesDerivedFrom<PostProcessEffectSettings>().Where(_003C_003Ec._003C_003E9__12_0 ?? (_003C_003Ec._003C_003E9__12_0 = _003C_003Ec._003C_003E9._003CReloadBaseTypes_003Eb__12_0)))
			{
				settingsTypes.Add(item, item.GetAttribute<PostProcessAttribute>());
				PostProcessEffectSettings postProcessEffectSettings = (PostProcessEffectSettings)ScriptableObject.CreateInstance(item);
				postProcessEffectSettings.SetAllOverridesTo(true, false);
				m_BaseSettings.Add(postProcessEffectSettings);
			}
		}

		public void GetActiveVolumes(PostProcessLayer layer, List<PostProcessVolume> results, bool skipDisabled = true, bool skipZeroWeight = true)
		{
			int value = layer.volumeLayer.value;
			Transform volumeTrigger = layer.volumeTrigger;
			bool flag = volumeTrigger == null;
			Vector3 vector = (flag ? Vector3.zero : volumeTrigger.position);
			foreach (PostProcessVolume item in GrabVolumes(value))
			{
				if ((skipDisabled && !item.enabled) || item.profileRef == null || (skipZeroWeight && item.weight <= 0f))
				{
					continue;
				}
				if (item.isGlobal)
				{
					results.Add(item);
				}
				else
				{
					if (flag)
					{
						continue;
					}
					List<Collider> tempColliders = m_TempColliders;
					item.GetComponents(tempColliders);
					if (tempColliders.Count == 0)
					{
						continue;
					}
					float num = float.PositiveInfinity;
					foreach (Collider item2 in tempColliders)
					{
						if (item2.enabled)
						{
							float sqrMagnitude = ((item2.ClosestPoint(vector) - vector) / 2f).sqrMagnitude;
							if (sqrMagnitude < num)
							{
								num = sqrMagnitude;
							}
						}
					}
					tempColliders.Clear();
					float num2 = item.blendDistance * item.blendDistance;
					if (num <= num2)
					{
						results.Add(item);
					}
				}
			}
		}

		public PostProcessVolume GetHighestPriorityVolume(PostProcessLayer layer)
		{
			if (layer == null)
			{
				throw new ArgumentNullException("layer");
			}
			return GetHighestPriorityVolume(layer.volumeLayer);
		}

		public PostProcessVolume GetHighestPriorityVolume(LayerMask mask)
		{
			float num = float.NegativeInfinity;
			PostProcessVolume result = null;
			List<PostProcessVolume> value;
			if (m_SortedVolumes.TryGetValue(mask, out value))
			{
				foreach (PostProcessVolume item in value)
				{
					if (item.priority > num)
					{
						num = item.priority;
						result = item;
					}
				}
				return result;
			}
			return result;
		}

		public PostProcessVolume QuickVolume(int layer, float priority, params PostProcessEffectSettings[] settings)
		{
			PostProcessVolume postProcessVolume = new GameObject
			{
				name = "Quick Volume",
				layer = layer,
				hideFlags = HideFlags.HideAndDontSave
			}.AddComponent<PostProcessVolume>();
			postProcessVolume.priority = priority;
			postProcessVolume.isGlobal = true;
			PostProcessProfile profile = postProcessVolume.profile;
			foreach (PostProcessEffectSettings effect in settings)
			{
				profile.AddSettings(effect);
			}
			return postProcessVolume;
		}

		internal void SetLayerDirty(int layer)
		{
			foreach (KeyValuePair<int, List<PostProcessVolume>> sortedVolume in m_SortedVolumes)
			{
				int key = sortedVolume.Key;
				if ((key & (1 << layer)) != 0)
				{
					m_SortNeeded[key] = true;
				}
			}
		}

		internal void UpdateVolumeLayer(PostProcessVolume volume, int prevLayer, int newLayer)
		{
			Unregister(volume, prevLayer);
			Register(volume, newLayer);
		}

		private void Register(PostProcessVolume volume, int layer)
		{
			m_Volumes.Add(volume);
			foreach (KeyValuePair<int, List<PostProcessVolume>> sortedVolume in m_SortedVolumes)
			{
				if ((sortedVolume.Key & (1 << layer)) != 0)
				{
					sortedVolume.Value.Add(volume);
				}
			}
			SetLayerDirty(layer);
		}

		internal void Register(PostProcessVolume volume)
		{
			int layer = volume.gameObject.layer;
			Register(volume, layer);
		}

		private void Unregister(PostProcessVolume volume, int layer)
		{
			m_Volumes.Remove(volume);
			foreach (KeyValuePair<int, List<PostProcessVolume>> sortedVolume in m_SortedVolumes)
			{
				if ((sortedVolume.Key & (1 << layer)) != 0)
				{
					sortedVolume.Value.Remove(volume);
				}
			}
		}

		internal void Unregister(PostProcessVolume volume)
		{
			int layer = volume.gameObject.layer;
			Unregister(volume, layer);
		}

		private void ReplaceData(PostProcessLayer postProcessLayer)
		{
			foreach (PostProcessEffectSettings baseSetting in m_BaseSettings)
			{
				PostProcessEffectSettings settings = postProcessLayer.GetBundle(baseSetting.GetType()).settings;
				int count = baseSetting.parameters.Count;
				for (int i = 0; i < count; i++)
				{
					settings.parameters[i].SetValue(baseSetting.parameters[i]);
				}
			}
		}

		internal void UpdateSettings(PostProcessLayer postProcessLayer, Camera camera)
		{
			ReplaceData(postProcessLayer);
			int value = postProcessLayer.volumeLayer.value;
			Transform volumeTrigger = postProcessLayer.volumeTrigger;
			bool flag = volumeTrigger == null;
			Vector3 vector = (flag ? Vector3.zero : volumeTrigger.position);
			foreach (PostProcessVolume item in GrabVolumes(value))
			{
				if (!item.enabled || item.profileRef == null || item.weight <= 0f)
				{
					continue;
				}
				List<PostProcessEffectSettings> settings = item.profileRef.settings;
				if (item.isGlobal)
				{
					postProcessLayer.OverrideSettings(settings, Mathf.Clamp01(item.weight));
				}
				else
				{
					if (flag)
					{
						continue;
					}
					List<Collider> tempColliders = m_TempColliders;
					item.GetComponents(tempColliders);
					if (tempColliders.Count == 0)
					{
						continue;
					}
					float num = float.PositiveInfinity;
					foreach (Collider item2 in tempColliders)
					{
						if (item2.enabled)
						{
							float sqrMagnitude = ((item2.ClosestPoint(vector) - vector) / 2f).sqrMagnitude;
							if (sqrMagnitude < num)
							{
								num = sqrMagnitude;
							}
						}
					}
					tempColliders.Clear();
					float num2 = item.blendDistance * item.blendDistance;
					if (!(num > num2))
					{
						float num3 = 1f;
						if (num2 > 0f)
						{
							num3 = 1f - num / num2;
						}
						postProcessLayer.OverrideSettings(settings, num3 * Mathf.Clamp01(item.weight));
					}
				}
			}
		}

		private List<PostProcessVolume> GrabVolumes(LayerMask mask)
		{
			List<PostProcessVolume> value;
			if (!m_SortedVolumes.TryGetValue(mask, out value))
			{
				value = new List<PostProcessVolume>();
				foreach (PostProcessVolume volume in m_Volumes)
				{
					if (((int)mask & (1 << volume.gameObject.layer)) != 0)
					{
						value.Add(volume);
						m_SortNeeded[mask] = true;
					}
				}
				m_SortedVolumes.Add(mask, value);
			}
			bool value2;
			if (m_SortNeeded.TryGetValue(mask, out value2) && value2)
			{
				m_SortNeeded[mask] = false;
				SortByPriority(value);
			}
			return value;
		}

		private static void SortByPriority(List<PostProcessVolume> volumes)
		{
			for (int i = 1; i < volumes.Count; i++)
			{
				PostProcessVolume postProcessVolume = volumes[i];
				int num = i - 1;
				while (num >= 0 && volumes[num].priority > postProcessVolume.priority)
				{
					volumes[num + 1] = volumes[num];
					num--;
				}
				volumes[num + 1] = postProcessVolume;
			}
		}

		private static bool IsVolumeRenderedByCamera(PostProcessVolume volume, Camera camera)
		{
			return true;
		}
	}
}
