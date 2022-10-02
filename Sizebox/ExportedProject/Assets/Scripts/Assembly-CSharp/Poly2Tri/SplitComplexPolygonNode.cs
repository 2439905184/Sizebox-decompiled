using System;
using System.Collections.Generic;
using System.Text;

namespace Poly2Tri
{
	public class SplitComplexPolygonNode
	{
		private List<SplitComplexPolygonNode> mConnected = new List<SplitComplexPolygonNode>();

		private Point2D mPosition;

		public int NumConnected
		{
			get
			{
				return mConnected.Count;
			}
		}

		public Point2D Position
		{
			get
			{
				return mPosition;
			}
			set
			{
				mPosition = value;
			}
		}

		public SplitComplexPolygonNode this[int index]
		{
			get
			{
				return mConnected[index];
			}
		}

		public SplitComplexPolygonNode()
		{
		}

		public SplitComplexPolygonNode(Point2D pos)
		{
			mPosition = pos;
		}

		public override bool Equals(object obj)
		{
			SplitComplexPolygonNode splitComplexPolygonNode = obj as SplitComplexPolygonNode;
			if (splitComplexPolygonNode == null)
			{
				return base.Equals(obj);
			}
			return Equals(splitComplexPolygonNode);
		}

		public bool Equals(SplitComplexPolygonNode pn)
		{
			if ((object)pn == null)
			{
				return false;
			}
			if (mPosition == null || pn.Position == null)
			{
				return false;
			}
			return mPosition.Equals(pn.Position);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(SplitComplexPolygonNode lhs, SplitComplexPolygonNode rhs)
		{
			if ((object)lhs != null)
			{
				return lhs.Equals(rhs);
			}
			if ((object)rhs == null)
			{
				return true;
			}
			return false;
		}

		public static bool operator !=(SplitComplexPolygonNode lhs, SplitComplexPolygonNode rhs)
		{
			if ((object)lhs != null)
			{
				return !lhs.Equals(rhs);
			}
			if ((object)rhs == null)
			{
				return false;
			}
			return true;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			stringBuilder.Append(mPosition.ToString());
			stringBuilder.Append(" -> ");
			for (int i = 0; i < NumConnected; i++)
			{
				if (i != 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(mConnected[i].Position.ToString());
			}
			return stringBuilder.ToString();
		}

		private bool IsRighter(double sinA, double cosA, double sinB, double cosB)
		{
			if (sinA < 0.0)
			{
				if (sinB > 0.0 || cosA <= cosB)
				{
					return true;
				}
				return false;
			}
			if (sinB < 0.0 || cosA <= cosB)
			{
				return false;
			}
			return true;
		}

		private int remainder(int x, int modulus)
		{
			int i;
			for (i = x % modulus; i < 0; i += modulus)
			{
			}
			return i;
		}

		public void AddConnection(SplitComplexPolygonNode toMe)
		{
			if (!mConnected.Contains(toMe) && toMe != this)
			{
				mConnected.Add(toMe);
			}
		}

		public void RemoveConnection(SplitComplexPolygonNode fromMe)
		{
			mConnected.Remove(fromMe);
		}

		private void RemoveConnectionByIndex(int index)
		{
			if (index >= 0 && index < mConnected.Count)
			{
				mConnected.RemoveAt(index);
			}
		}

		public void ClearConnections()
		{
			mConnected.Clear();
		}

		private bool IsConnectedTo(SplitComplexPolygonNode me)
		{
			return mConnected.Contains(me);
		}

		public SplitComplexPolygonNode GetRightestConnection(SplitComplexPolygonNode incoming)
		{
			if (NumConnected == 0)
			{
				throw new Exception("the connection graph is inconsistent");
			}
			if (NumConnected == 1)
			{
				return incoming;
			}
			Point2D point2D = mPosition - incoming.mPosition;
			double num = point2D.Magnitude();
			point2D.Normalize();
			if (num <= MathUtil.EPSILON)
			{
				throw new Exception("Length too small");
			}
			SplitComplexPolygonNode splitComplexPolygonNode = null;
			for (int i = 0; i < NumConnected; i++)
			{
				if (mConnected[i] == incoming)
				{
					continue;
				}
				Point2D point2D2 = mConnected[i].mPosition - mPosition;
				double num2 = point2D2.MagnitudeSquared();
				point2D2.Normalize();
				if (num2 <= MathUtil.EPSILON * MathUtil.EPSILON)
				{
					throw new Exception("Length too small");
				}
				double cosA = Point2D.Dot(point2D, point2D2);
				double sinA = Point2D.Cross(point2D, point2D2);
				if (splitComplexPolygonNode != null)
				{
					Point2D point2D3 = splitComplexPolygonNode.mPosition - mPosition;
					point2D3.Normalize();
					double cosB = Point2D.Dot(point2D, point2D3);
					double sinB = Point2D.Cross(point2D, point2D3);
					if (IsRighter(sinA, cosA, sinB, cosB))
					{
						splitComplexPolygonNode = mConnected[i];
					}
				}
				else
				{
					splitComplexPolygonNode = mConnected[i];
				}
			}
			return splitComplexPolygonNode;
		}

		public SplitComplexPolygonNode GetRightestConnection(Point2D incomingDir)
		{
			SplitComplexPolygonNode incoming = new SplitComplexPolygonNode(mPosition - incomingDir);
			return GetRightestConnection(incoming);
		}
	}
}
