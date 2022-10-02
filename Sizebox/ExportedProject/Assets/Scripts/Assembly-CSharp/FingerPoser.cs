using System.Collections.Generic;
using UnityEngine;

public class FingerPoser : MonoBehaviour
{
	private const float HANDLE_SCALE = 0.02f;

	private List<Transform> fingerBones;

	private List<FingerPoserHandle> fingerPoserHandles;

	public void Init(IPosable poser)
	{
		fingerBones = new List<Transform>();
		fingerPoserHandles = new List<FingerPoserHandle>();
		Animator animator = poser.Animator;
		float accurateScale = poser.AccurateScale;
		AddBone(accurateScale, animator.GetBoneTransform(HumanBodyBones.RightIndexDistal));
		AddBone(accurateScale, animator.GetBoneTransform(HumanBodyBones.RightIndexIntermediate));
		AddBone(accurateScale, animator.GetBoneTransform(HumanBodyBones.RightIndexProximal));
		AddBone(accurateScale, animator.GetBoneTransform(HumanBodyBones.RightLittleDistal));
		AddBone(accurateScale, animator.GetBoneTransform(HumanBodyBones.RightLittleIntermediate));
		AddBone(accurateScale, animator.GetBoneTransform(HumanBodyBones.RightLittleProximal));
		AddBone(accurateScale, animator.GetBoneTransform(HumanBodyBones.RightMiddleDistal));
		AddBone(accurateScale, animator.GetBoneTransform(HumanBodyBones.RightMiddleIntermediate));
		AddBone(accurateScale, animator.GetBoneTransform(HumanBodyBones.RightMiddleProximal));
		AddBone(accurateScale, animator.GetBoneTransform(HumanBodyBones.RightRingDistal));
		AddBone(accurateScale, animator.GetBoneTransform(HumanBodyBones.RightRingIntermediate));
		AddBone(accurateScale, animator.GetBoneTransform(HumanBodyBones.RightRingProximal));
		AddBone(accurateScale, animator.GetBoneTransform(HumanBodyBones.RightThumbDistal));
		AddBone(accurateScale, animator.GetBoneTransform(HumanBodyBones.RightThumbIntermediate));
		AddBone(accurateScale, animator.GetBoneTransform(HumanBodyBones.RightThumbProximal));
		AddBone(accurateScale, animator.GetBoneTransform(HumanBodyBones.LeftIndexDistal));
		AddBone(accurateScale, animator.GetBoneTransform(HumanBodyBones.LeftIndexIntermediate));
		AddBone(accurateScale, animator.GetBoneTransform(HumanBodyBones.LeftIndexProximal));
		AddBone(accurateScale, animator.GetBoneTransform(HumanBodyBones.LeftLittleDistal));
		AddBone(accurateScale, animator.GetBoneTransform(HumanBodyBones.LeftLittleIntermediate));
		AddBone(accurateScale, animator.GetBoneTransform(HumanBodyBones.LeftLittleProximal));
		AddBone(accurateScale, animator.GetBoneTransform(HumanBodyBones.LeftMiddleDistal));
		AddBone(accurateScale, animator.GetBoneTransform(HumanBodyBones.LeftMiddleIntermediate));
		AddBone(accurateScale, animator.GetBoneTransform(HumanBodyBones.LeftMiddleProximal));
		AddBone(accurateScale, animator.GetBoneTransform(HumanBodyBones.LeftRingDistal));
		AddBone(accurateScale, animator.GetBoneTransform(HumanBodyBones.LeftRingIntermediate));
		AddBone(accurateScale, animator.GetBoneTransform(HumanBodyBones.LeftRingProximal));
		AddBone(accurateScale, animator.GetBoneTransform(HumanBodyBones.LeftThumbDistal));
		AddBone(accurateScale, animator.GetBoneTransform(HumanBodyBones.LeftThumbIntermediate));
		AddBone(accurateScale, animator.GetBoneTransform(HumanBodyBones.LeftThumbProximal));
	}

	private void AddBone(float scale, Transform bone)
	{
		if (!(bone == null))
		{
			fingerBones.Add(bone);
			fingerPoserHandles.Add(CreateHandle(bone, scale));
		}
	}

	private void LateUpdate()
	{
		foreach (Transform fingerBone in fingerBones)
		{
			fingerBone.localRotation = fingerPoserHandles[fingerBones.IndexOf(fingerBone)].transform.localRotation;
		}
	}

	private void OnDestroy()
	{
		foreach (FingerPoserHandle fingerPoserHandle in fingerPoserHandles)
		{
			Object.Destroy(fingerPoserHandle.gameObject);
		}
	}

	public void Show(bool isVisible)
	{
		foreach (FingerPoserHandle fingerPoserHandle in fingerPoserHandles)
		{
			fingerPoserHandle.gameObject.GetComponentInChildren<SphereCollider>().enabled = isVisible;
			fingerPoserHandle.gameObject.GetComponentInChildren<MeshRenderer>().enabled = isVisible;
		}
	}

	private FingerPoserHandle CreateHandle(Transform target, float scale)
	{
		GameObject obj = Object.Instantiate((GameObject)Resources.Load("UI/Pose/FingerPoserHandle"));
		obj.transform.localScale = Vector3.one * scale * 0.02f;
		obj.transform.SetParent(target, true);
		obj.transform.localPosition = Vector3.zero;
		obj.transform.localRotation = target.localRotation;
		return obj.GetComponent<FingerPoserHandle>();
	}
}
