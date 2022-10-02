using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
	public class InputActionAsset : ScriptableObject, IInputActionCollection, IEnumerable<InputAction>, IEnumerable
	{
		[Serializable]
		internal struct WriteFileJson
		{
			public string name;

			public InputActionMap.WriteMapJson[] maps;

			public InputControlScheme.SchemeJson[] controlSchemes;
		}

		[Serializable]
		internal struct ReadFileJson
		{
			public string name;

			public InputActionMap.ReadMapJson[] maps;

			public InputControlScheme.SchemeJson[] controlSchemes;

			public void ToAsset(InputActionAsset asset)
			{
				asset.name = name;
				asset.m_ActionMaps = new InputActionMap.ReadFileJson
				{
					maps = maps
				}.ToMaps();
				asset.m_ControlSchemes = InputControlScheme.SchemeJson.ToSchemes(controlSchemes);
				if (asset.m_ActionMaps != null)
				{
					InputActionMap[] actionMaps = asset.m_ActionMaps;
					for (int i = 0; i < actionMaps.Length; i++)
					{
						actionMaps[i].m_Asset = asset;
					}
				}
			}
		}

		public const string Extension = "inputactions";

		[SerializeField]
		internal InputActionMap[] m_ActionMaps;

		[SerializeField]
		internal InputControlScheme[] m_ControlSchemes;

		[NonSerialized]
		internal InputActionState m_SharedStateForAllMaps;

		[NonSerialized]
		internal InputBinding? m_BindingMask;

		[NonSerialized]
		private int m_DevicesCount = -1;

		[NonSerialized]
		private InputDevice[] m_DevicesArray;

		public bool enabled
		{
			get
			{
				foreach (InputActionMap actionMap in actionMaps)
				{
					if (actionMap.enabled)
					{
						return true;
					}
				}
				return false;
			}
		}

		public ReadOnlyArray<InputActionMap> actionMaps
		{
			get
			{
				return new ReadOnlyArray<InputActionMap>(m_ActionMaps);
			}
		}

		public ReadOnlyArray<InputControlScheme> controlSchemes
		{
			get
			{
				return new ReadOnlyArray<InputControlScheme>(m_ControlSchemes);
			}
		}

		public InputBinding? bindingMask
		{
			get
			{
				return m_BindingMask;
			}
			set
			{
				if (!(m_BindingMask == value))
				{
					m_BindingMask = value;
					ReResolveIfNecessary();
				}
			}
		}

		public ReadOnlyArray<InputDevice>? devices
		{
			get
			{
				if (m_DevicesCount < 0)
				{
					return null;
				}
				return new ReadOnlyArray<InputDevice>(m_DevicesArray, 0, m_DevicesCount);
			}
			set
			{
				if (!value.HasValue)
				{
					if (m_DevicesCount < 0)
					{
						return;
					}
					if ((m_DevicesArray != null) & (m_DevicesCount > 0))
					{
						Array.Clear(m_DevicesArray, 0, m_DevicesCount);
					}
					m_DevicesCount = -1;
				}
				else
				{
					if (m_DevicesCount == value.Value.Count)
					{
						bool flag = true;
						for (int i = 0; i < m_DevicesCount; i++)
						{
							if (m_DevicesArray[i] != value.Value[i])
							{
								flag = false;
								break;
							}
						}
						if (flag)
						{
							return;
						}
					}
					if (m_DevicesCount > 0)
					{
						m_DevicesArray.Clear(ref m_DevicesCount);
					}
					m_DevicesCount = 0;
					ArrayHelpers.AppendListWithCapacity(ref m_DevicesArray, ref m_DevicesCount, value.Value);
				}
				ReResolveIfNecessary();
			}
		}

		public InputAction this[string actionNameOrId]
		{
			get
			{
				InputAction inputAction = FindAction(actionNameOrId);
				if (inputAction == null)
				{
					throw new KeyNotFoundException(string.Format("Cannot find action '{0}' in '{1}'", actionNameOrId, this));
				}
				return inputAction;
			}
		}

		public string ToJson()
		{
			WriteFileJson writeFileJson = default(WriteFileJson);
			writeFileJson.name = base.name;
			writeFileJson.maps = InputActionMap.WriteFileJson.FromMaps(m_ActionMaps).maps;
			writeFileJson.controlSchemes = InputControlScheme.SchemeJson.ToJson(m_ControlSchemes);
			return JsonUtility.ToJson(writeFileJson, true);
		}

		public void LoadFromJson(string json)
		{
			if (string.IsNullOrEmpty(json))
			{
				throw new ArgumentNullException("json");
			}
			JsonUtility.FromJson<ReadFileJson>(json).ToAsset(this);
		}

		public static InputActionAsset FromJson(string json)
		{
			if (string.IsNullOrEmpty(json))
			{
				throw new ArgumentNullException("json");
			}
			InputActionAsset inputActionAsset = ScriptableObject.CreateInstance<InputActionAsset>();
			inputActionAsset.LoadFromJson(json);
			return inputActionAsset;
		}

		public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
		{
			if (actionNameOrId == null)
			{
				throw new ArgumentNullException("actionNameOrId");
			}
			if (m_ActionMaps != null)
			{
				int num = actionNameOrId.IndexOf('/');
				if (num == -1)
				{
					for (int i = 0; i < m_ActionMaps.Length; i++)
					{
						InputAction inputAction = m_ActionMaps[i].FindAction(actionNameOrId);
						if (inputAction != null)
						{
							return inputAction;
						}
					}
				}
				else
				{
					Substring right = new Substring(actionNameOrId, 0, num);
					Substring right2 = new Substring(actionNameOrId, num + 1);
					if (right.isEmpty || right2.isEmpty)
					{
						throw new ArgumentException("Malformed action path: " + actionNameOrId, "actionNameOrId");
					}
					for (int j = 0; j < m_ActionMaps.Length; j++)
					{
						InputActionMap inputActionMap = m_ActionMaps[j];
						if (Substring.Compare(inputActionMap.name, right, StringComparison.InvariantCultureIgnoreCase) != 0)
						{
							continue;
						}
						InputAction[] actions = inputActionMap.m_Actions;
						foreach (InputAction inputAction2 in actions)
						{
							if (Substring.Compare(inputAction2.name, right2, StringComparison.InvariantCultureIgnoreCase) == 0)
							{
								return inputAction2;
							}
						}
						break;
					}
				}
			}
			if (throwIfNotFound)
			{
				throw new ArgumentException(string.Format("No action '{0}' in '{1}'", actionNameOrId, this));
			}
			return null;
		}

		public InputActionMap FindActionMap(string nameOrId, bool throwIfNotFound = false)
		{
			if (nameOrId == null)
			{
				throw new ArgumentNullException("nameOrId");
			}
			if (m_ActionMaps == null)
			{
				return null;
			}
			Guid result;
			if (nameOrId.Contains('-') && Guid.TryParse(nameOrId, out result))
			{
				for (int i = 0; i < m_ActionMaps.Length; i++)
				{
					InputActionMap inputActionMap = m_ActionMaps[i];
					if (inputActionMap.idDontGenerate == result)
					{
						return inputActionMap;
					}
				}
			}
			for (int j = 0; j < m_ActionMaps.Length; j++)
			{
				InputActionMap inputActionMap2 = m_ActionMaps[j];
				if (string.Compare(nameOrId, inputActionMap2.name, StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					return inputActionMap2;
				}
			}
			if (throwIfNotFound)
			{
				throw new ArgumentException(string.Format("Cannot find action map '{0}' in '{1}'", nameOrId, this));
			}
			return null;
		}

		public InputActionMap FindActionMap(Guid id)
		{
			if (m_ActionMaps == null)
			{
				return null;
			}
			for (int i = 0; i < m_ActionMaps.Length; i++)
			{
				InputActionMap inputActionMap = m_ActionMaps[i];
				if (inputActionMap.idDontGenerate == id)
				{
					return inputActionMap;
				}
			}
			return null;
		}

		public InputAction FindAction(Guid guid)
		{
			if (m_ActionMaps == null)
			{
				return null;
			}
			for (int i = 0; i < m_ActionMaps.Length; i++)
			{
				InputAction inputAction = m_ActionMaps[i].FindAction(guid);
				if (inputAction != null)
				{
					return inputAction;
				}
			}
			return null;
		}

		public int FindControlSchemeIndex(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			if (m_ControlSchemes == null)
			{
				return -1;
			}
			for (int i = 0; i < m_ControlSchemes.Length; i++)
			{
				if (string.Compare(name, m_ControlSchemes[i].name, StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					return i;
				}
			}
			return -1;
		}

		public InputControlScheme? FindControlScheme(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			int num = FindControlSchemeIndex(name);
			if (num == -1)
			{
				return null;
			}
			return m_ControlSchemes[num];
		}

		public void Enable()
		{
			foreach (InputActionMap actionMap in actionMaps)
			{
				actionMap.Enable();
			}
		}

		public void Disable()
		{
			foreach (InputActionMap actionMap in actionMaps)
			{
				actionMap.Disable();
			}
		}

		public bool Contains(InputAction action)
		{
			InputActionMap inputActionMap = ((action != null) ? action.actionMap : null);
			if (inputActionMap == null)
			{
				return false;
			}
			return inputActionMap.asset == this;
		}

		public IEnumerator<InputAction> GetEnumerator()
		{
			if (m_ActionMaps == null)
			{
				yield break;
			}
			int i = 0;
			while (i < m_ActionMaps.Length)
			{
				ReadOnlyArray<InputAction> actions = m_ActionMaps[i].actions;
				int actionCount = actions.Count;
				int num;
				for (int j = 0; j < actionCount; j = num)
				{
					yield return actions[j];
					num = j + 1;
				}
				num = i + 1;
				i = num;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private void ReResolveIfNecessary()
		{
			if (m_SharedStateForAllMaps != null)
			{
				m_ActionMaps[0].LazyResolveBindings();
			}
		}

		private void OnDestroy()
		{
			Disable();
			if (m_SharedStateForAllMaps != null)
			{
				m_SharedStateForAllMaps.Dispose();
				m_SharedStateForAllMaps = null;
			}
		}
	}
}
