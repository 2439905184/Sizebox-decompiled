using System;
using System.Runtime.CompilerServices;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
	public static class InputActionSetupExtensions
	{
		public struct BindingSyntax
		{
			private readonly InputActionMap m_ActionMap;

			private readonly InputAction m_Action;

			internal readonly int m_BindingIndex;

			internal BindingSyntax And
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			internal BindingSyntax(InputActionMap map, InputAction action, int bindingIndex)
			{
				m_ActionMap = map;
				m_Action = action;
				m_BindingIndex = bindingIndex;
			}

			public BindingSyntax WithName(string name)
			{
				m_ActionMap.m_Bindings[m_BindingIndex].name = name;
				m_ActionMap.ClearPerActionCachedBindingData();
				m_ActionMap.LazyResolveBindings();
				return this;
			}

			public BindingSyntax WithPath(string path)
			{
				m_ActionMap.m_Bindings[m_BindingIndex].path = path;
				m_ActionMap.ClearPerActionCachedBindingData();
				m_ActionMap.LazyResolveBindings();
				return this;
			}

			public BindingSyntax WithGroup(string group)
			{
				if (string.IsNullOrEmpty(group))
				{
					throw new ArgumentException("Group name cannot be null or empty", "group");
				}
				if (group.IndexOf(';') != -1)
				{
					throw new ArgumentException(string.Format("Group name cannot contain separator character '{0}'", ';'), "group");
				}
				return WithGroups(group);
			}

			public BindingSyntax WithGroups(string groups)
			{
				if (string.IsNullOrEmpty(groups))
				{
					return this;
				}
				string groups2 = m_ActionMap.m_Bindings[m_BindingIndex].groups;
				if (!string.IsNullOrEmpty(groups2))
				{
					groups = string.Join(";", groups2, groups);
				}
				m_ActionMap.m_Bindings[m_BindingIndex].groups = groups;
				m_ActionMap.ClearPerActionCachedBindingData();
				m_ActionMap.LazyResolveBindings();
				return this;
			}

			public BindingSyntax WithInteraction(string interaction)
			{
				if (string.IsNullOrEmpty(interaction))
				{
					throw new ArgumentException("Interaction cannot be null or empty", "interaction");
				}
				if (interaction.IndexOf(';') != -1)
				{
					throw new ArgumentException(string.Format("Interaction string cannot contain separator character '{0}'", ';'), "interaction");
				}
				return WithInteractions(interaction);
			}

			public BindingSyntax WithInteractions(string interactions)
			{
				if (string.IsNullOrEmpty(interactions))
				{
					return this;
				}
				string interactions2 = m_ActionMap.m_Bindings[m_BindingIndex].interactions;
				if (!string.IsNullOrEmpty(interactions2))
				{
					interactions = string.Join(";", interactions2, interactions);
				}
				m_ActionMap.m_Bindings[m_BindingIndex].interactions = interactions;
				m_ActionMap.ClearPerActionCachedBindingData();
				m_ActionMap.LazyResolveBindings();
				return this;
			}

			public BindingSyntax WithInteraction<TInteraction>() where TInteraction : IInputInteraction
			{
				InternedString internedString = InputProcessor.s_Processors.FindNameForType(typeof(TInteraction));
				if (internedString.IsEmpty())
				{
					throw new NotSupportedException(string.Format("Type '{0}' has not been registered as a processor", typeof(TInteraction)));
				}
				return WithInteraction(internedString);
			}

			public BindingSyntax WithProcessor(string processor)
			{
				if (string.IsNullOrEmpty(processor))
				{
					throw new ArgumentException("Processor cannot be null or empty", "processor");
				}
				if (processor.IndexOf(';') != -1)
				{
					throw new ArgumentException(string.Format("Interaction string cannot contain separator character '{0}'", ';'), "processor");
				}
				return WithProcessors(processor);
			}

			public BindingSyntax WithProcessors(string processors)
			{
				if (string.IsNullOrEmpty(processors))
				{
					return this;
				}
				string processors2 = m_ActionMap.m_Bindings[m_BindingIndex].processors;
				if (!string.IsNullOrEmpty(processors2))
				{
					processors = string.Join(";", processors2, processors);
				}
				m_ActionMap.m_Bindings[m_BindingIndex].processors = processors;
				m_ActionMap.ClearPerActionCachedBindingData();
				m_ActionMap.LazyResolveBindings();
				return this;
			}

			public BindingSyntax WithProcessor<TProcessor>()
			{
				InternedString internedString = InputProcessor.s_Processors.FindNameForType(typeof(TProcessor));
				if (internedString.IsEmpty())
				{
					throw new NotSupportedException(string.Format("Type '{0}' has not been registered as a processor", typeof(TProcessor)));
				}
				return WithProcessor(internedString);
			}

			public BindingSyntax Triggering(InputAction action)
			{
				if (action == null)
				{
					throw new ArgumentNullException("action");
				}
				if (action.isSingletonAction)
				{
					throw new ArgumentException(string.Format("Cannot change the action a binding triggers on singleton action '{0}'", action), "action");
				}
				m_ActionMap.m_Bindings[m_BindingIndex].action = action.name;
				m_ActionMap.ClearPerActionCachedBindingData();
				m_ActionMap.LazyResolveBindings();
				return this;
			}

			public BindingSyntax To(InputBinding binding)
			{
				m_ActionMap.m_Bindings[m_BindingIndex] = binding;
				m_ActionMap.ClearPerActionCachedBindingData();
				m_ActionMap.LazyResolveBindings();
				if (m_ActionMap.m_SingletonAction != null)
				{
					m_ActionMap.m_Bindings[m_BindingIndex].action = m_Action.name;
				}
				return this;
			}

			public void Erase()
			{
				ArrayHelpers.EraseAt(ref m_ActionMap.m_Bindings, m_BindingIndex);
				m_ActionMap.ClearPerActionCachedBindingData();
				m_ActionMap.LazyResolveBindings();
				if (m_ActionMap.m_SingletonAction != null)
				{
					m_ActionMap.m_SingletonAction.m_SingletonActionBindings = m_ActionMap.m_Bindings;
				}
			}
		}

		public struct CompositeSyntax
		{
			private readonly InputAction m_Action;

			private readonly InputActionMap m_ActionMap;

			private int m_CompositeIndex;

			internal CompositeSyntax(InputActionMap map, InputAction action, int compositeIndex)
			{
				m_Action = action;
				m_ActionMap = map;
				m_CompositeIndex = compositeIndex;
			}

			public CompositeSyntax With(string name, string binding, string groups = null)
			{
				int num = ((m_Action == null) ? m_ActionMap.AddBinding(binding, (string)null, groups, (string)null).m_BindingIndex : m_Action.AddBinding(binding, null, null, groups).m_BindingIndex);
				m_ActionMap.m_Bindings[num].name = name;
				m_ActionMap.m_Bindings[num].isPartOfComposite = true;
				return this;
			}
		}

		public struct ControlSchemeSyntax
		{
			private readonly InputActionAsset m_Asset;

			private readonly int m_ControlSchemeIndex;

			private InputControlScheme m_ControlScheme;

			internal ControlSchemeSyntax(InputActionAsset asset, int index)
			{
				m_Asset = asset;
				m_ControlSchemeIndex = index;
				m_ControlScheme = default(InputControlScheme);
			}

			internal ControlSchemeSyntax(InputControlScheme controlScheme)
			{
				m_Asset = null;
				m_ControlSchemeIndex = -1;
				m_ControlScheme = controlScheme;
			}

			public ControlSchemeSyntax WithBindingGroup(string bindingGroup)
			{
				if (string.IsNullOrEmpty(bindingGroup))
				{
					throw new ArgumentNullException("bindingGroup");
				}
				if (m_Asset == null)
				{
					m_ControlScheme.m_BindingGroup = bindingGroup;
				}
				else
				{
					m_Asset.m_ControlSchemes[m_ControlSchemeIndex].bindingGroup = bindingGroup;
				}
				return this;
			}

			public ControlSchemeSyntax WithRequiredDevice<TDevice>() where TDevice : InputDevice
			{
				return WithRequiredDevice(DeviceTypeToControlPath<TDevice>());
			}

			public ControlSchemeSyntax WithOptionalDevice<TDevice>() where TDevice : InputDevice
			{
				return WithOptionalDevice(DeviceTypeToControlPath<TDevice>());
			}

			public ControlSchemeSyntax OrWithRequiredDevice<TDevice>() where TDevice : InputDevice
			{
				return WithRequiredDevice(DeviceTypeToControlPath<TDevice>());
			}

			public ControlSchemeSyntax OrWithOptionalDevice<TDevice>() where TDevice : InputDevice
			{
				return WithOptionalDevice(DeviceTypeToControlPath<TDevice>());
			}

			public ControlSchemeSyntax WithRequiredDevice(string controlPath)
			{
				AddDeviceEntry(controlPath, InputControlScheme.DeviceRequirement.Flags.None);
				return this;
			}

			public ControlSchemeSyntax WithOptionalDevice(string controlPath)
			{
				AddDeviceEntry(controlPath, InputControlScheme.DeviceRequirement.Flags.Optional);
				return this;
			}

			public ControlSchemeSyntax OrWithRequiredDevice(string controlPath)
			{
				AddDeviceEntry(controlPath, InputControlScheme.DeviceRequirement.Flags.Or);
				return this;
			}

			public ControlSchemeSyntax OrWithOptionalDevice(string controlPath)
			{
				AddDeviceEntry(controlPath, InputControlScheme.DeviceRequirement.Flags.Optional | InputControlScheme.DeviceRequirement.Flags.Or);
				return this;
			}

			private string DeviceTypeToControlPath<TDevice>() where TDevice : InputDevice
			{
				string text = InputControlLayout.s_Layouts.TryFindLayoutForType(typeof(TDevice)).ToString();
				if (string.IsNullOrEmpty(text))
				{
					text = typeof(TDevice).Name;
				}
				return "<" + text + ">";
			}

			public InputControlScheme Done()
			{
				if (m_Asset != null)
				{
					return m_Asset.m_ControlSchemes[m_ControlSchemeIndex];
				}
				return m_ControlScheme;
			}

			private void AddDeviceEntry(string controlPath, InputControlScheme.DeviceRequirement.Flags flags)
			{
				if (string.IsNullOrEmpty(controlPath))
				{
					throw new ArgumentNullException("controlPath");
				}
				InputControlScheme inputControlScheme = ((m_Asset != null) ? m_Asset.m_ControlSchemes[m_ControlSchemeIndex] : m_ControlScheme);
				ArrayHelpers.Append(ref inputControlScheme.m_DeviceRequirements, new InputControlScheme.DeviceRequirement
				{
					m_ControlPath = controlPath,
					m_Flags = flags
				});
				if (m_Asset == null)
				{
					m_ControlScheme = inputControlScheme;
				}
				else
				{
					m_Asset.m_ControlSchemes[m_ControlSchemeIndex] = inputControlScheme;
				}
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass5_0
		{
			public InputBinding binding;

			internal bool _003CRemoveAction_003Eb__0(InputBinding b)
			{
				return b == binding;
			}
		}

		public static InputActionMap AddActionMap(this InputActionAsset asset, string name)
		{
			if (asset == null)
			{
				throw new ArgumentNullException("asset");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			if (asset.FindActionMap(name) != null)
			{
				throw new InvalidOperationException("An action map called '" + name + "' already exists in the asset");
			}
			InputActionMap inputActionMap = new InputActionMap(name);
			inputActionMap.GenerateId();
			asset.AddActionMap(inputActionMap);
			return inputActionMap;
		}

		public static void AddActionMap(this InputActionAsset asset, InputActionMap map)
		{
			if (asset == null)
			{
				throw new ArgumentNullException("asset");
			}
			if (map == null)
			{
				throw new ArgumentNullException("map");
			}
			if (string.IsNullOrEmpty(map.name))
			{
				throw new InvalidOperationException("Maps added to an input action asset must be named");
			}
			if (map.asset != null)
			{
				throw new InvalidOperationException(string.Format("Cannot add map '{0}' to asset '{1}' as it has already been added to asset '{2}'", map, asset, map.asset));
			}
			if (asset.FindActionMap(map.name) != null)
			{
				throw new InvalidOperationException("An action map called '" + map.name + "' already exists in the asset");
			}
			ArrayHelpers.Append(ref asset.m_ActionMaps, map);
			map.m_Asset = asset;
		}

		public static void RemoveActionMap(this InputActionAsset asset, InputActionMap map)
		{
			if (asset == null)
			{
				throw new ArgumentNullException("asset");
			}
			if (map == null)
			{
				throw new ArgumentNullException("map");
			}
			if (map.enabled)
			{
				throw new InvalidOperationException("Cannot remove an action map from the asset while it is enabled");
			}
			if (!(map.m_Asset != asset))
			{
				ArrayHelpers.Erase(ref asset.m_ActionMaps, map);
				map.m_Asset = null;
			}
		}

		public static void RemoveActionMap(this InputActionAsset asset, string nameOrId)
		{
			if (asset == null)
			{
				throw new ArgumentNullException("asset");
			}
			if (nameOrId == null)
			{
				throw new ArgumentNullException("nameOrId");
			}
			InputActionMap inputActionMap = asset.FindActionMap(nameOrId);
			if (inputActionMap != null)
			{
				asset.RemoveActionMap(inputActionMap);
			}
		}

		public static InputAction AddAction(this InputActionMap map, string name, InputActionType type = InputActionType.Value, string binding = null, string interactions = null, string processors = null, string groups = null, string expectedControlLayout = null)
		{
			if (map == null)
			{
				throw new ArgumentNullException("map");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("Action must have name", "name");
			}
			if (map.enabled)
			{
				throw new InvalidOperationException(string.Format("Cannot add action '{0}' to map '{1}' while it the map is enabled", name, map));
			}
			if (map.FindAction(name) != null)
			{
				throw new InvalidOperationException("Cannot add action with duplicate name '" + name + "' to set '" + map.name + "'");
			}
			InputAction inputAction = new InputAction(name, type)
			{
				expectedControlType = expectedControlLayout
			};
			inputAction.GenerateId();
			ArrayHelpers.Append(ref map.m_Actions, inputAction);
			inputAction.m_ActionMap = map;
			if (!string.IsNullOrEmpty(binding))
			{
				inputAction.AddBinding(binding, interactions, processors, groups);
			}
			else
			{
				if (!string.IsNullOrEmpty(groups))
				{
					throw new ArgumentException(string.Format("No binding path was specified for action '{0}' but groups was specified ('{1}'); cannot apply groups without binding", inputAction, groups), "groups");
				}
				inputAction.m_Interactions = interactions;
				inputAction.m_Processors = processors;
			}
			return inputAction;
		}

		public static void RemoveAction(this InputAction action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			InputActionMap actionMap = action.actionMap;
			if (actionMap == null)
			{
				throw new ArgumentException(string.Format("Action '{0}' does not belong to an action map; nowhere to remove from", action), "action");
			}
			if (actionMap.enabled)
			{
				throw new ArgumentException(string.Format("Cannot remove action '{0}' while its action map is enabled", action));
			}
			InputBinding[] array = action.bindings.ToArray();
			int index = actionMap.m_Actions.IndexOfReference(action);
			ArrayHelpers.EraseAt(ref actionMap.m_Actions, index);
			action.m_ActionMap = null;
			action.m_SingletonActionBindings = array;
			actionMap.ClearPerActionCachedBindingData();
			int num = actionMap.m_Bindings.Length - array.Length;
			if (num == 0)
			{
				actionMap.m_Bindings = null;
				return;
			}
			InputBinding[] array2 = new InputBinding[num];
			InputBinding[] bindings = actionMap.m_Bindings;
			int num2 = 0;
			for (int i = 0; i < bindings.Length; i++)
			{
				_003C_003Ec__DisplayClass5_0 _003C_003Ec__DisplayClass5_ = new _003C_003Ec__DisplayClass5_0();
				_003C_003Ec__DisplayClass5_.binding = bindings[i];
				if (array.IndexOf(_003C_003Ec__DisplayClass5_._003CRemoveAction_003Eb__0) == -1)
				{
					array2[num2++] = _003C_003Ec__DisplayClass5_.binding;
				}
			}
			actionMap.m_Bindings = array2;
		}

		public static void RemoveAction(this InputActionAsset asset, string nameOrId)
		{
			if (asset == null)
			{
				throw new ArgumentNullException("asset");
			}
			if (nameOrId == null)
			{
				throw new ArgumentNullException("nameOrId");
			}
			InputAction inputAction = asset.FindAction(nameOrId);
			if (inputAction != null)
			{
				inputAction.RemoveAction();
			}
		}

		public static BindingSyntax AddBinding(this InputAction action, string path, string interactions = null, string processors = null, string groups = null)
		{
			return action.AddBinding(new InputBinding
			{
				path = path,
				interactions = interactions,
				processors = processors,
				groups = groups
			});
		}

		public static BindingSyntax AddBinding(this InputAction action, InputControl control)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			return action.AddBinding(control.path);
		}

		public static BindingSyntax AddBinding(this InputAction action, InputBinding binding = default(InputBinding))
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			binding.action = action.name;
			InputActionMap orCreateActionMap = action.GetOrCreateActionMap();
			int bindingIndex = AddBindingInternal(orCreateActionMap, binding);
			return new BindingSyntax(orCreateActionMap, action, bindingIndex);
		}

		public static BindingSyntax AddBinding(this InputActionMap actionMap, string path, string interactions = null, string groups = null, string action = null)
		{
			if (path == null)
			{
				throw new ArgumentException("Binding path cannot be null", "path");
			}
			return actionMap.AddBinding(new InputBinding
			{
				path = path,
				interactions = interactions,
				groups = groups,
				action = action
			});
		}

		public static BindingSyntax AddBinding(this InputActionMap actionMap, string path, InputAction action, string interactions = null, string groups = null)
		{
			if (action != null && action.actionMap != actionMap)
			{
				throw new ArgumentException(string.Format("Action '{0}' is not part of action map '{1}'", action, actionMap), "action");
			}
			if (action == null)
			{
				return actionMap.AddBinding(path, interactions, groups);
			}
			return actionMap.AddBinding(path, action.id, interactions, groups);
		}

		public static BindingSyntax AddBinding(this InputActionMap actionMap, string path, Guid action, string interactions = null, string groups = null)
		{
			if (action == Guid.Empty)
			{
				return actionMap.AddBinding(path, interactions, groups);
			}
			return actionMap.AddBinding(path, interactions, groups, action.ToString());
		}

		public static BindingSyntax AddBinding(this InputActionMap actionMap, InputBinding binding)
		{
			if (actionMap == null)
			{
				throw new ArgumentNullException("actionMap");
			}
			if (binding.path == null)
			{
				throw new ArgumentException("Binding path cannot be null", "binding");
			}
			int bindingIndex = AddBindingInternal(actionMap, binding);
			return new BindingSyntax(actionMap, null, bindingIndex);
		}

		public static CompositeSyntax AddCompositeBinding(this InputAction action, string composite, string interactions = null, string processors = null)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			if (string.IsNullOrEmpty(composite))
			{
				throw new ArgumentException("Composite name cannot be null or empty", "composite");
			}
			InputActionMap orCreateActionMap = action.GetOrCreateActionMap();
			InputBinding binding = new InputBinding
			{
				path = composite,
				interactions = interactions,
				processors = processors,
				isComposite = true,
				action = action.name
			};
			int compositeIndex = AddBindingInternal(orCreateActionMap, binding);
			return new CompositeSyntax(orCreateActionMap, action, compositeIndex);
		}

		private static int AddBindingInternal(InputActionMap map, InputBinding binding)
		{
			if (string.IsNullOrEmpty(binding.m_Id))
			{
				binding.GenerateId();
			}
			int result = ArrayHelpers.Append(ref map.m_Bindings, binding);
			map.ClearPerActionCachedBindingData();
			map.LazyResolveBindings();
			if (map.m_SingletonAction != null)
			{
				map.m_SingletonAction.m_SingletonActionBindings = map.m_Bindings;
			}
			return result;
		}

		public static BindingSyntax ChangeBinding(this InputAction action, int index)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			int bindingIndex = action.BindingIndexOnActionToBindingIndexOnMap(index);
			return new BindingSyntax(action.GetOrCreateActionMap(), action, bindingIndex);
		}

		public static BindingSyntax ChangeBindingWithId(this InputAction action, string id)
		{
			return action.ChangeBinding(new InputBinding
			{
				m_Id = id
			});
		}

		public static BindingSyntax ChangeBindingWithId(this InputAction action, Guid id)
		{
			return action.ChangeBinding(new InputBinding
			{
				id = id
			});
		}

		public static BindingSyntax ChangeBindingWithGroup(this InputAction action, string group)
		{
			return action.ChangeBinding(new InputBinding
			{
				groups = group
			});
		}

		public static BindingSyntax ChangeBindingWithPath(this InputAction action, string path)
		{
			return action.ChangeBinding(new InputBinding
			{
				path = path
			});
		}

		public static BindingSyntax ChangeBinding(this InputAction action, InputBinding match)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			InputActionMap orCreateActionMap = action.GetOrCreateActionMap();
			int num = orCreateActionMap.FindBinding(match);
			if (num == -1)
			{
				throw new ArgumentException(string.Format("Cannot find binding matching '{0}' in '{1}'", match, action), "match");
			}
			return new BindingSyntax(orCreateActionMap, action, num);
		}

		public static void Rename(this InputAction action, string newName)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			if (string.IsNullOrEmpty(newName))
			{
				throw new ArgumentNullException("newName");
			}
			if (action.name == newName)
			{
				return;
			}
			InputActionMap actionMap = action.actionMap;
			if (((actionMap != null) ? actionMap.FindAction(newName) : null) != null)
			{
				throw new InvalidOperationException(string.Format("Cannot rename '{0}' to '{1}' in map '{2}' as the map already contains an action with that name", action, newName, actionMap));
			}
			string name = action.m_Name;
			action.m_Name = newName;
			InputBinding[] bindings = action.GetOrCreateActionMap().m_Bindings;
			int num = bindings.LengthSafe();
			for (int i = 0; i < num; i++)
			{
				if (string.Compare(bindings[i].action, name, StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					bindings[i].action = newName;
				}
			}
		}

		public static void AddControlScheme(this InputActionAsset asset, InputControlScheme controlScheme)
		{
			if (asset == null)
			{
				throw new ArgumentNullException("asset");
			}
			if (string.IsNullOrEmpty(controlScheme.name))
			{
				throw new ArgumentException("Cannot add control scheme without name to asset " + asset.name, "controlScheme");
			}
			if (asset.FindControlScheme(controlScheme.name).HasValue)
			{
				throw new InvalidOperationException("Asset '" + asset.name + "' already contains a control scheme called '" + controlScheme.name + "'");
			}
			ArrayHelpers.Append(ref asset.m_ControlSchemes, controlScheme);
		}

		public static ControlSchemeSyntax AddControlScheme(this InputActionAsset asset, string name)
		{
			if (asset == null)
			{
				throw new ArgumentNullException("asset");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			int count = asset.controlSchemes.Count;
			asset.AddControlScheme(new InputControlScheme(name));
			return new ControlSchemeSyntax(asset, count);
		}

		public static void RemoveControlScheme(this InputActionAsset asset, string name)
		{
			if (asset == null)
			{
				throw new ArgumentNullException("asset");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			int num = asset.FindControlSchemeIndex(name);
			if (num != -1)
			{
				ArrayHelpers.EraseAt(ref asset.m_ControlSchemes, num);
			}
		}

		public static InputControlScheme WithBindingGroup(this InputControlScheme scheme, string bindingGroup)
		{
			return new ControlSchemeSyntax(scheme).WithBindingGroup(bindingGroup).Done();
		}

		public static InputControlScheme WithRequiredDevice(this InputControlScheme scheme, string controlPath)
		{
			return new ControlSchemeSyntax(scheme).WithRequiredDevice(controlPath).Done();
		}

		public static InputControlScheme WithOptionalDevice(this InputControlScheme scheme, string controlPath)
		{
			return new ControlSchemeSyntax(scheme).WithOptionalDevice(controlPath).Done();
		}

		public static InputControlScheme OrWithRequiredDevice(this InputControlScheme scheme, string controlPath)
		{
			return new ControlSchemeSyntax(scheme).OrWithRequiredDevice(controlPath).Done();
		}

		public static InputControlScheme OrWithOptionalDevice(this InputControlScheme scheme, string controlPath)
		{
			return new ControlSchemeSyntax(scheme).OrWithOptionalDevice(controlPath).Done();
		}
	}
}
