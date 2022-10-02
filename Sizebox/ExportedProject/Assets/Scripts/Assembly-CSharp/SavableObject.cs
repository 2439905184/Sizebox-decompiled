using UnityEngine;

[RequireComponent(typeof(ISavable))]
public class SavableObject : MonoBehaviour
{
	public ISavable Savable { get; private set; }

	private void Awake()
	{
		Savable = GetComponent<ISavable>();
	}
}
