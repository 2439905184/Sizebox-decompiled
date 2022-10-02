using UnityEngine;

namespace UltimateFracturing
{
	public struct CapEdge
	{
		public Vector3 v1;

		public Vector3 v2;

		public int nHash1;

		public int nHash2;

		public float fLength;

		public CapEdge(int nHash1, int nHash2, Vector3 v1, Vector3 v2, float fLength)
		{
			this.nHash1 = nHash1;
			this.nHash2 = nHash2;
			this.v1 = v1;
			this.v2 = v2;
			this.fLength = fLength;
		}

		public int SharesVertex1Of(CapEdge edge)
		{
			if (nHash1 == edge.nHash1)
			{
				return 1;
			}
			if (nHash2 == edge.nHash1)
			{
				return 2;
			}
			return 0;
		}

		public int SharesVertex2Of(CapEdge edge)
		{
			if (nHash1 == edge.nHash2)
			{
				return 1;
			}
			if (nHash2 == edge.nHash2)
			{
				return 2;
			}
			return 0;
		}
	}
}
