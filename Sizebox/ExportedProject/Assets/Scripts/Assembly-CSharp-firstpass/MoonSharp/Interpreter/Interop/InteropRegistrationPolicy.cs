using System;
using MoonSharp.Interpreter.Interop.RegistrationPolicies;

namespace MoonSharp.Interpreter.Interop
{
	public static class InteropRegistrationPolicy
	{
		public static IRegistrationPolicy Default
		{
			get
			{
				return new DefaultRegistrationPolicy();
			}
		}

		[Obsolete("Please use InteropRegistrationPolicy.Default instead.")]
		public static IRegistrationPolicy Explicit
		{
			get
			{
				return new DefaultRegistrationPolicy();
			}
		}

		public static IRegistrationPolicy Automatic
		{
			get
			{
				return new AutomaticRegistrationPolicy();
			}
		}
	}
}
