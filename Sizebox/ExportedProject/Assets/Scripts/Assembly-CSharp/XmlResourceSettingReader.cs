using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

internal class XmlResourceSettingReader
{
	private readonly string[] settingsPath = new string[2]
	{
		Application.dataPath + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + "Settings.xml",
		Application.persistentDataPath + Path.DirectorySeparatorChar + "Settings.xml"
	};

	public List<string> GiantessPaths { get; private set; }

	public List<string> MaleNpcPaths { get; private set; }

	public List<string> FemaleNpcPaths { get; private set; }

	public List<string> ObjectPaths { get; private set; }

	public List<string> SoundPaths { get; private set; }

	public List<string> ScenePaths { get; private set; }

	public List<string> LuaPaths { get; private set; }

	public string ScreenShotPath { get; private set; }

	public string SavePath { get; private set; }

	public string ConfigPath { get; private set; }

	public string CharacterPath { get; private set; }

	public XmlResourceSettingReader(bool loadSettings = true)
	{
		if (loadSettings)
		{
			LoadFromFile();
		}
		else
		{
			InitLists();
		}
	}

	private void InitLists()
	{
		GiantessPaths = new List<string>();
		MaleNpcPaths = new List<string>();
		FemaleNpcPaths = new List<string>();
		ObjectPaths = new List<string>();
		SoundPaths = new List<string>();
		ScenePaths = new List<string>();
		LuaPaths = new List<string>();
	}

	private void LoadFromFile()
	{
		InitLists();
		ParseXmlFile();
	}

	private void ParseXmlFile()
	{
		string[] array = settingsPath;
		foreach (string text in array)
		{
			if (!File.Exists(text))
			{
				continue;
			}
			using (XmlReader xmlReader = XmlReader.Create(text))
			{
				while (xmlReader.Read())
				{
					if (xmlReader.NodeType == XmlNodeType.Element)
					{
						switch (xmlReader.Name)
						{
						case "GiantessFolder":
							LoadAssetPath(xmlReader, GiantessPaths);
							break;
						case "FemaleNpcFolder":
							LoadAssetPath(xmlReader, FemaleNpcPaths);
							break;
						case "MaleNpcFolder":
							LoadAssetPath(xmlReader, MaleNpcPaths);
							break;
						case "ObjectFolder":
							LoadAssetPath(xmlReader, ObjectPaths);
							break;
						case "SoundFolder":
							LoadAssetPath(xmlReader, SoundPaths);
							break;
						case "ScenesFolder":
							LoadAssetPath(xmlReader, ScenePaths);
							break;
						case "ScriptFolder":
							LoadAssetPath(xmlReader, LuaPaths);
							break;
						case "SavesFolder":
							SavePath = LoadAssetPath(xmlReader, SavePath);
							break;
						case "ScreenshotFolder":
							ScreenShotPath = LoadAssetPath(xmlReader, ScreenShotPath);
							break;
						case "ConfigFolder":
							ConfigPath = LoadAssetPath(xmlReader, ConfigPath);
							break;
						case "CharacterFolder":
							CharacterPath = LoadAssetPath(xmlReader, CharacterPath);
							break;
						}
					}
				}
			}
		}
	}

	private static void LoadAssetPath(XmlReader reader, ICollection<string> storageTarget)
	{
		string name = reader.Name;
		reader.Read();
		if (reader.HasValue)
		{
			if (Directory.Exists(reader.Value))
			{
				storageTarget.Add(reader.Value);
				return;
			}
			Debug.LogWarning("XML: Could not load entry '" + name + "' path '" + reader.Value + "'");
		}
	}

	private static string LoadAssetPath(XmlReader reader, string currentValue)
	{
		string name = reader.Name;
		reader.Read();
		if (reader.HasValue)
		{
			if (Directory.Exists(reader.Value))
			{
				if (currentValue != null)
				{
					Debug.LogWarning("XML: Entry '" + name + "' was overwritten from '" + currentValue + "' to '" + reader.Value + "'");
				}
				return reader.Value;
			}
			Debug.LogWarning("XML: Could not load entry '" + name + "' path '" + reader.Value + "'");
		}
		return currentValue;
	}
}
