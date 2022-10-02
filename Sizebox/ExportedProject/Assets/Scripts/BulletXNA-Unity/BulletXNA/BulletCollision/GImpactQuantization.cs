using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class GImpactQuantization
	{
		public static void CalcQuantizationParameters(out IndexedVector3 outMinBound, out IndexedVector3 outMaxBound, out IndexedVector3 bvhQuantization, ref IndexedVector3 srcMinBound, ref IndexedVector3 srcMaxBound, float quantizationMargin)
		{
			IndexedVector3 indexedVector = new IndexedVector3(quantizationMargin);
			outMinBound = srcMinBound - indexedVector;
			outMaxBound = srcMaxBound + indexedVector;
			IndexedVector3 indexedVector2 = outMaxBound - outMinBound;
			bvhQuantization = new IndexedVector3(65535f) / indexedVector2;
		}

		public static void QuantizeClamp(out UShortVector3 output, ref IndexedVector3 point, ref IndexedVector3 min_bound, ref IndexedVector3 max_bound, ref IndexedVector3 bvhQuantization)
		{
			IndexedVector3 output2 = point;
			MathUtil.VectorMax(ref min_bound, ref output2);
			MathUtil.VectorMin(ref max_bound, ref output2);
			IndexedVector3 indexedVector = (output2 - min_bound) * bvhQuantization;
			output = default(UShortVector3);
			output[0] = (ushort)(indexedVector.X + 0.5f);
			output[1] = (ushort)(indexedVector.Y + 0.5f);
			output[2] = (ushort)(indexedVector.Z + 0.5f);
		}

		public static IndexedVector3 Unquantize(ref UShortVector3 vecIn, ref IndexedVector3 offset, ref IndexedVector3 bvhQuantization)
		{
			IndexedVector3 indexedVector = new IndexedVector3((float)(int)vecIn[0] / bvhQuantization.X, (float)(int)vecIn[1] / bvhQuantization.Y, (float)(int)vecIn[2] / bvhQuantization.Z);
			return indexedVector + offset;
		}
	}
}
