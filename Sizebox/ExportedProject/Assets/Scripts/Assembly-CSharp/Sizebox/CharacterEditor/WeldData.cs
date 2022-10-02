using System;
using System.Collections.Generic;

namespace Sizebox.CharacterEditor
{
	[Serializable]
	internal class WeldData
	{
		public string objectName;

		public Queue<int> reusableIndexes;

		public Dictionary<string, int> weldIndexMap;

		public WeldData(string objectName)
		{
			this.objectName = objectName;
			reusableIndexes = new Queue<int>();
			weldIndexMap = new Dictionary<string, int>();
		}

		public string Add()
		{
			string text = GenerateObjectKey();
			weldIndexMap.Add(text, NextIndex());
			if (reusableIndexes.Count > 0)
			{
				reusableIndexes.Dequeue();
			}
			return text;
		}

		public void Remove(string weldKey)
		{
			if (weldIndexMap.ContainsKey(weldKey))
			{
				int item = weldIndexMap[weldKey];
				weldIndexMap.Remove(weldKey);
				reusableIndexes.Enqueue(item);
			}
		}

		private string GenerateObjectKey()
		{
			return objectName + NextIndex();
		}

		private int NextIndex()
		{
			if (reusableIndexes.Count > 0)
			{
				return reusableIndexes.Peek();
			}
			return weldIndexMap.Count;
		}
	}
}
