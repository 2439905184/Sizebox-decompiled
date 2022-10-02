using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Lua;
using MoonSharp.Interpreter;
using UnityEngine;

[DebuggerDisplay("{_internalName}")]
public class LuaBehavior : IBehavior
{
	public enum BehaviorSettingType
	{
		Bool = 1,
		String = 2,
		Float = 3,
		Array = 4,
		KeyBind = 5
	}

	public struct BehaviorSetting
	{
		public string VariableName;

		public string UIText;

		public BehaviorSettingType Type;

		public DynValue Value;

		public DynValue Array;
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass35_0
	{
		public LuaBehaviorInstance inst;

		internal void _003CUpdateInstanceScriptSettings_003Eb__0(BehaviorSetting setting)
		{
			inst.Instance.Set(setting.VariableName, setting.Value);
		}
	}

	public bool Enabled = true;

	private bool _forceAppearInManager;

	public string Filename = "";

	public string FullFilename = "";

	private string _internalName;

	private string _text = "";

	private EntityDef _agentDef;

	private EntityDef _targetDef;

	public bool AI;

	public bool CanUseAi;

	private bool _secondary;

	private readonly List<string> _flags;

	private string _desc = "";

	private readonly List<string> _tags;

	private Script _script;

	public List<BehaviorScore> Scores;

	public readonly List<BehaviorSetting> Settings;

	private bool _react;

	private bool _hidden;

	private Table _table;

	private DynValue _functionCreateInstance;

	public LuaBehavior(Table t, Script s)
	{
		_flags = new List<string>();
		_tags = new List<string>();
		Settings = new List<BehaviorSetting>();
		_agentDef.exclude = new List<TargetType>();
		_agentDef.include = new List<TargetType>();
		_targetDef.exclude = new List<TargetType>();
		_targetDef.include = new List<TargetType>();
		SetScript(t, s);
	}

	public void SetScript(Table t, Script s)
	{
		_script = s;
		_table = t;
		_flags.Clear();
		_tags.Clear();
		Settings.Clear();
		_agentDef.exclude.Clear();
		_agentDef.include.Clear();
		_targetDef.exclude.Clear();
		_targetDef.include.Clear();
		_internalName = "";
		foreach (TablePair pair in _table.Pairs)
		{
			if (pair.Key.Type == DataType.String)
			{
				switch (pair.Key.String)
				{
				case "name":
					_internalName = pair.Value.String;
					break;
				case "data":
					ParseData(pair.Value.Table);
					break;
				case "agentType":
					_agentDef.include.Add((TargetType)Enum.Parse(typeof(TargetType), pair.Value.String, true));
					break;
				case "targetType":
					_targetDef.include.Add((TargetType)Enum.Parse(typeof(TargetType), pair.Value.String, true));
					break;
				case "scores":
					AddScores(pair.Value);
					break;
				case "react":
					_react = pair.Value.Boolean;
					break;
				case "hidden":
					_hidden = pair.Value.Boolean;
					break;
				}
			}
		}
		IOManager.Instance.LoadSettings(this);
		if (_targetDef.include.Count == 0)
		{
			_targetDef.include.Add(TargetType.None);
		}
		_functionCreateInstance = _script.Globals.Get("CreateInstance");
		if (AI)
		{
			CanUseAi = true;
		}
	}

