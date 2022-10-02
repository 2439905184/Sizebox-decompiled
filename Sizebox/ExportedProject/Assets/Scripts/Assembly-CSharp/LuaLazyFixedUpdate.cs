public class LuaLazyFixedUpdate : LuaCallBatch
{
	public static LuaLazyFixedUpdate instance;

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
