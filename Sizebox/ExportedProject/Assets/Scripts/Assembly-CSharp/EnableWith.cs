using System.Collections.Generic;
using UnityEngine;

public class EnableWith : MonoBehaviour
{
	public List<GameObject> gameObjects;

	private void OnEnable()
	{
		foreach (GameObject gameObject in gameObjects)
		{
			gameObject.SetActive(true);
		}
	}

	private void OnDisable()
	{
		foreach (GameObject gameObject in gameObjects)
		{
			gameObject.SetActive(false);
		}
	}
}
