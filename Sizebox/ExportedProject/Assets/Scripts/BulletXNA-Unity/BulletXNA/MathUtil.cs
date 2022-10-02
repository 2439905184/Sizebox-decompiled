using System;
using System.Diagnostics;
using System.IO;
using BulletXNA.BulletCollision;
using BulletXNA.LinearMath;

namespace BulletXNA
{
	public static class MathUtil
	{
		public const float SIMD_EPSILON = 1.1920929E-07f;

		public const float SIMDSQRT12 = 0.70710677f;

		public const float BT_LARGE_FLOAT = 1E+18f;

		public const float SIMD_2_PI = (float)Math.PI * 2f;

		public const float SIMD_PI = (float)Math.PI;

		public const float SIMD_HALF_PI = (float)Math.PI / 2f;

		public const float SIMD_QUARTER_PI = (float)Math.PI / 4f;

		public const float SIMD_INFINITY = float.MaxValue;

		public const float SIMD_RADS_PER_DEG = (float)Math.PI / 180f;

		public const float SIMD_DEGS_PER_RAD = 180f / (float)Math.PI;

		public static IndexedVector3 MAX_VECTOR = new IndexedVector3(1E+18f);

		public static IndexedVector3 MIN_VECTOR = new IndexedVector3(-1E+18f);

		public static float[,] BasisMatrixToFloatArray(ref IndexedBasisMatrix m)
		{
			return new float[3, 3]
			{
				{
					m._el0.X,
					m._el0.Y,
					m._el0.Z
				},
				{
					m._el1.X,
					m._el1.Y,
					m._el1.Z
				},
				{
					m._el2.X,
					m._el2.Y,
					m._el2.Z
				}
			};
		}

		public static void FloatArrayToBasisMatrix(float[,] f, ref IndexedBasisMatrix m)
		{
			m._el0 = new IndexedVector3(f[0, 0], f[0, 1], f[0, 2]);
			m._el1 = new IndexedVector3(f[1, 0], f[1, 1], f[1, 2]);
			m._el2 = new IndexedVector3(f[2, 0], f[2, 1], f[2, 2]);
		}

		public static void InverseTransform(ref IndexedMatrix m, ref IndexedVector3 v, out IndexedVector3 o)
		{
			IndexedVector3 indexedVector = v - m._origin;
			o = m._basis.Transpose() * indexedVector;
		}

		public static IndexedVector3 InverseTransform(ref IndexedMatrix m, ref IndexedVector3 v)
		{
			IndexedVector3 indexedVector = v - m._origin;
			return m._basis.Transpose() * indexedVector;
		}

		public static float FSel(float a, float b, float c)
		{
			if (!(a >= 0f))
			{
				return c;
			}
			return b;
		}

		public static int MaxAxis(ref IndexedVector3 a)
		{
			if (!(a.X < a.Y))
			{
				if (!(a.X < a.Z))
				{
					return 0;
				}
				return 2;
			}
			if (!(a.Y < a.Z))
			{
				return 1;
			}
			return 2;
		}

		public static int MaxAxis(IndexedVector4 a)
		{
			return MaxAxis(ref a);
		}

		public static int MaxAxis(ref IndexedVector4 a)
		{
			int result = -1;
			float num = -1E+18f;
			if (a.X > num)
			{
				result = 0;
				num = a.X;
			}
			if (a.Y > num)
			{
				result = 1;
				num = a.Y;
			}
			if (a.Z > num)
			{
				result = 2;
				num = a.Z;
			}
			if (a.W > num)
			{
				result = 3;
			}
			return result;
		}

		public static int ClosestAxis(ref IndexedVector4 a)
		{
			return MaxAxis(AbsoluteVector4(ref a));
		}

		public static IndexedVector4 AbsoluteVector4(ref IndexedVector4 vec)
		{
			return new IndexedVector4(Math.Abs(vec.X), Math.Abs(vec.Y), Math.Abs(vec.Z), Math.Abs(vec.W));
		}

