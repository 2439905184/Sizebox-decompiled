using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.Layouts
{
	public class InputControlLayout
	{
		public struct ControlItem
		{
			[Flags]
			private enum Flags
			{
				isModifyingExistingControl = 1,
				IsNoisy = 2,
				IsSynthetic = 4,
				IsFirstDefinedInThisLayout = 8
			}

			public InternedString name { get; internal set; }

			public InternedString layout { get; internal set; }

			public InternedString variants { get; internal set; }

			public string useStateFrom { get; internal set; }

			public string displayName { get; internal set; }

			public string shortDisplayName { get; internal set; }

			public ReadOnlyArray<InternedString> usages { get; internal set; }

			public ReadOnlyArray<InternedString> aliases { get; internal set; }

			public ReadOnlyArray<NamedValue> parameters { get; internal set; }

			public ReadOnlyArray<NameAndParameters> processors { get; internal set; }

			public uint offset { get; internal set; }

			public uint bit { get; internal set; }

			public uint sizeInBits { get; internal set; }

			public FourCC format { get; internal set; }

			private Flags flags { get; set; }

			public int arraySize { get; internal set; }

			public PrimitiveValue defaultState { get; internal set; }

			public PrimitiveValue minValue { get; internal set; }

			public PrimitiveValue maxValue { get; internal set; }

			public bool isModifyingExistingControl
			{
				get
				{
					return (flags & Flags.isModifyingExistingControl) == Flags.isModifyingExistingControl;
				}
				internal set
				{
					if (value)
					{
						flags |= Flags.isModifyingExistingControl;
					}
					else
					{
						flags &= ~Flags.isModifyingExistingControl;
					}
				}
			}

			public bool isNoisy
			{
				get
				{
					return (flags & Flags.IsNoisy) == Flags.IsNoisy;
				}
				internal set
				{
					if (value)
					{
						flags |= Flags.IsNoisy;
					}
					else
					{
						flags &= ~Flags.IsNoisy;
					}
				}
			}

			public bool isSynthetic
			{
				get
				{
					return (flags & Flags.IsSynthetic) == Flags.IsSynthetic;
				}
				internal set
				{
					if (value)
					{
						flags |= Flags.IsSynthetic;
					}
					else
					{
						flags &= ~Flags.IsSynthetic;
					}
				}
			}

			public bool isFirstDefinedInThisLayout
			{
				get
				{
					return (flags & Flags.IsFirstDefinedInThisLayout) != 0;
				}
				internal set
				{
					if (value)
					{
						flags |= Flags.IsFirstDefinedInThisLayout;
					}
					else
					{
						flags &= ~Flags.IsFirstDefinedInThisLayout;
					}
				}
			}

			public bool isArray
			{
				get
				{
					return arraySize != 0;
				}
			}

			public ControlItem Merge(ControlItem other)
			{
				ControlItem result = default(ControlItem);
				result.name = name;
				result.isModifyingExistingControl = isModifyingExistingControl;
				result.displayName = (string.IsNullOrEmpty(displayName) ? other.displayName : displayName);
				result.shortDisplayName = (string.IsNullOrEmpty(shortDisplayName) ? other.shortDisplayName : shortDisplayName);
				result.layout = (layout.IsEmpty() ? other.layout : layout);
				result.variants = (variants.IsEmpty() ? other.variants : variants);
				result.useStateFrom = useStateFrom ?? other.useStateFrom;
				result.arraySize = ((!isArray) ? other.arraySize : arraySize);
				result.isNoisy = isNoisy || other.isNoisy;
				result.isSynthetic = isSynthetic || other.isSynthetic;
				result.isFirstDefinedInThisLayout = false;
				if (offset != uint.MaxValue)
				{
					result.offset = offset;
				}
				else
				{
					result.offset = other.offset;
				}
				if (bit != uint.MaxValue)
				{
					result.bit = bit;
				}
				else
				{
					result.bit = other.bit;
				}
				if (format != 0)
				{
					result.format = format;
				}
				else
				{
					result.format = other.format;
				}
				if (sizeInBits != 0)
				{
					result.sizeInBits = sizeInBits;
				}
				else
				{
					result.sizeInBits = other.sizeInBits;
				}
				if (aliases.Count > 0)
				{
					result.aliases = aliases;
				}
				else
				{
					result.aliases = other.aliases;
				}
				if (usages.Count > 0)
				{
					result.usages = usages;
				}
				else
				{
					result.usages = other.usages;
				}
				if (parameters.Count == 0)
				{
					result.parameters = other.parameters;
				}
				else
				{
					result.parameters = parameters;
				}
				if (processors.Count == 0)
				{
					result.processors = other.processors;
				}
				else
				{
					result.processors = processors;
				}
				if (!string.IsNullOrEmpty(displayName))
				{
					result.displayName = displayName;
				}
				else
				{
					result.displayName = other.displayName;
				}
				if (!defaultState.isEmpty)
				{
					result.defaultState = defaultState;
				}
				else
				{
					result.defaultState = other.defaultState;
				}
				if (!minValue.isEmpty)
				{
					result.minValue = minValue;
				}
				else
				{
					result.minValue = other.minValue;
				}
				if (!maxValue.isEmpty)
				{
					result.maxValue = maxValue;
				}
				else
				{
					result.maxValue = other.maxValue;
				}
				return result;
			}
		}

		public class Builder
		{
			public struct ControlBuilder
			{
				[Serializable]
				[CompilerGenerated]
				private sealed class _003C_003Ec
				{
					public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

					public static Func<string, InternedString> _003C_003E9__12_0;

					internal InternedString _003CWithUsages_003Eb__12_0(string x)
					{
						return new InternedString(x);
					}
				}

				internal Builder builder;

				internal int index;

				public ControlBuilder WithDisplayName(string displayName)
				{
					builder.m_Controls[index].displayName = displayName;
					return this;
				}

				public ControlBuilder WithLayout(string layout)
				{
					if (string.IsNullOrEmpty(layout))
					{
						throw new ArgumentException("Layout name cannot be null or empty", "layout");
					}
					builder.m_Controls[index].layout = new InternedString(layout);
					return this;
				}

				public ControlBuilder WithFormat(FourCC format)
				{
					builder.m_Controls[index].format = format;
					return this;
				}

				public ControlBuilder WithFormat(string format)
				{
					return WithFormat(new FourCC(format));
				}

				public ControlBuilder WithByteOffset(uint offset)
				{
					builder.m_Controls[index].offset = offset;
					return this;
				}

				public ControlBuilder WithBitOffset(uint bit)
				{
					builder.m_Controls[index].bit = bit;
					return this;
				}

				public ControlBuilder IsSynthetic(bool value)
				{
					builder.m_Controls[index].isSynthetic = value;
					return this;
				}

				public ControlBuilder IsNoisy(bool value)
				{
					builder.m_Controls[index].isNoisy = value;
					return this;
				}

				public ControlBuilder WithSizeInBits(uint sizeInBits)
				{
					builder.m_Controls[index].sizeInBits = sizeInBits;
					return this;
				}

				public ControlBuilder WithUsages(params InternedString[] usages)
				{
					if (usages == null || usages.Length == 0)
					{
						return this;
					}
					for (int i = 0; i < usages.Length; i++)
					{
						if (usages[i].IsEmpty())
						{
							throw new ArgumentException(string.Format("Empty usage entry at index {0} for control '{1}' in layout '{2}'", i, builder.m_Controls[index].name, builder.name), "usages");
						}
					}
					builder.m_Controls[index].usages = new ReadOnlyArray<InternedString>(usages);
					return this;
				}

				public ControlBuilder WithUsages(IEnumerable<string> usages)
				{
					InternedString[] usages2 = usages.Select(_003C_003Ec._003C_003E9__12_0 ?? (_003C_003Ec._003C_003E9__12_0 = _003C_003Ec._003C_003E9._003CWithUsages_003Eb__12_0)).ToArray();
					return WithUsages(usages2);
				}

				public ControlBuilder WithUsages(params string[] usages)
				{
					return WithUsages((IEnumerable<string>)usages);
				}

				public ControlBuilder WithParameters(string parameters)
				{
					if (string.IsNullOrEmpty(parameters))
					{
						return this;
					}
					NamedValue[] array = NamedValue.ParseMultiple(parameters);
					builder.m_Controls[index].parameters = new ReadOnlyArray<NamedValue>(array);
					return this;
				}

				public ControlBuilder WithProcessors(string processors)
				{
					if (string.IsNullOrEmpty(processors))
					{
						return this;
					}
					NameAndParameters[] array = NameAndParameters.ParseMultiple(processors).ToArray();
					builder.m_Controls[index].processors = new ReadOnlyArray<NameAndParameters>(array);
					return this;
				}

				public ControlBuilder WithDefaultState(PrimitiveValue value)
				{
					builder.m_Controls[index].defaultState = value;
					return this;
				}

				public ControlBuilder UsingStateFrom(string path)
				{
					if (string.IsNullOrEmpty(path))
					{
						return this;
					}
					builder.m_Controls[index].useStateFrom = path;
					return this;
				}

				public ControlBuilder AsArrayOfControlsWithSize(int arraySize)
				{
					builder.m_Controls[index].arraySize = arraySize;
					return this;
				}
			}

			private int m_ControlCount;

			private ControlItem[] m_Controls;

			public string name { get; set; }

			public string displayName { get; set; }

			public Type type { get; set; }

			public FourCC stateFormat { get; set; }

			public int stateSizeInBytes { get; set; }

			public string extendsLayout { get; set; }

			public bool? updateBeforeRender { get; set; }

			public ReadOnlyArray<ControlItem> controls
			{
				get
				{
					return new ReadOnlyArray<ControlItem>(m_Controls, 0, m_ControlCount);
				}
			}

			public ControlBuilder AddControl(string name)
			{
				if (string.IsNullOrEmpty(name))
				{
					throw new ArgumentException(name);
				}
				int index = ArrayHelpers.AppendWithCapacity(ref m_Controls, ref m_ControlCount, new ControlItem
				{
					name = new InternedString(name),
					isModifyingExistingControl = (name.IndexOf('/') != -1),
					offset = uint.MaxValue,
					bit = uint.MaxValue
				});
				ControlBuilder result = default(ControlBuilder);
				result.builder = this;
				result.index = index;
				return result;
			}

			public Builder WithName(string name)
			{
				this.name = name;
				return this;
			}

			public Builder WithDisplayName(string displayName)
			{
				this.displayName = displayName;
				return this;
			}

			public Builder WithType<T>() where T : InputControl
			{
				type = typeof(T);
				return this;
			}

			public Builder WithFormat(FourCC format)
			{
				stateFormat = format;
				return this;
			}

			public Builder WithFormat(string format)
			{
				return WithFormat(new FourCC(format));
			}

			public Builder WithSizeInBytes(int sizeInBytes)
			{
				stateSizeInBytes = sizeInBytes;
				return this;
			}

			public Builder Extend(string baseLayoutName)
			{
				extendsLayout = baseLayoutName;
				return this;
			}

			public InputControlLayout Build()
			{
				ControlItem[] destinationArray = null;
				if (m_ControlCount > 0)
				{
					destinationArray = new ControlItem[m_ControlCount];
					Array.Copy(m_Controls, destinationArray, m_ControlCount);
				}
				return new InputControlLayout(new InternedString(name), (type == null && string.IsNullOrEmpty(extendsLayout)) ? typeof(InputDevice) : type)
				{
					m_DisplayName = displayName,
					m_StateFormat = stateFormat,
					m_StateSizeInBytes = stateSizeInBytes,
					m_BaseLayouts = new InlinedArray<InternedString>(new InternedString(extendsLayout)),
					m_Controls = destinationArray,
					m_UpdateBeforeRender = updateBeforeRender
				};
			}
		}

		[Flags]
		private enum Flags
		{
			IsGenericTypeOfDevice = 1,
			HideInUI = 2,
			IsOverride = 4
		}

		[Serializable]
		internal struct LayoutJsonNameAndDescriptorOnly
		{
			public string name;

			public string extend;

			public string[] extendMultiple;

			public InputDeviceMatcher.MatcherJson device;
		}

		[Serializable]
		private struct LayoutJson
		{
			[Serializable]
			[CompilerGenerated]
			private sealed class _003C_003Ec
			{
				public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

				public static Func<string, InternedString> _003C_003E9__13_0;

				public static Func<InternedString, string> _003C_003E9__14_0;

				internal InternedString _003CToLayout_003Eb__13_0(string x)
				{
					return new InternedString(x);
				}

				internal string _003CFromLayout_003Eb__14_0(InternedString x)
				{
					return x.ToString();
				}
			}

			public string name;

			public string extend;

			public string[] extendMultiple;

			public string format;

			public string beforeRender;

			public string[] commonUsages;

			public string displayName;

			public string description;

			public string type;

			public string variant;

			public bool isGenericTypeOfDevice;

			public bool hideInUI;

			public ControlItemJson[] controls;

			public InputControlLayout ToLayout()
			{
				Type type = null;
				if (!string.IsNullOrEmpty(this.type))
				{
					type = Type.GetType(this.type, false);
					if (type == null)
					{
						Debug.Log("Cannot find type '" + this.type + "' used by layout '" + name + "'; falling back to using InputDevice");
						type = typeof(InputDevice);
					}
					else if (!typeof(InputControl).IsAssignableFrom(type))
					{
						throw new InvalidOperationException("'" + this.type + "' used by layout '" + name + "' is not an InputControl");
					}
				}
				else if (string.IsNullOrEmpty(extend))
				{
					type = typeof(InputDevice);
				}
				InputControlLayout inputControlLayout = new InputControlLayout(name, type)
				{
					m_DisplayName = displayName,
					m_Description = description,
					isGenericTypeOfDevice = isGenericTypeOfDevice,
					hideInUI = hideInUI,
					m_Variants = new InternedString(variant)
				};
				if (!string.IsNullOrEmpty(format))
				{
					inputControlLayout.m_StateFormat = new FourCC(format);
				}
				if (!string.IsNullOrEmpty(extend))
				{
					inputControlLayout.m_BaseLayouts.Append(new InternedString(extend));
				}
				if (extendMultiple != null)
				{
					string[] array = extendMultiple;
					foreach (string text in array)
					{
						inputControlLayout.m_BaseLayouts.Append(new InternedString(text));
					}
				}
				if (!string.IsNullOrEmpty(beforeRender))
				{
					string text2 = beforeRender.ToLower();
					if (text2 == "ignore")
					{
						inputControlLayout.m_UpdateBeforeRender = false;
					}
					else
					{
						if (!(text2 == "update"))
						{
							throw new InvalidOperationException("Invalid beforeRender setting '" + beforeRender + "'");
						}
						inputControlLayout.m_UpdateBeforeRender = true;
					}
				}
				if (commonUsages != null)
				{
					inputControlLayout.m_CommonUsages = ArrayHelpers.Select(commonUsages, _003C_003Ec._003C_003E9__13_0 ?? (_003C_003Ec._003C_003E9__13_0 = _003C_003Ec._003C_003E9._003CToLayout_003Eb__13_0));
				}
				if (controls != null)
				{
					List<ControlItem> list = new List<ControlItem>();
					ControlItemJson[] array2 = controls;
					foreach (ControlItemJson obj in array2)
					{
						if (string.IsNullOrEmpty(obj.name))
						{
							throw new InvalidOperationException("Control with no name in layout '" + name);
						}
						ControlItem controlItem = obj.ToLayout();
						ThrowIfControlItemIsDuplicate(ref controlItem, list, inputControlLayout.name);
						list.Add(controlItem);
					}
					inputControlLayout.m_Controls = list.ToArray();
				}
				return inputControlLayout;
			}

			public static LayoutJson FromLayout(InputControlLayout layout)
			{
				LayoutJson result = default(LayoutJson);
				result.name = layout.m_Name;
				result.type = layout.type.AssemblyQualifiedName;
				result.variant = layout.m_Variants;
				result.displayName = layout.m_DisplayName;
				result.description = layout.m_Description;
				result.isGenericTypeOfDevice = layout.isGenericTypeOfDevice;
				result.hideInUI = layout.hideInUI;
				result.extend = ((layout.m_BaseLayouts.length == 1) ? layout.m_BaseLayouts[0].ToString() : null);
				result.extendMultiple = ((layout.m_BaseLayouts.length > 1) ? layout.m_BaseLayouts.ToArray(_003C_003Ec._003C_003E9__14_0 ?? (_003C_003Ec._003C_003E9__14_0 = _003C_003Ec._003C_003E9._003CFromLayout_003Eb__14_0)) : null);
				result.format = layout.stateFormat.ToString();
				result.controls = ControlItemJson.FromControlItems(layout.m_Controls);
				return result;
			}
		}

		[Serializable]
		private class ControlItemJson
		{
			[Serializable]
			[CompilerGenerated]
			private sealed class _003C_003Ec
			{
				public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

				public static Func<string, InternedString> _003C_003E9__23_0;

				public static Func<string, InternedString> _003C_003E9__23_1;

				public static Func<NamedValue, string> _003C_003E9__24_0;

				public static Func<NameAndParameters, string> _003C_003E9__24_1;

				public static Func<InternedString, string> _003C_003E9__24_2;

				public static Func<InternedString, string> _003C_003E9__24_3;

				internal InternedString _003CToLayout_003Eb__23_0(string x)
				{
					return new InternedString(x);
				}

				internal InternedString _003CToLayout_003Eb__23_1(string x)
				{
					return new InternedString(x);
				}

				internal string _003CFromControlItems_003Eb__24_0(NamedValue x)
				{
					return x.ToString();
				}

				internal string _003CFromControlItems_003Eb__24_1(NameAndParameters x)
				{
					return x.ToString();
				}

				internal string _003CFromControlItems_003Eb__24_2(InternedString x)
				{
					return x.ToString();
				}

				internal string _003CFromControlItems_003Eb__24_3(InternedString x)
				{
					return x.ToString();
				}
			}

			public string name;

			public string layout;

			public string variants;

			public string usage;

			public string alias;

			public string useStateFrom;

			public uint offset;

			public uint bit;

			public uint sizeInBits;

			public string format;

			public int arraySize;

			public string[] usages;

			public string[] aliases;

			public string parameters;

			public string processors;

			public string displayName;

			public string shortDisplayName;

			public bool noisy;

			public bool synthetic;

			public string defaultState;

			public string minValue;

			public string maxValue;

			public ControlItemJson()
			{
				offset = uint.MaxValue;
				bit = uint.MaxValue;
			}

			public ControlItem ToLayout()
			{
				ControlItem controlItem = default(ControlItem);
				controlItem.name = new InternedString(name);
				controlItem.layout = new InternedString(layout);
				controlItem.variants = new InternedString(variants);
				controlItem.displayName = displayName;
				controlItem.shortDisplayName = shortDisplayName;
				controlItem.offset = offset;
				controlItem.useStateFrom = useStateFrom;
				controlItem.bit = bit;
				controlItem.sizeInBits = sizeInBits;
				controlItem.isModifyingExistingControl = name.IndexOf('/') != -1;
				controlItem.isNoisy = noisy;
				controlItem.isSynthetic = synthetic;
				controlItem.isFirstDefinedInThisLayout = true;
				controlItem.arraySize = arraySize;
				ControlItem result = controlItem;
				if (!string.IsNullOrEmpty(format))
				{
					result.format = new FourCC(format);
				}
				if (!string.IsNullOrEmpty(usage) || usages != null)
				{
					List<string> list = new List<string>();
					if (!string.IsNullOrEmpty(usage))
					{
						list.Add(usage);
					}
					if (usages != null)
					{
						list.AddRange(usages);
					}
					result.usages = new ReadOnlyArray<InternedString>(list.Select(_003C_003Ec._003C_003E9__23_0 ?? (_003C_003Ec._003C_003E9__23_0 = _003C_003Ec._003C_003E9._003CToLayout_003Eb__23_0)).ToArray());
				}
				if (!string.IsNullOrEmpty(alias) || aliases != null)
				{
					List<string> list2 = new List<string>();
					if (!string.IsNullOrEmpty(alias))
					{
						list2.Add(alias);
					}
					if (aliases != null)
					{
						list2.AddRange(aliases);
					}
					result.aliases = new ReadOnlyArray<InternedString>(list2.Select(_003C_003Ec._003C_003E9__23_1 ?? (_003C_003Ec._003C_003E9__23_1 = _003C_003Ec._003C_003E9._003CToLayout_003Eb__23_1)).ToArray());
				}
				if (!string.IsNullOrEmpty(parameters))
				{
					result.parameters = new ReadOnlyArray<NamedValue>(NamedValue.ParseMultiple(parameters));
				}
				if (!string.IsNullOrEmpty(processors))
				{
					result.processors = new ReadOnlyArray<NameAndParameters>(NameAndParameters.ParseMultiple(processors).ToArray());
				}
				if (defaultState != null)
				{
					result.defaultState = PrimitiveValue.FromObject(defaultState);
				}
				if (minValue != null)
				{
					result.minValue = PrimitiveValue.FromObject(minValue);
				}
				if (maxValue != null)
				{
					result.maxValue = PrimitiveValue.FromObject(maxValue);
				}
				return result;
			}

			public static ControlItemJson[] FromControlItems(ControlItem[] items)
			{
				if (items == null)
				{
					return null;
				}
				int num = items.Length;
				ControlItemJson[] array = new ControlItemJson[num];
				for (int i = 0; i < num; i++)
				{
					ControlItem controlItem = items[i];
					array[i] = new ControlItemJson
					{
						name = controlItem.name,
						layout = controlItem.layout,
						variants = controlItem.variants,
						displayName = controlItem.displayName,
						shortDisplayName = controlItem.shortDisplayName,
						bit = controlItem.bit,
						offset = controlItem.offset,
						sizeInBits = controlItem.sizeInBits,
						format = controlItem.format.ToString(),
						parameters = string.Join(",", controlItem.parameters.Select(_003C_003Ec._003C_003E9__24_0 ?? (_003C_003Ec._003C_003E9__24_0 = _003C_003Ec._003C_003E9._003CFromControlItems_003Eb__24_0)).ToArray()),
						processors = string.Join(",", controlItem.processors.Select(_003C_003Ec._003C_003E9__24_1 ?? (_003C_003Ec._003C_003E9__24_1 = _003C_003Ec._003C_003E9._003CFromControlItems_003Eb__24_1)).ToArray()),
						usages = controlItem.usages.Select(_003C_003Ec._003C_003E9__24_2 ?? (_003C_003Ec._003C_003E9__24_2 = _003C_003Ec._003C_003E9._003CFromControlItems_003Eb__24_2)).ToArray(),
						aliases = controlItem.aliases.Select(_003C_003Ec._003C_003E9__24_3 ?? (_003C_003Ec._003C_003E9__24_3 = _003C_003Ec._003C_003E9._003CFromControlItems_003Eb__24_3)).ToArray(),
						noisy = controlItem.isNoisy,
						synthetic = controlItem.isSynthetic,
						arraySize = controlItem.arraySize,
						defaultState = controlItem.defaultState.ToString(),
						minValue = controlItem.minValue.ToString(),
						maxValue = controlItem.maxValue.ToString()
					};
				}
				return array;
			}
		}

		internal struct Collection
		{
			public struct LayoutMatcher
			{
				public InternedString layoutName;

				public InputDeviceMatcher deviceMatcher;
			}

			public const float kBaseScoreForNonGeneratedLayouts = 1f;

			public Dictionary<InternedString, Type> layoutTypes;

			public Dictionary<InternedString, string> layoutStrings;

			public Dictionary<InternedString, Func<InputControlLayout>> layoutBuilders;

			public Dictionary<InternedString, InternedString> baseLayoutTable;

			public Dictionary<InternedString, InternedString[]> layoutOverrides;

			public HashSet<InternedString> layoutOverrideNames;

			public List<LayoutMatcher> layoutMatchers;

			public void Allocate()
			{
				layoutTypes = new Dictionary<InternedString, Type>();
				layoutStrings = new Dictionary<InternedString, string>();
				layoutBuilders = new Dictionary<InternedString, Func<InputControlLayout>>();
				baseLayoutTable = new Dictionary<InternedString, InternedString>();
				layoutOverrides = new Dictionary<InternedString, InternedString[]>();
				layoutOverrideNames = new HashSet<InternedString>();
				layoutMatchers = new List<LayoutMatcher>();
			}

			public InternedString TryFindLayoutForType(Type layoutType)
			{
				foreach (KeyValuePair<InternedString, Type> layoutType2 in layoutTypes)
				{
					if (layoutType2.Value == layoutType)
					{
						return layoutType2.Key;
					}
				}
				return default(InternedString);
			}

			public InternedString TryFindMatchingLayout(InputDeviceDescription deviceDescription)
			{
				float num = 0f;
				InternedString result = default(InternedString);
				int count = layoutMatchers.Count;
				for (int i = 0; i < count; i++)
				{
					float num2 = layoutMatchers[i].deviceMatcher.MatchPercentage(deviceDescription);
					if (num2 > 0f && !layoutBuilders.ContainsKey(layoutMatchers[i].layoutName))
					{
						num2 += 1f;
					}
					if (num2 > num)
					{
						num = num2;
						result = layoutMatchers[i].layoutName;
					}
				}
				return result;
			}

			public bool HasLayout(InternedString name)
			{
				if (!layoutTypes.ContainsKey(name) && !layoutStrings.ContainsKey(name))
				{
					return layoutBuilders.ContainsKey(name);
				}
				return true;
			}

			private InputControlLayout TryLoadLayoutInternal(InternedString name)
			{
				string value;
				if (layoutStrings.TryGetValue(name, out value))
				{
					return FromJson(value);
				}
				Type value2;
				if (layoutTypes.TryGetValue(name, out value2))
				{
					return FromType(name, value2);
				}
				Func<InputControlLayout> value3;
				if (layoutBuilders.TryGetValue(name, out value3))
				{
					InputControlLayout inputControlLayout = value3();
					if (inputControlLayout == null)
					{
						throw new InvalidOperationException(string.Format("Layout builder '{0}' returned null when invoked", name));
					}
					return inputControlLayout;
				}
				return null;
			}

			public InputControlLayout TryLoadLayout(InternedString name, Dictionary<InternedString, InputControlLayout> table = null)
			{
				InputControlLayout value;
				if (table != null && table.TryGetValue(name, out value))
				{
					return value;
				}
				value = TryLoadLayoutInternal(name);
				if (value != null)
				{
					value.m_Name = name;
					if (layoutOverrideNames.Contains(name))
					{
						value.isOverride = true;
					}
					InternedString value2 = default(InternedString);
					if (!value.isOverride && baseLayoutTable.TryGetValue(name, out value2))
					{
						InputControlLayout inputControlLayout = TryLoadLayout(value2, table);
						if (inputControlLayout == null)
						{
							throw new LayoutNotFoundException(string.Format("Cannot find base layout '{0}' of layout '{1}'", value2, name));
						}
						value.MergeLayout(inputControlLayout);
						if (value.m_BaseLayouts.length == 0)
						{
							value.m_BaseLayouts.Append(value2);
						}
					}
					InternedString[] value3;
					if (layoutOverrides.TryGetValue(name, out value3))
					{
						foreach (InternedString internedString in value3)
						{
							InputControlLayout inputControlLayout2 = TryLoadLayout(internedString);
							inputControlLayout2.MergeLayout(value);
							inputControlLayout2.m_BaseLayouts.Clear();
							inputControlLayout2.isOverride = false;
							inputControlLayout2.isGenericTypeOfDevice = value.isGenericTypeOfDevice;
							inputControlLayout2.m_Name = value.name;
							value = inputControlLayout2;
							value.m_AppliedOverrides.Append(internedString);
						}
					}
					if (table != null)
					{
						table[name] = value;
					}
				}
				return value;
			}

			public InternedString GetBaseLayoutName(InternedString layoutName)
			{
				InternedString value;
				if (baseLayoutTable.TryGetValue(layoutName, out value))
				{
					return value;
				}
				return default(InternedString);
			}

			public InternedString GetRootLayoutName(InternedString layoutName)
			{
				InternedString value;
				while (baseLayoutTable.TryGetValue(layoutName, out value))
				{
					layoutName = value;
				}
				return layoutName;
			}

			public bool ComputeDistanceInInheritanceHierarchy(InternedString firstLayout, InternedString secondLayout, out int distance)
			{
				distance = 0;
				int num = 0;
				InternedString internedString = secondLayout;
				while (!internedString.IsEmpty() && internedString != firstLayout)
				{
					internedString = GetBaseLayoutName(internedString);
					num++;
				}
				if (internedString == firstLayout)
				{
					distance = num;
					return true;
				}
				int num2 = 0;
				internedString = firstLayout;
				while (!internedString.IsEmpty() && internedString != secondLayout)
				{
					internedString = GetBaseLayoutName(internedString);
					num2++;
				}
				if (internedString == secondLayout)
				{
					distance = num2;
					return true;
				}
				return false;
			}

			public InternedString FindLayoutThatIntroducesControl(InputControl control, Cache cache)
			{
				InputControl inputControl = control;
				while (inputControl.parent != control.device)
				{
					inputControl = inputControl.parent;
				}
				InternedString internedString = control.device.m_Layout;
				InternedString value = internedString;
				while (baseLayoutTable.TryGetValue(value, out value))
				{
					if (cache.FindOrLoadLayout(value).FindControl(inputControl.m_Name).HasValue)
					{
						internedString = value;
					}
				}
				return internedString;
			}

			public Type GetControlTypeForLayout(InternedString layoutName)
			{
				while (layoutStrings.ContainsKey(layoutName))
				{
					InternedString value;
					if (baseLayoutTable.TryGetValue(layoutName, out value))
					{
						layoutName = value;
						continue;
					}
					return typeof(InputDevice);
				}
				Type value2;
				layoutTypes.TryGetValue(layoutName, out value2);
				return value2;
			}

			public bool ValueTypeIsAssignableFrom(InternedString layoutName, Type valueType)
			{
				Type controlTypeForLayout = GetControlTypeForLayout(layoutName);
				if (controlTypeForLayout == null)
				{
					return false;
				}
				Type genericTypeArgumentFromHierarchy = TypeHelpers.GetGenericTypeArgumentFromHierarchy(controlTypeForLayout, typeof(InputControl<>), 0);
				if (genericTypeArgumentFromHierarchy == null)
				{
					return false;
				}
				return valueType.IsAssignableFrom(genericTypeArgumentFromHierarchy);
			}

			public bool IsGeneratedLayout(InternedString layout)
			{
				return layoutBuilders.ContainsKey(layout);
			}

			public bool IsBasedOn(InternedString parentLayout, InternedString childLayout)
			{
				InternedString value = childLayout;
				while (baseLayoutTable.TryGetValue(value, out value))
				{
					if (value == parentLayout)
					{
						return true;
					}
				}
				return false;
			}

			public void AddMatcher(InternedString layout, InputDeviceMatcher matcher)
			{
				int count = layoutMatchers.Count;
				for (int i = 0; i < count; i++)
				{
					if (layoutMatchers[i].deviceMatcher == matcher)
					{
						return;
					}
				}
				layoutMatchers.Add(new LayoutMatcher
				{
					layoutName = layout,
					deviceMatcher = matcher
				});
			}
		}

		public class LayoutNotFoundException : Exception
		{
			[CompilerGenerated]
			private readonly string _003Clayout_003Ek__BackingField;

			public string layout
			{
				[CompilerGenerated]
				get
				{
					return _003Clayout_003Ek__BackingField;
				}
			}

			public LayoutNotFoundException()
			{
			}

			public LayoutNotFoundException(string name, string message)
				: base(message)
			{
				_003Clayout_003Ek__BackingField = name;
			}

			public LayoutNotFoundException(string name)
				: base("Cannot find control layout '" + name + "'")
			{
				_003Clayout_003Ek__BackingField = name;
			}

			public LayoutNotFoundException(string message, Exception innerException)
				: base(message, innerException)
			{
			}

			protected LayoutNotFoundException(SerializationInfo info, StreamingContext context)
				: base(info, context)
			{
			}
		}

		internal struct Cache
		{
			public Dictionary<InternedString, InputControlLayout> table;

			public void Clear()
			{
				table = null;
			}

			public InputControlLayout FindOrLoadLayout(string name, bool throwIfNotFound = true)
			{
				InternedString name2 = new InternedString(name);
				if (table == null)
				{
					table = new Dictionary<InternedString, InputControlLayout>();
				}
				InputControlLayout inputControlLayout = s_Layouts.TryLoadLayout(name2, table);
				if (inputControlLayout != null)
				{
					return inputControlLayout;
				}
				if (throwIfNotFound)
				{
					throw new LayoutNotFoundException(name);
				}
				return null;
			}
		}

		internal struct CacheRefInstance : IDisposable
		{
			public bool valid;

			public void Dispose()
			{
				if (valid)
				{
					s_CacheInstanceRef--;
					if (s_CacheInstanceRef <= 0)
					{
						s_CacheInstance = default(Cache);
						s_CacheInstanceRef = 0;
					}
					valid = false;
				}
			}
		}

		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<string, InternedString> _003C_003E9__46_0;

			public static Func<string, InternedString> _003C_003E9__69_0;

			public static Func<string, InternedString> _003C_003E9__69_1;

			internal InternedString _003CFromType_003Eb__46_0(string x)
			{
				return new InternedString(x);
			}

			internal InternedString _003CCreateControlItemFromMember_003Eb__69_0(string x)
			{
				return new InternedString(x);
			}

			internal InternedString _003CCreateControlItemFromMember_003Eb__69_1(string x)
			{
				return new InternedString(x);
			}
		}

		private static InternedString s_DefaultVariant = new InternedString("Default");

		public const string VariantSeparator = ";";

		private InternedString m_Name;

		private Type m_Type;

		private InternedString m_Variants;

		private FourCC m_StateFormat;

		internal int m_StateSizeInBytes;

		internal bool? m_UpdateBeforeRender;

		internal InlinedArray<InternedString> m_BaseLayouts;

		private InlinedArray<InternedString> m_AppliedOverrides;

		private InternedString[] m_CommonUsages;

		internal ControlItem[] m_Controls;

		internal string m_DisplayName;

		private string m_Description;

		private Flags m_Flags;

		internal static Collection s_Layouts;

		internal static Cache s_CacheInstance;

		internal static int s_CacheInstanceRef;

		public static InternedString DefaultVariant
		{
			get
			{
				return s_DefaultVariant;
			}
		}

		public InternedString name
		{
			get
			{
				return m_Name;
			}
		}

		public string displayName
		{
			get
			{
				return m_DisplayName ?? ((string)m_Name);
			}
		}

		public Type type
		{
			get
			{
				return m_Type;
			}
		}

		public InternedString variants
		{
			get
			{
				return m_Variants;
			}
		}

		public FourCC stateFormat
		{
			get
			{
				return m_StateFormat;
			}
		}

		public int stateSizeInBytes
		{
			get
			{
				return m_StateSizeInBytes;
			}
		}

		public IEnumerable<InternedString> baseLayouts
		{
			get
			{
				return m_BaseLayouts;
			}
		}

		public IEnumerable<InternedString> appliedOverrides
		{
			get
			{
				return m_AppliedOverrides;
			}
		}

		public ReadOnlyArray<InternedString> commonUsages
		{
			get
			{
				return new ReadOnlyArray<InternedString>(m_CommonUsages);
			}
		}

		public ReadOnlyArray<ControlItem> controls
		{
			get
			{
				return new ReadOnlyArray<ControlItem>(m_Controls);
			}
		}

		public bool updateBeforeRender
		{
			get
			{
				return m_UpdateBeforeRender ?? false;
			}
		}

		public bool isDeviceLayout
		{
			get
			{
				return typeof(InputDevice).IsAssignableFrom(m_Type);
			}
		}

		public bool isControlLayout
		{
			get
			{
				return !isDeviceLayout;
			}
		}

		public bool isOverride
		{
			get
			{
				return (m_Flags & Flags.IsOverride) != 0;
			}
			internal set
			{
				if (value)
				{
					m_Flags |= Flags.IsOverride;
				}
				else
				{
					m_Flags &= ~Flags.IsOverride;
				}
			}
		}

		public bool isGenericTypeOfDevice
		{
			get
			{
				return (m_Flags & Flags.IsGenericTypeOfDevice) != 0;
			}
			internal set
			{
				if (value)
				{
					m_Flags |= Flags.IsGenericTypeOfDevice;
				}
				else
				{
					m_Flags &= ~Flags.IsGenericTypeOfDevice;
				}
			}
		}

		public bool hideInUI
		{
			get
			{
				return (m_Flags & Flags.HideInUI) != 0;
			}
			internal set
			{
				if (value)
				{
					m_Flags |= Flags.HideInUI;
				}
				else
				{
					m_Flags &= ~Flags.HideInUI;
				}
			}
		}

		public ControlItem this[string path]
		{
			get
			{
				if (string.IsNullOrEmpty(path))
				{
					throw new ArgumentNullException("path");
				}
				if (m_Controls != null)
				{
					for (int i = 0; i < m_Controls.Length; i++)
					{
						if (m_Controls[i].name == path)
						{
							return m_Controls[i];
						}
					}
				}
				throw new KeyNotFoundException(string.Format("Cannot find control '{0}' in layout '{1}'", path, name));
			}
		}

		internal static ref Cache cache
		{
			get
			{
				return ref s_CacheInstance;
			}
		}

		public ControlItem? FindControl(InternedString path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			if (m_Controls == null)
			{
				return null;
			}
			for (int i = 0; i < m_Controls.Length; i++)
			{
				if (m_Controls[i].name == path)
				{
					return m_Controls[i];
				}
			}
			return null;
		}

		public ControlItem? FindControlIncludingArrayElements(string path, out int arrayIndex)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			arrayIndex = -1;
			if (m_Controls == null)
			{
				return null;
			}
			int num = 0;
			int num2 = path.Length;
			while (num2 > 0 && char.IsDigit(path[num2 - 1]))
			{
				num2--;
				num *= 10;
				num += path[num2] - 48;
			}
			int num3 = 0;
			if (num2 < path.Length && num2 > 0)
			{
				num3 = num2;
			}
			for (int i = 0; i < m_Controls.Length; i++)
			{
				ref ControlItem reference = ref m_Controls[i];
				if (string.Compare(reference.name, path, StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					return reference;
				}
				if (reference.isArray && num3 > 0 && num3 == reference.name.length && string.Compare(reference.name.ToString(), 0, path, 0, num3, StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					arrayIndex = num;
					return reference;
				}
			}
			return null;
		}

		public Type GetValueType()
		{
			return TypeHelpers.GetGenericTypeArgumentFromHierarchy(type, typeof(InputControl<>), 0);
		}

		public static InputControlLayout FromType(string name, Type type)
		{
			List<ControlItem> list = new List<ControlItem>();
			InputControlLayoutAttribute customAttribute = type.GetCustomAttribute<InputControlLayoutAttribute>(true);
			FourCC fourCC = default(FourCC);
			if (customAttribute != null && customAttribute.stateType != null)
			{
				AddControlItems(customAttribute.stateType, list, name);
				if (typeof(IInputStateTypeInfo).IsAssignableFrom(customAttribute.stateType))
				{
					fourCC = ((IInputStateTypeInfo)Activator.CreateInstance(customAttribute.stateType)).format;
				}
			}
			else
			{
				AddControlItems(type, list, name);
			}
			if (customAttribute != null && !string.IsNullOrEmpty(customAttribute.stateFormat))
			{
				fourCC = new FourCC(customAttribute.stateFormat);
			}
			InternedString internedString = default(InternedString);
			if (customAttribute != null)
			{
				internedString = new InternedString(customAttribute.variants);
			}
			InputControlLayout inputControlLayout = new InputControlLayout(name, type)
			{
				m_Controls = list.ToArray(),
				m_StateFormat = fourCC,
				m_Variants = internedString,
				m_UpdateBeforeRender = ((customAttribute != null) ? customAttribute.updateBeforeRenderInternal : null),
				isGenericTypeOfDevice = (customAttribute != null && customAttribute.isGenericTypeOfDevice),
				hideInUI = (customAttribute != null && customAttribute.hideInUI),
				m_Description = ((customAttribute != null) ? customAttribute.description : null),
				m_DisplayName = ((customAttribute != null) ? customAttribute.displayName : null)
			};
			if (((customAttribute != null) ? customAttribute.commonUsages : null) != null)
			{
				inputControlLayout.m_CommonUsages = ArrayHelpers.Select(customAttribute.commonUsages, _003C_003Ec._003C_003E9__46_0 ?? (_003C_003Ec._003C_003E9__46_0 = _003C_003Ec._003C_003E9._003CFromType_003Eb__46_0));
			}
			return inputControlLayout;
		}

		public string ToJson()
		{
			return JsonUtility.ToJson(LayoutJson.FromLayout(this), true);
		}

		public static InputControlLayout FromJson(string json)
		{
			return JsonUtility.FromJson<LayoutJson>(json).ToLayout();
		}

		private InputControlLayout(string name, Type type)
		{
			m_Name = new InternedString(name);
			m_Type = type;
		}

		private static void AddControlItems(Type type, List<ControlItem> controlLayouts, string layoutName)
		{
			AddControlItemsFromFields(type, controlLayouts, layoutName);
			AddControlItemsFromProperties(type, controlLayouts, layoutName);
		}

		private static void AddControlItemsFromFields(Type type, List<ControlItem> controlLayouts, string layoutName)
		{
			MemberInfo[] fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
			AddControlItemsFromMembers(fields, controlLayouts, layoutName);
		}

		private static void AddControlItemsFromProperties(Type type, List<ControlItem> controlLayouts, string layoutName)
		{
			MemberInfo[] properties = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
			AddControlItemsFromMembers(properties, controlLayouts, layoutName);
		}

		private static void AddControlItemsFromMembers(MemberInfo[] members, List<ControlItem> controlItems, string layoutName)
		{
			foreach (MemberInfo memberInfo in members)
			{
				if (memberInfo.DeclaringType == typeof(InputControl))
				{
					continue;
				}
				Type valueType = TypeHelpers.GetValueType(memberInfo);
				if (valueType != null && valueType.IsValueType && typeof(IInputStateTypeInfo).IsAssignableFrom(valueType))
				{
					int count = controlItems.Count;
					AddControlItems(valueType, controlItems, layoutName);
					if (memberInfo as FieldInfo != null)
					{
						int num = Marshal.OffsetOf(memberInfo.DeclaringType, memberInfo.Name).ToInt32();
						int count2 = controlItems.Count;
						for (int j = count; j < count2; j++)
						{
							ControlItem value = controlItems[j];
							if (controlItems[j].offset != uint.MaxValue)
							{
								value.offset += (uint)num;
								controlItems[j] = value;
							}
						}
					}
				}
				InputControlAttribute[] array = memberInfo.GetCustomAttributes<InputControlAttribute>(false).ToArray();
				if (array.Length != 0 || (!(valueType == null) && typeof(InputControl).IsAssignableFrom(valueType) && !(memberInfo is PropertyInfo)))
				{
					AddControlItemsFromMember(memberInfo, array, controlItems, layoutName);
				}
			}
		}

		private static void AddControlItemsFromMember(MemberInfo member, InputControlAttribute[] attributes, List<ControlItem> controlItems, string layoutName)
		{
			if (attributes.Length == 0)
			{
				ControlItem controlItem = CreateControlItemFromMember(member, null);
				ThrowIfControlItemIsDuplicate(ref controlItem, controlItems, layoutName);
				controlItems.Add(controlItem);
				return;
			}
			foreach (InputControlAttribute attribute in attributes)
			{
				ControlItem controlItem2 = CreateControlItemFromMember(member, attribute);
				ThrowIfControlItemIsDuplicate(ref controlItem2, controlItems, layoutName);
				controlItems.Add(controlItem2);
			}
		}

		private static ControlItem CreateControlItemFromMember(MemberInfo member, InputControlAttribute attribute)
		{
			string text = ((attribute != null) ? attribute.name : null);
			if (string.IsNullOrEmpty(text))
			{
				text = member.Name;
			}
			bool flag = text.IndexOf('/') != -1;
			string text2 = ((attribute != null) ? attribute.displayName : null);
			string shortDisplayName = ((attribute != null) ? attribute.shortDisplayName : null);
			string text3 = ((attribute != null) ? attribute.layout : null);
			if (string.IsNullOrEmpty(text3) && !flag && (!(member is FieldInfo) || member.GetCustomAttribute<FixedBufferAttribute>(false) == null))
			{
				text3 = InferLayoutFromValueType(TypeHelpers.GetValueType(member));
			}
			string text4 = null;
			if (attribute != null && !string.IsNullOrEmpty(attribute.variants))
			{
				text4 = attribute.variants;
			}
			uint offset = uint.MaxValue;
			if (attribute != null && attribute.offset != uint.MaxValue)
			{
				offset = attribute.offset;
			}
			else if (member is FieldInfo && !flag)
			{
				offset = (uint)Marshal.OffsetOf(member.DeclaringType, member.Name).ToInt32();
			}
			uint num = uint.MaxValue;
			if (attribute != null)
			{
				num = attribute.bit;
			}
			uint sizeInBits = 0u;
			if (attribute != null)
			{
				sizeInBits = attribute.sizeInBits;
			}
			FourCC format = default(FourCC);
			if (attribute != null && !string.IsNullOrEmpty(attribute.format))
			{
				format = new FourCC(attribute.format);
			}
			else if (!flag && num == uint.MaxValue)
			{
				format = InputStateBlock.GetPrimitiveFormatFromType(TypeHelpers.GetValueType(member));
			}
			InternedString[] array = null;
			if (attribute != null)
			{
				string[] array2 = ArrayHelpers.Join(attribute.alias, attribute.aliases);
				if (array2 != null)
				{
					array = array2.Select(_003C_003Ec._003C_003E9__69_0 ?? (_003C_003Ec._003C_003E9__69_0 = _003C_003Ec._003C_003E9._003CCreateControlItemFromMember_003Eb__69_0)).ToArray();
				}
			}
			InternedString[] array3 = null;
			if (attribute != null)
			{
				string[] array4 = ArrayHelpers.Join(attribute.usage, attribute.usages);
				if (array4 != null)
				{
					array3 = array4.Select(_003C_003Ec._003C_003E9__69_1 ?? (_003C_003Ec._003C_003E9__69_1 = _003C_003Ec._003C_003E9._003CCreateControlItemFromMember_003Eb__69_1)).ToArray();
				}
			}
			NamedValue[] array5 = null;
			if (attribute != null && !string.IsNullOrEmpty(attribute.parameters))
			{
				array5 = NamedValue.ParseMultiple(attribute.parameters);
			}
			NameAndParameters[] array6 = null;
			if (attribute != null && !string.IsNullOrEmpty(attribute.processors))
			{
				array6 = NameAndParameters.ParseMultiple(attribute.processors).ToArray();
			}
			string useStateFrom = null;
			if (attribute != null && !string.IsNullOrEmpty(attribute.useStateFrom))
			{
				useStateFrom = attribute.useStateFrom;
			}
			bool isNoisy = false;
			if (attribute != null)
			{
				isNoisy = attribute.noisy;
			}
			bool isSynthetic = false;
			if (attribute != null)
			{
				isSynthetic = attribute.synthetic;
			}
			int arraySize = 0;
			if (attribute != null)
			{
				arraySize = attribute.arraySize;
			}
			PrimitiveValue defaultState = default(PrimitiveValue);
			if (attribute != null)
			{
				defaultState = PrimitiveValue.FromObject(attribute.defaultState);
			}
			PrimitiveValue minValue = default(PrimitiveValue);
			PrimitiveValue maxValue = default(PrimitiveValue);
			if (attribute != null)
			{
				minValue = PrimitiveValue.FromObject(attribute.minValue);
				maxValue = PrimitiveValue.FromObject(attribute.maxValue);
			}
			ControlItem result = default(ControlItem);
			result.name = new InternedString(text);
			result.displayName = text2;
			result.shortDisplayName = shortDisplayName;
			result.layout = new InternedString(text3);
			result.variants = new InternedString(text4);
			result.useStateFrom = useStateFrom;
			result.format = format;
			result.offset = offset;
			result.bit = num;
			result.sizeInBits = sizeInBits;
			result.parameters = new ReadOnlyArray<NamedValue>(array5);
			result.processors = new ReadOnlyArray<NameAndParameters>(array6);
			result.usages = new ReadOnlyArray<InternedString>(array3);
			result.aliases = new ReadOnlyArray<InternedString>(array);
			result.isModifyingExistingControl = flag;
			result.isFirstDefinedInThisLayout = true;
			result.isNoisy = isNoisy;
			result.isSynthetic = isSynthetic;
			result.arraySize = arraySize;
			result.defaultState = defaultState;
			result.minValue = minValue;
			result.maxValue = maxValue;
			return result;
		}

		private static string InferLayoutFromValueType(Type type)
		{
			InternedString internedString = s_Layouts.TryFindLayoutForType(type);
			if (internedString.IsEmpty())
			{
				InternedString internedString2 = new InternedString(type.Name);
				if (s_Layouts.HasLayout(internedString2))
				{
					internedString = internedString2;
				}
				else if (type.Name.EndsWith("Control"))
				{
					internedString2 = new InternedString(type.Name.Substring(0, type.Name.Length - "Control".Length));
					if (s_Layouts.HasLayout(internedString2))
					{
						internedString = internedString2;
					}
				}
			}
			return internedString;
		}

		public void MergeLayout(InputControlLayout other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			m_UpdateBeforeRender = m_UpdateBeforeRender ?? other.m_UpdateBeforeRender;
			if (m_Variants.IsEmpty())
			{
				m_Variants = other.m_Variants;
			}
			if (m_Type == null)
			{
				m_Type = other.m_Type;
			}
			else if (m_Type.IsAssignableFrom(other.m_Type))
			{
				m_Type = other.m_Type;
			}
			bool flag = !m_Variants.IsEmpty();
			if (m_StateFormat == default(FourCC))
			{
				m_StateFormat = other.m_StateFormat;
			}
			m_CommonUsages = ArrayHelpers.Merge(other.m_CommonUsages, m_CommonUsages);
			m_AppliedOverrides.Merge(other.m_AppliedOverrides);
			if (string.IsNullOrEmpty(m_DisplayName))
			{
				m_DisplayName = other.m_DisplayName;
			}
			if (m_Controls == null)
			{
				m_Controls = other.m_Controls;
			}
			else
			{
				if (other.m_Controls == null)
				{
					return;
				}
				ControlItem[] controlItems = other.m_Controls;
				List<ControlItem> list = new List<ControlItem>();
				List<string> list2 = new List<string>();
				Dictionary<string, ControlItem> dictionary = CreateLookupTableForControls(controlItems, list2);
				foreach (KeyValuePair<string, ControlItem> item5 in CreateLookupTableForControls(m_Controls))
				{
					ControlItem value;
					if (dictionary.TryGetValue(item5.Key, out value))
					{
						ControlItem item = item5.Value.Merge(value);
						list.Add(item);
						dictionary.Remove(item5.Key);
					}
					else if (item5.Value.variants.IsEmpty() || item5.Value.variants == DefaultVariant)
					{
						bool flag2 = false;
						if (flag)
						{
							for (int i = 0; i < list2.Count; i++)
							{
								if (VariantsMatch(m_Variants.ToLower(), list2[i]))
								{
									string key = item5.Key + "@" + list2[i];
									if (dictionary.TryGetValue(key, out value))
									{
										ControlItem item2 = item5.Value.Merge(value);
										list.Add(item2);
										dictionary.Remove(key);
										flag2 = true;
									}
								}
							}
						}
						else
						{
							foreach (string item6 in list2)
							{
								string key2 = item5.Key + "@" + item6;
								if (dictionary.TryGetValue(key2, out value))
								{
									ControlItem item3 = item5.Value.Merge(value);
									list.Add(item3);
									dictionary.Remove(key2);
									flag2 = true;
								}
							}
						}
						if (!flag2)
						{
							list.Add(item5.Value);
						}
					}
					else if (dictionary.TryGetValue(item5.Value.name.ToLower(), out value))
					{
						ControlItem item4 = item5.Value.Merge(value);
						list.Add(item4);
						dictionary.Remove(item5.Value.name.ToLower());
					}
					else if (VariantsMatch(m_Variants, item5.Value.variants))
					{
						list.Add(item5.Value);
					}
				}
				if (!flag)
				{
					int count = list.Count;
					list.AddRange(dictionary.Values);
					for (int j = count; j < list.Count; j++)
					{
						ControlItem value2 = list[j];
						value2.isFirstDefinedInThisLayout = false;
						list[j] = value2;
					}
				}
				else
				{
					int count2 = list.Count;
					list.AddRange(dictionary.Values.Where(_003CMergeLayout_003Eb__71_0));
					for (int k = count2; k < list.Count; k++)
					{
						ControlItem value3 = list[k];
						value3.isFirstDefinedInThisLayout = false;
						list[k] = value3;
					}
				}
				m_Controls = list.ToArray();
			}
		}

		private static Dictionary<string, ControlItem> CreateLookupTableForControls(ControlItem[] controlItems, List<string> variants = null)
		{
			Dictionary<string, ControlItem> dictionary = new Dictionary<string, ControlItem>();
			for (int i = 0; i < controlItems.Length; i++)
			{
				string text = controlItems[i].name.ToLower();
				InternedString internedString = controlItems[i].variants;
				if (!internedString.IsEmpty() && internedString != DefaultVariant)
				{
					if (internedString.ToString().IndexOf(";"[0]) != -1)
					{
						string[] array = internedString.ToLower().Split(";"[0]);
						foreach (string text2 in array)
						{
							if (variants != null)
							{
								variants.Add(text2);
							}
							text = text + "@" + text2;
							dictionary[text] = controlItems[i];
						}
						continue;
					}
					text = text + "@" + internedString.ToLower();
					if (variants != null)
					{
						variants.Add(internedString.ToLower());
					}
				}
				dictionary[text] = controlItems[i];
			}
			return dictionary;
		}

		internal static bool VariantsMatch(InternedString expected, InternedString actual)
		{
			return VariantsMatch(expected.ToLower(), actual.ToLower());
		}

		internal static bool VariantsMatch(string expected, string actual)
		{
			if (actual != null && StringHelpers.CharacterSeparatedListsHaveAtLeastOneCommonElement(DefaultVariant, actual, ";"[0]))
			{
				return true;
			}
			if (expected == null)
			{
				return true;
			}
			if (actual == null)
			{
				return true;
			}
			return StringHelpers.CharacterSeparatedListsHaveAtLeastOneCommonElement(expected, actual, ";"[0]);
		}

		private static void ThrowIfControlItemIsDuplicate(ref ControlItem controlItem, IEnumerable<ControlItem> controlLayouts, string layoutName)
		{
			InternedString internedString = controlItem.name;
			foreach (ControlItem controlLayout in controlLayouts)
			{
				if (string.Compare(internedString, controlLayout.name, StringComparison.OrdinalIgnoreCase) == 0 && controlLayout.variants == controlItem.variants)
				{
					throw new InvalidOperationException(string.Format("Duplicate control '{0}' in layout '{1}'", internedString, layoutName));
				}
			}
		}

		internal static void ParseHeaderFieldsFromJson(string json, out InternedString name, out InlinedArray<InternedString> baseLayouts, out InputDeviceMatcher deviceMatcher)
		{
			LayoutJsonNameAndDescriptorOnly layoutJsonNameAndDescriptorOnly = JsonUtility.FromJson<LayoutJsonNameAndDescriptorOnly>(json);
			name = new InternedString(layoutJsonNameAndDescriptorOnly.name);
			baseLayouts = default(InlinedArray<InternedString>);
			if (!string.IsNullOrEmpty(layoutJsonNameAndDescriptorOnly.extend))
			{
				baseLayouts.Append(new InternedString(layoutJsonNameAndDescriptorOnly.extend));
			}
			if (layoutJsonNameAndDescriptorOnly.extendMultiple != null)
			{
				string[] extendMultiple = layoutJsonNameAndDescriptorOnly.extendMultiple;
				foreach (string text in extendMultiple)
				{
					baseLayouts.Append(new InternedString(text));
				}
			}
			deviceMatcher = layoutJsonNameAndDescriptorOnly.device.ToMatcher();
		}

		internal static CacheRefInstance CacheRef()
		{
			s_CacheInstanceRef++;
			CacheRefInstance result = default(CacheRefInstance);
			result.valid = true;
			return result;
		}

		[CompilerGenerated]
		private bool _003CMergeLayout_003Eb__71_0(ControlItem x)
		{
			return VariantsMatch(m_Variants, x.variants);
		}
	}
}
