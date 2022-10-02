using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MoonSharp.Interpreter.Compatibility;

namespace MoonSharp.Interpreter.Loaders
{
	public class UnityAssetsScriptLoader : ScriptLoaderBase
	{
		private Dictionary<string, string> m_Resources = new Dictionary<string, string>();

		public const string DEFAULT_PATH = "MoonSharp/Scripts";

		public UnityAssetsScriptLoader(string assetsPath = null)
		{
			assetsPath = assetsPath ?? "MoonSharp/Scripts";
			LoadResourcesWithReflection(assetsPath);
		}

		public UnityAssetsScriptLoader(Dictionary<string, string> scriptToCodeMap)
		{
			m_Resources = scriptToCodeMap;
		}

		private void LoadResourcesWithReflection(string assetsPath)
		{
			try
			{
				Type type = Type.GetType("UnityEngine.Resources, UnityEngine");
				Type type2 = Type.GetType("UnityEngine.TextAsset, UnityEngine");
				MethodInfo getMethod = Framework.Do.GetGetMethod(Framework.Do.GetProperty(type2, "name"));
				MethodInfo getMethod2 = Framework.Do.GetGetMethod(Framework.Do.GetProperty(type2, "text"));
				Array array = (Array)Framework.Do.GetMethod(type, "LoadAll", new Type[2]
				{
					typeof(string),
					typeof(Type)
				}).Invoke(null, new object[2] { assetsPath, type2 });
				for (int i = 0; i < array.Length; i++)
				{
					object value = array.GetValue(i);
					string key = getMethod.Invoke(value, null) as string;
					string value2 = getMethod2.Invoke(value, null) as string;
					m_Resources.Add(key, value2);
				}
			}
			catch (Exception arg)
			{
				Console.WriteLine("Error initializing UnityScriptLoader : {0}", arg);
			}
		}

		private string GetFileName(string filename)
		{
			int num = Math.Max(filename.LastIndexOf('\\'), filename.LastIndexOf('/'));
			if (num > 0)
			{
				filename = filename.Substring(num + 1);
			}
			return filename;
		}

		public override object LoadFile(string file, Table globalContext)
		{
			file = GetFileName(file);
			if (m_Resources.ContainsKey(file))
			{
				return m_Resources[file];
			}
			throw new Exception(string.Format("Cannot load script '{0}'. By default, scripts should be .txt files placed under a Assets/Resources/{1} directory.\nIf you want scripts to be put in another directory or another way, use a custom instance of UnityAssetsScriptLoader or implement\nyour own IScriptLoader (possibly extending ScriptLoaderBase).", file, "MoonSharp/Scripts"));
		}

		public override bool ScriptFileExists(string file)
		{
			file = GetFileName(file);
			return m_Resources.ContainsKey(file);
		}

		public string[] GetLoadedScripts()
		{
			return m_Resources.Keys.ToArray();
		}
	}
}
