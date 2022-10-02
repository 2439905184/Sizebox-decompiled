namespace BulletXNA.BulletCollision
{
	public interface IPrimitiveManagerBase
	{
		void Cleanup();

		bool IsTrimesh();

		int GetPrimitiveCount();

		void GetPrimitiveBox(int prim_index, out AABB primbox);

		void GetPrimitiveTriangle(int prim_index, PrimitiveTriangle triangle);
	}
}
