using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sizebox.CharacterEditor
{
	public class SkeletonEditSlider : MonoBehaviour, IPointerUpHandler, IEventSystemHandler
	{
		private enum BoneLengthCallType
		{
			First = 0,
			Sibling = 1,
			Child = 2
		}

		private const float GIANTESS_MULT = 1000f;

		[Header("Slider Components")]
		[SerializeField]
		private Slider slider;

		[SerializeField]
		private Image sliderBarFront;

		[SerializeField]
		private Image sliderBarBackground;

		[Header("Colors")]
		[SerializeField]
		private Color someMissingColor = Color.yellow;

		[SerializeField]
		private Color normalColor = Color.white;

		[SerializeField]
		private Color disabledColor = Color.gray;

		private SkeletonEdit skeletonEdit;

		private float previousValue = 0.5f;

		private SkeletonEditSliderObject sliderData;

		private string Id;

		private void Awake()
		{
			slider.onValueChanged.AddListener(OnSliderMove);
		}

		public void Initialize(string id, SkeletonEditSliderObject sliderObj)
		{
			Id = id;
			sliderData = sliderObj;
		}

		public void RegisterSkeleton(SkeletonEdit skeletonEdit)
		{
			this.skeletonEdit = skeletonEdit;
			int num = 0;
			SkeletonEditSliderData[] transformations = sliderData.Transformations;
			for (int i = 0; i < transformations.Length; i++)
			{
				SkeletonEditSliderData skeletonEditSliderData = transformations[i];
				if ((bool)skeletonEdit.GetBone(skeletonEditSliderData.bone))
				{
					num++;
				}
			}
			slider.interactable = num != 0;
			if (num == 0)
			{
				ColorBlock colors = slider.colors;
				colors.normalColor = disabledColor;
				slider.colors = colors;
				sliderBarFront.color = disabledColor;
				sliderBarBackground.color = disabledColor;
			}
			else if (num == sliderData.Transformations.Length)
			{
				ColorBlock colors2 = slider.colors;
				colors2.normalColor = normalColor;
				slider.colors = colors2;
				sliderBarFront.color = normalColor;
				sliderBarBackground.color = normalColor;
			}
			else
			{
				ColorBlock colors3 = slider.colors;
				colors3.normalColor = someMissingColor;
				slider.colors = colors3;
				sliderBarFront.color = someMissingColor;
				sliderBarBackground.color = someMissingColor;
			}
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			slider.onValueChanged.RemoveListener(OnSliderMove);
			slider.value = 0.5f;
			previousValue = 0.5f;
			slider.onValueChanged.AddListener(OnSliderMove);
		}

		private void OnSliderMove(float value)
		{
			if ((bool)skeletonEdit && (bool)sliderData)
			{
				ApplySlider(value - previousValue);
				previousValue = value;
			}
		}

		private void ApplySlider(float change)
		{
			Humanoid component = skeletonEdit.GetComponent<Humanoid>();
			SkeletonEditSliderData[] transformations = sliderData.Transformations;
			for (int i = 0; i < transformations.Length; i++)
			{
				SkeletonEditSliderData transformation = transformations[i];
				EditBone bone = skeletonEdit.GetBone(transformation.bone);
				if ((bool)bone)
				{
					HandleHipHeight(bone, transformation, component, change);
					HandleRotation(bone, transformation, component, change);
					HandleScaling(bone, transformation, component, change);
					HandleBoneLength(bone, transformation, component, change);
					HandleMovement(bone, transformation, component, change);
				}
			}
		}

		private void HandleMovement(EditBone bone, SkeletonEditSliderData transformation, Humanoid target, float change)
		{
			Vector3 movement = transformation.movement * change;
			if (target.isGiantess)
			{
				movement *= 1000f;
			}
			bone.Move(Id, movement, transformation.options.movement);
		}

		private void HandleRotation(EditBone bone, SkeletonEditSliderData transformation, Humanoid target, float change)
		{
			Quaternion rotation = Quaternion.SlerpUnclamped(Quaternion.identity, Quaternion.Euler(transformation.rotation), change);
			bone.Rotate(Id, rotation, transformation.options.rotation);
		}

		private void HandleScaling(EditBone bone, SkeletonEditSliderData transformation, Humanoid target, float change)
		{
			Vector3 scaling = transformation.scaling * change;
			scaling += Vector3.one;
			bone.Scale(Id, scaling, transformation.options.scaling);
		}

		private void HandleHipHeight(EditBone bone, SkeletonEditSliderData transformation, Humanoid target, float change)
		{
			EditBone bone2 = skeletonEdit.GetBone(SkeletonEditBones.Hips);
			EditBone bone3 = skeletonEdit.GetBone(transformation.secondary);
			float num = transformation.hipHeight * change;
			if (num != 0f)
			{
				Vector3 position = skeletonEdit.transform.position;
				if ((bool)bone3)
				{
					position = bone3.RealTransform.position;
				}
				Vector3 vector = bone.RealTransform.position - position;
				vector = skeletonEdit.transform.InverseTransformVector(vector) * num;
				vector.x = 0f;
				vector.z = 0f;
				SkeletonEditMovementOptions skeletonEditMovementOptions = default(SkeletonEditMovementOptions);
				skeletonEditMovementOptions.effectPairs = false;
				skeletonEditMovementOptions.speed = 1f;
				SkeletonEditMovementOptions options = skeletonEditMovementOptions;
				bone2.Move(Id, vector, options);
			}
		}

		private void HandleBoneLength(EditBone bone, SkeletonEditSliderData transformation, Humanoid target, float change, BoneLengthCallType callType = BoneLengthCallType.First)
		{
			if (!bone || transformation.boneLength == 0f)
			{
				return;
			}
			if (transformation.boneLengthOptions.effectSibling && callType == BoneLengthCallType.First)
			{
				HandleBoneLength(bone.SiblingBone, transformation, target, change, BoneLengthCallType.Sibling);
			}
			if (callType == BoneLengthCallType.First)
			{
				callType = BoneLengthCallType.Child;
			}
			BoneLengthModes mode = transformation.boneLengthOptions.mode;
			EditBone bone2 = skeletonEdit.GetBone(transformation.secondary);
			SkeletonEditMovementOptions skeletonEditMovementOptions = default(SkeletonEditMovementOptions);
			skeletonEditMovementOptions.effectPairs = false;
			skeletonEditMovementOptions.speed = 1f;
			SkeletonEditMovementOptions options = skeletonEditMovementOptions;
			if (mode == BoneLengthModes.TargetMode)
			{
				if ((bool)bone2)
				{
					Vector3 movement = bone.RealTransform.position - bone2.RealTransform.position;
					movement *= transformation.boneLength * change;
					movement = bone.Move(Id, movement, options);
				}
				return;
			}
			foreach (EditBone item in SkeletonEdit.FindFirstChildEditBones(bone.RealTransform))
			{
				Vector3 zero = Vector3.zero;
				switch (mode)
				{
				case BoneLengthModes.NormalMode:
					zero = (item.RealTransform.position - bone.RealTransform.position).normalized * (transformation.boneLength * change);
					if (target.isGiantess)
					{
						zero *= 1000f;
					}
					zero = item.Move(Id, zero, options);
					break;
				case BoneLengthModes.ProportionalMode:
					zero = item.RealTransform.position - bone.RealTransform.position;
					zero *= transformation.boneLength * change;
					zero = item.Move(Id, zero, options, false);
					break;
				}
				if (transformation.boneLengthOptions.effectChildren && item != bone2)
				{
					HandleBoneLength(item, transformation, target, change, callType);
				}
			}
		}
	}
}
