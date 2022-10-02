using System;
using UnityEngine;

public class MicroLod : EntityComponent
{
	private const float lodPercent = 0.016f;

	private const float cullPercent = 0.005f;

	[SerializeField]
	private Renderer lodRenderer;

	private LODGroup lodGroup;

	public override void Initialize(EntityBase entity)
	{
		bool flag = true;
		lodGroup = entity.model.GetComponent<LODGroup>();
		if (!lodGroup)
		{
			flag = false;
			lodGroup = entity.gameObject.AddComponent<LODGroup>();
		}
		Transform transform = entity.transform;
		Transform transform2 = entity.model.transform;
		Vector3 localScale = transform.localScale;
		transform.localScale = entity.model.transform.localScale.Inverse();
		if (flag)
		{
			LOD[] array = lodGroup.GetLODs();
			if (array.Length > 1)
			{
				Array.Resize(ref array, array.Length + 1);
				array[array.Length - 2].screenRelativeTransitionHeight = 0.016f;
				array[array.Length - 1] = new LOD(0.005f, new Renderer[1] { lodRenderer });
				lodGroup.SetLODs(array);
			}
			else
			{
				flag = false;
			}
		}
		if (!flag)
		{
			LOD[] array2 = new LOD[2];
			array2[0].renderers = entity.model.GetComponentsInChildren<Renderer>();
			array2[0].screenRelativeTransitionHeight = 0.016f;
			array2[1].renderers = new Renderer[1] { lodRenderer };
			array2[1].screenRelativeTransitionHeight = 0.005f;
			lodGroup.SetLODs(array2);
		}
		lodRenderer.transform.SetParent(entity.model.transform, true);
		lodRenderer.enabled = true;
		lodGroup.RecalculateBounds();
		transform.localScale = localScale;
		(entity as Micro).OnDeath += OnDeath;
		(entity as Micro).OnRevive += OnRevive;
	}

	private void OnDeath(Humanoid micro)
	{
		lodRenderer.transform.localPosition = Vector3.forward * (1f / micro.ModelScale);
		lodRenderer.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
	}

	private void OnRevive(Humanoid micro)
	{
		lodRenderer.transform.localPosition = Vector3.zero;
		lodRenderer.transform.localRotation = Quaternion.identity;
	}
}
