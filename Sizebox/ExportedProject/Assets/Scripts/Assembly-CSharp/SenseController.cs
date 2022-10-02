using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SenseController : EntityComponent
{
	private EntityBase entity;

	private Transform head;

	public float maxDistace = 200f;

	public float fieldOfView = 100f;

	public override void Initialize(EntityBase entity)
	{
		this.entity = entity;
		head = entity.transform;
		Humanoid humanoid = entity as Humanoid;
		if ((bool)humanoid && humanoid.Animator != null)
		{
			head = humanoid.Animator.GetBoneTransform(HumanBodyBones.Head);
		}
	}

	public bool CheckVisibility(EntityBase target)
	{
		return VisibilityTest(target);
	}

	private bool VisibilityTest(EntityBase target)
	{
		if (target == null)
		{
			return false;
		}
		Vector3 position = head.position;
		Vector3 vector = target.transform.position + Vector3.up * target.Height * 0.5f;
		float num = target.Height / entity.Height;
		float num2 = maxDistace * entity.Height * num;
		float num3 = fieldOfView * 0.5f;
		Debug.DrawRay(position, Quaternion.AngleAxis(0f - num3, head.up) * head.forward * num2, Color.yellow);
		Debug.DrawRay(position, Quaternion.AngleAxis(num3, head.up) * head.forward * num2, Color.yellow);
		Vector3 vector2 = (vector - position).normalized * num2;
		float num4 = Vector3.Distance(position, vector);
		if (num4 < num2 && Vector3.Angle(head.forward, vector2) < num3)
		{
			RaycastHit hitInfo;
			if (!Physics.Raycast(position, vector2, out hitInfo, num4, Layers.visibilityMask) || hitInfo.transform == target.transform)
			{
				Debug.DrawLine(position, vector, Color.green);
				return true;
			}
			Debug.DrawLine(position, hitInfo.point, Color.red);
		}
		return false;
	}

	public List<EntityBase> GetEntitiesInRadius(float maxDistance)
	{
		return GetEntities(maxDistance, Layers.visibilityMask, false);
	}

	public List<EntityBase> GetVisibleEntities(float maxDistance)
	{
		return GetEntities(maxDistance, Layers.visibilityMask, true);
	}

	public List<EntityBase> GetVisibleMicros(float maxDistance)
	{
		return GetEntities(maxDistance, Layers.crushableMask, true);
	}

	private List<EntityBase> GetEntities(float maxDistance, LayerMask mask, bool visible)
	{
		Vector3 position = entity.transform.position;
		float radius = entity.Height * maxDistace;
		List<EntityBase> list = new List<EntityBase>();
		Collider[] array = Physics.OverlapSphere(position, radius, mask);
		for (int i = 0; i < array.Length; i++)
		{
			EntityBase component = array[i].gameObject.GetComponent<EntityBase>();
			Humanoid humanoid = entity as Humanoid;
			if ((bool)component && (!humanoid || !humanoid.IsDead) && !component.locked)
			{
				list.Add(component);
			}
		}
		if (visible)
		{
			list = list.FindAll(_003CGetEntities_003Eb__10_0);
		}
		return list;
	}

	public EntityBase GetRandomMicro(float distance)
	{
		List<EntityBase> visibleMicros = GetVisibleMicros(distance);
		if (visibleMicros.Count > 0)
		{
			return visibleMicros[Random.Range(0, visibleMicros.Count)];
		}
		return null;
	}

	[CompilerGenerated]
	private bool _003CGetEntities_003Eb__10_0(EntityBase target)
	{
		return VisibilityTest(target);
	}
}
