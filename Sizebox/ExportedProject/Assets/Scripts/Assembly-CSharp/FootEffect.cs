using UnityEngine;

public class FootEffect : MonoBehaviour
{
	private Giantess _giantess;

	private static GameObject _dustPrefab;

	private Animator _modelAnimator;

	private AudioSource _audioSource;

	private float _scale;

	public float destructionRadius = 100f;

	private Transform _leftFoot;

	private Transform _rightFoot;

	private ParticleSystem _dustEmitter;

	private ParticleSystem.MainModule _mainModule;

	private ParticleSystem.ShapeModule _shape;

	private void Awake()
	{
		_modelAnimator = base.gameObject.GetComponent<Animator>();
		_leftFoot = _modelAnimator.GetBoneTransform(HumanBodyBones.LeftFoot);
		_rightFoot = _modelAnimator.GetBoneTransform(HumanBodyBones.RightFoot);
		if (!_leftFoot || !_rightFoot)
		{
			Object.Destroy(this);
		}
	}

	private void Start()
	{
		_audioSource = base.gameObject.AddComponent<AudioSource>();
		_audioSource.spatialBlend = 1f;
		_audioSource.dopplerLevel = 0f;
		_audioSource.outputAudioMixerGroup = SoundManager.AudioMixerMacro;
		if (_dustPrefab == null)
		{
			_dustPrefab = Resources.Load<GameObject>("Particles/StepDust");
		}
		_giantess = GetComponentInParent<Giantess>();
	}

	public void OnStep(AnimationEvent e)
	{
		_scale = _giantess.Scale;
		float magnitude = ((e.floatParameter == 0f) ? 1f : e.floatParameter);
		Vector3 vector = ((e.intParameter == 0) ? _leftFoot.position : _rightFoot.position);
		vector += base.transform.forward * (destructionRadius * _scale) / 2f;
		float maxDistance = (vector.y - _giantess.transform.position.y) * 1.25f;
		RaycastHit hitInfo;
		if (Physics.Raycast(vector, Vector3.down, out hitInfo, maxDistance, Layers.gtsWalkableMask))
		{
			vector = hitInfo.point;
		}
		DoStep(vector, magnitude, e.intParameter);
	}

	public void DoStep(Vector3 epicenter, float magnitude, int foot)
	{
		_scale = _giantess.Scale;
		EventManager.SendEvent(new StepEvent(_giantess, epicenter, magnitude, foot));
		VisualEffect(epicenter, magnitude);
		if ((GlobalPreferences.GtsPlayerBuildingDestruction.value || !_giantess.isPlayer) && (GlobalPreferences.GtsBuildingDestruction.value || _giantess.isPlayer))
		{
			DestructionEffect(epicenter, magnitude);
		}
	}

	private void VisualEffect(Vector3 epicenter, float magnitude)
	{
		if (GameController.IsMacroMap && !((double)magnitude < 0.3))
		{
			GameObject gameObject = Object.Instantiate(_dustPrefab, epicenter, Quaternion.identity);
			_dustEmitter = gameObject.transform.GetChild(0).GetComponent<ParticleSystem>();
			_mainModule = _dustEmitter.main;
			_shape = _dustEmitter.shape;
			gameObject.transform.position = epicenter;
			_mainModule.startSize = _scale * magnitude * 100f;
			_shape.radius = _scale * magnitude * 100f;
			_dustEmitter.Play();
			Object.Destroy(gameObject, _mainModule.duration + _mainModule.startLifetime.constant);
		}
	}

	private void DestructionEffect(Vector3 epicenter, float magnitude)
	{
		Collider[] array = Physics.OverlapSphere(epicenter, destructionRadius * _scale, Layers.buildingMask);
		foreach (Collider collider in array)
		{
			if (!collider)
			{
				continue;
			}
			IDamagable componentInParent = collider.GetComponentInParent<IDamagable>();
			if (componentInParent == null)
			{
				continue;
			}
			float num = _giantess.AccurateScale * 4f;
			float mass = _giantess.Rigidbody.mass;
			ICrushable crushable;
			if ((crushable = componentInParent as ICrushable) != null)
			{
				Vector3 vector = epicenter + Vector3.up * (_giantess.AccurateScale * 0.1f);
				if (crushable.TryToCrush((collider.transform.position - vector).normalized * (mass * num), _giantess))
				{
					continue;
				}
			}
			IDestructible destructible;
			if ((destructible = componentInParent as IDestructible) == null || !collider || !destructible.TryToDestroy(magnitude * _giantess.Height, epicenter))
			{
				componentInParent.Damage(Sbox.CalculateDamageDealt(num * mass));
			}
		}
	}

	public void UpdateScale(float footScale)
	{
		_scale = footScale;
	}
}
