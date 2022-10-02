using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public static class MMD4MecanimCommon
{
	public struct Version
	{
		public int major;

		public int minor;

		public int revision;

		public bool LaterThan(int major)
		{
			return this.major >= major;
		}

		public bool LaterThan(int major, int minor)
		{
			if (this.major < major)
			{
				return false;
			}
			if (this.major > major)
			{
				return true;
			}
			return this.minor >= minor;
		}

		public bool LaterThan(int major, int minor, int revision)
		{
			if (this.major < major)
			{
				return false;
			}
			if (this.major > major)
			{
				return true;
			}
			if (this.minor < minor)
			{
				return false;
			}
			if (this.minor > minor)
			{
				return true;
			}
			return this.revision >= revision;
		}
	}

	public struct GCHValue<T>
	{
		private GCHandle _gch_value;

		private IntPtr _valuePtr;

		public GCHValue(ref T value)
		{
			_gch_value = GCHandle.Alloc(value, GCHandleType.Pinned);
			_valuePtr = _gch_value.AddrOfPinnedObject();
		}

		public static implicit operator IntPtr(GCHValue<T> v)
		{
			return v._valuePtr;
		}

		public void Free()
		{
			if (_valuePtr != IntPtr.Zero)
			{
				_valuePtr = IntPtr.Zero;
				_gch_value.Free();
			}
		}
	}

	public struct GCHValues<T>
	{
		private GCHandle _gch_values;

		private IntPtr _valuesPtr;

		public int length;

		public GCHValues(T[] values)
		{
			if (values != null)
			{
				_gch_values = GCHandle.Alloc(values, GCHandleType.Pinned);
				_valuesPtr = _gch_values.AddrOfPinnedObject();
				length = values.Length;
			}
			else
			{
				_gch_values = default(GCHandle);
				_valuesPtr = IntPtr.Zero;
				length = 0;
			}
		}

		public static implicit operator IntPtr(GCHValues<T> v)
		{
			return v._valuesPtr;
		}

		public void Free()
		{
			if (_valuesPtr != IntPtr.Zero)
			{
				_valuesPtr = IntPtr.Zero;
				_gch_values.Free();
			}
		}
	}

	public class PropertyWriter
	{
		private List<int> _iValues = new List<int>();

		private List<float> _fValues = new List<float>();

		private int[] _lock_iValues;

		private float[] _lock_fValues;

		private GCHandle _gch_iValues;

		private GCHandle _gch_fValues;

		public IntPtr iValuesPtr;

		public IntPtr fValuesPtr;

		public int iValueLength
		{
			get
			{
				if (_lock_iValues == null)
				{
					return 0;
				}
				return _lock_iValues.Length;
			}
		}

		public int fValueLength
		{
			get
			{
				if (_lock_fValues == null)
				{
					return 0;
				}
				return _lock_fValues.Length;
			}
		}

		public void Clear()
		{
			_iValues.Clear();
			_fValues.Clear();
		}

		public void Write(string propertyName, bool value)
		{
			_iValues.Add(MurmurHash32(propertyName));
			_iValues.Add(value ? 1 : 0);
		}

		public void Write(string propertyName, int value)
		{
			_iValues.Add(MurmurHash32(propertyName));
			_iValues.Add(value);
		}

		public void Write(string propertyName, float value)
		{
			_iValues.Add(MurmurHash32(propertyName));
			_fValues.Add(value);
		}

		public void Write(string propertyName, Vector3 value)
		{
			_iValues.Add(MurmurHash32(propertyName));
			_fValues.Add(value.x);
			_fValues.Add(value.y);
			_fValues.Add(value.z);
		}

		public void Write(string propertyName, Quaternion value)
		{
			_iValues.Add(MurmurHash32(propertyName));
			_fValues.Add(value.x);
			_fValues.Add(value.y);
			_fValues.Add(value.z);
			_fValues.Add(value.w);
		}

		public void Lock()
		{
			_lock_iValues = _iValues.ToArray();
			_lock_fValues = _fValues.ToArray();
			_gch_iValues = GCHandle.Alloc(_lock_iValues, GCHandleType.Pinned);
			_gch_fValues = GCHandle.Alloc(_lock_fValues, GCHandleType.Pinned);
			iValuesPtr = _gch_iValues.AddrOfPinnedObject();
			fValuesPtr = _gch_fValues.AddrOfPinnedObject();
		}

		public void Unlock()
		{
			_gch_fValues.Free();
			_gch_iValues.Free();
			fValuesPtr = IntPtr.Zero;
			iValuesPtr = IntPtr.Zero;
		}
	}

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
			_fourCC = MMD4MecanimCommon.ReadInt(_fileBytes, 0);
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

	public enum TextureFileSign
	{
		None = 0,
		Bmp = 1,
		BmpWithAlpha = 2,
		Png = 3,
		PngWithAlpha = 4,
		Jpeg = 5,
		Targa = 6,
		TargaWithAlpha = 7
	}

	public class CloneMeshWork
	{
		public Mesh mesh;

		public string name;

		public Vector3[] vertices;

		public Vector3[] normals;

		public Vector4[] tangents;

		public Color32[] colors32;

		public BoneWeight[] boneWeights;

		public Bounds bounds;

		public HideFlags hideFlags;

		public Vector2[] uv1;

		public Vector2[] uv2;

		public Vector2[] uv;

		public Matrix4x4[] bindposes;

		public int[] triangles;
	}

	public struct FastVector3
	{
		public Vector3 value;

		private int _isZero;

		public bool isZero
		{
			get
			{
				if (_isZero == -1)
				{
					_isZero = ((value == Vector3.zero) ? 1 : 0);
				}
				return _isZero != 0;
			}
		}

		public static FastVector3 zero
		{
			get
			{
				return new FastVector3(Vector3.zero, true);
			}
		}

		public FastVector3(Vector3 value, bool isZero)
		{
			this.value = value;
			_isZero = (isZero ? 1 : 0);
		}

		public FastVector3(Vector3 value)
		{
			this.value = value;
			_isZero = -1;
		}

		public FastVector3(ref Vector3 value)
		{
			this.value = value;
			_isZero = -1;
		}

		public override bool Equals(object obj)
		{
			if (obj is FastVector3)
			{
				return Equals((FastVector3)obj);
			}
			return false;
		}

		public bool Equals(FastVector3 rhs)
		{
			return value == rhs.value;
		}

		public override int GetHashCode()
		{
			return value.GetHashCode();
		}

		public static implicit operator Vector3(FastVector3 v)
		{
			return v.value;
		}

		public static implicit operator FastVector3(Vector3 v)
		{
			return new FastVector3(ref v);
		}

		public static bool operator ==(FastVector3 lhs, Vector3 rhs)
		{
			return lhs.value == rhs;
		}

		public static bool operator !=(FastVector3 lhs, Vector3 rhs)
		{
			return lhs.value != rhs;
		}

		public static FastVector3 operator +(FastVector3 lhs, Vector3 rhs)
		{
			if (lhs.isZero)
			{
				return new FastVector3(rhs);
			}
			return new FastVector3(lhs.value + rhs);
		}

		public static FastVector3 operator +(Vector3 lhs, FastVector3 rhs)
		{
			if (rhs.isZero)
			{
				return new FastVector3(lhs);
			}
			return new FastVector3(lhs + rhs.value);
		}

		public static FastVector3 operator +(FastVector3 lhs, FastVector3 rhs)
		{
			if (lhs.isZero && rhs.isZero)
			{
				return lhs;
			}
			if (lhs.isZero)
			{
				return rhs;
			}
			if (rhs.isZero)
			{
				return lhs;
			}
			return new FastVector3(lhs.value + rhs.value);
		}
	}

	public struct FastQuaternion
	{
		public Quaternion value;

		private int _isIdentity;

		public bool isIdentity
		{
			get
			{
				if (_isIdentity == -1)
				{
					_isIdentity = ((value == Quaternion.identity) ? 1 : 0);
				}
				return _isIdentity != 0;
			}
		}

		public static FastQuaternion identity
		{
			get
			{
				return new FastQuaternion(Quaternion.identity, true);
			}
		}

		public FastQuaternion(Quaternion value, bool isIdentity)
		{
			this.value = value;
			_isIdentity = (isIdentity ? 1 : 0);
		}

		public FastQuaternion(Quaternion value)
		{
			this.value = value;
			_isIdentity = -1;
		}

		public FastQuaternion(ref Quaternion value)
		{
			this.value = value;
			_isIdentity = -1;
		}

		public override bool Equals(object obj)
		{
			if (obj is FastQuaternion)
			{
				return Equals((FastQuaternion)obj);
			}
			return false;
		}

		public bool Equals(FastQuaternion rhs)
		{
			return value == rhs.value;
		}

		public override int GetHashCode()
		{
			return value.GetHashCode();
		}

		public static implicit operator Quaternion(FastQuaternion q)
		{
			return q.value;
		}

		public static implicit operator FastQuaternion(Quaternion q)
		{
			return new FastQuaternion(q);
		}

		public static bool operator ==(FastQuaternion lhs, Quaternion rhs)
		{
			return lhs.value == rhs;
		}

		public static bool operator !=(FastQuaternion lhs, Quaternion rhs)
		{
			return lhs.value != rhs;
		}

		public static FastQuaternion operator *(FastQuaternion lhs, Quaternion rhs)
		{
			if (lhs.isIdentity)
			{
				return new FastQuaternion(rhs);
			}
			return new FastQuaternion(lhs.value * rhs);
		}

		public static FastQuaternion operator *(Quaternion lhs, FastQuaternion rhs)
		{
			if (rhs.isIdentity)
			{
				return new FastQuaternion(lhs);
			}
			return new FastQuaternion(lhs * rhs.value);
		}

		public static FastQuaternion operator *(FastQuaternion lhs, FastQuaternion rhs)
		{
			if (lhs.isIdentity && rhs.isIdentity)
			{
				return lhs;
			}
			if (lhs.isIdentity)
			{
				return rhs;
			}
			if (rhs.isIdentity)
			{
				return lhs;
			}
			return new FastQuaternion(lhs.value * rhs.value, false);
		}
	}

	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Predicate<GameObject> _003C_003E9__86_0;

		internal bool _003CFindRootObjects_003Eb__86_0(GameObject item)
		{
			return item.transform.parent == null;
		}
	}

	public static readonly Color MMDLit_centerAmbient = new Color(0.5f, 0.5f, 0.5f);

	public static readonly Color MMDLit_centerAmbientInv = new Color(2f, 2f, 2f);

	public static readonly Color MMDLit_globalLighting = new Color(0.6f, 0.6f, 0.6f);

	public static readonly float MMDLit_edgeScale = 0.001f;

	public static readonly string ExtensionAnimBytesLower = ".anim.bytes";

	public static readonly string ExtensionAnimBytesUpper = ".ANIM.BYTES";

	public static Color MMDLit_GetTempAmbientL(Color ambient)
	{
		Color color = MMDLit_centerAmbient - ambient;
		color.r = Mathf.Max(color.r, 0f);
		color.g = Mathf.Max(color.g, 0f);
		color.b = Mathf.Max(color.b, 0f);
		color.a = 0f;
		return color * MMDLit_centerAmbientInv.r;
	}

	public static Color MMDLit_GetTempAmbient(Color globalAmbient, Color ambient)
	{
		Color result = globalAmbient * (Color.white - MMDLit_GetTempAmbientL(ambient));
		result.a = 0f;
		return result;
	}

	public static Color MMDLit_GetTempDiffuse(Color globalAmbient, Color ambient, Color diffuse)
	{
		Color result = ambient + diffuse * MMDLit_globalLighting.r;
		result.r = Mathf.Min(result.r, 1f);
		result.g = Mathf.Min(result.g, 1f);
		result.b = Mathf.Min(result.b, 1f);
		result -= MMDLit_GetTempAmbient(globalAmbient, ambient);
		result.r = Mathf.Max(result.r, 0f);
		result.g = Mathf.Max(result.g, 0f);
		result.b = Mathf.Max(result.b, 0f);
		result.a = 0f;
		return result;
	}

	public static void WeakSetMaterialFloat(Material m, string name, float v)
	{
		if (m.GetFloat(name) != v)
		{
			m.SetFloat(name, v);
		}
	}

	public static void WeakSetMaterialVector(Material m, string name, Vector4 v)
	{
		if (m.GetVector(name) != v)
		{
			m.SetVector(name, v);
		}
	}

	public static void WeakSetMaterialColor(Material m, string name, Color v)
	{
		if (m.GetColor(name) != v)
		{
			m.SetColor(name, v);
		}
	}

	public static Transform WeakAddChildTransform(Transform parentTransform, string name)
	{
		GameObject gameObject = WeakAddChildGameObject((parentTransform != null) ? parentTransform.gameObject : null, name);
		if (!(gameObject != null))
		{
			return null;
		}
		return gameObject.transform;
	}

	public static GameObject WeakAddChildGameObject(GameObject parentGameObject, string name)
	{
		if (parentGameObject != null && !string.IsNullOrEmpty(name))
		{
			foreach (Transform item in parentGameObject.transform)
			{
				if (item.name == name)
				{
					return item.gameObject;
				}
			}
		}
		GameObject gameObject = new GameObject((name != null) ? name : "");
		if (parentGameObject != null)
		{
			gameObject.transform.parent = parentGameObject.transform;
		}
		gameObject.layer = parentGameObject.layer;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
		return gameObject;
	}

	public static Version GetUnityVersion()
	{
		Version result = default(Version);
		string unityVersion = Application.unityVersion;
		int num = unityVersion.IndexOf(".");
		int num2 = ((num >= 0) ? unityVersion.IndexOf(".", num + 1) : (-1));
		if (num2 >= 0)
		{
			result.major = ToInt(unityVersion, 0, num);
			result.minor = ToInt(unityVersion, num + 1, num2 - (num + 1));
			result.revision = ToInt(unityVersion, num2 + 1, unityVersion.Length - (num2 + 1));
		}
		else if (num >= 0)
		{
			result.major = ToInt(unityVersion, 0, num);
			result.minor = ToInt(unityVersion, num + 1, unityVersion.Length - (num + 1));
		}
		else
		{
			result.major = ToInt(unityVersion);
		}
		return result;
	}

	public static GameObject[] GetChildrenRecursivery(GameObject gameObject)
	{
		List<GameObject> list = new List<GameObject>();
		if (gameObject != null)
		{
			_GetChildrenRecursivery(list, gameObject);
		}
		return list.ToArray();
	}

	private static void _GetChildrenRecursivery(List<GameObject> children, GameObject gameObject)
	{
		foreach (Transform item in gameObject.transform)
		{
			children.Add(item.gameObject);
			_GetChildrenRecursivery(children, item.gameObject);
		}
	}

	public static void IgnoreCollisionRecursivery(GameObject gameObject, Collider targetCollider)
	{
		if (!(gameObject != null) || !(targetCollider != null))
		{
			return;
		}
		if (gameObject != targetCollider.gameObject)
		{
			Collider component = gameObject.GetComponent<Collider>();
			if (component != null && component.enabled && targetCollider.enabled)
			{
				Physics.IgnoreCollision(component, targetCollider);
			}
		}
		foreach (Transform item in gameObject.transform)
		{
			IgnoreCollisionRecursivery(item.gameObject, targetCollider);
		}
	}

	public static void IgnoreCollisionRecursivery(Collider collider, Collider targetCollider)
	{
		if (collider != null && targetCollider != null)
		{
			IgnoreCollisionRecursivery(collider.gameObject, targetCollider);
		}
	}

	public static bool ContainsNameInParents(GameObject gameObject, string name)
	{
		if (gameObject == null)
		{
			return false;
		}
		while (true)
		{
			if (gameObject.name.Contains(name))
			{
				return true;
			}
			if (gameObject.transform.parent == null)
			{
				break;
			}
			gameObject = gameObject.transform.parent.gameObject;
		}
		return false;
	}

	public static int MurmurHash32(string name)
	{
		return MurmurHash32(name, 2880293630u);
	}

	public static int MurmurHash32(string name, uint seed)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(name);
		return MurmurHash32(bytes, 0, bytes.Length, seed);
	}

	public static int MurmurHash32(byte[] bytes, int pos, int len)
	{
		return MurmurHash32(bytes, pos, len, 2880293630u);
	}

	private static uint mmh3_fmix32(uint h)
	{
		h ^= h >> 16;
		h *= 2246822507u;
		h ^= h >> 13;
		h *= 3266489909u;
		h ^= h >> 16;
		return h;
	}

	public static int MurmurHash32(byte[] bytes, int pos, int len, uint seed)
	{
		int num = len / 4;
		uint num2 = seed;
		uint num3 = 3432918353u;
		uint num4 = 461845907u;
		int i = 0;
		for (int num5 = num * 4; i < num5; i += 4)
		{
			uint num6 = (uint)(bytes[pos + i] | (bytes[pos + i + 1] << 8) | (bytes[pos + i + 2] << 16) | (bytes[pos + i + 3] << 24));
			num6 *= num3;
			num6 = (num6 << 15) | (num6 >> 17);
			num6 *= num4;
			num2 ^= num6;
			num2 = (num2 << 13) | (num2 >> 19);
			num2 = num2 * 5 + 3864292196u;
		}
		if (((uint)len & 3u) != 0)
		{
			uint num7 = 0u;
			if ((len & 3) >= 3)
			{
				num7 ^= (uint)(bytes[pos + num * 4 + 2] << 16);
			}
			if ((len & 3) >= 2)
			{
				num7 ^= (uint)(bytes[pos + num * 4 + 1] << 8);
			}
			if ((len & 3) >= 1)
			{
				num7 ^= bytes[pos + num * 4];
			}
			num7 *= num3;
			num7 = (num7 << 15) | (num7 >> 17);
			num7 *= num4;
			num2 ^= num7;
		}
		num2 ^= (uint)len;
		return (int)mmh3_fmix32(num2);
	}

	public static GCHValue<T> MakeGCHValue<T>(ref T value)
	{
		return new GCHValue<T>(ref value);
	}

	public static GCHValues<T> MakeGCHValues<T>(T[] values)
	{
		return new GCHValues<T>(values);
	}

	public static int ReadInt(byte[] bytes, int index)
	{
		if (bytes != null && index * 4 + 3 < bytes.Length)
		{
			return bytes[index * 4] | (bytes[index * 4 + 1] << 8) | (bytes[index * 4 + 2] << 16) | (bytes[index * 4 + 3] << 24);
		}
		return 0;
	}

	private static uint _Swap(uint v)
	{
		return (v >> 24) | ((v >> 8) & 0xFF00u) | ((v << 8) & 0xFF0000u) | (v << 24);
	}

	private static uint _MakeFourCC(char a, char b, char c, char d)
	{
		return (uint)((byte)a | ((byte)b << 8) | ((byte)c << 16) | ((byte)d << 24));
	}

	public static TextureFileSign GetTextureFileSign(string path)
	{
		try
		{
			using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				byte[] array = new byte[(int)fileStream.Length];
				fileStream.Read(array, 0, (int)fileStream.Length);
				if ((int)fileStream.Length >= 8)
				{
					bool flag = true;
					byte[] array2 = new byte[8] { 137, 80, 78, 71, 13, 10, 26, 10 };
					for (int i = 0; i < 8; i++)
					{
						if (array[i] != array2[i])
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						fileStream.Seek(0L, SeekOrigin.Begin);
						using (System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fileStream))
						{
							binaryReader.ReadInt32();
							binaryReader.ReadInt32();
							while (true)
							{
								try
								{
									uint num = _Swap(binaryReader.ReadUInt32());
									uint num2 = binaryReader.ReadUInt32();
									if (num2 == _MakeFourCC('I', 'H', 'D', 'R'))
									{
										if (num < 13)
										{
											return TextureFileSign.Png;
										}
										binaryReader.ReadUInt32();
										binaryReader.ReadUInt32();
										binaryReader.ReadByte();
										byte b = binaryReader.ReadByte();
										if (b == 4 || b == 6)
										{
											return TextureFileSign.PngWithAlpha;
										}
										num -= 10;
									}
									else
									{
										if (num2 == _MakeFourCC('t', 'R', 'N', 'S'))
										{
											return TextureFileSign.PngWithAlpha;
										}
										if (num2 == _MakeFourCC('I', 'E', 'N', 'D'))
										{
											return TextureFileSign.Png;
										}
										if (num2 == _MakeFourCC('I', 'D', 'A', 'T'))
										{
											return TextureFileSign.Png;
										}
									}
									for (uint num3 = 0u; num3 < num / 4u; num3++)
									{
										binaryReader.ReadUInt32();
									}
									for (uint num4 = 0u; num4 < num % 4u; num4++)
									{
										binaryReader.ReadByte();
									}
									binaryReader.ReadUInt32();
								}
								catch (Exception)
								{
									return TextureFileSign.PngWithAlpha;
								}
							}
						}
					}
				}
				if ((int)fileStream.Length >= 18 && array[0] == 66 && array[1] == 77)
				{
					uint num5 = (uint)(array[14] | (array[15] << 8) | (array[16] << 16) | (array[17] << 24));
					switch (num5)
					{
					case 12u:
						return TextureFileSign.Bmp;
					case 40u:
					case 52u:
					case 56u:
					case 60u:
					case 96u:
					case 108u:
					case 112u:
					case 120u:
					case 124u:
						if (num5 >= 56 && (array[66] | (array[67] << 8) | (array[68] << 16) | (array[69] << 9)) != 0)
						{
							return TextureFileSign.BmpWithAlpha;
						}
						switch (array[30] | (array[31] << 8) | (array[32] << 16) | (array[33] << 9))
						{
						case 0:
						case 1:
						case 2:
						case 3:
						case 4:
							return TextureFileSign.Bmp;
						case 5:
							return TextureFileSign.BmpWithAlpha;
						}
						break;
					}
				}
				if ((int)fileStream.Length >= 2 && array[0] == byte.MaxValue && array[1] == 216)
				{
					return TextureFileSign.Jpeg;
				}
				if ((int)fileStream.Length >= 18)
				{
					byte b2 = array[1];
					byte b3 = array[17];
					byte b4 = (byte)((b3 & 0x30) >> 4);
					if ((b2 == 0 || b2 == 1) && (b4 == 0 || b4 == 2))
					{
						byte b5 = array[2];
						byte b6 = array[16];
						byte b7 = (byte)(b3 & 0xFu);
						if ((b5 == 1 || b5 == 9) && (b6 == 1 || b6 == 2 || b6 == 4 || b6 == 8))
						{
							return (b7 > 0) ? TextureFileSign.TargaWithAlpha : TextureFileSign.Targa;
						}
						if ((b5 == 3 || b5 == 11) && (b6 == 1 || b6 == 2 || b6 == 4 || b6 == 8))
						{
							return (b7 > 0) ? TextureFileSign.TargaWithAlpha : TextureFileSign.Targa;
						}
						if ((b5 == 2 || b5 == 10) && (b6 == 16 || b6 == 24 || b6 == 32))
						{
							switch (b6)
							{
							case 24:
								return TextureFileSign.Targa;
							case 32:
								return TextureFileSign.TargaWithAlpha;
							default:
								return (b7 > 0) ? TextureFileSign.TargaWithAlpha : TextureFileSign.Targa;
							}
						}
					}
				}
			}
			return TextureFileSign.None;
		}
		catch (Exception)
		{
			return TextureFileSign.None;
		}
	}

	public static bool IsID(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return false;
		}
		if ((uint)(str[0] - 48) > 9u)
		{
			return false;
		}
		for (int i = 1; i != str.Length; i++)
		{
			if (str[i] == '.')
			{
				return true;
			}
			if ((uint)(str[0] - 48) > 9u)
			{
				return false;
			}
		}
		return false;
	}

	public static int ToInt(string str)
	{
		if (str == null)
		{
			return 0;
		}
		return ToInt(str, 0, str.Length);
	}

	public static int ToInt(string str, int pos, int len)
	{
		if (str == null)
		{
			return 0;
		}
		len += pos;
		if (len < str.Length)
		{
			len = str.Length;
		}
		bool flag = false;
		if (pos < len && str[pos] == '-')
		{
			flag = true;
			pos++;
		}
		int num = 0;
		if (pos < len)
		{
			uint num2 = (uint)(str[pos] - 48);
			if (num2 > 9)
			{
				return 0;
			}
			num = (int)num2;
			pos++;
		}
		while (pos < len)
		{
			uint num3 = (uint)(str[pos] - 48);
			if (num3 > 9)
			{
				break;
			}
			num = num * 10 + (int)num3;
			pos++;
		}
		if (!flag)
		{
			return num;
		}
		return -num;
	}

	public static CloneMeshWork CloneMesh(Mesh sharedMesh)
	{
		if (sharedMesh == null)
		{
			return null;
		}
		CloneMeshWork cloneMeshWork = new CloneMeshWork();
		Mesh mesh = (cloneMeshWork.mesh = new Mesh());
		cloneMeshWork.name = sharedMesh.name;
		cloneMeshWork.vertices = sharedMesh.vertices;
		cloneMeshWork.normals = sharedMesh.normals;
		cloneMeshWork.tangents = sharedMesh.tangents;
		cloneMeshWork.colors32 = sharedMesh.colors32;
		cloneMeshWork.boneWeights = sharedMesh.boneWeights;
		cloneMeshWork.bounds = sharedMesh.bounds;
		cloneMeshWork.hideFlags = sharedMesh.hideFlags;
		cloneMeshWork.uv = sharedMesh.uv;
		cloneMeshWork.uv2 = sharedMesh.uv2;
		cloneMeshWork.bindposes = sharedMesh.bindposes;
		cloneMeshWork.triangles = sharedMesh.triangles;
		mesh.name = cloneMeshWork.name;
		mesh.vertices = cloneMeshWork.vertices;
		mesh.normals = cloneMeshWork.normals;
		mesh.tangents = cloneMeshWork.tangents;
		mesh.colors32 = cloneMeshWork.colors32;
		mesh.boneWeights = cloneMeshWork.boneWeights;
		mesh.bounds = cloneMeshWork.bounds;
		mesh.hideFlags = cloneMeshWork.hideFlags;
		mesh.uv = cloneMeshWork.uv;
		mesh.uv2 = cloneMeshWork.uv2;
		mesh.bindposes = cloneMeshWork.bindposes;
		mesh.triangles = cloneMeshWork.triangles;
		if (sharedMesh.subMeshCount > 0)
		{
			mesh.subMeshCount = sharedMesh.subMeshCount;
			for (int i = 0; i < sharedMesh.subMeshCount; i++)
			{
				mesh.SetTriangles(sharedMesh.GetIndices(i), i);
			}
		}
		return cloneMeshWork;
	}

	public static Material CloneMaterial(Material sharedMaterial)
	{
		if (sharedMaterial == null)
		{
			return sharedMaterial;
		}
		return new Material(sharedMaterial);
	}

	public static bool Approx(ref float src, float dest, float step)
	{
		if (src > dest)
		{
			if ((src -= step) <= dest)
			{
				src = dest;
				return true;
			}
			return false;
		}
		if (src < dest)
		{
			if ((src += step) >= dest)
			{
				src = dest;
				return true;
			}
			return false;
		}
		return true;
	}

	public static bool FindAnything<Type>(List<Type> elements, Type element) where Type : class
	{
		if (elements != null)
		{
			for (int i = 0; i < elements.Count; i++)
			{
				if (element == elements[i])
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool IsAlphabet(char ch)
	{
		if ((uint)(ch - 65) > 25u && (uint)(ch - 97) > 25u && (uint)(ch - 65313) > 25u)
		{
			return (uint)(ch - 65345) <= 25u;
		}
		return true;
	}

	public static char ToHalfLower(char ch)
	{
		switch (ch)
		{
		case 'A':
		case 'B':
		case 'C':
		case 'D':
		case 'E':
		case 'F':
		case 'G':
		case 'H':
		case 'I':
		case 'J':
		case 'K':
		case 'L':
		case 'M':
		case 'N':
		case 'O':
		case 'P':
		case 'Q':
		case 'R':
		case 'S':
		case 'T':
		case 'U':
		case 'V':
		case 'W':
		case 'X':
		case 'Y':
		case 'Z':
			return (char)(ch - 65 + 97);
		case 'Ａ':
		case 'Ｂ':
		case 'Ｃ':
		case 'Ｄ':
		case 'Ｅ':
		case 'Ｆ':
		case 'Ｇ':
		case 'Ｈ':
		case 'Ｉ':
		case 'Ｊ':
		case 'Ｋ':
		case 'Ｌ':
		case 'Ｍ':
		case 'Ｎ':
		case 'Ｏ':
		case 'Ｐ':
		case 'Ｑ':
		case 'Ｒ':
		case 'Ｓ':
		case 'Ｔ':
		case 'Ｕ':
		case 'Ｖ':
		case 'Ｗ':
		case 'Ｘ':
		case 'Ｙ':
		case 'Ｚ':
			return (char)(ch - 65313 + 97);
		case 'ａ':
		case 'ｂ':
		case 'ｃ':
		case 'ｄ':
		case 'ｅ':
		case 'ｆ':
		case 'ｇ':
		case 'ｈ':
		case 'ｉ':
		case 'ｊ':
		case 'ｋ':
		case 'ｌ':
		case 'ｍ':
		case 'ｎ':
		case 'ｏ':
		case 'ｐ':
		case 'ｑ':
		case 'ｒ':
		case 'ｓ':
		case 'ｔ':
		case 'ｕ':
		case 'ｖ':
		case 'ｗ':
		case 'ｘ':
		case 'ｙ':
		case 'ｚ':
			return (char)(ch - 65345 + 97);
		default:
			return ch;
		}
	}

	public static float Reciplocal(float x)
	{
		if (x == 0f)
		{
			return 0f;
		}
		return 1f / x;
	}

	public static Vector3 Reciplocal(Vector3 scale)
	{
		return new Vector3(Reciplocal(scale.x), Reciplocal(scale.y), Reciplocal(scale.z));
	}

	public static Vector3 Reciplocal(ref Vector3 scale)
	{
		return new Vector3(Reciplocal(scale.x), Reciplocal(scale.y), Reciplocal(scale.z));
	}

	public static void ConvertMatrixBulletPhysics(ref Matrix4x4 matrix)
	{
		matrix.m03 = 0f - matrix.m03;
		matrix.m10 = 0f - matrix.m10;
		matrix.m20 = 0f - matrix.m20;
		matrix.m01 = 0f - matrix.m01;
		matrix.m02 = 0f - matrix.m02;
	}

	public static Vector3 ComputeMatrixScale(ref Matrix4x4 matrix)
	{
		return new Vector3(new Vector3(matrix.m00, matrix.m10, matrix.m20).magnitude, new Vector3(matrix.m01, matrix.m11, matrix.m21).magnitude, new Vector3(matrix.m02, matrix.m12, matrix.m22).magnitude);
	}

	public static Vector3 ComputeMatrixReciplocalScale(ref Matrix4x4 matrix)
	{
		return new Vector3(Reciplocal(new Vector3(matrix.m00, matrix.m10, matrix.m20).magnitude), Reciplocal(new Vector3(matrix.m01, matrix.m11, matrix.m21).magnitude), Reciplocal(new Vector3(matrix.m02, matrix.m12, matrix.m22).magnitude));
	}

	public static void SetMatrixBasis(ref Matrix4x4 matrix, ref Vector3 right, ref Vector3 up, ref Vector3 forward)
	{
		matrix.m00 = right.x;
		matrix.m10 = right.y;
		matrix.m20 = right.z;
		matrix.m01 = up.x;
		matrix.m11 = up.y;
		matrix.m21 = up.z;
		matrix.m02 = forward.x;
		matrix.m12 = forward.y;
		matrix.m22 = forward.z;
	}

	public static void NormalizeMatrixBasis(ref Matrix4x4 matrix)
	{
		Vector3 right = new Vector3(matrix.m00, matrix.m10, matrix.m20);
		Vector3 up = new Vector3(matrix.m01, matrix.m11, matrix.m21);
		Vector3 forward = new Vector3(matrix.m02, matrix.m12, matrix.m22);
		right *= Reciplocal(right.magnitude);
		up *= Reciplocal(up.magnitude);
		forward *= Reciplocal(forward.magnitude);
		SetMatrixBasis(ref matrix, ref right, ref up, ref forward);
	}

	public static void NormalizeMatrixBasis(ref Matrix4x4 matrix, ref Vector3 rScale)
	{
		Vector3 right = new Vector3(matrix.m00, matrix.m10, matrix.m20) * rScale.x;
		Vector3 up = new Vector3(matrix.m01, matrix.m11, matrix.m21) * rScale.y;
		Vector3 forward = new Vector3(matrix.m02, matrix.m12, matrix.m22) * rScale.z;
		SetMatrixBasis(ref matrix, ref right, ref up, ref forward);
	}

	public static bool IsNoEffects(ref float value, MMD4MecanimData.MorphMaterialOperation operation)
	{
		switch (operation)
		{
		case MMD4MecanimData.MorphMaterialOperation.Adding:
			return value <= Mathf.Epsilon;
		case MMD4MecanimData.MorphMaterialOperation.Multiply:
			return Mathf.Abs(value - 1f) <= Mathf.Epsilon;
		default:
			return true;
		}
	}

	public static bool IsNoEffects(ref Color color, MMD4MecanimData.MorphMaterialOperation operation)
	{
		switch (operation)
		{
		case MMD4MecanimData.MorphMaterialOperation.Adding:
			if (color.r <= Mathf.Epsilon && color.g <= Mathf.Epsilon && color.b <= Mathf.Epsilon)
			{
				return color.a <= Mathf.Epsilon;
			}
			return false;
		case MMD4MecanimData.MorphMaterialOperation.Multiply:
			if (Mathf.Abs(color.r - 1f) <= Mathf.Epsilon && Mathf.Abs(color.g - 1f) <= Mathf.Epsilon && Mathf.Abs(color.b - 1f) <= Mathf.Epsilon)
			{
				return Mathf.Abs(color.a - 1f) <= Mathf.Epsilon;
			}
			return false;
		default:
			return true;
		}
	}

	public static bool IsNoEffectsRGB(ref Color color, MMD4MecanimData.MorphMaterialOperation operation)
	{
		switch (operation)
		{
		case MMD4MecanimData.MorphMaterialOperation.Adding:
			if (color.r <= Mathf.Epsilon && color.g <= Mathf.Epsilon)
			{
				return color.b <= Mathf.Epsilon;
			}
			return false;
		case MMD4MecanimData.MorphMaterialOperation.Multiply:
			if (Mathf.Abs(color.r - 1f) <= Mathf.Epsilon && Mathf.Abs(color.g - 1f) <= Mathf.Epsilon)
			{
				return Mathf.Abs(color.b - 1f) <= Mathf.Epsilon;
			}
			return false;
		default:
			return true;
		}
	}

	public static float NormalizeAsDegree(float v)
	{
		if (v < -180f)
		{
			do
			{
				v += 360f;
			}
			while (v < -180f);
		}
		else if (v > 180f)
		{
			do
			{
				v -= 360f;
			}
			while (v > 180f);
		}
		return v;
	}

	public static Vector3 NormalizeAsDegree(Vector3 degrees)
	{
		return new Vector3(NormalizeAsDegree(degrees.x), NormalizeAsDegree(degrees.y), NormalizeAsDegree(degrees.z));
	}

	public static bool FuzzyZero(float v)
	{
		return Math.Abs(v) <= Mathf.Epsilon;
	}

	public static bool FuzzyZero(Vector3 v)
	{
		if (Mathf.Abs(v.x) <= Mathf.Epsilon && Mathf.Abs(v.y) <= Mathf.Epsilon)
		{
			return Mathf.Abs(v.z) <= Mathf.Epsilon;
		}
		return false;
	}

	public static Quaternion Inverse(Quaternion q)
	{
		return new Quaternion(0f - q.x, 0f - q.y, 0f - q.z, q.w);
	}

	public static bool FuzzyIdentity(Quaternion q)
	{
		if (Mathf.Abs(q.x) <= Mathf.Epsilon && Mathf.Abs(q.y) <= Mathf.Epsilon && Mathf.Abs(q.z) <= Mathf.Epsilon)
		{
			return Mathf.Abs(q.w - 1f) <= Mathf.Epsilon;
		}
		return false;
	}

	public static Quaternion FuzzyMul(Quaternion lhs, Quaternion rhs)
	{
		if (FuzzyIdentity(lhs))
		{
			return rhs;
		}
		if (FuzzyIdentity(rhs))
		{
			return lhs;
		}
		return lhs * rhs;
	}

	public static Quaternion FastMul(Quaternion lhs, Quaternion rhs)
	{
		if (lhs == Quaternion.identity)
		{
			return rhs;
		}
		if (rhs == Quaternion.identity)
		{
			return lhs;
		}
		return lhs * rhs;
	}

	public static void FuzzyAdd(ref float lhs, ref float rhs)
	{
		if (rhs > Mathf.Epsilon)
		{
			lhs += rhs;
		}
	}

	public static void FuzzyAdd(ref Color lhs, ref Color rhs)
	{
		if (rhs.r > Mathf.Epsilon)
		{
			lhs.r += rhs.r;
		}
		if (rhs.g > Mathf.Epsilon)
		{
			lhs.g += rhs.g;
		}
		if (rhs.b > Mathf.Epsilon)
		{
			lhs.b += rhs.b;
		}
		if (rhs.a > Mathf.Epsilon)
		{
			lhs.a += rhs.a;
		}
	}

	public static void FuzzyMul(ref float lhs, ref float rhs)
	{
		if (Mathf.Abs(rhs - 1f) > Mathf.Epsilon)
		{
			lhs *= rhs;
		}
	}

	public static void FuzzyMul(ref Color lhs, ref Color rhs)
	{
		if (Mathf.Abs(rhs.r - 1f) > Mathf.Epsilon)
		{
			lhs.r *= rhs.r;
		}
		if (Mathf.Abs(rhs.g - 1f) > Mathf.Epsilon)
		{
			lhs.g *= rhs.g;
		}
		if (Mathf.Abs(rhs.b - 1f) > Mathf.Epsilon)
		{
			lhs.b *= rhs.b;
		}
		if (Mathf.Abs(rhs.a - 1f) > Mathf.Epsilon)
		{
			lhs.a *= rhs.a;
		}
	}

	public static void FuzzyAdd(ref float lhs, ref float rhs, float weight)
	{
		if (rhs > Mathf.Epsilon)
		{
			lhs += rhs * weight;
		}
	}

	public static void FuzzyAdd(ref Color lhs, ref Color rhs, float weight)
	{
		if (rhs.r > Mathf.Epsilon)
		{
			lhs.r += rhs.r * weight;
		}
		if (rhs.g > Mathf.Epsilon)
		{
			lhs.g += rhs.g * weight;
		}
		if (rhs.b > Mathf.Epsilon)
		{
			lhs.b += rhs.b * weight;
		}
		if (rhs.a > Mathf.Epsilon)
		{
			lhs.a += rhs.a * weight;
		}
	}

	public static void FuzzyMul(ref float lhs, ref float rhs, float weight)
	{
		if (Mathf.Abs(rhs - 1f) > Mathf.Epsilon)
		{
			lhs *= rhs * weight + (1f - weight);
		}
	}

	public static void FuzzyMul(ref Color lhs, ref Color rhs, float weight)
	{
		if (Mathf.Abs(rhs.r - 1f) > Mathf.Epsilon)
		{
			lhs.r *= rhs.r * weight + (1f - weight);
		}
		if (Mathf.Abs(rhs.g - 1f) > Mathf.Epsilon)
		{
			lhs.g *= rhs.g * weight + (1f - weight);
		}
		if (Mathf.Abs(rhs.b - 1f) > Mathf.Epsilon)
		{
			lhs.b *= rhs.b * weight + (1f - weight);
		}
		if (Mathf.Abs(rhs.a - 1f) > Mathf.Epsilon)
		{
			lhs.a *= rhs.a * weight + (1f - weight);
		}
	}

	public static void OperationMaterial(ref MMD4MecanimData.MorphMaterialData currentMaterialData, ref MMD4MecanimData.MorphMaterialData operationMaterialData, float weight)
	{
		if (Mathf.Abs(weight - 1f) <= Mathf.Epsilon)
		{
			switch (operationMaterialData.operation)
			{
			case MMD4MecanimData.MorphMaterialOperation.Adding:
				FuzzyAdd(ref currentMaterialData.diffuse, ref operationMaterialData.diffuse);
				FuzzyAdd(ref currentMaterialData.specular, ref operationMaterialData.specular);
				FuzzyAdd(ref currentMaterialData.shininess, ref operationMaterialData.shininess);
				FuzzyAdd(ref currentMaterialData.ambient, ref operationMaterialData.ambient);
				FuzzyAdd(ref currentMaterialData.edgeColor, ref operationMaterialData.edgeColor);
				FuzzyAdd(ref currentMaterialData.edgeSize, ref operationMaterialData.edgeSize);
				break;
			case MMD4MecanimData.MorphMaterialOperation.Multiply:
				FuzzyMul(ref currentMaterialData.diffuse, ref operationMaterialData.diffuse);
				FuzzyMul(ref currentMaterialData.specular, ref operationMaterialData.specular);
				FuzzyMul(ref currentMaterialData.shininess, ref operationMaterialData.shininess);
				FuzzyMul(ref currentMaterialData.ambient, ref operationMaterialData.ambient);
				FuzzyMul(ref currentMaterialData.edgeColor, ref operationMaterialData.edgeColor);
				FuzzyMul(ref currentMaterialData.edgeSize, ref operationMaterialData.edgeSize);
				break;
			}
		}
		else
		{
			switch (operationMaterialData.operation)
			{
			case MMD4MecanimData.MorphMaterialOperation.Adding:
				FuzzyAdd(ref currentMaterialData.diffuse, ref operationMaterialData.diffuse, weight);
				FuzzyAdd(ref currentMaterialData.specular, ref operationMaterialData.specular, weight);
				FuzzyAdd(ref currentMaterialData.shininess, ref operationMaterialData.shininess, weight);
				FuzzyAdd(ref currentMaterialData.ambient, ref operationMaterialData.ambient, weight);
				FuzzyAdd(ref currentMaterialData.edgeColor, ref operationMaterialData.edgeColor, weight);
				FuzzyAdd(ref currentMaterialData.edgeSize, ref operationMaterialData.edgeSize, weight);
				break;
			case MMD4MecanimData.MorphMaterialOperation.Multiply:
				FuzzyMul(ref currentMaterialData.diffuse, ref operationMaterialData.diffuse, weight);
				FuzzyMul(ref currentMaterialData.specular, ref operationMaterialData.specular, weight);
				FuzzyMul(ref currentMaterialData.shininess, ref operationMaterialData.shininess, weight);
				FuzzyMul(ref currentMaterialData.ambient, ref operationMaterialData.ambient, weight);
				FuzzyMul(ref currentMaterialData.edgeColor, ref operationMaterialData.edgeColor, weight);
				FuzzyMul(ref currentMaterialData.edgeSize, ref operationMaterialData.edgeSize, weight);
				break;
			}
		}
	}

	public static void BackupMaterial(ref MMD4MecanimData.MorphMaterialData materialData, Material material)
	{
		if (material != null && material.shader != null)
		{
			materialData.materialID = ToInt(material.name);
			materialData.diffuse = material.GetColor("_Color");
			if (material.shader.name.StartsWith("MMD4Mecanim"))
			{
				materialData.specular = material.GetColor("_Specular");
				materialData.shininess = material.GetFloat("_Shininess");
				materialData.ambient = material.GetColor("_Ambient");
				materialData.edgeColor = material.GetColor("_EdgeColor");
				materialData.edgeScale = material.GetFloat("_EdgeScale");
				materialData.edgeSize = material.GetFloat("_EdgeSize");
				materialData.alPower = material.GetFloat("_ALPower");
			}
			materialData.specular *= 1f / MMDLit_globalLighting.r;
			if (materialData.edgeScale > 0f)
			{
				materialData.edgeSize *= 1f / materialData.edgeScale;
			}
		}
	}

	public static void FeedbackMaterial(ref MMD4MecanimData.MorphMaterialData materialData, Material material, MMD4MecanimModel.MorphAutoLuminous morphAutoLuminous)
	{
		if (material != null && material.shader != null && morphAutoLuminous != null)
		{
			material.SetColor("_Color", materialData.diffuse);
			if (material.shader.name.StartsWith("MMD4Mecanim"))
			{
				material.SetColor("_Specular", materialData.specular * MMDLit_globalLighting.r);
				material.SetFloat("_Shininess", materialData.shininess);
				material.SetColor("_Ambient", materialData.ambient);
				material.SetColor("_EdgeColor", materialData.edgeColor);
				material.SetFloat("_EdgeSize", materialData.edgeSize * materialData.edgeScale);
			}
			if (materialData.shininess > 100f)
			{
				Color value = ComputeAutoLuminousEmissiveColor(materialData.diffuse, materialData.ambient, materialData.shininess, materialData.alPower, morphAutoLuminous);
				material.SetColor("_Emissive", value);
			}
			else
			{
				material.SetColor("_Emissive", new Color(0f, 0f, 0f, 0f));
			}
		}
	}

	public static Color ComputeAutoLuminousEmissiveColor(Color diffuse, Color ambient, float shininess, float alPower)
	{
		if (alPower > 0f)
		{
			return (diffuse + ambient * 0.5f) * 0.5f * (shininess - 100f) * alPower * (1f / 7f);
		}
		return new Color(0f, 0f, 0f, 0f);
	}

	public static Color ComputeAutoLuminousEmissiveColor(Color diffuse, Color ambient, float shininess, float alPower, MMD4MecanimModel.MorphAutoLuminous morphAutoLuminous)
	{
		Color color = ComputeAutoLuminousEmissiveColor(diffuse, ambient, shininess, alPower);
		if (morphAutoLuminous != null)
		{
			return color * (1f + morphAutoLuminous.lightUp * 3f) * (1f - morphAutoLuminous.lightOff);
		}
		return color;
	}

	private static void _GetMeshRenderers(ArrayList meshRenderers, Transform parentTransform)
	{
		if ((bool)parentTransform.GetComponent<Animator>())
		{
			return;
		}
		MeshRenderer component = parentTransform.GetComponent<MeshRenderer>();
		if (component != null)
		{
			meshRenderers.Add(component);
		}
		foreach (Transform item in parentTransform)
		{
			_GetMeshRenderers(meshRenderers, item);
		}
	}

	public static MeshRenderer[] GetMeshRenderers(GameObject parentGameObject)
	{
		if (parentGameObject != null)
		{
			ArrayList arrayList = new ArrayList();
			foreach (Transform item in parentGameObject.transform)
			{
				if (item.name == "U_Char" || item.name.StartsWith("U_Char_"))
				{
					_GetMeshRenderers(arrayList, item);
				}
			}
			if (arrayList.Count == 0)
			{
				MeshRenderer component = parentGameObject.GetComponent<MeshRenderer>();
				if (component != null)
				{
					arrayList.Add(component);
				}
			}
			if (arrayList.Count > 0)
			{
				MeshRenderer[] array = new MeshRenderer[arrayList.Count];
				for (int i = 0; i < arrayList.Count; i++)
				{
					array[i] = (MeshRenderer)arrayList[i];
				}
				return array;
			}
		}
		return null;
	}

	private static void _GetSkinnedMeshRenderers(ArrayList skinnedMeshRenderers, Transform parentTransform)
	{
		if ((bool)parentTransform.GetComponent<Animator>())
		{
			return;
		}
		SkinnedMeshRenderer component = parentTransform.GetComponent<SkinnedMeshRenderer>();
		if (component != null)
		{
			skinnedMeshRenderers.Add(component);
		}
		foreach (Transform item in parentTransform)
		{
			_GetSkinnedMeshRenderers(skinnedMeshRenderers, item);
		}
	}

	public static SkinnedMeshRenderer[] GetSkinnedMeshRenderers(GameObject parentGameObject)
	{
		if (parentGameObject != null)
		{
			ArrayList arrayList = new ArrayList();
			foreach (Transform item in parentGameObject.transform)
			{
				if (item.name == "U_Char" || item.name.StartsWith("U_Char_"))
				{
					_GetSkinnedMeshRenderers(arrayList, item);
				}
			}
			if (arrayList.Count > 0)
			{
				SkinnedMeshRenderer[] array = new SkinnedMeshRenderer[arrayList.Count];
				for (int i = 0; i < arrayList.Count; i++)
				{
					array[i] = (SkinnedMeshRenderer)arrayList[i];
				}
				return array;
			}
		}
		return null;
	}

	private static MeshRenderer _GetMeshRenderer(Transform parentTransform)
	{
		if ((bool)parentTransform.GetComponent<Animator>())
		{
			return null;
		}
		MeshRenderer component = parentTransform.GetComponent<MeshRenderer>();
		if (component != null)
		{
			return component;
		}
		foreach (Transform item in parentTransform)
		{
			component = _GetMeshRenderer(item);
			if (component != null)
			{
				return component;
			}
		}
		return null;
	}

	public static MeshRenderer GetMeshRenderer(GameObject parentGameObject)
	{
		if (parentGameObject != null)
		{
			foreach (Transform item in parentGameObject.transform)
			{
				if (item.name == "U_Char" || item.name.StartsWith("U_Char_"))
				{
					MeshRenderer meshRenderer = _GetMeshRenderer(item);
					if (meshRenderer != null)
					{
						return meshRenderer;
					}
				}
			}
			MeshRenderer component = parentGameObject.GetComponent<MeshRenderer>();
			if (component != null)
			{
				return component;
			}
		}
		return null;
	}

	private static SkinnedMeshRenderer _GetSkinnedMeshRenderer(Transform parentTransform)
	{
		if ((bool)parentTransform.GetComponent<Animator>())
		{
			return null;
		}
		SkinnedMeshRenderer component = parentTransform.GetComponent<SkinnedMeshRenderer>();
		if (component != null)
		{
			return component;
		}
		foreach (Transform item in parentTransform)
		{
			component = _GetSkinnedMeshRenderer(item);
			if (component != null)
			{
				return component;
			}
		}
		return null;
	}

	public static SkinnedMeshRenderer GetSkinnedMeshRenderer(GameObject parentGameObject)
	{
		if (parentGameObject != null)
		{
			foreach (Transform item in parentGameObject.transform)
			{
				if (item.name == "U_Char" || item.name.StartsWith("U_Char_"))
				{
					SkinnedMeshRenderer skinnedMeshRenderer = _GetSkinnedMeshRenderer(item);
					if (skinnedMeshRenderer != null)
					{
						return skinnedMeshRenderer;
					}
				}
			}
		}
		return null;
	}

	public static GameObject[] FindRootObjects()
	{
		return Array.FindAll(UnityEngine.Object.FindObjectsOfType<GameObject>(), _003C_003Ec._003C_003E9__86_0 ?? (_003C_003Ec._003C_003E9__86_0 = _003C_003Ec._003C_003E9._003CFindRootObjects_003Eb__86_0));
	}

	public static bool IsExtensionAnimBytes(string name)
	{
		if (name != null)
		{
			int length = name.Length;
			int length2 = ExtensionAnimBytesLower.Length;
			if (length >= length2)
			{
				for (int i = 0; i < length2; i++)
				{
					if (name[length - length2 + i] != ExtensionAnimBytesLower[i] && name[length - length2 + i] != ExtensionAnimBytesUpper[i])
					{
						return false;
					}
				}
				return true;
			}
		}
		return false;
	}

	public static bool IsDebugShader(Material material)
	{
		if (material != null && material.shader != null && material.shader.name != null && material.shader.name.StartsWith("MMD4Mecanim") && material.shader.name.Contains("Test"))
		{
			return true;
		}
		return false;
	}

	public static bool IsDeferredShader(Material material)
	{
		if (material != null && material.shader != null && material.shader.name != null && material.shader.name.StartsWith("MMD4Mecanim") && material.shader.name.Contains("Deferred"))
		{
			return true;
		}
		return false;
	}

	public static int SizeOf<T>()
	{
		return Marshal.SizeOf(typeof(T));
	}

	public static Type WeakAddComponent<Type>(GameObject go) where Type : Behaviour
	{
		if (go != null)
		{
			Type component = go.GetComponent<Type>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				return component;
			}
			return go.AddComponent<Type>();
		}
		return null;
	}
}
