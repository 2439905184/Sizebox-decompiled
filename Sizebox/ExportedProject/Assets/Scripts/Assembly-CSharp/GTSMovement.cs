using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GTSMovement : MonoBehaviour
{
	public enum MacroMoveState
	{
		Move = 0,
		OnlyMoveWithPhysics = 1,
		ResetTransformPosition = 2,
		DoNotMove = 3
	}

	[Header("Required References")]
	public Giantess giantess;

	[SerializeField]
	private CapsuleCollider capsuleCollider;

	[FormerlySerializedAs("rbody")]
	[SerializeField]
	private Rigidbody rigidBody;

	[SerializeField]
	private Gravity gravity;

	private bool _terrainColliderActive = true;

	private TerrainCollider _terrainCollider;

	private float _terrainScale = 20f;

	private GiantessIK _ik;

	public MacroMoveState moveState;

	private bool onlyMoveWithPhysics
	{
		get
		{
			return moveState == MacroMoveState.OnlyMoveWithPhysics;
		}
	}

	public bool doNotMoveGts
	{
		get
		{
			return moveState == MacroMoveState.DoNotMove;
		}
		set
		{
			moveState = (value ? MacroMoveState.DoNotMove : MacroMoveState.ResetTransformPosition);
		}
	}

	public void SetGiantess(Giantess gts)
	{
		giantess = gts;
		rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
		base.transform.position = gts.transform.position;
	}

	private void Awake()
	{
		_terrainCollider = UnityEngine.Object.FindObjectOfType<TerrainCollider>();
	}

	private void Update()
	{
		if (!GameController.Instance.paused)
		{
			if (Math.Abs(base.transform.lossyScale.y - giantess.Scale) > float.Epsilon)
			{
				base.transform.localScale = Vector3.one * giantess.Scale;
			}
			UpdateCharacterPosition();
			CollisionChoose();
		}
	}

	private void CollisionChoose()
	{
		if ((bool)_terrainCollider && (bool)capsuleCollider)
		{
			float y = base.transform.lossyScale.y;
			if (_terrainColliderActive && y > _terrainScale)
			{
				Physics.IgnoreCollision(capsuleCollider, _terrainCollider, true);
				_terrainColliderActive = false;
				DisableGrounder(true);
			}
			else if (!_terrainColliderActive && y < _terrainScale)
			{
				Physics.IgnoreCollision(capsuleCollider, _terrainCollider, false);
				_terrainColliderActive = true;
				DisableGrounder(false);
			}
		}
	}

	private void DisableGrounder(bool disable)
	{
		if (!_ik)
		{
			_ik = giantess.GetComponent<GiantessIK>();
		}
	}

	private void UpdateCharacterPosition()
	{
		float deltaTime = Time.deltaTime;
		Transform transform = base.transform;
		if (onlyMoveWithPhysics || giantess.Movement.move)
		{
			MoveTransformToCapsule();
			giantess.transform.rotation = Quaternion.Slerp(giantess.transform.rotation, transform.rotation, 10f * deltaTime);
		}
		else
		{
			MoveCapsuleToTransform();
			transform.rotation = giantess.transform.rotation;
		}
	}

	private void MoveCapsuleToTransform()
	{
		Vector3 position = giantess.transform.position;
		rigidBody.position = position;
	}

	private void MoveTransformToCapsule()
	{
		switch (moveState)
		{
		case MacroMoveState.Move:
		case MacroMoveState.OnlyMoveWithPhysics:
		{
			float deltaTime = Time.deltaTime;
			Vector3 worldPos = Vector3.Lerp(giantess.transform.position, base.transform.position, deltaTime * 10f);
			giantess._MoveMesh(worldPos);
			break;
		}
		case MacroMoveState.ResetTransformPosition:
			base.transform.position = giantess.transform.position;
			moveState = MacroMoveState.Move;
			break;
		}
	}

	public void EnableCollider(bool enable)
	{
		capsuleCollider.enabled = enable;
		gravity.enabled = enable;
	}
}
