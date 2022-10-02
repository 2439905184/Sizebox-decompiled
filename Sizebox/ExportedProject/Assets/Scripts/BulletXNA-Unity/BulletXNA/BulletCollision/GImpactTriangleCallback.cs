using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	internal class GImpactTriangleCallback : ITriangleCallback
	{
		public GImpactCollisionAlgorithm algorithm;

		public CollisionObject body0;

		public CollisionObject body1;

		public GImpactShapeInterface gimpactshape0;

		public bool swapped;

		public float margin;

		public virtual void ProcessTriangle(IndexedVector3[] triangle, int partId, int triangleIndex)
		{
			TriangleShapeEx triangleShapeEx = new TriangleShapeEx(ref triangle[0], ref triangle[1], ref triangle[2]);
			triangleShapeEx.SetMargin(margin);
			if (swapped)
			{
				algorithm.SetPart0(partId);
				algorithm.SetFace0(triangleIndex);
			}
			else
			{
				algorithm.SetPart1(partId);
				algorithm.SetFace1(triangleIndex);
			}
			algorithm.GImpactVsShape(body0, body1, gimpactshape0, triangleShapeEx, swapped);
		}

		public void Cleanup()
		{
		}

		public bool graphics()
		{
			return false;
		}
	}
}
