using BulletXNA.BulletCollision;

namespace BulletXNA.BulletDynamics
{
	public interface IContactSolverFunc
	{
		float ContactSolverFunc(RigidBody body1, RigidBody body2, ManifoldPoint contactPoint, ContactSolverInfo info);
	}
}
