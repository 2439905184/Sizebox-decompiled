using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class Sbox
{
	public static class ToastId
	{
		public const string GameSpeed = "_GameSpeed";

		public const string PlayerSize = "_PlayerSize";
	}

	private const float DamageDivisor = 150f;

	private const float DestructionDivisor = 100f;

	public const float MacroScaleMultiplier = 1000f;

	public static bool isAprilFools
	{
		get
		{
			DateTime now = DateTime.Now;
			if (now.Month == 4)
			{
				return now.Day == 1;
			}
			return false;
		}
	}

	public static float DistanceVertical(Vector3 a, Vector3 b)
	{
		return Math.Abs(a.y - b.y);
	}

	public static float DistanceHorizontal(Vector3 a, Vector3 b)
	{
		float num = a.x - b.x;
		float num2 = a.z - b.z;
		return (float)Math.Sqrt((double)num * (double)num + (double)num2 * (double)num2);
	}

	public static float SetScale(float baseDistance, float scale)
	{
		return baseDistance * (1f / scale);
	}

	public static float GetScale(float relativeDistance, float scale)
	{
		return relativeDistance * scale;
	}

	public static float ScaleToMeter(float scale)
	{
		return scale / GameController.ReferenceScale;
	}

	public static float MeterToScale(float meters)
	{
		return meters * GameController.ReferenceScale;
	}

	public static float ConvertFootInchToMeters(string footInch, bool nativeFormat = false)
	{
		string[] array = footInch.Split('\'');
		float result = float.NegativeInfinity;
		if (array.Length != 0 && float.TryParse(array[0], NumberStyles.Float, nativeFormat ? CultureInfo.CurrentCulture : CultureInfo.InvariantCulture, out result))
		{
			result *= 0.3048f;
			float result2;
			if (array.Length == 2 && float.TryParse(array[1], out result2))
			{
				result += result2 * 0.0254f;
			}
		}
		return result;
	}

	public static void CreateSubDirectory(string directory, string subdirectory)
	{
		if (subdirectory.Contains("/"))
		{
			DirectoryInfo parent = Directory.GetParent(subdirectory);
			string path = ((parent != null) ? parent.ToString() : null);
			string text = Path.Combine(directory, path);
			if (!Directory.Exists(text))
			{
				try
				{
					Directory.CreateDirectory(text);
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogWarning("Error creating directory '" + text + "' " + ex);
				}
			}
		}
		string text2 = Path.Combine(directory, subdirectory);
		if (!Directory.Exists(text2))
		{
			try
			{
				Directory.CreateDirectory(text2);
			}
			catch (Exception ex2)
			{
				UnityEngine.Debug.LogWarning("Error creating directory '" + text2 + "' " + ex2);
			}
		}
	}

	public static void CreateSubDirectory(string directory, List<string> subdirectories)
	{
		foreach (string subdirectory in subdirectories)
		{
			CreateSubDirectory(directory, subdirectory);
		}
	}

	public static string StringPreferenceOrArgument(StringStored stringStored, string flag, string subdirectory = null)
	{
		bool processFlag = GetProcessFlag("-path-priority");
		string value;
		if (!processFlag)
		{
			value = stringStored.value;
			if (!string.IsNullOrEmpty(value))
			{
				return value;
			}
		}
		value = GetProcessFlagArgument(flag);
		if (string.IsNullOrEmpty(value))
		{
			value = GetProcessFlagArgument("-path");
			if (string.IsNullOrEmpty(value))
			{
				return string.Empty;
			}
			if (!string.IsNullOrEmpty(subdirectory))
			{
				value = Path.Combine(value, subdirectory);
			}
		}
		else if (processFlag)
		{
			value = stringStored.value;
		}
		value = Path.GetFullPath(value);
		if (!Directory.Exists(value))
		{
			return string.Empty;
		}
		return value;
	}

	public static void OsViewFolder(string path)
	{
		if (!Directory.Exists(path))
		{
			UnityEngine.Debug.LogWarning("Couldn't find requested directory '" + path + "'");
		}
		else
		{
			Process.Start(path);
		}
	}

	public static bool GetProcessFlag(string flag)
	{
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		for (int i = 0; i < commandLineArgs.Length; i++)
		{
			if (commandLineArgs[i] == flag)
			{
				return true;
			}
		}
		return false;
	}

	public static string GetProcessFlagArgument(string flag)
	{
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		for (int i = 0; i < commandLineArgs.Length; i++)
		{
			if (commandLineArgs[i] == flag)
			{
				if (commandLineArgs.Length > i + 1 && !commandLineArgs[i + 1].StartsWith("-"))
				{
					return commandLineArgs[i + 1];
				}
				return string.Empty;
			}
		}
		return null;
	}

	public static InputField AddSBoxInputField(GameObject gameObject)
	{
		InputField inputField = gameObject.AddComponent<InputField>();
		AddSBoxInputFieldEvents(inputField);
		return inputField;
	}

	public static void AddSBoxInputFieldEvents(InputField inputField)
	{
		EventTrigger eventTrigger = inputField.gameObject.AddComponent<EventTrigger>();
		EventTrigger.Entry entry = new EventTrigger.Entry
		{
			eventID = EventTriggerType.Select
		};
		entry.callback.AddListener(OnEntrySelect);
		eventTrigger.triggers.Add(entry);
		entry = new EventTrigger.Entry
		{
			eventID = EventTriggerType.Deselect
		};
		entry.callback.AddListener(OnEntryDeselect);
		eventTrigger.triggers.Add(entry);
	}

	private static void OnEntryDeselect(BaseEventData arg0)
	{
		StateManager.Keyboard.userIsTyping = false;
	}

	private static void OnEntrySelect(BaseEventData arg0)
	{
		StateManager.Keyboard.userIsTyping = true;
	}

	public static float CalculateDamageDealt(EntityBase entity)
	{
		Rigidbody rigidbody = entity.Rigidbody;
		float num = (rigidbody ? rigidbody.mass : 0f);
		float num2 = (rigidbody ? rigidbody.velocity.magnitude : 0f);
		return CalculateDamageDealt(num * num2);
	}

	public static float CalculateDamageDealt(float force)
	{
		return force / 150f;
	}

	public static float CalculateDestructionForce(EntityBase entity)
	{
		Rigidbody rigidbody = entity.Rigidbody;
		float mass = (rigidbody ? rigidbody.mass : 0f);
		float velocity = (rigidbody ? rigidbody.velocity.magnitude : 0f);
		return CalculateDestructionForce(mass, velocity);
	}

	public static float CalculateDestructionForce(float mass, float velocity)
	{
		return mass * velocity / 100f;
	}
}
