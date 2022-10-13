using System;
using BulletXNA.LinearMath;
using UnityEngine;

namespace MMD4MecanimInternal.Bullet;

public static class Math
{
	public static IndexedVector3 UnityToBulletPosition(ref IndexedVector3 v, float scale)
	{
		return new IndexedVector3(0f - v.X, v.Y, v.Z) * scale;
	}

	public static IndexedVector3 BulletToUnityPosition(ref IndexedVector3 v, float scale)
	{
		return new IndexedVector3(0f - v.X, v.Y, v.Z) * scale;
	}

	public static IndexedQuaternion UnityToBulletRotation(ref IndexedQuaternion r)
	{
		return new IndexedQuaternion(r.X, 0f - r.Y, 0f - r.Z, r.W);
	}

	public static IndexedQuaternion BulletToUnityRotation(ref IndexedQuaternion r)
	{
		return new IndexedQuaternion(r.X, 0f - r.Y, 0f - r.Z, r.W);
	}

	public static IndexedVector3 UnityToBulletPosition(IndexedVector3 v, float scale)
	{
		return new IndexedVector3(0f - v.X, v.Y, v.Z) * scale;
	}

	public static IndexedVector3 BulletToUnityPosition(IndexedVector3 v, float scale)
	{
		return new IndexedVector3(0f - v.X, v.Y, v.Z) * scale;
	}

	public static IndexedQuaternion UnityToBulletRotation(IndexedQuaternion r)
	{
		return new IndexedQuaternion(r.X, 0f - r.Y, 0f - r.Z, r.W);
	}

	public static IndexedQuaternion BulletToUnityRotation(IndexedQuaternion r)
	{
		return new IndexedQuaternion(r.X, 0f - r.Y, 0f - r.Z, r.W);
	}

	public static IndexedBasisMatrix GetBasis(ref IndexedVector3 dirX, ref IndexedVector3 dirY, ref IndexedVector3 dirZ)
	{
		return new IndexedBasisMatrix(dirX.X, dirY.X, dirZ.X, dirX.Y, dirY.Y, dirZ.Y, dirX.Z, dirY.Z, dirZ.Z);
	}

	public static bool Normalize(ref IndexedVector3 v)
	{
		float num = v.Length();
		if (num > float.Epsilon)
		{
			v *= 1f / num;
			return true;
		}
		return false;
	}

	public static float NormalizeAngle(float r)
	{
		float num = (float)System.Math.PI;
		float num2 = (float)System.Math.PI * 2f;
		if (r > num)
		{
			while (r > num)
			{
				r -= num2;
			}
		}
		else if (r < 0f - num)
		{
			while (r < 0f - num)
			{
				r += num2;
			}
		}
		return r;
	}

	public static float ClampAngle(float r, float lower, float upper)
	{
		if (r >= lower && r <= upper)
		{
			return r;
		}
		float num = Mathf.Abs(NormalizeAngle(r - lower));
		float num2 = Mathf.Abs(NormalizeAngle(r - upper));
		if (num <= num2)
		{
			return lower;
		}
		return upper;
	}

	public static IndexedVector3 ClampDirection(IndexedVector3 prevDir, IndexedVector3 dir, float dirDot, float limitTheta, float limitTheta2)
	{
		if (dirDot > 0.01f)
		{
			IndexedVector3 indexedVector = prevDir * limitTheta;
			float num = limitTheta / dirDot;
			IndexedVector3 indexedVector2 = dir * num;
			IndexedVector3 indexedVector3 = indexedVector2 - indexedVector;
			float num2 = Mathf.Sqrt(num * num - limitTheta2);
			return indexedVector + indexedVector3 * (1f / num2 * Mathf.Sqrt(1f - limitTheta2));
		}
		return prevDir;
	}

	public static IndexedVector3 GetReflVector(IndexedVector3 normal, IndexedVector3 ray)
	{
		return ray - 2f * normal.Dot(ray) * normal;
	}

