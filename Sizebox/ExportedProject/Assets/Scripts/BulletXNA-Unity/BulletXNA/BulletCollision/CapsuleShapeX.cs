using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class CapsuleShapeX : CapsuleShape
	{
		public CapsuleShapeX(float radius, float height)
		{
			m_upAxis = 0;
			m_implicitShapeDimensions = new IndexedVector3(0.5f * height, radius, radius);
		}

		public override string GetName()
		{
			return "CapsuleX";
		}
	}
}
