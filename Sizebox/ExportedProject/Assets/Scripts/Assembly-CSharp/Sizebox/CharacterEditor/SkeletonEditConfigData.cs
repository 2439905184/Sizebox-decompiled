using System;
using System.Collections.Generic;

namespace Sizebox.CharacterEditor
{
	[Serializable]
	public class SkeletonEditConfigData
	{
		public List<KeyPair> siblings = new List<KeyPair>();

		public List<KeyPair> links = new List<KeyPair>();

		public void AddPair(KeyPair pair)
		{
			if (!siblings.Contains(pair))
			{
				siblings.Add(pair);
			}
		}

		public void RemovePair(KeyPair pair)
		{
			if (siblings.Contains(pair))
			{
				siblings.Remove(pair);
			}
		}

		public void AddLink(KeyPair pair)
		{
			if (!links.Contains(pair))
			{
				links.Add(pair);
			}
		}

		public void RemoveLink(KeyPair pair)
		{
			if (links.Contains(pair))
			{
				links.Remove(pair);
			}
		}
	}
}
