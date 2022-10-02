using UnityEngine;
using UnityEngine.Serialization;

public class Wind : MonoBehaviour
{
	private Animator _anim;

	private Player _player;

	private Giantess _giantess;

	private int _animationSpeedHash;

	private AudioSource _leftHandSource;

	private AudioSource _rightHandSource;

	private AudioSource _leftLegSource;

	private AudioSource _rightLegSource;

	private AudioSource _leftFootSource;

	private AudioSource _rightFootSource;

	private AudioSource _hipSource;

	private AudioSource _headSource;

	private AudioSource _chestSource;

	private static AudioClip _handWind;

	private static AudioClip _legWind;

	private static AudioClip _footWind;

	private static AudioClip _hipWind;

	private static AudioClip _headWind;

	private static AudioClip _chestWind;

	public float minDistancePercentage = 0.015f;

	[FormerlySerializedAs("lefthandDistance")]
	public float leftHandDistance = 10000f;

	[FormerlySerializedAs("righthandDistance")]
	public float rightHandDistance = 10000f;

	[FormerlySerializedAs("leftfootDistance")]
	public float leftFootDistance = 10000f;

	[FormerlySerializedAs("rightfootDistance")]
	public float rightFootDistance = 10000f;

	[FormerlySerializedAs("leftlegDistance")]
	public float leftLegDistance = 10000f;

	[FormerlySerializedAs("rightlegDistance")]
	public float rightLegDistance = 10000f;

	public float hipDistance = 10000f;

	public float headDistance = 10000f;

	public float chestDistance = 10001f;

	[FormerlySerializedAs("lhand")]
	[FormerlySerializedAs("Lhand")]
	public Transform leftHand;

	[FormerlySerializedAs("l2Hand")]
	[FormerlySerializedAs("L2hand")]
	public Vector3 leftHandPosition;

	[FormerlySerializedAs("rhand")]
	[FormerlySerializedAs("Rhand")]
	public Transform rightHand;

	[FormerlySerializedAs("r2Hand")]
	[FormerlySerializedAs("R2hand")]
	public Vector3 rightHandPosition;

	[FormerlySerializedAs("lfoot")]
	[FormerlySerializedAs("Lfoot")]
	public Transform leftFoot;

	[FormerlySerializedAs("l2Foot")]
	[FormerlySerializedAs("L2foot")]
	public Vector3 leftFootPosition;

	[FormerlySerializedAs("rfoot")]
	[FormerlySerializedAs("Rfoot")]
	public Transform rightFoot;

	[FormerlySerializedAs("r2Foot")]
	[FormerlySerializedAs("R2foot")]
	public Vector3 rightFootPosition;

	[FormerlySerializedAs("lleg")]
	[FormerlySerializedAs("Lleg")]
	public Transform leftLeg;

	[FormerlySerializedAs("l2Leg")]
	[FormerlySerializedAs("L2leg")]
	public Vector3 leftLegPosition;

	[FormerlySerializedAs("rleg")]
	[FormerlySerializedAs("Rleg")]
	public Transform rightLeg;

	[FormerlySerializedAs("r2Leg")]
	[FormerlySerializedAs("R2leg")]
	public Vector3 rightLegPosition;

	[FormerlySerializedAs("thips")]
	[FormerlySerializedAs("Thips")]
	public Transform hipsTransform;

	[FormerlySerializedAs("t2Hips")]
	[FormerlySerializedAs("T2hips")]
	public Vector3 hipsTransformPosition;

	[FormerlySerializedAs("thead")]
	[FormerlySerializedAs("Thead")]
	public Transform headTransform;

	public Quaternion headRotation;

	[FormerlySerializedAs("tchest")]
	[FormerlySerializedAs("Tchest")]
	public Transform chestTransform;

	[FormerlySerializedAs("t2Chest")]
	[FormerlySerializedAs("T2chest")]
	public Vector3 chestTransformPosition;

	private float _pitchAdjust;

	private float _leftHandDisplacement;

	private float _rightHandDisplacement;

	private float _leftFootDisplacement;

	private float _rightFootDisplacement;

	private float _leftLegDisplacement;

	private float _rightLegDisplacement;

	private float _hipDisplacement;

	private float _headRotationalDisplacement;

	private float _chestDisplacement;

