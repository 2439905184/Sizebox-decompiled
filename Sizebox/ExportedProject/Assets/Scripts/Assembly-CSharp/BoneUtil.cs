using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class BoneUtil
{
	private static readonly string[] JiggleNames = new string[1] { "jiggle" };

	private static readonly string[] HairNames = new string[6] { "hair", "ponite", "palpus", "osage", "shippo", "burns" };

	private static readonly string[] GeneralBreastNames = new string[6] { "boob", "breast", "hidarichichi", "migichichi", "pectoral", "chichi" };

	private static readonly string[] LeftBreastNames = new string[7] { "left breast", "leftbreast", "breast left", "breastleft", "hidarichichi", "lpectoral", "lbreast" };

	private static readonly string[] RightBreastNames = new string[7] { "right breast", "rightbreast", "breast right", "breastright", "migichichi", "rpectoral", "rbreast" };

	private static readonly string[] BreastDisqualified = new string[4] { "wing", "tail", "collider", "edit bone" };

	public static Transform FindHairRoot(Animator anim)
	{
		Transform boneTransform = anim.GetBoneTransform(HumanBodyBones.Head);
		if (!boneTransform)
		{
			return null;
		}
		int num = 0;
		Transform result = null;
		foreach (Transform item in boneTransform)
		{
			if (Regex.IsMatch(item.name, ".*joint.*hair.*", RegexOptions.IgnoreCase))
			{
				num++;
				result = item;
			}
		}
		if (num == 1)
		{
			return result;
		}
		return boneTransform;
	}

	public static bool IsHairBone(Transform bone)
	{
		string text = bone.name.ToLowerInvariant();
		string[] hairNames = HairNames;
		foreach (string value in hairNames)
		{
			if (text.Contains(value))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsJiggleBone(Transform bone)
	{
		string text = bone.name.ToLowerInvariant();
		string[] jiggleNames = JiggleNames;
		foreach (string text2 in jiggleNames)
		{
			if (text.Contains(text2.ToLower()))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsHumanBone(Animator animator, Transform bone)
	{
		foreach (Transform item in new List<Transform>
		{
			animator.GetBoneTransform(HumanBodyBones.Chest),
			animator.GetBoneTransform(HumanBodyBones.Neck),
			animator.GetBoneTransform(HumanBodyBones.Head),
			animator.GetBoneTransform(HumanBodyBones.Spine),
			animator.GetBoneTransform(HumanBodyBones.Hips),
			animator.GetBoneTransform(HumanBodyBones.UpperChest),
			animator.GetBoneTransform(HumanBodyBones.LeftFoot),
			animator.GetBoneTransform(HumanBodyBones.LeftHand),
			animator.GetBoneTransform(HumanBodyBones.RightFoot),
			animator.GetBoneTransform(HumanBodyBones.RightHand)
		})
		{
			if ((object)bone == item)
			{
				return true;
			}
		}
		return false;
	}

	public static Transform FindAllBreastBones(Animator anim, out List<Transform> breastBones, out HashSet<Transform> exclusions)
	{
		breastBones = new List<Transform>();
		exclusions = new HashSet<Transform>();
		Transform boneTransform = anim.GetBoneTransform(HumanBodyBones.Head);
		Transform boneTransform2 = anim.GetBoneTransform(HumanBodyBones.Hips);
		if (!boneTransform || !boneTransform2)
		{
			return null;
		}
		List<Transform> list = new List<Transform>();
		Transform parent = boneTransform.parent;
		while (parent != boneTransform2 && parent != null)
		{
			list.Add(parent);
			parent = parent.parent;
		}
		foreach (Transform item in list)
		{
			if ((bool)item)
			{
				FindAllBreastBones(item, breastBones, exclusions, item);
			}
			if (breastBones.Count > 0)
			{
				return item;
			}
			exclusions.Clear();
		}
		return null;
	}

	private static void FindAllBreastBones(Transform target, List<Transform> breastBones, HashSet<Transform> exclusions, Transform root)
	{
		if (NameContains(target, GeneralBreastNames))
		{
			breastBones.Add(target);
		}
		else if (target != root)
		{
			exclusions.Add(target);
			return;
		}
		foreach (Transform item in target)
		{
			FindAllBreastBones(item, breastBones, exclusions, root);
		}
	}

	public static void FindBreastBones(Animator animator, out Transform leftBreast, out Transform rightBreast)
	{
		leftBreast = null;
		rightBreast = null;
		FindBreastTransforms(animator.GetBoneTransform(HumanBodyBones.Hips), ref leftBreast, ref rightBreast);
	}

	private static void FindBreastTransforms(Transform transform, ref Transform leftBreast, ref Transform rightBreast)
	{
		if (!BreastNameIsDisqualified(transform))
		{
			if (!leftBreast && (NameContains(transform, LeftBreastNames) || (NameContains(transform, GeneralBreastNames) && NameContains(transform, new string[1] { "left" }))))
			{
				leftBreast = transform;
			}
			if (!rightBreast && (NameContains(transform, RightBreastNames) || (NameContains(transform, GeneralBreastNames) && NameContains(transform, new string[1] { "right" }))))
			{
				rightBreast = transform;
			}
		}
		if ((bool)leftBreast && (bool)rightBreast)
		{
			return;
		}
		foreach (Transform item in transform)
		{
			FindBreastTransforms(item, ref leftBreast, ref rightBreast);
		}
	}

	private static bool NameContains(Transform transform, string[] breastNames)
	{
		string text = transform.name.ToLowerInvariant();
		foreach (string value in breastNames)
		{
			if (text.Contains(value))
			{
				return true;
			}
		}
		return false;
	}

	private static bool BreastNameIsDisqualified(Transform transform)
	{
		string text = transform.name.ToLowerInvariant();
		string[] breastDisqualified = BreastDisqualified;
		foreach (string value in breastDisqualified)
		{
			if (text.Contains(value))
			{
				return true;
			}
		}
		return false;
	}
}
