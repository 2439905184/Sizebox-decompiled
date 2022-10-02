using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace UnityEngine.Rendering.PostProcessing
{
	[Serializable]
	public class PostProcessEffectSettings : ScriptableObject
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<FieldInfo, bool> _003C_003E9__3_0;

			public static Func<FieldInfo, int> _003C_003E9__3_1;

			internal bool _003COnEnable_003Eb__3_0(FieldInfo t)
			{
				return t.FieldType.IsSubclassOf(typeof(ParameterOverride));
			}

			internal int _003COnEnable_003Eb__3_1(FieldInfo t)
			{
				return t.MetadataToken;
			}
		}

		public bool active = true;

		public BoolParameter enabled = new BoolParameter
		{
			overrideState = true,
			value = false
		};

		internal ReadOnlyCollection<ParameterOverride> parameters;

		private void OnEnable()
		{
			parameters = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).Where(_003C_003Ec._003C_003E9__3_0 ?? (_003C_003Ec._003C_003E9__3_0 = _003C_003Ec._003C_003E9._003COnEnable_003Eb__3_0)).OrderBy(_003C_003Ec._003C_003E9__3_1 ?? (_003C_003Ec._003C_003E9__3_1 = _003C_003Ec._003C_003E9._003COnEnable_003Eb__3_1))
				.Select(_003COnEnable_003Eb__3_2)
				.ToList()
				.AsReadOnly();
			foreach (ParameterOverride parameter in parameters)
			{
				parameter.OnEnable();
			}
		}

		private void OnDisable()
		{
			if (parameters == null)
			{
				return;
			}
			foreach (ParameterOverride parameter in parameters)
			{
				parameter.OnDisable();
			}
		}

		public void SetAllOverridesTo(bool state, bool excludeEnabled = true)
		{
			foreach (ParameterOverride parameter in parameters)
			{
				if (!excludeEnabled || parameter != enabled)
				{
					parameter.overrideState = state;
				}
			}
		}

		public virtual bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return enabled.value;
		}

		public int GetHash()
		{
			int num = 17;
			foreach (ParameterOverride parameter in parameters)
			{
				num = num * 23 + parameter.GetHash();
			}
			return num;
		}

		[CompilerGenerated]
		private ParameterOverride _003COnEnable_003Eb__3_2(FieldInfo t)
		{
			return (ParameterOverride)t.GetValue(this);
		}
	}
}
