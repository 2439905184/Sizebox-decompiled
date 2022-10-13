using BulletXNA;
using BulletXNA.LinearMath;

namespace MMD4MecanimInternal.Bullet;

public class KinematicMotionState : IMotionState
{
	public IndexedMatrix m_graphicsWorldTrans;

	public KinematicMotionState()
	{
		m_graphicsWorldTrans = IndexedMatrix.Identity;
	}

	public KinematicMotionState(ref IndexedMatrix startTrans)
	{
		m_graphicsWorldTrans = startTrans;
	}

	public virtual void GetWorldTransform(out IndexedMatrix centerOfMassWorldTrans)
	{
		centerOfMassWorldTrans = m_graphicsWorldTrans;
	}

	public virtual void SetWorldTransform(IndexedMatrix centerOfMassWorldTrans)
	{
	}

	public virtual void SetWorldTransform(ref IndexedMatrix centerOfMassWorldTrans)
	{
	}

	public virtual void Rotate(IndexedQuaternion iq)
	{
	}

	public virtual void Translate(IndexedVector3 v)
	{
	}
}
