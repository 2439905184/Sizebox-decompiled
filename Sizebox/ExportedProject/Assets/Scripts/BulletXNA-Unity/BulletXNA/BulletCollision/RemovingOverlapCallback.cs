namespace BulletXNA.BulletCollision
{
	public class RemovingOverlapCallback : IOverlapCallback
	{
		public virtual bool ProcessOverlap(BroadphasePair pair)
		{
			return false;
		}
	}
}
