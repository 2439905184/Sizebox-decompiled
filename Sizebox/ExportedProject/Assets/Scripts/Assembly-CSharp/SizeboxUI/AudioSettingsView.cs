using System.Runtime.CompilerServices;
using Pause;
using UnityEngine.UI;

namespace SizeboxUI
{
	public class AudioSettingsView : SettingsView
	{
		private Slider _master;

		private Slider _background;

		private Slider _destruction;

		private Slider _effects;

		private Slider _macro;

		private Slider _micro;

		private Slider _music;

		private Slider _voice;

		private Slider _windA;

		private Slider _windG;

		private Slider _raygun;

		private Slider _aiGuns;

		private Toggle _backgroundAudio;

		private void Start()
		{
			base.Title = "Audio";
			_master = AddSlider("Master", -80f, 0f);
			_master.onValueChanged.AddListener(_003CStart_003Eb__13_0);
			_background = AddSlider("Background", -80f, 0f);
			_background.onValueChanged.AddListener(_003CStart_003Eb__13_1);
			_destruction = AddSlider("Destruction", -80f, 0f);
			_destruction.onValueChanged.AddListener(_003CStart_003Eb__13_2);
			_effects = AddSlider("Effects", -80f, 0f);
			_effects.onValueChanged.AddListener(_003CStart_003Eb__13_3);
			_macro = AddSlider("Macros", -80f, 0f);
			_macro.onValueChanged.AddListener(_003CStart_003Eb__13_4);
			_micro = AddSlider("Micros", -80f, 0f);
			_micro.onValueChanged.AddListener(_003CStart_003Eb__13_5);
			_music = AddSlider("Music", -80f, 0f);
			_music.onValueChanged.AddListener(_003CStart_003Eb__13_6);
			_voice = AddSlider("Voice", -80f, 0f);
			_voice.onValueChanged.AddListener(_003CStart_003Eb__13_7);
			_windA = AddSlider("Ambient Wind", -80f, 0f);
			_windG = AddSlider("Giantess Wind", -80f, 0f);
			_windA.value = GlobalPreferences.VolumeWindA.value;
			_windG.value = GlobalPreferences.VolumeWindG.value;
			_windA.onValueChanged.AddListener(_003CStart_003Eb__13_8);
			_windG.onValueChanged.AddListener(_003CStart_003Eb__13_9);
			_raygun = AddSlider("Raygun Effects", -80f, 0f);
			_raygun.onValueChanged.AddListener(_003CStart_003Eb__13_10);
			_aiGuns = AddSlider("AI Gun Effects", -80f, 0f);
			_aiGuns.onValueChanged.AddListener(_003CStart_003Eb__13_11);
			_backgroundAudio = AddToggle("Play Audio when unfocused");
			_backgroundAudio.onValueChanged.AddListener(OnUnfocusMuteChanged);
			UpdateValues();
			initialized = true;
		}

		private void OnUnfocusMuteChanged(bool value)
		{
			GlobalPreferences.BackgroundAudio.value = value;
		}

		private void OnMasterChanged(float val, bool write = false)
		{
			if (SoundManager.AudioMixer != null)
			{
				SoundManager.AudioMixer.SetFloat("Master", val);
			}
			if (write)
			{
				GlobalPreferences.VolumeMaster.value = val;
			}
		}

		private void OnBackgroundChanged(float val, bool write = false)
		{
			if (SoundManager.AudioMixer != null)
			{
				SoundManager.AudioMixer.SetFloat("Background", val);
			}
			if (write)
			{
				GlobalPreferences.VolumeBackground.value = val;
			}
		}

		private void OnDestructionChanged(float val, bool write = false)
		{
			if (SoundManager.AudioMixer != null)
			{
				SoundManager.AudioMixer.SetFloat("Destruction", val);
			}
			if (write)
			{
				GlobalPreferences.VolumeDestruction.value = val;
			}
		}

		private void OnEffectsChanged(float val, bool write = false)
		{
			if (SoundManager.AudioMixer != null)
			{
				SoundManager.AudioMixer.SetFloat("Effects", val);
			}
			if (write)
			{
				GlobalPreferences.VolumeEffect.value = val;
			}
		}

		private void OnMacroChanged(float val, bool write = false)
		{
			if (SoundManager.AudioMixer != null)
			{
				SoundManager.AudioMixer.SetFloat("Macro", val);
			}
			if (write)
			{
				GlobalPreferences.VolumeMacro.value = val;
			}
		}

		private void OnMicroChanged(float val, bool write = false)
		{
			if (SoundManager.AudioMixer != null)
			{
				SoundManager.AudioMixer.SetFloat("Micro", val);
			}
			if (write)
			{
				GlobalPreferences.VolumeMicro.value = val;
			}
		}

