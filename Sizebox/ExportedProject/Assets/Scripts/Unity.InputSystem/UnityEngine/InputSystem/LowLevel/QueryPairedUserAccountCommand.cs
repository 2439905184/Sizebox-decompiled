using System;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 1040)]
	public struct QueryPairedUserAccountCommand : IInputDeviceCommandInfo
	{
		[Flags]
		public enum Result : long
		{
			DevicePairedToUserAccount = 2L,
			UserAccountSelectionInProgress = 4L,
			UserAccountSelectionComplete = 8L,
			UserAccountSelectionCanceled = 0x10L
		}

		internal const int kMaxNameLength = 256;

		internal const int kMaxIdLength = 256;

		internal const int kSize = 1040;

		[FieldOffset(0)]
		public InputDeviceCommand baseCommand;

		[FieldOffset(8)]
		public ulong handle;

		[FieldOffset(16)]
		internal unsafe fixed byte nameBuffer[512];

		[FieldOffset(528)]
		internal unsafe fixed byte idBuffer[512];

		public static FourCC Type
		{
			get
			{
				return new FourCC('P', 'A', 'C', 'C');
			}
		}

		public unsafe string id
		{
			get
			{
				fixed (byte* value = idBuffer)
				{
					return StringHelpers.ReadStringFromBuffer(new IntPtr(value), 256);
				}
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (value.Length > 256)
				{
					throw new ArgumentException(string.Format("ID '{0}' exceeds maximum supported length of {1} characters", value, 256), "value");
				}
				fixed (byte* value2 = idBuffer)
				{
					StringHelpers.WriteStringToBuffer(value, new IntPtr(value2), 256);
				}
			}
		}

		public unsafe string name
		{
			get
			{
				fixed (byte* value = nameBuffer)
				{
					return StringHelpers.ReadStringFromBuffer(new IntPtr(value), 256);
				}
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (value.Length > 256)
				{
					throw new ArgumentException(string.Format("Name '{0}' exceeds maximum supported length of {1} characters", value, 256), "value");
				}
				fixed (byte* value2 = nameBuffer)
				{
					StringHelpers.WriteStringToBuffer(value, new IntPtr(value2), 256);
				}
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return Type;
			}
		}

		public static QueryPairedUserAccountCommand Create()
		{
			QueryPairedUserAccountCommand result = default(QueryPairedUserAccountCommand);
			result.baseCommand = new InputDeviceCommand(Type, 1040);
			return result;
		}
	}
}
