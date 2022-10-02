public class LuaUpdate : LuaCallAll
{
	public static LuaUpdate instance;

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
