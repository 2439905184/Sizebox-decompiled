using MoonSharp.Interpreter;
using UnityEngine;

namespace Lua
{
	[MoonSharpUserData]
	public class Input
	{
		public static bool anyKey
		{
			get
			{
				if (GameController.Instance.allowCustomKeys)
				{
					return UnityEngine.Input.anyKey;
				}
				return false;
			}
		}

		public static bool anyKeyDown
		{
			get
			{
				if (GameController.Instance.allowCustomKeys)
				{
					return UnityEngine.Input.anyKeyDown;
				}
				return false;
			}
		}

		public static Vector3 mousePosition
		{
			get
			{
				return new Vector3(UnityEngine.Input.mousePosition);
			}
		}

		public static Vector3 mouseScrollDelta
		{
			get
			{
				return new Vector3(UnityEngine.Input.mouseScrollDelta);
			}
		}

		public static float GetAxis(string axisName)
		{
			return UnityEngine.Input.GetAxis(axisName);
		}

		public static float GetAxisRaw(string axisName)
		{
			return UnityEngine.Input.GetAxisRaw(axisName);
		}

		public static bool GetButton(string buttonName)
		{
			if (GameController.Instance.allowCustomKeys)
			{
				return UnityEngine.Input.GetButton(buttonName);
			}
			return false;
		}

		public static bool GetButtonDown(string buttonName)
		{
			if (GameController.Instance.allowCustomKeys)
			{
				return UnityEngine.Input.GetButtonDown(buttonName);
			}
			return false;
		}

		public static bool GetButtonUp(string buttonName)
		{
			if (GameController.Instance.allowCustomKeys)
			{
				return UnityEngine.Input.GetButtonUp(buttonName);
			}
			return false;
		}

		public static bool GetKey(string name)
		{
			if (GameController.Instance.allowCustomKeys)
			{
				return UnityEngine.Input.GetKey(name);
			}
			return false;
		}

		public static bool GetKeyDown(string name)
		{
			if (GameController.Instance.allowCustomKeys)
			{
				return UnityEngine.Input.GetKeyDown(name);
			}
			return false;
		}

		public static bool GetKeyUp(string name)
		{
			if (GameController.Instance.allowCustomKeys)
			{
				return UnityEngine.Input.GetKeyUp(name);
			}
			return false;
		}

		public static bool GetMouseButton(int button)
		{
			if (GameController.Instance.allowCustomKeys)
			{
				return UnityEngine.Input.GetMouseButton(button);
			}
			return false;
		}

		public static bool GetMouseButtonDown(int button)
		{
			if (GameController.Instance.allowCustomKeys)
			{
				return UnityEngine.Input.GetMouseButtonDown(button);
			}
			return false;
		}

		public static bool GetMouseButtonUp(int button)
		{
			if (GameController.Instance.allowCustomKeys)
			{
				return UnityEngine.Input.GetMouseButtonUp(button);
			}
			return false;
		}
	}
}
