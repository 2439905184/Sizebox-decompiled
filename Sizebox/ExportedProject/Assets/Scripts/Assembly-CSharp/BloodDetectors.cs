using UnityEngine;

public class BloodDetectors : MonoBehaviour
{
	public Transform targetTran;

	public BloodBounds parentScript;

	public int tIdNumber;

	private bool isDestroyed;

	private void FixedUpdate()
	{
		base.transform.position = targetTran.position;
		base.transform.eulerAngles = targetTran.eulerAngles;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == Layers.gtsBodyLayer && !isDestroyed)
		{
			if (tIdNumber == 1)
			{
				parentScript.t1 = null;
			}
			if (tIdNumber == 2)
			{
				parentScript.t2 = null;
			}
			if (tIdNumber == 3)
			{
				parentScript.t3 = null;
			}
			if (tIdNumber == 4)
			{
				parentScript.t4 = null;
			}
			if (tIdNumber == 0)
			{
				Debug.Log("NO ID FOR TRIGGER");
			}
			isDestroyed = true;
			Object.Destroy(base.gameObject);
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.layer == Layers.gtsBodyLayer && !isDestroyed)
		{
			if (tIdNumber == 1)
			{
				parentScript.t1 = null;
			}
			if (tIdNumber == 2)
			{
				parentScript.t2 = null;
			}
			if (tIdNumber == 3)
			{
				parentScript.t3 = null;
			}
			if (tIdNumber == 4)
			{
				parentScript.t4 = null;
			}
			if (tIdNumber == 0)
			{
				Debug.Log("NO ID FOR TRIGGER");
			}
			isDestroyed = true;
			Object.Destroy(base.gameObject);
		}
	}
}
