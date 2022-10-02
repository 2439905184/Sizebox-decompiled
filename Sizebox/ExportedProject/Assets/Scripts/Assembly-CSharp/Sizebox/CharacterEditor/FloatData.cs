using System;

namespace Sizebox.CharacterEditor
{
	[Serializable]
	public struct FloatData
	{
		public string propertyName;

		public float value;

		public FloatData(string property, float value)
		{
			propertyName = property;
			this.value = value;
		}
	}
}
