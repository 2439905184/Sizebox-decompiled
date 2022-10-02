using UnityEngine;

public class DebrisDestructor : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.tag.Equals("DetachedDebris"))
		{
			Object.Destroy(other.gameObject);
		}
	}
}
