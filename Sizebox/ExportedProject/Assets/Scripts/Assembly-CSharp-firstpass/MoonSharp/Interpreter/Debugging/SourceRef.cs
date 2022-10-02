using System;

namespace MoonSharp.Interpreter.Debugging
{
	public class SourceRef
	{
		public bool Breakpoint;

		public bool IsClrLocation { get; private set; }

		public int SourceIdx { get; private set; }

		public int FromChar { get; private set; }

		public int ToChar { get; private set; }

		public int FromLine { get; private set; }

		public int ToLine { get; private set; }

		public bool IsStepStop { get; private set; }

		public bool CannotBreakpoint { get; private set; }

		internal static SourceRef GetClrLocation()
		{
			return new SourceRef(0, 0, 0, 0, 0, false)
			{
				IsClrLocation = true
			};
		}

		public SourceRef(SourceRef src, bool isStepStop)
		{
			SourceIdx = src.SourceIdx;
			FromChar = src.FromChar;
			ToChar = src.ToChar;
			FromLine = src.FromLine;
			ToLine = src.ToLine;
			IsStepStop = isStepStop;
		}

		public SourceRef(int sourceIdx, int from, int to, int fromline, int toline, bool isStepStop)
		{
			SourceIdx = sourceIdx;
			FromChar = from;
			ToChar = to;
			FromLine = fromline;
			ToLine = toline;
			IsStepStop = isStepStop;
		}

		public override string ToString()
		{
			return string.Format("[{0}]{1} ({2}, {3}) -> ({4}, {5})", SourceIdx, IsStepStop ? "*" : " ", FromLine, FromChar, ToLine, ToChar);
		}

		internal int GetLocationDistance(int sourceIdx, int line, int col)
		{
			if (sourceIdx != SourceIdx)
			{
				return int.MaxValue;
			}
			if (FromLine == ToLine)
			{
				if (line == FromLine)
				{
					if (col >= FromChar && col <= ToChar)
					{
						return 0;
					}
					if (col < FromChar)
					{
						return FromChar - col;
					}
					return col - ToChar;
				}
				return Math.Abs(line - FromLine) * 1600;
			}
			if (line == FromLine)
			{
				if (col < FromChar)
				{
					return FromChar - col;
				}
				return 0;
			}
			if (line == ToLine)
			{
				if (col > ToChar)
				{
					return col - ToChar;
				}
				return 0;
			}
			if (line > FromLine && line < ToLine)
			{
				return 0;
			}
			if (line < FromLine)
			{
				return (FromLine - line) * 1600;
			}
			return (line - ToLine) * 1600;
		}

		public bool IncludesLocation(int sourceIdx, int line, int col)
		{
			if (sourceIdx != SourceIdx || line < FromLine || line > ToLine)
			{
				return false;
			}
			if (FromLine == ToLine)
			{
				if (col >= FromChar)
				{
					return col <= ToChar;
				}
				return false;
			}
			if (line == FromLine)
			{
				return col >= FromChar;
			}
			if (line == ToLine)
			{
				return col <= ToChar;
			}
			return true;
		}

		public SourceRef SetNoBreakPoint()
		{
			CannotBreakpoint = true;
			return this;
		}

		public string FormatLocation(Script script, bool forceClassicFormat = false)
		{
			SourceCode sourceCode = script.GetSourceCode(SourceIdx);
			if (IsClrLocation)
			{
				return "[clr]";
			}
			if (script.Options.UseLuaErrorLocations || forceClassicFormat)
			{
				return string.Format("{0}:{1}", sourceCode.Name, FromLine);
			}
			if (FromLine == ToLine)
			{
				if (FromChar == ToChar)
				{
					return string.Format("{0}:({1},{2})", sourceCode.Name, FromLine, FromChar, ToLine, ToChar);
				}
				return string.Format("{0}:({1},{2}-{4})", sourceCode.Name, FromLine, FromChar, ToLine, ToChar);
			}
			return string.Format("{0}:({1},{2}-{3},{4})", sourceCode.Name, FromLine, FromChar, ToLine, ToChar);
		}
	}
}
