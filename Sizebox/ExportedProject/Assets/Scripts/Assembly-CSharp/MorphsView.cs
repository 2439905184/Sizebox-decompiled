using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SizeboxUI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MorphsView : ViewCommon
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass9_0
	{
		public GameObject go;

		public MorphsView _003C_003E4__this;

		internal void _003CAwake_003Eb__0(float val)
		{
			_003C_003E4__this.OnValueChanged(go, val);
		}
	}

	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Func<EntityMorphData, string> _003C_003E9__12_0;

		internal string _003CReloadValues_003Eb__12_0(EntityMorphData m)
		{
			return m.Name;
		}
	}

	[SerializeField]
	private MorphSearchBar searchBar;

	private ThreadSafeList<EntityMorphData> _morphs;

	private GameObject _placeholder;

	private int _elementsCount;

	private GridLayoutGroup _grid;

	private InterfaceControl _control;

	private Text[] _text;

	private Slider[] _slider;

	private IMorphable _target;

	private void Awake()
	{
		_control = GuiManager.Instance.InterfaceControl;
		Button[] componentsInChildren = GetComponentsInChildren<Button>();
		componentsInChildren[0].onClick.AddListener(base.OnPrevious);
		componentsInChildren[1].onClick.AddListener(base.OnNext);
		componentsInChildren[2].onClick.AddListener(OnApply);
		_placeholder = Resources.Load("UI/Button/FieldSlider") as GameObject;
		_grid = GetComponentInChildren<GridLayoutGroup>();
		int elementsCount = (int)_grid.GetComponent<RectTransform>().rect.height / 30;
		_elementsCount = elementsCount;
		_text = new Text[_elementsCount];
		_slider = new Slider[_elementsCount];
		for (int i = 0; i < _elementsCount; i++)
		{
			_003C_003Ec__DisplayClass9_0 _003C_003Ec__DisplayClass9_ = new _003C_003Ec__DisplayClass9_0();
			_003C_003Ec__DisplayClass9_._003C_003E4__this = this;
			_003C_003Ec__DisplayClass9_.go = UnityEngine.Object.Instantiate(_placeholder, _grid.transform);
			_003C_003Ec__DisplayClass9_.go.name = "Slider " + i;
			_text[i] = _003C_003Ec__DisplayClass9_.go.GetComponentInChildren<Text>();
			_slider[i] = _003C_003Ec__DisplayClass9_.go.GetComponentInChildren<Slider>();
			_slider[i].onValueChanged.AddListener(_003C_003Ec__DisplayClass9_._003CAwake_003Eb__0);
		}
		InterfaceControl control = _control;
		control.onSelected = (UnityAction)Delegate.Combine(control.onSelected, new UnityAction(OnChangedCharacter));
		MorphSearchBar morphSearchBar = searchBar;
		morphSearchBar.onSearchCompleted = (UnityAction<ThreadSafeList<EntityMorphData>>)Delegate.Combine(morphSearchBar.onSearchCompleted, new UnityAction<ThreadSafeList<EntityMorphData>>(OnSearchComplete));
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
		if (!_control)
		{
			return;
		}
		if (!_control.selectedEntity)
		{
			base.gameObject.SetActive(false);
			return;
		}
		IMorphable component = _control.selectedEntity.GetComponent<IMorphable>();
		if (component != _target)
		{
			_target = component;
			if (_target == null)
			{
				base.gameObject.SetActive(false);
				return;
			}
			_morphs = new ThreadSafeList<EntityMorphData>(_target.Morphs);
			searchBar.SetSearchableCollection(_morphs, _003C_003Ec._003C_003E9__12_0 ?? (_003C_003Ec._003C_003E9__12_0 = _003C_003Ec._003C_003E9._003CReloadValues_003Eb__12_0));
		}
	}

	private void OnValueChanged(GameObject sliderGameObject, float value)
	{
		Text componentInChildren = sliderGameObject.GetComponentInChildren<Text>();
		_target.SetMorphValue(componentInChildren.text, value);
	}

	protected override void LoadPage(int pageNumber)
	{
		IList<EntityMorphData> list = (searchBar.HasSearchResults ? searchBar.SearchResults : _morphs);
		int num = pageNumber * _elementsCount;
		for (int i = 0; i < _elementsCount; i++)
		{
			int num2 = num + i;
			if (num2 < list.Count)
			{
				_text[i].text = list[num2].Name;
				_slider[i].value = list[num2].Weight;
				_text[i].transform.parent.gameObject.SetActive(true);
			}
			else
			{
				_text[i].transform.parent.gameObject.SetActive(false);
			}
		}
	}

	private void OnDisable()
	{
		if ((bool)_control)
		{
			_control.UpdateCollider();
		}
	}

	private void OnApply()
	{
		_control.UpdateCollider();
	}

	protected override int CalculatePageCount()
	{
		return Mathf.CeilToInt((float)((ICollection<EntityMorphData>)(searchBar.HasSearchResults ? searchBar.SearchResults : _morphs)).Count / (float)_elementsCount);
	}

	private void OnSearchComplete(ThreadSafeList<EntityMorphData> results)
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
}
