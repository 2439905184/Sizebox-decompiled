namespace UltimateFracturing
{
	public struct ClippedEdge
	{
		public int nOldIndexA;

		public int nOldIndexB;

		public int nNewIndexA;

		public int nNewIndexB;

		public int nClippedIndex;

		public ClippedEdge(int nOldIndexA, int nOldIndexB, int nNewIndexA, int nNewIndexB, int nClippedIndex)
		{
			this.nOldIndexA = nOldIndexA;
			this.nOldIndexB = nOldIndexB;
			this.nNewIndexA = nNewIndexA;
			this.nNewIndexB = nNewIndexB;
			this.nClippedIndex = nClippedIndex;
		}

		public int GetFirstIndex(int nOldIndexA)
		{
			if (this.nOldIndexA == nOldIndexA)
			{
				return nNewIndexA;
			}
			return nNewIndexB;
		}

		public int GetSecondIndex(int nOldIndexB)
		{
			if (this.nOldIndexB == nOldIndexB)
			{
				return nNewIndexB;
			}
			return nNewIndexA;
		}
	}
}
