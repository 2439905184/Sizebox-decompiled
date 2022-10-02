using UnityEngine;

public class SkeletonEditTransformHandle : MonoBehaviour
{
	private const float GIANTESS_SCALE_MULTIPLIER = 1000f;

	private const float HANDLE_SIZE = 0.02f;

	[SerializeField]
	private Transform visuals;

	private Transform targetTransform;

	private float scale;

	public Transform TargetTransform
	{
		get
		{
			return targetTransform;
		}
	}

	public void AssignTransform(Transform transform, Humanoid target)
	{
		targetTransform = transform;
		scale = 0.02f;
		if (target.isGiantess)
		{
			scale *= 1000f;
		}
		base.transform.SetParent(target.transform);
		UpdatePosition();
	}

	public void UpdatePosition()
	{
		if ((bool)targetTransform)
		{
			base.transform.position = targetTransform.position;
			base.transform.localScale = Vector3.one * scale;
		}
	}

	private void OnDisable()
	{
		targetTransform = null;
	}
}
