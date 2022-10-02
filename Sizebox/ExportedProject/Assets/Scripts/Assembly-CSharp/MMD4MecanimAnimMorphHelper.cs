using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MMD4MecanimAnimMorphHelper : MonoBehaviour, MMD4MecanimAnim.IAnimModel
{
	[Serializable]
	public class Anim : MMD4MecanimAnim.IAnim
	{
		public string animName;

		public TextAsset animFile;

		public AudioClip audioClip;

		[NonSerialized]
		public MMD4MecanimData.AnimData animData;

		[NonSerialized]
		public MMD4MecanimAnim.MorphMotion[] morphMotionList;

		string MMD4MecanimAnim.IAnim.animatorStateName
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		int MMD4MecanimAnim.IAnim.animatorStateNameHash
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		TextAsset MMD4MecanimAnim.IAnim.animFile
		{
			get
			{
				return animFile;
			}
			set
			{
				animFile = value;
			}
		}

		AudioClip MMD4MecanimAnim.IAnim.audioClip
		{
			get
			{
				return audioClip;
			}
			set
			{
				audioClip = value;
			}
		}

		MMD4MecanimData.AnimData MMD4MecanimAnim.IAnim.animData
		{
			get
			{
				return animData;
			}
			set
			{
				animData = value;
			}
		}

		MMD4MecanimAnim.MorphMotion[] MMD4MecanimAnim.IAnim.morphMotionList
		{
			get
			{
				return morphMotionList;
			}
			set
			{
				morphMotionList = value;
			}
		}
	}

	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Predicate<MMD4MecanimModel.Morph> _003C_003E9__53_0;

		internal bool _003C_UpdateMorph_003Eb__53_0(MMD4MecanimModel.Morph s)
		{
			if (s.weight == 0f)
			{
				return s.weight2 == 0f;
			}
			return false;
		}
	}

	public string animName = "";

	public string playingAnimName = "";

	public bool animEnabled = true;

	public bool animPauseOnEnd;

	public bool initializeOnAwake;

	public bool animSyncToAudio = true;

	public float morphSpeed = 0.1f;

	public bool overrideWeight;

	public Anim[] animList;

	private bool _initialized;

	private MMD4MecanimModel _model;

	private AudioSource _audioSource;

	private Anim _playingAnim;

	private float _animTime;

	private float _morphWeight;

	private float _weight2;

	private HashSet<MMD4MecanimModel.Morph> _inactiveModelMorphSet = new HashSet<MMD4MecanimModel.Morph>();

	public virtual bool isProcessing
	{
		get
		{
			if (_IsPlayingAnim())
			{
				return true;
			}
			if (_inactiveModelMorphSet.Count != 0)
			{
				return true;
			}
			return false;
		}
	}

	public virtual bool isAnimating
	{
		get
		{
			if (_IsPlayingAnim())
			{
				return true;
			}
			if (_inactiveModelMorphSet.Count != 0)
			{
				return true;
			}
			return false;
		}
	}

	int MMD4MecanimAnim.IAnimModel.morphCount
	{
		get
		{
			if (_model != null && _model.morphList != null)
			{
				return _model.morphList.Length;
			}
			return 0;
		}
	}

	int MMD4MecanimAnim.IAnimModel.animCount
	{
		get
		{
			if (animList != null)
			{
				return animList.Length;
			}
			return 0;
		}
	}

	Animator MMD4MecanimAnim.IAnimModel.animator
	{
		get
		{
			return null;
		}
	}

	AudioSource MMD4MecanimAnim.IAnimModel.audioSource
	{
		get
		{
			if (_audioSource == null)
			{
				_audioSource = MMD4MecanimCommon.WeakAddComponent<AudioSource>(base.gameObject);
			}
			return _audioSource;
		}
	}

	bool MMD4MecanimAnim.IAnimModel.animSyncToAudio
	{
		get
		{
			return animSyncToAudio;
		}
	}

	float MMD4MecanimAnim.IAnimModel.prevDeltaTime
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	MMD4MecanimAnim.IAnim MMD4MecanimAnim.IAnimModel.playingAnim
	{
		get
		{
			return _playingAnim;
		}
		set
		{
			_playingAnim = (Anim)value;
		}
	}

	public void PlayAnim(string animName)
	{
		this.animName = animName;
	}

	public void StopAnim()
	{
		playingAnimName = "";
	}

	MMD4MecanimAnim.IMorph MMD4MecanimAnim.IAnimModel.GetMorph(string name)
	{
		if (_model != null)
		{
			return _model.GetMorph(name);
		}
		return null;
	}

	MMD4MecanimAnim.IMorph MMD4MecanimAnim.IAnimModel.GetMorph(string name, bool startsWith)
	{
		if (_model != null)
		{
			return _model.GetMorph(name, startsWith);
		}
		return null;
	}

	MMD4MecanimAnim.IMorph MMD4MecanimAnim.IAnimModel.GetMorphAt(int index)
	{
		if (_model != null && _model.morphList != null && index >= 0 && index < _model.morphList.Length)
		{
			return _model.morphList[index];
		}
		return null;
	}

	MMD4MecanimAnim.IAnim MMD4MecanimAnim.IAnimModel.GetAnimAt(int index)
	{
		if (animList != null && index >= 0 && index < animList.Length)
		{
			return animList[index];
		}
		return null;
	}

	void MMD4MecanimAnim.IAnimModel._SetAnimMorphWeight(MMD4MecanimAnim.IMorph morph, float weight)
	{
		morph.weight = ((_morphWeight != 1f) ? (weight * _morphWeight) : weight);
	}

	private void Awake()
	{
		if (initializeOnAwake)
		{
			_Initialize();
		}
	}

	private void Start()
	{
		_Initialize();
	}

	private void Update()
	{
		_UpdateAnim();
		_UpdateMorph();
	}

	private void _Initialize()
	{
		if (!_initialized)
		{
			_initialized = true;
			_model = base.gameObject.GetComponent<MMD4MecanimModel>();
			if (!(_model == null))
			{
				_model.Initialize();
				MMD4MecanimAnim.InitializeAnimModel(this);
			}
		}
	}

	private void _UpdateAnim()
	{
		if (!animEnabled)
		{
			_StopAnim();
			return;
		}
		if (_playingAnim != null && (string.IsNullOrEmpty(playingAnimName) || _playingAnim.animName == null || playingAnimName != _playingAnim.animName))
		{
			_StopAnim();
		}
		bool flag = false;
		if (_playingAnim == null && !string.IsNullOrEmpty(animName) && animList != null)
		{
			for (int i = 0; i != animList.Length; i++)
			{
				if (animList[i].animName != null && animList[i].animName == animName)
				{
					_PlayAnim(animList[i]);
					flag = true;
					break;
				}
			}
		}
		if (_playingAnim != null)
		{
			if (_playingAnim.animData != null)
			{
				if (!animPauseOnEnd && _animTime >= (float)(_playingAnim.animData.maxFrame - 1) / 30f)
				{
					_StopAnim();
				}
			}
			else
			{
				_StopAnim();
			}
		}
		if (_playingAnim == null)
		{
			return;
		}
		if (!flag)
		{
			MMD4MecanimAnim.UpdateAnimModel(this, _playingAnim, _animTime);
		}
		if (_playingAnim == null)
		{
			return;
		}
		if (_playingAnim.morphMotionList != null)
		{
			for (int j = 0; j != _playingAnim.morphMotionList.Length; j++)
			{
				if (_playingAnim.morphMotionList[j].morph != null)
				{
					((MMD4MecanimModel.Morph)_playingAnim.morphMotionList[j].morph).weight2 = _weight2;
				}
			}
		}
		if (_playingAnim.audioClip != null && _audioSource != null && _audioSource.isPlaying && animSyncToAudio)
		{
			_animTime = _audioSource.time;
		}
		else
		{
			_animTime += Time.deltaTime;
		}
		if (_playingAnim.animData != null)
		{
			_animTime = Mathf.Min(_animTime, (float)_playingAnim.animData.maxFrame / 30f);
		}
		else
		{
			_animTime = 0f;
		}
	}

	private bool _IsPlayingAnim()
	{
		if (_playingAnim != null && _playingAnim.animData != null)
		{
			return _animTime < (float)(_playingAnim.animData.maxFrame - 1) / 30f;
		}
		return false;
	}

	private void _PlayAnim(Anim anim)
	{
		_StopAnim();
		_animTime = 0f;
		animName = "";
		if (anim != null)
		{
			playingAnimName = anim.animName;
		}
		MMD4MecanimAnim.UpdateAnimModel(this, anim, _animTime);
		if (_playingAnim == null || _inactiveModelMorphSet == null || _playingAnim.morphMotionList == null)
		{
			return;
		}
		for (int i = 0; i != _playingAnim.morphMotionList.Length; i++)
		{
			_playingAnim.morphMotionList[i].lastKeyFrameIndex = 0;
			MMD4MecanimModel.Morph morph = (MMD4MecanimModel.Morph)_playingAnim.morphMotionList[i].morph;
			if (morph != null)
			{
				_inactiveModelMorphSet.Remove(morph);
			}
		}
	}

	private void _StopAnim()
	{
		if (_playingAnim != null && _inactiveModelMorphSet != null && _playingAnim.morphMotionList != null)
		{
			for (int i = 0; i != _playingAnim.morphMotionList.Length; i++)
			{
				_playingAnim.morphMotionList[i].lastKeyFrameIndex = 0;
				MMD4MecanimModel.Morph morph = (MMD4MecanimModel.Morph)_playingAnim.morphMotionList[i].morph;
				if (morph != null && (morph.weight != 0f || morph.weight2 != 0f))
				{
					_inactiveModelMorphSet.Add(morph);
				}
			}
		}
		MMD4MecanimAnim.StopAnimModel(this);
		_animTime = 0f;
		playingAnimName = "";
	}

	private void _UpdateMorph()
	{
		float step = 1f;
		if (morphSpeed > 0f)
		{
			step = Time.deltaTime / morphSpeed;
		}
		if (_playingAnim != null)
		{
			MMD4MecanimCommon.Approx(ref _morphWeight, 1f, step);
			MMD4MecanimCommon.Approx(ref _weight2, overrideWeight ? 1f : 0f, step);
		}
		else
		{
			MMD4MecanimCommon.Approx(ref _morphWeight, 0f, step);
			MMD4MecanimCommon.Approx(ref _weight2, 0f, step);
		}
		if (_inactiveModelMorphSet == null)
		{
			return;
		}
		foreach (MMD4MecanimModel.Morph item in _inactiveModelMorphSet)
		{
			MMD4MecanimCommon.Approx(ref item.weight, 0f, step);
			MMD4MecanimCommon.Approx(ref item.weight2, 0f, step);
		}
		_inactiveModelMorphSet.RemoveWhere(_003C_003Ec._003C_003E9__53_0 ?? (_003C_003Ec._003C_003E9__53_0 = _003C_003Ec._003C_003E9._003C_UpdateMorph_003Eb__53_0));
	}
}
