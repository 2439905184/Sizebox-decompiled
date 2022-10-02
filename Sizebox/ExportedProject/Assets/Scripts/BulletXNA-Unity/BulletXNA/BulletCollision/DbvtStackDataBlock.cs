using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class DbvtStackDataBlock : IDisposable
	{
		public ObjectArray<DbvtNode> stack = new ObjectArray<DbvtNode>();

		public bool[] signs = new bool[3];

		public IndexedVector3[] bounds = new IndexedVector3[2];

		public void Dispose()
		{
			BulletGlobals.DbvtStackDataBlockPool.Free(this);
		}
	}
}
