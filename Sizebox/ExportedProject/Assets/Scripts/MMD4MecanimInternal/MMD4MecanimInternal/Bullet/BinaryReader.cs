using System;
using System.Text;
using UnityEngine;

namespace MMD4MecanimInternal.Bullet
{
	public class BinaryReader
	{
		private enum Header
		{
			HeaderIntValueListLength = 0,
			HeaderFloatValueListLength = 1,
			HeaderByteValueListLength = 2,
			IntValueLengthInHeader = 3,
			FloatValueLengthInHeader = 4,
			ByteValueLengthInHeader = 5,
			StructListLength = 6,
			StructIntValueListLength = 7,
			StructFloatValueListLength = 8,
			StructByteValueListLength = 9,
			IntValueListLength = 10,
			FloatValueListLength = 11,
			ByteValueListLength = 12,
			NameLengthListLength = 13,
			NameLength = 14,
			Max = 15
		}

		private enum ReadMode
		{
			None = 0,
			Header = 1,
			StructList = 2,
			Struct = 3
		}

		private byte[] _fileBytes;

		private int _fourCC;

		private int _structIntValueListPosition;

		private int _structFloatValueListPosition;

		private int _structByteValueListPosition;

		private int _intValueListPosition;

		private int _floatValueListPosition;

		private int _byteValueListPosition;

		private int _nameLengthPosition;

		private int _namePosition;

		private int[] _header;

		private int[] _structList;

		private int[] _intPool;

		private float[] _floatPool;

		private byte[] _bytePool;

		private string[] _nameList;

		private int _bytePoolPosition;

		private ReadMode _readMode;

		private bool _isError;

		private int _currentHeaderIntValueIndex;

		private int _currentHeaderFloatValueIndex;

		private int _currentHeaderByteValueIndex;

		private int _currentStructListIndex;

		private int _currentStructIndex;

		private int _currentStructFourCC;

		private int _currentStructFlags;

		private int _currentStructLength;

		private int _currentStructIntValueLength;

		private int _currentStructFloatValueLength;

		private int _currentStructByteValueLength;

		private int _currentStructIntValueIndex;

		private int _currentStructFloatValueIndex;

		private int _currentStructByteValueIndex;

		private int _currentIntPoolPosition;

		private int _currentIntPoolRemain;

		private int _currentFloatPoolPosition;

		private int _currentFloatPoolRemain;

		private int _currentBytePoolPosition;

		private int _currentBytePoolRemain;

		public int structListLength
		{
			get
			{
				if (_header == null)
				{
					return 0;
				}
				return _header[6];
			}
		}

		public int currentStructFourCC
		{
			get
			{
				return _currentStructFourCC;
			}
		}

		public int currentStructFlags
		{
			get
			{
				return _currentStructFlags;
			}
		}

		public int currentStructLength
		{
			get
			{
				return _currentStructLength;
			}
		}

		public int currentStructIndex
		{
			get
			{
				return _currentStructIndex;
			}
		}

		public static int MakeFourCC(string str)
		{
			return (int)(str[0] | ((uint)str[1] << 8) | ((uint)str[2] << 16) | ((uint)str[3] << 24));
		}

		public int GetFourCC()
		{
			return _fourCC;
		}

		public BinaryReader(byte[] fileBytes)
		{
			_fileBytes = fileBytes;
		}

