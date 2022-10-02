using System;
using System.Collections.Generic;
using System.Diagnostics;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class DbvtBroadphase : IBroadphaseInterface
	{
		public const int DYNAMIC_SET = 0;

		public const int FIXED_SET = 1;

		public const int STAGECOUNT = 2;

		public const float DBVT_BP_MARGIN = 0.05f;

		public const int DBVT_BP_PROFILING_RATE = 256;

		public Dbvt[] m_sets = new Dbvt[2];

		public DbvtProxy[] m_stageRoots = new DbvtProxy[3];

		public IOverlappingPairCache m_paircache;

		public float m_prediction;

		public int m_stageCurrent;

		public int m_fupdates;

		public int m_dupdates;

		public int m_cupdates;

		public int m_newpairs;

		public int m_fixedleft;

		public uint m_updates_call;

		public uint m_updates_done;

		public float m_updates_ratio;

		public int m_pid;

		public int m_cid;

		public int m_gid;

		public bool m_releasepaircache;

		public bool m_deferedcollide;

		public bool m_needcleanup;

		public static BroadphaseBenchmarkExperiment[] s_experiments = new BroadphaseBenchmarkExperiment[1]
		{
			new BroadphaseBenchmarkExperiment("1024o.10%", 1024, 10, 0, 8192, 0.005f, 100f)
		};

		public DbvtBroadphase()
			: this(null)
		{
		}

		public DbvtBroadphase(IOverlappingPairCache paircache)
		{
			m_sets[0] = new Dbvt();
			m_sets[1] = new Dbvt();
			m_deferedcollide = false;
			m_needcleanup = true;
			m_releasepaircache = paircache == null;
			m_prediction = 0f;
			m_stageCurrent = 0;
			m_fixedleft = 0;
			m_fupdates = 1;
			m_dupdates = 0;
			m_cupdates = 10;
			m_newpairs = 1;
			m_updates_call = 0u;
			m_updates_done = 0u;
			m_updates_ratio = 0f;
			m_paircache = ((paircache != null) ? paircache : new HashedOverlappingPairCache());
			m_gid = 0;
			m_pid = 0;
			m_cid = 0;
		}

		public virtual void Cleanup()
		{
			if (m_releasepaircache)
			{
				m_paircache.Cleanup();
				m_paircache = null;
			}
		}

		public void Collide(IDispatcher dispatcher)
		{
			BulletGlobals.StartProfile("BroadphaseCollide");
			m_sets[0].OptimizeIncremental(1 + m_sets[0].m_leaves * m_dupdates / 100);
			if (m_fixedleft > 0)
			{
				int num = 1 + m_sets[1].m_leaves * m_fupdates / 100;
				m_sets[1].OptimizeIncremental(1 + m_sets[1].m_leaves * m_fupdates / 100);
				m_fixedleft = Math.Max(0, m_fixedleft - num);
			}
			m_stageCurrent = (m_stageCurrent + 1) % 2;
			DbvtProxy dbvtProxy = m_stageRoots[m_stageCurrent];
			if (dbvtProxy != null)
			{
				DbvtTreeCollider dbvtTreeCollider = BulletGlobals.DbvtTreeColliderPool.Get();
				dbvtTreeCollider.Initialize(this);
				do
				{
					DbvtProxy dbvtProxy2 = dbvtProxy.links[1];
					ListRemove(dbvtProxy, ref m_stageRoots[dbvtProxy.stage]);
					ListAppend(dbvtProxy, ref m_stageRoots[2]);
					m_sets[0].Remove(dbvtProxy.leaf);
					DbvtAabbMm box = DbvtAabbMm.FromMM(ref dbvtProxy.m_aabbMin, ref dbvtProxy.m_aabbMax);
					dbvtProxy.leaf = m_sets[1].Insert(ref box, dbvtProxy);
					dbvtProxy.stage = 2;
					dbvtProxy = dbvtProxy2;
				}
				while (dbvtProxy != null);
				m_fixedleft = m_sets[1].m_leaves;
				BulletGlobals.DbvtTreeColliderPool.Free(dbvtTreeCollider);
				m_needcleanup = true;
			}
			DbvtTreeCollider dbvtTreeCollider2 = BulletGlobals.DbvtTreeColliderPool.Get();
			dbvtTreeCollider2.Initialize(this);
			if (m_deferedcollide)
			{
				Dbvt.CollideTTpersistentStack(m_sets[0].m_root, m_sets[1].m_root, dbvtTreeCollider2);
			}
			if (m_deferedcollide)
			{
				Dbvt.CollideTTpersistentStack(m_sets[0].m_root, m_sets[0].m_root, dbvtTreeCollider2);
			}
			BulletGlobals.DbvtTreeColliderPool.Free(dbvtTreeCollider2);
			if (m_needcleanup)
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				IList<BroadphasePair> overlappingPairArray = m_paircache.GetOverlappingPairArray();
				if (overlappingPairArray.Count > 0)
				{
					int num2 = Math.Min(overlappingPairArray.Count, Math.Max(m_newpairs, overlappingPairArray.Count * m_cupdates / 100));
					for (int i = 0; i < num2; i++)
					{
						BroadphasePair broadphasePair = overlappingPairArray[(m_cid + i) % overlappingPairArray.Count];
						DbvtProxy dbvtProxy3 = broadphasePair.m_pProxy0 as DbvtProxy;
						DbvtProxy dbvtProxy4 = broadphasePair.m_pProxy1 as DbvtProxy;
						if (!DbvtAabbMm.Intersect(ref dbvtProxy3.leaf.volume, ref dbvtProxy4.leaf.volume))
						{
							m_paircache.RemoveOverlappingPair(dbvtProxy3, dbvtProxy4, dispatcher);
							num2--;
							i--;
						}
					}
					if (overlappingPairArray.Count > 0)
					{
						m_cid = (m_cid + num2) % overlappingPairArray.Count;
					}
					else
					{
						m_cid = 0;
					}
				}
				stopwatch.Stop();
			}
			m_pid++;
			m_newpairs = 1;
			m_needcleanup = false;
			if (m_updates_call != 0)
			{
				m_updates_ratio = (float)m_updates_done / (float)m_updates_call;
			}
			else
			{
				m_updates_ratio = 0f;
			}
			m_updates_done /= 2u;
			m_updates_call /= 2u;
			BulletGlobals.StopProfile();
		}

		public void Optimize()
		{
			m_sets[0].OptimizeTopDown();
			m_sets[1].OptimizeTopDown();
		}

		public virtual BroadphaseProxy CreateProxy(IndexedVector3 aabbMin, IndexedVector3 aabbMax, BroadphaseNativeTypes shapeType, object userPtr, CollisionFilterGroups collisionFilterGroup, CollisionFilterGroups collisionFilterMask, IDispatcher dispatcher, object multiSapProxy)
		{
			return CreateProxy(ref aabbMin, ref aabbMax, shapeType, userPtr, collisionFilterGroup, collisionFilterMask, dispatcher, multiSapProxy);
		}

		public virtual BroadphaseProxy CreateProxy(ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax, BroadphaseNativeTypes shapeType, object userPtr, CollisionFilterGroups collisionFilterGroup, CollisionFilterGroups collisionFilterMask, IDispatcher dispatcher, object multiSapProxy)
		{
			DbvtProxy dbvtProxy = new DbvtProxy(ref aabbMin, ref aabbMax, userPtr, collisionFilterGroup, collisionFilterMask);
			DbvtAabbMm box = DbvtAabbMm.FromMM(ref aabbMin, ref aabbMax);
			dbvtProxy.stage = m_stageCurrent;
			dbvtProxy.m_uniqueId = ++m_gid;
			dbvtProxy.leaf = m_sets[0].Insert(ref box, dbvtProxy);
			ListAppend(dbvtProxy, ref m_stageRoots[m_stageCurrent]);
			if (!m_deferedcollide)
			{
				DbvtTreeCollider dbvtTreeCollider = BulletGlobals.DbvtTreeColliderPool.Get();
				dbvtTreeCollider.Initialize(this);
				dbvtTreeCollider.proxy = dbvtProxy;
				Dbvt.CollideTV(m_sets[0].m_root, ref box, dbvtTreeCollider);
				Dbvt.CollideTV(m_sets[1].m_root, ref box, dbvtTreeCollider);
				BulletGlobals.DbvtTreeColliderPool.Free(dbvtTreeCollider);
			}
			return dbvtProxy;
		}

		public virtual void DestroyProxy(BroadphaseProxy absproxy, IDispatcher dispatcher)
		{
			DbvtProxy dbvtProxy = absproxy as DbvtProxy;
			if (dbvtProxy.stage == 2)
			{
				m_sets[1].Remove(dbvtProxy.leaf);
			}
			else
			{
				m_sets[0].Remove(dbvtProxy.leaf);
			}
			ListRemove(dbvtProxy, ref m_stageRoots[dbvtProxy.stage]);
			if (m_paircache != null)
			{
				m_paircache.RemoveOverlappingPairsContainingProxy(dbvtProxy, dispatcher);
			}
			dbvtProxy = null;
			m_needcleanup = true;
		}

		public virtual void SetAabb(BroadphaseProxy absproxy, ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax, IDispatcher dispatcher)
		{
			DbvtProxy dbvtProxy = absproxy as DbvtProxy;
			DbvtAabbMm box = DbvtAabbMm.FromMM(ref aabbMin, ref aabbMax);
			bool flag = false;
			if (dbvtProxy.stage == 2)
			{
				m_sets[1].Remove(dbvtProxy.leaf);
				dbvtProxy.leaf = m_sets[0].Insert(ref box, dbvtProxy);
				flag = true;
			}
			else
			{
				m_updates_call++;
				if (DbvtAabbMm.Intersect(ref dbvtProxy.leaf.volume, ref box))
				{
					IndexedVector3 indexedVector = aabbMin - dbvtProxy.m_aabbMin;
					IndexedVector3 velocity = (dbvtProxy.m_aabbMax - dbvtProxy.m_aabbMin) / 2f * m_prediction;
					if (indexedVector.X < 0f)
					{
						velocity.X = 0f - velocity.X;
					}
					if (indexedVector.Y < 0f)
					{
						velocity.Y = 0f - velocity.Y;
					}
					if (indexedVector.Z < 0f)
					{
						velocity.Z = 0f - velocity.Z;
					}
					if (m_sets[0].Update(dbvtProxy.leaf, ref box, ref velocity, 0.05f))
					{
						m_updates_done++;
						flag = true;
					}
				}
				else
				{
					m_sets[0].Update(dbvtProxy.leaf, ref box);
					m_updates_done++;
					flag = true;
				}
			}
			ListRemove(dbvtProxy, ref m_stageRoots[dbvtProxy.stage]);
			dbvtProxy.m_aabbMin = aabbMin;
			dbvtProxy.m_aabbMax = aabbMax;
			dbvtProxy.stage = m_stageCurrent;
			ListAppend(dbvtProxy, ref m_stageRoots[m_stageCurrent]);
			if (flag)
			{
				m_needcleanup = true;
				if (!m_deferedcollide)
				{
					DbvtTreeCollider dbvtTreeCollider = BulletGlobals.DbvtTreeColliderPool.Get();
					dbvtTreeCollider.Initialize(this);
					Dbvt.CollideTTpersistentStack(m_sets[1].m_root, dbvtProxy.leaf, dbvtTreeCollider);
					Dbvt.CollideTTpersistentStack(m_sets[0].m_root, dbvtProxy.leaf, dbvtTreeCollider);
					BulletGlobals.DbvtTreeColliderPool.Free(dbvtTreeCollider);
				}
			}
		}

		public virtual void AabbTest(ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax, IBroadphaseAabbCallback aabbCallback)
		{
			BroadphaseAabbTester broadphaseAabbTester = new BroadphaseAabbTester(aabbCallback);
			DbvtAabbMm volume = DbvtAabbMm.FromMM(ref aabbMin, ref aabbMax);
			Dbvt.CollideTV(m_sets[0].m_root, ref volume, broadphaseAabbTester);
			Dbvt.CollideTV(m_sets[1].m_root, ref volume, broadphaseAabbTester);
		}

		public virtual void RayTest(ref IndexedVector3 rayFrom, ref IndexedVector3 rayTo, BroadphaseRayCallback rayCallback)
		{
			IndexedVector3 aabbMin = MathUtil.MIN_VECTOR;
			IndexedVector3 aabbMax = MathUtil.MAX_VECTOR;
			RayTest(ref rayFrom, ref rayTo, rayCallback, ref aabbMin, ref aabbMax);
		}

		public void Visualise()
		{
			DbvtDraw collideable = new DbvtDraw();
			Dbvt.EnumNodes(m_sets[0].m_root, collideable);
		}

		public virtual void RayTest(ref IndexedVector3 rayFrom, ref IndexedVector3 rayTo, BroadphaseRayCallback rayCallback, ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax)
		{
			using (BroadphaseRayTester broadphaseRayTester = BulletGlobals.BroadphaseRayTesterPool.Get())
			{
				broadphaseRayTester.Initialize(rayCallback);
				m_sets[0].RayTestInternal(m_sets[0].m_root, ref rayFrom, ref rayTo, ref rayCallback.m_rayDirectionInverse, rayCallback.m_signs, rayCallback.m_lambda_max, ref aabbMin, ref aabbMax, broadphaseRayTester);
				m_sets[1].RayTestInternal(m_sets[1].m_root, ref rayFrom, ref rayTo, ref rayCallback.m_rayDirectionInverse, rayCallback.m_signs, rayCallback.m_lambda_max, ref aabbMin, ref aabbMax, broadphaseRayTester);
			}
		}

		public virtual void GetAabb(BroadphaseProxy absproxy, out IndexedVector3 aabbMin, out IndexedVector3 aabbMax)
		{
			DbvtProxy dbvtProxy = absproxy as DbvtProxy;
			aabbMin = dbvtProxy.GetMinAABB();
			aabbMax = dbvtProxy.GetMaxAABB();
		}

		public void SetAabbForceUpdate(BroadphaseProxy absproxy, ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax, IDispatcher dispatcher)
		{
			DbvtProxy dbvtProxy = absproxy as DbvtProxy;
			DbvtAabbMm box = DbvtAabbMm.FromMM(ref aabbMin, ref aabbMax);
			bool flag = false;
			if (dbvtProxy.stage == 2)
			{
				m_sets[1].Remove(dbvtProxy.leaf);
				dbvtProxy.leaf = m_sets[0].Insert(ref box, dbvtProxy);
				flag = true;
			}
			else
			{
				m_updates_call++;
				m_sets[0].Update(dbvtProxy.leaf, ref box);
				m_updates_done++;
				flag = true;
			}
			ListRemove(dbvtProxy, ref m_stageRoots[dbvtProxy.stage]);
			dbvtProxy.m_aabbMin = aabbMin;
			dbvtProxy.m_aabbMax = aabbMax;
			dbvtProxy.stage = m_stageCurrent;
			ListAppend(dbvtProxy, ref m_stageRoots[m_stageCurrent]);
			if (flag)
			{
				m_needcleanup = true;
				if (!m_deferedcollide)
				{
					DbvtTreeCollider dbvtTreeCollider = BulletGlobals.DbvtTreeColliderPool.Get();
					dbvtTreeCollider.Initialize(this);
					Dbvt.CollideTTpersistentStack(m_sets[1].m_root, dbvtProxy.leaf, dbvtTreeCollider);
					Dbvt.CollideTTpersistentStack(m_sets[0].m_root, dbvtProxy.leaf, dbvtTreeCollider);
					BulletGlobals.DbvtTreeColliderPool.Free(dbvtTreeCollider);
				}
			}
		}

		public virtual void CalculateOverlappingPairs(IDispatcher dispatcher)
		{
			Collide(dispatcher);
			PerformDeferredRemoval(dispatcher);
		}

		public virtual IOverlappingPairCache GetOverlappingPairCache()
		{
			return m_paircache;
		}

		public virtual void GetBroadphaseAabb(out IndexedVector3 aabbMin, out IndexedVector3 aabbMax)
		{
			DbvtAabbMm r = default(DbvtAabbMm);
			if (!m_sets[0].Empty())
			{
				if (!m_sets[1].Empty())
				{
					DbvtAabbMm.Merge(ref m_sets[0].m_root.volume, ref m_sets[1].m_root.volume, ref r);
				}
				else
				{
					r = m_sets[0].m_root.volume;
				}
			}
			else if (!m_sets[1].Empty())
			{
				r = m_sets[1].m_root.volume;
			}
			else
			{
				IndexedVector3 c = IndexedVector3.Zero;
				r = DbvtAabbMm.FromCR(ref c, 0f);
			}
			aabbMin = r.Mins();
			aabbMax = r.Maxs();
		}

		public void SetVelocityPrediction(float prediction)
		{
			m_prediction = prediction;
		}

		public float GetVelocityPrediction()
		{
			return m_prediction;
		}

		public virtual void PrintStats()
		{
		}

		public static void Benchmark(IBroadphaseInterface broadphaseInterface)
		{
			IList<BroadphaseBenchmarkObject> list = new List<BroadphaseBenchmarkObject>();
			Stopwatch stopwatch = new Stopwatch();
			for (int i = 0; i < s_experiments.Length; i++)
			{
				BroadphaseBenchmarkExperiment broadphaseBenchmarkExperiment = s_experiments[i];
				int object_count = broadphaseBenchmarkExperiment.object_count;
				int num = object_count * broadphaseBenchmarkExperiment.update_count / 100;
				int num2 = object_count * broadphaseBenchmarkExperiment.spawn_count / 100;
				float speed = broadphaseBenchmarkExperiment.speed;
				float amplitude = broadphaseBenchmarkExperiment.amplitude;
				stopwatch.Reset();
				for (int j = 0; j < object_count; j++)
				{
					BroadphaseBenchmarkObject broadphaseBenchmarkObject = new BroadphaseBenchmarkObject();
					broadphaseBenchmarkObject.center.X = BroadphaseBenchmark.UnitRand() * 50f;
					broadphaseBenchmarkObject.center.Y = BroadphaseBenchmark.UnitRand() * 50f;
					broadphaseBenchmarkObject.center.Z = BroadphaseBenchmark.UnitRand() * 50f;
					broadphaseBenchmarkObject.extents.X = BroadphaseBenchmark.UnitRand() * 2f + 2f;
					broadphaseBenchmarkObject.extents.Y = BroadphaseBenchmark.UnitRand() * 2f + 2f;
					broadphaseBenchmarkObject.extents.Z = BroadphaseBenchmark.UnitRand() * 2f + 2f;
					broadphaseBenchmarkObject.time = BroadphaseBenchmark.UnitRand() * 2000f;
					broadphaseBenchmarkObject.proxy = broadphaseInterface.CreateProxy(broadphaseBenchmarkObject.center - broadphaseBenchmarkObject.extents, broadphaseBenchmarkObject.center + broadphaseBenchmarkObject.extents, BroadphaseNativeTypes.BOX_SHAPE_PROXYTYPE, broadphaseBenchmarkObject, CollisionFilterGroups.DefaultFilter, CollisionFilterGroups.DefaultFilter, null, null);
					list.Add(broadphaseBenchmarkObject);
				}
				BroadphaseBenchmark.OutputTime("\tInitialization", stopwatch, 1u);
				stopwatch.Reset();
				for (int k = 0; k < list.Count; k++)
				{
					list[k].update(speed, amplitude, broadphaseInterface);
				}
				BroadphaseBenchmark.OutputTime("\tFirst update", stopwatch, 1u);
				stopwatch.Reset();
				for (int l = 0; l < broadphaseBenchmarkExperiment.iterations; l++)
				{
					for (int m = 0; m < num; m++)
					{
						list[m].update(speed, amplitude, broadphaseInterface);
					}
					broadphaseInterface.CalculateOverlappingPairs(null);
				}
				BroadphaseBenchmark.OutputTime("\tUpdate", stopwatch, (uint)broadphaseBenchmarkExperiment.iterations);
				stopwatch.Reset();
				for (int n = 0; n < list.Count; n++)
				{
					broadphaseInterface.DestroyProxy(list[n].proxy, null);
					list[n] = null;
				}
				list.Clear();
				BroadphaseBenchmark.OutputTime("\tRelease", stopwatch, 1u);
			}
		}

		public void PerformDeferredRemoval(IDispatcher dispatcher)
		{
			if (!m_paircache.HasDeferredRemoval())
			{
				return;
			}
			ObjectArray<BroadphasePair> overlappingPairArray = m_paircache.GetOverlappingPairArray();
			overlappingPairArray.QuickSort(new BroadphasePairQuickSort());
			int num = 0;
			BroadphasePair broadphasePair = new BroadphasePair();
			for (int i = 0; i < overlappingPairArray.Count; i++)
			{
				BroadphasePair broadphasePair2 = overlappingPairArray[i];
				bool flag = broadphasePair2 == broadphasePair;
				broadphasePair = broadphasePair2;
				bool flag2 = false;
				if (!flag)
				{
					DbvtProxy dbvtProxy = broadphasePair2.m_pProxy0 as DbvtProxy;
					DbvtProxy dbvtProxy2 = broadphasePair2.m_pProxy1 as DbvtProxy;
					flag2 = ((!DbvtAabbMm.Intersect(ref dbvtProxy.leaf.volume, ref dbvtProxy2.leaf.volume)) ? true : false);
				}
				else
				{
					flag2 = true;
				}
				if (flag2)
				{
					m_paircache.CleanOverlappingPair(broadphasePair2, dispatcher);
					broadphasePair2.m_pProxy0 = null;
					broadphasePair2.m_pProxy1 = null;
					num++;
				}
			}
			if (num > 0)
			{
				int count = overlappingPairArray.Count;
				overlappingPairArray.QuickSort(new BroadphasePairQuickSort());
				overlappingPairArray.Truncate(num);
			}
		}

		public virtual void ResetPool(IDispatcher dispatcher)
		{
			if (m_sets[0].m_leaves + m_sets[1].m_leaves == 0)
			{
				m_sets[0].Clear();
				m_sets[1].Clear();
				m_deferedcollide = false;
				m_needcleanup = true;
				m_stageCurrent = 0;
				m_fixedleft = 0;
				m_fupdates = 1;
				m_dupdates = 0;
				m_cupdates = 10;
				m_newpairs = 1;
				m_updates_call = 0u;
				m_updates_done = 0u;
				m_updates_ratio = 0f;
				m_gid = 0;
				m_pid = 0;
				m_cid = 0;
				for (int i = 0; i <= 2; i++)
				{
					m_stageRoots[i] = null;
				}
			}
		}

		public static void ListAppend(DbvtProxy item, ref DbvtProxy list)
		{
			item.links[0] = null;
			item.links[1] = list;
			if (list != null)
			{
				list.links[0] = item;
			}
			list = item;
		}

		public static void ListRemove(DbvtProxy item, ref DbvtProxy list)
		{
			if (item.links[0] != null)
			{
				item.links[0].links[1] = item.links[1];
			}
			else
			{
				list = item.links[1];
			}
			if (item.links[1] != null)
			{
				item.links[1].links[0] = item.links[0];
			}
		}

		public static int ListCount(DbvtProxy root)
		{
			int num = 0;
			while (root != null)
			{
				num++;
				root = root.links[1];
			}
			return num;
		}
	}
}
