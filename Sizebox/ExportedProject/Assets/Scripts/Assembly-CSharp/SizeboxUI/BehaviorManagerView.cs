using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SizeboxUI
{
	public class BehaviorManagerView : MonoBehaviour
	{
		private class BehaviorPath
		{
			public string Name;

			public GameObject ListEntry;

			public List<BehaviorPath> SubPaths;

			public List<LuaBehavior> BehList;
		}

		private struct TagInfo
		{
			public List<LuaBehavior> BehList;

			public List<GameObject> GoList;
		}

		private enum VisibleState
		{
			All = 0,
			Untagged = 1,
			Tag = 2
		}

		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Comparison<GameObject> _003C_003E9__36_0;

			public static Comparison<GameObject> _003C_003E9__36_1;

			public static Predicate<IBehavior> _003C_003E9__48_0;

			internal int _003CSort_003Eb__36_0(GameObject a, GameObject b)
			{
				if (a == null && b == null)
				{
					return 0;
				}
				if (a == null)
				{
					return -1;
				}
				if (b == null)
				{
					return 1;
				}
				return string.Compare(a.GetComponentInChildren<Text>().text, b.GetComponentInChildren<Text>().text, StringComparison.Ordinal);
			}

			internal int _003CSort_003Eb__36_1(GameObject a, GameObject b)
			{
				if (a == null && b == null)
				{
					return 0;
				}
				if (a == null)
				{
					return -1;
				}
				if (b == null)
				{
					return 1;
				}
				if (b.name.StartsWith("PathObj_") && !a.name.StartsWith("PathObj_"))
				{
					return 1;
				}
				if (!b.name.StartsWith("PathObj_") && a.name.StartsWith("PathObj_"))
				{
					return -1;
				}
				return string.Compare(a.GetComponentInChildren<Text>().text, b.GetComponentInChildren<Text>().text, StringComparison.Ordinal);
			}

			internal bool _003CReloadBehaviorList_003Eb__48_0(IBehavior beh)
			{
				return beh.CanAppearInBehaviorManager();
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass41_0
		{
			public BehaviorManagerView _003C_003E4__this;

			public GameObject newPath;

			internal void _003CCreateBehaviorPathObject_003Eb__0()
			{
				_003C_003E4__this.ToggleCategoryExpanded(newPath);
			}

			internal void _003CCreateBehaviorPathObject_003Eb__1(bool val)
			{
				_003C_003E4__this.SetCategoryExpanded(newPath, val);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass48_0
		{
			public IBehavior bObj;

			public BehaviorManagerView _003C_003E4__this;

			internal void _003CReloadBehaviorList_003Eb__2()
			{
				_003C_003E4__this.OnClickElement(bObj);
			}

			internal void _003CReloadBehaviorList_003Eb__3(bool val)
			{
				LuaBehavior luaBehavior;
				if ((luaBehavior = bObj as LuaBehavior) != null)
				{
					luaBehavior.AI = val;
					IOManager.Instance.SaveSettings(luaBehavior);
				}
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass48_1
		{
			public string tagName;

			public BehaviorManagerView _003C_003E4__this;

			public UnityAction _003C_003E9__5;

			internal void _003CReloadBehaviorList_003Eb__4(GameObject go)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(_003C_003E4__this._tagResource, go.transform.Find("TagList"), false);
				gameObject.GetComponentInChildren<Text>().text = tagName;
				gameObject.GetComponent<Button>().onClick.AddListener(_003C_003E9__5 ?? (_003C_003E9__5 = _003CReloadBehaviorList_003Eb__5));
			}

			internal void _003CReloadBehaviorList_003Eb__5()
			{
				_003C_003E4__this._searchType.value = _003C_003E4__this.GetTagInfoIndex((string)tagName.Clone()) + 2;
				_003C_003E4__this._curVisState = VisibleState.Tag;
				_003C_003E4__this._curVisibleTag = (string)tagName.Clone();
				_003C_003E4__this.Sort();
			}
		}

		public UnityAction Hide;

		private Button _editButton;

		private bool _initialized;

		private Button _buttonPrefab;

		private Transform _listCanvas;

		private GameObject _settingsView;

		private IBehavior _selectedBehavior;

		private GameObject _tagResource;

		private GameObject _subPathResource;

		private InputField _searchBar;

		private Dropdown _searchType;

		private ColorBlock _tagColorNormal;

		private ColorBlock _tagColorSearchInactive;

		private readonly Dictionary<string, bool> _pathVisible = new Dictionary<string, bool>();

		private readonly List<BehaviorPath> _allPaths = new List<BehaviorPath>();

		private readonly List<BehaviorPath> _paths = new List<BehaviorPath>();

		private readonly List<string> _searchTypeOptions = new List<string>();

		private readonly Dictionary<string, TagInfo> _tagStore = new Dictionary<string, TagInfo>();

		private readonly Dictionary<Button, LuaBehavior> _lookupTable = new Dictionary<Button, LuaBehavior>();

		private readonly Dictionary<LuaBehavior, Button> _lookupTableButton = new Dictionary<LuaBehavior, Button>();

		private Sprite _bmDisabledCheck;

		private Sprite _bmLayoutList;

		private Sprite _bmLayoutTree;

		private Button _layoutBtn;

		private Button _reloadBtn;

		private VisibleState _curVisState;

		private string _curVisibleTag = "";

		private string _searchQuery = "";

		public bool useTreeLayout = true;

		private void AddSearchType(string text)
		{
			_searchTypeOptions.Add(text);
			_searchType.ClearOptions();
			_searchType.AddOptions(_searchTypeOptions);
		}

		private void Initialize()
		{
			_tagColorNormal.normalColor = new Color(0f, 73f / 106f, 0f, 32f / 51f);
			_tagColorNormal.highlightedColor = new Color(0f, 0.8392157f, 0f, 0.7843137f);
			_tagColorNormal.pressedColor = new Color(0f, 73f / 106f, 0f, 32f / 51f);
			_tagColorNormal.disabledColor = new Color(0.4206568f, 69f / 106f, 0.4206568f, 0.5019608f);
			_tagColorSearchInactive.normalColor = new Color(0f, 0.462f, 0f, 35f / 51f);
			_tagColorSearchInactive.highlightedColor = new Color(0f, 0.629f, 0f, 0.7843137f);
			_tagColorSearchInactive.pressedColor = new Color(0f, 1f, 0f, 1f);
			_tagColorSearchInactive.disabledColor = new Color(0.4206568f, 69f / 106f, 0.4206568f, 0.5019608f);
			ref ColorBlock tagColorNormal = ref _tagColorNormal;
			float colorMultiplier = (_tagColorSearchInactive.colorMultiplier = 1f);
			tagColorNormal.colorMultiplier = colorMultiplier;
			ref ColorBlock tagColorNormal2 = ref _tagColorNormal;
			colorMultiplier = (_tagColorSearchInactive.fadeDuration = 0.1f);
			tagColorNormal2.fadeDuration = colorMultiplier;
			_bmDisabledCheck = Resources.Load<Sprite>("UI/BehaviorMgr/BM_DisabledCheck");
			_bmLayoutList = Resources.Load<Sprite>("UI/BehaviorMgr/BM_Layout_List");
			_bmLayoutTree = Resources.Load<Sprite>("UI/BehaviorMgr/BM_Layout_Tree");
			_reloadBtn = base.transform.Find("Panel").Find("ManagerButtonsPanel").Find("ReloadButton")
				.GetComponent<Button>();
			_layoutBtn = base.transform.Find("Panel").Find("ManagerButtonsPanel").Find("LayoutButton")
				.GetComponent<Button>();
			base.transform.Find("Panel").Find("ManagerButtonsPanel").Find("EditButton")
				.GetComponent<Button>()
				.onClick.AddListener(EditBehavior);
			base.transform.Find("Panel").Find("ManagerButtonsPanel").Find("CancelButton")
				.GetComponent<Button>()
				.onClick.AddListener(Close);
			_layoutBtn.onClick.AddListener(_003CInitialize_003Eb__33_0);
			_reloadBtn.onClick.AddListener(ReloadScript);
			_reloadBtn.interactable = false;
			_searchBar = base.transform.Find("Panel").Find("SearchPanel").Find("SearchBar")
				.GetComponent<InputField>();
			_searchType = base.transform.Find("Panel").Find("SearchPanel").Find("SearchTag")
				.GetComponent<Dropdown>();
			_searchBar.onValueChanged.AddListener(OnSearchQueryChanged);
			_searchType.onValueChanged.AddListener(OnSearchTypeChanged);
			_subPathResource = Resources.Load<GameObject>("UI/BehaviorMgr/BehMgrSubPath");
			_tagResource = Resources.Load<GameObject>("UI/BehaviorMgr/BehMgrTag");
			_buttonPrefab = Resources.Load<Button>("UI/BehaviorMgr/BehMgrItem");
			_listCanvas = GetComponentInChildren<VerticalLayoutGroup>().transform;
			_initialized = true;
			_selectedBehavior = null;
		}

		private void InitSettingView()
		{
			if (!_settingsView)
			{
				_settingsView = UnityEngine.Object.Instantiate(Resources.Load("UI/BehaviorMgr/BehaviorSettingsMenu") as GameObject, base.transform, false);
				BehaviorSettingsView behaviorSettingsView = _settingsView.AddComponent<BehaviorSettingsView>();
				_settingsView.SetActive(false);
				behaviorSettingsView.SetBehaviorManager(this);
				behaviorSettingsView.Initialize();
			}
		}

		private void Recursive_AddToSortList(List<GameObject> list, List<GameObject> pathList, GameObject entry)
		{
			if (!useTreeLayout)
			{
				list.Add(entry);
				return;
			}
			if (!entry.name.StartsWith("PathObj_"))
			{
				list.Add(entry);
			}
			else
			{
				entry.SetActive(true);
				if (!entry.transform.Find("Panel").GetComponentInChildren<Toggle>().isOn && _searchQuery == "" && _curVisState == VisibleState.All)
				{
					return;
				}
				pathList.Add(entry);
			}
			for (int i = 0; i < entry.transform.childCount; i++)
			{
				GameObject gameObject = entry.transform.GetChild(i).gameObject;
				if (gameObject.name.StartsWith("BehMgrItem") || gameObject.name.StartsWith("PathObj_"))
				{
					Recursive_AddToSortList(list, pathList, gameObject);
				}
			}
		}

		private void Sort()
		{
			List<GameObject> list = new List<GameObject>();
			List<GameObject> list2 = new List<GameObject>();
			_reloadBtn.interactable = false;
			for (int i = 0; i < _listCanvas.childCount; i++)
			{
				Recursive_AddToSortList(list, list2, _listCanvas.GetChild(i).gameObject);
			}
			for (int j = 0; j < list.Count; j++)
			{
				GameObject gameObject = list[j];
				if (_searchQuery != "" && gameObject.GetComponentInChildren<Text>().text.ToLower().IndexOf(_searchQuery, StringComparison.Ordinal) == -1)
				{
					gameObject.SetActive(false);
					list.RemoveAt(j);
					j--;
					continue;
				}
				Transform transform = gameObject.transform.Find("TagList");
				if ((bool)transform)
				{
					for (int k = 0; k < transform.childCount; k++)
					{
						transform.GetChild(k).gameObject.GetComponent<Button>().colors = _tagColorNormal;
					}
				}
				if (!gameObject.activeInHierarchy)
				{
					gameObject.SetActive(true);
				}
			}
			switch (_curVisState)
			{
			case VisibleState.Untagged:
			{
				for (int n = 0; n < list.Count; n++)
				{
					GameObject gameObject4 = list[n];
					if (_lookupTable[gameObject4.GetComponent<Button>()].GetTagCount() == 0)
					{
						gameObject4.SetActive(true);
						continue;
					}
					gameObject4.SetActive(false);
					list.RemoveAt(n);
					n--;
				}
				break;
			}
			case VisibleState.Tag:
			{
				for (int l = 0; l < list.Count; l++)
				{
					GameObject gameObject2 = list[l];
					if (gameObject2.name.StartsWith("PathObj_"))
					{
						continue;
					}
					if (_lookupTable[gameObject2.GetComponent<Button>()].HasTag(_curVisibleTag))
					{
						Transform transform2 = gameObject2.transform.Find("TagList");
						for (int m = 0; m < transform2.childCount; m++)
						{
							GameObject gameObject3 = transform2.GetChild(m).gameObject;
							if (!(transform2.GetChild(m).GetComponentInChildren<Text>().text == _curVisibleTag))
							{
								gameObject3.GetComponent<Button>().colors = _tagColorSearchInactive;
							}
						}
						gameObject2.SetActive(true);
					}
					else
					{
						gameObject2.SetActive(false);
						list.RemoveAt(l);
						l--;
					}
				}
				break;
			}
			default:
				Debug.LogError("BehaviorManagerView::Sort() - Unhandled sorting method '" + _curVisState.ToString() + "'!");
				break;
			case VisibleState.All:
				break;
			}
			if (useTreeLayout)
			{
				for (int num = 0; num < list2.Count; num++)
				{
					GameObject gameObject5 = list2[num];
					int num2 = 0;
					bool flag = !gameObject5.transform.Find("Panel").GetComponentInChildren<Toggle>().isOn;
					for (int num3 = 1; num3 < gameObject5.transform.childCount; num3++)
					{
						Transform child = gameObject5.transform.GetChild(num3);
						num2 += (child.gameObject.activeInHierarchy ? 1 : 0);
						if (flag)
						{
							child.gameObject.SetActive(false);
						}
					}
					Text componentInChildren = gameObject5.transform.Find("Panel").GetComponentInChildren<Text>();
					string text = componentInChildren.text;
					if (text.IndexOf('(') != -1)
					{
						text = text.Substring(0, text.LastIndexOf('(')).Trim();
					}
					if (num2 != 0)
					{
						list.Add(gameObject5);
						if (num2 == gameObject5.transform.childCount - 1)
						{
							componentInChildren.text = text + " (" + num2 + ")";
							continue;
						}
						componentInChildren.text = text + " (" + num2 + " / " + (gameObject5.transform.childCount - 1) + ")";
					}
					else
					{
						gameObject5.SetActive(false);
						list2.RemoveAt(num);
						num--;
					}
				}
			}
			list.Sort(_003C_003Ec._003C_003E9__36_0 ?? (_003C_003Ec._003C_003E9__36_0 = _003C_003Ec._003C_003E9._003CSort_003Eb__36_0));
			for (int num4 = 0; num4 < list.Count; num4++)
			{
				GameObject gameObject6 = list[num4];
				if ((bool)gameObject6.transform.parent && gameObject6.transform.parent.gameObject.name.StartsWith("PathObj_"))
				{
					gameObject6.transform.SetSiblingIndex(num4 + 1);
				}
				else
				{
					gameObject6.transform.SetSiblingIndex(num4);
				}
			}
			foreach (GameObject item in list2)
			{
				list.Clear();
				for (int num5 = 1; num5 < item.transform.childCount; num5++)
				{
					list.Add(item.transform.GetChild(num5).gameObject);
				}
				list.Sort(_003C_003Ec._003C_003E9__36_1 ?? (_003C_003Ec._003C_003E9__36_1 = _003C_003Ec._003C_003E9._003CSort_003Eb__36_1));
				for (int num6 = 0; num6 < list.Count; num6++)
				{
					list[num6].transform.SetSiblingIndex(num6 + 1);
				}
			}
		}

		private void AssociateBehaviorWithTag(LuaBehavior beh, string tagName)
		{
			if (!_tagStore.ContainsKey(tagName))
			{
				TagInfo tagInfo = default(TagInfo);
				tagInfo.BehList = new List<LuaBehavior>();
				tagInfo.GoList = new List<GameObject>();
				TagInfo value = tagInfo;
				_tagStore.Add(tagName, value);
			}
			_tagStore[tagName].BehList.Add(beh);
		}

		private void AssociateListEntryWithTag(GameObject go, string tagName)
		{
			if (!_tagStore.ContainsKey(tagName))
			{
				TagInfo tagInfo = default(TagInfo);
				tagInfo.BehList = new List<LuaBehavior>();
				tagInfo.GoList = new List<GameObject>();
				TagInfo value = tagInfo;
				_tagStore.Add(tagName, value);
			}
			_tagStore[tagName].GoList.Add(go);
		}

		private BehaviorPath GetPath(string pathName, BehaviorPath newParent = null, bool create = true)
		{
			foreach (BehaviorPath allPath in _allPaths)
			{
				if (!(allPath.Name != pathName))
				{
					return allPath;
				}
			}
			if (!create)
			{
				return null;
			}
			BehaviorPath behaviorPath = new BehaviorPath
			{
				Name = pathName,
				ListEntry = null,
				BehList = new List<LuaBehavior>(),
				SubPaths = new List<BehaviorPath>()
			};
			if (newParent != null)
			{
				newParent.SubPaths.Add(behaviorPath);
			}
			else
			{
				_paths.Add(behaviorPath);
			}
			_allPaths.Add(behaviorPath);
			if (!_pathVisible.ContainsKey(pathName))
			{
				_pathVisible.Add(pathName, true);
			}
			return behaviorPath;
		}

		private void SetupBehaviorPath(LuaBehavior beh)
		{
			BehaviorPath path = GetPath("Client");
			string text = beh.GetText();
			int num = 0;
			int num2;
			while (text != "" && (num2 = text.IndexOf('/', num)) != -1)
			{
				path = GetPath(text.Substring(num, num2 - num), path);
				num = num2 + 1;
			}
			path.BehList.Add(beh);
		}

		private void CreateBehaviorPathObject(BehaviorPath bPath, Transform parent)
		{
			_003C_003Ec__DisplayClass41_0 _003C_003Ec__DisplayClass41_ = new _003C_003Ec__DisplayClass41_0();
			_003C_003Ec__DisplayClass41_._003C_003E4__this = this;
			if (bPath.ListEntry != null)
			{
				UnityEngine.Object.Destroy(bPath.ListEntry);
				bPath.ListEntry = null;
			}
			_003C_003Ec__DisplayClass41_.newPath = UnityEngine.Object.Instantiate(_subPathResource, parent, false);
			_003C_003Ec__DisplayClass41_.newPath.name = "PathObj_" + bPath.Name;
			_003C_003Ec__DisplayClass41_.newPath.GetComponentInChildren<Text>().text = bPath.Name;
			bPath.ListEntry = _003C_003Ec__DisplayClass41_.newPath;
			_003C_003Ec__DisplayClass41_.newPath.GetComponentInChildren<Button>().onClick.AddListener(_003C_003Ec__DisplayClass41_._003CCreateBehaviorPathObject_003Eb__0);
			_003C_003Ec__DisplayClass41_.newPath.GetComponentInChildren<Toggle>().onValueChanged.AddListener(_003C_003Ec__DisplayClass41_._003CCreateBehaviorPathObject_003Eb__1);
			foreach (BehaviorPath subPath in bPath.SubPaths)
			{
				CreateBehaviorPathObject(subPath, _003C_003Ec__DisplayClass41_.newPath.transform);
			}
			foreach (LuaBehavior beh in bPath.BehList)
			{
				_lookupTableButton[beh].gameObject.transform.SetParent(_003C_003Ec__DisplayClass41_.newPath.transform, false);
			}
		}

		private void SetCategoryExpanded(GameObject listEntry, bool value, bool cache = true)
		{
			listEntry.transform.GetChild(0).GetComponentInChildren<Toggle>().isOn = value;
			for (int i = 1; i < listEntry.transform.childCount; i++)
			{
				listEntry.transform.GetChild(i).gameObject.SetActive(value);
			}
			if (cache)
			{
				string text = listEntry.transform.Find("Panel").GetComponentInChildren<Text>().text;
				if (text.IndexOf('(') != -1)
				{
					text = text.Substring(0, text.LastIndexOf('(')).Trim();
				}
				if (_pathVisible != null && _pathVisible[text] != value)
				{
					_pathVisible[text] = value;
				}
			}
			if (_searchQuery != "" || _curVisState != 0)
			{
				Sort();
			}
		}

		private void ToggleCategoryExpanded(GameObject listEntry)
		{
			Toggle componentInChildren = listEntry.transform.GetChild(0).GetComponentInChildren<Toggle>();
			SetCategoryExpanded(listEntry, !componentInChildren.isOn);
		}

		private void OnSearchQueryChanged(string newQuery)
		{
			_searchQuery = newQuery.ToLower();
			Sort();
		}

		private void OnSearchTypeChanged(int index)
		{
			switch (index)
			{
			case 0:
				_curVisState = VisibleState.All;
				break;
			case 1:
				_curVisState = VisibleState.Untagged;
				break;
			default:
				_curVisState = VisibleState.Tag;
				_curVisibleTag = GetTagInfoByIndex(index - 2);
				break;
			}
			Sort();
		}

		private string GetTagInfoByIndex(int index)
		{
			int num = 0;
			foreach (KeyValuePair<string, TagInfo> item in _tagStore)
			{
				if (num != index)
				{
					num++;
					continue;
				}
				return item.Key;
			}
			return null;
		}

		private int GetTagInfoIndex(string tagName)
		{
			int num = 0;
			foreach (KeyValuePair<string, TagInfo> item in _tagStore)
			{
				if (item.Key != tagName)
				{
					num++;
					continue;
				}
				return num;
			}
			return 0;
		}

		public void ReloadBehaviorList()
		{
			if (BehaviorLists.Instance == null)
			{
				return;
			}
			_selectedBehavior = null;
			_reloadBtn.interactable = false;
			List<IBehavior> list = BehaviorLists.Instance.GetAllBehaviors().FindAll(_003C_003Ec._003C_003E9__48_0 ?? (_003C_003Ec._003C_003E9__48_0 = _003C_003Ec._003C_003E9._003CReloadBehaviorList_003Eb__48_0));
			_paths.Clear();
			_allPaths.Clear();
			_searchTypeOptions.Clear();
			_lookupTable.Clear();
			_lookupTableButton.Clear();
			_tagStore.Clear();
			AddSearchType("Show everything");
			AddSearchType("Show untagged entries");
			for (int i = 0; i < _listCanvas.transform.childCount; i++)
			{
				UnityEngine.Object.Destroy(_listCanvas.transform.GetChild(i).gameObject);
			}
			list.ForEach(_003CReloadBehaviorList_003Eb__48_1);
			foreach (KeyValuePair<string, TagInfo> item in _tagStore)
			{
				_003C_003Ec__DisplayClass48_1 _003C_003Ec__DisplayClass48_ = new _003C_003Ec__DisplayClass48_1();
				_003C_003Ec__DisplayClass48_._003C_003E4__this = this;
				_003C_003Ec__DisplayClass48_.tagName = item.Key;
				TagInfo value = item.Value;
				AddSearchType("Tag \"" + _003C_003Ec__DisplayClass48_.tagName + "\" (" + value.BehList.Count + ")");
				value.GoList.ForEach(_003C_003Ec__DisplayClass48_._003CReloadBehaviorList_003Eb__4);
			}
			if (useTreeLayout)
			{
				foreach (BehaviorPath path2 in _paths)
				{
					CreateBehaviorPathObject(path2, _listCanvas.transform);
				}
				foreach (KeyValuePair<string, bool> item2 in _pathVisible)
				{
					BehaviorPath path = GetPath(item2.Key, null, false);
					if (path != null && !(path.ListEntry == null) && !item2.Value)
					{
						SetCategoryExpanded(path.ListEntry, false, false);
					}
				}
			}
			_searchType.value = 0;
			Sort();
		}

		private void OnEnable()
		{
			if (!_initialized)
			{
				Initialize();
			}
			else if (_settingsView != null)
			{
				_settingsView.SetActive(false);
			}
			ReloadBehaviorList();
		}

		private void OnDisable()
		{
			_searchTypeOptions.Clear();
			_lookupTable.Clear();
			_lookupTableButton.Clear();
			_tagStore.Clear();
			for (int i = 0; i < _listCanvas.transform.childCount; i++)
			{
				UnityEngine.Object.Destroy(_listCanvas.transform.GetChild(i).gameObject);
			}
		}

		private void OnClickElement(IBehavior bObject)
		{
			if (_selectedBehavior == bObject)
			{
				EditBehavior();
				return;
			}
			_reloadBtn.interactable = true;
			_selectedBehavior = bObject;
		}

		private void ReloadScript()
		{
			GameController.Instance.ReloadScript(_selectedBehavior as LuaBehavior);
			ReloadBehaviorList();
		}

		private void Close()
		{
			Hide();
			if ((bool)_settingsView)
			{
				_settingsView.SetActive(false);
			}
			base.gameObject.SetActive(false);
		}

		private void EditBehavior()
		{
			if (_selectedBehavior != null)
			{
				InitSettingView();
				_settingsView.SetActive(true);
				_settingsView.GetComponent<BehaviorSettingsView>().SetBehavior(_selectedBehavior);
			}
		}

		public int GetTagScore(string tagName)
		{
			if (_tagStore.ContainsKey(tagName))
			{
				return _tagStore[tagName].BehList.Count;
			}
			return 0;
		}

		[CompilerGenerated]
		private void _003CInitialize_003Eb__33_0()
		{
			Image component = _layoutBtn.transform.Find("Image").GetComponent<Image>();
			useTreeLayout = !useTreeLayout;
			component.sprite = (useTreeLayout ? _bmLayoutTree : _bmLayoutList);
			ReloadBehaviorList();
		}

		[CompilerGenerated]
		private void _003CReloadBehaviorList_003Eb__48_1(IBehavior bObj)
		{
			_003C_003Ec__DisplayClass48_0 _003C_003Ec__DisplayClass48_ = new _003C_003Ec__DisplayClass48_0();
			_003C_003Ec__DisplayClass48_._003C_003E4__this = this;
			_003C_003Ec__DisplayClass48_.bObj = bObj;
			Button button = UnityEngine.Object.Instantiate(_buttonPrefab, _listCanvas.transform, false);
			button.GetComponentInChildren<Text>().text = (useTreeLayout ? _003C_003Ec__DisplayClass48_.bObj.GetText().Substring(_003C_003Ec__DisplayClass48_.bObj.GetText().LastIndexOf('/') + 1) : _003C_003Ec__DisplayClass48_.bObj.GetText());
			button.onClick.AddListener(_003C_003Ec__DisplayClass48_._003CReloadBehaviorList_003Eb__2);
			if (_003C_003Ec__DisplayClass48_.bObj.CanUseAI())
			{
				button.GetComponentInChildren<Toggle>().isOn = _003C_003Ec__DisplayClass48_.bObj.IsAI();
				button.GetComponentInChildren<Toggle>().onValueChanged.AddListener(_003C_003Ec__DisplayClass48_._003CReloadBehaviorList_003Eb__3);
			}
			else
			{
				button.GetComponentInChildren<Toggle>().isOn = true;
				button.GetComponentInChildren<Toggle>().interactable = false;
				((Image)button.GetComponentInChildren<Toggle>().graphic).sprite = _bmDisabledCheck;
			}
			LuaBehavior luaBehavior = _003C_003Ec__DisplayClass48_.bObj as LuaBehavior;
			if (useTreeLayout)
			{
				SetupBehaviorPath(luaBehavior);
			}
			_lookupTable.Add(button, luaBehavior);
			if (luaBehavior != null)
			{
				_lookupTableButton.Add(luaBehavior, button);
				for (int i = 0; i < luaBehavior.GetTagCount(); i++)
				{
					AssociateListEntryWithTag(button.gameObject, luaBehavior.GetTag(i));
					AssociateBehaviorWithTag(luaBehavior, luaBehavior.GetTag(i));
				}
			}
		}
	}
}