		public bool Preparse()
		{
			if (_fileBytes == null || _fileBytes.Length == 0)
			{
				Debug.LogError("(BinaryReader) fileBytes is Nothing.");
				_isError = true;
				return false;
			}
			int num = 0;
			_fourCC = _fileBytes[0] | (_fileBytes[1] << 8) | (_fileBytes[2] << 16) | (_fileBytes[3] << 24);
			num += 4;
			_header = new int[15];
			Buffer.BlockCopy(_fileBytes, num, _header, 0, _header.Length * 4);
			num += _header.Length * 4;
			_structList = new int[_header[6] * 9];
			Buffer.BlockCopy(_fileBytes, num, _structList, 0, _structList.Length * 4);
			num += _structList.Length * 4;
			int num2 = _header[0] + _header[7] + _header[10] + _header[13];
			_structIntValueListPosition = _header[0];
			_intValueListPosition = _structIntValueListPosition + _header[7];
			_nameLengthPosition = _intValueListPosition + _header[10];
			_intPool = new int[num2];
			Buffer.BlockCopy(_fileBytes, num, _intPool, 0, num2 * 4);
			num += num2 * 4;
			int num3 = _header[1] + _header[8] + _header[11];
			_structFloatValueListPosition = _header[1];
			_floatValueListPosition = _structFloatValueListPosition + _header[8];
			_floatPool = new float[num3];
			Buffer.BlockCopy(_fileBytes, num, _floatPool, 0, num3 * 4);
			num += num3 * 4;
			int num4 = _header[2] + _header[9] + _header[12] + _header[14];
			if (num + num4 > _fileBytes.Length)
			{
				Debug.LogError("(BinaryReader) Overflow.");
				_isError = true;
				return false;
			}
			_bytePool = _fileBytes;
			_bytePoolPosition = num;
			_structByteValueListPosition = _bytePoolPosition + _header[2];
			_byteValueListPosition = _structByteValueListPosition + _header[9];
			_namePosition = _byteValueListPosition + _header[12];
			return _PostfixPreparse();
		}

		private bool _PostfixPreparse()
		{
			if (_fileBytes == null || _intPool == null || _header == null)
			{
				Debug.LogError("(BinaryReader) null.");
				_isError = true;
				return false;
			}
			if (_nameLengthPosition + _header[13] > _intPool.Length)
			{
				Debug.LogError("(BinaryReader) Overflow.");
				_isError = true;
				return false;
			}
			int num = _header[13];
			_nameList = new string[num];
			int num2 = _nameLengthPosition;
			int num3 = _namePosition;
			for (int i = 0; i < num; i++)
			{
				int num4 = _intPool[num2];
				if (num3 + num4 > _fileBytes.Length)
				{
					Debug.LogError("(BinaryReader) Overflow.");
					_isError = true;
					return false;
				}
				_nameList[i] = Encoding.UTF8.GetString(_fileBytes, num3, num4);
				num2++;
				num3 += num4 + 1;
			}
			return true;
		}

		public void Rewind()
		{
			if (!_isError)
			{
				_readMode = ReadMode.None;
				_currentHeaderIntValueIndex = 0;
				_currentHeaderFloatValueIndex = 0;
				_currentHeaderByteValueIndex = 0;
				_currentStructListIndex = 0;
				_currentStructIndex = 0;
				_currentStructFourCC = 0;
				_currentStructFlags = 0;
				_currentStructLength = 0;
				_currentStructIntValueLength = 0;
				_currentStructFloatValueLength = 0;
				_currentStructByteValueLength = 0;
				_currentStructIntValueIndex = 0;
				_currentStructFloatValueIndex = 0;
				_currentStructByteValueIndex = 0;
				_currentIntPoolPosition = 0;
				_currentIntPoolRemain = 0;
				_currentFloatPoolPosition = 0;
				_currentFloatPoolRemain = 0;
				_currentBytePoolPosition = 0;
				_currentBytePoolRemain = 0;
			}
		}

		public bool BeginHeader()
		{
			if (_isError)
			{
				return false;
			}
			if (_readMode != 0)
			{
				Debug.LogError("(BinaryReader) invalid flow.");
				return false;
			}
			_currentHeaderIntValueIndex = 0;
			_currentHeaderFloatValueIndex = 0;
			_currentHeaderByteValueIndex = 0;
			_currentIntPoolPosition = 0;
			_currentFloatPoolPosition = 0;
			_currentBytePoolPosition = 0;
			_currentIntPoolRemain = _header[3];
			_currentFloatPoolRemain = _header[4];
			_currentBytePoolRemain = _header[5];
			_readMode = ReadMode.Header;
			return true;
		}

