using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using MoonSharp.Interpreter.Compatibility;
using MoonSharp.Interpreter.DataStructs;
using MoonSharp.Interpreter.Interop.BasicDescriptors;

namespace MoonSharp.Interpreter.Interop.UserDataRegistries
{
	internal class ExtensionMethodsRegistry
	{
		private class UnresolvedGenericMethod
		{
			public readonly MethodInfo Method;

			public readonly InteropAccessMode AccessMode;

			public readonly HashSet<Type> AlreadyAddedTypes = new HashSet<Type>();

			public UnresolvedGenericMethod(MethodInfo mi, InteropAccessMode mode)
			{
				AccessMode = mode;
				Method = mi;
			}
		}

		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<MethodInfo, bool> _003C_003E9__5_0;

			internal bool _003CRegisterExtensionType_003Eb__5_0(MethodInfo _mi)
			{
				return _mi.IsStatic;
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass9_0
		{
			public Type extendedType;

			internal bool _003CGetExtensionMethodsByNameAndType_003Eb__0(IOverloadableMemberDescriptor d)
			{
				if (d.ExtensionMethodType != null)
				{
					return Framework.Do.IsAssignableFrom(d.ExtensionMethodType, extendedType);
				}
				return false;
			}
		}

		private static object s_Lock = new object();

		private static MultiDictionary<string, IOverloadableMemberDescriptor> s_Registry = new MultiDictionary<string, IOverloadableMemberDescriptor>();

		private static MultiDictionary<string, UnresolvedGenericMethod> s_UnresolvedGenericsRegistry = new MultiDictionary<string, UnresolvedGenericMethod>();

		private static int s_ExtensionMethodChangeVersion = 0;

		public static void RegisterExtensionType(Type type, InteropAccessMode mode = InteropAccessMode.Default)
		{
			lock (s_Lock)
			{
				bool flag = false;
				foreach (MethodInfo item in Framework.Do.GetMethods(type).Where(_003C_003Ec._003C_003E9__5_0 ?? (_003C_003Ec._003C_003E9__5_0 = _003C_003Ec._003C_003E9._003CRegisterExtensionType_003Eb__5_0)))
				{
					if (item.GetCustomAttributes(typeof(ExtensionAttribute), false).Count() != 0)
					{
						if (item.ContainsGenericParameters)
						{
							s_UnresolvedGenericsRegistry.Add(item.Name, new UnresolvedGenericMethod(item, mode));
							flag = true;
						}
						else if (MethodMemberDescriptor.CheckMethodIsCompatible(item, false))
						{
							MethodMemberDescriptor value = new MethodMemberDescriptor(item, mode);
							s_Registry.Add(item.Name, value);
							flag = true;
						}
					}
				}
				if (flag)
				{
					s_ExtensionMethodChangeVersion++;
				}
			}
		}

		private static object FrameworkGetMethods()
		{
			throw new NotImplementedException();
		}

		public static IEnumerable<IOverloadableMemberDescriptor> GetExtensionMethodsByName(string name)
		{
			lock (s_Lock)
			{
				return new List<IOverloadableMemberDescriptor>(s_Registry.Find(name));
			}
		}

		public static int GetExtensionMethodsChangeVersion()
		{
			return s_ExtensionMethodChangeVersion;
		}

		public static List<IOverloadableMemberDescriptor> GetExtensionMethodsByNameAndType(string name, Type extendedType)
		{
			_003C_003Ec__DisplayClass9_0 _003C_003Ec__DisplayClass9_ = new _003C_003Ec__DisplayClass9_0();
			_003C_003Ec__DisplayClass9_.extendedType = extendedType;
			List<UnresolvedGenericMethod> list = null;
			lock (s_Lock)
			{
				list = s_UnresolvedGenericsRegistry.Find(name).ToList();
			}
			foreach (UnresolvedGenericMethod item in list)
			{
				ParameterInfo[] parameters = item.Method.GetParameters();
				if (parameters.Length == 0)
				{
					continue;
				}
				Type parameterType = parameters[0].ParameterType;
				Type genericMatch = GetGenericMatch(parameterType, _003C_003Ec__DisplayClass9_.extendedType);
				if (item.AlreadyAddedTypes.Add(genericMatch) && genericMatch != null)
				{
					MethodInfo methodInfo = InstantiateMethodInfo(item.Method, parameterType, genericMatch, _003C_003Ec__DisplayClass9_.extendedType);
					if (methodInfo != null && MethodMemberDescriptor.CheckMethodIsCompatible(methodInfo, false))
					{
						MethodMemberDescriptor value = new MethodMemberDescriptor(methodInfo, item.AccessMode);
						s_Registry.Add(item.Method.Name, value);
						s_ExtensionMethodChangeVersion++;
					}
				}
			}
			return s_Registry.Find(name).Where(_003C_003Ec__DisplayClass9_._003CGetExtensionMethodsByNameAndType_003Eb__0).ToList();
		}

		private static MethodInfo InstantiateMethodInfo(MethodInfo mi, Type extensionType, Type genericType, Type extendedType)
		{
			Type[] genericArguments = mi.GetGenericArguments();
			Type[] genericArguments2 = Framework.Do.GetGenericArguments(genericType);
			if (genericArguments2.Length == genericArguments.Length)
			{
				return mi.MakeGenericMethod(genericArguments2);
			}
			return null;
		}

		private static Type GetGenericMatch(Type extensionType, Type extendedType)
		{
			if (!extensionType.IsGenericParameter)
			{
				extensionType = extensionType.GetGenericTypeDefinition();
				foreach (Type allImplementedType in extendedType.GetAllImplementedTypes())
				{
					if (Framework.Do.IsGenericType(allImplementedType) && allImplementedType.GetGenericTypeDefinition() == extensionType)
					{
						return allImplementedType;
					}
				}
			}
			return null;
		}
	}
}
