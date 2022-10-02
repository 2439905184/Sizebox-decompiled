using UnityEngine;

public class GiantessBone : MonoBehaviour
{
	public bool canCrush = true;

	public bool isGrabbing;

	public MeshCollider colliderFront;

	public MeshCollider colliderBack;

	public Giantess giantess;

	public Mesh mesh;

	public Rigidbody rigidBody;

	public GiantessBoneParenting boneParentingScript;

	public SkinnedMeshCollider boneMaster;

	public ColliderReshaper colliderReshaper;

	public int boneId;

	public float farthestExtent;

	public Vector3 localCenterOfMesh;

	private void Start()
	{
		rigidBody = base.gameObject.GetComponent<Rigidbody>();
		boneParentingScript = base.gameObject.AddComponent<GiantessBoneParenting>();
		boneParentingScript.boneId = boneId;
		boneParentingScript.boneMaster = boneMaster;
		boneParentingScript.colliderReshaper = colliderReshaper;
	}

	public void SetCollisionDetectionMode(CollisionDetectionMode mode)
	{
		rigidBody.collisionDetectionMode = mode;
	}

	public void Initialize(Giantess gts)
	{
		giantess = gts;
	}

	public void FindFarthestExtent()
	{
		farthestExtent = Vector3.Distance(colliderFront.bounds.center, colliderFront.bounds.max);
		localCenterOfMesh = base.transform.InverseTransformPoint(colliderFront.bounds.center);
	}
}
