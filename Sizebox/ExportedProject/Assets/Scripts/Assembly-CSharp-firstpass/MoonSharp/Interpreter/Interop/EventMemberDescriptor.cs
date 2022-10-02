using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using MoonSharp.Interpreter.Compatibility;
using MoonSharp.Interpreter.DataStructs;
using MoonSharp.Interpreter.Interop.BasicDescriptors;
using MoonSharp.Interpreter.Interop.StandardDescriptors;

namespace MoonSharp.Interpreter.Interop
{
	public class EventMemberDescriptor : IMemberDescriptor
	{
		private delegate void EventWrapper00();

		private delegate void EventWrapper01(object o1);

		private delegate void EventWrapper02(object o1, object o2);

		private delegate void EventWrapper03(object o1, object o2, object o3);

		private delegate void EventWrapper04(object o1, object o2, object o3, object o4);

		private delegate void EventWrapper05(object o1, object o2, object o3, object o4, object o5);

		private delegate void EventWrapper06(object o1, object o2, object o3, object o4, object o5, object o6);

		private delegate void EventWrapper07(object o1, object o2, object o3, object o4, object o5, object o6, object o7);

		private delegate void EventWrapper08(object o1, object o2, object o3, object o4, object o5, object o6, object o7, object o8);

		private delegate void EventWrapper09(object o1, object o2, object o3, object o4, object o5, object o6, object o7, object o8, object o9);

		private delegate void EventWrapper10(object o1, object o2, object o3, object o4, object o5, object o6, object o7, object o8, object o9, object o10);

		private delegate void EventWrapper11(object o1, object o2, object o3, object o4, object o5, object o6, object o7, object o8, object o9, object o10, object o11);

		private delegate void EventWrapper12(object o1, object o2, object o3, object o4, object o5, object o6, object o7, object o8, object o9, object o10, object o11, object o12);

		private delegate void EventWrapper13(object o1, object o2, object o3, object o4, object o5, object o6, object o7, object o8, object o9, object o10, object o11, object o12, object o13);

		private delegate void EventWrapper14(object o1, object o2, object o3, object o4, object o5, object o6, object o7, object o8, object o9, object o10, object o11, object o12, object o13, object o14);

		private delegate void EventWrapper15(object o1, object o2, object o3, object o4, object o5, object o6, object o7, object o8, object o9, object o10, object o11, object o12, object o13, object o14, object o15);

