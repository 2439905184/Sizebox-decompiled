using UnityEngine;

[RequireComponent(typeof(VehicleParent))]
[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Input/Basic Input", 0)]
public class BasicInput : VehicleInputController
{
	private VehicleParent vp;

	private void Start()
	{
		vp = GetComponent<VehicleParent>();
	}

	private void FixedUpdate()
	{
		Vector2 vector = InputManager.inputs.Player.Move.ReadValue<Vector2>();
		bool boost = InputManager.inputs.Player.Sprint.ReadValue<float>() > 0.5f;
		float ebrake = InputManager.inputs.Micro.Jump.ReadValue<float>();
		vp.SetAccel(vector.y);
		vp.SetBrake(vector.y);
		vp.SetSteer(vector.x);
		vp.SetEbrake(ebrake);
		vp.SetBoost(boost);
	}

	private void OnDisable()
	{
		if (!(vp == null))
		{
			vp.SetAccel(0f);
			vp.SetBrake(0f);
			vp.SetSteer(0f);
			vp.SetEbrake(0f);
			vp.SetBoost(false);
		}
	}
}
