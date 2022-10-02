using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using MoonSharp.Interpreter.Compatibility;
using MoonSharp.Interpreter.Interop.BasicDescriptors;
using MoonSharp.Interpreter.Interop.Converters;

namespace MoonSharp.Interpreter.Interop
{
	public class OverloadedMethodMemberDescriptor : IOptimizableDescriptor, IMemberDescriptor, IWireableDescriptor
	{
		private class OverloadableMemberDescriptorComparer : IComparer<IOverloadableMemberDescriptor>
		{
			public int Compare(IOverloadableMemberDescriptor x, IOverloadableMemberDescriptor y)
			{
				return x.SortDiscriminant.CompareTo(y.SortDiscriminant);
			}
		}

		private class OverloadCacheItem
		{
			public bool HasObject;

			public IOverloadableMemberDescriptor Method;

			public List<DataType> ArgsDataType;

			public List<Type> ArgsUserDataType;

			public int HitIndexAtLastHit;
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass33_0
		{
			public OverloadedMethodMemberDescriptor _003C_003E4__this;

			public Script script;

			public object obj;

			internal DynValue _003CGetCallback_003Eb__0(ScriptExecutionContext context, CallbackArguments args)
			{
				return _003C_003E4__this.PerformOverloadedCall(script, obj, context, args);
			}
		}

		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<IOverloadableMemberDescriptor, bool> _003C_003E9__37_0;

			internal bool _003Cget_IsStatic_003Eb__37_0(IOverloadableMemberDescriptor o)
			{
				return o.IsStatic;
			}
		}

		private const int CACHE_SIZE = 5;

		private List<IOverloadableMemberDescriptor> m_Overloads = new List<IOverloadableMemberDescriptor>();

		private List<IOverloadableMemberDescriptor> m_ExtOverloads = new List<IOverloadableMemberDescriptor>();

		private bool m_Unsorted = true;

		private OverloadCacheItem[] m_Cache = new OverloadCacheItem[5];

		private int m_CacheHits;

		private int m_ExtensionMethodVersion;

		public bool IgnoreExtensionMethods { get; set; }

		public string Name { get; private set; }

		public Type DeclaringType { get; private set; }

		public int OverloadCount
		{
			get
			{
				return m_Overloads.Count;
			}
		}

		public bool IsStatic
		{
			get
			{
				return m_Overloads.Any(_003C_003Ec._003C_003E9__37_0 ?? (_003C_003Ec._003C_003E9__37_0 = _003C_003Ec._003C_003E9._003Cget_IsStatic_003Eb__37_0));
			}
		}

		public MemberDescriptorAccess MemberAccess
		{
			get
			{
				return MemberDescriptorAccess.CanRead | MemberDescriptorAccess.CanExecute;
			}
		}

		public OverloadedMethodMemberDescriptor(string name, Type declaringType)
		{
			Name = name;
			DeclaringType = declaringType;
		}

		public OverloadedMethodMemberDescriptor(string name, Type declaringType, IOverloadableMemberDescriptor descriptor)
			: this(name, declaringType)
		{
			m_Overloads.Add(descriptor);
		}

		public OverloadedMethodMemberDescriptor(string name, Type declaringType, IEnumerable<IOverloadableMemberDescriptor> descriptors)
			: this(name, declaringType)
		{
			m_Overloads.AddRange(descriptors);
		}

		internal void SetExtensionMethodsSnapshot(int version, List<IOverloadableMemberDescriptor> extMethods)
		{
			m_ExtOverloads = extMethods;
			m_ExtensionMethodVersion = version;
		}

		public void AddOverload(IOverloadableMemberDescriptor overload)
		{
			m_Overloads.Add(overload);
			m_Unsorted = true;
		}

