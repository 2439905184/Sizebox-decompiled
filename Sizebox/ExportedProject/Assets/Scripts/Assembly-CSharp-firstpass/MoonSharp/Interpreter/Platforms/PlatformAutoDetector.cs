using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using MoonSharp.Interpreter.Interop;
using MoonSharp.Interpreter.Loaders;

namespace MoonSharp.Interpreter.Platforms
{
	public static class PlatformAutoDetector
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<Assembly, IEnumerable<Type>> _003C_003E9__28_0;

			public static Func<Type, bool> _003C_003E9__28_1;

			internal IEnumerable<Type> _003CAutoDetectPlatformFlags_003Eb__28_0(Assembly a)
			{
				return a.SafeGetTypes();
			}

			internal bool _003CAutoDetectPlatformFlags_003Eb__28_1(Type t)
			{
				return t.FullName.StartsWith("UnityEngine.");
			}
		}

		private static bool? m_IsRunningOnAOT;

		private static bool m_AutoDetectionsDone;

		public static bool IsRunningOnMono { get; private set; }

		public static bool IsRunningOnClr4 { get; private set; }

		public static bool IsRunningOnUnity { get; private set; }

		public static bool IsPortableFramework { get; private set; }

		public static bool IsUnityNative { get; private set; }

		public static bool IsUnityIL2CPP { get; private set; }

		public static bool IsRunningOnAOT
		{
			get
			{
				if (!m_IsRunningOnAOT.HasValue)
				{
					try
					{
						((Expression<Func<int>>)(() => 5)).Compile();
						m_IsRunningOnAOT = false;
					}
					catch (Exception)
					{
						m_IsRunningOnAOT = true;
					}
				}
				return m_IsRunningOnAOT.Value;
			}
		}

		private static void AutoDetectPlatformFlags()
		{
			if (!m_AutoDetectionsDone)
			{
				IsRunningOnUnity = AppDomain.CurrentDomain.GetAssemblies().SelectMany(_003C_003Ec._003C_003E9__28_0 ?? (_003C_003Ec._003C_003E9__28_0 = _003C_003Ec._003C_003E9._003CAutoDetectPlatformFlags_003Eb__28_0)).Any(_003C_003Ec._003C_003E9__28_1 ?? (_003C_003Ec._003C_003E9__28_1 = _003C_003Ec._003C_003E9._003CAutoDetectPlatformFlags_003Eb__28_1));
				IsRunningOnMono = Type.GetType("Mono.Runtime") != null;
				IsRunningOnClr4 = Type.GetType("System.Lazy`1") != null;
				m_AutoDetectionsDone = true;
			}
		}

		internal static IPlatformAccessor GetDefaultPlatform()
		{
			AutoDetectPlatformFlags();
			if (IsRunningOnUnity)
			{
				return new LimitedPlatformAccessor();
			}
			return new StandardPlatformAccessor();
		}

		internal static IScriptLoader GetDefaultScriptLoader()
		{
			AutoDetectPlatformFlags();
			if (IsRunningOnUnity)
			{
				return new UnityAssetsScriptLoader();
			}
			return new FileSystemScriptLoader();
		}
	}
}
