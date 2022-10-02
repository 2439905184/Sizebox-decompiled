using UnityEngine;

namespace Sizebox.CharacterEditor
{
	public class SkeletonEditSliderSetUI : MonoBehaviour
	{
		[SerializeField]
		private Transform content;

		public Transform ContentTransform
		{
			get
			{
				return content;
			}
		}

		public SkeletonEditSliderSet Set { get; set; }
	}
}
