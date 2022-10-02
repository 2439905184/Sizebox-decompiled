using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TextureLoader
{
	private static Dictionary<int, bool> transparentTextures;

	public static Sprite LoadNewSprite(string filePath, float pixelsPerUnit = 100f, SpriteMeshType spriteType = SpriteMeshType.Tight)
	{
		Texture2D texture2D = LoadTexture(filePath);
		return Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0f, 0f), pixelsPerUnit, 0u, spriteType);
	}

	public static Texture2D LoadTexture(string filePath)
	{
		if (File.Exists(filePath))
		{
			byte[] data = File.ReadAllBytes(filePath);
			Texture2D texture2D = new Texture2D(2, 2);
			if (texture2D.LoadImage(data))
			{
				return texture2D;
			}
		}
		return null;
	}

	public static bool IsTransparent(Texture texture)
	{
		if (texture == null)
		{
			return false;
		}
		if (transparentTextures == null)
		{
			transparentTextures = new Dictionary<int, bool>();
		}
		int instanceID = texture.GetInstanceID();
		bool value;
		if (transparentTextures.TryGetValue(instanceID, out value))
		{
			return value;
		}
		Texture2D texture2D = (Texture2D)texture;
		if (texture2D == null || texture2D.width == 0 || texture2D.height == 0)
		{
			return false;
		}
		FilterMode filterMode = texture2D.filterMode;
		texture2D.filterMode = FilterMode.Point;
		RenderTexture temporary = RenderTexture.GetTemporary(texture2D.width, texture2D.height);
		temporary.filterMode = FilterMode.Point;
		RenderTexture.active = temporary;
		Graphics.Blit(texture2D, temporary);
		int num = 4;
		Texture2D texture2D2;
		try
		{
			texture2D2 = new Texture2D(texture2D.width / num, texture2D.height / num);
			texture2D2.ReadPixels(new Rect(0f, 0f, texture2D.width / num, texture2D.height / num), 0, 0, false);
		}
		catch
		{
			texture2D2 = new Texture2D(texture2D.width, texture2D.height);
			texture2D2.ReadPixels(new Rect(0f, 0f, texture2D.width, texture2D.height), 0, 0, false);
		}
		texture2D2.Apply();
		RenderTexture.active = null;
		texture2D.filterMode = filterMode;
		texture2D = texture2D2;
		Color32[] pixels = texture2D.GetPixels32();
		value = false;
		for (int i = 0; i < pixels.Length; i += 4)
		{
			if ((float)(int)pixels[i].a < 1f)
			{
				value = true;
				break;
			}
		}
		transparentTextures.Add(instanceID, value);
		return value;
	}
}
