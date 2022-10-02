using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class RayTester : DefaultCollide
	{
		public CollisionObject m_collisionObject;

		public CompoundShape m_compoundShape;

		public IndexedMatrix m_colObjWorldTransform;

		public IndexedMatrix m_rayFromTrans;

		public IndexedMatrix m_rayToTrans;

		public RayResultCallback m_resultCallback;

		public RayTester(CollisionObject collisionObject, CompoundShape compoundShape, ref IndexedMatrix colObjWorldTransform, ref IndexedMatrix rayFromTrans, ref IndexedMatrix rayToTrans, RayResultCallback resultCallback)
		{
			m_collisionObject = collisionObject;
			m_compoundShape = compoundShape;
			m_colObjWorldTransform = colObjWorldTransform;
			m_rayFromTrans = rayFromTrans;
			m_rayToTrans = rayToTrans;
			m_resultCallback = resultCallback;
		}

		public void Process(int i)
		{
			CollisionShape childShape = m_compoundShape.GetChildShape(i);
			IndexedMatrix childTransform = m_compoundShape.GetChildTransform(i);
			IndexedMatrix colObjWorldTransform = m_colObjWorldTransform * childTransform;
			CollisionShape collisionShape = m_collisionObject.GetCollisionShape();
			m_collisionObject.InternalSetTemporaryCollisionShape(childShape);
			LocalInfoAdder2 resultCallback = new LocalInfoAdder2(i, m_resultCallback);
			CollisionWorld.RayTestSingle(ref m_rayFromTrans, ref m_rayToTrans, m_collisionObject, childShape, ref colObjWorldTransform, resultCallback);
			m_collisionObject.InternalSetTemporaryCollisionShape(collisionShape);
		}

		public override void Process(DbvtNode leaf)
		{
			Process(leaf.dataAsInt);
		}

		public virtual void Cleanup()
		{
		}
	}
}
