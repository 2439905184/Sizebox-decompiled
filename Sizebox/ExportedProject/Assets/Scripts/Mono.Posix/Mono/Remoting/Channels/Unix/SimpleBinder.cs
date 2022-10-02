using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Mono.Remoting.Channels.Unix
{
	internal class SimpleBinder : SerializationBinder
	{
		public static SimpleBinder Instance = new SimpleBinder();

		public override Type BindToType(string assemblyName, string typeName)
		{
			Assembly assembly;
			if (assemblyName.IndexOf(',') != -1)
			{
				try
				{
					assembly = Assembly.Load(assemblyName);
					if (assembly == null)
					{
						return null;
					}
					Type type = assembly.GetType(typeName);
					if (type != null)
					{
						return type;
					}
				}
				catch
				{
				}
			}
			assembly = Assembly.LoadWithPartialName(assemblyName);
			if (assembly == null)
			{
				return null;
			}
			return assembly.GetType(typeName, true);
		}
	}
}
