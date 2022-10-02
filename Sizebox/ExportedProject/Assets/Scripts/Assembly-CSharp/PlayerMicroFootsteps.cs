using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerMicroFootsteps : MonoBehaviour
{
	private AudioSource _audioSource;

	private EntityBase _entity;

	private void Awake()
	{
		_audioSource = base.gameObject.AddComponent<AudioSource>();
		_entity = GetComponentInParent<EntityBase>();
		_audioSource.outputAudioMixerGroup = SoundManager.AudioMixerMicro;
	}

	public void OnStep(AnimationEvent e)
	{
		float magnitude = ((e.floatParameter == 0f) ? 1f : e.floatParameter);
		EventManager.SendEvent(new StepEvent(_entity, base.transform.position, magnitude, e.intParameter));
		_audioSource.pitch = 0.9f;
		_audioSource.volume = 1f;
		_audioSource.spatialBlend = 1f;
		_audioSource.dopplerLevel = 0f;
		_audioSource.minDistance = 0.5f * _entity.Scale;
		_audioSource.maxDistance = 10f * _entity.Scale;
		_audioSource.PlayOneShot(SoundManager.Instance.GetPlayerFootstepSound());
	}

	private void OnDestroy()
	{
		Object.Destroy(_audioSource);
	}
}