		public static float Vector3Triple(ref IndexedVector3 a, ref IndexedVector3 b, ref IndexedVector3 c)
		{
			return a.X * (b.Y * c.Z - b.Z * c.Y) + a.Y * (b.Z * c.X - b.X * c.Z) + a.Z * (b.X * c.Y - b.Y * c.X);
		}

		public static int GetQuantized(float x)
		{
			if ((double)x < 0.0)
			{
				return (int)((double)x - 0.5);
			}
			return (int)((double)x + 0.5);
		}

		public static int Clamp(int value, int min, int max)
		{
			if (value >= min)
			{
				if (value <= max)
				{
					return value;
				}
				return max;
			}
			return min;
		}

		public static void VectorClampMax(ref IndexedVector3 input, ref IndexedVector3 bounds)
		{
			input.X = Math.Min(input.X, bounds.X);
			input.Y = Math.Min(input.Y, bounds.Y);
			input.Z = Math.Min(input.Z, bounds.Z);
		}

		public static void VectorClampMin(ref IndexedVector3 input, ref IndexedVector3 bounds)
		{
			input.X = Math.Max(input.X, bounds.X);
			input.Y = Math.Max(input.Y, bounds.Y);
			input.Z = Math.Max(input.Z, bounds.Z);
		}

		public static void VectorMin(IndexedVector3 input, ref IndexedVector3 output)
		{
			VectorMin(ref input, ref output);
		}

		public static void VectorMin(ref IndexedVector3 input, ref IndexedVector3 output)
		{
			output.X = Math.Min(input.X, output.X);
			output.Y = Math.Min(input.Y, output.Y);
			output.Z = Math.Min(input.Z, output.Z);
		}

		public static void VectorMin(ref IndexedVector3 input1, ref IndexedVector3 input2, out IndexedVector3 output)
		{
			output = new IndexedVector3(Math.Min(input1.X, input2.X), Math.Min(input1.Y, input2.Y), Math.Min(input1.Z, input2.Z));
		}

		public static void VectorMax(IndexedVector3 input, ref IndexedVector3 output)
		{
			VectorMax(ref input, ref output);
		}

		public static void VectorMax(ref IndexedVector3 input, ref IndexedVector3 output)
		{
			output.X = Math.Max(input.X, output.X);
			output.Y = Math.Max(input.Y, output.Y);
			output.Z = Math.Max(input.Z, output.Z);
		}

		public static void VectorMax(ref IndexedVector3 input1, ref IndexedVector3 input2, out IndexedVector3 output)
		{
			output = new IndexedVector3(Math.Max(input1.X, input2.X), Math.Max(input1.Y, input2.Y), Math.Max(input1.Z, input2.Z));
		}

		public static float RecipSqrt(float a)
		{
			return (float)(1.0 / Math.Sqrt(a));
		}

		public static bool CompareFloat(float val1, float val2)
		{
			return Math.Abs(val1 - val2) <= 1.1920929E-07f;
		}

		public static bool FuzzyZero(float val)
		{
			return Math.Abs(val) <= 1.1920929E-07f;
		}

		public static uint Select(uint condition, uint valueIfConditionNonZero, uint valueIfConditionZero)
		{
			uint num = (uint)((int)(condition | (0 - condition)) >> 31);
			uint num2 = ~num;
			return (valueIfConditionNonZero & num) | (valueIfConditionZero & num2);
		}

		public static IndexedQuaternion ShortestArcQuat(IndexedVector3 axisInA, IndexedVector3 axisInB)
		{
			return ShortestArcQuat(ref axisInA, ref axisInB);
		}

		public static IndexedQuaternion ShortestArcQuat(ref IndexedVector3 axisInA, ref IndexedVector3 axisInB)
		{
			IndexedVector3 indexedVector = IndexedVector3.Cross(axisInA, axisInB);
			float num = IndexedVector3.Dot(axisInA, axisInB);
			if ((double)num < -0.9999998807907104)
			{
				return new IndexedQuaternion(0f, 1f, 0f, 0f);
			}
			float num2 = (float)Math.Sqrt((1f + num) * 2f);
			float num3 = 1f / num2;
			return new IndexedQuaternion(indexedVector.X * num3, indexedVector.Y * num3, indexedVector.Z * num3, num2 * 0.5f);
		}

