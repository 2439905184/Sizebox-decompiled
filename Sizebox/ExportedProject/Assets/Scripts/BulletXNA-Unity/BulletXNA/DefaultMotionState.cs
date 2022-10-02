using BulletXNA.LinearMath;

namespace BulletXNA
{
	public class DefaultMotionState : IMotionState
	{
		public IndexedMatrix m_graphicsWorldTrans;

		public IndexedMatrix m_centerOfMassOffset;

		public IndexedMatrix m_startWorldTrans;

		public object m_userPointer;

		public DefaultMotionState()
			: this(IndexedMatrix.Identity, IndexedMatrix.Identity)
		{
		}

		public DefaultMotionState(IndexedMatrix startTrans, IndexedMatrix centerOfMassOffset)
		{
			m_graphicsWorldTrans = startTrans;
			m_startWorldTrans = startTrans;
			m_centerOfMassOffset = centerOfMassOffset;
			m_userPointer = null;
		}

		public virtual void GetWorldTransform(out IndexedMatrix centerOfMassWorldTrans)
		{
			centerOfMassWorldTrans = m_centerOfMassOffset.Inverse() * m_graphicsWorldTrans;
		}

		public virtual void SetWorldTransform(IndexedMatrix centerOfMassWorldTrans)
		{
			SetWorldTransform(ref centerOfMassWorldTrans);
		}

		public virtual void SetWorldTransform(ref IndexedMatrix centerOfMassWorldTrans)
		{
			m_graphicsWorldTrans = centerOfMassWorldTrans * m_centerOfMassOffset;
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
}
