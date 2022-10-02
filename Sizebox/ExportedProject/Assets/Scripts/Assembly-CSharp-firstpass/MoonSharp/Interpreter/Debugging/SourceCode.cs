using System;
using System.Collections.Generic;
using System.Text;

namespace MoonSharp.Interpreter.Debugging
{
	public class SourceCode : IScriptPrivateResource
	{
		public string Name { get; private set; }

		public string Code { get; private set; }

		public string[] Lines { get; private set; }

		public Script OwnerScript { get; private set; }

		public int SourceID { get; private set; }

		internal List<SourceRef> Refs { get; private set; }

		internal SourceCode(string name, string code, int sourceID, Script ownerScript)
		{
			Refs = new List<SourceRef>();
			List<string> list = new List<string>();
			Name = name;
			Code = code;
			list.Add(string.Format("-- Begin of chunk : {0} ", name));
			list.AddRange(Code.Split('\n'));
			Lines = list.ToArray();
			OwnerScript = ownerScript;
			SourceID = sourceID;
		}

		public string GetCodeSnippet(SourceRef sourceCodeRef)
		{
			if (sourceCodeRef.FromLine == sourceCodeRef.ToLine)
			{
				int num = AdjustStrIndex(Lines[sourceCodeRef.FromLine], sourceCodeRef.FromChar);
				int num2 = AdjustStrIndex(Lines[sourceCodeRef.FromLine], sourceCodeRef.ToChar);
				return Lines[sourceCodeRef.FromLine].Substring(num, num2 - num);
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = sourceCodeRef.FromLine; i <= sourceCodeRef.ToLine; i++)
			{
				if (i == sourceCodeRef.FromLine)
				{
					int startIndex = AdjustStrIndex(Lines[i], sourceCodeRef.FromChar);
					stringBuilder.Append(Lines[i].Substring(startIndex));
				}
				else if (i == sourceCodeRef.ToLine)
				{
					int num3 = AdjustStrIndex(Lines[i], sourceCodeRef.ToChar);
					stringBuilder.Append(Lines[i].Substring(0, num3 + 1));
				}
				else
				{
					stringBuilder.Append(Lines[i]);
				}
			}
			return stringBuilder.ToString();
		}

		private int AdjustStrIndex(string str, int loc)
		{
			return Math.Max(Math.Min(str.Length, loc), 0);
		}
	}
}
