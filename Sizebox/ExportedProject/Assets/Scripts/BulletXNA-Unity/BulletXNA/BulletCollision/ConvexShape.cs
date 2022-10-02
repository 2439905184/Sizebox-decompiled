using System;
using System.Collections.Generic;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public abstract class ConvexShape : CollisionShape
	{
		public const int MAX_PREFERRED_PENETRATION_DIRECTIONS = 10;

		public ConvexShape()
		{
		}

		public override void Cleanup()
		{
			base.Cleanup();
		}

		public IndexedVector3 LocalGetSupportingVertex(IndexedVector3 vec)
		{
			return LocalGetSupportingVertex(ref vec);
		}

		public virtual IndexedVector3 LocalGetSupportingVertex(ref IndexedVector3 vec)
		{
			return IndexedVector3.Zero;
		}

		public virtual IndexedVector3 LocalGetSupportingVertexWithoutMargin(ref IndexedVector3 vec)
		{
			return IndexedVector3.Zero;
		}

		public IndexedVector3 LocalGetSupportVertexWithoutMarginNonVirtual(IndexedVector3 localDir)
		{
			return LocalGetSupportVertexWithoutMarginNonVirtual(ref localDir);
		}

		public IndexedVector3 LocalGetSupportVertexWithoutMarginNonVirtual(ref IndexedVector3 localDir)
		{
			IndexedVector3 zero = IndexedVector3.Zero;
			switch (m_shapeType)
			{
			case BroadphaseNativeTypes.SPHERE_SHAPE_PROXYTYPE:
				zero = default(IndexedVector3);
				break;
			case BroadphaseNativeTypes.BOX_SHAPE_PROXYTYPE:
			{
				BoxShape boxShape = this as BoxShape;
				IndexedVector3 implicitShapeDimensions = boxShape.GetImplicitShapeDimensions();
				zero = new IndexedVector3(MathUtil.FSel(localDir.X, implicitShapeDimensions.X, 0f - implicitShapeDimensions.X), MathUtil.FSel(localDir.Y, implicitShapeDimensions.Y, 0f - implicitShapeDimensions.Y), MathUtil.FSel(localDir.Z, implicitShapeDimensions.Z, 0f - implicitShapeDimensions.Z));
				break;
			}
			case BroadphaseNativeTypes.TRIANGLE_SHAPE_PROXYTYPE:
			{
				TriangleShape triangleShape = (TriangleShape)this;
				IndexedVector3 a2 = localDir;
				IndexedVector3[] vertices = triangleShape.m_vertices1;
				IndexedVector3 a3 = new IndexedVector3(IndexedVector3.Dot(ref a2, ref vertices[0]), IndexedVector3.Dot(ref a2, ref vertices[1]), IndexedVector3.Dot(ref a2, ref vertices[2]));
				int num11 = MathUtil.MaxAxis(ref a3);
				IndexedVector3 indexedVector7 = vertices[num11];
				zero = indexedVector7;
				break;
			}
			case BroadphaseNativeTypes.CYLINDER_SHAPE_PROXYTYPE:
			{
				CylinderShape cylinderShape = (CylinderShape)this;
				IndexedVector3 implicitShapeDimensions2 = cylinderShape.GetImplicitShapeDimensions();
				IndexedVector3 indexedVector5 = localDir;
				int upAxis2 = cylinderShape.GetUpAxis();
				int i = 1;
				int i2 = 0;
				int i3 = 2;
				switch (upAxis2)
				{
				case 0:
					i = 1;
					i2 = 0;
					i3 = 2;
					break;
				case 1:
					i = 0;
					i2 = 1;
					i3 = 2;
					break;
				case 2:
					i = 0;
					i2 = 2;
					i3 = 1;
					break;
				}
				float num5 = implicitShapeDimensions2[i];
				float num6 = implicitShapeDimensions2[upAxis2];
				IndexedVector3 indexedVector6 = default(IndexedVector3);
				float num7 = indexedVector5[i];
				float num8 = indexedVector5[i3];
				float num9 = (float)Math.Sqrt(num7 * num7 + num8 * num8);
				if (num9 != 0f)
				{
					float num10 = num5 / num9;
					indexedVector6[i] = indexedVector5[i] * num10;
					indexedVector6[i2] = ((indexedVector5[i2] < 0f) ? (0f - num6) : num6);
					indexedVector6[i3] = indexedVector5[i3] * num10;
					zero = indexedVector6;
				}
				else
				{
					indexedVector6[i] = num5;
					indexedVector6[i2] = ((indexedVector5[i2] < 0f) ? (0f - num6) : num6);
					indexedVector6[i3] = 0f;
					zero = indexedVector6;
				}
				break;
			}
			case BroadphaseNativeTypes.CAPSULE_SHAPE_PROXYTYPE:
			{
				IndexedVector3 indexedVector = localDir;
				CapsuleShape capsuleShape = this as CapsuleShape;
				float halfHeight = capsuleShape.GetHalfHeight();
				int upAxis = capsuleShape.GetUpAxis();
				float radius = capsuleShape.GetRadius();
				IndexedVector3 indexedVector2 = default(IndexedVector3);
				float num = float.MinValue;
				IndexedVector3 a = indexedVector;
				float num2 = a.LengthSquared();
				if (num2 < 0.0001f)
				{
					a = new IndexedVector3(1f, 0f, 0f);
				}
				else
				{
					float num3 = 1f / (float)Math.Sqrt(num2);
					a *= num3;
				}
				IndexedVector3 indexedVector3 = default(IndexedVector3);
				indexedVector3[upAxis] = halfHeight;
				IndexedVector3 b = indexedVector3 + a * radius - a * capsuleShape.GetMarginNV();
				float num4 = IndexedVector3.Dot(ref a, ref b);
				if (num4 > num)
				{
					num = num4;
					indexedVector2 = b;
				}
				IndexedVector3 indexedVector4 = default(IndexedVector3);
				indexedVector4[upAxis] = 0f - halfHeight;
				b = indexedVector4 + a * radius - a * capsuleShape.GetMarginNV();
				num4 = IndexedVector3.Dot(ref a, ref b);
				if (num4 > num)
				{
					num = num4;
					indexedVector2 = b;
				}
				zero = indexedVector2;
				break;
			}
			case BroadphaseNativeTypes.CONVEX_POINT_CLOUD_SHAPE_PROXYTYPE:
			{
				ConvexPointCloudShape convexPointCloudShape = (ConvexPointCloudShape)this;
				IList<IndexedVector3> unscaledPoints2 = convexPointCloudShape.GetUnscaledPoints();
				int numPoints2 = convexPointCloudShape.GetNumPoints();
				IndexedVector3 localScaling2 = convexPointCloudShape.GetLocalScalingNV();
				zero = ConvexHullSupport(ref localDir, unscaledPoints2, numPoints2, ref localScaling2);
				break;
			}
			case BroadphaseNativeTypes.CONVEX_HULL_SHAPE_PROXYTYPE:
			{
				ConvexHullShape convexHullShape = (ConvexHullShape)this;
				IList<IndexedVector3> unscaledPoints = convexHullShape.GetUnscaledPoints();
				int numPoints = convexHullShape.GetNumPoints();
				IndexedVector3 localScaling = convexHullShape.GetLocalScalingNV();
				zero = ConvexHullSupport(ref localDir, unscaledPoints, numPoints, ref localScaling);
				break;
			}
			default:
				zero = LocalGetSupportingVertexWithoutMargin(ref localDir);
				break;
			}
			return zero;
		}

		public IndexedVector3 LocalGetSupportVertexNonVirtual(ref IndexedVector3 localDir)
		{
			IndexedVector3 v = localDir;
			if (v.LengthSquared() < 1.4210855E-14f)
			{
				v = new IndexedVector3(-1f);
			}
			v = IndexedVector3.Normalize(ref v);
			return LocalGetSupportVertexWithoutMarginNonVirtual(ref v) + GetMarginNonVirtual() * v;
		}

		public float GetMarginNonVirtual()
		{
			switch (m_shapeType)
			{
			case BroadphaseNativeTypes.SPHERE_SHAPE_PROXYTYPE:
			{
				SphereShape sphereShape = this as SphereShape;
				return sphereShape.GetRadius();
			}
			case BroadphaseNativeTypes.BOX_SHAPE_PROXYTYPE:
			{
				BoxShape boxShape = this as BoxShape;
				return boxShape.GetMarginNV();
			}
			case BroadphaseNativeTypes.TRIANGLE_SHAPE_PROXYTYPE:
			{
				TriangleShape triangleShape = this as TriangleShape;
				return triangleShape.GetMarginNV();
			}
			case BroadphaseNativeTypes.CYLINDER_SHAPE_PROXYTYPE:
			{
				CylinderShape cylinderShape = this as CylinderShape;
				return cylinderShape.GetMarginNV();
			}
			case BroadphaseNativeTypes.CAPSULE_SHAPE_PROXYTYPE:
			{
				CapsuleShape capsuleShape = this as CapsuleShape;
				return capsuleShape.GetMarginNV();
			}
			case BroadphaseNativeTypes.CONVEX_HULL_SHAPE_PROXYTYPE:
			case BroadphaseNativeTypes.CONVEX_POINT_CLOUD_SHAPE_PROXYTYPE:
			{
				PolyhedralConvexShape polyhedralConvexShape = this as PolyhedralConvexShape;
				return polyhedralConvexShape.GetMarginNV();
			}
			default:
				return GetMargin();
			}
		}

		public virtual void GetAabbNonVirtual(ref IndexedMatrix t, ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax)
		{
			switch (m_shapeType)
			{
			case BroadphaseNativeTypes.SPHERE_SHAPE_PROXYTYPE:
			{
				SphereShape sphereShape = this as SphereShape;
				float x = sphereShape.GetImplicitShapeDimensions().X;
				float x2 = x + sphereShape.GetMarginNonVirtual();
				IndexedVector3 origin2 = t._origin;
				IndexedVector3 indexedVector4 = new IndexedVector3(x2);
				aabbMin = origin2 - indexedVector4;
				aabbMax = origin2 + indexedVector4;
				break;
			}
			case BroadphaseNativeTypes.BOX_SHAPE_PROXYTYPE:
			case BroadphaseNativeTypes.CYLINDER_SHAPE_PROXYTYPE:
			{
				BoxShape boxShape = this as BoxShape;
				float marginNonVirtual4 = boxShape.GetMarginNonVirtual();
				IndexedVector3 v2 = boxShape.GetImplicitShapeDimensions();
				v2 += new IndexedVector3(marginNonVirtual4);
				IndexedBasisMatrix indexedBasisMatrix2 = t._basis.Absolute();
				IndexedVector3 origin3 = t._origin;
				IndexedVector3 indexedVector5 = new IndexedVector3(indexedBasisMatrix2._el0.Dot(ref v2), indexedBasisMatrix2._el1.Dot(ref v2), indexedBasisMatrix2._el2.Dot(ref v2));
				aabbMin = origin3 - indexedVector5;
				aabbMax = origin3 + indexedVector5;
				break;
			}
			case BroadphaseNativeTypes.TRIANGLE_SHAPE_PROXYTYPE:
			{
				TriangleShape triangleShape = (TriangleShape)this;
				float marginNonVirtual3 = triangleShape.GetMarginNonVirtual();
				for (int i = 0; i < 3; i++)
				{
					IndexedVector3 indexedVector2 = default(IndexedVector3);
					indexedVector2[i] = 1f;
					IndexedVector3 indexedVector3 = LocalGetSupportVertexWithoutMarginNonVirtual(indexedVector2 * t._basis);
					aabbMax[i] = (t * indexedVector3)[i] + marginNonVirtual3;
					indexedVector2[i] = -1f;
					aabbMin[i] = (t * LocalGetSupportVertexWithoutMarginNonVirtual(indexedVector2 * t._basis))[i] - marginNonVirtual3;
				}
				break;
			}
			case BroadphaseNativeTypes.CAPSULE_SHAPE_PROXYTYPE:
			{
				CapsuleShape capsuleShape = this as CapsuleShape;
				float radius = capsuleShape.GetRadius();
				IndexedVector3 v = new IndexedVector3(radius);
				int upAxis = capsuleShape.GetUpAxis();
				v[upAxis] = radius + capsuleShape.GetHalfHeight();
				float marginNonVirtual2 = capsuleShape.GetMarginNonVirtual();
				v += new IndexedVector3(marginNonVirtual2);
				IndexedBasisMatrix indexedBasisMatrix = t._basis.Absolute();
				IndexedVector3 origin = t._origin;
				IndexedVector3 indexedVector = new IndexedVector3(indexedBasisMatrix._el0.Dot(ref v), indexedBasisMatrix._el1.Dot(ref v), indexedBasisMatrix._el2.Dot(ref v));
				aabbMin = origin - indexedVector;
				aabbMax = origin + indexedVector;
				break;
			}
			case BroadphaseNativeTypes.CONVEX_HULL_SHAPE_PROXYTYPE:
			case BroadphaseNativeTypes.CONVEX_POINT_CLOUD_SHAPE_PROXYTYPE:
			{
				PolyhedralConvexAabbCachingShape polyhedralConvexAabbCachingShape = (PolyhedralConvexAabbCachingShape)this;
				float marginNonVirtual = polyhedralConvexAabbCachingShape.GetMarginNonVirtual();
				polyhedralConvexAabbCachingShape.GetNonvirtualAabb(ref t, out aabbMin, out aabbMax, marginNonVirtual);
				break;
			}
			default:
				GetAabb(ref t, out aabbMin, out aabbMax);
				break;
			}
		}

		public virtual void Project(ref IndexedMatrix trans, ref IndexedVector3 dir, ref float min, ref float max)
		{
			IndexedVector3 indexedVector = dir * trans._basis;
			IndexedVector3 indexedVector2 = trans * LocalGetSupportingVertex(indexedVector);
			IndexedVector3 indexedVector3 = trans * LocalGetSupportingVertex(-indexedVector);
			min = indexedVector2.Dot(dir);
			max = indexedVector3.Dot(dir);
			if (min > max)
			{
				float num = min;
				min = max;
				max = num;
			}
		}

		public abstract void BatchedUnitVectorGetSupportingVertexWithoutMargin(IndexedVector3[] vectors, IndexedVector4[] supportVerticesOut, int numVectors);

		public abstract void GetAabbSlow(ref IndexedMatrix t, out IndexedVector3 aabbMin, out IndexedVector3 aabbMax);

		public abstract int GetNumPreferredPenetrationDirections();

		public abstract void GetPreferredPenetrationDirection(int index, out IndexedVector3 penetrationVector);

		public static IndexedVector3 ConvexHullSupport(ref IndexedVector3 localDirOrg, IList<IndexedVector3> points, int numPoints, ref IndexedVector3 localScaling)
		{
			IndexedVector3 a = localDirOrg * localScaling;
			float num = -1E+18f;
			int index = -1;
			for (int i = 0; i < numPoints; i++)
			{
				float num2 = IndexedVector3.Dot(a, points[i]);
				if (num2 > num)
				{
					num = num2;
					index = i;
				}
			}
			return points[index] * localScaling;
		}
	}
}