	public static IndexedVector3 GetAngAccVector(IndexedVector3 prev, IndexedVector3 prev2)
	{
		Vector3 vector = GetReflVector(prev, -prev2);
		return GetReflVector(vector, -prev);
	}

	public static void BlendTransform(ref IndexedMatrix m, ref IndexedMatrix lhs, ref IndexedMatrix rhs, float lhsRate)
	{
		float num = 1f - lhsRate;
		IndexedVector3 v = lhs._basis.GetColumn(0) * lhsRate + rhs._basis.GetColumn(0) * num;
		IndexedVector3 indexedVector = lhs._basis.GetColumn(2) * lhsRate + rhs._basis.GetColumn(2) * num;
		float num2 = v.Length();
		float num3 = indexedVector.Length();
		if (num2 < 0.01f || num3 < 0.01f)
		{
			m = lhs;
			return;
		}
		v *= 1f / num2;
		IndexedVector3 v2 = (indexedVector * (1f / num3)).Cross(ref v);
		float num4 = v2.Length();
		if (num4 < 0.01f)
		{
			m = lhs;
			return;
		}
		v2 *= 1f / num4;
		indexedVector = v.Cross(ref v2);
		m._basis[0] = new IndexedVector3(v[0], v2[0], indexedVector[0]);
		m._basis[1] = new IndexedVector3(v[1], v2[1], indexedVector[1]);
		m._basis[2] = new IndexedVector3(v[2], v2[2], indexedVector[2]);
		m._origin = lhs._origin * lhsRate + rhs._origin * num;
	}

	public static bool HitTestSphereToSphere(ref Vector3 translateAtoB, Vector3 spherePosA, Vector3 spherePosB, float lengthAtoB, float lengthAtoB2)
	{
		translateAtoB = spherePosB - spherePosA;
		float sqrMagnitude = translateAtoB.sqrMagnitude;
		if (sqrMagnitude < lengthAtoB2)
		{
			float num = Mathf.Sqrt(sqrMagnitude);
			if (num > float.Epsilon)
			{
				translateAtoB *= 1f / num * (lengthAtoB - num);
				return true;
			}
		}
		return false;
	}

	public static bool HitTestSphereToCapsule(ref Vector3 translateOrig, ref Vector3 translateAtoB, Vector3 r_spherePos, float sphereRadius, float cylinderHeightH, float cylinderRadius, float lengthAtoB, float lengthAtoB2)
	{
		translateOrig.Set(0f, 0f, 0f);
		translateAtoB.Set(0f, 0f, 0f);
		float num = r_spherePos[0] * r_spherePos[0] + r_spherePos[2] * r_spherePos[2];
		if (num < lengthAtoB2)
		{
			float num2 = Mathf.Abs(r_spherePos[1]);
			if (num2 < cylinderHeightH)
			{
				float num3 = Mathf.Sqrt(num);
				if (num3 > float.Epsilon)
				{
					translateAtoB.Set(0f - r_spherePos[0], 0f, 0f - r_spherePos[2]);
					translateAtoB *= 1f / num3;
					translateAtoB *= lengthAtoB - num3;
					translateOrig.Set(0f, r_spherePos[1], 0f);
					return true;
				}
				return false;
			}
			float num4 = num + (num2 - cylinderHeightH) * (num2 - cylinderHeightH);
			if (num4 < lengthAtoB2)
			{
				float num5 = Mathf.Sqrt(num4);
				if (num5 > float.Epsilon)
				{
					if (r_spherePos[1] >= 0f)
					{
						translateOrig.Set(0f, cylinderHeightH, 0f);
					}
					else
					{
						translateOrig.Set(0f, 0f - cylinderHeightH, 0f);
					}
					translateAtoB = translateOrig - r_spherePos;
					translateAtoB *= 1f / num5;
					translateAtoB *= lengthAtoB - num5;
					return true;
				}
				return false;
			}
		}
		return false;
	}

