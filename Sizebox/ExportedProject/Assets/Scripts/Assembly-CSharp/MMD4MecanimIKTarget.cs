using UnityEngine;

public class MMD4MecanimIKTarget : MonoBehaviour
{
	public MMD4MecanimModel model;

	public bool ikEnabled = true;

	public float ikWeight = 1f;

	private Transform _transform;

	public Matrix4x4 localToWorldMatrix
	{
		get
		{
			if (_transform == null)
			{
				_transform = base.transform;
			}
			return _transform.localToWorldMatrix;
		}
	}
}
