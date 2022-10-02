using MoonSharp.Interpreter;
using UnityEngine;

namespace Lua
{
	[MoonSharpUserData]
	public class AudioSource
	{
		private CustomAudioSource customSource;

		private static IOManager ioManager;

		public float maxDistance
		{
			get
			{
				return customSource.maxDistance;
			}
			set
			{
				customSource.maxDistance = value;
			}
		}

		public float minDistance
		{
			get
			{
				return customSource.minDistance;
			}
			set
			{
				customSource.minDistance = value;
			}
		}

		public string clip
		{
			get
			{
				return customSource.audioSource.clip.name;
			}
			set
			{
				customSource.audioSource.clip = GetClip(value);
			}
		}

		public bool isPlaying
		{
			get
			{
				return customSource.audioSource.isPlaying;
			}
		}

		public bool loop
		{
			get
			{
				return customSource.audioSource.loop;
			}
			set
			{
				customSource.audioSource.loop = value;
			}
		}

		public bool mute
		{
			get
			{
				return customSource.audioSource.mute;
			}
			set
			{
				customSource.audioSource.mute = value;
			}
		}

		public float pitch
		{
			get
			{
				return customSource.audioSource.pitch;
			}
			set
			{
				customSource.audioSource.pitch = value;
			}
		}

		public float spatialBlend
		{
			get
			{
				return customSource.audioSource.spatialBlend;
			}
			set
			{
				customSource.audioSource.spatialBlend = value;
			}
		}

		public float volume
		{
			get
			{
				return customSource.audioSource.volume;
			}
			set
			{
				customSource.audioSource.volume = value;
			}
		}

		[MoonSharpHidden]
		public AudioSource(Transform transform)
		{
			if (transform == null)
			{
				Debug.Log("No transform.");
			}
			customSource = transform._tf.GetComponent<CustomAudioSource>();
			if (customSource == null)
			{
				customSource = transform._tf.gameObject.AddComponent<CustomAudioSource>();
			}
			if (ioManager == null)
			{
				ioManager = IOManager.Instance;
			}
		}

		public static AudioSource New(Entity entity)
		{
			return new AudioSource(entity.transform);
		}

		public static AudioSource New(Transform transform)
		{
			return new AudioSource(transform);
		}

		private static AudioClip GetClip(string clipName)
		{
			if (ioManager == null)
			{
				ioManager = IOManager.Instance;
			}
			return ioManager.LoadAudioClip(clipName);
		}

		public void Pause()
		{
			customSource.audioSource.Pause();
		}

		public void Play()
		{
			customSource.Play();
		}

		public void PlayDelayed(float delay)
		{
			customSource.PlayDelayed(delay);
		}

		public void PlayOneShot(string clip, float volumeScale)
		{
			customSource.PlayOneShot(GetClip(clip), volumeScale);
		}

		public void PlayOneShot(string clip)
		{
			customSource.PlayOneShot(GetClip(clip));
		}

		public void Stop()
		{
			customSource.audioSource.Stop();
		}

		public void UnPause()
		{
			customSource.audioSource.UnPause();
		}

		public static void PlayClipAtPoint(string clip, Vector3 position, float volume)
		{
			UnityEngine.AudioSource.PlayClipAtPoint(GetClip(clip), position.virtualPosition.ToWorld(), volume);
		}

		public static float GetClipLength(string clip)
		{
			AudioClip audioClip = GetClip(clip);
			if (!audioClip || audioClip.loadState != AudioDataLoadState.Loaded)
			{
				return -1f;
			}
			return audioClip.length;
		}
	}
}