	private float _modifier;

	private float _relationship;

	private float _scale;

	private float _speed;

	private float _size;

	private float _modPitch;

	private float _legPitch;

	private float _headPitch;

	private void Awake()
	{
		_anim = base.gameObject.GetComponent<Animator>();
		_player = GameController.LocalClient.Player;
		_giantess = GetComponentInParent<Giantess>();
		_animationSpeedHash = Animator.StringToHash("animationSpeed");
		Transform boneTransform = _anim.GetBoneTransform(HumanBodyBones.LeftHand);
		if ((bool)boneTransform)
		{
			leftHand = boneTransform;
			leftHandPosition = leftHand.position;
			if (!_handWind)
			{
				_handWind = Resources.Load<AudioClip>("Sound/Giantess/windhands");
			}
			_leftHandSource = CreateAudioSource(boneTransform);
			_leftHandSource.clip = _handWind;
			_leftHandSource.Play();
		}
		boneTransform = _anim.GetBoneTransform(HumanBodyBones.RightHand);
		if ((bool)boneTransform)
		{
			rightHand = boneTransform;
			rightHandPosition = rightHand.position;
			if (!_handWind)
			{
				_handWind = Resources.Load<AudioClip>("Sound/Giantess/windhands");
			}
			_rightHandSource = CreateAudioSource(boneTransform);
			_rightHandSource.clip = _handWind;
			_rightHandSource.Play();
		}
		boneTransform = _anim.GetBoneTransform(HumanBodyBones.LeftFoot);
		if ((bool)boneTransform)
		{
			leftFoot = boneTransform;
			leftFootPosition = leftFoot.position;
			if (!_footWind)
			{
				_footWind = Resources.Load<AudioClip>("Sound/Giantess/windfeet");
			}
			_leftFootSource = CreateAudioSource(boneTransform);
			_leftFootSource.clip = _footWind;
			_leftFootSource.Play();
		}
		boneTransform = _anim.GetBoneTransform(HumanBodyBones.RightFoot);
		if ((bool)boneTransform)
		{
			rightFoot = boneTransform;
			rightFootPosition = rightFoot.position;
			if (!_footWind)
			{
				_footWind = Resources.Load<AudioClip>("Sound/Giantess/windfeet");
			}
			_rightFootSource = CreateAudioSource(boneTransform);
			_rightFootSource.clip = _footWind;
			_rightFootSource.Play();
		}
		boneTransform = _anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
		if ((bool)boneTransform)
		{
			leftLeg = boneTransform;
			leftLegPosition = leftLeg.position;
			if (!_legWind)
			{
				_legWind = Resources.Load<AudioClip>("Sound/Giantess/windlegs");
			}
			_leftLegSource = CreateAudioSource(boneTransform);
			_leftLegSource.clip = _legWind;
			_leftLegSource.Play();
		}
		boneTransform = _anim.GetBoneTransform(HumanBodyBones.RightLowerLeg);
		if ((bool)boneTransform)
		{
			rightLeg = boneTransform;
			rightLegPosition = rightLeg.position;
			if (!_legWind)
			{
				_legWind = Resources.Load<AudioClip>("Sound/Giantess/windlegs");
			}
			_rightLegSource = CreateAudioSource(boneTransform);
			_rightLegSource.clip = _legWind;
			_rightLegSource.Play();
		}
		boneTransform = _anim.GetBoneTransform(HumanBodyBones.Hips);
		if ((bool)boneTransform)
		{
			hipsTransform = boneTransform;
			hipsTransformPosition = hipsTransform.position;
			if (!_hipWind)
			{
				_hipWind = Resources.Load<AudioClip>("Sound/Giantess/windhips");
			}
			_hipSource = CreateAudioSource(boneTransform);
			_hipSource.clip = _hipWind;
			_hipSource.Play();
		}
		boneTransform = _anim.GetBoneTransform(HumanBodyBones.Head);
		if ((bool)boneTransform)
		{
			headTransform = boneTransform;
			headRotation = boneTransform.rotation;
			if (!_headWind)
			{
				_headWind = Resources.Load<AudioClip>("Sound/Giantess/windhead");
			}
			_headSource = CreateAudioSource(boneTransform);
			_headSource.clip = _headWind;
			_headSource.Play();
		}
		boneTransform = _anim.GetBoneTransform(HumanBodyBones.Chest);
		if ((bool)boneTransform)
		{
			chestTransform = boneTransform;
			chestTransformPosition = boneTransform.position;
			if (!_chestWind)
			{
				_chestWind = Resources.Load<AudioClip>("Sound/Giantess/windbreasts");
			}
			_chestSource = CreateAudioSource(boneTransform);
			_chestSource.clip = _chestWind;
			_chestSource.Play();
		}
	}

