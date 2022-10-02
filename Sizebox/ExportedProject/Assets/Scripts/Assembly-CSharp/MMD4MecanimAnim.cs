using System;
using UnityEngine;

public static class MMD4MecanimAnim
{
	[Serializable]
	public struct MorphMotion
	{
		public IMorph morph;

		public int lastKeyFrameIndex;
	}

	public interface IAnim
	{
		string animatorStateName { get; set; }

		int animatorStateNameHash { get; set; }

		TextAsset animFile { get; set; }

		AudioClip audioClip { get; set; }

		MMD4MecanimData.AnimData animData { get; set; }

		MorphMotion[] morphMotionList { get; set; }
	}

	[Serializable]
	public class Anim : IAnim
	{
		public string animatorStateName;

		[NonSerialized]
		public int animatorStateNameHash;

		public TextAsset animFile;

		public AudioClip audioClip;

		[NonSerialized]
		public MMD4MecanimData.AnimData animData;

		[NonSerialized]
		public MorphMotion[] morphMotionList;

		string IAnim.animatorStateName
		{
			get
			{
				return animatorStateName;
			}
			set
			{
				animatorStateName = value;
			}
		}

		int IAnim.animatorStateNameHash
		{
			get
			{
				return animatorStateNameHash;
			}
			set
			{
				animatorStateNameHash = value;
			}
		}

		TextAsset IAnim.animFile
		{
			get
			{
				return animFile;
			}
			set
			{
				animFile = value;
			}
		}

		AudioClip IAnim.audioClip
		{
			get
			{
				return audioClip;
			}
			set
			{
				audioClip = value;
			}
		}

		MMD4MecanimData.AnimData IAnim.animData
		{
			get
			{
				return animData;
			}
			set
			{
				animData = value;
			}
		}

		MorphMotion[] IAnim.morphMotionList
		{
			get
			{
				return morphMotionList;
			}
			set
			{
				morphMotionList = value;
			}
		}
	}

	public interface IMorph
	{
		string name { get; }

		float weight { get; set; }
	}

	public interface IAnimModel
	{
		int morphCount { get; }

		int animCount { get; }

		Animator animator { get; }

		AudioSource audioSource { get; }

		bool animSyncToAudio { get; }

		float prevDeltaTime { get; set; }

		IAnim playingAnim { get; set; }

		IMorph GetMorph(string name);

		IMorph GetMorph(string name, bool startsWith);

		IMorph GetMorphAt(int index);

		IAnim GetAnimAt(int index);

		void _SetAnimMorphWeight(IMorph morph, float weight);
	}

	public static void InitializeAnimModel(IAnimModel animModel)
	{
		if (animModel == null)
		{
			return;
		}
		int animCount = animModel.animCount;
		for (int i = 0; i < animCount; i++)
		{
			IAnim animAt = animModel.GetAnimAt(i);
			if (animAt == null)
			{
				continue;
			}
			if (animAt.animatorStateName != null)
			{
				animAt.animatorStateNameHash = Animator.StringToHash(animAt.animatorStateName);
			}
			if (animAt.animData == null)
			{
				animAt.animData = MMD4MecanimData.BuildAnimData(animAt.animFile);
				if (animAt.animData == null)
				{
					continue;
				}
			}
			MMD4MecanimData.MorphMotionData[] morphMotionDataList = animAt.animData.morphMotionDataList;
			if (morphMotionDataList == null)
			{
				continue;
			}
			animAt.morphMotionList = new MorphMotion[morphMotionDataList.Length];
			if (animAt.animData.supportNameIsFull)
			{
				for (int j = 0; j != morphMotionDataList.Length; j++)
				{
					animAt.morphMotionList[j].morph = animModel.GetMorph(morphMotionDataList[j].name, morphMotionDataList[j].nameIsFull);
				}
				continue;
			}
			for (int k = 0; k != morphMotionDataList.Length; k++)
			{
				animAt.morphMotionList[k].morph = animModel.GetMorph(morphMotionDataList[k].name, false);
			}
			for (int l = 0; l != morphMotionDataList.Length; l++)
			{
				if (animAt.morphMotionList[l].morph != null)
				{
					continue;
				}
				IMorph morph = animModel.GetMorph(morphMotionDataList[l].name, true);
				if (morph == null)
				{
					continue;
				}
				bool flag = false;
				for (int m = 0; m != morphMotionDataList.Length; m++)
				{
					if (flag)
					{
						break;
					}
					flag = animAt.morphMotionList[m].morph == morph;
				}
				if (!flag)
				{
					animAt.morphMotionList[l].morph = morph;
				}
			}
		}
	}