		private delegate void EventWrapper16(object o1, object o2, object o3, object o4, object o5, object o6, object o7, object o8, object o9, object o10, object o11, object o12, object o13, object o14, object o15, object o16);

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass20_0
		{
			public EventMemberDescriptor _003C_003E4__this;

			public object o;

			internal Delegate _003CRegisterCallback_003Eb__0()
			{
				Delegate @delegate = _003C_003E4__this.CreateDelegate(o);
				Delegate delegate2 = Delegate.CreateDelegate(_003C_003E4__this.EventInfo.EventHandlerType, @delegate.Target, @delegate.Method);
				_003C_003E4__this.m_Add.Invoke(o, new object[1] { delegate2 });
				return delegate2;
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass22_0
		{
			public EventMemberDescriptor _003C_003E4__this;

			public object sender;

			internal void _003CCreateDelegate_003Eb__0()
			{
				_003C_003E4__this.DispatchEvent(sender);
			}

			internal void _003CCreateDelegate_003Eb__1(object o1)
			{
				_003C_003E4__this.DispatchEvent(sender, o1);
			}

			internal void _003CCreateDelegate_003Eb__2(object o1, object o2)
			{
				_003C_003E4__this.DispatchEvent(sender, o1, o2);
			}

			internal void _003CCreateDelegate_003Eb__3(object o1, object o2, object o3)
			{
				_003C_003E4__this.DispatchEvent(sender, o1, o2, o3);
			}

			internal void _003CCreateDelegate_003Eb__4(object o1, object o2, object o3, object o4)
			{
				_003C_003E4__this.DispatchEvent(sender, o1, o2, o3, o4);
			}

			internal void _003CCreateDelegate_003Eb__5(object o1, object o2, object o3, object o4, object o5)
			{
				_003C_003E4__this.DispatchEvent(sender, o1, o2, o3, o4, o5);
			}

			internal void _003CCreateDelegate_003Eb__6(object o1, object o2, object o3, object o4, object o5, object o6)
			{
				_003C_003E4__this.DispatchEvent(sender, o1, o2, o3, o4, o5, o6);
			}

			internal void _003CCreateDelegate_003Eb__7(object o1, object o2, object o3, object o4, object o5, object o6, object o7)
			{
				_003C_003E4__this.DispatchEvent(sender, o1, o2, o3, o4, o5, o6, o7);
			}

			internal void _003CCreateDelegate_003Eb__8(object o1, object o2, object o3, object o4, object o5, object o6, object o7, object o8)
			{
				_003C_003E4__this.DispatchEvent(sender, o1, o2, o3, o4, o5, o6, o7, o8);
			}

			internal void _003CCreateDelegate_003Eb__9(object o1, object o2, object o3, object o4, object o5, object o6, object o7, object o8, object o9)
			{
				_003C_003E4__this.DispatchEvent(sender, o1, o2, o3, o4, o5, o6, o7, o8, o9);
			}

			internal void _003CCreateDelegate_003Eb__10(object o1, object o2, object o3, object o4, object o5, object o6, object o7, object o8, object o9, object o10)
			{
				_003C_003E4__this.DispatchEvent(sender, o1, o2, o3, o4, o5, o6, o7, o8, o9, o10);
			}

			internal void _003CCreateDelegate_003Eb__11(object o1, object o2, object o3, object o4, object o5, object o6, object o7, object o8, object o9, object o10, object o11)
			{
				_003C_003E4__this.DispatchEvent(sender, o1, o2, o3, o4, o5, o6, o7, o8, o9, o10, o11);
			}

			internal void _003CCreateDelegate_003Eb__12(object o1, object o2, object o3, object o4, object o5, object o6, object o7, object o8, object o9, object o10, object o11, object o12)
			{
				_003C_003E4__this.DispatchEvent(sender, o1, o2, o3, o4, o5, o6, o7, o8, o9, o10, o11, o12);
			}

			internal void _003CCreateDelegate_003Eb__13(object o1, object o2, object o3, object o4, object o5, object o6, object o7, object o8, object o9, object o10, object o11, object o12, object o13)
			{
				_003C_003E4__this.DispatchEvent(sender, o1, o2, o3, o4, o5, o6, o7, o8, o9, o10, o11, o12, o13);
			}

			internal void _003CCreateDelegate_003Eb__14(object o1, object o2, object o3, object o4, object o5, object o6, object o7, object o8, object o9, object o10, object o11, object o12, object o13, object o14)
			{
				_003C_003E4__this.DispatchEvent(sender, o1, o2, o3, o4, o5, o6, o7, o8, o9, o10, o11, o12, o13, o14);
			}

			internal void _003CCreateDelegate_003Eb__15(object o1, object o2, object o3, object o4, object o5, object o6, object o7, object o8, object o9, object o10, object o11, object o12, object o13, object o14, object o15)
			{
				_003C_003E4__this.DispatchEvent(sender, o1, o2, o3, o4, o5, o6, o7, o8, o9, o10, o11, o12, o13, o14, o15);
			}

			internal void _003CCreateDelegate_003Eb__16(object o1, object o2, object o3, object o4, object o5, object o6, object o7, object o8, object o9, object o10, object o11, object o12, object o13, object o14, object o15, object o16)
			{
				_003C_003E4__this.DispatchEvent(sender, o1, o2, o3, o4, o5, o6, o7, o8, o9, o10, o11, o12, o13, o14, o15, o16);
			}
		}

		public const int MAX_ARGS_IN_DELEGATE = 16;

		private object m_Lock = new object();

		private MultiDictionary<object, Closure> m_Callbacks = new MultiDictionary<object, Closure>(new ReferenceEqualityComparer());

		private Dictionary<object, Delegate> m_Delegates = new Dictionary<object, Delegate>(new ReferenceEqualityComparer());

		private MethodInfo m_Add;

		private MethodInfo m_Remove;

		public EventInfo EventInfo { get; private set; }

		public bool IsStatic { get; private set; }

		public string Name
		{
			get
			{
				return EventInfo.Name;
			}
		}

		public MemberDescriptorAccess MemberAccess
		{
			get
			{
				return MemberDescriptorAccess.CanRead;
			}
		}

		public static EventMemberDescriptor TryCreateIfVisible(EventInfo ei, InteropAccessMode accessMode)
		{
			if (!CheckEventIsCompatible(ei, false))
			{
				return null;
			}
			MethodInfo addMethod = Framework.Do.GetAddMethod(ei);
			MethodInfo removeMethod = Framework.Do.GetRemoveMethod(ei);
			bool? visibilityFromAttributes = ei.GetVisibilityFromAttributes();
			bool num;
			if (!visibilityFromAttributes.HasValue)
			{
				if (!(removeMethod != null) || !removeMethod.IsPublic || !(addMethod != null))
				{
					goto IL_006c;
				}
				num = addMethod.IsPublic;
			}
			else
			{
				num = visibilityFromAttributes.GetValueOrDefault();
			}
			if (num)
			{
				return new EventMemberDescriptor(ei, accessMode);
			}
			goto IL_006c;
			IL_006c:
			return null;
		}

		public static bool CheckEventIsCompatible(EventInfo ei, bool throwException)
		{
			if (Framework.Do.IsValueType(ei.DeclaringType))
			{
				if (throwException)
				{
					throw new ArgumentException("Events are not supported on value types");
				}
				return false;
			}
			if (Framework.Do.GetAddMethod(ei) == null || Framework.Do.GetRemoveMethod(ei) == null)
			{
				if (throwException)
				{
					throw new ArgumentException("Event must have add and remove methods");
				}
				return false;
			}
			MethodInfo method = Framework.Do.GetMethod(ei.EventHandlerType, "Invoke");
			if (method == null)
			{
				if (throwException)
				{
					throw new ArgumentException("Event handler type doesn't seem to be a delegate");
				}
				return false;
			}
			if (!MethodMemberDescriptor.CheckMethodIsCompatible(method, throwException))
			{
				return false;
			}
			if (method.ReturnType != typeof(void))
			{
				if (throwException)
				{
					throw new ArgumentException("Event handler cannot have a return type");
				}
				return false;
			}
			ParameterInfo[] parameters = method.GetParameters();
			if (parameters.Length > 16)
			{
				if (throwException)
				{
					throw new ArgumentException(string.Format("Event handler cannot have more than {0} parameters", 16));
				}
				return false;
			}
			ParameterInfo[] array = parameters;
			foreach (ParameterInfo parameterInfo in array)
			{
				if (Framework.Do.IsValueType(parameterInfo.ParameterType))
				{
					if (throwException)
					{
						throw new ArgumentException("Event handler cannot have value type parameters");
					}
					return false;
				}
				if (parameterInfo.ParameterType.IsByRef)
				{
					if (throwException)
					{
						throw new ArgumentException("Event handler cannot have by-ref type parameters");
					}
					return false;
				}
			}
			return true;
		}

		public EventMemberDescriptor(EventInfo ei, InteropAccessMode accessMode = InteropAccessMode.Default)
		{
			CheckEventIsCompatible(ei, true);
			EventInfo = ei;
			m_Add = Framework.Do.GetAddMethod(ei);
			m_Remove = Framework.Do.GetRemoveMethod(ei);
			IsStatic = m_Add.IsStatic;
		}

		public DynValue GetValue(Script script, object obj)
		{
			this.CheckAccess(MemberDescriptorAccess.CanRead, obj);
			if (IsStatic)
			{
				obj = this;
			}
			return UserData.Create(new EventFacade(this, obj));
		}

		internal DynValue AddCallback(object o, ScriptExecutionContext context, CallbackArguments args)
		{
			lock (m_Lock)
			{
				Closure function = args.AsType(0, string.Format("userdata<{0}>.{1}.add", EventInfo.DeclaringType, EventInfo.Name), DataType.Function).Function;
				if (m_Callbacks.Add(o, function))
				{
					RegisterCallback(o);
				}
				return DynValue.Void;
			}
		}

		internal DynValue RemoveCallback(object o, ScriptExecutionContext context, CallbackArguments args)
		{
			lock (m_Lock)
			{
				Closure function = args.AsType(0, string.Format("userdata<{0}>.{1}.remove", EventInfo.DeclaringType, EventInfo.Name), DataType.Function).Function;
				if (m_Callbacks.RemoveValue(o, function))
				{
					UnregisterCallback(o);
				}
				return DynValue.Void;
			}
		}

		private void RegisterCallback(object o)
		{
			_003C_003Ec__DisplayClass20_0 _003C_003Ec__DisplayClass20_ = new _003C_003Ec__DisplayClass20_0();
			_003C_003Ec__DisplayClass20_._003C_003E4__this = this;
			_003C_003Ec__DisplayClass20_.o = o;
			m_Delegates.GetOrCreate(_003C_003Ec__DisplayClass20_.o, _003C_003Ec__DisplayClass20_._003CRegisterCallback_003Eb__0);
		}

		private void UnregisterCallback(object o)
		{
			Delegate orDefault = m_Delegates.GetOrDefault(o);
			if ((object)orDefault == null)
			{
				throw new InternalErrorException("can't unregister null delegate");
			}
			m_Delegates.Remove(o);
			m_Remove.Invoke(o, new object[1] { orDefault });
		}

		private Delegate CreateDelegate(object sender)
		{
			_003C_003Ec__DisplayClass22_0 _003C_003Ec__DisplayClass22_ = new _003C_003Ec__DisplayClass22_0();
			_003C_003Ec__DisplayClass22_._003C_003E4__this = this;
			_003C_003Ec__DisplayClass22_.sender = sender;
			switch (Framework.Do.GetMethod(EventInfo.EventHandlerType, "Invoke").GetParameters().Length)
			{
			case 0:
				return new EventWrapper00(_003C_003Ec__DisplayClass22_._003CCreateDelegate_003Eb__0);
			case 1:
				return new EventWrapper01(_003C_003Ec__DisplayClass22_._003CCreateDelegate_003Eb__1);
			case 2:
				return new EventWrapper02(_003C_003Ec__DisplayClass22_._003CCreateDelegate_003Eb__2);
			case 3:
				return new EventWrapper03(_003C_003Ec__DisplayClass22_._003CCreateDelegate_003Eb__3);
			case 4:
				return new EventWrapper04(_003C_003Ec__DisplayClass22_._003CCreateDelegate_003Eb__4);
			case 5:
				return new EventWrapper05(_003C_003Ec__DisplayClass22_._003CCreateDelegate_003Eb__5);
			case 6:
				return new EventWrapper06(_003C_003Ec__DisplayClass22_._003CCreateDelegate_003Eb__6);
			case 7:
				return new EventWrapper07(_003C_003Ec__DisplayClass22_._003CCreateDelegate_003Eb__7);
			case 8:
				return new EventWrapper08(_003C_003Ec__DisplayClass22_._003CCreateDelegate_003Eb__8);
			case 9:
				return new EventWrapper09(_003C_003Ec__DisplayClass22_._003CCreateDelegate_003Eb__9);
			case 10:
				return new EventWrapper10(_003C_003Ec__DisplayClass22_._003CCreateDelegate_003Eb__10);
			case 11:
				return new EventWrapper11(_003C_003Ec__DisplayClass22_._003CCreateDelegate_003Eb__11);
			case 12:
				return new EventWrapper12(_003C_003Ec__DisplayClass22_._003CCreateDelegate_003Eb__12);
			case 13:
				return new EventWrapper13(_003C_003Ec__DisplayClass22_._003CCreateDelegate_003Eb__13);
			case 14:
				return new EventWrapper14(_003C_003Ec__DisplayClass22_._003CCreateDelegate_003Eb__14);
			case 15:
				return new EventWrapper15(_003C_003Ec__DisplayClass22_._003CCreateDelegate_003Eb__15);
			case 16:
				return new EventWrapper16(_003C_003Ec__DisplayClass22_._003CCreateDelegate_003Eb__16);
			default:
				throw new InternalErrorException("too many args in delegate type");
			}
		}

		private void DispatchEvent(object sender, object o01 = null, object o02 = null, object o03 = null, object o04 = null, object o05 = null, object o06 = null, object o07 = null, object o08 = null, object o09 = null, object o10 = null, object o11 = null, object o12 = null, object o13 = null, object o14 = null, object o15 = null, object o16 = null)
		{
			Closure[] array = null;
			lock (m_Lock)
			{
				array = m_Callbacks.Find(sender).ToArray();
			}
			Closure[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].Call(o01, o02, o03, o04, o05, o06, o07, o08, o09, o10, o11, o12, o13, o14, o15, o16);
			}
		}

		public void SetValue(Script script, object obj, DynValue v)
		{
			this.CheckAccess(MemberDescriptorAccess.CanWrite, obj);
		}
	}
}
