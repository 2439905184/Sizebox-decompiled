using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;

public class PoseIK
{
	private readonly GiantessIK _gtsIk;

	private readonly EntityBase _entity;

	private readonly FullBodyBipedIK _ik;

	public readonly IKSolverLookAt head;

	public readonly IKEffector body;

	public readonly IKEffector leftHand;

	public readonly IKEffector rightHand;

	public readonly IKEffector leftFoot;

	public readonly IKEffector rightFoot;

	public readonly IKEffector leftShoulder;

	public readonly IKEffector rightShoulder;

	public readonly IKEffector leftThigh;

	public readonly IKEffector rightThigh;

	public readonly IKConstraintBend leftElbow;

	public readonly IKConstraintBend rightElbow;

	public readonly IKConstraintBend leftKnee;

	public readonly IKConstraintBend rightKnee;

	private readonly List<IKEffector> _effectors;

	private readonly List<IKConstraintBend> _bendGoals;

	public PoseIK(GiantessIK gtsIK, FullBodyBipedIK ik, LookAtIK lookAt, EntityBase entity)
	{
		_ik = ik;
		_gtsIk = gtsIK;
		_entity = entity;
		_effectors = new List<IKEffector>();
		_bendGoals = new List<IKConstraintBend>();
		body = ik.solver.bodyEffector;
		_effectors.Add(body);
		leftHand = ik.solver.leftHandEffector;
		_effectors.Add(leftHand);
		rightHand = ik.solver.rightHandEffector;
		_effectors.Add(rightHand);
		rightFoot = ik.solver.rightFootEffector;
		_effectors.Add(rightFoot);
		leftFoot = ik.solver.leftFootEffector;
		_effectors.Add(leftFoot);
		leftShoulder = ik.solver.leftShoulderEffector;
		_effectors.Add(leftShoulder);
		rightShoulder = ik.solver.rightShoulderEffector;
		_effectors.Add(rightShoulder);
		rightThigh = ik.solver.rightThighEffector;
		_effectors.Add(rightThigh);
		leftThigh = ik.solver.leftThighEffector;
		_effectors.Add(leftThigh);
		leftElbow = ik.solver.leftArmChain.bendConstraint;
		leftElbow.bendGoal = new GameObject("Bend Goal").transform;
		leftElbow.bendGoal.SetParent(entity.transform);
		_bendGoals.Add(leftElbow);
		rightElbow = ik.solver.rightArmChain.bendConstraint;
		rightElbow.bendGoal = Object.Instantiate(leftElbow.bendGoal, entity.transform);
		_bendGoals.Add(rightElbow);
		leftKnee = ik.solver.leftLegChain.bendConstraint;
		leftKnee.bendGoal = Object.Instantiate(leftElbow.bendGoal, entity.transform);
		_bendGoals.Add(leftKnee);
		rightKnee = ik.solver.rightLegChain.bendConstraint;
		rightKnee.bendGoal = Object.Instantiate(leftElbow.bendGoal, entity.transform);
		_bendGoals.Add(rightKnee);
		head = lookAt.solver;
	}

	public void EnablePoseIk()
	{
		foreach (IKEffector effector in _effectors)
		{
			effector.position = effector.bone.position;
			effector.rotation = effector.bone.rotation;
		}
		foreach (IKConstraintBend bendGoal in _bendGoals)
		{
			Vector3 vector = (bendGoal.bone1.position + bendGoal.bone3.position) / 2f;
			Vector3 position = bendGoal.bone2.position;
			Vector3 normalized = (position - vector).normalized;
			bendGoal.bendGoal.position = position + normalized * (200f * _entity.Scale);
		}
		head.IKPosition = head.head.transform.position + head.head.transform.forward * (500f * _entity.Scale);
		head.headWeight = 1f;
		head.eyesWeight = 1f;
		head.bodyWeight = 0f;
		head.clampWeight = 0f;
		head.clampWeightHead = 0f;
		head.clampWeightEyes = 0f;
		_gtsIk.EnableIK(true);
		SetWeight(1f);
	}

	public void DisablePoseIk()
	{
		SetWeight(0f);
	}

