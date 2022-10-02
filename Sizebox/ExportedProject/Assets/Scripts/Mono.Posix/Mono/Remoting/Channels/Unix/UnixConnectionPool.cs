using System.Collections;
using System.Runtime.Remoting;
using System.Threading;

namespace Mono.Remoting.Channels.Unix
{
	internal class UnixConnectionPool
	{
		private static Hashtable _pools;

		private static int _maxOpenConnections;

		private static int _keepAliveSeconds;

		private static Thread _poolThread;

		public static int MaxOpenConnections
		{
			get
			{
				return _maxOpenConnections;
			}
			set
			{
				if (value < 1)
				{
					throw new RemotingException("MaxOpenConnections must be greater than zero");
				}
				_maxOpenConnections = value;
			}
		}

		public static int KeepAliveSeconds
		{
			get
			{
				return _keepAliveSeconds;
			}
			set
			{
				_keepAliveSeconds = value;
			}
		}

		static UnixConnectionPool()
		{
			_pools = new Hashtable();
			_maxOpenConnections = int.MaxValue;
			_keepAliveSeconds = 15;
			_poolThread = new Thread(ConnectionCollector);
			_poolThread.Start();
			_poolThread.IsBackground = true;
		}

		public static void Shutdown()
		{
			if (_poolThread != null)
			{
				_poolThread.Abort();
			}
		}

		public static UnixConnection GetConnection(string path)
		{
			HostConnectionPool hostConnectionPool;
			lock (_pools)
			{
				hostConnectionPool = (HostConnectionPool)_pools[path];
				if (hostConnectionPool == null)
				{
					hostConnectionPool = new HostConnectionPool(path);
					_pools[path] = hostConnectionPool;
				}
			}
			return hostConnectionPool.GetConnection();
		}

		private static void ConnectionCollector()
		{
			while (true)
			{
				Thread.Sleep(3000);
				lock (_pools)
				{
					foreach (HostConnectionPool value in _pools.Values)
					{
						value.PurgeConnections();
					}
				}
			}
		}
	}
}
