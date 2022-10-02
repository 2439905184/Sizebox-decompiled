using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using MoonSharp.Interpreter.Interop;
using MoonSharp.Interpreter.Interop.BasicDescriptors;
using MoonSharp.Interpreter.Interop.RegistrationPolicies;
using MoonSharp.Interpreter.Interop.StandardDescriptors;
using MoonSharp.Interpreter.Interop.UserDataRegistries;
using MoonSharp.Interpreter.Serialization.Json;

namespace MoonSharp.Interpreter
{
	public class UserData : RefIdObject
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<KeyValuePair<Type, IUserDataDescriptor>, Type> _003C_003E9__44_0;

			internal Type _003CGetRegisteredTypes_003Eb__44_0(KeyValuePair<Type, IUserDataDescriptor> p)
			{
				return p.Value.Type;
			}
		}

		public DynValue UserValue { get; set; }

		public object Object { get; private set; }

		public IUserDataDescriptor Descriptor { get; private set; }

		public static IRegistrationPolicy RegistrationPolicy
		{
			get
			{
				return TypeDescriptorRegistry.RegistrationPolicy;
			}
			set
			{
				TypeDescriptorRegistry.RegistrationPolicy = value;
			}
		}

		public static InteropAccessMode DefaultAccessMode
		{
			get
			{
				return TypeDescriptorRegistry.DefaultAccessMode;
			}
			set
			{
				TypeDescriptorRegistry.DefaultAccessMode = value;
			}
		}

		private UserData()
		{
		}

		static UserData()
		{
			RegistrationPolicy = InteropRegistrationPolicy.Default;
			RegisterType<EventFacade>(InteropAccessMode.NoReflectionAllowed);
			RegisterType<AnonWrapper>(InteropAccessMode.HideMembers);
			RegisterType<EnumerableWrapper>(InteropAccessMode.NoReflectionAllowed);
			RegisterType<JsonNull>(InteropAccessMode.Reflection);
			DefaultAccessMode = InteropAccessMode.LazyOptimized;
		}

		public static IUserDataDescriptor RegisterType<T>(InteropAccessMode accessMode = InteropAccessMode.Default, string friendlyName = null)
		{
			return TypeDescriptorRegistry.RegisterType_Impl(typeof(T), accessMode, friendlyName, null);
		}

		public static IUserDataDescriptor RegisterType(Type type, InteropAccessMode accessMode = InteropAccessMode.Default, string friendlyName = null)
		{
			return TypeDescriptorRegistry.RegisterType_Impl(type, accessMode, friendlyName, null);
		}

		public static IUserDataDescriptor RegisterProxyType(IProxyFactory proxyFactory, InteropAccessMode accessMode = InteropAccessMode.Default, string friendlyName = null)
		{
			return TypeDescriptorRegistry.RegisterProxyType_Impl(proxyFactory, accessMode, friendlyName);
		}

		public static IUserDataDescriptor RegisterProxyType<TProxy, TTarget>(Func<TTarget, TProxy> wrapDelegate, InteropAccessMode accessMode = InteropAccessMode.Default, string friendlyName = null) where TProxy : class where TTarget : class
		{
			return RegisterProxyType(new DelegateProxyFactory<TProxy, TTarget>(wrapDelegate), accessMode, friendlyName);
		}

		public static IUserDataDescriptor RegisterType<T>(IUserDataDescriptor customDescriptor)
		{
			return TypeDescriptorRegistry.RegisterType_Impl(typeof(T), InteropAccessMode.Default, null, customDescriptor);
		}

		public static IUserDataDescriptor RegisterType(Type type, IUserDataDescriptor customDescriptor)
		{
			return TypeDescriptorRegistry.RegisterType_Impl(type, InteropAccessMode.Default, null, customDescriptor);
		}

		public static IUserDataDescriptor RegisterType(IUserDataDescriptor customDescriptor)
		{
			return TypeDescriptorRegistry.RegisterType_Impl(customDescriptor.Type, InteropAccessMode.Default, null, customDescriptor);
		}

		public static void RegisterAssembly(Assembly asm = null, bool includeExtensionTypes = false)
		{
			if (asm == null)
			{
				asm = Assembly.GetCallingAssembly();
			}
			TypeDescriptorRegistry.RegisterAssembly(asm, includeExtensionTypes);
		}

		public static bool IsTypeRegistered(Type t)
		{
			return TypeDescriptorRegistry.IsTypeRegistered(t);
		}

		public static bool IsTypeRegistered<T>()
		{
			return TypeDescriptorRegistry.IsTypeRegistered(typeof(T));
		}

		public static void UnregisterType<T>()
		{
			TypeDescriptorRegistry.UnregisterType(typeof(T));
		}

		public static void UnregisterType(Type t)
		{
			TypeDescriptorRegistry.UnregisterType(t);
		}

		public static DynValue Create(object o, IUserDataDescriptor descr)
		{
			return DynValue.NewUserData(new UserData
			{
				Descriptor = descr,
				Object = o
			});
		}

		public static DynValue Create(object o)
		{
			IUserDataDescriptor descriptorForObject = GetDescriptorForObject(o);
			if (descriptorForObject == null)
			{
				if (o is Type)
				{
					return CreateStatic((Type)o);
				}
				return null;
			}
			return Create(o, descriptorForObject);
		}

		public static DynValue CreateStatic(IUserDataDescriptor descr)
		{
			if (descr == null)
			{
				return null;
			}
			return DynValue.NewUserData(new UserData
			{
				Descriptor = descr,
				Object = null
			});
		}

		public static DynValue CreateStatic(Type t)
		{
			return CreateStatic(GetDescriptorForType(t, false));
		}

		public static DynValue CreateStatic<T>()
		{
			return CreateStatic(GetDescriptorForType(typeof(T), false));
		}

		public static void RegisterExtensionType(Type type, InteropAccessMode mode = InteropAccessMode.Default)
		{
			ExtensionMethodsRegistry.RegisterExtensionType(type, mode);
		}

		public static List<IOverloadableMemberDescriptor> GetExtensionMethodsByNameAndType(string name, Type extendedType)
		{
			return ExtensionMethodsRegistry.GetExtensionMethodsByNameAndType(name, extendedType);
		}

		public static int GetExtensionMethodsChangeVersion()
		{
			return ExtensionMethodsRegistry.GetExtensionMethodsChangeVersion();
		}

		public static IUserDataDescriptor GetDescriptorForType<T>(bool searchInterfaces)
		{
			return TypeDescriptorRegistry.GetDescriptorForType(typeof(T), searchInterfaces);
		}

		public static IUserDataDescriptor GetDescriptorForType(Type type, bool searchInterfaces)
		{
			return TypeDescriptorRegistry.GetDescriptorForType(type, searchInterfaces);
		}

		public static IUserDataDescriptor GetDescriptorForObject(object o)
		{
			return TypeDescriptorRegistry.GetDescriptorForType(o.GetType(), true);
		}

		public static Table GetDescriptionOfRegisteredTypes(bool useHistoricalData = false)
		{
			DynValue dynValue = DynValue.NewPrimeTable();
			foreach (KeyValuePair<Type, IUserDataDescriptor> item in useHistoricalData ? TypeDescriptorRegistry.RegisteredTypesHistory : TypeDescriptorRegistry.RegisteredTypes)
			{
				IWireableDescriptor wireableDescriptor = item.Value as IWireableDescriptor;
				if (wireableDescriptor != null)
				{
					DynValue dynValue2 = DynValue.NewPrimeTable();
					dynValue.Table.Set(item.Key.FullName, dynValue2);
					wireableDescriptor.PrepareForWiring(dynValue2.Table);
				}
			}
			return dynValue.Table;
		}

		public static IEnumerable<Type> GetRegisteredTypes(bool useHistoricalData = false)
		{
			return (useHistoricalData ? TypeDescriptorRegistry.RegisteredTypesHistory : TypeDescriptorRegistry.RegisteredTypes).Select(_003C_003Ec._003C_003E9__44_0 ?? (_003C_003Ec._003C_003E9__44_0 = _003C_003Ec._003C_003E9._003CGetRegisteredTypes_003Eb__44_0));
		}
	}
}
