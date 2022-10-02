using System.Collections.Generic;
using UnityEngine;

namespace Sizebox.CharacterEditor
{
	[CreateAssetMenu(menuName = "Skeleton Editor/New Master Slider Set")]
	public class SkeletonEditSliderSetObject : ScriptableObject
	{
		[SerializeField]
		private List<SkeletonEditSliderSet> sets = new List<SkeletonEditSliderSet>();

		public List<SkeletonEditSliderSet> Sets
		{
			get
			{
				return sets;
			}
		}
	}
}
