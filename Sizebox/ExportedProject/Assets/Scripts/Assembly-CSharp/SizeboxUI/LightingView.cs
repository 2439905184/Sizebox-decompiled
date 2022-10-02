using System.Globalization;
using Pause;
using UnityEngine.UI;

namespace SizeboxUI
{
	public class LightingView : SettingsView
	{
		private InputSlider _time;

		private InputSlider _sunAngle;

		private InputSlider _intensity;

		private Slider _ambientLight;

		private Toggle _realtimeDayAndNight;

		private InputSlider _dayTimeScale;

		private void Start()
		{
			GetComponentInChildren<Text>().text = "Lighting";
			if ((bool)SimpleSunController.Sun)
			{
				AddHeader("Sun");
				_time = AddInputSlider("Time", 0f, 359.99f);
				_time.input.text = SimpleSunController.SunX.ToString(CultureInfo.InvariantCulture);
				_time.slider.onValueChanged.AddListener(OnTimeChanged);
				_sunAngle = AddInputSlider("Sun Angle", 0f, 359.99f);
				_sunAngle.input.text = SimpleSunController.SunY.ToString(CultureInfo.InvariantCulture);
				_sunAngle.slider.onValueChanged.AddListener(OnSunAngleChanged);
				_intensity = AddInputSlider("Intensity", 0f, 1f);
				float sunIntensity = SimpleSunController.SunIntensity;
				_intensity.input.text = sunIntensity.ToString(CultureInfo.InvariantCulture);
				_intensity.slider.value = sunIntensity;
				_intensity.slider.onValueChanged.AddListener(OnSunIntensityChanged);
				_realtimeDayAndNight = AddToggle("Realtime Day and Night");
				_realtimeDayAndNight.isOn = SimpleSunController.UseRealtimeDayAndNight;
				_realtimeDayAndNight.onValueChanged.AddListener(OnRealtimeDayAndNightChanged);
				_dayTimeScale = AddInputSlider("Day Time Scale", 0.1f, 10f);
				_dayTimeScale.input.text = SimpleSunController.DayTimeScale.ToString(CultureInfo.InvariantCulture);
				_dayTimeScale.slider.onValueChanged.AddListener(OnDayTimeScaleSliderChanged);
			}
			else
			{
				AddHeader("No dynamic sun in the scene.");
			}
		}

		private void OnSunIntensityChanged(float val)
		{
			SimpleSunController.SunIntensity = val;
			_intensity.input.text = _intensity.slider.value.ToString(CultureInfo.InvariantCulture);
		}

		private void OnTimeChanged(float val)
		{
			SimpleSunController.SunX = val;
			_time.input.text = _time.slider.value.ToString(CultureInfo.InvariantCulture);
		}

		private void OnSunAngleChanged(float val)
		{
			SimpleSunController.SunY = val;
			_sunAngle.input.text = _sunAngle.slider.value.ToString(CultureInfo.InvariantCulture);
		}

		private void OnRealtimeDayAndNightChanged(bool val)
		{
			SimpleSunController.UseRealtimeDayAndNight = val;
		}

		private void OnDayTimeScaleSliderChanged(float val)
		{
			SimpleSunController.DayTimeScale = val;
			_dayTimeScale.input.text = _dayTimeScale.slider.value.ToString(CultureInfo.InvariantCulture);
		}
	}
}
