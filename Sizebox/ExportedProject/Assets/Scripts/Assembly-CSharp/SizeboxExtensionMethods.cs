using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Sizebox.CharacterEditor;
using UnityEngine;

public static class SizeboxExtensionMethods
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass5_0
	{
		public string name;

		internal bool _003CFindAllRecursive_003Eb__0(Transform t)
		{
			return t.name.ToLower().Contains(name.ToLower());
		}

		internal bool _003CFindAllRecursive_003Eb__1(Transform t)
		{
			return t.name == name;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass6_0
	{
		public string name;

		internal bool _003CFindRecursive_003Eb__0(Transform t)
		{
			return t.name.ToLower().Contains(name.ToLower());
		}

		internal bool _003CFindRecursive_003Eb__1(Transform t)
		{
			return t.name == name;
		}
	}

	public static void SetRenderingLayer(this GameObject go, LayerMask layer)
	{
		Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.layer = layer;
		}
	}

	public static void SetLayerRecursively(this GameObject go, LayerMask layer)
	{
		go.layer = layer;
		foreach (Transform item in go.transform)
		{
			item.gameObject.SetLayerRecursively(layer);
		}
	}

	public static void SetRuntimeController(this Animator animator, RuntimeAnimatorController newController)
	{
		SkeletonEdit componentInParent = animator.GetComponentInParent<SkeletonEdit>();
		if ((bool)componentInParent)
		{
			componentInParent.Disable();
		}
		animator.runtimeAnimatorController = newController;
		if ((bool)componentInParent)
		{
			componentInParent.Enable();
		}
	}

	public static Vector3 Flatten(this Vector3 vector)
	{
		vector.y = 0f;
		return vector;
	}

	public static Vector3 Inverse(this Vector3 vector)
	{
		vector.x = 1f / vector.x;
		vector.y = 1f / vector.y;
		vector.z = 1f / vector.z;
		return vector;
	}

	public static List<Transform> FindAllRecursive(this Transform transform, string name, bool partial = false)
	{
		_003C_003Ec__DisplayClass5_0 _003C_003Ec__DisplayClass5_ = new _003C_003Ec__DisplayClass5_0();
		_003C_003Ec__DisplayClass5_.name = name;
		Transform[] componentsInChildren = transform.GetComponentsInChildren<Transform>();
		if (partial)
		{
			return componentsInChildren.Where(_003C_003Ec__DisplayClass5_._003CFindAllRecursive_003Eb__0).ToList();
		}
		return componentsInChildren.Where(_003C_003Ec__DisplayClass5_._003CFindAllRecursive_003Eb__1).ToList();
	}

	public static Transform FindRecursive(this Transform transform, string name, bool partial = false)
	{
		_003C_003Ec__DisplayClass6_0 _003C_003Ec__DisplayClass6_ = new _003C_003Ec__DisplayClass6_0();
		_003C_003Ec__DisplayClass6_.name = name;
		Transform[] componentsInChildren = transform.GetComponentsInChildren<Transform>();
		if (partial)
		{
			return componentsInChildren.Where(_003C_003Ec__DisplayClass6_._003CFindRecursive_003Eb__0).First();
		}
		return componentsInChildren.Where(_003C_003Ec__DisplayClass6_._003CFindRecursive_003Eb__1).First();
	}
}
