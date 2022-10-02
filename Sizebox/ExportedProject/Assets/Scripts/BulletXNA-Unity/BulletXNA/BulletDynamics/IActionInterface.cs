using BulletXNA.BulletCollision;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletDynamics
{
	public interface IActionInterface
	{
		void UpdateAction(CollisionWorld collisionWorld, float deltaTimeStep);

		void DebugDraw(IDebugDraw debugDrawer);
	}
}