	private void SetWeight(float weight)
	{
		foreach (IKEffector effector in _effectors)
		{
			effector.positionWeight = weight;
			effector.rotationWeight = weight;
		}
		foreach (IKConstraintBend bendGoal in _bendGoals)
		{
			bendGoal.weight = weight;
		}
		head.IKPositionWeight = weight;
		_ik.solver.leftArmMapping.weight = weight;
		_ik.solver.rightArmMapping.weight = weight;
	}

	public void OffsetEffectors(Vector3 offset)
	{
		foreach (IKEffector effector in _effectors)
		{
			effector.position += offset;
		}
		head.IKPosition += offset;
	}

	public CustomPose SavePose(string poseName, string animation)
	{
		Giantess component = _entity.GetComponent<Giantess>();
		List<PoseBoneData> boneData = new List<PoseBoneData>
		{
			new PoseBoneData(component, body.position, body.rotation),
			new PoseBoneData(component, leftHand.position, leftHand.rotation),
			new PoseBoneData(component, rightHand.position, rightHand.rotation),
			new PoseBoneData(component, leftFoot.position, leftFoot.rotation),
			new PoseBoneData(component, rightFoot.position, rightFoot.rotation),
			new PoseBoneData(component, leftShoulder.position, leftShoulder.rotation),
			new PoseBoneData(component, rightShoulder.position, rightShoulder.rotation),
			new PoseBoneData(component, leftThigh.position, leftThigh.rotation),
			new PoseBoneData(component, rightThigh.position, rightThigh.rotation)
		};
		List<PoseTargetData> targetData = new List<PoseTargetData>
		{
			new PoseTargetData(component, leftElbow.bendGoal.position),
			new PoseTargetData(component, rightElbow.bendGoal.position),
			new PoseTargetData(component, leftKnee.bendGoal.position),
			new PoseTargetData(component, rightKnee.bendGoal.position),
			new PoseTargetData(component, head.IKPosition)
		};
		return new CustomPose(poseName, animation, boneData, targetData);
	}

	public void LoadPose(CustomPose pose)
	{
		Transform transform = _entity.model.transform;
		List<PoseBoneData> boneData = pose.boneData;
		Quaternion rotation = transform.rotation;
		PoseBoneData poseBoneData = boneData[0];
		body.position = transform.TransformPoint(poseBoneData.localPosition);
		body.rotation = rotation * poseBoneData.localRotation;
		poseBoneData = boneData[1];
		leftHand.position = transform.TransformPoint(poseBoneData.localPosition);
		leftHand.rotation = rotation * poseBoneData.localRotation;
		poseBoneData = boneData[2];
		rightHand.position = transform.TransformPoint(poseBoneData.localPosition);
		rightHand.rotation = rotation * poseBoneData.localRotation;
		poseBoneData = boneData[3];
		leftFoot.position = transform.TransformPoint(poseBoneData.localPosition);
		leftFoot.rotation = rotation * poseBoneData.localRotation;
		poseBoneData = boneData[4];
		rightFoot.position = transform.TransformPoint(poseBoneData.localPosition);
		rightFoot.rotation = rotation * poseBoneData.localRotation;
		poseBoneData = boneData[5];
		leftShoulder.position = transform.TransformPoint(poseBoneData.localPosition);
		leftShoulder.rotation = rotation * poseBoneData.localRotation;
		poseBoneData = boneData[6];
		rightShoulder.position = transform.TransformPoint(poseBoneData.localPosition);
		rightShoulder.rotation = rotation * poseBoneData.localRotation;
		poseBoneData = boneData[7];
		leftThigh.position = transform.TransformPoint(poseBoneData.localPosition);
		leftThigh.rotation = rotation * poseBoneData.localRotation;
		poseBoneData = boneData[8];
		rightThigh.position = transform.TransformPoint(poseBoneData.localPosition);
		rightThigh.rotation = rotation * poseBoneData.localRotation;
		List<PoseTargetData> targetData = pose.targetData;
		leftElbow.bendGoal.position = transform.TransformPoint(targetData[0].localPosition);
		rightElbow.bendGoal.position = transform.TransformPoint(targetData[1].localPosition);
		leftKnee.bendGoal.position = transform.TransformPoint(targetData[2].localPosition);
		rightKnee.bendGoal.position = transform.TransformPoint(targetData[3].localPosition);
		head.IKPosition = transform.TransformPoint(targetData[4].localPosition);
	}
}
