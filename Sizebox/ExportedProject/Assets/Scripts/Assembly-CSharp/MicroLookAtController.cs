using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MicroLookAtController : MonoBehaviour
{
	protected EntityBase targetEntity;

	private Transform targetHead;

	protected Transform head;

	protected bool active;

	protected const float maxLookAngle = 85f;

	protected const float headTurnSpeed = 3f;

	protected const float minAngleForHeadLerp = 3f;

	protected Quaternion prevRot;

	protected Vector3 target
	{
		get
		{
			if (!targetEntity)
			{
				return Vector3.zero;
			}
			if (!(targetHead != null))
			{
				return targetEntity.transform.position + Vector3.up * targetEntity.Height * 0.9f;
			}
			return targetHead.position;
		}
	}

	protected virtual void Awake()
	{
		head = GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Head);
		base.enabled = false;
	}

	public void LookAt(EntityBase entity)
	{
		targetEntity = entity;
		if (targetEntity != null)
		{
			Animator component = targetEntity.gameObject.GetComponent<Animator>();
			targetHead = ((component != null) ? component.GetBoneTransform(HumanBodyBones.Head) : null);
		}
		active = targetEntity != null;
		if (prevRot == Quaternion.identity)
		{
			prevRot = head.rotation;
		}
	}

	protected void OnEnable()
	{
		prevRot = Quaternion.identity;
	}

	protected virtual void LateUpdate()
	{
		if (active && targetEntity == null)
		{
			active = false;
		}
		if (active)
		{
			if (Mathf.Abs(Vector3.Angle(base.transform.forward, target - head.position)) < 85f)
			{
				Quaternion b = Quaternion.LookRotation(target - head.position);
				head.rotation = Quaternion.Slerp(prevRot, b, Time.deltaTime * 3f);
			}
			else
			{
				head.rotation = Quaternion.Slerp(prevRot, head.rotation, Time.deltaTime * 3f);
			}
			prevRot = head.rotation;
		}
		else if (Quaternion.Angle(prevRot, head.rotation) > 3f)
		{
			head.rotation = Quaternion.Slerp(prevRot, head.rotation, Time.deltaTime * 3f);
			prevRot = head.rotation;
		}
		else
		{
			base.enabled = false;
		}
	}
}
