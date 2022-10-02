using RootMotion.FinalIK;
using UnityEngine;

public class PoseHandle : MonoBehaviour
{
	private IPosable target;

	private IKEffector effector;

	private IKConstraintBend bend;

	private IKSolverLookAt lookAt;

	private LineRenderer line;

	private const float LINE_WIDTH = 0.025f;

	public void SetEffector(IPosable target, IKEffector effector, Transform parentBone, float scale)
	{
		this.target = target;
		this.effector = effector;
		bend = null;
		base.gameObject.SetActive(true);
		base.transform.SetParent(parentBone);
		SetScale(scale);
		UpdateHandlePosition();
	}

	public void SetBendGoal(IPosable target, IKConstraintBend bend, Transform parentBone, float scale)
	{
		this.target = target;
		this.bend = bend;
		effector = null;
		if (line == null)
		{
			line = base.gameObject.AddComponent<LineRenderer>();
		}
		line.positionCount = 2;
		line.material = GetComponentInChildren<Renderer>().material;
		line.startColor = Color.yellow;
		line.endColor = Color.yellow;
		base.gameObject.SetActive(true);
		base.transform.SetParent(parentBone);
		SetScale(scale);
		UpdateHandlePosition();
	}

	public void SetLookAt(IPosable target, IKSolverLookAt lookAt, Transform parentBone, float scale)
	{
		this.target = target;
		this.lookAt = lookAt;
		if (line == null)
		{
			line = base.gameObject.AddComponent<LineRenderer>();
		}
		line.positionCount = 2;
		line.material = GetComponentInChildren<Renderer>().material;
		line.startColor = Color.yellow;
		line.endColor = Color.yellow;
		base.gameObject.SetActive(true);
		base.transform.SetParent(parentBone);
		SetScale(scale);
		UpdateHandlePosition();
	}

	private void SetScale(float scale)
	{
		Transform parent = base.transform.parent;
		base.transform.parent = null;
		base.transform.localScale = new Vector3(scale, scale, scale);
		base.transform.SetParent(parent, true);
	}

	private void Update()
	{
		if (effector == null && bend == null && lookAt == null)
		{
			base.gameObject.SetActive(false);
			return;
		}
		if (bend != null)
		{
			Vector3 position = bend.bone2.position;
			bend.bendGoal.position = base.transform.position;
			bend.bendGoal.rotation = base.transform.rotation;
			line.SetPosition(0, position);
			line.SetPosition(1, bend.bendGoal.position);
			line.widthMultiplier = target.AccurateScale * 0.025f;
		}
		else if (lookAt != null)
		{
			Vector3 position2 = lookAt.head.transform.position;
			lookAt.IKPosition = base.transform.position;
			line.SetPosition(0, position2);
			line.SetPosition(1, lookAt.IKPosition);
			line.widthMultiplier = target.AccurateScale * 0.025f;
		}
		UpdateEffectorPosition();
	}

	private void UpdateEffectorPosition()
	{
		if (effector != null)
		{
			effector.position = base.transform.position;
			effector.rotation = base.transform.rotation;
		}
	}

	private void UpdateHandlePosition()
	{
		if (effector != null)
		{
			base.transform.position = effector.position;
			base.transform.rotation = effector.rotation;
		}
		else if (bend != null)
		{
			base.transform.position = bend.bendGoal.position;
			base.transform.rotation = bend.bendGoal.rotation;
		}
		else if (lookAt != null)
		{
			base.transform.position = lookAt.IKPosition;
		}
	}
}
