using UnityEngine;

[ExecuteInEditMode]
public class AutoConfigureNpc : MonoBehaviour
{
	public float height = 1.7f;

	public float radius = 0.2f;

	private CapsuleCollider _capsuleCollider;

	private Rigidbody _rigid;

	private MicroNpc _controller;

	private Animator _anim;

	private void Start()
	{
		_rigid = base.gameObject.GetComponent<Rigidbody>();
		_capsuleCollider = base.gameObject.GetComponent<CapsuleCollider>();
		_controller = base.gameObject.GetComponent<MicroNpc>();
		_anim = base.gameObject.GetComponent<Animator>();
		if (_rigid == null)
		{
			_rigid = base.gameObject.AddComponent<Rigidbody>();
		}
		if (_capsuleCollider == null)
		{
			_capsuleCollider = base.gameObject.AddComponent<CapsuleCollider>();
		}
		if (_controller == null)
		{
			_controller = base.gameObject.AddComponent<MicroNpc>();
		}
		base.gameObject.layer = LayerMask.NameToLayer("NPC");
	}

	private void Update()
	{
		_capsuleCollider.height = height;
		_capsuleCollider.center = new Vector3(0f, height / 2f, 0f);
		_capsuleCollider.radius = radius;
		_rigid.constraints = RigidbodyConstraints.FreezeRotation;
		RuntimeAnimatorController microAnimatorController = IOManager.Instance.microAnimatorController;
		_anim.SetRuntimeController(microAnimatorController);
	}
}