		public static float QuatAngle(ref IndexedQuaternion quat)
		{
			return 2f * (float)Math.Acos(quat.W);
		}

		public static IndexedQuaternion QuatFurthest(ref IndexedQuaternion input1, ref IndexedQuaternion input2)
		{
			IndexedQuaternion indexedQuaternion = input1 - input2;
			IndexedQuaternion indexedQuaternion2 = input1 + input2;
			if (IndexedQuaternion.Dot(indexedQuaternion, indexedQuaternion) > IndexedQuaternion.Dot(indexedQuaternion2, indexedQuaternion2))
			{
				return input2;
			}
			return -input2;
		}

		public static IndexedVector3 QuatRotate(IndexedQuaternion rotation, IndexedVector3 v)
		{
			return QuatRotate(ref rotation, ref v);
		}

		public static IndexedVector3 QuatRotate(ref IndexedQuaternion rotation, ref IndexedVector3 v)
		{
			IndexedQuaternion indexedQuaternion = QuatVectorMultiply(ref rotation, ref v);
			indexedQuaternion *= QuaternionInverse(ref rotation);
			return new IndexedVector3(indexedQuaternion.X, indexedQuaternion.Y, indexedQuaternion.Z);
		}

		public static IndexedQuaternion QuatVectorMultiply(ref IndexedQuaternion q, ref IndexedVector3 w)
		{
			return new IndexedQuaternion(q.W * w.X + q.Y * w.Z - q.Z * w.Y, q.W * w.Y + q.Z * w.X - q.X * w.Z, q.W * w.Z + q.X * w.Y - q.Y * w.X, (0f - q.X) * w.X - q.Y * w.Y - q.Z * w.Z);
		}

		public static void GetSkewSymmetricMatrix(ref IndexedVector3 vecin, out IndexedVector3 v0, out IndexedVector3 v1, out IndexedVector3 v2)
		{
			v0 = new IndexedVector3(0f, 0f - vecin.Z, vecin.Y);
			v1 = new IndexedVector3(vecin.Z, 0f, 0f - vecin.X);
			v2 = new IndexedVector3(0f - vecin.Y, vecin.X, 0f);
		}

		[Conditional("DEBUG")]
		public static void SanityCheckVector(IndexedVector3 v)
		{
		}

		[Conditional("DEBUG")]
		public static void ZeroCheckVector(IndexedVector3 v)
		{
		}

		[Conditional("DEBUG")]
		public static void ZeroCheckVector(ref IndexedVector3 v)
		{
			FuzzyZero(v.LengthSquared());
		}

		[Conditional("DEBUG")]
		public static void SanityCheckVector(ref IndexedVector3 v)
		{
			if (!float.IsNaN(v.X) && !float.IsNaN(v.Y))
			{
				float.IsNaN(v.Z);
			}
		}

		[Conditional("DEBUG")]
		public static void SanityCheckFloat(float f)
		{
		}

		public static float GetMatrixElem(IndexedBasisMatrix mat, int index)
		{
			int i = index % 3;
			int j = index / 3;
			return mat[i, j];
		}

		public static float GetMatrixElem(ref IndexedBasisMatrix mat, int index)
		{
			int i = index % 3;
			int j = index / 3;
			return mat[i, j];
		}

		public static bool MatrixToEulerXYZ(ref IndexedBasisMatrix mat, out IndexedVector3 xyz)
		{
			float matrixElem = GetMatrixElem(ref mat, 0);
			float matrixElem2 = GetMatrixElem(ref mat, 1);
			float matrixElem3 = GetMatrixElem(ref mat, 2);
			float matrixElem4 = GetMatrixElem(ref mat, 3);
			float matrixElem5 = GetMatrixElem(ref mat, 4);
			float matrixElem6 = GetMatrixElem(ref mat, 5);
			GetMatrixElem(ref mat, 6);
			GetMatrixElem(ref mat, 7);
			float matrixElem7 = GetMatrixElem(ref mat, 8);
			float num = matrixElem3;
			if (num < 1f)
			{
				if (num > -1f)
				{
					xyz = new IndexedVector3((float)Math.Atan2(0f - matrixElem6, matrixElem7), (float)Math.Asin(matrixElem3), (float)Math.Atan2(0f - matrixElem2, matrixElem));
					return true;
				}
				xyz = new IndexedVector3((float)(0.0 - Math.Atan2(matrixElem4, matrixElem5)), -(float)Math.PI / 2f, 0f);
				return false;
			}
			xyz = new IndexedVector3((float)Math.Atan2(matrixElem4, matrixElem5), (float)Math.PI / 2f, 0f);
			return false;
		}

