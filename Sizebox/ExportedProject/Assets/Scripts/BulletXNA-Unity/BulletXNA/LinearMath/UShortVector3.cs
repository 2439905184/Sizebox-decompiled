namespace BulletXNA.LinearMath
{
	public struct UShortVector3
	{
		public ushort X;

		public ushort Y;

		public ushort Z;

		public ushort this[int i]
		{
			get
			{
				switch (i)
				{
				case 0:
					return X;
				case 1:
					return Y;
				case 2:
					return Z;
				default:
					return 0;
				}
			}
			set
			{
				switch (i)
				{
				case 0:
					X = value;
					break;
				case 1:
					Y = value;
					break;
				case 2:
					Z = value;
					break;
				}
			}
		}

		public void Min(ref UShortVector3 a)
		{
			if (X > a.X)
			{
				X = a.X;
			}
			if (Y > a.Y)
			{
				Y = a.Y;
			}
			if (Z > a.Z)
			{
				Z = a.Z;
			}
		}

		public void Max(ref UShortVector3 a)
		{
			if (X < a.X)
			{
				X = a.X;
			}
			if (Y < a.Y)
			{
				Y = a.Y;
			}
			if (Z < a.Z)
			{
				Z = a.Z;
			}
		}
	}
}
