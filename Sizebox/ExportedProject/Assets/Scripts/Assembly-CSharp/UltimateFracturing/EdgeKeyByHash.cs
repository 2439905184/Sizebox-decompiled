using System.Collections.Generic;

namespace UltimateFracturing
{
	public struct EdgeKeyByHash
	{
		public class EqualityComparer : IEqualityComparer<EdgeKeyByHash>
		{
			public bool Equals(EdgeKeyByHash x, EdgeKeyByHash y)
			{
				return x.CompareTo(y.nHashA, y.nHashB);
			}

			public int GetHashCode(EdgeKeyByHash x)
			{
				return x.nHashA.GetHashCode() + x.nHashB.GetHashCode();
			}
		}

		public int nHashA;

		public int nHashB;

		public EdgeKeyByHash(int nHashA, int nHashB)
		{
			this.nHashA = nHashA;
			this.nHashB = nHashB;
		}

		public void Set(int nHashA, int nHashB)
		{
			this.nHashA = nHashA;
			this.nHashB = nHashB;
		}

		public bool CompareTo(int nHashA, int nHashB)
		{
			if (this.nHashA == nHashA && this.nHashB == nHashB)
			{
				return true;
			}
			if (this.nHashA == nHashB && this.nHashB == nHashA)
			{
				return true;
			}
			return false;
		}
	}
}
