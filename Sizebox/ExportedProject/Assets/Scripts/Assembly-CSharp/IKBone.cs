using RootMotion.FinalIK;
using UnityEngine;

public class IKBone
{
	private IKEffector effector;

	public Vector3 virtualPosition;

	public Quaternion rotation;

	public float positionWeight;

	public float rotationWeight;

	public Vector3 position
	{
		get
		{
			return virtualPosition.ToWorld();
		}
		set
		{
			virtualPosition = value.ToVirtual();
		}
	}

	public IKBone(IKEffector effector)
	{
		this.effector = effector;
	}

	public void Update()
	{
		effector.position = position;
		effector.rotation = rotation;
		effector.positionWeight = positionWeight;
		effector.rotationWeight = rotationWeight;
	}
}
