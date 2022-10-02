using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.HSVPicker
{
	public class ColorPresetList
	{
		public string ListId { get; private set; }

		public List<Color> Colors { get; private set; }

		public event UnityAction<List<Color>> OnColorsUpdated;

		public ColorPresetList(string listId, List<Color> colors = null)
		{
			if (colors == null)
			{
				colors = new List<Color>();
			}
			Colors = colors;
			ListId = listId;
		}

		public void AddColor(Color color)
		{
			Colors.Add(color);
			if (this.OnColorsUpdated != null)
			{
				this.OnColorsUpdated(Colors);
			}
		}

		public void UpdateList(IEnumerable<Color> colors)
		{
			Colors.Clear();
			Colors.AddRange(colors);
			if (this.OnColorsUpdated != null)
			{
				this.OnColorsUpdated(Colors);
			}
		}
	}
}
