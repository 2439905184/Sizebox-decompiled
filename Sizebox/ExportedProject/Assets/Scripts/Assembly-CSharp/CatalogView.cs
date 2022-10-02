using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SizeboxUI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CatalogView : ViewCommon
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass28_0
	{
		public int number;

		public CatalogView _003C_003E4__this;

		internal void _003CEnsurePlaceholders_003Eb__0()
		{
			_003C_003E4__this.OnElementClick(number);
		}
	}

	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Func<AssetDescription, string> _003C_003E9__31_0;

		internal string _003CUpdateActiveAssetsArray_003Eb__31_0(AssetDescription asset)
		{
			return asset.AssetName;
		}
	}

	[Header("Prefabs")]
	[SerializeField]
	private CatalogEntry placeholder;

	[Header("Required References")]
	[SerializeField]
	private GridLayoutGroup grid;

	[SerializeField]
	private AssetSearchBar searchBar;

	[Header("Toggles")]
	[SerializeField]
	private Toggle playAsGiantess;

	[SerializeField]
	private Toggle playAsMicro;

	[Header("Buttons")]
	[SerializeField]
	private Button nextPageButton;

	[SerializeField]
	private Button prevPageButton;

	[Space]
	[SerializeField]
	private Text assetsCountText;

	[SerializeField]
	private Text pageCountText;

	[Header("GameObjects")]
	[SerializeField]
	private RectTransform contentPanel;

	[SerializeField]
	private GameObject microOptionsPanel;

	[SerializeField]
	private GameObject giantessOptionsPanel;

	private CatalogCategory _currentCategory;

	private readonly List<CatalogEntry> _entries = new List<CatalogEntry>();

	private ThreadSafeList<AssetDescription> _activeAssetsArray;

	private ThreadSafeList<AssetDescription> _modelAssets;

	private ThreadSafeList<AssetDescription> _objectSprites;

	private InterfaceControl _control;

	private EditPlacement _placement;

	private int placeholderCount
	{
		get
		{
			return UpdateCatalogSize();
		}
	}

	private float GridElementWidth
	{
		get
		{
			return grid.cellSize.x + grid.spacing.x;
		}
	}

	private float GridElementHeight
	{
		get
		{
			return grid.cellSize.y + grid.spacing.y;
		}
	}

	private void Awake()
	{
		_control = GuiManager.Instance.GetComponent<InterfaceControl>();
		_placement = GuiManager.Instance.GetComponent<EditPlacement>();
		prevPageButton.onClick.AddListener(base.OnPrevious);
		nextPageButton.onClick.AddListener(base.OnNext);
		AssetManager instance = AssetManager.Instance;
		_modelAssets = new ThreadSafeList<AssetDescription>(instance.GetModelAssets());
		_objectSprites = new ThreadSafeList<AssetDescription>(instance.GetObjectAssets());
		AssetSearchBar assetSearchBar = searchBar;
		assetSearchBar.onSearchCompleted = (UnityAction<ThreadSafeList<AssetDescription>>)Delegate.Combine(assetSearchBar.onSearchCompleted, new UnityAction<ThreadSafeList<AssetDescription>>(OnSearchComplete));
		AssetSearchBar assetSearchBar2 = searchBar;
		assetSearchBar2.onSearchUpdated = (UnityAction<ThreadSafeList<AssetDescription>>)Delegate.Combine(assetSearchBar2.onSearchUpdated, new UnityAction<ThreadSafeList<AssetDescription>>(OnSearchUpdate));
		playAsMicro.isOn = true;
	}

	private void OnEnable()
	{
		EnsurePlaceholders(placeholderCount);
		SetCategory(_currentCategory);
	}

	private int UpdateCatalogSize()
	{
		int num = (int)Mathf.Abs((contentPanel.rect.width - (float)(grid.padding.left + grid.padding.right)) / GridElementWidth);
		int num2 = (int)Mathf.Abs((contentPanel.rect.height - (float)(grid.padding.top + grid.padding.bottom)) / GridElementHeight);
		return num * num2;
	}

	private void EnsurePlaceholders(int requiredAmount)
	{
		int num = Mathf.Min(requiredAmount, _entries.Count);
		for (int i = 0; i < num; i++)
		{
			_entries[i].gameObject.SetActive(true);
		}
		for (int j = _entries.Count; j < requiredAmount; j++)
		{
			_003C_003Ec__DisplayClass28_0 _003C_003Ec__DisplayClass28_ = new _003C_003Ec__DisplayClass28_0();
			_003C_003Ec__DisplayClass28_._003C_003E4__this = this;
			CatalogEntry catalogEntry = UnityEngine.Object.Instantiate(placeholder, contentPanel);
			catalogEntry.name = "Entry " + j;
			_entries.Add(catalogEntry);
			Button button = catalogEntry.Button;
			_003C_003Ec__DisplayClass28_.number = j;
			button.onClick.AddListener(_003C_003Ec__DisplayClass28_._003CEnsurePlaceholders_003Eb__0);
		}
		for (int k = requiredAmount; k < _entries.Count; k++)
		{
			_entries[k].gameObject.SetActive(false);
		}
	}

	public void OnMenuClick(CatalogCategory category)
	{
		if (category == _currentCategory)
		{
			base.gameObject.SetActive(!base.gameObject.activeSelf);
			return;
		}
		SetCategory(category);
		base.gameObject.SetActive(true);
	}

	private void SetCategory(CatalogCategory newCategory)
	{
		bool active = newCategory == CatalogCategory.Giantess;
		giantessOptionsPanel.SetActive(active);
		bool active2 = newCategory == CatalogCategory.Micro;
		microOptionsPanel.SetActive(active2);
		_currentCategory = newCategory;
		UpdateActiveAssetsArray(_currentCategory);
		if (placeholderCount > 0)
		{
			searchBar.ClearSearch();
			base.page = 0;
			LoadPage(base.page);
		}
	}

	private void UpdateActiveAssetsArray(CatalogCategory type)
	{
		switch (type)
		{
		case CatalogCategory.Giantess:
			_activeAssetsArray = _modelAssets;
			break;
		case CatalogCategory.Micro:
			_activeAssetsArray = _modelAssets;
			break;
		case CatalogCategory.Object:
			_activeAssetsArray = _objectSprites;
			break;
		default:
			_activeAssetsArray = _modelAssets;
			break;
		}
		searchBar.SetSearchableCollection(_activeAssetsArray, _003C_003Ec._003C_003E9__31_0 ?? (_003C_003Ec._003C_003E9__31_0 = _003C_003Ec._003C_003E9._003CUpdateActiveAssetsArray_003Eb__31_0));
		searchBar.ClearSearch();
	}

	protected override void LoadPage(int pageNumber)
	{
		IList<AssetDescription> list = ((!searchBar.HasSearchResults && !searchBar.IsSearching) ? _activeAssetsArray : searchBar.SearchResults);
		int num = pageNumber * placeholderCount;
		for (int i = 0; i < placeholderCount; i++)
		{
			int num2 = num + i;
			_entries[i].SetAsset((num2 < list.Count) ? list[num2] : null);
		}
		assetsCountText.text = list.Count.ToString();
		pageCountText.text = pageNumber + 1 + "/" + CalculatePageCount();
	}

	private void OnElementClick(int i)
	{
		if (_entries[i].Active)
		{
			AssetDescription asset = _entries[i].Asset;
			if (_control.selectedEntity != null && !_control.selectedEntity.isPositioned)
			{
				_control.selectedEntity.DestroyObject();
			}
			switch (_currentCategory)
			{
			case CatalogCategory.Object:
				_placement.AddGameObject(asset);
				break;
			case CatalogCategory.Giantess:
				_placement.AddGiantess(asset, playAsGiantess.isOn);
				playAsGiantess.isOn = false;
				break;
			case CatalogCategory.Micro:
				_placement.AddMicro(asset, playAsMicro.isOn);
				playAsMicro.isOn = false;
				break;
			}
		}
	}

	private void OnSearchComplete(ThreadSafeList<AssetDescription> searchedAssets)
	{
		base.page = 0;
		LoadPage(base.page);
	}

	private void OnSearchUpdate(ThreadSafeList<AssetDescription> searchedAssets)
	{
		LoadPage(base.page);
	}

	protected override int CalculatePageCount()
	{
		int num = ((searchBar.HasSearchResults || searchBar.IsSearching) ? searchBar.SearchResults.Count : _activeAssetsArray.Count);
		int num2 = placeholderCount;
		if (num > 1 && num2 > 1)
		{
			num = num / num2 + 1;
		}
		return num;
	}
}
