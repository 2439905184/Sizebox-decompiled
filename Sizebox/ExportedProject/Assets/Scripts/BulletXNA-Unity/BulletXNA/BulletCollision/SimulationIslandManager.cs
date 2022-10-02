using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class SimulationIslandManager
	{
		public class PersistentManifoldSortPredicate : IQSComparer<PersistentManifold>
		{
			public bool Compare(PersistentManifold lhs, PersistentManifold rhs)
			{
				return GetIslandId(lhs) < GetIslandId(rhs);
			}
		}

		private UnionFind m_unionFind;

		private PersistentManifoldArray m_islandmanifold;

		private ObjectArray<CollisionObject> m_islandBodies;

		private bool m_splitIslands;

		private PersistentManifoldSortPredicate m_sortPredicate = new PersistentManifoldSortPredicate();

		public SimulationIslandManager()
		{
			m_splitIslands = true;
			m_unionFind = new UnionFind();
			m_islandmanifold = new PersistentManifoldArray();
			m_islandBodies = new ObjectArray<CollisionObject>();
		}

		public virtual void Cleanup()
		{
		}

		public void InitUnionFind(int n)
		{
			m_unionFind.Reset(n);
		}

		public UnionFind GetUnionFind()
		{
			return m_unionFind;
		}

		public void FindUnions(IDispatcher dispatcher, CollisionWorld collisionWorld)
		{
			ObjectArray<BroadphasePair> overlappingPairArray = collisionWorld.GetPairCache().GetOverlappingPairArray();
			int count = overlappingPairArray.Count;
			if (count <= 0)
			{
				return;
			}
			BroadphasePair[] rawArray = overlappingPairArray.GetRawArray();
			for (int i = 0; i < count; i++)
			{
				BroadphasePair broadphasePair = rawArray[i];
				CollisionObject collisionObject = broadphasePair.m_pProxy0.m_clientObject as CollisionObject;
				CollisionObject collisionObject2 = broadphasePair.m_pProxy1.m_clientObject as CollisionObject;
				if (collisionObject != null && collisionObject.MergesSimulationIslands() && collisionObject2 != null && collisionObject2.MergesSimulationIslands())
				{
					m_unionFind.Unite(collisionObject.GetIslandTag(), collisionObject2.GetIslandTag());
				}
			}
		}

		public void UpdateActivationState(CollisionWorld colWorld, IDispatcher dispatcher)
		{
			int n = 0;
			CollisionObject[] rawArray = colWorld.GetCollisionObjectArray().GetRawArray();
			int count = colWorld.GetCollisionObjectArray().Count;
			for (int i = 0; i < count; i++)
			{
				CollisionObject collisionObject = rawArray[i];
				if (!collisionObject.IsStaticOrKinematicObject())
				{
					collisionObject.SetIslandTag(n++);
				}
				collisionObject.SetCompanionId(-1);
				collisionObject.SetHitFraction(1f);
			}
			InitUnionFind(n);
			FindUnions(dispatcher, colWorld);
		}

		public void StoreIslandActivationState(CollisionWorld colWorld)
		{
			int num = 0;
			CollisionObject[] rawArray = colWorld.GetCollisionObjectArray().GetRawArray();
			int count = colWorld.GetCollisionObjectArray().Count;
			for (int i = 0; i < count; i++)
			{
				CollisionObject collisionObject = rawArray[i];
				if (!collisionObject.IsStaticOrKinematicObject())
				{
					collisionObject.SetIslandTag(m_unionFind.Find(num));
					m_unionFind.SetElementSize(num, i);
					collisionObject.SetCompanionId(-1);
					num++;
				}
				else
				{
					collisionObject.SetIslandTag(-1);
					collisionObject.SetCompanionId(-2);
				}
			}
		}

		public void BuildAndProcessIslands(IDispatcher dispatcher, CollisionWorld collisionWorld, IIslandCallback callback)
		{
			ObjectArray<CollisionObject> collisionObjectArray = collisionWorld.GetCollisionObjectArray();
			BuildIslands(dispatcher, collisionWorld);
			int num = 1;
			int numElements = GetUnionFind().GetNumElements();
			BulletGlobals.StartProfile("processIslands");
			if (!m_splitIslands)
			{
				PersistentManifoldArray internalManifoldPointer = dispatcher.GetInternalManifoldPointer();
				int numManifolds = dispatcher.GetNumManifolds();
				callback.ProcessIsland(collisionObjectArray, collisionObjectArray.Count, internalManifoldPointer, 0, numManifolds, -1);
			}
			else
			{
				int count = m_islandmanifold.Count;
				m_islandmanifold.QuickSort(m_sortPredicate);
				int num2 = 0;
				int i = 1;
				for (int num3 = 0; num3 < numElements; num3 = num)
				{
					int id = GetUnionFind().GetElement(num3).m_id;
					bool flag = true;
					for (num = num3; num < numElements && GetUnionFind().GetElement(num).m_id == id; num++)
					{
						int sz = GetUnionFind().GetElement(num).m_sz;
						CollisionObject collisionObject = collisionObjectArray[sz];
						m_islandBodies.Add(collisionObject);
						if (collisionObject.IsActive())
						{
							flag = false;
						}
					}
					int num4 = 0;
					if (num2 < count)
					{
						int islandId = GetIslandId(m_islandmanifold[num2]);
						if (islandId == id)
						{
							PersistentManifold persistentManifold = m_islandmanifold[num2];
							for (i = num2 + 1; i < count && id == GetIslandId(m_islandmanifold[i]); i++)
							{
							}
							num4 = i - num2;
						}
					}
					if (!flag)
					{
						callback.ProcessIsland(m_islandBodies, m_islandBodies.Count, m_islandmanifold, num2, num4, id);
					}
					if (num4 != 0)
					{
						num2 = i;
					}
					m_islandBodies.Clear();
				}
			}
			BulletGlobals.StopProfile();
		}

		public void BuildIslands(IDispatcher dispatcher, CollisionWorld collisionWorld)
		{
			BulletGlobals.StartProfile("islandUnionFindAndQuickSort");
			ObjectArray<CollisionObject> collisionObjectArray = collisionWorld.GetCollisionObjectArray();
			m_islandmanifold.Clear();
			GetUnionFind().sortIslands();
			int numElements = GetUnionFind().GetNumElements();
			int num = 1;
			for (int num2 = 0; num2 < numElements; num2 = num)
			{
				int id = GetUnionFind().GetElement(num2).m_id;
				for (num = num2 + 1; num < numElements && GetUnionFind().GetElement(num).m_id == id; num++)
				{
				}
				bool flag = true;
				for (int i = num2; i < num; i++)
				{
					int sz = GetUnionFind().GetElement(i).m_sz;
					CollisionObject collisionObject = collisionObjectArray[sz];
					if (collisionObject.GetIslandTag() != id)
					{
						collisionObject.GetIslandTag();
						int num3 = -1;
					}
					if (collisionObject.GetIslandTag() == id)
					{
						if (collisionObject.GetActivationState() == ActivationState.ACTIVE_TAG)
						{
							flag = false;
						}
						if (collisionObject.GetActivationState() == ActivationState.DISABLE_DEACTIVATION)
						{
							flag = false;
						}
					}
				}
				if (flag)
				{
					for (int j = num2; j < num; j++)
					{
						int sz2 = GetUnionFind().GetElement(j).m_sz;
						CollisionObject collisionObject2 = collisionObjectArray[sz2];
						int islandTag = collisionObject2.GetIslandTag();
						if (islandTag != id)
						{
							int num4 = -1;
						}
						if (islandTag == id)
						{
							collisionObject2.SetActivationState(ActivationState.ISLAND_SLEEPING);
						}
					}
				}
				else
				{
					for (int k = num2; k < num; k++)
					{
						int sz3 = GetUnionFind().GetElement(k).m_sz;
						CollisionObject collisionObject3 = collisionObjectArray[sz3];
						int islandTag2 = collisionObject3.GetIslandTag();
						if (islandTag2 != id)
						{
							int num5 = -1;
						}
						if (islandTag2 == id && collisionObject3.GetActivationState() == ActivationState.ISLAND_SLEEPING)
						{
							collisionObject3.SetActivationState(ActivationState.WANTS_DEACTIVATION);
							collisionObject3.SetDeactivationTime(0f);
						}
					}
				}
			}
			int numManifolds = dispatcher.GetNumManifolds();
			for (int l = 0; l < numManifolds; l++)
			{
				PersistentManifold manifoldByIndexInternal = dispatcher.GetManifoldByIndexInternal(l);
				CollisionObject collisionObject4 = manifoldByIndexInternal.GetBody0() as CollisionObject;
				CollisionObject collisionObject5 = manifoldByIndexInternal.GetBody1() as CollisionObject;
				if ((collisionObject4 != null && collisionObject4.GetActivationState() != ActivationState.ISLAND_SLEEPING) || (collisionObject5 != null && collisionObject5.GetActivationState() != ActivationState.ISLAND_SLEEPING))
				{
					if (collisionObject4.IsKinematicObject() && collisionObject4.GetActivationState() != ActivationState.ISLAND_SLEEPING && collisionObject4.HasContactResponse())
					{
						collisionObject5.Activate();
					}
					if (collisionObject5.IsKinematicObject() && collisionObject5.GetActivationState() != ActivationState.ISLAND_SLEEPING && collisionObject5.HasContactResponse())
					{
						collisionObject4.Activate();
					}
					if (m_splitIslands && dispatcher.NeedsResponse(collisionObject4, collisionObject5))
					{
						m_islandmanifold.Add(manifoldByIndexInternal);
					}
				}
			}
			BulletGlobals.StopProfile();
		}

		public bool GetSplitIslands()
		{
			return m_splitIslands;
		}

		public void SetSplitIslands(bool doSplitIslands)
		{
			m_splitIslands = doSplitIslands;
		}

		public static int GetIslandId(PersistentManifold lhs)
		{
			CollisionObject collisionObject = lhs.GetBody0() as CollisionObject;
			int islandTag = collisionObject.GetIslandTag();
			if (islandTag >= 0)
			{
				return islandTag;
			}
			CollisionObject collisionObject2 = lhs.GetBody1() as CollisionObject;
			return collisionObject2.GetIslandTag();
		}
	}
}
