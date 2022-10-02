using System;
using UnityEngine;

namespace BulletXNA.LinearMath
{
	public struct IndexedMatrix : IEquatable<IndexedMatrix>
	{
		private static IndexedMatrix _identity;

		public IndexedBasisMatrix _basis;

		public IndexedVector3 _origin;

		public static IndexedMatrix Identity
		{
			get
			{
				return _identity;
			}
		}

		static IndexedMatrix()
		{
			_identity = new IndexedMatrix(1f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 1f, 0f, 0f, 0f);
		}

		public IndexedMatrix(float m11, float m12, float m13, float m21, float m22, float m23, float m31, float m32, float m33, float m41, float m42, float m43)
		{
			_basis = new IndexedBasisMatrix(m11, m12, m13, m21, m22, m23, m31, m32, m33);
			_origin = new IndexedVector3(m41, m42, m43);
		}

		public IndexedMatrix(IndexedBasisMatrix basis, IndexedVector3 origin)
		{
			_basis = basis;
			_origin = origin;
		}

		public static IndexedMatrix CreateLookAt(IndexedVector3 cameraPosition, IndexedVector3 cameraTarget, IndexedVector3 cameraUpVector)
		{
			IndexedVector3 row = IndexedVector3.Normalize(cameraPosition - cameraTarget);
			IndexedVector3 row2 = IndexedVector3.Normalize(IndexedVector3.Cross(cameraUpVector, row));
			IndexedVector3 row3 = IndexedVector3.Cross(row, row2);
			IndexedMatrix identity = Identity;
			identity._basis = new IndexedBasisMatrix(ref row2, ref row3, ref row);
			identity._origin = new IndexedVector3(0f - IndexedVector3.Dot(row2, cameraPosition), 0f - IndexedVector3.Dot(row3, cameraPosition), 0f - IndexedVector3.Dot(row, cameraPosition));
			return identity;
		}

		public static IndexedMatrix CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
		{
			float num = 1f / (float)Math.Tan((double)fieldOfView * 0.5);
			float m = num / aspectRatio;
			IndexedMatrix identity = Identity;
			identity._basis = new IndexedBasisMatrix(m, 0f, 0f, 0f, num, 0f, 0f, 0f, farPlaneDistance / (nearPlaneDistance - farPlaneDistance));
			identity._origin = new IndexedVector3(0f, 0f, (float)((double)nearPlaneDistance * (double)farPlaneDistance / ((double)nearPlaneDistance - (double)farPlaneDistance)));
			return identity;
		}

		public static bool operator ==(IndexedMatrix matrix1, IndexedMatrix matrix2)
		{
			if (matrix1._basis == matrix2._basis)
			{
				return matrix1._origin == matrix2._origin;
			}
			return false;
		}

		public static bool operator !=(IndexedMatrix matrix1, IndexedMatrix matrix2)
		{
			if (!(matrix1._basis != matrix2._basis))
			{
				return matrix1._origin != matrix2._origin;
			}
			return true;
		}

		public static IndexedVector3 operator *(IndexedMatrix matrix1, IndexedVector3 v)
		{
			return new IndexedVector3(matrix1._basis._el0.Dot(ref v) + matrix1._origin.X, matrix1._basis._el1.Dot(ref v) + matrix1._origin.Y, matrix1._basis._el2.Dot(ref v) + matrix1._origin.Z);
		}

		public static void Multiply(out IndexedVector3 vout, ref IndexedMatrix matrix1, ref IndexedVector3 vin)
		{
			vout = new IndexedVector3(matrix1._basis._el0.Dot(ref vin) + matrix1._origin.X, matrix1._basis._el1.Dot(ref vin) + matrix1._origin.Y, matrix1._basis._el2.Dot(ref vin) + matrix1._origin.Z);
		}

		public static IndexedMatrix operator *(IndexedMatrix matrix1, IndexedMatrix matrix2)
		{
			IndexedMatrix result = default(IndexedMatrix);
			result._basis = matrix1._basis * matrix2._basis;
			result._origin = matrix1 * matrix2._origin;
			return result;
		}

		public static IndexedMatrix operator /(IndexedMatrix matrix1, float divider)
		{
			float num = 1f / divider;
			IndexedMatrix result = default(IndexedMatrix);
			result._basis = matrix1._basis * num;
			result._origin = matrix1._origin * num;
			return result;
		}

		public static IndexedMatrix CreateTranslation(IndexedVector3 position)
		{
			IndexedMatrix identity = Identity;
			identity._origin = position;
			return identity;
		}

		public static void CreateTranslation(ref IndexedVector3 position, out IndexedMatrix result)
		{
			result = Identity;
			result._origin = position;
		}

