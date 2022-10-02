using System.Collections.Generic;

namespace Assets.HSVPicker
{
	public static class ColorPresetManager
	{
		private static Dictionary<string, ColorPresetList> _presets = new Dictionary<string, ColorPresetList>();

		public static ColorPresetList Get(string listId = "default")
		{
			ColorPresetList value;
			if (!_presets.TryGetValue(listId, out value))
			{
				value = new ColorPresetList(listId);
				_presets.Add(listId, value);
			}
			return value;
		}
	}
}