		public int ReadHeaderInt()
		{
			if (_isError)
			{
				return 0;
			}
			if (_readMode != ReadMode.Header)
			{
				Debug.LogError("(BinaryReader) invalid flow.");
				_isError = true;
				return 0;
			}
			if (_currentHeaderIntValueIndex >= _header[0])
			{
				return 0;
			}
			int currentHeaderIntValueIndex = _currentHeaderIntValueIndex;
			if (_intPool == null || currentHeaderIntValueIndex >= _intPool.Length)
			{
				Debug.LogError("(BinaryReader) Overflow.");
				_isError = true;
				return 0;
			}
			int result = _intPool[currentHeaderIntValueIndex];
			_currentHeaderIntValueIndex++;
			return result;
		}

		public float ReadHeaderFloat()
		{
			if (_isError)
			{
				return 0f;
			}
			if (_readMode != ReadMode.Header)
			{
				Debug.LogError("(BinaryReader) invalid flow.");
				_isError = true;
				return 0f;
			}
			if (_currentHeaderFloatValueIndex >= _header[1])
			{
				return 0f;
			}
			int currentHeaderFloatValueIndex = _currentHeaderFloatValueIndex;
			if (_floatPool == null || currentHeaderFloatValueIndex >= _floatPool.Length)
			{
				Debug.LogError("(BinaryReader) Overflow.");
				_isError = true;
				return 0f;
			}
			float result = _floatPool[currentHeaderFloatValueIndex];
			_currentHeaderFloatValueIndex++;
			return result;
		}

		public byte ReadHeaderByte()
		{
			if (_isError)
			{
				return 0;
			}
			if (_readMode != ReadMode.Header)
			{
				Debug.LogError("(BinaryReader) invalid flow.");
				_isError = true;
				return 0;
			}
			if (_currentHeaderByteValueIndex >= _header[2])
			{
				return 0;
			}
			int num = _bytePoolPosition + _currentHeaderByteValueIndex;
			if (_bytePool == null || num >= _bytePool.Length)
			{
				Debug.LogError("(BinaryReader) Overflow.");
				_isError = true;
				return 0;
			}
			byte result = _bytePool[num];
			_currentHeaderByteValueIndex++;
			return result;
		}

		public bool EndHeader()
		{
			if (_isError)
			{
				return false;
			}
			if (_readMode != ReadMode.Header)
			{
				Debug.LogError("(BinaryReader) invalid flow.");
				return false;
			}
			_currentHeaderIntValueIndex = 0;
			_currentHeaderFloatValueIndex = 0;
			_currentHeaderByteValueIndex = 0;
			_currentIntPoolPosition = 0;
			_currentFloatPoolPosition = 0;
			_currentBytePoolPosition = 0;
			_currentIntPoolRemain = 0;
			_currentFloatPoolRemain = 0;
			_currentBytePoolRemain = 0;
			_readMode = ReadMode.None;
			return true;
		}

		public bool BeginStructList()
		{
			if (_isError)
			{
				return false;
			}
			if (_readMode != 0)
			{
				Debug.LogError("(BinaryReader) invalid flow.");
				return false;
			}
			if (_structList == null || _currentStructListIndex + 1 > _structList.Length)
			{
				Debug.LogError("(BinaryReader) Overflow.");
				return false;
			}
			_currentStructIndex = 0;
			_currentStructFourCC = _structList[_currentStructListIndex * 9];
			_currentStructFlags = _structList[_currentStructListIndex * 9 + 1];
			_currentStructLength = _structList[_currentStructListIndex * 9 + 2];
			_currentStructIntValueLength = _structList[_currentStructListIndex * 9 + 3];
			_currentStructFloatValueLength = _structList[_currentStructListIndex * 9 + 4];
			_currentStructByteValueLength = _structList[_currentStructListIndex * 9 + 5];
			_currentIntPoolPosition = _structList[_currentStructListIndex * 9 + 6];
			_currentFloatPoolPosition = _structList[_currentStructListIndex * 9 + 7];
			_currentBytePoolPosition = _structList[_currentStructListIndex * 9 + 8];
			_currentStructIntValueIndex = 0;
			_currentStructFloatValueIndex = 0;
			_currentStructByteValueIndex = 0;
			_currentIntPoolRemain = 0;
			_currentFloatPoolRemain = 0;
			_currentBytePoolRemain = 0;
			_readMode = ReadMode.StructList;
			return true;
		}

