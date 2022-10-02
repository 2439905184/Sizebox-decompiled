using UnityEngine;

public class DynamicBoneColliderBase : MonoBehaviour
{
	public enum Direction
	{
		X = 0,
		Y = 1,
		Z = 2
	}

	public enum Bound
	{
		Outside = 0,
		Inside = 1
	}

	public Direction m_Direction = Direction.Y;

	public Vector3 m_Center = Vector3.zero;

	public Bound m_Bound;

	public virtual void Collide(ref Vector3 particlePosition, float particleRadius)
	{
	}
}
