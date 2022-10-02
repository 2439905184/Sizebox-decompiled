using UnityEngine;
using UnityEngine.UI;

namespace Sizebox.CharacterEditor
{
	public class SkeletonEditSliderUI : MonoBehaviour
	{
		[SerializeField]
		private Text sliderText;

		[SerializeField]
		private SkeletonEditSlider slider;

		public SkeletonEditSlider Initialize(string id, SkeletonEditSliderObject sliderObj)
		{
			slider.Initialize(id, sliderObj);
			sliderText.text = sliderObj.SliderText;
			return slider;
		}
	}
}
