using System;
using System.Collections.Generic;
using System.IO;
using Lua;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using UnityEngine;

public class LuaManager : MonoBehaviour
{
	private class LuaScript
	{
		public Script Script;

		public DynValue Start;

		public DynValue Update;

		public DynValue Coroutine;

		public DynValue Exit;

		public LuaScript()
		{
			Script = new Script();
		}
	}

	private class StreamingAssetsScriptLoader : ScriptLoaderBase
	{
		internal StreamingAssetsScriptLoader()
		{
			base.ModulePaths = new string[2] { "?.lua", "behaviors/?.lua" };
		}

		public override object LoadFile(string file, Table globalContext)
		{
			string path = Application.streamingAssetsPath + "/lua/" + file;
			if (!File.Exists(path))
			{
				Debug.LogError(file + " could not be found in lua directory");
				return null;
			}
			return LoadLuaFile(path);
		}

		public override bool ScriptFileExists(string name)
		{
			return File.Exists(Application.streamingAssetsPath + "/lua/" + name);
		}
	}

	public static LuaManager Instance;

	public static float LastMessageBoxTime = -100f;

	public static string LastMessageBoxText;

	private List<LuaScript> _luaScripts;

	private IList<LuaScript> _luaScriptUpdate;

	private IList<LuaScript> _luaScriptCoroutine;

	private World _world;

	private AllGiantess _gts;

	private AllMicros _micros;

	private Globals _globalTable;

	private static string _initializationCode = "\n\t-- BEHAVIOR\n\tBehaviorBase = { agent = nil }\n\tbehaviorList = {}\n\n\tfunction BehaviorBase:new(o)\n\t\to = o or {}\n\t\tsetmetatable(o, self)\n\t\tself.__index = self\n\t\treturn o\n\tend\n\n\tfunction RegisterBehavior(behaviorName)\n\t\tlocal b = BehaviorBase:new{name = behaviorName}\n\t\tbehaviorList[behaviorName] = b\n\t\treturn b\n\tend\n\n\tfunction CreateInstance(behaviorName, newagent, target, cursor)\n\t\tlocal BehaviorClass = behaviorList[behaviorName]\n\t\tlocal instance = BehaviorClass:new{name = behaviorName, agent = newagent, target = target, cursorPoint = cursor}\n\t\treturn instance\n\tend\n\n\t";

	public static bool Initialized
	{
		get
		{
			return Instance != null;
		}
	}

	private void InitializeValues()
	{
		_world = new World();
		_gts = new AllGiantess();
		_micros = new AllMicros();
		_globalTable = new Globals();
	}

	private void Start()
	{
		Instance = this;
		UserData.RegisterAssembly();
		InitializeValues();
		LoadBehaviors();
		LuaInitialize();
		LuaStart();
	}

	private void Update()
	{
		int i = 0;
		for (int num = _luaScriptUpdate.Count; i < num; i++)
		{
			LuaScript luaScript = _luaScriptUpdate[i];
			try
			{
				luaScript.Script.Call(luaScript.Update);
			}
			catch (ScriptRuntimeException ex)
			{
				Debug.LogError(ex.DecoratedMessage);
				UiMessageBox.Create(ex.DecoratedMessage, "Script Exception").Popup();
				_luaScriptUpdate.Remove(luaScript);
				i--;
				num--;
			}
		}
		i = 0;
		for (int num = _luaScriptCoroutine.Count; i < num; i++)
		{
			LuaScript luaScript2 = _luaScriptCoroutine[i];
			if (luaScript2.Coroutine != null)
			{
				luaScript2.Coroutine.Coroutine.Resume();
				continue;
			}
			_luaScriptCoroutine.Remove(luaScript2);
			i--;
			num--;
		}
	}

	private void OnDestroy()
	{
		IList<LuaScript> list = new List<LuaScript>();
		foreach (LuaScript luaScript in _luaScripts)
		{
			DynValue exit = luaScript.Exit;
			if (exit != null && exit.Type == DataType.Function)
			{
				list.Add(luaScript);
			}
		}
		foreach (LuaScript item in list)
		{
			try
			{
				item.Script.Call(item.Exit);
			}
			catch (ScriptRuntimeException ex)
			{
				Debug.LogError(ex.DecoratedMessage);
			}
		}
	}

	public void UpdatePlayerEntity(EntityBase newEntity)
	{
		EventManager.SendEvent(new LocalPlayerChanged());
	}

