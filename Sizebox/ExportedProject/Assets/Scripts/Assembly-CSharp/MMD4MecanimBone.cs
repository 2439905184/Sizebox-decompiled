using System;
using UnityEngine;

public class MMD4MecanimBone : MonoBehaviour
{
	public MMD4MecanimModel model;

	public int boneID = -1;

	public bool ikEnabled;

	public float ikWeight = 1f;

	[NonSerialized]
	public int humanBodyBones = -1;

	private MMD4MecanimData.BoneData _boneData;

	[NonSerialized]
	public Vector3 _userPosition = Vector3.zero;

	[NonSerialized]
	public Vector3 _userEulerAngles = Vector3.zero;

	[NonSerialized]
	public Quaternion _userRotation = Quaternion.identity;

	[NonSerialized]
	public bool _userPositionIsZero = true;

	[NonSerialized]
	public bool _userRotationIsIdentity = true;

	public MMD4MecanimData.BoneData boneData
	{
		get
		{
			return _boneData;
		}
	}

	public Vector3 userPosition
	{
		get
		{
			return _userPosition;
		}
		set
		{
			if (_userPosition != value)
			{
				_userPosition = value;
				_userPositionIsZero = MMD4MecanimCommon.FuzzyZero(value);
			}
		}
	}

	public Vector3 userEulerAngles
	{
		get
		{
			return _userEulerAngles;
		}
		set
		{
			if (_userEulerAngles != value)
			{
				if (MMD4MecanimCommon.FuzzyZero(value))
				{
					_userRotation = Quaternion.identity;
					_userEulerAngles = Vector3.zero;
					_userRotationIsIdentity = true;
				}
				else
				{
					_userRotation = Quaternion.Euler(value);
					_userEulerAngles = value;
					_userRotationIsIdentity = false;
				}
			}
		}
	}

	public Quaternion userRotation
	{
		get
		{
			return _userRotation;
		}
		set
		{
			if (_userRotation != value)
			{
				if (MMD4MecanimCommon.FuzzyIdentity(value))
				{
					_userRotation = Quaternion.identity;
					_userEulerAngles = Vector3.zero;
					_userRotationIsIdentity = true;
				}
				else
				{
					_userRotation = value;
					_userEulerAngles = value.eulerAngles;
					_userRotationIsIdentity = false;
				}
			}
		}
	}

	public void Setup()
	{
		if (!(model == null) && model.modelData != null && model.modelData.boneDataList != null && boneID >= 0 && boneID < model.modelData.boneDataList.Length)
		{
			_boneData = model.modelData.boneDataList[boneID];
		}
	}

	public void Bind()
	{
		if (!(model == null))
		{
			MMD4MecanimData.BoneData boneDatum = _boneData;
		}
	}

	public void Destroy()
	{
		_boneData = null;
	}
}
