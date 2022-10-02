using UnityEngine;

public class Vehicle : EntityBase
{
	private VehicleEnterExit enterExit;

	public override EntityType EntityType
	{
		get
		{
			return EntityType.VEHICLE;
		}
	}

	protected override void FinishInitialization()
	{
		enterExit = GetComponentInChildren<VehicleEnterExit>();
		if (!enterExit)
		{
			Debug.LogError("The vehicle [" + base.name + "] does not have the required scripts. Deleting.");
			Object.Destroy(base.gameObject);
		}
		else
		{
			base.FinishInitialization();
		}
	}

	public void EnterVehicle()
	{
		enterExit.EnterVehicle();
	}
}
