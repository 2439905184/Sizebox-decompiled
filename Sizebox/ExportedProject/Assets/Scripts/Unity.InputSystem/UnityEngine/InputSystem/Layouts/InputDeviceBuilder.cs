using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.Layouts
{
	internal struct InputDeviceBuilder : IDisposable
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		internal struct RefInstance : IDisposable
		{
			public void Dispose()
			{
				s_InstanceRef--;
				if (s_InstanceRef <= 0)
				{
					s_Instance.Dispose();
					s_Instance = default(InputDeviceBuilder);
					s_InstanceRef = 0;
				}
				else
				{
					s_Instance.Reset();
				}
			}
		}

		private InputDevice m_Device;

		private InputControlLayout.CacheRefInstance m_LayoutCacheRef;

		private Dictionary<string, InputControlLayout.ControlItem> m_ChildControlOverrides;

		private StringBuilder m_StringBuilder;

		private const uint kSizeForControlUsingStateFromOtherControl = uint.MaxValue;

		private static InputDeviceBuilder s_Instance;

		private static int s_InstanceRef;

		internal static ref InputDeviceBuilder instance
		{
			get
			{
				return ref s_Instance;
			}
		}

		public void Setup(InternedString layout, InternedString variants, InputDeviceDescription deviceDescription = default(InputDeviceDescription))
		{
			m_LayoutCacheRef = InputControlLayout.CacheRef();
			InstantiateLayout(layout, variants, default(InternedString), null);
			FinalizeControlHierarchy();
			m_Device.m_Description = deviceDescription;
			m_Device.CallFinishSetupRecursive();
		}

		public InputDevice Finish()
		{
			InputDevice device = m_Device;
			Reset();
			return device;
		}

		public void Dispose()
		{
			m_LayoutCacheRef.Dispose();
		}

		private void Reset()
		{
			m_Device = null;
			Dictionary<string, InputControlLayout.ControlItem> childControlOverrides = m_ChildControlOverrides;
			if (childControlOverrides != null)
			{
				childControlOverrides.Clear();
			}
		}

		private InputControl InstantiateLayout(InternedString layout, InternedString variants, InternedString name, InputControl parent)
		{
			InputControlLayout layout2 = FindOrLoadLayout(layout);
			return InstantiateLayout(layout2, variants, name, parent);
		}

		private InputControl InstantiateLayout(InputControlLayout layout, InternedString variants, InternedString name, InputControl parent)
		{
			InputControl inputControl;
			if ((inputControl = Activator.CreateInstance(layout.type) as InputControl) == null)
			{
				throw new InvalidOperationException(string.Format("Type '{0}' referenced by layout '{1}' is not an InputControl", layout.type.Name, layout.name));
			}
			InputDevice device;
			if ((device = inputControl as InputDevice) != null)
			{
				if (parent != null)
				{
					throw new InvalidOperationException(string.Format("Cannot instantiate device layout '{0}' as child of '{1}'; devices must be added at root", layout.name, parent.path));
				}
				m_Device = device;
				m_Device.m_StateBlock.byteOffset = 0u;
				m_Device.m_StateBlock.bitOffset = 0u;
				m_Device.m_StateBlock.format = layout.stateFormat;
				m_Device.m_AliasesForEachControl = null;
				m_Device.m_ChildrenForEachControl = null;
				m_Device.m_UsagesForEachControl = null;
				m_Device.m_UsageToControl = null;
				if (layout.m_UpdateBeforeRender == true)
				{
					m_Device.m_DeviceFlags |= InputDevice.DeviceFlags.UpdateBeforeRender;
				}
			}
			else if (parent == null)
			{
				throw new InvalidOperationException(string.Format("Toplevel layout used with InputDeviceBuilder must be a device layout; '{0}' is a control layout", layout.name));
			}
			if (name.IsEmpty())
			{
				name = layout.name;
				int num = name.ToString().LastIndexOf(':');
				if (num != -1)
				{
					name = new InternedString(name.ToString().Substring(num + 1));
				}
			}
			if (variants.IsEmpty())
			{
				variants = layout.variants;
				if (variants.IsEmpty())
				{
					variants = InputControlLayout.DefaultVariant;
				}
			}
			inputControl.m_Name = name;
			inputControl.m_DisplayNameFromLayout = layout.m_DisplayName;
			inputControl.m_Layout = layout.name;
			inputControl.m_Variants = variants;
			inputControl.m_Parent = parent;
			inputControl.m_Device = m_Device;
			bool haveChildrenUsingStateFromOtherControls = false;
			try
			{
				AddChildControls(layout, variants, inputControl, ref haveChildrenUsingStateFromOtherControls);
			}
			catch
			{
				throw;
			}
			ComputeStateLayout(inputControl);
			if (haveChildrenUsingStateFromOtherControls)
			{
				InputControlLayout.ControlItem[] controls = layout.m_Controls;
				for (int i = 0; i < controls.Length; i++)
				{
					ref InputControlLayout.ControlItem reference = ref controls[i];
					if (!string.IsNullOrEmpty(reference.useStateFrom))
					{
						ApplyUseStateFrom(inputControl, ref reference, layout);
					}
				}
			}
			return inputControl;
		}

		private void AddChildControls(InputControlLayout layout, InternedString variants, InputControl parent, ref bool haveChildrenUsingStateFromOtherControls)
		{
			InputControlLayout.ControlItem[] controls = layout.m_Controls;
			if (controls == null)
			{
				return;
			}
			int num = 0;
			bool flag = false;
			for (int i = 0; i < controls.Length; i++)
			{
				if (!controls[i].variants.IsEmpty() && !StringHelpers.CharacterSeparatedListsHaveAtLeastOneCommonElement(controls[i].variants, variants, ','))
				{
					continue;
				}
				if (controls[i].isModifyingExistingControl)
				{
					if (controls[i].isArray)
					{
						throw new NotSupportedException(string.Format("Control '{0}' in layout '{1}' is modifying the child of another control but is marked as an array", controls[i].name, layout.name));
					}
					flag = true;
					InsertChildControlOverride(parent, ref controls[i]);
				}
				else
				{
					num = ((!controls[i].isArray) ? (num + 1) : (num + controls[i].arraySize));
				}
			}
			if (num == 0)
			{
				parent.m_ChildCount = 0;
				parent.m_ChildStartIndex = 0;
				haveChildrenUsingStateFromOtherControls = false;
				return;
			}
			int num2 = ArrayHelpers.GrowBy(ref m_Device.m_ChildrenForEachControl, num);
			int num3 = num2;
			for (int j = 0; j < controls.Length; j++)
			{
				InputControlLayout.ControlItem controlItem = controls[j];
				if (controlItem.isModifyingExistingControl || (!controlItem.variants.IsEmpty() && !StringHelpers.CharacterSeparatedListsHaveAtLeastOneCommonElement(controlItem.variants, variants, ',')))
				{
					continue;
				}
				if (controlItem.isArray)
				{
					for (int k = 0; k < controlItem.arraySize; k++)
					{
						string nameOverride = (string)controlItem.name + k;
						InputControl inputControl = AddChildControl(layout, variants, parent, ref haveChildrenUsingStateFromOtherControls, controlItem, num3, nameOverride);
						num3++;
						if (inputControl.m_StateBlock.byteOffset != uint.MaxValue)
						{
							inputControl.m_StateBlock.byteOffset += (uint)(k * (int)inputControl.m_StateBlock.alignedSizeInBytes);
						}
					}
				}
				else
				{
					AddChildControl(layout, variants, parent, ref haveChildrenUsingStateFromOtherControls, controlItem, num3);
					num3++;
				}
			}
			parent.m_ChildCount = num;
			parent.m_ChildStartIndex = num2;
			if (!flag)
			{
				return;
			}
			for (int l = 0; l < controls.Length; l++)
			{
				InputControlLayout.ControlItem controlItem2 = controls[l];
				if (controlItem2.isModifyingExistingControl && (controlItem2.variants.IsEmpty() || !(controlItem2.variants != variants)))
				{
					AddChildControlIfMissing(layout, variants, parent, ref haveChildrenUsingStateFromOtherControls, ref controlItem2);
				}
			}
		}

		private InputControl AddChildControl(InputControlLayout layout, InternedString variants, InputControl parent, ref bool haveChildrenUsingStateFromOtherControls, InputControlLayout.ControlItem controlItem, int childIndex, string nameOverride = null)
		{
			string text = nameOverride ?? ((string)controlItem.name);
			InternedString name = new InternedString(text);
			if (string.IsNullOrEmpty(controlItem.layout))
			{
				throw new InvalidOperationException(string.Format("Layout has not been set on control '{0}' in '{1}'", controlItem.name, layout.name));
			}
			if (m_ChildControlOverrides != null)
			{
				string key = (parent.path + "/" + text).ToLower();
				InputControlLayout.ControlItem value;
				if (m_ChildControlOverrides.TryGetValue(key, out value))
				{
					controlItem = value.Merge(controlItem);
				}
			}
			InternedString layout2 = controlItem.layout;
			InputControl inputControl;
			try
			{
				inputControl = InstantiateLayout(layout2, variants, name, parent);
			}
			catch (InputControlLayout.LayoutNotFoundException ex)
			{
				throw new InputControlLayout.LayoutNotFoundException(string.Format("Cannot find layout '{0}' used in control '{1}' of layout '{2}'", ex.layout, text, layout.name), ex);
			}
			m_Device.m_ChildrenForEachControl[childIndex] = inputControl;
			inputControl.noisy = controlItem.isNoisy;
			inputControl.synthetic = controlItem.isSynthetic;
			if (inputControl.noisy)
			{
				m_Device.noisy = true;
			}
			inputControl.m_DisplayNameFromLayout = controlItem.displayName;
			inputControl.m_ShortDisplayNameFromLayout = controlItem.shortDisplayName;
			inputControl.m_DefaultState = controlItem.defaultState;
			if (!inputControl.m_DefaultState.isEmpty)
			{
				m_Device.hasControlsWithDefaultState = true;
			}
			if (!controlItem.minValue.isEmpty)
			{
				inputControl.m_MinValue = controlItem.minValue;
			}
			if (!controlItem.maxValue.isEmpty)
			{
				inputControl.m_MaxValue = controlItem.maxValue;
			}
			if (string.IsNullOrEmpty(controlItem.useStateFrom))
			{
				inputControl.m_StateBlock.byteOffset = controlItem.offset;
				inputControl.m_StateBlock.bitOffset = controlItem.bit;
				if (controlItem.sizeInBits != 0)
				{
					inputControl.m_StateBlock.sizeInBits = controlItem.sizeInBits;
				}
				if (controlItem.format != 0)
				{
					SetFormat(inputControl, controlItem);
				}
			}
			else
			{
				inputControl.m_StateBlock.sizeInBits = uint.MaxValue;
				haveChildrenUsingStateFromOtherControls = true;
			}
			ReadOnlyArray<InternedString> usages = controlItem.usages;
			if (usages.Count > 0)
			{
				int count = usages.Count;
				int num = (inputControl.m_UsageStartIndex = ArrayHelpers.AppendToImmutable(ref m_Device.m_UsagesForEachControl, usages.m_Array));
				inputControl.m_UsageCount = count;
				ArrayHelpers.GrowBy(ref m_Device.m_UsageToControl, count);
				for (int i = 0; i < count; i++)
				{
					m_Device.m_UsageToControl[num + i] = inputControl;
				}
			}
			if (controlItem.aliases.Count > 0)
			{
				int count2 = controlItem.aliases.Count;
				int aliasStartIndex = ArrayHelpers.AppendToImmutable(ref m_Device.m_AliasesForEachControl, controlItem.aliases.m_Array);
				inputControl.m_AliasStartIndex = aliasStartIndex;
				inputControl.m_AliasCount = count2;
			}
			if (controlItem.parameters.Count > 0)
			{
				NamedValue.ApplyAllToObject(inputControl, controlItem.parameters);
			}
			if (controlItem.processors.Count > 0)
			{
				AddProcessors(inputControl, ref controlItem, layout.name);
			}
			return inputControl;
		}

		private void InsertChildControlOverride(InputControl parent, ref InputControlLayout.ControlItem controlItem)
		{
			if (m_ChildControlOverrides == null)
			{
				m_ChildControlOverrides = new Dictionary<string, InputControlLayout.ControlItem>();
			}
			string key = InputControlPath.Combine(parent, controlItem.name).ToLower();
			InputControlLayout.ControlItem value;
			if (!m_ChildControlOverrides.TryGetValue(key, out value))
			{
				m_ChildControlOverrides[key] = controlItem;
				return;
			}
			value = value.Merge(controlItem);
			m_ChildControlOverrides[key] = value;
		}

		private void AddChildControlIfMissing(InputControlLayout layout, InternedString variants, InputControl parent, ref bool haveChildrenUsingStateFromOtherControls, ref InputControlLayout.ControlItem controlItem)
		{
			InputControl inputControl = InputControlPath.TryFindChild(parent, controlItem.name);
			if (inputControl == null)
			{
				inputControl = InsertChildControl(layout, variants, parent, ref haveChildrenUsingStateFromOtherControls, ref controlItem);
				if (inputControl.parent != parent)
				{
					ComputeStateLayout(inputControl.parent);
				}
			}
		}

		private InputControl InsertChildControl(InputControlLayout layout, InternedString variant, InputControl parent, ref bool haveChildrenUsingStateFromOtherControls, ref InputControlLayout.ControlItem controlItem)
		{
			string text = controlItem.name.ToString();
			int num = text.LastIndexOf('/');
			if (num == -1)
			{
				throw new InvalidOperationException("InsertChildControl has to be called with a slash-separated path");
			}
			string text2 = text.Substring(0, num);
			InputControl inputControl = InputControlPath.TryFindChild(parent, text2);
			if (inputControl == null)
			{
				throw new InvalidOperationException(string.Format("Cannot find parent '{0}' of control '{1}' in layout '{2}'", text2, controlItem.name, layout.name));
			}
			string text3 = text.Substring(num + 1);
			if (text3.Length == 0)
			{
				throw new InvalidOperationException(string.Format("Path cannot end in '/' (control '{0}' in layout '{1}')", controlItem.name, layout.name));
			}
			int num2 = inputControl.m_ChildStartIndex;
			if (num2 == 0)
			{
				num2 = (inputControl.m_ChildStartIndex = m_Device.m_ChildrenForEachControl.LengthSafe());
			}
			int num3 = num2 + inputControl.m_ChildCount;
			ShiftChildIndicesInHierarchyOneUp(m_Device, num3, inputControl);
			ArrayHelpers.InsertAt(ref m_Device.m_ChildrenForEachControl, num3, null);
			inputControl.m_ChildCount++;
			return AddChildControl(layout, variant, inputControl, ref haveChildrenUsingStateFromOtherControls, controlItem, num3, text3);
		}

		private static void ApplyUseStateFrom(InputControl parent, ref InputControlLayout.ControlItem controlItem, InputControlLayout layout)
		{
			InputControl inputControl = InputControlPath.TryFindChild(parent, controlItem.name);
			InputControl inputControl2 = InputControlPath.TryFindChild(parent, controlItem.useStateFrom);
			if (inputControl2 == null)
			{
				throw new InvalidOperationException(string.Format("Cannot find control '{0}' referenced in 'useStateFrom' of control '{1}' in layout '{2}'", controlItem.useStateFrom, controlItem.name, layout.name));
			}
			inputControl.m_StateBlock = inputControl2.m_StateBlock;
			if (inputControl.parent != inputControl2.parent)
			{
				for (InputControl parent2 = inputControl2.parent; parent2 != parent; parent2 = parent2.parent)
				{
					inputControl.m_StateBlock.byteOffset += parent2.m_StateBlock.byteOffset;
				}
			}
		}

		private static void ShiftChildIndicesInHierarchyOneUp(InputDevice device, int startIndex, InputControl exceptControl)
		{
			InputControl[] childrenForEachControl = device.m_ChildrenForEachControl;
			int num = childrenForEachControl.Length;
			for (int i = 0; i < num; i++)
			{
				InputControl inputControl = childrenForEachControl[i];
				if (inputControl != null && inputControl != exceptControl && inputControl.m_ChildStartIndex >= startIndex)
				{
					inputControl.m_ChildStartIndex++;
				}
			}
		}

		private void SetDisplayName(InputControl control, string longDisplayNameFromLayout, string shortDisplayNameFromLayout, bool shortName)
		{
			string text = (shortName ? shortDisplayNameFromLayout : longDisplayNameFromLayout);
			if (string.IsNullOrEmpty(text))
			{
				if (shortName)
				{
					if (control.parent != null && control.parent != control.device)
					{
						if (m_StringBuilder == null)
						{
							m_StringBuilder = new StringBuilder();
						}
						m_StringBuilder.Length = 0;
						AddParentDisplayNameRecursive(control.parent, m_StringBuilder, true);
						if (m_StringBuilder.Length == 0)
						{
							control.m_ShortDisplayNameFromLayout = null;
							return;
						}
						if (!string.IsNullOrEmpty(longDisplayNameFromLayout))
						{
							m_StringBuilder.Append(longDisplayNameFromLayout);
						}
						else
						{
							m_StringBuilder.Append(control.name);
						}
						control.m_ShortDisplayNameFromLayout = m_StringBuilder.ToString();
					}
					else
					{
						control.m_ShortDisplayNameFromLayout = null;
					}
					return;
				}
				text = control.name;
			}
			if (control.parent != null && control.parent != control.device)
			{
				if (m_StringBuilder == null)
				{
					m_StringBuilder = new StringBuilder();
				}
				m_StringBuilder.Length = 0;
				AddParentDisplayNameRecursive(control.parent, m_StringBuilder, shortName);
				m_StringBuilder.Append(text);
				text = m_StringBuilder.ToString();
			}
			if (shortName)
			{
				control.m_ShortDisplayNameFromLayout = text;
			}
			else
			{
				control.m_DisplayNameFromLayout = text;
			}
		}

		private static void AddParentDisplayNameRecursive(InputControl control, StringBuilder stringBuilder, bool shortName)
		{
			if (control.parent != null && control.parent != control.device)
			{
				AddParentDisplayNameRecursive(control.parent, stringBuilder, shortName);
			}
			if (shortName)
			{
				string value = control.shortDisplayName;
				if (string.IsNullOrEmpty(value))
				{
					value = control.displayName;
				}
				stringBuilder.Append(value);
			}
			else
			{
				stringBuilder.Append(control.displayName);
			}
			stringBuilder.Append(' ');
		}

		private static void AddProcessors(InputControl control, ref InputControlLayout.ControlItem controlItem, string layoutName)
		{
			int count = controlItem.processors.Count;
			for (int i = 0; i < count; i++)
			{
				string name = controlItem.processors[i].name;
				Type type = InputProcessor.s_Processors.LookupTypeRegistration(name);
				if (type == null)
				{
					throw new InvalidOperationException(string.Format("Cannot find processor '{0}' referenced by control '{1}' in layout '{2}'", name, controlItem.name, layoutName));
				}
				object first = Activator.CreateInstance(type);
				ReadOnlyArray<NamedValue> parameters = controlItem.processors[i].parameters;
				if (parameters.Count > 0)
				{
					NamedValue.ApplyAllToObject(first, parameters);
				}
				control.AddProcessor(first);
			}
		}

		private static void SetFormat(InputControl control, InputControlLayout.ControlItem controlItem)
		{
			control.m_StateBlock.format = controlItem.format;
			if (controlItem.sizeInBits == 0)
			{
				int sizeOfPrimitiveFormatInBits = InputStateBlock.GetSizeOfPrimitiveFormatInBits(controlItem.format);
				if (sizeOfPrimitiveFormatInBits != -1)
				{
					control.m_StateBlock.sizeInBits = (uint)sizeOfPrimitiveFormatInBits;
				}
			}
		}

		private InputControlLayout FindOrLoadLayout(string name)
		{
			return InputControlLayout.cache.FindOrLoadLayout(name);
		}

		private static void ComputeStateLayout(InputControl control)
		{
			ReadOnlyArray<InputControl> children = control.children;
			if (control.m_StateBlock.sizeInBits == 0 && control.m_StateBlock.format != 0)
			{
				int sizeOfPrimitiveFormatInBits = InputStateBlock.GetSizeOfPrimitiveFormatInBits(control.m_StateBlock.format);
				if (sizeOfPrimitiveFormatInBits != -1)
				{
					control.m_StateBlock.sizeInBits = (uint)sizeOfPrimitiveFormatInBits;
				}
			}
			if (control.m_StateBlock.sizeInBits == 0 && children.Count == 0)
			{
				throw new InvalidOperationException("Control '" + control.path + "' with layout '" + control.layout + "' has no size set and has no children to compute size from");
			}
			if (children.Count == 0)
			{
				return;
			}
			uint num = 0u;
			foreach (InputControl item in children)
			{
				if (item.m_StateBlock.sizeInBits == uint.MaxValue)
				{
					continue;
				}
				uint sizeInBits = item.m_StateBlock.sizeInBits;
				if (sizeInBits == 0 || sizeInBits == uint.MaxValue)
				{
					throw new InvalidOperationException("Child '" + item.name + "' of '" + control.name + "' has no size set!");
				}
				if (item.m_StateBlock.byteOffset != uint.MaxValue && item.m_StateBlock.byteOffset != 4294967294u)
				{
					if (item.m_StateBlock.bitOffset == uint.MaxValue)
					{
						item.m_StateBlock.bitOffset = 0u;
					}
					uint num2 = MemoryHelpers.ComputeFollowingByteOffset(item.m_StateBlock.byteOffset, item.m_StateBlock.bitOffset + sizeInBits);
					if (num2 > num)
					{
						num = num2;
					}
				}
			}
			uint num3 = num;
			InputControl inputControl = null;
			uint num4 = 0u;
			foreach (InputControl item2 in children)
			{
				if ((item2.m_StateBlock.byteOffset != uint.MaxValue && item2.m_StateBlock.byteOffset != 4294967294u) || item2.m_StateBlock.sizeInBits == uint.MaxValue)
				{
					continue;
				}
				bool num5 = item2.m_StateBlock.sizeInBits % 8u != 0;
				if (num5)
				{
					if (inputControl == null)
					{
						inputControl = item2;
					}
					if (item2.m_StateBlock.bitOffset == uint.MaxValue || item2.m_StateBlock.bitOffset == 4294967294u)
					{
						item2.m_StateBlock.bitOffset = num4;
						num4 += item2.m_StateBlock.sizeInBits;
					}
					else
					{
						uint num6 = item2.m_StateBlock.bitOffset + item2.m_StateBlock.sizeInBits;
						if (num6 > num4)
						{
							num4 = num6;
						}
					}
				}
				else
				{
					if (inputControl != null)
					{
						num3 = MemoryHelpers.ComputeFollowingByteOffset(num3, num4);
						inputControl = null;
					}
					if (item2.m_StateBlock.bitOffset == uint.MaxValue)
					{
						item2.m_StateBlock.bitOffset = 0u;
					}
				}
				item2.m_StateBlock.byteOffset = num3;
				if (!num5)
				{
					num3 = MemoryHelpers.ComputeFollowingByteOffset(num3, item2.m_StateBlock.sizeInBits);
				}
			}
			if (inputControl != null)
			{
				num3 = MemoryHelpers.ComputeFollowingByteOffset(num3, num4);
			}
			uint num7 = num3;
			control.m_StateBlock.sizeInBits = num7 * 8;
		}

		private void FinalizeControlHierarchy()
		{
			FinalizeControlHierarchyRecursive(m_Device);
		}

		private void FinalizeControlHierarchyRecursive(InputControl control)
		{
			string displayNameFromLayout = control.m_DisplayNameFromLayout;
			string shortDisplayNameFromLayout = control.m_ShortDisplayNameFromLayout;
			SetDisplayName(control, displayNameFromLayout, shortDisplayNameFromLayout, false);
			SetDisplayName(control, displayNameFromLayout, shortDisplayNameFromLayout, true);
			uint byteOffset = control.m_StateBlock.byteOffset;
			foreach (InputControl child in control.children)
			{
				child.m_StateBlock.byteOffset += byteOffset;
				FinalizeControlHierarchyRecursive(child);
			}
		}

		internal static RefInstance Ref()
		{
			s_InstanceRef++;
			return default(RefInstance);
		}
	}
}