	public static bool HitTestSphereToBox(ref Vector3 translateOrig, ref Vector3 translateAtoB, Vector3 r_spherePos, float sphereRadius, float sphereRadius2, Vector3 boxSizeH)
	{
		translateOrig.Set(0f, 0f, 0f);
		translateAtoB.Set(0f, 0f, 0f);
		float num = Mathf.Abs(r_spherePos[0]);
		float num2 = Mathf.Abs(r_spherePos[1]);
		float num3 = Mathf.Abs(r_spherePos[2]);
		int num4 = -1;
		bool flag = num <= boxSizeH[0];
		bool flag2 = num2 <= boxSizeH[1];
		bool flag3 = num3 <= boxSizeH[2];
		if (flag && flag2 && flag3)
		{
			if (boxSizeH[0] <= float.Epsilon || boxSizeH[1] <= float.Epsilon || boxSizeH[2] <= float.Epsilon)
			{
				return false;
			}
			bool flag4 = num <= float.Epsilon;
			bool flag5 = num2 <= float.Epsilon;
			bool flag6 = num3 <= float.Epsilon;
			float num5 = boxSizeH[1] / boxSizeH[2];
			float num6 = boxSizeH[2] / boxSizeH[0];
			float num7 = boxSizeH[1] / boxSizeH[0];
			if (!flag4 || !flag5 || !flag6)
			{
				if (flag4 && flag5)
				{
					num4 = 2;
				}
				else if (flag4 && flag6)
				{
					num4 = 1;
				}
				else if (flag5 && flag6)
				{
					num4 = 0;
				}
				else if (flag4)
				{
					float num8 = num2 / num3;
					num4 = ((num8 > num5) ? 1 : 2);
				}
				else if (flag5)
				{
					float num9 = num3 / num;
					num4 = ((num9 > num6) ? 2 : 0);
				}
				else if (flag6)
				{
					float num10 = num2 / num;
					num4 = ((num10 > num7) ? 1 : 0);
				}
				else
				{
					float num11 = num2 / num;
					if (num11 > num7)
					{
						float num12 = num2 / num3;
						num4 = ((num12 > num5) ? 1 : 2);
					}
					else
					{
						float num13 = num3 / num;
						num4 = ((num13 > num6) ? 2 : 0);
					}
				}
			}
		}
		else if (num < boxSizeH[0] + sphereRadius && flag2 && flag3)
		{
			num4 = 0;
		}
		else if (flag && num2 < boxSizeH[1] + sphereRadius && flag3)
		{
			num4 = 1;
		}
		else if (flag && flag2 && num3 < boxSizeH[2] + sphereRadius)
		{
			num4 = 2;
		}
		switch (num4)
		{
		case 0:
		{
			float num19 = boxSizeH[0] - num + sphereRadius;
			translateOrig.Set(0f, r_spherePos[1], r_spherePos[2]);
			translateAtoB.Set((r_spherePos[0] >= 0f) ? (0f - num19) : num19, 0f, 0f);
			return true;
		}
		case 1:
		{
			float num20 = boxSizeH[1] - num2 + sphereRadius;
			translateOrig.Set(r_spherePos[0], 0f, r_spherePos[2]);
			translateAtoB.Set(0f, (r_spherePos[1] >= 0f) ? (0f - num20) : num20, 0f);
			return true;
		}
		case 2:
		{
			float num18 = boxSizeH[2] - num3 + sphereRadius;
			translateOrig.Set(r_spherePos[0], r_spherePos[1], 0f);
			translateAtoB.Set(0f, 0f, (r_spherePos[2] >= 0f) ? (0f - num18) : num18);
			return true;
		}
		default:
		{
			translateOrig.x = ((r_spherePos[0] >= 0f) ? boxSizeH[0] : (0f - boxSizeH[0]));
			translateOrig.y = ((r_spherePos[1] >= 0f) ? boxSizeH[1] : (0f - boxSizeH[1]));
			translateOrig.z = ((r_spherePos[2] >= 0f) ? boxSizeH[2] : (0f - boxSizeH[2]));
			float sqrMagnitude = (r_spherePos - translateOrig).sqrMagnitude;
			if (sqrMagnitude < sphereRadius2)
			{
				float num14 = Mathf.Sqrt(sqrMagnitude);
				if (num14 > float.Epsilon)
				{
					translateAtoB = translateOrig - r_spherePos;
					translateAtoB *= 1f / num14 * (sphereRadius - num14);
					Vector3 vector = r_spherePos - translateAtoB;
					float num15 = Mathf.Abs(vector.x);
					float num16 = Mathf.Abs(vector.y);
					float num17 = Mathf.Abs(vector.z);
					bool flag7 = num15 <= boxSizeH.x;
					bool flag8 = num16 <= boxSizeH.y;
					bool flag9 = num17 <= boxSizeH.z;
					if (!flag7 || !flag8 || !flag9)
					{
						return true;
					}
				}
			}
			return false;
		}
		}
	}

