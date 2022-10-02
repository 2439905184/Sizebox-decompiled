namespace BulletXNA.BulletCollision
{
	public class ConeShapeX : ConeShape
	{
		public ConeShapeX(float radius, float height)
			: base(radius, height)
		{
			SetConeUpIndex(0);
		}
	}
}
