using System.Collections.Generic;
using UnityEngine;

public class CheckDynamicCollision : MonoBehaviour
{
	private void Start()
	{
		FracturedObject component = GetComponent<FracturedObject>();
		if (component != null)
		{
			if (component.GetComponent<Collider>() != null)
			{
				component.GetComponent<Collider>().enabled = true;
			}
			else
			{
				Debug.LogWarning("Fracturable Object " + base.gameObject.name + " has a dynamic rigidbody but no collider. Object will not be able to collide.");
			}
			for (int i = 0; i < component.ListFracturedChunks.Count; i++)
			{
				EnableObjectColliders(component.ListFracturedChunks[i].gameObject, false);
			}
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.contacts == null || collision.contacts.Length == 0)
		{
			return;
		}
		FracturedObject component = base.gameObject.GetComponent<FracturedObject>();
		if (!(component != null))
		{
			return;
		}
		float num = ((collision.rigidbody != null) ? collision.rigidbody.mass : float.PositiveInfinity);
		if (collision.relativeVelocity.magnitude > component.EventDetachMinVelocity && num > component.EventDetachMinVelocity)
		{
			component.GetComponent<Collider>().enabled = false;
			Rigidbody component2 = component.GetComponent<Rigidbody>();
			if (component2 != null)
			{
				component2.isKinematic = true;
			}
			for (int i = 0; i < component.ListFracturedChunks.Count; i++)
			{
				EnableObjectColliders(component.ListFracturedChunks[i].gameObject, true);
			}
			component.Explode(collision.contacts[0].point, collision.relativeVelocity.magnitude);
		}
	}

	private void EnableObjectColliders(GameObject chunk, bool bEnable)
	{
		List<Collider> listOut = new List<Collider>();
		SearchForAllComponentsInHierarchy(chunk, ref listOut);
		for (int i = 0; i < listOut.Count; i++)
		{
			listOut[i].enabled = bEnable;
			if (bEnable)
			{
				listOut[i].isTrigger = false;
			}
		}
	}

	private static void SearchForAllComponentsInHierarchy<T>(GameObject current, ref List<T> listOut) where T : Component
	{
		T component = current.GetComponent<T>();
		if ((Object)component != (Object)null)
		{
			listOut.Add(component);
		}
		for (int i = 0; i < current.transform.childCount; i++)
		{
			SearchForAllComponentsInHierarchy(current.transform.GetChild(i).gameObject, ref listOut);
		}
	}
}