	private static void _FeedbackImpulse(MMDCollider colliderA, Vector3 translateAtoB, Vector3 translateOrig)
	{
		colliderA.isCollision = true;
		colliderA.transform._origin -= translateAtoB;
	}

	private static bool _FastCollideStoK(MMDCollider colliderA, MMDCollider colliderB)
	{
		IndexedMatrix transform = colliderB.transform;
		IndexedMatrix indexedMatrix = colliderB.transform.Inverse();
		IndexedMatrix transform2 = colliderA.transform;
		MMDColliderCircles circles = colliderA.circles;
		Vector3[] transformVertices = colliderA.circles.GetTransformVertices();
		int num = transformVertices.Length;
		Vector3 translateOrig = Vector3.zero;
		Vector3 translateAtoB = Vector3.zero;
		switch (colliderB.shape)
		{
		case 0:
		{
			float num3 = circles.GetRadius() + colliderB.size[0];
			float lengthAtoB2 = num3 * num3;
			Vector3 spherePosB = colliderB.transform._origin;
			Vector3 zero3 = Vector3.zero;
			circles.Transform(transform2);
			for (int k = 0; k != num; k++)
			{
				if (HitTestSphereToSphere(ref translateAtoB, transformVertices[k], spherePosB, num3, lengthAtoB2))
				{
					translateOrig = colliderA.transform._origin;
					zero3 -= translateAtoB;
					_FeedbackImpulse(colliderA, translateAtoB, translateOrig);
				}
			}
			return colliderA.isCollision;
		}
		case 1:
		{
			float radius2 = circles.GetRadius();
			float radius3 = circles.GetRadius2();
			Vector3 size = colliderB.size;
			Vector3 zero2 = Vector3.zero;
			circles.Transform(indexedMatrix * transform2);
			for (int j = 0; j != num; j++)
			{
				if (HitTestSphereToBox(ref translateOrig, ref translateAtoB, transformVertices[j] + zero2, radius2, radius3, size))
				{
					zero2 -= translateAtoB;
					translateAtoB = transform._basis * translateAtoB;
					_FeedbackImpulse(colliderA, translateAtoB, translateOrig);
				}
			}
			return colliderA.isCollision;
		}
		case 2:
		{
			float radius = circles.GetRadius();
			float num2 = circles.GetRadius() + colliderB.size[0];
			float lengthAtoB = num2 * num2;
			float cylinderHeightH = Mathf.Max(colliderB.size[1] * 0.5f, 0f);
			float cylinderRadius = colliderB.size[0];
			Vector3 zero = Vector3.zero;
			circles.Transform(indexedMatrix * transform2);
			for (int i = 0; i != num; i++)
			{
				if (HitTestSphereToCapsule(ref translateOrig, ref translateAtoB, transformVertices[i] + zero, radius, cylinderHeightH, cylinderRadius, num2, lengthAtoB))
				{
					zero -= translateAtoB;
					translateAtoB = transform._basis * translateAtoB;
					_FeedbackImpulse(colliderA, translateAtoB, translateOrig);
				}
			}
			return colliderA.isCollision;
		}
		default:
			return false;
		}
	}

