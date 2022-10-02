using System;
using System.Linq;

namespace MoonSharp.Interpreter.Compatibility.Frameworks
{
	internal class FrameworkCurrent : FrameworkClrBase
	{
		public override bool IsDbNull(object o)
		{
			if (o != null)
			{
				return Convert.IsDBNull(o);
			}
			return false;
		}

		public override bool StringContainsChar(string str, char chr)
		{
			return str.Contains(chr);
		}

		public override Type GetInterface(Type type, string name)
		{
			return type.GetInterface(name);
		}
	}
}
