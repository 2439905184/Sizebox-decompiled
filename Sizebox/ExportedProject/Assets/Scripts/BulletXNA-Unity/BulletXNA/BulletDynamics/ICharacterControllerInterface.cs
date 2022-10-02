using BulletXNA.BulletCollision;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletDynamics
{
	public interface ICharacterControllerInterface : IActionInterface
	{
		void SetWalkDirection(ref IndexedVector3 walkDirection);

		void SetVelocityForTimeInterval(ref IndexedVector3 velocity, float timeInterval);

		void Reset();

		void Warp(ref IndexedVector3 origin);

		void PreStep(CollisionWorld collisionWorld);

		void PlayerStep(CollisionWorld collisionWorld, float dt);

		bool CanJump();

		void Jump();

		bool OnGround();
	}
}