	public static bool FastCollide(MMDCollider colliderA, MMDCollider colliderB)
	{
		colliderA.Prepare();
		colliderB.Prepare();
		if (colliderA.isKinematic && colliderB.isKinematic)
		{
			return false;
		}
		if (colliderA.isKinematic)
		{
			return _FastCollideStoK(colliderB, colliderA);
		}
		if (colliderB.isKinematic)
		{
			return _FastCollideStoK(colliderA, colliderB);
		}
		return false;
	}

	public static IndexedMatrix MakeIndexedMatrix(ref Vector3 position, ref Quaternion rotation)
	{
		IndexedQuaternion q = new IndexedQuaternion(ref rotation);
		IndexedBasisMatrix basis = new IndexedBasisMatrix(q);
		IndexedVector3 origin = new IndexedVector3(ref position);
		return new IndexedMatrix(basis, origin);
	}

	public static void MakeIndexedMatrix(ref IndexedMatrix matrix, ref Vector3 position, ref Quaternion rotation)
	{
		matrix.SetRotation(new IndexedQuaternion(ref rotation));
		matrix._origin = position;
	}

	public static IndexedMatrix MakeIndexedMatrix(ref Matrix4x4 m)
	{
		return new IndexedMatrix(m.m00, m.m01, m.m02, m.m10, m.m11, m.m12, m.m20, m.m21, m.m22, m.m03, m.m13, m.m23);
	}

	public static void CopyBasis(ref IndexedBasisMatrix m0, ref Matrix4x4 m1)
	{
		m0._el0.X = m1.m00;
		m0._el1.X = m1.m10;
		m0._el2.X = m1.m20;
		m0._el0.Y = m1.m01;
		m0._el1.Y = m1.m11;
		m0._el2.Y = m1.m21;
		m0._el0.Z = m1.m02;
		m0._el1.Z = m1.m12;
		m0._el2.Z = m1.m22;
	}

	public static void CopyBasis(ref Matrix4x4 m0, ref IndexedBasisMatrix m1)
	{
		m0.m00 = m1._el0.X;
		m0.m10 = m1._el1.X;
		m0.m20 = m1._el2.X;
		m0.m01 = m1._el0.Y;
		m0.m11 = m1._el1.Y;
		m0.m21 = m1._el2.Y;
		m0.m02 = m1._el0.Z;
		m0.m12 = m1._el1.Z;
		m0.m22 = m1._el2.Z;
	}

	public static void CopyBasis(ref Matrix4x4 m0, ref Matrix4x4 m1)
	{
		m0.m00 = m1.m00;
		m0.m10 = m1.m10;
		m0.m20 = m1.m20;
		m0.m01 = m1.m01;
		m0.m11 = m1.m11;
		m0.m21 = m1.m21;
		m0.m02 = m1.m02;
		m0.m12 = m1.m12;
		m0.m22 = m1.m22;
	}

	public static void SetRotation(ref Matrix4x4 m, ref Quaternion q)
	{
		float num = q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w;
		float num2 = ((num > float.Epsilon) ? (2f / num) : 0f);
		float num3 = q.x * num2;
		float num4 = q.y * num2;
		float num5 = q.z * num2;
		float num6 = q.w * num3;
		float num7 = q.w * num4;
		float num8 = q.w * num5;
		float num9 = q.x * num3;
		float num10 = q.x * num4;
		float num11 = q.x * num5;
		float num12 = q.y * num4;
		float num13 = q.y * num5;
		float num14 = q.z * num5;
		m.m00 = 1f - (num12 + num14);
		m.m01 = num10 - num8;
		m.m02 = num11 + num7;
		m.m10 = num10 + num8;
		m.m11 = 1f - (num9 + num14);
		m.m12 = num13 - num6;
		m.m20 = num11 - num7;
		m.m21 = num13 + num6;
		m.m22 = 1f - (num9 + num12);
	}

