using RootMotion;
using RootMotion.FinalIK;
using UnityEngine;

public class PlayerIK : MonoBehaviour
{
	private FullBodyBipedIK _ik;

	[SerializeField]
	private AimIK aimIk;

	private AnimatedMicroNPC _playerMicro;

	private NewPlayerMicroController _microController;

	private bool _ikEnabled = true;

	private float _weight;

	public float weightWhenWalking = 0.3f;

	private bool _ikSetUp;

	private bool _hipsLocked;

	private float _cachedHipWeight;

	public void Initialize(AnimatedMicroNPC micro, NewPlayerMicroController microController)
	{
		_playerMicro = micro;
		_microController = microController;
		BipedReferences references = new BipedReferences();
		BipedReferences.AutoDetectReferences(ref references, _playerMicro.model.transform, BipedReferences.AutoDetectParams.Default);
		if (references.isFilled)
		{
			_ik = _playerMicro.model.AddComponent<FullBodyBipedIK>();
			if ((bool)references.root)
			{
				_ik.SetReferences(references, null);
				_ik.solver.iterations = 0;
				_ik.solver.SetLimbOrientations(BipedLimbOrientations.UMA);
				aimIk = _playerMicro.model.AddComponent<AimIK>();
				aimIk.solver.SetChain(_ik.solver.spineMapping.spineBones, _playerMicro.model.transform);
				aimIk.GetIKSolver().Initiate(_playerMicro.model.transform);
				aimIk.GetIKSolver().SetIKPositionWeight(1f);
				aimIk.enabled = false;
				Object.Destroy(_ik);
				_ik = null;
				_ikSetUp = false;
			}
		}
	}

	private void Update()
	{
		if (_ikSetUp)
		{
			float b = 0f;
			bool flag = !_microController.IsFlying && !_microController.IsClimbing;
			if (_ikEnabled && !flag)
			{
				_ikEnabled = false;
			}
			else if (!_ikEnabled && flag)
			{
				_ikEnabled = true;
			}
			if (_ikEnabled)
			{
				b = (_microController.IsMoving ? weightWhenWalking : 1f);
			}
			_weight = Mathf.Lerp(_weight, b, Time.deltaTime * 2f);
		}
	}

	public AimIK GetAimIk()
	{
		return aimIk;
	}

	public void LockHips()
	{
		if (!_hipsLocked)
		{
			_cachedHipWeight = aimIk.solver.bones[0].weight;
			aimIk.solver.bones[0].weight = 0f;
			_hipsLocked = true;
		}
	}

	public void UnlockHips()
	{
		if (_hipsLocked)
		{
			aimIk.solver.bones[0].weight = _cachedHipWeight;
			_hipsLocked = false;
		}
	}
}
