using System;
using System.Collections.Generic;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class CompoundLeafCallback : DefaultCollide, IDisposable
	{
		public CollisionObject m_compoundColObj;

		public CollisionObject m_otherObj;

		public IDispatcher m_dispatcher;

		public DispatcherInfo m_dispatchInfo;

		public ManifoldResult m_resultOut;

		public IList<CollisionAlgorithm> m_childCollisionAlgorithms;

		public PersistentManifold m_sharedManifold;

		public CompoundCollisionAlgorithm m_parent;

		public CompoundLeafCallback()
		{
		}

		public CompoundLeafCallback(CollisionObject compoundObj, CollisionObject otherObj, IDispatcher dispatcher, DispatcherInfo dispatchInfo, ManifoldResult resultOut, CompoundCollisionAlgorithm parent, IList<CollisionAlgorithm> childCollisionAlgorithms, PersistentManifold sharedManifold)
		{
			m_compoundColObj = compoundObj;
			m_otherObj = otherObj;
			m_dispatcher = dispatcher;
			m_dispatchInfo = dispatchInfo;
			m_resultOut = resultOut;
			m_childCollisionAlgorithms = childCollisionAlgorithms;
			m_sharedManifold = sharedManifold;
			m_parent = parent;
		}

		public void Initialize(CollisionObject compoundObj, CollisionObject otherObj, IDispatcher dispatcher, DispatcherInfo dispatchInfo, ManifoldResult resultOut, CompoundCollisionAlgorithm parent, IList<CollisionAlgorithm> childCollisionAlgorithms, PersistentManifold sharedManifold)
		{
			m_compoundColObj = compoundObj;
			m_otherObj = otherObj;
			m_dispatcher = dispatcher;
			m_dispatchInfo = dispatchInfo;
			m_resultOut = resultOut;
			m_childCollisionAlgorithms = childCollisionAlgorithms;
			m_sharedManifold = sharedManifold;
			m_parent = parent;
		}

		public void ProcessChildShape(CollisionShape childShape, int index)
		{
			CompoundShape compoundShape = (CompoundShape)m_compoundColObj.GetCollisionShape();
			IndexedMatrix worldTrans = m_compoundColObj.GetWorldTransform();
			IndexedMatrix trans = m_compoundColObj.GetInterpolationWorldTransform();
			IndexedMatrix childTransform = compoundShape.GetChildTransform(index);
			IndexedMatrix t = worldTrans * childTransform;
			IndexedVector3 aabbMin;
			IndexedVector3 aabbMax;
			childShape.GetAabb(ref t, out aabbMin, out aabbMax);
			IndexedVector3 aabbMin2;
			IndexedVector3 aabbMax2;
			m_otherObj.GetCollisionShape().GetAabb(m_otherObj.GetWorldTransform(), out aabbMin2, out aabbMax2);
			if (AabbUtil2.TestAabbAgainstAabb2(ref aabbMin, ref aabbMax, ref aabbMin2, ref aabbMax2))
			{
				m_compoundColObj.SetWorldTransform(ref t);
				m_compoundColObj.SetInterpolationWorldTransform(ref t);
				CollisionShape collisionShape = m_compoundColObj.GetCollisionShape();
				m_compoundColObj.InternalSetTemporaryCollisionShape(childShape);
				if (m_childCollisionAlgorithms[index] == null)
				{
					m_childCollisionAlgorithms[index] = m_dispatcher.FindAlgorithm(m_compoundColObj, m_otherObj, m_sharedManifold);
					CollisionAlgorithm collisionAlgorithm = m_childCollisionAlgorithms[index];
					CompoundCollisionAlgorithm parent = m_parent;
				}
				if (m_resultOut.GetBody0Internal() == m_compoundColObj)
				{
					m_resultOut.SetShapeIdentifiersA(-1, index);
				}
				else
				{
					m_resultOut.SetShapeIdentifiersB(-1, index);
				}
				m_childCollisionAlgorithms[index].ProcessCollision(m_compoundColObj, m_otherObj, m_dispatchInfo, m_resultOut);
				if (m_dispatchInfo.getDebugDraw() != null && (m_dispatchInfo.getDebugDraw().GetDebugMode() & DebugDrawModes.DBG_DrawAabb) != 0)
				{
					IndexedVector3 zero = IndexedVector3.Zero;
					IndexedVector3 zero2 = IndexedVector3.Zero;
					m_dispatchInfo.getDebugDraw().DrawAabb(aabbMin, aabbMax, new IndexedVector3(1f, 1f, 1f));
					m_dispatchInfo.getDebugDraw().DrawAabb(aabbMin2, aabbMax2, new IndexedVector3(1f, 1f, 1f));
				}
				m_compoundColObj.InternalSetTemporaryCollisionShape(collisionShape);
				m_compoundColObj.SetWorldTransform(ref worldTrans);
				m_compoundColObj.SetInterpolationWorldTransform(ref trans);
			}
		}

		public override void Process(DbvtNode leaf)
		{
			int dataAsInt = leaf.dataAsInt;
			CompoundShape compoundShape = (CompoundShape)m_compoundColObj.GetCollisionShape();
			CollisionShape childShape = compoundShape.GetChildShape(dataAsInt);
			if (m_dispatchInfo.getDebugDraw() != null && (m_dispatchInfo.getDebugDraw().GetDebugMode() & DebugDrawModes.DBG_DrawAabb) != 0)
			{
				IndexedMatrix trans = m_compoundColObj.GetWorldTransform();
				IndexedVector3 aabbMinOut;
				IndexedVector3 aabbMaxOut;
				AabbUtil2.TransformAabb(leaf.volume.Mins(), leaf.volume.Maxs(), 0f, ref trans, out aabbMinOut, out aabbMaxOut);
				m_dispatchInfo.getDebugDraw().DrawAabb(aabbMinOut, aabbMaxOut, new IndexedVector3(1f, 0f, 0f));
			}
			ProcessChildShape(childShape, dataAsInt);
		}

		public void Dispose()
		{
			BulletGlobals.CompoundLeafCallbackPool.Free(this);
		}
	}
}
