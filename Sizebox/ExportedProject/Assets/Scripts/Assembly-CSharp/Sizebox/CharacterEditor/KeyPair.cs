using System;

namespace Sizebox.CharacterEditor
{
	[Serializable]
	public struct KeyPair
	{
		public string key;

		public string key2;

		public KeyPair(string inKey, string inKey2)
		{
			if (string.Compare(inKey, inKey2) == 1)
			{
				key = inKey;
				key2 = inKey2;
			}
			else
			{
				key = inKey2;
				key2 = inKey;
			}
		}
	}
}
