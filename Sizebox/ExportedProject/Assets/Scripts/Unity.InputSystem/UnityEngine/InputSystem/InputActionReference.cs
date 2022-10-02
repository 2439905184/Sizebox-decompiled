using System;
using System.Linq;

namespace UnityEngine.InputSystem
{
	public class InputActionReference : ScriptableObject
	{
		[SerializeField]
		internal InputActionAsset m_Asset;

		[SerializeField]
		internal string m_ActionId;

		[NonSerialized]
		private InputAction m_Action;

		public InputActionAsset asset
		{
			get
			{
				return m_Asset;
			}
		}

		public InputAction action
		{
			get
			{
				if (m_Action == null)
				{
					if (m_Asset == null)
					{
						return null;
					}
					m_Action = m_Asset.FindAction(new Guid(m_ActionId));
				}
				return m_Action;
			}
		}

		public void Set(InputAction action)
		{
			if (action == null)
			{
				m_Asset = null;
				m_ActionId = null;
				return;
			}
			InputActionMap actionMap = action.actionMap;
			if (actionMap == null || actionMap.asset == null)
			{
				throw new InvalidOperationException(string.Format("Action '{0}' must be part of an InputActionAsset in order to be able to create an InputActionReference for it", action));
			}
			SetInternal(actionMap.asset, action);
		}

		public void Set(InputActionAsset asset, string mapName, string actionName)
		{
			if (asset == null)
			{
				throw new ArgumentNullException("asset");
			}
			if (string.IsNullOrEmpty(mapName))
			{
				throw new ArgumentNullException("mapName");
			}
			if (string.IsNullOrEmpty(actionName))
			{
				throw new ArgumentNullException("actionName");
			}
			InputActionMap inputActionMap = asset.FindActionMap(mapName);
			if (inputActionMap == null)
			{
				throw new ArgumentException(string.Format("No action map '{0}' in '{1}'", mapName, asset), "mapName");
			}
			InputAction inputAction = inputActionMap.FindAction(actionName);
			if (inputAction == null)
			{
				throw new ArgumentException(string.Format("No action '{0}' in map '{1}' of asset '{2}'", actionName, mapName, asset), "actionName");
			}
			SetInternal(asset, inputAction);
		}

		private void SetInternal(InputActionAsset asset, InputAction action)
		{
			InputActionMap actionMap = action.actionMap;
			if (!asset.actionMaps.Contains(actionMap))
			{
				throw new ArgumentException(string.Format("Action '{0}' is not contained in asset '{1}'", action, asset), "action");
			}
			m_Asset = asset;
			m_ActionId = action.id.ToString();
		}

		public override string ToString()
		{
			try
			{
				InputAction inputAction = action;
				return m_Asset.name + ":" + inputAction.actionMap.name + "/" + inputAction.name;
			}
			catch
			{
				if (m_Asset != null)
				{
					return m_Asset.name + ":" + m_ActionId;
				}
			}
			return base.ToString();
		}

		public static implicit operator InputAction(InputActionReference reference)
		{
			if ((object)reference == null)
			{
				return null;
			}
			return reference.action;
		}

		public static InputActionReference Create(InputAction action)
		{
			if (action == null)
			{
				return null;
			}
			InputActionReference inputActionReference = ScriptableObject.CreateInstance<InputActionReference>();
			inputActionReference.Set(action);
			return inputActionReference;
		}

		public InputAction ToInputAction()
		{
			return action;
		}
	}
}
