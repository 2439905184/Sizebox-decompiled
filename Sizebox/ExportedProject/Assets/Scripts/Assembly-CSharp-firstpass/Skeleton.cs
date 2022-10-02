using System;
using RootMotion.Dynamics;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
	public Animator animator;

	public PuppetMaster puppetMaster;

	public ConfigurableJoint[] leftLeg;

	public ConfigurableJoint[] rightLeg;

	private bool leftLegRemoved;

	private bool rightLegRemoved;

	private void Start()
	{
		PuppetMaster obj = puppetMaster;
		obj.OnMuscleRemoved = (PuppetMaster.MuscleDelegate)Delegate.Combine(obj.OnMuscleRemoved, new PuppetMaster.MuscleDelegate(OnMuscleRemoved));
	}

	private void OnMuscleRemoved(Muscle m)
	{
		bool isLeft = false;
		if (IsLegMuscle(m, out isLeft))
		{
			if (isLeft)
			{
				leftLegRemoved = true;
			}
			else
			{
				rightLegRemoved = true;
			}
			if (leftLegRemoved && rightLegRemoved)
			{
				puppetMaster.state = PuppetMaster.State.Dead;
			}
			else
			{
				animator.SetFloat("Legs", 1f);
			}
		}
	}

	private bool IsLegMuscle(Muscle m, out bool isLeft)
	{
		isLeft = false;
		ConfigurableJoint[] array = leftLeg;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == m.joint)
			{
				isLeft = true;
				return true;
			}
		}
		array = rightLeg;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == m.joint)
			{
				isLeft = false;
				return true;
			}
		}
		return false;
	}
}
