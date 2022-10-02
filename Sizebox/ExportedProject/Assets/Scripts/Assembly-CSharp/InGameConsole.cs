using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class InGameConsole : MonoBehaviour
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Func<char, bool> _003C_003E9__16_0;

		internal bool _003CTrimTop_003Eb__16_0(char x)
		{
			return x == '\n';
		}
	}

	public Text console;

	public GameObject parent;

	private Toast _toast;

	private const int MaxLines = 8;

	private string _lastStr = "";

	private LogType _lastType;

	private uint _repeat;

	public bool visible
	{
		get
		{
			return parent.activeSelf;
		}
		set
		{
			parent.SetActive(value);
		}
	}

	public void Toggle()
	{
		visible = !visible;
	}

	private void Awake()
	{
		console.text = StateManager.LogVersion();
		Application.logMessageReceived += HandleLog;
		_toast = new Toast("_errorLog");
	}

	private void OnDestroy()
	{
		Application.logMessageReceived -= HandleLog;
	}

	private static void IterateLine(Text console, uint repeat)
	{
		if (repeat > 1)
		{
			int num = console.text.LastIndexOf('\n');
			int num2 = console.text.LastIndexOf(" [x", StringComparison.Ordinal);
			if (num2 > 0 && num2 > num)
			{
				console.text = console.text.Substring(0, num2);
			}
		}
		console.text = console.text + " [x" + repeat + "]";
	}

	private static void NewLine(Text console, string logString)
	{
		console.text += logString;
	}

	private static int GetNthIndex(string s, char t, int n)
	{
		int num = 0;
		for (int i = 0; i < s.Length; i++)
		{
			if (s[i] == t)
			{
				num++;
				if (num == n)
				{
					return i;
				}
			}
		}
		return -1;
	}

	private static void TrimTop(Text console)
	{
		int num = console.text.Count(_003C_003Ec._003C_003E9__16_0 ?? (_003C_003Ec._003C_003E9__16_0 = _003C_003Ec._003C_003E9._003CTrimTop_003Eb__16_0)) - 8;
		if (num > 0)
		{
			int nthIndex = GetNthIndex(console.text, '\n', num);
			if (nthIndex > 0)
			{
				console.text = console.text.Substring(nthIndex + 1);
			}
		}
	}

	private static string FormatLog(string log, LogType type)
	{
		return string.Concat(type, ": ", log);
	}

	private void HandleLog(string logString, string stackTrace, LogType type)
	{
		if (ShouldFilter(logString))
		{
			return;
		}
		if (type == _lastType && string.CompareOrdinal(_lastStr, logString) == 0)
		{
			_repeat++;
			IterateLine(console, _repeat);
		}
		else
		{
			if (!string.IsNullOrEmpty(console.text))
			{
				console.text += "\n";
			}
			NewLine(console, FormatLog(logString, type));
			_lastStr = logString;
			_lastType = type;
			_repeat = 1u;
			TrimTop(console);
		}
		if ((uint)type <= 1u || type == LogType.Exception)
		{
			_toast.Print(visible ? null : "A error has occured!\nCheck console for details", Toast.Timeout.Long);
		}
	}

	private bool ShouldFilter(string text)
	{
		if (text.Contains("Screen position out of view frustum"))
		{
			return true;
		}
		return false;
	}
}
