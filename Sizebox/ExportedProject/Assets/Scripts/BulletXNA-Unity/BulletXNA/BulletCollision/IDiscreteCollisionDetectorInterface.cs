using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public interface IDiscreteCollisionDetectorInterface
	{
		void GetClosestPoints(ref ClosestPointInput input, IDiscreteCollisionDetectorInterfaceResult output, IDebugDraw debugDraw, bool swapResults);
	}
}
