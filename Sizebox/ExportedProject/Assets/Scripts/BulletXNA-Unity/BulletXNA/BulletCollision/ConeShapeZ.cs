namespace BulletXNA.BulletCollision
{
	public class ConeShapeZ : ConeShape
	{
		public ConeShapeZ(float radius, float height)
			: base(radius, height)
		{
			SetConeUpIndex(2);
		}
	}
}