	public static void PreUpdateAnimModel(IAnimModel animModel)
	{
		if (animModel != null && animModel.prevDeltaTime == 0f)
		{
			animModel.prevDeltaTime = Time.deltaTime;
		}
	}

	public static void PostUpdateAnimModel(IAnimModel animModel)
	{
		if (animModel != null)
		{
			animModel.prevDeltaTime = Time.deltaTime;
		}
	}

	public static void UpdateAnimModel(IAnimModel animModel)
	{
		if (animModel == null)
		{
			return;
		}
		Animator animator = animModel.animator;
		if (animator != null && animator.enabled)
		{
			AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
			int fullPathHash = currentAnimatorStateInfo.fullPathHash;
			float animationTime = currentAnimatorStateInfo.normalizedTime * currentAnimatorStateInfo.length;
			int animCount = animModel.animCount;
			for (int i = 0; i < animCount; i++)
			{
				IAnim animAt = animModel.GetAnimAt(i);
				if (animAt != null && animAt.animatorStateNameHash == fullPathHash)
				{
					UpdateAnimModel(animModel, animAt, animationTime);
					return;
				}
			}
			StopAnimModel(animModel);
		}
		else
		{
			StopAnimModel(animModel);
		}
	}

	public static void UpdateAnimModel(IAnimModel animModel, IAnim anim, float animationTime)
	{
		if (animModel == null)
		{
			return;
		}
		if (anim == null)
		{
			StopAnimModel(animModel);
			return;
		}
		float num = animationTime * 30f;
		int num2 = (int)num;
		bool flag = animModel.playingAnim != anim;
		if (animModel.playingAnim != anim)
		{
			StopAnimModel(animModel);
		}
		bool isPlayAudioSourceAtFirst = false;
		if (animModel.playingAnim == null && anim.audioClip != null)
		{
			AudioSource audioSource = animModel.audioSource;
			if (audioSource != null)
			{
				if (audioSource.clip != anim.audioClip)
				{
					audioSource.clip = anim.audioClip;
					audioSource.Play();
					isPlayAudioSourceAtFirst = true;
				}
				else if (!audioSource.isPlaying)
				{
					audioSource.Play();
					isPlayAudioSourceAtFirst = true;
				}
			}
		}
		animModel.playingAnim = anim;
		_SyncToAudio(animModel, animationTime, isPlayAudioSourceAtFirst);
		MorphMotion[] morphMotionList = anim.morphMotionList;
		MMD4MecanimData.AnimData animData = anim.animData;
		if (morphMotionList == null || animData == null || animData.morphMotionDataList == null)
		{
			return;
		}
		for (int i = 0; i != morphMotionList.Length; i++)
		{
			MMD4MecanimData.MorphMotionData morphMotionData = animData.morphMotionDataList[i];
			if (morphMotionList[i].morph == null || morphMotionData.frameNos == null || morphMotionData.f_frameNos == null || morphMotionData.weights == null)
			{
				continue;
			}
			if (flag || morphMotionList[i].lastKeyFrameIndex >= morphMotionData.frameNos.Length || morphMotionData.frameNos[morphMotionList[i].lastKeyFrameIndex] > num2)
			{
				morphMotionList[i].lastKeyFrameIndex = 0;
			}
			bool flag2 = false;
			for (int j = morphMotionList[i].lastKeyFrameIndex; j != morphMotionData.frameNos.Length; j++)
			{
				int num3 = morphMotionData.frameNos[j];
				if (num2 >= num3)
				{
					morphMotionList[i].lastKeyFrameIndex = j;
					continue;
				}
				if (morphMotionList[i].lastKeyFrameIndex + 1 < morphMotionData.frameNos.Length)
				{
					_ProcessKeyFrame2(animModel, morphMotionList[i].morph, morphMotionData, morphMotionList[i].lastKeyFrameIndex, morphMotionList[i].lastKeyFrameIndex + 1, num2, num);
					flag2 = true;
				}
				break;
			}
			if (!flag2 && morphMotionList[i].lastKeyFrameIndex < morphMotionData.frameNos.Length)
			{
				_ProcessKeyFrame(animModel, morphMotionList[i].morph, morphMotionData, morphMotionList[i].lastKeyFrameIndex);
			}
		}
	}

