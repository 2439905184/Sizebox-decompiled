using System;
using System.Collections.Generic;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class Dbvt
	{
		private static ObjectArray<sStkNN> CollideTTStack = new ObjectArray<sStkNN>(DOUBLE_STACKSIZE);

		private static int CollideTTCount = 0;

		private static ObjectArray<sStkNN> m_stkStack = new ObjectArray<sStkNN>();

		private static Stack<DbvtNode> CollideTVStack = new Stack<DbvtNode>(SIMPLE_STACKSIZE);

		private static int CollideTVCount = 0;

		public static IndexedVector3[] axis = new IndexedVector3[3]
		{
			new IndexedVector3(1f, 0f, 0f),
			new IndexedVector3(0f, 1f, 0f),
			new IndexedVector3(0f, 0f, 1f)
		};

		public static int SIMPLE_STACKSIZE = 64;

		public static int DOUBLE_STACKSIZE = SIMPLE_STACKSIZE * 2;

		public DbvtNode m_root;

		public int m_lkhd;

		public int m_leaves;

		public uint m_opath;

		public DbvtNode Root
		{
			get
			{
				return m_root;
			}
			set
			{
				m_root = value;
			}
		}

		public static int Nearest(int[] i, sStkNPS[] a, float v, int l, int h)
		{
			int num = 0;
			while (l < h)
			{
				num = l + h >> 1;
				if (a[i[num]].value >= v)
				{
					l = num + 1;
				}
				else
				{
					h = num;
				}
			}
			return h;
		}

		public static int Allocate(ObjectArray<int> ifree, ObjectArray<sStkNPS> stock, sStkNPS value)
		{
			return 0;
		}

		public static void SetMin(ref IndexedVector3 a, ref IndexedVector3 b)
		{
			a.X = Math.Min(a.X, b.X);
			a.Y = Math.Min(a.Y, b.Y);
			a.Z = Math.Min(a.Z, b.Z);
		}

		public static void SetMax(ref IndexedVector3 a, ref IndexedVector3 b)
		{
			a.X = Math.Max(a.X, b.X);
			a.Y = Math.Max(a.Y, b.Y);
			a.Z = Math.Max(a.Z, b.Z);
		}

		public Dbvt()
		{
			m_lkhd = -1;
		}

		public void Clear()
		{
			if (m_root != null)
			{
				RecurseDeleteNode(this, m_root);
			}
			m_lkhd = -1;
			m_stkStack.Clear();
			m_opath = 0u;
		}

		public bool Empty()
		{
			return Root == null;
		}

		public void OptimizeBottomUp()
		{
			if (Root != null)
			{
				ObjectArray<DbvtNode> objectArray = new ObjectArray<DbvtNode>(m_leaves);
				FetchLeafs(this, Root, objectArray);
				BottomUp(this, objectArray);
				Root = objectArray[0];
			}
		}

		public void OptimizeTopDown()
		{
			OptimizeTopDown(128);
		}

		public void OptimizeTopDown(int bu_threshold)
		{
			if (Root != null)
			{
				ObjectArray<DbvtNode> objectArray = new ObjectArray<DbvtNode>(m_leaves);
				FetchLeafs(this, Root, objectArray);
				Root = TopDown(this, objectArray, bu_threshold);
			}
		}

		public virtual void Cleanup()
		{
			Clear();
		}

		public void OptimizeIncremental(int passes)
		{
			if (passes < 0)
			{
				passes = m_leaves;
			}
			if (Root == null || passes <= 0)
			{
				return;
			}
			int num = 4;
			do
			{
				DbvtNode dbvtNode = Root;
				int num2 = 0;
				while (dbvtNode.IsInternal())
				{
					dbvtNode = Sort(dbvtNode, m_root)._children[(m_opath >> num2) & 1];
					num2 = (num2 + 1) & 0x1F;
				}
				Update(dbvtNode);
				m_opath++;
			}
			while (--passes > 0);
		}

		public DbvtNode Insert(ref DbvtAabbMm box, int data)
		{
			DbvtNode dbvtNode = CreateNode(this, null, ref box, data);
			InsertLeaf(this, Root, dbvtNode);
			m_leaves++;
			return dbvtNode;
		}

		public DbvtNode Insert(ref DbvtAabbMm box, object data)
		{
			DbvtNode dbvtNode = CreateNode(this, null, ref box, data);
			InsertLeaf(this, Root, dbvtNode);
			m_leaves++;
			return dbvtNode;
		}

		public void Update(DbvtNode leaf)
		{
			Update(leaf, -1);
		}

		public void Update(DbvtNode leaf, int lookahead)
		{
			DbvtNode dbvtNode = RemoveLeaf(this, leaf);
			if (dbvtNode != null)
			{
				if (lookahead >= 0)
				{
					for (int i = 0; i < lookahead; i++)
					{
						if (dbvtNode.parent == null)
						{
							break;
						}
						dbvtNode = dbvtNode.parent;
					}
				}
				else
				{
					dbvtNode = Root;
				}
			}
			InsertLeaf(this, dbvtNode, leaf);
		}

		public void Update(DbvtNode leaf, ref DbvtAabbMm volume)
		{
			DbvtNode dbvtNode = RemoveLeaf(this, leaf);
			if (dbvtNode != null)
			{
				if (m_lkhd >= 0)
				{
					for (int i = 0; i < m_lkhd; i++)
					{
						if (dbvtNode.parent == null)
						{
							break;
						}
						dbvtNode = dbvtNode.parent;
					}
				}
				else
				{
					dbvtNode = Root;
				}
			}
			leaf.volume = volume;
			InsertLeaf(this, dbvtNode, leaf);
		}

		public bool Update(DbvtNode leaf, ref DbvtAabbMm volume, ref IndexedVector3 velocity, float margin)
		{
			if (leaf.volume.Contain(ref volume))
			{
				return false;
			}
			volume.Expand(new IndexedVector3(margin));
			volume.SignedExpand(velocity);
			Update(leaf, ref volume);
			return true;
		}

		public bool Update(DbvtNode leaf, ref DbvtAabbMm volume, ref IndexedVector3 velocity)
		{
			if (leaf.volume.Contain(ref volume))
			{
				return false;
			}
			volume.SignedExpand(velocity);
			Update(leaf, ref volume);
			return true;
		}

		public bool Update(DbvtNode leaf, ref DbvtAabbMm volume, float margin)
		{
			if (leaf.volume.Contain(ref volume))
			{
				return false;
			}
			volume.Expand(new IndexedVector3(margin));
			Update(leaf, ref volume);
			return true;
		}

		public void Remove(DbvtNode leaf)
		{
			RemoveLeaf(this, leaf);
			DeleteNode(this, leaf);
			m_leaves--;
		}

		public static DbvtNode Sort(DbvtNode n, DbvtNode r)
		{
			DbvtNode parent = n.parent;
			if (parent != null && parent.id > n.id)
			{
				int num = IndexOf(n);
				int num2 = 1 - num;
				DbvtNode dbvtNode = parent._children[num2];
				DbvtNode parent2 = parent.parent;
				if (parent2 != null)
				{
					parent2._children[IndexOf(parent)] = n;
				}
				else
				{
					r = n;
				}
				dbvtNode.parent = n;
				parent.parent = n;
				n.parent = parent2;
				parent._children[0] = n._children[0];
				parent._children[1] = n._children[1];
				n._children[0].parent = parent;
				n._children[1].parent = parent;
				n._children[num] = parent;
				n._children[num2] = dbvtNode;
				Swap(ref parent.volume, ref n.volume);
				return parent;
			}
			return n;
		}

		public static void Swap(ref DbvtAabbMm a, ref DbvtAabbMm b)
		{
			DbvtAabbMm dbvtAabbMm = b;
			b = a;
			a = dbvtAabbMm;
		}

		public static void FetchLeafs(Dbvt pdbvt, DbvtNode root, ObjectArray<DbvtNode> leafs)
		{
			FetchLeafs(pdbvt, root, leafs, -1);
		}

		public static void FetchLeafs(Dbvt pdbvt, DbvtNode root, ObjectArray<DbvtNode> leafs, int depth)
		{
			if (root.IsInternal() && depth != 0)
			{
				FetchLeafs(pdbvt, root._children[0], leafs, depth - 1);
				FetchLeafs(pdbvt, root._children[1], leafs, depth - 1);
				DeleteNode(pdbvt, root);
			}
			else
			{
				leafs.Add(root);
			}
		}

		public static void Split(ObjectArray<DbvtNode> leaves, ObjectArray<DbvtNode> left, ObjectArray<DbvtNode> right, ref IndexedVector3 org, ref IndexedVector3 axis)
		{
			left.Resize(0);
			right.Resize(0);
			int i = 0;
			for (int count = leaves.Count; i < count; i++)
			{
				if (IndexedVector3.Dot(axis, leaves[i].volume.Center() - org) < 0f)
				{
					left.Add(leaves[i]);
				}
				else
				{
					right.Add(leaves[i]);
				}
			}
		}

		public static void GetMaxDepth(DbvtNode node, int depth, ref int maxDepth)
		{
			if (node.IsInternal())
			{
				GetMaxDepth(node._children[0], depth + 1, ref maxDepth);
				GetMaxDepth(node._children[1], depth + 1, ref maxDepth);
			}
			else
			{
				maxDepth = Math.Max(depth, maxDepth);
			}
		}

		public static void EnumNodes(DbvtNode root, ICollide collideable)
		{
			collideable.Process(root);
			if (root.IsInternal())
			{
				EnumNodes(root._children[0], collideable);
				EnumNodes(root._children[1], collideable);
			}
		}

		public static void EnumLeaves(DbvtNode root, ICollide collideable)
		{
			if (root.IsInternal())
			{
				EnumLeaves(root._children[0], collideable);
				EnumLeaves(root._children[1], collideable);
			}
			else
			{
				collideable.Process(root);
			}
		}

		public static void CollideTT(DbvtNode root0, DbvtNode root1, ICollide collideable)
		{
			CollideTTCount++;
			CollideTTStack.Clear();
			if (root0 != null && root1 != null)
			{
				int num = 1;
				int num2 = DOUBLE_STACKSIZE - 4;
				CollideTTStack[0] = new sStkNN(root0, root1);
				do
				{
					sStkNN sStkNN2 = CollideTTStack[--num];
					if (num > num2)
					{
						CollideTTStack.Resize(CollideTTStack.Count * 2);
						num2 = CollideTTStack.Count - 4;
					}
					if (sStkNN2.a == sStkNN2.b)
					{
						if (sStkNN2.a.IsInternal())
						{
							CollideTTStack[num++] = new sStkNN(sStkNN2.a._children[0], sStkNN2.a._children[0]);
							CollideTTStack[num++] = new sStkNN(sStkNN2.a._children[1], sStkNN2.a._children[1]);
							CollideTTStack[num++] = new sStkNN(sStkNN2.a._children[0], sStkNN2.a._children[1]);
						}
					}
					else
					{
						if (!DbvtAabbMm.Intersect(ref sStkNN2.a.volume, ref sStkNN2.b.volume))
						{
							continue;
						}
						if (sStkNN2.a.IsInternal())
						{
							if (sStkNN2.b.IsInternal())
							{
								CollideTTStack[num++] = new sStkNN(sStkNN2.a._children[0], sStkNN2.b._children[0]);
								CollideTTStack[num++] = new sStkNN(sStkNN2.a._children[1], sStkNN2.b._children[0]);
								CollideTTStack[num++] = new sStkNN(sStkNN2.a._children[0], sStkNN2.b._children[1]);
								CollideTTStack[num++] = new sStkNN(sStkNN2.a._children[1], sStkNN2.b._children[1]);
							}
							else
							{
								CollideTTStack[num++] = new sStkNN(sStkNN2.a._children[0], sStkNN2.b);
								CollideTTStack[num++] = new sStkNN(sStkNN2.a._children[1], sStkNN2.b);
							}
						}
						else if (sStkNN2.b.IsInternal())
						{
							CollideTTStack[num++] = new sStkNN(sStkNN2.a, sStkNN2.b._children[0]);
							CollideTTStack[num++] = new sStkNN(sStkNN2.a, sStkNN2.b._children[1]);
						}
						else
						{
							collideable.Process(sStkNN2.a, sStkNN2.b);
						}
					}
				}
				while (num > 0);
			}
			CollideTTCount--;
		}

		public static void CollideTTpersistentStack(DbvtNode root0, DbvtNode root1, ICollide collideable)
		{
			if (root0 == null || root1 == null)
			{
				return;
			}
			int num = 1;
			int num2 = DOUBLE_STACKSIZE - 4;
			m_stkStack.Resize(DOUBLE_STACKSIZE);
			m_stkStack[0] = new sStkNN(root0, root1);
			do
			{
				sStkNN sStkNN2 = m_stkStack[--num];
				if (num > num2)
				{
					m_stkStack.Resize(m_stkStack.Count * 2);
					num2 = m_stkStack.Count - 4;
				}
				if (sStkNN2.a == sStkNN2.b)
				{
					if (sStkNN2.a.IsInternal())
					{
						m_stkStack[num++] = new sStkNN(sStkNN2.a._children[0], sStkNN2.a._children[0]);
						m_stkStack[num++] = new sStkNN(sStkNN2.a._children[1], sStkNN2.a._children[1]);
						m_stkStack[num++] = new sStkNN(sStkNN2.a._children[0], sStkNN2.a._children[1]);
					}
				}
				else
				{
					if (!DbvtAabbMm.Intersect(ref sStkNN2.a.volume, ref sStkNN2.b.volume))
					{
						continue;
					}
					if (sStkNN2.a.IsInternal())
					{
						if (sStkNN2.b.IsInternal())
						{
							m_stkStack[num++] = new sStkNN(sStkNN2.a._children[0], sStkNN2.b._children[0]);
							m_stkStack[num++] = new sStkNN(sStkNN2.a._children[1], sStkNN2.b._children[0]);
							m_stkStack[num++] = new sStkNN(sStkNN2.a._children[0], sStkNN2.b._children[1]);
							m_stkStack[num++] = new sStkNN(sStkNN2.a._children[1], sStkNN2.b._children[1]);
						}
						else
						{
							m_stkStack[num++] = new sStkNN(sStkNN2.a._children[0], sStkNN2.b);
							m_stkStack[num++] = new sStkNN(sStkNN2.a._children[1], sStkNN2.b);
						}
					}
					else if (sStkNN2.b.IsInternal())
					{
						m_stkStack[num++] = new sStkNN(sStkNN2.a, sStkNN2.b._children[0]);
						m_stkStack[num++] = new sStkNN(sStkNN2.a, sStkNN2.b._children[1]);
					}
					else
					{
						collideable.Process(sStkNN2.a, sStkNN2.b);
					}
				}
			}
			while (num > 0);
		}

		public static void RayTest(DbvtNode root, ref IndexedVector3 rayFrom, ref IndexedVector3 rayTo, ICollide policy)
		{
			using (DbvtStackDataBlock dbvtStackDataBlock = BulletGlobals.DbvtStackDataBlockPool.Get())
			{
				if (root == null)
				{
					return;
				}
				IndexedVector3 a = rayTo - rayFrom;
				a.Normalize();
				IndexedVector3 rayInvDirection = new IndexedVector3((a.X == 0f) ? 1E+18f : (1f / a.X), (a.Y == 0f) ? 1E+18f : (1f / a.Y), (a.Z == 0f) ? 1E+18f : (1f / a.Z));
				dbvtStackDataBlock.signs[0] = rayInvDirection.X < 0f;
				dbvtStackDataBlock.signs[1] = rayInvDirection.Y < 0f;
				dbvtStackDataBlock.signs[2] = rayInvDirection.Z < 0f;
				float lambda_max = IndexedVector3.Dot(a, rayTo - rayFrom);
				int num = 1;
				int num2 = DOUBLE_STACKSIZE - 2;
				dbvtStackDataBlock.stack.Resize(DOUBLE_STACKSIZE);
				dbvtStackDataBlock.stack[0] = root;
				do
				{
					DbvtNode dbvtNode = dbvtStackDataBlock.stack[--num];
					dbvtStackDataBlock.bounds[0] = dbvtNode.volume.Mins();
					dbvtStackDataBlock.bounds[1] = dbvtNode.volume.Maxs();
					float tmin = 1f;
					float lambda_min = 0f;
					if (!AabbUtil2.RayAabb2(ref rayFrom, ref rayInvDirection, dbvtStackDataBlock.signs, dbvtStackDataBlock.bounds, out tmin, lambda_min, lambda_max))
					{
						continue;
					}
					if (dbvtNode.IsInternal())
					{
						if (num > num2)
						{
							dbvtStackDataBlock.stack.Resize(dbvtStackDataBlock.stack.Count * 2);
							num2 = dbvtStackDataBlock.stack.Count - 2;
						}
						dbvtStackDataBlock.stack[num++] = dbvtNode._children[0];
						dbvtStackDataBlock.stack[num++] = dbvtNode._children[1];
					}
					else
					{
						policy.Process(dbvtNode);
					}
				}
				while (num != 0);
			}
		}

		public void RayTestInternal(DbvtNode root, ref IndexedVector3 rayFrom, ref IndexedVector3 rayTo, ref IndexedVector3 rayDirectionInverse, bool[] signs, float lambda_max, ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax, ICollide policy)
		{
			using (DbvtStackDataBlock dbvtStackDataBlock = BulletGlobals.DbvtStackDataBlockPool.Get())
			{
				if (root == null)
				{
					return;
				}
				new IndexedVector3(0f, 1f, 0f);
				int num = 1;
				int num2 = DOUBLE_STACKSIZE - 2;
				dbvtStackDataBlock.stack[0] = root;
				do
				{
					DbvtNode dbvtNode = dbvtStackDataBlock.stack[--num];
					dbvtStackDataBlock.bounds[0] = dbvtNode.volume.Mins() - aabbMax;
					dbvtStackDataBlock.bounds[1] = dbvtNode.volume.Maxs() - aabbMin;
					float tmin = 1f;
					float lambda_min = 0f;
					if (!AabbUtil2.RayAabb2(ref rayFrom, ref rayDirectionInverse, signs, dbvtStackDataBlock.bounds, out tmin, lambda_min, lambda_max))
					{
						continue;
					}
					if (dbvtNode.IsInternal())
					{
						if (num > num2)
						{
							dbvtStackDataBlock.stack.Resize(dbvtStackDataBlock.stack.Count * 2);
							num2 = dbvtStackDataBlock.stack.Count - 2;
						}
						dbvtStackDataBlock.stack[num++] = dbvtNode._children[0];
						dbvtStackDataBlock.stack[num++] = dbvtNode._children[1];
					}
					else
					{
						policy.Process(dbvtNode);
					}
				}
				while (num != 0);
			}
		}

		public static void CollideTV(DbvtNode root, ref DbvtAabbMm volume, ICollide collideable)
		{
			CollideTVCount++;
			CollideTVStack.Clear();
			if (root != null)
			{
				CollideTVStack.Push(root);
				do
				{
					DbvtNode dbvtNode = CollideTVStack.Pop();
					if (DbvtAabbMm.Intersect(ref dbvtNode.volume, ref volume))
					{
						if (dbvtNode.IsInternal())
						{
							CollideTVStack.Push(dbvtNode._children[0]);
							CollideTVStack.Push(dbvtNode._children[1]);
						}
						else
						{
							collideable.Process(dbvtNode);
						}
					}
				}
				while (CollideTVStack.Count > 0);
			}
			CollideTVCount--;
		}

		public static DbvtAabbMm Bounds(ObjectArray<DbvtNode> leafs)
		{
			DbvtAabbMm a = leafs[0].volume;
			int i = 1;
			for (int count = leafs.Count; i < count; i++)
			{
				DbvtAabbMm.Merge(ref a, ref leafs[i].volume, ref a);
			}
			return a;
		}

		public static void BottomUp(Dbvt pdbvt, ObjectArray<DbvtNode> leaves)
		{
			while (leaves.Count > 1)
			{
				float num = float.MaxValue;
				int[] array = new int[2] { -1, -1 };
				for (int i = 0; i < leaves.Count; i++)
				{
					for (int j = i + 1; j < leaves.Count; j++)
					{
						DbvtAabbMm a = DbvtAabbMm.Merge(ref leaves[i].volume, ref leaves[j].volume);
						float num2 = Size(ref a);
						if (num2 < num)
						{
							num = num2;
							array[0] = i;
							array[1] = j;
						}
					}
				}
				DbvtNode[] array2 = new DbvtNode[2]
				{
					leaves[array[0]],
					leaves[array[1]]
				};
				DbvtNode dbvtNode = CreateNode(pdbvt, null, ref array2[0].volume, ref array2[1].volume, null);
				dbvtNode._children[0] = array2[0];
				dbvtNode._children[1] = array2[1];
				array2[0].parent = dbvtNode;
				array2[1].parent = dbvtNode;
				leaves[array[0]] = dbvtNode;
				leaves.Swap(array[1], leaves.Count - 1);
				leaves.PopBack();
			}
		}

		public static DbvtNode TopDown(Dbvt pdbvt, ObjectArray<DbvtNode> leaves, int bu_treshold)
		{
			if (leaves.Count > 1)
			{
				if (leaves.Count > bu_treshold)
				{
					DbvtAabbMm volume = Bounds(leaves);
					IndexedVector3 org = volume.Center();
					ObjectArray<DbvtNode>[] array = new ObjectArray<DbvtNode>[2]
					{
						new ObjectArray<DbvtNode>(),
						new ObjectArray<DbvtNode>()
					};
					int num = -1;
					int num2 = leaves.Count;
					int[] array2 = new int[2];
					int[] array3 = array2;
					int[] array4 = new int[2];
					int[] array5 = array4;
					int[] array6 = new int[2];
					int[] array7 = array6;
					int[][] array8 = new int[3][] { array3, array5, array7 };
					for (int i = 0; i < leaves.Count; i++)
					{
						IndexedVector3 a = leaves[i].volume.Center() - org;
						for (int j = 0; j < 3; j++)
						{
							array8[j][(IndexedVector3.Dot(a, axis[j]) > 0f) ? 1u : 0u]++;
						}
					}
					for (int i = 0; i < 3; i++)
					{
						if (array8[i][0] > 0 && array8[i][1] > 0)
						{
							int num3 = Math.Abs(array8[i][0] - array8[i][1]);
							if (num3 < num2)
							{
								num = i;
								num2 = num3;
							}
						}
					}
					if (num >= 0)
					{
						array[0].EnsureCapacity(array8[num][0]);
						array[1].EnsureCapacity(array8[num][1]);
						Split(leaves, array[0], array[1], ref org, ref axis[num]);
					}
					else
					{
						array[0].EnsureCapacity(leaves.Count / 2 + 1);
						array[1].EnsureCapacity(leaves.Count / 2);
						int k = 0;
						for (int count = leaves.Count; k < count; k++)
						{
							array[k & 1].Add(leaves[k]);
						}
					}
					DbvtNode dbvtNode = CreateNode(pdbvt, null, ref volume, null);
					dbvtNode._children[0] = TopDown(pdbvt, array[0], bu_treshold);
					dbvtNode._children[1] = TopDown(pdbvt, array[1], bu_treshold);
					dbvtNode._children[0].parent = dbvtNode;
					dbvtNode._children[1].parent = dbvtNode;
					return dbvtNode;
				}
				BottomUp(pdbvt, leaves);
				return leaves[0];
			}
			return leaves[0];
		}

		public static DbvtNode CreateNode(Dbvt pdbvt, DbvtNode parent, int data)
		{
			DbvtNode dbvtNode = BulletGlobals.DbvtNodePool.Get();
			dbvtNode.parent = parent;
			dbvtNode.data = null;
			dbvtNode.dataAsInt = data;
			dbvtNode._children[0] = null;
			dbvtNode._children[1] = null;
			return dbvtNode;
		}

		public static DbvtNode CreateNode(Dbvt pdbvt, DbvtNode parent, object data)
		{
			DbvtNode dbvtNode = BulletGlobals.DbvtNodePool.Get();
			dbvtNode.parent = parent;
			dbvtNode.data = data;
			if (dbvtNode.data is int)
			{
				dbvtNode.dataAsInt = (int)dbvtNode.data;
			}
			dbvtNode._children[0] = null;
			dbvtNode._children[1] = null;
			return dbvtNode;
		}

		public static DbvtNode CreateNode2(Dbvt tree, DbvtNode aparent, ref DbvtAabbMm avolume, object adata)
		{
			DbvtNode dbvtNode = BulletGlobals.DbvtNodePool.Get();
			dbvtNode.volume = avolume;
			dbvtNode.parent = aparent;
			dbvtNode.data = adata;
			dbvtNode._children[0] = null;
			dbvtNode._children[1] = null;
			if (dbvtNode.data is int)
			{
				dbvtNode.dataAsInt = (int)dbvtNode.data;
			}
			return dbvtNode;
		}

		public static DbvtNode CreateNode(Dbvt pdbvt, DbvtNode parent, ref DbvtAabbMm volume, int data)
		{
			DbvtNode dbvtNode = CreateNode(pdbvt, parent, data);
			dbvtNode.volume = volume;
			return dbvtNode;
		}

		public static DbvtNode CreateNode(Dbvt pdbvt, DbvtNode parent, ref DbvtAabbMm volume, object data)
		{
			DbvtNode dbvtNode = CreateNode(pdbvt, parent, data);
			dbvtNode.volume = volume;
			return dbvtNode;
		}

		public static DbvtNode CreateNode(Dbvt pdbvt, DbvtNode parent, ref DbvtAabbMm volume0, ref DbvtAabbMm volume1, object data)
		{
			DbvtNode dbvtNode = CreateNode(pdbvt, parent, data);
			DbvtAabbMm.Merge(ref volume0, ref volume1, ref dbvtNode.volume);
			return dbvtNode;
		}

		public static void DeleteNode(Dbvt pdbvt, DbvtNode node)
		{
			node.Reset();
			BulletGlobals.DbvtNodePool.Free(node);
		}

		public static void RecurseDeleteNode(Dbvt pdbvt, DbvtNode node)
		{
			if (!node.IsLeaf())
			{
				RecurseDeleteNode(pdbvt, node._children[0]);
				RecurseDeleteNode(pdbvt, node._children[1]);
			}
			if (node == pdbvt.m_root)
			{
				pdbvt.m_root = null;
			}
			DeleteNode(pdbvt, node);
		}

		public static void InsertLeaf(Dbvt pdbvt, DbvtNode root, DbvtNode leaf)
		{
			if (pdbvt.Root == null)
			{
				pdbvt.Root = leaf;
				leaf.parent = null;
				return;
			}
			if (!root.IsLeaf())
			{
				do
				{
					root = root._children[DbvtAabbMm.Select(ref leaf.volume, ref root._children[0].volume, ref root._children[1].volume)];
				}
				while (!root.IsLeaf());
			}
			DbvtNode parent = root.parent;
			DbvtAabbMm avolume = DbvtAabbMm.Merge(ref leaf.volume, ref root.volume);
			DbvtNode dbvtNode = CreateNode2(pdbvt, parent, ref avolume, null);
			if (parent != null)
			{
				parent._children[IndexOf(root)] = dbvtNode;
				dbvtNode._children[0] = root;
				root.parent = dbvtNode;
				dbvtNode._children[1] = leaf;
				leaf.parent = dbvtNode;
				while (!parent.volume.Contain(ref dbvtNode.volume))
				{
					DbvtAabbMm.Merge(ref parent._children[0].volume, ref parent._children[1].volume, ref parent.volume);
					dbvtNode = parent;
					if ((parent = dbvtNode.parent) == null)
					{
						break;
					}
				}
			}
			else
			{
				dbvtNode._children[0] = root;
				root.parent = dbvtNode;
				dbvtNode._children[1] = leaf;
				leaf.parent = dbvtNode;
				pdbvt.Root = dbvtNode;
			}
		}

		public static DbvtNode RemoveLeaf(Dbvt pdbvt, DbvtNode leaf)
		{
			if (leaf == pdbvt.Root)
			{
				pdbvt.Root = null;
				return null;
			}
			DbvtNode parent = leaf.parent;
			DbvtNode parent2 = parent.parent;
			DbvtNode dbvtNode = parent._children[1 - IndexOf(leaf)];
			if (parent2 != null)
			{
				parent2._children[IndexOf(parent)] = dbvtNode;
				dbvtNode.parent = parent2;
				DeleteNode(pdbvt, parent);
				while (parent2 != null)
				{
					DbvtAabbMm a = parent2.volume;
					DbvtAabbMm.Merge(ref parent2._children[0].volume, ref parent2._children[1].volume, ref parent2.volume);
					if (!DbvtAabbMm.NotEqual(ref a, ref parent2.volume))
					{
						break;
					}
					dbvtNode = parent2;
					parent2 = parent2.parent;
				}
				if (parent2 == null)
				{
					return pdbvt.Root;
				}
				return parent2;
			}
			pdbvt.Root = dbvtNode;
			dbvtNode.parent = null;
			DeleteNode(pdbvt, parent);
			return pdbvt.Root;
		}

		public static int IndexOf(DbvtNode node)
		{
			if (node.parent._children[1] != node)
			{
				return 0;
			}
			return 1;
		}

		public static float Size(ref DbvtAabbMm a)
		{
			IndexedVector3 indexedVector = a.Lengths();
			return indexedVector.X * indexedVector.Y * indexedVector.Z + indexedVector.X + indexedVector.Y + indexedVector.Z;
		}
	}
}
