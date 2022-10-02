using System;
using System.Collections.Generic;

namespace Sizebox.CharacterEditor
{
	[Serializable]
	public class MaterialDataSet
	{
		public List<MaterialData> matData = new List<MaterialData>();

		public void Add(MaterialData data)
		{
			matData.Add(data);
		}
	}
}
