using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace Sizebox.CharacterEditor
{
	public class JiggleAdvancedView : BaseView
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Converter<DynamicBone, Transform> _003C_003E9__29_0;

			internal Transform _003CStyleJiggleHandles_003Eb__29_0(DynamicBone d)
			{
				return d.transform;
			}
		}

		[Header("References")]
		[SerializeField]
		private HandleManager handleManager;

		[SerializeField]
		private DynamicBoneConfigWindow jiggleConfigWindow;

		[Header("Controls")]
		[SerializeField]
		private Button createJiggleButton;

		[SerializeField]
		private Button removeJiggleButton;

		[SerializeField]
		private Button createExclusionButton;

		[SerializeField]
		private Button removeExclusionButton;

		[SerializeField]
		private Button configureJiggleButton;

		[Header("Visibility")]
		[SerializeField]
		private Slider handleSizeSlider;

		[Header("Hide Toggles")]
		[SerializeField]
		private Toggle hideHeadToggle;

		[SerializeField]
		private Toggle hideTorsoToggle;

		[SerializeField]
		private Toggle hideArmsToggle;

		[SerializeField]
		private Toggle hideLegsToggle;

		private HashSet<string> hiddenKeys = new HashSet<string>();

		private JiggleEdit jiggleEdit;

		protected override void Awake()
		{
			base.Awake();
			createJiggleButton.onClick.AddListener(CreateJiggle);
			removeJiggleButton.onClick.AddListener(RemoveJiggle);
			createExclusionButton.onClick.AddListener(CreateExclusion);
			removeExclusionButton.onClick.AddListener(RemoveExclusion);
			handleSizeSlider.onValueChanged.AddListener(OnHandleSize);
			hideHeadToggle.onValueChanged.AddListener(HideHeadBones);
			hideTorsoToggle.onValueChanged.AddListener(HideTorsoBones);
			hideArmsToggle.onValueChanged.AddListener(HideArmBones);
			hideLegsToggle.onValueChanged.AddListener(HideLegBones);
			configureJiggleButton.onClick.AddListener(OnConfigureJiggle);
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			jiggleEdit = handleManager.TargetEditor.JiggleEdit;
			ShowHandles();
			handleManager.SetHandleSize(handleSizeSlider.value);
			HandleManager obj = handleManager;
			obj.onHandleSelection = (HandleManager.OnHandleSelect)Delegate.Combine(obj.onHandleSelection, new HandleManager.OnHandleSelect(OnHandleSelect));
			HandleManager obj2 = handleManager;
			obj2.onHandleDeselection = (HandleManager.OnHandleSelect)Delegate.Combine(obj2.onHandleDeselection, new HandleManager.OnHandleSelect(OnHandleDeselect));
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			HandleManager obj = handleManager;
			obj.onHandleSelection = (HandleManager.OnHandleSelect)Delegate.Remove(obj.onHandleSelection, new HandleManager.OnHandleSelect(OnHandleSelect));
			HandleManager obj2 = handleManager;
			obj2.onHandleDeselection = (HandleManager.OnHandleSelect)Delegate.Remove(obj2.onHandleDeselection, new HandleManager.OnHandleSelect(OnHandleDeselect));
			handleManager.DisableHandlesMode();
		}

		private void CreateJiggle()
		{
			if (!handleManager.TargetHandle)
			{
				return;
			}
			foreach (SkeletonEditHandle targetHandle in handleManager.TargetHandles)
			{
				Transform realTransform = targetHandle.EditBone.RealTransform;
				jiggleEdit.CreateJiggleSource(realTransform);
			}
			ShowHandles();
		}

		private void RemoveJiggle()
		{
			if (!handleManager.TargetHandle)
			{
				return;
			}
			foreach (SkeletonEditHandle targetHandle in handleManager.TargetHandles)
			{
				Transform realTransform = targetHandle.EditBone.RealTransform;
				jiggleEdit.RemoveJiggleSource(realTransform);
			}
			ShowHandles();
		}

		private void CreateExclusion()
		{
			if (!handleManager.TargetHandle)
			{
				return;
			}
			foreach (SkeletonEditHandle targetHandle in handleManager.TargetHandles)
			{
				Transform realTransform = targetHandle.EditBone.RealTransform;
				jiggleEdit.CreateExclusion(realTransform);
			}
			ShowHandles();
		}

		private void RemoveExclusion()
		{
			if (!handleManager.TargetHandle)
			{
				return;
			}
			foreach (SkeletonEditHandle targetHandle in handleManager.TargetHandles)
			{
				Transform realTransform = targetHandle.EditBone.RealTransform;
				jiggleEdit.RemoveExclusion(realTransform);
			}
			ShowHandles();
		}

		private void OnConfigureJiggle()
		{
			List<SkeletonEditHandle> targetHandles = handleManager.TargetHandles;
			List<DynamicBone> list = new List<DynamicBone>();
			foreach (SkeletonEditHandle item in targetHandles)
			{
				DynamicBone component = item.EditBone.RealTransform.GetComponent<DynamicBone>();
				if ((bool)component)
				{
					list.Add(component);
				}
			}
			if (list.Count > 0)
			{
				jiggleConfigWindow.SetTargets(list);
				jiggleConfigWindow.gameObject.SetActive(true);
			}
		}

		private void OnHandleSelect(SkeletonEditHandle handle)
		{
			UpdateActiveButtons();
		}

		private void OnHandleDeselect(SkeletonEditHandle handle)
		{
			UpdateActiveButtons();
		}

		private void UpdateActiveButtons()
		{
			List<SkeletonEditHandle> targetHandles = handleManager.TargetHandles;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = targetHandles.Count == 1;
			foreach (SkeletonEditHandle item in targetHandles)
			{
				Transform realTransform = item.EditBone.RealTransform;
				flag = flag || jiggleEdit.IsJiggleSource(realTransform);
				flag2 = flag2 || jiggleEdit.IsExcludedBone(realTransform);
			}
			createJiggleButton.interactable = flag3 && !flag;
			removeJiggleButton.interactable = flag;
			createExclusionButton.interactable = targetHandles.Count > 0;
			removeExclusionButton.interactable = flag2;
			configureJiggleButton.interactable = flag;
		}

		private void OnHandleSize(float size)
		{
			handleManager.SetHandleSize(size);
		}

		private void HideKeys(ICollection<string> keys)
		{
			foreach (string key in keys)
			{
				if (!hiddenKeys.Contains(key))
				{
					hiddenKeys.Add(key);
				}
			}
		}

		private void UnhideKeys(ICollection<string> keys)
		{
			foreach (string key in keys)
			{
				if (hiddenKeys.Contains(key))
				{
					hiddenKeys.Remove(key);
				}
			}
		}

		private void ShowHandles()
		{
			if ((bool)handleManager.TargetEditor)
			{
				handleManager.DisableHandlesMode();
				List<string> keys = new List<string>(handleManager.TargetEditor.SkeletonEdit.BoneMap.Keys);
				handleManager.EnableHandlesMode(null, keys);
				handleManager.SetHandleStyle(SkeletonEditHandle.HandleStyle.Inactive, keys);
				StyleJiggleHandles();
				handleManager.DisableHandles(hiddenKeys);
				UpdateActiveButtons();
			}
		}

		private void StyleJiggleHandles()
		{
			List<DynamicBone> jiggleSources = jiggleEdit.GetJiggleSources();
			HashSet<Transform> allSourceTransforms = new HashSet<Transform>(jiggleSources.ConvertAll(_003C_003Ec._003C_003E9__29_0 ?? (_003C_003Ec._003C_003E9__29_0 = _003C_003Ec._003C_003E9._003CStyleJiggleHandles_003Eb__29_0)));
			foreach (DynamicBone item in jiggleSources)
			{
				handleManager.SetHandleStyle(SkeletonEditHandle.HandleStyle.JiggleSource, item.transform);
				foreach (Transform item2 in item.transform)
				{
					StyleChildBones(item2, item, allSourceTransforms);
				}
			}
		}

		private void StyleChildBones(Transform transform, DynamicBone source, HashSet<Transform> allSourceTransforms)
		{
			if (allSourceTransforms.Contains(transform))
			{
				return;
			}
			if (source.m_Exclusions.Contains(transform))
			{
				handleManager.SetHandleStyle(SkeletonEditHandle.HandleStyle.JiggleExclude, transform);
				return;
			}
			handleManager.SetHandleStyle(SkeletonEditHandle.HandleStyle.Active, transform);
			foreach (Transform item in transform)
			{
				StyleChildBones(item, source, allSourceTransforms);
			}
		}

		private void HideHeadBones(bool hideBones)
		{
			Humanoid component = handleManager.TargetEditor.GetComponent<Humanoid>();
			if ((bool)component)
			{
				Transform boneTransform = component.Animator.GetBoneTransform(HumanBodyBones.Head);
				HashSet<string> keys = FindChildBoneKeys(boneTransform);
				if (hideBones)
				{
					HideKeys(keys);
				}
				else
				{
					UnhideKeys(keys);
				}
				ShowHandles();
			}
		}

		private void HideArmBones(bool hideBones)
		{
			Humanoid component = handleManager.TargetEditor.GetComponent<Humanoid>();
			if ((bool)component)
			{
				Transform boneTransform = component.Animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
				Transform boneTransform2 = component.Animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
				HashSet<string> keys = FindChildBoneKeys(boneTransform);
				HashSet<string> keys2 = FindChildBoneKeys(boneTransform2);
				if (hideBones)
				{
					HideKeys(keys);
					HideKeys(keys2);
				}
				else
				{
					UnhideKeys(keys);
					UnhideKeys(keys2);
				}
				ShowHandles();
			}
		}

		private void HideLegBones(bool hideBones)
		{
			Humanoid component = handleManager.TargetEditor.GetComponent<Humanoid>();
			if ((bool)component)
			{
				Transform boneTransform = component.Animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
				Transform boneTransform2 = component.Animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
				HashSet<string> keys = FindChildBoneKeys(boneTransform);
				HashSet<string> keys2 = FindChildBoneKeys(boneTransform2);
				if (hideBones)
				{
					HideKeys(keys);
					HideKeys(keys2);
				}
				else
				{
					UnhideKeys(keys);
					UnhideKeys(keys2);
				}
				ShowHandles();
			}
		}

		private void HideTorsoBones(bool hideBones)
		{
			Humanoid component = handleManager.TargetEditor.GetComponent<Humanoid>();
			if ((bool)component)
			{
				Animator animator = component.Animator;
				List<Transform> list = new List<Transform>();
				Transform boneTransform = animator.GetBoneTransform(HumanBodyBones.Head);
				if ((bool)boneTransform)
				{
					list.Add(boneTransform);
				}
				boneTransform = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
				if ((bool)boneTransform)
				{
					list.Add(boneTransform);
				}
				boneTransform = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
				if ((bool)boneTransform)
				{
					list.Add(boneTransform);
				}
				boneTransform = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
				if ((bool)boneTransform)
				{
					list.Add(boneTransform);
				}
				boneTransform = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
				if ((bool)boneTransform)
				{
					list.Add(boneTransform);
				}
				HashSet<string> keys = FindChildBoneKeys(component.transform, list);
				if (hideBones)
				{
					HideKeys(keys);
				}
				else
				{
					UnhideKeys(keys);
				}
				ShowHandles();
			}
		}

		private HashSet<string> FindChildBoneKeys(Transform inTransform, List<Transform> stopPoints = null)
		{
			if (!inTransform)
			{
				return null;
			}
			if (stopPoints == null)
			{
				stopPoints = new List<Transform>();
			}
			HashSet<string> hashSet = new HashSet<string>();
			EditBone[] componentsInChildren = inTransform.GetComponentsInChildren<EditBone>();
			List<EditBone> list = new List<EditBone>();
			EditBone[] array = componentsInChildren;
			foreach (EditBone editBone in array)
			{
				if (stopPoints.Contains(editBone.RealTransform))
				{
					list.Add(editBone);
				}
				if (handleManager.TargetEditor.SkeletonEdit.KeyMap.ContainsKey(editBone))
				{
					hashSet.Add(editBone.Key);
				}
			}
			EditBone componentInParent = inTransform.GetComponentInParent<EditBone>();
			if ((bool)componentInParent && componentInParent.RealTransform == inTransform)
			{
				hashSet.Add(componentInParent.Key);
			}
			foreach (EditBone item in list)
			{
				List<EditBone> list2 = new List<EditBone>();
				item.GetComponentsInChildren(list2);
				foreach (EditBone item2 in list2)
				{
					hashSet.Remove(item2.Key);
				}
			}
			return hashSet;
		}
	}
}
