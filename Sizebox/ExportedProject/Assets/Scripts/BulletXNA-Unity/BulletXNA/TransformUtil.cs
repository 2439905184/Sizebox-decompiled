using System;
using System.Collections.Generic;
using BulletXNA.LinearMath;

namespace BulletXNA
{
	public static class TransformUtil
	{
		public static float ANGULAR_MOTION_THRESHOLD = (float)Math.PI / 4f;

		public static IList<float> FloatToList(float f)
		{
			IList<float> list = new List<float>();
			list.Add(f);
			return list;
		}

		public static IList<float> VectorToList(IndexedVector3 vector)
		{
			return VectorToList(ref vector);
		}

		public static IList<float> VectorToList(ref IndexedVector3 vector)
		{
			IList<float> list = new List<float>();
			list.Add(vector.X);
			list.Add(vector.Y);
			list.Add(vector.Z);
			return list;
		}

		public static IList<IndexedVector3> VectorsFromList(IList<float> list)
		{
			IList<IndexedVector3> list2 = new List<IndexedVector3>();
			int num = list.Count / 3;
			for (int i = 0; i < num; i++)
			{
				IndexedVector3 item = new IndexedVector3(list[3 * i], list[3 * i + 1], list[3 * i + 2]);
				list2.Add(item);
			}
			return list2;
		}

		public static void PlaneSpace1(ref IndexedVector3 n, out IndexedVector3 p, out IndexedVector3 q)
		{
			if (Math.Abs(n.Z) > 0.70710677f)
			{
				float num = n.Y * n.Y + n.Z * n.Z;
				float num2 = MathUtil.RecipSqrt(num);
				p = new IndexedVector3(0f, (0f - n.Z) * num2, n.Y * num2);
				q = new IndexedVector3(num * num2, (0f - n.X) * p.Z, n.X * p.Y);
			}
			else
			{
				float num3 = n.X * n.X + n.Y * n.Y;
				float num4 = MathUtil.RecipSqrt(num3);
				p = new IndexedVector3((0f - n.Y) * num4, n.X * num4, 0f);
				q = new IndexedVector3((0f - n.Z) * p.Y, n.Z * p.X, num3 * num4);
			}
		}

		public static IndexedVector3 AabbSupport(ref IndexedVector3 halfExtents, ref IndexedVector3 supportDir)
		{
			return new IndexedVector3((supportDir.X < 0f) ? (0f - halfExtents.X) : halfExtents.X, (supportDir.Y < 0f) ? (0f - halfExtents.Y) : halfExtents.Y, (supportDir.Z < 0f) ? (0f - halfExtents.Z) : halfExtents.Z);
		}

		public static void IntegrateTransform(IndexedMatrix curTrans, IndexedVector3 linvel, IndexedVector3 angvel, float timeStep, out IndexedMatrix predictedTransform)
		{
			IntegrateTransform(ref curTrans, ref linvel, ref angvel, timeStep, out predictedTransform);
		}

		public static void IntegrateTransform(ref IndexedMatrix curTrans, ref IndexedVector3 linvel, ref IndexedVector3 angvel, float timeStep, out IndexedMatrix predictedTransform)
		{
			predictedTransform = IndexedMatrix.CreateTranslation(curTrans._origin + linvel * timeStep);
			float num = angvel.Length();
			if (num * timeStep > ANGULAR_MOTION_THRESHOLD)
			{
				num = ANGULAR_MOTION_THRESHOLD / timeStep;
			}
			IndexedVector3 indexedVector = ((!(num < 0.001f)) ? (angvel * ((float)Math.Sin(0.5f * num * timeStep) / num)) : (angvel * (0.5f * timeStep - timeStep * timeStep * timeStep * (1f / 48f) * num * num)));
			IndexedQuaternion indexedQuaternion = new IndexedQuaternion(indexedVector.X, indexedVector.Y, indexedVector.Z, (float)Math.Cos(num * timeStep * 0.5f));
			IndexedQuaternion rotation = curTrans.GetRotation();
			IndexedQuaternion q = indexedQuaternion * rotation;
			q.Normalize();
			predictedTransform._basis = IndexedMatrix.CreateFromQuaternion(q)._basis;
		}

		public static void CalculateVelocityQuaternion(ref IndexedVector3 pos0, ref IndexedVector3 pos1, ref IndexedQuaternion orn0, ref IndexedQuaternion orn1, float timeStep, out IndexedVector3 linVel, out IndexedVector3 angVel)
		{
			linVel = (pos1 - pos0) / timeStep;
			if (orn0 != orn1)
			{
				IndexedVector3 axis;
				float angle;
				CalculateDiffAxisAngleQuaternion(ref orn0, ref orn1, out axis, out angle);
				angVel = axis * (angle / timeStep);
			}
			else
			{
				angVel = IndexedVector3.Zero;
			}
		}

		public static void CalculateDiffAxisAngleQuaternion(ref IndexedQuaternion orn0, ref IndexedQuaternion orn1a, out IndexedVector3 axis, out float angle)
		{
			IndexedQuaternion indexedQuaternion = MathUtil.QuatFurthest(ref orn0, ref orn1a);
			IndexedQuaternion quat = indexedQuaternion * MathUtil.QuaternionInverse(ref orn0);
			quat.Normalize();
			angle = MathUtil.QuatAngle(ref quat);
			axis = new IndexedVector3(quat.X, quat.Y, quat.Z);
			float num = axis.LengthSquared();
			if (num < 1.4210855E-14f)
			{
				axis = new IndexedVector3(1f, 0f, 0f);
			}
			else
			{
				axis.Normalize();
			}
		}

		public static void CalculateVelocity(ref IndexedMatrix transform0, ref IndexedMatrix transform1, float timeStep, out IndexedVector3 linVel, out IndexedVector3 angVel)
		{
			linVel = (transform1._origin - transform0._origin) / timeStep;
			IndexedVector3 axis;
			float angle;
			CalculateDiffAxisAngle(ref transform0, ref transform1, out axis, out angle);
			angVel = axis * (angle / timeStep);
		}

		public static void CalculateDiffAxisAngle(ref IndexedMatrix transform0, ref IndexedMatrix transform1, out IndexedVector3 axis, out float angle)
		{
			IndexedBasisMatrix a = transform1._basis * transform0._basis.Inverse();
			IndexedQuaternion rot = IndexedQuaternion.Identity;
			GetRotation(ref a, out rot);
			rot.Normalize();
			angle = MathUtil.QuatAngle(ref rot);
			axis = new IndexedVector3(rot.X, rot.Y, rot.Z);
			float num = axis.LengthSquared();
			if (num < 1.4210855E-14f)
			{
				axis = new IndexedVector3(1f, 0f, 0f);
			}
			else
			{
				axis.Normalize();
			}
		}

		public static void GetRotation(ref IndexedBasisMatrix a, out IndexedQuaternion rot)
		{
			rot = a.GetRotation();
		}

		public static IndexedQuaternion GetRotation(ref IndexedBasisMatrix a)
		{
			return a.GetRotation();
		}
	}
}
