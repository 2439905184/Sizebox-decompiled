using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Gravity : EntityComponent
{
	public static float GravityDensity = 9.8f;

	public float baseScale = 1f;

	private EntityBase _entity;

	private Rigidbody _rigidbody;

	public override void Initialize(EntityBase entity)
	{
		_entity = entity;
		_rigidbody = GetComponent<Rigidbody>();
		_rigidbody.useGravity = false;
		base.initialized = true;
	}

	public void PauseForSeconds(float seconds)
	{
		base.initialized = false;
		StartCoroutine(Unpause(seconds));
	}

	private IEnumerator Unpause(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		base.initialized = true;
	}

	private void FixedUpdate()
	{
		if (base.initialized)
		{
			float accurateScale = _entity.AccurateScale;
			_rigidbody.AddForce(new Vector3(0f, (0f - GravityDensity) * accurateScale, 0f), ForceMode.Acceleration);
		}
	}
}
