using UnityEngine;

namespace RuntimeGizmos
{
	public struct Square
	{
		public Vector3 bottomLeft;

		public Vector3 bottomRight;

		public Vector3 topLeft;

		public Vector3 topRight;

		public Vector3 this[int index]
		{
			get
			{
				switch (index)
				{
				case 0:
					return bottomLeft;
				case 1:
					return bottomRight;
				case 2:
					return topLeft;
				case 3:
					return topRight;
				case 4:
					return bottomLeft;
				default:
					return Vector3.zero;
				}
			}
		}
	}
}
