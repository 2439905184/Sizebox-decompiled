using BulletXNA.LinearMath;

namespace BulletXNA
{
	public interface IMotionState
	{
		void GetWorldTransform(out IndexedMatrix worldTrans);

		void SetWorldTransform(IndexedMatrix worldTrans);

		void SetWorldTransform(ref IndexedMatrix worldTrans);
	}
}