		private DynValue PerformOverloadedCall(Script script, object obj, ScriptExecutionContext context, CallbackArguments args)
		{
			bool flag = IgnoreExtensionMethods || obj == null || m_ExtensionMethodVersion == UserData.GetExtensionMethodsChangeVersion();
			if (m_Overloads.Count == 1 && m_ExtOverloads.Count == 0 && flag)
			{
				return m_Overloads[0].Execute(script, obj, context, args);
			}
			if (m_Unsorted)
			{
				m_Overloads.Sort(new OverloadableMemberDescriptorComparer());
				m_Unsorted = false;
			}
			if (flag)
			{
				for (int i = 0; i < m_Cache.Length; i++)
				{
					if (m_Cache[i] != null && CheckMatch(obj != null, args, m_Cache[i]))
					{
						return m_Cache[i].Method.Execute(script, obj, context, args);
					}
				}
			}
			int num = 0;
			IOverloadableMemberDescriptor overloadableMemberDescriptor = null;
			for (int j = 0; j < m_Overloads.Count; j++)
			{
				if (obj != null || m_Overloads[j].IsStatic)
				{
					int num2 = CalcScoreForOverload(context, args, m_Overloads[j], false);
					if (num2 > num)
					{
						num = num2;
						overloadableMemberDescriptor = m_Overloads[j];
					}
				}
			}
			if (!IgnoreExtensionMethods && obj != null)
			{
				if (!flag)
				{
					m_ExtensionMethodVersion = UserData.GetExtensionMethodsChangeVersion();
					m_ExtOverloads = UserData.GetExtensionMethodsByNameAndType(Name, DeclaringType);
				}
				for (int k = 0; k < m_ExtOverloads.Count; k++)
				{
					int num3 = CalcScoreForOverload(context, args, m_ExtOverloads[k], true);
					if (num3 > num)
					{
						num = num3;
						overloadableMemberDescriptor = m_ExtOverloads[k];
					}
				}
			}
			if (overloadableMemberDescriptor != null)
			{
				Cache(obj != null, args, overloadableMemberDescriptor);
				return overloadableMemberDescriptor.Execute(script, obj, context, args);
			}
			throw new ScriptRuntimeException("function call doesn't match any overload");
		}

		private void Cache(bool hasObject, CallbackArguments args, IOverloadableMemberDescriptor bestOverload)
		{
			int num = int.MaxValue;
			OverloadCacheItem overloadCacheItem = null;
			for (int i = 0; i < m_Cache.Length; i++)
			{
				if (m_Cache[i] == null)
				{
					overloadCacheItem = new OverloadCacheItem
					{
						ArgsDataType = new List<DataType>(),
						ArgsUserDataType = new List<Type>()
					};
					m_Cache[i] = overloadCacheItem;
					break;
				}
				if (m_Cache[i].HitIndexAtLastHit < num)
				{
					num = m_Cache[i].HitIndexAtLastHit;
					overloadCacheItem = m_Cache[i];
				}
			}
			if (overloadCacheItem == null)
			{
				m_Cache = new OverloadCacheItem[5];
				overloadCacheItem = new OverloadCacheItem
				{
					ArgsDataType = new List<DataType>(),
					ArgsUserDataType = new List<Type>()
				};
				m_Cache[0] = overloadCacheItem;
				m_CacheHits = 0;
			}
			overloadCacheItem.Method = bestOverload;
			overloadCacheItem.HitIndexAtLastHit = ++m_CacheHits;
			overloadCacheItem.ArgsDataType.Clear();
			overloadCacheItem.HasObject = hasObject;
			for (int j = 0; j < args.Count; j++)
			{
				overloadCacheItem.ArgsDataType.Add(args[j].Type);
				if (args[j].Type == DataType.UserData)
				{
					overloadCacheItem.ArgsUserDataType.Add(args[j].UserData.Descriptor.Type);
				}
				else
				{
					overloadCacheItem.ArgsUserDataType.Add(null);
				}
			}
		}

		private bool CheckMatch(bool hasObject, CallbackArguments args, OverloadCacheItem overloadCacheItem)
		{
			if (overloadCacheItem.HasObject && !hasObject)
			{
				return false;
			}
			if (args.Count != overloadCacheItem.ArgsDataType.Count)
			{
				return false;
			}
			for (int i = 0; i < args.Count; i++)
			{
				if (args[i].Type != overloadCacheItem.ArgsDataType[i])
				{
					return false;
				}
				if (args[i].Type == DataType.UserData && args[i].UserData.Descriptor.Type != overloadCacheItem.ArgsUserDataType[i])
				{
					return false;
				}
			}
			overloadCacheItem.HitIndexAtLastHit = ++m_CacheHits;
			return true;
		}

