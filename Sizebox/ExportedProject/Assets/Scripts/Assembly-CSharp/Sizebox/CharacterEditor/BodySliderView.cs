using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace Sizebox.CharacterEditor
{
	public class BodySliderView : BaseView
	{
		[Space(15f)]
		[SerializeField]
		private HandleManager handleManager;

		[SerializeField]
		private BodyView bodyView;

		[SerializeField]
		private RectTransform sliderSetsParent;

		[Header("Prefabs")]
		[SerializeField]
		private SkeletonEditSliderSetObject sliderSetsObject;

		[SerializeField]
		private SkeletonEditSliderSetUI sliderSetUiPrefab;

		[SerializeField]
		private SkeletonEditSliderUI sliderUiPrefab;

		[Header("Buttons")]
		[SerializeField]
		private Button nextSetButton;

		[SerializeField]
		private Button prevSetButton;

		[Header("Set Name Text")]
		[SerializeField]
		private Text setNameText;

		private List<SkeletonEditSliderSetUI> sliderSets = new List<SkeletonEditSliderSetUI>();

		private List<SkeletonEditSlider> sliders = new List<SkeletonEditSlider>();

		private int index;

		protected override void Awake()
		{
			base.Awake();
			nextSetButton.onClick.AddListener(_003CAwake_003Eb__11_0);
			prevSetButton.onClick.AddListener(_003CAwake_003Eb__11_1);
			CreateSliders();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			PrepareSliders();
		}

		private void PrepareSliders()
		{
			if (!handleManager.TargetEditor)
			{
				return;
			}
			SkeletonEdit component = handleManager.TargetEditor.GetComponent<SkeletonEdit>();
			if (!component)
			{
				return;
			}
			foreach (SkeletonEditSlider slider in sliders)
			{
				slider.RegisterSkeleton(component);
			}
			foreach (SkeletonEditSliderSetUI sliderSet in sliderSets)
			{
				sliderSet.gameObject.SetActive(false);
			}
			ChangeSliderSet(0);
		}

		private void CreateSliders()
		{
			sliders = new List<SkeletonEditSlider>();
			foreach (SkeletonEditSliderSet set in sliderSetsObject.Sets)
			{
				SkeletonEditSliderSetUI skeletonEditSliderSetUI = Object.Instantiate(sliderSetUiPrefab, sliderSetsParent);
				skeletonEditSliderSetUI.Set = set;
				sliderSets.Add(skeletonEditSliderSetUI);
				foreach (SkeletonEditSliderObject slider in set.sliders)
				{
					if ((bool)slider)
					{
						SkeletonEditSlider item = Object.Instantiate(sliderUiPrefab, skeletonEditSliderSetUI.ContentTransform).Initialize(bodyView.DataId, slider);
						sliders.Add(item);
					}
				}
			}
			if (sliderSets.Count <= 0)
			{
				return;
			}
			foreach (SkeletonEditSliderSetUI sliderSet in sliderSets)
			{
				sliderSet.gameObject.SetActive(false);
			}
		}

		private void ChangeSliderSet(int indexChange)
		{
			sliderSets[index].gameObject.SetActive(false);
			index += indexChange;
			if (index < 0)
			{
				index = sliderSets.Count - 1;
			}
			else if (index >= sliderSets.Count)
			{
				index = 0;
			}
			sliderSets[index].gameObject.SetActive(true);
			setNameText.text = sliderSets[index].Set.name;
		}

		[CompilerGenerated]
		private void _003CAwake_003Eb__11_0()
		{
			ChangeSliderSet(1);
		}

		[CompilerGenerated]
		private void _003CAwake_003Eb__11_1()
		{
			ChangeSliderSet(-1);
		}
	}
}
