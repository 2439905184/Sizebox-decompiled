using System;
using MoonSharp.Interpreter.Compatibility;

namespace MoonSharp.Interpreter.Interop
{
	public sealed class ProxyUserDataDescriptor : IUserDataDescriptor
	{
		private IUserDataDescriptor m_ProxyDescriptor;

		private IProxyFactory m_ProxyFactory;

		public IUserDataDescriptor InnerDescriptor
		{
			get
			{
				return m_ProxyDescriptor;
			}
		}

		public string Name { get; private set; }

		public Type Type
		{
			get
			{
				return m_ProxyFactory.TargetType;
			}
		}

		internal ProxyUserDataDescriptor(IProxyFactory proxyFactory, IUserDataDescriptor proxyDescriptor, string friendlyName = null)
		{
			m_ProxyFactory = proxyFactory;
			Name = friendlyName ?? (proxyFactory.TargetType.Name + "::proxy");
			m_ProxyDescriptor = proxyDescriptor;
		}

		private object Proxy(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			return m_ProxyFactory.CreateProxyObject(obj);
		}

		public DynValue Index(Script script, object obj, DynValue index, bool isDirectIndexing)
		{
			return m_ProxyDescriptor.Index(script, Proxy(obj), index, isDirectIndexing);
		}

		public bool SetIndex(Script script, object obj, DynValue index, DynValue value, bool isDirectIndexing)
		{
			return m_ProxyDescriptor.SetIndex(script, Proxy(obj), index, value, isDirectIndexing);
		}

		public string AsString(object obj)
		{
			return m_ProxyDescriptor.AsString(Proxy(obj));
		}

		public DynValue MetaIndex(Script script, object obj, string metaname)
		{
			return m_ProxyDescriptor.MetaIndex(script, Proxy(obj), metaname);
		}

		public bool IsTypeCompatible(Type type, object obj)
		{
			return Framework.Do.IsInstanceOfType(type, obj);
		}
	}
}
