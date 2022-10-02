public class LuaLazyUpdate : LuaCallBatch
{
	public static LuaLazyUpdate instance;

	private void Awake()
	{
		instance = this;
	}

	private void Update()
	{
		if (!IsEmpty())
		{
			LuaCallFunc();
		}
	}
}
