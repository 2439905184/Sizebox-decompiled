using UnityEngine;

namespace RootMotion.Dynamics
{
	public struct MuscleCollision
	{
		public int muscleIndex;

		public Collision collision;

		public bool isStay;

		public MuscleCollision(int muscleIndex, Collision collision, bool isStay = false)
		{
			this.muscleIndex = muscleIndex;
			this.collision = collision;
			this.isStay = isStay;
		}
	}
}