	private void Mute()
	{
		if ((bool)_hipSource)
		{
			_hipSource.volume = 0f;
		}
		if ((bool)_headSource)
		{
			_headSource.volume = 0f;
		}
		if ((bool)_chestSource)
		{
			_chestSource.volume = 0f;
		}
		if ((bool)_leftHandSource)
		{
			_leftHandSource.volume = 0f;
		}
		if ((bool)_rightHandSource)
		{
			_rightHandSource.volume = 0f;
		}
		if ((bool)_leftFootSource)
		{
			_leftFootSource.volume = 0f;
		}
		if ((bool)_rightFootSource)
		{
			_rightFootSource.volume = 0f;
		}
		if ((bool)_leftLegSource)
		{
			_leftLegSource.volume = 0f;
		}
		if ((bool)_rightLegSource)
		{
			_rightLegSource.volume = 0f;
		}
	}

	private void Update()
	{
		if (_giantess.IsPosed || SoundManager.WindVolume <= 0f)
		{
			Mute();
			return;
		}
		_size = _giantess.Scale * 1000f;
		_speed = Mathf.Clamp(0.175f, _anim.GetFloat(_animationSpeedHash), _size);
		_modifier = Mathf.Pow(_speed, 2f) * _size;
		_relationship = (1f - _player.Scale * 2.5f / _size) * SoundManager.WindVolume;
		_pitchAdjust = 1f / Mathf.Sqrt(_size / 125f + 1f);
		_modPitch = 0.75f * _pitchAdjust + 0.9f;
		_legPitch = 1.1f * _pitchAdjust + 0.9f;
		_headPitch = 0.9f * _pitchAdjust + 0.9f;
		float num = 0f;
		if ((bool)_hipSource)
		{
			Vector3 position = hipsTransform.position;
			_hipDisplacement = Vector3.Distance(hipsTransformPosition, position) / _modifier;
			hipsTransformPosition = position;
			_hipSource.maxDistance = hipDistance * _scale;
			_hipSource.minDistance = _hipSource.maxDistance * minDistancePercentage;
			_hipSource.pitch = _legPitch;
			_hipSource.volume = Mathf.Lerp(_hipSource.volume, _relationship * 20f * (_hipDisplacement - 0.001f), Time.deltaTime * 8f);
			num += _hipSource.volume;
			if (num > 1f)
			{
				return;
			}
		}
		if ((bool)_headSource)
		{
			Quaternion localRotation = headTransform.localRotation;
			_headRotationalDisplacement = Quaternion.Angle(headRotation, localRotation) / (_modifier / _size * 180f);
			headRotation = localRotation;
			_headSource.maxDistance = headDistance * _scale;
			_headSource.minDistance = _headSource.maxDistance * minDistancePercentage;
			_headSource.pitch = _headPitch;
			_headSource.volume = Mathf.Lerp(_headSource.volume, _relationship * 22f * (_headRotationalDisplacement - 0.00325f), Time.deltaTime * 8f);
			num += _hipSource.volume;
		}
		if ((bool)_chestSource)
		{
			Vector3 position2 = chestTransform.position;
			_chestDisplacement = Vector3.Distance(chestTransformPosition, position2) / _modifier;
			chestTransformPosition = position2;
			_chestSource.maxDistance = chestDistance * _scale;
			_chestSource.minDistance = _hipSource.maxDistance * minDistancePercentage;
			_chestSource.pitch = _legPitch;
			_chestSource.volume = Mathf.Lerp(_chestSource.volume, _relationship * 20f * (_chestDisplacement - 0.001f), Time.deltaTime * 8f);
			num += _hipSource.volume;
		}
		if (!(num > 1f))
		{
			if ((bool)_leftHandSource)
			{
				Vector3 position3 = leftHand.position;
				_leftHandDisplacement = Vector3.Distance(leftHandPosition, position3) / _modifier;
				leftHandPosition = position3;
				_leftHandSource.maxDistance = leftHandDistance * _scale;
				_leftHandSource.minDistance = _leftHandSource.maxDistance * minDistancePercentage;
				_leftHandSource.pitch = _modPitch;
				_leftHandSource.volume = Mathf.Lerp(_leftHandSource.volume, _relationship * 14f * (_leftHandDisplacement - 0.001f), Time.deltaTime * 8f);
			}
			if ((bool)_rightHandSource)
			{
				Vector3 position4 = rightHand.position;
				_rightHandDisplacement = Vector3.Distance(rightHandPosition, position4) / _modifier;
				rightHandPosition = position4;
				_rightHandSource.maxDistance = rightHandDistance * _scale;
				_rightHandSource.minDistance = _rightHandSource.maxDistance * minDistancePercentage;
				_rightHandSource.pitch = _modPitch;
				_rightHandSource.volume = Mathf.Lerp(_rightHandSource.volume, _relationship * 14f * (_rightHandDisplacement - 0.001f), Time.deltaTime * 8f);
			}
			if ((bool)_leftFootSource)
			{
				Vector3 position5 = leftFoot.position;
				_leftFootDisplacement = Mathf.Abs(leftFootPosition.y - position5.y) / _modifier;
				leftFootPosition = position5;
				_leftFootSource.maxDistance = leftFootDistance * _scale;
				_leftFootSource.minDistance = _leftFootSource.maxDistance * minDistancePercentage;
				_leftFootSource.pitch = _modPitch;
				_leftFootSource.volume = Mathf.Lerp(_leftFootSource.volume, _relationship * 64f * (_leftFootDisplacement - 0.00155f), Time.deltaTime * 8f);
			}
			if ((bool)_rightFootSource)
			{
				Vector3 position6 = rightFoot.position;
				_rightFootDisplacement = Mathf.Abs(rightFootPosition.y - position6.y) / _modifier;
				rightFootPosition = position6;
				_rightFootSource.maxDistance = rightFootDistance * _scale;
				_rightFootSource.minDistance = _rightFootSource.maxDistance * minDistancePercentage;
				_rightFootSource.pitch = _modPitch;
				_rightFootSource.volume = Mathf.Lerp(_rightFootSource.volume, _relationship * 64f * (_rightFootDisplacement - 0.00155f), Time.deltaTime * 8f);
			}
			if ((bool)_leftLegSource)
			{
				Vector3 position7 = leftLeg.position;
				_leftLegDisplacement = Vector3.Distance(leftLegPosition, position7) / _modifier;
				leftLegPosition = position7;
				_leftLegSource.maxDistance = leftLegDistance * _scale;
				_leftLegSource.minDistance = _leftLegSource.maxDistance * minDistancePercentage;
				_leftLegSource.pitch = _legPitch;
				_leftLegSource.volume = Mathf.Lerp(_leftLegSource.volume, _relationship * 12.5f * (_leftLegDisplacement - 0.000875f), Time.deltaTime * 7f);
			}
			if ((bool)_rightLegSource)
			{
				Vector3 position8 = rightLeg.position;
				_rightLegDisplacement = Vector3.Distance(rightLegPosition, position8) / _modifier;
				rightLegPosition = position8;
				_rightLegSource.maxDistance = rightLegDistance * _scale;
				_rightLegSource.minDistance = _rightLegSource.maxDistance * minDistancePercentage;
				_rightLegSource.pitch = _legPitch;
				_rightLegSource.volume = Mathf.Lerp(_rightLegSource.volume, _relationship * 12.5f * (_rightLegDisplacement - 0.000875f), Time.deltaTime * 7f);
			}
		}
	}

	public void UpdateAudioScale(float scale)
	{
		_scale = scale;
	}

	private AudioSource CreateAudioSource(Transform bone)
	{
		AudioSource audioSource = bone.gameObject.AddComponent<AudioSource>();
		audioSource.dopplerLevel = 0f;
		audioSource.spatialBlend = 1f;
		audioSource.volume = 0.35f;
		audioSource.outputAudioMixerGroup = SoundManager.AudioMixerWingMacro;
		audioSource.loop = true;
		return audioSource;
	}
}
