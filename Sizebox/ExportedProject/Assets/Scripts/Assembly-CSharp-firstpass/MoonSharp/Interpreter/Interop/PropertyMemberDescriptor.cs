using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using MoonSharp.Interpreter.Compatibility;
using MoonSharp.Interpreter.Diagnostics;
using MoonSharp.Interpreter.Interop.BasicDescriptors;
using MoonSharp.Interpreter.Interop.Converters;

namespace MoonSharp.Interpreter.Interop
{
	public class PropertyMemberDescriptor : IMemberDescriptor, IOptimizableDescriptor, IWireableDescriptor
	{
		private MethodInfo m_Getter;

		private MethodInfo m_Setter;

		private Func<object, object> m_OptimizedGetter;

		private Action<object, object> m_OptimizedSetter;

		public PropertyInfo PropertyInfo { get; private set; }

		public InteropAccessMode AccessMode { get; private set; }

		public bool IsStatic { get; private set; }

		public string Name { get; private set; }

		public bool CanRead
		{
			get
			{
				return m_Getter != null;
			}
		}

		public bool CanWrite
		{
			get
			{
				return m_Setter != null;
			}
		}

		public MemberDescriptorAccess MemberAccess
		{
			get
			{
				MemberDescriptorAccess memberDescriptorAccess = (MemberDescriptorAccess)0;
				if (m_Setter != null)
				{
					memberDescriptorAccess |= MemberDescriptorAccess.CanWrite;
				}
				if (m_Getter != null)
				{
					memberDescriptorAccess |= MemberDescriptorAccess.CanRead;
				}
				return memberDescriptorAccess;
			}
		}

		public static PropertyMemberDescriptor TryCreateIfVisible(PropertyInfo pi, InteropAccessMode accessMode)
		{
			MethodInfo getMethod = Framework.Do.GetGetMethod(pi);
			MethodInfo setMethod = Framework.Do.GetSetMethod(pi);
			bool? visibilityFromAttributes = pi.GetVisibilityFromAttributes();
			bool? visibilityFromAttributes2 = getMethod.GetVisibilityFromAttributes();
			bool? visibilityFromAttributes3 = setMethod.GetVisibilityFromAttributes();
			if (visibilityFromAttributes.HasValue)
			{
				return TryCreate(pi, accessMode, (visibilityFromAttributes2 ?? visibilityFromAttributes.Value) ? getMethod : null, (visibilityFromAttributes3 ?? visibilityFromAttributes.Value) ? setMethod : null);
			}
			return TryCreate(pi, accessMode, (visibilityFromAttributes2 ?? getMethod.IsPublic) ? getMethod : null, (visibilityFromAttributes3 ?? setMethod.IsPublic) ? setMethod : null);
		}

		private static PropertyMemberDescriptor TryCreate(PropertyInfo pi, InteropAccessMode accessMode, MethodInfo getter, MethodInfo setter)
		{
			if (getter == null && setter == null)
			{
				return null;
			}
			return new PropertyMemberDescriptor(pi, accessMode, getter, setter);
		}

		public PropertyMemberDescriptor(PropertyInfo pi, InteropAccessMode accessMode)
			: this(pi, accessMode, Framework.Do.GetGetMethod(pi), Framework.Do.GetSetMethod(pi))
		{
		}

		public PropertyMemberDescriptor(PropertyInfo pi, InteropAccessMode accessMode, MethodInfo getter, MethodInfo setter)
		{
			if (getter == null && setter == null)
			{
				throw new ArgumentNullException("getter and setter cannot both be null");
			}
			if (Script.GlobalOptions.Platform.IsRunningOnAOT())
			{
				accessMode = InteropAccessMode.Reflection;
			}
			PropertyInfo = pi;
			AccessMode = accessMode;
			Name = pi.Name;
			m_Getter = getter;
			m_Setter = setter;
			IsStatic = (m_Getter ?? m_Setter).IsStatic;
			if (AccessMode == InteropAccessMode.Preoptimized)
			{
				OptimizeGetter();
				OptimizeSetter();
			}
		}

		public DynValue GetValue(Script script, object obj)
		{
			this.CheckAccess(MemberDescriptorAccess.CanRead, obj);
			if (m_Getter == null)
			{
				throw new ScriptRuntimeException("userdata property '{0}.{1}' cannot be read from.", PropertyInfo.DeclaringType.Name, Name);
			}
			if (AccessMode == InteropAccessMode.LazyOptimized && m_OptimizedGetter == null)
			{
				OptimizeGetter();
			}
			object obj2 = null;
			obj2 = ((m_OptimizedGetter == null) ? m_Getter.Invoke(IsStatic ? null : obj, null) : m_OptimizedGetter(obj));
			return ClrToScriptConversions.ObjectToDynValue(script, obj2);
		}

