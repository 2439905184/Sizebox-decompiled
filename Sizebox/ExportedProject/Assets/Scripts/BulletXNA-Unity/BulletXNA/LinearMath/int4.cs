namespace BulletXNA.LinearMath
{
	public class int4
	{
		public int x;

		public int y;

		public int z;

		public int w;

		public int4()
		{
		}

		public int4(int _x, int _y, int _z, int _w)
		{
			x = _x;
			y = _y;
			z = _z;
			w = _w;
		}

		public int At(int index)
		{
			switch (index)
			{
			case 0:
				return x;
			case 1:
				return y;
			case 2:
				return z;
			case 3:
				return w;
			default:
				return -1;
			}
		}

		public void At(int index, int value)
		{
			switch (index)
			{
			case 0:
				x = value;
				break;
			case 1:
				y = value;
				break;
			case 2:
				z = value;
				break;
			case 3:
				w = value;
				break;
			}
		}
	}
}
