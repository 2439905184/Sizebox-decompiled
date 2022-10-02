using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using MoonSharp.Interpreter.Compatibility;
using MoonSharp.Interpreter.Interop.BasicDescriptors;
using MoonSharp.Interpreter.Interop.RegistrationPolicies;

namespace MoonSharp.Interpreter.Interop.UserDataRegistries
{
	internal static class TypeDescriptorRegistry
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<Type, _003C_003Ef__AnonymousType8<Type, Attribute[]>> _003C_003E9__4_3;

			public static Func<_003C_003Ef__AnonymousType8<Type, Attribute[]>, bool> _003C_003E9__4_4;

			public static Func<_003C_003Ef__AnonymousType8<Type, Attribute[]>, _003C_003Ef__AnonymousType9<Attribute[], Type>> _003C_003E9__4_5;

			public static Func<Type, _003C_003Ef__AnonymousType8<Type, Attribute[]>> _003C_003E9__4_0;

			public static Func<_003C_003Ef__AnonymousType8<Type, Attribute[]>, bool> _003C_003E9__4_1;

			public static Func<_003C_003Ef__AnonymousType8<Type, Attribute[]>, _003C_003Ef__AnonymousType9<Attribute[], Type>> _003C_003E9__4_2;

			public static Func<Type, bool> _003C_003E9__11_0;

			internal _003C_003Ef__AnonymousType8<Type, Attribute[]> _003CRegisterAssembly_003Eb__4_3(Type t)
			{
				return new _003C_003Ef__AnonymousType8<Type, Attribute[]>(t, Framework.Do.GetCustomAttributes(t, typeof(ExtensionAttribute), true));
			}

			internal bool _003CRegisterAssembly_003Eb__4_4(_003C_003Ef__AnonymousType8<Type, Attribute[]> _003C_003Eh__TransparentIdentifier0)
			{
				if (_003C_003Eh__TransparentIdentifier0.attributes != null)
				{
					return _003C_003Eh__TransparentIdentifier0.attributes.Length != 0;
				}
				return false;
			}

			internal _003C_003Ef__AnonymousType9<Attribute[], Type> _003CRegisterAssembly_003Eb__4_5(_003C_003Ef__AnonymousType8<Type, Attribute[]> _003C_003Eh__TransparentIdentifier0)
			{
				return new _003C_003Ef__AnonymousType9<Attribute[], Type>(_003C_003Eh__TransparentIdentifier0.attributes, _003C_003Eh__TransparentIdentifier0.t);
			}

			internal _003C_003Ef__AnonymousType8<Type, Attribute[]> _003CRegisterAssembly_003Eb__4_0(Type t)
			{
				return new _003C_003Ef__AnonymousType8<Type, Attribute[]>(t, Framework.Do.GetCustomAttributes(t, typeof(MoonSharpUserDataAttribute), true));
			}

			internal bool _003CRegisterAssembly_003Eb__4_1(_003C_003Ef__AnonymousType8<Type, Attribute[]> _003C_003Eh__TransparentIdentifier0)
			{
				if (_003C_003Eh__TransparentIdentifier0.attributes != null)
				{
					return _003C_003Eh__TransparentIdentifier0.attributes.Length != 0;
				}
				return false;
			}

			internal _003C_003Ef__AnonymousType9<Attribute[], Type> _003CRegisterAssembly_003Eb__4_2(_003C_003Ef__AnonymousType8<Type, Attribute[]> _003C_003Eh__TransparentIdentifier0)
			{
				return new _003C_003Ef__AnonymousType9<Attribute[], Type>(_003C_003Eh__TransparentIdentifier0.attributes, _003C_003Eh__TransparentIdentifier0.t);
			}

