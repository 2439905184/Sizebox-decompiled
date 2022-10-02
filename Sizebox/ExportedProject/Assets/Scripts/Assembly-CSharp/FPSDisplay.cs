using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
	private float deltaTime;

	private string cFormat = "{0:0.0} ms ({1:0.} fps)";

	private int w;

	private int h;

	private Rect rect;

	private GUIStyle style;

	private string text;

	private void RefreshScreen()
	{
		w = Screen.width;
		h = Screen.height;
		style = new GUIStyle();
		rect = new Rect(0f, 0f, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 100;
		style.normal.textColor = new Color(0f, 0f, 0.5f, 1f);
	}

	private void Start()
	{
		RefreshScreen();
	}

	private void Update()
	{
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
		float num = deltaTime * 1000f;
		float num2 = 1f / deltaTime;
		text = string.Format(cFormat, num, num2);
	}

	private void OnGUI()
	{
		GUI.Label(rect, text, style);
	}
}
