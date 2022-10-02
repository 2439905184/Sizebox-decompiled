using System.Collections.Generic;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class CollisionWorld
	{
		protected ObjectArray<CollisionObject> m_collisionObjects;

		protected IDispatcher m_dispatcher1;

		protected DispatcherInfo m_dispatchInfo;

		protected IBroadphaseInterface m_broadphasePairCache;

		protected IDebugDraw m_debugDrawer;

		protected bool m_forceUpdateAllAabbs;

		protected IProfileManager m_profileManager;

		public CollisionWorld(IDispatcher dispatcher, IBroadphaseInterface broadphasePairCache, ICollisionConfiguration collisionConfiguration)
		{
			m_dispatcher1 = dispatcher;
			m_broadphasePairCache = broadphasePairCache;
			m_collisionObjects = new ObjectArray<CollisionObject>();
			m_dispatchInfo = new DispatcherInfo();
			m_forceUpdateAllAabbs = true;
		}

		public virtual void Cleanup()
		{
			foreach (CollisionObject item in (IEnumerable<CollisionObject>)m_collisionObjects)
			{
				BroadphaseProxy broadphaseHandle = item.GetBroadphaseHandle();
				if (broadphaseHandle != null)
				{
					if (GetBroadphase().GetOverlappingPairCache() != null)
					{
						GetBroadphase().GetOverlappingPairCache().CleanProxyFromPairs(broadphaseHandle, m_dispatcher1);
					}
					GetBroadphase().DestroyProxy(broadphaseHandle, m_dispatcher1);
					item.SetBroadphaseHandle(null);
				}
			}
		}

		public void SetBroadphase(IBroadphaseInterface pairCache)
		{
			m_broadphasePairCache = pairCache;
		}

		public IBroadphaseInterface GetBroadphase()
		{
			return m_broadphasePairCache;
		}

		public IOverlappingPairCache GetPairCache()
		{
			return m_broadphasePairCache.GetOverlappingPairCache();
		}

		public IDispatcher GetDispatcher()
		{
			return m_dispatcher1;
		}

		public void UpdateSingleAabb(CollisionObject colObj)
		{
			IndexedMatrix t = colObj.GetWorldTransform();
			IndexedVector3 aabbMin;
			IndexedVector3 aabbMax;
			colObj.GetCollisionShape().GetAabb(ref t, out aabbMin, out aabbMax);
			IndexedVector3 indexedVector = new IndexedVector3(BulletGlobals.gContactBreakingThreshold);
			aabbMin -= indexedVector;
			aabbMax += indexedVector;
			if (GetDispatchInfo().m_useContinuous && colObj.GetInternalType() == CollisionObjectTypes.CO_RIGID_BODY && !colObj.IsStaticOrKinematicObject())
			{
				IndexedVector3 aabbMin2;
				IndexedVector3 aabbMax2;
				colObj.GetCollisionShape().GetAabb(colObj.GetInterpolationWorldTransform(), out aabbMin2, out aabbMax2);
				aabbMin2 -= indexedVector;
				aabbMax2 += indexedVector;
				MathUtil.VectorMin(ref aabbMin2, ref aabbMin);
				MathUtil.VectorMax(ref aabbMax2, ref aabbMax);
			}
			IBroadphaseInterface broadphasePairCache = m_broadphasePairCache;
			if (colObj.IsStaticObject() || (aabbMax - aabbMin).LengthSquared() < 1E+12f)
			{
				broadphasePairCache.SetAabb(colObj.GetBroadphaseHandle(), ref aabbMin, ref aabbMax, m_dispatcher1);
				return;
			}
			colObj.SetActivationState(ActivationState.DISABLE_SIMULATION);
			if (true && m_debugDrawer != null)
			{
				bool flag = false;
				m_debugDrawer.ReportErrorWarning("Overflow in AABB, object removed from simulation");
				m_debugDrawer.ReportErrorWarning("If you can reproduce this, please email bugs@continuousphysics.com\n");
				m_debugDrawer.ReportErrorWarning("Please include above information, your Platform, version of OS.\n");
				m_debugDrawer.ReportErrorWarning("Thanks.\n");
			}
		}

		public virtual void UpdateAabbs()
		{
			BulletGlobals.StartProfile("updateAabbs");
			int count = m_collisionObjects.Count;
			for (int i = 0; i < count; i++)
			{
				CollisionObject collisionObject = m_collisionObjects[i];
				if (m_forceUpdateAllAabbs || collisionObject.IsActive())
				{
					UpdateSingleAabb(collisionObject);
				}
			}
			BulletGlobals.StopProfile();
		}

		public virtual void SetDebugDrawer(IDebugDraw debugDrawer)
		{
			m_debugDrawer = debugDrawer;
			BulletGlobals.gDebugDraw = debugDrawer;
		}

		public virtual IDebugDraw GetDebugDrawer()
		{
			return m_debugDrawer;
		}

		public virtual void DebugDrawWorld()
		{
			if (GetDebugDrawer() != null && (GetDebugDrawer().GetDebugMode() & DebugDrawModes.DBG_DrawContactPoints) != 0)
			{
				int numManifolds = GetDispatcher().GetNumManifolds();
				IndexedVector3 color = new IndexedVector3(1f, 1f, 0.5f);
				for (int i = 0; i < numManifolds; i++)
				{
					PersistentManifold manifoldByIndexInternal = GetDispatcher().GetManifoldByIndexInternal(i);
					int numContacts = manifoldByIndexInternal.GetNumContacts();
					for (int j = 0; j < numContacts; j++)
					{
						ManifoldPoint contactPoint = manifoldByIndexInternal.GetContactPoint(j);
						GetDebugDrawer().DrawContactPoint(contactPoint.GetPositionWorldOnB(), contactPoint.GetNormalWorldOnB(), contactPoint.GetDistance(), contactPoint.GetLifeTime(), color);
					}
				}
			}
			if (GetDebugDrawer() == null)
			{
				return;
			}
			DebugDrawModes debugMode = GetDebugDrawer().GetDebugMode();
			bool flag = (debugMode & DebugDrawModes.DBG_DrawWireframe) != 0;
			bool flag2 = (debugMode & DebugDrawModes.DBG_DrawAabb) != 0;
			if (!flag && !flag2)
			{
				return;
			}
			int count = m_collisionObjects.Count;
			for (int k = 0; k < count; k++)
			{
				CollisionObject collisionObject = m_collisionObjects[k];
				if (flag)
				{
					IndexedVector3 indexedVector = new IndexedVector3(255f, 255f, 255f);
					switch (collisionObject.GetActivationState())
					{
					case ActivationState.ACTIVE_TAG:
						indexedVector = new IndexedVector3(255f, 255f, 255f);
						break;
					case ActivationState.ISLAND_SLEEPING:
						indexedVector = new IndexedVector3(0f, 255f, 0f);
						break;
					case ActivationState.WANTS_DEACTIVATION:
						indexedVector = new IndexedVector3(0f, 255f, 255f);
						break;
					case ActivationState.DISABLE_DEACTIVATION:
						indexedVector = new IndexedVector3(255f, 0f, 0f);
						break;
					case ActivationState.DISABLE_SIMULATION:
						indexedVector = new IndexedVector3(255f, 255f, 0f);
						break;
					default:
						indexedVector = new IndexedVector3(255f, 0f, 0f);
						break;
					}
					IndexedMatrix worldTransform = collisionObject.GetWorldTransform();
					DebugDrawObject(ref worldTransform, collisionObject.GetCollisionShape(), ref indexedVector);
				}
				if (flag2)
				{
					IndexedVector3 color2 = new IndexedVector3(1f, 0f, 0f);
					IndexedVector3 aabbMin;
					IndexedVector3 aabbMax;
					collisionObject.GetCollisionShape().GetAabb(collisionObject.GetWorldTransform(), out aabbMin, out aabbMax);
					IndexedVector3 indexedVector2 = new IndexedVector3(BulletGlobals.gContactBreakingThreshold);
					aabbMin -= indexedVector2;
					aabbMax += indexedVector2;
					if (GetDispatchInfo().m_useContinuous && collisionObject.GetInternalType() == CollisionObjectTypes.CO_RIGID_BODY && !collisionObject.IsStaticOrKinematicObject())
					{
						IndexedMatrix interpolationWorldTransform = collisionObject.GetInterpolationWorldTransform();
						IndexedVector3 aabbMin2;
						IndexedVector3 aabbMax2;
						collisionObject.GetCollisionShape().GetAabb(interpolationWorldTransform, out aabbMin2, out aabbMax2);
						aabbMin2 -= indexedVector2;
						aabbMax2 += indexedVector2;
						MathUtil.VectorMin(ref aabbMin2, ref aabbMin);
						MathUtil.VectorMax(ref aabbMax2, ref aabbMax);
					}
					m_debugDrawer.DrawAabb(ref aabbMin, ref aabbMax, ref color2);
				}
			}
		}

		public virtual void DebugDrawObject(ref IndexedMatrix worldTransform, CollisionShape shape, ref IndexedVector3 color)
		{
			GetDebugDrawer().DrawTransform(ref worldTransform, 1f);
			switch (shape.GetShapeType())
			{
			case BroadphaseNativeTypes.COMPOUND_SHAPE_PROXYTYPE:
			{
				CompoundShape compoundShape = (CompoundShape)shape;
				for (int num2 = compoundShape.GetNumChildShapes() - 1; num2 >= 0; num2--)
				{
					IndexedMatrix childTransform = compoundShape.GetChildTransform(num2);
					CollisionShape childShape = compoundShape.GetChildShape(num2);
					IndexedMatrix worldTransform2 = worldTransform * childTransform;
					DebugDrawObject(ref worldTransform2, childShape, ref color);
				}
				return;
			}
			case BroadphaseNativeTypes.BOX_SHAPE_PROXYTYPE:
			{
				BoxShape boxShape = shape as BoxShape;
				IndexedVector3 bbMax = boxShape.GetHalfExtentsWithMargin();
				IndexedVector3 bbMin = -bbMax;
				GetDebugDrawer().DrawBox(ref bbMin, ref bbMax, ref worldTransform, ref color);
				return;
			}
			case BroadphaseNativeTypes.SPHERE_SHAPE_PROXYTYPE:
			{
				SphereShape sphereShape = shape as SphereShape;
				float margin = sphereShape.GetMargin();
				GetDebugDrawer().DrawSphere(margin, ref worldTransform, ref color);
				return;
			}
			case BroadphaseNativeTypes.MULTI_SPHERE_SHAPE_PROXYTYPE:
			{
				MultiSphereShape multiSphereShape = (MultiSphereShape)shape;
				for (int num = multiSphereShape.GetSphereCount() - 1; num >= 0; num--)
				{
					IndexedMatrix transform = worldTransform;
					transform._origin += multiSphereShape.GetSpherePosition(num);
					GetDebugDrawer().DrawSphere(multiSphereShape.GetSphereRadius(num), ref transform, ref color);
				}
				return;
			}
			case BroadphaseNativeTypes.CAPSULE_SHAPE_PROXYTYPE:
			{
				CapsuleShape capsuleShape = shape as CapsuleShape;
				float radius2 = capsuleShape.GetRadius();
				float halfHeight2 = capsuleShape.GetHalfHeight();
				int upAxis2 = capsuleShape.GetUpAxis();
				GetDebugDrawer().DrawCapsule(radius2, halfHeight2, upAxis2, ref worldTransform, ref color);
				return;
			}
			case BroadphaseNativeTypes.CONE_SHAPE_PROXYTYPE:
			{
				ConeShape coneShape = (ConeShape)shape;
				float radius3 = coneShape.GetRadius();
				float height = coneShape.GetHeight();
				int coneUpIndex = coneShape.GetConeUpIndex();
				GetDebugDrawer().DrawCone(radius3, height, coneUpIndex, ref worldTransform, ref color);
				return;
			}
			case BroadphaseNativeTypes.CYLINDER_SHAPE_PROXYTYPE:
			{
				CylinderShape cylinderShape = (CylinderShape)shape;
				int upAxis = cylinderShape.GetUpAxis();
				float radius = cylinderShape.GetRadius();
				float halfHeight = cylinderShape.GetHalfExtentsWithMargin()[upAxis];
				GetDebugDrawer().DrawCylinder(radius, halfHeight, upAxis, ref worldTransform, ref color);
				return;
			}
			case BroadphaseNativeTypes.STATIC_PLANE_PROXYTYPE:
			{
				StaticPlaneShape staticPlaneShape = shape as StaticPlaneShape;
				float planeConstant = staticPlaneShape.GetPlaneConstant();
				IndexedVector3 planeNormal = staticPlaneShape.GetPlaneNormal();
				GetDebugDrawer().DrawPlane(ref planeNormal, planeConstant, ref worldTransform, ref color);
				return;
			}
			}
			if (shape.IsPolyhedral())
			{
				PolyhedralConvexShape polyhedralConvexShape = shape as PolyhedralConvexShape;
				ConvexPolyhedron convexPolyhedron = polyhedralConvexShape.GetConvexPolyhedron();
				if (convexPolyhedron != null)
				{
					for (int i = 0; i < convexPolyhedron.m_faces.Count; i++)
					{
						IndexedVector3 zero = IndexedVector3.Zero;
						int count = convexPolyhedron.m_faces[i].m_indices.Count;
						if (count != 0)
						{
							int index = convexPolyhedron.m_faces[i].m_indices[count - 1];
							for (int j = 0; j < convexPolyhedron.m_faces[i].m_indices.Count; j++)
							{
								int num3 = convexPolyhedron.m_faces[i].m_indices[j];
								zero += convexPolyhedron.m_vertices[num3];
								GetDebugDrawer().DrawLine(worldTransform * convexPolyhedron.m_vertices[index], worldTransform * convexPolyhedron.m_vertices[num3], color);
								index = num3;
							}
						}
						zero *= 1f / (float)count;
						IndexedVector3 color2 = new IndexedVector3(1f, 1f, 0f);
						IndexedVector3 indexedVector = new IndexedVector3(convexPolyhedron.m_faces[i].m_plane[0], convexPolyhedron.m_faces[i].m_plane[1], convexPolyhedron.m_faces[i].m_plane[2]);
						GetDebugDrawer().DrawLine(worldTransform * zero, worldTransform * (zero + indexedVector), color2);
					}
				}
				else
				{
					for (int k = 0; k < polyhedralConvexShape.GetNumEdges(); k++)
					{
						IndexedVector3 pa;
						IndexedVector3 pb;
						polyhedralConvexShape.GetEdge(k, out pa, out pb);
						IndexedVector3 from = worldTransform * pa;
						IndexedVector3 to = worldTransform * pb;
						GetDebugDrawer().DrawLine(ref from, ref to, ref color);
					}
				}
			}
			if (shape.IsConcave())
			{
				ConcaveShape concaveShape = (ConcaveShape)shape;
				IndexedVector3 aabbMax = MathUtil.MAX_VECTOR;
				IndexedVector3 aabbMin = MathUtil.MIN_VECTOR;
				using (BulletXNA.DebugDrawcallback debugDrawcallback = BulletGlobals.DebugDrawcallbackPool.Get())
				{
					debugDrawcallback.Initialise(GetDebugDrawer(), ref worldTransform, ref color);
					concaveShape.ProcessAllTriangles(debugDrawcallback, ref aabbMin, ref aabbMax);
					return;
				}
			}
			if (shape.GetShapeType() == BroadphaseNativeTypes.CONVEX_TRIANGLEMESH_SHAPE_PROXYTYPE)
			{
				ConvexTriangleMeshShape convexTriangleMeshShape = (ConvexTriangleMeshShape)shape;
				IndexedVector3 mAX_VECTOR = MathUtil.MAX_VECTOR;
				IndexedVector3 mIN_VECTOR = MathUtil.MIN_VECTOR;
			}
		}

		public int GetNumCollisionObjects()
		{
			return m_collisionObjects.Count;
		}

		public virtual void RayTest(ref IndexedVector3 rayFromWorld, ref IndexedVector3 rayToWorld, RayResultCallback resultCallback)
		{
			BulletGlobals.StartProfile("rayTest");
			using (SingleRayCallback singleRayCallback = BulletGlobals.SingleRayCallbackPool.Get())
			{
				singleRayCallback.Initialize(ref rayFromWorld, ref rayToWorld, this, resultCallback);
				m_broadphasePairCache.RayTest(ref rayFromWorld, ref rayToWorld, singleRayCallback);
			}
			BulletGlobals.StopProfile();
		}

		public void ContactTest(CollisionObject colObj, ContactResultCallback resultCallback)
		{
		}

		public void ContactPairTest(CollisionObject colObjA, CollisionObject colObjB, ContactResultCallback resultCallback)
		{
		}

		public virtual void ConvexSweepTest(ConvexShape castShape, IndexedMatrix convexFromWorld, IndexedMatrix convexToWorld, ConvexResultCallback resultCallback, float allowedCcdPenetration)
		{
			ConvexSweepTest(castShape, ref convexFromWorld, ref convexToWorld, resultCallback, allowedCcdPenetration);
		}

		public virtual void ConvexSweepTest(ConvexShape castShape, ref IndexedMatrix convexFromWorld, ref IndexedMatrix convexToWorld, ConvexResultCallback resultCallback, float allowedCcdPenetration)
		{
			BulletGlobals.StartProfile("convexSweepTest");
			IndexedMatrix transform = convexFromWorld;
			IndexedMatrix transform2 = convexToWorld;
			IndexedVector3 linVel;
			IndexedVector3 angVel;
			TransformUtil.CalculateVelocity(ref transform, ref transform2, 1f, out linVel, out angVel);
			IndexedVector3 linvel = default(IndexedVector3);
			IndexedMatrix curTrans = IndexedMatrix.Identity;
			curTrans.SetRotation(transform.GetRotation());
			IndexedVector3 temporalAabbMin;
			IndexedVector3 temporalAabbMax;
			castShape.CalculateTemporalAabb(ref curTrans, ref linvel, ref angVel, 1f, out temporalAabbMin, out temporalAabbMax);
			SingleSweepCallback singleSweepCallback = BulletGlobals.SingleSweepCallbackPool.Get();
			singleSweepCallback.Initialize(castShape, ref convexFromWorld, ref convexToWorld, this, resultCallback, allowedCcdPenetration);
			IndexedVector3 rayFrom = transform._origin;
			IndexedVector3 rayTo = transform2._origin;
			m_broadphasePairCache.RayTest(ref rayFrom, ref rayTo, singleSweepCallback, ref temporalAabbMin, ref temporalAabbMax);
			singleSweepCallback.Cleanup();
			BulletGlobals.SingleSweepCallbackPool.Free(singleSweepCallback);
			BulletGlobals.StopProfile();
		}

		public virtual void AddCollisionObject(CollisionObject collisionObject)
		{
			AddCollisionObject(collisionObject, CollisionFilterGroups.DefaultFilter, CollisionFilterGroups.AllFilter);
		}

		public virtual void AddCollisionObject(CollisionObject collisionObject, CollisionFilterGroups collisionFilterGroup, CollisionFilterGroups collisionFilterMask)
		{
			if (!m_collisionObjects.Contains(collisionObject))
			{
				m_collisionObjects.Add(collisionObject);
				IndexedMatrix t = collisionObject.GetWorldTransform();
				IndexedVector3 aabbMin;
				IndexedVector3 aabbMax;
				collisionObject.GetCollisionShape().GetAabb(ref t, out aabbMin, out aabbMax);
				BroadphaseNativeTypes shapeType = collisionObject.GetCollisionShape().GetShapeType();
				collisionObject.SetBroadphaseHandle(GetBroadphase().CreateProxy(ref aabbMin, ref aabbMax, shapeType, collisionObject, collisionFilterGroup, collisionFilterMask, m_dispatcher1, 0));
			}
		}

		public ObjectArray<CollisionObject> GetCollisionObjectArray()
		{
			return m_collisionObjects;
		}

		public DispatcherInfo GetDispatchInfo()
		{
			return m_dispatchInfo;
		}

		public virtual void PerformDiscreteCollisionDetection()
		{
			BulletGlobals.StartProfile("performDiscreteCollisionDetection");
			DispatcherInfo dispatchInfo = GetDispatchInfo();
			UpdateAabbs();
			BulletGlobals.StartProfile("calculateOverlappingPairs");
			m_broadphasePairCache.CalculateOverlappingPairs(m_dispatcher1);
			BulletGlobals.StopProfile();
			IDispatcher dispatcher = GetDispatcher();
			BulletGlobals.StartProfile("dispatchAllCollisionPairs");
			if (dispatcher != null)
			{
				dispatcher.DispatchAllCollisionPairs(m_broadphasePairCache.GetOverlappingPairCache(), dispatchInfo, m_dispatcher1);
			}
			BulletGlobals.StopProfile();
			BulletGlobals.StopProfile();
		}

		public virtual void RemoveCollisionObject(CollisionObject collisionObject)
		{
			BroadphaseProxy broadphaseHandle = collisionObject.GetBroadphaseHandle();
			if (broadphaseHandle != null)
			{
				GetBroadphase().GetOverlappingPairCache().CleanProxyFromPairs(broadphaseHandle, m_dispatcher1);
				GetBroadphase().DestroyProxy(broadphaseHandle, m_dispatcher1);
				collisionObject.SetBroadphaseHandle(null);
			}
			m_collisionObjects.Remove(collisionObject);
		}

		public bool GetForceUpdateAllAabbs()
		{
			return m_forceUpdateAllAabbs;
		}

		public void SetForceUpdateAllAabbs(bool forceUpdateAllAabbs)
		{
			m_forceUpdateAllAabbs = forceUpdateAllAabbs;
		}

		public static void RayTestSingle(ref IndexedMatrix rayFromTrans, ref IndexedMatrix rayToTrans, CollisionObject collisionObject, CollisionShape collisionShape, ref IndexedMatrix colObjWorldTransform, RayResultCallback resultCallback)
		{
			SphereShape sphereShape = BulletGlobals.SphereShapePool.Get();
			sphereShape.Initialize(0f);
			sphereShape.SetMargin(0f);
			ConvexShape shapeA = sphereShape;
			if (collisionShape.IsConvex())
			{
				BulletGlobals.StartProfile("rayTestConvex");
				CastResult castResult = BulletGlobals.CastResultPool.Get();
				castResult.m_fraction = resultCallback.m_closestHitFraction;
				ConvexShape shapeB = collisionShape as ConvexShape;
				VoronoiSimplexSolver voronoiSimplexSolver = BulletGlobals.VoronoiSimplexSolverPool.Get();
				SubSimplexConvexCast subSimplexConvexCast = BulletGlobals.SubSimplexConvexCastPool.Get();
				subSimplexConvexCast.Initialize(shapeA, shapeB, voronoiSimplexSolver);
				if (subSimplexConvexCast.CalcTimeOfImpact(ref rayFromTrans, ref rayToTrans, ref colObjWorldTransform, ref colObjWorldTransform, castResult) && castResult.m_normal.LengthSquared() > 0.0001f && castResult.m_fraction < resultCallback.m_closestHitFraction)
				{
					castResult.m_normal = rayFromTrans._basis * castResult.m_normal;
					castResult.m_normal.Normalize();
					LocalRayResult rayResult = new LocalRayResult(collisionObject, ref castResult.m_normal, castResult.m_fraction);
					bool normalInWorldSpace = true;
					resultCallback.AddSingleResult(ref rayResult, normalInWorldSpace);
				}
				castResult.Cleanup();
				BulletGlobals.SubSimplexConvexCastPool.Free(subSimplexConvexCast);
				BulletGlobals.VoronoiSimplexSolverPool.Free(voronoiSimplexSolver);
				BulletGlobals.StopProfile();
			}
			else if (collisionShape.IsConcave())
			{
				BulletGlobals.StartProfile("rayTestConcave");
				if (collisionShape.GetShapeType() == BroadphaseNativeTypes.TRIANGLE_MESH_SHAPE_PROXYTYPE && collisionShape is BvhTriangleMeshShape)
				{
					BvhTriangleMeshShape bvhTriangleMeshShape = (BvhTriangleMeshShape)collisionShape;
					IndexedMatrix indexedMatrix = colObjWorldTransform.Inverse();
					IndexedVector3 from = indexedMatrix * rayFromTrans._origin;
					IndexedVector3 to = indexedMatrix * rayToTrans._origin;
					IndexedMatrix colObjWorldTransform2 = IndexedMatrix.Identity;
					using (BridgeTriangleRaycastCallback bridgeTriangleRaycastCallback = BulletGlobals.BridgeTriangleRaycastCallbackPool.Get())
					{
						bridgeTriangleRaycastCallback.Initialize(ref from, ref to, resultCallback, collisionObject, bvhTriangleMeshShape, ref colObjWorldTransform2);
						bridgeTriangleRaycastCallback.m_hitFraction = resultCallback.m_closestHitFraction;
						bvhTriangleMeshShape.PerformRaycast(bridgeTriangleRaycastCallback, ref from, ref to);
					}
				}
				else if (collisionShape.GetShapeType() == BroadphaseNativeTypes.TERRAIN_SHAPE_PROXYTYPE && collisionShape is HeightfieldTerrainShape)
				{
					HeightfieldTerrainShape heightfieldTerrainShape = (HeightfieldTerrainShape)collisionShape;
					IndexedMatrix indexedMatrix2 = colObjWorldTransform.Inverse();
					IndexedVector3 from2 = indexedMatrix2 * rayFromTrans._origin;
					IndexedVector3 to2 = indexedMatrix2 * rayToTrans._origin;
					IndexedMatrix colObjWorldTransform3 = IndexedMatrix.Identity;
					using (BridgeTriangleConcaveRaycastCallback bridgeTriangleConcaveRaycastCallback = BulletGlobals.BridgeTriangleConcaveRaycastCallbackPool.Get())
					{
						bridgeTriangleConcaveRaycastCallback.Initialize(ref from2, ref to2, resultCallback, collisionObject, heightfieldTerrainShape, ref colObjWorldTransform3);
						bridgeTriangleConcaveRaycastCallback.m_hitFraction = resultCallback.m_closestHitFraction;
						heightfieldTerrainShape.PerformRaycast(bridgeTriangleConcaveRaycastCallback, ref from2, ref to2);
					}
				}
				else
				{
					ConcaveShape concaveShape = (ConcaveShape)collisionShape;
					IndexedMatrix indexedMatrix3 = colObjWorldTransform.Inverse();
					IndexedVector3 from3 = indexedMatrix3 * rayFromTrans._origin;
					IndexedVector3 to3 = indexedMatrix3 * rayToTrans._origin;
					IndexedMatrix colObjWorldTransform4 = IndexedMatrix.Identity;
					using (BridgeTriangleConcaveRaycastCallback bridgeTriangleConcaveRaycastCallback2 = BulletGlobals.BridgeTriangleConcaveRaycastCallbackPool.Get())
					{
						bridgeTriangleConcaveRaycastCallback2.Initialize(ref from3, ref to3, resultCallback, collisionObject, concaveShape, ref colObjWorldTransform4);
						bridgeTriangleConcaveRaycastCallback2.m_hitFraction = resultCallback.m_closestHitFraction;
						IndexedVector3 output = from3;
						MathUtil.VectorMin(ref to3, ref output);
						IndexedVector3 output2 = from3;
						MathUtil.VectorMax(ref to3, ref output2);
						concaveShape.ProcessAllTriangles(bridgeTriangleConcaveRaycastCallback2, ref output, ref output2);
					}
				}
				BulletGlobals.StopProfile();
			}
			else
			{
				BulletGlobals.StartProfile("rayTestCompound");
				if (collisionShape.IsCompound())
				{
					CompoundShape compoundShape = collisionShape as CompoundShape;
					Dbvt dynamicAabbTree = compoundShape.GetDynamicAabbTree();
					RayTester rayTester = new RayTester(collisionObject, compoundShape, ref colObjWorldTransform, ref rayFromTrans, ref rayToTrans, resultCallback);
					if (dynamicAabbTree != null)
					{
						IndexedVector3 rayFrom = colObjWorldTransform.InverseTimes(ref rayFromTrans)._origin;
						IndexedVector3 rayTo = colObjWorldTransform.InverseTimes(ref rayToTrans)._origin;
						Dbvt.RayTest(dynamicAabbTree.m_root, ref rayFrom, ref rayTo, rayTester);
					}
					else
					{
						int i = 0;
						for (int numChildShapes = compoundShape.GetNumChildShapes(); i < numChildShapes; i++)
						{
							rayTester.Process(i);
						}
					}
					rayTester.Cleanup();
					BulletGlobals.StopProfile();
				}
			}
			BulletGlobals.SphereShapePool.Free(sphereShape);
		}

		public static void ObjectQuerySingle(ConvexShape castShape, ref IndexedMatrix convexFromTrans, ref IndexedMatrix convexToTrans, CollisionObject collisionObject, CollisionShape collisionShape, ref IndexedMatrix colObjWorldTransform, ConvexResultCallback resultCallback, float allowedPenetration)
		{
			if (collisionShape.IsConvex())
			{
				BulletGlobals.StartProfile("convexSweepConvex");
				CastResult castResult = BulletGlobals.CastResultPool.Get();
				castResult.m_allowedPenetration = allowedPenetration;
				castResult.m_fraction = resultCallback.m_closestHitFraction;
				ConvexShape shapeB = collisionShape as ConvexShape;
				VoronoiSimplexSolver voronoiSimplexSolver = BulletGlobals.VoronoiSimplexSolverPool.Get();
				GjkEpaPenetrationDepthSolver gjkEpaPenetrationDepthSolver = BulletGlobals.GjkEpaPenetrationDepthSolverPool.Get();
				ContinuousConvexCollision continuousConvexCollision = BulletGlobals.ContinuousConvexCollisionPool.Get();
				continuousConvexCollision.Initialize(castShape, shapeB, voronoiSimplexSolver, gjkEpaPenetrationDepthSolver);
				IConvexCast convexCast = continuousConvexCollision;
				if (convexCast.CalcTimeOfImpact(ref convexFromTrans, ref convexToTrans, ref colObjWorldTransform, ref colObjWorldTransform, castResult) && castResult.m_normal.LengthSquared() > 0.0001f && castResult.m_fraction < resultCallback.m_closestHitFraction)
				{
					castResult.m_normal.Normalize();
					LocalConvexResult convexResult = new LocalConvexResult(collisionObject, ref castResult.m_normal, ref castResult.m_hitPoint, castResult.m_fraction);
					bool normalInWorldSpace = true;
					resultCallback.AddSingleResult(ref convexResult, normalInWorldSpace);
				}
				BulletGlobals.ContinuousConvexCollisionPool.Free(continuousConvexCollision);
				BulletGlobals.GjkEpaPenetrationDepthSolverPool.Free(gjkEpaPenetrationDepthSolver);
				BulletGlobals.VoronoiSimplexSolverPool.Free(voronoiSimplexSolver);
				castResult.Cleanup();
				BulletGlobals.StopProfile();
			}
			else if (collisionShape.IsConcave())
			{
				if (collisionShape.GetShapeType() == BroadphaseNativeTypes.TRIANGLE_MESH_SHAPE_PROXYTYPE)
				{
					BulletGlobals.StartProfile("convexSweepbtBvhTriangleMesh");
					BvhTriangleMeshShape bvhTriangleMeshShape = (BvhTriangleMeshShape)collisionShape;
					IndexedMatrix indexedMatrix = colObjWorldTransform.Inverse();
					IndexedVector3 boxSource = indexedMatrix * convexFromTrans._origin;
					IndexedVector3 boxTarget = indexedMatrix * convexToTrans._origin;
					IndexedMatrix t = new IndexedMatrix(indexedMatrix._basis * convexToTrans._basis, new IndexedVector3(0f));
					using (BridgeTriangleConvexcastCallback bridgeTriangleConvexcastCallback = BulletGlobals.BridgeTriangleConvexcastCallbackPool.Get())
					{
						bridgeTriangleConvexcastCallback.Initialize(castShape, ref convexFromTrans, ref convexToTrans, resultCallback, collisionObject, bvhTriangleMeshShape, ref colObjWorldTransform);
						bridgeTriangleConvexcastCallback.m_hitFraction = resultCallback.m_closestHitFraction;
						bridgeTriangleConvexcastCallback.m_allowedPenetration = allowedPenetration;
						IndexedVector3 aabbMin;
						IndexedVector3 aabbMax;
						castShape.GetAabb(ref t, out aabbMin, out aabbMax);
						bvhTriangleMeshShape.PerformConvexCast(bridgeTriangleConvexcastCallback, ref boxSource, ref boxTarget, ref aabbMin, ref aabbMax);
					}
					BulletGlobals.StopProfile();
					return;
				}
				if (collisionShape.GetShapeType() != BroadphaseNativeTypes.STATIC_PLANE_PROXYTYPE)
				{
					BulletGlobals.StartProfile("convexSweepConcave");
					ConcaveShape concaveShape = (ConcaveShape)collisionShape;
					IndexedMatrix indexedMatrix2 = colObjWorldTransform.Inverse();
					IndexedVector3 indexedVector = indexedMatrix2 * convexFromTrans._origin;
					IndexedVector3 input = indexedMatrix2 * convexToTrans._origin;
					IndexedMatrix t2 = new IndexedMatrix(indexedMatrix2._basis * convexToTrans._basis, new IndexedVector3(0f));
					using (BridgeTriangleConvexcastCallback bridgeTriangleConvexcastCallback2 = BulletGlobals.BridgeTriangleConvexcastCallbackPool.Get())
					{
						bridgeTriangleConvexcastCallback2.Initialize(castShape, ref convexFromTrans, ref convexToTrans, resultCallback, collisionObject, concaveShape, ref colObjWorldTransform);
						bridgeTriangleConvexcastCallback2.m_hitFraction = resultCallback.m_closestHitFraction;
						bridgeTriangleConvexcastCallback2.m_allowedPenetration = allowedPenetration;
						IndexedVector3 aabbMin2;
						IndexedVector3 aabbMax2;
						castShape.GetAabb(ref t2, out aabbMin2, out aabbMax2);
						IndexedVector3 output = indexedVector;
						MathUtil.VectorMin(ref input, ref output);
						IndexedVector3 output2 = indexedVector;
						MathUtil.VectorMax(ref input, ref output2);
						output += aabbMin2;
						output2 += aabbMax2;
						concaveShape.ProcessAllTriangles(bridgeTriangleConvexcastCallback2, ref output, ref output2);
						BulletGlobals.StopProfile();
						return;
					}
				}
				CastResult castResult2 = BulletGlobals.CastResultPool.Get();
				castResult2.m_allowedPenetration = allowedPenetration;
				castResult2.m_fraction = resultCallback.m_closestHitFraction;
				StaticPlaneShape plane = collisionShape as StaticPlaneShape;
				ContinuousConvexCollision continuousConvexCollision2 = new ContinuousConvexCollision(castShape, plane);
				if (continuousConvexCollision2.CalcTimeOfImpact(ref convexFromTrans, ref convexToTrans, ref colObjWorldTransform, ref colObjWorldTransform, castResult2) && castResult2.m_normal.LengthSquared() > 0.0001f && castResult2.m_fraction < resultCallback.m_closestHitFraction)
				{
					castResult2.m_normal.Normalize();
					LocalConvexResult convexResult2 = new LocalConvexResult(collisionObject, ref castResult2.m_normal, ref castResult2.m_hitPoint, castResult2.m_fraction);
					bool normalInWorldSpace2 = true;
					resultCallback.AddSingleResult(ref convexResult2, normalInWorldSpace2);
				}
				castResult2.Cleanup();
			}
			else if (collisionShape.IsCompound())
			{
				BulletGlobals.StartProfile("convexSweepCompound");
				CompoundShape compoundShape = (CompoundShape)collisionShape;
				for (int i = 0; i < compoundShape.GetNumChildShapes(); i++)
				{
					IndexedMatrix childTransform = compoundShape.GetChildTransform(i);
					CollisionShape childShape = compoundShape.GetChildShape(i);
					IndexedMatrix colObjWorldTransform2 = colObjWorldTransform * childTransform;
					CollisionShape collisionShape2 = collisionObject.GetCollisionShape();
					collisionObject.InternalSetTemporaryCollisionShape(childShape);
					LocalInfoAdder localInfoAdder = new LocalInfoAdder(i, resultCallback);
					localInfoAdder.m_closestHitFraction = resultCallback.m_closestHitFraction;
					ObjectQuerySingle(castShape, ref convexFromTrans, ref convexToTrans, collisionObject, childShape, ref colObjWorldTransform2, localInfoAdder, allowedPenetration);
					collisionObject.InternalSetTemporaryCollisionShape(collisionShape2);
				}
				BulletGlobals.StopProfile();
			}
		}
	}
}
