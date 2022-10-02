using UnityEngine;

public abstract class AIWeapon : MonoBehaviour
{
	protected GameObject owner;

	public void SetOwner(GameObject owner)
	{
		this.owner = owner;
	}
}