		public static IndexedQuaternion QuaternionInverse(IndexedQuaternion q)
		{
			return QuaternionInverse(ref q);
		}

		public static IndexedQuaternion QuaternionInverse(ref IndexedQuaternion q)
		{
			return new IndexedQuaternion(0f - q.X, 0f - q.Y, 0f - q.Z, q.W);
		}

		public static IndexedQuaternion QuaternionMultiply(IndexedQuaternion a, IndexedQuaternion b)
		{
			return QuaternionMultiply(ref a, ref b);
		}

		public static IndexedQuaternion QuaternionMultiply(ref IndexedQuaternion a, ref IndexedQuaternion b)
		{
			return a * b;
		}

		public static float NormalizeAngle(float angleInRadians)
		{
			angleInRadians %= (float)Math.PI * 2f;
			if (angleInRadians < -(float)Math.PI)
			{
				return angleInRadians + (float)Math.PI * 2f;
			}
			if (angleInRadians > (float)Math.PI)
			{
				return angleInRadians - (float)Math.PI * 2f;
			}
			return angleInRadians;
		}

		public static float DegToRadians(float degrees)
		{
			return degrees / 360f * ((float)Math.PI * 2f);
		}

		public static IndexedVector3 Interpolate3(IndexedVector3 v0, IndexedVector3 v1, float rt)
		{
			float num = 1f - rt;
			return new IndexedVector3(num * v0.X + rt * v1.X, num * v0.Y + rt * v1.Y, num * v0.Z + rt * v1.Z);
		}

		public static IndexedVector3 Interpolate3(ref IndexedVector3 v0, ref IndexedVector3 v1, float rt)
		{
			float num = 1f - rt;
			return new IndexedVector3(num * v0.X + rt * v1.X, num * v0.Y + rt * v1.Y, num * v0.Z + rt * v1.Z);
		}

		public static IndexedMatrix SetEulerZYX(float eulerX, float eulerY, float eulerZ)
		{
			IndexedMatrix identity = IndexedMatrix.Identity;
			identity._basis.SetEulerZYX(eulerX, eulerY, eulerZ);
			return identity;
		}

		public static IndexedVector3 Vector4ToVector3(IndexedVector4 v4)
		{
			return new IndexedVector3(v4.X, v4.Y, v4.Z);
		}

		public static IndexedVector3 Vector4ToVector3(ref IndexedVector4 v4)
		{
			return new IndexedVector3(v4.X, v4.Y, v4.Z);
		}

		public static void PrintQuaternion(TextWriter writer, IndexedQuaternion q)
		{
			writer.Write(string.Format("{{X:{0:0.00000000} Y:{1:0.00000000} Z:{2:0.00000000} W:{3:0.00000000}}}", q.X, q.Y, q.Z, q.W));
		}

		public static void PrintVector3(TextWriter writer, IndexedVector3 v)
		{
			writer.Write("{");
			PrintScalar(writer, "X:", v.X);
			writer.Write(" ");
			PrintScalar(writer, "Y:", v.Y);
			writer.Write(" ");
			PrintScalar(writer, "Z:", v.Z);
			writer.WriteLine("}");
		}

		public static void PrintVector3(TextWriter writer, string name, IndexedVector3 v)
		{
			writer.Write(string.Format("[{0}]", name));
			writer.Write("{");
			PrintScalar(writer, "X:", v.X);
			writer.Write(" ");
			PrintScalar(writer, "Y:", v.Y);
			writer.Write(" ");
			PrintScalar(writer, "Z:", v.Z);
			writer.WriteLine("}");
		}

