using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	public struct InputStateBlock
	{
		public const uint InvalidOffset = uint.MaxValue;

		public const uint AutomaticOffset = 4294967294u;

		public static readonly FourCC FormatBit = new FourCC('B', 'I', 'T');

		public static readonly FourCC FormatSBit = new FourCC('S', 'B', 'I', 'T');

		public static readonly FourCC FormatInt = new FourCC('I', 'N', 'T');

		public static readonly FourCC FormatUInt = new FourCC('U', 'I', 'N', 'T');

		public static readonly FourCC FormatShort = new FourCC('S', 'H', 'R', 'T');

		public static readonly FourCC FormatUShort = new FourCC('U', 'S', 'H', 'T');

		public static readonly FourCC FormatByte = new FourCC('B', 'Y', 'T', 'E');

		public static readonly FourCC FormatSByte = new FourCC('S', 'B', 'Y', 'T');

		public static readonly FourCC FormatLong = new FourCC('L', 'N', 'G');

		public static readonly FourCC FormatULong = new FourCC('U', 'L', 'N', 'G');

		public static readonly FourCC FormatFloat = new FourCC('F', 'L', 'T');

		public static readonly FourCC FormatDouble = new FourCC('D', 'B', 'L');

		public static readonly FourCC FormatVector2 = new FourCC('V', 'E', 'C', '2');

		public static readonly FourCC FormatVector3 = new FourCC('V', 'E', 'C', '3');

		public static readonly FourCC FormatQuaternion = new FourCC('Q', 'U', 'A', 'T');

		public static readonly FourCC FormatVector2Short = new FourCC('V', 'C', '2', 'S');

		public static readonly FourCC FormatVector3Short = new FourCC('V', 'C', '3', 'S');

		public static readonly FourCC FormatVector2Byte = new FourCC('V', 'C', '2', 'B');

		public static readonly FourCC FormatVector3Byte = new FourCC('V', 'C', '3', 'B');

		public FourCC format { get; set; }

		public uint byteOffset { get; set; }

		public uint bitOffset { get; set; }

		public uint sizeInBits { get; set; }

		internal uint alignedSizeInBytes
		{
			get
			{
				return sizeInBits + 7 >> 3;
			}
		}

		public static int GetSizeOfPrimitiveFormatInBits(FourCC type)
		{
			if (type == FormatBit || type == FormatSBit)
			{
				return 1;
			}
			if (type == FormatInt || type == FormatUInt)
			{
				return 32;
			}
			if (type == FormatShort || type == FormatUShort)
			{
				return 16;
			}
			if (type == FormatByte || type == FormatSByte)
			{
				return 8;
			}
			if (type == FormatFloat)
			{
				return 32;
			}
			if (type == FormatDouble)
			{
				return 64;
			}
			if (type == FormatLong || type == FormatULong)
			{
				return 64;
			}
			if (type == FormatVector2)
			{
				return 64;
			}
			if (type == FormatVector3)
			{
				return 96;
			}
			if (type == FormatQuaternion)
			{
				return 128;
			}
			if (type == FormatVector2Short)
			{
				return 32;
			}
			if (type == FormatVector3Short)
			{
				return 48;
			}
			if (type == FormatVector2Byte)
			{
				return 16;
			}
			if (type == FormatVector3Byte)
			{
				return 24;
			}
			return -1;
		}

		public static FourCC GetPrimitiveFormatFromType(Type type)
		{
			if ((object)type == typeof(int))
			{
				return FormatInt;
			}
			if ((object)type == typeof(uint))
			{
				return FormatUInt;
			}
			if ((object)type == typeof(short))
			{
				return FormatShort;
			}
			if ((object)type == typeof(ushort))
			{
				return FormatUShort;
			}
			if ((object)type == typeof(byte))
			{
				return FormatByte;
			}
			if ((object)type == typeof(sbyte))
			{
				return FormatSByte;
			}
			if ((object)type == typeof(float))
			{
				return FormatFloat;
			}
			if ((object)type == typeof(double))
			{
				return FormatDouble;
			}
			if ((object)type == typeof(long))
			{
				return FormatLong;
			}
			if ((object)type == typeof(ulong))
			{
				return FormatULong;
			}
			if ((object)type == typeof(Vector2))
			{
				return FormatVector2;
			}
			if ((object)type == typeof(Vector3))
			{
				return FormatVector3;
			}
			if ((object)type == typeof(Quaternion))
			{
				return FormatQuaternion;
			}
			return default(FourCC);
		}

		public unsafe int ReadInt(void* statePtr)
		{
			byte* ptr = (byte*)statePtr + (int)byteOffset;
			if (format == FormatInt || format == FormatUInt)
			{
				return *(int*)ptr;
			}
			if (format == FormatBit)
			{
				if (sizeInBits == 1)
				{
					return MemoryHelpers.ReadSingleBit(ptr, bitOffset) ? 1 : 0;
				}
				return MemoryHelpers.ReadIntFromMultipleBits(ptr, bitOffset, sizeInBits);
			}
			if (format == FormatSBit)
			{
				if (sizeInBits == 1)
				{
					return MemoryHelpers.ReadSingleBit(ptr, bitOffset) ? 1 : (-1);
				}
				int num = (1 << (int)sizeInBits) / 2;
				return MemoryHelpers.ReadIntFromMultipleBits(ptr, bitOffset, sizeInBits) - num;
			}
			if (format == FormatByte)
			{
				return *ptr;
			}
			if (format == FormatSByte)
			{
				return *ptr;
			}
			if (format == FormatShort)
			{
				return *(short*)ptr;
			}
			if (format == FormatUShort)
			{
				return *(ushort*)ptr;
			}
			throw new InvalidOperationException(string.Format("State format '{0}' is not supported as integer format", format));
		}

		public unsafe void WriteInt(void* statePtr, int value)
		{
			byte* ptr = (byte*)statePtr + (int)byteOffset;
			if (format == FormatInt || format == FormatUInt)
			{
				*(int*)ptr = value;
			}
			else if (format == FormatBit)
			{
				if (sizeInBits == 1)
				{
					MemoryHelpers.WriteSingleBit(ptr, bitOffset, value != 0);
				}
				else
				{
					MemoryHelpers.WriteIntFromMultipleBits(ptr, bitOffset, sizeInBits, value);
				}
			}
			else if (format == FormatSBit)
			{
				if (sizeInBits == 1)
				{
					MemoryHelpers.WriteSingleBit(ptr, bitOffset, value > 0);
					return;
				}
				int num = (1 << (int)sizeInBits) / 2;
				MemoryHelpers.WriteIntFromMultipleBits(ptr, bitOffset, sizeInBits, value + num);
			}
			else if (format == FormatByte)
			{
				*ptr = (byte)value;
			}
			else if (format == FormatSByte)
			{
				*ptr = (byte)(sbyte)value;
			}
			else if (format == FormatShort)
			{
				*(short*)ptr = (short)value;
			}
			else
			{
				if (!(format == FormatUShort))
				{
					throw new Exception(string.Format("State format '{0}' is not supported as integer format", format));
				}
				*(ushort*)ptr = (ushort)value;
			}
		}

		public unsafe float ReadFloat(void* statePtr)
		{
			byte* ptr = (byte*)statePtr + (int)byteOffset;
			if (format == FormatFloat)
			{
				return *(float*)ptr;
			}
			if (format == FormatBit || format == FormatSBit)
			{
				if (sizeInBits == 1)
				{
					return MemoryHelpers.ReadSingleBit(ptr, bitOffset) ? 1f : ((format == FormatSBit) ? (-1f) : 0f);
				}
				if (sizeInBits <= 31)
				{
					float num = 1 << (int)sizeInBits;
					float num2 = MemoryHelpers.ReadIntFromMultipleBits(ptr, bitOffset, sizeInBits);
					if (format == FormatSBit)
					{
						return Mathf.Clamp(num2 / num * 2f - 1f, -1f, 1f);
					}
					return Mathf.Clamp(num2 / num, 0f, 1f);
				}
				throw new NotImplementedException("Cannot yet convert multi-bit fields greater than 31 bits to floats");
			}
			if (format == FormatShort)
			{
				return (float)(*(short*)ptr) / 32768f;
			}
			if (format == FormatUShort)
			{
				return (float)(int)(*(ushort*)ptr) / 65535f;
			}
			if (format == FormatByte)
			{
				return (float)(int)(*ptr) / 255f;
			}
			if (format == FormatSByte)
			{
				return (float)(int)(*ptr) / 128f;
			}
			if (format == FormatInt)
			{
				return (float)(*(int*)ptr) / 2.1474836E+09f;
			}
			if (format == FormatUInt)
			{
				return (float)(*(uint*)ptr) / 4.2949673E+09f;
			}
			if (format == FormatDouble)
			{
				return (float)(*(double*)ptr);
			}
			throw new InvalidOperationException(string.Format("State format '{0}' is not supported as floating-point format", format));
		}

		public unsafe void WriteFloat(void* statePtr, float value)
		{
			byte* ptr = (byte*)statePtr + (int)byteOffset;
			if (format == FormatFloat)
			{
				*(float*)ptr = value;
			}
			else if (format == FormatBit)
			{
				if (sizeInBits == 1)
				{
					MemoryHelpers.WriteSingleBit(ptr, bitOffset, value >= 0.5f);
					return;
				}
				int num = (1 << (int)sizeInBits) - 1;
				int value2 = (int)(value * (float)num);
				MemoryHelpers.WriteIntFromMultipleBits(ptr, bitOffset, sizeInBits, value2);
			}
			else if (format == FormatShort)
			{
				*(short*)ptr = (short)(value * 32768f);
			}
			else if (format == FormatUShort)
			{
				*(ushort*)ptr = (ushort)(value * 65535f);
			}
			else if (format == FormatByte)
			{
				*ptr = (byte)(value * 255f);
			}
			else if (format == FormatSByte)
			{
				*ptr = (byte)(sbyte)(value * 128f);
			}
			else
			{
				if (!(format == FormatDouble))
				{
					throw new Exception(string.Format("State format '{0}' is not supported as floating-point format", format));
				}
				*(double*)ptr = value;
			}
		}

		internal PrimitiveValue FloatToPrimitiveValue(float value)
		{
			if (format == FormatFloat)
			{
				return value;
			}
			if (format == FormatBit)
			{
				if (sizeInBits == 1)
				{
					return value >= 0.5f;
				}
				int num = (1 << (int)sizeInBits) - 1;
				return (int)(value * (float)num);
			}
			if (format == FormatInt)
			{
				return (int)(value * 2.1474836E+09f);
			}
			if (format == FormatUInt)
			{
				return (uint)(value * 4.2949673E+09f);
			}
			if (format == FormatShort)
			{
				return (short)(value * 32768f);
			}
			if (format == FormatUShort)
			{
				return (ushort)(value * 65535f);
			}
			if (format == FormatByte)
			{
				return (byte)(value * 255f);
			}
			if (format == FormatSByte)
			{
				return (sbyte)(value * 128f);
			}
			if (format == FormatDouble)
			{
				return value;
			}
			throw new Exception(string.Format("State format '{0}' is not supported as floating-point format", format));
		}

		public unsafe double ReadDouble(void* statePtr)
		{
			byte* ptr = (byte*)statePtr + (int)byteOffset;
			if (format == FormatFloat)
			{
				return *(float*)ptr;
			}
			if (format == FormatBit || format == FormatSBit)
			{
				if (sizeInBits == 1)
				{
					return MemoryHelpers.ReadSingleBit(ptr, bitOffset) ? 1f : ((format == FormatSBit) ? (-1f) : 0f);
				}
				if (sizeInBits != 31)
				{
					float num = 1 << (int)sizeInBits;
					float num2 = MemoryHelpers.ReadIntFromMultipleBits(ptr, bitOffset, sizeInBits);
					if (format == FormatSBit)
					{
						return Mathf.Clamp(num2 / num * 2f - 1f, -1f, 1f);
					}
					return Mathf.Clamp(num2 / num, 0f, 1f);
				}
				throw new NotImplementedException("Cannot yet convert multi-bit fields greater than 31 bits to floats");
			}
			if (format == FormatShort)
			{
				return (float)(*(short*)ptr) / 32768f;
			}
			if (format == FormatUShort)
			{
				return (float)(int)(*(ushort*)ptr) / 65535f;
			}
			if (format == FormatByte)
			{
				return (float)(int)(*ptr) / 255f;
			}
			if (format == FormatSByte)
			{
				return (float)(int)(*ptr) / 128f;
			}
			if (format == FormatInt)
			{
				return (float)(*(int*)ptr) / 2.1474836E+09f;
			}
			if (format == FormatUInt)
			{
				return (float)(*(uint*)ptr) / 4.2949673E+09f;
			}
			if (format == FormatDouble)
			{
				return *(double*)ptr;
			}
			throw new Exception(string.Format("State format '{0}' is not supported as floating-point format", format));
		}

		public unsafe void WriteDouble(void* statePtr, double value)
		{
			byte* ptr = (byte*)statePtr + (int)byteOffset;
			if (format == FormatFloat)
			{
				*(float*)ptr = (float)value;
			}
			else if (format == FormatBit)
			{
				if (sizeInBits == 1)
				{
					MemoryHelpers.WriteSingleBit(ptr, bitOffset, value >= 0.5);
					return;
				}
				int num = (1 << (int)sizeInBits) - 1;
				int value2 = (int)(value * (double)num);
				MemoryHelpers.WriteIntFromMultipleBits(ptr, bitOffset, sizeInBits, value2);
			}
			else if (format == FormatShort)
			{
				*(short*)ptr = (short)(value * 32768.0);
			}
			else if (format == FormatUShort)
			{
				*(ushort*)ptr = (ushort)(value * 65535.0);
			}
			else if (format == FormatByte)
			{
				*ptr = (byte)(value * 255.0);
			}
			else if (format == FormatSByte)
			{
				*ptr = (byte)(sbyte)(value * 128.0);
			}
			else
			{
				if (!(format == FormatDouble))
				{
					throw new InvalidOperationException(string.Format("State format '{0}' is not supported as floating-point format", format));
				}
				*(double*)ptr = value;
			}
		}

		public unsafe void Write(void* statePtr, PrimitiveValue value)
		{
			byte* ptr = (byte*)statePtr + (int)byteOffset;
			if (format == FormatBit || format == FormatSBit)
			{
				if (sizeInBits > 32)
				{
					throw new NotImplementedException("Cannot yet write primitive values into bitfields wider than 32 bits");
				}
				if (sizeInBits == 1)
				{
					MemoryHelpers.WriteSingleBit(ptr, bitOffset, value.ToBoolean());
				}
				else
				{
					MemoryHelpers.WriteIntFromMultipleBits(ptr, bitOffset, sizeInBits, value.ToInt32());
				}
			}
			else if (format == FormatFloat)
			{
				*(float*)ptr = value.ToSingle();
			}
			else if (format == FormatByte)
			{
				*ptr = value.ToByte();
			}
			else if (format == FormatShort)
			{
				*(short*)ptr = value.ToInt16();
			}
			else if (format == FormatInt)
			{
				*(int*)ptr = value.ToInt32();
			}
			else if (format == FormatSByte)
			{
				*ptr = (byte)value.ToSByte();
			}
			else if (format == FormatUShort)
			{
				*(ushort*)ptr = value.ToUInt16();
			}
			else
			{
				if (!(format == FormatUInt))
				{
					throw new NotImplementedException(string.Format("Writing primitive value of type '{0}' into state block with format '{1}'", value.type, format));
				}
				*(uint*)ptr = value.ToUInt32();
			}
		}

		public unsafe void CopyToFrom(void* toStatePtr, void* fromStatePtr)
		{
			if (bitOffset != 0 || sizeInBits % 8u != 0)
			{
				throw new NotImplementedException("Copying bitfields");
			}
			byte* source = (byte*)fromStatePtr + byteOffset;
			UnsafeUtility.MemCpy((byte*)toStatePtr + byteOffset, source, alignedSizeInBytes);
		}
	}
}
