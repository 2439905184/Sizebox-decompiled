using System;

namespace Poly2Tri
{
	public class TriangulationConstraint : Edge
	{
		private uint mContraintCode;

		public TriangulationPoint P
		{
			get
			{
				return mP as TriangulationPoint;
			}
			set
			{
				if (value != null && mP != value)
				{
					mP = value;
					CalculateContraintCode();
				}
			}
		}

		public TriangulationPoint Q
		{
			get
			{
				return mQ as TriangulationPoint;
			}
			set
			{
				if (value != null && mQ != value)
				{
					mQ = value;
					CalculateContraintCode();
				}
			}
		}

		public uint ConstraintCode
		{
			get
			{
				return mContraintCode;
			}
		}

		public TriangulationConstraint(TriangulationPoint p1, TriangulationPoint p2)
		{
			mP = p1;
			mQ = p2;
			if (p1.Y > p2.Y)
			{
				mQ = p1;
				mP = p2;
			}
			else if (p1.Y == p2.Y)
			{
				if (p1.X > p2.X)
				{
					mQ = p1;
					mP = p2;
				}
				else
				{
					double x = p1.X;
					double x2 = p2.X;
				}
			}
			CalculateContraintCode();
		}

		public override string ToString()
		{
			return "[P=" + P.ToString() + ", Q=" + Q.ToString() + " : {" + mContraintCode + "}]";
		}

		public void CalculateContraintCode()
		{
			mContraintCode = CalculateContraintCode(P, Q);
		}

		public static uint CalculateContraintCode(TriangulationPoint p, TriangulationPoint q)
		{
			if (p == null || p == null)
			{
				throw new ArgumentNullException();
			}
			uint nInitialValue = MathUtil.Jenkins32Hash(BitConverter.GetBytes(p.VertexCode), 0u);
			return MathUtil.Jenkins32Hash(BitConverter.GetBytes(q.VertexCode), nInitialValue);
		}
	}
}
