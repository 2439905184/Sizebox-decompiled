namespace BulletXNA.LinearMath
{
	public class int3
	{
		public int x;

		public int y;

		public int z;

		public int3()
		{
		}

		public int3(int _x, int _y, int _z)
		{
			x = _x;
			y = _y;
			z = _z;
		}

		public override bool Equals(object obj)
		{
			int3 int5 = (int3)obj;
			if (x != int5.x || y != int5.y || z != int5.z)
			{
				return false;
			}
			return true;
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
			}
		}
	}
}
