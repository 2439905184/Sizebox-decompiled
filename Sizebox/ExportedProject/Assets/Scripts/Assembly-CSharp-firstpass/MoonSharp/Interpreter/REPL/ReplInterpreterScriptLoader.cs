using System;
using MoonSharp.Interpreter.Loaders;

namespace MoonSharp.Interpreter.REPL
{
	public class ReplInterpreterScriptLoader : FileSystemScriptLoader
	{
		public ReplInterpreterScriptLoader()
		{
			string environmentVariable = Environment.GetEnvironmentVariable("MOONSHARP_PATH");
			if (!string.IsNullOrEmpty(environmentVariable))
			{
				base.ModulePaths = ScriptLoaderBase.UnpackStringPaths(environmentVariable);
			}
			if (base.ModulePaths == null)
			{
				environmentVariable = Environment.GetEnvironmentVariable("LUA_PATH_5_2");
				if (!string.IsNullOrEmpty(environmentVariable))
				{
					base.ModulePaths = ScriptLoaderBase.UnpackStringPaths(environmentVariable);
				}
			}
			if (base.ModulePaths == null)
			{
				environmentVariable = Environment.GetEnvironmentVariable("LUA_PATH");
				if (!string.IsNullOrEmpty(environmentVariable))
				{
					base.ModulePaths = ScriptLoaderBase.UnpackStringPaths(environmentVariable);
				}
			}
			if (base.ModulePaths == null)
			{
				base.ModulePaths = ScriptLoaderBase.UnpackStringPaths("?;?.lua");
			}
		}

		public override string ResolveModuleName(string modname, Table globalContext)
		{
			DynValue dynValue = globalContext.RawGet("LUA_PATH");
			if (dynValue != null && dynValue.Type == DataType.String)
			{
				return ResolveModuleName(modname, ScriptLoaderBase.UnpackStringPaths(dynValue.String));
			}
			return base.ResolveModuleName(modname, globalContext);
		}
	}
}