		public static IndexedMatrix CreateTranslation(float xPosition, float yPosition, float zPosition)
		{
			IndexedMatrix identity = Identity;
			identity._origin = new IndexedVector3(xPosition, yPosition, zPosition);
			return identity;
		}

		public static void CreateTranslation(float xPosition, float yPosition, float zPosition, out IndexedMatrix result)
		{
			result = Identity;
			result._origin = new IndexedVector3(xPosition, yPosition, zPosition);
		}

		public static IndexedMatrix CreateScale(float x, float y, float z)
		{
			IndexedMatrix identity = Identity;
			identity._basis = IndexedBasisMatrix.CreateScale(new IndexedVector3(x, y, z));
			return identity;
		}

		public static void CreateScale(float xScale, float yScale, float zScale, out IndexedMatrix result)
		{
			result = Identity;
			result._basis = IndexedBasisMatrix.CreateScale(new IndexedVector3(xScale, yScale, zScale));
		}

		public static IndexedMatrix CreateScale(IndexedVector3 scales)
		{
			IndexedMatrix identity = Identity;
			identity._basis = IndexedBasisMatrix.CreateScale(scales);
			return identity;
		}

		public static void CreateScale(ref IndexedVector3 scales, out IndexedMatrix result)
		{
			result = Identity;
			result._basis = IndexedBasisMatrix.CreateScale(scales);
		}

		public static IndexedMatrix CreateScale(float scale)
		{
			IndexedMatrix identity = Identity;
			identity._basis = IndexedBasisMatrix.CreateScale(new IndexedVector3(scale));
			return identity;
		}

		public static void CreateScale(float scale, out IndexedMatrix result)
		{
			result = Identity;
			result._basis = IndexedBasisMatrix.CreateScale(new IndexedVector3(scale));
		}

		public static IndexedMatrix CreateRotationX(float radians)
		{
			IndexedMatrix result = default(IndexedMatrix);
			result._basis = IndexedBasisMatrix.CreateRotationX(radians);
			result._origin = new IndexedVector3(0f, 0f, 0f);
			return result;
		}

		public static void CreateRotationX(float radians, out IndexedMatrix result)
		{
			result._basis = IndexedBasisMatrix.CreateRotationX(radians);
			result._origin = new IndexedVector3(0f, 0f, 0f);
		}

		public static IndexedMatrix CreateRotationY(float radians)
		{
			IndexedMatrix result = default(IndexedMatrix);
			result._basis = IndexedBasisMatrix.CreateRotationY(radians);
			result._origin = new IndexedVector3(0f, 0f, 0f);
			return result;
		}

		public static void CreateRotationY(float radians, out IndexedMatrix result)
		{
			result._basis = IndexedBasisMatrix.CreateRotationY(radians);
			result._origin = new IndexedVector3(0f, 0f, 0f);
		}

		public static IndexedMatrix CreateRotationZ(float radians)
		{
			IndexedMatrix result = default(IndexedMatrix);
			result._basis = IndexedBasisMatrix.CreateRotationZ(radians);
			result._origin = new IndexedVector3(0f, 0f, 0f);
			return result;
		}

		public static void CreateRotationZ(float radians, out IndexedMatrix result)
		{
			result._basis = IndexedBasisMatrix.CreateRotationZ(radians);
			result._origin = new IndexedVector3(0f, 0f, 0f);
		}

