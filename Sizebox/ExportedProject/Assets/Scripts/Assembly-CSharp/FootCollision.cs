using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FootCollision : MonoBehaviour
{
	private Rigidbody _rigidBody;

	private Humanoid _humanoid;

	private static readonly WaitForFixedUpdate RoutineWait = new WaitForFixedUpdate();

	public Collider myCollider;

	private void Awake()
	{
		_humanoid = GetComponentInParent<Humanoid>();
		_rigidBody = GetComponent<Rigidbody>();
		_rigidBody.isKinematic = true;
		myCollider = base.gameObject.GetComponentInChildren<Collider>();
	}

	private IEnumerator CrushRoutine(ICrushable crush, Collision collision)
	{
		Vector3 currentPosition2 = base.transform.position;
		float time2 = Time.timeSinceLevelLoad;
		float timeout = time2 + Time.maximumDeltaTime;
		while (currentPosition2 == base.transform.position)
		{
			if (Time.timeSinceLevelLoad > timeout)
			{
				yield break;
			}
			yield return RoutineWait;
		}
		time2 = Time.timeSinceLevelLoad - time2;
		Vector3 vector = currentPosition2;
		currentPosition2 = base.transform.position;
		Vector3 velocity = (currentPosition2 - vector) / time2;
		float mass = _humanoid.Rigidbody.mass / 6f;
		crush.TryToCrush(mass, velocity, collision, _humanoid, myCollider);
	}

	private void OnCollisionEnter(Collision collision)
	{
		ICrushable componentInParent = collision.gameObject.GetComponentInParent<ICrushable>();
		if (componentInParent != null)
		{
			StartCoroutine(CrushRoutine(componentInParent, collision));
		}
	}
}
