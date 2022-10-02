using System;
using UnityEngine;
using UnityEngine.Events;

public class Voice : MonoBehaviour
{
	private AudioSource _mouthSource;

	private AudioSource _chestSource;

	private static AudioClip _heartbeat;

	private static AudioClip _breathing;

	private Animator _anim;

	public float minDistancePercentage = 0.01f;

	public float chestDistance = 300f;

	public float mouthDistance = 260f;

	public float pitch = 1f;

	private void Awake()
	{
		_anim = base.gameObject.GetComponent<Animator>();
		GameController instance = GameController.Instance;
		instance.onPaused = (UnityAction<bool>)Delegate.Combine(instance.onPaused, new UnityAction<bool>(OnPaused));
		Transform boneTransform = _anim.GetBoneTransform(HumanBodyBones.Chest);
		if ((bool)boneTransform)
		{
			if (!_heartbeat)
			{
				_heartbeat = Resources.Load<AudioClip>("Sound/Giantess/heartbeat");
			}
			_chestSource = CreateAudioSource(boneTransform);
			_chestSource.clip = _heartbeat;
			_chestSource.outputAudioMixerGroup = SoundManager.AudioMixerMacro;
			_chestSource.Play();
		}
		Transform boneTransform2 = _anim.GetBoneTransform(HumanBodyBones.Head);
		if ((bool)boneTransform2)
		{
			if (!_breathing)
			{
				_breathing = Resources.Load<AudioClip>("Sound/Giantess/female_breathing");
			}
			_mouthSource = CreateAudioSource(boneTransform2);
			_mouthSource.clip = _breathing;
			_mouthSource.outputAudioMixerGroup = SoundManager.AudioMixerMacro;
			_mouthSource.Play();
			_mouthSource.volume = 1f;
		}
	}

	private void OnDestroy()
	{
		GameController instance = GameController.Instance;
		instance.onPaused = (UnityAction<bool>)Delegate.Remove(instance.onPaused, new UnityAction<bool>(OnPaused));
	}

	private static void ToggleAudioSource(AudioSource audioSource, bool paused)
	{
		if (paused)
		{
			audioSource.Pause();
		}
		else
		{
			audioSource.Play();
		}
	}

	private static void SetAudioScale(AudioSource audioSource, float pitch, float minDistancePercentage, float scale, float distance)
	{
		float num = distance * scale;
		float minDistance = num * minDistancePercentage;
		audioSource.maxDistance = num;
		audioSource.minDistance = minDistance;
		audioSource.pitch = pitch;
	}

	private static AudioSource CreateAudioSource(Transform bone)
	{
		AudioSource audioSource = bone.gameObject.AddComponent<AudioSource>();
		audioSource.outputAudioMixerGroup = SoundManager.AudioMixerVoice;
		audioSource.dopplerLevel = 0f;
		audioSource.spatialBlend = 1f;
		audioSource.volume = 1f;
		audioSource.loop = true;
		return audioSource;
	}

	private void OnPaused(bool paused)
	{
		if ((bool)_chestSource)
		{
			ToggleAudioSource(_chestSource, paused);
		}
		if ((bool)_mouthSource)
		{
			ToggleAudioSource(_mouthSource, paused);
		}
	}

	public void UpdateAudioScale(float scale)
	{
		if ((bool)_chestSource)
		{
			SetAudioScale(_chestSource, pitch, minDistancePercentage, scale, chestDistance);
		}
		if ((bool)_mouthSource)
		{
			SetAudioScale(_mouthSource, pitch, minDistancePercentage, scale, mouthDistance);
		}
	}
}
