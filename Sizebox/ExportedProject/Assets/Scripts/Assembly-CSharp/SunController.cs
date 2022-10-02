using UnityEngine;

internal class SunController : MonoBehaviour
{
	[Range(5f, 40000f)]
	public float Height = 5f;

	public Transform DirLightTransform;

	public bool ShowHelp = true;

	private Vector3 prevMousePos;

	public void Start()
	{
		prevMousePos = Input.mousePosition;
		Height = base.transform.position.y;
	}

	public void Update()
	{
		Vector3 position = base.transform.position;
		position.y = Height;
		base.transform.position = position;
		if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
		{
			prevMousePos = Input.mousePosition;
		}
		Vector3 mousePosition = Input.mousePosition;
		Vector3 vector = mousePosition - prevMousePos;
		prevMousePos = mousePosition;
		if (Input.GetMouseButton(0))
		{
			DirLightTransform.Rotate(0f, vector.x * 0.1f, 0f, Space.World);
			DirLightTransform.Rotate(vector.y * 0.1f, 0f, 0f, Space.Self);
		}
		if (Input.GetMouseButton(1))
		{
			base.transform.Rotate(0f, vector.x * 0.1f, 0f, Space.World);
			base.transform.Rotate((0f - vector.y) * 0.1f, 0f, 0f, Space.Self);
		}
		if (Input.GetKey(KeyCode.A))
		{
			Height += 2000f * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.Z))
		{
			Height -= 2000f * Time.deltaTime;
		}
		if (Input.GetKeyDown(KeyCode.BackQuote))
		{
			ShowHelp = !ShowHelp;
		}
	}

	private void OnGUI()
	{
		if (ShowHelp)
		{
			GUILayout.Label("~ - Toggle help");
			GUILayout.BeginHorizontal();
			GUILayout.Label("Camera Height", GUILayout.ExpandWidth(false));
			Height = GUILayout.HorizontalSlider(Height, 10f, 40000f, GUILayout.Width(400f));
			GUILayout.EndHorizontal();
			GUILayout.Label("LMB - Rotate Sun");
			GUILayout.Label("RMB - Rotate Camera");
			GUILayout.Label("A/Z - Move Camera Up/Down");
		}
	}
}
