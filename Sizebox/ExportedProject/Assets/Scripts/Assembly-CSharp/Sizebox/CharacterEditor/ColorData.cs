using System;
using UnityEngine;

namespace Sizebox.CharacterEditor
{
	[Serializable]
	public struct ColorData
	{
		public string propertyName;

		public Color color;

		public ColorData(string property, Color color)
		{
			propertyName = property;
			this.color = color;
		}
	}
}