		public bool BeginStruct()
		{
			if (_isError)
			{
				return false;
			}
			if (_readMode != ReadMode.StructList)
			{
				Debug.LogError("(BinaryReader) invalid flow.");
				return false;
			}
			if (_currentStructIndex >= _currentStructLength)
			{
				Debug.LogError("(BinaryReader) Overflow.");
				return false;
			}
			if (_currentStructIntValueIndex + 3 > _currentStructIntValueLength)
			{
				Debug.LogError("(BinaryReader) Overflow.");
				_isError = true;
				return false;
			}
			if (_intPool == null || _structIntValueListPosition + 3 > _intPool.Length)
			{
				Debug.LogError("(BinaryReader) Overflow.");
				_isError = true;
				return false;
			}
			_currentIntPoolRemain = _intPool[_structIntValueListPosition];
			_currentFloatPoolRemain = _intPool[_structIntValueListPosition + 1];
			_currentBytePoolRemain = _intPool[_structIntValueListPosition + 2];
			_currentStructIntValueIndex = 3;
			_structIntValueListPosition += 3;
			_readMode = ReadMode.Struct;
			return true;
		}

		public int ReadStructInt()
		{
			if (_isError)
			{
				return 0;
			}
			if (_readMode != ReadMode.Struct)
			{
				Debug.LogError("(BinaryReader) invalid flow.");
				return 0;
			}
			if (_currentStructIntValueIndex >= _currentStructIntValueLength)
			{
				return 0;
			}
			if (_intPool == null || _structIntValueListPosition >= _intPool.Length)
			{
				Debug.LogError("(BinaryReader) Overflow.");
				_isError = true;
				return 0;
			}
			int result = _intPool[_structIntValueListPosition];
			_currentStructIntValueIndex++;
			_structIntValueListPosition++;
			return result;
		}

		public float ReadStructFloat()
		{
			if (_isError)
			{
				return 0f;
			}
			if (_readMode != ReadMode.Struct)
			{
				Debug.LogError("(BinaryReader) invalid flow.");
				return 0f;
			}
			if (_currentStructFloatValueIndex >= _currentStructFloatValueLength)
			{
				return 0f;
			}
			if (_floatPool == null || _structFloatValueListPosition >= _floatPool.Length)
			{
				Debug.LogError("(BinaryReader) Overflow.");
				_isError = true;
				return 0f;
			}
			float result = _floatPool[_structFloatValueListPosition];
			_currentStructFloatValueIndex++;
			_structFloatValueListPosition++;
			return result;
		}

		public Vector2 ReadStructVector2()
		{
			Vector2 zero = Vector2.zero;
			zero.x = ReadStructFloat();
			zero.y = ReadStructFloat();
			return zero;
		}

		public Vector3 ReadStructVector3()
		{
			Vector3 zero = Vector3.zero;
			zero.x = ReadStructFloat();
			zero.y = ReadStructFloat();
			zero.z = ReadStructFloat();
			return zero;
		}

		public byte ReadStructByte()
		{
			if (_isError)
			{
				return 0;
			}
			if (_readMode != ReadMode.Struct)
			{
				Debug.LogError("(BinaryReader) invalid flow.");
				return 0;
			}
			if (_currentStructByteValueIndex >= _currentStructByteValueLength)
			{
				return 0;
			}
			if (_bytePool == null || _structByteValueListPosition >= _bytePool.Length)
			{
				Debug.LogError("(BinaryReader) Overflow.");
				_isError = true;
				return 0;
			}
			byte result = _bytePool[_structByteValueListPosition];
			_currentStructByteValueIndex++;
			_structByteValueListPosition++;
			return result;
		}

