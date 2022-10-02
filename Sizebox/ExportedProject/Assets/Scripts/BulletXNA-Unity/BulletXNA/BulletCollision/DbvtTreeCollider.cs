namespace BulletXNA.BulletCollision
{
	public class DbvtTreeCollider : ICollide
	{
		public DbvtBroadphase pbp;

		public DbvtProxy proxy;

		public DbvtTreeCollider()
		{
		}

		public DbvtTreeCollider(DbvtBroadphase p)
		{
			pbp = p;
			proxy = null;
		}

		public void Initialize(DbvtBroadphase p)
		{
			pbp = p;
			proxy = null;
		}

		public void Process(DbvtNode na, DbvtNode nb)
		{
			if (na != nb)
			{
				DbvtProxy proxy = na.data as DbvtProxy;
				DbvtProxy proxy2 = nb.data as DbvtProxy;
				pbp.m_paircache.AddOverlappingPair(proxy, proxy2);
				pbp.m_newpairs++;
			}
		}

		public void Process(DbvtNode n)
		{
			Process(n, proxy.leaf);
		}

		public void Process(DbvtNode n, float f)
		{
			Process(n);
		}

		public bool Descent(DbvtNode n)
		{
			return true;
		}

		public bool AllLeaves(DbvtNode n)
		{
			return true;
		}
	}
}
