using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace Sizebox.CharacterEditor
{
	public class ModelConfigView : BaseView
	{
		[SerializeField]
		private HandleManager handleManager;

		[Header("View References")]
		[SerializeField]
		private BodyView bodyView;

		[SerializeField]
		private HairView hairView;

		[SerializeField]
		private ExtraView extraView;

		[Header("Model Config")]
		[SerializeField]
		private Button pairBoneButton;

		[Header("View Config")]
		[SerializeField]
		private Button bodyViewButton;

		[SerializeField]
		private Button hairViewButton;

		[SerializeField]
		private Button extraViewButton;

		[SerializeField]
		private Button visibilityButton;

		[Header("Visibility")]
		[SerializeField]
		private Button showHiddenButton;

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

		[Header("Save Settings")]
		[SerializeField]
		private Button saveButton;

		[SerializeField]
		private Button discardButton;

		private ViewName currentView;

		private CharacterEditor previousTarget;

		private bool configIsDirty;

		private HashSet<string> hiddenKeys = new HashSet<string>();

		private bool showingHidden;

		protected override void Awake()
		{
			base.Awake();
			pairBoneButton.onClick.AddListener(OnPairBones);
			showHiddenButton.onClick.AddListener(OnShowHidden);
			visibilityButton.onClick.AddListener(OnVisibility);
			bodyViewButton.onClick.AddListener(_003CAwake_003Eb__21_0);
			hairViewButton.onClick.AddListener(_003CAwake_003Eb__21_1);
			extraViewButton.onClick.AddListener(_003CAwake_003Eb__21_2);
			handleSizeSlider.onValueChanged.AddListener(OnHandleSize);
			OnViewSelect(ViewName.Body);
			hideHeadToggle.onValueChanged.AddListener(HideHeadBones);
			hideTorsoToggle.onValueChanged.AddListener(HideTorsoBones);
			hideArmsToggle.onValueChanged.AddListener(HideArmBones);
			hideLegsToggle.onValueChanged.AddListener(HideLegBones);
			saveButton.onClick.AddListener(OnSave);
			discardButton.onClick.AddListener(OnDiscard);
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (previousTarget != handleManager.TargetEditor)
			{
				previousTarget = handleManager.TargetEditor;
				SetConfigDirty(false);
			}
			UpdateKeys();
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
			handleManager.DisableHandlesMode();
			HandleManager obj = handleManager;
			obj.onHandleSelection = (HandleManager.OnHandleSelect)Delegate.Remove(obj.onHandleSelection, new HandleManager.OnHandleSelect(OnHandleSelect));
			HandleManager obj2 = handleManager;
			obj2.onHandleDeselection = (HandleManager.OnHandleSelect)Delegate.Remove(obj2.onHandleDeselection, new HandleManager.OnHandleSelect(OnHandleDeselect));
		}

		private void Update()
		{
			ModelConfigGui();
		}

		private void ModelConfigGui()
		{
			if (handleManager.TargetHandles.Count == 2)
			{
				pairBoneButton.interactable = true;
			}
			else
			{
				pairBoneButton.interactable = false;
			}
		}

		private void UpdateKeys()
		{
			if (!configIsDirty)
			{
				bodyView.UpdateKeys();
				hairView.UpdateKeys();
				extraView.UpdateKeys();
			}
		}

		private List<string> GetKeys(ViewName view)
		{
			switch (view)
			{
			case ViewName.Body:
				return bodyView.Keys;
			case ViewName.Hair:
				return hairView.Keys;
			case ViewName.Extra:
				return extraView.Keys;
			default:
				return null;
			}
		}

		private void SetConfigDirty(bool value)
		{
			configIsDirty = value;
			if (configIsDirty)
			{
				saveButton.interactable = true;
				discardButton.interactable = true;
			}
			else
			{
				saveButton.interactable = false;
				discardButton.interactable = false;
			}
		}

		private void ShowHandles()
		{
			if ((bool)handleManager.TargetEditor)
			{
				handleManager.DisableHandlesMode();
				if (showingHidden)
				{
					List<string> keys = new List<string>(handleManager.TargetEditor.SkeletonEdit.BoneMap.Keys);
					List<string> keys2 = GetKeys(currentView);
					handleManager.EnableHandlesMode(null, keys);
					handleManager.SetHandleStyle(SkeletonEditHandle.HandleStyle.Active, keys2, true);
				}
				else
				{
					List<string> keys3 = GetKeys(currentView);
					handleManager.EnableHandlesMode(null, keys3);
					handleManager.SetHandleStyle(SkeletonEditHandle.HandleStyle.Active, keys3);
				}
				handleManager.DisableHandles(hiddenKeys);
			}
		}

		private void OnViewSelect(ViewName view)
		{
			currentView = view;
			bodyViewButton.interactable = true;
			hairViewButton.interactable = true;
			extraViewButton.interactable = true;
			switch (view)
			{
			default:
				return;
			case ViewName.Body:
				bodyViewButton.interactable = false;
				break;
			case ViewName.Hair:
				hairViewButton.interactable = false;
				break;
			case ViewName.Extra:
				extraViewButton.interactable = false;
				break;
			}
			ShowHandles();
		}

		private void OnHandleSelect(SkeletonEditHandle handle)
		{
			if ((bool)handle.EditBone)
			{
				EditBone siblingBone = handle.EditBone.SiblingBone;
				if ((bool)siblingBone && handleManager.HandleKeyMap.ContainsKey(siblingBone.Key))
				{
					handleManager.HandleKeyMap[siblingBone.Key].ShowPaired = true;
				}
			}
		}

		private void OnHandleDeselect(SkeletonEditHandle handle)
		{
			if ((bool)handle.EditBone)
			{
				EditBone siblingBone = handle.EditBone.SiblingBone;
				if ((bool)siblingBone && handleManager.HandleKeyMap.ContainsKey(siblingBone.Key))
				{
					handleManager.HandleKeyMap[siblingBone.Key].ShowPaired = false;
				}
			}
		}

		private void OnHandleSize(float size)
		{
			handleManager.SetHandleSize(size);
		}

		private void OnPairBones()
		{
			handleManager.PairBones();
			SetConfigDirty(true);
		}

		private void OnVisibility()
		{
			foreach (SkeletonEditHandle targetHandle in handleManager.TargetHandles)
			{
				string item = handleManager.KeyMap[targetHandle];
				List<string> keys;
				if (currentView == ViewName.Body)
				{
					keys = bodyView.Keys;
				}
				else if (currentView == ViewName.Hair)
				{
					keys = hairView.Keys;
				}
				else
				{
					if (currentView != ViewName.Extra)
					{
						continue;
					}
					keys = extraView.Keys;
				}
				if (keys.Contains(item))
				{
					keys.Remove(item);
				}
				else
				{
					keys.Add(item);
				}
			}
			ShowHandles();
			SetConfigDirty(true);
		}

		private void OnShowHidden()
		{
			showingHidden = !showingHidden;
			ShowHandles();
		}

		private void OnSave()
		{
			if ((bool)handleManager.TargetEditor)
			{
				handleManager.TargetEditor.SkeletonEdit.SaveConfig();
				bodyView.SaveKeys(handleManager.TargetEditor);
				hairView.SaveKeys(handleManager.TargetEditor);
				extraView.SaveKeys(handleManager.TargetEditor);
				SetConfigDirty(false);
			}
		}

		private void OnDiscard()
		{
			if ((bool)handleManager.TargetEditor)
			{
				handleManager.TargetEditor.SkeletonEdit.LoadConfigDataFromFile();
				bodyView.UpdateKeys();
				hairView.UpdateKeys();
				extraView.UpdateKeys();
				ShowHandles();
				SetConfigDirty(false);
			}
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

		[CompilerGenerated]
		private void _003CAwake_003Eb__21_0()
		{
			OnViewSelect(ViewName.Body);
		}

		[CompilerGenerated]
		private void _003CAwake_003Eb__21_1()
		{
			OnViewSelect(ViewName.Hair);
		}

		[CompilerGenerated]
		private void _003CAwake_003Eb__21_2()
		{
			OnViewSelect(ViewName.Extra);
		}
	}
}
