using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public static class PlaneShape
	{
		public static void GetPlaneEquation(StaticPlaneShape plane, out IndexedVector4 equation)
		{
			equation = new IndexedVector4(plane.GetPlaneNormal(), plane.GetPlaneConstant());
		}

		public static void GetPlaneEquationTransformed(StaticPlaneShape plane, ref IndexedMatrix trans, out IndexedVector4 equation)
		{
			equation = default(IndexedVector4);
			IndexedVector3 v = plane.GetPlaneNormal();
			equation.X = trans._basis.GetRow(0).Dot(ref v);
			equation.Y = trans._basis.GetRow(1).Dot(ref v);
			equation.Z = trans._basis.GetRow(2).Dot(ref v);
			equation.W = trans._origin.Dot(ref v) + plane.GetPlaneConstant();
		}
	}
}
