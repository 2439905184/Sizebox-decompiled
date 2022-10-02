using System;

namespace UnityEngine.InputSystem.Layouts
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public sealed class InputControlLayoutAttribute : Attribute
	{
		internal bool? updateBeforeRenderInternal;

		public Type stateType { get; set; }

		public string stateFormat { get; set; }

		public string[] commonUsages { get; set; }

		public string variants { get; set; }

		public bool updateBeforeRender
		{
			get
			{
				return updateBeforeRenderInternal.Value;
			}
			set
			{
				updateBeforeRenderInternal = value;
			}
		}

		public bool isGenericTypeOfDevice { get; set; }

		public string displayName { get; set; }

		public string description { get; set; }

		public bool hideInUI { get; set; }
	}
}
