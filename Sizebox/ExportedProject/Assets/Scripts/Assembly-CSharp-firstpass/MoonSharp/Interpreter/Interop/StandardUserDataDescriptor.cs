using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using MoonSharp.Interpreter.Compatibility;
using MoonSharp.Interpreter.Interop.BasicDescriptors;

namespace MoonSharp.Interpreter.Interop
{
	public class StandardUserDataDescriptor : DispatchingUserDataDescriptor, IWireableDescriptor
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<MoonSharpHideMemberAttribute, string> _003C_003E9__5_0;

			internal string _003CFillMemberList_003Eb__5_0(MoonSharpHideMemberAttribute a)
			{
				return a.MemberName;
			}
		}

		public InteropAccessMode AccessMode { get; private set; }

		public StandardUserDataDescriptor(Type type, InteropAccessMode accessMode, string friendlyName = null)
			: base(type, friendlyName)
		{
			if (accessMode == InteropAccessMode.NoReflectionAllowed)
			{
				throw new ArgumentException("Can't create a StandardUserDataDescriptor under a NoReflectionAllowed access mode");
			}
			if (Script.GlobalOptions.Platform.IsRunningOnAOT())
			{
				accessMode = InteropAccessMode.Reflection;
			}
			if (accessMode == InteropAccessMode.Default)
			{
				accessMode = UserData.DefaultAccessMode;
			}
			AccessMode = accessMode;
			FillMemberList();
		}

		private void FillMemberList()
		{
			HashSet<string> hashSet = new HashSet<string>(Framework.Do.GetCustomAttributes(base.Type, typeof(MoonSharpHideMemberAttribute), true).OfType<MoonSharpHideMemberAttribute>().Select(_003C_003Ec._003C_003E9__5_0 ?? (_003C_003Ec._003C_003E9__5_0 = _003C_003Ec._003C_003E9._003CFillMemberList_003Eb__5_0)));
			Type type = base.Type;
			if (AccessMode == InteropAccessMode.HideMembers)
			{
				return;
			}
			if (!type.IsDelegateType())
			{
				ConstructorInfo[] constructors = Framework.Do.GetConstructors(type);
				foreach (ConstructorInfo methodBase in constructors)
				{
					if (!hashSet.Contains("__new"))
					{
						AddMember("__new", MethodMemberDescriptor.TryCreateIfVisible(methodBase, AccessMode));
					}
				}
				if (Framework.Do.IsValueType(type) && !hashSet.Contains("__new"))
				{
					AddMember("__new", new ValueTypeDefaultCtorMemberDescriptor(type));
				}
			}
			MethodInfo[] methods = Framework.Do.GetMethods(type);
			foreach (MethodInfo methodInfo in methods)
			{
				if (hashSet.Contains(methodInfo.Name))
				{
					continue;
				}
				MethodMemberDescriptor methodMemberDescriptor = MethodMemberDescriptor.TryCreateIfVisible(methodInfo, AccessMode);
				if (methodMemberDescriptor == null || !MethodMemberDescriptor.CheckMethodIsCompatible(methodInfo, false))
				{
					continue;
				}
				string name = methodInfo.Name;
				if (methodInfo.IsSpecialName && (methodInfo.Name == "op_Explicit" || methodInfo.Name == "op_Implicit"))
				{
					name = methodInfo.ReturnType.GetConversionMethodName();
				}
				AddMember(name, methodMemberDescriptor);
				foreach (string metaNamesFromAttribute in methodInfo.GetMetaNamesFromAttributes())
				{
					AddMetaMember(metaNamesFromAttribute, methodMemberDescriptor);
				}
			}
			PropertyInfo[] properties = Framework.Do.GetProperties(type);
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (!propertyInfo.IsSpecialName && !propertyInfo.GetIndexParameters().Any() && !hashSet.Contains(propertyInfo.Name))
				{
					AddMember(propertyInfo.Name, PropertyMemberDescriptor.TryCreateIfVisible(propertyInfo, AccessMode));
				}
			}
			FieldInfo[] fields = Framework.Do.GetFields(type);
			foreach (FieldInfo fieldInfo in fields)
			{
				if (!fieldInfo.IsSpecialName && !hashSet.Contains(fieldInfo.Name))
				{
					AddMember(fieldInfo.Name, FieldMemberDescriptor.TryCreateIfVisible(fieldInfo, AccessMode));
				}
			}
			EventInfo[] events = Framework.Do.GetEvents(type);
			foreach (EventInfo eventInfo in events)
			{
				if (!eventInfo.IsSpecialName && !hashSet.Contains(eventInfo.Name))
				{
					AddMember(eventInfo.Name, EventMemberDescriptor.TryCreateIfVisible(eventInfo, AccessMode));
				}
			}
			Type[] nestedTypes = Framework.Do.GetNestedTypes(type);
			foreach (Type type2 in nestedTypes)
			{
				if (!hashSet.Contains(type2.Name) && !Framework.Do.IsGenericTypeDefinition(type2) && (Framework.Do.IsNestedPublic(type2) || Framework.Do.GetCustomAttributes(type2, typeof(MoonSharpUserDataAttribute), true).Length != 0) && UserData.RegisterType(type2, AccessMode) != null)
				{
					AddDynValue(type2.Name, UserData.CreateStatic(type2));
				}
			}
			if (hashSet.Contains("[this]"))
			{
				return;
			}
			if (base.Type.IsArray)
			{
				int arrayRank = base.Type.GetArrayRank();
				ParameterDescriptor[] array = new ParameterDescriptor[arrayRank];
				ParameterDescriptor[] array2 = new ParameterDescriptor[arrayRank + 1];
				for (int j = 0; j < arrayRank; j++)
				{
					array[j] = (array2[j] = new ParameterDescriptor("idx" + j, typeof(int)));
				}
				array2[arrayRank] = new ParameterDescriptor("value", base.Type.GetElementType());
				AddMember("set_Item", new ArrayMemberDescriptor("set_Item", true, array2));
				AddMember("get_Item", new ArrayMemberDescriptor("get_Item", false, array));
			}
			else if (base.Type == typeof(Array))
			{
				AddMember("set_Item", new ArrayMemberDescriptor("set_Item", true));
				AddMember("get_Item", new ArrayMemberDescriptor("get_Item", false));
			}
		}

		public void PrepareForWiring(Table t)
		{
			if (AccessMode == InteropAccessMode.HideMembers || Framework.Do.GetAssembly(base.Type) == Framework.Do.GetAssembly(GetType()))
			{
				t.Set("skip", DynValue.NewBoolean(true));
				return;
			}
			t.Set("visibility", DynValue.NewString(base.Type.GetClrVisibility()));
			t.Set("class", DynValue.NewString(GetType().FullName));
			DynValue dynValue = DynValue.NewPrimeTable();
			t.Set("members", dynValue);
			DynValue dynValue2 = DynValue.NewPrimeTable();
			t.Set("metamembers", dynValue2);
			Serialize(dynValue.Table, base.Members);
			Serialize(dynValue2.Table, base.MetaMembers);
		}

		private void Serialize(Table t, IEnumerable<KeyValuePair<string, IMemberDescriptor>> members)
		{
			foreach (KeyValuePair<string, IMemberDescriptor> member in members)
			{
				IWireableDescriptor wireableDescriptor = member.Value as IWireableDescriptor;
				if (wireableDescriptor != null)
				{
					DynValue dynValue = DynValue.NewPrimeTable();
					t.Set(member.Key, dynValue);
					wireableDescriptor.PrepareForWiring(dynValue.Table);
				}
				else
				{
					t.Set(member.Key, DynValue.NewString("unsupported member type : " + member.Value.GetType().FullName));
				}
			}
		}
	}
}
