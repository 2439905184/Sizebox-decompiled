using UnityEngine;

public class PlayerGun : Equipment
{
	[SerializeField]
	protected Transform firingPoint;

	[SerializeField]
	protected GameObject projectilePrefab;

	public Transform GetFiringPoint()
	{
		return firingPoint;
	}
}