		public static void PrintVector4(TextWriter writer, IndexedVector4 v)
		{
			writer.WriteLine(string.Format("{{X:{0:0.00000000} Y:{1:0.00000000} Z:{2:0.00000000} W:{3:0.00000000}}}", v.X, v.Y, v.Z, v.W));
		}

		public static void PrintVector4(TextWriter writer, string name, IndexedVector4 v)
		{
			writer.WriteLine(string.Format("[{0}] {{X:{1:0.00000000} Y:{2:0.00000000} Z:{3:0.00000000} W:{4:0.00000000}}}", name, v.X, v.Y, v.Z, v.W));
		}

		public static void PrintMatrix(TextWriter writer, IndexedMatrix m)
		{
			PrintMatrix(writer, null, m);
		}

		public static void PrintMatrix(TextWriter writer, string name, IndexedMatrix m)
		{
			if (writer != null)
			{
				if (name != null)
				{
					writer.WriteLine(name);
				}
				PrintVector3(writer, "Right       ", m._basis.GetColumn(0));
				PrintVector3(writer, "Up          ", m._basis.GetColumn(1));
				PrintVector3(writer, "Backward    ", m._basis.GetColumn(2));
				PrintVector3(writer, "Translation ", m._origin);
			}
		}

		public static void PrintMatrix(TextWriter writer, IndexedBasisMatrix m)
		{
			PrintMatrix(writer, null, m);
		}

		public static void PrintMatrix(TextWriter writer, string name, IndexedBasisMatrix m)
		{
			if (writer != null)
			{
				if (name != null)
				{
					writer.WriteLine(name);
				}
				PrintVector3(writer, "Right       ", m.GetColumn(0));
				PrintVector3(writer, "Up          ", m.GetColumn(1));
				PrintVector3(writer, "Backward    ", m.GetColumn(2));
			}
		}

		public static void PrintScalar(TextWriter writer, string name, float s)
		{
			float num = s;
			if (num < 0f && FuzzyZero(num))
			{
				num = 0f;
			}
			writer.Write("{0} {1:0.000}", name, num);
		}

		public static void PrintContactPoint(StreamWriter streamWriter, ManifoldPoint mp)
		{
			if (streamWriter != null)
			{
				streamWriter.WriteLine("ContactPoint");
				PrintVector3(streamWriter, "localPointA", mp.m_localPointA);
				PrintVector3(streamWriter, "localPointB", mp.m_localPointB);
				PrintVector3(streamWriter, "posWorldA", mp.m_positionWorldOnA);
				PrintVector3(streamWriter, "posWorldB", mp.m_positionWorldOnB);
				PrintVector3(streamWriter, "normalWorldB", mp.m_normalWorldOnB);
			}
		}

		public static bool IsAlmostZero(IndexedVector3 v)
		{
			if ((double)Math.Abs(v.X) > 1E-06 || (double)Math.Abs(v.Y) > 1E-06 || (double)Math.Abs(v.Z) > 1E-06)
			{
				return false;
			}
			return true;
		}

		public static bool IsAlmostZero(ref IndexedVector3 v)
		{
			if ((double)Math.Abs(v.X) > 1E-06 || (double)Math.Abs(v.Y) > 1E-06 || (double)Math.Abs(v.Z) > 1E-06)
			{
				return false;
			}
			return true;
		}

		public static IndexedVector3 Vector3Lerp(ref IndexedVector3 a, ref IndexedVector3 b, float t)
		{
			return new IndexedVector3(a.X + (b.X - a.X) * t, a.Y + (b.Y - a.Y) * t, a.Z + (b.Z - a.Z) * t);
		}

		public static float Vector3Distance2XZ(IndexedVector3 x, IndexedVector3 y)
		{
			IndexedVector3 indexedVector = new IndexedVector3(x.X, 0f, x.Z);
			IndexedVector3 indexedVector2 = new IndexedVector3(y.X, 0f, y.Z);
			return (indexedVector - indexedVector2).LengthSquared();
		}

		public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
		{
			T result = value;
			if (value.CompareTo(max) > 0)
			{
				result = max;
			}
			if (value.CompareTo(min) < 0)
			{
				result = min;
			}
			return result;
		}
	}
}
