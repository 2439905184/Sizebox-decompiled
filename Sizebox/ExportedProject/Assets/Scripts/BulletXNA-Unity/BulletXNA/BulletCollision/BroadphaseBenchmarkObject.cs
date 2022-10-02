using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class BroadphaseBenchmarkObject
	{
		public IndexedVector3 center;

		public IndexedVector3 extents;

		public BroadphaseProxy proxy;

		public float time;

		public void update(float speed, float amplitude, IBroadphaseInterface pbi)
		{
			time += speed;
			center.X = (float)(Math.Cos(time * 2.17f) * (double)amplitude + Math.Sin(time) * (double)amplitude / 2.0);
			center.Y = (float)(Math.Cos(time * 1.38f) * (double)amplitude + Math.Sin(time) * (double)amplitude);
			center.Z = (float)(Math.Sin(time * 0.777f) * (double)amplitude);
			IndexedVector3 aabbMin = center - extents;
			IndexedVector3 aabbMax = center + extents;
			pbi.SetAabb(proxy, ref aabbMin, ref aabbMax, null);
		}
	}
}