			internal bool _003CRegisterType_Impl_003Eb__11_0(Type ii)
			{
				return ii == typeof(IUserDataType);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass11_0
		{
			public StandardUserDataDescriptor udd;

			internal void _003CRegisterType_Impl_003Eb__1(object o)
			{
				((IOptimizableDescriptor)udd).Optimize();
			}
		}

		private static object s_Lock = new object();

		private static Dictionary<Type, IUserDataDescriptor> s_TypeRegistry = new Dictionary<Type, IUserDataDescriptor>();

		private static Dictionary<Type, IUserDataDescriptor> s_TypeRegistryHistory = new Dictionary<Type, IUserDataDescriptor>();

		private static InteropAccessMode s_DefaultAccessMode;

		internal static InteropAccessMode DefaultAccessMode
		{
			get
			{
				return s_DefaultAccessMode;
			}
			set
			{
				if (value == InteropAccessMode.Default)
				{
					throw new ArgumentException("InteropAccessMode is InteropAccessMode.Default");
				}
				s_DefaultAccessMode = value;
			}
		}

		public static IEnumerable<KeyValuePair<Type, IUserDataDescriptor>> RegisteredTypes
		{
			get
			{
				lock (s_Lock)
				{
					return s_TypeRegistry.ToArray();
				}
			}
		}

		public static IEnumerable<KeyValuePair<Type, IUserDataDescriptor>> RegisteredTypesHistory
		{
			get
			{
				lock (s_Lock)
				{
					return s_TypeRegistryHistory.ToArray();
				}
			}
		}

		internal static IRegistrationPolicy RegistrationPolicy { get; set; }

		internal static void RegisterAssembly(Assembly asm = null, bool includeExtensionTypes = false)
		{
			if (asm == null)
			{
				asm = Assembly.GetCallingAssembly();
			}
			if (includeExtensionTypes)
			{
				foreach (_003C_003Ef__AnonymousType9<Attribute[], Type> item in asm.SafeGetTypes().Select(_003C_003Ec._003C_003E9__4_3 ?? (_003C_003Ec._003C_003E9__4_3 = _003C_003Ec._003C_003E9._003CRegisterAssembly_003Eb__4_3)).Where(_003C_003Ec._003C_003E9__4_4 ?? (_003C_003Ec._003C_003E9__4_4 = _003C_003Ec._003C_003E9._003CRegisterAssembly_003Eb__4_4))
					.Select(_003C_003Ec._003C_003E9__4_5 ?? (_003C_003Ec._003C_003E9__4_5 = _003C_003Ec._003C_003E9._003CRegisterAssembly_003Eb__4_5)))
				{
					UserData.RegisterExtensionType(item.DataType);
				}
			}
			foreach (_003C_003Ef__AnonymousType9<Attribute[], Type> item2 in asm.SafeGetTypes().Select(_003C_003Ec._003C_003E9__4_0 ?? (_003C_003Ec._003C_003E9__4_0 = _003C_003Ec._003C_003E9._003CRegisterAssembly_003Eb__4_0)).Where(_003C_003Ec._003C_003E9__4_1 ?? (_003C_003Ec._003C_003E9__4_1 = _003C_003Ec._003C_003E9._003CRegisterAssembly_003Eb__4_1))
				.Select(_003C_003Ec._003C_003E9__4_2 ?? (_003C_003Ec._003C_003E9__4_2 = _003C_003Ec._003C_003E9._003CRegisterAssembly_003Eb__4_2)))
			{
				UserData.RegisterType(item2.DataType, item2.Attributes.OfType<MoonSharpUserDataAttribute>().First().AccessMode);
			}
		}

		internal static bool IsTypeRegistered(Type type)
		{
			lock (s_Lock)
			{
				return s_TypeRegistry.ContainsKey(type);
			}
		}

		internal static void UnregisterType(Type t)
		{
			lock (s_Lock)
			{
				if (s_TypeRegistry.ContainsKey(t))
				{
					PerformRegistration(t, null, s_TypeRegistry[t]);
				}
			}
		}

		internal static IUserDataDescriptor RegisterProxyType_Impl(IProxyFactory proxyFactory, InteropAccessMode accessMode, string friendlyName)
		{
			IUserDataDescriptor proxyDescriptor = RegisterType_Impl(proxyFactory.ProxyType, accessMode, friendlyName, null);
			return RegisterType_Impl(proxyFactory.TargetType, accessMode, friendlyName, new ProxyUserDataDescriptor(proxyFactory, proxyDescriptor, friendlyName));
		}

		internal static IUserDataDescriptor RegisterType_Impl(Type type, InteropAccessMode accessMode, string friendlyName, IUserDataDescriptor descriptor)
		{
			accessMode = ResolveDefaultAccessModeForType(accessMode, type);
			lock (s_Lock)
			{
				IUserDataDescriptor value = null;
				s_TypeRegistry.TryGetValue(type, out value);
				if (descriptor == null)
				{
					if (IsTypeBlacklisted(type))
					{
						return null;
					}
					if (Framework.Do.GetInterfaces(type).Any(_003C_003Ec._003C_003E9__11_0 ?? (_003C_003Ec._003C_003E9__11_0 = _003C_003Ec._003C_003E9._003CRegisterType_Impl_003Eb__11_0)))
					{
						AutoDescribingUserDataDescriptor newDescriptor = new AutoDescribingUserDataDescriptor(type, friendlyName);
						return PerformRegistration(type, newDescriptor, value);
					}
					if (Framework.Do.IsGenericTypeDefinition(type))
					{
						StandardGenericsUserDataDescriptor newDescriptor2 = new StandardGenericsUserDataDescriptor(type, accessMode);
						return PerformRegistration(type, newDescriptor2, value);
					}
					if (Framework.Do.IsEnum(type))
					{
						StandardEnumUserDataDescriptor newDescriptor3 = new StandardEnumUserDataDescriptor(type, friendlyName);
						return PerformRegistration(type, newDescriptor3, value);
					}
					_003C_003Ec__DisplayClass11_0 _003C_003Ec__DisplayClass11_ = new _003C_003Ec__DisplayClass11_0();
					_003C_003Ec__DisplayClass11_.udd = new StandardUserDataDescriptor(type, accessMode, friendlyName);
					if (accessMode == InteropAccessMode.BackgroundOptimized)
					{
						ThreadPool.QueueUserWorkItem(_003C_003Ec__DisplayClass11_._003CRegisterType_Impl_003Eb__1);
					}
					return PerformRegistration(type, _003C_003Ec__DisplayClass11_.udd, value);
				}
				PerformRegistration(type, descriptor, value);
				return descriptor;
			}
		}

		private static IUserDataDescriptor PerformRegistration(Type type, IUserDataDescriptor newDescriptor, IUserDataDescriptor oldDescriptor)
		{
			IUserDataDescriptor userDataDescriptor = RegistrationPolicy.HandleRegistration(newDescriptor, oldDescriptor);
			if (userDataDescriptor != oldDescriptor)
			{
				if (userDataDescriptor == null)
				{
					s_TypeRegistry.Remove(type);
				}
				else
				{
					s_TypeRegistry[type] = userDataDescriptor;
					s_TypeRegistryHistory[type] = userDataDescriptor;
				}
			}
			return userDataDescriptor;
		}

		internal static InteropAccessMode ResolveDefaultAccessModeForType(InteropAccessMode accessMode, Type type)
		{
			if (accessMode == InteropAccessMode.Default)
			{
				MoonSharpUserDataAttribute moonSharpUserDataAttribute = Framework.Do.GetCustomAttributes(type, true).OfType<MoonSharpUserDataAttribute>().SingleOrDefault();
				if (moonSharpUserDataAttribute != null)
				{
					accessMode = moonSharpUserDataAttribute.AccessMode;
				}
			}
			if (accessMode == InteropAccessMode.Default)
			{
				accessMode = s_DefaultAccessMode;
			}
			return accessMode;
		}

		internal static IUserDataDescriptor GetDescriptorForType(Type type, bool searchInterfaces)
		{
			lock (s_Lock)
			{
				IUserDataDescriptor userDataDescriptor = null;
				if (s_TypeRegistry.ContainsKey(type))
				{
					return s_TypeRegistry[type];
				}
				if (RegistrationPolicy.AllowTypeAutoRegistration(type) && !Framework.Do.IsAssignableFrom(typeof(Delegate), type))
				{
					return RegisterType_Impl(type, DefaultAccessMode, type.FullName, null);
				}
				Type type2 = type;
				while (type2 != null)
				{
					IUserDataDescriptor value;
					if (s_TypeRegistry.TryGetValue(type2, out value))
					{
						userDataDescriptor = value;
						break;
					}
					if (Framework.Do.IsGenericType(type2) && s_TypeRegistry.TryGetValue(type2.GetGenericTypeDefinition(), out value))
					{
						userDataDescriptor = value;
						break;
					}
					type2 = Framework.Do.GetBaseType(type2);
				}
				if (userDataDescriptor is IGeneratorUserDataDescriptor)
				{
					userDataDescriptor = ((IGeneratorUserDataDescriptor)userDataDescriptor).Generate(type);
				}
				if (!searchInterfaces)
				{
					return userDataDescriptor;
				}
				List<IUserDataDescriptor> list = new List<IUserDataDescriptor>();
				if (userDataDescriptor != null)
				{
					list.Add(userDataDescriptor);
				}
				if (searchInterfaces)
				{
					Type[] interfaces = Framework.Do.GetInterfaces(type);
					foreach (Type type3 in interfaces)
					{
						IUserDataDescriptor value2;
						if (s_TypeRegistry.TryGetValue(type3, out value2))
						{
							if (value2 is IGeneratorUserDataDescriptor)
							{
								value2 = ((IGeneratorUserDataDescriptor)value2).Generate(type);
							}
							if (value2 != null)
							{
								list.Add(value2);
							}
						}
						else if (Framework.Do.IsGenericType(type3) && s_TypeRegistry.TryGetValue(type3.GetGenericTypeDefinition(), out value2))
						{
							if (value2 is IGeneratorUserDataDescriptor)
							{
								value2 = ((IGeneratorUserDataDescriptor)value2).Generate(type);
							}
							if (value2 != null)
							{
								list.Add(value2);
							}
						}
					}
				}
				if (list.Count == 1)
				{
					return list[0];
				}
				if (list.Count == 0)
				{
					return null;
				}
				return new CompositeUserDataDescriptor(list, type);
			}
		}

		private static bool FrameworkIsAssignableFrom(Type type)
		{
			throw new NotImplementedException();
		}

		public static bool IsTypeBlacklisted(Type t)
		{
			if (Framework.Do.IsValueType(t) && Framework.Do.GetInterfaces(t).Contains(typeof(IEnumerator)))
			{
				return true;
			}
			return false;
		}
	}
}
