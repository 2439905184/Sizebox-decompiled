using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem.HID
{
	[Preserve]
	public class HID : InputDevice
	{
		[Serializable]
		private class HIDLayoutBuilder
		{
			[Serializable]
			[CompilerGenerated]
			private sealed class _003C_003Ec
			{
				public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

				public static Predicate<HIDElementDescriptor> _003C_003E9__4_0;

				public static Predicate<HIDElementDescriptor> _003C_003E9__4_1;

				public static Func<InputControlLayout.ControlItem, string> _003C_003E9__4_2;

				internal bool _003CBuild_003Eb__4_0(HIDElementDescriptor element)
				{
					if (element.usagePage == UsagePage.GenericDesktop)
					{
						return element.usage == 48;
					}
					return false;
				}

				internal bool _003CBuild_003Eb__4_1(HIDElementDescriptor element)
				{
					if (element.usagePage == UsagePage.GenericDesktop)
					{
						return element.usage == 49;
					}
					return false;
				}

				internal string _003CBuild_003Eb__4_2(InputControlLayout.ControlItem x)
				{
					return x.name;
				}
			}

			public string displayName;

			public HIDDeviceDescriptor hidDescriptor;

			public string parentLayout;

			public Type deviceType;

			public InputControlLayout Build()
			{
				InputControlLayout.Builder builder = new InputControlLayout.Builder
				{
					displayName = displayName,
					type = deviceType,
					extendsLayout = parentLayout,
					stateFormat = new FourCC('H', 'I', 'D')
				};
				HIDElementDescriptor hIDElementDescriptor = Array.Find(hidDescriptor.elements, _003C_003Ec._003C_003E9__4_0 ?? (_003C_003Ec._003C_003E9__4_0 = _003C_003Ec._003C_003E9._003CBuild_003Eb__4_0));
				HIDElementDescriptor hIDElementDescriptor2 = Array.Find(hidDescriptor.elements, _003C_003Ec._003C_003E9__4_1 ?? (_003C_003Ec._003C_003E9__4_1 = _003C_003Ec._003C_003E9._003CBuild_003Eb__4_1));
				bool flag = hIDElementDescriptor.usage == 48 && hIDElementDescriptor2.usage == 49;
				if (flag)
				{
					int bit;
					int num;
					int sizeInBits;
					if (hIDElementDescriptor.reportOffsetInBits <= hIDElementDescriptor2.reportOffsetInBits)
					{
						bit = hIDElementDescriptor.reportOffsetInBits % 8;
						num = hIDElementDescriptor.reportOffsetInBits / 8;
						sizeInBits = hIDElementDescriptor2.reportOffsetInBits + hIDElementDescriptor2.reportSizeInBits - hIDElementDescriptor.reportOffsetInBits;
					}
					else
					{
						bit = hIDElementDescriptor2.reportOffsetInBits % 8;
						num = hIDElementDescriptor2.reportOffsetInBits / 8;
						sizeInBits = hIDElementDescriptor.reportOffsetInBits + hIDElementDescriptor.reportSizeInBits - hIDElementDescriptor2.reportSizeInBits;
					}
					builder.AddControl("stick").WithDisplayName("Stick").WithLayout("Stick")
						.WithBitOffset((uint)bit)
						.WithByteOffset((uint)num)
						.WithSizeInBits((uint)sizeInBits)
						.WithUsages(CommonUsages.Primary2DMotion);
					string text = hIDElementDescriptor.DetermineParameters();
					string text2 = hIDElementDescriptor2.DetermineParameters();
					builder.AddControl("stick/x").WithFormat(hIDElementDescriptor.isSigned ? InputStateBlock.FormatSBit : InputStateBlock.FormatBit).WithByteOffset((uint)(hIDElementDescriptor.reportOffsetInBits / 8 - num))
						.WithBitOffset((uint)(hIDElementDescriptor.reportOffsetInBits % 8))
						.WithSizeInBits((uint)hIDElementDescriptor.reportSizeInBits)
						.WithParameters(text)
						.WithDefaultState(hIDElementDescriptor.DetermineDefaultState())
						.WithProcessors(hIDElementDescriptor.DetermineProcessors());
					builder.AddControl("stick/y").WithFormat(hIDElementDescriptor2.isSigned ? InputStateBlock.FormatSBit : InputStateBlock.FormatBit).WithByteOffset((uint)(hIDElementDescriptor2.reportOffsetInBits / 8 - num))
						.WithBitOffset((uint)(hIDElementDescriptor2.reportOffsetInBits % 8))
						.WithSizeInBits((uint)hIDElementDescriptor2.reportSizeInBits)
						.WithParameters(text2)
						.WithDefaultState(hIDElementDescriptor2.DetermineDefaultState())
						.WithProcessors(hIDElementDescriptor2.DetermineProcessors());
					builder.AddControl("stick/up").WithParameters(StringHelpers.Join<string>(",", text2, "clamp=2,clampMin=-1,clampMax=0,invert=true"));
					builder.AddControl("stick/down").WithParameters(StringHelpers.Join<string>(",", text2, "clamp=2,clampMin=0,clampMax=1,invert=false"));
					builder.AddControl("stick/left").WithParameters(StringHelpers.Join<string>(",", text, "clamp=2,clampMin=-1,clampMax=0,invert"));
					builder.AddControl("stick/right").WithParameters(StringHelpers.Join<string>(",", text, "clamp=2,clampMin=0,clampMax=1"));
				}
				HIDElementDescriptor[] elements = hidDescriptor.elements;
				int num2 = elements.Length;
				for (int i = 0; i < num2; i++)
				{
					ref HIDElementDescriptor reference = ref elements[i];
					if (reference.reportType != HIDReportType.Input || (flag && (reference.Is(UsagePage.GenericDesktop, 48) || reference.Is(UsagePage.GenericDesktop, 49))))
					{
						continue;
					}
					string text3 = reference.DetermineLayout();
					if (text3 != null)
					{
						string baseName = reference.DetermineName();
						baseName = StringHelpers.MakeUniqueName(baseName, builder.controls, _003C_003Ec._003C_003E9__4_2 ?? (_003C_003Ec._003C_003E9__4_2 = _003C_003Ec._003C_003E9._003CBuild_003Eb__4_2));
						InputControlLayout.Builder.ControlBuilder controlBuilder = builder.AddControl(baseName).WithDisplayName(reference.DetermineDisplayName()).WithLayout(text3)
							.WithByteOffset((uint)reference.reportOffsetInBits / 8u)
							.WithBitOffset((uint)reference.reportOffsetInBits % 8u)
							.WithSizeInBits((uint)reference.reportSizeInBits)
							.WithFormat(reference.DetermineFormat())
							.WithDefaultState(reference.DetermineDefaultState())
							.WithProcessors(reference.DetermineProcessors());
						string text4 = reference.DetermineParameters();
						if (!string.IsNullOrEmpty(text4))
						{
							controlBuilder.WithParameters(text4);
						}
						InternedString[] array = reference.DetermineUsages();
						if (array != null)
						{
							controlBuilder.WithUsages(array);
						}
						reference.AddChildControls(ref reference, baseName, ref builder);
					}
				}
				return builder.Build();
			}
		}

		public enum HIDReportType
		{
			Unknown = 0,
			Input = 1,
			Output = 2,
			Feature = 3
		}

		public enum HIDCollectionType
		{
			Physical = 0,
			Application = 1,
			Logical = 2,
			Report = 3,
			NamedArray = 4,
			UsageSwitch = 5,
			UsageModifier = 6
		}

		[Flags]
		public enum HIDElementFlags
		{
			Constant = 1,
			Variable = 2,
			Relative = 4,
			Wrap = 8,
			NonLinear = 0x10,
			NoPreferred = 0x20,
			NullState = 0x40,
			Volatile = 0x80,
			BufferedBytes = 0x100
		}

		[Serializable]
		public struct HIDElementDescriptor
		{
			public int usage;

			public UsagePage usagePage;

			public int unit;

			public int unitExponent;

			public int logicalMin;

			public int logicalMax;

			public int physicalMin;

			public int physicalMax;

			public HIDReportType reportType;

			public int collectionIndex;

			public int reportId;

			public int reportSizeInBits;

			public int reportOffsetInBits;

			public HIDElementFlags flags;

			public int? usageMin;

			public int? usageMax;

			public bool hasNullState
			{
				get
				{
					return (flags & HIDElementFlags.NullState) == HIDElementFlags.NullState;
				}
			}

			public bool hasPreferredState
			{
				get
				{
					return (flags & HIDElementFlags.NoPreferred) != HIDElementFlags.NoPreferred;
				}
			}

			public bool isArray
			{
				get
				{
					return (flags & HIDElementFlags.Variable) != HIDElementFlags.Variable;
				}
			}

			public bool isNonLinear
			{
				get
				{
					return (flags & HIDElementFlags.NonLinear) == HIDElementFlags.NonLinear;
				}
			}

			public bool isRelative
			{
				get
				{
					return (flags & HIDElementFlags.Relative) == HIDElementFlags.Relative;
				}
			}

			public bool isConstant
			{
				get
				{
					return (flags & HIDElementFlags.Constant) == HIDElementFlags.Constant;
				}
			}

			public bool isWrapping
			{
				get
				{
					return (flags & HIDElementFlags.Wrap) == HIDElementFlags.Wrap;
				}
			}

			internal bool isSigned
			{
				get
				{
					return logicalMin < 0;
				}
			}

			internal float minFloatValue
			{
				get
				{
					int num = (1 << reportSizeInBits) - 1;
					if (isSigned)
					{
						return (float)logicalMin / (float)((num + 1) / 2);
					}
					return (float)logicalMin / (float)num;
				}
			}

			internal float maxFloatValue
			{
				get
				{
					int num = (1 << reportSizeInBits) - 1;
					if (isSigned)
					{
						return (float)logicalMax / (float)((num + 1) / 2);
					}
					return (float)logicalMax / (float)num;
				}
			}

			public bool Is(UsagePage usagePage, int usage)
			{
				if (usagePage == this.usagePage)
				{
					return usage == this.usage;
				}
				return false;
			}

			internal string DetermineName()
			{
				switch (usagePage)
				{
				case UsagePage.Button:
					if (usage == 1)
					{
						return "trigger";
					}
					return string.Format("button{0}", usage);
				case UsagePage.GenericDesktop:
				{
					if (usage == 57)
					{
						return "hat";
					}
					GenericDesktop genericDesktop = (GenericDesktop)usage;
					string text = genericDesktop.ToString();
					return char.ToLowerInvariant(text[0]) + text.Substring(1);
				}
				default:
					return string.Format("UsagePage({0:X}) Usage({1:X})", usagePage, usage);
				}
			}

			internal string DetermineDisplayName()
			{
				switch (usagePage)
				{
				case UsagePage.Button:
					if (usage == 1)
					{
						return "Trigger";
					}
					return string.Format("Button {0}", usage);
				case UsagePage.GenericDesktop:
				{
					GenericDesktop genericDesktop = (GenericDesktop)usage;
					return genericDesktop.ToString();
				}
				default:
					return null;
				}
			}

			internal bool IsUsableElement()
			{
				int num = usage;
				if ((uint)(num - 48) <= 1u)
				{
					return usagePage == UsagePage.GenericDesktop;
				}
				return DetermineLayout() != null;
			}

			internal string DetermineLayout()
			{
				if (reportType != HIDReportType.Input)
				{
					return null;
				}
				switch (usagePage)
				{
				case UsagePage.Button:
					return "Button";
				case UsagePage.GenericDesktop:
					switch (usage)
					{
					case 48:
					case 49:
					case 50:
					case 51:
					case 52:
					case 53:
					case 54:
					case 55:
					case 56:
					case 64:
					case 65:
					case 66:
					case 67:
					case 68:
					case 69:
						return "Axis";
					case 61:
					case 62:
					case 144:
					case 145:
					case 146:
					case 147:
						return "Button";
					case 57:
						if (logicalMax - logicalMin + 1 == 8)
						{
							return "Dpad";
						}
						break;
					}
					break;
				}
				return null;
			}

			internal FourCC DetermineFormat()
			{
				switch (reportSizeInBits)
				{
				case 8:
					if (isSigned)
					{
						return InputStateBlock.FormatSByte;
					}
					return InputStateBlock.FormatByte;
				case 16:
					if (isSigned)
					{
						return InputStateBlock.FormatShort;
					}
					return InputStateBlock.FormatUShort;
				case 32:
					if (isSigned)
					{
						return InputStateBlock.FormatInt;
					}
					return InputStateBlock.FormatUInt;
				default:
					return InputStateBlock.FormatBit;
				}
			}

			internal InternedString[] DetermineUsages()
			{
				if (usagePage == UsagePage.Button && usage == 1)
				{
					return new InternedString[2]
					{
						CommonUsages.PrimaryTrigger,
						CommonUsages.PrimaryAction
					};
				}
				if (usagePage == UsagePage.Button && usage == 2)
				{
					return new InternedString[2]
					{
						CommonUsages.SecondaryTrigger,
						CommonUsages.SecondaryAction
					};
				}
				if (usagePage == UsagePage.GenericDesktop && usage == 53)
				{
					return new InternedString[1] { CommonUsages.Twist };
				}
				return null;
			}

			internal string DetermineParameters()
			{
				if (usagePage == UsagePage.GenericDesktop)
				{
					switch (usage)
					{
					case 48:
					case 50:
					case 51:
					case 53:
					case 54:
					case 55:
					case 56:
					case 64:
					case 66:
					case 67:
					case 69:
						return DetermineAxisNormalizationParameters();
					case 49:
					case 52:
					case 65:
					case 68:
						return StringHelpers.Join<string>(",", "invert", DetermineAxisNormalizationParameters());
					}
				}
				return null;
			}

			private string DetermineAxisNormalizationParameters()
			{
				if (logicalMin == 0 && logicalMax == 0)
				{
					return "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5";
				}
				float num = minFloatValue;
				float num2 = maxFloatValue;
				if (Mathf.Approximately(0f, num) && Mathf.Approximately(0f, num2))
				{
					return null;
				}
				float num3 = num + (num2 - num) / 2f;
				return string.Format(CultureInfo.InvariantCulture, "normalize,normalizeMin={0},normalizeMax={1},normalizeZero={2}", num, num2, num3);
			}

			internal string DetermineProcessors()
			{
				UsagePage usagePage = this.usagePage;
				if (usagePage == UsagePage.GenericDesktop)
				{
					int num = usage;
					if ((uint)(num - 48) <= 8u || (uint)(num - 64) <= 5u)
					{
						return "axisDeadzone";
					}
				}
				return null;
			}

			internal PrimitiveValue DetermineDefaultState()
			{
				UsagePage usagePage = this.usagePage;
				if (usagePage == UsagePage.GenericDesktop)
				{
					switch (usage)
					{
					case 57:
						if (hasNullState)
						{
							int num2 = logicalMin - 1;
							if (num2 >= 0)
							{
								return new PrimitiveValue(num2);
							}
							int num3 = logicalMax + 1;
							if (num3 <= (1 << reportSizeInBits) - 1)
							{
								return new PrimitiveValue(num3);
							}
						}
						break;
					case 48:
					case 49:
					case 50:
					case 51:
					case 52:
					case 53:
					case 54:
					case 55:
					case 56:
					case 64:
					case 65:
					case 66:
					case 67:
					case 68:
					case 69:
						if (!isSigned)
						{
							int num = logicalMin + (logicalMax - logicalMin) / 2;
							if (num != 0)
							{
								return new PrimitiveValue(num);
							}
						}
						break;
					}
				}
				return default(PrimitiveValue);
			}

			internal void AddChildControls(ref HIDElementDescriptor element, string controlName, ref InputControlLayout.Builder builder)
			{
				if (usagePage == UsagePage.GenericDesktop && usage == 57)
				{
					PrimitiveValue primitiveValue = DetermineDefaultState();
					if (!primitiveValue.isEmpty)
					{
						builder.AddControl(controlName + "/up").WithFormat(InputStateBlock.FormatBit).WithLayout("DiscreteButton")
							.WithParameters(string.Format(CultureInfo.InvariantCulture, "minValue={0},maxValue={1},nullValue={2},wrapAtValue={3}", logicalMax, logicalMin + 1, primitiveValue.ToString(), logicalMax))
							.WithBitOffset((uint)element.reportOffsetInBits % 8u)
							.WithSizeInBits((uint)reportSizeInBits);
						builder.AddControl(controlName + "/right").WithFormat(InputStateBlock.FormatBit).WithLayout("DiscreteButton")
							.WithParameters(string.Format(CultureInfo.InvariantCulture, "minValue={0},maxValue={1}", logicalMin + 1, logicalMin + 3))
							.WithBitOffset((uint)element.reportOffsetInBits % 8u)
							.WithSizeInBits((uint)reportSizeInBits);
						builder.AddControl(controlName + "/down").WithFormat(InputStateBlock.FormatBit).WithLayout("DiscreteButton")
							.WithParameters(string.Format(CultureInfo.InvariantCulture, "minValue={0},maxValue={1}", logicalMin + 3, logicalMin + 5))
							.WithBitOffset((uint)element.reportOffsetInBits % 8u)
							.WithSizeInBits((uint)reportSizeInBits);
						builder.AddControl(controlName + "/left").WithFormat(InputStateBlock.FormatBit).WithLayout("DiscreteButton")
							.WithParameters(string.Format(CultureInfo.InvariantCulture, "minValue={0},maxValue={1}", logicalMin + 5, logicalMin + 7))
							.WithBitOffset((uint)element.reportOffsetInBits % 8u)
							.WithSizeInBits((uint)reportSizeInBits);
					}
				}
			}
		}

		[Serializable]
		public struct HIDCollectionDescriptor
		{
			public HIDCollectionType type;

			public int usage;

			public UsagePage usagePage;

			public int parent;

			public int childCount;

			public int firstChild;
		}

		[Serializable]
		public struct HIDDeviceDescriptor
		{
			public int vendorId;

			public int productId;

			public int usage;

			public UsagePage usagePage;

			public int inputReportSize;

			public int outputReportSize;

			public int featureReportSize;

			public HIDElementDescriptor[] elements;

			public HIDCollectionDescriptor[] collections;

			public string ToJson()
			{
				return JsonUtility.ToJson(this, true);
			}

			public static HIDDeviceDescriptor FromJson(string json)
			{
				return JsonUtility.FromJson<HIDDeviceDescriptor>(json);
			}
		}

		public struct HIDDeviceDescriptorBuilder
		{
			public UsagePage usagePage;

			public int usage;

			private int m_CurrentReportId;

			private HIDReportType m_CurrentReportType;

			private int m_CurrentReportOffsetInBits;

			private List<HIDElementDescriptor> m_Elements;

			private List<HIDCollectionDescriptor> m_Collections;

			private int m_InputReportSize;

			private int m_OutputReportSize;

			private int m_FeatureReportSize;

			public HIDDeviceDescriptorBuilder(UsagePage usagePage, int usage)
			{
				this = default(HIDDeviceDescriptorBuilder);
				this.usagePage = usagePage;
				this.usage = usage;
			}

			public HIDDeviceDescriptorBuilder(GenericDesktop usage)
				: this(UsagePage.GenericDesktop, (int)usage)
			{
			}

			public HIDDeviceDescriptorBuilder StartReport(HIDReportType reportType, int reportId = 1)
			{
				m_CurrentReportId = reportId;
				m_CurrentReportType = reportType;
				m_CurrentReportOffsetInBits = 8;
				return this;
			}

			public HIDDeviceDescriptorBuilder AddElement(UsagePage usagePage, int usage, int sizeInBits)
			{
				if (m_Elements == null)
				{
					m_Elements = new List<HIDElementDescriptor>();
				}
				else
				{
					foreach (HIDElementDescriptor element in m_Elements)
					{
						if (element.reportId == m_CurrentReportId && element.reportType == m_CurrentReportType && element.usagePage == usagePage && element.usage == usage)
						{
							throw new InvalidOperationException(string.Format("Cannot add two elements with the same usage page '{0}' and usage '0x{1:X} the to same device", usagePage, usage));
						}
					}
				}
				m_Elements.Add(new HIDElementDescriptor
				{
					usage = usage,
					usagePage = usagePage,
					reportOffsetInBits = m_CurrentReportOffsetInBits,
					reportSizeInBits = sizeInBits,
					reportType = m_CurrentReportType,
					reportId = m_CurrentReportId
				});
				m_CurrentReportOffsetInBits += sizeInBits;
				return this;
			}

			public HIDDeviceDescriptorBuilder AddElement(GenericDesktop usage, int sizeInBits)
			{
				return AddElement(UsagePage.GenericDesktop, (int)usage, sizeInBits);
			}

			public HIDDeviceDescriptorBuilder WithPhysicalMinMax(int min, int max)
			{
				int num = m_Elements.Count - 1;
				if (num < 0)
				{
					throw new InvalidOperationException("No element has been added to the descriptor yet");
				}
				HIDElementDescriptor value = m_Elements[num];
				value.physicalMin = min;
				value.physicalMax = max;
				m_Elements[num] = value;
				return this;
			}

			public HIDDeviceDescriptorBuilder WithLogicalMinMax(int min, int max)
			{
				int num = m_Elements.Count - 1;
				if (num < 0)
				{
					throw new InvalidOperationException("No element has been added to the descriptor yet");
				}
				HIDElementDescriptor value = m_Elements[num];
				value.logicalMin = min;
				value.logicalMax = max;
				m_Elements[num] = value;
				return this;
			}

			public HIDDeviceDescriptor Finish()
			{
				HIDDeviceDescriptor result = default(HIDDeviceDescriptor);
				result.usage = usage;
				result.usagePage = usagePage;
				List<HIDElementDescriptor> elements = m_Elements;
				result.elements = ((elements != null) ? elements.ToArray() : null);
				List<HIDCollectionDescriptor> collections = m_Collections;
				result.collections = ((collections != null) ? collections.ToArray() : null);
				return result;
			}
		}

		public enum UsagePage
		{
			Undefined = 0,
			GenericDesktop = 1,
			Simulation = 2,
			VRControls = 3,
			SportControls = 4,
			GameControls = 5,
			GenericDeviceControls = 6,
			Keyboard = 7,
			LEDs = 8,
			Button = 9,
			Ordinal = 10,
			Telephony = 11,
			Consumer = 12,
			Digitizer = 13,
			PID = 15,
			Unicode = 16,
			AlphanumericDisplay = 20,
			MedicalInstruments = 64,
			Monitor = 128,
			Power = 132,
			BarCodeScanner = 140,
			MagneticStripeReader = 142,
			Camera = 144,
			Arcade = 145,
			VendorDefined = 65280
		}

		public enum GenericDesktop
		{
			Undefined = 0,
			Pointer = 1,
			Mouse = 2,
			Joystick = 4,
			Gamepad = 5,
			Keyboard = 6,
			Keypad = 7,
			MultiAxisController = 8,
			TabletPCControls = 9,
			AssistiveControl = 10,
			X = 48,
			Y = 49,
			Z = 50,
			Rx = 51,
			Ry = 52,
			Rz = 53,
			Slider = 54,
			Dial = 55,
			Wheel = 56,
			HatSwitch = 57,
			CountedBuffer = 58,
			ByteCount = 59,
			MotionWakeup = 60,
			Start = 61,
			Select = 62,
			Vx = 64,
			Vy = 65,
			Vz = 66,
			Vbrx = 67,
			Vbry = 68,
			Vbrz = 69,
			Vno = 70,
			FeatureNotification = 71,
			ResolutionMultiplier = 72,
			SystemControl = 128,
			SystemPowerDown = 129,
			SystemSleep = 130,
			SystemWakeUp = 131,
			SystemContextMenu = 132,
			SystemMainMenu = 133,
			SystemAppMenu = 134,
			SystemMenuHelp = 135,
			SystemMenuExit = 136,
			SystemMenuSelect = 137,
			SystemMenuRight = 138,
			SystemMenuLeft = 139,
			SystemMenuUp = 140,
			SystemMenuDown = 141,
			SystemColdRestart = 142,
			SystemWarmRestart = 143,
			DpadUp = 144,
			DpadDown = 145,
			DpadRight = 146,
			DpadLeft = 147,
			SystemDock = 160,
			SystemUndock = 161,
			SystemSetup = 162,
			SystemBreak = 163,
			SystemDebuggerBreak = 164,
			ApplicationBreak = 165,
			ApplicationDebuggerBreak = 166,
			SystemSpeakerMute = 167,
			SystemHibernate = 168,
			SystemDisplayInvert = 176,
			SystemDisplayInternal = 177,
			SystemDisplayExternal = 178,
			SystemDisplayBoth = 179,
			SystemDisplayDual = 180,
			SystemDisplayToggleIntExt = 181,
			SystemDisplaySwapPrimarySecondary = 182,
			SystemDisplayLCDAutoScale = 183
		}

		public enum Simulation
		{
			Undefined = 0,
			FlightSimulationDevice = 1,
			AutomobileSimulationDevice = 2,
			TankSimulationDevice = 3,
			SpaceshipSimulationDevice = 4,
			SubmarineSimulationDevice = 5,
			SailingSimulationDevice = 6,
			MotorcycleSimulationDevice = 7,
			SportsSimulationDevice = 8,
			AirplaneSimulationDevice = 9,
			HelicopterSimulationDevice = 10,
			MagicCarpetSimulationDevice = 11,
			BicylcleSimulationDevice = 12,
			FlightControlStick = 32,
			FlightStick = 33,
			CyclicControl = 34,
			CyclicTrim = 35,
			FlightYoke = 36,
			TrackControl = 37,
			Aileron = 176,
			AileronTrim = 177,
			AntiTorqueControl = 178,
			AutopilotEnable = 179,
			ChaffRelease = 180,
			CollectiveControl = 181,
			DiveBreak = 182,
			ElectronicCountermeasures = 183,
			Elevator = 184,
			ElevatorTrim = 185,
			Rudder = 186,
			Throttle = 187,
			FlightCommunications = 188,
			FlareRelease = 189,
			LandingGear = 190,
			ToeBreak = 191,
			Trigger = 192,
			WeaponsArm = 193,
			WeaponsSelect = 194,
			WingFlaps = 195,
			Accelerator = 196,
			Brake = 197,
			Clutch = 198,
			Shifter = 199,
			Steering = 200,
			TurretDirection = 201,
			BarrelElevation = 202,
			DivePlane = 203,
			Ballast = 204,
			BicycleCrank = 205,
			HandleBars = 206,
			FrontBrake = 207,
			RearBrake = 208
		}

		public enum Button
		{
			Undefined = 0,
			Primary = 1,
			Secondary = 2,
			Tertiary = 3
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass12_0
		{
			public HIDLayoutBuilder layout;

			internal InputControlLayout _003COnFindLayoutForDevice_003Eb__0()
			{
				return layout.Build();
			}
		}

		internal const string kHIDInterface = "HID";

		internal const string kHIDNamespace = "HID";

		private bool m_HaveParsedHIDDescriptor;

		private HIDDeviceDescriptor m_HIDDescriptor;

		public static FourCC QueryHIDReportDescriptorDeviceCommandType
		{
			get
			{
				return new FourCC('H', 'I', 'D', 'D');
			}
		}

		public static FourCC QueryHIDReportDescriptorSizeDeviceCommandType
		{
			get
			{
				return new FourCC('H', 'I', 'D', 'S');
			}
		}

		public static FourCC QueryHIDParsedReportDescriptorDeviceCommandType
		{
			get
			{
				return new FourCC('H', 'I', 'D', 'P');
			}
		}

		public HIDDeviceDescriptor hidDescriptor
		{
			get
			{
				if (!m_HaveParsedHIDDescriptor)
				{
					if (!string.IsNullOrEmpty(base.description.capabilities))
					{
						m_HIDDescriptor = JsonUtility.FromJson<HIDDeviceDescriptor>(base.description.capabilities);
					}
					m_HaveParsedHIDDescriptor = true;
				}
				return m_HIDDescriptor;
			}
		}

		internal static string OnFindLayoutForDevice(ref InputDeviceDescription description, string matchedLayout, InputDeviceExecuteCommandDelegate executeDeviceCommand)
		{
			_003C_003Ec__DisplayClass12_0 _003C_003Ec__DisplayClass12_ = new _003C_003Ec__DisplayClass12_0();
			if (!string.IsNullOrEmpty(matchedLayout))
			{
				return null;
			}
			if (description.interfaceName != "HID")
			{
				return null;
			}
			HIDDeviceDescriptor hIDDeviceDescriptor = ReadHIDDeviceDescriptor(ref description, executeDeviceCommand);
			if (!Enumerable.Contains(HIDSupport.supportedHIDUsages, new HIDSupport.HIDPageUsage(hIDDeviceDescriptor.usagePage, hIDDeviceDescriptor.usage)))
			{
				return null;
			}
			bool flag = false;
			if (hIDDeviceDescriptor.elements != null)
			{
				HIDElementDescriptor[] elements = hIDDeviceDescriptor.elements;
				foreach (HIDElementDescriptor hIDElementDescriptor in elements)
				{
					if (hIDElementDescriptor.IsUsableElement())
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				return null;
			}
			Type typeFromHandle = typeof(HID);
			string text = "HID";
			if (hIDDeviceDescriptor.usagePage == UsagePage.GenericDesktop && (hIDDeviceDescriptor.usage == 4 || hIDDeviceDescriptor.usage == 5))
			{
				text = "Joystick";
				typeFromHandle = typeof(Joystick);
			}
			string text2 = "";
			if (text != "Joystick")
			{
				text2 = ((hIDDeviceDescriptor.usagePage == UsagePage.GenericDesktop) ? string.Format(" {0}", (GenericDesktop)hIDDeviceDescriptor.usage) : string.Format(" {0}-{1}", hIDDeviceDescriptor.usagePage, hIDDeviceDescriptor.usage));
			}
			InputDeviceMatcher inputDeviceMatcher = InputDeviceMatcher.FromDeviceDescription(description);
			string result;
			if (!string.IsNullOrEmpty(description.product) && !string.IsNullOrEmpty(description.manufacturer))
			{
				result = "HID::" + description.manufacturer + " " + description.product + text2;
			}
			else if (!string.IsNullOrEmpty(description.product))
			{
				result = "HID::" + description.product + text2;
			}
			else
			{
				if (hIDDeviceDescriptor.vendorId == 0)
				{
					return null;
				}
				result = string.Format("{0}::{1:X}-{2:X}{3}", "HID", hIDDeviceDescriptor.vendorId, hIDDeviceDescriptor.productId, text2);
				inputDeviceMatcher = inputDeviceMatcher.WithCapability("productId", hIDDeviceDescriptor.productId).WithCapability("vendorId", hIDDeviceDescriptor.vendorId);
			}
			inputDeviceMatcher = inputDeviceMatcher.WithCapability("usage", hIDDeviceDescriptor.usage).WithCapability("usagePage", hIDDeviceDescriptor.usagePage);
			_003C_003Ec__DisplayClass12_.layout = new HIDLayoutBuilder
			{
				displayName = description.product,
				hidDescriptor = hIDDeviceDescriptor,
				parentLayout = text,
				deviceType = (typeFromHandle ?? typeof(HID))
			};
			InputSystem.RegisterLayoutBuilder(_003C_003Ec__DisplayClass12_._003COnFindLayoutForDevice_003Eb__0, result, text, inputDeviceMatcher);
			return result;
		}

		internal unsafe static HIDDeviceDescriptor ReadHIDDeviceDescriptor(ref InputDeviceDescription deviceDescription, InputDeviceExecuteCommandDelegate executeCommandDelegate)
		{
			if (deviceDescription.interfaceName != "HID")
			{
				throw new ArgumentException(string.Format("Device '{0}' is not a HID", deviceDescription));
			}
			bool flag = true;
			HIDDeviceDescriptor deviceDescriptor = default(HIDDeviceDescriptor);
			if (!string.IsNullOrEmpty(deviceDescription.capabilities))
			{
				try
				{
					deviceDescriptor = HIDDeviceDescriptor.FromJson(deviceDescription.capabilities);
					if (deviceDescriptor.elements != null && deviceDescriptor.elements.Length != 0)
					{
						flag = false;
					}
				}
				catch (Exception exception)
				{
					Debug.LogError(string.Format("Could not parse HID descriptor of device '{0}'", deviceDescription));
					Debug.LogException(exception);
				}
			}
			if (flag)
			{
				InputDeviceCommand command = new InputDeviceCommand(QueryHIDReportDescriptorSizeDeviceCommandType);
				long num = executeCommandDelegate(ref command);
				if (num <= 0)
				{
					using (NativeArray<byte> nativeArray = InputDeviceCommand.AllocateNative(QueryHIDParsedReportDescriptorDeviceCommandType, 2097152))
					{
						InputDeviceCommand* unsafePtr = (InputDeviceCommand*)nativeArray.GetUnsafePtr();
						long num2 = executeCommandDelegate(ref *unsafePtr);
						if (num2 < 0)
						{
							return default(HIDDeviceDescriptor);
						}
						byte[] array = new byte[num2];
						try
						{
							fixed (byte* destination = array)
							{
								UnsafeUtility.MemCpy(destination, unsafePtr->payloadPtr, num2);
							}
						}
						finally
						{
						}
						string @string = Encoding.UTF8.GetString(array, 0, (int)num2);
						try
						{
							deviceDescriptor = HIDDeviceDescriptor.FromJson(@string);
						}
						catch (Exception exception2)
						{
							Debug.LogError(string.Format("Could not parse HID descriptor of device '{0}'", deviceDescription));
							Debug.LogException(exception2);
							return default(HIDDeviceDescriptor);
						}
						deviceDescription.capabilities = @string;
						return deviceDescriptor;
					}
				}
				using (NativeArray<byte> nativeArray2 = InputDeviceCommand.AllocateNative(QueryHIDReportDescriptorDeviceCommandType, (int)num))
				{
					InputDeviceCommand* unsafePtr2 = (InputDeviceCommand*)nativeArray2.GetUnsafePtr();
					if (executeCommandDelegate(ref *unsafePtr2) != num)
					{
						return default(HIDDeviceDescriptor);
					}
					if (!HIDParser.ParseReportDescriptor((byte*)unsafePtr2->payloadPtr, (int)num, ref deviceDescriptor))
					{
						return default(HIDDeviceDescriptor);
					}
				}
				deviceDescription.capabilities = deviceDescriptor.ToJson();
			}
			return deviceDescriptor;
		}

		public static string UsagePageToString(UsagePage usagePage)
		{
			if (usagePage < UsagePage.VendorDefined)
			{
				return usagePage.ToString();
			}
			return "Vendor-Defined";
		}

		public static string UsageToString(UsagePage usagePage, int usage)
		{
			switch (usagePage)
			{
			case UsagePage.GenericDesktop:
			{
				GenericDesktop genericDesktop = (GenericDesktop)usage;
				return genericDesktop.ToString();
			}
			case UsagePage.Simulation:
			{
				Simulation simulation = (Simulation)usage;
				return simulation.ToString();
			}
			default:
				return null;
			}
		}
	}
}
