using System.Diagnostics;

namespace BulletXNA.BulletCollision
{
	public static class BroadphaseBenchmark
	{
		public static int UnsignedRand()
		{
			return UnsignedRand(BulletGlobals.RAND_MAX - 1);
		}

		public static int UnsignedRand(int range)
		{
			return BulletGlobals.gRandom.Next(range + 1);
		}

		public static float UnitRand()
		{
			return (float)UnsignedRand(16384) / 16384f;
		}

		public static void OutputTime(string name, Stopwatch sw, uint count)
		{
			ulong elapsedMilliseconds = (ulong)sw.ElapsedMilliseconds;
			ulong num = (elapsedMilliseconds + 500) / 1000uL;
			float num2 = (float)elapsedMilliseconds / 1000000f;
		}
	}
}
