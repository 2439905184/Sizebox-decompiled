using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class SphereBoxCollisionAlgorithm : ActivatingCollisionAlgorithm
	{
		private bool m_ownManifold;

		private PersistentManifold m_manifoldPtr;

		private bool m_isSwapped;

		public SphereBoxCollisionAlgorithm()
		{
		}

		public SphereBoxCollisionAlgorithm(PersistentManifold mf, CollisionAlgorithmConstructionInfo ci, CollisionObject col0, CollisionObject col1, bool isSwapped)
			: base(ci, col0, col1)
		{
			m_isSwapped = isSwapped;
			m_ownManifold = false;
			m_manifoldPtr = mf;
			CollisionObject body = (m_isSwapped ? col1 : col0);
			CollisionObject body2 = (m_isSwapped ? col0 : col1);
			if (m_manifoldPtr == null && m_dispatcher.NeedsCollision(body, body2))
			{
				m_manifoldPtr = m_dispatcher.GetNewManifold(body, body2);
				m_ownManifold = true;
			}
		}

		public void Initialize(PersistentManifold mf, CollisionAlgorithmConstructionInfo ci, CollisionObject col0, CollisionObject col1, bool isSwapped)
		{
			base.Initialize(ci, col0, col1);
			m_isSwapped = isSwapped;
			m_ownManifold = false;
			m_manifoldPtr = mf;
			CollisionObject body = (m_isSwapped ? col1 : col0);
			CollisionObject body2 = (m_isSwapped ? col0 : col1);
			if (m_manifoldPtr == null && m_dispatcher.NeedsCollision(body, body2))
			{
				m_manifoldPtr = m_dispatcher.GetNewManifold(body, body2);
				m_ownManifold = true;
			}
		}

		public override void Cleanup()
		{
			if (m_ownManifold)
			{
				if (m_manifoldPtr != null)
				{
					m_dispatcher.ReleaseManifold(m_manifoldPtr);
					m_manifoldPtr = null;
				}
				m_ownManifold = false;
			}
			m_ownManifold = false;
			BulletGlobals.SphereBoxCollisionAlgorithmPool.Free(this);
		}

		public override void ProcessCollision(CollisionObject body0, CollisionObject body1, DispatcherInfo dispatchInfo, ManifoldResult resultOut)
		{
			if (m_manifoldPtr != null)
			{
				CollisionObject collisionObject = (m_isSwapped ? body1 : body0);
				CollisionObject boxObj = (m_isSwapped ? body0 : body1);
				IndexedVector3 pointOnBox = default(IndexedVector3);
				IndexedVector3 normal = default(IndexedVector3);
				float penetrationDepth = 0f;
				IndexedVector3 origin = collisionObject.GetWorldTransform()._origin;
				SphereShape sphereShape = collisionObject.GetCollisionShape() as SphereShape;
				float radius = sphereShape.GetRadius();
				float contactBreakingThreshold = m_manifoldPtr.GetContactBreakingThreshold();
				resultOut.SetPersistentManifold(m_manifoldPtr);
				if (GetSphereDistance(boxObj, ref pointOnBox, ref normal, ref penetrationDepth, origin, radius, contactBreakingThreshold))
				{
					resultOut.AddContactPoint(normal, pointOnBox, penetrationDepth);
				}
				if (m_ownManifold && m_manifoldPtr.GetNumContacts() > 0)
				{
					resultOut.RefreshContactPoints();
				}
			}
		}

		public override float CalculateTimeOfImpact(CollisionObject body0, CollisionObject body1, DispatcherInfo dispatchInfo, ManifoldResult resultOut)
		{
			return 1f;
		}

		public override void GetAllContactManifolds(PersistentManifoldArray manifoldArray)
		{
			if (m_manifoldPtr != null && m_ownManifold)
			{
				manifoldArray.Add(m_manifoldPtr);
			}
		}

		public bool GetSphereDistance(CollisionObject boxObj, ref IndexedVector3 pointOnBox, ref IndexedVector3 normal, ref float penetrationDepth, IndexedVector3 sphereCenter, float fRadius, float maxContactDistance)
		{
			BoxShape boxShape = boxObj.GetCollisionShape() as BoxShape;
			IndexedVector3 boxHalfExtent = boxShape.GetHalfExtentsWithoutMargin();
			float margin = boxShape.GetMargin();
			penetrationDepth = 1f;
			IndexedMatrix worldTransform = boxObj.GetWorldTransform();
			IndexedVector3 sphereRelPos = worldTransform.InvXform(sphereCenter);
			IndexedVector3 closestPoint = sphereRelPos;
			closestPoint.X = Math.Min(boxHalfExtent.X, closestPoint.X);
			closestPoint.X = Math.Max(0f - boxHalfExtent.X, closestPoint.X);
			closestPoint.Y = Math.Min(boxHalfExtent.Y, closestPoint.Y);
			closestPoint.Y = Math.Max(0f - boxHalfExtent.Y, closestPoint.Y);
			closestPoint.Z = Math.Min(boxHalfExtent.Z, closestPoint.Z);
			closestPoint.Z = Math.Max(0f - boxHalfExtent.Z, closestPoint.Z);
			float num = fRadius + margin;
			float num2 = num + maxContactDistance;
			normal = sphereRelPos - closestPoint;
			float num3 = normal.LengthSquared();
			if (num3 > num2 * num2)
			{
				return false;
			}
			float num4;
			if (num3 == 0f)
			{
				num4 = 0f - GetSpherePenetration(ref boxHalfExtent, ref sphereRelPos, ref closestPoint, ref normal);
			}
			else
			{
				num4 = normal.Length();
				normal /= num4;
			}
			pointOnBox = closestPoint + normal * margin;
			penetrationDepth = num4 - num;
			IndexedVector3 indexedVector = worldTransform * pointOnBox;
			pointOnBox = indexedVector;
			indexedVector = worldTransform._basis * normal;
			normal = indexedVector;
			return true;
		}

		public float GetSpherePenetration(ref IndexedVector3 boxHalfExtent, ref IndexedVector3 sphereRelPos, ref IndexedVector3 closestPoint, ref IndexedVector3 normal)
		{
			float num = boxHalfExtent.X - sphereRelPos.X;
			float num2 = num;
			closestPoint.X = boxHalfExtent.X;
			normal = new IndexedVector3(1f, 0f, 0f);
			num = boxHalfExtent.X + sphereRelPos.X;
			if (num < num2)
			{
				num2 = num;
				closestPoint = sphereRelPos;
				closestPoint.X = 0f - boxHalfExtent.X;
				normal = new IndexedVector3(-1f, 0f, 0f);
			}
			num = boxHalfExtent.Y - sphereRelPos.Y;
			if (num < num2)
			{
				num2 = num;
				closestPoint = sphereRelPos;
				closestPoint.Y = boxHalfExtent.Y;
				normal = new IndexedVector3(0f, 1f, 0f);
			}
			num = boxHalfExtent.Y + sphereRelPos.Y;
			if (num < num2)
			{
				num2 = num;
				closestPoint = sphereRelPos;
				closestPoint.Y = 0f - boxHalfExtent.Y;
				normal = new IndexedVector3(0f, -1f, 0f);
			}
			num = boxHalfExtent.Z - sphereRelPos.Z;
			if (num < num2)
			{
				num2 = num;
				closestPoint = sphereRelPos;
				closestPoint.Z = boxHalfExtent.Z;
				normal = new IndexedVector3(0f, 0f, 1f);
			}
			num = boxHalfExtent.Z + sphereRelPos.Z;
			if (num < num2)
			{
				num2 = num;
				closestPoint = sphereRelPos;
				closestPoint.Z = 0f - boxHalfExtent.Z;
				normal = new IndexedVector3(0f, 0f, -1f);
			}
			return num2;
		}
	}
}