		public bool EndStruct()
		{
			if (_readMode != ReadMode.Struct)
			{
				Debug.LogError("(BinaryReader) invalid flow.");
				return false;
			}
			if (_currentStructIntValueIndex < _currentStructIntValueLength)
			{
				_structIntValueListPosition += _currentStructIntValueLength - _currentStructIntValueIndex;
			}
			if (_currentStructFloatValueIndex < _currentStructFloatValueLength)
			{
				_structFloatValueListPosition += _currentStructFloatValueLength - _currentStructFloatValueIndex;
			}
			if (_currentStructByteValueIndex < _currentStructByteValueLength)
			{
				_structByteValueListPosition += _currentStructByteValueLength - _currentStructByteValueIndex;
			}
			_currentIntPoolPosition += _currentIntPoolRemain;
			_currentFloatPoolPosition += _currentFloatPoolRemain;
			_currentBytePoolPosition += _currentBytePoolRemain;
			_currentStructIndex++;
			_currentStructIntValueIndex = 0;
			_currentStructFloatValueIndex = 0;
			_currentStructByteValueIndex = 0;
			_currentIntPoolRemain = 0;
			_currentFloatPoolRemain = 0;
			_currentBytePoolRemain = 0;
			_readMode = ReadMode.StructList;
			return true;
		}

		public bool EndStructList()
		{
			if (_readMode != ReadMode.StructList)
			{
				Debug.LogError("(BinaryReader) invalid flow.");
				return false;
			}
			if (_currentStructIndex < _currentStructLength)
			{
				_structIntValueListPosition += _currentStructIntValueLength * (_currentStructLength - _currentStructIndex);
				_structFloatValueListPosition += _currentStructFloatValueLength * (_currentStructLength - _currentStructIndex);
				_structByteValueListPosition += _currentStructByteValueLength * (_currentStructLength - _currentStructIndex);
			}
			_currentStructListIndex++;
			_currentStructIndex = 0;
			_currentStructFourCC = 0;
			_currentStructFlags = 0;
			_currentStructLength = 0;
			_currentStructIntValueLength = 0;
			_currentStructFloatValueLength = 0;
			_currentStructByteValueLength = 0;
			_currentIntPoolPosition = 0;
			_currentFloatPoolPosition = 0;
			_currentBytePoolPosition = 0;
			_currentStructIntValueIndex = 0;
			_currentStructFloatValueIndex = 0;
			_currentStructByteValueIndex = 0;
			_currentIntPoolRemain = 0;
			_currentFloatPoolRemain = 0;
			_currentBytePoolRemain = 0;
			_readMode = ReadMode.None;
			return true;
		}

		public int ReadInt()
		{
			if (_intPool == null || _currentIntPoolRemain == 0)
			{
				return 0;
			}
			int result = _intPool[_intValueListPosition + _currentIntPoolPosition];
			_currentIntPoolPosition++;
			_currentIntPoolRemain--;
			return result;
		}

		public float ReadFloat()
		{
			if (_floatPool == null || _currentFloatPoolRemain == 0)
			{
				return 0f;
			}
			float result = _floatPool[_floatValueListPosition + _currentFloatPoolPosition];
			_currentFloatPoolPosition++;
			_currentFloatPoolRemain--;
			return result;
		}

		public Color ReadColor()
		{
			float r = ReadFloat();
			float g = ReadFloat();
			float b = ReadFloat();
			float a = ReadFloat();
			return new Color(r, g, b, a);
		}

		public Color ReadColorRGB()
		{
			float r = ReadFloat();
			float g = ReadFloat();
			float b = ReadFloat();
			return new Color(r, g, b, 1f);
		}

		public Vector3 ReadVector3()
		{
			float x = ReadFloat();
			float y = ReadFloat();
			float z = ReadFloat();
			return new Vector3(x, y, z);
		}

		public Quaternion ReadQuaternion()
		{
			float x = ReadFloat();
			float y = ReadFloat();
			float z = ReadFloat();
			float w = ReadFloat();
			return new Quaternion(x, y, z, w);
		}

		public byte ReadByte()
		{
			if (_fileBytes == null || _currentBytePoolRemain == 0)
			{
				return 0;
			}
			byte result = _fileBytes[_byteValueListPosition + _currentBytePoolPosition];
			_currentBytePoolPosition++;
			_currentBytePoolRemain--;
			return result;
		}

		public string GetName(int index)
		{
			if (_nameList != null && (uint)index < (uint)_nameList.Length)
			{
				return _nameList[index];
			}
			return "";
		}
	}
}
