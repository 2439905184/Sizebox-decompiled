using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UnityEngine.InputSystem
{
	public struct InputBindingCompositeContext
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct DefaultComparer<TValue> : IComparer<TValue> where TValue : IComparable<TValue>
		{
			public int Compare(TValue x, TValue y)
			{
				return x.CompareTo(y);
			}
		}

		internal InputActionState m_State;

		internal int m_BindingIndex;

		public unsafe TValue ReadValue<TValue>(int partNumber) where TValue : struct, IComparable<TValue>
		{
			if (m_State == null)
			{
				return default(TValue);
			}
			int controlIndex;
			return m_State.ReadCompositePartValue<TValue, DefaultComparer<TValue>>(m_BindingIndex, partNumber, null, out controlIndex);
		}

		public unsafe TValue ReadValue<TValue>(int partNumber, out InputControl sourceControl) where TValue : struct, IComparable<TValue>
		{
			if (m_State == null)
			{
				sourceControl = null;
				return default(TValue);
			}
			int controlIndex;
			TValue result = m_State.ReadCompositePartValue<TValue, DefaultComparer<TValue>>(m_BindingIndex, partNumber, null, out controlIndex);
			if (controlIndex != -1)
			{
				sourceControl = m_State.controls[controlIndex];
				return result;
			}
			sourceControl = null;
			return result;
		}

		public unsafe TValue ReadValue<TValue, TComparer>(int partNumber, TComparer comparer = default(TComparer)) where TValue : struct where TComparer : IComparer<TValue>
		{
			if (m_State == null)
			{
				return default(TValue);
			}
			int controlIndex;
			return m_State.ReadCompositePartValue<TValue, TComparer>(m_BindingIndex, partNumber, null, out controlIndex, comparer);
		}

		public unsafe TValue ReadValue<TValue, TComparer>(int partNumber, out InputControl sourceControl, TComparer comparer = default(TComparer)) where TValue : struct where TComparer : IComparer<TValue>
		{
			if (m_State == null)
			{
				sourceControl = null;
				return default(TValue);
			}
			int controlIndex;
			TValue result = m_State.ReadCompositePartValue<TValue, TComparer>(m_BindingIndex, partNumber, null, out controlIndex, comparer);
			if (controlIndex != -1)
			{
				sourceControl = m_State.controls[controlIndex];
				return result;
			}
			sourceControl = null;
			return result;
		}

		public unsafe bool ReadValueAsButton(int partNumber)
		{
			if (m_State == null)
			{
				return false;
			}
			bool result = false;
			int controlIndex;
			m_State.ReadCompositePartValue<float, DefaultComparer<float>>(m_BindingIndex, partNumber, &result, out controlIndex);
			return result;
		}
	}
}
