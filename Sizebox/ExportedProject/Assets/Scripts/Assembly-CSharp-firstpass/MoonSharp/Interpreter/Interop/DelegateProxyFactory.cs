using System;

namespace MoonSharp.Interpreter.Interop
{
	public class DelegateProxyFactory<TProxy, TTarget> : IProxyFactory<TProxy, TTarget>, IProxyFactory where TProxy : class where TTarget : class
	{
		private Func<TTarget, TProxy> wrapDelegate;

		public Type TargetType
		{
			get
			{
				return typeof(TTarget);
			}
		}

		public Type ProxyType
		{
			get
			{
				return typeof(TProxy);
			}
		}

		public DelegateProxyFactory(Func<TTarget, TProxy> wrapDelegate)
		{
			this.wrapDelegate = wrapDelegate;
		}

		public TProxy CreateProxyObject(TTarget target)
		{
			return wrapDelegate(target);
		}

		public object CreateProxyObject(object o)
		{
			return CreateProxyObject((TTarget)o);
		}
	}
}
