using UnityEngine;

public class SpaceScript : MonoBehaviour
{
	private AudioSource _audioSource;

	private AudioClip[] _spaceSounds;

	private void Start()
	{
		_spaceSounds = Resources.LoadAll<AudioClip>("Sound/Space");
		_audioSource = base.gameObject.AddComponent<AudioSource>();
		_audioSource.outputAudioMixerGroup = SoundManager.AudioMixerBackground;
		SoundManager.SetUpAmbientAudioSource(_audioSource);
	}

	private void Update()
	{
		_audioSource.volume = SoundManager.AmbientVolume * 0.5f;
		if (!_audioSource.isPlaying)
		{
			_audioSource.clip = SoundManager.GetRandomClip(_spaceSounds);
			_audioSource.Play();
		}
	}
}
