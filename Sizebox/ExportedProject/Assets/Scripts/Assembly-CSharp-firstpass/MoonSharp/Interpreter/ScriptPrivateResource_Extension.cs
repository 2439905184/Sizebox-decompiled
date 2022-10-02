namespace MoonSharp.Interpreter
{
	internal static class ScriptPrivateResource_Extension
	{
		public static void CheckScriptOwnership(this IScriptPrivateResource containingResource, DynValue[] values)
		{
			foreach (DynValue value in values)
			{
				containingResource.CheckScriptOwnership(value);
			}
		}

		public static void CheckScriptOwnership(this IScriptPrivateResource containingResource, DynValue value)
		{
			if (value != null)
			{
				IScriptPrivateResource asPrivateResource = value.GetAsPrivateResource();
				if (asPrivateResource != null)
				{
					containingResource.CheckScriptOwnership(asPrivateResource);
				}
			}
		}

		public static void CheckScriptOwnership(this IScriptPrivateResource resource, Script script)
		{
			if (resource.OwnerScript != null && resource.OwnerScript != script && script != null)
			{
				throw new ScriptRuntimeException("Attempt to access a resource owned by a script, from another script");
			}
		}

		public static void CheckScriptOwnership(this IScriptPrivateResource containingResource, IScriptPrivateResource itemResource)
		{
			if (itemResource != null)
			{
				if (containingResource.OwnerScript != null && containingResource.OwnerScript != itemResource.OwnerScript && itemResource.OwnerScript != null)
				{
					throw new ScriptRuntimeException("Attempt to perform operations with resources owned by different scripts.");
				}
				if (containingResource.OwnerScript == null && itemResource.OwnerScript != null)
				{
					throw new ScriptRuntimeException("Attempt to perform operations with a script private resource on a shared resource.");
				}
			}
		}
	}
}
