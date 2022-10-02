using System;
using UnityEngine;

namespace Sizebox.CharacterEditor
{
	[Serializable]
	public struct TextureData
	{
		public string propertyName;

		public string textureName;

		public Vector2 tiling;

		public TextureData(string property, string textureName, Vector2 tiling)
		{
			propertyName = property;
			this.textureName = textureName;
			this.tiling = tiling;
		}
	}
}