	public static void SetPosition(ref Matrix4x4 m, Vector3 v)
	{
		m.m03 = v.x;
		m.m13 = v.y;
		m.m23 = v.z;
	}

	public static void SetPosition(ref Matrix4x4 m, ref Vector3 v)
	{
		m.m03 = v.x;
		m.m13 = v.y;
		m.m23 = v.z;
	}

	public static Vector3 GetPosition(ref Matrix4x4 m)
	{
		return new Vector3(m.m03, m.m13, m.m23);
	}

	public static void ComputeEulerZYX(ref IndexedBasisMatrix m, ref float yaw, ref float pitch, ref float roll)
	{
		if (Mathf.Abs(m._el2.X) >= 1f)
		{
			yaw = 0f;
			float num = Mathf.Atan2(m._el0.X, m._el0.Z);
			if (Mathf.Abs(m._el2.X) > 0f)
			{
				pitch = (float)System.Math.PI / 2f;
				roll = pitch + num;
			}
			else
			{
				pitch = -(float)System.Math.PI / 2f;
				roll = 0f - pitch + num;
			}
		}
		else
		{
			pitch = 0f - Mathf.Asin(Mathf.Clamp(m._el2.X, -1f, 1f));
			float num2 = Mathf.Cos(pitch);
			if (Mathf.Abs(num2) > float.Epsilon)
			{
				float num3 = 1f / num2;
				roll = Mathf.Atan2(m._el2.Y * num3, m._el2.Z * num3);
				yaw = Mathf.Atan2(m._el1.X * num3, m._el0.X * num3);
			}
			else
			{
				roll = (yaw = 0f);
			}
		}
	}

	public static IndexedBasisMatrix EulerX(float eulerX)
	{
		float num = Mathf.Cos(eulerX);
		float num2 = Mathf.Sin(eulerX);
		return new IndexedBasisMatrix(1f, 0f, 0f, 0f, num, 0f - num2, 0f, num2, num);
	}

	public static IndexedBasisMatrix EulerY(float eulerY)
	{
		float num = Mathf.Cos(eulerY);
		float num2 = Mathf.Sin(eulerY);
		return new IndexedBasisMatrix(num, 0f, num2, 0f, 1f, 0f, 0f - num2, 0f, num);
	}

	public static IndexedBasisMatrix EulerZ(float eulerZ)
	{
		float num = Mathf.Cos(eulerZ);
		float num2 = Mathf.Sin(eulerZ);
		return new IndexedBasisMatrix(num, 0f - num2, 0f, num2, num, 0f, 0f, 0f, 1f);
	}

	public static IndexedBasisMatrix EulerZYX(float eulerX, float eulerY, float eulerZ)
	{
		float num = Mathf.Cos(eulerX);
		float num2 = Mathf.Cos(eulerY);
		float num3 = Mathf.Cos(eulerZ);
		float num4 = Mathf.Sin(eulerX);
		float num5 = Mathf.Sin(eulerY);
		float num6 = Mathf.Sin(eulerZ);
		float num7 = num * num3;
		float num8 = num * num6;
		float num9 = num4 * num3;
		float num10 = num4 * num6;
		return new IndexedBasisMatrix(num2 * num3, num5 * num9 - num8, num5 * num7 + num10, num2 * num6, num5 * num10 + num7, num5 * num8 - num9, 0f - num5, num2 * num4, num2 * num);
	}

	public static IndexedBasisMatrix BasisRotationYXZ(ref Vector3 rotation)
	{
		IndexedBasisMatrix indexedBasisMatrix = EulerX(rotation.x);
		IndexedBasisMatrix indexedBasisMatrix2 = EulerY(rotation.y);
		IndexedBasisMatrix indexedBasisMatrix3 = EulerZ(rotation.z);
		return indexedBasisMatrix2 * indexedBasisMatrix * indexedBasisMatrix3;
	}

