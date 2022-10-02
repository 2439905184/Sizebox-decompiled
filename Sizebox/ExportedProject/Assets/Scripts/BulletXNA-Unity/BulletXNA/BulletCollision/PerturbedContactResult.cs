using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class PerturbedContactResult : ManifoldResult
	{
		public ManifoldResult m_originalManifoldResult;

		public IndexedMatrix m_transformA;

		public IndexedMatrix m_transformB;

		public IndexedMatrix m_unPerturbedTransform;

		public bool m_perturbA;

		public IDebugDraw m_debugDrawer;

		public PerturbedContactResult(ManifoldResult originalResult, ref IndexedMatrix transformA, ref IndexedMatrix transformB, ref IndexedMatrix unPerturbedTransform, bool perturbA, IDebugDraw debugDrawer)
		{
			m_originalManifoldResult = originalResult;
			m_transformA = transformA;
			m_transformB = transformB;
			m_perturbA = perturbA;
			m_unPerturbedTransform = unPerturbedTransform;
			m_debugDrawer = debugDrawer;
		}

		public override void AddContactPoint(ref IndexedVector3 normalOnBInWorld, ref IndexedVector3 pointInWorld, float orgDepth)
		{
			new IndexedVector3(0f, 1f, 0f);
			IndexedVector3 indexedVector2;
			float num;
			IndexedVector3 pointInWorld2;
			if (m_perturbA)
			{
				IndexedVector3 indexedVector = pointInWorld + normalOnBInWorld * orgDepth;
				indexedVector2 = m_unPerturbedTransform * m_transformA.Inverse() * indexedVector;
				num = IndexedVector3.Dot(indexedVector2 - pointInWorld, normalOnBInWorld);
				pointInWorld2 = indexedVector2 + normalOnBInWorld * num;
			}
			else
			{
				indexedVector2 = pointInWorld + normalOnBInWorld * orgDepth;
				pointInWorld2 = m_unPerturbedTransform * m_transformB.Inverse() * pointInWorld;
				num = IndexedVector3.Dot(indexedVector2 - pointInWorld2, normalOnBInWorld);
			}
			m_debugDrawer.DrawLine(pointInWorld2, indexedVector2, new IndexedVector3(1f, 0f, 0f));
			m_debugDrawer.DrawSphere(pointInWorld2, 0.5f, new IndexedVector3(0f, 1f, 0f));
			m_debugDrawer.DrawSphere(indexedVector2, 0.5f, new IndexedVector3(0f, 0f, 1f));
			m_originalManifoldResult.AddContactPoint(ref normalOnBInWorld, ref pointInWorld2, num);
		}
	}
}
