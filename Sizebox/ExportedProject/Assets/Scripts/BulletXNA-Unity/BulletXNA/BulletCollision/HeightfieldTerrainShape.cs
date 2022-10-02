using System;
using System.Collections.Generic;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class HeightfieldTerrainShape : ConcaveShape
	{
		public class QuadTreeNode
		{
			public IndexedVector3 vmin;

			public IndexedVector3 vmax;

			public QuadTreeNode[] children;

			public int depth;

			public QuadTreeNode()
			{
			}

			public QuadTreeNode(ref IndexedVector3 minv, ref IndexedVector3 maxv, int xAxis, int yAxis, int zAxis, float xDiff, float zDiff, bool x, bool z, int d)
			{
				IndexedVector3 indexedVector = minv;
				IndexedVector3 indexedVector2 = maxv;
				indexedVector[xAxis] += (x ? xDiff : 0f);
				indexedVector[zAxis] += (z ? zDiff : 0f);
				indexedVector[yAxis] = 100000f;
				indexedVector2[yAxis] = -100000f;
				indexedVector2[xAxis] = indexedVector[xAxis] + xDiff;
				indexedVector2[zAxis] = indexedVector[zAxis] + zDiff;
				depth = d;
			}

			public bool Intersects(IndexedVector3 source, IndexedVector3 direction)
			{
				float num = 0f;
				float num2 = float.MaxValue;
				if ((double)Math.Abs(direction.X) < 9.99999997475243E-07)
				{
					if (direction.X < vmin.X || direction.X > vmax.X)
					{
						return false;
					}
				}
				else
				{
					float num3 = 1f / direction.X;
					float num4 = (vmin.X - direction.X) * num3;
					float num5 = (vmax.X - direction.X) * num3;
					if (num4 > num5)
					{
						float num6 = num4;
						num4 = num5;
						num5 = num6;
					}
					num = ((num4 > num) ? num4 : num);
					num2 = ((num5 < num2) ? num5 : num2);
					if (num > num2)
					{
						return false;
					}
				}
				if ((double)Math.Abs(direction.Y) < 9.99999997475243E-07)
				{
					if (source.Y < vmin.Y || source.Y > vmax.Y)
					{
						return false;
					}
				}
				else
				{
					float num7 = 1f / direction.Y;
					float num8 = (vmin.Y - source.Y) * num7;
					float num9 = (vmax.Y - source.Y) * num7;
					if (num8 > num9)
					{
						float num10 = num8;
						num8 = num9;
						num9 = num10;
					}
					num = ((num8 > num) ? num8 : num);
					num2 = ((num9 < num2) ? num9 : num2);
					if (num > num2)
					{
						return false;
					}
				}
				if ((double)Math.Abs(direction.Z) < 9.99999997475243E-07)
				{
					if (source.Z < vmin.Z || source.Z > vmax.Z)
					{
						return false;
					}
				}
				else
				{
					float num11 = 1f / direction.Z;
					float num12 = (vmin.Z - source.Z) * num11;
					float num13 = (vmax.Z - source.Z) * num11;
					if (num12 > num13)
					{
						float num14 = num12;
						num12 = num13;
						num13 = num14;
					}
					num = ((num12 > num) ? num12 : num);
					float num15 = ((num13 < num2) ? num13 : num2);
					if (num > num15)
					{
						return false;
					}
				}
				return true;
			}

			public void AdjustHeightValues(int xAxis, int yAxis, int zAxis, ref float min, ref float max, HeightfieldTerrainShape shape)
			{
				if (children == null)
				{
					min = vmin[yAxis];
					max = vmax[yAxis];
					int[] array = new int[3];
					int[] array2 = new int[3];
					shape.QuantizeWithClamp(array, ref vmin, 0);
					shape.QuantizeWithClamp(array2, ref vmax, 1);
					shape.InspectVertexHeights(array[xAxis], array2[xAxis], array[zAxis], array2[zAxis], yAxis, ref min, ref max);
					vmin[yAxis] = min;
					vmax[yAxis] = max;
					return;
				}
				float min2 = min;
				float max2 = max;
				children[0].AdjustHeightValues(xAxis, yAxis, zAxis, ref min2, ref max2, shape);
				if (min2 < min)
				{
					min = min2;
				}
				if (max2 > max)
				{
					max = max2;
				}
				children[1].AdjustHeightValues(xAxis, yAxis, zAxis, ref min2, ref max2, shape);
				if (min2 < min)
				{
					min = min2;
				}
				if (max2 > max)
				{
					max = max2;
				}
				children[2].AdjustHeightValues(xAxis, yAxis, zAxis, ref min2, ref max2, shape);
				if (min2 < min)
				{
					min = min2;
				}
				if (max2 > max)
				{
					max = max2;
				}
				children[3].AdjustHeightValues(xAxis, yAxis, zAxis, ref min2, ref max2, shape);
				if (min2 < min)
				{
					min = min2;
				}
				if (max2 > max)
				{
					max = max2;
				}
				vmin[yAxis] = min;
				vmax[yAxis] = max;
			}
		}

		protected IndexedVector3 m_localAabbMin;

		protected IndexedVector3 m_localAabbMax;

		protected IndexedVector3 m_localOrigin;

		protected int m_heightStickWidth;

		protected int m_heightStickLength;

		protected float m_minHeight;

		protected float m_maxHeight;

		protected float m_width;

		protected float m_length;

		protected float m_heightScale;

		protected byte[] m_heightFieldDataByte;

		protected float[] m_heightFieldDataFloat;

		protected PHY_ScalarType m_heightDataType;

		protected bool m_flipQuadEdges;

		protected bool m_useDiamondSubdivision;

		protected int m_upAxis;

		protected IndexedVector3 m_localScaling;

		protected QuadTreeNode m_rootQuadTreeNode;

		protected int m_minNodeSize;

		protected int m_maxDepth;

		private int[] quantizedAabbMin = new int[3];

		private int[] quantizedAabbMax = new int[3];

		private IndexedVector3[] vertices = new IndexedVector3[3];

		public void RebuildQuadTree()
		{
			RebuildQuadTree(5, 4);
		}

		public void RebuildQuadTree(int maxDepth, int minNodeSize)
		{
			m_minNodeSize = minNodeSize;
			m_maxDepth = maxDepth;
			m_rootQuadTreeNode = new QuadTreeNode();
			int xAxis = 0;
			int yAxis = 1;
			int zAxis = 2;
			float min = 100000f;
			float max = -100000f;
			if (m_upAxis == 0)
			{
				xAxis = 1;
				yAxis = 0;
				zAxis = 2;
			}
			else if (m_upAxis == 2)
			{
				xAxis = 0;
				yAxis = 2;
				zAxis = 1;
			}
			m_rootQuadTreeNode.vmin = m_localAabbMin;
			m_rootQuadTreeNode.vmax = m_localAabbMax;
			BuildNodes(m_rootQuadTreeNode, 0, maxDepth, minNodeSize, xAxis, yAxis, zAxis);
			m_rootQuadTreeNode.AdjustHeightValues(xAxis, yAxis, zAxis, ref min, ref max, this);
		}

		private void BuildNodes(QuadTreeNode parent, int depth, int maxDepth, int minNodeSize, int xAxis, int yAxis, int zAxis)
		{
			if (depth < maxDepth && parent.children == null)
			{
				IndexedVector3 indexedVector = (parent.vmax - parent.vmin) / 2f;
				if (indexedVector[xAxis] >= (float)minNodeSize || indexedVector[zAxis] >= (float)minNodeSize)
				{
					parent.children = new QuadTreeNode[4];
					parent.children[0] = new QuadTreeNode(ref parent.vmin, ref parent.vmax, xAxis, yAxis, zAxis, indexedVector[xAxis], indexedVector[zAxis], false, false, depth);
					BuildNodes(parent.children[0], depth + 1, maxDepth, minNodeSize, xAxis, yAxis, zAxis);
					parent.children[1] = new QuadTreeNode(ref parent.vmin, ref parent.vmax, xAxis, yAxis, zAxis, indexedVector[xAxis], indexedVector[zAxis], true, false, depth);
					BuildNodes(parent.children[1], depth + 1, maxDepth, minNodeSize, xAxis, yAxis, zAxis);
					parent.children[2] = new QuadTreeNode(ref parent.vmin, ref parent.vmax, xAxis, yAxis, zAxis, indexedVector[xAxis], indexedVector[zAxis], false, true, depth);
					BuildNodes(parent.children[2], depth + 1, maxDepth, minNodeSize, xAxis, yAxis, zAxis);
					parent.children[3] = new QuadTreeNode(ref parent.vmin, ref parent.vmax, xAxis, yAxis, zAxis, indexedVector[xAxis], indexedVector[zAxis], true, true, depth);
					BuildNodes(parent.children[3], depth + 1, maxDepth, minNodeSize, xAxis, yAxis, zAxis);
				}
			}
		}

		public bool HasAccelerator()
		{
			return m_rootQuadTreeNode != null;
		}

		private void InspectVertexHeights(int startX, int endX, int startZ, int endZ, int upAxis, ref float min, ref float max)
		{
			IndexedVector3 indexedVector = new IndexedVector3(1f) / m_localScaling;
			for (int i = startZ; i < endZ; i++)
			{
				for (int j = startX; j < endX; j++)
				{
					float rawHeightFieldValue = GetRawHeightFieldValue(j, i);
					if (rawHeightFieldValue < min)
					{
						min = rawHeightFieldValue;
					}
					if (rawHeightFieldValue > max)
					{
						max = rawHeightFieldValue;
					}
				}
			}
		}

		public float TestAndClampHeight(float newHeight, float comparison, bool min)
		{
			if (min)
			{
				if (!(newHeight < comparison))
				{
					return comparison;
				}
				return newHeight;
			}
			if (!(newHeight > comparison))
			{
				return comparison;
			}
			return newHeight;
		}

		public void PerformRaycast(ITriangleCallback callback, ref IndexedVector3 raySource, ref IndexedVector3 rayTarget)
		{
			if (!HasAccelerator())
			{
				ProcessAllTriangles(callback, ref raySource, ref rayTarget);
				return;
			}
			float num = (rayTarget - raySource).LengthSquared();
			int i = 0;
			if (m_upAxis == 0)
			{
				i = 1;
			}
			if (num < (float)m_minNodeSize * m_localScaling[i])
			{
				ProcessAllTriangles(callback, ref raySource, ref rayTarget);
				return;
			}
			ObjectArray<QuadTreeNode> objectArray = new ObjectArray<QuadTreeNode>();
			IndexedVector3 indexedVector = new IndexedVector3(1f) / m_localScaling;
			IndexedVector3 indexedVector2 = raySource * indexedVector;
			IndexedVector3 indexedVector3 = rayTarget * indexedVector;
			indexedVector2 += m_localOrigin;
			indexedVector3 += m_localOrigin;
			IndexedVector3 direction = indexedVector3 - indexedVector2;
			direction.Normalize();
			QueryNode(m_rootQuadTreeNode, objectArray, indexedVector2, direction);
			foreach (QuadTreeNode item in (IEnumerable<QuadTreeNode>)objectArray)
			{
				int num2 = 0;
				int num3 = m_heightStickWidth - 1;
				int num4 = 0;
				int num5 = m_heightStickLength - 1;
				IndexedVector3 vmin = item.vmin;
				IndexedVector3 vmax = item.vmax;
				switch (m_upAxis)
				{
				case 0:
					if (vmin.Y > (float)num2)
					{
						num2 = (int)vmin.Y;
					}
					if (vmax.Y < (float)num3)
					{
						num3 = (int)vmax.Y;
					}
					if (vmin.Z > (float)num4)
					{
						num4 = (int)vmin.Z;
					}
					if (vmax.Z < (float)num5)
					{
						num5 = (int)vmax.Z;
					}
					break;
				case 1:
					if (vmin.X > (float)num2)
					{
						num2 = (int)vmin.X;
					}
					if (vmax.X < (float)num3)
					{
						num3 = (int)vmax.X;
					}
					if (vmin.Z > (float)num4)
					{
						num4 = (int)vmin.Z;
					}
					if (vmax.Z < (float)num5)
					{
						num5 = (int)vmax.Z;
					}
					break;
				case 2:
					if (vmin.X > (float)num2)
					{
						num2 = (int)vmin.X;
					}
					if (vmax.X < (float)num3)
					{
						num3 = (int)vmax.X;
					}
					if (vmin.Y > (float)num4)
					{
						num4 = (int)vmin.Y;
					}
					if (vmax.Y < (float)num5)
					{
						num5 = (int)vmax.Y;
					}
					break;
				}
				IndexedVector3[] array = new IndexedVector3[3];
				for (int j = num4; j < num5; j++)
				{
					for (int k = num2; k < num3; k++)
					{
						if (m_flipQuadEdges || (m_useDiamondSubdivision && ((j + k) & 1) > 0))
						{
							GetVertex(k, j, out array[0]);
							GetVertex(k + 1, j, out array[1]);
							GetVertex(k + 1, j + 1, out array[2]);
							callback.ProcessTriangle(array, k, j);
							GetVertex(k, j, out array[0]);
							GetVertex(k + 1, j + 1, out array[1]);
							GetVertex(k, j + 1, out array[2]);
							callback.ProcessTriangle(array, k, j);
						}
						else
						{
							GetVertex(k, j, out array[0]);
							GetVertex(k, j + 1, out array[1]);
							GetVertex(k + 1, j, out array[2]);
							callback.ProcessTriangle(array, k, j);
							GetVertex(k + 1, j, out array[0]);
							GetVertex(k, j + 1, out array[1]);
							GetVertex(k + 1, j + 1, out array[2]);
							callback.ProcessTriangle(array, k, j);
						}
					}
				}
			}
		}

		private IndexedVector3 LocalToWorld2(IndexedVector3 local)
		{
			float num = local[m_upAxis];
			if (m_upAxis != 0)
			{
				int upAxis = m_upAxis;
				int num2 = 2;
			}
			switch (m_upAxis)
			{
			case 0:
				local = new IndexedVector3(num - m_localOrigin.X, (0f - m_width) / 2f + local.X, (0f - m_length) / 2f + local.Y);
				break;
			case 1:
				local = new IndexedVector3((0f - m_width) / 2f + local.X, num - m_localOrigin.Y, (0f - m_length) / 2f + local.Z);
				break;
			case 2:
				local = new IndexedVector3((0f - m_width) / 2f + local.X, (0f - m_length) / 2f + local.Y, num - m_localOrigin.Z);
				break;
			default:
				local = IndexedVector3.Zero;
				break;
			}
			local *= m_localScaling;
			local.Y -= 20f;
			return local;
		}

		private IndexedVector3 LocalToWorld(IndexedVector3 local)
		{
			IndexedVector3 indexedVector = local * m_localScaling;
			return indexedVector - m_localOrigin;
		}

		public void QueryNode(QuadTreeNode node, ObjectArray<QuadTreeNode> results, IndexedVector3 source, IndexedVector3 direction)
		{
			if (node.children == null)
			{
				results.Add(node);
				return;
			}
			for (int i = 0; i < 4; i++)
			{
				if (node.children[i].Intersects(source, direction))
				{
					QueryNode(node.children[i], results, source, direction);
				}
			}
		}

		public HeightfieldTerrainShape(int heightStickWidth, int heightStickLength, byte[] heightfieldData, float heightScale, float minHeight, float maxHeight, int upAxis, PHY_ScalarType heightDataType, bool flipQuadEdges)
		{
			Initialize(heightStickWidth, heightStickLength, heightfieldData, heightScale, minHeight, maxHeight, upAxis, heightDataType, flipQuadEdges);
		}

		public HeightfieldTerrainShape(int heightStickWidth, int heightStickLength, byte[] heightfieldData, float maxHeight, int upAxis, bool useFloatData, bool flipQuadEdges)
		{
			PHY_ScalarType hdt = ((!useFloatData) ? PHY_ScalarType.PHY_UCHAR : PHY_ScalarType.PHY_FLOAT);
			float minHeight = 0f;
			float heightScale = maxHeight / 65535f;
			Initialize(heightStickWidth, heightStickLength, heightfieldData, heightScale, minHeight, maxHeight, upAxis, hdt, flipQuadEdges);
		}

		public HeightfieldTerrainShape(int heightStickWidth, int heightStickLength, float[] heightfieldData, float heightScale, float minHeight, float maxHeight, int upAxis, bool flipQuadEdges)
		{
			Initialize(heightStickWidth, heightStickLength, heightfieldData, heightScale, minHeight, maxHeight, upAxis, PHY_ScalarType.PHY_FLOAT, flipQuadEdges);
		}

		protected virtual float GetRawHeightFieldValue(int x, int y)
		{
			float result = 0f;
			switch (m_heightDataType)
			{
			case PHY_ScalarType.PHY_FLOAT:
				if (m_heightFieldDataFloat != null)
				{
					int num2 = y * m_heightStickWidth + x;
					result = m_heightFieldDataFloat[num2];
				}
				else
				{
					int startIndex2 = (y * m_heightStickWidth + x) * 4;
					result = BitConverter.ToSingle(m_heightFieldDataByte, startIndex2);
				}
				break;
			case PHY_ScalarType.PHY_UCHAR:
			{
				byte b = m_heightFieldDataByte[y * m_heightStickWidth + x];
				result = (float)(int)b * m_heightScale;
				break;
			}
			case PHY_ScalarType.PHY_SHORT:
			{
				int startIndex = (y * m_heightStickWidth + x) * 2;
				short num = BitConverter.ToInt16(m_heightFieldDataByte, startIndex);
				result = (float)num * m_heightScale;
				break;
			}
			}
			return result;
		}

		protected void QuantizeWithClamp(int[] output, ref IndexedVector3 point, int isMax)
		{
			IndexedVector3 input = point;
			MathUtil.VectorClampMax(ref input, ref m_localAabbMax);
			MathUtil.VectorClampMin(ref input, ref m_localAabbMin);
			output[0] = MathUtil.GetQuantized(input.X);
			output[1] = MathUtil.GetQuantized(input.Y);
			output[2] = MathUtil.GetQuantized(input.Z);
		}

		protected void GetVertex(int x, int y, out IndexedVector3 vertex)
		{
			float rawHeightFieldValue = GetRawHeightFieldValue(x, y);
			switch (m_upAxis)
			{
			case 0:
				vertex = new IndexedVector3(rawHeightFieldValue - m_localOrigin.X, (0f - m_width) / 2f + (float)x, (0f - m_length) / 2f + (float)y);
				break;
			case 1:
				vertex = new IndexedVector3((0f - m_width) / 2f + (float)x, rawHeightFieldValue - m_localOrigin.Y, (0f - m_length) / 2f + (float)y);
				break;
			case 2:
				vertex = new IndexedVector3((0f - m_width) / 2f + (float)x, (0f - m_length) / 2f + (float)y, rawHeightFieldValue - m_localOrigin.Z);
				break;
			default:
				vertex = IndexedVector3.Zero;
				break;
			}
			IndexedVector3.Multiply(ref vertex, ref vertex, ref m_localScaling);
		}

		protected void Initialize(int heightStickWidth, int heightStickLength, object heightfieldData, float heightScale, float minHeight, float maxHeight, int upAxis, PHY_ScalarType hdt, bool flipQuadEdges)
		{
			m_shapeType = BroadphaseNativeTypes.TERRAIN_SHAPE_PROXYTYPE;
			m_heightStickWidth = heightStickWidth;
			m_heightStickLength = heightStickLength;
			m_minHeight = minHeight;
			m_maxHeight = maxHeight;
			m_width = heightStickWidth - 1;
			m_length = heightStickLength - 1;
			m_heightScale = heightScale;
			m_heightFieldDataByte = heightfieldData as byte[];
			m_heightFieldDataFloat = heightfieldData as float[];
			m_heightDataType = hdt;
			m_flipQuadEdges = flipQuadEdges;
			m_useDiamondSubdivision = false;
			m_upAxis = upAxis;
			m_localScaling = new IndexedVector3(1f);
			switch (m_upAxis)
			{
			case 0:
				m_localAabbMin = new IndexedVector3(m_minHeight, 0f, 0f);
				m_localAabbMax = new IndexedVector3(m_maxHeight, m_width, m_length);
				break;
			case 1:
				m_localAabbMin = new IndexedVector3(0f, m_minHeight, 0f);
				m_localAabbMax = new IndexedVector3(m_width, m_maxHeight, m_length);
				break;
			case 2:
				m_localAabbMin = new IndexedVector3(0f, 0f, m_minHeight);
				m_localAabbMax = new IndexedVector3(m_width, m_length, m_maxHeight);
				break;
			}
			m_localOrigin = 0.5f * (m_localAabbMin + m_localAabbMax);
		}

		public void SetUseDiamondSubdivision(bool useDiamondSubdivision)
		{
			m_useDiamondSubdivision = useDiamondSubdivision;
		}

		public override void GetAabb(ref IndexedMatrix t, out IndexedVector3 aabbMin, out IndexedVector3 aabbMax)
		{
			IndexedVector3 v = (m_localAabbMax - m_localAabbMin) * m_localScaling * 0.5f;
			IndexedVector3 zero = IndexedVector3.Zero;
			zero[m_upAxis] = (m_minHeight + m_maxHeight) * 0.5f;
			zero *= m_localScaling;
			IndexedBasisMatrix indexedBasisMatrix = t._basis.Absolute();
			IndexedVector3 origin = t._origin;
			IndexedVector3 indexedVector = new IndexedVector3(indexedBasisMatrix._el0.Dot(ref v), indexedBasisMatrix._el1.Dot(ref v), indexedBasisMatrix._el2.Dot(ref v));
			indexedVector += new IndexedVector3(GetMargin());
			aabbMin = origin - indexedVector;
			aabbMax = origin + indexedVector;
		}

		public override void ProcessAllTriangles(ITriangleCallback callback, ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax)
		{
			IndexedVector3 indexedVector = new IndexedVector3(1f) / m_localScaling;
			IndexedVector3 point = aabbMin * indexedVector;
			IndexedVector3 point2 = aabbMax * indexedVector;
			point += m_localOrigin;
			point2 += m_localOrigin;
			QuantizeWithClamp(quantizedAabbMin, ref point, 0);
			QuantizeWithClamp(quantizedAabbMax, ref point2, 1);
			for (int i = 0; i < 3; i++)
			{
				quantizedAabbMin[i]--;
				quantizedAabbMax[i]++;
			}
			int num = 0;
			int num2 = m_heightStickWidth - 1;
			int num3 = 0;
			int num4 = m_heightStickLength - 1;
			switch (m_upAxis)
			{
			case 0:
				if (quantizedAabbMin[1] > num)
				{
					num = quantizedAabbMin[1];
				}
				if (quantizedAabbMax[1] < num2)
				{
					num2 = quantizedAabbMax[1];
				}
				if (quantizedAabbMin[2] > num3)
				{
					num3 = quantizedAabbMin[2];
				}
				if (quantizedAabbMax[2] < num4)
				{
					num4 = quantizedAabbMax[2];
				}
				break;
			case 1:
				if (quantizedAabbMin[0] > num)
				{
					num = quantizedAabbMin[0];
				}
				if (quantizedAabbMax[0] < num2)
				{
					num2 = quantizedAabbMax[0];
				}
				if (quantizedAabbMin[2] > num3)
				{
					num3 = quantizedAabbMin[2];
				}
				if (quantizedAabbMax[2] < num4)
				{
					num4 = quantizedAabbMax[2];
				}
				break;
			case 2:
				if (quantizedAabbMin[0] > num)
				{
					num = quantizedAabbMin[0];
				}
				if (quantizedAabbMax[0] < num2)
				{
					num2 = quantizedAabbMax[0];
				}
				if (quantizedAabbMin[1] > num3)
				{
					num3 = quantizedAabbMin[1];
				}
				if (quantizedAabbMax[1] < num4)
				{
					num4 = quantizedAabbMax[1];
				}
				break;
			}
			for (int j = num3; j < num4; j++)
			{
				for (int k = num; k < num2; k++)
				{
					if (m_flipQuadEdges || (m_useDiamondSubdivision && ((j + k) & 1) > 0))
					{
						GetVertex(k, j, out vertices[0]);
						GetVertex(k + 1, j, out vertices[1]);
						GetVertex(k + 1, j + 1, out vertices[2]);
						callback.ProcessTriangle(vertices, k, j);
						GetVertex(k, j, out vertices[0]);
						GetVertex(k + 1, j + 1, out vertices[1]);
						GetVertex(k, j + 1, out vertices[2]);
						callback.ProcessTriangle(vertices, k, j);
					}
					else
					{
						GetVertex(k, j, out vertices[0]);
						GetVertex(k, j + 1, out vertices[1]);
						GetVertex(k + 1, j, out vertices[2]);
						callback.ProcessTriangle(vertices, k, j);
						GetVertex(k + 1, j, out vertices[0]);
						GetVertex(k, j + 1, out vertices[1]);
						GetVertex(k + 1, j + 1, out vertices[2]);
						callback.ProcessTriangle(vertices, k, j);
					}
				}
			}
		}

		public override void CalculateLocalInertia(float mass, out IndexedVector3 inertia)
		{
			inertia = IndexedVector3.Zero;
		}

		public override void SetLocalScaling(ref IndexedVector3 scaling)
		{
			m_localScaling = scaling;
		}

		public override IndexedVector3 GetLocalScaling()
		{
			return m_localScaling;
		}

		public override string GetName()
		{
			return "HEIGHTFIELD";
		}
	}
}