		public bool Equals(IndexedMatrix other)
		{
			if (_basis.Equals(other._basis))
			{
				return _origin.Equals(other._origin);
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			bool result = false;
			if (obj is IndexedMatrix)
			{
				result = Equals((IndexedMatrix)obj);
			}
			return result;
		}

		public override int GetHashCode()
		{
			return _basis.GetHashCode() + _origin.GetHashCode();
		}

		public IndexedMatrix Inverse()
		{
			IndexedBasisMatrix indexedBasisMatrix = _basis.Transpose();
			return new IndexedMatrix(indexedBasisMatrix, indexedBasisMatrix * -_origin);
		}

		public IndexedVector3 InvXform(IndexedVector3 inVec)
		{
			IndexedVector3 indexedVector = inVec - _origin;
			return _basis.Transpose() * indexedVector;
		}

		public IndexedVector3 InvXform(ref IndexedVector3 inVec)
		{
			IndexedVector3 indexedVector = inVec - _origin;
			return _basis.Transpose() * indexedVector;
		}

		public IndexedMatrix InverseTimes(ref IndexedMatrix t)
		{
			IndexedVector3 v = new IndexedVector3(t._origin.X - _origin.X, t._origin.Y - _origin.Y, t._origin.Z - _origin.Z);
			IndexedVector3 vout = default(IndexedVector3);
			IndexedBasisMatrix.Multiply(ref vout, ref _basis, ref v);
			return new IndexedMatrix(_basis.TransposeTimes(ref t._basis), v * _basis);
		}

		public void SetRotation(IndexedQuaternion q)
		{
			_basis.SetRotation(ref q);
		}

		public void SetRotation(ref IndexedQuaternion q)
		{
			_basis.SetRotation(ref q);
		}

		public IndexedQuaternion GetRotation()
		{
			return _basis.GetRotation();
		}

		public static IndexedMatrix CreateFromQuaternion(IndexedQuaternion q)
		{
			IndexedMatrix result = default(IndexedMatrix);
			result._basis.SetRotation(ref q);
			return result;
		}

		public static IndexedMatrix CreateFromQuaternion(ref IndexedQuaternion q)
		{
			IndexedMatrix result = default(IndexedMatrix);
			result._basis.SetRotation(ref q);
			return result;
		}

		public void SetMatrix4x4(Matrix4x4 m)
		{
			_basis._el0.X = m.m00;
			_basis._el1.X = m.m10;
			_basis._el2.X = m.m20;
			_basis._el0.Y = m.m01;
			_basis._el1.Y = m.m11;
			_basis._el2.Y = m.m21;
			_basis._el0.Z = m.m02;
			_basis._el1.Z = m.m12;
			_basis._el2.Z = m.m22;
			_origin.X = m.m03;
			_origin.Y = m.m13;
			_origin.Z = m.m23;
		}

		public void SetMatrix4x4(ref Matrix4x4 m)
		{
			_basis._el0.X = m.m00;
			_basis._el1.X = m.m10;
			_basis._el2.X = m.m20;
			_basis._el0.Y = m.m01;
			_basis._el1.Y = m.m11;
			_basis._el2.Y = m.m21;
			_basis._el0.Z = m.m02;
			_basis._el1.Z = m.m12;
			_basis._el2.Z = m.m22;
			_origin.X = m.m03;
			_origin.Y = m.m13;
			_origin.Z = m.m23;
		}

		public IndexedMatrix(Matrix4x4 m)
		{
			_basis._el0.X = m.m00;
			_basis._el1.X = m.m10;
			_basis._el2.X = m.m20;
			_basis._el0.Y = m.m01;
			_basis._el1.Y = m.m11;
			_basis._el2.Y = m.m21;
			_basis._el0.Z = m.m02;
			_basis._el1.Z = m.m12;
			_basis._el2.Z = m.m22;
			_origin.X = m.m03;
			_origin.Y = m.m13;
			_origin.Z = m.m23;
		}

		public IndexedMatrix(ref Matrix4x4 m)
		{
			_basis._el0.X = m.m00;
			_basis._el1.X = m.m10;
			_basis._el2.X = m.m20;
			_basis._el0.Y = m.m01;
			_basis._el1.Y = m.m11;
			_basis._el2.Y = m.m21;
			_basis._el0.Z = m.m02;
			_basis._el1.Z = m.m12;
			_basis._el2.Z = m.m22;
			_origin.X = m.m03;
			_origin.Y = m.m13;
			_origin.Z = m.m23;
		}

		public Matrix4x4 ToMatrix4x4()
		{
			Matrix4x4 identity = Matrix4x4.identity;
			identity.m00 = _basis._el0.X;
			identity.m10 = _basis._el1.X;
			identity.m20 = _basis._el2.X;
			identity.m01 = _basis._el0.Y;
			identity.m11 = _basis._el1.Y;
			identity.m21 = _basis._el2.Y;
			identity.m02 = _basis._el0.Z;
			identity.m12 = _basis._el1.Z;
			identity.m22 = _basis._el2.Z;
			identity.m03 = _origin.X;
			identity.m13 = _origin.Y;
			identity.m23 = _origin.Z;
			return identity;
		}

		public void ToMatrix4x4(out Matrix4x4 m)
		{
			m.m00 = _basis._el0.X;
			m.m10 = _basis._el1.X;
			m.m20 = _basis._el2.X;
			m.m30 = 0f;
			m.m01 = _basis._el0.Y;
			m.m11 = _basis._el1.Y;
			m.m21 = _basis._el2.Y;
			m.m31 = 0f;
			m.m02 = _basis._el0.Z;
			m.m12 = _basis._el1.Z;
			m.m22 = _basis._el2.Z;
			m.m32 = 0f;
			m.m03 = _origin.X;
			m.m13 = _origin.Y;
			m.m23 = _origin.Z;
			m.m33 = 1f;
		}

		public static implicit operator Matrix4x4(IndexedMatrix im)
		{
			return im.ToMatrix4x4();
		}
	}
}