	private static void _ProcessKeyFrame2(IAnimModel animModel, IMorph morph, MMD4MecanimData.MorphMotionData motionMorphData, int keyFrameIndex0, int keyFrameIndex1, int frameNo, float f_frameNo)
	{
		int num = motionMorphData.frameNos[keyFrameIndex0];
		int num2 = motionMorphData.frameNos[keyFrameIndex1];
		float num3 = motionMorphData.f_frameNos[keyFrameIndex0];
		float num4 = motionMorphData.f_frameNos[keyFrameIndex1];
		if (frameNo <= num || num2 - num == 1)
		{
			animModel._SetAnimMorphWeight(morph, motionMorphData.weights[keyFrameIndex0]);
			return;
		}
		if (frameNo >= num2)
		{
			animModel._SetAnimMorphWeight(morph, motionMorphData.weights[keyFrameIndex1]);
			return;
		}
		float num5 = (f_frameNo - num3) / (num4 - num3);
		float num6 = motionMorphData.weights[keyFrameIndex0];
		float num7 = motionMorphData.weights[keyFrameIndex1];
		animModel._SetAnimMorphWeight(morph, num6 + (num7 - num6) * num5);
	}

	private static void _ProcessKeyFrame(IAnimModel animModel, IMorph morph, MMD4MecanimData.MorphMotionData motionMorphData, int keyFrameIndex)
	{
		animModel._SetAnimMorphWeight(morph, motionMorphData.weights[keyFrameIndex]);
	}

	public static void StopAnimModel(IAnimModel animModel)
	{
		if (animModel == null)
		{
			return;
		}
		IAnim playingAnim = animModel.playingAnim;
		if (playingAnim == null)
		{
			return;
		}
		if (playingAnim.audioClip != null)
		{
			AudioSource audioSource = animModel.audioSource;
			if (audioSource != null && audioSource.clip == playingAnim.audioClip)
			{
				audioSource.Stop();
				audioSource.clip = null;
				if (animModel.animSyncToAudio)
				{
					Animator animator = animModel.animator;
					if (animator != null)
					{
						animator.speed = 1f;
					}
				}
			}
		}
		animModel.playingAnim = null;
	}

	private static void _SyncToAudio(IAnimModel animModel, float animationTime, bool isPlayAudioSourceAtFirst)
	{
		if (!animModel.animSyncToAudio)
		{
			return;
		}
		IAnim playingAnim = animModel.playingAnim;
		if (playingAnim == null || playingAnim.audioClip == null)
		{
			return;
		}
		AudioSource audioSource = animModel.audioSource;
		if (audioSource == null)
		{
			return;
		}
		Animator animator = animModel.animator;
		if (!(animator != null) || !animator.enabled)
		{
			return;
		}
		float prevDeltaTime = animModel.prevDeltaTime;
		if (audioSource.isPlaying)
		{
			float time = audioSource.time;
			if (time == 0f)
			{
				animator.speed = 0f;
				return;
			}
			float num = (prevDeltaTime + Time.deltaTime) * 0.5f;
			float num2 = time - animationTime;
			if (Mathf.Abs(num2) <= num)
			{
				animator.speed = 1f;
				return;
			}
			if (!isPlayAudioSourceAtFirst && num > Mathf.Epsilon && num < 0.1f)
			{
				float value = 1f + num2 / num;
				value = Mathf.Clamp(value, 0.5f, 2f);
				if (animator.speed == 0f)
				{
					animator.speed = value;
				}
				else
				{
					animator.speed = animator.speed * 0.95f + value * 0.05f;
				}
				return;
			}
			AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
			if (currentAnimatorStateInfo.length > Mathf.Epsilon)
			{
				int fullPathHash = currentAnimatorStateInfo.fullPathHash;
				animator.Play(fullPathHash, 0, time / currentAnimatorStateInfo.length);
				animator.speed = 1f;
			}
		}
		else
		{
			animator.speed = 1f;
		}
	}
}
