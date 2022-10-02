using UnityEngine;

namespace BulletXNA.LinearMath
{
	public struct IndexedQuaternion
	{
		public float X;

		public float Y;

		public float Z;

		public float W;

		private static IndexedQuaternion _identity = new IndexedQuaternion(0f, 0f, 0f, 1f);

		public static IndexedQuaternion Identity
		{
			get
			{
				return _identity;
			}
		}

		public IndexedQuaternion(float x, float y, float z, float w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public IndexedQuaternion(IndexedVector3 axis, float angle)
		{
			float num = axis.Length();
			float f = angle * 0.5f;
			float num2 = ((num > float.Epsilon) ? (Mathf.Sin(f) / num) : 0f);
			X = axis.X * num2;
			Y = axis.Y * num2;
			Z = axis.Z * num2;
			W = Mathf.Cos(f);
		}

		public void SetRotation(ref IndexedVector3 axis, float angle)
		{
			float num = axis.Length();
			float f = angle * 0.5f;
			float num2 = ((num > float.Epsilon) ? (Mathf.Sin(f) / num) : 0f);
			X = axis.X * num2;
			Y = axis.Y * num2;
			Z = axis.Z * num2;
			W = Mathf.Cos(f);
		}

		public void SetValue(float x, float y, float z, float w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public static IndexedQuaternion operator +(IndexedQuaternion quaternion1, IndexedQuaternion quaternion2)
		{
			IndexedQuaternion result = default(IndexedQuaternion);
			result.X = quaternion1.X + quaternion2.X;
			result.Y = quaternion1.Y + quaternion2.Y;
			result.Z = quaternion1.Z + quaternion2.Z;
			result.W = quaternion1.W + quaternion2.W;
			return result;
		}

		public static IndexedQuaternion operator -(IndexedQuaternion quaternion1, IndexedQuaternion quaternion2)
		{
			IndexedQuaternion result = default(IndexedQuaternion);
			result.X = quaternion1.X - quaternion2.X;
			result.Y = quaternion1.Y - quaternion2.Y;
			result.Z = quaternion1.Z - quaternion2.Z;
			result.W = quaternion1.W - quaternion2.W;
			return result;
		}

		public static IndexedQuaternion operator *(IndexedQuaternion q1, IndexedQuaternion q2)
		{
			return new IndexedQuaternion(q1.W * q2.X + q1.X * q2.W + q1.Y * q2.Z - q1.Z * q2.Y, q1.W * q2.Y + q1.Y * q2.W + q1.Z * q2.X - q1.X * q2.Z, q1.W * q2.Z + q1.Z * q2.W + q1.X * q2.Y - q1.Y * q2.X, q1.W * q2.W - q1.X * q2.X - q1.Y * q2.Y - q1.Z * q2.Z);
		}

		public static IndexedQuaternion operator *(IndexedQuaternion q1, IndexedVector3 v1)
		{
			return new IndexedQuaternion(q1.W * v1.X + q1.Y * v1.Z - q1.Z * v1.Y, q1.W * v1.Y + q1.Z * v1.X - q1.X * v1.Z, q1.W * v1.Z + q1.X * v1.Y - q1.Y * v1.X, (0f - q1.X) * v1.X - q1.Y * v1.Y - q1.Z * v1.Z);
		}

		public static IndexedQuaternion operator *(IndexedVector3 v1, IndexedQuaternion q1)
		{
			return new IndexedQuaternion(v1.X * q1.W + v1.Y * q1.Z - v1.Z * q1.Y, v1.Y * q1.W + v1.Z * q1.X - v1.X * q1.Z, v1.Z * q1.W + v1.X * q1.Y - v1.Y * q1.X, (0f - v1.X) * q1.X - v1.Y * q1.Y - v1.Z * q1.Z);
		}

		public static IndexedQuaternion operator -(IndexedQuaternion value)
		{
			IndexedQuaternion result = default(IndexedQuaternion);
			result.X = 0f - value.X;
			result.Y = 0f - value.Y;
			result.Z = 0f - value.Z;
			result.W = 0f - value.W;
			return result;
		}

		public IndexedQuaternion(ref Quaternion q)
		{
			X = q.x;
			Y = q.y;
			Z = q.z;
			W = q.w;
		}

		public IndexedQuaternion(Quaternion q)
		{
			X = q.x;
			Y = q.y;
			Z = q.z;
			W = q.w;
		}

		public static implicit operator Quaternion(IndexedQuaternion q)
		{
			return new Quaternion(q.X, q.Y, q.Z, q.W);
		}

		public static implicit operator IndexedQuaternion(Quaternion q)
		{
			return new IndexedQuaternion(q.x, q.y, q.z, q.w);
		}

		public float LengthSquared()
		{
			return X * X + Y * Y + Z * Z + W * W;
		}

		public float Length()
		{
			return Mathf.Sqrt(X * X + Y * Y + Z * Z + W * W);
		}

		public void Normalize()
		{
			float num = X * X + Y * Y + Z * Z + W * W;
			if (num > float.Epsilon)
			{
				num = 1f / Mathf.Sqrt(num);
				X *= num;
				Y *= num;
				Z *= num;
				W *= num;
			}
			else
			{
				X = 0f;
				Y = 0f;
				Z = 0f;
				W = 1f;
			}
		}

		public static IndexedQuaternion Inverse(IndexedQuaternion q)
		{
			return new IndexedQuaternion(0f - q.X, 0f - q.Y, 0f - q.Z, q.W);
		}

		public IndexedQuaternion Inverse()
		{
			return new IndexedQuaternion(0f - X, 0f - Y, 0f - Z, W);
		}

		public float Dot(IndexedQuaternion q)
		{
			return X * q.X + Y * q.Y + Z * q.Z + W * q.W;
		}

		public static float Dot(IndexedQuaternion q, IndexedQuaternion q2)
		{
			return q.X * q2.X + q.Y * q2.Y + q.Z * q2.Z + q.W * q2.W;
		}

		public static bool operator ==(IndexedQuaternion value1, IndexedQuaternion value2)
		{
			if (value1.X == value2.X && value1.Y == value2.Y && value1.Z == value2.Z)
			{
				return value1.W == value2.W;
			}
			return false;
		}

		public static bool operator !=(IndexedQuaternion value1, IndexedQuaternion value2)
		{
			if (value1.X == value2.X && value1.Y == value2.Y && value1.Z == value2.Z)
			{
				return value1.W != value2.W;
			}
			return true;
		}

		public IndexedVector3 QuatRotate(IndexedQuaternion rotation, IndexedVector3 v)
		{
			IndexedQuaternion indexedQuaternion = rotation * v;
			indexedQuaternion *= rotation.Inverse();
			return new IndexedVector3(indexedQuaternion.X, indexedQuaternion.Y, indexedQuaternion.Z);
		}
	}
}
