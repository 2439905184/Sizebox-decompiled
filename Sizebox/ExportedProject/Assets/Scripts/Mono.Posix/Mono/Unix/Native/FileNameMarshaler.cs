using System;
using System.Runtime.InteropServices;

namespace Mono.Unix.Native
{
	internal class FileNameMarshaler : ICustomMarshaler
	{
		private static FileNameMarshaler Instance = new FileNameMarshaler();

		public static ICustomMarshaler GetInstance(string s)
		{
			return Instance;
		}

		public void CleanUpManagedData(object o)
		{
		}

		public void CleanUpNativeData(IntPtr pNativeData)
		{
			UnixMarshal.FreeHeap(pNativeData);
		}

		public int GetNativeDataSize()
		{
			return IntPtr.Size;
		}

		public IntPtr MarshalManagedToNative(object obj)
		{
			string text = obj as string;
			if (text == null)
			{
				return IntPtr.Zero;
			}
			return UnixMarshal.StringToHeap(text, UnixEncoding.Instance);
		}

		public object MarshalNativeToManaged(IntPtr pNativeData)
		{
			return UnixMarshal.PtrToString(pNativeData, UnixEncoding.Instance);
		}
	}
}
