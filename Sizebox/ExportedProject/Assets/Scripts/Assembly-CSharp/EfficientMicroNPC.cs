using UnityEngine;

public class EfficientMicroNPC : MicroNpc
{
	[SerializeField]
	protected float deathAnimationDuration = 1f;

	protected override void Awake()
	{
		base.Awake();
		ConvertComponents();
	}

	public void ConvertComponents()
	{
		base.Animator.enabled = false;
		base.Animator.speed = 0f;
		SkinnedMeshRenderer[] componentsInChildren = GetComponentsInChildren<SkinnedMeshRenderer>();
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
		{
			GameObject obj = skinnedMeshRenderer.gameObject;
			MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
			MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
			meshFilter.sharedMesh = skinnedMeshRenderer.sharedMesh;
			meshRenderer.sharedMaterials = skinnedMeshRenderer.sharedMaterials;
			Object.Destroy(skinnedMeshRenderer);
		}
		Object.Destroy(base.transform.GetChild(0).gameObject);
	}
}
