using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class BridgedManifoldResult : ManifoldResult
	{
		private ContactResultCallback m_resultCallback;

		public BridgedManifoldResult(CollisionObject obj0, CollisionObject obj1, ContactResultCallback resultCallback)
			: base(obj0, obj1)
		{
			m_resultCallback = resultCallback;
		}

		public override void AddContactPoint(ref IndexedVector3 normalOnBInWorld, ref IndexedVector3 pointInWorld, float depth)
		{
			bool flag = m_manifoldPtr.GetBody0() != m_body0;
			IndexedVector3 v = pointInWorld + normalOnBInWorld * depth;
			IndexedVector3 o;
			IndexedVector3 o2;
			if (flag)
			{
				MathUtil.InverseTransform(ref m_rootTransB, ref v, out o);
				MathUtil.InverseTransform(ref m_rootTransA, ref pointInWorld, out o2);
			}
			else
			{
				MathUtil.InverseTransform(ref m_rootTransA, ref v, out o);
				MathUtil.InverseTransform(ref m_rootTransB, ref pointInWorld, out o2);
			}
			ManifoldPoint cp = BulletGlobals.ManifoldPointPool.Get();
			cp.Initialise(ref o, ref o2, ref normalOnBInWorld, depth);
			cp.m_positionWorldOnA = v;
			cp.m_positionWorldOnB = pointInWorld;
			if (flag)
			{
				cp.m_partId0 = m_partId1;
				cp.m_partId1 = m_partId0;
				cp.m_index0 = m_index1;
				cp.m_index1 = m_index0;
			}
			else
			{
				cp.m_partId0 = m_partId0;
				cp.m_partId1 = m_partId1;
				cp.m_index0 = m_index0;
				cp.m_index1 = m_index1;
			}
			CollisionObject colObj = (flag ? m_body1 : m_body0);
			CollisionObject colObj2 = (flag ? m_body0 : m_body1);
			m_resultCallback.AddSingleResult(ref cp, colObj, cp.m_partId0, cp.m_index0, colObj2, cp.m_partId1, cp.m_index1);
		}
	}
}
