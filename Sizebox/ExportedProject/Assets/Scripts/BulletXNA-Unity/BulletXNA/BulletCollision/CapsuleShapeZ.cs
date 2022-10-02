using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class CapsuleShapeZ : CapsuleShape
	{
		public CapsuleShapeZ(float radius, float height)
		{
			m_upAxis = 2;
			m_implicitShapeDimensions = new IndexedVector3(radius, radius, 0.5f * height);
		}

		public override string GetName()
		{
			return "CapsuleZ";
		}
	}
}
