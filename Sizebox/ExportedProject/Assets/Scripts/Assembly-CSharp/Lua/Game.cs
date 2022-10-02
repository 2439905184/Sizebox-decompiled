using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using UnityEngine;

namespace Lua
{
	[MoonSharpUserData]
	public static class Game
	{
		public class Toast
		{
			private string _id;

			private ToastInternal _toastInternal;

			public static Toast New()
			{
				return new Toast
				{
					_id = string.Concat("LuaToast-", Guid.NewGuid(), DateTime.UtcNow)
				};
			}

			public static Toast New(string identifier)
			{
				if (string.IsNullOrWhiteSpace(identifier) || identifier[0] == '_')
				{
					return New();
				}
				return new Toast
				{
					_id = identifier
				};
			}

			public void Print(string message = null)
			{
				_toastInternal = ToastInternal.TryCreate(_toastInternal, _id, message);
			}
		}

		public static class Version
		{
			public static int Major
			{
				get
				{
					return global::Version.GetMajorNumber();
				}
			}

			public static int Minor
			{
				get
				{
					return global::Version.GetMinorNumber();
				}
			}

			public static string Text
			{
				get
				{
					return global::Version.GetText();
				}
			}

			public static bool Require(int major, int minor)
			{
				int majorNumber = global::Version.GetMajorNumber();
				if (majorNumber > major)
				{
					return true;
				}
				if (majorNumber == major && minor >= global::Version.GetMinorNumber())
				{
					return true;
				}
				return false;
			}
		}

		public static Entity GetLocalSelection()
		{
			return InterfaceControl.instance.selectedEntity;
		}

		public static Entity GetLocalPlayer()
		{
			LocalClient localClient = GameController.LocalClient;
			return localClient ? localClient.Player.Entity : null;
		}

		public static IList<Entity> GetLocalSelections()
		{
			Entity entity = InterfaceControl.instance.selectedEntity;
			if (entity != null)
			{
				return new List<Entity> { entity };
			}
			return null;
		}

		public static Player GetLocalPlayerSettings()
		{
			return new Player(GameController.LocalClient);
		}

		public static void Message(string message, string title, bool log = true)
		{
			float time = Time.time;
			if (time > LuaManager.LastMessageBoxTime + 10f)
			{
				LuaManager.LastMessageBoxTime = time;
				if (message != LuaManager.LastMessageBoxText)
				{
					UiMessageBox.Create(message, title).Popup();
					LuaManager.LastMessageBoxText = message;
					if (log)
					{
						Debug.Log("MessageBox: " + (string.IsNullOrEmpty(title) ? message : (title + " - " + message)));
					}
				}
				else
				{
					Debug.Log("MessageBox: " + (string.IsNullOrEmpty(title) ? message : (title + " - " + message)));
				}
			}
			else
			{
				Debug.Log("MessageBox: " + (string.IsNullOrEmpty(title) ? message : (title + " - " + message)));
			}
		}
	}
}
