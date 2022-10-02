using UnityEngine;

public class BloodBounds : MonoBehaviour
{
	private const float bloodDepthScale = 0.5f;

	public Transform bloodMesh;

	public float bloodDepthAdjustment;

	public float rayLength;

	private bool outsideBounds;

	private int childCheck;

	public int childCheckTreshold = 5;

	public GameObject t1;

	public GameObject t2;

	public GameObject t3;

	public GameObject t4;

	public static bool IsOddlyPlaced(Transform blood, Collider gtsCollider)
	{
		Vector3 extents = blood.GetComponent<MeshFilter>().mesh.bounds.extents;
		Vector3 vector = blood.TransformVector(extents.x * Vector3.right);
		Vector3 vector2 = blood.TransformVector(extents.z * Vector3.forward);
		Vector3 vector3 = blood.TransformVector(Mathf.Max(extents.x, extents.z) * 0.5f * Vector3.down);
		float maxDistance = 2f * vector3.magnitude;
		Vector3 vector4 = blood.position - vector3;
		Vector3 normalized = vector3.normalized;
		RaycastHit hitInfo;
		if (gtsCollider.Raycast(new Ray(vector4 + vector, normalized), out hitInfo, maxDistance) && gtsCollider.Raycast(new Ray(vector4 - vector, normalized), out hitInfo, maxDistance) && gtsCollider.Raycast(new Ray(vector4 + vector2, normalized), out hitInfo, maxDistance))
		{
			return !gtsCollider.Raycast(new Ray(vector4 - vector2, normalized), out hitInfo, maxDistance);
		}
		return true;
	}

	private void OnDestroy()
	{
		Object.Destroy(t1);
		Object.Destroy(t2);
		Object.Destroy(t3);
		Object.Destroy(t4);
	}

	public void setGTSBlood()
	{
		bloodMesh.localPosition = new Vector3(0f, bloodDepthAdjustment, 0f);
		t1.SetActive(true);
		t2.SetActive(true);
		t3.SetActive(true);
		t4.SetActive(true);
		t1.transform.parent = null;
		t2.transform.parent = null;
		t3.transform.parent = null;
		t4.transform.parent = null;
	}

	private void FixedUpdate()
	{
		childCheck++;
		if (childCheck > childCheckTreshold && (t1 != null || t2 != null || t3 != null || t4 != null))
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		if (GlobalPreferences.CrushStickDuration.value == 0f)
		{
			base.enabled = false;
		}
	}
}
