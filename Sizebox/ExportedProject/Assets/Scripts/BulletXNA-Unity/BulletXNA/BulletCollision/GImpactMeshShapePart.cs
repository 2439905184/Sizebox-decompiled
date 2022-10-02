using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class GImpactMeshShapePart : GImpactShapeInterface
	{
		public class TrimeshPrimitiveManager : IPrimitiveManagerBase
		{
			public float m_margin;

			public StridingMeshInterface m_meshInterface;

			public IndexedVector3 m_scale;

			public int m_part;

			public int m_lock_count;

			public object vertexbase;

			public int numverts;

			public PHY_ScalarType type;

			public int stride;

			public object indexbase;

			public int indexstride;

			public int numfaces;

			public PHY_ScalarType indicestype;

			public int[] indicesHolder = new int[3];

			public TrimeshPrimitiveManager()
			{
				m_margin = 0.01f;
				m_scale = IndexedVector3.One;
				m_lock_count = 0;
				vertexbase = null;
				numverts = 0;
				stride = 0;
				indexbase = null;
				indexstride = 0;
				numfaces = 0;
			}

			public TrimeshPrimitiveManager(TrimeshPrimitiveManager manager)
			{
				m_meshInterface = manager.m_meshInterface;
				m_part = manager.m_part;
				m_margin = manager.m_margin;
				m_scale = manager.m_scale;
				m_lock_count = 0;
				vertexbase = 0;
				numverts = 0;
				stride = 0;
				indexbase = 0;
				indexstride = 0;
				numfaces = 0;
			}

			public TrimeshPrimitiveManager(StridingMeshInterface meshInterface, int part)
			{
				m_meshInterface = meshInterface;
				m_part = part;
				m_scale = m_meshInterface.GetScaling();
				m_margin = 0.1f;
				m_lock_count = 0;
				vertexbase = 0;
				numverts = 0;
				stride = 0;
				indexbase = 0;
				indexstride = 0;
				numfaces = 0;
			}

			public virtual void Cleanup()
			{
			}

			public void Lock()
			{
				if (m_lock_count > 0)
				{
					m_lock_count++;
					return;
				}
				m_meshInterface.GetLockedReadOnlyVertexIndexBase(out vertexbase, out numverts, out type, out stride, out indexbase, out indexstride, out numfaces, out indicestype, m_part);
				m_lock_count = 1;
			}

			public void Unlock()
			{
				if (m_lock_count != 0)
				{
					if (m_lock_count > 1)
					{
						m_lock_count--;
						return;
					}
					m_meshInterface.UnLockReadOnlyVertexBase(m_part);
					vertexbase = null;
					m_lock_count = 0;
				}
			}

			public virtual bool IsTrimesh()
			{
				return true;
			}

			public virtual int GetPrimitiveCount()
			{
				return numfaces;
			}

			public int GetVertexCount()
			{
				return numverts;
			}

			public void GetIndices(int face_index, out int i0, out int i1, out int i2)
			{
				if (indicestype == PHY_ScalarType.PHY_SHORT)
				{
					ObjectArray<ushort> objectArray = indexbase as ObjectArray<ushort>;
					if (objectArray != null)
					{
						ushort[] rawArray = objectArray.GetRawArray();
						int num = face_index * indexstride;
						i0 = rawArray[num];
						i1 = rawArray[num + 1];
						i2 = rawArray[num + 2];
					}
					else
					{
						i0 = 0;
						i1 = 1;
						i2 = 2;
					}
				}
				else
				{
					ObjectArray<int> objectArray2 = indexbase as ObjectArray<int>;
					if (objectArray2 != null)
					{
						int[] rawArray2 = objectArray2.GetRawArray();
						int num2 = face_index * indexstride;
						i0 = rawArray2[num2];
						i1 = rawArray2[num2 + 1];
						i2 = rawArray2[num2 + 2];
					}
					else
					{
						i0 = 0;
						i1 = 1;
						i2 = 2;
					}
				}
			}

			public void GetVertex(int vertex_index, out IndexedVector3 vertex)
			{
				ObjectArray<IndexedVector3> objectArray = vertexbase as ObjectArray<IndexedVector3>;
				vertex = IndexedVector3.Zero;
				if (objectArray != null)
				{
					int index = vertex_index * stride;
					vertex = objectArray[index] * m_scale.X;
					return;
				}
				ObjectArray<float> objectArray2 = vertexbase as ObjectArray<float>;
				if (objectArray != null)
				{
					float[] rawArray = objectArray2.GetRawArray();
					int num = vertex_index * stride;
					vertex.X = rawArray[num] * m_scale.X;
					vertex.Y = rawArray[num + 1] * m_scale.Y;
					vertex.Z = rawArray[num + 2] * m_scale.Z;
				}
			}

			public virtual void GetPrimitiveBox(int prim_index, out AABB primbox)
			{
				PrimitiveTriangle primitiveTriangle = new PrimitiveTriangle();
				GetPrimitiveTriangle(prim_index, primitiveTriangle);
				primbox = default(AABB);
				primbox.CalcFromTriangleMargin(ref primitiveTriangle.m_vertices[0], ref primitiveTriangle.m_vertices[1], ref primitiveTriangle.m_vertices[2], primitiveTriangle.m_margin);
			}

			public virtual void GetPrimitiveTriangle(int prim_index, PrimitiveTriangle triangle)
			{
				GetIndices(prim_index, out indicesHolder[0], out indicesHolder[1], out indicesHolder[2]);
				GetVertex(indicesHolder[0], out triangle.m_vertices[0]);
				GetVertex(indicesHolder[1], out triangle.m_vertices[1]);
				GetVertex(indicesHolder[2], out triangle.m_vertices[2]);
				triangle.m_margin = m_margin;
			}

			public void GetBulletTriangle(int prim_index, TriangleShapeEx triangle)
			{
				GetIndices(prim_index, out indicesHolder[0], out indicesHolder[1], out indicesHolder[2]);
				GetVertex(indicesHolder[0], out triangle.m_vertices1[0]);
				GetVertex(indicesHolder[1], out triangle.m_vertices1[1]);
				GetVertex(indicesHolder[2], out triangle.m_vertices1[2]);
				triangle.SetMargin(m_margin);
			}
		}

		protected TrimeshPrimitiveManager m_primitive_manager = new TrimeshPrimitiveManager();

		public GImpactMeshShapePart()
		{
			m_box_set.SetPrimitiveManager(m_primitive_manager);
		}

		public GImpactMeshShapePart(StridingMeshInterface meshInterface, int part)
		{
			m_primitive_manager.m_meshInterface = meshInterface;
			m_primitive_manager.m_part = part;
			m_box_set.SetPrimitiveManager(m_primitive_manager);
		}

		public override void Cleanup()
		{
			base.Cleanup();
		}

		public override bool ChildrenHasTransform()
		{
			return false;
		}

		public override void LockChildShapes()
		{
			TrimeshPrimitiveManager trimeshPrimitiveManager = m_box_set.GetPrimitiveManager() as TrimeshPrimitiveManager;
			trimeshPrimitiveManager.Lock();
		}

		public override void UnlockChildShapes()
		{
			TrimeshPrimitiveManager trimeshPrimitiveManager = m_box_set.GetPrimitiveManager() as TrimeshPrimitiveManager;
			trimeshPrimitiveManager.Unlock();
		}

		public override int GetNumChildShapes()
		{
			return m_primitive_manager.GetPrimitiveCount();
		}

		public override CollisionShape GetChildShape(int index)
		{
			return null;
		}

		public override IndexedMatrix GetChildTransform(int index)
		{
			return IndexedMatrix.Identity;
		}

		public override void SetChildTransform(int index, ref IndexedMatrix transform)
		{
		}

		public override IPrimitiveManagerBase GetPrimitiveManager()
		{
			return m_primitive_manager;
		}

		public TrimeshPrimitiveManager GetTrimeshPrimitiveManager()
		{
			return m_primitive_manager;
		}

		public virtual void CalculateLocalInertia(float mass, ref IndexedVector3 inertia)
		{
			LockChildShapes();
			inertia = IndexedVector3.Zero;
			int vertexCount = GetVertexCount();
			float mass2 = mass / (float)vertexCount;
			while (vertexCount-- != 0)
			{
				IndexedVector3 vertex;
				GetVertex(vertexCount, out vertex);
				vertex = GImpactMassUtil.GimGetPointInertia(ref vertex, mass2);
				inertia += vertex;
			}
			UnlockChildShapes();
		}

		public override string GetName()
		{
			return "GImpactMeshShapePart";
		}

		public override GIMPACT_SHAPE_TYPE GetGImpactShapeType()
		{
			return GIMPACT_SHAPE_TYPE.CONST_GIMPACT_TRIMESH_SHAPE_PART;
		}

		public override bool NeedsRetrieveTriangles()
		{
			return true;
		}

		public override bool NeedsRetrieveTetrahedrons()
		{
			return false;
		}

		public override void GetBulletTriangle(int prim_index, TriangleShapeEx triangle)
		{
			m_primitive_manager.GetBulletTriangle(prim_index, triangle);
		}

		public override void GetBulletTetrahedron(int prim_index, TetrahedronShapeEx tetrahedron)
		{
		}

		public int GetVertexCount()
		{
			return m_primitive_manager.GetVertexCount();
		}

		public void GetVertex(int vertex_index, out IndexedVector3 vertex)
		{
			m_primitive_manager.GetVertex(vertex_index, out vertex);
		}

		public override void SetMargin(float margin)
		{
			m_primitive_manager.m_margin = margin;
			PostUpdate();
		}

		public override float GetMargin()
		{
			return m_primitive_manager.m_margin;
		}

		public override void SetLocalScaling(ref IndexedVector3 scaling)
		{
			m_primitive_manager.m_scale = scaling;
			PostUpdate();
		}

		public override IndexedVector3 GetLocalScaling()
		{
			return m_primitive_manager.m_scale;
		}

		public int GetPart()
		{
			return m_primitive_manager.m_part;
		}

		public override void ProcessAllTriangles(ITriangleCallback callback, ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax)
		{
			LockChildShapes();
			AABB box = default(AABB);
			box.m_min = aabbMin;
			box.m_max = aabbMax;
			ObjectArray<int> objectArray = new ObjectArray<int>();
			m_box_set.BoxQuery(ref box, objectArray);
			if (objectArray.Count == 0)
			{
				UnlockChildShapes();
				return;
			}
			int part = GetPart();
			PrimitiveTriangle primitiveTriangle = new PrimitiveTriangle();
			int count = objectArray.Count;
			while (count-- != 0)
			{
				GetPrimitiveTriangle(objectArray[count], primitiveTriangle);
				callback.ProcessTriangle(primitiveTriangle.m_vertices, part, objectArray[count]);
			}
			UnlockChildShapes();
		}
	}
}
