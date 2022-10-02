using UnityEngine;

public class Destructor : MonoBehaviour
{
	[SerializeField]
	private EntityBase entityBase;

	private void Awake()
	{
		if (!entityBase)
		{
			entityBase = base.gameObject.GetComponentInParent<EntityBase>();
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.layer == Layers.buildingLayer && (!entityBase.isMicro || !entityBase.isPlayer || GlobalPreferences.MicroPlayerBuildingDestruction.value) && (!entityBase.isGiantess || ((!entityBase.isPlayer || GlobalPreferences.GtsPlayerBuildingDestruction.value) && (entityBase.isPlayer || GlobalPreferences.GtsBuildingDestruction.value))))
		{
			IDamagable componentInParent = collision.gameObject.GetComponentInParent<IDamagable>();
			ICrushable crushable;
			IDestructible destructible;
			if (((crushable = componentInParent as ICrushable) == null || !crushable.TryToCrush(entityBase.AccurateScale, collision.relativeVelocity, collision, entityBase)) && (destructible = componentInParent as IDestructible) != null)
			{
				Vector3 point = collision.GetContact(0).point;
				destructible.TryToDestroy(entityBase.Height, point, entityBase);
			}
		}
	}
}
