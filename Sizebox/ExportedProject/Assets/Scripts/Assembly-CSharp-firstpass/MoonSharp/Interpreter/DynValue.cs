using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using MoonSharp.Interpreter.Interop.Converters;

namespace MoonSharp.Interpreter
{
	public sealed class DynValue
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<DynValue, bool> _003C_003E9__54_0;

			public static Func<DynValue, string> _003C_003E9__77_0;

			public static Func<DynValue, string> _003C_003E9__78_0;

			public static Func<DynValue, string> _003C_003E9__79_0;

			public static Func<DynValue, string> _003C_003E9__79_1;

			internal bool _003CNewTupleNested_003Eb__54_0(DynValue v)
			{
				return v.Type == DataType.Tuple;
			}

			internal string _003CToPrintString_003Eb__77_0(DynValue t)
			{
				return t.ToPrintString();
			}

			internal string _003CToDebugPrintString_003Eb__78_0(DynValue t)
			{
				return t.ToPrintString();
			}

			internal string _003CToString_003Eb__79_0(DynValue t)
			{
				return t.ToString();
			}

			internal string _003CToString_003Eb__79_1(DynValue t)
			{
				return t.ToString();
			}
		}

		private static int s_RefIDCounter;

		private int m_RefID = ++s_RefIDCounter;

		private int m_HashCode = -1;

		private bool m_ReadOnly;

		private double m_Number;

		private object m_Object;

		private DataType m_Type;

		public int ReferenceID
		{
			get
			{
				return m_RefID;
			}
		}

		public DataType Type
		{
			get
			{
				return m_Type;
			}
		}

		public Closure Function
		{
			get
			{
				return m_Object as Closure;
			}
		}

		public double Number
		{
			get
			{
				return m_Number;
			}
		}

		public DynValue[] Tuple
		{
			get
			{
				return m_Object as DynValue[];
			}
		}

		public Coroutine Coroutine
		{
			get
			{
				return m_Object as Coroutine;
			}
		}

		public Table Table
		{
			get
			{
				return m_Object as Table;
			}
		}

		public bool Boolean
		{
			get
			{
				return Number != 0.0;
			}
		}

		public string String
		{
			get
			{
				return m_Object as string;
			}
		}

		public CallbackFunction Callback
		{
			get
			{
				return m_Object as CallbackFunction;
			}
		}

		public TailCallData TailCallData
		{
			get
			{
				return m_Object as TailCallData;
			}
		}

		public YieldRequest YieldRequest
		{
			get
			{
				return m_Object as YieldRequest;
			}
		}

		public UserData UserData
		{
			get
			{
				return m_Object as UserData;
			}
		}

		public bool ReadOnly
		{
			get
			{
				return m_ReadOnly;
			}
		}

		public static DynValue Void { get; private set; }

		public static DynValue Nil { get; private set; }

		public static DynValue True { get; private set; }

		public static DynValue False { get; private set; }

		public static DynValue NewNil()
		{
			return new DynValue();
		}

		public static DynValue NewBoolean(bool v)
		{
			return new DynValue
			{
				m_Number = (v ? 1 : 0),
				m_Type = DataType.Boolean
			};
		}

		public static DynValue NewNumber(double num)
		{
			return new DynValue
			{
				m_Number = num,
				m_Type = DataType.Number,
				m_HashCode = -1
			};
		}

		public static DynValue NewString(string str)
		{
			return new DynValue
			{
				m_Object = str,
				m_Type = DataType.String
			};
		}

		public static DynValue NewString(StringBuilder sb)
		{
			return new DynValue
			{
				m_Object = sb.ToString(),
				m_Type = DataType.String
			};
		}

		public static DynValue NewString(string format, params object[] args)
		{
			return new DynValue
			{
				m_Object = string.Format(format, args),
				m_Type = DataType.String
			};
		}

		public static DynValue NewCoroutine(Coroutine coroutine)
		{
			return new DynValue
			{
				m_Object = coroutine,
				m_Type = DataType.Thread
			};
		}

		public static DynValue NewClosure(Closure function)
		{
			return new DynValue
			{
				m_Object = function,
				m_Type = DataType.Function
			};
		}

		public static DynValue NewCallback(Func<ScriptExecutionContext, CallbackArguments, DynValue> callBack, string name = null)
		{
			return new DynValue
			{
				m_Object = new CallbackFunction(callBack, name),
				m_Type = DataType.ClrFunction
			};
		}

		public static DynValue NewCallback(CallbackFunction function)
		{
			return new DynValue
			{
				m_Object = function,
				m_Type = DataType.ClrFunction
			};
		}

		public static DynValue NewTable(Table table)
		{
			return new DynValue
			{
				m_Object = table,
				m_Type = DataType.Table
			};
		}

		public static DynValue NewPrimeTable()
		{
			return NewTable(new Table(null));
		}

		public static DynValue NewTable(Script script)
		{
			return NewTable(new Table(script));
		}

		public static DynValue NewTable(Script script, params DynValue[] arrayValues)
		{
			return NewTable(new Table(script, arrayValues));
		}

		public static DynValue NewTailCallReq(DynValue tailFn, params DynValue[] args)
		{
			return new DynValue
			{
				m_Object = new TailCallData
				{
					Args = args,
					Function = tailFn
				},
				m_Type = DataType.TailCallRequest
			};
		}

		public static DynValue NewTailCallReq(TailCallData tailCallData)
		{
			return new DynValue
			{
				m_Object = tailCallData,
				m_Type = DataType.TailCallRequest
			};
		}

		public static DynValue NewYieldReq(DynValue[] args)
		{
			return new DynValue
			{
				m_Object = new YieldRequest
				{
					ReturnValues = args
				},
				m_Type = DataType.YieldRequest
			};
		}

		internal static DynValue NewForcedYieldReq()
		{
			return new DynValue
			{
				m_Object = new YieldRequest
				{
					Forced = true
				},
				m_Type = DataType.YieldRequest
			};
		}

		public static DynValue NewTuple(params DynValue[] values)
		{
			if (values.Length == 0)
			{
				return NewNil();
			}
			if (values.Length == 1)
			{
				return values[0];
			}
			return new DynValue
			{
				m_Object = values,
				m_Type = DataType.Tuple
			};
		}

		public static DynValue NewTupleNested(params DynValue[] values)
		{
			if (!values.Any(_003C_003Ec._003C_003E9__54_0 ?? (_003C_003Ec._003C_003E9__54_0 = _003C_003Ec._003C_003E9._003CNewTupleNested_003Eb__54_0)))
			{
				return NewTuple(values);
			}
			if (values.Length == 1)
			{
				return values[0];
			}
			List<DynValue> list = new List<DynValue>();
			foreach (DynValue dynValue in values)
			{
				if (dynValue.Type == DataType.Tuple)
				{
					list.AddRange(dynValue.Tuple);
				}
				else
				{
					list.Add(dynValue);
				}
			}
			return new DynValue
			{
				m_Object = list.ToArray(),
				m_Type = DataType.Tuple
			};
		}

		public static DynValue NewUserData(UserData userData)
		{
			return new DynValue
			{
				m_Object = userData,
				m_Type = DataType.UserData
			};
		}

		public DynValue AsReadOnly()
		{
			if (ReadOnly)
			{
				return this;
			}
			return Clone(true);
		}

		public DynValue Clone()
		{
			return Clone(ReadOnly);
		}

		public DynValue Clone(bool readOnly)
		{
			return new DynValue
			{
				m_Object = m_Object,
				m_Number = m_Number,
				m_HashCode = m_HashCode,
				m_Type = m_Type,
				m_ReadOnly = readOnly
			};
		}

		public DynValue CloneAsWritable()
		{
			return Clone(false);
		}

		static DynValue()
		{
			Nil = new DynValue
			{
				m_Type = DataType.Nil
			}.AsReadOnly();
			Void = new DynValue
			{
				m_Type = DataType.Void
			}.AsReadOnly();
			True = NewBoolean(true).AsReadOnly();
			False = NewBoolean(false).AsReadOnly();
		}

		public string ToPrintString()
		{
			if (m_Object != null && m_Object is RefIdObject)
			{
				RefIdObject refIdObject = (RefIdObject)m_Object;
				string typeString = Type.ToLuaTypeString();
				if (m_Object is UserData)
				{
					UserData userData = (UserData)m_Object;
					string text = userData.Descriptor.AsString(userData.Object);
					if (text != null)
					{
						return text;
					}
				}
				return refIdObject.FormatTypeString(typeString);
			}
			switch (Type)
			{
			case DataType.String:
				return String;
			case DataType.Tuple:
				return string.Join("\t", Tuple.Select(_003C_003Ec._003C_003E9__77_0 ?? (_003C_003Ec._003C_003E9__77_0 = _003C_003Ec._003C_003E9._003CToPrintString_003Eb__77_0)).ToArray());
			case DataType.TailCallRequest:
				return "(TailCallRequest -- INTERNAL!)";
			case DataType.YieldRequest:
				return "(YieldRequest -- INTERNAL!)";
			default:
				return ToString();
			}
		}

		public string ToDebugPrintString()
		{
			if (m_Object != null && m_Object is RefIdObject)
			{
				RefIdObject refIdObject = (RefIdObject)m_Object;
				string typeString = Type.ToLuaTypeString();
				if (m_Object is UserData)
				{
					UserData userData = (UserData)m_Object;
					string text = userData.Descriptor.AsString(userData.Object);
					if (text != null)
					{
						return text;
					}
				}
				return refIdObject.FormatTypeString(typeString);
			}
			switch (Type)
			{
			case DataType.Tuple:
				return string.Join("\t", Tuple.Select(_003C_003Ec._003C_003E9__78_0 ?? (_003C_003Ec._003C_003E9__78_0 = _003C_003Ec._003C_003E9._003CToDebugPrintString_003Eb__78_0)).ToArray());
			case DataType.TailCallRequest:
				return "(TailCallRequest)";
			case DataType.YieldRequest:
				return "(YieldRequest)";
			default:
				return ToString();
			}
		}

		public override string ToString()
		{
			switch (Type)
			{
			case DataType.Void:
				return "void";
			case DataType.Nil:
				return "nil";
			case DataType.Boolean:
				return Boolean.ToString().ToLower();
			case DataType.Number:
				return Number.ToString(CultureInfo.InvariantCulture);
			case DataType.String:
				return "\"" + String + "\"";
			case DataType.Function:
				return string.Format("(Function {0:X8})", Function.EntryPointByteCodeLocation);
			case DataType.ClrFunction:
				return string.Format("(Function CLR)", Function);
			case DataType.Table:
				return "(Table)";
			case DataType.Tuple:
				return string.Join(", ", Tuple.Select(_003C_003Ec._003C_003E9__79_0 ?? (_003C_003Ec._003C_003E9__79_0 = _003C_003Ec._003C_003E9._003CToString_003Eb__79_0)).ToArray());
			case DataType.TailCallRequest:
				return "Tail:(" + string.Join(", ", Tuple.Select(_003C_003Ec._003C_003E9__79_1 ?? (_003C_003Ec._003C_003E9__79_1 = _003C_003Ec._003C_003E9._003CToString_003Eb__79_1)).ToArray()) + ")";
			case DataType.UserData:
				return "(UserData)";
			case DataType.Thread:
				return string.Format("(Coroutine {0:X8})", Coroutine.ReferenceID);
			default:
				return "(???)";
			}
		}

		public override int GetHashCode()
		{
			if (m_HashCode != -1)
			{
				return m_HashCode;
			}
			int num = (int)Type << 27;
			switch (Type)
			{
			case DataType.Nil:
			case DataType.Void:
				m_HashCode = 0;
				break;
			case DataType.Boolean:
				m_HashCode = (Boolean ? 1 : 2);
				break;
			case DataType.Number:
				m_HashCode = num ^ Number.GetHashCode();
				break;
			case DataType.String:
				m_HashCode = num ^ String.GetHashCode();
				break;
			case DataType.Function:
				m_HashCode = num ^ Function.GetHashCode();
				break;
			case DataType.ClrFunction:
				m_HashCode = num ^ Callback.GetHashCode();
				break;
			case DataType.Table:
				m_HashCode = num ^ Table.GetHashCode();
				break;
			case DataType.Tuple:
			case DataType.TailCallRequest:
				m_HashCode = num ^ Tuple.GetHashCode();
				break;
			default:
				m_HashCode = 999;
				break;
			}
			return m_HashCode;
		}

		public override bool Equals(object obj)
		{
			DynValue dynValue = obj as DynValue;
			if (dynValue == null)
			{
				return false;
			}
			if ((dynValue.Type == DataType.Nil && Type == DataType.Void) || (dynValue.Type == DataType.Void && Type == DataType.Nil))
			{
				return true;
			}
			if (dynValue.Type != Type)
			{
				return false;
			}
			switch (Type)
			{
			case DataType.Nil:
			case DataType.Void:
				return true;
			case DataType.Boolean:
				return Boolean == dynValue.Boolean;
			case DataType.Number:
				return Number == dynValue.Number;
			case DataType.String:
				return String == dynValue.String;
			case DataType.Function:
				return Function == dynValue.Function;
			case DataType.ClrFunction:
				return Callback == dynValue.Callback;
			case DataType.Table:
				return Table == dynValue.Table;
			case DataType.Tuple:
			case DataType.TailCallRequest:
				return Tuple == dynValue.Tuple;
			case DataType.Thread:
				return Coroutine == dynValue.Coroutine;
			case DataType.UserData:
			{
				UserData userData = UserData;
				UserData userData2 = dynValue.UserData;
				if (userData == null || userData2 == null)
				{
					return false;
				}
				if (userData.Descriptor != userData2.Descriptor)
				{
					return false;
				}
				if (userData.Object == null && userData2.Object == null)
				{
					return true;
				}
				if (userData.Object != null && userData2.Object != null)
				{
					return userData.Object.Equals(userData2.Object);
				}
				return false;
			}
			default:
				return this == dynValue;
			}
		}

		public string CastToString()
		{
			DynValue dynValue = ToScalar();
			if (dynValue.Type == DataType.Number)
			{
				return dynValue.Number.ToString();
			}
			if (dynValue.Type == DataType.String)
			{
				return dynValue.String;
			}
			return null;
		}

		public double? CastToNumber()
		{
			DynValue dynValue = ToScalar();
			if (dynValue.Type == DataType.Number)
			{
				return dynValue.Number;
			}
			double result;
			if (dynValue.Type == DataType.String && double.TryParse(dynValue.String, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
			{
				return result;
			}
			return null;
		}

		public bool CastToBool()
		{
			DynValue dynValue = ToScalar();
			if (dynValue.Type == DataType.Boolean)
			{
				return dynValue.Boolean;
			}
			if (dynValue.Type != 0)
			{
				return dynValue.Type != DataType.Void;
			}
			return false;
		}

		public IScriptPrivateResource GetAsPrivateResource()
		{
			return m_Object as IScriptPrivateResource;
		}

		public DynValue ToScalar()
		{
			if (Type != DataType.Tuple)
			{
				return this;
			}
			if (Tuple.Length == 0)
			{
				return Void;
			}
			return Tuple[0].ToScalar();
		}

		public void Assign(DynValue value)
		{
			if (ReadOnly)
			{
				throw new ScriptRuntimeException("Assigning on r-value");
			}
			m_Number = value.m_Number;
			m_Object = value.m_Object;
			m_Type = value.Type;
			m_HashCode = -1;
		}

		public DynValue GetLength()
		{
			if (Type == DataType.Table)
			{
				return NewNumber(Table.Length);
			}
			if (Type == DataType.String)
			{
				return NewNumber(String.Length);
			}
			throw new ScriptRuntimeException("Can't get length of type {0}", Type);
		}

		public bool IsNil()
		{
			if (Type != 0)
			{
				return Type == DataType.Void;
			}
			return true;
		}

		public bool IsNotNil()
		{
			if (Type != 0)
			{
				return Type != DataType.Void;
			}
			return false;
		}

		public bool IsVoid()
		{
			return Type == DataType.Void;
		}

		public bool IsNotVoid()
		{
			return Type != DataType.Void;
		}

		public bool IsNilOrNan()
		{
			if (Type != 0 && Type != DataType.Void)
			{
				if (Type == DataType.Number)
				{
					return double.IsNaN(Number);
				}
				return false;
			}
			return true;
		}

		internal void AssignNumber(double num)
		{
			if (ReadOnly)
			{
				throw new InternalErrorException(null, "Writing on r-value");
			}
			if (Type != DataType.Number)
			{
				throw new InternalErrorException("Can't assign number to type {0}", Type);
			}
			m_Number = num;
		}

		public static DynValue FromObject(Script script, object obj)
		{
			return ClrToScriptConversions.ObjectToDynValue(script, obj);
		}

		public object ToObject()
		{
			return ScriptToClrConversions.DynValueToObject(this);
		}

		public object ToObject(Type desiredType)
		{
			return ScriptToClrConversions.DynValueToObjectOfType(this, desiredType, null, false);
		}

		public T ToObject<T>()
		{
			return (T)ToObject(typeof(T));
		}

		public DynValue CheckType(string funcName, DataType desiredType, int argNum = -1, TypeValidationFlags flags = TypeValidationFlags.AutoConvert)
		{
			if (Type == desiredType)
			{
				return this;
			}
			bool flag = (flags & TypeValidationFlags.AllowNil) != 0;
			if (flag && IsNil())
			{
				return this;
			}
			if ((flags & TypeValidationFlags.AutoConvert) != 0)
			{
				switch (desiredType)
				{
				case DataType.Boolean:
					return NewBoolean(CastToBool());
				case DataType.Number:
				{
					double? num = CastToNumber();
					if (num.HasValue)
					{
						return NewNumber(num.Value);
					}
					break;
				}
				}
				if (desiredType == DataType.String)
				{
					string text = CastToString();
					if (text != null)
					{
						return NewString(text);
					}
				}
			}
			if (IsVoid())
			{
				throw ScriptRuntimeException.BadArgumentNoValue(argNum, funcName, desiredType);
			}
			throw ScriptRuntimeException.BadArgument(argNum, funcName, desiredType, Type, flag);
		}

		public T CheckUserDataType<T>(string funcName, int argNum = -1, TypeValidationFlags flags = TypeValidationFlags.AutoConvert)
		{
			DynValue dynValue = CheckType(funcName, DataType.UserData, argNum, flags);
			bool allowNil = (flags & TypeValidationFlags.AllowNil) != 0;
			if (dynValue.IsNil())
			{
				return default(T);
			}
			object @object = dynValue.UserData.Object;
			if (@object != null && @object is T)
			{
				return (T)@object;
			}
			throw ScriptRuntimeException.BadArgumentUserData(argNum, funcName, typeof(T), @object, allowNil);
		}
	}
}