		private int CalcScoreForOverload(ScriptExecutionContext context, CallbackArguments args, IOverloadableMemberDescriptor method, bool isExtMethod)
		{
			int num = 100;
			int num2 = (args.IsMethodCall ? 1 : 0);
			int num3 = num2;
			bool flag = false;
			for (int i = 0; i < method.Parameters.Length; i++)
			{
				if ((isExtMethod && i == 0) || method.Parameters[i].IsOut)
				{
					continue;
				}
				Type type = method.Parameters[i].Type;
				if (type == typeof(Script) || type == typeof(ScriptExecutionContext) || type == typeof(CallbackArguments))
				{
					continue;
				}
				if (i == method.Parameters.Length - 1 && method.VarArgsArrayType != null)
				{
					int num4 = 0;
					DynValue dynValue = null;
					int num5 = num;
					while (true)
					{
						DynValue dynValue2 = args.RawGet(num3, false);
						if (dynValue2 == null)
						{
							break;
						}
						if (dynValue == null)
						{
							dynValue = dynValue2;
						}
						num3++;
						num4++;
						int val = CalcScoreForSingleArgument(method.Parameters[i], method.VarArgsElementType, dynValue2, false);
						num = Math.Min(num, val);
					}
					if (num4 == 1 && dynValue.Type == DataType.UserData && dynValue.UserData.Object != null && Framework.Do.IsAssignableFrom(method.VarArgsArrayType, dynValue.UserData.Object.GetType()))
					{
						num = num5;
						continue;
					}
					if (num4 == 0)
					{
						num = Math.Min(num, 40);
					}
					flag = true;
				}
				else
				{
					DynValue arg = args.RawGet(num3, false) ?? DynValue.Void;
					int val2 = CalcScoreForSingleArgument(method.Parameters[i], type, arg, method.Parameters[i].HasDefaultValue);
					num = Math.Min(num, val2);
					num3++;
				}
			}
			if (num > 0)
			{
				if (args.Count - num2 <= method.Parameters.Length)
				{
					num += 100;
					num *= 1000;
				}
				else if (flag)
				{
					num--;
					num *= 1000;
				}
				else
				{
					num *= 1000;
					num -= 2 * (args.Count - num2 - method.Parameters.Length);
					num = Math.Max(1, num);
				}
			}
			return num;
		}

		private static int CalcScoreForSingleArgument(ParameterDescriptor desc, Type parameterType, DynValue arg, bool isOptional)
		{
			int num = ScriptToClrConversions.DynValueToObjectOfTypeWeight(arg, parameterType, isOptional);
			if (parameterType.IsByRef || desc.IsOut || desc.IsRef)
			{
				num = Math.Max(0, num + -10);
			}
			return num;
		}

		public Func<ScriptExecutionContext, CallbackArguments, DynValue> GetCallback(Script script, object obj)
		{
			_003C_003Ec__DisplayClass33_0 _003C_003Ec__DisplayClass33_ = new _003C_003Ec__DisplayClass33_0();
			_003C_003Ec__DisplayClass33_._003C_003E4__this = this;
			_003C_003Ec__DisplayClass33_.script = script;
			_003C_003Ec__DisplayClass33_.obj = obj;
			return _003C_003Ec__DisplayClass33_._003CGetCallback_003Eb__0;
		}

		void IOptimizableDescriptor.Optimize()
		{
			foreach (IOptimizableDescriptor item in m_Overloads.OfType<IOptimizableDescriptor>())
			{
				item.Optimize();
			}
		}

		public CallbackFunction GetCallbackFunction(Script script, object obj = null)
		{
			return new CallbackFunction(GetCallback(script, obj), Name);
		}

		public DynValue GetValue(Script script, object obj)
		{
			return DynValue.NewCallback(GetCallbackFunction(script, obj));
		}

		public void SetValue(Script script, object obj, DynValue value)
		{
			this.CheckAccess(MemberDescriptorAccess.CanWrite, obj);
		}

		public void PrepareForWiring(Table t)
		{
			t.Set("class", DynValue.NewString(GetType().FullName));
			t.Set("name", DynValue.NewString(Name));
			t.Set("decltype", DynValue.NewString(DeclaringType.FullName));
			DynValue dynValue = DynValue.NewPrimeTable();
			t.Set("overloads", dynValue);
			int num = 0;
			foreach (IOverloadableMemberDescriptor overload in m_Overloads)
			{
				IWireableDescriptor wireableDescriptor = overload as IWireableDescriptor;
				if (wireableDescriptor != null)
				{
					DynValue dynValue2 = DynValue.NewPrimeTable();
					dynValue.Table.Set(++num, dynValue2);
					wireableDescriptor.PrepareForWiring(dynValue2.Table);
				}
				else
				{
					dynValue.Table.Set(++num, DynValue.NewString(string.Format("unsupported - {0} is not serializable", overload.GetType().FullName)));
				}
			}
		}
	}
}