	public static IndexedQuaternion QuaternionX(float x)
	{
		float f = x * 0.5f;
		float x2 = Mathf.Sin(f);
		return new IndexedQuaternion(x2, 0f, 0f, Mathf.Cos(f));
	}

	public static IndexedQuaternion QuaternionY(float y)
	{
		float f = y * 0.5f;
		float y2 = Mathf.Sin(f);
		return new IndexedQuaternion(0f, y2, 0f, Mathf.Cos(f));
	}

	public static IndexedQuaternion QuaternionZ(float z)
	{
		float f = z * 0.5f;
		float z2 = Mathf.Sin(f);
		return new IndexedQuaternion(0f, 0f, z2, Mathf.Cos(f));
	}

	public static float ComputeEulerX(ref IndexedBasisMatrix m)
	{
		float num = 0f;
		num = ((!(Mathf.Abs(m._el1.Z) <= float.Epsilon) || !(Mathf.Abs(m._el2.Z) <= float.Epsilon)) ? (Mathf.Atan2(m._el1.Z, 0f - m._el2.Z) - (float)System.Math.PI) : (Mathf.Atan2(m._el1.Y, 0f - m._el2.Y) - (float)System.Math.PI / 2f));
		return NormalizeAngle(num);
	}

	public static float ComputeEulerY(ref IndexedBasisMatrix m)
	{
		float num = 0f;
		num = ((!(Mathf.Abs(m._el0.X) <= float.Epsilon) || !(Mathf.Abs(m._el2.X) <= float.Epsilon)) ? Mathf.Atan2(0f - m._el2.X, m._el0.X) : (Mathf.Atan2(0f - m._el2.Z, m._el0.Z) - 4.712389f));
		return NormalizeAngle(num);
	}

	public static float ComputeEulerZ(ref IndexedBasisMatrix m)
	{
		float num = 0f;
		num = ((!(Mathf.Abs(m._el0.X) <= float.Epsilon) || !(Mathf.Abs(m._el1.X) <= float.Epsilon)) ? Mathf.Atan2(m._el1.X, m._el0.X) : (Mathf.Atan2(m._el1.Y, m._el0.Y) - (float)System.Math.PI / 2f));
		return NormalizeAngle(num);
	}

	public static void GetLinearLimitFromLeftHand(ref IndexedVector3 limitPosFrom, ref IndexedVector3 limitPosTo)
	{
		IndexedVector3 indexedVector = limitPosFrom;
		IndexedVector3 indexedVector2 = limitPosTo;
		limitPosFrom = new IndexedVector3(indexedVector[0], indexedVector[1], 0f - indexedVector2[2]);
		limitPosTo = new IndexedVector3(indexedVector2[0], indexedVector2[1], 0f - indexedVector[2]);
	}

	public static void GetAngularLimitFromLeftHand(ref IndexedVector3 limitRotFrom, ref IndexedVector3 limitRotTo)
	{
		IndexedVector3 indexedVector = limitRotFrom;
		IndexedVector3 indexedVector2 = limitRotTo;
		limitRotFrom = new IndexedVector3(0f - indexedVector2[0], 0f - indexedVector2[1], indexedVector[2]);
		limitRotTo = new IndexedVector3(0f - indexedVector[0], 0f - indexedVector[1], indexedVector2[2]);
	}

	public static IndexedVector3 Lerp(ref IndexedVector3 lhs, ref IndexedVector3 rhs, float t)
	{
		return (rhs - lhs) * t + lhs;
	}

	public static float Dot(ref IndexedQuaternion lhs, ref IndexedQuaternion rhs)
	{
		return lhs.X * rhs.X + lhs.Y * rhs.Y + lhs.Z * rhs.Z + lhs.W * rhs.W;
	}

	public static float Length2(ref IndexedQuaternion q)
	{
		return q.X * q.X + q.Y * q.Y + q.Z * q.Z + q.W * q.W;
	}

