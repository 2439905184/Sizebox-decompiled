using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class CollisionDispatcher : IDispatcher
	{
		private PersistentManifoldArray m_manifoldsPtr = new PersistentManifoldArray();

		private DispatcherFlags m_dispatcherFlags;

		private ManifoldResult m_defaultManifoldResult;

		private INearCallback m_nearCallback;

		private ICollisionConfiguration m_collisionConfiguration;

		private CollisionPairCallback m_collisionCallback = new CollisionPairCallback(null, null);

		public static int gNumManifold;

		private CollisionAlgorithmCreateFunc[,] m_doubleDispatch;

		public CollisionDispatcher(ICollisionConfiguration collisionConfiguration)
		{
			m_collisionConfiguration = collisionConfiguration;
			m_dispatcherFlags = DispatcherFlags.CD_USE_RELATIVE_CONTACT_BREAKING_THRESHOLD;
			SetNearCallback(new DefaultNearCallback());
			m_doubleDispatch = new CollisionAlgorithmCreateFunc[36, 36];
			for (int i = 0; i < 36; i++)
			{
				for (int j = 0; j < 36; j++)
				{
					m_doubleDispatch[i, j] = m_collisionConfiguration.GetCollisionAlgorithmCreateFunc((BroadphaseNativeTypes)i, (BroadphaseNativeTypes)j);
				}
			}
		}

		public DispatcherFlags GetDispatcherFlags()
		{
			return m_dispatcherFlags;
		}

		public void SetDispatcherFlags(DispatcherFlags flags)
		{
			m_dispatcherFlags = flags;
		}

		public virtual void Cleanup()
		{
		}

		public virtual PersistentManifold GetNewManifold(CollisionObject b0, CollisionObject b1)
		{
			gNumManifold++;
			float contactBreakingThreshold = (((m_dispatcherFlags & DispatcherFlags.CD_USE_RELATIVE_CONTACT_BREAKING_THRESHOLD) > (DispatcherFlags)0) ? Math.Min(b0.GetCollisionShape().GetContactBreakingThreshold(BulletGlobals.gContactBreakingThreshold), b1.GetCollisionShape().GetContactBreakingThreshold(BulletGlobals.gContactBreakingThreshold)) : BulletGlobals.gContactBreakingThreshold);
			float contactProcessingThreshold = Math.Min(b0.GetContactProcessingThreshold(), b1.GetContactProcessingThreshold());
			PersistentManifold persistentManifold = BulletGlobals.PersistentManifoldPool.Get();
			persistentManifold.Initialise(b0, b1, 0, contactBreakingThreshold, contactProcessingThreshold);
			persistentManifold.m_index1a = m_manifoldsPtr.Count;
			m_manifoldsPtr.Add(persistentManifold);
			return persistentManifold;
		}

		public virtual void ReleaseManifold(PersistentManifold manifold)
		{
			gNumManifold--;
			ClearManifold(manifold);
			int index1a = manifold.m_index1a;
			m_manifoldsPtr.RemoveAtQuick(index1a);
			m_manifoldsPtr[index1a].m_index1a = index1a;
			BulletGlobals.PersistentManifoldPool.Free(manifold);
		}

		public virtual void ClearManifold(PersistentManifold manifold)
		{
			manifold.ClearManifold();
		}

		public CollisionAlgorithm FindAlgorithm(CollisionObject body0, CollisionObject body1)
		{
			return FindAlgorithm(body0, body1, null);
		}

		public CollisionAlgorithm FindAlgorithm(CollisionObject body0, CollisionObject body1, PersistentManifold sharedManifold)
		{
			CollisionAlgorithmConstructionInfo caci = new CollisionAlgorithmConstructionInfo(this, -1);
			caci.SetManifold(sharedManifold);
			int shapeType = (int)body0.GetCollisionShape().GetShapeType();
			int shapeType2 = (int)body1.GetCollisionShape().GetShapeType();
			return m_doubleDispatch[shapeType, shapeType2].CreateCollisionAlgorithm(caci, body0, body1);
		}

		public virtual bool NeedsCollision(CollisionObject body0, CollisionObject body1)
		{
			bool result = true;
			if (!body0.IsActive() && !body1.IsActive())
			{
				result = false;
			}
			else if (!body0.CheckCollideWith(body1))
			{
				result = false;
			}
			return result;
		}

		public virtual bool NeedsResponse(CollisionObject body0, CollisionObject body1)
		{
			return body0.HasContactResponse() && body1.HasContactResponse() && (!body0.IsStaticOrKinematicObject() || !body1.IsStaticOrKinematicObject());
		}

		public virtual void DispatchAllCollisionPairs(IOverlappingPairCache pairCache, DispatcherInfo dispatchInfo, IDispatcher dispatcher)
		{
			m_collisionCallback.Initialize(dispatchInfo, this);
			pairCache.ProcessAllOverlappingPairs(m_collisionCallback, dispatcher);
			m_collisionCallback.cleanup();
		}

		public void SetNearCallback(INearCallback nearCallback)
		{
			m_nearCallback = nearCallback;
		}

		public INearCallback GetNearCallback()
		{
			return m_nearCallback;
		}

		public void RegisterCollisionCreateFunc(int proxyType0, int proxyType1, CollisionAlgorithmCreateFunc createFunc)
		{
			m_doubleDispatch[proxyType0, proxyType1] = createFunc;
		}

		public int GetNumManifolds()
		{
			return m_manifoldsPtr.Count;
		}

		public PersistentManifold GetManifoldByIndexInternal(int index)
		{
			return m_manifoldsPtr[index];
		}

		public virtual object AllocateCollisionAlgorithm(int size)
		{
			return null;
		}

		public virtual void FreeCollisionAlgorithm(CollisionAlgorithm collisionAlgorithm)
		{
			if (collisionAlgorithm != null)
			{
				collisionAlgorithm.Cleanup();
			}
		}

		public ICollisionConfiguration GetCollisionConfiguration()
		{
			return m_collisionConfiguration;
		}

		public void SetCollisionConfiguration(ICollisionConfiguration config)
		{
			m_collisionConfiguration = config;
		}

		public virtual PersistentManifoldArray GetInternalManifoldPointer()
		{
			return m_manifoldsPtr;
		}

		public ManifoldResult GetNewManifoldResult(CollisionObject o1, CollisionObject o2)
		{
			ManifoldResult manifoldResult = null;
			manifoldResult = BulletGlobals.ManifoldResultPool.Get();
			manifoldResult.Initialise(o1, o2);
			return manifoldResult;
		}

		public void FreeManifoldResult(ManifoldResult result)
		{
			BulletGlobals.ManifoldResultPool.Free(result);
		}
	}
}
