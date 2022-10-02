using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RuntimeGizmos;
using SizeboxUI;
using UnityEngine;
using UnityEngine.UI;

namespace Sizebox.CharacterEditor
{
	public abstract class BaseAdvancedView : BaseView
	{
		[Space(15f)]
		[SerializeField]
		protected HandleManager handleManager;

		[SerializeField]
		protected BaseSkeletonHandleView handleView;

		[SerializeField]
		protected SkeletonEditOptionsGui optionsGui;

		[Header("Visibility Options")]
		[SerializeField]
		protected Button showAllButton;

		[SerializeField]
		protected Slider handleSizeSlider;

		[Header("Reset Buttons")]
		[SerializeField]
		protected Button resetBoneButton;

		[SerializeField]
		protected Button resetButton;

		[SerializeField]
		protected Button resetAllButton;

		[Header("Hide Toggles")]
		[SerializeField]
		protected Toggle hideHeadToggle;

		[SerializeField]
		protected Toggle hideTorsoToggle;

		[SerializeField]
		protected Toggle hideArmsToggle;

		[SerializeField]
		protected Toggle hideLegsToggle;

		protected bool showingAll;

		protected List<string> hiddenKeys;

		protected TransformGizmo gizmo;

		protected override void Awake()
		{
			base.Awake();
			gizmo = GuiManager.Instance.TransformGizmo;
			hiddenKeys = new List<string>();
			showAllButton.onClick.AddListener(OnShowAll);
			handleSizeSlider.onValueChanged.AddListener(OnHandleSize);
			resetAllButton.onClick.AddListener(_003CAwake_003Eb__15_0);
			resetBoneButton.onClick.AddListener(_003CAwake_003Eb__15_1);
			resetButton.onClick.AddListener(_003CAwake_003Eb__15_2);
			hideHeadToggle.onValueChanged.AddListener(HideHeadBones);
			hideTorsoToggle.onValueChanged.AddListener(HideTorsoBones);
			hideArmsToggle.onValueChanged.AddListener(HideArmBones);
			hideLegsToggle.onValueChanged.AddListener(HideLegBones);
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			showingAll = false;
			handleManager.SetOptions(optionsGui.Options);
			handleManager.SetHandleSize(handleSizeSlider.value);
			ShowHandles();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			handleManager.DisableHandlesMode();
		}

		private void Update()
		{
			UpdateResetButtonText();
		}

		private void UpdateResetButtonText()
		{
			Text componentInChildren = resetButton.GetComponentInChildren<Text>();
			switch (gizmo.type)
			{
			case TransformType.Move:
				componentInChildren.text = "Position";
				break;
			case TransformType.Rotate:
				componentInChildren.text = "Rotation";
				break;
			case TransformType.Scale:
				componentInChildren.text = "Scale";
				break;
			}
		}

		protected void ShowHandles(List<string> keys = null)
		{
			if (keys == null)
			{
				keys = (showingAll ? new List<string>(handleManager.TargetEditor.SkeletonEdit.BoneMap.Keys) : handleView.Keys);
			}
			handleManager.DisableHandlesMode();
			handleManager.EnableHandlesMode(handleView.DataId, keys);
			if (!showingAll)
			{
				handleManager.SetHandleStyle(SkeletonEditHandle.HandleStyle.Active, keys);
			}
			else
			{
				handleManager.SetHandleStyle(SkeletonEditHandle.HandleStyle.Active, handleView.Keys, true);
			}
			handleManager.DisableHandles(hiddenKeys);
		}

		private void OnShowAll()
		{
			showingAll = !showingAll;
			ShowHandles();
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
		private void _003CAwake_003Eb__15_0()
		{
			handleManager.ResetAll(handleView.DataId);
		}

		[CompilerGenerated]
		private void _003CAwake_003Eb__15_1()
		{
			handleManager.ResetTarget(handleView.DataId, optionsGui.Options);
		}

		[CompilerGenerated]
		private void _003CAwake_003Eb__15_2()
		{
			handleManager.Reset(handleView.DataId, optionsGui.Options);
		}
	}
}
