using System;
using UnityEngine;

namespace RootMotion.Dynamics
{
	[CreateAssetMenu(fileName = "PuppetMaster Humanoid Config", menuName = "PuppetMaster/Humanoid Config", order = 1)]
	public class PuppetMasterHumanoidConfig : ScriptableObject
	{
		[Serializable]
		public class HumanoidMuscle
		{
			[SerializeField]
			[HideInInspector]
			public string name;

			public HumanBodyBones bone;

			public Muscle.Props props;
		}

		[LargeHeader("Simulation")]
		public PuppetMaster.State state;

		public PuppetMaster.StateSettings stateSettings = PuppetMaster.StateSettings.Default;

		public PuppetMaster.Mode mode;

		public float blendTime = 0.1f;

		public bool fixTargetTransforms = true;

		public int solverIterationCount = 6;

		public bool visualizeTargetPose = true;

		[LargeHeader("Master Weights")]
		[Range(0f, 1f)]
		public float mappingWeight = 1f;

		[Range(0f, 1f)]
		public float pinWeight = 1f;

		[Range(0f, 1f)]
		public float muscleWeight = 1f;

		[LargeHeader("Joint and Muscle Settings")]
		public float muscleSpring = 100f;

		public float muscleDamper;

		[Range(1f, 8f)]
		public float pinPow = 4f;

		[Range(0f, 100f)]
		public float pinDistanceFalloff = 5f;

		public bool updateJointAnchors = true;

		public bool supportTranslationAnimation;

		public bool angularLimits;

		public bool internalCollisions;

		[LargeHeader("Individual Muscle Settings")]
		public HumanoidMuscle[] muscles = new HumanoidMuscle[0];

		public void ApplyTo(PuppetMaster p)
		{
			if (p.targetRoot == null)
			{
				Debug.LogWarning("Please assign 'Target Root' for PuppetMaster using a Humanoid Config.", p.transform);
				return;
			}
			if (p.targetAnimator == null)
			{
				Debug.LogError("PuppetMaster 'Target Root' does not have an Animator component. Can not use Humanoid Config.", p.transform);
				return;
			}
			if (!p.targetAnimator.isHuman)
			{
				Debug.LogError("PuppetMaster target is not a Humanoid. Can not use Humanoid Config.", p.transform);
				return;
			}
			p.state = state;
			p.stateSettings = stateSettings;
			p.mode = mode;
			p.blendTime = blendTime;
			p.fixTargetTransforms = fixTargetTransforms;
			p.solverIterationCount = solverIterationCount;
			p.visualizeTargetPose = visualizeTargetPose;
			p.mappingWeight = mappingWeight;
			p.pinWeight = pinWeight;
			p.muscleWeight = muscleWeight;
			p.muscleSpring = muscleSpring;
			p.muscleDamper = muscleDamper;
			p.pinPow = pinPow;
			p.pinDistanceFalloff = pinDistanceFalloff;
			p.updateJointAnchors = updateJointAnchors;
			p.supportTranslationAnimation = supportTranslationAnimation;
			p.angularLimits = angularLimits;
			p.internalCollisions = internalCollisions;
			HumanoidMuscle[] array = muscles;
			foreach (HumanoidMuscle humanoidMuscle in array)
			{
				Muscle muscle = GetMuscle(humanoidMuscle.bone, p.targetAnimator, p);
				if (muscle != null)
				{
					muscle.props.group = humanoidMuscle.props.group;
					muscle.props.mappingWeight = humanoidMuscle.props.mappingWeight;
					muscle.props.mapPosition = humanoidMuscle.props.mapPosition;
					muscle.props.muscleDamper = humanoidMuscle.props.muscleDamper;
					muscle.props.muscleWeight = humanoidMuscle.props.muscleWeight;
					muscle.props.pinWeight = humanoidMuscle.props.pinWeight;
				}
			}
		}

		private Muscle GetMuscle(HumanBodyBones boneId, Animator animator, PuppetMaster puppetMaster)
		{
			Transform boneTransform = animator.GetBoneTransform(boneId);
			if (boneTransform == null)
			{
				return null;
			}
			Muscle[] array = puppetMaster.muscles;
			foreach (Muscle muscle in array)
			{
				if (muscle.target == boneTransform)
				{
					return muscle;
				}
			}
			return null;
		}
	}
}
