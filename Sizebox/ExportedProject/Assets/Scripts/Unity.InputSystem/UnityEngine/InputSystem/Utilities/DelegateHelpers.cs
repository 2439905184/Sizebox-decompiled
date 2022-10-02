using System;

namespace UnityEngine.InputSystem.Utilities
{
	internal static class DelegateHelpers
	{
		public static void InvokeCallbacksSafe(ref InlinedArray<Action> callbacks, string callbackName, object context = null)
		{
			if (callbacks.length == 0)
			{
				return;
			}
			for (int i = 0; i < callbacks.length; i++)
			{
				int length = callbacks.length;
				try
				{
					callbacks[i]();
				}
				catch (Exception ex)
				{
					if (context != null)
					{
						Debug.LogError(string.Format("{0} while executing '{1}' callbacks of '{2}'", ex.GetType().Name, callbackName, context));
					}
					else
					{
						Debug.LogError(ex.GetType().Name + " while executing '" + callbackName + "' callbacks");
					}
					Debug.LogException(ex);
				}
				if (callbacks.length == length - 1)
				{
					i--;
				}
			}
		}

		public static void InvokeCallbacksSafe<TValue>(ref InlinedArray<Action<TValue>> callbacks, TValue argument, string callbackName, object context = null)
		{
			if (callbacks.length == 0)
			{
				return;
			}
			for (int i = 0; i < callbacks.length; i++)
			{
				int length = callbacks.length;
				try
				{
					callbacks[i](argument);
				}
				catch (Exception ex)
				{
					if (context != null)
					{
						Debug.LogError(string.Format("{0} while executing '{1}' callbacks of '{2}'", ex.GetType().Name, callbackName, context));
					}
					else
					{
						Debug.LogError(ex.GetType().Name + " while executing '" + callbackName + "' callbacks");
					}
					Debug.LogException(ex);
				}
				if (callbacks.length == length - 1)
				{
					i--;
				}
			}
		}

		public static void InvokeCallbacksSafe<TValue1, TValue2>(ref InlinedArray<Action<TValue1, TValue2>> callbacks, TValue1 argument1, TValue2 argument2, string callbackName, object context = null)
		{
			if (callbacks.length == 0)
			{
				return;
			}
			for (int i = 0; i < callbacks.length; i++)
			{
				int length = callbacks.length;
				try
				{
					callbacks[i](argument1, argument2);
				}
				catch (Exception ex)
				{
					if (context != null)
					{
						Debug.LogError(string.Format("{0} while executing '{1}' callbacks of '{2}'", ex.GetType().Name, callbackName, context));
					}
					else
					{
						Debug.LogError(ex.GetType().Name + " while executing '" + callbackName + "' callbacks");
					}
					Debug.LogException(ex);
				}
				if (callbacks.length == length - 1)
				{
					i--;
				}
			}
		}
	}
}