	private string[] GetAllLuaFiles()
	{
		List<string> list = new List<string>();
		List<string> list2 = new List<string> { Application.streamingAssetsPath + "/lua/behaviors/" };
		string text = Sbox.StringPreferenceOrArgument(GlobalPreferences.PathScript, "-path-scripts", "Scripts");
		if (!string.IsNullOrEmpty(text))
		{
			list2.Add(text);
		}
		foreach (string item in list2)
		{
			if (Directory.Exists(item))
			{
				list.AddRange(Directory.GetFiles(item));
			}
			else
			{
				Debug.Log("Could not find lua script folder: " + item);
			}
		}
		return list.ToArray();
	}

	public bool ReloadScript(LuaBehavior behavior)
	{
		string filename = behavior.Filename;
		string code = File.ReadAllText(behavior.FullFilename);
		Script script = InitializeScript();
		try
		{
			script.DoString(code, null, filename);
			try
			{
				foreach (TablePair pair in script.Globals.Get("behaviorList").Table.Pairs)
				{
					try
					{
						behavior.SetScript(pair.Value.Table, script);
						return true;
					}
					catch (Exception ex)
					{
						Debug.LogError("Error reloading script: " + filename);
						Debug.LogError(ex.Message);
						Debug.LogError(ex.StackTrace);
					}
				}
			}
			catch (ScriptRuntimeException ex2)
			{
				Debug.LogError(ex2.DecoratedMessage);
			}
		}
		catch (ScriptRuntimeException ex3)
		{
			Debug.LogError(ex3.DecoratedMessage);
		}
		catch (SyntaxErrorException ex4)
		{
			Debug.LogError(ex4.DecoratedMessage);
		}
		return false;
	}

	private void LoadBehaviors()
	{
		BehaviorLists.Initialize();
		string[] allLuaFiles = GetAllLuaFiles();
		foreach (string text in allLuaFiles)
		{
			if (!text.EndsWith(".lua"))
			{
				continue;
			}
			string fileName = Path.GetFileName(text);
			string code = File.ReadAllText(text);
			Script script = InitializeScript();
			try
			{
				script.DoString(code, null, fileName);
				try
				{
					foreach (TablePair pair in script.Globals.Get("behaviorList").Table.Pairs)
					{
						try
						{
							LuaBehavior behavior = new LuaBehavior(pair.Value.Table, script)
							{
								Filename = fileName,
								FullFilename = text
							};
							BehaviorLists.Instance.AddBehavior(behavior);
						}
						catch (Exception ex)
						{
							Debug.LogError("Error loading script: " + fileName);
							Debug.LogError(ex.Message);
							Debug.LogError(ex.StackTrace);
						}
					}
				}
				catch (ScriptRuntimeException ex2)
				{
					Debug.LogError(ex2.DecoratedMessage);
				}
			}
			catch (ScriptRuntimeException ex3)
			{
				Debug.LogError(ex3.DecoratedMessage);
			}
			catch (SyntaxErrorException ex4)
			{
				Debug.LogError(ex4.DecoratedMessage);
			}
		}
	}

	private void LuaInitialize()
	{
		List<string[]> list = LoadScripts();
		_luaScripts = new List<LuaScript>();
		_luaScriptUpdate = new List<LuaScript>();
		_luaScriptCoroutine = new List<LuaScript>();
		foreach (string[] item in list)
		{
			LuaScript luaScript = new LuaScript
			{
				Script = InitializeScript()
			};
			try
			{
				luaScript.Script.DoString(item[1], null, item[0]);
			}
			catch (ScriptRuntimeException ex)
			{
				Debug.LogError(ex.DecoratedMessage);
			}
			luaScript.Start = luaScript.Script.Globals.Get("Start" + 3u);
			if (object.Equals(luaScript.Start, DynValue.Nil))
			{
				luaScript.Start = luaScript.Script.Globals.Get("Start");
			}
			if (object.Equals(luaScript.Start, DynValue.Nil))
			{
				luaScript.Start = null;
			}
			luaScript.Update = luaScript.Script.Globals.Get("Update" + 3u);
			if (object.Equals(luaScript.Update, DynValue.Nil))
			{
				luaScript.Update = luaScript.Script.Globals.Get("Update");
			}
			if (object.Equals(luaScript.Update, DynValue.Nil))
			{
				luaScript.Update = null;
			}
			else
			{
				_luaScriptUpdate.Add(luaScript);
			}
			luaScript.Coroutine = luaScript.Script.Globals.Get("Coroutine" + 3u);
			if (object.Equals(luaScript.Coroutine, DynValue.Nil))
			{
				luaScript.Coroutine = luaScript.Script.Globals.Get("Coroutine");
			}
			if (object.Equals(luaScript.Coroutine, DynValue.Nil))
			{
				luaScript.Coroutine = null;
			}
			else
			{
				DynValue coroutine = luaScript.Coroutine;
				luaScript.Coroutine = luaScript.Script.CreateCoroutine(coroutine);
				_luaScriptCoroutine.Add(luaScript);
			}
			luaScript.Exit = luaScript.Script.Globals.Get("Exit" + 3u);
			if (object.Equals(luaScript.Exit, DynValue.Nil))
			{
				luaScript.Exit = luaScript.Script.Globals.Get("Exit");
			}
			if (object.Equals(luaScript.Exit, DynValue.Nil))
			{
				luaScript.Exit = null;
			}
			_luaScripts.Add(luaScript);
		}
	}

