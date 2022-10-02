using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MMD4MecanimMorphHelper : MonoBehaviour
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Predicate<MMD4MecanimModel.Morph> _003C_003E9__16_0;

		internal bool _003C_UpdateMorph_003Eb__16_0(MMD4MecanimModel.Morph s)
		{
			if (s.weight == 0f)
			{
				return s.weight2 == 0f;
			}
			return false;
		}
	}

	public float morphSpeed = 0.1f;

	public string morphName;

	public float morphWeight;

	public bool overrideWeight;

	protected MMD4MecanimModel _model;

	private MMD4MecanimModel.Morph _modelMorph;

	private float _morphTime;

	private HashSet<MMD4MecanimModel.Morph> _inactiveModelMorphSet = new HashSet<MMD4MecanimModel.Morph>();

	private float _weight2;

	public virtual bool isProcessing
	{
		get
		{
			if (_modelMorph != null && _modelMorph.weight != morphWeight)
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
			if (_modelMorph != null && _modelMorph.weight != morphWeight)
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

	protected virtual void Start()
	{
		_model = GetComponent<MMD4MecanimModel>();
		if (_model != null)
		{
			_model.Initialize();
		}
	}

	protected virtual void Update()
	{
		_UpdateMorph(Time.deltaTime);
	}

	public virtual void ForceUpdate()
	{
		_UpdateMorph(0f);
	}

	private void _UpdateMorph(float deltaTime)
	{
		_UpdateModelMorph();
		float step = 1f;
		if (morphSpeed > 0f)
		{
			step = deltaTime / morphSpeed;
		}
		if (_modelMorph != null)
		{
			MMD4MecanimCommon.Approx(ref _modelMorph.weight, morphWeight, step);
			MMD4MecanimCommon.Approx(ref _weight2, overrideWeight ? 1f : 0f, step);
			_modelMorph.weight2 = _weight2;
		}
		else
		{
			MMD4MecanimCommon.Approx(ref _weight2, 1f, step);
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
		_inactiveModelMorphSet.RemoveWhere(_003C_003Ec._003C_003E9__16_0 ?? (_003C_003Ec._003C_003E9__16_0 = _003C_003Ec._003C_003E9._003C_UpdateMorph_003Eb__16_0));
	}

	private void _UpdateModelMorph()
	{
		if (_modelMorph != null && (string.IsNullOrEmpty(morphName) || _modelMorph.name != morphName))
		{
			if (_modelMorph.weight != 0f || _modelMorph.weight2 != 0f)
			{
				_inactiveModelMorphSet.Add(_modelMorph);
			}
			_modelMorph = null;
		}
		if (_modelMorph == null && _model != null)
		{
			_modelMorph = _model.GetMorph(morphName);
			if (_modelMorph != null && _inactiveModelMorphSet != null)
			{
				_inactiveModelMorphSet.Remove(_modelMorph);
			}
		}
	}
}
