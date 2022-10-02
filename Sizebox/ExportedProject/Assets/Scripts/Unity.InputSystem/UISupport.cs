using UnityEngine.InputSystem;

internal static class UISupport
{
	public static void Initialize()
	{
		InputSystem.RegisterLayout("\r\n            {\r\n                \"name\" : \"VirtualMouse\",\r\n                \"extend\" : \"Mouse\"\r\n            }\r\n        ");
	}
}
