public class Toast
{
	public enum Timeout
	{
		Automatic = 0,
		Blink = 1,
		Short = 2,
		Normal = 3,
		Long = 4
	}

	private readonly string _id;

	private ToastInternal _toastInternal;

	public void Print(string message, Timeout timeout = Timeout.Automatic)
	{
		_toastInternal = ToastInternal.TryCreate(_toastInternal, _id, message);
	}

	public Toast(string id = "_default-Toast")
	{
		_id = id;
	}
}
