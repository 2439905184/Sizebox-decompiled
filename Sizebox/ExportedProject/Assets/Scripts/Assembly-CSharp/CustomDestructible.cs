using UnityEngine;
using UnityEngine.Serialization;

public class CustomDestructible : MonoBehaviour
{
	public enum DestructibleType
	{
		Stationary = 0,
		Dynamic = 1
	}

	public GameObject destroyedPrefab;

	public float minimumDestructionHeight;

	public float destroyedTimeToLiveSeconds = 60f;

	[FormerlySerializedAs("rigidbody")]
	public Rigidbody myRigidbody;

	public DestructibleType destructibleType;
}
