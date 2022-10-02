using System;
using MoonSharp.Interpreter;
using UnityEngine;

namespace Lua
{
	[MoonSharpUserData]
	public class Morphs
	{
		private EntityBase entity;

		[MoonSharpHidden]
		public Morphs(EntityBase e)
		{
			if (e == null)
			{
				Debug.LogError("Creating Morphs with no entity");
			}
			entity = e;
		}

		public int GetMorphCount()
		{
			return entity.Morphs.Count;
		}

		public int FindMorphIndex(string morphName)
		{
			for (int i = 0; i < entity.Morphs.Count; i++)
			{
				if (entity.Morphs[i].Name.Equals(morphName, StringComparison.OrdinalIgnoreCase))
				{
					return i + 1;
				}
			}
			return -1;
		}

		public void ResetMorphs()
		{
			for (int i = 0; i < entity.Morphs.Count; i++)
			{
				entity.SetMorphValue(i, 0f);
			}
		}

		public bool HasMorph(string morphName)
		{
			for (int i = 0; i < entity.Morphs.Count; i++)
			{
				if (entity.Morphs[i].Name.Equals(morphName, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		public void SetMorphValue(string morphName, float weight)
		{
			weight = ((weight < 0f) ? 0f : ((weight > 1f) ? 1f : weight));
			for (int i = 0; i < entity.Morphs.Count; i++)
			{
				if (entity.Morphs[i].Name.Equals(morphName, StringComparison.OrdinalIgnoreCase))
				{
					entity.SetMorphValue(i, weight);
					return;
				}
			}
			Debug.LogWarning("SetMorphValue() - Failed to find morph \"" + morphName + "\".");
		}

		public void SetMorphValue(int morphIndex, float weight)
		{
			morphIndex--;
			if (morphIndex < 0 || morphIndex >= entity.Morphs.Count)
			{
				Debug.LogError("SetMorphValue() - Index is out of range; index = " + morphIndex + ", size = " + entity.Morphs.Count);
				return;
			}
			weight = ((weight < 0f) ? 0f : ((weight > 1f) ? 1f : weight));
			entity.SetMorphValue(morphIndex, weight);
		}

		public float GetMorphValue(string morphName)
		{
			for (int i = 0; i < entity.Morphs.Count; i++)
			{
				if (entity.Morphs[i].Name.Equals(morphName, StringComparison.OrdinalIgnoreCase))
				{
					return entity.Morphs[i].Weight;
				}
			}
			Debug.LogError("GetMorphValue() - Failed to find morph \"" + morphName + "\".");
			return 0f;
		}

		public float GetMorphValue(int morphIndex)
		{
			morphIndex--;
			if (morphIndex < 0 || morphIndex >= entity.Morphs.Count)
			{
				Debug.LogError("GetMorphValue() - Index is out of range; index = " + morphIndex + ", size = " + entity.Morphs.Count);
				return 0f;
			}
			return entity.Morphs[morphIndex].Weight;
		}

		public string GetMorphName(int index)
		{
			index--;
			if (index < 0 || index >= entity.Morphs.Count)
			{
				Debug.LogError("GetMorphName() - Index is out of range; index = " + index + ", size = " + entity.Morphs.Count);
				return null;
			}
			return entity.Morphs[index].Name;
		}

		public string[] GetMorphList()
		{
			string[] array = new string[entity.Morphs.Count];
			int num = 0;
			foreach (EntityMorphData morph in entity.Morphs)
			{
				array[num++] = (string)morph.Name.Clone();
			}
			return array;
		}
	}
}
