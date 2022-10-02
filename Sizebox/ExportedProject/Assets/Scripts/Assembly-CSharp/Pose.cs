using System;
using UnityEngine;

[Serializable]
public class Pose
{
	public Sprite sprite;

	public string name;

	public virtual bool IsCustom
	{
		get
		{
			return false;
		}
	}

	public Pose(Sprite sprite)
	{
		this.sprite = sprite;
		if ((bool)sprite)
		{
			name = sprite.name.Split('.')[0];
		}
	}
}
