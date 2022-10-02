using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

[RequireComponent(typeof(CarController))]
public class StandardCarControl : VehicleInputController
{
	private CarController m_Car;

	private void Awake()
	{
		m_Car = GetComponent<CarController>();
	}

	private void FixedUpdate()
	{
		Vector2 vector = InputManager.inputs.Player.Move.ReadValue<Vector2>();
		float handbrake = InputManager.inputs.Micro.Jump.ReadValue<float>();
		m_Car.Move(vector.x, vector.y, vector.y, handbrake);
	}
}
