using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AudioLoader
{
	private readonly Dictionary<string, AudioClip> _audioClips;

	public AudioLoader()
	{
		_audioClips = new Dictionary<string, AudioClip>();
	}

	public AudioClip LoadAudioClip(string clipName)
	{
		AudioClip value;
		if (!_audioClips.TryGetValue(clipName, out value))
		{
			Debug.LogWarning("Audio file " + clipName + " not found.");
		}
		else
		{
			value.name = clipName;
		}
		return value;
	}

	public void SearchAndLoadClips(List<string> folder)
	{
		foreach (string file in IOManager.GetFileList(folder))
		{
			string extension = Path.GetExtension(file);
			if (!(extension != ".wav") || !(extension != ".ogg"))
			{
				string fileName = Path.GetFileName(file);
				if (!_audioClips.ContainsKey(fileName))
				{
					_audioClips.Add(fileName, LoadClip(file));
				}
			}
		}
	}

	private AudioClip LoadClip(string path)
	{
		return new WWW("file:///" + path).GetAudioClip();
	}
}
