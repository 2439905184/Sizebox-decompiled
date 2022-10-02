using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class ThreadSafeEnumerator<T> : IEnumerator<T>, IEnumerator, IDisposable
{
	private IEnumerator<T> _enumerator;

	private readonly object _lock;

	private readonly object _ownerLock;

	private bool _hasBeenDisposed;

	public T Current
	{
		get
		{
			lock (_lock)
			{
				return _enumerator.Current;
			}
		}
	}

	object IEnumerator.Current
	{
		get
		{
			lock (_lock)
			{
				return _enumerator.Current;
			}
		}
	}

	public ThreadSafeEnumerator(IEnumerator<T> enumerator, object ownerLock)
	{
		_enumerator = enumerator;
		_ownerLock = ownerLock;
		_lock = new object();
		_hasBeenDisposed = false;
		Monitor.Enter(_ownerLock);
	}

	public void Dispose()
	{
		lock (_lock)
		{
			_enumerator.Dispose();
			if (!_hasBeenDisposed)
			{
				Monitor.Exit(_ownerLock);
				_hasBeenDisposed = true;
			}
		}
	}

	public bool MoveNext()
	{
		lock (_lock)
		{
			return _enumerator.MoveNext();
		}
	}

	public void Reset()
	{
		lock (_lock)
		{
			_enumerator.Reset();
		}
	}
}
