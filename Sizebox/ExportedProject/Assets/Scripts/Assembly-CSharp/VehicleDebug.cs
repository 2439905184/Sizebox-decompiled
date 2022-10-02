using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Vehicle Controllers/Vehicle Debug", 3)]
public class VehicleDebug : MonoBehaviour
{
	public Vector3 spawnPos;

	public Vector3 spawnRot;

	[Tooltip("Y position below which the vehicle will be reset")]
	private float fallLimit = -1000f;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftControl))
		{
			StartCoroutine(ResetRotation());
		}
		if (CenterOrigin.WorldToVirtual(base.transform.position).y < fallLimit)
		{
			StartCoroutine(ResetPosition());
		}
	}

	private IEnumerator ResetRotation()
	{
		if ((bool)GetComponent<VehicleDamage>())
		{
			GetComponent<VehicleDamage>().Repair();
		}
		yield return new WaitForFixedUpdate();
		base.transform.eulerAngles = new Vector3(0f, base.transform.eulerAngles.y, 0f);
		base.transform.Translate(Vector3.up, Space.World);
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
	}

	private IEnumerator ResetPosition()
	{
		if ((bool)GetComponent<VehicleDamage>())
		{
			GetComponent<VehicleDamage>().Repair();
		}
		base.transform.position = spawnPos;
		yield return new WaitForFixedUpdate();
		base.transform.rotation = Quaternion.LookRotation(spawnRot, GlobalControl.worldUpDir);
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
	}
}
