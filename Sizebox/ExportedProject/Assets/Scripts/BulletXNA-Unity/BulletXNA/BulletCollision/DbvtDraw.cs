using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class DbvtDraw : ICollide
	{
		public virtual void Process(DbvtNode n, DbvtNode n2)
		{
		}

		public virtual void Process(DbvtNode n)
		{
			IndexedMatrix trans = IndexedMatrix.Identity;
			IndexedVector3 color = new IndexedVector3(1f, 1f, 1f);
			BulletGlobals.gDebugDraw.DrawBox(ref n.volume._min, ref n.volume._max, ref trans, ref color);
		}

		public virtual void Process(DbvtNode n, float f)
		{
			IndexedMatrix trans = IndexedMatrix.Identity;
			IndexedVector3 color = new IndexedVector3(1f, 1f, 1f);
			BulletGlobals.gDebugDraw.DrawBox(ref n.volume._min, ref n.volume._max, ref trans, ref color);
		}

		public virtual bool Descent(DbvtNode n)
		{
			return true;
		}

		public virtual bool AllLeaves(DbvtNode n)
		{
			return true;
		}
	}
}
