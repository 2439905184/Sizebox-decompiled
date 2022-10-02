using UnityEngine;

public class RootMotionTransfer : MonoBehaviour
{
	private Animator myAnimator;

	private EntityBase myEntity;

	private Micro myMicro;

	private Rigidbody _rigidbody;

	private Vector3 delta;

	private Quaternion deltaRotation = Quaternion.identity;

	private void Awake()
	{
		myEntity = GetComponentInParent<EntityBase>();
		myMicro = myEntity as Micro;
		myAnimator = GetComponent<Animator>();
		_rigidbody = myEntity.Rigidbody;
	}

	private void OnAnimatorMove()
	{
		delta += myAnimator.deltaPosition;
		deltaRotation = myAnimator.deltaRotation * deltaRotation;
	}

	private void FixedUpdate()
	{
		if ((bool)_rigidbody)
		{
			_rigidbody.MovePosition(_rigidbody.position + delta);
			_rigidbody.MoveRotation(deltaRotation * _rigidbody.rotation);
		}
		delta = Vector3.zero;
		deltaRotation = Quaternion.identity;
	}

	private void OnStep(AnimationEvent e)
	{
		if ((bool)myMicro)
		{
			myMicro.OnStep(e);
		}
	}
}
