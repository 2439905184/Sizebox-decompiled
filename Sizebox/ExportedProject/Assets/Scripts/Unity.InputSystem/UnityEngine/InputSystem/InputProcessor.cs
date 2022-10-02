using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem
{
	[Preserve]
	public abstract class InputProcessor
	{
		internal static TypeTable s_Processors;

		public abstract object ProcessAsObject(object value, InputControl control);

		public unsafe abstract void Process(void* buffer, int bufferSize, InputControl control);

		internal static Type GetValueTypeFromType(Type processorType)
		{
			if (processorType == null)
			{
				throw new ArgumentNullException("processorType");
			}
			return TypeHelpers.GetGenericTypeArgumentFromHierarchy(processorType, typeof(InputProcessor<>), 0);
		}
	}
	[Preserve]
	public abstract class InputProcessor<TValue> : InputProcessor where TValue : struct
	{
		public abstract TValue Process(TValue value, InputControl control);

		public override object ProcessAsObject(object value, InputControl control)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (!(value is TValue))
			{
				throw new ArgumentException(string.Format("Expecting value of type '{0}' but got value '{1}' of type '{2}'", typeof(TValue).Name, value, value.GetType().Name), "value");
			}
			TValue value2 = (TValue)value;
			return Process(value2, control);
		}

		public unsafe override void Process(void* buffer, int bufferSize, InputControl control)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			int num = UnsafeUtility.SizeOf<TValue>();
			if (bufferSize < num)
			{
				throw new ArgumentException(string.Format("Expected buffer of at least {0} bytes but got buffer with just {1} bytes", num, bufferSize), "bufferSize");
			}
			TValue output = default(TValue);
			void* destination = UnsafeUtility.AddressOf(ref output);
			UnsafeUtility.MemCpy(destination, buffer, num);
			output = Process(output, control);
			destination = UnsafeUtility.AddressOf(ref output);
			UnsafeUtility.MemCpy(buffer, destination, num);
		}
	}
}
