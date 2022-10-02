using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MoonSharp.Interpreter.Loaders
{
	public class EmbeddedResourcesScriptLoader : ScriptLoaderBase
	{
		private Assembly m_ResourceAssembly;

		private HashSet<string> m_ResourceNames;

		private string m_Namespace;

		public EmbeddedResourcesScriptLoader(Assembly resourceAssembly = null)
		{
			if (resourceAssembly == null)
			{
				resourceAssembly = Assembly.GetCallingAssembly();
			}
			m_ResourceAssembly = resourceAssembly;
			m_Namespace = m_ResourceAssembly.FullName.Split(',').First();
			m_ResourceNames = new HashSet<string>(m_ResourceAssembly.GetManifestResourceNames());
		}

		private string FileNameToResource(string file)
		{
			file = file.Replace('/', '.');
			file = file.Replace('\\', '.');
			return m_Namespace + "." + file;
		}

		public override bool ScriptFileExists(string name)
		{
			name = FileNameToResource(name);
			return m_ResourceNames.Contains(name);
		}

		public override object LoadFile(string file, Table globalContext)
		{
			file = FileNameToResource(file);
			return m_ResourceAssembly.GetManifestResourceStream(file);
		}
	}
}
