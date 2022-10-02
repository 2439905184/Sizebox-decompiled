namespace MoonSharp.Interpreter.CoreLib.StringLib
{
	internal class StringRange
	{
		public int Start { get; set; }

		public int End { get; set; }

		public StringRange()
		{
			Start = 0;
			End = 0;
		}

		public StringRange(int start, int end)
		{
			Start = start;
			End = end;
		}

		public static StringRange FromLuaRange(DynValue start, DynValue end, int? defaultEnd = null)
		{
			int num = (start.IsNil() ? 1 : ((int)start.Number));
			int end2 = ((!end.IsNil()) ? ((int)end.Number) : (defaultEnd ?? num));
			return new StringRange(num, end2);
		}

		public string ApplyToString(string value)
		{
			int num = ((Start < 0) ? (Start + value.Length + 1) : Start);
			int num2 = ((End < 0) ? (End + value.Length + 1) : End);
			if (num < 1)
			{
				num = 1;
			}
			if (num2 > value.Length)
			{
				num2 = value.Length;
			}
			if (num > num2)
			{
				return string.Empty;
			}
			return value.Substring(num - 1, num2 - num + 1);
		}

		public int Length()
		{
			return End - Start + 1;
		}
	}
}
