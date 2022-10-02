using System;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.InputSystem.Utilities
{
	internal static class MemoryHelpers
	{
		public struct BitRegion
		{
			public uint bitOffset;

			public uint sizeInBits;

			public bool isEmpty
			{
				get
				{
					return sizeInBits == 0;
				}
			}

			public BitRegion(uint bitOffset, uint sizeInBits)
			{
				this.bitOffset = bitOffset;
				this.sizeInBits = sizeInBits;
			}

			public BitRegion(uint byteOffset, uint bitOffset, uint sizeInBits)
			{
				this.bitOffset = byteOffset * 8 + bitOffset;
				this.sizeInBits = sizeInBits;
			}

			public BitRegion Overlap(BitRegion other)
			{
				uint num = bitOffset + sizeInBits;
				uint num2 = other.bitOffset + other.sizeInBits;
				if (num <= other.bitOffset || num2 <= bitOffset)
				{
					return default(BitRegion);
				}
				uint num3 = Math.Min(num, num2);
				uint num4 = Math.Max(bitOffset, other.bitOffset);
				return new BitRegion(num4, num3 - num4);
			}
		}

		public unsafe static bool Compare(void* ptr1, void* ptr2, BitRegion region)
		{
			if (region.sizeInBits == 1)
			{
				return ReadSingleBit(ptr1, region.bitOffset) == ReadSingleBit(ptr2, region.bitOffset);
			}
			return MemCmpBitRegion(ptr1, ptr2, region.bitOffset, region.sizeInBits, null);
		}

		public static uint ComputeFollowingByteOffset(uint byteOffset, uint sizeInBits)
		{
			return (uint)(byteOffset + sizeInBits / 8u + ((sizeInBits % 8u != 0) ? 1 : 0));
		}

		public unsafe static void WriteSingleBit(void* ptr, uint bitOffset, bool value)
		{
			switch (bitOffset)
			{
			case 0u:
			case 1u:
			case 2u:
			case 3u:
			case 4u:
			case 5u:
			case 6u:
			case 7u:
				if (value)
				{
					*(byte*)ptr = (byte)(*(byte*)ptr | (byte)(1 << (int)bitOffset));
				}
				else
				{
					*(byte*)ptr = (byte)(*(byte*)ptr & (byte)(~(1 << (int)bitOffset)));
				}
				return;
			case 8u:
			case 9u:
			case 10u:
			case 11u:
			case 12u:
			case 13u:
			case 14u:
			case 15u:
			case 16u:
			case 17u:
			case 18u:
			case 19u:
			case 20u:
			case 21u:
			case 22u:
			case 23u:
			case 24u:
			case 25u:
			case 26u:
			case 27u:
			case 28u:
			case 29u:
			case 30u:
			case 31u:
				if (value)
				{
					*(int*)ptr |= 1 << (int)bitOffset;
				}
				else
				{
					*(int*)ptr &= ~(1 << (int)bitOffset);
				}
				return;
			}
			uint num = bitOffset / 8u;
			bitOffset %= 8u;
			if (value)
			{
				byte* intPtr = (byte*)ptr + num;
				*intPtr = (byte)(*intPtr | (byte)(1 << (int)bitOffset));
			}
			else
			{
				byte* intPtr2 = (byte*)ptr + num;
				*intPtr2 = (byte)(*intPtr2 & (byte)(~(1 << (int)bitOffset)));
			}
		}

		public unsafe static bool ReadSingleBit(void* ptr, uint bitOffset)
		{
			int num2;
			switch (bitOffset)
			{
			case 0u:
			case 1u:
			case 2u:
			case 3u:
			case 4u:
			case 5u:
			case 6u:
			case 7u:
				num2 = *(byte*)ptr;
				break;
			case 8u:
			case 9u:
			case 10u:
			case 11u:
			case 12u:
			case 13u:
			case 14u:
			case 15u:
			case 16u:
			case 17u:
			case 18u:
			case 19u:
			case 20u:
			case 21u:
			case 22u:
			case 23u:
			case 24u:
			case 25u:
			case 26u:
			case 27u:
			case 28u:
			case 29u:
			case 30u:
			case 31u:
				num2 = *(int*)ptr;
				break;
			default:
			{
				uint num = bitOffset / 8u;
				bitOffset %= 8u;
				num2 = ((byte*)ptr)[num];
				break;
			}
			}
			return (num2 & (1 << (int)bitOffset)) != 0;
		}

		public unsafe static bool MemCmpBitRegion(void* ptr1, void* ptr2, uint bitOffset, uint bitCount, void* mask = null)
		{
			byte* ptr3 = (byte*)ptr1;
			byte* ptr4 = (byte*)ptr2;
			byte* ptr5 = (byte*)mask;
			if (bitOffset >= 8)
			{
				uint num = bitOffset / 8u;
				ptr3 += num;
				ptr4 += num;
				if (ptr5 != null)
				{
					ptr5 += num;
				}
				bitOffset %= 8u;
			}
			if (bitOffset != 0)
			{
				int num2 = 255 << (int)bitOffset;
				if (bitCount + bitOffset < 8)
				{
					num2 &= 255 >> (int)(8 - (bitCount + bitOffset));
				}
				if (ptr5 != null)
				{
					num2 &= *ptr5;
					ptr5++;
				}
				int num3 = *ptr3 & num2;
				int num4 = *ptr4 & num2;
				if (num3 != num4)
				{
					return false;
				}
				ptr3++;
				ptr4++;
				if (bitCount + bitOffset <= 8)
				{
					return true;
				}
				bitCount -= 8 - bitOffset;
			}
			uint num5 = bitCount / 8u;
			if (num5 >= 1)
			{
				if (ptr5 != null)
				{
					for (int i = 0; i < num5; i++)
					{
						byte num6 = ptr3[i];
						byte b = ptr4[i];
						byte b2 = ptr5[i];
						if ((num6 & b2) != (b & b2))
						{
							return false;
						}
					}
				}
				else if (UnsafeUtility.MemCmp(ptr3, ptr4, num5) != 0)
				{
					return false;
				}
			}
			uint num7 = bitCount % 8u;
			if (num7 != 0)
			{
				ptr3 += num5;
				ptr4 += num5;
				int num8 = 255 >> (int)(8 - num7);
				if (ptr5 != null)
				{
					ptr5 += num5;
					num8 &= *ptr5;
				}
				int num9 = *ptr3 & num8;
				int num10 = *ptr4 & num8;
				if (num9 != num10)
				{
					return false;
				}
			}
			return true;
		}

		public unsafe static void MemSet(void* destination, int numBytes, byte value)
		{
			int num = 0;
			while (numBytes >= 8)
			{
				*(ulong*)((byte*)destination + num) = ((ulong)value << 56) | ((ulong)value << 48) | ((ulong)value << 40) | ((ulong)value << 32) | ((ulong)value << 24) | ((ulong)value << 16) | ((ulong)value << 8) | value;
				numBytes -= 8;
				num += 8;
			}
			while (numBytes >= 4)
			{
				*(int*)((byte*)destination + num) = (value << 24) | (value << 16) | (value << 8) | value;
				numBytes -= 4;
				num += 4;
			}
			while (numBytes > 0)
			{
				((byte*)destination)[num] = value;
				numBytes--;
				num++;
			}
		}

		public unsafe static void MemCpyMasked(void* destination, void* source, int numBytes, void* mask)
		{
			int num = 0;
			while (numBytes >= 8)
			{
				*(long*)((byte*)destination + num) &= ~(*(long*)((byte*)mask + num));
				*(long*)((byte*)destination + num) |= *(long*)((byte*)source + num) & *(long*)((byte*)mask + num);
				numBytes -= 8;
				num += 8;
			}
			while (numBytes >= 4)
			{
				*(int*)((byte*)destination + num) &= (int)(~(*(uint*)((byte*)mask + num)));
				*(int*)((byte*)destination + num) |= (int)(*(uint*)((byte*)source + num) & *(uint*)((byte*)mask + num));
				numBytes -= 4;
				num += 4;
			}
			while (numBytes > 0)
			{
				byte* intPtr = (byte*)destination + num;
				*intPtr = (byte)(*intPtr & (byte)(~((byte*)mask)[num]));
				byte* intPtr2 = (byte*)destination + num;
				*intPtr2 = (byte)(*intPtr2 | (byte)(((byte*)source)[num] & ((byte*)mask)[num]));
				numBytes--;
				num++;
			}
		}

		public unsafe static int ReadIntFromMultipleBits(void* ptr, uint bitOffset, uint bitCount)
		{
			if (ptr == null)
			{
				throw new ArgumentNullException("ptr");
			}
			if (bitCount >= 32)
			{
				throw new ArgumentException("Trying to read more than 32 bits as int", "bitCount");
			}
			if (bitOffset > 32)
			{
				int num = (int)bitOffset % 32;
				int num2 = ((int)bitOffset - num) / 32;
				ptr = (byte*)ptr + num2 * 4;
				bitOffset = (uint)num;
			}
			if (bitOffset + bitCount <= 8)
			{
				byte num3 = (byte)(*(byte*)ptr >> (int)bitOffset);
				int num4 = 255 >> (int)(8 - bitCount);
				return num3 & num4;
			}
			if (bitOffset + bitCount <= 16)
			{
				ushort num5 = (ushort)(*(ushort*)ptr >> (int)bitOffset);
				int num6 = 65535 >> (int)(16 - bitCount);
				return num5 & num6;
			}
			if (bitOffset + bitCount <= 32)
			{
				uint num7 = *(uint*)ptr >> (int)bitOffset;
				uint num8 = uint.MaxValue >> (int)(32 - bitCount);
				return (int)(num7 & num8);
			}
			throw new NotImplementedException("Reading int straddling int boundary");
		}

		public unsafe static void WriteIntFromMultipleBits(void* ptr, uint bitOffset, uint bitCount, int value)
		{
			if (ptr == null)
			{
				throw new ArgumentNullException("ptr");
			}
			if (bitCount >= 32)
			{
				throw new ArgumentException("Trying to write more than 32 bits as int", "bitCount");
			}
			if (bitOffset + bitCount <= 8)
			{
				byte b = (byte)value;
				b = (byte)(b << (int)bitOffset);
				int num = ~(255 >> (int)(8 - bitCount) << (int)bitOffset);
				*(byte*)ptr = (byte)((*(byte*)ptr & num) | b);
				return;
			}
			if (bitOffset + bitCount <= 16)
			{
				ushort num2 = (ushort)value;
				num2 = (ushort)(num2 << (int)bitOffset);
				int num3 = ~(65535 >> (int)(16 - bitCount) << (int)bitOffset);
				*(ushort*)ptr = (ushort)((*(ushort*)ptr & num3) | num2);
				return;
			}
			if (bitOffset + bitCount <= 32)
			{
				uint num4 = (uint)value;
				num4 <<= (int)bitOffset;
				uint num5 = ~(uint.MaxValue >> (int)(32 - bitCount) << (int)bitOffset);
				*(int*)ptr = (int)((*(int*)ptr & num5) | num4);
				return;
			}
			throw new NotImplementedException("Writing int straddling int boundary");
		}

		public unsafe static void SetBitsInBuffer(void* buffer, int byteOffset, int bitOffset, int sizeInBits, bool value)
		{
			if (buffer == null)
			{
				throw new ArgumentException("A buffer must be provided to apply the bitmask on", "buffer");
			}
			if (sizeInBits < 0)
			{
				throw new ArgumentException("Negative sizeInBits", "sizeInBits");
			}
			if (bitOffset < 0)
			{
				throw new ArgumentException("Negative bitOffset", "bitOffset");
			}
			if (byteOffset < 0)
			{
				throw new ArgumentException("Negative byteOffset", "byteOffset");
			}
			if (bitOffset >= 8)
			{
				int num = bitOffset / 8;
				byteOffset += num;
				bitOffset %= 8;
			}
			byte* ptr = (byte*)buffer + byteOffset;
			int num2 = sizeInBits;
			if (bitOffset != 0)
			{
				int num3 = 255 << bitOffset;
				if (num2 + bitOffset < 8)
				{
					num3 &= 255 >> 8 - (num2 + bitOffset);
				}
				if (value)
				{
					byte* intPtr = ptr;
					*intPtr = (byte)(*intPtr | (byte)num3);
				}
				else
				{
					byte* intPtr2 = ptr;
					*intPtr2 = (byte)(*intPtr2 & (byte)(~num3));
				}
				ptr++;
				num2 -= 8 - bitOffset;
			}
			while (num2 >= 8)
			{
				*ptr = (byte)(value ? byte.MaxValue : 0);
				ptr++;
				num2 -= 8;
			}
			if (num2 > 0)
			{
				byte b = (byte)(255 >> 8 - num2);
				if (value)
				{
					byte* intPtr3 = ptr;
					*intPtr3 = (byte)(*intPtr3 | b);
				}
				else
				{
					byte* intPtr4 = ptr;
					*intPtr4 = (byte)(*intPtr4 & (byte)(~b));
				}
			}
		}

		public static void Swap<TValue>(ref TValue a, ref TValue b)
		{
			TValue val = a;
			a = b;
			b = val;
		}
	}
}
