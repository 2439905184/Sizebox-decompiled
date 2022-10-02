using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SizeboxUI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AnimationView : ViewCommon
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass11_0
	{
		public int number;

		public AnimationView _003C_003E4__this;

		internal void _003CAwake_003Eb__1()
		{
			_003C_003E4__this.OnElementClick(number);
		}
	}

	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Func<string, string> _003C_003E9__11_0;

		internal string _003CAwake_003Eb__11_0(string s)
		{
			return s;
		}
	}

	[SerializeField]
	private ListElement listEntryPrefab;

	[SerializeField]
	private GridLayoutGroup grid;

	[SerializeField]
	private Button nextButton;

	[SerializeField]
	private Button prevButton;

	[SerializeField]
	private Slider speedSlider;

	[SerializeField]
	private Toggle disableMovementToggle;

	[FormerlySerializedAs("animationSearchBar")]
	[SerializeField]
	private StringSearchBar searchBar;

	private InterfaceControl _control;

	private ListElement[] _animationEntries;

	private int _entriesPerPage;

	private ThreadSafeList<string> _animationList;

	private void Awake()
	{
		_control = GuiManager.Instance.InterfaceControl;
		prevButton.onClick.AddListener(base.OnPrevious);
		nextButton.onClick.AddListener(base.OnNext);
		speedSlider.minValue = 0f;
		speedSlider.maxValue = 3f;
		speedSlider.value = 1f;
		speedSlider.onValueChanged.AddListener(_control.ChangeAnimationSpeed);
		_animationList = new ThreadSafeList<string>(_control.animations);
		searchBar.SetSearchableCollection(_animationList, _003C_003Ec._003C_003E9__11_0 ?? (_003C_003Ec._003C_003E9__11_0 = _003C_003Ec._003C_003E9._003CAwake_003Eb__11_0));
		searchBar.onSearchCompleted = OnSearchComplete;
		disableMovementToggle.onValueChanged.AddListener(DisableAutoRepositioningClicked);
		RectTransform component = grid.GetComponent<RectTransform>();
		_entriesPerPage = (int)component.rect.height / 30;
		_animationEntries = new ListElement[_entriesPerPage];
		for (int i = 0; i < _entriesPerPage; i++)
		{
			_003C_003Ec__DisplayClass11_0 _003C_003Ec__DisplayClass11_ = new _003C_003Ec__DisplayClass11_0();
			_003C_003Ec__DisplayClass11_._003C_003E4__this = this;
			ListElement listElement = UnityEngine.Object.Instantiate(listEntryPrefab, grid.transform);
			listElement.name = "Element " + i;
			_animationEntries[i] = listElement;
			_003C_003Ec__DisplayClass11_.number = i;
			listElement.onClick.AddListener(_003C_003Ec__DisplayClass11_._003CAwake_003Eb__1);
		}
		base.page = 0;
		InterfaceControl control = _control;
		control.onSelected = (UnityAction)Delegate.Combine(control.onSelected, new UnityAction(OnChangedCharacter));
		base.gameObject.SetActive(false);
	}

	private void OnChangedCharacter()
	{
		if (base.gameObject.activeSelf)
		{
			ReloadValues();
		}
	}

	private void OnEnable()
	{
		ReloadValues();
	}

	private void ReloadValues()
	{
		if (!(_control == null))
		{
			if (_control.humanoid == null)
			{
				base.gameObject.SetActive(false);
				return;
			}
			speedSlider.value = _control.GetAnimationSpeed();
			disableMovementToggle.isOn = _control.GetDoNotMoveMacroSetting();
		}
	}

	private void OnElementClick(int i)
	{
		_control.SetAnimation(_animationEntries[i].Text, disableMovementToggle.isOn);
	}

	private void DisableAutoRepositioningClicked(bool isOn)
	{
		_control.SetDoNotMoveMacro(isOn);
	}

	protected override void LoadPage(int pageNumber)
	{
		IList<string> list = (searchBar.HasSearchResults ? searchBar.SearchResults : _animationList);
		int num = pageNumber * _entriesPerPage;
		for (int i = 0; i < _entriesPerPage; i++)
		{
			int num2 = num + i;
			if (num2 < list.Count)
			{
				_animationEntries[i].Text = list[num2];
				_animationEntries[i].gameObject.SetActive(true);
			}
			else
			{
				_animationEntries[i].gameObject.SetActive(false);
			}
		}
	}

	private void OnSearchComplete(ThreadSafeList<string> results)
	{
		if (searchBar.HasSearchQuery)
		{
			base.page = 0;
		}
		else
		{
			LoadPage(base.page);
		}
	}

	protected override int CalculatePageCount()
	{
		return Mathf.CeilToInt((float)((ICollection<string>)(searchBar.HasSearchResults ? searchBar.SearchResults : _animationList)).Count / (float)_entriesPerPage);
	}
}