	private void RegisterSetting(Table s)
	{
		if (s.Get(1).String.Length == 0 || s.Get(1).String.Contains(" ") || ((s.Get(1).String.ToLower()[0] < 'a' || s.Get(1).String.ToLower()[0] > 'z') && s.Get(1).String.ToLower()[0] != '_'))
		{
			UnityEngine.Debug.LogError("RegisterSetting() - Failed to register setting \"" + s.Get(1).String + "\" for behavior \"" + _text + "\" (Invalid variable name)");
			return;
		}
		if (s.Get(2).String.Length == 0 || s.Get(2).String.Replace(" ", "").Length == 0)
		{
			UnityEngine.Debug.LogError("RegisterSetting() - Failed to register setting \"" + s.Get(2).String + "\" for behavior \"" + _text + "\" (Invalid UI text)");
			return;
		}
		BehaviorSetting item = default(BehaviorSetting);
		try
		{
			item.VariableName = s.Get(1).String;
			item.UIText = s.Get(2).String;
			item.Type = (BehaviorSettingType)Enum.Parse(typeof(BehaviorSettingType), s.Get(3).String, true);
			switch (item.Type)
			{
			case BehaviorSettingType.Bool:
				item.Value = DynValue.NewBoolean(s.Get(4).Boolean);
				break;
			case BehaviorSettingType.String:
				item.Value = DynValue.NewString(s.Get(4).String);
				break;
			case BehaviorSettingType.Float:
			{
				double? num = s.Get(4).CastToNumber();
				if (num.HasValue)
				{
					item.Value = DynValue.NewNumber(num.Value);
					if (s.Length > 4)
					{
						item.Array = DynValue.NewTable(s.Get(5).Table);
					}
				}
				else
				{
					UnityEngine.Debug.LogErrorFormat("'{0}' could not be understood", item.VariableName);
				}
				break;
			}
			case BehaviorSettingType.Array:
				item.Value = DynValue.NewNumber(s.Get(4).Number);
				item.Array = DynValue.NewTable(s.Get(5).Table);
				break;
			case BehaviorSettingType.KeyBind:
				try
				{
					UnityEngine.Input.GetKey(s.Get(4).String[0].ToString());
					item.Value = DynValue.NewString(s.Get(4).String[0].ToString());
				}
				catch
				{
					UnityEngine.Debug.LogError("RegisterSetting() - Failed to register setting \"" + s.Get(1).String + "\" for behavior \"" + _text + "\" (Invalid default key bind)");
					return;
				}
				break;
			}
			Settings.Add(item);
		}
		catch
		{
			UnityEngine.Debug.LogError("RegisterSetting() - Failed to register setting \"" + s.Get(1).String + "\" for behavior \"" + _text + "\" (Bad settingType \"" + s.Get(3).String + "\")");
		}
	}

	public bool HasTag(string tagName)
	{
		return _tags.Contains(tagName);
	}

	public bool AddTag(string tagName)
	{
		tagName = tagName.Trim().ToLower();
		if (tagName.Length == 0)
		{
			return false;
		}
		string[] array = new string[10] { "gts", "shrink", "grow", "foot", "sounds", "size", "moving", "moveing", "morphing", "morpheing" };
		string[] array2 = new string[10] { "giantess", "shrinking", "growing", "feet", "sound", "sizechange", "movement", "movement", "morphs", "morphs" };
		for (int i = 0; i < array.Length; i++)
		{
			if (!(array[i] != tagName))
			{
				UnityEngine.Debug.LogWarning("Behavior " + _text + ": Tag \"" + tagName + "\" should be changed to \"" + array2[i] + "\" for simplicity! Please do that before releasing this script.");
				tagName = array2[i];
				break;
			}
		}
		if (_tags.Contains(tagName))
		{
			UnityEngine.Debug.LogError("Behavior " + _text + ": Tag \"" + tagName + "\" is duplicated; Please fix this before releasing this script.");
			return false;
		}
		_tags.Add(tagName);
		return true;
	}

	public string GetTag(int index)
	{
		if (index < 0 || index >= _tags.Count)
		{
			UnityEngine.Debug.LogError("GetTag() - Index out of range; index = " + index + ", size = " + _tags.Count);
			return null;
		}
		return _tags[index];
	}

	public int GetTagCount()
	{
		return _tags.Count;
	}

	public void ClearTags()
	{
		_tags.Clear();
	}

	public bool RemoveTag(int index)
	{
		if (index < 0 || index >= _tags.Count)
		{
			UnityEngine.Debug.LogError("GetTag() - Index out of range; index = " + index + ", size = " + _tags.Count);
			return false;
		}
		_tags.RemoveAt(index);
		return true;
	}

	private void ParseData(Table data)
	{
		foreach (TablePair pair in data.Pairs)
		{
			string @string = pair.Key.String;
			switch (@string)
			{
			case "secondary":
				_secondary = pair.Value.Boolean;
				break;
			case "flags":
				foreach (TablePair pair2 in pair.Value.Table.Pairs)
				{
					_flags.Add(pair2.Value.String);
				}
				_secondary = true;
				break;
			case "canUseAI":
			case "ai":
				CanUseAi = @string == "ai" || pair.Value.Boolean;
				break;
			case "menuEntry":
				_text = pair.Value.String;
				break;
			case "agent":
				_agentDef = ParseEntityDef(pair.Value.Table, _agentDef);
				break;
			case "target":
				_targetDef = ParseEntityDef(pair.Value.Table, _targetDef);
				break;
			case "hideMenu":
				_hidden = pair.Value.Boolean;
				break;
			case "settings":
				foreach (TablePair pair3 in pair.Value.Table.Pairs)
				{
					Table table = pair3.Value.Table;
					RegisterSetting(table);
				}
				break;
			case "forceAppearInManager":
				_forceAppearInManager = pair.Value.Boolean;
				break;
			case "description":
				_desc = pair.Value.String;
				break;
			case "tags":
			{
				string[] array = pair.Value.String.Split(',');
				foreach (string tagName in array)
				{
					AddTag(tagName);
				}
				break;
			}
			}
		}
	}

