using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MoonSharp.Interpreter;
using Pause;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SizeboxUI
{
	public class BehaviorSettingsView : SettingsView
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass20_0
		{
			public GameObject newButton;

			public BehaviorSettingsView _003C_003E4__this;

			internal void _003CInitialize_003Eb__0()
			{
				_003C_003E4__this.SetCurrentTab(newButton.name);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass25_0
		{
			public BehaviorSettingsView _003C_003E4__this;

			public InputField inputField;

			public Text placeHolderText;

			public UnityAction<string> _003C_003E9__2;

			internal void _003CPopulateTags_003Eb__0(string input)
			{
				string text = "";
				string text2 = input;
				input = input.Trim().ToLower();
				if (!(input == "") || !(text2 == ""))
				{
					if (text == "" && input == "" && text2 != "")
					{
						text = "Invalid tag name";
					}
					if (text == "" && _003C_003E4__this._curTagList.Contains(input))
					{
						text = "Cannot add a duplicate tag";
					}
					if (text == "" && !_003C_003E4__this.AddTag(input))
					{
						text = "Internal error: AddTag() returned false";
					}
					inputField.text = "";
					if (text != "")
					{
						inputField.onValueChanged.AddListener(_003C_003E9__2 ?? (_003C_003E9__2 = _003CPopulateTags_003Eb__2));
						placeHolderText.color = new Color(1f, 0f, 0f, 1f);
						placeHolderText.text = "ERROR: " + text;
						Debug.LogWarning("ERROR: " + text);
					}
				}
			}

			internal void _003CPopulateTags_003Eb__2(string inputText)
			{
				placeHolderText.text = "Add Tag... (Type Here)";
				placeHolderText.color = new Color(1f, 1f, 1f, 0.498f);
				inputField.onValueChanged.RemoveAllListeners();
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass25_1
		{
			public GameObject newTag;

			public _003C_003Ec__DisplayClass25_0 CS_0024_003C_003E8__locals1;

			internal void _003CPopulateTags_003Eb__1()
			{
				CS_0024_003C_003E8__locals1._003C_003E4__this.RemoveTag(newTag.GetComponentInChildren<Text>().text);
			}
		}

		private static ColorBlock _tabColorSelected;

		private static ColorBlock _tabColorUnselected;

		private BehaviorManagerView _mgrView;

		private LuaBehavior _bCmd;

		private Toggle _enabled;

		private Toggle _aiEnabled;

		private GameObject _addTagResource;

		private GameObject _tabResource;

		private GameObject _tagResource;

		private int _skipNo;

		private readonly Dictionary<GameObject, Button> _tabButtonTable = new Dictionary<GameObject, Button>();

		private readonly List<string> _curTagList = new List<string>();

		private Text _behNameText;

		private Text _behDescText;

		private Transform _tagGroup;

		private Transform _tabGroup;

		private Transform _tabButtonGroup;

		private bool _initCache;

		public void SetBehaviorManager(BehaviorManagerView mgr)
		{
			_mgrView = mgr;
		}

		private void Start()
		{
			Initialize();
		}

		public void Initialize()
		{
			if (initialized)
			{
				return;
			}
			if (!_initCache)
			{
				_initCache = true;
				_tabColorSelected.normalColor = new Color(1f, 1f, 1f);
				_tabColorUnselected.normalColor = new Color(0.75f, 0.75f, 0.75f);
				Color disabledColor = (_tabColorUnselected.disabledColor = new Color(0.784f, 0.784f, 0.784f));
				_tabColorSelected.disabledColor = disabledColor;
				disabledColor = (_tabColorUnselected.highlightedColor = new Color(0.96f, 0.96f, 0.96f));
				_tabColorSelected.highlightedColor = disabledColor;
				disabledColor = (_tabColorUnselected.pressedColor = new Color(0.784f, 0.784f, 0.784f));
				_tabColorSelected.pressedColor = disabledColor;
				float fadeDuration = (_tabColorUnselected.fadeDuration = 0.1f);
				_tabColorSelected.fadeDuration = fadeDuration;
				fadeDuration = (_tabColorUnselected.colorMultiplier = 1f);
				_tabColorSelected.colorMultiplier = fadeDuration;
				_addTagResource = Resources.Load<GameObject>("UI/BehaviorMgr/BehMgr_Settings_AddTag");
				_tabResource = Resources.Load<GameObject>("UI/BehaviorMgr/BehMgr_Settings_Tab");
				_tagResource = Resources.Load<GameObject>("UI/BehaviorMgr/BehMgr_Settings_Tag");
				_behNameText = base.transform.Find("Decoration").Find("TabGroup").Find("BasicInfoTab")
					.Find("BehName")
					.GetComponent<Text>();
				_behDescText = base.transform.Find("Decoration").Find("TabGroup").Find("BasicInfoTab")
					.Find("DescPanel")
					.Find("BehDesc")
					.GetComponent<Text>();
				_tagGroup = base.transform.Find("Decoration").Find("TabGroup").Find("BasicInfoTab")
					.Find("TagsPanel")
					.Find("TagContent");
				_tabGroup = base.transform.Find("Decoration").Find("TabGroup");
				_tabButtonGroup = base.transform.Find("Decoration").Find("TabButtonGroup");
				gridGroup = base.transform.Find("Decoration").Find("TabGroup").Find("SettingsTab")
					.Find("SettingsPanel")
					.Find("SettingsContent")
					.GetComponent<GridLayoutGroup>();
			}
			base.transform.Find("Cancel").GetComponent<Button>().onClick.AddListener(Close);
			base.transform.Find("Apply").GetComponent<Button>().onClick.AddListener(ApplySettings);
			for (int i = 0; i < _tabButtonGroup.childCount; i++)
			{
				Object.Destroy(_tabButtonGroup.GetChild(i).gameObject);
			}
			_tabButtonTable.Clear();
			for (int j = 0; j < _tabGroup.childCount; j++)
			{
				_003C_003Ec__DisplayClass20_0 _003C_003Ec__DisplayClass20_ = new _003C_003Ec__DisplayClass20_0();
				_003C_003Ec__DisplayClass20_._003C_003E4__this = this;
				GameObject gameObject = _tabGroup.GetChild(j).gameObject;
				string text = gameObject.name;
				string text2 = "";
				if (text.EndsWith("Tab"))
				{
					text = text.Remove(text.Length - 3, 3);
				}
				for (int k = 0; k < text.Length; k++)
				{
					text2 = ((k == 0 || !char.IsUpper(text[k])) ? (text2 + text.Substring(k, 1)) : (text2 + " " + text.Substring(k, 1)));
				}
				text2 = text2.Trim();
				_003C_003Ec__DisplayClass20_.newButton = Object.Instantiate(_tabResource, _tabButtonGroup, true);
				_003C_003Ec__DisplayClass20_.newButton.name = gameObject.name + "Btn";
				_003C_003Ec__DisplayClass20_.newButton.GetComponentInChildren<Text>().text = text2;
				_003C_003Ec__DisplayClass20_.newButton.GetComponent<Button>().onClick.AddListener(_003C_003Ec__DisplayClass20_._003CInitialize_003Eb__0);
				_tabButtonTable.Add(gameObject, _003C_003Ec__DisplayClass20_.newButton.GetComponent<Button>());
			}
			SetCurrentTab(0);
			AddHeader("Basic Settings");
			_enabled = AddToggle("Enable Behavior");
			AddHeader("AI Settings");
			_aiEnabled = AddToggle("Allow AI use");
			AddHeader("Script Settings");
			_skipNo = gridGroup.transform.childCount;
			_bCmd = null;
			initialized = true;
		}

		private void Close()
		{
			base.gameObject.SetActive(false);
		}

		private void ApplySettings()
		{
			if (_bCmd == null)
			{
				return;
			}
			_bCmd.Enabled = _enabled.isOn;
			_bCmd.AI = _bCmd.CanUseAI() && _aiEnabled.isOn;
			for (int i = 0; i < _bCmd.Settings.Count; i++)
			{
				LuaBehavior.BehaviorSetting value = _bCmd.Settings[i];
				GameObject gameObject = gridGroup.transform.GetChild(_skipNo + i).gameObject;
				switch (value.Type)
				{
				case LuaBehavior.BehaviorSettingType.Bool:
					value.Value = DynValue.NewBoolean(gameObject.GetComponent<Toggle>().isOn);
					break;
				case LuaBehavior.BehaviorSettingType.String:
					value.Value = DynValue.NewString(gameObject.GetComponentInChildren<InputField>().text);
					break;
				case LuaBehavior.BehaviorSettingType.Float:
					value.Value = DynValue.NewNumber(gameObject.GetComponentInChildren<Slider>().value);
					break;
				case LuaBehavior.BehaviorSettingType.Array:
					value.Value = DynValue.NewNumber(gameObject.GetComponentInChildren<Dropdown>().value);
					break;
				case LuaBehavior.BehaviorSettingType.KeyBind:
				{
					string text = gameObject.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text;
					if (text[0] == '\'' && text[text.Length - 1] == '\'')
					{
						value.Value = DynValue.NewString(text.Substring(1, text.Length - 2));
					}
					break;
				}
				}
				_bCmd.Settings[i] = value;
			}
			_bCmd.ClearTags();
			foreach (string curTag in _curTagList)
			{
				_bCmd.AddTag(curTag);
			}
			IOManager.Instance.SaveSettings(_bCmd);
			_mgrView.ReloadBehaviorList();
			Close();
		}

		private bool AddTag(string tagName)
		{
			tagName = tagName.ToLower().Trim();
			if (tagName == "" || _curTagList.Contains(tagName.ToLower()))
			{
				return false;
			}
			_curTagList.Add(tagName.ToLower());
			PopulateTags();
			return true;
		}

		private void RemoveTag(string tagName)
		{
			_curTagList.Remove(tagName);
			PopulateTags();
		}

		private void PopulateTags()
		{
			_003C_003Ec__DisplayClass25_0 _003C_003Ec__DisplayClass25_ = new _003C_003Ec__DisplayClass25_0();
			_003C_003Ec__DisplayClass25_._003C_003E4__this = this;
			if (_bCmd == null)
			{
				return;
			}
			for (int i = 0; i < _tagGroup.childCount; i++)
			{
				Object.Destroy(_tagGroup.GetChild(i).gameObject);
			}
			foreach (string curTag in _curTagList)
			{
				_003C_003Ec__DisplayClass25_1 _003C_003Ec__DisplayClass25_2 = new _003C_003Ec__DisplayClass25_1();
				_003C_003Ec__DisplayClass25_2.CS_0024_003C_003E8__locals1 = _003C_003Ec__DisplayClass25_;
				_003C_003Ec__DisplayClass25_2.newTag = Object.Instantiate(_tagResource, _tagGroup, false);
				_003C_003Ec__DisplayClass25_2.newTag.GetComponentInChildren<Text>().text = curTag;
				_003C_003Ec__DisplayClass25_2.newTag.GetComponentInChildren<Button>().onClick.AddListener(_003C_003Ec__DisplayClass25_2._003CPopulateTags_003Eb__1);
			}
			GameObject gameObject = Object.Instantiate(_addTagResource, _tagGroup, false);
			_003C_003Ec__DisplayClass25_.inputField = gameObject.GetComponentInChildren<InputField>();
			_003C_003Ec__DisplayClass25_.placeHolderText = _003C_003Ec__DisplayClass25_.inputField.transform.Find("Placeholder").GetComponent<Text>();
			_003C_003Ec__DisplayClass25_.inputField.onEndEdit.AddListener(_003C_003Ec__DisplayClass25_._003CPopulateTags_003Eb__0);
		}

		private void PopulateScriptSettings()
		{
			if (_bCmd == null)
			{
				return;
			}
			for (int i = _skipNo; i < gridGroup.transform.childCount; i++)
			{
				Object.Destroy(gridGroup.transform.GetChild(i).gameObject);
			}
			foreach (LuaBehavior.BehaviorSetting setting in _bCmd.Settings)
			{
				switch (setting.Type)
				{
				case LuaBehavior.BehaviorSettingType.Bool:
					AddToggle(setting.UIText).isOn = setting.Value.Boolean;
					break;
				case LuaBehavior.BehaviorSettingType.String:
					AddInput(setting.UIText, "").text = setting.Value.String;
					break;
				case LuaBehavior.BehaviorSettingType.Float:
				{
					Slider slider = AddSlider(setting.UIText, 0f, 1f);
					if (setting.Array != null)
					{
						double? num = setting.Array.Table.Get(1).CastToNumber();
						double? num2 = setting.Array.Table.Get(2).CastToNumber();
						if (num.HasValue && num2.HasValue && num.Value < num2.Value)
						{
							slider.minValue = (float)num.Value;
							slider.maxValue = (float)num2.Value;
						}
					}
					slider.value = (float)setting.Value.Number;
					break;
				}
				case LuaBehavior.BehaviorSettingType.Array:
				{
					List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();
					foreach (TablePair pair in setting.Array.Table.Pairs)
					{
						Dropdown.OptionData item = new Dropdown.OptionData
						{
							text = pair.Value.ToPrintString()
						};
						list.Add(item);
					}
					AddDropdown(setting.UIText, list).value = (int)setting.Value.Number;
					break;
				}
				case LuaBehavior.BehaviorSettingType.KeyBind:
					AddCustomKeyBind(setting.UIText).GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = "'" + setting.Value.String + "'";
					break;
				}
			}
		}

		private void SetCurrentTab(int index)
		{
			if (index < 0 || index >= _tabGroup.childCount)
			{
				Debug.LogError("SetCurrentTab() - Index is out of range; index = " + index + ", size = " + _tabGroup.childCount);
				return;
			}
			for (int i = 0; i < _tabGroup.childCount; i++)
			{
				GameObject gameObject = _tabGroup.GetChild(i).gameObject;
				_tabButtonTable[gameObject].colors = ((i == index) ? _tabColorSelected : _tabColorUnselected);
				if (gameObject.activeInHierarchy != (i == index))
				{
					gameObject.SetActive(i == index);
				}
			}
		}

		private void SetCurrentTab(string tabText)
		{
			for (int i = 0; i < _tabGroup.childCount; i++)
			{
				if (!(tabText != _tabGroup.GetChild(i).name + "Btn"))
				{
					SetCurrentTab(i);
					return;
				}
			}
			Debug.LogError("SetCurrentTab() - Failed to find tab \"" + tabText + "\"");
		}

		public bool SetBehavior(IBehavior bObject)
		{
			_bCmd = bObject as LuaBehavior;
			if (_bCmd == null)
			{
				Debug.Log("bObject isn't a LuaBehavior? What?");
				return false;
			}
			if (!initialized)
			{
				Initialize();
			}
			_curTagList.Clear();
			for (int i = 0; i < _bCmd.GetTagCount(); i++)
			{
				_curTagList.Add(_bCmd.GetTag(i));
			}
			SetCurrentTab(0);
			PopulateScriptSettings();
			PopulateTags();
			UpdateValues();
			return true;
		}

		protected override void UpdateValues()
		{
			if (_bCmd != null)
			{
				if (_bCmd.CanUseAI())
				{
					_aiEnabled.isOn = _bCmd.IsAI();
					_aiEnabled.interactable = true;
					((Image)_aiEnabled.graphic).sprite = ((Image)_enabled.graphic).sprite;
				}
				else
				{
					_aiEnabled.isOn = true;
					_aiEnabled.interactable = false;
					((Image)_aiEnabled.graphic).sprite = Resources.Load<Sprite>("UI/BehaviorMgr/BM_DisabledCheck");
				}
				_enabled.isOn = _bCmd.IsEnabled();
				_behNameText.text = _bCmd.GetText().Split('/')[_bCmd.GetText().Split('/').Length - 1];
				_behDescText.text = ((_bCmd.GetDesc() == "") ? "(No description available)" : _bCmd.GetDesc());
			}
		}
	}
}
