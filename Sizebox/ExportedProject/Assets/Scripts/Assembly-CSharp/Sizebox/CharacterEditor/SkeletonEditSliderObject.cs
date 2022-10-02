using UnityEngine;

namespace Sizebox.CharacterEditor
{
	[CreateAssetMenu(menuName = "Skeleton Editor/New Slider")]
	public class SkeletonEditSliderObject : ScriptableObject
	{
		[SerializeField]
		private string sliderText = "SLIDER";

		[SerializeField]
		private SkeletonEditSliderData[] transformations;

		public string SliderText
		{
			get
			{
				return sliderText;
			}
		}

		public SkeletonEditSliderData[] Transformations
		{
			get
			{
				return transformations;
			}
		}
	}
}