	private EntityDef ParseEntityDef(Table data, EntityDef def)
	{
		foreach (TablePair pair in data.Pairs)
		{
			string @string = pair.Key.String;
			if (@string == "type")
			{
				foreach (TablePair pair2 in pair.Value.Table.Pairs)
				{
					def.include.Add((TargetType)Enum.Parse(typeof(TargetType), pair2.Value.String, true));
				}
			}
			else
			{
				if (!(@string == "exclude"))
				{
					continue;
				}
				foreach (TablePair pair3 in pair.Value.Table.Pairs)
				{
					def.exclude.Add((TargetType)Enum.Parse(typeof(TargetType), pair3.Value.String, true));
				}
			}
		}
		return def;
	}

	public IBehaviorInstance CreateInstance(EntityBase agent, EntityBase target, UnityEngine.Vector3 cursorPoint)
	{
		LuaBehaviorInstance luaBehaviorInstance = null;
		Entity luaEntity = agent.GetLuaEntity();
		Entity entity = null;
		if (target != null)
		{
			entity = target.GetLuaEntity();
		}
		try
		{
			luaBehaviorInstance = new LuaBehaviorInstance(_script.Call(_functionCreateInstance, _internalName, luaEntity, entity, new Lua.Vector3(cursorPoint)).Table, _script)
			{
				behaviorFilename = Filename,
				behaviorName = _internalName
			};
			UpdateInstanceScriptSettings(luaBehaviorInstance);
			return luaBehaviorInstance;
		}
		catch (ScriptRuntimeException ex)
		{
			UnityEngine.Debug.LogError(ex.DecoratedMessage);
			return luaBehaviorInstance;
		}
	}

	private void UpdateInstanceScriptSettings(LuaBehaviorInstance inst)
	{
		_003C_003Ec__DisplayClass35_0 _003C_003Ec__DisplayClass35_ = new _003C_003Ec__DisplayClass35_0();
		_003C_003Ec__DisplayClass35_.inst = inst;
		Settings.ForEach(_003C_003Ec__DisplayClass35_._003CUpdateInstanceScriptSettings_003Eb__0);
	}

	public bool CanAppearInBehaviorManager()
	{
		if (_hidden)
		{
			return _forceAppearInManager;
		}
		return true;
	}

	public bool IsReactive()
	{
		return _react;
	}

	public bool IsHidden()
	{
		if (!_hidden)
		{
			return !Enabled;
		}
		return true;
	}

	public string GetName()
	{
		return _internalName;
	}

	public string GetDesc()
	{
		return _desc;
	}

	public EntityDef GetAgentDef()
	{
		return _agentDef;
	}

	public EntityDef GetTargetDef()
	{
		return _targetDef;
	}

	public string GetText()
	{
		if (_text.Length == 0)
		{
			return GetName();
		}
		return _text;
	}

	public bool IsSecondary()
	{
		return _secondary;
	}

	public bool IsEnabled()
	{
		return Enabled;
	}

	public bool IsAI()
	{
		return AI;
	}

	public bool CanUseAI()
	{
		return CanUseAi;
	}

	public List<string> GetFlags()
	{
		return _flags;
	}

	private void AddScores(DynValue value)
	{
		if (value.Type != DataType.Table)
		{
			UnityEngine.Debug.LogError("behavior.scores must be in a table");
			return;
		}
		Scores = new List<BehaviorScore>();
		foreach (TablePair pair in value.Table.Pairs)
		{
			string @string = pair.Key.String;
			float val = (float)pair.Value.Number;
			try
			{
				BehaviorScore item = new BehaviorScore(@string, val);
				Scores.Add(item);
			}
			catch
			{
			}
		}
		AI = true;
	}
}
