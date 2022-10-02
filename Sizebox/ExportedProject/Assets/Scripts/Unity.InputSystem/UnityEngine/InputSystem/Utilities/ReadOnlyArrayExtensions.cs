using System;

namespace UnityEngine.InputSystem.Utilities
{
	public static class ReadOnlyArrayExtensions
	{
		public static bool Contains<TValue>(this ReadOnlyArray<TValue> array, TValue value) where TValue : IComparable<TValue>
		{
			for (int i = 0; i < array.m_Length; i++)
			{
				if (array.m_Array[array.m_StartIndex + i].CompareTo(value) == 0)
				{
					return true;
				}
			}
			return false;
		}

		public static bool ContainsReference<TValue>(this ReadOnlyArray<TValue> array, TValue value) where TValue : class
		{
			return array.IndexOfReference(value) != -1;
		}

		public static int IndexOfReference<TValue>(this ReadOnlyArray<TValue> array, TValue value) where TValue : class
		{
			for (int i = 0; i < array.m_Length; i++)
			{
				if (array.m_Array[array.m_StartIndex + i] == value)
				{
					return i;
				}
			}
			return -1;
		}
	}
}
