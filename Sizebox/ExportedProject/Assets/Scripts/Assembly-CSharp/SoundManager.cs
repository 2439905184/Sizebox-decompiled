using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class SoundManager : MonoBehaviour, IListener
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Func<AudioClip, bool> _003C_003E9__58_0;

		internal bool _003CGetCitySoundPeaceful_003Eb__58_0(AudioClip x)
		{
			return x.name == "CITY_PEACEFUL";
		}
	}

	public static float AmbientVolume = 0.5f;

	public static float WindVolume = 1f;

	public AudioClip[] giantSteps;

	private AudioClip[] _gigaSteps;

	public AudioClip[] playerSteps;

	public AudioClip[] crushedSound;

	public AudioClip flySound;

	public AudioClip[] destructionSounds;

	public AudioClip[] impactSounds;

	public AudioClip[] explosionSounds;

	public AudioClip[] citySounds;

	public AudioClip natureSound;

	public AudioClip seaSound;

	public AudioClip windSound;

	private AudioSource _tinyAudioSource;

	public AudioClip playerRaygunArmingSound;

	public AudioClip playerRaygunDisarmingSound;

	public AudioClip playerRaygunModeSwitchSound;

	public AudioClip playerRaygunUtilitySound;

	public AudioClip playerRaygunPolaritySound;

	public AudioClip playerRaygunProjFireSound;

	public AudioClip[] playerRaygunProjImpactSounds;

	public AudioClip playerRaygunLaserSustainSound;

	public AudioClip playerRaygunSonicFireSound;

	public AudioClip playerRaygunSonicSustainSound;

	public AudioClip npcRaygunProjFireSound;

	public AudioClip[] npcRaygunProjImpactSounds;

	public AudioClip npcSmgProjFireSound;

	public AudioClip npcSmgProjImpactSound;

	public static AudioMixer AudioMixer;

	public static AudioMixerGroup AudioMixerBackground;

	public static AudioMixerGroup AudioMixerDestruction;

	public static AudioMixerGroup AudioMixerEffects;

	public static AudioMixerGroup AudioMixerMacro;

	public static AudioMixerGroup AudioMixerMicro;

	public static AudioMixerGroup AudioMixerVoice;

	public static AudioMixerGroup AudioMixerWindAmbient;

	public static AudioMixerGroup AudioMixerWingMacro;

	public static AudioMixerGroup AudioMixerRaygun;

	private int _lastStep;

	private int _lastEffect;

	public static SoundManager Instance { get; private set; }

	public void OnNotify(IEvent e)
	{
		StepEvent stepEvent = (StepEvent)e;
		if (stepEvent.entity.isGiantess)
		{
			AudioSource component = stepEvent.entity.model.GetComponent<AudioSource>();
			float scale = stepEvent.entity.Scale;
			component.minDistance = 200f * scale * stepEvent.magnitude;
			component.maxDistance = 10000f * scale * stepEvent.magnitude;
			float num = (((bool)GameController.LocalClient.Player.Entity && !GameController.LocalClient.Player.Entity.isGiantess) ? GameController.LocalClient.Player.Entity.MeshHeight : 1.6f);
			float num2 = Mathf.Log10(stepEvent.entity.Height / num * stepEvent.magnitude);
			float num3 = 1.68f;
			float num4 = 6f;
			AudioClip randomClip = GetRandomClip((num2 > num3) ? _gigaSteps : giantSteps);
			if (num2 < 0f)
			{
				component.volume = 0f;
			}
			else if (num2 < num3)
			{
				component.volume = num2 / num3;
			}
			else
			{
				component.volume = 1f;
			}
			float minPitch = 0.6f;
			float num5 = 0.3f;
			float num6 = ((num2 < 0f) ? Mathf.Clamp(1f + (1f - num2), 1f, 3f) : ((num2 < num3) ? LinearPitchFall(minPitch, 0f, num3, num2) : ((!(num2 < num4)) ? num5 : LinearPitchFall(num5, num3, num4, num2))));
			float num7 = 0.2f * num6;
			component.pitch = num6 + UnityEngine.Random.Range(0f - num7, num7);
			component.PlayOneShot(randomClip);
		}
	}

	private float LinearPitchFall(float minPitch, float floor, float ceil, float scale)
	{
		return minPitch + (1f - minPitch) * (1f - (scale - floor) / (ceil - floor));
	}

	public static void InitializeMixers()
	{
		if (!AudioMixer)
		{
			AudioMixer = Resources.Load("Sound/Mixer") as AudioMixer;
			if (!AudioMixer)
			{
				Debug.LogError("Unable to initialize audio mixer");
				return;
			}
			AudioMixerBackground = AudioMixer.FindMatchingGroups("Background")[0];
			AudioMixerDestruction = AudioMixer.FindMatchingGroups("Destruction")[0];
			AudioMixerEffects = AudioMixer.FindMatchingGroups("Effects")[0];
			AudioMixerMacro = AudioMixer.FindMatchingGroups("Macro")[0];
			AudioMixerMicro = AudioMixer.FindMatchingGroups("Micro")[0];
			AudioMixerWindAmbient = AudioMixer.FindMatchingGroups("WindA")[0];
			AudioMixerWingMacro = AudioMixer.FindMatchingGroups("WindG")[0];
			AudioMixerVoice = AudioMixer.FindMatchingGroups("Voice")[0];
			AudioMixerRaygun = AudioMixer.FindMatchingGroups("Raygun")[0];
		}
	}

	public static void LoadMixerLevels()
	{
		AudioMixer.SetFloat("Master", GlobalPreferences.VolumeMaster.value);
		AudioMixer.SetFloat("Background", GlobalPreferences.VolumeBackground.value);
		AudioMixer.SetFloat("Destruction", GlobalPreferences.VolumeDestruction.value);
		AudioMixer.SetFloat("Macro", GlobalPreferences.VolumeMacro.value);
		AudioMixer.SetFloat("Micro", GlobalPreferences.VolumeMicro.value);
		AudioMixer.SetFloat("Music", GlobalPreferences.VolumeMusic.value);
		AudioMixer.SetFloat("Voice", GlobalPreferences.VolumeVoice.value);
		AudioMixer.SetFloat("WindA", GlobalPreferences.VolumeWindA.value);
		AudioMixer.SetFloat("WindG", GlobalPreferences.VolumeWindG.value);
		AudioMixer.SetFloat("Raygun", GlobalPreferences.VolumeRaygun.value);
		AudioMixer.SetFloat("AIGuns", GlobalPreferences.VolumeAIGuns.value);
	}

	private void Awake()
	{
		InitializeMixers();
		natureSound = Resources.Load<AudioClip>("Sound/nature2");
		seaSound = Resources.Load<AudioClip>("Sound/sea2");
		windSound = Resources.Load<AudioClip>("Sound/storm");
		flySound = Resources.Load<AudioClip>("Sound/Player/Fly3");
		_gigaSteps = Resources.LoadAll<AudioClip>("Sound/Footstep/Giga/");
		destructionSounds = Resources.LoadAll<AudioClip>("Sound/Destruction/");
		impactSounds = Resources.LoadAll<AudioClip>("Sound/Impacts/");
		explosionSounds = Resources.LoadAll<AudioClip>("Sound/Explosions/");
		citySounds = Resources.LoadAll<AudioClip>("Sound/City");
		playerSteps = Resources.LoadAll<AudioClip>("Sound/Footstep/Player");
		giantSteps = Resources.LoadAll<AudioClip>("Sound/Footstep/Giant");
		crushedSound = Resources.LoadAll<AudioClip>("Sound/Crushed");
		playerRaygunArmingSound = Resources.Load<AudioClip>("Sound/Raygun/LASER_CLICK_2");
		playerRaygunDisarmingSound = Resources.Load<AudioClip>("Sound/Raygun/LASER_CLICK_1");
		playerRaygunModeSwitchSound = Resources.Load<AudioClip>("Sound/Raygun/GUN_MODE_CHANGE");
		playerRaygunUtilitySound = Resources.Load<AudioClip>("Sound/Raygun/DIAL_CLICK_2");
		playerRaygunPolaritySound = Resources.Load<AudioClip>("Sound/Raygun/DIAL_CLICK_1");
		playerRaygunProjFireSound = Resources.Load<AudioClip>("Sound/Raygun/LASER_SHOT_1");
		playerRaygunProjImpactSounds = new AudioClip[3]
		{
			Resources.Load<AudioClip>("Sound/Raygun/LASER_IMPACT_1"),
			Resources.Load<AudioClip>("Sound/Raygun/LASER_IMPACT_2"),
			Resources.Load<AudioClip>("Sound/Raygun/LASER_IMPACT_3")
		};
		playerRaygunLaserSustainSound = Resources.Load<AudioClip>("Sound/Raygun/LASER_SUSTAIN_1");
		playerRaygunSonicFireSound = Resources.Load<AudioClip>("Sound/Raygun/SONIC_RING_FIRE");
		playerRaygunSonicSustainSound = Resources.Load<AudioClip>("Sound/Raygun/SONIC_RING_LOOP");
		npcRaygunProjFireSound = Resources.Load<AudioClip>("Sound/Raygun/LASER_SHOT_1");
		npcRaygunProjImpactSounds = new AudioClip[3]
		{
			Resources.Load<AudioClip>("Sound/Raygun/LASER_IMPACT_1"),
			Resources.Load<AudioClip>("Sound/Raygun/LASER_IMPACT_2"),
			Resources.Load<AudioClip>("Sound/Raygun/LASER_IMPACT_3")
		};
		npcSmgProjFireSound = Resources.Load<AudioClip>("Sound/SMG/SINGLE_SHOT");
		if (!Instance)
		{
			Instance = this;
		}
		_tinyAudioSource = new GameObject("Audio Source").AddComponent<AudioSource>();
		_tinyAudioSource.spatialBlend = 1f;
		_tinyAudioSource.dopplerLevel = 0f;
		_tinyAudioSource.minDistance = 10f;
		_tinyAudioSource.maxDistance = 100f;
		EventManager.Register(this, EventCode.OnStep);
	}

	private void Start()
	{
		LoadMixerLevels();
		GameController instance = GameController.Instance;
		instance.onPaused = (UnityAction<bool>)Delegate.Combine(instance.onPaused, new UnityAction<bool>(SetPaused));
	}

	private void OnDestroy()
	{
		GameController instance = GameController.Instance;
		instance.onPaused = (UnityAction<bool>)Delegate.Remove(instance.onPaused, new UnityAction<bool>(SetPaused));
	}

	public void SetPaused(bool paused)
	{
		if (paused)
		{
			AudioMixer.SetFloat("WindA", -80f);
			AudioMixer.SetFloat("WindG", -80f);
		}
		else
		{
			AudioMixer.SetFloat("WindA", GlobalPreferences.VolumeWindA.value);
			AudioMixer.SetFloat("WindG", GlobalPreferences.VolumeWindG.value);
		}
	}

	public static void SetAndPlaySoundClip(AudioSource source, AudioClip clip)
	{
		if (source.clip != clip)
		{
			source.clip = clip;
			source.Play();
		}
		if (!source.isPlaying)
		{
			source.Play();
		}
	}

	private AudioClip GetRandomSound(AudioClip[] clips)
	{
		int num = UnityEngine.Random.Range(0, clips.Length);
		if (num != _lastEffect)
		{
			_lastEffect = num;
			return clips[num];
		}
		num = ((num < clips.Length - 1) ? (num + 1) : 0);
		return clips[num];
	}

	public AudioClip GetDestructionSound()
	{
		return GetRandomSound(destructionSounds);
	}

	public AudioClip GetImpactSound()
	{
		return GetRandomSound(impactSounds);
	}

	public AudioClip GetExplosionSound()
	{
		return GetRandomSound(explosionSounds);
	}

	public AudioClip GetCitySoundPeaceful()
	{
		return citySounds.Single(_003C_003Ec._003C_003E9__58_0 ?? (_003C_003Ec._003C_003E9__58_0 = _003C_003Ec._003C_003E9._003CGetCitySoundPeaceful_003Eb__58_0));
	}

	public AudioClip GetPlayerFootstepSound()
	{
		_lastStep++;
		if (_lastStep >= playerSteps.Length)
		{
			_lastStep = 0;
		}
		return playerSteps[_lastStep];
	}

	public AudioClip GetPlayerRaygunProjectileImpactSound()
	{
		return playerRaygunProjImpactSounds[UnityEngine.Random.Range(0, playerRaygunProjImpactSounds.Length)];
	}

	public AudioClip GetNpcRaygunProjectileImpactSound()
	{
		return npcRaygunProjImpactSounds[UnityEngine.Random.Range(0, npcRaygunProjImpactSounds.Length)];
	}

	public AudioClip GetNpcSmgProjectileImpactSound()
	{
		return npcSmgProjImpactSound;
	}

	public void PlayCrushed(Vector3 position, float size)
	{
		_tinyAudioSource.minDistance = 1f * size;
		_tinyAudioSource.maxDistance = 20f * size;
		_tinyAudioSource.transform.position = position;
		_tinyAudioSource.outputAudioMixerGroup = AudioMixerEffects;
		int max = crushedSound.Length;
		_tinyAudioSource.PlayOneShot(crushedSound[UnityEngine.Random.Range(0, max)]);
	}

	public static void SetUpAmbientAudioSource(AudioSource source)
	{
		source.loop = true;
		source.dopplerLevel = 0f;
	}

	public static AudioClip GetRandomClip(AudioClip[] clips)
	{
		if (clips.Length != 0)
		{
			return clips[UnityEngine.Random.Range(0, clips.Length)];
		}
		return null;
	}

	public void SetPlayerRaygunArmingSound(string clip)
	{
		playerRaygunArmingSound = IOManager.Instance.LoadAudioClip(clip);
		UpdatePlayerRaygunSounds();
	}

	public void ResetPlayerRaygunArmingSound()
	{
		playerRaygunArmingSound = Resources.Load<AudioClip>("Sound/Raygun/LASER_CLICK_2");
		UpdatePlayerRaygunSounds();
	}

	public void SetPlayerRaygunDisarmingSound(string clip)
	{
		playerRaygunDisarmingSound = IOManager.Instance.LoadAudioClip(clip);
		UpdatePlayerRaygunSounds();
	}

	public void ResetPlayerRaygunDisarmingSound()
	{
		playerRaygunDisarmingSound = Resources.Load<AudioClip>("Sound/Raygun/LASER_CLICK_1");
		UpdatePlayerRaygunSounds();
	}

	public void SetPlayerRaygunModeSwitchSound(string clip)
	{
		playerRaygunModeSwitchSound = IOManager.Instance.LoadAudioClip(clip);
		UpdatePlayerRaygunSounds();
	}

	public void ResetPlayerRaygunModeSwitchSound()
	{
		playerRaygunModeSwitchSound = Resources.Load<AudioClip>("Sound/Raygun/GUN_MODE_CHANGE");
		UpdatePlayerRaygunSounds();
	}

	public void SetPlayerRaygunUtilitySound(string clip)
	{
		playerRaygunUtilitySound = IOManager.Instance.LoadAudioClip(clip);
		UpdatePlayerRaygunSounds();
	}

	public void ResetPlayerRaygunUtilitySound()
	{
		playerRaygunUtilitySound = Resources.Load<AudioClip>("Sound/Raygun/DIAL_CLICK_2");
		UpdatePlayerRaygunSounds();
	}

	public void SetPlayerRaygunPolaritySound(string clip)
	{
		playerRaygunPolaritySound = IOManager.Instance.LoadAudioClip(clip);
		UpdatePlayerRaygunSounds();
	}

	public void ResetPlayerRaygunPolaritySound()
	{
		playerRaygunPolaritySound = Resources.Load<AudioClip>("Sound/Raygun/DIAL_CLICK_1");
		UpdatePlayerRaygunSounds();
	}

	public void SetPlayerRaygunProjFireSound(string clip)
	{
		playerRaygunProjFireSound = IOManager.Instance.LoadAudioClip(clip);
		UpdatePlayerRaygunSounds();
	}

	public void ResetPlayerRaygunProjFireSound()
	{
		playerRaygunProjFireSound = Resources.Load<AudioClip>("Sound/Raygun/LASER_SHOT_1");
		UpdatePlayerRaygunSounds();
	}

	public void SetPlayerRaygunProjImpactSound(string clip)
	{
		playerRaygunProjImpactSounds = new AudioClip[1] { IOManager.Instance.LoadAudioClip(clip) };
		UpdatePlayerRaygunSounds();
	}

	public void ResetPlayerRaygunProjImpactSounds()
	{
		playerRaygunProjImpactSounds = new AudioClip[3]
		{
			Resources.Load<AudioClip>("Sound/Raygun/LASER_IMPACT_1"),
			Resources.Load<AudioClip>("Sound/Raygun/LASER_IMPACT_2"),
			Resources.Load<AudioClip>("Sound/Raygun/LASER_IMPACT_3")
		};
		UpdatePlayerRaygunSounds();
	}

	public void SetPlayerRaygunLaserSustainSound(string clip)
	{
		playerRaygunLaserSustainSound = IOManager.Instance.LoadAudioClip(clip);
		UpdatePlayerRaygunSounds();
	}

	public void ResetPlayerRaygunLaserSustainSound()
	{
		playerRaygunLaserSustainSound = Resources.Load<AudioClip>("Sound/Raygun/LASER_SUSTAIN_1");
		UpdatePlayerRaygunSounds();
	}

	public void SetPlayerRaygunSonicFireSound(string clip)
	{
		playerRaygunSonicFireSound = IOManager.Instance.LoadAudioClip(clip);
		UpdatePlayerRaygunSounds();
	}

	public void ResetPlayerRaygunSonicFireSound()
	{
		playerRaygunSonicFireSound = Resources.Load<AudioClip>("Sound/Raygun/SONIC_RING_FIRE");
		UpdatePlayerRaygunSounds();
	}

	public void SetPlayerRaygunSonicSustainSound(string clip)
	{
		playerRaygunSonicSustainSound = IOManager.Instance.LoadAudioClip(clip);
		UpdatePlayerRaygunSounds();
	}

	public void ResetPlayerRaygunSonicSustainSound()
	{
		playerRaygunSonicSustainSound = Resources.Load<AudioClip>("Sound/Raygun/SONIC_RING_LOOP");
		UpdatePlayerRaygunSounds();
	}

	public void SetNpcRaygunProjFireSound(string clip)
	{
		npcRaygunProjFireSound = IOManager.Instance.LoadAudioClip(clip);
	}

	public void ResetNpcRaygunProjFireSound()
	{
		npcRaygunProjFireSound = Resources.Load<AudioClip>("Sound/Raygun/LASER_SHOT_1");
	}

	public void SetNpcRaygunProjImpactSound(string clip)
	{
		npcRaygunProjImpactSounds = new AudioClip[1] { IOManager.Instance.LoadAudioClip(clip) };
	}

	public void ResetNpcRaygunProjImpactSounds()
	{
		npcRaygunProjImpactSounds = new AudioClip[3]
		{
			Resources.Load<AudioClip>("Sound/Raygun/LASER_IMPACT_1"),
			Resources.Load<AudioClip>("Sound/Raygun/LASER_IMPACT_2"),
			Resources.Load<AudioClip>("Sound/Raygun/LASER_IMPACT_3")
		};
	}

	public void SetNpcSmgProjFireSound(string clip)
	{
		npcSmgProjFireSound = IOManager.Instance.LoadAudioClip(clip);
	}

	public void ResetNpcSmgProjFireSound()
	{
		npcSmgProjFireSound = Resources.Load<AudioClip>("Sound/SMG/SINGLE_SHOT");
	}

	public void SetNpcSmgProjImpactSound(string clip)
	{
		npcSmgProjImpactSound = IOManager.Instance.LoadAudioClip(clip);
	}

	public void ResetNpcSmgProjImpactSound()
	{
		npcSmgProjImpactSound = null;
	}

	private void UpdatePlayerRaygunSounds()
	{
		if (PlayerRaygun.instance != null)
		{
			PlayerRaygun.instance.UpdateSounds();
		}
	}
}
