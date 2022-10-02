using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
	private static List<Listener> listeners = new List<Listener>();

	public static Listener Register(IListener listener, string interest)
	{
		Listener listener2 = new Listener(listener, interest);
		listeners.Add(listener2);
		return listener2;
	}

	public static void Unregister(Listener registration)
	{
		listeners.Remove(registration);
	}

	public static void SendEvent(IEvent e)
	{
		foreach (Listener listener in listeners)
		{
			if (listener.listener == null)
			{
				Debug.LogError("One listener is null, please remove from the list");
			}
			else if (e.code == listener.interestCode)
			{
				listener.listener.OnNotify(e);
			}
		}
	}

	public static void Clear()
	{
		listeners.Clear();
	}
}
