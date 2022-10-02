using System;

namespace MoonSharp.Interpreter.Interop
{
	public interface IProxyFactory
	{
		Type TargetType { get; }

		Type ProxyType { get; }

		object CreateProxyObject(object o);
	}
	public interface IProxyFactory<TProxy, TTarget> : IProxyFactory where TProxy : class where TTarget : class
	{
		TProxy CreateProxyObject(TTarget target);
	}
}