		private void OnMusicChanged(float val, bool write = false)
		{
			if (SoundManager.AudioMixer != null)
			{
				SoundManager.AudioMixer.SetFloat("Music", val);
			}
			if (write)
			{
				GlobalPreferences.VolumeMusic.value = val;
			}
		}

		private void OnVoiceChanged(float val, bool write = false)
		{
			if (SoundManager.AudioMixer != null)
			{
				SoundManager.AudioMixer.SetFloat("Voice", val);
			}
			if (write)
			{
				GlobalPreferences.VolumeVoice.value = val;
			}
		}

		private void OnWindAChanged(float val, bool write = false)
		{
			if (SoundManager.AudioMixer != null)
			{
				SoundManager.AudioMixer.SetFloat("WindA", val);
			}
			if (write)
			{
				GlobalPreferences.VolumeWindA.value = val;
			}
		}

		private void OnWindGChanged(float val, bool write = false)
		{
			if (SoundManager.AudioMixer != null)
			{
				SoundManager.AudioMixer.SetFloat("WindG", val);
			}
			if (write)
			{
				GlobalPreferences.VolumeWindG.value = val;
			}
		}

		private void OnRaygunChanged(float val, bool write = false)
		{
			if (SoundManager.AudioMixer != null)
			{
				SoundManager.AudioMixer.SetFloat("Raygun", val);
			}
			if (write)
			{
				GlobalPreferences.VolumeRaygun.value = val;
			}
		}

		private void OnAIGunsChanged(float val, bool write = false)
		{
			if (SoundManager.AudioMixer != null)
			{
				SoundManager.AudioMixer.SetFloat("AIGuns", val);
			}
			if (write)
			{
				GlobalPreferences.VolumeAIGuns.value = val;
			}
		}

		protected override void UpdateValues()
		{
			if (SoundManager.AudioMixer != null)
			{
				_backgroundAudio.isOn = GlobalPreferences.BackgroundAudio.value;
				float value;
				SoundManager.AudioMixer.GetFloat("Master", out value);
				_master.value = value;
				SoundManager.AudioMixer.GetFloat("Background", out value);
				_background.value = value;
				SoundManager.AudioMixer.GetFloat("Destruction", out value);
				_destruction.value = value;
				SoundManager.AudioMixer.GetFloat("Effects", out value);
				_effects.value = value;
				SoundManager.AudioMixer.GetFloat("Macro", out value);
				_macro.value = value;
				SoundManager.AudioMixer.GetFloat("Micro", out value);
				_micro.value = value;
				SoundManager.AudioMixer.GetFloat("Music", out value);
				_music.value = value;
				SoundManager.AudioMixer.GetFloat("Voice", out value);
				_voice.value = value;
				SoundManager.AudioMixer.GetFloat("Raygun", out value);
				_raygun.value = value;
				SoundManager.AudioMixer.GetFloat("AIGuns", out value);
				_aiGuns.value = value;
				_windA.value = GlobalPreferences.VolumeWindA.value;
				_windG.value = GlobalPreferences.VolumeWindG.value;
			}
			else
			{
				_backgroundAudio.isOn = GlobalPreferences.BackgroundAudio.value;
				_master.value = GlobalPreferences.VolumeMaster.value;
				_background.value = GlobalPreferences.VolumeBackground.value;
				_destruction.value = GlobalPreferences.VolumeDestruction.value;
				_effects.value = GlobalPreferences.VolumeEffect.value;
				_macro.value = GlobalPreferences.VolumeMacro.value;
				_micro.value = GlobalPreferences.VolumeMicro.value;
				_music.value = GlobalPreferences.VolumeMusic.value;
				_voice.value = GlobalPreferences.VolumeVoice.value;
				_windA.value = GlobalPreferences.VolumeWindA.value;
				_windG.value = GlobalPreferences.VolumeWindG.value;
				_raygun.value = GlobalPreferences.VolumeRaygun.value;
				_aiGuns.value = GlobalPreferences.VolumeAIGuns.value;
			}
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__13_0(float v)
		{
			OnMasterChanged(v, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__13_1(float v)
		{
			OnBackgroundChanged(v, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__13_2(float v)
		{
			OnDestructionChanged(v, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__13_3(float v)
		{
			OnEffectsChanged(v, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__13_4(float v)
		{
			OnMacroChanged(v, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__13_5(float v)
		{
			OnMicroChanged(v, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__13_6(float v)
		{
			OnMusicChanged(v, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__13_7(float v)
		{
			OnVoiceChanged(v, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__13_8(float v)
		{
			OnWindAChanged(v, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__13_9(float v)
		{
			OnWindGChanged(v, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__13_10(float v)
		{
			OnRaygunChanged(v, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__13_11(float v)
		{
			OnAIGunsChanged(v, true);
		}
	}
}
