using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Sizebox.CharacterEditor
{
	[RequireComponent(typeof(CharacterEditor))]
	public class JiggleEdit : MonoBehaviour
	{
		private CharacterEditor characterEdit;

		private List<DynamicBone> jiggleSources;

		private HashSet<Transform> excludedBones;

		private void Awake()
		{
			characterEdit = GetComponent<CharacterEditor>();
			jiggleSources = new List<DynamicBone>(GetComponentsInChildren<DynamicBone>());
			excludedBones = new HashSet<Transform>();
			foreach (DynamicBone jiggleSource in jiggleSources)
			{
				excludedBones.UnionWith(jiggleSource.m_Exclusions);
			}
		}

		public void LoadPreset(JigglePresetData data)
		{
			ClearAllJiggleSources();
			ErrorCollector errorCollector = new ErrorCollector();
			foreach (DynamicBoneData jiggleBone in data.jiggleBones)
			{
				LoadBoneData(jiggleBone, errorCollector);
			}
			if (errorCollector.Count > 0)
			{
				Debug.LogError(errorCollector.Count + " jiggle sources failed to load correctly.");
			}
		}

		public JigglePresetData SavePreset()
		{
			return new JigglePresetData(characterEdit);
		}

		private void LoadBoneData(DynamicBoneData data, ErrorCollector errors)
		{
			try
			{
				Transform targetBone = characterEdit.GetTransform(data.boneKey);
				DynamicBone dynamicBone = CreateJiggleSource(targetBone);
				DynamicBoneSettings settings = data.settings;
				dynamicBone.m_Damping = settings.dampening;
				dynamicBone.m_Stiffness = settings.stiffness;
				dynamicBone.m_Elasticity = settings.elasticity;
				dynamicBone.m_Inert = settings.inertia;
				dynamicBone.UpdateParameters();
				HashSet<Transform> other = (dynamicBone.m_Exclusions = new HashSet<Transform>(data.exclusions.ConvertAll<Transform>(_003CLoadBoneData_003Eb__6_0)));
				excludedBones.UnionWith(other);
				dynamicBone.ResetParticles();
			}
			catch (Exception e)
			{
				errors.Add(e);
			}
		}

		public DynamicBone CreateJiggleSource(Transform targetBone)
		{
			if (!characterEdit.Transforms.Contains(targetBone) || (bool)targetBone.GetComponent<DynamicBone>())
			{
				return null;
			}
			DynamicBone dynamicBone = targetBone.gameObject.AddComponent<DynamicBone>();
			dynamicBone.m_Root = targetBone;
			dynamicBone.m_UpdateRate = 60f;
			dynamicBone.m_Damping = 0.2f;
			dynamicBone.m_Elasticity = 0.1f;
			dynamicBone.m_Stiffness = 0.1f;
			dynamicBone.m_Inert = 0.1f;
			dynamicBone.m_Radius = 0f;
			dynamicBone.m_EndLength = 5f;
			dynamicBone.m_EndOffset = new Vector3(-0.1f, -0.1f, 0.3f);
			dynamicBone.m_Gravity = new Vector3(0f, 0f, 0f);
			dynamicBone.m_DistanceToObject = 20f;
			dynamicBone.m_DistantDisable = false;
			dynamicBone.m_Exclusions = new HashSet<Transform>();
			dynamicBone.m_Colliders = new List<DynamicBoneColliderBase>();
			dynamicBone.UpdateParameters();
			jiggleSources.Add(dynamicBone);
			return dynamicBone;
		}

		public void RemoveJiggleSource(Transform targetBone)
		{
			if (characterEdit.Transforms.Contains(targetBone))
			{
				DynamicBone component = targetBone.GetComponent<DynamicBone>();
				jiggleSources.Remove(component);
				excludedBones.ExceptWith(component.m_Exclusions);
				UnityEngine.Object.DestroyImmediate(component);
			}
		}

		public void CreateExclusion(Transform targetBone)
		{
			if (characterEdit.Transforms.Contains(targetBone))
			{
				DynamicBone componentInParent = targetBone.GetComponentInParent<DynamicBone>();
				if ((bool)componentInParent && componentInParent.transform == targetBone)
				{
					componentInParent = targetBone.parent.GetComponentInParent<DynamicBone>();
				}
				if ((bool)componentInParent)
				{
					componentInParent.m_Exclusions.Add(targetBone);
					excludedBones.Add(targetBone);
					componentInParent.ResetParticles();
				}
			}
		}

		public void RemoveExclusion(Transform targetBone)
		{
			if (characterEdit.Transforms.Contains(targetBone))
			{
				DynamicBone componentInParent = targetBone.GetComponentInParent<DynamicBone>();
				if ((bool)componentInParent)
				{
					componentInParent.m_Exclusions.Remove(targetBone);
					excludedBones.Remove(targetBone);
					componentInParent.ResetParticles();
				}
			}
		}

		private void ClearAllJiggleSources()
		{
			for (int num = jiggleSources.Count - 1; num >= 0; num--)
			{
				RemoveJiggleSource(jiggleSources[num].transform);
			}
		}

		public List<DynamicBone> GetJiggleSources()
		{
			return jiggleSources;
		}

		public HashSet<Transform> GetExcludedBones()
		{
			return excludedBones;
		}

		public bool IsJiggleSource(Transform bone)
		{
			return bone.GetComponent<DynamicBone>();
		}

		public bool IsExcludedBone(Transform bone)
		{
			return excludedBones.Contains(bone);
		}

		public void Load(JiggleEditSaveData data)
		{
			if (data.presetData.jiggleBones.Count > 0)
			{
				LoadPreset(data.presetData);
			}
		}

		public JiggleEditSaveData Save()
		{
			return new JiggleEditSaveData
			{
				presetData = SavePreset()
			};
		}

		[CompilerGenerated]
		private Transform _003CLoadBoneData_003Eb__6_0(string key)
		{
			return characterEdit.GetTransform(key);
		}
	}
}
