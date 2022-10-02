using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class SearchBar<T> : MonoBehaviour
{
	[SerializeField]
	private InputField searchInput;

	[SerializeField]
	private float searchDelay = 0.35f;

	public UnityAction<ThreadSafeList<T>> onSearchCompleted;

	public UnityAction<ThreadSafeList<T>> onSearchUpdated;

	private ThreadSafeList<T> _searchableList;

	private Func<T, string> _searchMethod;

	private bool _isCoroutineRunning;

	private float _lastKeyPressTime;

	private Thread _searchThread;

	private bool _isSearchComplete;

	private bool _exitThread;

	private string searchInputText
	{
		get
		{
			return searchInput.text.ToLowerInvariant();
		}
	}

	public ThreadSafeList<T> SearchResults { get; private set; } = new ThreadSafeList<T>(100);


	public bool HasSearchQuery
	{
		get
		{
			return searchInput.text.Length > 2;
		}
	}

	public bool HasSearchResults { get; private set; }

	public bool IsSearching { get; private set; }

	private void Awake()
	{
		_searchThread = new Thread(ThreadedSearch);
		Sbox.AddSBoxInputFieldEvents(searchInput);
		searchInput.onValueChanged.AddListener(OnUserInput);
	}

	private void OnEnable()
	{
		if ((bool)searchInput)
		{
			DelayedStartSearch();
		}
	}

	public void SetSearchableCollection(ThreadSafeList<T> list, Func<T, string> searchMethod)
	{
		ClearSearch();
		_searchableList = list;
		_searchMethod = searchMethod;
	}

	private void OnUserInput(string searchTerm)
	{
		if (string.IsNullOrWhiteSpace(searchTerm))
		{
			ClearSearch();
		}
		else if (_searchableList != null)
		{
			_lastKeyPressTime = Time.time;
			DelayedStartSearch();
		}
	}

	private void DelayedStartSearch()
	{
		if (!_isCoroutineRunning || IsSearching)
		{
			CancelSearch();
			StartCoroutine(SearchCoroutine());
		}
	}

	private IEnumerator SearchCoroutine()
	{
		_isCoroutineRunning = true;
		StopThread();
		string searchQuery = searchInputText;
		while (_searchThread.IsAlive || _lastKeyPressTime + searchDelay > Time.time)
		{
			yield return null;
			if (searchQuery != searchInputText)
			{
				_lastKeyPressTime = Time.time;
				searchQuery = searchInputText;
			}
		}
		SearchResults.Clear();
		_exitThread = false;
		_searchThread = new Thread(ThreadedSearch);
		_searchThread.Start(searchQuery);
		while (!_isSearchComplete)
		{
			UnityAction<ThreadSafeList<T>> unityAction = onSearchUpdated;
			if (unityAction != null)
			{
				unityAction(SearchResults);
			}
			yield return null;
		}
		CompleteSearch();
		_isCoroutineRunning = false;
	}

	private void ThreadedSearch(object searchString)
	{
		IsSearching = true;
		string value = (string)searchString;
		foreach (T searchable in _searchableList)
		{
			if (_searchMethod(searchable).ToLowerInvariant().Contains(value))
			{
				SearchResults.Add(searchable);
			}
			if (_exitThread)
			{
				return;
			}
		}
		IsSearching = false;
		_isSearchComplete = true;
	}

	public void ClearSearch()
	{
		CancelSearch();
		HasSearchResults = false;
		searchInput.text = "";
		SearchResults.Clear();
		UnityAction<ThreadSafeList<T>> unityAction = onSearchCompleted;
		if (unityAction != null)
		{
			unityAction(SearchResults);
		}
	}

	private void CancelSearch()
	{
		StopThread();
		StopCoroutine();
	}

	private void StopCoroutine()
	{
		StopAllCoroutines();
		_isCoroutineRunning = false;
	}

	private void StopThread()
	{
		_exitThread = true;
		_isSearchComplete = false;
		IsSearching = false;
	}

	private void CompleteSearch()
	{
		_isSearchComplete = false;
		HasSearchResults = true;
		UnityAction<ThreadSafeList<T>> unityAction = onSearchCompleted;
		if (unityAction != null)
		{
			unityAction(SearchResults);
		}
	}
}
