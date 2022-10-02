using System;

namespace UnityEngine.InputSystem.Utilities
{
	internal static class NumberHelpers
	{
		public static int AlignToMultipleOf(this int number, int alignment)
		{
			int num = number % alignment;
			if (num == 0)
			{
				return number;
			}
			return number + alignment - num;
		}

		public static uint AlignToMultipleOf(this uint number, uint alignment)
		{
			uint num = number % alignment;
			if (num == 0)
			{
				return number;
			}
			return number + alignment - num;
		}

		public static bool Approximately(double a, double b)
		{
			return Math.Abs(b - a) < Math.Max(1E-06 * Math.Max(Math.Abs(a), Math.Abs(b)), 4E-323);
		}
	}
}