	public static float Angle(ref IndexedQuaternion lhs, ref IndexedQuaternion rhs)
	{
		float num = Mathf.Sqrt(Length2(ref lhs) * Length2(ref rhs));
		if (!(Mathf.Abs(num) > float.Epsilon))
		{
			return 0f;
		}
		return Mathf.Acos(Mathf.Clamp(Dot(ref lhs, ref rhs) / num, -1f, 1f));
	}

	public static IndexedVector3 GetAxis(ref IndexedQuaternion q)
	{
		float num = 1f - q.W * q.W;
		if (num < 1.401298E-44f)
		{
			return new IndexedVector3(1f, 0f, 0f);
		}
		float num2 = 1f / Mathf.Sqrt(num);
		return new IndexedVector3(q.X * num2, q.Y * num2, q.Z * num2);
	}

	public static float GetAngle(ref IndexedQuaternion q)
	{
		return 2f * Mathf.Acos(Mathf.Clamp(q.W, -1f, 1f));
	}

	public static IndexedQuaternion SlerpFromIdentity(ref IndexedQuaternion rhs, float t)
	{
		float num = Mathf.Sqrt(Length2(ref rhs));
		float num2 = ((Mathf.Abs(num) > float.Epsilon) ? Mathf.Acos(Mathf.Clamp(rhs.W / num, -1f, 1f)) : 0f);
		if (num2 != 0f)
		{
			float num3 = 1f / Mathf.Sin(num2);
			float num4 = Mathf.Sin((1f - t) * num2);
			float num5 = Mathf.Sin(t * num2);
			if (rhs.W < 0f)
			{
				return new IndexedQuaternion((0f - rhs.X) * num5 * num3, (0f - rhs.Y) * num5 * num3, (0f - rhs.Z) * num5 * num3, (num4 + (0f - rhs.W) * num5) * num3);
			}
			return new IndexedQuaternion(rhs.X * num5 * num3, rhs.Y * num5 * num3, rhs.Z * num5 * num3, (num4 + rhs.W * num5) * num3);
		}
		return IndexedQuaternion.Identity;
	}

	public static IndexedQuaternion Slerp(ref IndexedQuaternion lhs, ref IndexedQuaternion rhs, float t)
	{
		float num = Angle(ref lhs, ref rhs);
		if (num != 0f)
		{
			float num2 = 1f / Mathf.Sin(num);
			float num3 = Mathf.Sin((1f - t) * num);
			float num4 = Mathf.Sin(t * num);
			if (Dot(ref lhs, ref rhs) < 0f)
			{
				return new IndexedQuaternion((lhs.X * num3 + (0f - rhs.X) * num4) * num2, (lhs.Y * num3 + (0f - rhs.Y) * num4) * num2, (lhs.Z * num3 + (0f - rhs.Z) * num4) * num2, (lhs.W * num3 + (0f - rhs.W) * num4) * num2);
			}
			return new IndexedQuaternion((lhs.X * num3 + rhs.X * num4) * num2, (lhs.Y * num3 + rhs.Y * num4) * num2, (lhs.Z * num3 + rhs.Z * num4) * num2, (lhs.W * num3 + rhs.W * num4) * num2);
		}
		return lhs;
	}

	public static bool IsPositionFuzzyZero(ref IndexedVector3 v)
	{
		if (Mathf.Abs(v.X) < 1E-05f && Mathf.Abs(v.Y) < 1E-05f)
		{
			return Mathf.Abs(v.Z) < 1E-05f;
		}
		return false;
	}

	public static bool IsRotationFuzzyIdentity(ref IndexedQuaternion r)
	{
		if (Mathf.Abs(r.X) < 1E-07f && Mathf.Abs(r.Y) < 1E-07f && Mathf.Abs(r.Z) < 1E-07f)
		{
			return Mathf.Abs(1f - r.W) < 1E-07f;
		}
		return false;
	}

	public static bool FuzzyZero(float x)
	{
		return Mathf.Abs(x) <= float.Epsilon;
	}
}
