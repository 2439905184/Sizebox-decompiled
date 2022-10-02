using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;
using MoonSharp.Interpreter;
using UnityEngine;

public class IOManager : MonoBehaviour
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass75_0
	{
		public XmlWriter writer;

		internal void _003CSaveSettings_003Eb__0(LuaBehavior.BehaviorSetting setting)
		{
			writer.WriteWhitespace("\r\n\t");
			writer.WriteStartElement("setting");
			writer.WriteAttributeString("name", setting.VariableName);
			writer.WriteAttributeString("value", setting.Value.ToPrintString());
			writer.WriteEndElement();
		}
	}

	public const string FlagPath = "-path";

	public const string FlagModelPath = "-path-models";

	public const string FlagSoundPath = "-path-sounds";

	public const string FlagDataPath = "-path-data";

	public const string FlagScenePath = "-path-scenes";

	public const string FlagScriptPath = "-path-scripts";

	public const string FlagScreenshotPath = "-path-screenshots";

	public const string DefDirectorySound = "Sounds";

	public const string DefDirectoryScript = "Scripts";

	public const string DefDirectoryScreenshot = "Screenshots";

	public const string DefDirectoryScene = "Scenes";

	public const string RefDirectoryModel = "Models";

	public const string RefDirectoryData = "Data";

	public const string SubDirectoryGts = "Giantess";

	public const string SubDirectoryMaleNpc = "MaleNPC";

	public const string SubDirectoryFemaleNpc = "FemaleNPC";

	public const string SubDirectoryObjects = "Objects";

	public const string SubDirectorySaves = "Saves";

	public const string SubDirectoryLuaData = "Config";

	public const string SubDirectoryCharacter = "Character";

	public const string SubDirectoryPoses = "Poses";

	private string _settingsConfigVersion;

	private string _dataCache;

	private AudioLoader _audioLoader;

	private static IOManager _instance;

	public Dictionary<string, RuntimeAnimatorController> AnimationControllers;

	private string[] _animationList;

	private List<string> _folderGtsModels;

	private List<string> _folderMaleNpc;

	private List<string> _folderFemaleNpc;

	private List<string> _folderObjects;

	private List<string> _folderSounds;

	public RuntimeAnimatorController microAnimatorController;

	public RuntimeAnimatorController playerAnimatorController;

	public RuntimeAnimatorController gtsPlayerAnimatorController;

	public RuntimeAnimatorController poseAnimatorController;

	public RuntimeAnimatorController gtsAnimatorController;

	public HashSet<string> Poses;

	public List<string> posesList;

	private string _folderSaves;

	private string _folderScreenshots;

	private string _folderConfig;

	public bool isLoadingData;

	public static IOManager Instance
	{
		get
		{
			if ((bool)_instance)
			{
				return _instance;
			}
			new GameObject("IOManager").AddComponent<IOManager>();
			return _instance;
		}
	}

	public static bool Initialized
	{
		get
		{
			return _instance;
		}
	}

	public IList<string> GtsAssetFolder
	{
		get
		{
			return _folderGtsModels.AsReadOnly();
		}
	}

	public IList<string> MaleMicroAssetFolder
	{
		get
		{
			return _folderMaleNpc.AsReadOnly();
		}
	}

	public IList<string> FemaleMicroAssetFolder
	{
		get
		{
			return _folderFemaleNpc.AsReadOnly();
		}
	}

	public IList<string> ObjectAssetFolder
	{
		get
		{
			return _folderObjects.AsReadOnly();
		}
	}

	public IList<string> SoundAssetFolder
	{
		get
		{
			return _folderSounds.AsReadOnly();
		}
	}

	public string LoadCachedDataFile()
	{
		isLoadingData = false;
		return _dataCache;
	}

	private void Awake()
	{
		if (!_instance)
		{
			_instance = this;
			_instance.Initialize();
		}
	}

	private void Initialize()
	{
		Layers.Initialize();
		UnityEngine.Object.DontDestroyOnLoad(_instance.gameObject);
		_settingsConfigVersion = "1.0";
		_audioLoader = new AudioLoader();
		_folderGtsModels = new List<string>();
		_folderMaleNpc = new List<string>();
		_folderFemaleNpc = new List<string>();
		_folderObjects = new List<string>();
		_folderSounds = new List<string>();
		microAnimatorController = Resources.Load<RuntimeAnimatorController>("GTSAnimator");
		playerAnimatorController = Resources.Load<RuntimeAnimatorController>("CharacterController");
		gtsPlayerAnimatorController = Resources.Load<RuntimeAnimatorController>("GTSPlayer");
		poseAnimatorController = Resources.Load<RuntimeAnimatorController>("GTSStaticAnimator");
		gtsAnimatorController = Resources.Load<RuntimeAnimatorController>("GTSAnimator");
		Poses = new HashSet<string>();
		AnimationClip[] animationClips = poseAnimatorController.animationClips;
		foreach (AnimationClip animationClip in animationClips)
		{
			Poses.Add(animationClip.name);
		}
		posesList = new List<string>(Poses);
		AnimationControllers = new Dictionary<string, RuntimeAnimatorController>();
		animationClips = gtsAnimatorController.animationClips;
		foreach (AnimationClip animationClip2 in animationClips)
		{
			if (!animationClip2.name.StartsWith("_"))
			{
				AnimationControllers[animationClip2.name] = gtsAnimatorController;
			}
		}
		AddIf(_folderGtsModels, GetUserDirectory(Path.Combine("Models", "Giantess")));
		AddIf(_folderMaleNpc, GetUserDirectory(Path.Combine("Models", "MaleNPC")));
		AddIf(_folderFemaleNpc, GetUserDirectory(Path.Combine("Models", "FemaleNPC")));
		AddIf(_folderObjects, GetUserDirectory(Path.Combine("Models", "Objects")));
		AddIf(_folderSounds, GetUserDirectory("Sounds"));
		AddIf(_folderGtsModels, GetApplicationDirectory(Path.Combine("Models", "Giantess")));
		AddIf(_folderMaleNpc, GetApplicationDirectory(Path.Combine("Models", "MaleNPC")));
		AddIf(_folderFemaleNpc, GetApplicationDirectory(Path.Combine("Models", "FemaleNPC")));
		AddIf(_folderObjects, GetApplicationDirectory(Path.Combine("Models", "Objects")));
		AddIf(_folderSounds, GetApplicationDirectory("Sounds"));
		string text = Sbox.StringPreferenceOrArgument(GlobalPreferences.PathModel, "-path-models");
		if (!string.IsNullOrEmpty(text) && Directory.Exists(text))
		{
			AddIf(_folderGtsModels, Path.Combine(text, "Giantess"));
			AddIf(_folderMaleNpc, Path.Combine(text, "MaleNPC"));
			AddIf(_folderFemaleNpc, Path.Combine(text, "FemaleNPC"));
			AddIf(_folderObjects, Path.Combine(text, "Objects"));
		}
		text = Sbox.StringPreferenceOrArgument(GlobalPreferences.PathSound, "-path-sounds", "Sounds");
		if (!string.IsNullOrEmpty(text) && Directory.Exists(text))
		{
			AddIf(_folderSounds, text);
		}
		text = Sbox.StringPreferenceOrArgument(GlobalPreferences.PathData, "-path-data");
		if (!string.IsNullOrEmpty(text))
		{
			_folderSaves = Path.Combine(text, "Saves");
			_folderConfig = Path.Combine(text, "Config");
		}
		_folderScreenshots = Sbox.StringPreferenceOrArgument(GlobalPreferences.PathScreenshot, "-path-screenshots", "Screenshots");
		RecheckLivePaths();
		StartCoroutine(LoadSoundFiles());
	}

	public void RecheckLivePaths()
	{
		if (!Directory.Exists(_folderSaves))
		{
			string userDirectory = GetUserDirectory("Saves", true);
			if (!string.IsNullOrEmpty(_folderSaves))
			{
				Debug.LogWarning("Couldn't use path '" + _folderSaves + "'. Fallback '" + userDirectory + "' used.");
			}
			_folderSaves = userDirectory;
		}
		if (!Directory.Exists(_folderScreenshots))
		{
			string userDirectory2 = GetUserDirectory("Screenshots", true);
			if (!string.IsNullOrEmpty(_folderScreenshots))
			{
				Debug.LogWarning("Couldn't use path '" + _folderScreenshots + "'. Fallback '" + userDirectory2 + "' used.");
			}
			_folderScreenshots = userDirectory2;
		}
		if (!Directory.Exists(_folderConfig))
		{
			string userDirectory3 = GetUserDirectory("Scripts", true);
			if (!string.IsNullOrEmpty(_folderConfig))
			{
				Debug.LogWarning("Couldn't use path '" + _folderConfig + "'. Fallback '" + userDirectory3 + "' used.");
			}
			_folderConfig = userDirectory3;
		}
	}

	public void GetScreenshotFileName(out string fullPath, out string fileName)
	{
		string text = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
		fileName = "Screenshot " + text + ".png";
		fullPath = Path.Combine(_folderScreenshots, fileName);
		uint num = 2u;
		while (File.Exists(fullPath))
		{
			fileName = "Screenshot " + text + "-" + num + ".png";
			fullPath = Path.Combine(_folderScreenshots, fileName);
			num++;
		}
	}

	public static string[] GetListLoadableSaveFiles()
	{
		string[] array = null;
		string[] array2 = null;
		string[] array3 = null;
		string userDirectory = GetUserDirectory("Saves");
		if (userDirectory != null)
		{
			array = Directory.GetFileSystemEntries(userDirectory);
		}
		string applicationDirectory = GetApplicationDirectory("Saves");
		if (applicationDirectory != null)
		{
			array2 = Directory.GetFileSystemEntries(applicationDirectory);
		}
		string text = Sbox.StringPreferenceOrArgument(GlobalPreferences.PathData, "-path-data");
		if (!string.IsNullOrEmpty(text))
		{
			text = Path.Combine(text, "Saves");
			if (Directory.Exists(text) && !text.Equals(userDirectory) && !text.Equals(applicationDirectory))
			{
				array3 = Directory.GetFileSystemEntries(text);
			}
		}
		int num = 0;
		int num2 = 0;
		if (array != null)
		{
			num += array.Length;
		}
		if (array2 != null)
		{
			num += array2.Length;
		}
		if (array3 != null)
		{
			num += array3.Length;
		}
		string[] array4 = new string[num];
		if (array != null)
		{
			array.CopyTo(array4, num2);
			num2 += array.Length;
		}
		if (array2 != null)
		{
			array2.CopyTo(array4, num2);
			num2 += array2.Length;
		}
		if (array3 != null)
		{
			array3.CopyTo(array4, num2);
		}
		return array4;
	}

	public string[] GetListSavedFiles()
	{
		string[] fileSystemEntries = Directory.GetFileSystemEntries(_folderSaves);
		for (int i = 0; i < fileSystemEntries.Length; i++)
		{
			string[] array = fileSystemEntries[i].Split(Path.DirectorySeparatorChar);
			fileSystemEntries[i] = array[array.Length - 1].Replace(".save", "");
		}
		return fileSystemEntries;
	}

	public bool SaveExists(string filename)
	{
		return File.Exists(_folderSaves + Path.DirectorySeparatorChar + filename);
	}

	public void SaveFile(string filename, string content)
	{
		StreamWriter streamWriter = new StreamWriter(_folderSaves + Path.DirectorySeparatorChar + filename);
		streamWriter.Write(content);
		streamWriter.Close();
	}

	public string LoadFile(string filename)
	{
		if (File.Exists(filename))
		{
			string text = File.ReadAllText(filename);
			isLoadingData = true;
			_dataCache = text;
			return text;
		}
		Debug.LogError("No saved scene found: " + filename);
		_dataCache = null;
		return null;
	}

	public static void AddIf(List<string> list, string path)
	{
		if (path != null)
		{
			list.Add(path);
		}
	}

	public static void AddIfExists(List<string> list, string path)
	{
		if (Directory.Exists(path))
		{
			list.Add(path);
		}
	}

	public static string GetUserDirectory()
	{
		return Application.persistentDataPath.Replace('/', '\\');
	}

	private static string GetCustomDirectory(string path, bool write, bool mandatory)
	{
		if (!Directory.Exists(path))
		{
			if (write)
			{
				try
				{
					Directory.CreateDirectory(path);
					return path;
				}
				catch (Exception ex)
				{
					Debug.LogWarning("Couldn't create folder '" + path + "' " + ex);
					return null;
				}
			}
			if (mandatory)
			{
				Debug.LogWarning("Couldn't find directory '" + path + "'");
			}
			return null;
		}
		return path;
	}

	public static string GetUserDirectory(string path, bool write = false, bool mandatory = false)
	{
		return GetCustomDirectory(Path.Combine(Application.persistentDataPath + Path.DirectorySeparatorChar, path) + Path.DirectorySeparatorChar, write, mandatory);
	}

	public static string GetApplicationDirectory()
	{
		return Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
	}

	public static string GetApplicationDirectory(string path)
	{
		string path2 = Path.Combine(Application.dataPath, ".." + Path.DirectorySeparatorChar);
		path2 = Path.Combine(path2, path);
		path2 += Path.DirectorySeparatorChar;
		if (!Directory.Exists(path2))
		{
			try
			{
				Directory.CreateDirectory(path2);
				return path2;
			}
			catch (Exception ex)
			{
				Debug.LogWarning("Couldn't create folder '" + path2 + "' " + ex);
				return null;
			}
		}
		return path2;
	}

	public string[] GetAnimationList()
	{
		if (_animationList == null)
		{
			ICollection keys = AnimationControllers.Keys;
			_animationList = new string[keys.Count];
			keys.CopyTo(_animationList, 0);
			Array.Sort(_animationList);
		}
		return _animationList;
	}

	public void SaveSettings(LuaBehavior behavior)
	{
		string outputFileName = Path.Combine(_folderConfig, behavior.GetName() + "-config.xml");
		_003C_003Ec__DisplayClass75_0 _003C_003Ec__DisplayClass75_ = new _003C_003Ec__DisplayClass75_0();
		_003C_003Ec__DisplayClass75_.writer = XmlWriter.Create(outputFileName);
		try
		{
			_003C_003Ec__DisplayClass75_.writer.WriteWhitespace("\r\n");
			_003C_003Ec__DisplayClass75_.writer.WriteStartElement("config");
			_003C_003Ec__DisplayClass75_.writer.WriteAttributeString("version", _settingsConfigVersion);
			_003C_003Ec__DisplayClass75_.writer.WriteWhitespace("\r\n\t");
			_003C_003Ec__DisplayClass75_.writer.WriteStartElement("enabled");
			_003C_003Ec__DisplayClass75_.writer.WriteAttributeString("value", behavior.Enabled.ToString());
			_003C_003Ec__DisplayClass75_.writer.WriteEndElement();
			_003C_003Ec__DisplayClass75_.writer.WriteWhitespace("\r\n\t");
			_003C_003Ec__DisplayClass75_.writer.WriteStartElement("canUseAI");
			_003C_003Ec__DisplayClass75_.writer.WriteAttributeString("value", behavior.CanUseAI().ToString());
			_003C_003Ec__DisplayClass75_.writer.WriteEndElement();
			_003C_003Ec__DisplayClass75_.writer.WriteWhitespace("\r\n\t");
			_003C_003Ec__DisplayClass75_.writer.WriteStartElement("ai");
			_003C_003Ec__DisplayClass75_.writer.WriteAttributeString("value", behavior.IsAI().ToString());
			_003C_003Ec__DisplayClass75_.writer.WriteEndElement();
			for (int i = 0; i < behavior.GetTagCount(); i++)
			{
				_003C_003Ec__DisplayClass75_.writer.WriteWhitespace("\r\n\t");
				_003C_003Ec__DisplayClass75_.writer.WriteStartElement("tag");
				_003C_003Ec__DisplayClass75_.writer.WriteAttributeString("name", behavior.GetTag(i));
				_003C_003Ec__DisplayClass75_.writer.WriteEndElement();
			}
			behavior.Settings.ForEach(_003C_003Ec__DisplayClass75_._003CSaveSettings_003Eb__0);
			_003C_003Ec__DisplayClass75_.writer.WriteWhitespace("\r\n");
			_003C_003Ec__DisplayClass75_.writer.WriteEndElement();
			_003C_003Ec__DisplayClass75_.writer.Close();
		}
		finally
		{
			if (_003C_003Ec__DisplayClass75_.writer != null)
			{
				((IDisposable)_003C_003Ec__DisplayClass75_.writer).Dispose();
			}
		}
	}

	public void LoadSettings(LuaBehavior behavior)
	{
		string text = Path.Combine(_folderConfig, behavior.GetName() + "-config.xml");
		if (!File.Exists(text))
		{
			return;
		}
		using (XmlReader xmlReader = XmlReader.Create(text))
		{
			while (xmlReader.Read())
			{
				if (xmlReader.NodeType != XmlNodeType.Element)
				{
					continue;
				}
				if (xmlReader.Name == "config")
				{
					if (xmlReader.GetAttribute("version") != _settingsConfigVersion)
					{
						Debug.LogWarning("Behavior \"" + behavior.GetText() + "\" config file outdated; not loading");
						break;
					}
					behavior.ClearTags();
				}
				else if (xmlReader.Name == "setting")
				{
					for (int i = 0; i < behavior.Settings.Count; i++)
					{
						LuaBehavior.BehaviorSetting value = behavior.Settings[i];
						if (!(value.VariableName != xmlReader.GetAttribute("name")))
						{
							switch (value.Type)
							{
							case LuaBehavior.BehaviorSettingType.Bool:
								value.Value = DynValue.NewBoolean(bool.Parse(xmlReader.GetAttribute("value") ?? string.Empty));
								break;
							case LuaBehavior.BehaviorSettingType.String:
								value.Value = DynValue.NewString(xmlReader.GetAttribute("value"));
								break;
							case LuaBehavior.BehaviorSettingType.Array:
								value.Value = DynValue.NewNumber(int.Parse(xmlReader.GetAttribute("value") ?? string.Empty));
								break;
							case LuaBehavior.BehaviorSettingType.Float:
								value.Value = DynValue.NewNumber(double.Parse(xmlReader.GetAttribute("value") ?? string.Empty));
								break;
							}
							behavior.Settings[i] = value;
						}
					}
				}
				else if (xmlReader.Name == "enabled")
				{
					behavior.Enabled = bool.Parse(xmlReader.GetAttribute("value") ?? string.Empty);
				}
				else if (xmlReader.Name == "canUseAI")
				{
					behavior.CanUseAi = bool.Parse(xmlReader.GetAttribute("value") ?? string.Empty);
				}
				else if (xmlReader.Name == "ai")
				{
					bool flag = bool.Parse(xmlReader.GetAttribute("value") ?? string.Empty);
					if (!behavior.CanUseAI() && flag)
					{
						Debug.LogWarning("Behavior \"" + behavior.GetText() + "\" is not setup to use AI.");
					}
					else
					{
						behavior.AI = flag;
					}
				}
				else if (xmlReader.Name == "tag")
				{
					behavior.AddTag(xmlReader.GetAttribute("name"));
				}
			}
		}
	}

	private IEnumerator LoadSoundFiles()
	{
		_audioLoader.SearchAndLoadClips(_folderSounds);
		yield return null;
	}

	public AudioClip LoadAudioClip(string clipName)
	{
		return _audioLoader.LoadAudioClip(clipName);
	}

	public static List<string> GetFileList(List<string> folders)
	{
		List<string> list = new List<string>();
		foreach (string folder in folders)
		{
			if (Directory.Exists(folder.Trim()))
			{
				list.AddRange(GetFileList(folder.Trim()));
			}
			else
			{
				Debug.LogError("Directory '" + folder.Trim() + "' doesn't exist");
			}
		}
		return list;
	}

	private static List<string> GetFileList(string folder)
	{
		List<string> list = new List<string>();
		string[] directories = Directory.GetDirectories(folder);
		for (int i = 0; i < directories.Length; i++)
		{
			foreach (string file in GetFileList(directories[i]))
			{
				list.Add(file);
			}
		}
		directories = Directory.GetFiles(folder);
		foreach (string path in directories)
		{
			list.Add(Path.Combine(folder, path));
		}
		return list;
	}
}
