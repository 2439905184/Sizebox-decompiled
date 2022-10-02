using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

internal class ToastInternal : MonoBehaviour
{
	[SerializeField]
	private Image toastImage;

	[SerializeField]
	private Text toastText;

	internal const string DefaultId = "_default-Toast";

	public const string NoToast = "-no-toast";

	private const float ImageAlpha = 0.5f;

	private const float TextAlpha = 1f;

	public static bool disabled;

	private static readonly WaitForSeconds Timeout5 = new WaitForSeconds(5f);

	private static readonly WaitForSeconds Timeout3 = new WaitForSeconds(3f);

	private static readonly WaitForSeconds Timeout2 = new WaitForSeconds(2f);

	private static readonly WaitForSeconds TimeoutDot5 = new WaitForSeconds(0.5f);

	private static readonly WaitForSeconds FadeOut = new WaitForSeconds(1f);

	public string id = "_default-Toast";

	private Coroutine _waitFor;

	private static readonly Dictionary<string, ToastInternal> Toasts = new Dictionary<string, ToastInternal>();

	private static Transform _canvasTransform;

	public static void UpdateToastCanvas()
	{
		UpdateToastEnableState();
		GameObject obj = GameObject.FindWithTag("MainCanvas");
		object canvasTransform;
		if ((object)obj == null)
		{
			canvasTransform = null;
		}
		else
		{
			Transform obj2 = obj.transform.Find("Toast Holder");
			canvasTransform = (((object)obj2 != null) ? obj2.transform : null);
		}
		_canvasTransform = (Transform)canvasTransform;
		if (!_canvasTransform)
		{
			disabled = true;
		}
	}

	private static void UpdateToastEnableState()
	{
		disabled = Sbox.GetProcessFlag("-no-toast");
	}

	public static void UpdateToastCanvas(Transform transform)
	{
		_canvasTransform = transform;
	}

	private void SetMessage(string text)
	{
		toastText.text = text;
	}

	private void PopUp(Toast.Timeout timeout = Toast.Timeout.Automatic)
	{
		if (_waitFor != null)
		{
			StopCoroutine(_waitFor);
		}
		_waitFor = StartCoroutine(WaitForTimeout(timeout));
	}

	private void PopUp(string text, Toast.Timeout timeout = Toast.Timeout.Automatic)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			if (_waitFor != null)
			{
				StopCoroutine(_waitFor);
			}
			_waitFor = StartCoroutine(FadeAndDestroy());
		}
		else
		{
			SetMessage(text);
			PopUp(timeout);
		}
	}

	private IEnumerator FadeAndDestroy()
	{
		toastImage.CrossFadeAlpha(0f, 0.7f, true);
		toastText.CrossFadeAlpha(0f, 0.7f, true);
		yield return FadeOut;
		Object.Destroy(base.gameObject);
	}

	private void OnDestroy()
	{
		Toasts.Remove(id);
	}

	private IEnumerator WaitForTimeout(Toast.Timeout timeout)
	{
		toastImage.CrossFadeAlpha(0.5f, 0.1f, true);
		toastText.CrossFadeAlpha(1f, 0.1f, true);
		if (timeout == Toast.Timeout.Automatic)
		{
			uint num = 1u;
			string text = toastText.text;
			for (int i = 0; i < text.Length; i++)
			{
				if (char.IsWhiteSpace(text[i]))
				{
					num++;
				}
			}
			int num2;
			switch (num)
			{
			case 0u:
			case 1u:
			case 2u:
				num2 = 1;
				break;
			case 3u:
			case 4u:
			case 5u:
				num2 = 2;
				break;
			case 6u:
			case 7u:
			case 8u:
			case 9u:
			case 10u:
				num2 = 3;
				break;
			default:
				num2 = 4;
				break;
			}
			timeout = (Toast.Timeout)num2;
		}
		WaitForSeconds waitForSeconds;
		switch (timeout)
		{
		default:
			waitForSeconds = Timeout3;
			break;
		case Toast.Timeout.Long:
			waitForSeconds = Timeout5;
			break;
		case Toast.Timeout.Short:
			waitForSeconds = Timeout2;
			break;
		case Toast.Timeout.Blink:
			waitForSeconds = TimeoutDot5;
			break;
		}
		yield return waitForSeconds;
		yield return FadeAndDestroy();
	}

	private static ToastInternal ToastExists(string id)
	{
		ToastInternal toastInternal = null;
		if (Toasts.ContainsKey(id))
		{
			toastInternal = Toasts[id];
		}
		if ((bool)toastInternal)
		{
			return toastInternal;
		}
		return null;
	}

	private static ToastInternal TryCreate(string id = "_default-Toast")
	{
		if (disabled || !_canvasTransform)
		{
			return null;
		}
		ToastInternal toastInternal = ToastExists(id);
		if ((bool)toastInternal)
		{
			return toastInternal;
		}
		toastInternal = Object.Instantiate(Resources.Load<GameObject>("UI/Toast/Toast"), _canvasTransform, false).GetComponent<ToastInternal>();
		toastInternal.id = id;
		Toasts.Add(id, toastInternal);
		return toastInternal;
	}

	private static ToastInternal TryCreate(ToastInternal toastInternal, string id = "_default-Toast")
	{
		if (!toastInternal)
		{
			return TryCreate(id);
		}
		return toastInternal;
	}

	internal static ToastInternal TryCreate(ToastInternal toastInternal, string id, string message, Toast.Timeout timeout = Toast.Timeout.Automatic)
	{
		if (!toastInternal && string.IsNullOrWhiteSpace(message))
		{
			return null;
		}
		toastInternal = TryCreate(toastInternal, id);
		if ((bool)toastInternal)
		{
			toastInternal.PopUp(message, timeout);
		}
		return toastInternal;
	}
}
