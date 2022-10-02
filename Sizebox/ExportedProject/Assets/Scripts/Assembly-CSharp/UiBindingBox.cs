using UnityEngine;
using UnityEngine.UI;

public class UiBindingBox : UiPopup
{
	public Text message;

	public new static UiBindingBox Create()
	{
		Transform parent = GameObject.FindWithTag("MainCanvas").transform;
		return Object.Instantiate(Resources.Load<GameObject>("UI/UiBindingBox"), parent, false).GetComponent<UiBindingBox>();
	}
}