		internal void OptimizeGetter()
		{
			using (PerformanceStatistics.StartGlobalStopwatch(PerformanceCounter.AdaptersCompilation))
			{
				if (m_Getter != null)
				{
					if (IsStatic)
					{
						ParameterExpression parameterExpression = Expression.Parameter(typeof(object), "dummy");
						Expression<Func<object, object>> expression = Expression.Lambda<Func<object, object>>(Expression.Convert(Expression.Property(null, PropertyInfo), typeof(object)), new ParameterExpression[1] { parameterExpression });
						Interlocked.Exchange(ref m_OptimizedGetter, expression.Compile());
					}
					else
					{
						ParameterExpression parameterExpression2 = Expression.Parameter(typeof(object), "obj");
						Expression<Func<object, object>> expression2 = Expression.Lambda<Func<object, object>>(Expression.Convert(Expression.Property(Expression.Convert(parameterExpression2, PropertyInfo.DeclaringType), PropertyInfo), typeof(object)), new ParameterExpression[1] { parameterExpression2 });
						Interlocked.Exchange(ref m_OptimizedGetter, expression2.Compile());
					}
				}
			}
		}

		internal void OptimizeSetter()
		{
			using (PerformanceStatistics.StartGlobalStopwatch(PerformanceCounter.AdaptersCompilation))
			{
				if (m_Setter != null && !Framework.Do.IsValueType(PropertyInfo.DeclaringType))
				{
					MethodInfo setMethod = Framework.Do.GetSetMethod(PropertyInfo);
					if (IsStatic)
					{
						ParameterExpression parameterExpression = Expression.Parameter(typeof(object), "dummy");
						ParameterExpression parameterExpression2 = Expression.Parameter(typeof(object), "val");
						UnaryExpression arg = Expression.Convert(parameterExpression2, PropertyInfo.PropertyType);
						Expression<Action<object, object>> expression = Expression.Lambda<Action<object, object>>(Expression.Call(setMethod, arg), new ParameterExpression[2] { parameterExpression, parameterExpression2 });
						Interlocked.Exchange(ref m_OptimizedSetter, expression.Compile());
					}
					else
					{
						ParameterExpression parameterExpression3 = Expression.Parameter(typeof(object), "obj");
						ParameterExpression parameterExpression4 = Expression.Parameter(typeof(object), "val");
						UnaryExpression instance = Expression.Convert(parameterExpression3, PropertyInfo.DeclaringType);
						UnaryExpression unaryExpression = Expression.Convert(parameterExpression4, PropertyInfo.PropertyType);
						Expression<Action<object, object>> expression2 = Expression.Lambda<Action<object, object>>(Expression.Call(instance, setMethod, unaryExpression), new ParameterExpression[2] { parameterExpression3, parameterExpression4 });
						Interlocked.Exchange(ref m_OptimizedSetter, expression2.Compile());
					}
				}
			}
		}

		public void SetValue(Script script, object obj, DynValue v)
		{
			this.CheckAccess(MemberDescriptorAccess.CanWrite, obj);
			if (m_Setter == null)
			{
				throw new ScriptRuntimeException("userdata property '{0}.{1}' cannot be written to.", PropertyInfo.DeclaringType.Name, Name);
			}
			object obj2 = ScriptToClrConversions.DynValueToObjectOfType(v, PropertyInfo.PropertyType, null, false);
			try
			{
				if (obj2 is double)
				{
					obj2 = NumericConversions.DoubleToType(PropertyInfo.PropertyType, (double)obj2);
				}
				if (AccessMode == InteropAccessMode.LazyOptimized && m_OptimizedSetter == null)
				{
					OptimizeSetter();
				}
				if (m_OptimizedSetter != null)
				{
					m_OptimizedSetter(obj, obj2);
					return;
				}
				m_Setter.Invoke(IsStatic ? null : obj, new object[1] { obj2 });
			}
			catch (ArgumentException)
			{
				throw ScriptRuntimeException.UserDataArgumentTypeMismatch(v.Type, PropertyInfo.PropertyType);
			}
			catch (InvalidCastException)
			{
				throw ScriptRuntimeException.UserDataArgumentTypeMismatch(v.Type, PropertyInfo.PropertyType);
			}
		}

		void IOptimizableDescriptor.Optimize()
		{
			OptimizeGetter();
			OptimizeSetter();
		}

		public void PrepareForWiring(Table t)
		{
			t.Set("class", DynValue.NewString(GetType().FullName));
			t.Set("visibility", DynValue.NewString(PropertyInfo.GetClrVisibility()));
			t.Set("name", DynValue.NewString(Name));
			t.Set("static", DynValue.NewBoolean(IsStatic));
			t.Set("read", DynValue.NewBoolean(CanRead));
			t.Set("write", DynValue.NewBoolean(CanWrite));
			t.Set("decltype", DynValue.NewString(PropertyInfo.DeclaringType.FullName));
			t.Set("declvtype", DynValue.NewBoolean(Framework.Do.IsValueType(PropertyInfo.DeclaringType)));
			t.Set("type", DynValue.NewString(PropertyInfo.PropertyType.FullName));
		}
	}
}
