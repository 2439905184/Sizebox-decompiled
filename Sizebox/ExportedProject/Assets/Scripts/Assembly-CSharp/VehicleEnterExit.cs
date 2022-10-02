using UnityEngine;
using UnityStandardAssets.Vehicles.Aeroplane;

public class VehicleEnterExit : MonoBehaviour
{
	private VehicleInputController vehicleInput;

	private AeroplaneUserControl2Axis aeroplaneControl;

	private Camera mainCamera;

	private bool isOn;

	private bool justEntered;

	private Player player;

	private void Awake()
	{
		player = GameController.LocalClient.Player;
		vehicleInput = GetComponent<VehicleInputController>();
		aeroplaneControl = GetComponent<AeroplaneUserControl2Axis>();
		EnableControl(false);
		isOn = false;
		mainCamera = Camera.main;
	}

	private void LateUpdate()
	{
		if ((bool)base.transform.parent)
		{
			base.transform.SetParent(null);
		}
		if (base.transform.localScale.y != 1f)
		{
			base.transform.localScale = Vector3.one;
		}
		if (!justEntered && isOn && InputManager.inputs.Micro.Interact.triggered)
		{
			isOn = false;
			GetPlayerOutsideOfVehicle();
			EnableControl(false);
		}
		if (isOn && (bool)player.Entity)
		{
			player.Entity.transform.position = base.transform.position;
		}
		if (justEntered)
		{
			justEntered = false;
		}
	}

	public void EnterVehicle()
	{
		justEntered = true;
		isOn = true;
		player.Entity.SetActive(false);
		player.Entity.ChangeScale(1f);
		EnableControl(true);
	}

	private void EnableControl(bool value)
	{
		if ((bool)vehicleInput)
		{
			vehicleInput.enabled = value;
		}
		if ((bool)aeroplaneControl)
		{
			aeroplaneControl.enabled = value;
		}
	}

	private void GetPlayerOutsideOfVehicle()
	{
		Transform transform = base.transform;
		player.Entity.transform.position = transform.position - transform.right * 2f;
		player.Entity.SetActive(true);
		player.Control.MicroController.ResetState();
	}

	private void OnDestroy()
	{
		if (isOn)
		{
			GetPlayerOutsideOfVehicle();
		}
	}
}
