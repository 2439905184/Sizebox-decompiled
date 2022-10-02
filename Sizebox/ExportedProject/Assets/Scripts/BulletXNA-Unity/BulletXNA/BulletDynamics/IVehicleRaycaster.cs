using BulletXNA.LinearMath;

namespace BulletXNA.BulletDynamics
{
	public interface IVehicleRaycaster
	{
		object CastRay(ref IndexedVector3 from, ref IndexedVector3 to, ref VehicleRaycasterResult result);
	}
}
