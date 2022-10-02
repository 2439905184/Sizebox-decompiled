using UnityEngine;

public class Stored
{
	internal string Key;

	public void Reset()
	{
		PlayerPrefs.DeleteKey(Key);
	}
}
