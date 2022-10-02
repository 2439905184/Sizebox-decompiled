namespace BulletXNA.BulletCollision
{
	public interface ICollide
	{
		void Process(DbvtNode n, DbvtNode n2);

		void Process(DbvtNode n);

		void Process(DbvtNode n, float f);

		bool Descent(DbvtNode n);

		bool AllLeaves(DbvtNode n);
	}
}
