using UnityEngine;

namespace UnityStandardAssets.Vehicles.Aeroplane
{
	[RequireComponent(typeof(AeroplaneController))]
	public class AeroplaneUserControl2Axis : MonoBehaviour
	{
		public float maxRollAngle = 80f;

		public float maxPitchAngle = 80f;

		private AeroplaneController m_Aeroplane;

		private void Awake()
		{
			m_Aeroplane = GetComponent<AeroplaneController>();
		}

		private void FixedUpdate()
		{
			Vector2 vector = InputManager.inputs.Player.Move.ReadValue<Vector2>();
			bool flag = InputManager.inputs.Micro.Jump.ReadValue<float>() > 0.5f;
			float throttleInput = ((!flag) ? 1 : (-1));
			m_Aeroplane.Move(vector.x, vector.y, 0f, throttleInput, flag);
		}
	}
}
