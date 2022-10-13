using BulletXNA;
using BulletXNA.LinearMath;

namespace MMD4MecanimInternal.Bullet;

public class SimpleMotionState : IMotionState
{
	public IndexedMatrix m_graphicsWorldTrans;

	public SimpleMotionState()
		: this(IndexedMatrix.Identity)
	{
	}

	public SimpleMotionState(IndexedMatrix startTrans)
	{
		m_graphicsWorldTrans = startTrans;
	}

	public SimpleMotionState(ref IndexedMatrix startTrans)
	{
		m_graphicsWorldTrans = startTrans;
	}

	public virtual void GetWorldTransform(out IndexedMatrix centerOfMassWorldTrans)
	{
		centerOfMassWorldTrans = m_graphicsWorldTrans;
	}

	public virtual void SetWorldTransform(IndexedMatrix centerOfMassWorldTrans)
	{
		SetWorldTransform(ref centerOfMassWorldTrans);
	}

	public virtual void SetWorldTransform(ref IndexedMatrix centerOfMassWorldTrans)
	{
		m_graphicsWorldTrans = centerOfMassWorldTrans;
	}

	public virtual void Rotate(IndexedQuaternion iq)
	{
		IndexedMatrix centerOfMassWorldTrans = IndexedMatrix.CreateFromQuaternion(iq);
		centerOfMassWorldTrans._origin = m_graphicsWorldTrans._origin;
		SetWorldTransform(ref centerOfMassWorldTrans);
	}

	public virtual void Translate(IndexedVector3 v)
	{
		m_graphicsWorldTrans._origin += v;
	}
}
