using UnityEngine;
using UnityEngine.UI;

public class UILoadingStage : MonoBehaviour
{
	public GameObject loadingBar;

	public Text text;

	public AsyncOperation AsyncOperation;

	public bool IsLoading
	{
		get
		{
			return loadingBar.activeSelf;
		}
		set
		{
			loadingBar.SetActive(value);
		}
	}

	private void OnEnable()
	{
		if (AsyncOperation == null)
		{
			loadingBar.SetActive(false);
		}
	}

	private void Update()
	{
		text.text = AsyncOperation.progress.ToString("P0");
	}
}
