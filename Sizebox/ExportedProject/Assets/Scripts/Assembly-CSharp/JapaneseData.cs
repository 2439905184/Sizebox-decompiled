using System;
using System.Collections.Generic;
using UnityEngine;

public class JapaneseData : ScriptableObject
{
	[Serializable]
	public class MorphTranslation
	{
		public string english;

		public string japanese;
	}

	public List<MorphTranslation> translation;

	public string GetTranslation(string japanese)
	{
		if (translation == null)
		{
			translation = new List<MorphTranslation>();
		}
		foreach (MorphTranslation item in translation)
		{
			if (item.japanese == japanese)
			{
				return item.english;
			}
		}
		MorphTranslation morphTranslation = new MorphTranslation();
		morphTranslation.japanese = japanese;
		morphTranslation.english = "";
		translation.Add(morphTranslation);
		return "";
	}
}
