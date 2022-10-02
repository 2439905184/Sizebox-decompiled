using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.Collections;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
	public static class InputControlPath
	{
		[Flags]
		public enum HumanReadableStringOptions
		{
			None = 0,
			OmitDevice = 2,
			UseShortNames = 4
		}

		private enum PathComponentType
		{
			Name = 0,
			DisplayName = 1,
			Usage = 2,
			Layout = 3
		}

		internal struct ParsedPathComponent
		{
			public Substring layout;

			public Substring[] usages;

			public Substring name;

			public Substring displayName;

			public bool isWildcard
			{
				get
				{
					return name == "*";
				}
			}

			public bool isDoubleWildcard
			{
				get
				{
					return name == "**";
				}
			}

			public string ToHumanReadableString(string parentLayoutName, string parentControlPath, out string referencedLayoutName, out string controlPath, HumanReadableStringOptions options)
			{
				referencedLayoutName = null;
				controlPath = null;
				string text = string.Empty;
				if (isWildcard)
				{
					text += "Any";
				}
				if (usages != null)
				{
					string text2 = string.Empty;
					for (int i = 0; i < usages.Length; i++)
					{
						if (!usages[i].isEmpty)
						{
							text2 = ((!(text2 != string.Empty)) ? ToHumanReadableString(usages[i]) : (text2 + " & " + ToHumanReadableString(usages[i])));
						}
					}
					if (text2 != string.Empty)
					{
						text = ((!(text != string.Empty)) ? (text + text2) : (text + " " + text2));
					}
				}
				if (!layout.isEmpty)
				{
					referencedLayoutName = layout.ToString();
					InputControlLayout inputControlLayout = InputControlLayout.cache.FindOrLoadLayout(referencedLayoutName, false);
					string text3 = ((inputControlLayout == null || string.IsNullOrEmpty(inputControlLayout.m_DisplayName)) ? ToHumanReadableString(layout) : inputControlLayout.m_DisplayName);
					text = (string.IsNullOrEmpty(text) ? (text + text3) : (text + " " + text3));
				}
				if (!name.isEmpty && !isWildcard)
				{
					string text4 = null;
					if (!string.IsNullOrEmpty(parentLayoutName))
					{
						InputControlLayout inputControlLayout2 = InputControlLayout.cache.FindOrLoadLayout(new InternedString(parentLayoutName), false);
						if (inputControlLayout2 != null)
						{
							InternedString internedString = new InternedString(name.ToString());
							int arrayIndex;
							InputControlLayout.ControlItem? controlItem = inputControlLayout2.FindControlIncludingArrayElements(internedString, out arrayIndex);
							if (controlItem.HasValue)
							{
								if (string.IsNullOrEmpty(parentControlPath))
								{
									if (arrayIndex != -1)
									{
										controlPath = string.Format("{0}{1}", controlItem.Value.name, arrayIndex);
									}
									else
									{
										controlPath = controlItem.Value.name;
									}
								}
								else if (arrayIndex != -1)
								{
									controlPath = string.Format("{0}/{1}{2}", parentControlPath, controlItem.Value.name, arrayIndex);
								}
								else
								{
									controlPath = string.Format("{0}/{1}", parentControlPath, controlItem.Value.name);
								}
								string text5 = (((options & HumanReadableStringOptions.UseShortNames) != 0) ? controlItem.Value.shortDisplayName : null);
								string text6 = ((!string.IsNullOrEmpty(text5)) ? text5 : controlItem.Value.displayName);
								if (!string.IsNullOrEmpty(text6))
								{
									text4 = ((arrayIndex == -1) ? text6 : string.Format("{0} #{1}", text6, arrayIndex));
								}
								if (string.IsNullOrEmpty(referencedLayoutName))
								{
									referencedLayoutName = controlItem.Value.layout;
								}
							}
						}
					}
					if (text4 == null)
					{
						text4 = ToHumanReadableString(name);
					}
					text = (string.IsNullOrEmpty(text) ? (text + text4) : (text + " " + text4));
				}
				if (!displayName.isEmpty)
				{
					string text7 = "\"" + ToHumanReadableString(displayName) + "\"";
					text = (string.IsNullOrEmpty(text) ? (text + text7) : (text + " " + text7));
				}
				return text;
			}

			private static string ToHumanReadableString(Substring substring)
			{
				return substring.ToString().Unescape("/*{<", "/*{<");
			}

			public bool Matches(InputControl control)
			{
				if (!layout.isEmpty)
				{
					bool flag = Substring.Compare(layout, control.layout, StringComparison.InvariantCultureIgnoreCase) == 0;
					if (!flag)
					{
						InternedString value = control.m_Layout;
						while (InputControlLayout.s_Layouts.baseLayoutTable.TryGetValue(value, out value) && !flag)
						{
							flag = Substring.Compare(layout, value.ToString(), StringComparison.InvariantCultureIgnoreCase) == 0;
						}
					}
					if (!flag)
					{
						return false;
					}
				}
				if (usages != null)
				{
					for (int i = 0; i < usages.Length; i++)
					{
						if (usages[i].isEmpty)
						{
							continue;
						}
						ReadOnlyArray<InternedString> readOnlyArray = control.usages;
						bool flag2 = false;
						for (int j = 0; j < readOnlyArray.Count; j++)
						{
							if (Substring.Compare(readOnlyArray[j].ToString(), usages[i], StringComparison.InvariantCultureIgnoreCase) == 0)
							{
								flag2 = true;
								break;
							}
						}
						if (!flag2)
						{
							return false;
						}
					}
				}
				if (!name.isEmpty && !isWildcard && Substring.Compare(control.name, name, StringComparison.InvariantCultureIgnoreCase) != 0)
				{
					return false;
				}
				if (!displayName.isEmpty && Substring.Compare(control.displayName, displayName, StringComparison.InvariantCultureIgnoreCase) != 0)
				{
					return false;
				}
				return true;
			}
		}

		internal struct PathParser
		{
			public string path;

			public int length;

			public int leftIndexInPath;

			public int rightIndexInPath;

			public ParsedPathComponent current;

			public bool isAtEnd
			{
				get
				{
					return rightIndexInPath == length;
				}
			}

			public PathParser(string path)
			{
				this.path = path;
				length = path.Length;
				leftIndexInPath = 0;
				rightIndexInPath = 0;
				current = default(ParsedPathComponent);
			}

			public bool MoveToNextComponent()
			{
				if (rightIndexInPath == length)
				{
					return false;
				}
				leftIndexInPath = rightIndexInPath;
				if (path[leftIndexInPath] == '/')
				{
					leftIndexInPath++;
					rightIndexInPath = leftIndexInPath;
					if (leftIndexInPath == length)
					{
						return false;
					}
				}
				Substring layout = default(Substring);
				if (rightIndexInPath < length && path[rightIndexInPath] == '<')
				{
					layout = ParseComponentPart('>');
				}
				List<Substring> list = new List<Substring>();
				while (rightIndexInPath < length && path[rightIndexInPath] == '{')
				{
					list.Add(ParseComponentPart('}'));
				}
				Substring displayName = default(Substring);
				if (rightIndexInPath < length - 1 && path[rightIndexInPath] == '#' && path[rightIndexInPath + 1] == '(')
				{
					rightIndexInPath++;
					displayName = ParseComponentPart(')');
				}
				Substring name = default(Substring);
				if (rightIndexInPath < length && path[rightIndexInPath] != '/')
				{
					name = ParseComponentPart('/');
				}
				current = new ParsedPathComponent
				{
					layout = layout,
					usages = list.ToArray(),
					name = name,
					displayName = displayName
				};
				return leftIndexInPath != rightIndexInPath;
			}

			private Substring ParseComponentPart(char terminator)
			{
				if (terminator != '/')
				{
					rightIndexInPath++;
				}
				int num = rightIndexInPath;
				while (rightIndexInPath < length && path[rightIndexInPath] != terminator)
				{
					rightIndexInPath++;
				}
				int num2 = rightIndexInPath - num;
				if (rightIndexInPath < length && terminator != '/')
				{
					rightIndexInPath++;
				}
				return new Substring(path, num, num2);
			}
		}

		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Converter<Substring, string> _003C_003E9__7_0;

			internal string _003CTryGetDeviceUsages_003Eb__7_0(Substring i)
			{
				return i.ToString();
			}
		}

		public const string Wildcard = "*";

		public const string DoubleWildcard = "**";

		public const char Separator = '/';

		public static string Combine(InputControl parent, string path)
		{
			if (parent == null)
			{
				if (string.IsNullOrEmpty(path))
				{
					return string.Empty;
				}
				if (path[0] != '/')
				{
					return "/" + path;
				}
				return path;
			}
			if (string.IsNullOrEmpty(path))
			{
				return parent.path;
			}
			return parent.path + "/" + path;
		}

		public static string ToHumanReadableString(string path, HumanReadableStringOptions options = HumanReadableStringOptions.None, InputControl control = null)
		{
			string deviceLayoutName;
			string controlPath;
			return ToHumanReadableString(path, out deviceLayoutName, out controlPath, options, control);
		}

		public static string ToHumanReadableString(string path, out string deviceLayoutName, out string controlPath, HumanReadableStringOptions options = HumanReadableStringOptions.None, InputControl control = null)
		{
			deviceLayoutName = null;
			controlPath = null;
			if (string.IsNullOrEmpty(path))
			{
				return string.Empty;
			}
			if (control != null)
			{
				InputControl inputControl = TryFindControl(control, path) ?? (Matches(path, control) ? control : null);
				if (inputControl != null)
				{
					string text = (((options & HumanReadableStringOptions.UseShortNames) != 0 && !string.IsNullOrEmpty(inputControl.shortDisplayName)) ? inputControl.shortDisplayName : inputControl.displayName);
					if ((options & HumanReadableStringOptions.OmitDevice) == 0)
					{
						text = text + " [" + inputControl.device.displayName + "]";
					}
					deviceLayoutName = inputControl.device.layout;
					if (!(inputControl is InputDevice))
					{
						controlPath = inputControl.path.Substring(inputControl.device.path.Length + 1);
					}
					return text;
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			PathParser pathParser = new PathParser(path);
			using (InputControlLayout.CacheRef())
			{
				if (pathParser.MoveToNextComponent())
				{
					string referencedLayoutName;
					string controlPath2;
					string value = pathParser.current.ToHumanReadableString(null, null, out referencedLayoutName, out controlPath2, options);
					deviceLayoutName = referencedLayoutName;
					bool flag = true;
					while (pathParser.MoveToNextComponent())
					{
						if (!flag)
						{
							stringBuilder.Append('/');
						}
						stringBuilder.Append(pathParser.current.ToHumanReadableString(referencedLayoutName, controlPath, out referencedLayoutName, out controlPath, options));
						flag = false;
					}
					if ((options & HumanReadableStringOptions.OmitDevice) == 0 && !string.IsNullOrEmpty(value))
					{
						stringBuilder.Append(" [");
						stringBuilder.Append(value);
						stringBuilder.Append(']');
					}
				}
				if (stringBuilder.Length == 0)
				{
					return path;
				}
				return stringBuilder.ToString();
			}
		}

		public static string[] TryGetDeviceUsages(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			PathParser pathParser = new PathParser(path);
			if (!pathParser.MoveToNextComponent())
			{
				return null;
			}
			if (pathParser.current.usages != null && pathParser.current.usages.Length != 0)
			{
				return Array.ConvertAll(pathParser.current.usages, _003C_003Ec._003C_003E9__7_0 ?? (_003C_003Ec._003C_003E9__7_0 = _003C_003Ec._003C_003E9._003CTryGetDeviceUsages_003Eb__7_0));
			}
			return null;
		}

		public static string TryGetDeviceLayout(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			PathParser pathParser = new PathParser(path);
			if (!pathParser.MoveToNextComponent())
			{
				return null;
			}
			if (pathParser.current.layout.length > 0)
			{
				return pathParser.current.layout.ToString();
			}
			if (pathParser.current.isWildcard)
			{
				return "*";
			}
			return null;
		}

		public static string TryGetControlLayout(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			int length = path.Length;
			int num = path.LastIndexOf('/');
			if (num == -1 || num == 0)
			{
				return null;
			}
			if (length > num + 2 && path[num + 1] == '<' && path[length - 1] == '>')
			{
				int num2 = num + 2;
				int length2 = length - num2 - 1;
				return path.Substring(num2, length2);
			}
			PathParser parser = new PathParser(path);
			if (!parser.MoveToNextComponent())
			{
				return null;
			}
			if (parser.current.isWildcard)
			{
				throw new NotImplementedException();
			}
			if (parser.current.layout.length == 0)
			{
				return null;
			}
			string layoutName = parser.current.layout.ToString();
			if (!parser.MoveToNextComponent())
			{
				return null;
			}
			if (parser.current.isWildcard)
			{
				return "*";
			}
			return FindControlLayoutRecursive(ref parser, layoutName);
		}

		private static string FindControlLayoutRecursive(ref PathParser parser, string layoutName)
		{
			using (InputControlLayout.CacheRef())
			{
				InputControlLayout inputControlLayout = InputControlLayout.cache.FindOrLoadLayout(new InternedString(layoutName), false);
				if (inputControlLayout == null)
				{
					return null;
				}
				return FindControlLayoutRecursive(ref parser, inputControlLayout);
			}
		}

		private static string FindControlLayoutRecursive(ref PathParser parser, InputControlLayout layout)
		{
			string text = null;
			int count = layout.controls.Count;
			for (int i = 0; i < count; i++)
			{
				if (!ControlLayoutMatchesPathComponent(ref layout.m_Controls[i], ref parser))
				{
					continue;
				}
				InternedString layout2 = layout.m_Controls[i].layout;
				if (!parser.isAtEnd)
				{
					PathParser parser2 = parser;
					if (!parser2.MoveToNextComponent())
					{
						continue;
					}
					string text2 = FindControlLayoutRecursive(ref parser2, layout2);
					if (text2 != null)
					{
						if (text != null && text2 != text)
						{
							return null;
						}
						text = text2;
					}
				}
				else
				{
					if (text != null && layout2 != text)
					{
						return null;
					}
					text = layout2.ToString();
				}
			}
			return text;
		}

		private static bool ControlLayoutMatchesPathComponent(ref InputControlLayout.ControlItem controlItem, ref PathParser parser)
		{
			Substring layout = parser.current.layout;
			if (layout.length > 0 && !StringMatches(layout, controlItem.layout))
			{
				return false;
			}
			if (parser.current.usages != null)
			{
				for (int i = 0; i < parser.current.usages.Length; i++)
				{
					Substring str = parser.current.usages[i];
					if (str.length <= 0)
					{
						continue;
					}
					int count = controlItem.usages.Count;
					bool flag = false;
					for (int j = 0; j < count; j++)
					{
						if (StringMatches(str, controlItem.usages[j]))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						return false;
					}
				}
			}
			Substring name = parser.current.name;
			if (name.length > 0 && !StringMatches(name, controlItem.name))
			{
				return false;
			}
			return true;
		}

		private static bool StringMatches(Substring str, InternedString matchTo)
		{
			int length = str.length;
			int length2 = matchTo.length;
			string text = matchTo.ToLower();
			int i = 0;
			int j;
			for (j = 0; j < length; j++)
			{
				if (i >= length2)
				{
					break;
				}
				char c = str[j];
				if (c == '*')
				{
					if (j == length - 1)
					{
						return true;
					}
					j++;
					for (c = char.ToLower(str[j]); i < length2 && text[i] != c; i++)
					{
					}
					if (i == length2)
					{
						return false;
					}
				}
				else if (char.ToLower(c) != text[i])
				{
					return false;
				}
				i++;
			}
			if (i == length2)
			{
				return j == length;
			}
			return false;
		}

		public static InputControl TryFindControl(InputControl control, string path, int indexInPath = 0)
		{
			return TryFindControl<InputControl>(control, path, indexInPath);
		}

		public static InputControl[] TryFindControls(InputControl control, string path, int indexInPath = 0)
		{
			InputControlList<InputControl> matches = new InputControlList<InputControl>(Allocator.Temp);
			try
			{
				TryFindControls(control, path, indexInPath, ref matches);
				return matches.ToArray();
			}
			finally
			{
				matches.Dispose();
			}
		}

		public static int TryFindControls(InputControl control, string path, ref InputControlList<InputControl> matches, int indexInPath = 0)
		{
			return TryFindControls(control, path, indexInPath, ref matches);
		}

		public static TControl TryFindControl<TControl>(InputControl control, string path, int indexInPath = 0) where TControl : InputControl
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (string.IsNullOrEmpty(path))
			{
				return null;
			}
			if (indexInPath == 0 && path[0] == '/')
			{
				indexInPath++;
			}
			InputControlList<TControl> matches = default(InputControlList<TControl>);
			return MatchControlsRecursive(control, path, indexInPath, ref matches, false);
		}

		public static int TryFindControls<TControl>(InputControl control, string path, int indexInPath, ref InputControlList<TControl> matches) where TControl : InputControl
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (indexInPath == 0 && path[0] == '/')
			{
				indexInPath++;
			}
			int count = matches.Count;
			MatchControlsRecursive(control, path, indexInPath, ref matches, true);
			return matches.Count - count;
		}

		public static InputControl TryFindChild(InputControl control, string path, int indexInPath = 0)
		{
			return TryFindChild<InputControl>(control, path, indexInPath);
		}

		public static TControl TryFindChild<TControl>(InputControl control, string path, int indexInPath = 0) where TControl : InputControl
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			ReadOnlyArray<InputControl> children = control.children;
			int count = children.Count;
			for (int i = 0; i < count; i++)
			{
				TControl val = TryFindControl<TControl>(children[i], path, indexInPath);
				if (val != null)
				{
					return val;
				}
			}
			return null;
		}

		public static bool Matches(string expected, InputControl control)
		{
			if (string.IsNullOrEmpty(expected))
			{
				throw new ArgumentNullException("expected");
			}
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			PathParser parser = new PathParser(expected);
			return MatchesRecursive(ref parser, control);
		}

		public static bool MatchesPrefix(string expected, InputControl control)
		{
			if (string.IsNullOrEmpty(expected))
			{
				throw new ArgumentNullException("expected");
			}
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			for (InputControl inputControl = control; inputControl != null; inputControl = inputControl.parent)
			{
				PathParser parser = new PathParser(expected);
				if (MatchesRecursive(ref parser, inputControl) && parser.isAtEnd)
				{
					return true;
				}
			}
			return false;
		}

		private static bool MatchesRecursive(ref PathParser parser, InputControl currentControl)
		{
			InputControl parent = currentControl.parent;
			if (parent != null && !MatchesRecursive(ref parser, parent))
			{
				return false;
			}
			if (!parser.MoveToNextComponent())
			{
				return false;
			}
			return parser.current.Matches(currentControl);
		}

		private static TControl MatchControlsRecursive<TControl>(InputControl control, string path, int indexInPath, ref InputControlList<TControl> matches, bool matchMultiple) where TControl : InputControl
		{
			int length = path.Length;
			bool flag = true;
			if (path[indexInPath] == '<')
			{
				indexInPath++;
				flag = MatchPathComponent(control.layout, path, ref indexInPath, PathComponentType.Layout);
				if (!flag)
				{
					InternedString value = control.m_Layout;
					while (InputControlLayout.s_Layouts.baseLayoutTable.TryGetValue(value, out value))
					{
						flag = MatchPathComponent(value, path, ref indexInPath, PathComponentType.Layout);
						if (flag)
						{
							break;
						}
					}
				}
			}
			while (indexInPath < length && path[indexInPath] == '{' && flag)
			{
				indexInPath++;
				for (int i = 0; i < control.usages.Count; i++)
				{
					flag = MatchPathComponent(control.usages[i], path, ref indexInPath, PathComponentType.Usage);
					if (flag)
					{
						break;
					}
				}
			}
			if (indexInPath < length - 1 && flag && path[indexInPath] == '#' && path[indexInPath + 1] == '(')
			{
				indexInPath += 2;
				flag = MatchPathComponent(control.displayName, path, ref indexInPath, PathComponentType.DisplayName);
			}
			if (indexInPath < length && flag && path[indexInPath] != '/')
			{
				flag = MatchPathComponent(control.name, path, ref indexInPath, PathComponentType.Name);
				if (!flag)
				{
					for (int j = 0; j < control.aliases.Count; j++)
					{
						if (flag)
						{
							break;
						}
						flag = MatchPathComponent(control.aliases[j], path, ref indexInPath, PathComponentType.Name);
					}
				}
			}
			if (flag)
			{
				if (indexInPath < length && path[indexInPath] == '*')
				{
					indexInPath++;
				}
				if (indexInPath == length)
				{
					TControl val;
					if ((val = control as TControl) == null)
					{
						return null;
					}
					if (matchMultiple)
					{
						matches.Add(val);
					}
					return val;
				}
				if (path[indexInPath] == '/')
				{
					indexInPath++;
					if (indexInPath == length)
					{
						TControl val2;
						if ((val2 = control as TControl) == null)
						{
							return null;
						}
						if (matchMultiple)
						{
							matches.Add(val2);
						}
						return val2;
					}
					if (path[indexInPath] == '{')
					{
						if (control.device != control)
						{
							throw new NotImplementedException("Matching usages inside subcontrols instead of at device root");
						}
						return MatchByUsageAtDeviceRootRecursive(control.device, path, indexInPath, ref matches, matchMultiple);
					}
					return MatchChildrenRecursive(control, path, indexInPath, ref matches, matchMultiple);
				}
			}
			return null;
		}

		private static TControl MatchByUsageAtDeviceRootRecursive<TControl>(InputDevice device, string path, int indexInPath, ref InputControlList<TControl> matches, bool matchMultiple) where TControl : InputControl
		{
			InternedString[] usagesForEachControl = device.m_UsagesForEachControl;
			if (usagesForEachControl == null)
			{
				return null;
			}
			int num = device.m_UsageToControl.Length;
			int num2 = indexInPath + 1;
			bool flag = PathComponentCanYieldMultipleMatches(path, indexInPath);
			int length = path.Length;
			indexInPath++;
			if (indexInPath == length)
			{
				throw new ArgumentException("Invalid path spec '" + path + "'; trailing '{'", "path");
			}
			TControl val = null;
			for (int i = 0; i < num; i++)
			{
				if (!MatchPathComponent(usagesForEachControl[i], path, ref indexInPath, PathComponentType.Usage))
				{
					indexInPath = num2;
					continue;
				}
				InputControl inputControl = device.m_UsageToControl[i];
				if (indexInPath < length && path[indexInPath] == '/')
				{
					val = MatchChildrenRecursive(inputControl, path, indexInPath + 1, ref matches, matchMultiple);
					if ((val != null && !flag) || (val != null && !matchMultiple))
					{
						break;
					}
					continue;
				}
				val = inputControl as TControl;
				if (val != null)
				{
					if (!matchMultiple)
					{
						break;
					}
					matches.Add(val);
				}
			}
			return val;
		}

		private static TControl MatchChildrenRecursive<TControl>(InputControl control, string path, int indexInPath, ref InputControlList<TControl> matches, bool matchMultiple) where TControl : InputControl
		{
			ReadOnlyArray<InputControl> children = control.children;
			int count = children.Count;
			TControl result = null;
			bool flag = PathComponentCanYieldMultipleMatches(path, indexInPath);
			for (int i = 0; i < count; i++)
			{
				TControl val = MatchControlsRecursive(children[i], path, indexInPath, ref matches, matchMultiple);
				if (val != null)
				{
					if (!flag)
					{
						return val;
					}
					if (!matchMultiple)
					{
						return val;
					}
					result = val;
				}
			}
			return result;
		}

		private static bool MatchPathComponent(string component, string path, ref int indexInPath, PathComponentType componentType, int startIndexInComponent = 0)
		{
			int length = component.Length;
			int length2 = path.Length;
			int num = indexInPath;
			int num2 = startIndexInComponent;
			while (indexInPath < length2)
			{
				char c = path[indexInPath];
				if (c == '\\' && indexInPath + 1 < length2)
				{
					indexInPath++;
					c = path[indexInPath];
				}
				else
				{
					if (c == '/')
					{
						break;
					}
					if ((c == '>' && componentType == PathComponentType.Layout) || (c == '}' && componentType == PathComponentType.Usage) || (c == ')' && componentType == PathComponentType.DisplayName))
					{
						indexInPath++;
						break;
					}
					if (c == '*')
					{
						int indexInPath2 = indexInPath + 1;
						if (indexInPath < length2 - 1 && num2 < length && MatchPathComponent(component, path, ref indexInPath2, componentType, num2))
						{
							indexInPath = indexInPath2;
							return true;
						}
						if (num2 < length)
						{
							num2++;
							continue;
						}
						return true;
					}
				}
				if (num2 == length)
				{
					indexInPath = num;
					return false;
				}
				if (char.ToLower(component[num2]) == char.ToLower(c))
				{
					num2++;
					indexInPath++;
					continue;
				}
				indexInPath = num;
				return false;
			}
			if (num2 == length)
			{
				return true;
			}
			indexInPath = num;
			return false;
		}

		private static bool PathComponentCanYieldMultipleMatches(string path, int indexInPath)
		{
			int num = path.IndexOf('/', indexInPath);
			if (num == -1)
			{
				if (path.IndexOf('*', indexInPath) == -1)
				{
					return path.IndexOf('<', indexInPath) != -1;
				}
				return true;
			}
			int count = num - indexInPath;
			if (path.IndexOf('*', indexInPath, count) == -1)
			{
				return path.IndexOf('<', indexInPath, count) != -1;
			}
			return true;
		}
	}
}
