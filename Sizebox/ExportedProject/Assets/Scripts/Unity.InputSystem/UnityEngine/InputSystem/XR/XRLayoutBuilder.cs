using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.XR;

namespace UnityEngine.InputSystem.XR
{
	internal class XRLayoutBuilder
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass5_0
		{
			public XRLayoutBuilder layout;

			internal InputControlLayout _003COnFindLayoutForDevice_003Eb__0()
			{
				return layout.Build();
			}
		}

		private string parentLayout;

		private string interfaceName;

		private XRDeviceDescriptor descriptor;

		private static uint GetSizeOfFeature(XRFeatureDescriptor featureDescriptor)
		{
			switch (featureDescriptor.featureType)
			{
			case FeatureType.Binary:
				return 1u;
			case FeatureType.DiscreteStates:
				return 4u;
			case FeatureType.Axis1D:
				return 4u;
			case FeatureType.Axis2D:
				return 8u;
			case FeatureType.Axis3D:
				return 12u;
			case FeatureType.Rotation:
				return 16u;
			case FeatureType.Hand:
				return 104u;
			case FeatureType.Bone:
				return 32u;
			case FeatureType.Eyes:
				return 76u;
			case FeatureType.Custom:
				return featureDescriptor.customSize;
			default:
				return 0u;
			}
		}

		private static string SanitizeString(string original, bool allowPaths = false)
		{
			int length = original.Length;
			StringBuilder stringBuilder = new StringBuilder(length);
			for (int i = 0; i < length; i++)
			{
				char c = original[i];
				if (char.IsUpper(c) || char.IsLower(c) || char.IsDigit(c) || (allowPaths && c == '/'))
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		internal static string OnFindLayoutForDevice(ref InputDeviceDescription description, string matchedLayout, InputDeviceExecuteCommandDelegate executeCommandDelegate)
		{
			_003C_003Ec__DisplayClass5_0 _003C_003Ec__DisplayClass5_ = new _003C_003Ec__DisplayClass5_0();
			if (description.interfaceName != "XRInputV1" && description.interfaceName != "XRInput")
			{
				return null;
			}
			if (string.IsNullOrEmpty(description.capabilities))
			{
				return null;
			}
			XRDeviceDescriptor xRDeviceDescriptor;
			try
			{
				xRDeviceDescriptor = XRDeviceDescriptor.FromJson(description.capabilities);
			}
			catch (Exception)
			{
				return null;
			}
			if (xRDeviceDescriptor == null)
			{
				return null;
			}
			if (string.IsNullOrEmpty(matchedLayout))
			{
				if ((xRDeviceDescriptor.characteristics & InputDeviceCharacteristics.HeadMounted) != 0)
				{
					matchedLayout = "XRHMD";
				}
				else if ((xRDeviceDescriptor.characteristics & (InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller)) == (InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller))
				{
					matchedLayout = "XRController";
				}
			}
			string text = ((!string.IsNullOrEmpty(description.manufacturer)) ? (SanitizeString(description.interfaceName) + "::" + SanitizeString(description.manufacturer) + "::" + SanitizeString(description.product)) : (SanitizeString(description.interfaceName) + "::" + SanitizeString(description.product)));
			_003C_003Ec__DisplayClass5_.layout = new XRLayoutBuilder
			{
				descriptor = xRDeviceDescriptor,
				parentLayout = matchedLayout,
				interfaceName = description.interfaceName
			};
			InputSystem.RegisterLayoutBuilder(_003C_003Ec__DisplayClass5_._003COnFindLayoutForDevice_003Eb__0, text, matchedLayout);
			return text;
		}

		private static string ConvertPotentialAliasToName(InputControlLayout layout, string nameOrAlias)
		{
			InternedString internedString = new InternedString(nameOrAlias);
			ReadOnlyArray<InputControlLayout.ControlItem> controls = layout.controls;
			for (int i = 0; i < controls.Count; i++)
			{
				InputControlLayout.ControlItem controlItem = controls[i];
				if (controlItem.name == internedString)
				{
					return nameOrAlias;
				}
				ReadOnlyArray<InternedString> aliases = controlItem.aliases;
				for (int j = 0; j < aliases.Count; j++)
				{
					if (aliases[j] == nameOrAlias)
					{
						return controlItem.name.ToString();
					}
				}
			}
			return nameOrAlias;
		}

		private InputControlLayout Build()
		{
			InputControlLayout.Builder builder = new InputControlLayout.Builder
			{
				stateFormat = new FourCC('X', 'R', 'S', '0'),
				extendsLayout = parentLayout,
				updateBeforeRender = true
			};
			InputControlLayout inputControlLayout = ((!string.IsNullOrEmpty(parentLayout)) ? InputSystem.LoadLayout(parentLayout) : null);
			List<string> list = new List<string>();
			uint num = 0u;
			foreach (XRFeatureDescriptor inputFeature in descriptor.inputFeatures)
			{
				list.Clear();
				if (inputFeature.usageHints != null)
				{
					foreach (UsageHint usageHint in inputFeature.usageHints)
					{
						if (!string.IsNullOrEmpty(usageHint.content))
						{
							list.Add(usageHint.content);
						}
					}
				}
				string name = inputFeature.name;
				name = SanitizeString(name, true);
				if (inputControlLayout != null)
				{
					name = ConvertPotentialAliasToName(inputControlLayout, name);
				}
				name = name.ToLower();
				uint sizeOfFeature = GetSizeOfFeature(inputFeature);
				if (!(interfaceName == "XRInput") && sizeOfFeature >= 4 && num % 4u != 0)
				{
					num += 4 - num % 4u;
				}
				switch (inputFeature.featureType)
				{
				case FeatureType.Binary:
					builder.AddControl(name).WithLayout("Button").WithByteOffset(num)
						.WithFormat(InputStateBlock.FormatBit)
						.WithUsages(list);
					break;
				case FeatureType.DiscreteStates:
					builder.AddControl(name).WithLayout("Integer").WithByteOffset(num)
						.WithFormat(InputStateBlock.FormatInt)
						.WithUsages(list);
					break;
				case FeatureType.Axis1D:
					builder.AddControl(name).WithLayout("Analog").WithByteOffset(num)
						.WithFormat(InputStateBlock.FormatFloat)
						.WithUsages(list);
					break;
				case FeatureType.Axis2D:
					builder.AddControl(name).WithLayout("Vector2").WithByteOffset(num)
						.WithFormat(InputStateBlock.FormatVector2)
						.WithUsages(list);
					break;
				case FeatureType.Axis3D:
					builder.AddControl(name).WithLayout("Vector3").WithByteOffset(num)
						.WithFormat(InputStateBlock.FormatVector3)
						.WithUsages(list);
					break;
				case FeatureType.Rotation:
					builder.AddControl(name).WithLayout("Quaternion").WithByteOffset(num)
						.WithFormat(InputStateBlock.FormatQuaternion)
						.WithUsages(list);
					break;
				case FeatureType.Bone:
					builder.AddControl(name).WithLayout("Bone").WithByteOffset(num)
						.WithUsages(list);
					break;
				case FeatureType.Eyes:
					builder.AddControl(name).WithLayout("Eyes").WithByteOffset(num)
						.WithUsages(list);
					break;
				}
				num += sizeOfFeature;
			}
			return builder.Build();
		}
	}
}
