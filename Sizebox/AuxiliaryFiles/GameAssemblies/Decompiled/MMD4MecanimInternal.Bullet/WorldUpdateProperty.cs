using UnityEngine;

namespace MMD4MecanimInternal.Bullet;

public class WorldUpdateProperty
{
	public float gravityScale = 10f;

	public float gravityNoise;

	public Vector3 gravityDirection = new Vector3(0f, -1f, 0f);
}
