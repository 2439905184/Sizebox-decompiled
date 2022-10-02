using Sizebox.CharacterEditor;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ComplexMicroLookAtController : MicroLookAtController
{
	private Transform assistReference;

	protected override void Awake()
	{
		base.Awake();
		GameObject gameObject = new GameObject();
		gameObject.AddComponent<NotABone>();
		assistReference = gameObject.transform;
		assistReference.position = head.position + base.transform.forward;
		assistReference.parent = head;
		base.enabled = false;
	}

	protected override void LateUpdate()
	{
		if (active && targetEntity == null)
		{
			active = false;
		}
		if (active)
		{
			if (Mathf.Abs(Vector3.Angle(base.transform.forward, base.target - head.position)) < 85f)
			{
				Vector3 fromDirection = assistReference.position - head.position;
				Vector3 toDirection = base.target - head.position;
				Quaternion b = Quaternion.FromToRotation(fromDirection, toDirection) * head.rotation;
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

	private void OnDestroy()
	{
		if (assistReference != null)
		{
			Object.Destroy(assistReference.gameObject);
		}
	}
}
