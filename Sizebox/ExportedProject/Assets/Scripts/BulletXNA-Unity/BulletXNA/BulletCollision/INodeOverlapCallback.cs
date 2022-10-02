namespace BulletXNA.BulletCollision
{
	public interface INodeOverlapCallback
	{
		void ProcessNode(int subPart, int triangleIndex);

		void Cleanup();
	}
}
