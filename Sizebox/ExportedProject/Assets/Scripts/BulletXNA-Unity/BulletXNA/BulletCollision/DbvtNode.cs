namespace BulletXNA.BulletCollision
{
	public class DbvtNode
	{
		public DbvtAabbMm volume;

		public DbvtNode parent;

		public DbvtNode[] _children = new DbvtNode[2];

		public object data;

		public int dataAsInt;

		public int id;

		public static int counter;

		public DbvtNode()
		{
			id = counter++;
		}

		public DbvtNode(Dbvt tree, DbvtNode aparent, ref DbvtAabbMm avolume, object adata)
			: this()
		{
			volume = avolume;
			parent = aparent;
			data = adata;
			if (data is int)
			{
				dataAsInt = (int)data;
			}
		}

		public void Reset()
		{
			parent = null;
			data = null;
			dataAsInt = 0;
			id = counter++;
		}

		public bool IsLeaf()
		{
			return _children[1] == null;
		}

		public bool IsInternal()
		{
			return !IsLeaf();
		}
	}
}
