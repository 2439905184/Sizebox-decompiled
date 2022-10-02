using UnityEngine;
using UnityEngine.UI;

public class VersionText : MonoBehaviour
{
	private void Start()
	{
		base.gameObject.GetComponent<Text>().text = "Sizebox v" + Version.GetText();
	}
}
