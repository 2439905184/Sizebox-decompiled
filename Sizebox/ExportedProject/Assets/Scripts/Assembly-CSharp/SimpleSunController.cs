using UnityEngine;

public class SimpleSunController : MonoBehaviour
{
	private static Vector3 _sunDirection = Vector3.zero;

	private static float _dayTimeScale = 0.1f;

	private static float _sunX;

	private static float _sunY;

	private static Light _sun;

	private static float _hue;

	private static float _saturation;

	private static float _sunIntensity;

	public static Vector3 SunDirection
	{
		get
		{
			return _sunDirection;
		}
	}

	public static bool UseRealtimeDayAndNight { get; set; }

	public static float DayTimeScale
	{
		get
		{
			return _dayTimeScale;
		}
		set
		{
			_dayTimeScale = value;
		}
	}

	public static float SunX
	{
		get
		{
			return _sunX;
		}
		set
		{
			_sunX = value;
			UpdateSunRotation();
		}
	}

	public static float SunY
	{
		get
		{
			return _sunY;
		}
		set
		{
			_sunY = value;
			UpdateSunRotation();
		}
	}

	public static float SunIntensity
	{
		get
		{
			return _sunIntensity;
		}
		set
		{
			_sun.color = Color.HSVToRGB(_hue, _saturation, value, true);
			_sunIntensity = value;
		}
	}

	public static Light Sun
	{
		get
		{
			return _sun;
		}
		set
		{
			_sun = value;
		}
	}

	public static GameObject SunGameObject
	{
		get
		{
			return _sun.gameObject;
		}
	}

	private void Start()
	{
		if (RenderSettings.sun == null)
		{
			Light[] array = Object.FindObjectsOfType<Light>();
			foreach (Light light in array)
			{
				if (light.type == LightType.Directional)
				{
					_sun = light;
					break;
				}
			}
			if (!_sun)
			{
				Object.Destroy(this);
			}
		}
		else
		{
			_sun = RenderSettings.sun;
		}
		Vector3 localEulerAngles = _sun.transform.localEulerAngles;
		_sunX = localEulerAngles.x;
		_sunY = localEulerAngles.y;
		Color.RGBToHSV(_sun.color, out _hue, out _saturation, out _sunIntensity);
	}

	private void Update()
	{
		if (UseRealtimeDayAndNight)
		{
			SunX += Time.deltaTime * DayTimeScale;
		}
	}

	private static void UpdateSunRotation()
	{
		if (SunX > 360f)
		{
			_sunX = SunX - 360f;
		}
		if (SunY > 360f)
		{
			_sunY = SunY - 360f;
		}
		Transform obj = Sun.transform;
		obj.localEulerAngles = new Vector3(_sunX, _sunY, 0f);
		_sunDirection = obj.eulerAngles;
	}
}
