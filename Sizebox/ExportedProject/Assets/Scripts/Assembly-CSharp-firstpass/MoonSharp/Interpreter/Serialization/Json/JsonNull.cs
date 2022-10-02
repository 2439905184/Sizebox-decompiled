namespace MoonSharp.Interpreter.Serialization.Json
{
	public sealed class JsonNull
	{
		public static bool isNull()
		{
			return true;
		}

		[MoonSharpHidden]
		public static bool IsJsonNull(DynValue v)
		{
			if (v.Type == DataType.UserData && v.UserData.Descriptor != null)
			{
				return v.UserData.Descriptor.Type == typeof(JsonNull);
			}
			return false;
		}

		[MoonSharpHidden]
		public static DynValue Create()
		{
			return UserData.CreateStatic<JsonNull>();
		}
	}
}
