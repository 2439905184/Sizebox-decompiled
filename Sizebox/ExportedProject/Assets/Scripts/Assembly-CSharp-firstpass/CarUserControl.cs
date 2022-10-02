using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

[RequireComponent(typeof(CarController))]
public class CarUserControl : MonoBehaviour
{
	private CarController m_Car;

	private void Awake()
	{
		m_Car = GetComponent<CarController>();
	}

	private void FixedUpdate()
	{
		float axis = Input.GetAxis("Horizontal");
		float axis2 = Input.GetAxis("Vertical");
		float axis3 = Input.GetAxis("Jump");
		m_Car.Move(axis, axis2, axis2, axis3);
	}
}
