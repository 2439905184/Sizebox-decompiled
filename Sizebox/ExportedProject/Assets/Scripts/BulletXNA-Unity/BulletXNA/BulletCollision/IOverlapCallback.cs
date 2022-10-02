namespace BulletXNA.BulletCollision
{
	public interface IOverlapCallback
	{
		bool ProcessOverlap(BroadphasePair pair);
	}
}