	private void LuaStart()
	{
		int i = 0;
		for (int num = _luaScripts.Count; i < num; i++)
		{
			LuaScript luaScript = _luaScripts[i];
			if (luaScript.Start != null)
			{
				try
				{
					luaScript.Script.Call(luaScript.Start);
				}
				catch (ScriptRuntimeException ex)
				{
					Debug.LogError(ex.DecoratedMessage);
					UiMessageBox.Create(ex.DecoratedMessage, "Script Exception").Popup();
					_luaScripts.Remove(luaScript);
					_luaScriptUpdate.Remove(luaScript);
					_luaScriptCoroutine.Remove(luaScript);
					i--;
					num--;
				}
			}
		}
	}

	private List<string[]> LoadScripts()
	{
		string[] files = Directory.GetFiles(Application.streamingAssetsPath + "/lua/");
		List<string[]> list = new List<string[]>();
		string[] array = files;
		foreach (string text in array)
		{
			if (text.EndsWith(".lua"))
			{
				string fileName = Path.GetFileName(text);
				string text2 = LoadLuaFile(text);
				list.Add(new string[2] { fileName, text2 });
			}
		}
		return list;
	}

	private static string LoadLuaFile(string path)
	{
		return File.ReadAllText(path);
	}

	private static void Log(string message)
	{
		Debug.Log(message);
	}

	private Script InitializeScript()
	{
		Script script = new Script();
		object key = "Log";
		script.Globals[key] = new Action<string>(Log);
		object key2 = "log";
		script.Globals[key2] = new Action<string>(Log);
		object key3 = "world";
		script.Globals[key3] = _world;
		object key4 = "gts";
		script.Globals[key4] = _gts;
		object key5 = "micros";
		script.Globals[key5] = _micros;
		object key6 = "globals";
		script.Globals[key6] = _globalTable;
		object key7 = "Animation";
		script.Globals[key7] = typeof(Lua.Animation);
		object key8 = "AudioSource";
		script.Globals[key8] = typeof(Lua.AudioSource);
		object key9 = "Entity";
		script.Globals[key9] = typeof(Entity);
		object key10 = "Event";
		script.Globals[key10] = typeof(Lua.Event);
		object key11 = "EventCode";
		script.Globals[key11] = typeof(EventCode);
		object key12 = "Game";
		script.Globals[key12] = typeof(Game);
		object key13 = "Input";
		script.Globals[key13] = typeof(Lua.Input);
		object key14 = "Mathf";
		script.Globals[key14] = typeof(Lua.Mathf);
		object key15 = "Quaternion";
		script.Globals[key15] = typeof(Lua.Quaternion);
		object key16 = "Random";
		script.Globals[key16] = typeof(Lua.Random);
		object key17 = "Screen";
		script.Globals[key17] = typeof(Lua.Screen);
		object key18 = "CustomSoundManager";
		script.Globals[key18] = typeof(CustomSoundManager);
		object key19 = "Time";
		script.Globals[key19] = typeof(Lua.Time);
		object key20 = "Vector3";
		script.Globals[key20] = typeof(Lua.Vector3);
		script.Options.DebugPrint = Log;
		script.Options.ScriptLoader = new StreamingAssetsScriptLoader();
		script.DoString(_initializationCode);
		return script;
	}
}
