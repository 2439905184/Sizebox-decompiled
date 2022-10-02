using System;
using System.IO;
using MoonSharp.Interpreter.Loaders;

namespace MoonSharp.Interpreter
{
	public class ScriptOptions
	{
		public IScriptLoader ScriptLoader { get; set; }

		public Action<string> DebugPrint { get; set; }

		public Func<string, string> DebugInput { get; set; }

		public bool UseLuaErrorLocations { get; set; }

		public ColonOperatorBehaviour ColonOperatorClrCallbackBehaviour { get; set; }

		public Stream Stdin { get; set; }

		public Stream Stdout { get; set; }

		public Stream Stderr { get; set; }

		public int TailCallOptimizationThreshold { get; set; }

		public bool CheckThreadAccess { get; set; }

		internal ScriptOptions()
		{
		}

		internal ScriptOptions(ScriptOptions defaults)
		{
			DebugInput = defaults.DebugInput;
			DebugPrint = defaults.DebugPrint;
			UseLuaErrorLocations = defaults.UseLuaErrorLocations;
			Stdin = defaults.Stdin;
			Stdout = defaults.Stdout;
			Stderr = defaults.Stderr;
			TailCallOptimizationThreshold = defaults.TailCallOptimizationThreshold;
			ScriptLoader = defaults.ScriptLoader;
			CheckThreadAccess = defaults.CheckThreadAccess;
		}
	}
}
