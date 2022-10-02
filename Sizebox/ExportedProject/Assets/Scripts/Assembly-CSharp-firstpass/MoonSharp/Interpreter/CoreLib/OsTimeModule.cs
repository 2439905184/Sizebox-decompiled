using System;
using System.Collections.Generic;
using System.Text;

namespace MoonSharp.Interpreter.CoreLib
{
	[MoonSharpModule(Namespace = "os")]
	public class OsTimeModule
	{
		private static DateTime Time0 = DateTime.UtcNow;

		private static DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		private static DynValue GetUnixTime(DateTime dateTime, DateTime? epoch = null)
		{
			double totalSeconds = (dateTime - (epoch ?? Epoch)).TotalSeconds;
			if (totalSeconds < 0.0)
			{
				return DynValue.Nil;
			}
			return DynValue.NewNumber(totalSeconds);
		}

		private static DateTime FromUnixTime(double unixtime)
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds(unixtime);
			return Epoch + timeSpan;
		}

		[MoonSharpModuleMethod]
		public static DynValue clock(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return GetUnixTime(DateTime.UtcNow, Time0);
		}

		[MoonSharpModuleMethod]
		public static DynValue difftime(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue dynValue = args.AsType(0, "difftime", DataType.Number);
			DynValue dynValue2 = args.AsType(1, "difftime", DataType.Number, true);
			if (dynValue2.IsNil())
			{
				return DynValue.NewNumber(dynValue.Number);
			}
			return DynValue.NewNumber(dynValue.Number - dynValue2.Number);
		}

		[MoonSharpModuleMethod]
		public static DynValue time(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DateTime dateTime = DateTime.UtcNow;
			if (args.Count > 0)
			{
				DynValue dynValue = args.AsType(0, "time", DataType.Table, true);
				if (dynValue.Type == DataType.Table)
				{
					dateTime = ParseTimeTable(dynValue.Table);
				}
			}
			return GetUnixTime(dateTime);
		}

		private static DateTime ParseTimeTable(Table t)
		{
			int second = GetTimeTableField(t, "sec") ?? 0;
			int minute = GetTimeTableField(t, "min") ?? 0;
			int hour = GetTimeTableField(t, "hour") ?? 12;
			int? timeTableField = GetTimeTableField(t, "day");
			int? timeTableField2 = GetTimeTableField(t, "month");
			int? timeTableField3 = GetTimeTableField(t, "year");
			if (!timeTableField.HasValue)
			{
				throw new ScriptRuntimeException("field 'day' missing in date table");
			}
			if (!timeTableField2.HasValue)
			{
				throw new ScriptRuntimeException("field 'month' missing in date table");
			}
			if (!timeTableField3.HasValue)
			{
				throw new ScriptRuntimeException("field 'year' missing in date table");
			}
			return new DateTime(timeTableField3.Value, timeTableField2.Value, timeTableField.Value, hour, minute, second);
		}

		private static int? GetTimeTableField(Table t, string key)
		{
			double? num = t.Get(key).CastToNumber();
			if (num.HasValue)
			{
				return (int)num.Value;
			}
			return null;
		}

		[MoonSharpModuleMethod]
		public static DynValue date(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DateTime dateTime = DateTime.UtcNow;
			DynValue dynValue = args.AsType(0, "date", DataType.String, true);
			DynValue dynValue2 = args.AsType(1, "date", DataType.Number, true);
			string text = (dynValue.IsNil() ? "%c" : dynValue.String);
			if (dynValue2.IsNotNil())
			{
				dateTime = FromUnixTime(dynValue2.Number);
			}
			bool v = false;
			if (text.StartsWith("!"))
			{
				text = text.Substring(1);
			}
			else
			{
				try
				{
					dateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.Local);
					v = dateTime.IsDaylightSavingTime();
				}
				catch (TimeZoneNotFoundException)
				{
				}
			}
			if (text == "*t")
			{
				Table table = new Table(executionContext.GetScript());
				table.Set("year", DynValue.NewNumber(dateTime.Year));
				table.Set("month", DynValue.NewNumber(dateTime.Month));
				table.Set("day", DynValue.NewNumber(dateTime.Day));
				table.Set("hour", DynValue.NewNumber(dateTime.Hour));
				table.Set("min", DynValue.NewNumber(dateTime.Minute));
				table.Set("sec", DynValue.NewNumber(dateTime.Second));
				table.Set("wday", DynValue.NewNumber((double)(dateTime.DayOfWeek + 1)));
				table.Set("yday", DynValue.NewNumber(dateTime.DayOfYear));
				table.Set("isdst", DynValue.NewBoolean(v));
				return DynValue.NewTable(table);
			}
			return DynValue.NewString(StrFTime(text, dateTime));
		}

		private static string StrFTime(string format, DateTime d)
		{
			Dictionary<char, string> dictionary = new Dictionary<char, string>
			{
				{ 'a', "ddd" },
				{ 'A', "dddd" },
				{ 'b', "MMM" },
				{ 'B', "MMMM" },
				{ 'c', "f" },
				{ 'd', "dd" },
				{ 'D', "MM/dd/yy" },
				{ 'F', "yyyy-MM-dd" },
				{ 'g', "yy" },
				{ 'G', "yyyy" },
				{ 'h', "MMM" },
				{ 'H', "HH" },
				{ 'I', "hh" },
				{ 'm', "MM" },
				{ 'M', "mm" },
				{ 'p', "tt" },
				{ 'r', "h:mm:ss tt" },
				{ 'R', "HH:mm" },
				{ 'S', "ss" },
				{ 'T', "HH:mm:ss" },
				{ 'y', "yy" },
				{ 'Y', "yyyy" },
				{ 'x', "d" },
				{ 'X', "T" },
				{ 'z', "zzz" },
				{ 'Z', "zzz" }
			};
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			foreach (char c in format)
			{
				if (c == '%')
				{
					if (flag)
					{
						stringBuilder.Append('%');
						flag = false;
					}
					else
					{
						flag = true;
					}
				}
				else if (!flag)
				{
					stringBuilder.Append(c);
				}
				else
				{
					if (c == 'O' || c == 'E')
					{
						continue;
					}
					flag = false;
					if (dictionary.ContainsKey(c))
					{
						stringBuilder.Append(d.ToString(dictionary[c]));
						continue;
					}
					switch (c)
					{
					case 'e':
					{
						string text = d.ToString("%d");
						if (text.Length < 2)
						{
							text = " " + text;
						}
						stringBuilder.Append(text);
						break;
					}
					case 'n':
						stringBuilder.Append('\n');
						break;
					case 't':
						stringBuilder.Append('\t');
						break;
					case 'C':
						stringBuilder.Append(d.Year / 100);
						break;
					case 'j':
						stringBuilder.Append(d.DayOfYear.ToString("000"));
						break;
					case 'u':
					{
						int num = (int)d.DayOfWeek;
						if (num == 0)
						{
							num = 7;
						}
						stringBuilder.Append(num);
						break;
					}
					case 'w':
					{
						int dayOfWeek = (int)d.DayOfWeek;
						stringBuilder.Append(dayOfWeek);
						break;
					}
					case 'U':
						stringBuilder.Append("??");
						break;
					case 'V':
						stringBuilder.Append("??");
						break;
					case 'W':
						stringBuilder.Append("??");
						break;
					default:
						throw new ScriptRuntimeException("bad argument #1 to 'date' (invalid conversion specifier '{0}')", format);
					}
				}
			}
			return stringBuilder.ToString();
		}
	}
}
