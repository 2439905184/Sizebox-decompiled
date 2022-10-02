using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class GImpactCollisionAlgorithm : ActivatingCollisionAlgorithm
	{
		protected CollisionAlgorithm m_convex_algorithm;

		protected PersistentManifold m_manifoldPtr;

		protected ManifoldResult m_resultOut;

		protected DispatcherInfo m_dispatchInfo;

		protected int m_triface0;

		protected int m_part0;

		protected int m_triface1;

		protected int m_part1;

		private GIM_TRIANGLE_CONTACT m_contact_data = new GIM_TRIANGLE_CONTACT();

		public GImpactCollisionAlgorithm()
		{
		}

		protected PersistentManifold NewContactManifold(CollisionObject body0, CollisionObject body1)
		{
			m_manifoldPtr = m_dispatcher.GetNewManifold(body0, body1);
			return m_manifoldPtr;
		}

		protected void DestroyConvexAlgorithm()
		{
			if (m_convex_algorithm != null)
			{
				m_dispatcher.FreeCollisionAlgorithm(m_convex_algorithm);
				m_convex_algorithm = null;
			}
		}

		protected void DestroyContactManifolds()
		{
			if (m_manifoldPtr != null)
			{
				m_dispatcher.ReleaseManifold(m_manifoldPtr);
				m_manifoldPtr = null;
			}
		}

		protected void ClearCache()
		{
			DestroyContactManifolds();
			DestroyConvexAlgorithm();
			m_triface0 = -1;
			m_part0 = -1;
			m_triface1 = -1;
			m_part1 = -1;
		}

		protected PersistentManifold GetLastManifold()
		{
			return m_manifoldPtr;
		}

		protected void CheckManifold(CollisionObject body0, CollisionObject body1)
		{
			if (GetLastManifold() == null)
			{
				NewContactManifold(body0, body1);
			}
			m_resultOut.SetPersistentManifold(GetLastManifold());
		}

		protected CollisionAlgorithm NewAlgorithm(CollisionObject body0, CollisionObject body1)
		{
			CheckManifold(body0, body1);
			return m_dispatcher.FindAlgorithm(body0, body1, GetLastManifold());
		}

		protected void CheckConvexAlgorithm(CollisionObject body0, CollisionObject body1)
		{
			if (m_convex_algorithm == null)
			{
				m_convex_algorithm = NewAlgorithm(body0, body1);
			}
		}

		protected void AddContactPoint(CollisionObject body0, CollisionObject body1, IndexedVector3 point, IndexedVector3 normal, float distance)
		{
			AddContactPoint(body0, body1, ref point, ref normal, distance);
		}

		protected void AddContactPoint(CollisionObject body0, CollisionObject body1, ref IndexedVector3 point, ref IndexedVector3 normal, float distance)
		{
			m_resultOut.SetShapeIdentifiersA(m_part0, m_triface0);
			m_resultOut.SetShapeIdentifiersB(m_part1, m_triface1);
			CheckManifold(body0, body1);
			m_resultOut.AddContactPoint(normal, point, distance);
		}

		protected void CollideGjkTriangles(CollisionObject body0, CollisionObject body1, GImpactMeshShapePart shape0, GImpactMeshShapePart shape1, ObjectArray<int> pairs, int pair_count)
		{
			TriangleShapeEx triangleShapeEx = new TriangleShapeEx();
			TriangleShapeEx triangleShapeEx2 = new TriangleShapeEx();
			shape0.LockChildShapes();
			shape1.LockChildShapes();
			int num = 0;
			while (pair_count-- != 0)
			{
				m_triface0 = pairs[num];
				m_triface1 = pairs[num + 1];
				num += 2;
				shape0.GetBulletTriangle(m_triface0, triangleShapeEx);
				shape1.GetBulletTriangle(m_triface1, triangleShapeEx2);
				if (triangleShapeEx.OverlapTestConservative(triangleShapeEx2))
				{
					ConvexVsConvexCollision(body0, body1, triangleShapeEx, triangleShapeEx2);
				}
			}
			shape0.UnlockChildShapes();
			shape1.UnlockChildShapes();
		}

		protected void CollideSatTriangles(CollisionObject body0, CollisionObject body1, GImpactMeshShapePart shape0, GImpactMeshShapePart shape1, PairSet pairs, int pair_count)
		{
			IndexedMatrix t = body0.GetWorldTransform();
			IndexedMatrix t2 = body1.GetWorldTransform();
			PrimitiveTriangle primitiveTriangle = new PrimitiveTriangle();
			PrimitiveTriangle primitiveTriangle2 = new PrimitiveTriangle();
			shape0.LockChildShapes();
			shape1.LockChildShapes();
			int num = 0;
			while (pair_count-- != 0)
			{
				m_triface0 = pairs[num].m_index1;
				m_triface1 = pairs[num].m_index2;
				num++;
				shape0.GetPrimitiveTriangle(m_triface0, primitiveTriangle);
				shape1.GetPrimitiveTriangle(m_triface1, primitiveTriangle2);
				primitiveTriangle.ApplyTransform(ref t);
				primitiveTriangle2.ApplyTransform(ref t2);
				primitiveTriangle.BuildTriPlane();
				primitiveTriangle2.BuildTriPlane();
				if (primitiveTriangle.OverlapTestConservative(primitiveTriangle2) && primitiveTriangle.FindTriangleCollisionClipMethod(primitiveTriangle2, m_contact_data))
				{
					int point_count = m_contact_data.m_point_count;
					while (point_count-- != 0)
					{
						AddContactPoint(body0, body1, m_contact_data.m_points[point_count], MathUtil.Vector4ToVector3(ref m_contact_data.m_separating_normal), 0f - m_contact_data.m_penetration_depth);
					}
				}
			}
			shape0.UnlockChildShapes();
			shape1.UnlockChildShapes();
		}

		protected void ShapeVsShapeCollision(CollisionObject body0, CollisionObject body1, CollisionShape shape0, CollisionShape shape1)
		{
			CollisionShape collisionShape = body0.GetCollisionShape();
			CollisionShape collisionShape2 = body1.GetCollisionShape();
			body0.InternalSetTemporaryCollisionShape(shape0);
			body1.InternalSetTemporaryCollisionShape(shape1);
			CollisionAlgorithm collisionAlgorithm = NewAlgorithm(body0, body1);
			m_resultOut.SetShapeIdentifiersA(m_part0, m_triface0);
			m_resultOut.SetShapeIdentifiersB(m_part1, m_triface1);
			collisionAlgorithm.ProcessCollision(body0, body1, m_dispatchInfo, m_resultOut);
			m_dispatcher.FreeCollisionAlgorithm(collisionAlgorithm);
			body0.InternalSetTemporaryCollisionShape(collisionShape);
			body1.InternalSetTemporaryCollisionShape(collisionShape2);
		}

		protected void ConvexVsConvexCollision(CollisionObject body0, CollisionObject body1, CollisionShape shape0, CollisionShape shape1)
		{
			CollisionShape collisionShape = body0.GetCollisionShape();
			CollisionShape collisionShape2 = body1.GetCollisionShape();
			body0.InternalSetTemporaryCollisionShape(shape0);
			body1.InternalSetTemporaryCollisionShape(shape1);
			m_resultOut.SetShapeIdentifiersA(m_part0, m_triface0);
			m_resultOut.SetShapeIdentifiersB(m_part1, m_triface1);
			CheckConvexAlgorithm(body0, body1);
			m_convex_algorithm.ProcessCollision(body0, body1, m_dispatchInfo, m_resultOut);
			body0.InternalSetTemporaryCollisionShape(collisionShape);
			body1.InternalSetTemporaryCollisionShape(collisionShape2);
		}

		protected void GImpactVsGImpactFindPairs(ref IndexedMatrix trans0, ref IndexedMatrix trans1, GImpactShapeInterface shape0, GImpactShapeInterface shape1, PairSet pairset)
		{
			if (shape0.HasBoxSet() && shape1.HasBoxSet())
			{
				GImpactQuantizedBvh.FindCollision(shape0.GetBoxSet(), ref trans0, shape1.GetBoxSet(), ref trans1, pairset);
				return;
			}
			AABB other = default(AABB);
			AABB aABB = default(AABB);
			int numChildShapes = shape0.GetNumChildShapes();
			while (numChildShapes-- != 0)
			{
				shape0.GetChildAabb(numChildShapes, ref trans0, out other.m_min, out other.m_max);
				int numChildShapes2 = shape1.GetNumChildShapes();
				while (numChildShapes2-- != 0)
				{
					shape1.GetChildAabb(numChildShapes, ref trans1, out aABB.m_min, out aABB.m_max);
					if (aABB.HasCollision(ref other))
					{
						pairset.PushPair(numChildShapes, numChildShapes2);
					}
				}
			}
		}

		protected void GImpactVsShapeFindPairs(ref IndexedMatrix trans0, ref IndexedMatrix trans1, GImpactShapeInterface shape0, CollisionShape shape1, ObjectArray<int> collided_primitives)
		{
			AABB box = default(AABB);
			if (shape0.HasBoxSet())
			{
				IndexedMatrix t = trans0.Inverse();
				t *= trans1;
				shape1.GetAabb(ref t, out box.m_min, out box.m_max);
				shape0.GetBoxSet().BoxQuery(ref box, collided_primitives);
				return;
			}
			shape1.GetAabb(ref trans1, out box.m_min, out box.m_max);
			AABB other = default(AABB);
			int numChildShapes = shape0.GetNumChildShapes();
			while (numChildShapes-- != 0)
			{
				shape0.GetChildAabb(numChildShapes, ref trans0, out other.m_min, out other.m_max);
				if (box.HasCollision(ref other))
				{
					collided_primitives.Add(numChildShapes);
				}
			}
		}

		protected void GImpactTrimeshpartVsPlaneCollision(CollisionObject body0, CollisionObject body1, GImpactMeshShapePart shape0, StaticPlaneShape shape1, bool swapped)
		{
			IndexedMatrix t = body0.GetWorldTransform();
			IndexedMatrix trans = body1.GetWorldTransform();
			IndexedVector4 equation;
			PlaneShape.GetPlaneEquationTransformed(shape1, ref trans, out equation);
			AABB aABB = default(AABB);
			shape0.GetAabb(ref t, out aABB.m_min, out aABB.m_max);
			aABB.IncrementMargin(shape1.GetMargin());
			if (aABB.PlaneClassify(ref equation) != BT_PLANE_INTERSECTION_TYPE.BT_CONST_COLLIDE_PLANE)
			{
				return;
			}
			shape0.LockChildShapes();
			float num = shape0.GetMargin() + shape1.GetMargin();
			int vertexCount = shape0.GetVertexCount();
			while (vertexCount-- != 0)
			{
				IndexedVector3 vertex;
				shape0.GetVertex(vertexCount, out vertex);
				vertex = t * vertex;
				float num2 = IndexedVector3.Dot(vertex, MathUtil.Vector4ToVector3(ref equation)) - equation.W - num;
				if (num2 < 0f)
				{
					if (swapped)
					{
						AddContactPoint(body1, body0, vertex, MathUtil.Vector4ToVector3(-equation), num2);
					}
					else
					{
						AddContactPoint(body0, body1, vertex, MathUtil.Vector4ToVector3(ref equation), num2);
					}
				}
			}
			shape0.UnlockChildShapes();
		}

		public GImpactCollisionAlgorithm(CollisionAlgorithmConstructionInfo ci, CollisionObject body0, CollisionObject body1)
			: base(ci, body0, body1)
		{
		}

		public override void Initialize(CollisionAlgorithmConstructionInfo ci, CollisionObject body0, CollisionObject body1)
		{
			base.Initialize(ci, body0, body1);
		}

		public override void Cleanup()
		{
			BulletGlobals.GImpactCollisionAlgorithmPool.Free(this);
		}

		public override void ProcessCollision(CollisionObject body0, CollisionObject body1, DispatcherInfo dispatchInfo, ManifoldResult resultOut)
		{
			ClearCache();
			m_resultOut = resultOut;
			m_dispatchInfo = dispatchInfo;
			if (body0.GetCollisionShape().GetShapeType() == BroadphaseNativeTypes.GIMPACT_SHAPE_PROXYTYPE)
			{
				GImpactShapeInterface shape = body0.GetCollisionShape() as GImpactShapeInterface;
				if (body1.GetCollisionShape().GetShapeType() == BroadphaseNativeTypes.GIMPACT_SHAPE_PROXYTYPE)
				{
					GImpactShapeInterface shape2 = body1.GetCollisionShape() as GImpactShapeInterface;
					GImpactVsGImpact(body0, body1, shape, shape2);
				}
				else
				{
					GImpactVsShape(body0, body1, shape, body1.GetCollisionShape(), false);
				}
			}
			else if (body1.GetCollisionShape().GetShapeType() == BroadphaseNativeTypes.GIMPACT_SHAPE_PROXYTYPE)
			{
				GImpactShapeInterface shape2 = body1.GetCollisionShape() as GImpactShapeInterface;
				GImpactVsShape(body1, body0, shape2, body0.GetCollisionShape(), true);
			}
		}

		public override float CalculateTimeOfImpact(CollisionObject body0, CollisionObject body1, DispatcherInfo dispatchInfo, ManifoldResult resultOut)
		{
			return 1f;
		}

		public override void GetAllContactManifolds(PersistentManifoldArray manifoldArray)
		{
			if (m_manifoldPtr != null)
			{
				manifoldArray.Add(m_manifoldPtr);
			}
		}

		public static void RegisterAlgorithm(CollisionDispatcher dispatcher)
		{
			GImpactCreateFunc createFunc = new GImpactCreateFunc();
			for (int i = 0; i < 36; i++)
			{
				dispatcher.RegisterCollisionCreateFunc(25, i, createFunc);
			}
			for (int i = 0; i < 36; i++)
			{
				dispatcher.RegisterCollisionCreateFunc(i, 25, createFunc);
			}
		}

		public static float GetAverageTreeCollisionTime()
		{
			return 1f;
		}

		public static float GetAverageTriangleCollisionTime()
		{
			return 1f;
		}

		public void GImpactVsGImpact(CollisionObject body0, CollisionObject body1, GImpactShapeInterface shape0, GImpactShapeInterface shape1)
		{
			if (shape0.GetGImpactShapeType() == GIMPACT_SHAPE_TYPE.CONST_GIMPACT_TRIMESH_SHAPE)
			{
				GImpactMeshShape gImpactMeshShape = shape0 as GImpactMeshShape;
				m_part0 = gImpactMeshShape.GetMeshPartCount();
				while (m_part0-- != 0)
				{
					GImpactVsGImpact(body0, body1, gImpactMeshShape.GetMeshPart(m_part0), shape1);
				}
				return;
			}
			if (shape1.GetGImpactShapeType() == GIMPACT_SHAPE_TYPE.CONST_GIMPACT_TRIMESH_SHAPE)
			{
				GImpactMeshShape gImpactMeshShape2 = shape1 as GImpactMeshShape;
				m_part1 = gImpactMeshShape2.GetMeshPartCount();
				while (m_part1-- != 0)
				{
					GImpactVsGImpact(body0, body1, shape0, gImpactMeshShape2.GetMeshPart(m_part1));
				}
				return;
			}
			IndexedMatrix trans = body0.GetWorldTransform();
			IndexedMatrix trans2 = body1.GetWorldTransform();
			PairSet pairSet = new PairSet();
			GImpactVsGImpactFindPairs(ref trans, ref trans2, shape0, shape1, pairSet);
			if (pairSet.Count == 0)
			{
				return;
			}
			if (shape0.GetGImpactShapeType() == GIMPACT_SHAPE_TYPE.CONST_GIMPACT_TRIMESH_SHAPE_PART && shape1.GetGImpactShapeType() == GIMPACT_SHAPE_TYPE.CONST_GIMPACT_TRIMESH_SHAPE_PART)
			{
				GImpactMeshShapePart shape2 = shape0 as GImpactMeshShapePart;
				GImpactMeshShapePart shape3 = shape1 as GImpactMeshShapePart;
				CollideSatTriangles(body0, body1, shape2, shape3, pairSet, pairSet.Count);
				return;
			}
			shape0.LockChildShapes();
			shape1.LockChildShapes();
			using (GIM_ShapeRetriever gIM_ShapeRetriever = BulletGlobals.GIM_ShapeRetrieverPool.Get())
			{
				using (GIM_ShapeRetriever gIM_ShapeRetriever2 = BulletGlobals.GIM_ShapeRetrieverPool.Get())
				{
					gIM_ShapeRetriever.Initialize(shape0);
					gIM_ShapeRetriever2.Initialize(shape1);
					bool flag = shape0.ChildrenHasTransform();
					bool flag2 = shape1.ChildrenHasTransform();
					int count = pairSet.Count;
					while (count-- != 0)
					{
						GIM_PAIR gIM_PAIR = pairSet[count];
						m_triface0 = gIM_PAIR.m_index1;
						m_triface1 = gIM_PAIR.m_index2;
						CollisionShape childShape = gIM_ShapeRetriever.GetChildShape(m_triface0);
						CollisionShape childShape2 = gIM_ShapeRetriever2.GetChildShape(m_triface1);
						if (flag)
						{
							body0.SetWorldTransform(trans * shape0.GetChildTransform(m_triface0));
						}
						if (flag2)
						{
							body1.SetWorldTransform(trans2 * shape1.GetChildTransform(m_triface1));
						}
						ConvexVsConvexCollision(body0, body1, childShape, childShape2);
						if (flag)
						{
							body0.SetWorldTransform(ref trans);
						}
						if (flag2)
						{
							body1.SetWorldTransform(ref trans2);
						}
					}
					shape0.UnlockChildShapes();
					shape1.UnlockChildShapes();
				}
			}
		}

		public void GImpactVsShape(CollisionObject body0, CollisionObject body1, GImpactShapeInterface shape0, CollisionShape shape1, bool swapped)
		{
			if (shape0.GetGImpactShapeType() == GIMPACT_SHAPE_TYPE.CONST_GIMPACT_TRIMESH_SHAPE)
			{
				GImpactMeshShape gImpactMeshShape = shape0 as GImpactMeshShape;
				int meshPartCount = gImpactMeshShape.GetMeshPartCount();
				while (meshPartCount-- != 0)
				{
					GImpactVsShape(body0, body1, gImpactMeshShape.GetMeshPart(meshPartCount), shape1, swapped);
				}
				if (swapped)
				{
					m_part1 = meshPartCount;
				}
				else
				{
					m_part0 = meshPartCount;
				}
				return;
			}
			if (shape0.GetGImpactShapeType() == GIMPACT_SHAPE_TYPE.CONST_GIMPACT_TRIMESH_SHAPE_PART && shape1.GetShapeType() == BroadphaseNativeTypes.STATIC_PLANE_PROXYTYPE)
			{
				GImpactMeshShapePart shape2 = shape0 as GImpactMeshShapePart;
				StaticPlaneShape shape3 = shape1 as StaticPlaneShape;
				GImpactTrimeshpartVsPlaneCollision(body0, body1, shape2, shape3, swapped);
				return;
			}
			if (shape1.IsCompound())
			{
				CompoundShape shape4 = shape1 as CompoundShape;
				GImpactVsCompoundshape(body0, body1, shape0, shape4, swapped);
				return;
			}
			if (shape1.IsConcave())
			{
				ConcaveShape shape5 = shape1 as ConcaveShape;
				GImpactVsConcave(body0, body1, shape0, shape5, swapped);
				return;
			}
			IndexedMatrix trans = body0.GetWorldTransform();
			IndexedMatrix trans2 = body1.GetWorldTransform();
			ObjectArray<int> objectArray = new ObjectArray<int>(64);
			GImpactVsShapeFindPairs(ref trans, ref trans2, shape0, shape1, objectArray);
			if (objectArray.Count == 0)
			{
				return;
			}
			shape0.LockChildShapes();
			using (GIM_ShapeRetriever gIM_ShapeRetriever = BulletGlobals.GIM_ShapeRetrieverPool.Get())
			{
				gIM_ShapeRetriever.Initialize(shape0);
				bool flag = shape0.ChildrenHasTransform();
				int count = objectArray.Count;
				while (count-- != 0)
				{
					int num = objectArray[count];
					if (swapped)
					{
						m_triface1 = num;
					}
					else
					{
						m_triface0 = num;
					}
					CollisionShape childShape = gIM_ShapeRetriever.GetChildShape(num);
					if (flag)
					{
						body0.SetWorldTransform(trans * shape0.GetChildTransform(num));
					}
					if (swapped)
					{
						ShapeVsShapeCollision(body1, body0, shape1, childShape);
					}
					else
					{
						ShapeVsShapeCollision(body0, body1, childShape, shape1);
					}
					if (flag)
					{
						body0.SetWorldTransform(ref trans);
					}
				}
				shape0.UnlockChildShapes();
			}
		}

		public void GImpactVsCompoundshape(CollisionObject body0, CollisionObject body1, GImpactShapeInterface shape0, CompoundShape shape1, bool swapped)
		{
			IndexedMatrix worldTrans = body1.GetWorldTransform();
			int numChildShapes = shape1.GetNumChildShapes();
			while (numChildShapes-- != 0)
			{
				CollisionShape childShape = shape1.GetChildShape(numChildShapes);
				IndexedMatrix worldTrans2 = worldTrans * shape1.GetChildTransform(numChildShapes);
				body1.SetWorldTransform(ref worldTrans2);
				GImpactVsShape(body0, body1, shape0, childShape, swapped);
				body1.SetWorldTransform(ref worldTrans);
			}
		}

		public void GImpactVsConcave(CollisionObject body0, CollisionObject body1, GImpactShapeInterface shape0, ConcaveShape shape1, bool swapped)
		{
			GImpactTriangleCallback gImpactTriangleCallback = new GImpactTriangleCallback();
			gImpactTriangleCallback.algorithm = this;
			gImpactTriangleCallback.body0 = body0;
			gImpactTriangleCallback.body1 = body1;
			gImpactTriangleCallback.gimpactshape0 = shape0;
			gImpactTriangleCallback.swapped = swapped;
			gImpactTriangleCallback.margin = shape1.GetMargin();
			IndexedMatrix t = body1.GetWorldTransform().Inverse() * body0.GetWorldTransform();
			IndexedVector3 aabbMin;
			IndexedVector3 aabbMax;
			shape0.GetAabb(t, out aabbMin, out aabbMax);
			shape1.ProcessAllTriangles(gImpactTriangleCallback, ref aabbMin, ref aabbMax);
		}

		public void SetFace0(int value)
		{
			m_triface0 = value;
		}

		public int GetFace0()
		{
			return m_triface0;
		}

		public void SetFace1(int value)
		{
			m_triface1 = value;
		}

		public int GetFace1()
		{
			return m_triface1;
		}

		public void SetPart0(int value)
		{
			m_part0 = value;
		}

		public int GetPart0()
		{
			return m_part0;
		}

		public void SetPart1(int value)
		{
			m_part1 = value;
		}

		public int GetPart1()
		{
			return m_part1;
		}
	}
}
