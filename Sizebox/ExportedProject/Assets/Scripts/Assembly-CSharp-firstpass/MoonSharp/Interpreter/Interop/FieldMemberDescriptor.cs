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
	public class FieldMemberDescriptor : IMemberDescriptor, IOptimizableDescriptor, IWireableDescriptor
	{
		private object m_ConstValue;

		private Func<object, object> m_OptimizedGetter;

		public FieldInfo FieldInfo { get; private set; }

		public InteropAccessMode AccessMode { get; private set; }

		public bool IsStatic { get; private set; }

		public string Name { get; private set; }

		public bool IsConst { get; private set; }

		public bool IsReadonly { get; private set; }

		public MemberDescriptorAccess MemberAccess
		{
			get
			{
				if (IsReadonly || IsConst)
				{
					return MemberDescriptorAccess.CanRead;
				}
				return MemberDescriptorAccess.CanRead | MemberDescriptorAccess.CanWrite;
			}
		}

		public static FieldMemberDescriptor TryCreateIfVisible(FieldInfo fi, InteropAccessMode accessMode)
		{
			if (fi.GetVisibilityFromAttributes() ?? fi.IsPublic)
			{
				return new FieldMemberDescriptor(fi, accessMode);
			}
			return null;
		}

		public FieldMemberDescriptor(FieldInfo fi, InteropAccessMode accessMode)
		{
			if (Script.GlobalOptions.Platform.IsRunningOnAOT())
			{
				accessMode = InteropAccessMode.Reflection;
			}
			FieldInfo = fi;
			AccessMode = accessMode;
			Name = fi.Name;
			IsStatic = FieldInfo.IsStatic;
			if (FieldInfo.IsLiteral)
			{
				IsConst = true;
				m_ConstValue = FieldInfo.GetValue(null);
			}
			else
			{
				IsReadonly = FieldInfo.IsInitOnly;
			}
			if (AccessMode == InteropAccessMode.Preoptimized)
			{
				OptimizeGetter();
			}
		}

		public DynValue GetValue(Script script, object obj)
		{
			this.CheckAccess(MemberDescriptorAccess.CanRead, obj);
			if (IsConst)
			{
				return ClrToScriptConversions.ObjectToDynValue(script, m_ConstValue);
			}
			if (AccessMode == InteropAccessMode.LazyOptimized && m_OptimizedGetter == null)
			{
				OptimizeGetter();
			}
			object obj2 = null;
			obj2 = ((m_OptimizedGetter == null) ? FieldInfo.GetValue(obj) : m_OptimizedGetter(obj));
			return ClrToScriptConversions.ObjectToDynValue(script, obj2);
		}

		internal void OptimizeGetter()
		{
			if (IsConst)
			{
				return;
			}
			using (PerformanceStatistics.StartGlobalStopwatch(PerformanceCounter.AdaptersCompilation))
			{
				if (IsStatic)
				{
					ParameterExpression parameterExpression = Expression.Parameter(typeof(object), "dummy");
					Expression<Func<object, object>> expression = Expression.Lambda<Func<object, object>>(Expression.Convert(Expression.Field(null, FieldInfo), typeof(object)), new ParameterExpression[1] { parameterExpression });
					Interlocked.Exchange(ref m_OptimizedGetter, expression.Compile());
				}
				else
				{
					ParameterExpression parameterExpression2 = Expression.Parameter(typeof(object), "obj");
					Expression<Func<object, object>> expression2 = Expression.Lambda<Func<object, object>>(Expression.Convert(Expression.Field(Expression.Convert(parameterExpression2, FieldInfo.DeclaringType), FieldInfo), typeof(object)), new ParameterExpression[1] { parameterExpression2 });
					Interlocked.Exchange(ref m_OptimizedGetter, expression2.Compile());
				}
			}
		}

		public void SetValue(Script script, object obj, DynValue v)
		{
			this.CheckAccess(MemberDescriptorAccess.CanWrite, obj);
			if (IsReadonly || IsConst)
			{
				throw new ScriptRuntimeException("userdata field '{0}.{1}' cannot be written to.", FieldInfo.DeclaringType.Name, Name);
			}
			object obj2 = ScriptToClrConversions.DynValueToObjectOfType(v, FieldInfo.FieldType, null, false);
			try
			{
				if (obj2 is double)
				{
					obj2 = NumericConversions.DoubleToType(FieldInfo.FieldType, (double)obj2);
				}
				FieldInfo.SetValue(IsStatic ? null : obj, obj2);
			}
			catch (ArgumentException)
			{
				throw ScriptRuntimeException.UserDataArgumentTypeMismatch(v.Type, FieldInfo.FieldType);
			}
			catch (InvalidCastException)
			{
				throw ScriptRuntimeException.UserDataArgumentTypeMismatch(v.Type, FieldInfo.FieldType);
			}
			catch (FieldAccessException ex3)
			{
				throw new ScriptRuntimeException(ex3);
			}
		}

		void IOptimizableDescriptor.Optimize()
		{
			if (m_OptimizedGetter == null)
			{
				OptimizeGetter();
			}
		}

		public void PrepareForWiring(Table t)
		{
			t.Set("class", DynValue.NewString(GetType().FullName));
			t.Set("visibility", DynValue.NewString(FieldInfo.GetClrVisibility()));
			t.Set("name", DynValue.NewString(Name));
			t.Set("static", DynValue.NewBoolean(IsStatic));
			t.Set("const", DynValue.NewBoolean(IsConst));
			t.Set("readonly", DynValue.NewBoolean(IsReadonly));
			t.Set("decltype", DynValue.NewString(FieldInfo.DeclaringType.FullName));
			t.Set("declvtype", DynValue.NewBoolean(Framework.Do.IsValueType(FieldInfo.DeclaringType)));
			t.Set("type", DynValue.NewString(FieldInfo.FieldType.FullName));
			t.Set("read", DynValue.NewBoolean(true));
			t.Set("write", DynValue.NewBoolean(!IsConst && !IsReadonly));
		}
	}
}
