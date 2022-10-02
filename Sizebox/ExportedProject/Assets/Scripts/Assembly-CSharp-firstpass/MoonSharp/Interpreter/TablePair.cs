namespace MoonSharp.Interpreter
{
	public struct TablePair
	{
		private static TablePair s_NilNode = new TablePair(DynValue.Nil, DynValue.Nil);

		private DynValue key;

		private DynValue value;

		public DynValue Key
		{
			get
			{
				return key;
			}
			private set
			{
				Key = key;
			}
		}

		public DynValue Value
		{
			get
			{
				return value;
			}
			set
			{
				if (key.IsNotNil())
				{
					Value = value;
				}
			}
		}

		public static TablePair Nil
		{
			get
			{
				return s_NilNode;
			}
		}

		public TablePair(DynValue key, DynValue val)
		{
			this.key = key;
			value = val;
		}
	}
}
