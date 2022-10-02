using System.Collections.Generic;
using UnityEngine;

public class CullBoxHide : MonoBehaviour
{
	public List<GameObject> Object;

	public Collider Collider;

	private void Start()
	{
		foreach (GameObject item in Object)
		{
			item.SetActive(true);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!(other == Collider))
		{
			return;
		}
		foreach (GameObject item in Object)
		{
			item.SetActive(false);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!(other == Collider))
		{
			return;
		}
		foreach (GameObject item in Object)
		{
			item.SetActive(true);
		}
	}
}
