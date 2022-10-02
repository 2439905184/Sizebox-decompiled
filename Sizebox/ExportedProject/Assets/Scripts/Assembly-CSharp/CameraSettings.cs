using UnityEngine;

public class CameraSettings : MonoBehaviour
{
	public Camera primaryCamera;

	protected virtual void Awake()
	{
		Application.targetFrameRate = GlobalPreferences.Fps.value;
		SetMultiSampleAntiAliasing(GlobalPreferences.Msaa.value);
		QualitySettings.vSyncCount = GlobalPreferences.Vsync.value;
	}

	public virtual void SetMultiSampleAntiAliasing(int v)
	{
		switch (v)
		{
		default:
			QualitySettings.antiAliasing = 0;
			break;
		case 1:
			QualitySettings.antiAliasing = 2;
			break;
		case 2:
			QualitySettings.antiAliasing = 4;
			break;
		case 3:
			QualitySettings.antiAliasing = 8;
			break;
		}
		primaryCamera.allowMSAA = QualitySettings.antiAliasing > 0;
	}

	public static void SetUiScale(Canvas canvas, float multiply = 1f)
	{
		canvas.scaleFactor = SetupCanvasScale(multiply);
	}

	public static void SetUiScale(float multiply = 1f)
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("MainCanvas");
		if ((bool)gameObject)
		{
			Canvas component = gameObject.GetComponent<Canvas>();
			if ((bool)component)
			{
				SetUiScale(component, multiply);
			}
		}
	}

	private static float SetupCanvasScale(float multiply)
	{
		return (float)Screen.height / 600f * multiply;
	}
}
