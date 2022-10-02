using System.Collections.Generic;

namespace UltimateFracturing
{
	public struct EdgeKeyByIndex
	{
		public class EqualityComparer : IEqualityComparer<EdgeKeyByIndex>
		{
			public bool Equals(EdgeKeyByIndex x, EdgeKeyByIndex y)
			{
				return x.CompareTo(y.nIndexA, y.nIndexB);
			}

			public int GetHashCode(EdgeKeyByIndex x)
			{
				return x.nIndexA.GetHashCode() + x.nIndexB.GetHashCode();
			}
		}

		public int nIndexA;

		public int nIndexB;

		public EdgeKeyByIndex(int nIndexA, int nIndexB)
		{
			this.nIndexA = nIndexA;
			this.nIndexB = nIndexB;
		}

		public void Set(int nIndexA, int nIndexB)
		{
			this.nIndexA = nIndexA;
			this.nIndexB = nIndexB;
		}

		public bool CompareTo(int nIndexA, int nIndexB)
		{
			if (this.nIndexA == nIndexA && this.nIndexB == nIndexB)
			{
				return true;
			}
			if (this.nIndexA == nIndexB && this.nIndexB == nIndexA)
			{
				return true;
			}
			return false;
		}
	}
}
