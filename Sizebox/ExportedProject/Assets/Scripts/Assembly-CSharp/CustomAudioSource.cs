using System.Collections;
using UnityEngine;

public class CustomAudioSource : MonoBehaviour
{
	public float maxDistance;

	public float minDistance;

	private Transform _transform;

	private EntityBase _entity;

	public AudioSource audioSource { get; private set; }

	private void Awake()
	{
		_transform = base.transform;
		audioSource = base.gameObject.AddComponent<AudioSource>();
		audioSource.playOnAwake = false;
		audioSource.dopplerLevel = 0f;
		audioSource.rolloffMode = AudioRolloffMode.Linear;
		audioSource.outputAudioMixerGroup = SoundManager.AudioMixerEffects;
		FindEntity();
		maxDistance = 5f;
		audioSource.maxDistance *= GetScale();
		audioSource.minDistance = 0f;
		minDistance = audioSource.minDistance;
	}

	private void Update()
	{
		float scale = GetScale();
		audioSource.maxDistance = maxDistance * scale;
		audioSource.minDistance = minDistance * scale;
	}

	private float GetScale()
	{
		if (_entity == null)
		{
			return _transform.lossyScale.y;
		}
		return _entity.AccurateScale;
	}

	public void Play()
	{
		StartCoroutine(PlayRoutine());
	}

	private IEnumerator PlayRoutine()
	{
		AudioClip clip = audioSource.clip;
		if ((bool)clip)
		{
			while (clip.loadState == AudioDataLoadState.Loading)
			{
				yield return null;
			}
			if (clip.loadState == AudioDataLoadState.Loaded)
			{
				audioSource.Play();
			}
			else
			{
				Debug.LogError("Load of audio clip " + clip.name + " has failed");
			}
		}
	}

	public void PlayDelayed(float delay)
	{
		StartCoroutine(PlayDelayedRoutine(delay));
	}

	private IEnumerator PlayDelayedRoutine(float delay)
	{
		AudioClip clip = audioSource.clip;
		while (clip.loadState == AudioDataLoadState.Loading)
		{
			yield return null;
		}
		if (clip.loadState == AudioDataLoadState.Loaded)
		{
			audioSource.PlayDelayed(delay);
		}
		else
		{
			Debug.LogError("Load of audio clip " + clip.name + " has failed");
		}
	}

	public void PlayOneShot(AudioClip clip, float volumeScale)
	{
		StartCoroutine(PlayOneShotRoutine(clip, volumeScale));
	}

	private IEnumerator PlayOneShotRoutine(AudioClip clip, float volumeScale)
	{
		while (clip.loadState == AudioDataLoadState.Loading)
		{
			yield return null;
		}
		if (clip.loadState == AudioDataLoadState.Loaded)
		{
			audioSource.PlayOneShot(clip, volumeScale);
		}
		else
		{
			Debug.LogError("Load of audio clip " + clip.name + " has failed");
		}
	}

	public void PlayOneShot(AudioClip clip)
	{
		StartCoroutine(PlayOneShotRoutine(clip));
	}

	private IEnumerator PlayOneShotRoutine(AudioClip clip)
	{
		while (clip.loadState == AudioDataLoadState.Loading)
		{
			yield return null;
		}
		if (clip.loadState == AudioDataLoadState.Loaded)
		{
			audioSource.PlayOneShot(clip);
		}
		else
		{
			Debug.LogError("Load of audio clip " + clip.name + " has failed");
		}
	}

	private void FindEntity()
	{
		Transform parent = _transform;
		while (_entity == null && parent != null)
		{
			_entity = parent.GetComponent<EntityBase>();
			parent = parent.parent;
		}
	}
}
