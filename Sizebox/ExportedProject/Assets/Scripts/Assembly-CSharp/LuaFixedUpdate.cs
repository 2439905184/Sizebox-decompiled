public class LuaFixedUpdate : LuaCallAll
{
	public static LuaFixedUpdate instance;

	private void Awake()
	{
		instance = this;
	}

	private void FixedUpdate()
	{
		if (!IsEmpty())
		{
			LuaCallFunc();
		}
	}
}
