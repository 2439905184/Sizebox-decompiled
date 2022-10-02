namespace BulletXNA.LinearMath
{
	public class HullTriangle : int3
	{
		public int3 n;

		public int id;

		public int vmax;

		public float rise;

		public HullTriangle(int a, int b, int c)
			: base(a, b, c)
		{
			n = new int3(-1, -1, -1);
			vmax = -1;
			rise = 0f;
		}

		public int Neib(int a, int b)
		{
			int result = -1;
			for (int i = 0; i < 3; i++)
			{
				int index = (i + 1) % 3;
				int index2 = (i + 2) % 3;
				if (At(i) == a && At(index) == b)
				{
					return n.At(index2);
				}
				if (At(i) == b && At(index) == a)
				{
					return n.At(index2);
				}
			}
			return result;
		}

		public void Neib(int a, int b, int value)
		{
			for (int i = 0; i < 3; i++)
			{
				int index = (i + 1) % 3;
				int index2 = (i + 2) % 3;
				if (At(i) == a && At(index) == b)
				{
					n.At(index2, value);
					break;
				}
				if (At(i) == b && At(index) == a)
				{
					n.At(index2, value);
					break;
				}
			}
		}
	}
}
