using System;
using UnityEngine;

public class TerrainScript : MonoBehaviour
{
	private AudioSource _ambientAudioSource;

	private Camera _mainCamera;

	private float _windLevel = 1000f;

	private Material _skybox;

	private Transform _cameraTransform;

	private float _seaLevel = 205f;

	private void Start()
	{
		_mainCamera = Camera.main;
		_cameraTransform = _mainCamera.transform;
		_skybox = UnityEngine.Object.Instantiate(Resources.Load<Material>("Shaders/IslandSky"));
		_ambientAudioSource = base.gameObject.AddComponent<AudioSource>();
		_ambientAudioSource.clip = SoundManager.Instance.natureSound;
		_ambientAudioSource.loop = true;
		_ambientAudioSource.outputAudioMixerGroup = SoundManager.AudioMixerBackground;
		_ambientAudioSource.dopplerLevel = 0f;
		_ambientAudioSource.Play();
	}

	private void Update()
	{
		UpdateAtmosphere();
		UpdateSounds();
	}

	private void UpdateAtmosphere()
	{
		float value = 1f - CenterOrigin.WorldToVirtual(_cameraTransform.position).y / 50000f;
		value = Mathf.Clamp(value, 0.5f, 1f);
		_skybox.SetFloat("_AtmosphereThickness", value);
	}

	private void UpdateSounds()
	{
		Vector3 position = _cameraTransform.position;
		Vector3 vector = position.ToVirtual();
		RaycastHit hitInfo;
		if (Physics.Raycast(position, Vector3.down, out hitInfo, 10000f, Layers.mapMask))
		{
			float num = CenterOrigin.WorldToVirtual(hitInfo.point).y;
			float num2 = hitInfo.distance;
			if (num < _seaLevel)
			{
				num = _seaLevel;
				float y = vector.y;
				num2 = ((!(y > _seaLevel)) ? 0f : (y - _seaLevel));
			}
			float num3 = num2 + num;
			float num4 = 1f - Mathf.Clamp(num3 / _windLevel, 0f, 1f);
			float num5 = 1f - Mathf.Clamp(num2 / 100f, 0f, 1f);
			float num6 = num4 * num5;
			if (num6 < 0.01f)
			{
				float num7 = Mathf.Min(_windLevel, num + 100f);
				num6 = Mathf.Clamp01((vector.y - num7) / 500f);
				SetAmbientClip(SoundManager.Instance.windSound);
			}
			else
			{
				SetAmbientClip((Math.Abs(num - _seaLevel) < float.Epsilon) ? SoundManager.Instance.seaSound : SoundManager.Instance.natureSound);
			}
			_ambientAudioSource.volume = num6;
		}
	}

	private void SetAmbientClip(AudioClip clip)
	{
		SoundManager.SetAndPlaySoundClip(_ambientAudioSource, clip);
	}
}
